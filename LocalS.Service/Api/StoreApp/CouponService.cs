using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{


    public class CouponService : BaseDbContext
    {
        private bool GetCanSelected(E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue, decimal atLeastAmount, string merchId, string storeId, string clientUserId, List<OrderConfirmProductSkuModel> productSkus)
        {
            //foreach (var productSku in productSkus)
            //{
            //    if (productSku.ShopMode == E_SellChannelRefType.Machine || productSku.ShopMode == E_SellChannelRefType.Mall)
            //    {
            //        var r_productSku = CacheServiceFactory.Product.GetSkuInfo(merchId, storeId, store.GetSellChannelRefIds(productSku.ShopMode), productSku.Id);

            //        productSku.Name = r_productSku.Name;
            //        productSku.MainImgUrl = r_productSku.MainImgUrl;
            //        productSku.SalePrice = r_productSku.Stocks[0].SalePrice;
            //        productSku.OriginalPrice = r_productSku.Stocks[0].SalePrice;
            //        productSku.SumSalePrice = productSku.Quantity * productSku.SalePrice;
            //        productSku.SumOriginalPrice = productSku.Quantity * productSku.OriginalPrice;

            //    }
            //    else if (productSku.ShopMode == E_SellChannelRefType.MemberFee)
            //    {
            //        var memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == merchId && m.Id == productSku.Id).FirstOrDefault();
            //        if (memberFeeSt != null)
            //        {
            //            productSku.Name = memberFeeSt.Name;
            //            productSku.MainImgUrl = memberFeeSt.MainImgUrl;
            //            productSku.SalePrice = memberFeeSt.FeeValue;
            //            productSku.OriginalPrice = memberFeeSt.FeeValue;
            //            productSku.SumSalePrice = productSku.Quantity * productSku.SalePrice;
            //            productSku.SumOriginalPrice = productSku.Quantity * productSku.SumOriginalPrice;
            //        }
            //    }
            //}


            if (useAreaType == E_Coupon_UseAreaType.All)
            {
                return true;
            }
            else if (useAreaType == E_Coupon_UseAreaType.Store)
            {
                string[] arr_useAreaValue = useAreaValue.ToJsonObject<string[]>();

                if (arr_useAreaValue != null)
                {
                    if (arr_useAreaValue.Contains(storeId))
                    {
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

            foreach (var item in list)
            {
                var couponModel = new CouponModel();
                couponModel.Id = item.Id;
                couponModel.Name = item.Name;

                switch (item.FaceType)
                {
                    case E_Coupon_FaceType.ShopVoucher:
                        couponModel.FaceValue = item.FaceValue.ToF2Price();
                        couponModel.FaceUnit = "元";
                        if (item.AtLeastAmount > 0)
                        {
                            couponModel.FaceTip = string.Format("满{0}元使用");
                        }
                        else
                        {
                            couponModel.FaceTip = string.Format("代金券");
                        }
                        break;
                    case E_Coupon_FaceType.ShopDiscount:
                        couponModel.FaceValue = item.FaceValue.ToF2Price();
                        couponModel.FaceUnit = "元";
                        if (item.AtLeastAmount > 0)
                        {
                            couponModel.FaceTip = string.Format("满{0}元使用");
                        }
                        else
                        {
                            couponModel.FaceTip = string.Format("代金券");
                        }
                        break;
                    case E_Coupon_FaceType.RentVoucher:
                        couponModel.FaceValue = item.FaceValue.ToF2Price();
                        couponModel.FaceUnit = "元";
                        if (item.AtLeastAmount > 0)
                        {
                            couponModel.FaceTip = string.Format("满{0}元使用");
                        }
                        else
                        {
                            couponModel.FaceTip = string.Format("代金券");
                        }
                        break;
                    case E_Coupon_FaceType.DepositVoucher:
                        couponModel.FaceValue = item.FaceValue.ToF2Price();
                        couponModel.FaceUnit = "元";
                        if (item.AtLeastAmount > 0)
                        {
                            couponModel.FaceTip = string.Format("满{0}元使用");
                        }
                        else
                        {
                            couponModel.FaceTip = string.Format("代金券");
                        }
                        break;
                }


                couponModel.ValidDate = "有效到" + item.ValidEndTime.ToUnifiedFormatDate();


                switch (item.UseAreaType)
                {
                    case E_Coupon_UseAreaType.All:
                        couponModel.Description = "全场通用";
                        break;
                    case E_Coupon_UseAreaType.Store:
                        couponModel.Description = "指定店铺使用";
                        break;
                    case E_Coupon_UseAreaType.ProductKind:
                        couponModel.Description = "指定品类使用";
                        break;
                    case E_Coupon_UseAreaType.ProductSpu:
                        couponModel.Description = "指定商品使用";
                        break;
                }


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

    }
}
