using LocalS.BLL;
using LocalS.BLL.Task;
using LocalS.Entity;
using LocalS.BLL.Mq;
using LocalS.BLL.Mq.MqByRedis;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TgPaySdk;
using MyAlipaySdk;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;

namespace LocalS.BLL.Biz
{
    public class OrderService : BaseDbContext
    {
        public StatusModel GetExStatus(bool isHasEx, bool isHandleComplete)
        {
            var statusModel = new StatusModel();

            if (isHasEx)
            {
                if (isHandleComplete)
                {
                    statusModel.Value = 0;
                    statusModel.Text = "异常，已处理";
                }
                else
                {
                    statusModel.Value = 2;
                    statusModel.Text = "异常，未处理";
                }
            }
            else
            {
                statusModel.Value = 0;
                statusModel.Text = "否";
            }
            //switch (status)
            //{
            //    case E_AdContentStatus.Normal:
            //        statusModel.Value = 1;
            //        statusModel.Text = "正常";
            //        break;
            //    case E_AdContentStatus.Deleted:
            //        statusModel.Value = 2;
            //        statusModel.Text = "已删除";
            //        break;
            //}


            return statusModel;
        }

        public bool GetCanHandleEx(bool isHappen, bool isHandle)
        {
            if (isHappen && isHandle == false)
                return true;

            return false;
        }

        public StatusModel GetStatus(E_OrderStatus orderStatus)
        {
            var status = new StatusModel();

            switch (orderStatus)
            {
                case E_OrderStatus.Submitted:
                    status.Value = 1000;
                    status.Text = "已提交";
                    break;
                case E_OrderStatus.WaitPay:
                    status.Value = 2000;
                    status.Text = "待支付";
                    break;
                case E_OrderStatus.Payed:
                    status.Value = 3000;
                    status.Text = "已支付";
                    break;
                case E_OrderStatus.Completed:
                    status.Value = 4000;
                    status.Text = "已完成";
                    break;
                case E_OrderStatus.Canceled:
                    status.Value = 5000;
                    status.Text = "已取消";
                    break;
            }
            return status;
        }

        public StatusModel GetPickupStatus(E_OrderPickupStatus pickupStatus)
        {
            var status = new StatusModel();

            switch (pickupStatus)
            {
                case E_OrderPickupStatus.Submitted:
                    status.Value = 1000;
                    status.Text = "已提交";
                    break;
                case E_OrderPickupStatus.WaitPay:
                    status.Value = 2000;
                    status.Text = "待支付";
                    break;
                //case E_OrderDetailsChildSonStatus.Payed:
                //    status.Value = 3000;
                //    status.Text = "已支付";
                //    break;
                case E_OrderPickupStatus.WaitPickup:
                    status.Value = 3010;
                    status.Text = "待取货";
                    break;
                case E_OrderPickupStatus.SendPickupCmd:
                    status.Value = 3011;
                    status.Text = "取货中";
                    break;
                case E_OrderPickupStatus.Pickuping:
                    status.Value = 3012;
                    status.Text = "取货中";
                    break;
                case E_OrderPickupStatus.Taked:
                    status.Value = 4000;
                    status.Text = "已完成";
                    break;
                case E_OrderPickupStatus.Canceled:
                    status.Value = 5000;
                    status.Text = "已取消";
                    break;
                case E_OrderPickupStatus.Exception:
                    status.Value = 6000;
                    status.Text = "异常未处理";
                    break;
                case E_OrderPickupStatus.ExPickupSignTaked:
                    status.Value = 6010;
                    status.Text = "异常已处理，标记为已取货";
                    break;
                case E_OrderPickupStatus.ExPickupSignUnTaked:
                    status.Value = 6011;
                    status.Text = "异常已处理，标记为未取货";
                    break;
            }
            return status;
        }

        public string GetSourceName(E_OrderSource orderSource)
        {
            string name = "";
            switch (orderSource)
            {
                case E_OrderSource.Api:
                    name = "开放接口";
                    break;
                case E_OrderSource.Wxmp:
                    name = "微信小程序";
                    break;
                case E_OrderSource.Machine:
                    name = "终端机器";
                    break;
            }
            return name;
        }

        public string GetPickImgUrl(string imgId)
        {
            if (string.IsNullOrEmpty(imgId))
                return null;
            return string.Format("http://file.17fanju.com/upload/pickup/{0}.jpg", imgId);
        }

        public Order GetOne(string id)
        {
            var order = CurrentDb.Order.Where(m => m.Id == id).FirstOrDefault();

            return order;
        }
        private static readonly object lock_Reserve = new object();
        public CustomJsonResult<RetOrderReserve> Reserve(string operater, RopOrderReserve rop)
        {
            CustomJsonResult<RetOrderReserve> result = new CustomJsonResult<RetOrderReserve>();

            lock (lock_Reserve)
            {
                if (rop.Blocks == null || rop.Blocks.Count == 0)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定商品为空", null);
                }

                var store = BizFactory.Store.GetOne(rop.StoreId);

                if (store == null)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定店铺无效", null);
                }

                if (!store.IsOpen)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "店铺已暂停营业", null);
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    RetOrderReserve ret = new RetOrderReserve();

                    List<BuildOrderSub.ProductSku> buildOrderSubSkus = new List<BuildOrderSub.ProductSku>();

                    #region 检查可售商品信息是否符合实际环境
                    List<string> warn_tips = new List<string>();

