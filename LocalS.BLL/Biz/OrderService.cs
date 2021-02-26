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
    public class OrderService : BaseService
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

        public StatusModel GetPayStatus(E_PayStatus payStatus)
        {
            var status = new StatusModel();

            switch (payStatus)
            {
                case E_PayStatus.WaitPay:
                    status.Value = 1;
                    status.Text = "待支付";
                    break;
                case E_PayStatus.Paying:
                    status.Value = 2;
                    status.Text = "支付中";
                    break;
                case E_PayStatus.PaySuccess:
                    status.Value = 3;
                    status.Text = "已支付";
                    break;
                case E_PayStatus.PayCancle:
                    status.Value = 4;
                    status.Text = "已取消";
                    break;
                case E_PayStatus.PayTimeout:
                    status.Value = 5;
                    status.Text = "已超时";
                    break;
                default:
                    status.Value = 0;
                    status.Text = "未知";
                    break;
            }
            return status;
        }

        public StatusModel GetPayWay(E_PayWay payWay)
        {
            var status = new StatusModel();

            switch (payWay)
            {
                case E_PayWay.Wx:
                    status.Value = 1;
                    status.Text = "微信支付";
                    break;
                case E_PayWay.Zfb:
                    status.Value = 2;
                    status.Text = "支付宝";
                    break;
                default:
                    status.Value = 0;
                    status.Text = "未知";
                    break;
            }
            return status;

        }

        public StatusModel GetPayPartner(E_PayPartner payPartner)
        {
            var status = new StatusModel();

            switch (payPartner)
            {
                case E_PayPartner.Wx:
                    status.Value = 1;
                    status.Text = "微信支付";
                    break;
                case E_PayPartner.Zfb:
                    status.Value = 2;
                    status.Text = "支付宝";
                    break;
                case E_PayPartner.Tg:
                    status.Value = 91;
                    status.Text = "通莞";
                    break;
                case E_PayPartner.Xrt:
                    status.Value = 92;
                    status.Text = "深银联";
                    break;
                default:
                    status.Value = 0;
                    status.Text = "未知";
                    break;
            }
            return status;

        }

        public StatusModel GetPickupTrgStatus(E_ReceiveMode receiveMode, bool pickupIsTrg)
        {
            var status = new StatusModel();

            if (pickupIsTrg)
            {
                status.Value = 1;
                status.Text = "已触发";
            }
            else
            {
                status.Value = 0;
                status.Text = "未触发";
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

        public decimal CalCouponAmount(decimal sum_amount, decimal atLeastAmount, E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, string storeId, string productId, int kindId3, decimal saleAmount)
        {
            LogUtil.Info("=>1");
            if (atLeastAmount > sum_amount)
            {
                return 0;
            }

            if (faceType != E_Coupon_FaceType.ShopDiscount)
            {
                if (sum_amount == 0)
                    return 0;
            }

            LogUtil.Info("=>2");
            decimal couponAmount = 0;
            List<UseAreaModel> arr_useArea = null;
            switch (useAreaType)
            {
                case E_Coupon_UseAreaType.All:

                    switch (faceType)
                    {
                        case E_Coupon_FaceType.ShopVoucher:
                        case E_Coupon_FaceType.DepositVoucher:
                        case E_Coupon_FaceType.RentVoucher:
                            couponAmount = (faceValue) * (saleAmount / sum_amount);
                            break;
                        case E_Coupon_FaceType.ShopDiscount:
                            couponAmount = (10 - faceValue) * 0.1m * saleAmount;
                            break;
                    }

                    LogUtil.Info("faceValue:" + faceValue);
                    LogUtil.Info("saleAmount:" + saleAmount);
                    LogUtil.Info("sum_amount:" + sum_amount);
                    LogUtil.Info("couponAmount:" + couponAmount);
                    return couponAmount;
                case E_Coupon_UseAreaType.Store:
                    LogUtil.Info("=>a2");
                    arr_useArea = useAreaValue.ToJsonObject<List<UseAreaModel>>();
                    if (arr_useArea != null)
                    {
                        var obj_useArea = arr_useArea.Where(m => m.Id == storeId).FirstOrDefault();
                        if (obj_useArea != null)
                        {
                            switch (faceType)
                            {
                                case E_Coupon_FaceType.ShopVoucher:
                                case E_Coupon_FaceType.DepositVoucher:
                                case E_Coupon_FaceType.RentVoucher:
                                    couponAmount = (faceValue) * (saleAmount / sum_amount);
                                    break;
                                case E_Coupon_FaceType.ShopDiscount:
                                    couponAmount = (10 - faceValue) * 0.1m * saleAmount;
                                    break;
                            }
                        }
                    }
                    LogUtil.Info("faceValue:" + faceValue);
                    LogUtil.Info("saleAmount:" + saleAmount);
                    LogUtil.Info("sum_amount:" + sum_amount);
                    LogUtil.Info("couponAmount:" + couponAmount);
                    return couponAmount;
                case E_Coupon_UseAreaType.ProductKind:
                    LogUtil.Info("=>3");
                    arr_useArea = useAreaValue.ToJsonObject<List<UseAreaModel>>();
                    if (arr_useArea != null)
                    {
                        LogUtil.Info("=>4");
                        string str_kindId3 = kindId3.ToString();
                        var obj_useArea = arr_useArea.Where(m => m.Id == str_kindId3).FirstOrDefault();
                        if (obj_useArea != null)
                            LogUtil.Info("=>5");
                        switch (faceType)
                        {
                            case E_Coupon_FaceType.ShopVoucher:
                            case E_Coupon_FaceType.DepositVoucher:
                            case E_Coupon_FaceType.RentVoucher:
                                couponAmount = (faceValue) * (saleAmount / sum_amount);
                                break;
                            case E_Coupon_FaceType.ShopDiscount:
                                couponAmount = (10 - faceValue) * 0.1m * saleAmount;
                                break;
                        }
                    }

                    return couponAmount;
                case E_Coupon_UseAreaType.ProductSpu:
                    LogUtil.Info("=>3");
                    arr_useArea = useAreaValue.ToJsonObject<List<UseAreaModel>>();
                    if (arr_useArea != null)
                    {
                        LogUtil.Info("=>4");
                        var obj_useArea = arr_useArea.Where(m => m.Id == productId).FirstOrDefault();
                        if (obj_useArea != null)
                        {
                            LogUtil.Info("=>5");
                            switch (faceType)
                            {
                                case E_Coupon_FaceType.ShopVoucher:
                                case E_Coupon_FaceType.DepositVoucher:
                                case E_Coupon_FaceType.RentVoucher:
                                    couponAmount = (faceValue) * (saleAmount / sum_amount);
                                    break;
                                case E_Coupon_FaceType.ShopDiscount:
                                    couponAmount = (10 - faceValue) * 0.1m * saleAmount;
                                    break;
                            }
                        }
                    }
                    return couponAmount;
            }

            return couponAmount;
        }


        private static readonly object lock_Reserve = new object();
        public CustomJsonResult<RetOrderReserve> Reserve(string operater, RopOrderReserve rop)
        {
            CustomJsonResult<RetOrderReserve> result = new CustomJsonResult<RetOrderReserve>();

            if (rop.Blocks == null || rop.Blocks.Count == 0)
            {
                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[01]，商品数据为空", null);
            }

            string trgerId = "";

            List<Order> s_Orders = new List<Order>();
            List<StockChangeRecordModel> s_StockChangeRecords = new List<StockChangeRecordModel>();
            lock (lock_Reserve)
            {

                var store = BizFactory.Store.GetOne(rop.StoreId);

                if (store == null)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[02]，无效店铺", null);
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    string clientUserName = "匿名";
                    string clientPhoneNumber = null;
                    int clientMemberLevel = 0;

                    var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == rop.ClientUserId).FirstOrDefault();

                    if (clientUser != null)
                    {
                        if (!string.IsNullOrEmpty(clientUser.NickName))
                        {
                            clientUserName = clientUser.NickName;
                        }

                        clientPhoneNumber = clientUser.PhoneNumber;
                        clientMemberLevel = clientUser.MemberLevel;
                    }

                    RetOrderReserve ret = new RetOrderReserve();

                    List<BuildSku> buildOrderSkus = new List<BuildSku>();

                    BuildOrderTool buildOrderTool = new BuildOrderTool(store.MerchId, store.StoreId, clientMemberLevel, rop.CouponIdsByShop);

                    foreach (var block in rop.Blocks)
                    {
                        foreach (var productSku in block.Skus)
                        {
                            buildOrderTool.AddSku(productSku.Id, productSku.Quantity, productSku.CartId, productSku.ShopMode, rop.ShopMethod, block.ReceiveMode, productSku.ShopId, productSku.MachineIds);
                        }
                    }

                    buildOrderSkus = buildOrderTool.BuildSkus();

                    if (!buildOrderTool.IsSuccess)
                    {
                        return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, buildOrderTool.Message, null);
                    }

                    if (rop.ShopMethod == E_ShopMethod.Buy)
                    {
                        #region 计算优惠券金额
                        if (rop.CouponIdsByShop != null && rop.CouponIdsByShop.Count > 0)
                        {
                            string couponIdByShop = rop.CouponIdsByShop[0];
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == couponIdByShop).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                if (d_clientCoupon.Status != E_ClientCouponStatus.WaitUse || d_clientCoupon.ValidStartTime > DateTime.Now || d_clientCoupon.ValidEndTime < DateTime.Now)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[03]，优惠券无效", null);
                                }

                                d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                                d_clientCoupon.Mender = operater;
                                d_clientCoupon.MendTime = DateTime.Now;


                                var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                if (d_coupon != null)
                                {
                                    if (d_coupon.FaceType == E_Coupon_FaceType.ShopDiscount || d_coupon.FaceType == E_Coupon_FaceType.ShopVoucher)
                                    {
                                        buildOrderTool.CalCouponAmount(d_coupon.AtLeastAmount, d_coupon.UseAreaType, d_coupon.UseAreaValue, d_coupon.FaceType, d_coupon.FaceValue);
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    else if (rop.ShopMethod == E_ShopMethod.Rent)
                    {
                        #region 计算押金卷，租金券金额

                        if (!string.IsNullOrEmpty(rop.CouponIdByDeposit))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == rop.CouponIdByDeposit).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                if (d_clientCoupon.Status != E_ClientCouponStatus.WaitUse || d_clientCoupon.ValidStartTime > DateTime.Now || d_clientCoupon.ValidEndTime < DateTime.Now)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[04]，押金券无效", null);
                                }

                                d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                                d_clientCoupon.Mender = operater;
                                d_clientCoupon.MendTime = DateTime.Now;


                                var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                if (d_coupon != null)
                                {
                                    if (d_coupon.FaceType == E_Coupon_FaceType.DepositVoucher)
                                    {
                                        buildOrderTool.CalCouponAmount(d_coupon.AtLeastAmount, d_coupon.UseAreaType, d_coupon.UseAreaValue, d_coupon.FaceType, d_coupon.FaceValue);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(rop.CouponIdByRent))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == rop.CouponIdByRent).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                if (d_clientCoupon.Status != E_ClientCouponStatus.WaitUse || d_clientCoupon.ValidStartTime > DateTime.Now || d_clientCoupon.ValidEndTime < DateTime.Now)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[05]，租金券无效", null);
                                }

                                d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                                d_clientCoupon.Mender = operater;
                                d_clientCoupon.MendTime = DateTime.Now;

                                var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                if (d_coupon != null)
                                {
                                    if (d_coupon.FaceType == E_Coupon_FaceType.RentVoucher)
                                    {
                                        buildOrderTool.CalCouponAmount(d_coupon.AtLeastAmount, d_coupon.UseAreaType, d_coupon.UseAreaValue, d_coupon.FaceType, d_coupon.FaceValue);
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    else if (rop.ShopMethod == E_ShopMethod.RentFee)
                    {
                        #region 计算租金券金额

                        if (!string.IsNullOrEmpty(rop.CouponIdByRent))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == rop.CouponIdByRent).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                if (d_clientCoupon.Status != E_ClientCouponStatus.WaitUse || d_clientCoupon.ValidStartTime > DateTime.Now || d_clientCoupon.ValidEndTime < DateTime.Now)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[05]，租金券无效", null);
                                }

                                d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                                d_clientCoupon.Mender = operater;
                                d_clientCoupon.MendTime = DateTime.Now;

                                var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                if (d_coupon != null)
                                {
                                    if (d_coupon.FaceType == E_Coupon_FaceType.RentVoucher)
                                    {
                                        buildOrderTool.CalCouponAmount(d_coupon.AtLeastAmount, d_coupon.UseAreaType, d_coupon.UseAreaValue, d_coupon.FaceType, d_coupon.FaceValue);
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    LogUtil.Info("rop.bizProductSkus:" + buildOrderSkus.ToJsonString());
                    var buildOrders = buildOrderTool.BuildOrders();
                    LogUtil.Info("SlotStock.buildOrders:" + buildOrders.ToJsonString());

                    #region 更改购物车标识

                    if (!string.IsNullOrEmpty(rop.ClientUserId))
                    {
                        var cartsIds = buildOrderSkus.Select(m => m.CartId).Distinct().ToArray();
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

                    #region 获取推荐用户ID
                    string reffUserId = null;
                    if (!string.IsNullOrEmpty(rop.ReffSign))
                    {
                        var reffUser = CurrentDb.SysClientUser.Where(m => m.WxMpOpenId == rop.ReffSign).FirstOrDefault();
                        if (reffUser != null)
                        {
                            if (reffUser.Id != rop.ClientUserId)
                            {
                                reffUserId = reffUser.Id;
                            }
                        }
                    }
                    #endregion

                    string unId = IdWorker.Build(IdType.NewGuid);


                    //指定开发者ID 为测试模式
                    if (rop.ClientUserId == "e3246aa715254ecf9a56916e889b928b")
                    {
                        rop.IsTestMode = true;
                    }

                    foreach (var buildOrder in buildOrders)
                    {
                        var d_Order = new Order();
                        d_Order.Id = IdWorker.Build(IdType.OrderId);
                        d_Order.UnId = unId;
                        d_Order.ClientUserId = rop.ClientUserId;
                        d_Order.ClientUserName = clientUserName;
                        d_Order.MerchId = store.MerchId;
                        d_Order.MerchName = store.MerchName;
                        d_Order.StoreId = rop.StoreId;
                        d_Order.StoreName = store.Name;
                        d_Order.ShopId = buildOrder.ShopId;

                        var shop = CurrentDb.Shop.Where(m => m.Id == buildOrder.ShopId).FirstOrDefault();
                        if (shop != null)
                        {
                            d_Order.ShopName = shop.Name;
                        }
                        d_Order.ShopMode = buildOrder.ShopMode;
                        d_Order.ShopId = buildOrder.ShopId;
                        d_Order.MachineId = buildOrder.MachineId;
                        d_Order.SaleOutletId = rop.SaleOutletId;
                        d_Order.PayExpireTime = DateTime.Now.AddSeconds(300);
                        d_Order.PickupCode = IdWorker.BuildPickupCode();
                        d_Order.PickupCodeExpireTime = DateTime.Now.AddDays(10);//todo 取货码10内有效
                        d_Order.SubmittedTime = DateTime.Now;
                        d_Order.CouponIdsByShop = rop.CouponIdsByShop.ToJsonString();
                        d_Order.CouponAmountByShop = buildOrder.CouponAmountByShop;
                        d_Order.CouponIdByRent = rop.CouponIdByRent;
                        d_Order.CouponAmountByRent = buildOrder.CouponAmountByRent;
                        d_Order.CouponIdByDeposit = rop.CouponIdByDeposit;
                        d_Order.CouponAmountByDeposit = buildOrder.CouponAmountByDeposit;
                        d_Order.ShopMethod = rop.ShopMethod;
                        d_Order.ReffSign = rop.ReffSign;
                        d_Order.ReffUserId = reffUserId;

                        switch (buildOrder.ReceiveMode)
                        {
                            case E_ReceiveMode.Delivery:
                                #region Delivery

                                var rm_Delivery = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.Delivery).FirstOrDefault();

                                if (rm_Delivery == null && rm_Delivery.Delivery == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[06]，配送地址为空", null);
                                }

                                d_Order.ReceiveMode = E_ReceiveMode.Delivery;
                                d_Order.ReceiveModeName = "配送到手";
                                d_Order.Receiver = rm_Delivery.Delivery.Contact.Consignee;
                                d_Order.ReceptionId = rm_Delivery.Delivery.Contact.Id;
                                d_Order.ReceiverPhoneNumber = rm_Delivery.Delivery.Contact.PhoneNumber;
                                d_Order.ReceptionAreaCode = rm_Delivery.Delivery.Contact.AreaCode;
                                d_Order.ReceptionAreaName = rm_Delivery.Delivery.Contact.AreaName;
                                d_Order.ReceptionAddress = rm_Delivery.Delivery.Contact.Address;
                                d_Order.ReceptionMarkName = rm_Delivery.Delivery.Contact.MarkName;
                                #endregion
                                break;
                            case E_ReceiveMode.SelfTakeByStore:
                                #region StoreSelfTake

                                var rm_StoreSelfTake = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.SelfTakeByStore).FirstOrDefault();

                                if (rm_StoreSelfTake == null || rm_StoreSelfTake.SelfTake == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[07]，自提地址为空", null);
                                }

                                d_Order.ReceiveMode = E_ReceiveMode.SelfTakeByStore;
                                d_Order.ReceiveModeName = "到店自提";
                                d_Order.Receiver = rm_StoreSelfTake.SelfTake.Contact.Consignee;
                                d_Order.ReceiverPhoneNumber = rm_StoreSelfTake.SelfTake.Contact.PhoneNumber;
                                d_Order.ReceptionId = rm_StoreSelfTake.SelfTake.Mark.Id;
                                d_Order.ReceptionAreaCode = rm_StoreSelfTake.SelfTake.Mark.AreaCode;
                                d_Order.ReceptionAreaName = rm_StoreSelfTake.SelfTake.Mark.AreaName;
                                d_Order.ReceptionAddress = rm_StoreSelfTake.SelfTake.Mark.Address;
                                d_Order.ReceptionMarkName = rm_StoreSelfTake.SelfTake.Mark.Name;


                                if (rm_StoreSelfTake.SelfTake.BookTime != null && !string.IsNullOrEmpty(rm_StoreSelfTake.SelfTake.BookTime.Value))
                                {
                                    //1为具体时间值 例如 2020-11-24 13:00
                                    //2为时间段区间 例如 2020-11-24 13:00,2020-11-24 13:00
                                    if (rm_StoreSelfTake.SelfTake.BookTime.Type == 1)
                                    {
                                        if (Lumos.CommonUtil.IsDateTime(rm_StoreSelfTake.SelfTake.BookTime.Value))
                                        {
                                            d_Order.ReceptionBookStartTime = DateTime.Parse(rm_StoreSelfTake.SelfTake.BookTime.Value);
                                        }
                                    }
                                    else if (rm_StoreSelfTake.SelfTake.BookTime.Type == 2)
                                    {
                                        string[] arr_time = rm_StoreSelfTake.SelfTake.BookTime.Value.Split(',');
                                        if (arr_time.Length == 2)
                                        {
                                            if (Lumos.CommonUtil.IsDateTime(arr_time[0]))
                                            {
                                                d_Order.ReceptionBookStartTime = DateTime.Parse(arr_time[0]);
                                            }

                                            if (Lumos.CommonUtil.IsDateTime(arr_time[1]))
                                            {
                                                d_Order.ReceptionBookEndTime = DateTime.Parse(arr_time[1]);
                                            }
                                        }

                                    }
                                }

                                #endregion
                                break;
                            case E_ReceiveMode.SelfTakeByMachine:
                                #region MachineSelfTake

                                if (d_Order.PickupCode == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[08]，取货码生成失败", null);
                                }

                                var rm_MachineSelfTake = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.SelfTakeByMachine).FirstOrDefault();

                                if (rm_MachineSelfTake == null || rm_MachineSelfTake.SelfTake == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[09]，自提地址为空", null);
                                }

                                d_Order.ReceiveMode = E_ReceiveMode.SelfTakeByMachine;
                                d_Order.ReceiveModeName = "机器自提";
                                d_Order.Receiver = rm_MachineSelfTake.SelfTake.Contact.Consignee;
                                d_Order.ReceiverPhoneNumber = rm_MachineSelfTake.SelfTake.Contact.PhoneNumber;
                                d_Order.ReceptionId = rm_MachineSelfTake.SelfTake.Mark.Id;
                                d_Order.ReceptionAreaCode = rm_MachineSelfTake.SelfTake.Mark.AreaCode;
                                d_Order.ReceptionAreaName = rm_MachineSelfTake.SelfTake.Mark.AreaName;
                                d_Order.ReceptionAddress = rm_MachineSelfTake.SelfTake.Mark.Address;
                                d_Order.ReceptionMarkName = rm_MachineSelfTake.SelfTake.Mark.Name;
                                #endregion
                                break;
                            case E_ReceiveMode.FeeByRent:
                                #region FeeByRent
                                d_Order.ReceiveMode = E_ReceiveMode.FeeByRent;
                                d_Order.ReceiveModeName = "租金费";
                                d_Order.Receiver = clientUserName;
                                d_Order.ReceiverPhoneNumber = clientPhoneNumber;
                                d_Order.IsNoDisplayClient = true;
                                #endregion
                                break;
                            case E_ReceiveMode.FeeByMember:
                                #region MemberFee
                                d_Order.ReceiveMode = E_ReceiveMode.FeeByMember;
                                d_Order.ReceiveModeName = "会员费";
                                d_Order.Receiver = clientUserName;
                                d_Order.ReceiverPhoneNumber = clientPhoneNumber;
                                d_Order.IsNoDisplayClient = true;
                                #endregion
                                break;

                        }

                        d_Order.SaleAmount = buildOrder.SaleAmount;
                        d_Order.OriginalAmount = buildOrder.OriginalAmount;
                        d_Order.DiscountAmount = buildOrder.DiscountAmount;
                        d_Order.ChargeAmount = buildOrder.ChargeAmount;
                        d_Order.Quantity = buildOrder.Quantity;
                        d_Order.PayStatus = E_PayStatus.WaitPay;
                        d_Order.Status = E_OrderStatus.WaitPay;
                        d_Order.Source = rop.Source;
                        d_Order.AppId = rop.AppId;
                        d_Order.IsTestMode = rop.IsTestMode;
                        d_Order.Creator = operater;
                        d_Order.CreateTime = DateTime.Now;
                        CurrentDb.Order.Add(d_Order);

                        s_Orders.Add(d_Order);

                        foreach (var buildOrderSub in buildOrder.Childs)
                        {
                            var productSku = buildOrderSkus.Where(m => m.Id == buildOrderSub.ProductSkuId).FirstOrDefault();

                            var d_OrderSub = new OrderSub();
                            d_OrderSub.Id = d_Order.Id + buildOrder.Childs.IndexOf(buildOrderSub).ToString();
                            d_OrderSub.ClientUserId = d_Order.ClientUserId;
                            d_OrderSub.ClientUserName = d_Order.ClientUserName;
                            d_OrderSub.MerchId = d_Order.MerchId;
                            d_OrderSub.MerchName = d_Order.MerchName;
                            d_OrderSub.StoreId = d_Order.StoreId;
                            d_OrderSub.StoreName = d_Order.StoreName;
                            d_OrderSub.ShopMode = d_Order.ShopMode;
                            d_OrderSub.ShopId = d_Order.ShopId;
                            d_OrderSub.ShopName = d_Order.ShopName;
                            d_OrderSub.MachineId = d_Order.MachineId;
                            d_OrderSub.ReceiveModeName = d_Order.ReceiveModeName;
                            d_OrderSub.ReceiveMode = d_Order.ReceiveMode;
                            d_OrderSub.CabinetId = buildOrderSub.CabinetId;
                            d_OrderSub.SlotId = buildOrderSub.SlotId;
                            d_OrderSub.OrderId = d_Order.Id;
                            d_OrderSub.PrdProductSkuId = buildOrderSub.ProductSkuId;
                            d_OrderSub.PrdProductId = productSku.ProductId;
                            d_OrderSub.PrdProductSkuName = productSku.Name;
                            d_OrderSub.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                            d_OrderSub.PrdProductSkuSpecDes = productSku.SpecDes;
                            d_OrderSub.PrdProductSkuProducer = productSku.Producer;
                            d_OrderSub.PrdProductSkuBarCode = productSku.BarCode;
                            d_OrderSub.PrdProductSkuCumCode = productSku.CumCode;
                            d_OrderSub.PrdKindId1 = productSku.KindId1;
                            d_OrderSub.PrdKindId2 = productSku.KindId2;
                            d_OrderSub.PrdKindId3 = productSku.KindId3;
                            d_OrderSub.Quantity = buildOrderSub.Quantity;
                            d_OrderSub.SalePrice = buildOrderSub.SalePrice;
                            d_OrderSub.OriginalPrice = buildOrderSub.OriginalPrice;
                            d_OrderSub.SaleAmount = buildOrderSub.SaleAmount;
                            d_OrderSub.OriginalAmount = buildOrderSub.OriginalAmount;
                            d_OrderSub.DiscountAmount = buildOrderSub.DiscountAmount;
                            d_OrderSub.ChargeAmount = buildOrderSub.ChargeAmount;
                            d_OrderSub.RentTermUnit = buildOrderSub.RentTermUnit;
                            d_OrderSub.RentTermValue = buildOrderSub.RentTermValue;
                            d_OrderSub.RentAmount = buildOrderSub.RentAmount;
                            d_OrderSub.DepositAmount = buildOrderSub.DepositAmount;
                            d_OrderSub.CouponAmountByDeposit = buildOrderSub.CouponAmountByDeposit;
                            d_OrderSub.CouponAmountByShop = buildOrderSub.CouponAmountByShop;
                            d_OrderSub.CouponAmountByRent = buildOrderSub.CouponAmountByRent;
                            d_OrderSub.PayStatus = E_PayStatus.WaitPay;
                            d_OrderSub.PickupStatus = E_OrderPickupStatus.WaitPay;
                            d_OrderSub.SvcConsulterId = productSku.SvcConsulterId;
                            d_OrderSub.SaleOutletId = d_Order.SaleOutletId;
                            d_OrderSub.IsTestMode = d_Order.IsTestMode;
                            d_OrderSub.Creator = d_Order.Creator;
                            d_OrderSub.CreateTime = d_Order.CreateTime;
                            d_OrderSub.ShopMethod = d_Order.ShopMethod;
                            d_OrderSub.ReffSign = d_Order.ReffSign;
                            d_OrderSub.ReffUserId = d_Order.ReffUserId;
                            CurrentDb.OrderSub.Add(d_OrderSub);


                            //购物或租赁进行库存操作
                            if (d_OrderSub.ShopMethod == E_ShopMethod.Buy || d_OrderSub.ShopMethod == E_ShopMethod.Rent)
                            {
                                var ret_OperateStock = BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.OrderReserveSuccess, d_Order.ShopMode, d_Order.MerchId, d_Order.StoreId, d_OrderSub.ShopId, d_OrderSub.MachineId, d_OrderSub.CabinetId, d_OrderSub.SlotId, d_OrderSub.PrdProductSkuId, d_OrderSub.Quantity);

                                if (ret_OperateStock.Result != ResultType.Success)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定失败[10]，库存扣减失败", null);
                                }

                                s_StockChangeRecords.AddRange(ret_OperateStock.Data.ChangeRecords);

                            }
                        }
                    }


                    CurrentDb.SaveChanges();
                    ts.Complete();


                    foreach (var s_Order in s_Orders)
                    {
                        ret.Orders.Add(new RetOrderReserve.Order { Id = s_Order.Id, ChargeAmount = s_Order.ChargeAmount.ToF2Price() });

                        Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckReservePay, s_Order.Id, s_Order.PayExpireTime.Value, new Order2CheckPayModel { Id = s_Order.Id, MerchId = s_Order.MerchId });

                    }

                    result = new CustomJsonResult<RetOrderReserve>(ResultType.Success, ResultCode.Success, "预定成功", ret);
                }
            }


            if (result.Result == ResultType.Success)
            {
                if (rop.AppId == AppId.STORETERM)
                {
                    trgerId = s_Orders[0].MachineId;
                }
                else if (rop.AppId == AppId.WXMINPRAGROM)
                {
                    trgerId = s_Orders[0].StoreId;
                }

                MqFactory.Global.PushOperateLog(operater, rop.AppId, trgerId, EventCode.OrderReserveSuccess, string.Format("订单号：{0}，预定成功", string.Join("", s_Orders.Select(m => m.Id).ToArray())), new { Rop = rop, StockChangeRecords = s_StockChangeRecords });
            }

            return result;

        }

        private static readonly object lock_PayResultNotify = new object();
        public CustomJsonResult PayTransResultNotify(string operater, E_PayPartner payPartner, E_PayTransLogNotifyFrom from, string content)
        {
            LogUtil.Info("PayTransResultNotify");
            lock (lock_PayResultNotify)
            {
                var payResult = new PayTransResult();

                if (payPartner == E_PayPartner.Wx)
                {
                    #region 解释微信支付协议
                    LogUtil.Info("解释微信支付协议");

                    if (from == E_PayTransLogNotifyFrom.Query)
                    {
                        payResult = SdkFactory.Wx.Convert2PayTransResultByQuery(null, content);
                    }
                    else if (from == E_PayTransLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.Wx.Convert2PayTransResultByNotifyUrl(null, content);
                    }
                    #endregion
                }
                else if (payPartner == E_PayPartner.Zfb)
                {
                    #region 解释支付宝支付协议
                    LogUtil.Info("解释支付宝支付协议");

                    if (from == E_PayTransLogNotifyFrom.Query)
                    {
                        payResult = SdkFactory.Zfb.Convert2PayTransResultByQuery(null, content);
                    }
                    else if (from == E_PayTransLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.Zfb.Convert2PayTransResultByNotifyUrl(null, content);
                    }

                    #endregion
                }
                else if (payPartner == E_PayPartner.Tg)
                {
                    #region 解释通莞支付协议

                    if (from == E_PayTransLogNotifyFrom.Query)
                    {
                        payResult = SdkFactory.TgPay.Convert2PayTransResultByQuery(null, content);
                    }

                    else if (from == E_PayTransLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.TgPay.Convert2PayTransResultByNotifyUrl(null, content);
                    }
                    #endregion
                }
                else if (payPartner == E_PayPartner.Xrt)
                {
                    #region 解释XRT支付协议
                    if (from == E_PayTransLogNotifyFrom.Query)
                    {
                        payResult = SdkFactory.XrtPay.Convert2PayTransResultByQuery(null, content);
                    }
                    else if (from == E_PayTransLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.XrtPay.Convert2PayTransResultByNotifyUrl(null, content);
                    }
                    #endregion
                }

                if (payResult.IsPaySuccess)
                {
                    LogUtil.Info("解释支付协议结果，支付成功");
                    Dictionary<string, string> pms = new Dictionary<string, string>();
                    pms.Add("clientUserName", payResult.ClientUserName);

                    PayTransSuccess(operater, payResult.PayTransId, payPartner, payResult.PayPartnerPayTransId, payResult.PayWay, DateTime.Now, pms);
                }


                var payNotifyLog = new PayNotifyLog();
                payNotifyLog.Id = IdWorker.Build(IdType.NewGuid);
                payNotifyLog.PayTransId = payResult.PayTransId;
                payNotifyLog.PayPartner = payPartner;
                payNotifyLog.PayPartnerPayTransId = payResult.PayPartnerPayTransId;
                payNotifyLog.NotifyContent = content;
                payNotifyLog.NotifyFrom = from;
                payNotifyLog.NotifyType = E_PayTransLogNotifyType.PayTrans;
                payNotifyLog.CreateTime = DateTime.Now;
                payNotifyLog.Creator = operater;
                CurrentDb.PayNotifyLog.Add(payNotifyLog);
                CurrentDb.SaveChanges();


            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
        public CustomJsonResult PayTransSuccess(string operater, string payTransId, E_PayPartner payPartner, string payPartnerPayTransId, E_PayWay payWay, DateTime completedTime, Dictionary<string, string> pms = null)
        {
            CustomJsonResult result = new CustomJsonResult();

            List<Order> s_Orders = new List<Order>();
            List<StockChangeRecordModel> s_StockChangeRecords = new List<StockChangeRecordModel>();
            using (TransactionScope ts = new TransactionScope())
            {
                LogUtil.Info("payTransId:" + payTransId);

                var d_PayTrans = CurrentDb.PayTrans.Where(m => m.Id == payTransId).FirstOrDefault();

                if (d_PayTrans == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", payTransId));
                }

                if (d_PayTrans.PayStatus == E_PayStatus.PaySuccess)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号({0})已经支付通知成功", payTransId));
                }

                if (d_PayTrans.PayStatus == E_PayStatus.WaitPay || d_PayTrans.PayStatus == E_PayStatus.Paying)
                {
                    LogUtil.Info("进入PaySuccess修改订单,开始");

                    operater = d_PayTrans.Creator;

                    d_PayTrans.PayPartner = payPartner;
                    d_PayTrans.PayPartnerPayTransId = payPartnerPayTransId;
                    d_PayTrans.PayWay = payWay;
                    d_PayTrans.PayStatus = E_PayStatus.PaySuccess;
                    d_PayTrans.PayedTime = DateTime.Now;

                    if (pms != null)
                    {
                        if (pms.ContainsKey("clientUserName"))
                        {
                            if (!string.IsNullOrEmpty(pms["clientUserName"]))
                            {
                                d_PayTrans.ClientUserName = pms["clientUserName"];
                            }
                        }
                    }

                    var d_clientUser = CurrentDb.SysClientUser.Where(m => m.Id == d_PayTrans.ClientUserId).FirstOrDefault();

                    var orderIds = d_PayTrans.OrderIds.Split(',');
                    var d_Orders = CurrentDb.Order.Where(m => orderIds.Contains(m.Id)).ToList();
                    foreach (var d_Order in d_Orders)
                    {
                        d_Order.ClientUserId = d_PayTrans.ClientUserId;
                        d_Order.ClientUserName = d_PayTrans.ClientUserName;
                        d_Order.PayedTime = d_PayTrans.PayedTime;
                        d_Order.PayStatus = d_PayTrans.PayStatus;
                        d_Order.PayWay = d_PayTrans.PayWay;
                        d_Order.PayPartner = payPartner;
                        d_Order.PayPartnerPayTransId = payPartnerPayTransId;
                        d_Order.MendTime = DateTime.Now;
                        d_Order.Mender = operater;

                        switch (d_Order.ReceiveMode)
                        {
                            case E_ReceiveMode.Delivery:
                                d_Order.Status = E_OrderStatus.Payed;
                                d_Order.PickupFlowLastDesc = "您已成功支付，等待发货";
                                d_Order.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.SelfTakeByStore:
                                d_Order.Status = E_OrderStatus.Payed;
                                d_Order.PickupFlowLastDesc = string.Format("您已成功支付，请到店铺【{0}】,出示取货码【{1}】，给店员", d_Order.ReceptionMarkName, d_Order.PickupCode);
                                d_Order.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.SelfTakeByMachine:
                                d_Order.Status = E_OrderStatus.Payed;
                                d_Order.PickupFlowLastDesc = string.Format("您已成功支付，请到店铺【{0}】找到机器【{1}】,在取货界面输入取货码【{2}】", d_Order.ReceptionMarkName, d_Order.MachineId, d_Order.PickupCode);
                                d_Order.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.FeeByMember:
                                d_Order.Status = E_OrderStatus.Completed;
                                d_Order.PickupFlowLastDesc = "您已成功支付";
                                d_Order.PickupFlowLastTime = DateTime.Now;
                                d_Order.IsNoDisplayClient = false;
                                break;
                        }

                        var d_OrderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_Order.Id).ToList();

                        foreach (var d_OrderSub in d_OrderSubs)
                        {
                            d_OrderSub.PayWay = d_Order.PayWay;
                            d_OrderSub.PayStatus = d_Order.PayStatus;
                            d_OrderSub.PayedTime = d_Order.PayedTime;
                            d_OrderSub.ClientUserId = d_Order.ClientUserId;
                            d_OrderSub.Mender = operater;
                            d_OrderSub.MendTime = DateTime.Now;

                            if (d_OrderSub.ShopMethod == E_ShopMethod.Buy)
                            {
                                #region Shop
                                d_OrderSub.PickupStatus = E_OrderPickupStatus.WaitPickup;
                                d_OrderSub.PickupFlowLastDesc = d_Order.PickupFlowLastDesc;
                                d_OrderSub.PickupFlowLastTime = d_Order.PickupFlowLastTime;
                                #endregion 
                            }
                            else if (d_OrderSub.ShopMethod == E_ShopMethod.Rent)
                            {
                                #region 
                                d_OrderSub.PickupStatus = E_OrderPickupStatus.WaitPickup;
                                d_OrderSub.PickupFlowLastDesc = d_Order.PickupFlowLastDesc;
                                d_OrderSub.PickupFlowLastTime = d_Order.PickupFlowLastTime;

                                var d_rentOrder = new RentOrder();
                                d_rentOrder.Id = IdWorker.Build(IdType.NewGuid);
                                d_rentOrder.MerchId = d_OrderSub.MerchId;
                                d_rentOrder.OrdeId = d_OrderSub.OrderId;
                                d_rentOrder.ClientUserId = d_OrderSub.ClientUserId;
                                d_rentOrder.SpuId = d_OrderSub.PrdProductId;
                                d_rentOrder.SkuId = d_OrderSub.PrdProductSkuId;
                                d_rentOrder.SkuName = d_OrderSub.PrdProductSkuName;
                                d_rentOrder.SkuCumCode = d_OrderSub.PrdProductSkuCumCode;
                                d_rentOrder.SkuBarCode = d_OrderSub.PrdProductSkuBarCode;
                                d_rentOrder.SkuSpecDes = d_OrderSub.PrdProductSkuSpecDes;
                                d_rentOrder.SkuProducer = d_OrderSub.PrdProductSkuProducer;
                                d_rentOrder.SkuMainImgUrl = d_OrderSub.PrdProductSkuMainImgUrl;
                                d_rentOrder.DepositAmount = d_OrderSub.ChargeAmount;
                                d_rentOrder.IsPayDeposit = true;
                                d_rentOrder.PayDepositTime = DateTime.Now;
                                d_rentOrder.RentTermUnit = d_OrderSub.RentTermUnit;
                                d_rentOrder.RentTermValue = d_OrderSub.RentTermValue;
                                d_rentOrder.RentTermUnitText = "月";
                                d_rentOrder.RentAmount = d_OrderSub.RentAmount;
                                d_rentOrder.NextPayRentTime = DateTime.Now.AddMonths(d_OrderSub.RentTermValue);
                                d_rentOrder.Creator = operater;
                                d_rentOrder.CreateTime = DateTime.Now;
                                CurrentDb.RentOrder.Add(d_rentOrder);

                                var d_rentOrderTransRecord = new RentOrderTransRecord();
                                d_rentOrderTransRecord.Id = IdWorker.Build(IdType.NewGuid);
                                d_rentOrderTransRecord.MerchId = d_OrderSub.MerchId;
                                d_rentOrderTransRecord.OrdeId = d_OrderSub.OrderId;
                                d_rentOrderTransRecord.RentOrderId = d_rentOrder.Id;
                                d_rentOrderTransRecord.ClientUserId = d_OrderSub.ClientUserId;
                                d_rentOrderTransRecord.TransType = E_RentTransTpye.Pay;
                                d_rentOrderTransRecord.Amount = d_OrderSub.ChargeAmount;
                                d_rentOrderTransRecord.TransTime = DateTime.Now;
                                d_rentOrderTransRecord.AmountType = E_RentAmountType.DepositAndRent;
                                d_rentOrderTransRecord.NextPayRentTime = DateTime.Now.AddMonths(d_OrderSub.RentTermValue);
                                d_rentOrderTransRecord.Creator = operater;
                                d_rentOrderTransRecord.CreateTime = DateTime.Now;
                                d_rentOrderTransRecord.Description = string.Format("您已支付设备押金和租金，合计：{0}", d_OrderSub.ChargeAmount);
                                CurrentDb.RentOrderTransRecord.Add(d_rentOrderTransRecord);

                                #endregion
                            }
                            else if(d_OrderSub.ShopMethod== E_ShopMethod.RentFee)
                            {
                                #region

                                //var d_rentOrder=CurrentDb.RentOrder.Where(m)


                                #endregion
                            }
                            else if (d_OrderSub.ShopMethod == E_ShopMethod.MemberFee)
                            {
                                #region MemberFee
                                d_OrderSub.PickupStatus = E_OrderPickupStatus.Taked;
                                d_OrderSub.PickupFlowLastDesc = d_Order.PickupFlowLastDesc;
                                d_OrderSub.PickupFlowLastTime = d_Order.PickupFlowLastTime;


                                var d_memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == d_OrderSub.MerchId && m.Id == d_OrderSub.PrdProductSkuId).FirstOrDefault();
                                if (d_memberFeeSt != null)
                                {
                                    var d_memberCouponSts = CurrentDb.MemberCouponSt.Where(m => m.MerchId == d_OrderSub.MerchId && m.LevelStId == d_memberFeeSt.LevelStId).ToList();

                                    foreach (var d_memberCouponSt in d_memberCouponSts)
                                    {
                                        var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_memberCouponSt.CouponId).FirstOrDefault();

                                        for (int i = 0; i < d_memberCouponSt.Quantity; i++)
                                        {
                                            var d_clientCoupon = new ClientCoupon();
                                            d_clientCoupon.Id = IdWorker.Build(IdType.NewGuid);
                                            d_clientCoupon.Sn = "";
                                            d_clientCoupon.MerchId = d_OrderSub.MerchId;
                                            d_clientCoupon.ClientUserId = d_OrderSub.ClientUserId;
                                            d_clientCoupon.CouponId = d_coupon.Id;
                                            if (d_coupon.UseTimeType == E_Coupon_UseTimeType.ValidDay)
                                            {
                                                d_clientCoupon.ValidStartTime = DateTime.Now;
                                                d_clientCoupon.ValidEndTime = DateTime.Now.AddDays(int.Parse(d_coupon.UseTimeValue));
                                            }
                                            else if (d_coupon.UseTimeType == E_Coupon_UseTimeType.TimeArea)
                                            {
                                                string[] arr_UseTimeValue = d_coupon.UseTimeValue.ToJsonObject<string[]>();
                                                if (arr_UseTimeValue.Length == 2)
                                                {
                                                    d_clientCoupon.ValidStartTime = DateTime.Parse(arr_UseTimeValue[0]);
                                                    d_clientCoupon.ValidEndTime = DateTime.Parse(arr_UseTimeValue[1]);
                                                }
                                            }

                                            d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                                            d_clientCoupon.SourceType = E_ClientCouponSourceType.SysGive;
                                            d_clientCoupon.SourceObjType = "System";
                                            d_clientCoupon.SourceObjId = IdWorker.Build(IdType.EmptyGuid);
                                            d_clientCoupon.SourcePoint = "PaySuccess";
                                            d_clientCoupon.SourceTime = DateTime.Now;
                                            d_clientCoupon.SourceDes = "开通会员赠送";
                                            d_clientCoupon.Creator = operater;
                                            d_clientCoupon.CreateTime = DateTime.Now;
                                            CurrentDb.ClientCoupon.Add(d_clientCoupon);

                                            d_coupon.ReceivedQuantity += 1;
                                            d_coupon.Mender = operater;
                                            d_coupon.MendTime = DateTime.Now;

                                            CurrentDb.SaveChanges();

                                        }

                                    }

                                    if (d_clientUser != null)
                                    {
                                        var memberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.Id == d_memberFeeSt.LevelStId).FirstOrDefault();
                                        if (memberLevelSt != null)
                                        {
                                            d_clientUser.MemberLevel = memberLevelSt.Level;

                                            switch (d_memberFeeSt.FeeType)
                                            {
                                                case E_MemberFeeSt_FeeType.TwelveMonth:
                                                    d_clientUser.MemberExpireTime = DateTime.Now.AddMonths(12);
                                                    break;
                                                case E_MemberFeeSt_FeeType.SixMonth:
                                                    d_clientUser.MemberExpireTime = DateTime.Now.AddMonths(6);
                                                    break;
                                                case E_MemberFeeSt_FeeType.ThreeMonth:
                                                    d_clientUser.MemberExpireTime = DateTime.Now.AddMonths(3);
                                                    break;
                                                case E_MemberFeeSt_FeeType.OneMonth:
                                                    d_clientUser.MemberExpireTime = DateTime.Now.AddMonths(1);
                                                    break;
                                                case E_MemberFeeSt_FeeType.LongTerm:
                                                    d_clientUser.MemberExpireTime = DateTime.Parse("2099-12-31");
                                                    break;
                                            }

                                            CurrentDb.SaveChanges();
                                        }
                                    }

                                }

                                #endregion
                            }

                            //购物和租赁进行库存操作
                            if (d_OrderSub.ShopMethod == E_ShopMethod.Buy || d_OrderSub.ShopMethod == E_ShopMethod.Rent)
                            {
                                var result_OperateStock = BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.OrderPaySuccess, d_Order.ShopMode, d_Order.MerchId, d_Order.StoreId, d_OrderSub.ShopId, d_OrderSub.MachineId, d_OrderSub.CabinetId, d_OrderSub.SlotId, d_OrderSub.PrdProductSkuId, d_OrderSub.Quantity);
                                if (result_OperateStock.Result != ResultType.Success)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扣减库存失败");
                                }

                                s_StockChangeRecords.AddRange(result_OperateStock.Data.ChangeRecords);
                            }

                            if (!string.IsNullOrEmpty(d_Order.ReffUserId))
                            {
                                var d_ClientReffSku = new ClientReffSku();
                                d_ClientReffSku.Id = IdWorker.Build(IdType.NewGuid);
                                d_ClientReffSku.MerchId = d_OrderSub.MerchId;
                                d_ClientReffSku.ClientUserId = d_OrderSub.ClientUserId;
                                d_ClientReffSku.OrderId = d_OrderSub.OrderId;
                                d_ClientReffSku.OrderSubId = d_OrderSub.Id;
                                d_ClientReffSku.SkuId = d_OrderSub.PrdProductSkuId;
                                d_ClientReffSku.SkuName = d_OrderSub.PrdProductSkuName;
                                d_ClientReffSku.SkuMainImgUrl = d_OrderSub.PrdProductSkuMainImgUrl;
                                d_ClientReffSku.SkuBarCode = d_OrderSub.PrdProductSkuBarCode;
                                d_ClientReffSku.SkuCumCode = d_OrderSub.PrdProductSkuCumCode;
                                d_ClientReffSku.SkuSpecDes = d_OrderSub.PrdProductSkuSpecDes;
                                d_ClientReffSku.SkuProducer = d_OrderSub.PrdProductSkuProducer;
                                d_ClientReffSku.Quantity = d_OrderSub.Quantity;
                                d_ClientReffSku.Creator = d_OrderSub.Creator;
                                d_ClientReffSku.CreateTime = d_OrderSub.CreateTime;
                                d_ClientReffSku.ReffClientUserId = d_OrderSub.ReffUserId;
                                d_ClientReffSku.Status = E_ClientReffSkuStatus.Valid;
                                CurrentDb.ClientReffSku.Add(d_ClientReffSku);
                                CurrentDb.SaveChanges();
                            }

                            s_Orders.Add(d_Order);
                        }

                        if (!string.IsNullOrEmpty(d_Order.CouponIdsByShop))
                        {
                            string[] l_couponIdsByShop = d_Order.CouponIdsByShop.ToJsonObject<string[]>();

                            foreach (var l_couponIdByShop in l_couponIdsByShop)
                            {
                                var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == l_couponIdByShop).FirstOrDefault();
                                if (d_clientCoupon != null)
                                {
                                    d_clientCoupon.Status = E_ClientCouponStatus.Used;
                                    d_clientCoupon.UseTime = DateTime.Now;
                                    d_clientCoupon.Mender = operater;
                                    d_clientCoupon.MendTime = DateTime.Now;

                                    var d_copon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                    if (d_copon != null)
                                    {
                                        d_copon.UsedQuantity += 1;
                                        d_copon.Mender = operater;
                                        d_copon.MendTime = DateTime.Now;
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(d_Order.CouponIdByRent))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_Order.CouponIdByRent).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                d_clientCoupon.Status = E_ClientCouponStatus.Used;
                                d_clientCoupon.UseTime = DateTime.Now;
                                d_clientCoupon.Mender = operater;
                                d_clientCoupon.MendTime = DateTime.Now;

                                var d_copon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                if (d_copon != null)
                                {
                                    d_copon.UsedQuantity += 1;
                                    d_copon.Mender = operater;
                                    d_copon.MendTime = DateTime.Now;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(d_Order.CouponIdByDeposit))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_Order.CouponIdByDeposit).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                d_clientCoupon.Status = E_ClientCouponStatus.Used;
                                d_clientCoupon.UseTime = DateTime.Now;
                                d_clientCoupon.Mender = operater;
                                d_clientCoupon.MendTime = DateTime.Now;

                                var d_copon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                if (d_copon != null)
                                {
                                    d_copon.UsedQuantity += 1;
                                    d_copon.Mender = operater;
                                    d_copon.MendTime = DateTime.Now;
                                }
                            }
                        }

                        var d_orderPickupLog = new OrderPickupLog();
                        d_orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                        d_orderPickupLog.OrderId = d_Order.Id;
                        d_orderPickupLog.ShopMode = d_Order.ShopMode;
                        d_orderPickupLog.MerchId = d_Order.MerchId;
                        d_orderPickupLog.StoreId = d_Order.StoreId;
                        d_orderPickupLog.ShopId = d_Order.ShopId;
                        d_orderPickupLog.MachineId = d_Order.MachineId;
                        d_orderPickupLog.UniqueId = d_Order.Id;
                        d_orderPickupLog.UniqueType = E_UniqueType.Order;
                        d_orderPickupLog.ActionRemark = d_Order.PickupFlowLastDesc;
                        d_orderPickupLog.ActionTime = d_Order.PickupFlowLastTime;
                        d_orderPickupLog.Remark = "";
                        d_orderPickupLog.CreateTime = DateTime.Now;
                        d_orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(d_orderPickupLog);
                        CurrentDb.SaveChanges();


                        var d_PayTransSub = new PayTransSub();
                        d_PayTransSub.Id = IdWorker.Build(IdType.NewGuid);
                        d_PayTransSub.PayTransId = d_PayTrans.Id;
                        d_PayTransSub.MerchId = d_PayTrans.MerchId;
                        d_PayTransSub.MerchName = d_Order.MerchName;
                        d_PayTransSub.StoreId = d_Order.StoreId;
                        d_PayTransSub.StoreName = d_Order.StoreName;
                        d_PayTransSub.ShopId = d_Order.ShopId;
                        d_PayTransSub.ShopName = d_Order.ShopName;
                        d_PayTransSub.OrderId = d_Order.Id;
                        d_PayTransSub.OriginalAmount = d_Order.OriginalAmount;
                        d_PayTransSub.DiscountAmount = d_Order.DiscountAmount;
                        d_PayTransSub.ChargeAmount = d_Order.ChargeAmount;
                        d_PayTransSub.Quantity = d_Order.Quantity;
                        d_PayTransSub.IsTestMode = d_Order.IsTestMode;
                        d_PayTransSub.AppId = d_Order.AppId;
                        d_PayTransSub.ClientUserId = d_Order.ClientUserId;
                        d_PayTransSub.ClientUserName = d_Order.ClientUserName;
                        d_PayTransSub.SubmittedTime = d_Order.SubmittedTime;
                        d_PayTransSub.Source = d_Order.Source;
                        d_PayTransSub.CreateTime = DateTime.Now;
                        d_PayTransSub.Creator = operater;
                        d_PayTransSub.PayCaller = d_PayTrans.PayCaller;
                        d_PayTransSub.PayPartner = d_PayTrans.PayPartner;
                        d_PayTransSub.PayWay = d_PayTrans.PayWay;
                        d_PayTransSub.PayStatus = d_PayTrans.PayStatus;
                        CurrentDb.PayTransSub.Add(d_PayTransSub);
                        CurrentDb.SaveChanges();

                    }


                    CurrentDb.SaveChanges();
                    ts.Complete();

                    LogUtil.Info("进入PaySuccess修改订单,结束");

                    Task4Factory.Tim2Global.Exit(Task4TimType.PayTrans2CheckStatus, d_PayTrans.Id);
                    Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckReservePay, d_Orders.Select(m => m.Id).ToArray());

                    string trgerId = "";
                    if (d_Orders[0].AppId == AppId.STORETERM)
                    {
                        trgerId = d_Orders[0].MachineId;
                    }
                    else if (d_Orders[0].AppId == AppId.WXMINPRAGROM)
                    {
                        trgerId = d_Orders[0].StoreId;
                    }

                    MqFactory.Global.PushOperateLog(operater, d_Orders[0].AppId, trgerId, EventCode.OrderPaySuccess, string.Format("订单号：{0}，支付成功", string.Join(",", d_Orders.Select(m => m.Id).ToArray())), new
                    {
                        Rop = new
                        {
                            payTransId = payTransId,
                            payPartner = payPartner,
                            payPartnerPayTransId = payPartnerPayTransId,
                            payWay = payWay,
                            completedTime = completedTime,
                            pms = pms
                        },
                        StockChangeRecords = s_StockChangeRecords
                    });
                }

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, string.Format("支付完成通知：交易号({0})通知成功", payTransId));
            }

            return result;
        }
        public CustomJsonResult<RetPayResultQuery> PayTransResultQuery(string operater, string payTransId)
        {
            var result = new CustomJsonResult<RetPayResultQuery>();

            var payTrans = CurrentDb.PayTrans.Where(m => m.Id == payTransId).FirstOrDefault();

            if (payTrans == null)
            {
                return new CustomJsonResult<RetPayResultQuery>(ResultType.Failure, ResultCode.Failure, "找不到订单", null);
            }

            var ret = new RetPayResultQuery();

            ret.PayTransId = payTrans.Id;
            ret.PayStatus = payTrans.PayStatus;
            ret.OrderIds = payTrans.OrderIds.Split(',').ToList();

            result = new CustomJsonResult<RetPayResultQuery>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
        public CustomJsonResult Cancle(string operater, string orderId, E_OrderCancleType cancleType, string cancelReason)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                List<StockChangeRecordModel> s_StockChangeRecords = new List<StockChangeRecordModel>();

                var d_Order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

                if (d_Order == null)
                {
                    LogUtil.Info(string.Format("该订单号:{0},找不到", orderId));
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该订单号:{0},找不到", orderId));
                }

                if (d_Order.PayStatus == E_PayStatus.PayCancle)
                {
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经取消");
                }

                if (d_Order.PayStatus == E_PayStatus.PayTimeout)
                {
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经超时");
                }

                if (d_Order.PayStatus == E_PayStatus.PaySuccess)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经支付成功");
                }


                operater = d_Order.Creator;

                if (d_Order.PayStatus != E_PayStatus.PaySuccess)
                {
                    d_Order.Status = E_OrderStatus.Canceled;
                    d_Order.CancelOperator = operater;
                    d_Order.CanceledTime = DateTime.Now;
                    d_Order.CancelReason = cancelReason;
                    d_Order.Mender = operater;
                    d_Order.MendTime = DateTime.Now;

                    if (cancleType == E_OrderCancleType.PayCancle)
                    {
                        d_Order.PayStatus = E_PayStatus.PayCancle;
                    }
                    else if (cancleType == E_OrderCancleType.PayTimeout)
                    {
                        d_Order.PayStatus = E_PayStatus.PayTimeout;
                    }

                    var d_OrderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_Order.Id).ToList();

                    foreach (var d_OrderSub in d_OrderSubs)
                    {

                        d_OrderSub.Mender = operater;
                        d_OrderSub.MendTime = DateTime.Now;

                        if (cancleType == E_OrderCancleType.PayCancle)
                        {
                            d_OrderSub.PayStatus = E_PayStatus.PayCancle;
                        }
                        else if (cancleType == E_OrderCancleType.PayTimeout)
                        {
                            d_OrderSub.PayStatus = E_PayStatus.PayTimeout;
                        }

                        d_OrderSub.PickupStatus = E_OrderPickupStatus.Canceled;

                        //购物货租赁进行库存操作
                        if (d_OrderSub.ShopMethod == E_ShopMethod.Buy || d_OrderSub.ShopMethod == E_ShopMethod.Rent)
                        {
                            var result_OperateStock = BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.OrderCancle, d_Order.ShopMode, d_Order.MerchId, d_Order.StoreId, d_OrderSub.ShopId, d_OrderSub.MachineId, d_OrderSub.CabinetId, d_OrderSub.SlotId, d_OrderSub.PrdProductSkuId, d_OrderSub.Quantity);
                            if (result_OperateStock.Result != ResultType.Success)
                            {
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扣减库存失败");
                            }
                            s_StockChangeRecords.AddRange(result_OperateStock.Data.ChangeRecords);
                        }

                    }

                    CurrentDb.SaveChanges();

                    var d_UnOrders = CurrentDb.Order.Where(m => m.UnId == d_Order.UnId && m.Status != E_OrderStatus.Canceled).ToList();
                    if (d_UnOrders.Count == 0)
                    {
                        if (!string.IsNullOrEmpty(d_Order.CouponIdsByShop))
                        {
                            string[] l_couponIdsByShops = d_Order.CouponIdsByShop.ToJsonObject<string[]>();

                            foreach (var l_couponIdByShop in l_couponIdsByShops)
                            {
                                var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == l_couponIdByShop).FirstOrDefault();
                                if (d_clientCoupon != null)
                                {
                                    d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(d_Order.CouponIdByRent))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_Order.CouponIdByRent).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                            }
                        }

                        if (!string.IsNullOrEmpty(d_Order.CouponIdByDeposit))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_Order.CouponIdByDeposit).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                            }
                        }
                    }



                    CurrentDb.SaveChanges();

                    ts.Complete();

                    Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckReservePay, d_Order.Id);
                    Task4Factory.Tim2Global.Exit(Task4TimType.PayTrans2CheckStatus, d_Order.PayTransId);

                    string trgerId = "";
                    if (d_Order.AppId == AppId.STORETERM)
                    {
                        trgerId = d_Order.MachineId;
                    }
                    else if (d_Order.AppId == AppId.WXMINPRAGROM)
                    {
                        trgerId = d_Order.StoreId;
                    }

                    MqFactory.Global.PushOperateLog(operater, d_Order.AppId, trgerId, EventCode.OrderCancle, string.Format("订单号：{0}，取消成功", d_Order.Id), new
                    {
                        Rop = new
                        {
                            orderId = orderId,
                            cancleType = cancleType,
                            cancelReason = cancelReason
                        },
                        StockChangeRecords = s_StockChangeRecords
                    });


                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "已取消");
                }
            }


            return result;
        }
        public CustomJsonResult BuildPayParams(string operater, RopOrderBuildPayParams rop)
        {
            var result = new CustomJsonResult();

            string payTransId = IdWorker.Build(IdType.PayTransId);
            using (TransactionScope ts = new TransactionScope())
            {
                var orders = new List<Order>();

                if (rop.OrderIds == null || rop.OrderIds.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付的订单数据不能为空");
                }

                var payTrans = new PayTrans();
                payTrans.Id = payTransId;


                foreach (var orderId in rop.OrderIds)
                {

                    var d_order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

                    if (d_order == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号：{0}，单号不存在", orderId));
                    }

                    if (d_order.PayStatus == E_PayStatus.PayCancle)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号：{0}，已被取消，请重新下单", orderId));
                    }

                    if (d_order.PayStatus == E_PayStatus.PayTimeout)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号：{0}，该订单已超时，请重新下单", orderId));
                    }

                    d_order.PayTransId = payTrans.Id;

                    if (rop.Blocks != null)
                    {
                        if (rop.Blocks.Count > 0)
                        {
                            var rm_Delivery = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.Delivery).FirstOrDefault();
                            if (rm_Delivery != null)
                            {
                                d_order.ReceiveModeName = "配送到手";
                                d_order.ReceiveMode = E_ReceiveMode.Delivery;
                                d_order.Receiver = rm_Delivery.Delivery.Contact.Consignee;
                                d_order.ReceiverPhoneNumber = rm_Delivery.Delivery.Contact.PhoneNumber;
                                d_order.ReceptionId = rm_Delivery.Delivery.Contact.Id;
                                d_order.ReceptionAreaCode = rm_Delivery.Delivery.Contact.AreaCode;
                                d_order.ReceptionAreaName = rm_Delivery.Delivery.Contact.AreaName;
                                d_order.ReceptionAddress = rm_Delivery.Delivery.Contact.Address;
                            }

                            var rm_StoreSelfTake = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.SelfTakeByStore).FirstOrDefault();
                            if (rm_StoreSelfTake != null)
                            {
                                d_order.ReceiveMode = E_ReceiveMode.SelfTakeByStore;
                                d_order.Receiver = rm_StoreSelfTake.SelfTake.Contact.Consignee;
                                d_order.ReceiverPhoneNumber = rm_StoreSelfTake.SelfTake.Contact.PhoneNumber;
                                d_order.ReceptionAreaCode = rm_StoreSelfTake.SelfTake.Mark.AreaCode;
                                d_order.ReceptionAreaName = rm_StoreSelfTake.SelfTake.Mark.AreaName;
                                d_order.ReceptionAddress = rm_StoreSelfTake.SelfTake.Mark.Address;
                                d_order.ReceptionMarkName = rm_StoreSelfTake.SelfTake.Mark.Name;


                                if (rm_StoreSelfTake.SelfTake.BookTime != null && !string.IsNullOrEmpty(rm_StoreSelfTake.SelfTake.BookTime.Value))
                                {
                                    //1为具体时间值 例如 2020-11-24 13:00
                                    //2为时间段区间 例如 2020-11-24 13:00,2020-11-24 13:00
                                    if (rm_StoreSelfTake.SelfTake.BookTime.Type == 1)
                                    {
                                        if (Lumos.CommonUtil.IsDateTime(rm_StoreSelfTake.SelfTake.BookTime.Value))
                                        {
                                            d_order.ReceptionBookStartTime = DateTime.Parse(rm_StoreSelfTake.SelfTake.BookTime.Value);
                                        }
                                    }
                                    else if (rm_StoreSelfTake.SelfTake.BookTime.Type == 2)
                                    {
                                        string[] arr_time = rm_StoreSelfTake.SelfTake.BookTime.Value.Split(',');
                                        if (arr_time.Length == 2)
                                        {
                                            if (Lumos.CommonUtil.IsDateTime(arr_time[0]))
                                            {
                                                d_order.ReceptionBookStartTime = DateTime.Parse(arr_time[0]);
                                            }

                                            if (Lumos.CommonUtil.IsDateTime(arr_time[1]))
                                            {
                                                d_order.ReceptionBookEndTime = DateTime.Parse(arr_time[1]);
                                            }
                                        }

                                    }
                                }
                            }

                            var d_orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_order.Id).ToList();

                            foreach (var d_orderSub in d_orderSubs)
                            {
                                d_orderSub.ReceiveMode = d_order.ReceiveMode;
                                d_orderSub.ReceiveModeName = d_order.ReceiveModeName;
                            }
                        }
                    }

                    CurrentDb.SaveChanges();

                    orders.Add(d_order);
                }

                if (orders.Count != rop.OrderIds.Count)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付订单号与后台数据不对应");
                }

                payTrans.MerchId = orders[0].MerchId;
                payTrans.MerchName = orders[0].MerchName;
                payTrans.StoreId = orders[0].StoreId;
                payTrans.StoreName = orders[0].StoreName;
                payTrans.ShopIds = string.Join(",", orders.Select(m => m.ShopId));
                payTrans.ShopNames = string.Join(",", orders.Select(m => m.ShopName));
                payTrans.OrderIds = string.Join(",", orders.Select(m => m.Id));
                payTrans.OriginalAmount = orders.Sum(m => m.OriginalAmount);
                payTrans.DiscountAmount = orders.Sum(m => m.DiscountAmount);
                payTrans.ChargeAmount = orders.Sum(m => m.ChargeAmount);
                payTrans.Quantity = orders.Sum(m => m.Quantity);
                payTrans.IsTestMode = orders[0].IsTestMode;
                payTrans.AppId = orders[0].AppId;
                payTrans.ClientUserId = orders[0].ClientUserId;
                payTrans.ClientUserName = orders[0].ClientUserName;
                payTrans.SubmittedTime = DateTime.Now;
                payTrans.Source = orders[0].Source;
                payTrans.CreateTime = DateTime.Now;
                payTrans.Creator = operater;
                payTrans.PayExpireTime = orders[0].PayExpireTime;
                payTrans.PayCaller = rop.PayCaller;

                if (payTrans.IsTestMode)
                {
                    if (payTrans.ChargeAmount > 0)
                    {
                        payTrans.ChargeAmount = 0.01m;
                    }
                }

                if (payTrans.ChargeAmount > 0)
                {
                    // var orderAttach = new BLL.Biz.OrderAttachModel();

                    var d_clientUser = CurrentDb.SysClientUser.Where(m => m.Id == payTrans.ClientUserId).FirstOrDefault();

                    switch (rop.PayPartner)
                    {
                        case E_PayPartner.Wx:
                            #region  微信商户支付
                            switch (rop.PayCaller)
                            {
                                case E_PayCaller.WxByNt:
                                    #region WxByNt
                                    payTrans.PayPartner = E_PayPartner.Wx;
                                    payTrans.PayWay = E_PayWay.Wx;
                                    payTrans.PayStatus = E_PayStatus.Paying;
                                    var wxByNt_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(payTrans.MerchId);
                                    var wx_PayBuildQrCode = SdkFactory.Wx.PayBuildQrCode(wxByNt_AppInfoConfig, E_PayCaller.WxByNt, payTrans.MerchId, payTrans.StoreId, "", payTrans.Id, payTrans.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", payTrans.PayExpireTime.Value);
                                    if (string.IsNullOrEmpty(wx_PayBuildQrCode.CodeUrl))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                    }

                                    var wxByNt_PayParams = new { PayTransId = payTrans.Id, ParamType = "url", ParamData = wx_PayBuildQrCode.CodeUrl, ChargeAmount = payTrans.ChargeAmount.ToF2Price() };

                                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wxByNt_PayParams);
                                    #endregion
                                    break;
                                case E_PayCaller.WxByMp:
                                    #region WxByMp
                                    payTrans.PayPartner = E_PayPartner.Wx;
                                    payTrans.PayWay = E_PayWay.Wx;
                                    payTrans.PayStatus = E_PayStatus.Paying;

                                    if (d_clientUser == null)
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户数据");
                                    }

                                    var wxByMp_AppInfoConfig = BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(payTrans.MerchId);

                                    if (wxByMp_AppInfoConfig == null)
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户信息认证失败");
                                    }

                                    //orderAttach.MerchId = payTrans.MerchId;
                                    //orderAttach.StoreId = payTrans.StoreId;
                                    //orderAttach.PayCaller = rop.PayCaller;

                                    var wxByMp_PayBuildWxJsPayInfo = SdkFactory.Wx.PayBuildWxJsPayInfo(wxByMp_AppInfoConfig, payTrans.MerchId, "", "", d_clientUser.WxMpOpenId, payTrans.Id, payTrans.ChargeAmount, "", rop.CreateIp, "自助商品", payTrans.PayExpireTime.Value);

                                    if (string.IsNullOrEmpty(wxByMp_PayBuildWxJsPayInfo.Package))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付参数生成失败");
                                    }

                                    wxByMp_PayBuildWxJsPayInfo.PayTransId = payTrans.Id;

                                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wxByMp_PayBuildWxJsPayInfo);
                                    #endregion
                                    break;
                                default:
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);

                            }
                            #endregion
                            break;
                        case E_PayPartner.Zfb:
                            #region  支付宝商户支付
                            switch (rop.PayCaller)
                            {
                                case E_PayCaller.ZfbByNt:
                                    #region ZfbByNt
                                    payTrans.PayPartner = E_PayPartner.Zfb;
                                    payTrans.PayWay = E_PayWay.Zfb;
                                    payTrans.PayStatus = E_PayStatus.Paying;
                                    var zfbByNt_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetZfbMpAppInfoConfig(payTrans.MerchId);
                                    var zfbByNt_PayBuildQrCode = SdkFactory.Zfb.PayBuildQrCode(zfbByNt_AppInfoConfig, E_PayCaller.ZfbByNt, payTrans.MerchId, "", "", payTrans.Id, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", payTrans.PayExpireTime.Value);
                                    if (string.IsNullOrEmpty(zfbByNt_PayBuildQrCode.CodeUrl))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                    }

                                    var zfbByNt_PayParams = new { PayTransId = payTrans.Id, ParamType = "url", ParamData = zfbByNt_PayBuildQrCode.CodeUrl, ChargeAmount = payTrans.ChargeAmount.ToF2Price() };

                                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", zfbByNt_PayParams);
                                    #endregion
                                    break;
                                default:
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                            }
                            #endregion
                            break;
                        case E_PayPartner.Tg:
                            #region 通莞商户支付

                            var tgPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetTgPayInfoConfg(payTrans.MerchId);

                            payTrans.PayPartner = E_PayPartner.Tg;
                            payTrans.PayStatus = E_PayStatus.Paying;

                            switch (rop.PayCaller)
                            {
                                case E_PayCaller.AggregatePayByNt:
                                    #region AggregatePayByNt

                                    var tgPay_AllQrcodePay = SdkFactory.TgPay.PayBuildQrCode(tgPayInfoConfig, rop.PayCaller, payTrans.MerchId, "", "", payTrans.Id, payTrans.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", payTrans.PayExpireTime.Value);
                                    if (string.IsNullOrEmpty(tgPay_AllQrcodePay.CodeUrl))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                    }

                                    var tg_AllQrcodePay_PayParams = new { PayTransId = payTrans.Id, ParamType = "url", ParamData = tgPay_AllQrcodePay.CodeUrl, ChargeAmount = payTrans.ChargeAmount.ToF2Price() };

                                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", tg_AllQrcodePay_PayParams);

                                    #endregion
                                    break;
                                default:
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                            }
                            #endregion
                            break;
                        case E_PayPartner.Xrt:
                            #region Xrt支付

                            // todo 发布去掉

                            var xrtPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetXrtPayInfoConfg(payTrans.MerchId);

                            payTrans.PayPartner = E_PayPartner.Xrt;
                            payTrans.PayStatus = E_PayStatus.Paying;

                            switch (rop.PayCaller)
                            {
                                case E_PayCaller.WxByNt:
                                    #region WxByNt

                                    var xrtPay_WxPayBuildByNtResult = SdkFactory.XrtPay.PayBuildQrCode(xrtPayInfoConfig, rop.PayCaller, payTrans.MerchId, "", "", payTrans.Id, payTrans.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", payTrans.PayExpireTime.Value);

                                    if (string.IsNullOrEmpty(xrtPay_WxPayBuildByNtResult.CodeUrl))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                    }

                                    var xrtPay_WxPayBuildByNtResultParams = new { PayTransId = payTrans.Id, ParamType = "url", ParamData = xrtPay_WxPayBuildByNtResult.CodeUrl, ChargeAmount = payTrans.ChargeAmount.ToF2Price() };

                                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", xrtPay_WxPayBuildByNtResultParams);

                                    #endregion
                                    break;
                                case E_PayCaller.WxByMp:
                                    #region WxByMp

                                    payTrans.PayPartner = E_PayPartner.Wx;
                                    payTrans.PayWay = E_PayWay.Wx;
                                    payTrans.PayStatus = E_PayStatus.Paying;

                                    if (d_clientUser == null)
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户数据");
                                    }

                                    // orderAttach.MerchId = payTrans.MerchId;
                                    //orderAttach.StoreId = payTrans.StoreId;
                                    // orderAttach.PayCaller = rop.PayCaller;

                                    var wxByMp_PayBuildWxJsPayInfo = SdkFactory.XrtPay.PayBuildWxJsPayInfo(xrtPayInfoConfig, payTrans.MerchId, "", "", d_clientUser.WxMpAppId, d_clientUser.WxMpOpenId, payTrans.Id, payTrans.ChargeAmount, "", rop.CreateIp, "自助商品", payTrans.PayExpireTime.Value);

                                    if (string.IsNullOrEmpty(wxByMp_PayBuildWxJsPayInfo.Package))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付参数生成失败");
                                    }

                                    wxByMp_PayBuildWxJsPayInfo.PayTransId = payTrans.Id;


                                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wxByMp_PayBuildWxJsPayInfo);

                                    #endregion
                                    break;
                                case E_PayCaller.ZfbByNt:
                                    #region ZfbByNt

                                    var xrtPay_ZfbByNtBuildByNtResult = SdkFactory.XrtPay.PayBuildQrCode(xrtPayInfoConfig, rop.PayCaller, payTrans.MerchId, "", "", payTrans.Id, payTrans.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", payTrans.PayExpireTime.Value);

                                    if (string.IsNullOrEmpty(xrtPay_ZfbByNtBuildByNtResult.CodeUrl))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                    }

                                    var xrtPay_ZfbByNtBuildByNtResultParams = new { OrderId = payTrans.OrderIds, PayTransId = payTrans.Id, ParamType = "url", ParamData = xrtPay_ZfbByNtBuildByNtResult.CodeUrl, ChargeAmount = payTrans.ChargeAmount.ToF2Price() };

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
                }
                else
                {
                    payTrans.PayPartner = E_PayPartner.MyAccount;
                    payTrans.PayWay = E_PayWay.MyAccount;
                    payTrans.PayStatus = E_PayStatus.Paying;

                    result = new CustomJsonResult(ResultType.Success, "1040", "操作成功", new { payTransId = payTransId });
                }

                CurrentDb.PayTrans.Add(payTrans);
                CurrentDb.SaveChanges();

                ts.Complete();

                if (payTrans.ChargeAmount > 0)
                {
                    Task4Factory.Tim2Global.Enter(Task4TimType.PayTrans2CheckStatus, payTrans.Id, payTrans.PayExpireTime.Value, new PayTrans2CheckStatusModel { Id = payTrans.Id, MerchId = payTrans.MerchId, PayCaller = payTrans.PayCaller, PayPartner = payTrans.PayPartner });
                }

            }

            if (result.Code == "1040")
            {
                PayTransSuccess(operater, payTransId, E_PayPartner.MyAccount, "", E_PayWay.MyAccount, DateTime.Now);
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
                                    case E_PayCaller.WxByMp:
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
            var models = new List<OrderProductSkuByPickupModel>();

            var order = CurrentDb.Order.Where(m => m.Id == orderId && m.MachineId == machineId).FirstOrDefault();
            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == orderId && m.MachineId == machineId).ToList();

            LogUtil.Info("orderId:" + orderId);
            LogUtil.Info("machineId:" + machineId);
            LogUtil.Info("orderSubs.Count:" + orderSubs.Count);


            var productSkuIds = orderSubs.Select(m => m.PrdProductSkuId).Distinct().ToArray();

            foreach (var productSkuId in productSkuIds)
            {
                var orderSubs_Sku = orderSubs.Where(m => m.PrdProductSkuId == productSkuId).ToList();

                var model = new OrderProductSkuByPickupModel();
                model.ProductSkuId = productSkuId;
                model.Name = orderSubs_Sku[0].PrdProductSkuName;
                model.MainImgUrl = orderSubs_Sku[0].PrdProductSkuMainImgUrl;
                model.Quantity = orderSubs_Sku.Sum(m => m.Quantity);
                model.QuantityBySuccess = orderSubs_Sku.Where(m => m.PickupStatus == E_OrderPickupStatus.Taked || m.PickupStatus == E_OrderPickupStatus.ExPickupSignTaked).Count();

                foreach (var orderSubs_SkuSlot in orderSubs_Sku)
                {
                    var slot = new OrderProductSkuByPickupModel.Slot();
                    slot.UniqueId = orderSubs_SkuSlot.Id;
                    slot.CabinetId = orderSubs_SkuSlot.CabinetId;
                    slot.SlotId = orderSubs_SkuSlot.SlotId;
                    slot.Status = orderSubs_SkuSlot.PickupStatus;

                    if (orderSubs_SkuSlot.PayStatus == E_PayStatus.PaySuccess)
                    {
                        if (!order.PickupIsTrg)
                        {
                            if (orderSubs_SkuSlot.PickupStatus == E_OrderPickupStatus.WaitPickup)
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
        public CustomJsonResult HandleExByMachineSelfTake(string operater, RopOrderHandleExByMachineSelfTake rop)
        {
            var result = new CustomJsonResult();


            List<Order> s_Orders = new List<Order>();

            List<StockChangeRecordModel> s_StockChangeRecords = new List<StockChangeRecordModel>();

            using (TransactionScope ts = new TransactionScope())
            {

                if (string.IsNullOrEmpty(rop.Remark))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，备注信息不能为空");
                }

                foreach (var item in rop.Items)
                {
                    LogUtil.Info("Item:" + item.ItemId);

                    var d_Order = CurrentDb.Order.Where(m => m.Id == item.ItemId).FirstOrDefault();
                    if (d_Order == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，该订单信息不存在");
                    }

                    if (!d_Order.ExIsHappen)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，该订单不是异常状态");
                    }

                    if (d_Order.ExIsHandle)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，该异常订单已经处理完毕");
                    }


                    if (item.IsRefund)
                    {

                        var payRefunds = CurrentDb.PayRefund.Where(m => m.OrderId == item.ItemId).ToList();

                        decimal refundedAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
                        decimal refundingAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Sum(m => m.ApplyAmount);

                        if (item.RefundAmount > (d_Order.ChargeAmount - (refundedAmount + refundingAmount)))
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，退款的金额不能大于可退金额");
                        }

                        var payTran = CurrentDb.PayTrans.Where(m => m.Id == d_Order.PayTransId).FirstOrDefault();

                        if (item.RefundAmount > payTran.ChargeAmount)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，退款的金额不能大于可退金额");
                        }

                        string payRefundId = IdWorker.Build(IdType.PayRefundId);

                        var payRefund = new PayRefund();
                        payRefund.Id = payRefundId;
                        payRefund.MerchId = d_Order.MerchId;
                        payRefund.MerchName = d_Order.MerchName;
                        payRefund.StoreId = d_Order.StoreId;
                        payRefund.StoreName = d_Order.StoreName;
                        payRefund.ClientUserId = d_Order.ClientUserId;
                        payRefund.ClientUserName = d_Order.ClientUserName;
                        payRefund.OrderId = d_Order.Id;
                        payRefund.PayPartnerPayTransId = d_Order.PayPartnerPayTransId;
                        payRefund.PayTransId = d_Order.PayTransId;
                        payRefund.ApplyTime = DateTime.Now;
                        payRefund.ApplyMethod = item.RefundMethod;
                        payRefund.ApplyRemark = rop.Remark;
                        payRefund.ApplyAmount = item.RefundAmount;
                        payRefund.Applyer = operater;
                        payRefund.Status = E_PayRefundStatus.WaitHandle;
                        payRefund.CreateTime = DateTime.Now;
                        payRefund.Creator = operater;
                        CurrentDb.PayRefund.Add(payRefund);
                    }

                    LogUtil.Info("orderSubs");

                    var d_OrderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == item.ItemId && m.ExPickupIsHappen == true && m.ExPickupIsHandle == false && m.PickupStatus == E_OrderPickupStatus.Exception).ToList();


                    foreach (var d_OrderSub in d_OrderSubs)
                    {
                        LogUtil.Info("orderSubs");

                        var detailItem = item.Uniques.Where(m => m.UniqueId == d_OrderSub.Id).FirstOrDefault();
                        if (detailItem == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，该订单里对应商品异常记录未找到");
                        }

                        if (detailItem.SignStatus != 1 && detailItem.SignStatus != 2)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，该订单不能处理该异常状态:" + detailItem.SignStatus);
                        }

                        if (detailItem.SignStatus == 1)
                        {

                            if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                            {
                                var result_OperateStock = BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.OrderPickupOneManMadeSignTakeByNotComplete, E_ShopMode.Machine, d_OrderSub.MerchId, d_OrderSub.StoreId, d_OrderSub.ShopId, d_OrderSub.MachineId, d_OrderSub.CabinetId, d_OrderSub.SlotId, d_OrderSub.PrdProductSkuId, 1);
                                if (result_OperateStock.Result != ResultType.Success)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，扣减库存失败");
                                }

                                s_StockChangeRecords.AddRange(result_OperateStock.Data.ChangeRecords);
                            }

                            d_OrderSub.ExPickupIsHandle = true;
                            d_OrderSub.ExPickupHandleTime = DateTime.Now;
                            d_OrderSub.ExPickupHandleSign = E_OrderExPickupHandleSign.Taked;
                            d_OrderSub.PickupStatus = E_OrderPickupStatus.ExPickupSignTaked;


                            var orderPickupLog = new OrderPickupLog();
                            orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                            orderPickupLog.OrderId = d_OrderSub.OrderId;
                            orderPickupLog.ShopMode = E_ShopMode.Machine;
                            orderPickupLog.MerchId = d_OrderSub.MerchId;
                            orderPickupLog.StoreId = d_OrderSub.StoreId;
                            orderPickupLog.ShopId = d_OrderSub.ShopId;
                            orderPickupLog.MachineId = d_OrderSub.MachineId;
                            orderPickupLog.UniqueId = d_OrderSub.Id;
                            orderPickupLog.UniqueType = E_UniqueType.OrderSub;
                            orderPickupLog.PrdProductSkuId = d_OrderSub.PrdProductSkuId;
                            orderPickupLog.CabinetId = d_OrderSub.CabinetId;
                            orderPickupLog.SlotId = d_OrderSub.SlotId;
                            orderPickupLog.Status = E_OrderPickupStatus.ExPickupSignTaked;
                            orderPickupLog.ActionRemark = "人为标识已取货";
                            orderPickupLog.Remark = "";
                            orderPickupLog.CreateTime = DateTime.Now;
                            orderPickupLog.Creator = operater;
                            CurrentDb.OrderPickupLog.Add(orderPickupLog);
                        }
                        else if (detailItem.SignStatus == 2)
                        {
                            if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                            {
                                var result_OperateStock = BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.OrderPickupOneManMadeSignNotTakeByNotComplete, E_ShopMode.Machine, d_OrderSub.MerchId, d_OrderSub.StoreId, d_OrderSub.ShopId, d_OrderSub.MachineId, d_OrderSub.CabinetId, d_OrderSub.SlotId, d_OrderSub.PrdProductSkuId, 1);

                                if (result_OperateStock.Result != ResultType.Success)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "异常处理失败，扣减库存失败");
                                }

                                s_StockChangeRecords.AddRange(result_OperateStock.Data.ChangeRecords);
                            }

                            d_OrderSub.ExPickupIsHandle = true;
                            d_OrderSub.ExPickupHandleTime = DateTime.Now;
                            d_OrderSub.ExPickupHandleSign = E_OrderExPickupHandleSign.UnTaked;
                            d_OrderSub.PickupStatus = E_OrderPickupStatus.ExPickupSignUnTaked;

                            var orderPickupLog = new OrderPickupLog();
                            orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                            orderPickupLog.OrderId = d_OrderSub.OrderId;
                            orderPickupLog.ShopMode = E_ShopMode.Machine;
                            orderPickupLog.MerchId = d_OrderSub.MerchId;
                            orderPickupLog.StoreId = d_OrderSub.StoreId;
                            orderPickupLog.MachineId = d_OrderSub.MachineId;
                            orderPickupLog.UniqueId = d_OrderSub.Id;
                            orderPickupLog.UniqueType = E_UniqueType.OrderSub;
                            orderPickupLog.PrdProductSkuId = d_OrderSub.PrdProductSkuId;
                            orderPickupLog.CabinetId = d_OrderSub.CabinetId;
                            orderPickupLog.SlotId = d_OrderSub.SlotId;
                            orderPickupLog.Status = E_OrderPickupStatus.ExPickupSignUnTaked;
                            orderPickupLog.ActionRemark = "人为标识未取货";
                            orderPickupLog.Remark = "";
                            orderPickupLog.CreateTime = DateTime.Now;
                            orderPickupLog.Creator = operater;
                            CurrentDb.OrderPickupLog.Add(orderPickupLog);
                        }
                    }

                    d_Order.ExIsHandle = true;
                    d_Order.ExHandleTime = DateTime.Now;
                    d_Order.ExHandleRemark = rop.Remark;
                    d_Order.Status = E_OrderStatus.Completed;
                    d_Order.CompletedTime = DateTime.Now;

                    s_Orders.Add(d_Order);


                    LogUtil.Info("orders");
                }

                LogUtil.Info("IsRunning");

                if (rop.IsRunning)
                {
                    if (string.IsNullOrEmpty(rop.MachineId))
                    {
                        var machineIds = s_Orders.Where(m => m.ReceiveMode == E_ReceiveMode.SelfTakeByMachine).Select(m => m.MachineId).ToArray();

                        foreach (var machineId in machineIds)
                        {
                            var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();
                            if (machine != null)
                            {
                                machine.RunStatus = E_MachineRunStatus.Running;
                                machine.ExIsHas = false;
                                machine.MendTime = DateTime.Now;
                                machine.Mender = operater;
                                CurrentDb.SaveChanges();
                            }
                        }
                    }
                    else
                    {

                        var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();
                        if (machine != null)
                        {
                            machine.RunStatus = E_MachineRunStatus.Running;
                            machine.ExIsHas = false;
                            machine.MendTime = DateTime.Now;
                            machine.Mender = operater;
                            CurrentDb.SaveChanges();
                        }
                    }

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "异常处理成功");
            }

            if (result.Result == ResultType.Success)
            {
                string trgerId = "";
                if (rop.AppId == AppId.STORETERM)
                {
                    trgerId = rop.MachineId;
                }
                else if (rop.AppId == AppId.MERCH)
                {
                    trgerId = rop.MerchId;
                }

                MqFactory.Global.PushOperateLog(operater, rop.AppId, trgerId, EventCode.OrderHandleException, "处理异常订单", new { Rop = rop, StockChangeRecords = s_StockChangeRecords });

            }

            return result;
        }
        public CustomJsonResult PayRefundResultNotify(string operater, E_PayPartner payPartner, E_PayTransLogNotifyFrom from, string payTransId, string payRefundId, string content)
        {
            LogUtil.Info("PayRefundResultNotify2");


            string refundStatus = "";
            string payPartnerPayTransId = "";
            string refundRemark = "";
            decimal refundAmount = 0m;
            switch (payPartner)
            {
                case E_PayPartner.Xrt:

                    var dic = XmlUtil.ToDictionary(content);

                    string status = "";

                    if (dic.ContainsKey("status"))
                    {
                        status = dic["status"].ToString();
                    }

                    string result_code = "";
                    if (dic.ContainsKey("result_code"))
                    {
                        result_code = dic["result_code"].ToString();
                    }

                    if (status == "0" && result_code == "0")
                    {
                        if (dic.ContainsKey("out_trade_no"))
                        {
                            payTransId = dic["out_trade_no"].ToString();
                        }

                        if (dic.ContainsKey("refund_count"))
                        {
                            int refund_count = Convert.ToInt32(dic["refund_count"].ToString());

                            for (var i = 0; i < refund_count; i++)
                            {
                                if (dic.ContainsKey("out_refund_no_" + i))
                                {
                                    string out_refund_no = dic["out_refund_no_" + i].ToString();

                                    if (out_refund_no == payRefundId)
                                    {
                                        refundStatus = dic["refund_status_" + i].ToString();
                                        refundAmount = decimal.Parse(dic["refund_fee_" + i].ToString()) * 0.01m;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    break;
            }


            if (refundStatus == "SUCCESS")
            {
                refundRemark = "系统自动退款成功";
            }
            else if (refundStatus == "FAIL")
            {
                refundRemark = "系统自动退款失败";
            }

            PayRefundHandle(IdWorker.Build(IdType.EmptyGuid), payRefundId, refundStatus, refundAmount, refundRemark);

            var payNotifyLog = new PayNotifyLog();
            payNotifyLog.Id = IdWorker.Build(IdType.NewGuid);
            payNotifyLog.PayTransId = payTransId;
            payNotifyLog.PayPartner = payPartner;
            payNotifyLog.PayPartnerPayTransId = payPartnerPayTransId;
            payNotifyLog.PayRefundId = payRefundId;
            payNotifyLog.NotifyContent = content;
            payNotifyLog.NotifyFrom = from;
            payNotifyLog.NotifyType = E_PayTransLogNotifyType.PayRefund;
            payNotifyLog.CreateTime = DateTime.Now;
            payNotifyLog.Creator = operater;
            CurrentDb.PayNotifyLog.Add(payNotifyLog);
            CurrentDb.SaveChanges();


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
        public CustomJsonResult PayRefundHandle(string operater, string refundId, string refundStatus, decimal refundAmount, string refundRemark)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var payRefund = CurrentDb.PayRefund.Where(m => m.Id == refundId).FirstOrDefault();
                if (payRefund == null)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该信息");
                }

                if (payRefund.Status == E_PayRefundStatus.Success || payRefund.Status == E_PayRefundStatus.Failure)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已被处理");
                }

                if (refundStatus == "SUCCESS")
                {
                    payRefund.Status = E_PayRefundStatus.Success;
                    payRefund.RefundedTime = DateTime.Now;
                    payRefund.RefundedAmount = refundAmount;
                    payRefund.Handler = operater;
                    payRefund.HandleRemark = refundRemark;
                    payRefund.HandleTime = DateTime.Now;
                    payRefund.Mender = operater;
                    payRefund.MendTime = DateTime.Now;

                    var order = CurrentDb.Order.Where(m => m.Id == payRefund.OrderId).FirstOrDefault();
                    if (order != null)
                    {
                        order.RefundedAmount += refundAmount;
                        order.Mender = operater;
                        order.MendTime = DateTime.Now;
                    }

                    Task4Factory.Tim2Global.Exit(Task4TimType.PayRefundCheckStatus, refundId);
                }
                else if (refundStatus == "FAIL")
                {
                    payRefund.Status = E_PayRefundStatus.Failure;
                    payRefund.Handler = operater;
                    payRefund.HandleRemark = refundRemark;
                    payRefund.Mender = operater;
                    payRefund.MendTime = DateTime.Now;
                    payRefund.HandleTime = DateTime.Now;
                    Task4Factory.Tim2Global.Exit(Task4TimType.PayRefundCheckStatus, refundId);
                }
                else if (refundStatus == "INVAILD")
                {
                    payRefund.Status = E_PayRefundStatus.InVaild;
                    payRefund.Handler = operater;
                    payRefund.HandleRemark = refundRemark;
                    payRefund.Mender = operater;
                    payRefund.MendTime = DateTime.Now;
                    payRefund.HandleTime = DateTime.Now;
                    Task4Factory.Tim2Global.Exit(Task4TimType.PayRefundCheckStatus, refundId);
                }
                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理成功");
            }

            return result;

        }

    }
}
