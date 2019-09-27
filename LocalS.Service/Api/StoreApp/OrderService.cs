using LocalS.BLL;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WeiXinSdk;

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
                case E_OrderStatus.Cancled:
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
            CustomJsonResult result = new CustomJsonResult();
            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.Source = rop.Source;
            bizRop.StoreId = rop.StoreId;
            bizRop.ReserveMode = E_ReserveMode.Online;
            bizRop.ClientUserId = clientUserId;
            bizRop.PayWay = rop.PayWay;
            bizRop.PayCaller = rop.PayCaller;

            foreach (var productSku in rop.ProductSkus)
            {
                bizRop.ProductSkus.Add(new LocalS.BLL.Biz.RopOrderReserve.ProductSku() { CartId = productSku.CartId, Id = productSku.Id, Quantity = productSku.Quantity, ReceptionMode = productSku.ReceptionMode });
            }
    
            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(operater, bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                RetOrderReserve ret = new RetOrderReserve();
                ret.Id = bizResult.Data.Id;
                ret.Sn = bizResult.Data.Sn;
                ret.PayUrl = bizResult.Data.PayUrl;
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


            if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品为空");
            }

            var ret = new RetOrderConfirm();
            var subtotalItem = new List<OrderConfirmSubtotalItemModel>();
            var skus = new List<OrderConfirmProductSkuModel>();

            decimal skuAmountByActual = 0;//实际总价
            decimal skuAmountByOriginal = 0;//原总价
            decimal skuAmountByMemebr = 0;//普通用户总价
            decimal skuAmountByVip = 0;//会员总价
            Store store;

            if (string.IsNullOrEmpty(rop.OrderId))
            {
                store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();

                foreach (var item in rop.ProductSkus)
                {
                    var productSku = BLL.Biz.BizFactory.PrdProduct.GetProductSku(item.Id);
                    if (productSku != null)
                    {
                        item.Name = productSku.Name;
                        item.MainImgUrl = productSku.MainImgUrl;
                        item.SalePrice = productSku.SalePrice;

                        skuAmountByOriginal += (productSku.SalePrice * item.Quantity);
                        skuAmountByMemebr += (productSku.SalePrice * item.Quantity);
                        skuAmountByVip += (productSku.SalePriceByVip * item.Quantity);

                        skus.Add(item);
                    }
                    else
                    {
                        LogUtil.Info("商品Id ：" + item.Id + ",信息为空");
                    }
                }

            }
            else
            {
                var order = CurrentDb.Order.Where(m => m.Id == rop.OrderId).FirstOrDefault();

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单");
                }

                if (order.Status != E_OrderStatus.WaitPay)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不在就绪支付状态");
                }


                var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == rop.OrderId).ToList();

                var storeId = orderDetailsChilds[0].StoreId;

                store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

                foreach (var item in orderDetailsChilds)
                {
                    var orderConfirmSkuModel = new OrderConfirmProductSkuModel();
                    orderConfirmSkuModel.Id = item.PrdProductSkuId;
                    orderConfirmSkuModel.Name = item.PrdProductSkuName;
                    orderConfirmSkuModel.MainImgUrl = item.PrdProductSkuMainImgUrl;
                    orderConfirmSkuModel.Quantity = item.Quantity;
                    orderConfirmSkuModel.SalePrice = item.SalePrice;
                    skuAmountByOriginal += (item.SalePrice * item.Quantity);
                    skuAmountByMemebr += (item.SalePrice * item.Quantity);
                    skus.Add(orderConfirmSkuModel);
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

            var skus_Express = skus.Where(m => m.ReceptionMode == E_ReceptionMode.Express).ToList();
            if (skus_Express.Count > 0)
            {
                var orderBlock_Express = new OrderBlockModel();
                orderBlock_Express.TagName = "快递商品";
                orderBlock_Express.Skus = skus_Express;
                orderBlock_Express.ReceptionMode = E_ReceptionMode.Express;
                var shippingAddressModel = new DeliveryAddressModel();
                shippingAddressModel.CanSelectElse = true;
                var shippingAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == clientUserId && m.IsDefault == true).FirstOrDefault();
                if (shippingAddress == null)
                {
                    shippingAddressModel.Id = "";
                    shippingAddressModel.Consignee = "快寄地址";
                    shippingAddressModel.PhoneNumber = "选择";
                    shippingAddressModel.AreaName = "";
                    shippingAddressModel.Address = "";
                    shippingAddressModel.IsDefault = false;
                    shippingAddressModel.DefaultText = "";
                }
                else
                {
                    shippingAddressModel.Id = shippingAddress.Id;
                    shippingAddressModel.Consignee = shippingAddress.Consignee;
                    shippingAddressModel.PhoneNumber = shippingAddress.PhoneNumber;
                    shippingAddressModel.AreaName = shippingAddress.AreaName;
                    shippingAddressModel.Address = shippingAddress.Address;
                    shippingAddressModel.IsDefault = shippingAddress.IsDefault;
                    shippingAddressModel.DefaultText = shippingAddress.IsDefault == true ? "默认" : "";
                }
                orderBlock_Express.DeliveryAddress = shippingAddressModel;
                orderBlock.Add(orderBlock_Express);
            }

            var skus_SelfTake = skus.Where(m => m.ReceptionMode == E_ReceptionMode.SelfTake).ToList();
            if (skus_SelfTake.Count > 0)
            {
                var orderBlock_SelfTake = new OrderBlockModel();
                orderBlock_SelfTake.TagName = "店内自取";
                orderBlock_SelfTake.Skus = skus_SelfTake;
                orderBlock_SelfTake.ReceptionMode = E_ReceptionMode.SelfTake;
                var shippingAddressModel = new DeliveryAddressModel();
                shippingAddressModel.Id = null;
                shippingAddressModel.Consignee = "店铺名称";
                shippingAddressModel.IsDefault = true;
                shippingAddressModel.DefaultText = "店内自取";
                shippingAddressModel.PhoneNumber = store.Name;
                shippingAddressModel.AreaName = "";
                shippingAddressModel.Address = store.Address;
                shippingAddressModel.CanSelectElse = false;

                orderBlock_SelfTake.DeliveryAddress = shippingAddressModel;
                orderBlock.Add(orderBlock_SelfTake);
            }

            var skus_Machine = skus.Where(m => m.ReceptionMode == E_ReceptionMode.Machine).ToList();
            if (skus_Machine.Count > 0)
            {
                var orderBlock_Machine = new OrderBlockModel();
                orderBlock_Machine.TagName = "自提商品";
                orderBlock_Machine.Skus = skus_Machine;
                orderBlock_Machine.ReceptionMode = E_ReceptionMode.Machine;
                var shippingAddressModel = new DeliveryAddressModel();
                shippingAddressModel.Id = null;
                shippingAddressModel.Consignee = "店铺名称";
                shippingAddressModel.IsDefault = true;
                shippingAddressModel.DefaultText = "机器自取";
                shippingAddressModel.PhoneNumber = store.Name;
                shippingAddressModel.AreaName = "";
                shippingAddressModel.Address = store.Address;
                shippingAddressModel.CanSelectElse = false;

                orderBlock_Machine.DeliveryAddress = shippingAddressModel;

                orderBlock.Add(orderBlock_Machine);
            }

            ret.Block = orderBlock;

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

            ret.SubtotalItem = subtotalItem;


            ret.ActualAmount = skuAmountByActual.ToF2Price();

            ret.OriginalAmount = skuAmountByOriginal.ToF2Price();

            ret.OrderId = rop.OrderId;
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
        }

        public CustomJsonResult List(string operater, string clientUserId, RupOrderList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.Order
                         where o.ClientUserId == clientUserId
                         select new { o.Id, o.Sn, o.StoreId, o.PickCode, o.StoreName, o.Status, o.SubmitTime, o.CompletedTime, o.ChargeAmount, o.CancledTime }
             );


            if (rup.Status != E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.Status);
            }

            int pageSize = 10;

            query = query.OrderByDescending(r => r.SubmitTime).Skip(pageSize * (rup.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<OrderModel> models = new List<OrderModel>();

            foreach (var item in list)
            {
                var model = new OrderModel();

                model.Id = item.Id;
                model.Sn = item.Sn;
                model.Tag.Name = new FsText(item.StoreName, "");


                //model.StatusName = item.Status.GetCnName();
                model.ChargeAmount = item.ChargeAmount.ToF2Price();


                var orderDetails = CurrentDb.OrderDetails.Where(c => c.OrderId == item.Id).ToList();

                foreach (var orderDetail in orderDetails)
                {
                    var block = new FsBlock();

                    block.Tag.Name = new FsText(orderDetail.SellChannelRefName, "");


                    if (item.Status == E_OrderStatus.Payed)
                    {
                        if (orderDetail.SellChannelRefType == E_SellChannelRefType.Machine)
                        {
                            block.Tag.Desc = new FsField("取货码", "", item.PickCode, "#f18d00");
                        }
                    }

                    var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderDetailsId == orderDetail.Id).ToList();

                    foreach (var orderDetailsChild in orderDetailsChilds)
                    {

                        var field = new FsTemplateData();

                        field.Type = "SkuTmp";

                        var sku = new FsTemplateData.TmplOrderSku();

                        sku.Id = orderDetailsChild.Id;
                        sku.Name = orderDetailsChild.PrdProductSkuName;
                        sku.MainImgUrl = orderDetailsChild.PrdProductSkuMainImgUrl;
                        sku.Quantity = orderDetailsChild.Quantity.ToString();
                        sku.ChargeAmount = orderDetailsChild.ChargeAmount.ToF2Price();


                        field.Value = sku;

                        block.Data.Add(field);
                    }


                    model.Blocks.Add(block);
                }

                switch (item.Status)
                {
                    case E_OrderStatus.WaitPay:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "取消订单", Color = "red" }, OpType = "FUN", OpVal = "cancleOrder" });
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "继续支付", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id) });
                        break;
                    case E_OrderStatus.Payed:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id) });
                        break;
                    case E_OrderStatus.Completed:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id) });
                        break;
                    case E_OrderStatus.Cancled:
                        model.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = OperateService.GetOrderDetailsUrl(rup.Caller, item.Id) });
                        break;
                }

                models.Add(model);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", models);

            return result;
        }

        public CustomJsonResult Details(string operater, string clientUserId, string orderId)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderDetails();

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();


            ret.Status = order.Status;


            ret.Tag.Name = new FsText(order.StoreName, "");
            ret.Tag.Desc = new FsField("状态", "", GetOrderStatus(order.Status), "");

            var fsBlockByField = new FsBlockByField();

            fsBlockByField.Tag.Name = new FsText("订单信息", "");

            fsBlockByField.Data.Add(new FsField("订单编号", "", order.Sn, ""));
            fsBlockByField.Data.Add(new FsField("创建时间", "", order.SubmitTime.ToUnifiedFormatDateTime(), ""));
            if (order.PayTime != null)
            {
                fsBlockByField.Data.Add(new FsField("付款时间", "", order.PayTime.ToUnifiedFormatDateTime(), ""));
            }
            if (order.CompletedTime != null)
            {
                fsBlockByField.Data.Add(new FsField("完成时间", "", order.CompletedTime.ToUnifiedFormatDateTime(), ""));
            }

            if (order.CancledTime != null)
            {
                fsBlockByField.Data.Add(new FsField("取消时间", "", order.CancledTime.ToUnifiedFormatDateTime(), ""));
                fsBlockByField.Data.Add(new FsField("取消原因", "", order.CancelReason, ""));
            }

            ret.FieldBlocks.Add(fsBlockByField);


            var orderDetails = CurrentDb.OrderDetails.Where(c => c.OrderId == order.Id).ToList();


            foreach (var orderDetail in orderDetails)
            {
                var block = new FsBlock();

                block.Tag.Name = new FsText(orderDetail.SellChannelRefName, "");


                var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderDetailsId == orderDetail.Id).ToList();

                foreach (var orderDetailsChild in orderDetailsChilds)
                {

                    var data = new FsTemplateData();

                    data.Type = "SkuTmp";

                    var sku = new FsTemplateData.TmplOrderSku();

                    sku.Id = orderDetailsChild.Id;
                    sku.Name = orderDetailsChild.PrdProductSkuName;
                    sku.MainImgUrl = orderDetailsChild.PrdProductSkuMainImgUrl;
                    sku.Quantity = orderDetailsChild.Quantity.ToString();
                    sku.ChargeAmount = orderDetailsChild.ChargeAmount.ToF2Price();


                    data.Value = sku;

                    block.Data.Add(data);
                }


                ret.Blocks.Add(block);
            }




            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

        }

        public CustomJsonResult Cancle(string operater, string clientUserId, RopOrderCancle rop)
        {
            return BLL.Biz.BizFactory.Order.Cancle(operater, rop.Id, "用户取消");
        }

        public CustomJsonResult JsApiPaymentPms(string operater, string clientUserId, RupOrderJsApiPaymentPms rup)
        {
            var result = new CustomJsonResult();

            var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientUserId == clientUserId).FirstOrDefault();

            if (wxUserInfo == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户数据");
            }

            var order = CurrentDb.Order.Where(m => m.Id == rup.OrderId).FirstOrDefault();

            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单数据");
            }

            LogUtil.Info("MerchId:" + order.MerchId);
            LogUtil.Info("WxMpAppId:" + rup.AppId);

            var wxAppInfoConfig = BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);

            if (wxAppInfoConfig == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户信息认证失败");
            }


            order.AppId = wxAppInfoConfig.AppId;
            order.ClientUserId = wxUserInfo.ClientUserId;
            order.PayExpireTime = DateTime.Now.AddMinutes(5);
            order.PayCaller = rup.PayCaller;
            order.PayWay = rup.PayWay;
            switch (rup.PayCaller)
            {
                case  E_OrderPayCaller.WechatByMp:

                    var orderAttach = new BLL.Biz.OrderAttachModel();
                    orderAttach.MerchId = order.MerchId;
                    orderAttach.StoreId = order.StoreId;
    
                    var ret_UnifiedOrder = SdkFactory.Wx.UnifiedOrderByJsApi(wxAppInfoConfig, wxUserInfo.OpenId, order.Sn, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);

                    if (string.IsNullOrEmpty(ret_UnifiedOrder.PrepayId))
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                    }
                    order.PayPrepayId = ret_UnifiedOrder.PrepayId;
                    order.PayQrCodeUrl = ret_UnifiedOrder.CodeUrl;
                    CurrentDb.SaveChanges();

                    //Task4Factory.Global.Enter(TimerTaskType.CheckOrderPay, order.PayExpireTime.Value, order);

                    var pms = SdkFactory.Wx.GetJsApiPayParams(wxAppInfoConfig, order.Id, order.Sn, ret_UnifiedOrder.PrepayId);

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", pms);
                    break;
            }







            //JsApiPayParams parms = new JsApiPayParams("wxb01e0e16d57bd762", "37b016a9569e4f519702696e1274d63a", ret_UnifiedOrder.PrepayId, order.Id, order.Sn);

            //return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret.Data);

            return result;
        }


        public CustomJsonResult PayResultNotify(string operater, E_OrderNotifyLogNotifyFrom from, string content, string orderSn, out bool isPaySuccessed)
        {
            return BLL.Biz.BizFactory.Order.PayResultNotify(operater, from, content, orderSn, out isPaySuccessed);
        }
    }
}