using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupUserGetList: RupBaseGetList
    {

        public string UserName { get; set; }

        public string FullName { get; set; }
    }
}
