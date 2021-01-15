using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopImServiceSeats
    {
        public string MachineId { get; set; }
        public List<ProductSku> ProductSkus { get; set; }
        public class ProductSku
        {
            public string ProductSkuId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
