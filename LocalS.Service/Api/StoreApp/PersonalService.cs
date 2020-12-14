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
        public CustomJsonResult<RetPersonalPageData> PageData(string operater, string clientUserId, RupPersonalPageData rup)
        {
            var result = new CustomJsonResult<RetPersonalPageData>();

            var ret = new RetPersonalPageData();

            var user = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            if (user != null)
            {
                var userInfo = new UserInfoModel();
                userInfo.UserId = user.Id;
                userInfo.NickName = user.NickName;
                userInfo.PhoneNumber = CommonUtil.GetEncryptionPhoneNumber(user.PhoneNumber);
                userInfo.Avatar = user.Avatar;
                userInfo.MemberLevel = user.MemberLevel;
                userInfo.MemberTag = "普通会员";
                userInfo.MemberExpireTime = user.MemberExpireTime.ToUnifiedFormatDate();
                var memberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == user.MerchId && m.Level == user.MemberLevel).FirstOrDefault();
                if (memberLevelSt != null)
                {
                    userInfo.MemberTag = memberLevelSt.Name;
                }
                ret.UserInfo = userInfo;
            }


            result = new CustomJsonResult<RetPersonalPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
