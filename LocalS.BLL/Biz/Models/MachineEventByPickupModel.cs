using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class MachineEventByPickupModel : MachineEventBaseModel
    {
        public string OrderId { get; set; }
        public string UniqueId { get; set; }

        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public string ProductSkuId { get; set; }
        public E_OrderPickupStatus Status { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int ActionStatusCode { get; set; }
        public string ActionStatusName { get; set; }
        public int PickupUseTime { get; set; }
        public bool IsPickupComplete { get; set; }
        public string ImgId { get; set; }
        public bool IsTest { get; set; }
        public string Remark { get; set; }
    }
}
