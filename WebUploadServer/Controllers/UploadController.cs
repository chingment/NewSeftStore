using log4net;
using Lumos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;

namespace WebUploadServer.Controllers
{
    public class UploadController : ApiController
    {
        private static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public HttpResponseMessage Post()
        {
            log.Info("调用Upload.Post");

            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象 
            CustomJsonResult r = new CustomJsonResult();

            log.Info("文件Count:" + request.Files.Count);
            for (var i = 0; i < request.Files.Count; i++)
            {
                log.Info("文件名称:" + request.Files[i].FileName);
            }

            log.Info("表单Count:" + request.Form.Count);
            for (var i = 0; i < request.Form.Count; i++)
            {
                log.Info("文件名称:" + request.Form[i]);
            }


            string json = r.ToString();
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(json, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }
}
