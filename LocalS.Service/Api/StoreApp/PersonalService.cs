using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class PersonalService : BaseDbContext
    {
        public CustomJsonResult<RetPersonalGetPageData> GetPageData(string operater, string clientUserId, string storeId)
        {
            var result = new CustomJsonResult<RetPersonalGetPageData>();

            var ret = new RetPersonalGetPageData();

            var user = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            if (user != null)
            {
                var userInfo = new UserInfoModel();
                userInfo.UserId = user.Id;
                userInfo.NickName = user.Nickname;
                userInfo.PhoneNumber = user.PhoneNumber;
                userInfo.Avatar = user.Avatar;
                userInfo.IsVip = user.IsVip;

                ret.UserInfo = userInfo;
            }


            result = new CustomJsonResult<RetPersonalGetPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
