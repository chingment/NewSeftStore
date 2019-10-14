using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class SlotStockModel
    {
        public string Id { get; set; }
        public string SlotId { get; set; }
        public string Name { get; set; }
        public string SalePrice { get; set; }
        public string MainImgUrl { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
    }
}
