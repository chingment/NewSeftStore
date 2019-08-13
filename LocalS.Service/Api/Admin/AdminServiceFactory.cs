using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class AdminServiceFactory
    {
        public static AdminUserService AdminUser
        {
            get
            {
                return new AdminUserService();
            }
        }

        public static SysRoleService SysRole
        {
            get
            {
                return new SysRoleService();
            }
        }

        public static SysMenuService SysMenu
        {
            get
            {
                return new SysMenuService();
            }
        }

        public static AdminOrgService AdminOrg
        {
            get
            {
                return new AdminOrgService();
            }
        }

        public static AgentMasterService AgentMaster
        {
            get
            {
                return new AgentMasterService();
            }
        }

        public static MerchMasterService MerchMaster
        {
            get
            {
                return new MerchMasterService();
            }
        }
    }
}
