
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


        public string CurrentMerchId
        {
            get
            {
                string merchId = null;

                var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;

                var str_auth = request.Headers["Authorization"];
                if (str_auth != null)
                {
                    str_auth = request.Headers["Authorization"].ToString();

                    string[] arr_auth2 = str_auth.Split(',');

                    foreach (var item in arr_auth2)
                    {
                        string[] keyvalue = item.Split('=');

                        if (keyvalue[0] == "merch_id")
                        {
                            merchId = keyvalue[1];
                        }
                    }

                }

                return merchId;

            }

        }


    }
}