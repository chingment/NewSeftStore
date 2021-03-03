using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class PayRefundResultNotifyModel
    {
        public string Content { get; set; }
        public E_PayTransLogNotifyFrom From { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string PayTransId { get; set; }
        public string PayRefundId { get; set; }
    }
}
