using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class ExUnique
    {
        public string Id { get; set; }
        public int SignStatus { get; set; }
    }

    public class ExItem
    {
        public string Id { get; set; }
        public List<ExUnique> Uniques { get; set; }
    }

    public class ExReason
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class RopOrderHandleExOrderByMachineSelfTake
    {
        public RopOrderHandleExOrderByMachineSelfTake()
        {
            this.Items = new List<ExItem>();
        }

        public List<ExItem> Items { get; set; }
        public string Remark { get; set; }
        public bool IsRunning { get; set; }
    }
}
