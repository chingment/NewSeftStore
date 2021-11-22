using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupMemberGetPayLevelSt
    {
        public string StoreId { get; set; }
        public string SaleOutletId { get; set; }
        public string OpenId { get; set; }
        public RupMemberGetPayLevelSt()
        {
            this.SaleOutletId = "";
        }
    }
}
