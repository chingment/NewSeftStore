using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class UserInfoModel
    {
        public string UserId { get; set; }

        public string NickName { get; set; }

        public string PhoneNumber { get; set; }

        public string Avatar { get; set; }
        public bool IsVip { get; set; }
        public int MemberLevel { get; set; }
        public string MemberTag { get; set; }
    }
}
