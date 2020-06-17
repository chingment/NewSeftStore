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
        public string GetOrderStatus(E_OrderStatus status)
        {
            string text = "";
            switch (status)
            {
                case E_OrderStatus.Submitted:
                    text = "已提交";
                    break;
                case E_OrderStatus.WaitPay:
                    text = "待支付";
                    break;
                case E_OrderStatus.Payed:
                    text = "已支付";
                    break;
                case E_OrderStatus.Completed:
                    text = "已完成";
                    break;
                case E_OrderStatus.Canceled:
                    text = "已取消";
                    break;
                default:
                    text = "";
                    break;
            }

            return text;
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
            bizRop.IsTestMode = store.IsTestMode;
            bizRop.Blocks = rop.Blocks;

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(operater, bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                RetOrderReserve ret = new RetOrderReserve();
                ret.OrderId = bizResult.Data.OrderId;
                ret.ChargeAmount = bizResult.Data.ChargeAmount;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
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
            var subtotalItem = new List<OrderConfirmSubtotalItemModel>();
            var skus = new List<OrderConfirmProductSkuModel>();

            decimal skuAmountByActual = 0;//实际总价
            decimal skuAmountByOriginal = 0;//原总价
            decimal skuAmountByMemebr = 0;//普通用户总价
            decimal skuAmountByVip = 0;//会员总价

            StoreInfoModel store;
            DeliveryModel dliveryModel = new DeliveryModel();
            E_ReceiveMode receiveMode_Mall = E_ReceiveMode.Delivery;
            if (string.IsNullOrEmpty(rop.OrderId))
            {

                if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品为空");
                }


                store = BizFactory.Store.GetOne(rop.StoreId);

                foreach (var item in rop.ProductSkus)
                {
                    var productSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.Id, store.GetSellChannelRefIds(item.ShopMode), item.Id);
                    if (productSku != null)
                    {
                        item.Name = productSku.Name;
                        item.MainImgUrl = productSku.MainImgUrl;

                        if (productSku.Stocks.Count > 0)
                        {
                            item.SalePrice = productSku.Stocks[0].SalePrice;
                            skuAmountByOriginal += (productSku.Stocks[0].SalePrice * item.Quantity);
                            skuAmountByMemebr += (productSku.Stocks[0].SalePrice * item.Quantity);
                            skuAmountByVip += (productSku.Stocks[0].SalePriceByVip * item.Quantity);
                            skus.Add(item);
                        }
                    }
                    else
                    {
                        LogUtil.Info("商品Id ：" + item.Id + ",信息为空");
                    }
                }

                var shippingAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == clientUserId && m.IsDefault == true).FirstOrDefault();
                if (shippingAddress == null)
                {
                    dliveryModel.Id = "";
                    dliveryModel.Consignee = "快寄地址";
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
                var order = BizFactory.Order.GetOne(rop.OrderId);

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单");
                }

                if (order.Status != E_OrderStatus.WaitPay)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不在就绪支付状态");
                }

                store = BizFactory.Store.GetOne(order.StoreId);

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == rop.OrderId).ToList();

                foreach (var item in orderSubChilds)
                {
                    var orderConfirmSkuModel = new OrderConfirmProductSkuModel();
                    orderConfirmSkuModel.Id = item.PrdProductSkuId;
                    orderConfirmSkuModel.Name = item.PrdProductSkuName;
                    orderConfirmSkuModel.MainImgUrl = item.PrdProductSkuMainImgUrl;
                    orderConfirmSkuModel.Quantity = item.Quantity;
                    orderConfirmSkuModel.SalePrice = item.SalePrice;
                    orderConfirmSkuModel.ShopMode = item.SellChannelRefType;
                    skuAmountByOriginal += (item.SalePrice * item.Quantity);
                    skuAmountByMemebr += (item.SalePrice * item.Quantity);
                    skus.Add(orderConfirmSkuModel);
                }

                var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == rop.OrderId).ToList();


                var orderSub_Mall = orderSub.Where(m => m.SellChannelRefType == E_SellChannelRefType.Mall).FirstOrDefault();
                if (orderSub_Mall != null)
                {
                    receiveMode_Mall = orderSub_Mall.ReceiveMode;

                    dliveryModel.Id = "";
                    dliveryModel.Consignee = orderSub_Mall.Receiver;
                    dliveryModel.PhoneNumber = orderSub_Mall.ReceiverPhoneNumber;
                    dliveryModel.AreaCode = orderSub_Mall.ReceptionAreaCode;
                    dliveryModel.AreaName = orderSub_Mall.ReceptionAreaName;
                    dliveryModel.Address = orderSub_Mall.ReceptionAddress;
                    dliveryModel.IsDefault = false;
                }

            }


            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            bool isVip = false;

            if (clientUser != null)
            {
                if (clientUser.IsVip)
                {
                    isVip = true;
                }
            }


            if (isVip)
            {
                skuAmountByActual = skuAmountByVip;//会员用户总价 为 实际总价

                subtotalItem.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "会员优惠", Amount = string.Format("-{0}", (skuAmountByOriginal - skuAmountByVip).ToF2Price()), IsDcrease = true });
            }
            else
            {
                skuAmountByActual = skuAmountByMemebr;
            }


            var orderBlock = new List<OrderBlockModel>();

            var skus_Mall = skus.Where(m => m.ShopMode == E_SellChannelRefType.Mall).ToList();
            if (skus_Mall.Count > 0)
            {
                var orderBlock_Mall = new OrderBlockModel();
                orderBlock_Mall.TagName = "线上商城";
                orderBlock_Mall.Skus = skus_Mall;
                orderBlock_Mall.TabMode = E_TabMode.DeliveryAndStoreSelfTake;
                orderBlock_Mall.ShopMode = E_SellChannelRefType.Mall;
                orderBlock_Mall.ReceiveMode = receiveMode_Mall;
                orderBlock_Mall.Delivery = dliveryModel;
                orderBlock_Mall.SelfTake.StoreName = store.Name;
                orderBlock_Mall.SelfTake.StoreAddress = store.Address;
                orderBlock.Add(orderBlock_Mall);
            }


            var skus_Machine = skus.Where(m => m.ShopMode == E_SellChannelRefType.Machine).ToList();
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

            ret.Blocks = orderBlock;

            #region 暂时不开通 Coupon

            //if (rop.CouponId == null || rop.CouponId.Count == 0)
            //{
            //    var couponsCount = CurrentDb.ClientCoupon.Where(m => m.ClientId == pClientId && m.Status == Entity.Enumeration.CouponStatus.WaitUse && m.EndTime > DateTime.Now).Count();

            //    if (couponsCount == 0)
            //    {
            //        ret.Coupon = new OrderConfirmCouponModel { TipMsg = "暂无可用优惠卷", TipType = TipType.NoCanUse };
            //    }
            //    else
            //    {
            //        ret.Coupon = new OrderConfirmCouponModel { TipMsg = string.Format("{0}个可用", couponsCount), TipType = TipType.CanUse };
            //    }
            //}
            //else
            //{

            //    var coupons = CurrentDb.ClientCoupon.Where(m => m.ClientId == pClientId && rop.CouponId.Contains(m.Id)).ToList();

            //    foreach (var item in coupons)
            //    {
            //        var amount = 0m;
            //        switch (item.Type)
            //        {
            //            case Enumeration.CouponType.FullCut:
            //            case Enumeration.CouponType.UnLimitedCut:
            //                if (skuAmountByActual >= item.LimitAmount)
            //                {
            //                    amount = -item.Discount;
            //                    skuAmountByActual = skuAmountByActual - item.Discount;

            //                    //subtotalItem.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = item.Name, Amount = string.Format("{0}", amount.ToF2Price()), IsDcrease = true });

            //                    ret.Coupon = new OrderConfirmCouponModel { TipMsg = string.Format("{0}", amount.ToF2Price()), TipType = TipType.InUse };

            //                }

            //                break;
            //            case Enumeration.CouponType.Discount:

            //                amount = skuAmountByActual - (skuAmountByActual * (item.Discount / 10));

            //                skuAmountByActual = skuAmountByActual * (item.Discount / 10);

            //                // subtotalItem.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = item.Name, Amount = string.Format("{0}", amount.ToF2Price()), IsDcrease = true });
            //                ret.Coupon = new OrderConfirmCouponModel { TipMsg = string.Format("{0}", amount.ToF2Price()), TipType = TipType.InUse };
            //                break;
            //        }
            //    }

            //}

            #endregion

            //subtotalItem.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "满5减3元", Amount = "-9", IsDcrease = true });
            //subtotalItem.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "优惠卷", Amount = "-10", IsDcrease = true });

            ret.SubtotalItems = subtotalItem;

            ret.ActualAmount = skuAmountByActual.ToF2Price();

            ret.OriginalAmount = skuAmountByOriginal.ToF2Price();

            ret.OrderId = rop.OrderId;

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
                         where o.ClientUserId == clientUserId
                         select new { o.Id, o.StoreId, o.StoreName, o.Status, o.SubmittedTime, o.ExIsHappen, o.CompletedTime, o.ChargeAmount, o.CanceledTime }
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


                //model.StatusName = item.Status.GetCnName();
                model.ChargeAmount = item.ChargeAmount.ToF2Price();


                var orderSubs = CurrentDb.OrderSub.Where(c => c.OrderId == item.Id).ToList();

                foreach (var orderSub in orderSubs)
                {
                    var block = new FsBlock();

                    block.Tag.Name = new FsText(orderSub.SellChannelRefName, "");


                    if (item.Status == E_OrderStatus.Payed)
                    {
                        if (orderSub.SellChannelRefType == E_SellChannelRefType.Machine)
                        {
                            block.Tag.Desc = new FsField("取货码", "", orderSub.PickupCode, "#f18d00");
                            block.Qrcode = new FsQrcode { Code = orderSub.PickupCode, Url = "http://file.17fanju.com/Upload/product/a055a033-d6c4-4fb1-b5a7-155579d1179b_O.jpg" };

                        }
                    }

                    var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == orderSub.Id).ToList();

                    foreach (var orderSubChild in orderSubChilds)
                    {

                        var field = new FsTemplateData();

                        field.Type = "SkuTmp";

                        var sku = new FsTemplateData.TmplOrderSku();

                        sku.Id = orderSubChild.Id;
                        sku.Name = orderSubChild.PrdProductSkuName;
                        sku.MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl;
                        sku.Quantity = orderSubChild.Quantity.ToString();
                        sku.ChargeAmount = orderSubChild.ChargeAmount.ToF2Price();


                        field.Value = sku;

                        block.Data.Add(field);
                    }


                    model.Blocks.Add(block);
                }

                switch (item.Status)
                {
                    case E_OrderStatus.WaitPay:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "取消订单", Color = "red" }, OpType = "FUN", OpVal = "cancleOrder" });
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "继续支付", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.Status) });
                        break;
                    case E_OrderStatus.Payed:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.Status) });
                        break;
                    case E_OrderStatus.Completed:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.Status) });
                        break;
                    case E_OrderStatus.Canceled:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id, item.Status) });
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
            ret.Tag.Desc = new FsField("", "", GetOrderStatus(order.Status), "");

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


            var orderSubs = CurrentDb.OrderSub.Where(c => c.OrderId == order.Id).ToList();


            foreach (var orderSub in orderSubs)
            {
                var block = new FsBlock();

                block.Tag.Name = new FsText(orderSub.SellChannelRefName, "");

                if (order.Status == E_OrderStatus.Payed)
                {
                    if (orderSub.SellChannelRefType == E_SellChannelRefType.Machine)
                    {
                        block.Tag.Desc = new FsField("取货码", "", orderSub.PickupCode, "#f18d00");
                        block.Qrcode = new FsQrcode { Code = BizFactory.Order.BuildQrcode2PickupCode(orderSub.PickupCode), Remark = "扫码取货", Url = "" };
                    }
                }

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == orderSub.Id).ToList();

                foreach (var orderSubChild in orderSubChilds)
                {

                    var data = new FsTemplateData();

                    data.Type = "SkuTmp";

                    var sku = new FsTemplateData.TmplOrderSku();

                    sku.Id = orderSubChild.Id;
                    sku.Name = orderSubChild.PrdProductSkuName;
                    sku.MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl;
                    sku.Quantity = orderSubChild.Quantity.ToString();
                    sku.ChargeAmount = orderSubChild.ChargeAmount.ToF2Price();


                    data.Value = sku;

                    block.Data.Add(data);
                }


                ret.Blocks.Add(block);
            }




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
            bizRop.OrderId = rop.OrderId;
            bizRop.PayCaller = rop.PayCaller;
            bizRop.PayPartner = rop.PayPartner;
            bizRop.CreateIp = rop.CreateIp;
            bizRop.Blocks = rop.Blocks;
            return BLL.Biz.BizFactory.Order.BuildPayParams(operater, bizRop);
        }

    }
}