using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopAdminUserEdit
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public List<ImgSet> Avatar { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public bool IsDisable { get; set; }
        public List<string> RoleIds { get; set; }
        public bool ImIsUse { get; set; }
        public int WorkBench { get; set; }
    }
}
