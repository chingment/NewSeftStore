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

        }

        public int No;
        public string ProductSkuId { get; set; }
        public string SlotId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int MaxQuantity { get; set; }

        public bool IsOffSell { get; set; }

        public decimal SalePrice { get; set; }

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
