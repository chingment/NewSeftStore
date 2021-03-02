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
    public class CouponService : BaseService
    {
        private bool GetCanSelected(E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, decimal atLeastAmount, string merchId, string storeId, string clientUserId, List<BuildSku> skus)
        {
            if (useAreaType == E_Coupon_UseAreaType.All)
            {
                var sum_amount = skus.Sum(m => m.SaleAmount);

                if (atLeastAmount > sum_amount)
                    return false;

                return true;
            }
            else if (useAreaType == E_Coupon_UseAreaType.Store)
            {
                var arr_useAreaValue = useAreaValue.ToJsonObject<List<UseAreaModel>>();

                if (arr_useAreaValue != null)
                {
                    if (arr_useAreaValue.Where(m => m.Id == storeId).Count() > 0)
                    {
                        var sum_amount = skus.Sum(m => m.SaleAmount);

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

                var skuIds = skus.Select(m => m.Id).ToList();

                foreach (var skuId in skuIds)
                {
                    var r_Sku = CacheServiceFactory.Product.GetSkuInfo(merchId, skuId);

                    if (r_Sku != null)
                    {
                        foreach (var kind1 in kinds1)
                        {
                            if (kind1["id"].ToString() == r_Sku.KindId3.ToString())
                                return true;
                        }
                    }
                }

            }
            else if (useAreaType == E_Coupon_UseAreaType.ProductSpu)
            {
                var prodcuts1 = useAreaValue.ToJsonObject<JArray>();

                var skuIds = skus.Select(m => m.Id).ToList();

                foreach (var skuId in skuIds)
                {
                    var r_productSku = CacheServiceFactory.Product.GetSkuInfo(merchId, skuId);

                    if (r_productSku != null)
                    {
                        foreach (var prodcut1 in prodcuts1)
                        {
                            if (prodcut1["id"].ToString() == r_productSku.SpuId)
                                return true;
                        }
                    }
                }
            }

            return false;
        }
        public bool GetCanSelected(E_ShopMethod shopMethod, E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, decimal atLeastAmount, E_ClientCouponStatus couponStatus, DateTime validTimeStart, DateTime validTimeEnd, string merchId, string storeId, string clientUserId, List<BuildSku> productSkus)
        {

            if (shopMethod == E_ShopMethod.Unknow)
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

            if (shopMethod == E_ShopMethod.Buy)
            {
                if (faceType == E_Coupon_FaceType.ShopVoucher || faceType == E_Coupon_FaceType.ShopDiscount)
                {
                    return GetCanSelected(useAreaType, useAreaValue, faceType, faceValue, atLeastAmount, merchId, storeId, clientUserId, productSkus);
                }
            }
            else if (shopMethod == E_ShopMethod.Rent)
            {
                if (faceType == E_Coupon_FaceType.DepositVoucher || faceType == E_Coupon_FaceType.RentVoucher)
                {
                    return GetCanSelected(useAreaType, useAreaValue, faceType, faceValue, atLeastAmount, merchId, storeId, clientUserId, productSkus);
                }
            }
            else if (shopMethod == E_ShopMethod.RentFee)
            {
                if (faceType == E_Coupon_FaceType.DepositVoucher || faceType == E_Coupon_FaceType.RentVoucher)
                {
                    return GetCanSelected(useAreaType, useAreaValue, faceType, faceValue, atLeastAmount, merchId, storeId, clientUserId, productSkus);
                }
            }

            return false;
        }
        public RetOrderConfirm.CouponModel GetCanUseCount(E_ShopMethod shopMethod, E_Coupon_FaceType[] faceTypes, List<BuildSku> productSkus, string merchId, string storeId, string clientUserId, List<string> selectCouponIds)
        {
            var model = new RetOrderConfirm.CouponModel();

            RopCouponMy rup = new RopCouponMy();
            rup.StoreId = storeId;
            rup.ShopMethod = shopMethod;
            rup.ProductSkus = productSkus;
            rup.FaceTypes = faceTypes;

            rup.SelectCouponIds = selectCouponIds;

            var ret = My("", clientUserId, rup);

            if (ret == null || ret.Result != ResultType.Success)
            {
                model.TipMsg = "暂无可用券";
                model.TipType = TipType.NoCanUse;
                return model;
            }

            var couponCanUses = ret.Data.Coupons.Where(m => m.CanSelected).ToList();

            if (couponCanUses.Count == 0)
            {
                model.TipMsg = "暂无可用券";
                model.TipType = TipType.NoCanUse;
                return model;
            }

            if (selectCouponIds == null || selectCouponIds.Count == 0)
            {
                var first = couponCanUses.OrderByDescending(m => m.CouponAmount).FirstOrDefault();

                List<string> firstId = new List<string>();
                firstId.Add(first.Id);

                model.TipMsg = string.Format("-{0}", first.CouponAmount);
                model.TipType = TipType.InUse;
                model.SelectedCouponIds = firstId;
                model.CouponAmount = first.CouponAmount;

                return model;
            }
            else
            {
                var selectCouponId = selectCouponIds[0];

                var coupon = couponCanUses.Where(m => m.Id == selectCouponId).FirstOrDefault();
                if (coupon == null)
                {
                    List<string> firstId = new List<string>();

                    model.TipMsg = string.Format("{0}个可用", couponCanUses.Count);
                    model.TipType = TipType.CanUse;
                    model.SelectedCouponIds = firstId;
                    model.CouponAmount = 0;
                }
                else
                {
                    List<string> firstId = new List<string>();
                    firstId.Add(coupon.Id);

                    model.TipMsg = string.Format("-{0}", coupon.CouponAmount);
                    model.TipType = TipType.InUse;
                    model.SelectedCouponIds = firstId;
                    model.CouponAmount = coupon.CouponAmount;
                }


            }


            return model;
        }
        private CouponModel CovertCouponModel(string id, string sn, string name, E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, decimal atLeastAmount, DateTime? validTimeStart, DateTime? validTimeEnd)
        {
            var model = new CouponModel();

            model.Id = id;
            model.Sn = sn;
            model.Name = name;
            model.FaceType = faceType;
            model.WtCode = MyDESCryptoUtil.BuildQrcode2CouponWtCode(id);
            if (validTimeEnd != null)
            {
                model.ValidDate = "有效到" + validTimeEnd.Value.ToUnifiedFormatDate();
            }
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
                    model.FaceUnit = "折";
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
                case E_Coupon_FaceType.EntranceTicket:
                    model.FaceValue = faceValue.ToF2Price();
                    model.FaceUnit = "元";
                    model.FaceTip = string.Format("入场券");
                    break;
            }

            if (faceType == E_Coupon_FaceType.EntranceTicket)
            {
                model.Description = "入场时使用";
            }
            else
            {
                var arr = useAreaValue.ToJsonObject<List<UseAreaModel>>();

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
                         select new { u.Id, u.ClientUserId, u.Sn, u.MerchId, tt.Name, tt.UseAreaType, tt.UseAreaValue, u.Status, u.ValidEndTime, u.ValidStartTime, tt.FaceType, tt.FaceValue, tt.AtLeastAmount });

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

            List<string> selectCouponIds = new List<string>();

            if (rop.SelectCouponIds != null && rop.SelectCouponIds.Count > 0)
            {
                foreach (var selectCouponId in rop.SelectCouponIds)
                {
                    if (selectCouponId != null)
                    {
                        selectCouponIds.Add(selectCouponId);
                    }
                }
            }

            var list = query.OrderBy(m => m.Name).ToList();


            var store = BizFactory.Store.GetOne(rop.StoreId);

            int memberLevel = 0;

            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            if (clientUser != null)
            {
                memberLevel = clientUser.MemberLevel;
            }

            if (store != null)
            {
                BuildOrderTool buildOrderService = new BuildOrderTool(store.MerchId, store.StoreId, memberLevel, selectCouponIds);

                foreach (var productSku in rop.ProductSkus)
                {
                    buildOrderService.AddSku(productSku.Id, productSku.Quantity, productSku.CartId, productSku.ShopMode, productSku.ShopMethod, E_ReceiveMode.Unknow, productSku.ShopId, null);
                }


                rop.ProductSkus = buildOrderService.BuildSkus();
            }

            foreach (var item in list)
            {
                var couponModel = CovertCouponModel(item.Id, item.Sn, item.Name, item.UseAreaType, item.UseAreaValue, item.FaceType, item.FaceValue, item.AtLeastAmount, item.ValidStartTime, item.ValidEndTime);

                if (rop.CouponIds != null)
                {
                    if (rop.CouponIds.Contains(item.Id))
                    {
                        couponModel.IsSelected = true;
                    }
                }

                decimal cal_sum_amount = 0;
                if (item.UseAreaType == E_Coupon_UseAreaType.All)
                {
                    cal_sum_amount = rop.ProductSkus.Sum(m => m.SaleAmount);
                }
                else if (item.UseAreaType == E_Coupon_UseAreaType.Store)
                {
                    cal_sum_amount = rop.ProductSkus.Sum(m => m.SaleAmount);
                }
                else if (item.UseAreaType == E_Coupon_UseAreaType.ProductKind)
                {
                    var olist = item.UseAreaValue.ToJsonObject<List<UseAreaModel>>();
                    if (olist != null)
                    {
                        int[] ids = olist.Select(s => Int32.Parse(s.Id)).ToArray();

                        if (ids != null)
                        {
                            cal_sum_amount = rop.ProductSkus.Where(m => ids.Contains(m.KindId3)).Sum(m => m.SaleAmount);
                        }
                    }

                }
                else if (item.UseAreaType == E_Coupon_UseAreaType.ProductSpu)
                {
                    var olist = item.UseAreaValue.ToJsonObject<List<UseAreaModel>>();
                    if (list != null)
                    {
                        string[] ids = olist.Select(m => m.Id).ToArray();

                        if (ids != null)
                        {
                            cal_sum_amount = rop.ProductSkus.Where(m => ids.Contains(m.SpuId)).Sum(m => m.SaleAmount);
                        }
                    }

                }

                LogUtil.Info("rop.ProductSkus:" + rop.ProductSkus.ToJsonString());

                foreach (var productSku in rop.ProductSkus)
                {
                    productSku.CouponAmount = Decimal.Round(BizFactory.Order.CalCouponAmount(cal_sum_amount, item.AtLeastAmount, item.UseAreaType, item.UseAreaValue, item.FaceType, item.FaceValue, rop.StoreId, productSku.SpuId, productSku.KindId3, productSku.SaleAmount), 2);
                }


                //金额补差

                if (rop.ProductSkus != null)
                {
                    if (rop.ProductSkus.Count > 0)
                    {
                        if (item.FaceType == E_Coupon_FaceType.ShopDiscount)
                        {

                        }
                        else
                        {
                            var sumCouponAmount1 = item.FaceValue;
                            var sumCouponAmount2 = rop.ProductSkus.Sum(m => m.CouponAmount);
                            if (sumCouponAmount1 != sumCouponAmount2)
                            {
                                var diff = sumCouponAmount1 - sumCouponAmount2;
                                rop.ProductSkus[rop.ProductSkus.Count - 1].CouponAmount = rop.ProductSkus[rop.ProductSkus.Count - 1].CouponAmount + diff;
                            }
                        }
                    }
                }


                couponModel.CouponAmount = rop.ProductSkus.Sum(m => m.CouponAmount);

                LogUtil.Info(" couponModel.CouponAmount:" + couponModel.CouponAmount);

                couponModel.CanSelected = GetCanSelected(rop.ShopMethod, item.UseAreaType, item.UseAreaValue, item.FaceType, item.FaceValue, item.AtLeastAmount, item.Status, item.ValidStartTime, item.ValidEndTime, item.MerchId, rop.StoreId, clientUserId, rop.ProductSkus);

                ret.Coupons.Add(couponModel);
            }

            ret.Coupons = ret.Coupons.OrderByDescending(m => m.CanSelected).ToList();


            result = new CustomJsonResult<RetCouponMy>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Details(string operater, string clientUserId, string couponId)
        {
            var result = new CustomJsonResult();

            var ret = new { };
            var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == couponId).FirstOrDefault();
            if (d_clientCoupon != null)
            {
                var d_coupon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                if (d_coupon != null)
                {

                    var couponModel = CovertCouponModel(d_clientCoupon.Id, d_clientCoupon.Sn, d_coupon.Name, d_coupon.UseAreaType, d_coupon.UseAreaValue, d_coupon.FaceType, d_coupon.FaceValue, d_coupon.AtLeastAmount, d_clientCoupon.ValidStartTime, d_clientCoupon.ValidEndTime);


                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", couponModel);

                }
            }

            result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "", ret);

            return result;
        }

        public CustomJsonResult RevPosSt(string operater, string clientUserId, RupCouponRevPosSt rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetCouponRevCenterSt();

            var d_CouponRevPosSt = CurrentDb.CouponRevPosSt.Where(m => m.MerchId == rup.MerchId && m.Code == rup.PosCode).FirstOrDefault();

            if (d_CouponRevPosSt != null)
            {
                ret.TopImgUrl = d_CouponRevPosSt.TopImgUrl;


                var l_CouponIds = CurrentDb.CouponRevCouponSt.Where(m => m.RevPosId == d_CouponRevPosSt.Id).Select(m => m.CouponId).ToList();

                var query = (from u in CurrentDb.Coupon
                             where l_CouponIds.Contains(u.Id)
                             select new { u.Id, u.MerchId, u.Name, u.UseAreaType, u.UseAreaValue, u.FaceType, u.FaceValue, u.AtLeastAmount });

                var list = query.OrderBy(m => m.Name).ToList();

                foreach (var item in list)
                {
                    var couponModel = CovertCouponModel(item.Id, "", item.Name, item.UseAreaType, item.UseAreaValue, item.FaceType, item.FaceValue, item.AtLeastAmount, null, null);


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

            var d_CouponRevCouponSt = CurrentDb.CouponRevCouponSt.Where(m => m.Id == rop.RevCenterId).FirstOrDefault();

            var limitMemberLevels = d_CouponRevCouponSt.LimitMemberLevels.ToJsonObject<List<string>>();

            if (limitMemberLevels != null)
            {
                if (!limitMemberLevels.Contains(d_clientUser.MemberLevel.ToString()))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoRight, "无资格领取该优惠券");
                }
            }

            var d_clientCoupons = CurrentDb.ClientCoupon.Where(m => m.ClientUserId == clientUserId && m.CouponId == d_coupon.Id).ToList();

            if (d_CouponRevCouponSt.PerLimitNum == d_clientCoupons.Count)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您领取的数量已经超过限制");
            }

            if (d_CouponRevCouponSt.PerLimitTimeType == E_Coupon_PerLimitTimeType.Day)
            {
                DateTime start = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                DateTime end = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

                d_clientCoupons = d_clientCoupons.Where(m => m.SourceTime >= start && m.SourceTime <= end).ToList();

                if (d_CouponRevCouponSt.PerLimitTimeNum == d_clientCoupons.Count)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "当天领取已经超过大限制，请明天再试试");
                }

            }
            else if (d_CouponRevCouponSt.PerLimitTimeType == E_Coupon_PerLimitTimeType.Month)
            {
                int nowYear = int.Parse(DateTime.Now.ToString("yyyy"));
                int nowMonth = int.Parse(DateTime.Now.ToString("MM"));

                d_clientCoupons = d_clientCoupons.Where(m => m.SourceTime.Year == nowYear && m.SourceTime.Month == nowMonth
                ).ToList();

                if (d_CouponRevCouponSt.PerLimitTimeNum <= d_clientCoupons.Count)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "当月领取已经超过大限制，请下个月再试试");
                }
            }
            else if (d_CouponRevCouponSt.PerLimitTimeType == E_Coupon_PerLimitTimeType.Quarter)
            {
                int nowYear = int.Parse(DateTime.Now.ToString("yyyy"));
                int nowMonth = int.Parse(DateTime.Now.ToString("MM"));

                int[] arr_nowMonths;

                if (nowMonth > 0 && nowMonth <= 3)
                {
                    arr_nowMonths = new int[] { 1, 2, 3 };
                }
                else if (nowMonth > 3 && nowMonth <= 6)
                {
                    arr_nowMonths = new int[] { 4, 5, 6 };
                }
                else if (nowMonth > 7 && nowMonth <= 9)
                {
                    arr_nowMonths = new int[] { 7, 8, 9 };
                }
                else
                {
                    arr_nowMonths = new int[] { 10, 11, 12 };
                }

                d_clientCoupons = d_clientCoupons.Where(m => m.SourceTime.Year == nowYear && arr_nowMonths.Contains(m.SourceTime.Month)
                ).ToList();

                if (d_CouponRevCouponSt.PerLimitTimeNum <= d_clientCoupons.Count)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "当季领取已经超过大限制，请下个月再试试");
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
