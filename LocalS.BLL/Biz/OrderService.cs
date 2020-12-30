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
    public class UseAreaModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
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
                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定商品为空", null);
            }


            lock (lock_Reserve)
            {

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

                    List<BuildOrder.ProductSku> buildOrderSkus = new List<BuildOrder.ProductSku>();

                    decimal couponAmountByDeposit = 0;
                    E_Coupon_UseAreaType couponUseAreaTypeByDeposit = E_Coupon_UseAreaType.Unknow;
                    List<UseAreaValueModel> couponUseAreaValueByDeposit = null;

                    decimal couponAmountByRent = 0;
                    E_Coupon_UseAreaType couponUseAreaTypeByRent = E_Coupon_UseAreaType.Unknow;
                    List<UseAreaValueModel> couponUseAreaValueByRent = null;

                    if (!string.IsNullOrEmpty(rop.CouponIdByDeposit))
                    {
                        var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == rop.CouponIdByDeposit).FirstOrDefault();
                        if (d_clientCoupon != null)
                        {
                            if (d_clientCoupon.Status != E_ClientCouponStatus.WaitUse || d_clientCoupon.ValidStartTime > DateTime.Now || d_clientCoupon.ValidEndTime < DateTime.Now)
                            {
                                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "优惠券无效", null);
                            }


                            d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                            d_clientCoupon.Mender = operater;
                            d_clientCoupon.MendTime = DateTime.Now;


                            var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                            if (d_coupon != null)
                            {
                                if (d_coupon.FaceType == E_Coupon_FaceType.DepositVoucher)
                                {
                                    couponAmountByDeposit = d_coupon.FaceValue;
                                    couponUseAreaTypeByDeposit = d_coupon.UseAreaType;
                                    couponUseAreaValueByDeposit = d_coupon.UseAreaValue.ToJsonObject<List<UseAreaValueModel>>();
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
                                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "优惠券无效", null);
                            }

                            d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                            d_clientCoupon.Mender = operater;
                            d_clientCoupon.MendTime = DateTime.Now;

                            var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                            if (d_coupon != null)
                            {
                                if (d_coupon.FaceType == E_Coupon_FaceType.RentVoucher)
                                {
                                    couponAmountByRent = d_coupon.FaceValue;
                                    couponUseAreaTypeByRent = d_coupon.UseAreaType;
                                    couponUseAreaValueByRent = d_coupon.UseAreaValue.ToJsonObject<List<UseAreaValueModel>>();
                                }
                            }
                        }
                    }


                    #region 检查可售商品信息是否符合实际环境
                    List<string> warn_tips = new List<string>();

                    foreach (var block in rop.Blocks)
                    {
                        var skus = block.Skus;

                        foreach (var sku in skus)
                        {
                            if (rop.ShopMethod == E_OrderShopMethod.Shop)
                            {
                                #region Shop

                                string[] sellChannelRefIds = new string[] { };

                                if (sku.ShopMode == E_SellChannelRefType.Mall)
                                {
                                    sellChannelRefIds = new string[] { SellChannelStock.MallSellChannelRefId };
                                }
                                else if (sku.ShopMode == E_SellChannelRefType.Machine)
                                {
                                    if (sku.SellChannelRefIds == null || sku.SellChannelRefIds.Length == 0)
                                    {
                                        sellChannelRefIds = store.SellMachineIds;
                                    }
                                    else
                                    {
                                        sellChannelRefIds = sku.SellChannelRefIds;
                                    }
                                }

                                var r_sku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, rop.StoreId, sellChannelRefIds, sku.Id);

                                if (r_sku == null)
                                {
                                    warn_tips.Add(string.Format("{0}商品信息不存在", r_sku.Name));
                                }
                                else
                                {
                                    if (r_sku.Stocks.Count == 0)
                                    {
                                        warn_tips.Add(string.Format("{0}商品库存信息不存在", r_sku.Name));
                                    }
                                    else
                                    {
                                        var sellQuantity = r_sku.Stocks.Sum(m => m.SellQuantity);

                                        Console.WriteLine("sellQuantity：" + sellQuantity);

                                        if (r_sku.Stocks[0].IsOffSell)
                                        {
                                            warn_tips.Add(string.Format("{0}已经下架", r_sku.Name));
                                        }
                                        else
                                        {
                                            if (sellQuantity < sku.Quantity)
                                            {
                                                warn_tips.Add(string.Format("{0}的可销售数量为{1}个", r_sku.Name, sellQuantity));
                                            }
                                        }
                                    }

                                    var buildOrderSku = new BuildOrder.ProductSku();
                                    buildOrderSku.Id = sku.Id;
                                    buildOrderSku.ProductId = r_sku.ProductId;
                                    buildOrderSku.ReceiveMode = block.ReceiveMode;
                                    buildOrderSku.Name = r_sku.Name;
                                    buildOrderSku.MainImgUrl = r_sku.MainImgUrl;
                                    buildOrderSku.BarCode = r_sku.BarCode;
                                    buildOrderSku.CumCode = r_sku.CumCode;
                                    buildOrderSku.SpecDes = r_sku.SpecDes;
                                    buildOrderSku.Producer = r_sku.Producer;
                                    buildOrderSku.CartId = sku.CartId;
                                    buildOrderSku.SvcConsulterId = sku.SvcConsulterId;
                                    buildOrderSku.KindId1 = r_sku.KindId1;
                                    buildOrderSku.KindId2 = r_sku.KindId2;
                                    buildOrderSku.KindId3 = r_sku.KindId3;
                                    buildOrderSku.Quantity = sku.Quantity;
                                    buildOrderSku.ShopMode = sku.ShopMode;
                                    buildOrderSku.Stocks = r_sku.Stocks;
                                    //todo 会员价 变换
                                    decimal salePrice = r_sku.Stocks[0].SalePrice;
                                    decimal originalPrice = r_sku.Stocks[0].SalePrice;

                                    buildOrderSku.SalePrice = salePrice;
                                    buildOrderSku.SaleAmount = salePrice * sku.Quantity;
                                    buildOrderSku.OriginalPrice = originalPrice;
                                    buildOrderSku.OriginalAmount = originalPrice * sku.Quantity;
                                    buildOrderSkus.Add(buildOrderSku);
                                }

                                #endregion
                            }
                            if (rop.ShopMethod == E_OrderShopMethod.Rent)
                            {
                                #region Rent

                                var r_sku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, rop.StoreId, new string[] { SellChannelStock.MallSellChannelRefId }, sku.Id);

                                if (r_sku == null)
                                {
                                    warn_tips.Add(string.Format("{0}商品信息不存在", r_sku.Name));
                                }
                                else
                                {
                                    if (r_sku.Stocks.Count == 0)
                                    {
                                        warn_tips.Add(string.Format("{0}商品库存信息不存在", r_sku.Name));
                                    }
                                    else
                                    {
                                        var sellQuantity = r_sku.Stocks.Sum(m => m.SellQuantity);

                                        Console.WriteLine("sellQuantity：" + sellQuantity);

                                        if (r_sku.Stocks[0].IsOffSell)
                                        {
                                            warn_tips.Add(string.Format("{0}已经下架", r_sku.Name));
                                        }
                                        else if (!r_sku.Stocks[0].IsUseRent)
                                        {
                                            warn_tips.Add(string.Format("{0}不支持租用", r_sku.Name));
                                        }
                                        else
                                        {
                                            if (sellQuantity < sku.Quantity)
                                            {
                                                warn_tips.Add(string.Format("{0}的可销售数量为{1}个", r_sku.Name, sellQuantity));
                                            }
                                        }
                                    }

                                    var buildOrderSku = new BuildOrder.ProductSku();
                                    buildOrderSku.Id = sku.Id;
                                    buildOrderSku.ProductId = r_sku.ProductId;
                                    buildOrderSku.ReceiveMode = block.ReceiveMode;
                                    buildOrderSku.Name = r_sku.Name;
                                    buildOrderSku.MainImgUrl = r_sku.MainImgUrl;
                                    buildOrderSku.BarCode = r_sku.BarCode;
                                    buildOrderSku.CumCode = r_sku.CumCode;
                                    buildOrderSku.SpecDes = r_sku.SpecDes;
                                    buildOrderSku.Producer = r_sku.Producer;
                                    buildOrderSku.Quantity = sku.Quantity;
                                    buildOrderSku.ShopMode = sku.ShopMode;
                                    buildOrderSku.Stocks = r_sku.Stocks;
                                    buildOrderSku.CartId = sku.CartId;
                                    buildOrderSku.SvcConsulterId = sku.SvcConsulterId;
                                    buildOrderSku.KindId1 = r_sku.KindId1;
                                    buildOrderSku.KindId2 = r_sku.KindId2;
                                    buildOrderSku.KindId3 = r_sku.KindId3;

                                    #region CouponAmountByDeposit
                                    if (couponUseAreaTypeByDeposit == E_Coupon_UseAreaType.All)
                                    {
                                        buildOrderSku.CouponAmountByDeposit = couponAmountByDeposit;
                                    }
                                    else if (couponUseAreaTypeByDeposit == E_Coupon_UseAreaType.Store)
                                    {
                                        if (couponUseAreaValueByDeposit != null)
                                        {
                                            if (couponUseAreaValueByDeposit.Where(m => m.Id == rop.StoreId).Count() > 0)
                                            {
                                                buildOrderSku.CouponAmountByDeposit = couponAmountByDeposit;
                                            }
                                        }
                                    }
                                    else if (couponUseAreaTypeByDeposit == E_Coupon_UseAreaType.ProductKind)
                                    {
                                        if (couponUseAreaValueByDeposit != null)
                                        {
                                            string kind3 = r_sku.KindId3.ToString();
                                            if (couponUseAreaValueByDeposit.Where(m => m.Id == kind3).Count() > 0)
                                            {
                                                buildOrderSku.CouponAmountByDeposit = couponAmountByDeposit;
                                            }
                                        }
                                    }
                                    else if (couponUseAreaTypeByDeposit == E_Coupon_UseAreaType.ProductSpu)
                                    {
                                        if (couponUseAreaValueByDeposit != null)
                                        {
                                            if (couponUseAreaValueByDeposit.Where(m => m.Id == r_sku.ProductId).Count() > 0)
                                            {
                                                buildOrderSku.CouponAmountByDeposit = couponAmountByDeposit;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region CouponAmountByRent
                                    if (couponUseAreaTypeByRent == E_Coupon_UseAreaType.All)
                                    {
                                        buildOrderSku.CouponAmountByRent = couponAmountByRent;
                                    }
                                    else if (couponUseAreaTypeByRent == E_Coupon_UseAreaType.Store)
                                    {
                                        if (couponUseAreaValueByRent != null)
                                        {
                                            if (couponUseAreaValueByRent.Where(m => m.Id == rop.StoreId).Count() > 0)
                                            {
                                                buildOrderSku.CouponAmountByRent = couponAmountByRent;
                                            }
                                        }
                                    }
                                    else if (couponUseAreaTypeByRent == E_Coupon_UseAreaType.ProductKind)
                                    {
                                        if (couponUseAreaValueByRent != null)
                                        {
                                            string kind3 = r_sku.KindId3.ToString();
                                            if (couponUseAreaValueByRent.Where(m => m.Id == kind3).Count() > 0)
                                            {
                                                buildOrderSku.CouponAmountByRent = couponAmountByRent;
                                            }
                                        }
                                    }
                                    else if (couponUseAreaTypeByRent == E_Coupon_UseAreaType.ProductSpu)
                                    {
                                        if (couponUseAreaValueByRent != null)
                                        {
                                            if (couponUseAreaValueByRent.Where(m => m.Id == r_sku.ProductId).Count() > 0)
                                            {
                                                buildOrderSku.CouponAmountByRent = couponAmountByRent;
                                            }
                                        }
                                    }
                                    #endregion

                                    decimal salePrice = r_sku.Stocks[0].DepositPrice + r_sku.Stocks[0].RentMhPrice;
                                    decimal originalPrice = r_sku.Stocks[0].DepositPrice + r_sku.Stocks[0].RentMhPrice;

                                    buildOrderSku.SalePrice = salePrice;
                                    buildOrderSku.SaleAmount = salePrice * sku.Quantity;
                                    buildOrderSku.OriginalPrice = originalPrice;
                                    buildOrderSku.OriginalAmount = originalPrice * sku.Quantity;
                                    buildOrderSku.DepositAmount = r_sku.Stocks[0].DepositPrice;
                                    buildOrderSku.RentAmount = r_sku.Stocks[0].RentMhPrice;
                                    buildOrderSku.RentTermUnit = E_RentTermUnit.Month;
                                    buildOrderSku.RentTermValue = 1;
                                    buildOrderSkus.Add(buildOrderSku);
                                }

                                #endregion
                            }
                            else if (rop.ShopMethod == E_OrderShopMethod.MemberFee)
                            {
                                #region MemberFee

                                var memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.Id == sku.Id).FirstOrDefault();
                                if (memberFeeSt == null)
                                {
                                    warn_tips.Add(string.Format("{0}商品信息不存在", "服务"));
                                }
                                else
                                {
                                    var stocks = new List<ProductSkuStockModel>();

                                    var stock = new ProductSkuStockModel();

                                    stock.RefType = E_SellChannelRefType.Mall;
                                    stock.RefId = SellChannelStock.MemberFeeSellChannelRefId;
                                    stock.CabinetId = "";
                                    stock.SlotId = "";
                                    stock.SumQuantity = 0;
                                    stock.LockQuantity = 0;
                                    stock.SellQuantity = 0;
                                    stock.IsOffSell = false;
                                    stock.SalePrice = memberFeeSt.FeeSaleValue;

                                    stocks.Add(stock);

                                    var buildOrderSku = new BuildOrder.ProductSku();
                                    buildOrderSku.Id = sku.Id;
                                    buildOrderSku.ProductId = "";
                                    buildOrderSku.ReceiveMode = E_ReceiveMode.MemberFee;
                                    buildOrderSku.Name = memberFeeSt.Name;
                                    buildOrderSku.MainImgUrl = memberFeeSt.MainImgUrl;
                                    buildOrderSku.BarCode = "";
                                    buildOrderSku.CumCode = "";
                                    buildOrderSku.SpecDes = null;
                                    buildOrderSku.Producer = "";
                                    buildOrderSku.Stocks = stocks;
                                    buildOrderSku.CartId = "";
                                    buildOrderSku.SvcConsulterId = "";
                                    buildOrderSku.KindId1 = 0;
                                    buildOrderSku.KindId2 = 0;
                                    buildOrderSku.KindId3 = 0;
                                    buildOrderSku.Quantity = sku.Quantity;
                                    buildOrderSku.ShopMode = sku.ShopMode;
                                    buildOrderSku.SalePrice = memberFeeSt.FeeSaleValue;
                                    buildOrderSku.SaleAmount = memberFeeSt.FeeSaleValue;
                                    buildOrderSku.OriginalPrice = memberFeeSt.FeeOriginalValue;
                                    buildOrderSku.OriginalAmount = memberFeeSt.FeeOriginalValue;
                                    buildOrderSkus.Add(buildOrderSku);

                                }

                                #endregion
                            }
                        }
                    }

                    if (warn_tips.Count > 0)
                    {
                        return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, string.Join("\n", warn_tips.ToArray()), null);
                    }

                    #endregion

                    if (rop.ShopMethod == E_OrderShopMethod.Shop)
                    {
                        if (rop.CouponIdsByShop != null && rop.CouponIdsByShop.Count > 0)
                        {
                            string couponIdByShop = rop.CouponIdsByShop[0];
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == couponIdByShop).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                if (d_clientCoupon.Status != E_ClientCouponStatus.WaitUse || d_clientCoupon.ValidStartTime > DateTime.Now || d_clientCoupon.ValidEndTime < DateTime.Now)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "优惠券无效", null);
                                }

                                d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                                d_clientCoupon.Mender = operater;
                                d_clientCoupon.MendTime = DateTime.Now;


                                var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                                if (d_coupon != null)
                                {
                                    decimal cal_sum_amount = 0;
                                    if (d_coupon.UseAreaType == E_Coupon_UseAreaType.All)
                                    {
                                        cal_sum_amount = buildOrderSkus.Sum(m => m.SaleAmount);
                                    }
                                    else if (d_coupon.UseAreaType == E_Coupon_UseAreaType.Store)
                                    {
                                        cal_sum_amount = buildOrderSkus.Sum(m => m.SaleAmount);
                                    }
                                    else if (d_coupon.UseAreaType == E_Coupon_UseAreaType.ProductKind)
                                    {
                                        var list = d_coupon.UseAreaValue.ToJsonObject<List<UseAreaValueModel>>();
                                        if (list != null)
                                        {
                                            int[] ids = list.Select(s => Int32.Parse(s.Id)).ToArray();

                                            if (ids != null)
                                            {
                                                cal_sum_amount = buildOrderSkus.Where(m => ids.Contains(m.KindId3)).Sum(m => m.SaleAmount);
                                            }
                                        }

                                    }
                                    else if (d_coupon.UseAreaType == E_Coupon_UseAreaType.ProductSpu)
                                    {
                                        var list = d_coupon.UseAreaValue.ToJsonObject<List<UseAreaValueModel>>();
                                        if (list != null)
                                        {
                                            string[] ids = list.Select(m => m.Id).ToArray();

                                            if (ids != null)
                                            {
                                                cal_sum_amount = buildOrderSkus.Where(m => ids.Contains(m.ProductId)).Sum(m => m.SaleAmount);
                                            }
                                        }

                                    }

                                    foreach (var buildOrderSku in buildOrderSkus)
                                    {
                                        buildOrderSku.CouponAmountByShop = Decimal.Round(BizFactory.Order.CalCouponAmount(cal_sum_amount, d_coupon.AtLeastAmount, d_coupon.UseAreaType, d_coupon.UseAreaValue, d_coupon.FaceType, d_coupon.FaceValue, rop.StoreId, buildOrderSku.ProductId, buildOrderSku.KindId3, buildOrderSku.SaleAmount), 2);
                                    }
                                }
                            }
                        }
                    }


                    LogUtil.Info("rop.bizProductSkus:" + buildOrderSkus.ToJsonString());
                    var buildOrders = BuildOrders(rop.ShopMethod, buildOrderSkus);
                    LogUtil.Info("SlotStock.buildOrders:" + buildOrders.ToJsonString());

                    #region 更改购物车标识
                    var clientUserName = "匿名";
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

                        var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == rop.ClientUserId).FirstOrDefault();
                        if (clientUser != null)
                        {
                            if (!string.IsNullOrEmpty(clientUser.NickName))
                            {
                                clientUserName = clientUser.NickName;
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

                    List<Order> orders = new List<Order>();

                    string unId = IdWorker.Build(IdType.NewGuid);

                    foreach (var buildOrder in buildOrders)
                    {
                        var order = new Order();
                        order.Id = IdWorker.Build(IdType.OrderId);
                        order.UnId = unId;
                        order.ClientUserId = rop.ClientUserId;
                        order.ClientUserName = clientUserName;
                        order.MerchId = store.MerchId;
                        order.MerchName = store.MerchName;
                        order.StoreId = rop.StoreId;
                        order.StoreName = store.Name;
                        order.SellChannelRefType = buildOrder.SellChannelRefType;
                        order.SellChannelRefId = buildOrder.SellChannelRefId;
                        order.SaleOutletId = rop.SaleOutletId;
                        order.PayExpireTime = DateTime.Now.AddSeconds(300);
                        order.PickupCode = IdWorker.BuildPickupCode();
                        order.PickupCodeExpireTime = DateTime.Now.AddDays(10);//todo 取货码10内有效
                        order.SubmittedTime = DateTime.Now;
                        order.CouponIdsByShop = rop.CouponIdsByShop.ToJsonString();
                        order.CouponAmountByShop = buildOrder.CouponAmountByShop;
                        order.CouponIdByRent = rop.CouponIdByRent;
                        order.CouponAmountByRent = buildOrder.CouponAmountByRent;
                        order.CouponIdByDeposit = rop.CouponIdByDeposit;
                        order.CouponAmountByDeposit = buildOrder.CouponAmountByDeposit;
                        order.ShopMethod = rop.ShopMethod;
                        order.ReffSign = rop.ReffSign;
                        order.ReffUserId = reffUserId;

                        switch (buildOrder.ReceiveMode)
                        {
                            case E_ReceiveMode.Delivery:
                                #region Delivery

                                var rm_Delivery = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.Delivery).FirstOrDefault();

                                if (rm_Delivery == null && rm_Delivery.Delivery == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式配送地址为空", null);
                                }

                                order.ReceiveMode = E_ReceiveMode.Delivery;
                                order.ReceiveModeName = "配送到手";
                                order.Receiver = rm_Delivery.Delivery.Consignee;
                                order.ReceptionId = rm_Delivery.Delivery.Id;
                                order.ReceiverPhoneNumber = rm_Delivery.Delivery.PhoneNumber;
                                order.ReceptionAreaCode = rm_Delivery.Delivery.AreaCode;
                                order.ReceptionAreaName = rm_Delivery.Delivery.AreaName;
                                order.ReceptionAddress = rm_Delivery.Delivery.Address;

                                #endregion
                                break;
                            case E_ReceiveMode.StoreSelfTake:
                                #region StoreSelfTake

                                var rm_StoreSelfTake = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.StoreSelfTake).FirstOrDefault();

                                if (rm_StoreSelfTake == null || rm_StoreSelfTake.SelfTake == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线上商城售卖模式自取地址为空", null);
                                }

                                order.ReceiveMode = E_ReceiveMode.StoreSelfTake;
                                order.ReceiveModeName = "到店自提";
                                order.Receiver = rm_StoreSelfTake.SelfTake.Consignee;
                                order.ReceiverPhoneNumber = rm_StoreSelfTake.SelfTake.PhoneNumber;
                                order.ReceptionId = rm_StoreSelfTake.SelfTake.Id;
                                order.ReceptionAreaCode = rm_StoreSelfTake.SelfTake.AreaCode;
                                order.ReceptionAreaName = rm_StoreSelfTake.SelfTake.AreaName;
                                order.ReceptionAddress = rm_StoreSelfTake.SelfTake.Address;
                                order.ReceptionMarkName = rm_StoreSelfTake.SelfTake.MarkName;


                                if (rm_StoreSelfTake.BookTime != null && !string.IsNullOrEmpty(rm_StoreSelfTake.BookTime.Value))
                                {
                                    //1为具体时间值 例如 2020-11-24 13:00
                                    //2为时间段区间 例如 2020-11-24 13:00,2020-11-24 13:00
                                    if (rm_StoreSelfTake.BookTime.Type == 1)
                                    {
                                        if (Lumos.CommonUtil.IsDateTime(rm_StoreSelfTake.BookTime.Value))
                                        {
                                            order.ReceptionBookStartTime = DateTime.Parse(rm_StoreSelfTake.BookTime.Value);
                                        }
                                    }
                                    else if (rm_StoreSelfTake.BookTime.Type == 2)
                                    {
                                        string[] arr_time = rm_StoreSelfTake.BookTime.Value.Split(',');
                                        if (arr_time.Length == 2)
                                        {
                                            if (Lumos.CommonUtil.IsDateTime(arr_time[0]))
                                            {
                                                order.ReceptionBookStartTime = DateTime.Parse(arr_time[0]);
                                            }

                                            if (Lumos.CommonUtil.IsDateTime(arr_time[1]))
                                            {
                                                order.ReceptionBookEndTime = DateTime.Parse(arr_time[1]);
                                            }
                                        }

                                    }
                                }

                                #endregion
                                break;
                            case E_ReceiveMode.MachineSelfTake:
                                #region MachineSelfTake

                                if (order.PickupCode == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定下单生成取货码失败", null);
                                }

                                var rm_MachineSelfTake = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.MachineSelfTake).FirstOrDefault();

                                if (rm_MachineSelfTake == null || rm_MachineSelfTake.SelfTake == null)
                                {
                                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "线下机器售卖模式自提地址为空", null);
                                }

                                order.ReceiveMode = E_ReceiveMode.MachineSelfTake;
                                order.ReceiveModeName = "机器自提";
                                order.Receiver = null;
                                order.ReceiverPhoneNumber = null;
                                order.ReceptionId = rm_MachineSelfTake.SelfTake.Id;
                                order.ReceptionAreaCode = rm_MachineSelfTake.SelfTake.AreaCode;
                                order.ReceptionAreaName = rm_MachineSelfTake.SelfTake.AreaName;
                                order.ReceptionAddress = rm_MachineSelfTake.SelfTake.Address;
                                order.ReceptionMarkName = rm_MachineSelfTake.SelfTake.MarkName;
                                #endregion
                                break;
                            case E_ReceiveMode.MemberFee:
                                #region MemberFee
                                order.ReceiveMode = E_ReceiveMode.MemberFee;
                                order.ReceiveModeName = "会员费";
                                order.IsNoDisplayClient = true;
                                #endregion
                                break;

                        }

                        order.SaleAmount = buildOrder.SaleAmount;
                        order.OriginalAmount = buildOrder.OriginalAmount;
                        order.DiscountAmount = buildOrder.DiscountAmount;
                        order.ChargeAmount = buildOrder.ChargeAmount;
                        order.Quantity = buildOrder.Quantity;
                        order.PayStatus = E_PayStatus.WaitPay;
                        order.Status = E_OrderStatus.WaitPay;
                        order.Source = rop.Source;
                        order.AppId = rop.AppId;
                        order.IsTestMode = rop.IsTestMode;
                        order.Creator = operater;
                        order.CreateTime = DateTime.Now;
                        CurrentDb.Order.Add(order);
                        orders.Add(order);

                        foreach (var buildOrderSub in buildOrder.Childs)
                        {
                            var productSku = buildOrderSkus.Where(m => m.Id == buildOrderSub.ProductSkuId).FirstOrDefault();

                            var orderSub = new OrderSub();
                            orderSub.Id = order.Id + buildOrder.Childs.IndexOf(buildOrderSub).ToString();
                            orderSub.ClientUserId = order.ClientUserId;
                            orderSub.ClientUserName = order.ClientUserName;
                            orderSub.MerchId = order.MerchId;
                            orderSub.MerchName = order.MerchName;
                            orderSub.StoreId = order.StoreId;
                            orderSub.StoreName = order.StoreName;
                            orderSub.SellChannelRefType = order.SellChannelRefType;
                            orderSub.SellChannelRefId = order.SellChannelRefId;
                            orderSub.ReceiveModeName = order.ReceiveModeName;
                            orderSub.ReceiveMode = order.ReceiveMode;
                            orderSub.CabinetId = buildOrderSub.CabinetId;
                            orderSub.SlotId = buildOrderSub.SlotId;
                            orderSub.OrderId = order.Id;
                            orderSub.PrdProductSkuId = buildOrderSub.ProductSkuId;
                            orderSub.PrdProductId = productSku.ProductId;
                            orderSub.PrdProductSkuName = productSku.Name;
                            orderSub.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                            orderSub.PrdProductSkuSpecDes = productSku.SpecDes.ToJsonString();
                            orderSub.PrdProductSkuProducer = productSku.Producer;
                            orderSub.PrdProductSkuBarCode = productSku.BarCode;
                            orderSub.PrdProductSkuCumCode = productSku.CumCode;
                            orderSub.PrdKindId1 = productSku.KindId1;
                            orderSub.PrdKindId2 = productSku.KindId2;
                            orderSub.PrdKindId3 = productSku.KindId3;
                            orderSub.Quantity = buildOrderSub.Quantity;
                            orderSub.SalePrice = buildOrderSub.SalePrice;
                            orderSub.OriginalPrice = buildOrderSub.OriginalPrice;
                            orderSub.SaleAmount = buildOrderSub.SaleAmount;
                            orderSub.OriginalAmount = buildOrderSub.OriginalAmount;
                            orderSub.DiscountAmount = buildOrderSub.DiscountAmount;
                            orderSub.ChargeAmount = buildOrderSub.ChargeAmount;
                            orderSub.RentTermUnit = buildOrderSub.RentTermUnit;
                            orderSub.RentTermValue = buildOrderSub.RentTermValue;
                            orderSub.RentAmount = buildOrderSub.RentAmount;
                            orderSub.DepositAmount = buildOrderSub.DepositAmount;
                            orderSub.CouponAmountByDeposit = buildOrderSub.CouponAmountByDeposit;
                            orderSub.CouponAmountByShop = buildOrderSub.CouponAmountByShop;
                            orderSub.CouponAmountByRent = buildOrderSub.CouponAmountByRent;
                            orderSub.PayStatus = E_PayStatus.WaitPay;
                            orderSub.PickupStatus = E_OrderPickupStatus.WaitPay;
                            orderSub.SvcConsulterId = productSku.SvcConsulterId;
                            orderSub.SaleOutletId = order.SaleOutletId;
                            orderSub.IsTestMode = order.IsTestMode;
                            orderSub.Creator = order.Creator;
                            orderSub.CreateTime = order.CreateTime;
                            orderSub.ShopMethod = order.ShopMethod;
                            orderSub.ReffSign = order.ReffSign;
                            orderSub.ReffUserId = order.ReffUserId;
                            CurrentDb.OrderSub.Add(orderSub);


                            //判断ShopMethod 是 Shop 才进行库存操作
                            if (orderSub.ShopMethod == E_OrderShopMethod.Shop)
                            {
                                BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderReserveSuccess, rop.AppId, order.MerchId, order.StoreId, orderSub.SellChannelRefId, orderSub.CabinetId, orderSub.SlotId, orderSub.PrdProductSkuId, orderSub.Quantity);
                            }
                        }
                    }


                    CurrentDb.SaveChanges();
                    ts.Complete();

                    foreach (var order in orders)
                    {
                        Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckReservePay, order.Id, order.PayExpireTime.Value, new Order2CheckPayModel { Id = order.Id, MerchId = order.MerchId });
                        MqFactory.Global.PushEventNotify(operater, rop.AppId, order.MerchId, order.StoreId, "", EventCode.OrderReserveSuccess, string.Format("订单号：{0}，预定成功", order.Id));
                        ret.Orders.Add(new RetOrderReserve.Order { Id = order.Id, ChargeAmount = order.ChargeAmount.ToF2Price() });
                    }

                    result = new CustomJsonResult<RetOrderReserve>(ResultType.Success, ResultCode.Success, "预定成功", ret);

                }
            }

            return result;

        }
        private List<BuildOrder> BuildOrders(E_OrderShopMethod shopMethod, List<BuildOrder.ProductSku> reserveProductSkus)
        {
            List<BuildOrder> buildOrders = new List<BuildOrder>();

            if (reserveProductSkus == null || reserveProductSkus.Count == 0)
                return buildOrders;

            List<BuildOrder.Child> buildOrderChilds = new List<BuildOrder.Child>();

            var d_s_orders = (from d in reserveProductSkus select new { d.ShopMode, d.ReceiveMode }).Distinct().ToArray();

            foreach (var d_s_order in d_s_orders)
            {
                var shopModeProductSkus = reserveProductSkus.Where(m => m.ShopMode == d_s_order.ShopMode && m.ReceiveMode == d_s_order.ReceiveMode).ToList();

                foreach (var shopModeProductSku in shopModeProductSkus)
                {
                    var productSku_Stocks = shopModeProductSku.Stocks;

                    if (d_s_order.ShopMode == E_SellChannelRefType.Mall)
                    {
                        //SalePrice,OriginalPrice 以 shopModeProductSku 的 SalePrice和 OriginalPrice作为标准
                        var buildOrderChild = new BuildOrder.Child();
                        buildOrderChild.SellChannelRefType = productSku_Stocks[0].RefType;
                        buildOrderChild.SellChannelRefId = productSku_Stocks[0].RefId;
                        buildOrderChild.ProductSkuId = shopModeProductSku.Id;
                        buildOrderChild.ReceiveMode = d_s_order.ReceiveMode;
                        buildOrderChild.CabinetId = productSku_Stocks[0].CabinetId;
                        buildOrderChild.SlotId = productSku_Stocks[0].SlotId;
                        buildOrderChild.Quantity = shopModeProductSku.Quantity;
                        buildOrderChild.SalePrice = shopModeProductSku.SalePrice;
                        buildOrderChild.SaleAmount = shopModeProductSku.SaleAmount;
                        buildOrderChild.OriginalPrice = shopModeProductSku.OriginalPrice;
                        buildOrderChild.OriginalAmount = shopModeProductSku.OriginalAmount;
                        buildOrderChild.RentTermUnit = shopModeProductSku.RentTermUnit;
                        buildOrderChild.RentTermValue = shopModeProductSku.RentTermValue;
                        buildOrderChild.RentAmount = shopModeProductSku.RentAmount;
                        buildOrderChild.DepositAmount = shopModeProductSku.DepositAmount;
                        buildOrderChild.CouponAmountByDeposit = shopModeProductSku.CouponAmountByDeposit;
                        buildOrderChild.CouponAmountByShop = shopModeProductSku.CouponAmountByShop;
                        buildOrderChild.CouponAmountByRent = shopModeProductSku.CouponAmountByRent;
                        buildOrderChilds.Add(buildOrderChild);
                    }
                    else if (d_s_order.ShopMode == E_SellChannelRefType.Machine)
                    {
                        foreach (var item in productSku_Stocks)
                        {
                            bool isFlag = false;
                            for (var i = 0; i < item.SellQuantity; i++)
                            {
                                int reservedQuantity = buildOrderChilds.Where(m => m.ProductSkuId == shopModeProductSku.Id && m.SellChannelRefType == d_s_order.ShopMode).Sum(m => m.Quantity);//已订的数量
                                int needReserveQuantity = shopModeProductSku.Quantity;//需要订的数量
                                if (reservedQuantity != needReserveQuantity)
                                {
                                    var buildOrderChild = new BuildOrder.Child();
                                    buildOrderChild.SellChannelRefType = item.RefType;
                                    buildOrderChild.SellChannelRefId = item.RefId;
                                    buildOrderChild.ProductSkuId = shopModeProductSku.Id;
                                    buildOrderChild.ReceiveMode = d_s_order.ReceiveMode;
                                    buildOrderChild.CabinetId = item.CabinetId;
                                    buildOrderChild.SlotId = item.SlotId;
                                    buildOrderChild.Quantity = 1;
                                    buildOrderChild.SalePrice = shopModeProductSku.SalePrice;
                                    buildOrderChild.SaleAmount = buildOrderChild.Quantity * shopModeProductSku.SalePrice;
                                    buildOrderChild.OriginalPrice = shopModeProductSku.OriginalPrice;
                                    buildOrderChild.OriginalAmount = buildOrderChild.Quantity * shopModeProductSku.OriginalPrice;
                                    buildOrderChild.CouponAmountByDeposit = shopModeProductSku.CouponAmountByDeposit;
                                    buildOrderChild.CouponAmountByShop = shopModeProductSku.CouponAmountByShop;
                                    buildOrderChild.CouponAmountByRent = shopModeProductSku.CouponAmountByRent;
                                    buildOrderChilds.Add(buildOrderChild);
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

            var sumSaleAmount = buildOrderChilds.Sum(m => m.SaleAmount);
            var sumDiscountAmount = 0m;
            for (int i = 0; i < buildOrderChilds.Count; i++)
            {
                decimal scale = (sumSaleAmount == 0 ? 0 : (buildOrderChilds[i].SaleAmount / sumSaleAmount));
                buildOrderChilds[i].DiscountAmount = Decimal.Round(scale * sumDiscountAmount, 2);
                buildOrderChilds[i].ChargeAmount = buildOrderChilds[i].SaleAmount - buildOrderChilds[i].DiscountAmount - buildOrderChilds[i].CouponAmountByShop - buildOrderChilds[i].CouponAmountByRent - buildOrderChilds[i].CouponAmountByDeposit;
            }

            var sumDiscountAmount2 = buildOrderChilds.Sum(m => m.DiscountAmount);
            if (sumDiscountAmount != sumDiscountAmount2)
            {
                var diff = sumDiscountAmount - sumDiscountAmount2;

                buildOrderChilds[buildOrderChilds.Count - 1].DiscountAmount = buildOrderChilds[buildOrderChilds.Count - 1].DiscountAmount + diff;
                buildOrderChilds[buildOrderChilds.Count - 1].SaleAmount = buildOrderChilds[buildOrderChilds.Count - 1].SaleAmount - buildOrderChilds[buildOrderChilds.Count - 1].DiscountAmount;
            }

            var l_buildOrders = (from c in buildOrderChilds
                                 select new
                                 {
                                     c.SellChannelRefType,
                                     c.SellChannelRefId,
                                     c.ReceiveMode,
                                 }).Distinct().ToList();


            foreach (var l_buildOrder in l_buildOrders)
            {
                var buildOrder = new BuildOrder();
                buildOrder.SellChannelRefType = l_buildOrder.SellChannelRefType;
                buildOrder.SellChannelRefId = l_buildOrder.SellChannelRefId;
                buildOrder.ReceiveMode = l_buildOrder.ReceiveMode;
                buildOrder.Quantity = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.Quantity);
                buildOrder.SaleAmount = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.SaleAmount);
                buildOrder.OriginalAmount = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.OriginalAmount);
                buildOrder.DiscountAmount = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.DiscountAmount);
                buildOrder.ChargeAmount = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.ChargeAmount);
                buildOrder.CouponAmountByDeposit = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.CouponAmountByDeposit);
                buildOrder.CouponAmountByRent = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.CouponAmountByRent);
                buildOrder.CouponAmountByShop = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).Sum(m => m.CouponAmountByShop);
                buildOrder.Childs = buildOrderChilds.Where(m => m.SellChannelRefId == l_buildOrder.SellChannelRefId).ToList();
                buildOrders.Add(buildOrder);
            }


            return buildOrders;
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

            using (TransactionScope ts = new TransactionScope())
            {
                LogUtil.Info("payTransId:" + payTransId);

                var d_payTrans = CurrentDb.PayTrans.Where(m => m.Id == payTransId).FirstOrDefault();

                if (d_payTrans == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", payTransId));
                }

                if (d_payTrans.PayStatus == E_PayStatus.PaySuccess)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号({0})已经支付通知成功", payTransId));
                }

                if (d_payTrans.PayStatus == E_PayStatus.WaitPay || d_payTrans.PayStatus == E_PayStatus.Paying)
                {
                    LogUtil.Info("进入PaySuccess修改订单,开始");

                    operater = d_payTrans.Creator;

                    d_payTrans.PayPartner = payPartner;
                    d_payTrans.PayPartnerPayTransId = payPartnerPayTransId;
                    d_payTrans.PayWay = payWay;
                    d_payTrans.PayStatus = E_PayStatus.PaySuccess;
                    d_payTrans.PayedTime = DateTime.Now;

                    if (pms != null)
                    {
                        if (pms.ContainsKey("clientUserName"))
                        {
                            if (!string.IsNullOrEmpty(pms["clientUserName"]))
                            {
                                d_payTrans.ClientUserName = pms["clientUserName"];
                            }
                        }
                    }

                    var d_clientUser = CurrentDb.SysClientUser.Where(m => m.Id == d_payTrans.ClientUserId).FirstOrDefault();

                    var orderIds = d_payTrans.OrderIds.Split(',');
                    var d_orders = CurrentDb.Order.Where(m => orderIds.Contains(m.Id)).ToList();
                    foreach (var d_order in d_orders)
                    {
                        d_order.ClientUserId = d_payTrans.ClientUserId;
                        d_order.ClientUserName = d_payTrans.ClientUserName;
                        d_order.PayedTime = d_payTrans.PayedTime;
                        d_order.PayStatus = d_payTrans.PayStatus;
                        d_order.PayWay = d_payTrans.PayWay;
                        d_order.PayPartner = payPartner;
                        d_order.PayPartnerPayTransId = payPartnerPayTransId;
                        d_order.MendTime = DateTime.Now;
                        d_order.Mender = operater;

                        switch (d_order.ReceiveMode)
                        {
                            case E_ReceiveMode.Delivery:
                                d_order.Status = E_OrderStatus.Payed;
                                d_order.PickupFlowLastDesc = "您已成功支付，等待发货";
                                d_order.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.StoreSelfTake:
                                d_order.Status = E_OrderStatus.Payed;
                                d_order.PickupFlowLastDesc = string.Format("您已成功支付，请到店铺【{0}】,出示取货码【{1}】，给店员", d_order.ReceptionMarkName, d_order.PickupCode);
                                d_order.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.MachineSelfTake:
                                d_order.Status = E_OrderStatus.Payed;
                                d_order.PickupFlowLastDesc = string.Format("您已成功支付，请到店铺【{0}】找到机器【{1}】,在取货界面输入取货码【{2}】", d_order.ReceptionMarkName, d_order.SellChannelRefId, d_order.PickupCode);
                                d_order.PickupFlowLastTime = DateTime.Now;
                                break;
                            case E_ReceiveMode.MemberFee:
                                d_order.Status = E_OrderStatus.Completed;
                                d_order.PickupFlowLastDesc = "您已成功支付";
                                d_order.PickupFlowLastTime = DateTime.Now;
                                d_order.IsNoDisplayClient = false;
                                break;
                        }

                        var d_orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_order.Id).ToList();

                        foreach (var d_orderSub in d_orderSubs)
                        {
                            d_orderSub.PayWay = d_order.PayWay;
                            d_orderSub.PayStatus = d_order.PayStatus;
                            d_orderSub.PayedTime = d_order.PayedTime;
                            d_orderSub.ClientUserId = d_order.ClientUserId;
                            d_orderSub.Mender = operater;
                            d_orderSub.MendTime = DateTime.Now;

                            if (d_orderSub.ShopMethod == E_OrderShopMethod.Shop)
                            {
                                #region Shop
                                d_orderSub.PickupStatus = E_OrderPickupStatus.WaitPickup;
                                d_orderSub.PickupFlowLastDesc = d_order.PickupFlowLastDesc;
                                d_orderSub.PickupFlowLastTime = d_order.PickupFlowLastTime;

                                BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPaySuccess, d_order.AppId, d_order.MerchId, d_order.StoreId, d_orderSub.SellChannelRefId, d_orderSub.CabinetId, d_orderSub.SlotId, d_orderSub.PrdProductSkuId, d_orderSub.Quantity);

                                #endregion 
                            }
                            else if (d_orderSub.ShopMethod == E_OrderShopMethod.Rent)
                            {
                                #region 
                                d_orderSub.PickupStatus = E_OrderPickupStatus.WaitPickup;
                                d_orderSub.PickupFlowLastDesc = d_order.PickupFlowLastDesc;
                                d_orderSub.PickupFlowLastTime = d_order.PickupFlowLastTime;


                                var d_rentOrder = new RentOrder();
                                d_rentOrder.Id = IdWorker.Build(IdType.NewGuid);
                                d_rentOrder.MerchId = d_orderSub.MerchId;
                                d_rentOrder.OrdeId = d_orderSub.OrderId;
                                d_rentOrder.ClientUserId = d_orderSub.ClientUserId;
                                d_rentOrder.SpuId = d_orderSub.PrdProductId;
                                d_rentOrder.SkuId = d_orderSub.PrdProductSkuId;
                                d_rentOrder.SkuName = d_orderSub.PrdProductSkuName;
                                d_rentOrder.SkuCumCode = d_orderSub.PrdProductSkuCumCode;
                                d_rentOrder.SkuBarCode = d_orderSub.PrdProductSkuBarCode;
                                d_rentOrder.SkuSpecDes = d_orderSub.PrdProductSkuSpecDes;
                                d_rentOrder.SkuProducer = d_orderSub.PrdProductSkuProducer;
                                d_rentOrder.SkuMainImgUrl = d_orderSub.PrdProductSkuMainImgUrl;
                                d_rentOrder.DepositAmount = d_orderSub.ChargeAmount;
                                d_rentOrder.IsPayDeposit = true;
                                d_rentOrder.PayDepositTime = DateTime.Now;
                                d_rentOrder.RentTermUnit = d_orderSub.RentTermUnit;
                                d_rentOrder.RentTermValue = d_orderSub.RentTermValue;
                                d_rentOrder.RentTermUnitText = "月";
                                d_rentOrder.RentAmount = d_orderSub.RentAmount;
                                d_rentOrder.NextPayRentTime = DateTime.Now.AddMonths(d_orderSub.RentTermValue);
                                d_rentOrder.Creator = operater;
                                d_rentOrder.CreateTime = DateTime.Now;
                                CurrentDb.RentOrder.Add(d_rentOrder);

                                var d_rentOrderTransRecord = new RentOrderTransRecord();
                                d_rentOrderTransRecord.Id = IdWorker.Build(IdType.NewGuid);
                                d_rentOrderTransRecord.MerchId = d_orderSub.MerchId;
                                d_rentOrderTransRecord.OrdeId = d_orderSub.OrderId;
                                d_rentOrderTransRecord.RentOrderId = d_rentOrder.Id;
                                d_rentOrderTransRecord.ClientUserId = d_orderSub.ClientUserId;
                                d_rentOrderTransRecord.TransType = E_RentTransTpye.Pay;
                                d_rentOrderTransRecord.Amount = d_orderSub.ChargeAmount;
                                d_rentOrderTransRecord.TransTime = DateTime.Now;
                                d_rentOrderTransRecord.AmountType = E_RentAmountType.DepositAndRent;
                                d_rentOrderTransRecord.NextPayRentTime = DateTime.Now.AddMonths(d_orderSub.RentTermValue);
                                d_rentOrderTransRecord.Creator = operater;
                                d_rentOrderTransRecord.CreateTime = DateTime.Now;
                                d_rentOrderTransRecord.Description = string.Format("您已支付设备押金和租金，合计：{0}", d_orderSub.ChargeAmount);
                                CurrentDb.RentOrderTransRecord.Add(d_rentOrderTransRecord);

                                #endregion
                            }
                            else if (d_orderSub.ShopMethod == E_OrderShopMethod.MemberFee)
                            {
                                #region MemberFee
                                d_orderSub.PickupStatus = E_OrderPickupStatus.Taked;
                                d_orderSub.PickupFlowLastDesc = d_order.PickupFlowLastDesc;
                                d_orderSub.PickupFlowLastTime = d_order.PickupFlowLastTime;


                                var d_memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == d_orderSub.MerchId && m.Id == d_orderSub.PrdProductSkuId).FirstOrDefault();
                                if (d_memberFeeSt != null)
                                {
                                    string[] couponIds = d_memberFeeSt.CouponIds.ToJsonObject<string[]>();

                                    if (couponIds != null && couponIds.Length > 0)
                                    {
                                        var d_coupons = CurrentDb.Coupon.Where(m => couponIds.Contains(m.Id) && m.MerchId == d_orderSub.MerchId).ToList();

                                        foreach (var d_coupon in d_coupons)
                                        {
                                            for (int i = 0; i < d_coupon.PerLimitNum; i++)
                                            {
                                                var d_clientCoupon = new ClientCoupon();
                                                d_clientCoupon.Id = IdWorker.Build(IdType.NewGuid);
                                                d_clientCoupon.Sn = "";
                                                d_clientCoupon.MerchId = d_orderSub.MerchId;
                                                d_clientCoupon.ClientUserId = d_orderSub.ClientUserId;
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
                                    }

                                    if (d_clientUser != null)
                                    {
                                        var memberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.Id == d_memberFeeSt.LevelStId).FirstOrDefault();
                                        if (memberLevelSt != null)
                                        {
                                            d_clientUser.MemberLevel = memberLevelSt.Level;

                                            switch (d_memberFeeSt.FeeType)
                                            {
                                                case E_MemberFeeSt_FeeType.Year:
                                                    d_clientUser.MemberExpireTime = DateTime.Now.AddYears(1);
                                                    break;
                                                case E_MemberFeeSt_FeeType.Quarter:
                                                    d_clientUser.MemberExpireTime = DateTime.Now.AddMonths(3);
                                                    break;
                                                case E_MemberFeeSt_FeeType.Month:
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
                        }

                        if (!string.IsNullOrEmpty(d_order.CouponIdsByShop))
                        {
                            string[] l_couponIdsByShop = d_order.CouponIdsByShop.ToJsonObject<string[]>();

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

                        if (!string.IsNullOrEmpty(d_order.CouponIdByRent))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_order.CouponIdByRent).FirstOrDefault();
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

                        if (!string.IsNullOrEmpty(d_order.CouponIdByDeposit))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_order.CouponIdByDeposit).FirstOrDefault();
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
                        d_orderPickupLog.OrderId = d_order.Id;
                        d_orderPickupLog.SellChannelRefType = d_order.SellChannelRefType;
                        d_orderPickupLog.SellChannelRefId = d_order.SellChannelRefId;
                        d_orderPickupLog.UniqueId = d_order.Id;
                        d_orderPickupLog.UniqueType = E_UniqueType.Order;
                        d_orderPickupLog.ActionRemark = d_order.PickupFlowLastDesc;
                        d_orderPickupLog.ActionTime = d_order.PickupFlowLastTime;
                        d_orderPickupLog.Remark = "";
                        d_orderPickupLog.CreateTime = DateTime.Now;
                        d_orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(d_orderPickupLog);
                        CurrentDb.SaveChanges();
                    }


                    CurrentDb.SaveChanges();
                    ts.Complete();

                    LogUtil.Info("进入PaySuccess修改订单,结束");

                    Task4Factory.Tim2Global.Exit(Task4TimType.PayTrans2CheckStatus, d_payTrans.Id);

                    foreach (var d_order in d_orders)
                    {
                        Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckReservePay, d_order.Id);
                        MqFactory.Global.PushEventNotify(operater, d_order.AppId, d_order.MerchId, d_order.StoreId, "", EventCode.OrderPaySuccess, string.Format("订单号：{0}，支付成功", d_order.Id));

                    }
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
                var d_order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

                if (d_order == null)
                {
                    LogUtil.Info(string.Format("该订单号:{0},找不到", orderId));
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该订单号:{0},找不到", orderId));
                }

                if (d_order.PayStatus == E_PayStatus.PayCancle)
                {
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经取消");
                }

                if (d_order.PayStatus == E_PayStatus.PayTimeout)
                {
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经超时");
                }

                if (d_order.PayStatus == E_PayStatus.PaySuccess)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经支付成功");
                }


                operater = d_order.Creator;

                if (d_order.PayStatus != E_PayStatus.PaySuccess)
                {
                    d_order.Status = E_OrderStatus.Canceled;
                    d_order.CancelOperator = operater;
                    d_order.CanceledTime = DateTime.Now;
                    d_order.CancelReason = cancelReason;
                    d_order.Mender = operater;
                    d_order.MendTime = DateTime.Now;

                    if (cancleType == E_OrderCancleType.PayCancle)
                    {
                        d_order.PayStatus = E_PayStatus.PayCancle;
                    }
                    else if (cancleType == E_OrderCancleType.PayTimeout)
                    {
                        d_order.PayStatus = E_PayStatus.PayTimeout;
                    }

                    var d_orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_order.Id).ToList();

                    foreach (var d_orderSub in d_orderSubs)
                    {

                        d_orderSub.Mender = operater;
                        d_orderSub.MendTime = DateTime.Now;

                        if (cancleType == E_OrderCancleType.PayCancle)
                        {
                            d_orderSub.PayStatus = E_PayStatus.PayCancle;
                        }
                        else if (cancleType == E_OrderCancleType.PayTimeout)
                        {
                            d_orderSub.PayStatus = E_PayStatus.PayTimeout;
                        }

                        d_orderSub.PickupStatus = E_OrderPickupStatus.Canceled;

                        if (d_orderSub.ShopMethod == E_OrderShopMethod.Shop)
                        {
                            BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderCancle, d_order.AppId, d_order.MerchId, d_order.StoreId, d_orderSub.SellChannelRefId, d_orderSub.CabinetId, d_orderSub.SlotId, d_orderSub.PrdProductSkuId, d_orderSub.Quantity);
                        }

                    }

                    CurrentDb.SaveChanges();

                    var d_orders = CurrentDb.Order.Where(m => m.UnId == d_order.UnId && m.Status != E_OrderStatus.Canceled).ToList();
                    if (d_orders.Count == 0)
                    {
                        if (!string.IsNullOrEmpty(d_order.CouponIdsByShop))
                        {
                            string[] l_couponIdsByShops = d_order.CouponIdsByShop.ToJsonObject<string[]>();

                            foreach (var l_couponIdByShop in l_couponIdsByShops)
                            {
                                var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == l_couponIdByShop).FirstOrDefault();
                                if (d_clientCoupon != null)
                                {
                                    d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(d_order.CouponIdByRent))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_order.CouponIdByRent).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                            }
                        }

                        if (!string.IsNullOrEmpty(d_order.CouponIdByDeposit))
                        {
                            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == d_order.CouponIdByDeposit).FirstOrDefault();
                            if (d_clientCoupon != null)
                            {
                                d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                            }
                        }
                    }

                    CurrentDb.SaveChanges();

                    ts.Complete();

                    Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckReservePay, d_order.Id);

                    if (!string.IsNullOrEmpty(d_order.PayTransId))
                    {
                        Task4Factory.Tim2Global.Exit(Task4TimType.PayTrans2CheckStatus, d_order.PayTransId);
                    }

                    MqFactory.Global.PushEventNotify(operater, d_order.AppId, d_order.MerchId, d_order.StoreId, "", EventCode.OrderCancle, string.Format("订单号：{0}，取消成功", d_order.Id));

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
                                d_order.Receiver = rm_Delivery.Delivery.Consignee;
                                d_order.ReceiverPhoneNumber = rm_Delivery.Delivery.PhoneNumber;
                                d_order.ReceptionId = rm_Delivery.Delivery.Id;
                                d_order.ReceptionAreaCode = rm_Delivery.Delivery.AreaCode;
                                d_order.ReceptionAreaName = rm_Delivery.Delivery.AreaName;
                                d_order.ReceptionAddress = rm_Delivery.Delivery.Address;
                            }

                            var rm_StoreSelfTake = rop.Blocks.Where(m => m.ReceiveMode == E_ReceiveMode.StoreSelfTake).FirstOrDefault();
                            if (rm_StoreSelfTake != null)
                            {
                                d_order.ReceiveMode = E_ReceiveMode.StoreSelfTake;
                                d_order.Receiver = rm_StoreSelfTake.SelfTake.Consignee;
                                d_order.ReceiverPhoneNumber = rm_StoreSelfTake.SelfTake.PhoneNumber;
                                d_order.ReceptionAreaCode = rm_StoreSelfTake.SelfTake.AreaCode;
                                d_order.ReceptionAreaName = rm_StoreSelfTake.SelfTake.AreaName;
                                d_order.ReceptionAddress = rm_StoreSelfTake.SelfTake.Address;
                                d_order.ReceptionMarkName = rm_StoreSelfTake.SelfTake.MarkName;


                                if (rm_StoreSelfTake.BookTime != null && !string.IsNullOrEmpty(rm_StoreSelfTake.BookTime.Value))
                                {
                                    //1为具体时间值 例如 2020-11-24 13:00
                                    //2为时间段区间 例如 2020-11-24 13:00,2020-11-24 13:00
                                    if (rm_StoreSelfTake.BookTime.Type == 1)
                                    {
                                        if (Lumos.CommonUtil.IsDateTime(rm_StoreSelfTake.BookTime.Value))
                                        {
                                            d_order.ReceptionBookStartTime = DateTime.Parse(rm_StoreSelfTake.BookTime.Value);
                                        }
                                    }
                                    else if (rm_StoreSelfTake.BookTime.Type == 2)
                                    {
                                        string[] arr_time = rm_StoreSelfTake.BookTime.Value.Split(',');
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
                    var orderAttach = new BLL.Biz.OrderAttachModel();

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

                                    orderAttach.MerchId = payTrans.MerchId;
                                    orderAttach.StoreId = payTrans.StoreId;
                                    orderAttach.PayCaller = rop.PayCaller;

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

                                    orderAttach.MerchId = payTrans.MerchId;
                                    orderAttach.StoreId = payTrans.StoreId;
                                    orderAttach.PayCaller = rop.PayCaller;

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

            var order = CurrentDb.Order.Where(m => m.Id == orderId && m.SellChannelRefId == machineId).FirstOrDefault();
            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == orderId && m.SellChannelRefId == machineId).ToList();

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

            using (TransactionScope ts = new TransactionScope())
            {

                if (string.IsNullOrEmpty(rop.Remark))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "处理的备注信息不能为空");
                }

                List<Order> orders = new List<Order>();

                foreach (var item in rop.Items)
                {
                    LogUtil.Info("Item:" + item.ItemId);

                    var order = CurrentDb.Order.Where(m => m.Id == item.ItemId).FirstOrDefault();
                    if (order == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单信息不存在");
                    }

                    if (!order.ExIsHappen)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不是异常状态，不能处理");
                    }

                    if (order.ExIsHandle)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该异常订单已经处理完毕");
                    }


                    if (item.IsRefund)
                    {

                        var payRefunds = CurrentDb.PayRefund.Where(m => m.OrderId == item.ItemId).ToList();

                        decimal refundedAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
                        decimal refundingAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Sum(m => m.ApplyAmount);

                        if (item.RefundAmount > (order.ChargeAmount - (refundedAmount + refundingAmount)))
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "退款的金额不能大于可退金额");
                        }

                        var payTran = CurrentDb.PayTrans.Where(m => m.Id == order.PayTransId).FirstOrDefault();

                        if (item.RefundAmount > payTran.ChargeAmount)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "退款的金额不能大于可退金额");
                        }

                        string payRefundId = IdWorker.Build(IdType.PayRefundId);

                        var payRefund = new PayRefund();
                        payRefund.Id = payRefundId;
                        payRefund.MerchId = order.MerchId;
                        payRefund.MerchName = order.MerchName;
                        payRefund.StoreId = order.StoreId;
                        payRefund.StoreName = order.StoreName;
                        payRefund.ClientUserId = order.ClientUserId;
                        payRefund.ClientUserName = order.ClientUserName;
                        payRefund.OrderId = order.Id;
                        payRefund.PayPartnerPayTransId = order.PayPartnerPayTransId;
                        payRefund.PayTransId = order.PayTransId;
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

                    var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == item.ItemId && m.ExPickupIsHappen == true && m.ExPickupIsHandle == false && m.PickupStatus == E_OrderPickupStatus.Exception).ToList();


                    foreach (var orderSub in orderSubs)
                    {
                        LogUtil.Info("orderSubs");

                        var detailItem = item.Uniques.Where(m => m.UniqueId == orderSub.Id).FirstOrDefault();
                        if (detailItem == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单里对应商品异常记录未找到");
                        }

                        if (detailItem.SignStatus != 1 && detailItem.SignStatus != 2)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不能处理该异常状态:" + detailItem.SignStatus);
                        }

                        if (detailItem.SignStatus == 1)
                        {

                            if (orderSub.PickupStatus != E_OrderPickupStatus.Taked && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                            {
                                BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete, AppId.MERCH, orderSub.MerchId, orderSub.StoreId, orderSub.SellChannelRefId, orderSub.CabinetId, orderSub.SlotId, orderSub.PrdProductSkuId, 1);
                            }

                            orderSub.ExPickupIsHandle = true;
                            orderSub.ExPickupHandleTime = DateTime.Now;
                            orderSub.ExPickupHandleSign = E_OrderExPickupHandleSign.Taked;
                            orderSub.PickupStatus = E_OrderPickupStatus.ExPickupSignTaked;


                            var orderPickupLog = new OrderPickupLog();
                            orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                            orderPickupLog.OrderId = orderSub.OrderId;
                            orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                            orderPickupLog.SellChannelRefId = orderSub.SellChannelRefId;
                            orderPickupLog.UniqueId = orderSub.Id;
                            orderPickupLog.UniqueType = E_UniqueType.OrderSub;
                            orderPickupLog.PrdProductSkuId = orderSub.PrdProductSkuId;
                            orderPickupLog.CabinetId = orderSub.CabinetId;
                            orderPickupLog.SlotId = orderSub.SlotId;
                            orderPickupLog.Status = E_OrderPickupStatus.ExPickupSignTaked;
                            orderPickupLog.ActionRemark = "人为标识已取货";
                            orderPickupLog.Remark = "";
                            orderPickupLog.CreateTime = DateTime.Now;
                            orderPickupLog.Creator = operater;
                            CurrentDb.OrderPickupLog.Add(orderPickupLog);
                        }
                        else if (detailItem.SignStatus == 2)
                        {
                            if (orderSub.PickupStatus != E_OrderPickupStatus.Taked && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                            {
                                BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete, AppId.STORETERM, orderSub.MerchId, orderSub.StoreId, orderSub.SellChannelRefId, orderSub.CabinetId, orderSub.SlotId, orderSub.PrdProductSkuId, 1);
                            }

                            orderSub.ExPickupIsHandle = true;
                            orderSub.ExPickupHandleTime = DateTime.Now;
                            orderSub.ExPickupHandleSign = E_OrderExPickupHandleSign.UnTaked;
                            orderSub.PickupStatus = E_OrderPickupStatus.ExPickupSignUnTaked;

                            var orderPickupLog = new OrderPickupLog();
                            orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                            orderPickupLog.OrderId = orderSub.OrderId;
                            orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                            orderPickupLog.SellChannelRefId = orderSub.SellChannelRefId;
                            orderPickupLog.UniqueId = orderSub.Id;
                            orderPickupLog.UniqueType = E_UniqueType.OrderSub;
                            orderPickupLog.PrdProductSkuId = orderSub.PrdProductSkuId;
                            orderPickupLog.CabinetId = orderSub.CabinetId;
                            orderPickupLog.SlotId = orderSub.SlotId;
                            orderPickupLog.Status = E_OrderPickupStatus.ExPickupSignUnTaked;
                            orderPickupLog.ActionRemark = "人为标识未取货";
                            orderPickupLog.Remark = "";
                            orderPickupLog.CreateTime = DateTime.Now;
                            orderPickupLog.Creator = operater;
                            CurrentDb.OrderPickupLog.Add(orderPickupLog);
                        }
                    }

                    order.ExIsHandle = true;
                    order.ExHandleTime = DateTime.Now;
                    order.ExHandleRemark = rop.Remark;
                    order.Status = E_OrderStatus.Completed;
                    order.CompletedTime = DateTime.Now;

                    orders.Add(order);


                    LogUtil.Info("orders");
                }

                LogUtil.Info("IsRunning");

                if (rop.IsRunning)
                {
                    if (string.IsNullOrEmpty(rop.MachineId))
                    {
                        var machineIds = orders.Where(m => m.ReceiveMode == E_ReceiveMode.MachineSelfTake).Select(m => m.SellChannelRefId).ToArray();

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


                //MqFactory.Global.PushEventNotify(operater, AppId.MERCH, orderSub.MerchId, "", "", EventCode.OrderHandleExOrder, string.Format("处理异常子订单号：{0}，备注：{1}", orderSub.Id, orderSub.ExHandleRemark));


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理成功");
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
