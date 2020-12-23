using LocalS.BLL;
using LocalS.Entity;
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

            ret.UserInfo = GetUserInfo(clientUserId, rup.OpenId);

            var d_ordersByWaitpay_Count = CurrentDb.Order.Where(m => m.ClientUserId == clientUserId && m.Status == E_OrderStatus.WaitPay).Count();

            if (d_ordersByWaitpay_Count > 0)
            {
                ret.BadgeByWaitPayOrders = new UI.Badge { Type = "number", Value = d_ordersByWaitpay_Count.ToString() };
            }

            result = new CustomJsonResult<RetPersonalPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public UserInfoModel GetUserInfo(string clientUserId, string openId)
        {
            var m_userInfo = new UserInfoModel();

            var d_clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId || (m.WxMpOpenId != null && m.WxMpOpenId == openId)).FirstOrDefault();
            if (d_clientUser != null)
            {
                m_userInfo.UserId = d_clientUser.Id;
                m_userInfo.NickName = d_clientUser.NickName;
                m_userInfo.PhoneNumber = CommonUtil.GetEncryptionPhoneNumber(d_clientUser.PhoneNumber);
                m_userInfo.Avatar = d_clientUser.Avatar;
                m_userInfo.MemberLevel = d_clientUser.MemberLevel;
                m_userInfo.MemberTag = "普通用户";
                if (d_clientUser.MemberLevel > 0)
                {
                    m_userInfo.MemberExpireTip = string.Format("{0}天后过期", Convert.ToInt16((d_clientUser.MemberExpireTime.Value - DateTime.Now).TotalDays));
                }
                var memberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == d_clientUser.MerchId && m.Level == d_clientUser.MemberLevel).FirstOrDefault();
                if (memberLevelSt != null)
                {
                    m_userInfo.MemberTag = memberLevelSt.Name;
                }
            }

            return m_userInfo;

        }
    }
}
