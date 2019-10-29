using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class SlotColModel
    {
        public SlotColModel()
        {
            this.SlotInfo = new SlotModel();
        }

        public int No;

        public string SlotId { get; set; }

        public SlotModel SlotInfo { get; set; }

        public class SlotModel
        {
            public string Id { get; set; }
            public string ProductSkuId { get; set; }
            public string ProductSkuName { get; set; }
            public string ProductSkuMainImgUrl { get; set; }
            public int SumQuantity { get; set; }
            public int LockQuantity { get; set; }
            public int SellQuantity { get; set; }
            public int MaxQuantity { get; set; }
        }
    }
    public class SlotRowModel
    {

        public SlotRowModel()
        {
            this.Cols = new List<SlotColModel>();
        }
        
        public int No { get; set; }
        public List<SlotColModel> Cols { get; set; }
    }
}
