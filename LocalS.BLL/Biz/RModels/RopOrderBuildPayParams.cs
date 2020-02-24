using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public enum E_PayOption
    {
        Unknow = 0,
        Wechat = 1,
        Alipay = 2,
        Aggregate = 99
    }
    public class RopOrderBuildPayParams
    {
        public string OrderId { get; set; }
        public E_OrderPayCaller PayCaller { get; set; }

        public E_OrderPayPartner PayPartner { get; set; }

        public string CreateIp { get; set; }
    }
}
