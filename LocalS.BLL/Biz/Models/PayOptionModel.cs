using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class PayOptionModel
    {
        public E_PayCaller Caller { get; set; }

        public E_PayPartner Partner { get; set; }
    }
}
