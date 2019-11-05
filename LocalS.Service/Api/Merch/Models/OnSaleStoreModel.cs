using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class OnSaleStoreModel
    {
        public OnSaleStoreModel()
        {
            this.Refs = new List<RefModel>();
        }

        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string ProductSkuId { get; set; }
        public string ProductSkuName { get; set; }
        public string ProductSkuMainImgUrl { get; set; }
        public decimal ProductSkuSalePrice { get; set; }
        public bool ProductSkuIsOffSell { get; set; }
        public List<RefModel> Refs { get; set; }
        public class RefModel
        {
            public E_SellChannelRefType ReType { get; set; }
            public string RefId { get; set; }
            public string RefName { get; set; }

            public List<SlotModel> Slots { get; set; }
        }

        public class SlotModel
        {
            public string Id { get; set; }

            public int SellQuantity { get; set; }

            public int LockQuantity { get; set; }

            public int SumQuantity { get; set; }
        }
    }
}
