using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class CouponModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FaceValue { get; set; }
        public string FaceUnit { get; set; }
        public string FaceTip { get; set; }
        public string ValidDate { get; set; }
        public string Description { get; set; }
        public bool CanSelected { get; set; }
        public bool IsSelected { get; set; }

        public decimal CouponAmount { get; set; }
    }
}
