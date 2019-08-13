using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class RupAdminUserGetList: RupBaseGetList
    {

        public string UserName { get; set; }

        public string FullName { get; set; }
    }
}
