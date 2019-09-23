using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupClientGetList : RupBaseGetList
    {
        public string NickName { get; set; }

        public string PhoneNumber { get; set; }
    }
}
