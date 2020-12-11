using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public enum TipType
    {
        Unknow = 0,
        NoCanUse = 1,
        CanUse = 2,
        InUse = 3
    }



    public class OrderConfirmCouponModel
    {

        public OrderConfirmCouponModel()
        {

        }

        public TipType TipType { get; set; }

        public string TipMsg { get; set; }

        //public int CanUseQuantity { get; set; }

        public List<string> SelecedCouponIds { get; set; }
    }


}
