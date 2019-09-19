using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupStoreManageProductGetProductList : RupBaseGetList
    {
        public string StoreId { get; set; }
        public string ProductSkuName { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
    }
}
