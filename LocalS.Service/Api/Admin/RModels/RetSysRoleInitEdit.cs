using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class RetSysRoleInitEdit
    {
        public string RoleId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public List<TreeNode> Menus { get; set; }

        public List<string> MenuIds { get; set; }
    }
}
