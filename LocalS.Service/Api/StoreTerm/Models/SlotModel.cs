using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class SlotModel
    {
        public string Id { get; set; }
        public string ProductSkuId { get; set; }
        public string ProductSkuCumCode { get; set; }
        public string ProductSkuName { get; set; }
        public string ProductSkuMainImgUrl { get; set; }
        public string ProductSkuSpecDes { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int MaxQuantity { get; set; }

        public int Version { get; set; }

        public string CabinetId { get; set; }
    }
}
