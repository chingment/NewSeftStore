using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{ 
    public enum OperateLogType
    {

        Unknow = 0,
        Login = 1,
        Logout = 2
    }
    public class OperateLogModel
    {
        public string Operater { get;set;}
        public OperateLogType Type  {   get; set; }
        public string Remark { get; set; }
    }
}
