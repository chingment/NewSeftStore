using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Task
{
    public class PayTrans2CheckStatusModel
    {
        public string Id { get; set; }
        public string MerchId { get; set; }
        public E_PayCaller PayCaller { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public List<string> OrderIds { get; set; }
    }
}
