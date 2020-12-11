using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{


    public class CouponService : BaseDbContext
    {
        public CustomJsonResult My(string operater, string clientUserId, RupCouponMy rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetCouponMy();

            var query = (from u in CurrentDb.ClientCoupon
                         join m in CurrentDb.Coupon on u.CouponId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         select new { u.Id, u.ClientUserId, tt.Name, tt.UseAreaType, tt.UseAreaValue, u.Status, u.ValidEndTime, u.ValidStartTime, tt.FaceType, tt.FaceValue, tt.AtLeastAmount });

            if (!rup.IsGetHis)
            {
                query = query.Where(u => u.ClientUserId == clientUserId && u.Status == E_ClientCouponStatus.WaitUse && u.ValidEndTime > DateTime.Now);
            }
            else
            {
                query = query.Where(m => m.ClientUserId == clientUserId && (m.Status == E_ClientCouponStatus.Used || m.Status == E_ClientCouponStatus.Expired) && m.ValidEndTime < DateTime.Now);
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
                        couponModel.Description = "全场使用";
                        break;
                    case E_Coupon_UseAreaType.Store:
                        couponModel.Description = "指定店铺使用";
                        break;
                    case E_Coupon_UseAreaType.ProductKind:
                        couponModel.Description = "指定商品品类使用";
                        break;
                    case E_Coupon_UseAreaType.ProductSpu:
                        couponModel.Description = "指定商品使用";
                        break;
                }


                if (rup.CouponIds != null)
                {
                    if (rup.CouponIds.Contains(item.Id))
                    {
                        couponModel.IsSelected = true;
                    }
                }

                if (rup.ShopMethod == E_ShopMethod.Unknow)
                {
                    couponModel.CanSelected = false;
                }
                else
                {
                    couponModel.CanSelected = true;
                }

                ret.Coupons.Add(couponModel);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
