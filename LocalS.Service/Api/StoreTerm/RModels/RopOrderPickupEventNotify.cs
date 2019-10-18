using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopOrderPickupEventNotify
    {
        public string MachineId { get; set; }
        public string OrderId { get; set; }
        public string UniqueId { get; set; }
        public string ProductSkuId { get; set; }
        public string SlotId { get; set; }
        /// <summary>
        /// 1000 发起取货
        /// 2000 正在取货
        /// 3000 取货成功
        /// 4000 取货异常
        /// </summary>
        public string EventCode { get; set; }
        public string EventRemark { get; set; }
    }
}
