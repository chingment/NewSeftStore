using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetMachineGetRunExHandleItems
    {
        public RetMachineGetRunExHandleItems()
        {
            this.ExReasons = new List<ExReason>();
            this.ExItems = new List<ExItem>();
        }

        public List<ExReason> ExReasons { get; set; }
        public List<ExItem> ExItems { get; set; }
        public class ExItem
        {
            public ExItem()
            {
                this.Uniques = new List<ExUnique>();
            }

            public string ItemId { get; set; }
            public List<ExUnique> Uniques { get; set; }

        }

        public class ExUnique
        {
            public string UniqueId { get; set; }
            public string ProductSkuId { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string SlotId { get; set; }
            public bool CanHandle { get; set; }
            public int SignStatus { get; set; }
            public StatusModel Status { get; set; }
        }

        public class ExReason
        {

            public string ReasonId { get; set; }
            public string Title { get; set; }
            public bool IsChecked { get; set; }
        }
    }
}
