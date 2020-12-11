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


            //List<ClientCoupon> coupons;
            //if (!rup.IsGetHis)
            //{
            //    coupons = CurrentDb.ClientCoupon.Where(m => m.ClientUserId == clientUserId && m.Status == E_ClientCouponStatus.WaitUse && m.EndTime > DateTime.Now).ToList();
            //}
            //else
            //{
            //    coupons = CurrentDb.ClientCoupon.Where(m => m.ClientUserId == clientUserId && (m.Status == E_ClientCouponStatus.Used || m.Status == E_ClientCouponStatus.Expired) && m.EndTime < DateTime.Now).ToList();
            //}

            //foreach (var item in coupons)
            //{
            //    if (item.EndTime < DateTime.Now)
            //    {
            //        item.Status = E_ClientCouponStatus.Expired;
            //        CurrentDb.SaveChanges();
            //    }

            //    var couponModel = new CouponModel();

            //    couponModel.Id = item.Id;
            //    couponModel.Name = item.Name;
            //    switch (item.Type)
            //    {
            //        case E_ClientCouponType.FullCut:
            //        case E_ClientCouponType.UnLimitedCut:
            //            couponModel.Discount = item.Discount.ToF2Price();
            //            couponModel.DiscountUnit = "元";
            //            couponModel.DiscountTip = "满减卷";
            //            break;
            //        case E_ClientCouponType.Discount:
            //            couponModel.Discount = item.Discount.ToF2Price();
            //            couponModel.DiscountUnit = "折";
            //            couponModel.DiscountTip = "折扣卷";
            //            break;
            //    }

            //    couponModel.ValidDate = "有效到" + item.EndTime.ToUnifiedFormatDate();
            //    couponModel.Description = "全场使用";

            //    if (rup.CouponId != null)
            //    {
            //        if (rup.CouponId.Contains(item.Id))
            //        {
            //            couponModel.IsSelected = true;
            //        }
            //    }

            //    ret.Coupons.Add(couponModel);
            //}

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
