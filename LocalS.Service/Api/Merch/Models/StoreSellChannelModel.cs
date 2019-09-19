using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class StoreSellChannelModel
    {
        public StoreSellChannelModel()
        {
            this.ProductSkus = new List<ProductSkuModel>();
        }

        public string Name { get; set; }
        public E_SellChannelRefType RefType { get; set; }
        public string RefId { get; set; }
        public List<ProductSkuModel> ProductSkus { get; set; }
    }
}
