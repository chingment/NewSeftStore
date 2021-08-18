using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{

    public class RopDeviceEventNotify
    {
        public string AppId { get; set; }
        public string DeviceId { get; set; }
        public object Content { get; set; }
        public string EventCode { get; set; }
        public string EventRemark { get; set; }
        public int MsgId { get; set; }
        public string MsgMode { get; set; }
    }
}
