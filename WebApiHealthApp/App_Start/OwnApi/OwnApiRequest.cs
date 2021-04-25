using Lumos;
using Lumos.Session;
using System.Web;

namespace WebApiHealthApp
{
    public static class OwnApiRequest
    {
        public const string SESSION_NAME = "SessionName";


        public static string Token
        {
            get
            {
                var request = HttpContext.Current.Request;
                var token = request.QueryString["token"];
                if (string.IsNullOrEmpty(token))
                {
                    token = request.Headers["X-Token"];
                    if (token != null)
                    {
                        token = request.Headers["X-Token"].ToString();
                    }
                }

                return token;
            }
        }
        public static TokenInfo TokenInfo
        {
            get
            {
                var tokenInfo = SSOUtil.GetTokenInfo(OwnApiRequest.Token);
                if (tokenInfo == null)
                {
                    tokenInfo = new TokenInfo();
                    tokenInfo.UserId = "";
                }
                return tokenInfo;
            }
        }
    }
}