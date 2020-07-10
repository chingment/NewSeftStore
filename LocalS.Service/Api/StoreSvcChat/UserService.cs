using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreSvcChat
{
    public class UserService : BaseDbContext
    {
        public CustomJsonResult GetInfoByUserName(string operater, string userId, RupUserGetInfo rup)
        {
            var result = new CustomJsonResult();


            var sysUser = CurrentDb.SysUser.Where(m => m.UserName == rup.UserName).FirstOrDefault();

            if (sysUser != null)
            {
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { Avatar = sysUser.Avatar, NickName = sysUser.NickName });
            }


            return result;

        }

    }
}
