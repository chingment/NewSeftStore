
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
        public OwnApiHttpResponse Details(string id)
        {
            IResult result = StoreAppServiceFactory.Order.Details(this.CurrentUserId, this.CurrentUserId, id);
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
            IResult result = StoreAppServiceFactory.Order.BuildPayParams(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage PayResultNotifyByWx()
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
                    MqFactory.Global.PushPayResultNotify(GuidUtil.New(), E_OrderPayPartner.Wx, E_OrderNotifyLogNotifyFrom.NotifyUrl, content);
                }
            }
            catch (System.Exception ex)
            {

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

                    MqFactory.Global.PushPayResultNotify(GuidUtil.New(), E_OrderPayPartner.Ali, E_OrderNotifyLogNotifyFrom.NotifyUrl, content);
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                rt = "success";
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(rt, Encoding.UTF8, "text/plain") };

            //var dicXml = MyWeiXinSdk.CommonUtil.ToDictionary(content);

            //if (!dicXml.ContainsKey("appid"))
            //{
            //    LogUtil.Warn("查找不到appid");
            //    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };
            //}

            //string str_attach = dicXml["attach"].ToString();

            //if (string.IsNullOrEmpty(str_attach))
            //{
            //    LogUtil.Warn("attach 不符合格式");
            //    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };
            //}

            //var obj_attach = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalS.BLL.Biz.OrderAttachModel>(str_attach);

            //WxAppInfoConfig appInfo = null;

            //switch (obj_attach.PayCaller)
            //{
            //    case E_OrderPayCaller.WechatByNative:
            //        appInfo = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(obj_attach.MerchId);
            //        break;
            //    case E_OrderPayCaller.WechatByMp:
            //        appInfo = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(obj_attach.MerchId);
            //        break;
            //    case E_OrderPayCaller.WechatByPa:
            //        appInfo = LocalS.BLL.Biz.BizFactory.Merch.GetWxPaAppInfoConfig(obj_attach.MerchId);
            //        break;
            //}

            //if (!SdkFactory.Wx.CheckPayNotifySign(appInfo, xml))
            //{
            //    LogUtil.Warn("支付通知结果签名验证失败");
            //    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", Encoding.UTF8, "text/plain") };
            //}
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

                    MqFactory.Global.PushPayResultNotify(GuidUtil.New(), E_OrderPayPartner.Tg, E_OrderNotifyLogNotifyFrom.NotifyUrl, content);
                }
            }
            catch (System.Exception ex)
            {

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

                    //MqFactory.Global.PushPayResultNotify(GuidUtil.New(), E_OrderPayPartner.Tg, E_OrderNotifyLogNotifyFrom.NotifyUrl, content);
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                rt = "SUCCESS";

            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(rt, Encoding.UTF8, "text/plain") };

        }
    }
}