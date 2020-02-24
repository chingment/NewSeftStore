using LocalS.Entity;
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
        public string UniqueId { get; set; }
        public E_OrderSubDetailUnitStatus Status { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int ActionStatusCode { get; set; }
        public string ActionStatusName { get; set; }
        public string Remark { get; set; }
        public int PickupUseTime { get; set; }
        public bool IsPickupComplete { get; set; }
        public string ImgId { get; set; }
    }
}
