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
    public class OrderService : BaseService
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
                if (order.ReceiveMode == E_ReceiveMode.SelfTakeByMachine)
                {
                    block.Tag.Desc = new FsField("取货码", "", order.PickupCode, "#f18d00");
                    block.Qrcode = new FsQrcode { Code = MyDESCryptoUtil.BuildQrcode2PickupCode(order.PickupCode), Url = "", Remark = "扫码枪扫一扫" };
                }
                else if (order.ReceiveMode == E_ReceiveMode.SelfTakeByStore)
                {
                    block.Tag.Desc = new FsField("取货码", "", order.PickupCode, "#f18d00");
                    block.Qrcode = new FsQrcode { Code = MyDESCryptoUtil.BuildQrcode2PickupCode(order.PickupCode), Url = "", Remark = "出示给店员扫一扫" };
                }

                if (order.ReceiveMode == E_ReceiveMode.Delivery || order.ReceiveMode == E_ReceiveMode.SelfTakeByMachine || order.ReceiveMode == E_ReceiveMode.SelfTakeByStore)
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
                    sku.OriginalAmount = orderSub.OriginalAmount.ToF2Price();
                    sku.SaleAmount = orderSub.SaleAmount.ToF2Price();
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
                    sku.SaleAmount = orderSubChilds_Sku.Sum(m => m.SaleAmount).ToF2Price();
                    sku.OriginalAmount = orderSubChilds_Sku.Sum(m => m.OriginalAmount).ToF2Price();
                    sku.StatusName = "";
                    field.Value = sku;
                    block.Data.Add(field);
                }
            }




            fsBlocks.Add(block);


            return fsBlocks;
        }


        public void BuidProductSkus()
        {

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
            bizRop.ReffSign = rop.ReffSign;


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

            var c_subtotalItems = new List<RetOrderConfirm.SubtotalItemModel>();

            var c_prodcutSkus = new List<BuildSku>();

            decimal amount_charge = 0;//实际支付总价
            decimal amount_original = 0;//原来的支付总价
            decimal amount_sale = 0;//用户的支付总价
                                    // decimal amount_discount = 0;//折扣总额
            decimal amount_couponByShop = 0;//购物优惠总额
            decimal amount_couponByRent = 0;//租金优惠总额
            decimal amount_couponByDeposit = 0;//押金优惠总额
            StoreInfoModel store;
            DeliveryModel dliveryModel = new DeliveryModel();
            SelfTakeModel selfTakeModel = new SelfTakeModel();

            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            if (rop.OrderIds == null || rop.OrderIds.Count == 0)
            {

                if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择商品");
                }

                if (rop.ProductSkus.Where(m => m.ShopMode == E_ShopMode.Unknow).Count() != 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品指定的购物模式有误");
                }

                if (rop.ShopMethod == E_ShopMethod.Unknow)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择购物类型");
                }

                store = BizFactory.Store.GetOne(rop.StoreId);

                int clientMemberLevel = 0;
                if (clientUser != null)
                {
                    clientMemberLevel = clientUser.MemberLevel;
                }


                BuildOrderTool buildOrderTool = new BuildOrderTool(store.MerchId, store.StoreId, clientMemberLevel);

                foreach (var productSku in rop.ProductSkus)
                {
                    buildOrderTool.AddSku(productSku.Id, productSku.Quantity, productSku.CartId, productSku.ShopMode, productSku.ShopMethod, E_ReceiveMode.Unknow, productSku.ShopId, null);
                }

                c_prodcutSkus = buildOrderTool.BuildSkus();

                if (buildOrderTool.IsSuccess)
                {
                    ret.IsCanPay = true;
                }
                else
                {
                    ret.IsCanPay = false;
                }

                var d_shippingAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == clientUserId && m.IsDefault == true).FirstOrDefault();
                if (d_shippingAddress == null)
                {
                    dliveryModel.Contact.Id = "";
                    dliveryModel.Contact.Consignee = "配送地址";
                    dliveryModel.Contact.PhoneNumber = "选择";
                    dliveryModel.Contact.AreaName = "";
                    dliveryModel.Contact.Address = "";
                    dliveryModel.Contact.IsDefault = false;

                    selfTakeModel.Contact.Id = "";
                    selfTakeModel.Contact.Consignee = "";
                    selfTakeModel.Contact.PhoneNumber = "";
                    selfTakeModel.Contact.AreaName = "";
                    selfTakeModel.Contact.AreaCode = "";
                    selfTakeModel.Contact.Address = "";
                }
                else
                {
                    dliveryModel.Contact.Id = d_shippingAddress.Id;
                    dliveryModel.Contact.Consignee = d_shippingAddress.Consignee;
                    dliveryModel.Contact.PhoneNumber = d_shippingAddress.PhoneNumber;
                    dliveryModel.Contact.AreaName = d_shippingAddress.AreaName;
                    dliveryModel.Contact.Address = d_shippingAddress.Address;
                    dliveryModel.Contact.IsDefault = d_shippingAddress.IsDefault;

                    selfTakeModel.Contact.Id = d_shippingAddress.Id;
                    selfTakeModel.Contact.Consignee = d_shippingAddress.Consignee;
                    selfTakeModel.Contact.PhoneNumber = d_shippingAddress.PhoneNumber;
                    selfTakeModel.Contact.AreaName = d_shippingAddress.AreaName;
                    selfTakeModel.Contact.AreaCode = d_shippingAddress.AreaCode;
                    selfTakeModel.Contact.Address = d_shippingAddress.Address;
                    selfTakeModel.Contact.IsDefault = d_shippingAddress.IsDefault;
                }

                var selfPickAddress = (from s in CurrentDb.StoreShop
                                       join m in CurrentDb.Shop on s.ShopId equals m.Id into temp
                                       from u in temp.DefaultIfEmpty()
                                       where
                                       u.MerchId == store.MerchId
                                       && s.StoreId == store.StoreId
                                       select new { u.Id, u.Name, u.Address, u.AreaCode, u.AreaName, u.MerchId, s.StoreId, u.ContactName, u.ContactPhone, u.ContactAddress }).FirstOrDefault();

                if (selfPickAddress == null)
                {
                    selfTakeModel.Mark.Id = "";
                    selfTakeModel.Mark.Name = "";
                    selfTakeModel.Mark.Consignee = "";
                    selfTakeModel.Mark.PhoneNumber = "";
                    selfTakeModel.Mark.AreaCode = "";
                    selfTakeModel.Mark.AreaName = "";
                    selfTakeModel.Mark.Address = "";
                }
                else
                {
                    selfTakeModel.Mark.Id = selfPickAddress.Id;
                    selfTakeModel.Mark.Name = selfPickAddress.Name;
                    selfTakeModel.Mark.Consignee = selfPickAddress.ContactName;
                    selfTakeModel.Mark.PhoneNumber = selfPickAddress.ContactPhone;
                    selfTakeModel.Mark.AreaName = selfPickAddress.AreaName;
                    selfTakeModel.Mark.AreaCode = selfPickAddress.AreaCode;
                    selfTakeModel.Mark.Address = selfPickAddress.Address;

                }

                string time = "";
                if (DateTime.Now.Minute > 0 && DateTime.Now.Minute < 30)
                {
                    time = DateTime.Now.ToString("HH:30");
                }
                else
                {
                    time = DateTime.Now.AddHours(1).ToString("HH:00");
                }

                selfTakeModel.BookTime.Type = "1";
                selfTakeModel.BookTime.Date = DateTime.Now.ToString("yyy-MM-dd");
                selfTakeModel.BookTime.Time = time;
                selfTakeModel.BookTime.Week = Lumos.CommonUtil.GetCnWeekDayName(DateTime.Now);
                selfTakeModel.BookTime.Text = string.Format("（{0}）{1} {2}", selfTakeModel.BookTime.Week, selfTakeModel.BookTime.Date, selfTakeModel.BookTime.Time);
                selfTakeModel.BookTime.Value = string.Format("{0} {1}", selfTakeModel.BookTime.Date, selfTakeModel.BookTime.Time);


                amount_original = c_prodcutSkus.Sum(m => m.OriginalAmount);

                amount_sale = c_prodcutSkus.Sum(m => m.SaleAmount);

                if (rop.ShopMethod == E_ShopMethod.Buy || rop.ShopMethod == E_ShopMethod.MemberFee)
                {
                    ret.CouponByShop = StoreAppServiceFactory.Coupon.GetCanUseCount(rop.ShopMethod, new E_Coupon_FaceType[] { E_Coupon_FaceType.ShopVoucher, E_Coupon_FaceType.ShopDiscount }, c_prodcutSkus, store.MerchId, store.StoreId, clientUserId, rop.CouponIdsByShop);

                    amount_couponByShop = ret.CouponByShop.CouponAmount;
                }
                else if (rop.ShopMethod == E_ShopMethod.Rent)
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


                var orderSubs = CurrentDb.OrderSub.Where(m => rop.OrderIds.Contains(m.OrderId)).ToList();

                foreach (var orderSub in orderSubs)
                {
                    var r_productSku = CacheServiceFactory.Product.GetSkuStock(orderSub.ShopMode, store.MerchId, store.StoreId, orderSub.ShopId, new string[] { orderSub.MachineId }, orderSub.PrdProductSkuId);

                    var c_prodcutSku = new BuildSku();
                    c_prodcutSku.Id = orderSub.PrdProductSkuId;
                    c_prodcutSku.Name = orderSub.PrdProductSkuName;
                    c_prodcutSku.MainImgUrl = orderSub.PrdProductSkuMainImgUrl;
                    c_prodcutSku.Quantity = orderSub.Quantity;
                    c_prodcutSku.SalePrice = orderSub.SalePrice;
                    c_prodcutSku.OriginalPrice = orderSub.OriginalPrice;
                    c_prodcutSku.SaleAmount = orderSub.SaleAmount;
                    c_prodcutSku.OriginalAmount = orderSub.OriginalAmount;
                    c_prodcutSku.ShopMethod = orderSub.ShopMethod;
                    c_prodcutSku.ShopMode = orderSub.ShopMode;
                    c_prodcutSku.SupReceiveMode = r_productSku.SupReceiveMode;
                    c_prodcutSku.ReceiveMode = orderSub.ReceiveMode;
                    c_prodcutSku.RentTermUnitText = "月";
                    c_prodcutSku.RentTermUnit = orderSub.RentTermUnit;
                    c_prodcutSku.RentTermValue = orderSub.RentTermValue;
                    c_prodcutSku.RentAmount = orderSub.RentAmount;
                    c_prodcutSku.DepositAmount = orderSub.DepositAmount;
                    c_prodcutSku.KindId3 = orderSub.PrdKindId3;
                    c_prodcutSku.ShopId = orderSub.ShopId;
                    c_prodcutSku.MachineIds = new string[] { orderSub.MachineId };
                    if (orderSub.ShopMethod == E_ShopMethod.MemberFee)
                    {
                        c_prodcutSku.IsOffSell = false;
                    }
                    else
                    {
                        c_prodcutSku.IsOffSell = r_productSku.IsOffSell;
                    }
                    c_prodcutSkus.Add(c_prodcutSku);
                }

                var d_shippingAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == clientUserId && m.IsDefault == true).FirstOrDefault();

                var delivery = orders.Where(m => m.ReceiveMode == E_ReceiveMode.Delivery).FirstOrDefault();
                var selfTake = orders.Where(m => m.ReceiveMode == E_ReceiveMode.SelfTakeByMachine || m.ReceiveMode == E_ReceiveMode.SelfTakeByStore).FirstOrDefault();

                if (delivery == null)
                {
                    if (d_shippingAddress == null)
                    {
                        dliveryModel.Contact.Id = "";
                        dliveryModel.Contact.Consignee = "配送地址";
                        dliveryModel.Contact.PhoneNumber = "选择";
                        dliveryModel.Contact.AreaName = "";
                        dliveryModel.Contact.Address = "";
                        dliveryModel.Contact.IsDefault = false;
                    }
                    else
                    {
                        dliveryModel.Contact.Id = d_shippingAddress.Id;
                        dliveryModel.Contact.Consignee = d_shippingAddress.Consignee;
                        dliveryModel.Contact.PhoneNumber = d_shippingAddress.PhoneNumber;
                        dliveryModel.Contact.AreaName = d_shippingAddress.AreaName;
                        dliveryModel.Contact.Address = d_shippingAddress.Address;
                        dliveryModel.Contact.IsDefault = d_shippingAddress.IsDefault;

                        selfTakeModel.Contact.Id = d_shippingAddress.Id;
                        selfTakeModel.Contact.Consignee = d_shippingAddress.Consignee;
                        selfTakeModel.Contact.PhoneNumber = d_shippingAddress.PhoneNumber;
                        selfTakeModel.Contact.AreaName = d_shippingAddress.AreaName;
                        selfTakeModel.Contact.AreaCode = d_shippingAddress.AreaCode;
                        selfTakeModel.Contact.Address = d_shippingAddress.Address;
                    }
                }
                else
                {
                    dliveryModel.Contact.Id = delivery.Id;
                    dliveryModel.Contact.Consignee = delivery.Receiver;
                    dliveryModel.Contact.PhoneNumber = delivery.ReceiverPhoneNumber;
                    dliveryModel.Contact.AreaCode = delivery.ReceptionAreaCode;
                    dliveryModel.Contact.AreaName = delivery.ReceptionAreaName;
                    dliveryModel.Contact.Address = delivery.ReceptionAddress;
                    dliveryModel.Contact.IsDefault = false;

                    selfTakeModel.Contact.Id = delivery.Id;
                    selfTakeModel.Contact.Consignee = delivery.Receiver;
                    selfTakeModel.Contact.PhoneNumber = delivery.ReceiverPhoneNumber;
                    selfTakeModel.Contact.AreaName = delivery.ReceptionAreaName;
                    selfTakeModel.Contact.AreaCode = delivery.ReceptionAreaCode;
                    selfTakeModel.Contact.Address = delivery.ReceptionAddress;
                }

                if (selfTake == null)
                {

                    var selfPickAddress = (from s in CurrentDb.StoreShop
                                           join m in CurrentDb.Shop on s.ShopId equals m.Id into temp
                                           from u in temp.DefaultIfEmpty()
                                           where
                                     u.MerchId == store.MerchId
                                         && s.StoreId == store.StoreId
                                           select new { u.Id, u.Name, u.Address, u.MainImgUrl, u.IsOpen, u.AreaCode, u.AreaName, u.MerchId, s.StoreId, u.ContactName, u.ContactPhone, u.ContactAddress, u.CreateTime }).FirstOrDefault();


                    if (selfPickAddress == null)
                    {
                        selfTakeModel.Mark.Id = "";
                        selfTakeModel.Mark.Consignee = "自提地址";
                        selfTakeModel.Mark.PhoneNumber = "选择";
                        selfTakeModel.Mark.AreaName = "";
                        selfTakeModel.Mark.AreaCode = "";
                        selfTakeModel.Mark.Address = "";
                    }
                    else
                    {
                        selfTakeModel.Mark.Id = selfPickAddress.Id;
                        selfTakeModel.Mark.Name = selfPickAddress.Name;
                        selfTakeModel.Mark.Consignee = selfPickAddress.ContactName;
                        selfTakeModel.Mark.PhoneNumber = selfPickAddress.ContactPhone;
                        selfTakeModel.Mark.AreaName = selfPickAddress.AreaName;
                        selfTakeModel.Mark.AreaCode = selfPickAddress.AreaCode;
                        selfTakeModel.Mark.Address = selfPickAddress.Address;
                    }

                }
                else
                {
                    selfTakeModel.Contact.Id = selfTake.ReceptionId;
                    selfTakeModel.Contact.Consignee = selfTake.Receiver;
                    selfTakeModel.Contact.PhoneNumber = selfTake.ReceiverPhoneNumber;
                    selfTakeModel.Contact.AreaCode = selfTake.ReceptionAreaCode;
                    selfTakeModel.Contact.AreaName = selfTake.ReceptionAreaName;
                    selfTakeModel.Contact.Address = selfTake.ReceptionAddress;
                    selfTakeModel.Contact.IsDefault = false;

                    selfTakeModel.Mark.Id = selfTake.Id;
                    selfTakeModel.Mark.Name = selfTake.ReceptionMarkName;
                    selfTakeModel.Mark.Consignee = selfTake.Receiver;
                    selfTakeModel.Mark.PhoneNumber = selfTake.ReceiverPhoneNumber;
                    selfTakeModel.Mark.AreaName = selfTake.ReceptionAreaName;
                    selfTakeModel.Mark.Address = selfTake.ReceptionAddress;
                    selfTakeModel.Mark.AreaName = selfTake.ReceptionAreaName;
                    selfTakeModel.Mark.AreaCode = selfTake.ReceptionAreaCode;

                    if (selfTake.ReceptionBookStartTime != null)
                    {
                        selfTakeModel.BookTime.Type = "1";
                        selfTakeModel.BookTime.Date = selfTake.ReceptionBookStartTime.Value.ToString("yyy-MM-dd");
                        selfTakeModel.BookTime.Time = selfTake.ReceptionBookStartTime.Value.ToString("HH:mm");
                        selfTakeModel.BookTime.Week = Lumos.CommonUtil.GetCnWeekDayName(selfTake.ReceptionBookStartTime.Value);
                        selfTakeModel.BookTime.Text = string.Format("（{0}）{1}", selfTakeModel.BookTime.Week, selfTake.ReceptionBookStartTime.Value.ToString("yyy-MM-dd HH:mm"));
                        selfTakeModel.BookTime.Value = selfTake.ReceptionBookStartTime.Value.ToString("yyy-MM-dd HH:mm");
                    }
                }


                ret.ShopMethod = orders[0].ShopMethod;

                amount_couponByShop = orders.Sum(m => m.CouponAmountByShop);
                amount_couponByRent = orders.Sum(m => m.CouponAmountByRent);
                amount_couponByDeposit = orders.Sum(m => m.CouponAmountByDeposit);

                ret.CouponByShop = new RetOrderConfirm.CouponModel { TipMsg = amount_couponByShop == 0 ? "无优惠" : string.Format("-{0}", amount_couponByShop.ToF2Price()), TipType = TipType.InUse };
                ret.CouponByDeposit = new RetOrderConfirm.CouponModel { TipMsg = amount_couponByDeposit == 0 ? "无优惠" : string.Format("-{0}", amount_couponByDeposit.ToF2Price()), TipType = TipType.InUse };
                ret.CouponByRent = new RetOrderConfirm.CouponModel { TipMsg = amount_couponByRent == 0 ? "无优惠" : string.Format("-{0}", amount_couponByRent.ToF2Price()), TipType = TipType.InUse };

                amount_original = orders.Sum(m => m.OriginalAmount);


                amount_sale = orders.Sum(m => m.SaleAmount);



                amount_charge = orders.Sum(m => m.ChargeAmount);
            }


            var orderBlock = new List<RetOrderConfirm.BlockModel>();

            var skus_Mall = c_prodcutSkus.Where(m => m.ShopMode == E_ShopMode.Mall).ToList();

            if (skus_Mall.Count > 0)
            {
                var skus_ShopOrRent = skus_Mall.Where(m => m.ShopMethod == E_ShopMethod.Buy || m.ShopMethod == E_ShopMethod.Rent).ToList();

                if (skus_ShopOrRent.Count > 0)
                {
                    var skus_Delivery = skus_ShopOrRent.Where(m => m.SupReceiveMode == E_SupReceiveMode.Delivery).ToList();
                    if (skus_Delivery.Count > 0)
                    {
                        var ob_Delivery = new RetOrderConfirm.BlockModel();
                        ob_Delivery.TagName = "线上商城[配送]";
                        ob_Delivery.Skus = skus_Delivery;
                        ob_Delivery.TabMode = E_TabMode.Delivery;
                        ob_Delivery.ReceiveMode = E_ReceiveMode.Delivery;
                        ob_Delivery.Delivery = dliveryModel;
                        orderBlock.Add(ob_Delivery);

                    }

                    var skus_SelfTakeByStore = skus_ShopOrRent.Where(m => m.SupReceiveMode == E_SupReceiveMode.SelfTakeByStore).ToList();

                    if (skus_SelfTakeByStore.Count > 0)
                    {

                        var ob_SelfTakeByStore = new RetOrderConfirm.BlockModel();
                        ob_SelfTakeByStore.TagName = "线上商城[自提]";
                        ob_SelfTakeByStore.Skus = skus_SelfTakeByStore;
                        ob_SelfTakeByStore.TabMode = E_TabMode.SelfTakeByStore;
                        ob_SelfTakeByStore.ReceiveMode = E_ReceiveMode.SelfTakeByStore;
                        ob_SelfTakeByStore.SelfTake = selfTakeModel;
                        orderBlock.Add(ob_SelfTakeByStore);

                    }

                    var skus_DeliveryOrSelfTakeByStore = skus_ShopOrRent.Where(m => m.SupReceiveMode == E_SupReceiveMode.DeliveryOrSelfTakeByStore).ToList();

                    if (skus_DeliveryOrSelfTakeByStore.Count > 0)
                    {
                        var ob_DeliveryOrSelfTakeByStore = new RetOrderConfirm.BlockModel();
                        ob_DeliveryOrSelfTakeByStore.TagName = "线上商城[配送或自提]";
                        ob_DeliveryOrSelfTakeByStore.Skus = skus_DeliveryOrSelfTakeByStore;
                        ob_DeliveryOrSelfTakeByStore.TabMode = E_TabMode.DeliveryOrSelfTakeByStore;

                        if (rop.OrderIds == null || rop.OrderIds.Count == 0)
                        {
                            ob_DeliveryOrSelfTakeByStore.ReceiveMode = E_ReceiveMode.Delivery;
                        }
                        else
                        {
                            int skus_Delivery_Count = skus_DeliveryOrSelfTakeByStore.Where(m => m.ReceiveMode == E_ReceiveMode.Delivery).Count();
                            if (skus_Delivery_Count > 0)
                            {
                                ob_DeliveryOrSelfTakeByStore.ReceiveMode = E_ReceiveMode.Delivery;
                            }
                            else
                            {
                                ob_DeliveryOrSelfTakeByStore.ReceiveMode = E_ReceiveMode.SelfTakeByStore;
                            }
                        }

                        ob_DeliveryOrSelfTakeByStore.Delivery = dliveryModel;
                        ob_DeliveryOrSelfTakeByStore.SelfTake = selfTakeModel;
                        orderBlock.Add(ob_DeliveryOrSelfTakeByStore);

                    }

                }


                var skus_FeeByMember = skus_Mall.Where(m => m.ShopMethod == E_ShopMethod.MemberFee).ToList();

                if (skus_FeeByMember.Count > 0)
                {
                    var ob_MemberFee = new RetOrderConfirm.BlockModel();
                    ob_MemberFee.TagName = "会员费";
                    ob_MemberFee.Skus = skus_FeeByMember;
                    ob_MemberFee.TabMode = E_TabMode.FeeByMember;
                    ob_MemberFee.ReceiveMode = E_ReceiveMode.FeeByMember;
                    orderBlock.Add(ob_MemberFee);
                }

            }

            var skus_SelfTakeByMachines = (from u in c_prodcutSkus where u.ShopMode == E_ShopMode.Machine select new { u.ShopMode, u.ShopId }).Distinct().ToList();

            if (skus_SelfTakeByMachines.Count > 0)
            {
                foreach (var skus_SelfTakeByMachine in skus_SelfTakeByMachines)
                {
                    var shop = CurrentDb.Shop.Where(m => m.Id == skus_SelfTakeByMachine.ShopId).FirstOrDefault();

                    var l_skus = c_prodcutSkus.Where(m => m.ShopId == skus_SelfTakeByMachine.ShopId && m.ShopMode == E_ShopMode.Machine).ToList();

                    var ob_SelfTakeByMachine = new RetOrderConfirm.BlockModel();
                    ob_SelfTakeByMachine.TagName = "线下机器";
                    ob_SelfTakeByMachine.Skus = l_skus;
                    ob_SelfTakeByMachine.TabMode = E_TabMode.SelfTakeByMachine;
                    ob_SelfTakeByMachine.ReceiveMode = E_ReceiveMode.SelfTakeByMachine;
                    ob_SelfTakeByMachine.SelfTake.Mark.Id = shop.Id;
                    ob_SelfTakeByMachine.SelfTake.Mark.Name = shop.Name;
                    ob_SelfTakeByMachine.SelfTake.Mark.Address = shop.Address;
                    orderBlock.Add(ob_SelfTakeByMachine);
                }
            }

            ret.Blocks = orderBlock;


            c_subtotalItems.Add(new RetOrderConfirm.SubtotalItemModel { ImgUrl = "https://file.17fanju.com/Upload/Icon/icon_discountamont.png", Name = "商品总额", Amount = amount_original.ToF2Price() });
            c_subtotalItems.Add(new RetOrderConfirm.SubtotalItemModel { ImgUrl = "https://file.17fanju.com/Upload/Icon/icon_discountamont.png", Name = "商品优惠", Amount = "-" + (amount_original - amount_sale).ToF2Price() });

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
                    if (item.ReceiveMode == E_ReceiveMode.SelfTakeByMachine)
                    {
                        block.Tag.Desc = new FsField("取货码", "", item.PickupCode, "#f18d00");
                        //block.Qrcode = new FsQrcode { Code = MyDESCryptoUtil.BuildQrcode2PickupCode(item.PickupCode), Url = "", Remark = string.Format("扫码枪扫一扫", item.ma) };
                    }

                    if (item.ReceiveMode == E_ReceiveMode.Delivery || item.ReceiveMode == E_ReceiveMode.SelfTakeByMachine || item.ReceiveMode == E_ReceiveMode.SelfTakeByStore)
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
                        sku.OriginalAmount = orderSub.OriginalAmount.ToF2Price();
                        sku.SaleAmount = orderSub.SaleAmount.ToF2Price();
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
                        sku.OriginalAmount = orderSubChilds_Sku.Sum(m => m.OriginalAmount).ToF2Price();
                        sku.SaleAmount = orderSubChilds_Sku.Sum(m => m.SaleAmount).ToF2Price();
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

            if (order.PayedTime != null)
            {
                fsBlockByField.Data.Add(new FsField("商品总额", "", order.OriginalAmount.ToF2Price(), ""));
                fsBlockByField.Data.Add(new FsField("商品优惠", "", "-" + order.DiscountAmount.ToF2Price(), ""));

                if (order.CouponAmountByShop > 0)
                {
                    fsBlockByField.Data.Add(new FsField("优惠券", "", "-" + order.CouponAmountByShop.ToF2Price(), ""));
                }

                if (order.CouponAmountByDeposit > 0)
                {
                    fsBlockByField.Data.Add(new FsField("押金券", "", "-" + order.CouponAmountByDeposit.ToF2Price(), ""));
                }

                if (order.CouponAmountByRent > 0)
                {
                    fsBlockByField.Data.Add(new FsField("租金券", "", "-" + order.CouponAmountByRent.ToF2Price(), ""));
                }

                fsBlockByField.Data.Add(new FsField("实际支付", "", order.ChargeAmount.ToF2Price(), ""));

            }

            fsBlockByField.Data.Add(new FsField("下单时间", "", order.SubmittedTime.ToUnifiedFormatDateTime(), ""));

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
                    case E_ReceiveMode.SelfTakeByStore:
                        ret.Top.CircleText = "自";
                        ret.Top.Field1 = order.ReceptionMarkName;
                        ret.Top.Field2 = order.ReceptionAddress;
                        ret.Top.Field3 = string.Format("客服热线 {0}", merch.CsrPhoneNumber);
                        ret.RecordTop.CircleText = "自";
                        ret.RecordTop.Description = order.ReceptionAddress;
                        break;
                    case E_ReceiveMode.SelfTakeByMachine:
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
                dateArea.Week = Lumos.CommonUtil.GetCnWeekDayName(dateTime);
                dateArea.Date = dateTime.ToUnifiedFormatDate();
                dateArea.Value = dateTime.ToUnifiedFormatDate();
                dateArea.Status = 1;
                dateArea.Tip = "";

                DateTime statDate = DateTime.Parse(dateTime.ToString("yyy-MM-dd"));
                for (int z = 8; z < 19; z++)
                {
                    if (DateTime.Now < DateTime.Parse(statDate.AddHours(z).ToString("yyyy-MM-dd HH:00")))
                    {
                        string time1 = statDate.AddHours(z).ToString("HH:00");
                        dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = time1, Tip = "", Status = 1, Value = time1, Type = 1 });

                    }

                    if (DateTime.Now < DateTime.Parse(statDate.AddHours(z).ToString("yyyy-MM-dd HH:30")))
                    {
                        string time2 = statDate.AddHours(z).ToString("HH:30");
                        dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = time2, Tip = "", Status = 1, Value = time2, Type = 1 });
                    }
                }


                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "8:30", Tip = "", Status = 1, Value = "8:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "9:00", Tip = "", Status = 1, Value = "9:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "9:30", Tip = "", Status = 1, Value = "9:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "10:00", Tip = "", Status = 1, Value = "10:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "10:30", Tip = "", Status = 1, Value = "10:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "11:30", Tip = "", Status = 1, Value = "11:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "12:00", Tip = "", Status = 1, Value = "12:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "12:30", Tip = "", Status = 1, Value = "12:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "13:00", Tip = "", Status = 1, Value = "13:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "13:30", Tip = "", Status = 1, Value = "13:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "14:00", Tip = "", Status = 1, Value = "14:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "14:30", Tip = "", Status = 1, Value = "14:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "15:00", Tip = "", Status = 1, Value = "15:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "15:30", Tip = "", Status = 1, Value = "15:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "16:00", Tip = "", Status = 1, Value = "16:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "16:30", Tip = "", Status = 1, Value = "16:30", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "17:00", Tip = "", Status = 1, Value = "17:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "17:30", Tip = "", Status = 1, Value = "12:00", Type = 1 });
                //dateArea.TimeArea.Add(new BookTimeTimeAreaModel { Time = "18:00", Tip = "", Status = 1, Value = "17:30", Type = 1 });


                if (dateArea.TimeArea.Count > 0)
                {
                    ret.DateArea.Add(dateArea);
                }

            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

    }
}