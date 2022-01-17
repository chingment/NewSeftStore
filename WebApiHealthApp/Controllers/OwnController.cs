using System;
using System.Web.Http;
using Lumos;
using Lumos.Session;
using LocalS.Service.Api.Account;
using LocalS.Service.Api.HealthApp;
using MyWeiXinSdk;
using LocalS.BLL;
using Lumos.Redis;
using System.Collections.Generic;
using LocalS.BLL.Biz;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Aliyun.Acs.Dysmsapi.Model.V20170525;

namespace WebApiHealthApp.Controllers
{
    public class OwnController : OwnApiBaseController
    {
        //初始页面-我的信息
        [HttpGet]
        public OwnApiHttpResponse InitInfo()
        {
            var result = HealthAppServiceFactory.Own.InitInfo(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse AuthUrl(RopOwnAuthUrl rop)
        {
            var result = HealthAppServiceFactory.Own.AuthUrl(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse AuthInfo(RopOwnAuthInfo rop)
        {
            var result = HealthAppServiceFactory.Own.AuthInfo(rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse AuthTokenCheck(string token)
        {
            var result = HealthAppServiceFactory.Own.AuthTokenCheck(token);
            return new OwnApiHttpResponse(result);
        }

        public string BuildValidCode()
        {
            VerifyCodeHelper v = new VerifyCodeHelper();
            v.CodeSerial = "0,1,2,3,4,5,6,7,8,9";
            v.Length = 6;
            string code = v.CreateVerifyCode(); //取随机码 

            return code;
        }

        [HttpPost]
        public OwnApiHttpResponse GetPhoneValidCode(RopOwnGetPhoneVaildCode rop)
        {


            String product = "Dysmsapi";//短信API产品名称
            String domain = "dysmsapi.aliyuncs.com";//短信API产品域名
            CustomJsonResult result = new CustomJsonResult();
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAIBXXcSKEgAxxH", "XgZJ029tZR4upF6Qrbxq6YXywPsTIP");

            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);

            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();

            try
            {
                string validcode = BuildValidCode();
                string phoneNumber = "15989287032";
                string templateCode = "SMS_88990017";
                string templateParam = "{\"code\":\"" + validcode + "\"}";
                request.SignName = "贩聚社团";//"管理控制台中配置的短信签名（状态必须是验证通过）"
                request.PhoneNumbers = phoneNumber;//"接收号码，多个号码可以逗号分隔"
                request.TemplateCode = templateCode;//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
                request.TemplateParam = templateParam;//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"

                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);


                result = new CustomJsonResult(ResultType.Success,ResultCode.Success,  "发送成功");

            }
            catch (Exception  ex)
            {
                result = new CustomJsonResult(ResultType.Exception, ResultCode.Failure, "发送失败");
            }

            return new OwnApiHttpResponse(result);
        }
    }
}