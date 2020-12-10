using Lumos;
using System.Web;
using System.Configuration;
using Lumos.Web.Http;
using Lumos.Session;
using System.IO;
using System;

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
                string token = request.QueryString["token"];
                if (string.IsNullOrEmpty(token))
                {
                    token = request.Headers["token"];
                }
                return token;
            }
        }
        public string AppId
        {
            get
            {
                var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                string open_id = request.Headers["app_id"];
                return open_id;
            }
        }
        public string OpenId
        {
            get
            {
                var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                string open_id = request.Headers["open_id"];
                return open_id;
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
        public string GetRequestContent()
        {
            string content = null;

            try
            {
                var myRequest = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                Stream stream = myRequest.InputStream;
                stream.Seek(0, SeekOrigin.Begin);

                content = new StreamReader(stream).ReadToEnd();
            }
            catch
            {
                content = null;
            }

            return content;

        }
    }
}