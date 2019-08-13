using Lumos;
using System.Web;
using System.Configuration;
using Lumos.Web.Http;
using Lumos.Session;

namespace WebApiAccount
{
    [OwnApiAuthorize]
    public class OwnApiBaseController : BaseController
    {
        private OwnApiHttpResult _result = new OwnApiHttpResult();

        public OwnApiHttpResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }

        public OwnApiBaseController()
        {
            LogUtil.SetTrackId();
        }

        public OwnApiHttpResponse ResponseResult(OwnApiHttpResult result)
        {
            return new OwnApiHttpResponse(result);
        }

        public OwnApiHttpResponse ResponseResult(ResultType resultType, string resultCode, string message = null, object data = null)
        {
            _result.Result = resultType;
            _result.Code = resultCode;
            _result.Message = message;
            _result.Data = data;
            return new OwnApiHttpResponse(_result);
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