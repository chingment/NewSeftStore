using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{

    public class RopCouponMy
    {
        public bool IsGetHis { get; set; }
        public List<string> CouponIds { get; set; }
        public E_ShopMethod ShopMethod { get; set; }
        public string StoreId { get; set; }
        public List<BuildSku> Skus { get; set; }
        public E_Coupon_FaceType[] FaceTypes { get; set; }
        public List<string> SelectCouponIds { get; set; }
    }
}
