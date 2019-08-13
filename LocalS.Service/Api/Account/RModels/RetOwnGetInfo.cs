using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RetOwnGetInfo
    {
        public RetOwnGetInfo()
        {
            this.Menus = new List<MenuNode>();
        }
        public string Introduction { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<MenuNode> Menus { get; set; }
    }
}
