using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class ExUnique
    {
        public string UniqueId { get; set; }
        public int SignStatus { get; set; }
    }

    public class ExItem
    {
        public string ItemId { get; set; }
        public List<ExUnique> Uniques { get; set; }
        public bool IsRefund { get; set; }
        public decimal RefundAmount { get; set; }
        public E_PayRefundMethod RefundMethod { get; set; }
    }

    public class ExReason
    {
        public string ReasonId { get; set; }
        public string Title { get; set; }
    }

    public class RopOrderHandleExByDeviceSelfTake
    {
        public RopOrderHandleExByDeviceSelfTake()
        {
            this.Items = new List<ExItem>();
        }

        public string AppId { get; set; }
        public List<ExItem> Items { get; set; }
        public string Remark { get; set; }
        public bool IsRunning { get; set; }
        public string DeviceId { get; set; }

        public string MerchId { get; set; }
    }
}
