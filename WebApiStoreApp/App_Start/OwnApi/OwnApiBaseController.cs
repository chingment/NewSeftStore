using Lumos;
using System.Web;
using System.Configuration;
using Lumos.Web.Http;
using Lumos.Session;

namespace WebApiStoreApp
{
    [OwnApiAuthorize]
    public class OwnApiBaseController : BaseController
    {
        public OwnApiBaseController()
        {
            LogUtil.SetTrackId();
        }

        public string Token
        {
            get
            {
                var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
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
        private TokenInfo TokenInfo
        {
            get
            {
                var tokenInfo = SSOUtil.GetTokenInfo(this.Token);
                if (tokenInfo == null)
                {
                    tokenInfo = new TokenInfo();
                    tokenInfo.UserId = "";
                }
                return tokenInfo;
            }
        }

        public string CurrentUserId
        {
            get
            {
                return this.TokenInfo.UserId;
            }
        }
    }
}