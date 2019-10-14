using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetProductSkuSearch
    {
        public RetProductSkuSearch()
        {
            this.ProductSkus = new List<ProductSkuInfoBySearchModel>();
        }

        public List<ProductSkuInfoBySearchModel> ProductSkus { get; set; }
    }
}
