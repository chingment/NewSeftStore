using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class ProductSkuInStoreModel
    {
        public ProductSkuInStoreModel()
        {
            this.Refs = new List<RefModel>();
        }
        public string StoreId { get; set; }

        public string StoreName { get; set; }

        public List<RefModel> Refs { get; set; }
        public class RefModel
        {
            public E_SellChannelRefType RefType { get; set; }
            public string RefId { get; set; }

            public string RefName { get; set; }
        }
    }
}
