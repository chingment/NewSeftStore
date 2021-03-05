using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Task
{
    public class RentOrder2CheckExpire
    {
        public string OrderId { get; set; }
        public string ClientUserId { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public DateTime ExpireDate { get; set; }
        public string POrderId { get; set; }
    }
}
