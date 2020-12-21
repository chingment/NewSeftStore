﻿using LocalS.BLL;
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
            bizRop.CouponIds = rop.CouponIds;
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

                if (rop.ShopMethod == E_ShopMethod.Unknow)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择购物类型");
                }

                store = BizFactory.Store.GetOne(rop.StoreId);

                foreach (var productSku in rop.ProductSkus)
                {
                    if (productSku.ShopMode == E_SellChannelRefType.Machine || productSku.ShopMode == E_SellChannelRefType.Mall)
                    {
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
                        productSku.Kind3 = r_productSku.KindId3;
                        productSku.RentMhPrice = r_productSku.Stocks[0].RentMhPrice;
                        productSku.DepositPrice = r_productSku.Stocks[0].RentMhPrice;

                        if (rop.ShopMethod == E_ShopMethod.Shop)
                        {
                            productSku.SalePrice = r_productSku.Stocks[0].SalePrice;

                            LogUtil.Info("clientUser.MemberLeve:" + clientUser.MemberLevel);
                            //切换会员价
                            if (clientUser.MemberLevel > 0)
                            {
                                var memberProductSkuSt = CurrentDb.MemberProductSkuSt.Where(m => m.MerchId == store.MerchId && m.StoreId == store.StoreId && m.PrdProductSkuId == productSku.Id && m.MemberLevel == clientUser.MemberLevel && m.IsDisabled == false).FirstOrDefault();
                                if (memberProductSkuSt != null)
                                {
                                    productSku.SalePrice = memberProductSkuSt.MemberPrice;
                                    LogUtil.Info("clientUser.MemberPrice:" + memberProductSkuSt.MemberPrice);
                                }
                            }

                            productSku.OriginalPrice = r_productSku.Stocks[0].SalePrice;
                            productSku.SaleAmount = productSku.Quantity * productSku.SalePrice;
                            productSku.OriginalAmount = productSku.Quantity * productSku.OriginalPrice;
                        }
                        else if (rop.ShopMethod == E_ShopMethod.Rent)
                        {
                            productSku.SalePrice = r_productSku.Stocks[0].RentMhPrice + r_productSku.Stocks[0].DepositPrice;
                            productSku.OriginalPrice = r_productSku.Stocks[0].RentMhPrice + r_productSku.Stocks[0].DepositPrice;
                            productSku.SaleAmount = productSku.Quantity * productSku.SalePrice;
                            productSku.OriginalAmount = productSku.Quantity * productSku.OriginalPrice;
                        }


                        c_prodcutSkus.Add(productSku);

                    }
                    else if (productSku.ShopMode == E_SellChannelRefType.MemberFee)
                    {
                        var memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == store.MerchId && m.Id == productSku.Id).FirstOrDefault();
                        if (memberFeeSt != null)
                        {
                            productSku.Name = memberFeeSt.Name;
                            productSku.MainImgUrl = memberFeeSt.MainImgUrl;
                            productSku.SalePrice = memberFeeSt.FeeSaleValue;
                            productSku.OriginalPrice = memberFeeSt.FeeOriginalValue;
                            productSku.SaleAmount = productSku.Quantity * productSku.SalePrice;
                            productSku.OriginalAmount = productSku.Quantity * productSku.OriginalPrice;
                            productSku.ProductId = "";
                            productSku.Kind3 = 0;
                            c_prodcutSkus.Add(productSku);
                        }
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

                if (rop.ShopMethod == E_ShopMethod.Shop)
                {
                    if (rop.CouponIdsByShop == null || rop.CouponIdsByShop.Count == 0)
                    {
                        var couponCanUseCount = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, new E_Coupon_FaceType[] { E_Coupon_FaceType.ShopVoucher, E_Coupon_FaceType.ShopDiscount }, c_prodcutSkus, store.MerchId, store.StoreId, clientUserId);

                        if (couponCanUseCount == 0)
                        {
                            ret.CouponByShop = new OrderConfirmCouponModel { TipMsg = "暂无可用优惠卷", TipType = TipType.NoCanUse };
                        }
                        else
                        {
                            ret.CouponByShop = new OrderConfirmCouponModel { TipMsg = string.Format("{0}个可用", couponCanUseCount), TipType = TipType.CanUse };
                        }
                    }
                    else
                    {
                        //只能使用一张优惠券
                        var coupon = (from u in CurrentDb.ClientCoupon
                                      join m in CurrentDb.Coupon on u.CouponId equals m.Id into temp
                                      from tt in temp.DefaultIfEmpty()
                                      where u.ClientUserId == clientUserId && rop.CouponIdsByShop.Contains(u.Id)
                                      select new { u.Id, u.ClientUserId, u.MerchId, tt.Name, tt.UseAreaType, tt.UseAreaValue, u.Status, u.ValidEndTime, u.ValidStartTime, tt.FaceType, tt.FaceValue, tt.AtLeastAmount }).FirstOrDefault();
                        if (coupon != null)
                        {

                            //若有优惠券重新计算优惠金额
                            foreach (var productSku in rop.ProductSkus)
                            {
                                bool isCalComplete = false;
                                productSku.CouponAmount = BizFactory.Order.CalCouponAmount(amount_original, coupon.AtLeastAmount, coupon.UseAreaType, coupon.UseAreaValue, coupon.FaceType, coupon.FaceValue, productSku.ProductId, productSku.Kind3, productSku.SaleAmount, out isCalComplete);
                                if (isCalComplete)
                                    break;
                            }

                            amount_couponByShop = rop.ProductSkus.Sum(m => m.CouponAmount);

                            ret.CouponByShop = new OrderConfirmCouponModel { TipMsg = string.Format("-{0}", amount_couponByShop.ToF2Price()), TipType = TipType.InUse };
                        }
                    }
                }
                else if (rop.ShopMethod == E_ShopMethod.Rent)
                {
                    if (string.IsNullOrEmpty(rop.CouponIdByRent))
                    {
                        var couponCanUseCount = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, new E_Coupon_FaceType[] { E_Coupon_FaceType.RentVoucher }, c_prodcutSkus, store.MerchId, store.StoreId, clientUserId);

                        if (couponCanUseCount == 0)
                        {
                            ret.CouponByRent = new OrderConfirmCouponModel { TipMsg = "暂无可用租金券", TipType = TipType.NoCanUse };
                        }
                        else
                        {
                            ret.CouponByRent = new OrderConfirmCouponModel { TipMsg = string.Format("{0}个可用", couponCanUseCount), TipType = TipType.CanUse };
                        }
                    }
                    else
                    {
                        //只能使用一张优惠券
                        var coupon = (from u in CurrentDb.ClientCoupon
                                      join m in CurrentDb.Coupon on u.CouponId equals m.Id into temp
                                      from tt in temp.DefaultIfEmpty()
                                      where u.ClientUserId == clientUserId && u.Id == rop.CouponIdByRent
                                      select new { u.Id, u.ClientUserId, u.MerchId, tt.Name, tt.UseAreaType, tt.UseAreaValue, u.Status, u.ValidEndTime, u.ValidStartTime, tt.FaceType, tt.FaceValue, tt.AtLeastAmount }).FirstOrDefault();
                        if (coupon != null)
                        {
                            amount_couponByRent = coupon.FaceValue;
                            ret.CouponByRent = new OrderConfirmCouponModel { TipMsg = string.Format("-{0}", amount_couponByRent.ToF2Price()), TipType = TipType.InUse };
                        }
                    }

                    if (string.IsNullOrEmpty(rop.CouponIdByDeposit))
                    {
                        var couponCanUseCount = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, new E_Coupon_FaceType[] { E_Coupon_FaceType.DepositVoucher }, c_prodcutSkus, store.MerchId, store.StoreId, clientUserId);

                        if (couponCanUseCount == 0)
                        {
                            ret.CouponByDeposit = new OrderConfirmCouponModel { TipMsg = "暂无可用押金券", TipType = TipType.NoCanUse };
                        }
                        else
                        {
                            ret.CouponByDeposit = new OrderConfirmCouponModel { TipMsg = string.Format("{0}个可用", couponCanUseCount), TipType = TipType.CanUse };
                        }
                    }
                    else
                    {
                        //只能使用一张优惠券
                        var coupon = (from u in CurrentDb.ClientCoupon
                                      join m in CurrentDb.Coupon on u.CouponId equals m.Id into temp
                                      from tt in temp.DefaultIfEmpty()
                                      where u.ClientUserId == clientUserId && u.Id == rop.CouponIdByDeposit
                                      select new { u.Id, u.ClientUserId, u.MerchId, tt.Name, tt.UseAreaType, tt.UseAreaValue, u.Status, u.ValidEndTime, u.ValidStartTime, tt.FaceType, tt.FaceValue, tt.AtLeastAmount }).FirstOrDefault();
                        if (coupon != null)
                        {
                            amount_couponByDeposit = coupon.FaceValue;

                            ret.CouponByDeposit = new OrderConfirmCouponModel { TipMsg = string.Format("-{0}", amount_couponByDeposit.ToF2Price()), TipType = TipType.InUse };
                        }
                    }
                }


                int memberLevel = 0;

                if (clientUser != null)
                {
                    memberLevel = clientUser.MemberLevel;
                }

                //if (memberLevel > 0)
                //{
                amount_charge = amount_sale;//会员用户总价 为 实际总价

                //decimal member_coupon = amount_original - amount_sale;
                // c_subtotalItems.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "会员优惠", Amount = string.Format("-{0}", , IsDcrease = true });
                //}
                //else
                //{
                //  amount_charge = amount_original;
                //}

                amount_charge = amount_charge - amount_couponByShop - amount_couponByRent - amount_couponByDeposit;

                if (amount_charge < 0)
                {
                    amount_charge = 0;
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
                    c_prodcutSku.OriginalPrice = l_orderSubChilds[0].OriginalPrice;
                    c_prodcutSku.SaleAmount = l_orderSubChilds.Sum(m => m.SaleAmount);
                    c_prodcutSku.OriginalAmount = l_orderSubChilds.Sum(m => m.OriginalAmount);
                    c_prodcutSku.ShopMode = shopModeSku.SellChannelRefType;
                    c_prodcutSkus.Add(c_prodcutSku);
                }

                amount_original = orders.Sum(m => m.OriginalAmount);
                amount_sale = orders.Sum(m => m.SaleAmount);
                amount_couponByShop = orders.Sum(m => m.DiscountAmount);
                amount_charge = orders.Sum(m => m.ChargeAmount);
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


            //c_subtotalItems.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "优惠卷", Amount = "-10", IsDcrease = true });
            //subtotalItem.Add(new OrderConfirmSubtotalItemModel { ImgUrl = "", Name = "满5减3元", Amount = "-9", IsDcrease = true });

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