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

            var d_clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            if (d_clientUser != null)
            {
                var m_userInfo = new UserInfoModel();
                m_userInfo.UserId = d_clientUser.Id;
                m_userInfo.NickName = d_clientUser.NickName;
                m_userInfo.PhoneNumber = CommonUtil.GetEncryptionPhoneNumber(d_clientUser.PhoneNumber);
                m_userInfo.Avatar = d_clientUser.Avatar;
                m_userInfo.MemberLevel = d_clientUser.MemberLevel;
                m_userInfo.MemberTag = "普通用户";
                if (d_clientUser.MemberLevel == 1)
                {
                    m_userInfo.MemberExpireTip = string.Format("{0}天后过期", Convert.ToInt16((d_clientUser.MemberExpireTime.Value - DateTime.Now).TotalDays));
                }
                else if (d_clientUser.MemberLevel == 2)
                {
                    m_userInfo.MemberExpireTip = "永久";
                }
                var memberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == d_clientUser.MerchId && m.Level == d_clientUser.MemberLevel).FirstOrDefault();
                if (memberLevelSt != null)
                {
                    m_userInfo.MemberTag = memberLevelSt.Name;
                }

                ret.UserInfo = m_userInfo;
            }


            result = new CustomJsonResult<RetPersonalPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
