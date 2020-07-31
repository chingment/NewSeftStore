
using System.Web.Http;
using Lumos;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Web;
using LocalS.Service.Api.StoreApp;
using LocalS.Entity;
using MyWeiXinSdk;
using LocalS.BLL.Mq;
using Lumos.Redis;

namespace WebApiStoreApp.Controllers
{
    public class OrderController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse Confirm([FromBody]RopOrderConfirm rop)
        {
            IResult result = StoreAppServiceFactory.Order.Confrim(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Reserve([FromBody]RopOrderReserve rop)
        {
            IResult result = StoreAppServiceFactory.Order.Reserve(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse List([FromUri]RupOrderList rup)
        {
            IResult result = StoreAppServiceFactory.Order.List(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Details(string ids)
        {
            IResult result = StoreAppServiceFactory.Order.Details(this.CurrentUserId, this.CurrentUserId, ids);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Cancle(RopOrderCancle rop)
        {
            IResult result = StoreAppServiceFactory.Order.Cancle(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BuildPayParams([FromBody]RopOrderBuildPayParams rop)
        {
            rop.CreateIp = Lumos.CommonUtil.GetIpAddress(this.HttpRequest);

            IResult result = StoreAppServiceFactory.Order.BuildPayParams(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse BuildPayOptions([FromUri]RupOrderBuildPayOptions rup)
        {
            IResult result = StoreAppServiceFactory.Order.BuildPayOptions(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage PayResultNotifyByWx()
        {
            LogUtil.Info("PayResultNotifyByWx接收支付结果");

            string content = "";
            string rt = "";
            try
            {
                var myRequest = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                Stream stream = myRequest.InputStream;
                stream.Seek(0, SeekOrigin.Begin);

                content = new StreamReader(stream).ReadToEnd();

                LogUtil.Info("接收支付结果:" + content);

                if (!string.IsNullOrEmpty(content))
                {
                    MqFactory.Global.PushPayResultNotify(IdWorker.Build(IdType.NewGuid), E_PayPartner.Wx, E_PayTransLogNotifyFrom.NotifyUrl, content);
                }
            }
            finally
            {
                //微信支付异步返回结果
                rt = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(rt, Encoding.UTF8, "text/plain") };
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage PayResultNotifyByAlipay()
        {
            string content = "";
            string rt = "";
            try
            {
                var myRequest = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                Stream stream = myRequest.InputStream;
                stream.Seek(0, SeekOrigin.Begin);

                content = new StreamReader(stream).ReadToEnd();

                LogUtil.Info("接收支付结果:" + content);

                if (!string.IsNullOrEmpty(content))
                {

                    MqFactory.Global.PushPayResultNotify(IdWorker.Build(IdType.NewGuid), E_PayPartner.Zfb, E_PayTransLogNotifyFrom.NotifyUrl, content);
                }
            }
            finally
            {
                rt = "success";
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(rt, Encoding.UTF8, "text/plain") };
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage PayResultNotifyByTg()
        {
            string content = "";
            string rt = "";
            try
            {
                var myRequest = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                Stream stream = myRequest.InputStream;
                stream.Seek(0, SeekOrigin.Begin);

                content = new StreamReader(stream).ReadToEnd();

                LogUtil.Info("接收支付结果:" + content);

                if (!string.IsNullOrEmpty(content))
                {

                    MqFactory.Global.PushPayResultNotify(IdWorker.Build(IdType.NewGuid), E_PayPartner.Tg, E_PayTransLogNotifyFrom.NotifyUrl, content);
                }
            }
            finally
            {
                rt = "SUCCESS";

            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(rt, Encoding.UTF8, "text/plain") };

        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage PayResultNotifyByXrt()
        {
            string content = GetRequestContent();
            LogUtil.Info("接收支付结果:" + content);
            if (!string.IsNullOrEmpty(content))
            {
                MqFactory.Global.PushPayResultNotify(IdWorker.Build(IdType.NewGuid), E_PayPartner.Xrt,E_PayTransLogNotifyFrom.NotifyUrl, content);
            }
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("success", Encoding.UTF8, "text/plain") };
        }


        [HttpGet]
        public OwnApiHttpResponse ReceiptTimeAxis([FromUri]RupOrderReceiptTimeAxis rup)
        {
            IResult result = StoreAppServiceFactory.Order.ReceiptTimeAxis(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        
    }
}