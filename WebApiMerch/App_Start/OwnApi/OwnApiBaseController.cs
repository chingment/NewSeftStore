using Lumos;
using System.Web;
using System.Configuration;
using Lumos.Web.Http;
using Lumos.Session;

namespace WebApiMerch
{
    [OwnApiAuthorize]
    public class OwnApiBaseController : BaseController
    {
        public OwnApiBaseController()
        {
            LogUtil.SetTrackId(OwnApiRequest.Token);
        }

        //public string Token
        //{
        //    get
        //    {
        //        var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
        //        var token = request.QueryString["token"];
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            token = request.Headers["X-Token"];
        //            if (token != null)
        //            {
        //                token = request.Headers["X-Token"].ToString();
        //            }
        //        }

        //        return token;
        //    }
        //}
        //private TokenInfo TokenInfo
        //{
        //    get
        //    {
        //        var tokenInfo = SSOUtil.GetTokenInfo(this.Token);
        //        if (tokenInfo == null)
        //        {
        //            tokenInfo = new TokenInfo();
        //            tokenInfo.UserId = "";
        //            tokenInfo.MerchId = "";

        //        }
        //        return tokenInfo;
        //    }
        //}

        public string Token
        {
            get
            {
                return OwnApiRequest.Token;
            }
        }

        public string CurrentUserId
        {
            get
            {
                return OwnApiRequest.TokenInfo.UserId;
            }
        }

        public string CurrentMerchId
        {
            get
            {
                return OwnApiRequest.TokenInfo.BelongId;
            }

        }
    }
}