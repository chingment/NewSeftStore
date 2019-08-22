using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public enum E_OperateType
    {
        Unknow = 0,
        SendPaySuccessCheck = 1,
        SendPayCancleCheck = 2
    }

    public class RupOperateGetResult
    {
        public string Id { get; set; }
        public E_OperateType Type { get; set; }
        public E_AppCaller AppCaller { get;set;}
    }
}
