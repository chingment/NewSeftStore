using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopMachineHandleRunExItems
    {
        public RopMachineHandleRunExItems()
        {
            this.Items = new List<ExItem>();
            this.Reasons = new List<ExReason>();
        }

        public string MachineId { get; set; }
        public List<ExItem> Items { get; set; }
        public List<ExReason> Reasons { get; set; }
    }
}
