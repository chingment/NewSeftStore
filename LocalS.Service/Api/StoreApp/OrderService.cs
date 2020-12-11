using LocalS.BLL;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MyWeiXinSdk;
using LocalS.BLL.Biz;

namespace LocalS.Service.Api.StoreApp
{

    public class OrderService : BaseDbContext
    {
        private List<FsBlock> GetOrderBlocks(Order order)
        {
            var fsBlocks = new List<FsBlock>();


            var block = new FsBlock();

            block.UniqueId = order.Id;
            block.UniqueType = E_UniqueType.Order;

            block.Tag.Name = new FsText(order.ReceiveModeName, "");


            if (order.PayStatus == E_PayStatus.PaySuccess)
            {
                if (order.SellChannelRefType == E_SellChannelRefType.Machine)
                {
                    block.Tag.Desc = new FsField("取货码", "", order.PickupCode, "#f18d00");
                    block.Qrcode = new FsQrcode { Code = BizFactory.Order.BuildQrcode2PickupCode(order.PickupCode), Url = "", Remark = string.Format("扫码枪扫一扫", order.SellChannelRefId) };
                }

                if (order.SellChannelRefType == E_SellChannelRefType.Machine || order.SellChannelRefType == E_SellChannelRefType.Mall)
                {
                    block.ReceiptInfo = new FsReceiptInfo { LastTime = order.PickupFlowLastTime.ToUnifiedFormatDateTime(), Description = order.PickupFlowLastDesc };
                }
            }

            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).ToList();

            if (order.PayStatus == E_PayStatus.PaySuccess)
            {
                foreach (var orderSub in orderSubs)
                {
                    var field = new FsTemplateData();
                    field.Type = "SkuTmp";
                    var sku = new FsTemplateData.TmplOrderSku();
                    sku.UniqueId = orderSub.Id;
                    sku.UniqueType = E_UniqueType.OrderSub;
                    sku.Id = orderSub.PrdProductSkuId;
                    sku.Name = orderSub.PrdProductSkuName;
                    sku.MainImgUrl = orderSub.PrdProductSkuMainImgUrl;
                    sku.Quantity = orderSub.Quantity.ToString();
                    sku.ChargeAmount = orderSub.ChargeAmount.ToF2Price();
                    sku.StatusName = "";
                    field.Value = sku;

                    block.Data.Add(field);
                }
            }
            else
            {
                var productSkuIds = orderSubs.Select(m => m.PrdProductSkuId).Distinct().ToArray();

                foreach (var productSkuId in productSkuIds)
                {
                    var orderSubChilds_Sku = orderSubs.Where(m => m.PrdProductSkuId == productSkuId).ToList();

                    var field = new FsTemplateData();

                    field.Type = "SkuTmp";

                    var sku = new FsTemplateData.TmplOrderSku();
                    sku.Id = orderSubChilds_Sku[0].PrdProductSkuId;
                    sku.UniqueId = order.Id;
                    sku.UniqueType = E_UniqueType.Order;
                    sku.Name = orderSubChilds_Sku[0].PrdProductSkuName;
                    sku.MainImgUrl = orderSubChilds_Sku[0].PrdProductSkuMainImgUrl;
                    sku.Quantity = orderSubChilds_Sku.Sum(m => m.Quantity).ToString();
                    sku.ChargeAmount = orderSubChilds_Sku.Sum(m => m.ChargeAmount).ToF2Price();
                    sku.StatusName = "";
                    field.Value = sku;
                    block.Data.Add(field);
                }
            }




            fsBlocks.Add(block);


            return fsBlocks;
        }

        public CustomJsonResult Reserve(string operater, string clientUserId, RopOrderReserve rop)
        {

            var store = BizFactory.Store.GetOne(rop.StoreId);

            CustomJsonResult result = new CustomJsonResult();
            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.AppId = AppId.WXMINPRAGROM;
            bizRop.Source = rop.Source;
            bizRop.StoreId = rop.StoreId;
            bizRop.ClientUserId = clientUserId;
            bizRop.SaleOutletId = rop.SaleOutletId;
            bizRop.IsTestMode = store.IsTestMode;
            bizRop.Blocks = rop.Blocks;

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(operater, bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", bizResult.Data);
            }
            else
            {
                result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, bizResult.Message);
            }

