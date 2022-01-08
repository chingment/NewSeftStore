using Lumos;
using System.Web;
using System.Configuration;
using Lumos.Web.Http;
using Lumos.Session;
using System.IO;
using System;
using System.Collections.Generic;

namespace WebApiHealthApp
{
    [OwnApiAuthorize]
    public class OwnApiBaseController : BaseController
    {
        public OwnApiBaseController()
        {
            LogUtil.SetTrackId();
        }

        public string CurrentUserId
        {
            get
            {
                string userId = null;

                var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                string token = request.Headers["X-Token"];

                if (!string.IsNullOrEmpty(token))
                {
                    var session = new Session();
                    var token_val = session.Get<Dictionary<string, string>>(string.Format("token:{0}", token));
                    if (token_val != null)
                    {
                        userId = token_val["userId"];
                        LogUtil.Info("userId2:" + userId);
                    }
                }

                return userId;
            }
        }

    }
}