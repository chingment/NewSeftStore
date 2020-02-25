
using System;

namespace Lumos.DbRelay
{
    /// <summary>
    /// 系统的枚举
    /// </summary>
    public partial class Enumeration
    {
        public enum SmsSendResult
        {
            Unknow = 0,
            Success = 1,
            Failure = 2,
            Exception = 3
        }
        public enum BelongSite
        {
            Unknow = 0,
            Account = 1,
            Admin = 2,
            Agent = 3,
            Merch = 4,
            Client = 5
        }

        public enum BelongType
        {
            Unknow = 0,
            Admin = 2,
            Agent = 3,
            Merch = 4,
            Client = 5
        }


        public enum LoginWay
        {
            Unknow = 0,
            Website = 1,
            Android = 2,
            Ios = 3,
            Wxmp = 4,
            Terminal = 5
        }

        public enum LoginFun
        {
            Unknow = 0,
            Account = 1,
            FingerVein = 2
        }

        public enum LoginResult
        {

            Unknow = 0,
            LoginSuccess = 1,
            LoginFailure = 2,
            LogoutSuccess = 3,
            LogoutFailure = 4
        }

        public enum OperateType
        {
            Unknow = 0,
            Login = 1,
            Logout = 2
        }

        public enum SysOrgStatus
        {

            Unknow = 0,
            Valid = 1,
            Invalid = 2
        }

        public enum WxAutoReplyType
        {
            Unknow = 0,
            Subscribe = 1,
            Keyword = 2,
            MenuClick = 3,
            NotKeyword = 4
        }
    }
}
