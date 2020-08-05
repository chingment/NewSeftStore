using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RopOrderBuildPayParams
    {
        public string AppId { get; set; }
        public List<string> OrderIds { get; set; }
        public E_PayCaller PayCaller { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string CreateIp { get; set; }

        public List<OrderReserveBlockModel> Blocks { get; set; }
    }
}
