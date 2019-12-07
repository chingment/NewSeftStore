using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq.MqMessageConentModel
{
    public class PayResultNotifyModel
    {

        public string Content { get; set; }

        public E_OrderNotifyLogNotifyFrom From { get; set; }

        public E_OrderPayPartner PayPartner { get; set; }
    }
}
