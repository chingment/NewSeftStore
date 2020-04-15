using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupLogGetListByOperate:RupBaseGetList
    {
        public string EventName { get; set; }
        public string OperateUserName { get; set; }
        public string Remark { get; set; }
        public string AppId { get; set; }
    }
}
