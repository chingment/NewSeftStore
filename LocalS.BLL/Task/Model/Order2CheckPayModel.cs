using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Task
{
    public class Order2CheckPayModel
    {
        public string Id { get; set; }
        public string MerchId { get; set; }
        public E_OrderPayCaller PayCaller { get; set; }
        public E_OrderPayPartner PayPartner { get; set; }
    }
}
