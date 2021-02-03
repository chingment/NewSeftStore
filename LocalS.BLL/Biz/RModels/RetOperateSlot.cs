using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class RetOperateSlot
    {
        public RetOperateSlot()
        {
            this.ChangeRecords = new List<StockChangeRecordModel>();
        }

        public List<StockChangeRecordModel> ChangeRecords { get; set; }

        public string StockId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public string ProductSkuId { get; set; }
        public string Name { get; set; }
        public string CumCode { get; set; }
        public string MainImgUrl { get; set; }
        public string SpecDes { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public int WarnQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public int Version { get; set; }
        public bool IsCanAlterMaxQuantity { get; set; }
    }
}
