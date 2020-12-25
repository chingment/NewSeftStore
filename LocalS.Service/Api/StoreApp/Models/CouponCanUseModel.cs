using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class CouponCanUseModel
    {
        public int Count { get; set; }

        public List<string> SelectedCouponIds { get; set; }

        public string TipMsg { get; set; }

        public TipType TipType { get; set; }
    }
}