            return result;
        }

        public CustomJsonResult Confrim(string operater, string clientUserId, RopOrderConfirm rop)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderConfirm();

            var c_subtotalItems = new List<OrderConfirmSubtotalItemModel>();
            var c_prodcutSkus = new List<OrderConfirmProductSkuModel>();

            decimal amount_actual = 0;//实际总价
            decimal amount_original = 0;//原总价
            decimal amount_memebr = 0;//普通用户总价
            decimal amount_vip = 0;//会员总价

            StoreInfoModel store;
            DeliveryModel dliveryModel = new DeliveryModel();
            E_ReceiveMode receiveMode_Mall = E_ReceiveMode.Delivery;
            BookTimeModel bookTimeModel = new BookTimeModel();
            if (rop.OrderIds == null || rop.OrderIds.Count == 0)
            {

                if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品为空");
                }

                store = BizFactory.Store.GetOne(rop.StoreId);

                foreach (var productSku in rop.ProductSkus)
                {
                    if (productSku.ShopMode == E_SellChannelRefType.Machine || productSku.ShopMode == E_SellChannelRefType.Mall)
                    {
                        var r_productSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.StoreId, store.GetSellChannelRefIds(productSku.ShopMode), productSku.Id);
                        if (r_productSku != null)
                        {
                            productSku.Name = r_productSku.Name;
                            productSku.MainImgUrl = r_productSku.MainImgUrl;

                            if (r_productSku.Stocks.Count > 0)
                            {
                                productSku.SalePrice = r_productSku.Stocks[0].SalePrice;
                                productSku.SalePriceByVip = r_productSku.Stocks[0].SalePriceByVip;

                                c_prodcutSkus.Add(productSku);

                                amount_original += (productSku.SalePrice * productSku.Quantity);
                                amount_memebr += (productSku.SalePrice * productSku.Quantity);
                                amount_vip += (productSku.SalePriceByVip * productSku.Quantity);
                            }
                        }
                    }
                    else if (productSku.ShopMode == E_SellChannelRefType.MemberFee)
                    {
                        var memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == store.MerchId && m.Id == productSku.Id).FirstOrDefault();
                        if (memberFeeSt != null)
                        {
                            productSku.Name = memberFeeSt.Name;
                            productSku.MainImgUrl = memberFeeSt.MainImgUrl;
                            productSku.SalePrice = memberFeeSt.FeeValue;

                            c_prodcutSkus.Add(productSku);

                            amount_original += (productSku.SalePrice * productSku.Quantity);
                            amount_memebr += (productSku.SalePrice * productSku.Quantity);
                            amount_vip += (productSku.SalePrice * productSku.Quantity);
                        }
                    }
                    else
                    {
                        LogUtil.Info("商品Id ：" + productSku.Id + ",信息为空");
                    }
                }

                var shippingAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == clientUserId && m.IsDefault == true).FirstOrDefault();
                if (shippingAddress == null)
                {
                    dliveryModel.Id = "";
                    dliveryModel.Consignee = "配送地址";
                    dliveryModel.PhoneNumber = "选择";
                    dliveryModel.AreaName = "";
                    dliveryModel.Address = "";
                    dliveryModel.IsDefault = false;
                }
                else
                {
                    dliveryModel.Id = shippingAddress.Id;
                    dliveryModel.Consignee = shippingAddress.Consignee;
                    dliveryModel.PhoneNumber = shippingAddress.PhoneNumber;
                    dliveryModel.AreaName = shippingAddress.AreaName;
                    dliveryModel.Address = shippingAddress.Address;
                    dliveryModel.IsDefault = shippingAddress.IsDefault;
                }

            }
            else
            {

                store = BizFactory.Store.GetOne(rop.StoreId);

                var orders = CurrentDb.Order.Where(m => rop.OrderIds.Contains(m.Id)).ToList();

                var orderByMall = orders.Where(m => m.SellChannelRefType == E_SellChannelRefType.Mall).FirstOrDefault();
                if (orderByMall != null)
                {
                    receiveMode_Mall = orderByMall.ReceiveMode;

                    dliveryModel.Id = "";

                    if (string.IsNullOrEmpty(orderByMall.Receiver))
                    {
                        var clientDeliveryAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == orderByMall.ClientUserId && m.IsDelete == false).OrderByDescending(m => m.IsDefault).FirstOrDefault();
                        if (clientDeliveryAddress != null)
                        {
                            dliveryModel.Id = clientDeliveryAddress.Id;
                            dliveryModel.Consignee = clientDeliveryAddress.Consignee;
                            dliveryModel.PhoneNumber = clientDeliveryAddress.PhoneNumber;
                            dliveryModel.AreaCode = clientDeliveryAddress.AreaCode;
                            dliveryModel.AreaName = clientDeliveryAddress.AreaName;
                            dliveryModel.Address = clientDeliveryAddress.Address;
                            dliveryModel.IsDefault = clientDeliveryAddress.IsDefault;
                        }
                    }
                    else
                    {
                        dliveryModel.Consignee = orderByMall.Receiver;
                        dliveryModel.PhoneNumber = orderByMall.ReceiverPhoneNumber;
                        dliveryModel.AreaCode = orderByMall.ReceptionAreaCode;
                        dliveryModel.AreaName = orderByMall.ReceptionAreaName;
                        dliveryModel.Address = orderByMall.ReceptionAddress;
                        dliveryModel.IsDefault = false;
                    }
                }

                var orderSubs = CurrentDb.OrderSub.Where(m => rop.OrderIds.Contains(m.OrderId)).ToList();

                var shopModeSkus = (from c in orderSubs
                                    select new
                                    {
                                        c.SellChannelRefType,
                                        c.PrdProductSkuId
                                    }).Distinct().ToList();


                foreach (var shopModeSku in shopModeSkus)
                {
                    var l_orderSubChilds = orderSubs.Where(m => m.PrdProductSkuId == shopModeSku.PrdProductSkuId && m.SellChannelRefType == shopModeSku.SellChannelRefType).ToList();

                    var c_prodcutSku = new OrderConfirmProductSkuModel();
                    c_prodcutSku.Id = l_orderSubChilds[0].PrdProductSkuId;
                    c_prodcutSku.Name = l_orderSubChilds[0].PrdProductSkuName;
                    c_prodcutSku.MainImgUrl = l_orderSubChilds[0].PrdProductSkuMainImgUrl;
                    c_prodcutSku.Quantity = l_orderSubChilds.Sum(m => m.Quantity);
                    c_prodcutSku.SalePrice = l_orderSubChilds[0].SalePrice;
                    c_prodcutSku.ShopMode = shopModeSku.SellChannelRefType;
                    amount_original += (l_orderSubChilds[0].SalePrice * c_prodcutSku.Quantity);
                    amount_memebr += (l_orderSubChilds[0].SalePriceByVip * c_prodcutSku.Quantity);
                    c_prodcutSkus.Add(c_prodcutSku);
                }
            }


            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            int memberLevel = 0;

            if (clientUser != null)
            {
                memberLevel = clientUser.MemberLevel;
            }


            if (memberLevel > 0)
            {
                amount_actual = amount_vip;//会员用户总价 为 实际总价

                c_subtotalItems.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "会员优惠", Amount = string.Format("-{0}", (amount_original - amount_vip).ToF2Price()), IsDcrease = true });
            }
            else
            {
                amount_actual = amount_memebr;
            }


            var orderBlock = new List<OrderBlockModel>();

            var skus_Mall = c_prodcutSkus.Where(m => m.ShopMode == E_SellChannelRefType.Mall).ToList();
            if (skus_Mall.Count > 0)
            {
                var orderBlock_Mall = new OrderBlockModel();
                orderBlock_Mall.TagName = "线上商城";
                orderBlock_Mall.Skus = skus_Mall;

                if (store.SctMode.Contains("T1"))
                {
                    orderBlock_Mall.TabMode = E_TabMode.Delivery;
                    orderBlock_Mall.ReceiveMode = E_ReceiveMode.Delivery;
                }
                else if (store.SctMode.Contains("T2"))
                {
                    orderBlock_Mall.TabMode = E_TabMode.StoreSelfTake;
                    orderBlock_Mall.ReceiveMode = E_ReceiveMode.StoreSelfTake;
                }
                else if (store.SctMode.Contains("T3"))
                {
                    orderBlock_Mall.TabMode = E_TabMode.DeliveryAndStoreSelfTake;
                    orderBlock_Mall.ReceiveMode = receiveMode_Mall;
                }
                else
                {
                    orderBlock_Mall.TabMode = E_TabMode.Delivery;
                    orderBlock_Mall.ReceiveMode = E_ReceiveMode.Delivery;
                }

                orderBlock_Mall.ShopMode = E_SellChannelRefType.Mall;
                orderBlock_Mall.Delivery = dliveryModel;
                orderBlock_Mall.BookTime = bookTimeModel;
                orderBlock_Mall.SelfTake.StoreName = store.Name;
                orderBlock_Mall.SelfTake.StoreAddress = store.Address;
                orderBlock.Add(orderBlock_Mall);
            }


            var skus_Machine = c_prodcutSkus.Where(m => m.ShopMode == E_SellChannelRefType.Machine).ToList();
            if (skus_Machine.Count > 0)
            {
                var orderBlock_Machine = new OrderBlockModel();
                orderBlock_Machine.TagName = "线下机器";
                orderBlock_Machine.Skus = skus_Machine;
                orderBlock_Machine.TabMode = E_TabMode.MachineSelfTake;
                orderBlock_Machine.ShopMode = E_SellChannelRefType.Machine;
                orderBlock_Machine.ReceiveMode = E_ReceiveMode.MachineSelfTake;
                orderBlock_Machine.SelfTake.StoreName = store.Name;
                orderBlock_Machine.SelfTake.StoreAddress = store.Address;
                orderBlock.Add(orderBlock_Machine);
            }

            var skus_MemberFee = c_prodcutSkus.Where(m => m.ShopMode == E_SellChannelRefType.MemberFee).ToList();

            if (skus_MemberFee.Count > 0)
            {
                var orderBlock_MemberFee = new OrderBlockModel();
                orderBlock_MemberFee.TagName = "会员费";
                orderBlock_MemberFee.Skus = skus_MemberFee;
                orderBlock_MemberFee.TabMode = E_TabMode.MemerbFee;
                orderBlock_MemberFee.ShopMode = E_SellChannelRefType.MemberFee;
                orderBlock_MemberFee.ReceiveMode = E_ReceiveMode.MemberFee;
                orderBlock_MemberFee.SelfTake.StoreName = store.Name;
                orderBlock_MemberFee.SelfTake.StoreAddress = store.Address;
                orderBlock.Add(orderBlock_MemberFee);
            }

            ret.Blocks = orderBlock;

            #region 暂时不开通 Coupon

            if (rop.CouponIds == null || rop.CouponIds.Count == 0)
            {
                var couponsCount = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, rop.StoreId, rop.ProductSkus, clientUserId);

                if (couponsCount == 0)
                {
                    ret.Coupon = new OrderConfirmCouponModel { TipMsg = "暂无可用优惠卷", TipType = TipType.NoCanUse };
                }
                else
                {
                    ret.Coupon = new OrderConfirmCouponModel { TipMsg = string.Format("{0}个可用", couponsCount), TipType = TipType.CanUse };
                }
            }
            else
            {
                decimal couponAmount = 0;

                var coupons = (from u in CurrentDb.ClientCoupon
                               join m in CurrentDb.Coupon on u.CouponId equals m.Id into temp
                               from tt in temp.DefaultIfEmpty()
                               where u.ClientUserId == clientUserId && rop.CouponIds.Contains(u.Id)
                               select new { u.Id, u.ClientUserId, u.MerchId, tt.Name, tt.UseAreaType, tt.UseAreaValue, u.Status, u.ValidEndTime, u.ValidStartTime, tt.FaceType, tt.FaceValue, tt.AtLeastAmount }).ToList();

                foreach (var item in coupons)
                {
                    switch(item.FaceType)
                    {
                        case E_Coupon_FaceType.ShopVoucher:
                        case E_Coupon_FaceType.DepositVoucher:
                        case E_Coupon_FaceType.RentVoucher:
                            couponAmount += item.FaceValue;
                            break;
                    }
                }

                ret.Coupon = new OrderConfirmCouponModel { TipMsg = string.Format("-{0}", couponAmount.ToF2Price()), TipType = TipType.InUse };

            }

            #endregion



            //c_subtotalItems.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "优惠卷", Amount = "-10", IsDcrease = true });
            //subtotalItem.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "满5减3元", Amount = "-9", IsDcrease = true });

            ret.SubtotalItems = c_subtotalItems;

            ret.ActualAmount = amount_actual.ToF2Price();

            ret.OriginalAmount = amount_original.ToF2Price();

            ret.OrderIds = rop.OrderIds;

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
        }

        public CustomJsonResult BuildPayOptions(string operater, string clientUserId, RupOrderBuildPayOptions rup)
        {
            LocalS.BLL.Biz.RupOrderBuildPayOptions bizRup = new LocalS.BLL.Biz.RupOrderBuildPayOptions();
            bizRup.AppCaller = rup.AppCaller;
            bizRup.AppId = rup.AppId;
            bizRup.MerchId = rup.MerchId;
            bizRup.ClientUserId = clientUserId;
            return BLL.Biz.BizFactory.Order.BuildPayOptions(operater, bizRup);
        }

        public CustomJsonResult List(string operater, string clientUserId, RupOrderList rup)
        {
            var result = new CustomJsonResult();

            var pageEntiy = new PageEntity<OrderModel>();


            var query = (from o in CurrentDb.Order
                         where (o.ClientUserId == clientUserId && o.IsNoDisplayClient == false)
                         select new
                         {
                             Id = o.Id,
                             StoreId = o.StoreId,
                             StoreName = o.StoreName,
                             Status = o.Status,
                             PayStatus = o.PayStatus,
                             SubmittedTime = o.SubmittedTime,
                             CompletedTime = o.CompletedTime,
                             ChargeAmount = o.ChargeAmount,
                             CanceledTime = o.CanceledTime,
                             ReceiveModeName = o.ReceiveModeName,
                             SellChannelRefType = o.SellChannelRefType,
                             SellChannelRefId = o.SellChannelRefId,
                             PickupFlowLastTime = o.PickupFlowLastTime,
                             PickupFlowLastDesc = o.PickupFlowLastDesc,
                             PickupCode = o.PickupCode
                         }
             );


            if (rup.Status != E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.Status);
            }


            int pageSize = 10;

            pageEntiy.PageIndex = rup.PageIndex;
            pageEntiy.PageSize = pageSize;
            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.SubmittedTime).Skip(pageSize * (rup.PageIndex)).Take(pageSize);



            var list = query.ToList();

            List<OrderModel> models = new List<OrderModel>();

            foreach (var item in list)
            {
                var model = new OrderModel();

                model.Id = item.Id;
                model.Tag.Name = new FsText(item.StoreName, "");
                model.Tag.Desc = new FsField(BizFactory.Order.GetStatus(item.Status).Text, "");
                model.ChargeAmount = item.ChargeAmount.ToF2Price();


                var fsBlocks = new List<FsBlock>();


                var block = new FsBlock();

                block.UniqueId = item.Id;
                block.UniqueType = E_UniqueType.Order;

                block.Tag.Name = new FsText(item.ReceiveModeName, "");


                if (item.PayStatus == E_PayStatus.PaySuccess)
                {
                    if (item.SellChannelRefType == E_SellChannelRefType.Machine)
                    {
                        block.Tag.Desc = new FsField("取货码", "", item.PickupCode, "#f18d00");
                        block.Qrcode = new FsQrcode { Code = BizFactory.Order.BuildQrcode2PickupCode(item.PickupCode), Url = "", Remark = string.Format("扫码枪扫一扫", item.SellChannelRefId) };
                    }

                    if (item.SellChannelRefType == E_SellChannelRefType.Machine || item.SellChannelRefType == E_SellChannelRefType.Mall)
                    {
                        block.ReceiptInfo = new FsReceiptInfo { LastTime = item.PickupFlowLastTime.ToUnifiedFormatDateTime(), Description = item.PickupFlowLastDesc };

                    }
                }

                var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == item.Id).ToList();

                if (item.PayStatus == E_PayStatus.PaySuccess)
                {
                    foreach (var orderSub in orderSubs)
                    {
                        var field = new FsTemplateData();
                        field.Type = "SkuTmp";
                        var sku = new FsTemplateData.TmplOrderSku();
                        sku.UniqueId = orderSub.Id;
                        sku.UniqueType = E_UniqueType.OrderSub;
                        sku.Id = orderSub.PrdProductSkuId;
                        sku.Name = orderSub.PrdProductSkuName;
                        sku.MainImgUrl = orderSub.PrdProductSkuMainImgUrl;
                        sku.Quantity = orderSub.Quantity.ToString();
                        sku.ChargeAmount = orderSub.ChargeAmount.ToF2Price();
                        sku.StatusName = "";
                        field.Value = sku;

                        block.Data.Add(field);
                    }
                }
                else
                {
                    var productSkuIds = orderSubs.Select(m => m.PrdProductSkuId).Distinct().ToArray();

                    foreach (var productSkuId in productSkuIds)
                    {
                        var orderSubChilds_Sku = orderSubs.Where(m => m.PrdProductSkuId == productSkuId).ToList();

                        var field = new FsTemplateData();

                        field.Type = "SkuTmp";

                        var sku = new FsTemplateData.TmplOrderSku();
                        sku.Id = orderSubChilds_Sku[0].PrdProductSkuId;
                        sku.UniqueId = item.Id;
                        sku.UniqueType = E_UniqueType.Order;
                        sku.Name = orderSubChilds_Sku[0].PrdProductSkuName;
                        sku.MainImgUrl = orderSubChilds_Sku[0].PrdProductSkuMainImgUrl;
                        sku.Quantity = orderSubChilds_Sku.Sum(m => m.Quantity).ToString();
                        sku.ChargeAmount = orderSubChilds_Sku.Sum(m => m.ChargeAmount).ToF2Price();
                        sku.StatusName = "";
                        field.Value = sku;
                        block.Data.Add(field);
                    }
                }



                fsBlocks.Add(block);

                model.Blocks = fsBlocks;

                switch (item.Status)
                {
                    case E_OrderStatus.WaitPay:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "取消订单", Color = "red" }, OpType = "FUN", OpVal = "cancleOrder" });
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "继续支付", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.PayStatus) });
                        break;
                    case E_OrderStatus.Payed:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.PayStatus) });
                        break;
                    case E_OrderStatus.Completed:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.PayStatus) });
                        break;
                    case E_OrderStatus.Canceled:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.PayStatus) });
                        break;
                }

                pageEntiy.Items.Add(model);

            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", pageEntiy);

            return result;
        }

        public CustomJsonResult Details(string operater, string clientUserId, string orderId)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderDetails();

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

            ret.Id = order.Id;
            ret.Status = order.Status;
            ret.Tag.Name = new FsText(order.StoreName, "");
            ret.Tag.Desc = new FsField(BizFactory.Order.GetStatus(order.Status).Text, "");

            var fsBlockByField = new FsBlockByField();

            fsBlockByField.Tag.Name = new FsText("订单信息", "");

            fsBlockByField.Data.Add(new FsField("订单编号", "", order.Id, ""));
            fsBlockByField.Data.Add(new FsField("创建时间", "", order.SubmittedTime.ToUnifiedFormatDateTime(), ""));
            if (order.PayedTime != null)
            {
                fsBlockByField.Data.Add(new FsField("付款时间", "", order.PayedTime.ToUnifiedFormatDateTime(), ""));
            }
            if (order.CompletedTime != null)
            {
                fsBlockByField.Data.Add(new FsField("完成时间", "", order.CompletedTime.ToUnifiedFormatDateTime(), ""));
            }

            if (order.CanceledTime != null)
            {
                fsBlockByField.Data.Add(new FsField("取消时间", "", order.CanceledTime.ToUnifiedFormatDateTime(), ""));
                fsBlockByField.Data.Add(new FsField("取消原因", "", order.CancelReason, ""));
            }

            ret.FieldBlocks.Add(fsBlockByField);


            ret.Blocks = GetOrderBlocks(order);


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

        }

        public CustomJsonResult Cancle(string operater, string clientUserId, RopOrderCancle rop)
        {
            return BLL.Biz.BizFactory.Order.Cancle(operater, rop.Id, E_OrderCancleType.PayCancle, "用户取消");
        }

        public CustomJsonResult BuildPayParams(string operater, string clientUserId, RopOrderBuildPayParams rop)
        {
            LocalS.BLL.Biz.RopOrderBuildPayParams bizRop = new LocalS.BLL.Biz.RopOrderBuildPayParams();
            bizRop.AppId = rop.AppId;
            bizRop.OrderIds = rop.OrderIds;
            bizRop.PayCaller = rop.PayCaller;
            bizRop.PayPartner = rop.PayPartner;
            bizRop.CreateIp = rop.CreateIp;
            bizRop.Blocks = rop.Blocks;
            return BLL.Biz.BizFactory.Order.BuildPayParams(operater, bizRop);
        }

        public CustomJsonResult ReceiptTimeAxis(string operater, string clientUserId, RupOrderReceiptTimeAxis rup)
        {
            var result = new CustomJsonResult();


            var ret = new RetOrderReceiptTimeAxis();

            Order order = null;
            List<OrderPickupLog> orderPickupLogs = null;
            switch (rup.UniqueType)
            {
                case E_UniqueType.Order:
                    order = CurrentDb.Order.Where(m => m.Id == rup.UniqueId).FirstOrDefault();
                    orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.OrderId == rup.UniqueId).OrderByDescending(m => m.CreateTime).ToList();
                    break;
            }


            if (order != null)
            {
                var merch = CurrentDb.Merch.Where(m => m.Id == order.MerchId).FirstOrDefault();

                switch (order.ReceiveMode)
                {
                    case E_ReceiveMode.Delivery:

                        ret.Top = null;
                        ret.RecordTop.CircleText = "收";
                        ret.RecordTop.Description = order.ReceptionAddress;
                        break;
                    case E_ReceiveMode.StoreSelfTake:
                        ret.Top.CircleText = "自";
                        ret.Top.Field1 = order.ReceptionMarkName;
                        ret.Top.Field2 = order.ReceptionAddress;
                        ret.Top.Field3 = string.Format("客服热线 {0}", merch.CsrPhoneNumber);
                        ret.RecordTop.CircleText = "自";
                        ret.RecordTop.Description = order.ReceptionAddress;
                        break;
                    case E_ReceiveMode.MachineSelfTake:
                        ret.Top.CircleText = "提";
                        ret.Top.Field1 = order.ReceptionMarkName;
                        ret.Top.Field2 = order.ReceptionAddress;
                        ret.Top.Field3 = string.Format("客服热线 {0}", merch.CsrPhoneNumber);
                        ret.RecordTop.CircleText = "提";
                        ret.RecordTop.Description = order.ReceptionMarkName;
                        break;
                }

                for (var i = 0; i < orderPickupLogs.Count; i++)
                {
                    var record = new RetOrderReceiptTimeAxis.RecordModel();
                    string time1 = orderPickupLogs[i].CreateTime.ToString("MM-dd");
                    DateTime dateNow = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime createTime = DateTime.Parse(orderPickupLogs[i].CreateTime.ToString("yyyy-MM-dd"));
                    if (dateNow == createTime)
                    {
                        time1 = "今天";
                    }
                    else if ((dateNow - createTime).TotalDays == 1)
                    {
                        time1 = "昨天";
                    }
                    else if ((dateNow - createTime).TotalDays == 2)
                    {
                        time1 = "前天";
                    }
                    record.Time1 = time1;
                    record.Time2 = orderPickupLogs[i].CreateTime.ToString("HH:mm");
                    record.Description = orderPickupLogs[i].ActionRemark.NullToEmpty();
                    record.Status = orderPickupLogs[i].ActionStatusName.NullToEmpty();
                    if (i == 0)
                    {
                        record.IsLastest = true;
                    }
                    else
                    {
                        record.IsLastest = false;
                    }

                    ret.Records.Add(record);
                }
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult BuildBookTimeArea(string operater, string clientUserId, RupOrderBuildBookTimeArea rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderBuildBookTimeArea();

            DateTime startTime = DateTime.Now;
            //DateTime endTime=

            for (int i = 0; i < 7; i++)
            {
                DateTime dateTime = DateTime.Now.AddDays(i);

                var dateArea = new BookTimeDateAreaModel();

                if ((dateTime - startTime).TotalDays == 0)
                {
                    dateArea.Week = "今天";
                }
                else if ((dateTime - startTime).TotalDays == 1)
                {
                    dateArea.Week = "明天";
                }
                else if ((dateTime - startTime).TotalDays == 2)
                {
                    dateArea.Week = "后天";
                }
                else
                {
                    dateArea.Week = Lumos.CommonUtil.GetCnWeekDayName(dateTime);
                }

                dateArea.Date = dateTime.ToUnifiedFormatDate();
                dateArea.Value = dateTime.ToUnifiedFormatDate();
                dateArea.Status = 1;
                dateArea.Tip = "";

                dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "8:00", Tip = "", Status = 1, Value = "8:00", Type = 1 });

                dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "8:30", Tip = "", Status = 1, Value = "8:30", Type = 1 });

                dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "9:00", Tip = "", Status = 1, Value = "9:00", Type = 1 });

                dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "9:30", Tip = "", Status = 1, Value = "9:30", Type = 1 });

                dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "10:00", Tip = "", Status = 1, Value = "10:00", Type = 1 });

                dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "10:30", Tip = "", Status = 1, Value = "10:30", Type = 1 });

                dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "11:30", Tip = "", Status = 1, Value = "11:30", Type = 1 });


                ret.DateArea.Add(dateArea);

            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

    }
}