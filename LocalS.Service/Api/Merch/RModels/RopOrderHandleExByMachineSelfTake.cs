using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopOrderHandleExByMachineSelfTake
    {
        public RopOrderHandleExByMachineSelfTake()
        {
            this.Uniques = new List<ExUnique>();
        }

        public string Id { get; set; }
        public List<ExUnique> Uniques { get; set; }
        public string Remark { get; set; }
        public bool IsRunning { get; set; }
    }
}
