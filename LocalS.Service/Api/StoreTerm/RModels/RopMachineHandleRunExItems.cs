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
            this.ExItems = new List<ExItem>();
            this.ExReasons = new List<ExReason>();
        }

        public string MerchId { get; set; }
        public string MachineId { get; set; }
        public List<ExItem> ExItems { get; set; }
        public List<ExReason> ExReasons { get; set; }

        public string AppId { get; set; }
    }
}