                    foreach (var block in rop.Blocks)
                    {
                        var productSkus = block.Skus;

                        foreach (var productSku in productSkus)
                        {
                            string[] sellChannelRefIds = new string[] { };

                            if (productSku.SellChannelRefIds == null || productSku.SellChannelRefIds.Length == 0)
                            {
                                if (productSku.ShopMode == E_SellChannelRefType.Mall)
                                {
                                    sellChannelRefIds = new string[] { SellChannelStock.MallSellChannelRefId };
                                }
                                else if (productSku.ShopMode == E_SellChannelRefType.Machine)
                                {
                                    sellChannelRefIds = store.SellMachineIds;
                                }
                            }
                            else
                            {
                                sellChannelRefIds = productSku.SellChannelRefIds;
                            }

                            var bizProductSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, rop.StoreId, sellChannelRefIds, productSku.Id);

                            if (bizProductSku == null)
                            {
                                warn_tips.Add(string.Format("{0}商品信息不存在", bizProductSku.Name));
                            }
                            else
                            {
                                if (bizProductSku.Stocks.Count == 0)
                                {
                                    warn_tips.Add(string.Format("{0}商品库存信息不存在", bizProductSku.Name));
                                }
                                else
                                {
                                    var sellQuantity = bizProductSku.Stocks.Sum(m => m.SellQuantity);

                                    Console.WriteLine("sellQuantity：" + sellQuantity);

                                    if (bizProductSku.Stocks[0].IsOffSell)
                                    {
                                        warn_tips.Add(string.Format("{0}已经下架", bizProductSku.Name));
                                    }
                                    else
                                    {
                                        if (sellQuantity < productSku.Quantity)
                                        {
                                            warn_tips.Add(string.Format("{0}的可销售数量为{1}个", bizProductSku.Name, sellQuantity));
                                        }
                                        else
                                        {
                                            var _skus = new BuildOrderSub.ProductSku();

                                            _skus.Id = productSku.Id;
                                            _skus.ProductId = bizProductSku.ProductId;
                                            _skus.Name = bizProductSku.Name;
                                            _skus.MainImgUrl = bizProductSku.MainImgUrl;
                                            _skus.BarCode = bizProductSku.BarCode;
                                            _skus.CumCode = bizProductSku.CumCode;
                                            _skus.SpecDes = bizProductSku.SpecDes;
                                            _skus.Producer = bizProductSku.Producer;
                                            _skus.Quantity = productSku.Quantity;
                                            _skus.ShopMode = productSku.ShopMode;
                                            _skus.Stocks = bizProductSku.Stocks;
                                            _skus.CartId = productSku.CartId;
                                            buildOrderSubSkus.Add(_skus);
                                        }
                                    }
                                }
                            }
                        }

                    }

                    if (warn_tips.Count > 0)
                    {
                        return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, string.Join("\n", warn_tips.ToArray()), null);
                    }

                    #endregion

                    LogUtil.Info("rop.bizProductSkus:" + buildOrderSubSkus.ToJsonString());
                    var buildOrderSubs = BuildOrderSubs(buildOrderSubSkus);
                    LogUtil.Info("SlotStock.buildOrderSubs:" + buildOrderSubs.ToJsonString());

                    var order = new Order();
                    order.Id = IdWorker.Build(IdType.OrderId);
                    order.MerchId = store.MerchId;
                    order.StoreId = rop.StoreId;
                    order.StoreName = store.Name;
                    order.ClientUserId = rop.ClientUserId;
                    order.ClientUserName = BizFactory.Merch.GetClientName(order.MerchId, rop.ClientUserId);
                    order.Quantity = buildOrderSubs.Sum(m => m.Quantity);
                    order.Status = E_OrderStatus.WaitPay;
                    order.PayStatus = E_OrderPayStatus.WaitPay;
                    order.Source = rop.Source;
                    order.IsTestMode = rop.IsTestMode;
                    order.AppId = rop.AppId;
                    order.SubmittedTime = DateTime.Now;
                    order.PayExpireTime = DateTime.Now.AddSeconds(300);
                    order.Creator = operater;
                    order.CreateTime = DateTime.Now;

                    #region 更改购物车标识

                    if (!string.IsNullOrEmpty(rop.ClientUserId))
                    {
                        var cartsIds = buildOrderSubSkus.Select(m => m.CartId).Distinct().ToArray();
                        if (cartsIds != null)
                        {
                            var clientCarts = CurrentDb.ClientCart.Where(m => cartsIds.Contains(m.Id) && m.ClientUserId == rop.ClientUserId).ToList();
                            if (clientCarts != null)
                            {
                                foreach (var cart in clientCarts)
                                {
                                    cart.Status = E_ClientCartStatus.Settling;
                                    cart.Mender = operater;
                                    cart.MendTime = DateTime.Now;
                                    CurrentDb.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion


                    LogUtil.Info("IsTestMode:" + rop.IsTestMode);

                    if (rop.IsTestMode)
                    {
                        order.OriginalAmount = buildOrderSubs.Sum(m => m.OriginalAmount);
                        order.DiscountAmount = buildOrderSubs.Sum(m => m.DiscountAmount);
                        order.ChargeAmount = 0.01m;
                    }
                    else
                    {
                        order.OriginalAmount = buildOrderSubs.Sum(m => m.OriginalAmount);
                        order.DiscountAmount = buildOrderSubs.Sum(m => m.DiscountAmount);
                        order.ChargeAmount = buildOrderSubs.Sum(m => m.ChargeAmount);
                    }


                    List<SellChannelRefModel> sellChannelRefModels = new List<Biz.SellChannelRefModel>();
                    foreach (var buildOrderSub in buildOrderSubs)
                    {
                        var orderSub = new OrderSub();
                        orderSub.Id = order.Id + buildOrderSubs.IndexOf(buildOrderSub).ToString();
                        orderSub.ClientUserId = rop.ClientUserId;
                        orderSub.MerchId = store.MerchId;
                        orderSub.StoreId = rop.StoreId;
                        orderSub.StoreName = store.Name;
                        orderSub.SellChannelRefType = buildOrderSub.SellChannelRefType;
                        orderSub.SellChannelRefId = buildOrderSub.SellChannelRefId;

                        switch (buildOrderSub.SellChannelRefType)
                        {
                            case E_SellChannelRefType.Machine:
                                var shopModeByMachine = rop.Blocks.Where(m => m.ShopMode == E_SellChannelRefType.Machine).FirstOrDefault();

                                if (shopModeByMachine == null || shopModeByMachine.SelfTake == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线下机器售卖模式自提地址为空", null);
                                }

                                if (shopModeByMachine.ReceiveMode != E_ReceiveMode.MachineSelfTake)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线下机器售卖模式请指定收货方式", null);
                                }

                                orderSub.SellChannelRefName = "机器自提";
                                orderSub.ReceiveMode = shopModeByMachine.ReceiveMode;
                                orderSub.Receiver = null;
                                orderSub.ReceiverPhoneNumber = null;
                                orderSub.ReceptionAddress = shopModeByMachine.SelfTake.StoreAddress;
                                orderSub.ReceptionMarkName = shopModeByMachine.SelfTake.StoreName;
                                break;
                            case E_SellChannelRefType.Mall:

                                var shopModeByMall = rop.Blocks.Where(m => m.ShopMode == E_SellChannelRefType.Mall).FirstOrDefault();
                                if (shopModeByMall == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式数据为空", null);
                                }
                                else if (shopModeByMall.ReceiveMode == E_ReceiveMode.Delivery && shopModeByMall.Delivery == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式配送地址为空", null);
                                }
                                else if (shopModeByMall.ReceiveMode == E_ReceiveMode.StoreSelfTake && shopModeByMall.SelfTake == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式自取地址为空", null);
                                }

                                if (shopModeByMall.ReceiveMode == E_ReceiveMode.Delivery)
                                {
                                    orderSub.SellChannelRefName = "配送到手";
                                    orderSub.ReceiveMode = E_ReceiveMode.Delivery;
                                    orderSub.Receiver = shopModeByMall.Delivery.Consignee;
                                    orderSub.ReceiverPhoneNumber = shopModeByMall.Delivery.PhoneNumber;
                                    orderSub.ReceptionAreaCode = shopModeByMall.Delivery.AreaCode;
                                    orderSub.ReceptionAreaName = shopModeByMall.Delivery.AreaName;
                                    orderSub.ReceptionAddress = shopModeByMall.Delivery.Address;
                                }
                                else if (shopModeByMall.ReceiveMode == E_ReceiveMode.StoreSelfTake)
                                {
                                    orderSub.SellChannelRefName = "到店自取";
                                    orderSub.ReceiveMode = E_ReceiveMode.StoreSelfTake;
                                    orderSub.Receiver = shopModeByMall.SelfTake.Consignee;
                                    orderSub.ReceiverPhoneNumber = shopModeByMall.SelfTake.PhoneNumber;
                                    orderSub.ReceptionAreaCode = shopModeByMall.SelfTake.AreaCode;
                                    orderSub.ReceptionAreaName = shopModeByMall.SelfTake.AreaName;
                                    orderSub.ReceptionAddress = shopModeByMall.SelfTake.StoreAddress;
                                    orderSub.ReceptionMarkName = shopModeByMall.SelfTake.StoreName;
                                }

                                break;
                        }

                        orderSub.OrderId = order.Id;
                        orderSub.OriginalAmount = buildOrderSub.OriginalAmount;
                        orderSub.DiscountAmount = buildOrderSub.DiscountAmount;
                        orderSub.ChargeAmount = buildOrderSub.ChargeAmount;
                        orderSub.Quantity = buildOrderSub.Quantity;
                        orderSub.PickupCode = IdWorker.BuildPickupCode();
                        if (orderSub.PickupCode == null)
                        {
                            return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定下单生成取货码失败", null);
                        }
                        orderSub.PayStatus = E_OrderPayStatus.WaitPay;
                        orderSub.Creator = operater;
                        orderSub.CreateTime = DateTime.Now;
                        CurrentDb.OrderSub.Add(orderSub);

                        sellChannelRefModels.Add(new SellChannelRefModel { Id = orderSub.SellChannelRefId, Type = orderSub.SellChannelRefType, Name = orderSub.SellChannelRefName });


                        foreach (var buildOrderSubChid in buildOrderSub.Childs)
                        {
                            var productSku = buildOrderSubSkus.Where(m => m.Id == buildOrderSubChid.ProductSkuId).FirstOrDefault();

                            var orderSubChild = new OrderSubChild();
                            orderSubChild.Id = orderSub.Id + buildOrderSub.Childs.IndexOf(buildOrderSubChid).ToString();
                            orderSubChild.ClientUserId = rop.ClientUserId;
                            orderSubChild.MerchId = store.MerchId;
                            orderSubChild.StoreId = rop.StoreId;
                            orderSubChild.StoreName = store.Name;
                            orderSubChild.SellChannelRefType = buildOrderSubChid.SellChannelRefType;
                            orderSubChild.SellChannelRefId = buildOrderSubChid.SellChannelRefId;
                            orderSubChild.SellChannelRefName = orderSub.SellChannelRefName;
                            orderSubChild.CabinetId = buildOrderSubChid.CabinetId;
                            orderSubChild.SlotId = buildOrderSubChid.SlotId;
                            orderSubChild.OrderId = order.Id;
                            orderSubChild.OrderSubId = orderSub.Id;
                            orderSubChild.PrdProductSkuId = buildOrderSubChid.ProductSkuId;
                            orderSubChild.PrdProductId = productSku.ProductId;
                            orderSubChild.PrdProductSkuName = productSku.Name;
                            orderSubChild.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                            orderSubChild.PrdProductSkuSpecDes = productSku.SpecDes.ToJsonString();
                            orderSubChild.PrdProductSkuProducer = productSku.Producer;
                            orderSubChild.PrdProductSkuBarCode = productSku.BarCode;
                            orderSubChild.PrdProductSkuCumCode = productSku.CumCode;
                            orderSubChild.SalePrice = buildOrderSubChid.SalePrice;
                            orderSubChild.SalePriceByVip = buildOrderSubChid.SalePriceByVip;
                            orderSubChild.Quantity = buildOrderSubChid.Quantity;
                            orderSubChild.OriginalAmount = buildOrderSubChid.OriginalAmount;
                            orderSubChild.DiscountAmount = buildOrderSubChid.DiscountAmount;
                            orderSubChild.ChargeAmount = buildOrderSubChid.ChargeAmount;
                            orderSubChild.PayStatus = E_OrderPayStatus.WaitPay;
                            orderSubChild.PickupStatus = E_OrderPickupStatus.WaitPay;

                            orderSubChild.Creator = operater;
                            orderSubChild.CreateTime = DateTime.Now;
                            CurrentDb.OrderSubChild.Add(orderSubChild);


                            LogUtil.Info("SlotStock:" + buildOrderSubChid.ToJsonString());

                            BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderReserveSuccess, rop.AppId, order.MerchId, order.StoreId, orderSubChild.SellChannelRefId, orderSubChild.CabinetId, orderSubChild.SlotId, orderSubChild.PrdProductSkuId, orderSubChild.Quantity);

                        }
                    }


                    order.SellChannelRefIds = string.Join(",", sellChannelRefModels.Select(m => m.Id).ToArray());
                    order.SellChannelRefNames = string.Join(",", sellChannelRefModels.Select(m => m.Name).ToArray());

                    CurrentDb.Order.Add(order);
                    CurrentDb.SaveChanges();
                    ts.Complete();

                    Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPay, order.Id, order.PayExpireTime.Value, new Order2CheckPayModel { Id = order.Id, MerchId = order.MerchId, PayCaller = order.PayCaller, PayPartner = order.PayPartner });

                    MqFactory.Global.PushEventNotify(operater, rop.AppId, order.MerchId, order.StoreId, "", EventCode.OrderReserveSuccess, string.Format("订单号：{0}，预定成功", order.Id));

                    ret.OrderId = order.Id;
                    ret.ChargeAmount = order.ChargeAmount.ToF2Price();

                    result = new CustomJsonResult<RetOrderReserve>(ResultType.Success, ResultCode.Success, "预定成功", ret);

                }
            }

            return result;

        }

        public List<BuildOrderSub> BuildOrderSubs(List<BuildOrderSub.ProductSku> reserveProductSkus)
        {
            List<BuildOrderSub> buildOrderSubs = new List<BuildOrderSub>();

            if (reserveProductSkus == null || reserveProductSkus.Count == 0)
                return buildOrderSubs;

            List<BuildOrderSub.Child> buildOrderSubChilds = new List<BuildOrderSub.Child>();

            var shopModes = reserveProductSkus.Select(m => m.ShopMode).Distinct().ToArray();

            foreach (var shopMode in shopModes)
            {

                var shopModeProductSkus = reserveProductSkus.Where(m => m.ShopMode == shopMode).ToList();

                foreach (var shopModeProductSku in shopModeProductSkus)
                {

                    var productSku_Stocks = shopModeProductSku.Stocks;


                    if (shopMode == E_SellChannelRefType.Mall)
                    {
                        var buildOrderSubChild = new BuildOrderSub.Child();
                        buildOrderSubChild.SellChannelRefType = productSku_Stocks[0].RefType;
                        buildOrderSubChild.SellChannelRefId = productSku_Stocks[0].RefId;
                        buildOrderSubChild.ProductSkuId = shopModeProductSku.Id;
                        buildOrderSubChild.CabinetId = productSku_Stocks[0].CabinetId;
                        buildOrderSubChild.SlotId = productSku_Stocks[0].SlotId;
                        buildOrderSubChild.Quantity = shopModeProductSku.Quantity;
                        buildOrderSubChild.SalePrice = productSku_Stocks[0].SalePrice;
                        buildOrderSubChild.SalePriceByVip = productSku_Stocks[0].SalePriceByVip;
                        buildOrderSubChild.OriginalAmount = buildOrderSubChild.Quantity * productSku_Stocks[0].SalePrice;
                        buildOrderSubChilds.Add(buildOrderSubChild);
                    }
                    else if (shopMode == E_SellChannelRefType.Machine)
                    {
                        foreach (var item in productSku_Stocks)
                        {
                            bool isFlag = false;
                            for (var i = 0; i < item.SellQuantity; i++)
                            {
                                int reservedQuantity = buildOrderSubChilds.Where(m => m.ProductSkuId == shopModeProductSku.Id && m.SellChannelRefType == shopMode).Sum(m => m.Quantity);//已订的数量
                                int needReserveQuantity = shopModeProductSku.Quantity;//需要订的数量
                                if (reservedQuantity != needReserveQuantity)
                                {
                                    var buildOrderSubChild = new BuildOrderSub.Child();
                                    buildOrderSubChild.SellChannelRefType = item.RefType;
                                    buildOrderSubChild.SellChannelRefId = item.RefId;
                                    buildOrderSubChild.ProductSkuId = shopModeProductSku.Id;
                                    buildOrderSubChild.CabinetId = item.CabinetId;
                                    buildOrderSubChild.SlotId = item.SlotId;
                                    buildOrderSubChild.Quantity = 1;
                                    buildOrderSubChild.SalePrice = productSku_Stocks[0].SalePrice;
                                    buildOrderSubChild.SalePriceByVip = productSku_Stocks[0].SalePriceByVip;
                                    buildOrderSubChild.OriginalAmount = buildOrderSubChild.Quantity * productSku_Stocks[0].SalePrice;
                                    buildOrderSubChilds.Add(buildOrderSubChild);
                                }
                                else
                                {
                                    isFlag = true;
                                    break;
                                }
                            }

                            if (isFlag)
                                break;
                        }

                    }

                }

            }

            var sumOriginalAmount = buildOrderSubChilds.Sum(m => m.OriginalAmount);
            var sumDiscountAmount = 0m;
            for (int i = 0; i < buildOrderSubChilds.Count; i++)
            {
                decimal scale = (sumOriginalAmount == 0 ? 0 : (buildOrderSubChilds[i].OriginalAmount / sumOriginalAmount));
                buildOrderSubChilds[i].DiscountAmount = Decimal.Round(scale * sumDiscountAmount, 2);
                buildOrderSubChilds[i].ChargeAmount = buildOrderSubChilds[i].OriginalAmount - buildOrderSubChilds[i].DiscountAmount;
            }

            var sumDiscountAmount2 = buildOrderSubChilds.Sum(m => m.DiscountAmount);
            if (sumDiscountAmount != sumDiscountAmount2)
            {
                var diff = sumDiscountAmount - sumDiscountAmount2;

                buildOrderSubChilds[buildOrderSubChilds.Count - 1].DiscountAmount = buildOrderSubChilds[buildOrderSubChilds.Count - 1].DiscountAmount + diff;
                buildOrderSubChilds[buildOrderSubChilds.Count - 1].ChargeAmount = buildOrderSubChilds[buildOrderSubChilds.Count - 1].OriginalAmount - buildOrderSubChilds[buildOrderSubChilds.Count - 1].DiscountAmount;
            }

            var l_OrderSubs = (from c in buildOrderSubChilds
                               select new
                               {
                                   c.SellChannelRefType,
                                   c.SellChannelRefId
                               }).Distinct().ToList();


            foreach (var orderSub in l_OrderSubs)
            {
                var buildOrderSub = new BuildOrderSub();
                buildOrderSub.SellChannelRefType = orderSub.SellChannelRefType;
                buildOrderSub.SellChannelRefId = orderSub.SellChannelRefId;
                buildOrderSub.Quantity = buildOrderSubChilds.Where(m => m.SellChannelRefId == orderSub.SellChannelRefId).Sum(m => m.Quantity);
                buildOrderSub.OriginalAmount = buildOrderSubChilds.Where(m => m.SellChannelRefId == orderSub.SellChannelRefId).Sum(m => m.OriginalAmount);
                buildOrderSub.DiscountAmount = buildOrderSubChilds.Where(m => m.SellChannelRefId == orderSub.SellChannelRefId).Sum(m => m.DiscountAmount);
                buildOrderSub.ChargeAmount = buildOrderSubChilds.Where(m => m.SellChannelRefId == orderSub.SellChannelRefId).Sum(m => m.ChargeAmount);
                buildOrderSub.Childs = buildOrderSubChilds.Where(m => m.SellChannelRefId == orderSub.SellChannelRefId).ToList();
                buildOrderSubs.Add(buildOrderSub);
            }


            return buildOrderSubs;
        }

        private static readonly object lock_PayResultNotify = new object();
        public CustomJsonResult PayResultNotify(string operater, E_OrderPayPartner payPartner, E_OrderNotifyLogNotifyFrom from, string content)
        {
            LogUtil.Info("PayResultNotify");
            lock (lock_PayResultNotify)
            {
                var payResult = new PayResult();

                if (payPartner == E_OrderPayPartner.Wx)
                {
                    #region 解释微信支付协议
                    LogUtil.Info("解释微信支付协议");

                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.Wx.Convert2PayResultByPayQuery(null, content);
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.Wx.Convert2PayResultByNotifyUrl(null, content);
                    }
                    #endregion
                }
                else if (payPartner == E_OrderPayPartner.Zfb)
                {
                    #region 解释支付宝支付协议
                    LogUtil.Info("解释支付宝支付协议");

                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.Zfb.Convert2PayResultByPayQuery(null, content);
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.Zfb.Convert2PayResultByNotifyUrl(null, content);
                    }

                    #endregion
                }
                else if (payPartner == E_OrderPayPartner.Tg)
                {
                    #region 解释通莞支付协议

                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.TgPay.Convert2PayResultByPayQuery(null, content);
                    }

                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.TgPay.Convert2PayResultByNotifyUrl(null, content);
                    }
                    #endregion
                }
                else if (payPartner == E_OrderPayPartner.Xrt)
                {
                    #region 解释XRT支付协议
                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.XrtPay.Convert2PayResultByPayQuery(null, content);
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.XrtPay.Convert2PayResultByNotifyUrl(null, content);
                    }
                    #endregion
                }

                if (payResult.IsPaySuccess)
                {
                    LogUtil.Info("解释支付协议结果，支付成功");
                    Dictionary<string, string> pms = new Dictionary<string, string>();
                    pms.Add("clientUserName", payResult.ClientUserName);

                    PaySuccess(operater, payResult.OrderId, payResult.PayPartnerOrderId, payResult.OrderPayWay, DateTime.Now, pms);
                }


                var mod_OrderNotifyLog = new OrderNotifyLog();
                mod_OrderNotifyLog.Id = IdWorker.Build(IdType.NewGuid);
                mod_OrderNotifyLog.OrderId = payResult.OrderId;
                mod_OrderNotifyLog.PayPartner = payPartner;
                mod_OrderNotifyLog.PayPartnerOrderId = payResult.PayPartnerOrderId;
                mod_OrderNotifyLog.NotifyContent = content;
                mod_OrderNotifyLog.NotifyFrom = from;
                mod_OrderNotifyLog.NotifyType = E_OrderNotifyLogNotifyType.Pay;
                mod_OrderNotifyLog.CreateTime = DateTime.Now;
                mod_OrderNotifyLog.Creator = operater;
                CurrentDb.OrderNotifyLog.Add(mod_OrderNotifyLog);
                CurrentDb.SaveChanges();


            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
        public CustomJsonResult PaySuccess(string operater, string orderId, string payPartnerOrderId, E_OrderPayWay payWay, DateTime completedTime, Dictionary<string, string> pms = null)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                LogUtil.Info("orderId:" + orderId);

                var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", orderId));
                }

                if (order.Status == E_OrderStatus.Payed || order.Status == E_OrderStatus.Completed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号({0})已经支付通知成功", orderId));
                }

                LogUtil.Info("进入PaySuccess修改订单,开始");

                //if (order.Status != E_OrderStatus.WaitPay)
                //{
                //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", orderId));
                //}

                order.PayWay = payWay;
                order.PayStatus = E_OrderPayStatus.PaySuccess;
                order.PayedTime = DateTime.Now;
                order.PayPartnerOrderId = payPartnerOrderId;

                if (order.Status == E_OrderStatus.WaitPay)
                {
                    order.Status = E_OrderStatus.Payed;

                    if (pms != null)
                    {
                        if (pms.ContainsKey("clientUserName"))
                        {
                            if (!string.IsNullOrEmpty(pms["clientUserName"]))
                            {
                                order.ClientUserName = pms["clientUserName"];
                            }
                        }
                    }

                    var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).ToList();
                    var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var orderSub in orderSubs)
                    {
                        orderSub.PayWay = payWay;
                        orderSub.PayStatus = E_OrderPayStatus.PaySuccess;
                        orderSub.PayedTime = DateTime.Now;

                        switch (orderSub.ReceiveMode)
                        {
                            case E_ReceiveMode.Delivery:
                                orderSub.PickupFlowLastDesc = "您提交了订单，等待发货";
                                orderSub.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.StoreSelfTake:
                                orderSub.PickupFlowLastDesc = string.Format("您提交了订单，请到店铺【{0}】,出示取货码【{1}】，给店员", orderSub.ReceptionMarkName, orderSub.PickupCode);
                                orderSub.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.MachineSelfTake:
                                orderSub.PickupFlowLastDesc = string.Format("您提交了订单，请到店铺【{0}】找到机器【{1}】,在取货界面输入取货码【{2}】", orderSub.ReceptionMarkName, orderSub.SellChannelRefId, orderSub.PickupCode);
                                orderSub.PickupFlowLastTime = DateTime.Now;
                                break;
                        }

                        orderSub.Mender = IdWorker.Build(IdType.EmptyGuid);
                        orderSub.MendTime = DateTime.Now;


                        var l_orderSubChilds = orderSubChilds.Where(m => m.OrderSubId == orderSub.Id).ToList();

                        foreach (var orderSubChild in l_orderSubChilds)
                        {

                            LogUtil.Info("进入PaySuccess修改订单,SkuId:" + orderSubChild.PrdProductSkuId + ",Quantity:" + orderSubChild.Quantity);

                            orderSubChild.PayWay = payWay;
                            orderSubChild.PayStatus = E_OrderPayStatus.PaySuccess;
                            orderSubChild.PayedTime = DateTime.Now;
                            orderSubChild.PickupStatus = E_OrderPickupStatus.WaitPickup;
                            orderSubChild.Mender = IdWorker.Build(IdType.EmptyGuid);
                            orderSubChild.MendTime = DateTime.Now;

                            BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPaySuccess, order.AppId, order.MerchId, order.StoreId, orderSubChild.SellChannelRefId, orderSubChild.CabinetId, orderSubChild.SlotId, orderSubChild.PrdProductSkuId, orderSubChild.Quantity);
                        }
                    }
                }

                order.MendTime = DateTime.Now;
                order.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                LogUtil.Info("进入PaySuccess修改订单,结束");

                Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPay, order.Id);
                MqFactory.Global.PushEventNotify(operater, order.AppId, order.MerchId, order.StoreId, "", EventCode.OrderPaySuccess, string.Format("订单号：{0}，支付成功", order.Id));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, string.Format("支付完成通知：订单号({0})通知成功", order.Id));
            }

            return result;
        }
        public CustomJsonResult<RetPayResultQuery> PayResultQuery(string operater, string orderId)
        {
            var result = new CustomJsonResult<RetPayResultQuery>();

            var order = BizFactory.Order.GetOne(orderId);

            if (order == null)
            {
                return new CustomJsonResult<RetPayResultQuery>(ResultType.Failure, ResultCode.Failure, "找不到订单", null);
            }

            var ret = new RetPayResultQuery();

            ret.OrderId = order.Id;
            ret.Status = order.Status;

            result = new CustomJsonResult<RetPayResultQuery>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
        public CustomJsonResult Cancle(string operater, string orderId, E_OrderCancleType cancleType, string cancelReason)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

                if (order == null)
                {
                    LogUtil.Info(string.Format("该订单号:{0},找不到", orderId));
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该订单号:{0},找不到", orderId));
                }

                if (order.Status == E_OrderStatus.Canceled)
                {
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经取消");
                }

                if (order.Status == E_OrderStatus.Payed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经支付成功");
                }

                if (order.Status == E_OrderStatus.Completed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经完成");
                }

                if (order.Status != E_OrderStatus.Payed && order.Status != E_OrderStatus.Completed)
                {
                    order.Status = E_OrderStatus.Canceled;
                    order.Mender = operater;
                    order.MendTime = DateTime.Now;
                    order.CancelOperator = operater;
                    order.CanceledTime = DateTime.Now;
                    order.CancelReason = cancelReason;

                    if (cancleType == E_OrderCancleType.PayCancle)
                    {
                        order.PayStatus = E_OrderPayStatus.PayCancle;
                    }
                    else if (cancleType == E_OrderCancleType.PayTimeout)
                    {
                        order.PayStatus = E_OrderPayStatus.PayTimeout;
                    }

                    var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).ToList();
                    var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var orderSub in orderSubs)
                    {
                        orderSub.Mender = operater;
                        orderSub.MendTime = DateTime.Now;

                        if (cancleType == E_OrderCancleType.PayCancle)
                        {
                            orderSub.PayStatus = E_OrderPayStatus.PayCancle;
                        }
                        else if (cancleType == E_OrderCancleType.PayTimeout)
                        {
                            orderSub.PayStatus = E_OrderPayStatus.PayTimeout;
                        }

                        var l_orderSubChilds = orderSubChilds.Where(m => m.OrderSubId == orderSub.Id).ToList();

                        foreach (var orderSubChild in l_orderSubChilds)
                        {
                            orderSubChild.Mender = operater;
                            orderSubChild.MendTime = DateTime.Now;

                            if (cancleType == E_OrderCancleType.PayCancle)
                            {
                                orderSubChild.PayStatus = E_OrderPayStatus.PayCancle;
                            }
                            else if (cancleType == E_OrderCancleType.PayTimeout)
                            {
                                orderSubChild.PayStatus = E_OrderPayStatus.PayTimeout;
                            }

                            orderSubChild.PickupStatus = E_OrderPickupStatus.Canceled;


                            BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderCancle, order.AppId, order.MerchId, order.StoreId, orderSubChild.SellChannelRefId, orderSubChild.CabinetId, orderSubChild.SlotId, orderSubChild.PrdProductSkuId, orderSubChild.Quantity);

                        }

                    }




                    CurrentDb.SaveChanges();
                    ts.Complete();

                    Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPay, order.Id);

                    MqFactory.Global.PushEventNotify(operater, order.AppId, order.MerchId, order.StoreId, "", EventCode.OrderCancle, string.Format("订单号：{0}，取消成功", order.Id));

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "已取消");
                }
            }

            return result;
        }
        public CustomJsonResult BuildPayParams(string operater, RopOrderBuildPayParams rop)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Id == rop.OrderId).FirstOrDefault();

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单数据");
                }

                if (order.Status == E_OrderStatus.Canceled)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已被取消，请重新下单");
                }


                order.PayCaller = rop.PayCaller;


                if (rop.Blocks != null)
                {
                    if (rop.Blocks.Count > 0)
                    {

                        var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == rop.OrderId).ToList();

                        foreach (var orderSub in orderSubs)
                        {
                            switch (orderSub.SellChannelRefType)
                            {
                                case E_SellChannelRefType.Mall:

                                    var shopModeByMall = rop.Blocks.Where(m => m.ShopMode == E_SellChannelRefType.Mall).FirstOrDefault();
                                    if (shopModeByMall == null)
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式数据为空");
                                    }
                                    else if (shopModeByMall.ReceiveMode == E_ReceiveMode.Delivery && shopModeByMall.Delivery == null)
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式配送地址为空");
                                    }
                                    else if (shopModeByMall.ReceiveMode == E_ReceiveMode.StoreSelfTake && shopModeByMall.SelfTake == null)
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式自取地址为空");
                                    }

                                    if (shopModeByMall.ReceiveMode == E_ReceiveMode.Delivery)
                                    {
                                        orderSub.SellChannelRefName = "配送到手";
                                        orderSub.ReceiveMode = E_ReceiveMode.Delivery;
                                        orderSub.Receiver = shopModeByMall.Delivery.Consignee;
                                        orderSub.ReceiverPhoneNumber = shopModeByMall.Delivery.PhoneNumber;
                                        orderSub.ReceptionAreaCode = shopModeByMall.Delivery.AreaCode;
                                        orderSub.ReceptionAreaName = shopModeByMall.Delivery.AreaName;
                                        orderSub.ReceptionAddress = shopModeByMall.Delivery.Address;
                                    }
                                    else if (shopModeByMall.ReceiveMode == E_ReceiveMode.StoreSelfTake)
                                    {
                                        orderSub.SellChannelRefName = "到店自取";
                                        orderSub.ReceiveMode = E_ReceiveMode.StoreSelfTake;
                                        orderSub.Receiver = shopModeByMall.SelfTake.Consignee;
                                        orderSub.ReceiverPhoneNumber = shopModeByMall.SelfTake.PhoneNumber;
                                        orderSub.ReceptionAreaCode = shopModeByMall.SelfTake.AreaCode;
                                        orderSub.ReceptionAreaName = shopModeByMall.SelfTake.AreaName;
                                        orderSub.ReceptionAddress = shopModeByMall.SelfTake.StoreAddress;
                                        orderSub.ReceptionMarkName = shopModeByMall.SelfTake.StoreName;
                                    }

                                    break;
                            }

                        }
                    }
                }




                var orderAttach = new BLL.Biz.OrderAttachModel();


                switch (rop.PayPartner)
                {
                    case E_OrderPayPartner.Wx:
                        #region  微信商户支付
                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.WxByNt:
                                #region WxByNt
                                order.PayPartner = E_OrderPayPartner.Wx;
                                order.PayWay = E_OrderPayWay.Wx;
                                order.PayStatus = E_OrderPayStatus.Paying;
                                var wxByNt_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);
                                var wx_PayBuildQrCode = SdkFactory.Wx.PayBuildQrCode(wxByNt_AppInfoConfig, E_OrderPayCaller.WxByNt, order.MerchId, order.StoreId, "", order.Id, order.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);
                                if (string.IsNullOrEmpty(wx_PayBuildQrCode.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var wxByNt_PayParams = new { PayUrl = wx_PayBuildQrCode.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wxByNt_PayParams);
                                #endregion
                                break;
                            case E_OrderPayCaller.WxByMp:
                                #region WxByMp
                                order.PayPartner = E_OrderPayPartner.Wx;
                                order.PayWay = E_OrderPayWay.Wx;
                                order.PayStatus = E_OrderPayStatus.Paying;
                                var wxByMp_UserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientUserId == order.ClientUserId).FirstOrDefault();

                                if (wxByMp_UserInfo == null)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户数据");
                                }

                                var wxByMp_AppInfoConfig = BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);

                                if (wxByMp_AppInfoConfig == null)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户信息认证失败");
                                }

                                orderAttach.MerchId = order.MerchId;
                                orderAttach.StoreId = order.StoreId;
                                orderAttach.PayCaller = rop.PayCaller;

                                var wxByMp_PayBuildWxJsPayInfo = SdkFactory.Wx.PayBuildWxJsPayInfo(wxByMp_AppInfoConfig, order.MerchId, order.StoreId, "", wxByMp_UserInfo.OpenId, order.Id, order.ChargeAmount, "", rop.CreateIp, "自助商品", order.PayExpireTime.Value);

                                if (string.IsNullOrEmpty(wxByMp_PayBuildWxJsPayInfo.Package))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付参数生成失败");
                                }

                                wxByMp_PayBuildWxJsPayInfo.OrderId = order.Id;

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wxByMp_PayBuildWxJsPayInfo);
                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);

                        }
                        #endregion 
                        break;
                    case E_OrderPayPartner.Zfb:
                        #region  支付宝商户支付
                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.ZfbByNt:
                                #region ZfbByNt
                                order.PayPartner = E_OrderPayPartner.Zfb;
                                order.PayWay = E_OrderPayWay.Zfb;
                                order.PayStatus = E_OrderPayStatus.Paying;
                                var zfbByNt_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetZfbMpAppInfoConfig(order.MerchId);
                                var zfbByNt_PayBuildQrCode = SdkFactory.Zfb.PayBuildQrCode(zfbByNt_AppInfoConfig, E_OrderPayCaller.ZfbByNt, order.MerchId, order.StoreId, "", order.Id, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);
                                if (string.IsNullOrEmpty(zfbByNt_PayBuildQrCode.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var zfbByNt_PayParams = new { PayUrl = zfbByNt_PayBuildQrCode.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", zfbByNt_PayParams);
                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                        }
                        #endregion
                        break;
                    case E_OrderPayPartner.Tg:
                        #region 通莞商户支付

                        var tgPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetTgPayInfoConfg(order.MerchId);

                        order.PayPartner = E_OrderPayPartner.Tg;
                        order.PayStatus = E_OrderPayStatus.Paying;

                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.AggregatePayByNt:
                                #region AggregatePayByNt

                                var tgPay_AllQrcodePay = SdkFactory.TgPay.PayBuildQrCode(tgPayInfoConfig, rop.PayCaller, order.MerchId, order.StoreId, "", order.Id, order.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);
                                if (string.IsNullOrEmpty(tgPay_AllQrcodePay.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var tg_AllQrcodePay_PayParams = new { PayUrl = tgPay_AllQrcodePay.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", tg_AllQrcodePay_PayParams);

                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                        }
                        #endregion 
                        break;
                    case E_OrderPayPartner.Xrt:
                        #region Xrt支付

                        // todo 发布去掉

                        var xrtPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetXrtPayInfoConfg(order.MerchId);

                        order.PayPartner = E_OrderPayPartner.Xrt;
                        order.PayStatus = E_OrderPayStatus.Paying;

                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.WxByNt:
                                #region WxByNt

                                var xrtPay_WxPayBuildByNtResult = SdkFactory.XrtPay.PayBuildQrCode(xrtPayInfoConfig, rop.PayCaller, order.MerchId, order.StoreId, "", order.Id, order.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);

                                if (string.IsNullOrEmpty(xrtPay_WxPayBuildByNtResult.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var xrtPay_WxPayBuildByNtResultParams = new { PayUrl = xrtPay_WxPayBuildByNtResult.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", xrtPay_WxPayBuildByNtResultParams);

                                #endregion
                                break;
                            case E_OrderPayCaller.WxByMp:
                                #region WxByMp

                                order.PayPartner = E_OrderPayPartner.Wx;
                                order.PayWay = E_OrderPayWay.Wx;
                                order.PayStatus = E_OrderPayStatus.Paying;
                                var wxByMp_UserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientUserId == order.ClientUserId).FirstOrDefault();

                                if (wxByMp_UserInfo == null)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户数据");
                                }

                                orderAttach.MerchId = order.MerchId;
                                orderAttach.StoreId = order.StoreId;
                                orderAttach.PayCaller = rop.PayCaller;

                                var wxByMp_PayBuildWxJsPayInfo = SdkFactory.XrtPay.PayBuildWxJsPayInfo(xrtPayInfoConfig, order.MerchId, order.StoreId, "", wxByMp_UserInfo.AppId, wxByMp_UserInfo.OpenId, order.Id, order.ChargeAmount, "", rop.CreateIp, "自助商品", order.PayExpireTime.Value);

                                if (string.IsNullOrEmpty(wxByMp_PayBuildWxJsPayInfo.Package))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付参数生成失败");
                                }

                                wxByMp_PayBuildWxJsPayInfo.OrderId = order.Id;


                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wxByMp_PayBuildWxJsPayInfo);

                                #endregion
                                break;
                            case E_OrderPayCaller.ZfbByNt:
                                #region ZfbByNt

                                var xrtPay_ZfbByNtBuildByNtResult = SdkFactory.XrtPay.PayBuildQrCode(xrtPayInfoConfig, rop.PayCaller, order.MerchId, order.StoreId, "", order.Id, order.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);

                                if (string.IsNullOrEmpty(xrtPay_ZfbByNtBuildByNtResult.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var xrtPay_ZfbByNtBuildByNtResultParams = new { PayUrl = xrtPay_ZfbByNtBuildByNtResult.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", xrtPay_ZfbByNtBuildByNtResultParams);

                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                        }
                        #endregion
                        break;
                    default:
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                }

                if (order.PayExpireTime == null)
                {
                    order.PayExpireTime = DateTime.Now.AddMinutes(5);
                }

                Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPay, order.Id, order.PayExpireTime.Value, new Order2CheckPayModel { Id = order.Id, MerchId = order.MerchId, PayCaller = order.PayCaller, PayPartner = order.PayPartner });

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }
        public CustomJsonResult BuildPayOptions(string operater, RupOrderBuildPayOptions rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderBuildPayOptions();

            ret.Title = "支付方式";

            switch (rup.AppCaller)
            {
                case E_AppCaller.Wxmp:
                    var merch = CurrentDb.Merch.Where(m => m.Id == rup.MerchId).FirstOrDefault();
                    if (merch != null)
                    {
                        var options = merch.WxmpAppPayOptions.ToJsonObject<List<PayOption>>();
                        if (options != null)
                        {
                            foreach (var option in options)
                            {
                                var my_option = new RetOrderBuildPayOptions.Option();
                                my_option.PayCaller = option.Caller;
                                my_option.PayPartner = option.Partner;
                                my_option.PaySupportWays = option.SupportWays;
                                switch (option.Caller)
                                {
                                    case E_OrderPayCaller.WxByMp:
                                        my_option.IsSelect = true;
                                        my_option.Title = "微信支付";
                                        my_option.Desc = "快捷支付";
                                        break;
                                }

                                ret.Options.Add(my_option);
                            }
                        }
                    }
                    break;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

            return result;
        }
        public List<OrderProductSkuByPickupModel> GetOrderProductSkuByPickup(string orderId, string machineId)
        {
            //todo 
            var models = new List<OrderProductSkuByPickupModel>();

            var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == orderId && m.SellChannelRefId == machineId).FirstOrDefault();
            var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == orderId && m.SellChannelRefId == machineId).ToList();

            LogUtil.Info("orderId:" + orderId);
            LogUtil.Info("machineId:" + machineId);
            LogUtil.Info("orderSubChilds.Count:" + orderSubChilds.Count);


            var productSkuIds = orderSubChilds.Select(m => m.PrdProductSkuId).Distinct().ToArray();

            foreach (var productSkuId in productSkuIds)
            {
                var orderSubChilds_Sku = orderSubChilds.Where(m => m.PrdProductSkuId == productSkuId).ToList();

                var model = new OrderProductSkuByPickupModel();
                model.Id = productSkuId;
                model.Name = orderSubChilds_Sku[0].PrdProductSkuName;
                model.MainImgUrl = orderSubChilds_Sku[0].PrdProductSkuMainImgUrl;
                model.Quantity = orderSubChilds_Sku.Sum(m => m.Quantity);
                model.QuantityBySuccess = orderSubChilds_Sku.Where(m => m.PickupStatus == E_OrderPickupStatus.Taked || m.PickupStatus == E_OrderPickupStatus.ExPickupSignTaked).Count();

                foreach (var orderSubChilds_SkuSlot in orderSubChilds_Sku)
                {
                    var slot = new OrderProductSkuByPickupModel.Slot();
                    slot.UniqueId = orderSubChilds_SkuSlot.Id;
                    slot.CabinetId = orderSubChilds_SkuSlot.CabinetId;
                    slot.SlotId = orderSubChilds_SkuSlot.SlotId;
                    slot.Status = orderSubChilds_SkuSlot.PickupStatus;

                    if (orderSubChilds_SkuSlot.PayStatus == E_OrderPayStatus.PaySuccess)
                    {
                        if (!orderSub.PickupIsTrg)
                        {
                            if (orderSubChilds_SkuSlot.PickupStatus == E_OrderPickupStatus.WaitPickup)
                            {
                                slot.IsAllowPickup = true;
                            }
                        }
                    }

                    model.Slots.Add(slot);
                }

                models.Add(model);
            }

            return models;
        }
        public string GetPayWayName(E_OrderPayWay payWay)
        {
            string str = "";
            switch (payWay)
            {
                case E_OrderPayWay.Wx:
                    str = "微信支付";
                    break;
                case E_OrderPayWay.Zfb:
                    str = "支付宝";
                    break;
                default:
                    str = "未知";
                    break;
            }

            return str;
        }

        public string BuildQrcode2PickupCode(string pickupCode)
        {
            string encode_qrcode = PickupCodeEncode(pickupCode);
            string buildqrcode = "pickupcode@v2:" + encode_qrcode;
            return buildqrcode;
        }

        public string DecodeQrcode2PickupCode(string buildqrcode)
        {
            if (buildqrcode.IndexOf("pickupcode@v2:") < 0)
                return null;

            string pickupCode = PickupCodeDecode(buildqrcode.Split(':')[1]);
            return pickupCode;
        }

        private const string PickupCode_KEY_64 = "VavicXbv";//注意了，是8个字符，64位
        private const string PickupCode_IV_64 = "VavicXbv";
        private string PickupCodeEncode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_IV_64);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }

        private string PickupCodeDecode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_IV_64);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

    }
}
