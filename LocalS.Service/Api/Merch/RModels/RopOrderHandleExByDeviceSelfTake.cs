using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopOrderHandleExByDeviceSelfTake
    {
        public RopOrderHandleExByDeviceSelfTake()
        {
            this.Uniques = new List<ExUnique>();
        }

        public string Id { get; set; }
        public string AppId { get; set; }
        public List<ExUnique> Uniques { get; set; }
        public string Remark { get; set; }
        public bool IsRunning { get; set; }
        public bool IsRefund { get; set; }
        public decimal RefundAmount { get; set; }
        public E_PayRefundMethod RefundMethod { get; set; }

        public string DeviceId { get; set; }
    }
}
