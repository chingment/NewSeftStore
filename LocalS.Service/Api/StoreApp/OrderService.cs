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
                if (order.ReceiveMode == E_ReceiveMode.MachineSelfTake)
                {
                    block.Tag.Desc = new FsField("取货码", "", order.PickupCode, "#f18d00");
                    block.Qrcode = new FsQrcode { Code = BizFactory.Order.BuildQrcode2PickupCode(order.PickupCode), Url = "", Remark = string.Format("扫码枪扫一扫", order.SellChannelRefId) };
                }

                if (order.ReceiveMode == E_ReceiveMode.Delivery || order.ReceiveMode == E_ReceiveMode.MachineSelfTake || order.ReceiveMode == E_ReceiveMode.StoreSelfTake)
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
            bizRop.CouponIdsByShop = rop.CouponIdsByShop;
            bizRop.CouponIdByDeposit = rop.CouponIdByDeposit;
            bizRop.CouponIdByRent = rop.CouponIdByRent;
            bizRop.ShopMethod = rop.ShopMethod;
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

            decimal amount_charge = 0;//实际支付总价
            decimal amount_original = 0;//原来的支付总价
            decimal amount_sale = 0;//用户的支付总价
                                    // decimal amount_discount = 0;//折扣总额
            decimal amount_couponByShop = 0;//购物优惠总额
            decimal amount_couponByRent = 0;//租金优惠总额
            decimal amount_couponByDeposit = 0;//押金优惠总额
            StoreInfoModel store;
            DeliveryModel dliveryModel = new DeliveryModel();
            E_ReceiveMode receiveMode_Mall = E_ReceiveMode.Delivery;
            BookTimeModel bookTimeModel = new BookTimeModel();
            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            if (rop.OrderIds == null || rop.OrderIds.Count == 0)
            {

                if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择商品");
                }

                if (rop.ProductSkus.Where(m => m.ShopMode == E_SellChannelRefType.Unknow).Count() != 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品指定的购物模式有误");
                }

                if (rop.ShopMethod == E_OrderShopMethod.Unknow)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择购物类型");
                }

                store = BizFactory.Store.GetOne(rop.StoreId);


                foreach (var productSku in rop.ProductSkus)
                {
                    if (productSku.ShopMethod == E_OrderShopMethod.Shop)
                    {
                        #region Shop
                        var r_productSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.StoreId, store.GetSellChannelRefIds(productSku.ShopMode), productSku.Id);

                        if (r_productSku == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统发生异常01", ret);
                        }

                        if (r_productSku.Stocks.Count == 0)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统发生异常02", ret);
                        }

                        productSku.Name = r_productSku.Name;
                        productSku.MainImgUrl = r_productSku.MainImgUrl;
                        productSku.ProductId = r_productSku.ProductId;
                        productSku.KindId3 = r_productSku.KindId3;
                        productSku.RentTermUnit = E_RentTermUnit.Month;
                        productSku.SupReceiveMode = r_productSku.SupReceiveMode;
                        productSku.RentTermUnitText = "月";
                        productSku.RentTermValue = 1;
                        productSku.RentAmount = r_productSku.Stocks[0].RentMhPrice;
                        productSku.DepositAmount = r_productSku.Stocks[0].DepositPrice;

                        decimal salePrice = r_productSku.Stocks[0].SalePrice;

                        decimal originalPrice = salePrice;

                        LogUtil.Info("clientUser.MemberLeve:" + clientUser.MemberLevel);

                        //切换会员价
                        if (clientUser.MemberLevel > 0)
                        {
                            var memberProductSkuSt = CurrentDb.MemberProductSkuSt.Where(m => m.MerchId == store.MerchId && m.StoreId == store.StoreId && m.PrdProductSkuId == productSku.Id && m.MemberLevel == clientUser.MemberLevel && m.IsDisabled == false).FirstOrDefault();
                            if (memberProductSkuSt != null)
                            {
                                salePrice = memberProductSkuSt.MemberPrice;
                                LogUtil.Info("clientUser.MemberPrice:" + memberProductSkuSt.MemberPrice);
                            }
                        }

                        productSku.SalePrice = salePrice;
                        productSku.SaleAmount = productSku.Quantity * salePrice;
                        productSku.OriginalPrice = originalPrice;
                        productSku.OriginalAmount = productSku.Quantity * originalPrice;

                        c_prodcutSkus.Add(productSku);

                        #endregion
                    }
                    else if (productSku.ShopMethod == E_OrderShopMethod.Rent)
                    {
                        #region Rent
                        var r_productSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.StoreId, store.GetSellChannelRefIds(productSku.ShopMode), productSku.Id);

                        if (r_productSku == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统发生异常01", ret);
                        }

                        if (r_productSku.Stocks.Count == 0)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统发生异常02", ret);
                        }

                        productSku.Name = r_productSku.Name;
                        productSku.MainImgUrl = r_productSku.MainImgUrl;
                        productSku.ProductId = r_productSku.ProductId;
                        productSku.KindId3 = r_productSku.KindId3;
                        productSku.SupReceiveMode = r_productSku.SupReceiveMode;
                        productSku.RentTermUnit = E_RentTermUnit.Month;
                        productSku.RentTermUnitText = "月";
                        productSku.RentTermValue = 1;
                        productSku.RentAmount = r_productSku.Stocks[0].RentMhPrice;
                        productSku.DepositAmount = r_productSku.Stocks[0].DepositPrice;
                        productSku.SalePrice = r_productSku.Stocks[0].RentMhPrice + r_productSku.Stocks[0].DepositPrice;
                        productSku.OriginalPrice = r_productSku.Stocks[0].RentMhPrice + r_productSku.Stocks[0].DepositPrice;
                        productSku.SaleAmount = productSku.Quantity * productSku.SalePrice;
                        productSku.OriginalAmount = productSku.Quantity * productSku.OriginalPrice;
                        c_prodcutSkus.Add(productSku);

                        #endregion
                    }
                    else if (productSku.ShopMethod == E_OrderShopMethod.MemberFee)
                    {
                        #region MemberFee
                        var memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == store.MerchId && m.Id == productSku.Id).FirstOrDefault();
                        if (memberFeeSt != null)
                        {
                            productSku.Name = memberFeeSt.Name;
                            productSku.SupReceiveMode =  E_SupReceiveMode.MemerbFee;
                            productSku.MainImgUrl = memberFeeSt.MainImgUrl;
                            productSku.SalePrice = memberFeeSt.FeeSaleValue;
                            productSku.SaleAmount = productSku.Quantity * productSku.SalePrice;
                            productSku.OriginalPrice = memberFeeSt.FeeOriginalValue;
                            productSku.OriginalAmount = productSku.Quantity * productSku.OriginalPrice;
                            productSku.ProductId = "";
                            productSku.KindId3 = 0;
                            c_prodcutSkus.Add(productSku);
                        }

                        #endregion
                    }
                }

                var d_shippingAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == clientUserId && m.IsDefault == true).FirstOrDefault();
                if (d_shippingAddress == null)
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
                    dliveryModel.Id = d_shippingAddress.Id;
                    dliveryModel.Consignee = d_shippingAddress.Consignee;
                    dliveryModel.PhoneNumber = d_shippingAddress.PhoneNumber;
                    dliveryModel.AreaName = d_shippingAddress.AreaName;
                    dliveryModel.Address = d_shippingAddress.Address;
                    dliveryModel.IsDefault = d_shippingAddress.IsDefault;
                }

                amount_original = c_prodcutSkus.Sum(m => m.OriginalAmount);

                amount_sale = c_prodcutSkus.Sum(m => m.SaleAmount);

                if (rop.ShopMethod == E_OrderShopMethod.Shop || rop.ShopMethod == E_OrderShopMethod.MemberFee)
                {
                    ret.CouponByShop = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, new E_Coupon_FaceType[] { E_Coupon_FaceType.ShopVoucher, E_Coupon_FaceType.ShopDiscount }, c_prodcutSkus, store.MerchId, store.StoreId, clientUserId, rop.CouponIdsByShop);

                    amount_couponByShop = ret.CouponByShop.CouponAmount;
                }
                else if (rop.ShopMethod == E_OrderShopMethod.Rent)
                {
                    #region Rent

                    List<string> couponIdsByRent = new List<string>();
                    if (rop.CouponIdByRent != null)
                    {
                        couponIdsByRent.Add(rop.CouponIdByRent);
                    }

                    ret.CouponByRent = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, new E_Coupon_FaceType[] { E_Coupon_FaceType.RentVoucher }, c_prodcutSkus, store.MerchId, store.StoreId, clientUserId, couponIdsByRent);

                    amount_couponByRent = ret.CouponByRent.CouponAmount;

                    List<string> couponIdsByDeposit = new List<string>();
                    if (rop.CouponIdByDeposit != null)
                    {
                        couponIdsByDeposit.Add(rop.CouponIdByDeposit);
                    }
                    ret.CouponByDeposit = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, new E_Coupon_FaceType[] { E_Coupon_FaceType.DepositVoucher }, c_prodcutSkus, store.MerchId, store.StoreId, clientUserId, couponIdsByDeposit);

                    amount_couponByDeposit = ret.CouponByDeposit.CouponAmount;

                    #endregion
                }

                amount_charge = amount_sale;

                amount_charge = amount_charge - amount_couponByShop - amount_couponByRent - amount_couponByDeposit;

                if (amount_charge < 0)
                {
                    amount_charge = 0;
                }

                ret.ShopMethod = rop.ShopMethod;
            }
            else
            {
                var orders = CurrentDb.Order.Where(m => rop.OrderIds.Contains(m.Id)).ToList();

                store = BizFactory.Store.GetOne(orders[0].StoreId);

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

                foreach (var orderSub in orderSubs)
                {
                    var r_productSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.StoreId, new string[] { orderSub.SellChannelRefId }, orderSub.PrdProductSkuId);

                    var c_prodcutSku = new OrderConfirmProductSkuModel();
                    c_prodcutSku.Id = orderSub.PrdProductSkuId;
                    c_prodcutSku.Name = orderSub.PrdProductSkuName;
                    c_prodcutSku.MainImgUrl = orderSub.PrdProductSkuMainImgUrl;
                    c_prodcutSku.Quantity = orderSub.Quantity;
                    c_prodcutSku.SalePrice = orderSub.SalePrice;
                    c_prodcutSku.OriginalPrice = orderSub.OriginalPrice;
                    c_prodcutSku.SaleAmount = orderSub.SaleAmount;
                    c_prodcutSku.OriginalAmount = orderSub.OriginalAmount;
                    c_prodcutSku.ShopMethod = orderSub.ShopMethod;
                    c_prodcutSku.ShopMode = orderSub.SellChannelRefType;
                    c_prodcutSku.SupReceiveMode = r_productSku.SupReceiveMode;
                    c_prodcutSku.RentTermUnitText = "月";
                    c_prodcutSku.RentTermUnit = orderSub.RentTermUnit;
                    c_prodcutSku.RentTermValue = orderSub.RentTermValue;
                    c_prodcutSku.RentAmount = orderSub.RentAmount;
                    c_prodcutSku.DepositAmount = orderSub.DepositAmount;
                    c_prodcutSku.KindId3 = orderSub.PrdKindId3;
                    c_prodcutSkus.Add(c_prodcutSku);
                }


                ret.ShopMethod = orders[0].ShopMethod;

                amount_couponByShop = orders.Sum(m => m.CouponAmountByShop);
                amount_couponByRent = orders.Sum(m => m.CouponAmountByRent);
                amount_couponByDeposit = orders.Sum(m => m.CouponAmountByDeposit);

                ret.CouponByShop = new OrderConfirmCouponModel { TipMsg = amount_couponByShop == 0 ? "无优惠" : string.Format("-{0}", amount_couponByShop.ToF2Price()), TipType = TipType.InUse };
                ret.CouponByDeposit = new OrderConfirmCouponModel { TipMsg = amount_couponByDeposit == 0 ? "无优惠" : string.Format("-{0}", amount_couponByDeposit.ToF2Price()), TipType = TipType.InUse };
                ret.CouponByRent = new OrderConfirmCouponModel { TipMsg = amount_couponByRent == 0 ? "无优惠" : string.Format("-{0}", amount_couponByRent.ToF2Price()), TipType = TipType.InUse };

                amount_original = orders.Sum(m => m.OriginalAmount);
                amount_sale = orders.Sum(m => m.SaleAmount);
                amount_couponByShop = orders.Sum(m => m.DiscountAmount);
                amount_charge = orders.Sum(m => m.ChargeAmount);
            }


            var orderBlock = new List<OrderBlockModel>();

            var skus_Mall = c_prodcutSkus.Where(m => m.ShopMode == E_SellChannelRefType.Mall).ToList();

            if (skus_Mall.Count > 0)
            {
                var skus_DeliveryOrStoreSelfTake = skus_Mall.Where(m => m.ShopMethod == E_OrderShopMethod.Shop || m.ShopMethod == E_OrderShopMethod.Rent).ToList();

                if (skus_DeliveryOrStoreSelfTake.Count > 0)
                {
                    var skus_Delivery = skus_DeliveryOrStoreSelfTake.Where(m => m.SupReceiveMode ==  E_SupReceiveMode.Delivery).ToList();
                    if (skus_Delivery.Count > 0)
                    {
                        var orderBlock_Delivery = new OrderBlockModel();
                        orderBlock_Delivery.TagName = "线上商城[配送]";
                        orderBlock_Delivery.Skus = skus_Delivery;
                        orderBlock_Delivery.TabMode = E_TabMode.Delivery;
                        orderBlock_Delivery.ReceiveMode = E_ReceiveMode.Delivery;
                        orderBlock_Delivery.Delivery = dliveryModel;
                        orderBlock_Delivery.BookTime = bookTimeModel;
                        orderBlock_Delivery.SelfTake.StoreName = store.Name;
                        orderBlock_Delivery.SelfTake.StoreAddress = store.Address;
                        orderBlock.Add(orderBlock_Delivery);

                    }

                    var skus_StoreSelfTake = skus_DeliveryOrStoreSelfTake.Where(m => m.SupReceiveMode ==  E_SupReceiveMode.StoreSelfTake).ToList();

                    if (skus_StoreSelfTake.Count > 0)
                    {
  
                        var orderBlock_StoreSelfTake = new OrderBlockModel();
                        orderBlock_StoreSelfTake.TagName = "线上商城[自提]";
                        orderBlock_StoreSelfTake.Skus = skus_StoreSelfTake;
                        orderBlock_StoreSelfTake.TabMode = E_TabMode.StoreSelfTake;
                        orderBlock_StoreSelfTake.ReceiveMode = E_ReceiveMode.StoreSelfTake;
                        orderBlock_StoreSelfTake.Delivery = dliveryModel;
                        orderBlock_StoreSelfTake.BookTime = bookTimeModel;
                        orderBlock_StoreSelfTake.SelfTake.StoreName = store.Name;
                        orderBlock_StoreSelfTake.SelfTake.StoreAddress = store.Address;
                        orderBlock.Add(orderBlock_StoreSelfTake);

                    }

                    var skus_DeliveryAndStoreSelfTake = skus_DeliveryOrStoreSelfTake.Where(m => m.SupReceiveMode == E_SupReceiveMode.DeliveryAndStoreSelfTake).ToList();

                    if (skus_DeliveryAndStoreSelfTake.Count > 0)
                    {
                        var orderBlock_DeliveryOrStoreSelfTake = new OrderBlockModel();
                        orderBlock_DeliveryOrStoreSelfTake.TagName = "线上商城[配送或自提]";
                        orderBlock_DeliveryOrStoreSelfTake.Skus = skus_DeliveryAndStoreSelfTake;
                        orderBlock_DeliveryOrStoreSelfTake.TabMode = E_TabMode.DeliveryAndStoreSelfTake;
                        orderBlock_DeliveryOrStoreSelfTake.ReceiveMode = E_ReceiveMode.Delivery;
                        orderBlock_DeliveryOrStoreSelfTake.Delivery = dliveryModel;
                        orderBlock_DeliveryOrStoreSelfTake.BookTime = bookTimeModel;
                        orderBlock_DeliveryOrStoreSelfTake.SelfTake.StoreName = store.Name;
                        orderBlock_DeliveryOrStoreSelfTake.SelfTake.StoreAddress = store.Address;
                        orderBlock.Add(orderBlock_DeliveryOrStoreSelfTake);

                    }

                }


                var skus_MemberFee = skus_Mall.Where(m => m.ShopMethod == E_OrderShopMethod.MemberFee).ToList();

                if (skus_MemberFee.Count > 0)
                {
                    var orderBlock_MemberFee = new OrderBlockModel();
                    orderBlock_MemberFee.TagName = "会员费";
                    orderBlock_MemberFee.Skus = skus_MemberFee;
                    orderBlock_MemberFee.TabMode = E_TabMode.MemerbFee;
                    orderBlock_MemberFee.ReceiveMode = E_ReceiveMode.MemberFee;
                    orderBlock.Add(orderBlock_MemberFee);
                }

            }

            var skus_MachineSelfTake = c_prodcutSkus.Where(m => m.ShopMode == E_SellChannelRefType.Machine).ToList();
            if (skus_MachineSelfTake.Count > 0)
            {
                var orderBlock_MachineSelfTake = new OrderBlockModel();
                orderBlock_MachineSelfTake.TagName = "线下机器";
                orderBlock_MachineSelfTake.Skus = skus_MachineSelfTake;
                orderBlock_MachineSelfTake.TabMode = E_TabMode.MachineSelfTake;
                orderBlock_MachineSelfTake.ReceiveMode = E_ReceiveMode.MachineSelfTake;
                orderBlock_MachineSelfTake.SelfTake.StoreName = store.Name;
                orderBlock_MachineSelfTake.SelfTake.StoreAddress = store.Address;
                orderBlock.Add(orderBlock_MachineSelfTake);
            }

            ret.Blocks = orderBlock;

            ret.SubtotalItems = c_subtotalItems;

            ret.ActualAmount = amount_charge.ToF2Price();

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
                             ReceiveMode = o.ReceiveMode,
                             ReceiveModeName = o.ReceiveModeName,
                             SellChannelRefId = o.SellChannelRefId,
                             PickupFlowLastTime = o.PickupFlowLastTime,
                             PickupFlowLastDesc = o.PickupFlowLastDesc,
                             PickupCode = o.PickupCode,
                             ShopMethod = o.ShopMethod,
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
                    if (item.ReceiveMode == E_ReceiveMode.MachineSelfTake)
                    {
                        block.Tag.Desc = new FsField("取货码", "", item.PickupCode, "#f18d00");
                        block.Qrcode = new FsQrcode { Code = BizFactory.Order.BuildQrcode2PickupCode(item.PickupCode), Url = "", Remark = string.Format("扫码枪扫一扫", item.SellChannelRefId) };
                    }

                    if (item.ReceiveMode == E_ReceiveMode.Delivery || item.ReceiveMode == E_ReceiveMode.MachineSelfTake || item.ReceiveMode == E_ReceiveMode.StoreSelfTake)
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