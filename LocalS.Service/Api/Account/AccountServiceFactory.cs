using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class AccountServiceFactory
    {
        public static HomeService Home
        {
            get
            {
                return new HomeService();
            }
        }

        public static OwnService Own
        {
            get
            {
                return new OwnService();
            }
        }

        public static LoginLogService LoginLog
        {
            get
            {
                return new LoginLogService();
            }
        }

        public static UserInfoService UserInfo
        {
            get
            {
                return new UserInfoService();
            }
        }
    }
}
