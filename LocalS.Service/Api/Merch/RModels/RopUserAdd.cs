using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopUserAdd
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public List<string> RoleIds { get; set; }
    }
}
