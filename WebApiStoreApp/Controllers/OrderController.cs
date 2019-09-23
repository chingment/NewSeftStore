﻿
using System.Web.Http;
using Lumos;
using System.IO;
using System.Net.Http;
using Lumos.BLL;
using System.Net;
using System.Text;
using System.Web;
using LocalS.Service.Api.StoreApp;
using LocalS.Entity;
using WeiXinSdk;

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
        public CustomJsonResult Details(string id)
        {
            return StoreAppServiceFactory.Order.Details(this.CurrentUserId, this.CurrentUserId, id);
        }

        [HttpPost]
        public CustomJsonResult Cancle(RopOrderCancle rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            return StoreAppServiceFactory.Order.Cancle(this.CurrentUserId, this.CurrentUserId, rop);
        }

        [HttpGet]
        public OwnApiHttpResponse JsApiPaymentPms([FromUri]RupOrderJsApiPaymentPms rop)
        {
            IResult result = StoreAppServiceFactory.Order.JsApiPaymentPms(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }


        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage PayResultNotify()
        {
            var myRequest = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
            Stream stream = myRequest.InputStream;
            stream.Seek(0, SeekOrigin.Begin);
            string xml = new StreamReader(stream).ReadToEnd();

            if (string.IsNullOrEmpty(xml))
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };
            }

            LogUtil.Info("接收支付结果:" + xml);

            var dicXml = WeiXinSdk.CommonUtil.ToDictionary(xml);
            if (!dicXml.ContainsKey("appid"))
            {
                LogUtil.Warn("查找不到appid");
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };
            }

            string str_attach = dicXml["attach"].ToString();

            if (!string.IsNullOrEmpty(str_attach))
            {
                LogUtil.Warn("attach 不符合格式");
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };
            }

            var obj_attach = Newtonsoft.Json.JsonConvert.DeserializeObject< LocalS.BLL.Biz.OrderAttachModel >(str_attach);

            string appId = dicXml["appid"].ToString();

            WxAppInfoConfig appInfo = null;

            switch (obj_attach.Caller)
            {
                case 1:
                    break;
                case 2:
                    appInfo = StoreAppServiceFactory.Order.GetWxMpAppInfoConfig(obj_attach.MerchId, appId);
                    break;
            }

            if (!SdkFactory.Wx.CheckPayNotifySign(appInfo, xml))
            {
                LogUtil.Warn("支付通知结果签名验证失败");
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };
            }

            string orderSn = "";

            if (dicXml.ContainsKey("out_trade_no") && dicXml.ContainsKey("result_code"))
            {
                orderSn = dicXml["out_trade_no"].ToString();
            }

            bool isPaySuccessed = false;

            var result = StoreAppServiceFactory.Order.PayResultNotify(GuidUtil.Empty(), E_OrderNotifyLogNotifyFrom.NotifyUrl, xml, orderSn, out isPaySuccessed);

            if (result.Result == ResultType.Success)
            {
                string sb = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(sb, Encoding.UTF8, "text/plain") };
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };

        }
    }
}