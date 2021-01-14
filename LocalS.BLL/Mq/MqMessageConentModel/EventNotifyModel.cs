using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class EventNotifyModel
    {
        public string AppId { get; set; }
        public string Operater { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public string MachineId { get; set; }
        public string EventCode { get; set; }
        public string EventRemark { get; set; }
        public object EventContent { get; set; }
    }
}
