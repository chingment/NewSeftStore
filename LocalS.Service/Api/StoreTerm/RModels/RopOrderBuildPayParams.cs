using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopOrderBuildPayParams
    {
        public string OrderId { get; set; }
        /// <summary>
        /// 1: Wechat,  2 AliPay
        /// </summary>
        public E_OrderPayWay PayWay { get; set; }
        public E_OrderPayCaller PayCaller { get; set; }
    }
}
