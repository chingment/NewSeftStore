using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class CouponService : BaseDbContext
    {
        private bool GetCanSelected(E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, decimal atLeastAmount, string merchId, string storeId, string clientUserId, List<OrderConfirmProductSkuModel> productSkus)
        {
            if (useAreaType == E_Coupon_UseAreaType.All)
            {
                var sum_amount = productSkus.Sum(m => m.SaleAmount);

                if (atLeastAmount > sum_amount)
                    return false;

                return true;
            }
            else if (useAreaType == E_Coupon_UseAreaType.Store)
            {
                var arr_useAreaValue = useAreaValue.ToJsonObject<List<UseAreaValueModel>>();

                if (arr_useAreaValue != null)
                {
                    if (arr_useAreaValue.Where(m => m.Id == storeId).Count() > 0)
                    {
                        var sum_amount = productSkus.Sum(m => m.SaleAmount);

                        if (atLeastAmount > sum_amount)
                            return false;

                        return true;
                    }
                }
            }
            else if (useAreaType == E_Coupon_UseAreaType.ProductKind)
            {
                var kinds1 = useAreaValue.ToJsonObject<JArray>();

                List<string> kindIds2 = new List<string>();

                var productSkuIds = productSkus.Select(m => m.Id).ToList();

                foreach (var productSkuId in productSkuIds)
                {
                    var r_productSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);

                    if (r_productSku != null)
                    {
                        foreach (var kind1 in kinds1)
                        {
                            if (kind1["id"].ToString() == r_productSku.KindId3.ToString())
                                return true;
                        }
                    }
                }

            }
            else if (useAreaType == E_Coupon_UseAreaType.ProductSpu)
            {
                var prodcuts1 = useAreaValue.ToJsonObject<JArray>();

                var productSkuIds = productSkus.Select(m => m.Id).ToList();

                foreach (var productSkuId in productSkuIds)
                {
                    var r_productSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);

                    if (r_productSku != null)
                    {
                        foreach (var prodcut1 in prodcuts1)
                        {
                            if (prodcut1["id"].ToString() == r_productSku.ProductId)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool GetCanSelected(E_OrderShopMethod shopMethod, E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, decimal atLeastAmount, E_ClientCouponStatus couponStatus, DateTime validTimeStart, DateTime validTimeEnd, string merchId, string storeId, string clientUserId, List<OrderConfirmProductSkuModel> productSkus)
        {

            if (shopMethod == E_OrderShopMethod.Unknow)
                return false;
            if (useAreaType == E_Coupon_UseAreaType.Unknow)
                return false;
            if (faceType == E_Coupon_FaceType.Unknow)
                return false;
            if (productSkus == null || productSkus.Count == 0)
                return false;
            if (couponStatus == E_ClientCouponStatus.Unknow)
                return false;

            if (couponStatus == E_ClientCouponStatus.Delete || couponStatus == E_ClientCouponStatus.Used || couponStatus == E_ClientCouponStatus.Expired || couponStatus == E_ClientCouponStatus.Frozen)
                return false;

            if (validTimeStart > DateTime.Now)
                return false;

            if (validTimeEnd < DateTime.Now)
                return false;

            if (string.IsNullOrEmpty(merchId) || string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(clientUserId))
                return false;

            if (shopMethod == E_OrderShopMethod.Shop)
            {
                if (faceType == E_Coupon_FaceType.ShopVoucher || faceType == E_Coupon_FaceType.ShopDiscount)
                {
                    return GetCanSelected(useAreaType, useAreaValue, faceType, faceValue, atLeastAmount, merchId, storeId, clientUserId, productSkus);
                }
            }
            else if (shopMethod == E_OrderShopMethod.Rent)
            {
                if (faceType == E_Coupon_FaceType.DepositVoucher || faceType == E_Coupon_FaceType.RentVoucher)
                {
                    return GetCanSelected(useAreaType, useAreaValue, faceType, faceValue, atLeastAmount, merchId, storeId, clientUserId, productSkus);
                }
            }

            return false;
        }

        public int GetCanUseCount(E_OrderShopMethod shopMethod, E_Coupon_FaceType[] faceTypes, List<OrderConfirmProductSkuModel> productSkus, string merchId, string storeId, string clientUserId)
        {
            RopCouponMy rup = new RopCouponMy();
            rup.StoreId = storeId;
            rup.ShopMethod = shopMethod;
            rup.ProductSkus = productSkus;
            rup.FaceTypes = faceTypes;

            var ret = My("", clientUserId, rup);

            if (ret == null)
                return 0;

            if (ret.Result != ResultType.Success)
                return 0;

            return ret.Data.Coupons.Where(m => m.CanSelected).Count();
        }

        private CouponModel CovertCouponModel(string id, string name, E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, decimal atLeastAmount)
        {
            var model = new CouponModel();

            model.Id = id;
            model.Name = name;

            switch (faceType)
            {
                case E_Coupon_FaceType.ShopVoucher:
                    model.FaceValue = faceValue.ToF2Price();
                    model.FaceUnit = "元";
                    if (atLeastAmount > 0)
                    {
                        model.FaceTip = string.Format("满{0}元使用", atLeastAmount);
                    }
                    else
                    {
                        model.FaceTip = string.Format("代金券");
                    }
                    break;
                case E_Coupon_FaceType.ShopDiscount:
                    model.FaceValue = faceValue.ToF2Price();
                    model.FaceUnit = "元";
                    if (atLeastAmount > 0)
                    {
                        model.FaceTip = string.Format("满{0}元使用", atLeastAmount);
                    }
                    else
                    {
                        model.FaceTip = string.Format("代金券");
                    }
                    break;
                case E_Coupon_FaceType.RentVoucher:
                    model.FaceValue = faceValue.ToF2Price();
                    model.FaceUnit = "元";
                    if (atLeastAmount > 0)
                    {
                        model.FaceTip = string.Format("满{0}元使用", atLeastAmount);
                    }
                    else
                    {
                        model.FaceTip = string.Format("代金券");
                    }
                    break;
                case E_Coupon_FaceType.DepositVoucher:
                    model.FaceValue = faceValue.ToF2Price();
                    model.FaceUnit = "元";
                    if (atLeastAmount > 0)
                    {
                        model.FaceTip = string.Format("满{0}元使用", atLeastAmount);
                    }
                    else
                    {
                        model.FaceTip = string.Format("代金券");
                    }
                    break;
            }

            var arr = useAreaValue.ToJsonObject<List<UseAreaValueModel>>();

            switch (useAreaType)
            {
                case E_Coupon_UseAreaType.All:
                    model.Description = "全场通用";
                    break;
                case E_Coupon_UseAreaType.Store:
                    model.Description = "指定店铺使用";
                    if (arr != null && arr.Count > 0)
                    {
                        model.Description = string.Format("指定店铺[{0}]使用", string.Join(",", arr.Select(m => m.Name)));
                    }
                    break;
                case E_Coupon_UseAreaType.ProductKind:
                    model.Description = "指定品类使用";
                    if (arr != null && arr.Count > 0)
                    {
                        model.Description = string.Format("指定品类[{0}]使用", string.Join(",", arr.Select(m => m.Name)));
                    }
                    break;
                case E_Coupon_UseAreaType.ProductSpu:
                    model.Description = "指定商品使用";
                    if (arr != null && arr.Count > 0)
                    {
                        model.Description = string.Format("指定商品[{0}]使用", string.Join(",", arr.Select(m => m.Name)));
                    }
                    break;
            }

            return model;
        }

        public CustomJsonResult<RetCouponMy> My(string operater, string clientUserId, RopCouponMy rop)
        {
            var result = new CustomJsonResult<RetCouponMy>();

            var ret = new RetCouponMy();

            var query = (from u in CurrentDb.ClientCoupon
                         join m in CurrentDb.Coupon on u.CouponId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         select new { u.Id, u.ClientUserId, u.MerchId, tt.Name, tt.UseAreaType, tt.UseAreaValue, u.Status, u.ValidEndTime, u.ValidStartTime, tt.FaceType, tt.FaceValue, tt.AtLeastAmount });

            if (!rop.IsGetHis)
            {
                query = query.Where(u => u.ClientUserId == clientUserId && u.Status == E_ClientCouponStatus.WaitUse && u.ValidEndTime > DateTime.Now);
            }
            else
            {
                query = query.Where(m => m.ClientUserId == clientUserId && (m.Status == E_ClientCouponStatus.Used || m.Status != E_ClientCouponStatus.Expired) && m.ValidEndTime < DateTime.Now);
            }

            if (rop.FaceTypes != null)
            {
                query = query.Where(m => rop.FaceTypes.Contains(m.FaceType));
            }


            var list = query.OrderBy(m => m.Name).ToList();


            var store = BizFactory.Store.GetOne(rop.StoreId);

            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            foreach (var productSku in rop.ProductSkus)
            {
                if (productSku.ShopMethod == E_OrderShopMethod.Shop)
                {
                    #region Shop
                    var r_productSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.StoreId, store.GetSellChannelRefIds(productSku.ShopMode), productSku.Id);
                    if (r_productSku != null)
                    {
                        productSku.Name = r_productSku.Name;
                        productSku.MainImgUrl = r_productSku.MainImgUrl;
                        productSku.ProductId = r_productSku.ProductId;
                        productSku.KindId3 = r_productSku.KindId3;
                        productSku.RentTermUnit = E_RentTermUnit.Month;
                        productSku.RentTermUnitText = "月";
                        productSku.RentTermValue = 1;
                        productSku.RentAmount = r_productSku.Stocks[0].RentMhPrice;
                        productSku.DepositAmount = r_productSku.Stocks[0].DepositPrice;
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
                    }

                    productSku.OriginalPrice = r_productSku.Stocks[0].SalePrice;
                    productSku.SaleAmount = productSku.Quantity * productSku.SalePrice;
                    productSku.OriginalAmount = productSku.Quantity * productSku.OriginalPrice;

                    #endregion
                }
                else if (productSku.ShopMethod == E_OrderShopMethod.Rent)
                {
                    #region Rent
                    var r_productSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.StoreId, store.GetSellChannelRefIds(productSku.ShopMode), productSku.Id);
                    if (r_productSku != null)
                    {
                        productSku.Name = r_productSku.Name;
                        productSku.MainImgUrl = r_productSku.MainImgUrl;
                        productSku.ProductId = r_productSku.ProductId;
                        productSku.KindId3 = r_productSku.KindId3;
                        productSku.RentTermUnit = E_RentTermUnit.Month;
                        productSku.RentTermUnitText = "月";
                        productSku.RentTermValue = 1;
                        productSku.RentAmount = r_productSku.Stocks[0].RentMhPrice;
                        productSku.DepositAmount = r_productSku.Stocks[0].DepositPrice;
                        productSku.SalePrice = r_productSku.Stocks[0].RentMhPrice + r_productSku.Stocks[0].DepositPrice;
                        productSku.OriginalPrice = r_productSku.Stocks[0].RentMhPrice + r_productSku.Stocks[0].DepositPrice;
                        productSku.SaleAmount = productSku.Quantity * productSku.SalePrice;
                        productSku.OriginalAmount = productSku.Quantity * productSku.OriginalPrice;
                    }

                    #endregion
                }
                else if (productSku.ShopMethod == E_OrderShopMethod.MemberFee)
                {
                    #region 
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
                        productSku.KindId3 = 0;
                    }

                    #endregion
                }
            }

            foreach (var item in list)
            {
                var couponModel = CovertCouponModel(item.Id, item.Name, item.UseAreaType, item.UseAreaValue, item.FaceType, item.FaceValue, item.AtLeastAmount);

                couponModel.ValidDate = "有效到" + item.ValidEndTime.ToUnifiedFormatDate();

                if (rop.CouponIds != null)
                {
                    if (rop.CouponIds.Contains(item.Id))
                    {
                        couponModel.IsSelected = true;
                    }
                }

                couponModel.CanSelected = GetCanSelected(rop.ShopMethod, item.UseAreaType, item.UseAreaValue, item.FaceType, item.FaceValue, item.AtLeastAmount, item.Status, item.ValidStartTime, item.ValidEndTime, item.MerchId, rop.StoreId, clientUserId, rop.ProductSkus);

                ret.Coupons.Add(couponModel);
            }

            result = new CustomJsonResult<RetCouponMy>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult RevCenterSt(string operater, string clientUserId, RupCouponRevCenterSt rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetCouponRevCenterSt();

            var d_couponRevCenterSt = CurrentDb.CouponRevCenterSt.Where(m => m.MerchId == rup.MerchId).FirstOrDefault();

            if (d_couponRevCenterSt != null)
            {
                ret.TopImgUrl = d_couponRevCenterSt.TopImgUrl;

                List<string> l_couponIds = d_couponRevCenterSt.CouponIds.ToJsonObject<List<string>>();


                var query = (from u in CurrentDb.Coupon
                             where l_couponIds.Contains(u.Id)
                             select new { u.Id, u.MerchId, u.Name, u.UseAreaType, u.UseAreaValue, u.FaceType, u.FaceValue, u.AtLeastAmount });

                var list = query.OrderBy(m => m.Name).ToList();

                foreach (var item in list)
                {
                    var couponModel = CovertCouponModel(item.Id, item.Name, item.UseAreaType, item.UseAreaValue, item.FaceType, item.FaceValue, item.AtLeastAmount);


                    ret.Coupons.Add(couponModel);
                }

            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Receive(string operater, string clientUserId, RopCouponReceive rop)
        {
            var result = new CustomJsonResult();

            var d_coupon = CurrentDb.Coupon.Where(m => m.Id == rop.CouponId).FirstOrDefault();

            if (d_coupon == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效优惠券");
            }

            if (d_coupon.IsDelete)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该优惠券已被删除");
            }

            if (d_coupon.StartTime > DateTime.Now || d_coupon.EndTime < DateTime.Now)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该优惠券有效期已过");
            }

            if (d_coupon.IssueQuantity != -1)
            {
                if (d_coupon.ReceivedQuantity >= d_coupon.IssueQuantity)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该优惠券已被领取完");
                }
            }

            var d_clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            var limitMemberLevels = d_coupon.LimitMemberLevels.ToJsonObject<List<string>>();

            if (limitMemberLevels != null)
            {
                if (!limitMemberLevels.Contains(d_clientUser.MemberLevel.ToString()))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoRight, "无资格领取该优惠券");
                }
            }

            var d_clientCoupons = CurrentDb.ClientCoupon.Where(m => m.ClientUserId == clientUserId && m.CouponId == d_coupon.Id).ToList();

            if (d_coupon.PerLimitNum == d_clientCoupons.Count)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您领取的数量已经超过限制");
            }

            if (d_coupon.PerLimitTimeType == E_Coupon_PerLimitTimeType.Day)
            {
                DateTime start = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                DateTime end = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

                d_clientCoupons = d_clientCoupons.Where(m => m.SourceTime >= start && m.SourceTime <= end).ToList();

                if (d_coupon.PerLimitTimeNum == d_clientCoupons.Count)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "当天领取已经超过大限制，请明天再试试");
                }

            }
            else if (d_coupon.PerLimitTimeType == E_Coupon_PerLimitTimeType.Month)
            {
                int nowYear = int.Parse(DateTime.Now.ToString("yyyy"));
                int nowMonth = int.Parse(DateTime.Now.ToString("MM"));

                d_clientCoupons = d_clientCoupons.Where(m => m.SourceTime.Year == nowYear && m.SourceTime.Month == nowMonth
                ).ToList();

                if (d_coupon.PerLimitTimeNum <= d_clientCoupons.Count)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "当月领取已经超过大限制，请下个月再试试");
                }
            }


            var d_clientCoupon = new ClientCoupon();
            d_clientCoupon.Id = IdWorker.Build(IdType.NewGuid);
            d_clientCoupon.Sn = "";
            d_clientCoupon.MerchId = d_coupon.MerchId;
            d_clientCoupon.ClientUserId = clientUserId;
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
            d_clientCoupon.SourceType = E_ClientCouponSourceType.SelfTake;
            d_clientCoupon.SourceObjType = "AppUser";
            d_clientCoupon.SourceObjId = clientUserId;
            d_clientCoupon.SourcePoint = "RevCouponCenter";
            d_clientCoupon.SourceTime = DateTime.Now;
            d_clientCoupon.SourceDes = "领券中心";
            d_clientCoupon.Creator = operater;
            d_clientCoupon.CreateTime = DateTime.Now;
            CurrentDb.ClientCoupon.Add(d_clientCoupon);

            d_coupon.ReceivedQuantity += 1;
            d_coupon.Mender = operater;
            d_coupon.MendTime = DateTime.Now;

            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "领取成功");

            return result;
        }
    }
}
