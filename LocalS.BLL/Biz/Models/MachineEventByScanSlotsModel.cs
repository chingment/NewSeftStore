using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class MachineEventByScanSlotsModel : MachineEventBaseModel
    {
        public string Remark { get; set; }
        public int Status { set; get; }
        public string CabinetId { set; get; }
    }
}
