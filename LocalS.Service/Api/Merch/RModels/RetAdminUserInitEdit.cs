﻿using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetAdminUserInitEdit
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public bool IsDisable { get; set; }
        public bool ImIsUse { get; set; }

        public int WorkBench { get; set; }
        public List<string> OrgIds { get; set; }

        public List<TreeNode> Orgs { get; set; }

        public List<string> RoleIds { get; set; }

        public List<TreeNode> Roles { get; set; }
    }
}
