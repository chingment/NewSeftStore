
using Lumos.Web.Http;
using Lumos;
using Lumos.Session;
using System.Web;

namespace WebApiIotTerm
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

        public OwnApiHttpResponse ResponseResult(string code, string msg = null, object data = null)
        {
            _result.Code = code;
            _result.Msg = msg;
            _result.Data = data;

            return new OwnApiHttpResponse(_result);
        }
    }
}