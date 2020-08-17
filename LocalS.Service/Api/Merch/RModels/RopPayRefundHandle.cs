using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPayRefundHandle
    {
        public string PayRefundId { get; set; }
        public string Remark { get; set; }

        public bool IsSuccess { get; set; }
    }
}
