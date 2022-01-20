using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using MyWeiXinSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LocalS.Service.Api.HealthApp
{
    public class PhoneToken
    {
        public string PhoneNumber { get; set; }
        public string ValidCode { get; set; }
    }

    public class DeviceService : BaseService
    {
        public CustomJsonResult InitBind(string operater, string userId, string deviceId, string requestUrl)
        {
            var result = new CustomJsonResult();

            var app_Config = BizFactory.Senviv.GetWxAppConfigByUserId(userId);
            int step = 1;
            var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == deviceId).FirstOrDefault();

            if (d_UserDevice != null)
            {
                if (d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.NotBind || d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.UnBind)
                {
                    if (d_UserDevice.BindDeviceIdTime == null)
                        step = 1;
                    else if (d_UserDevice.BindPhoneTime == null)
                        step = 2;
                    else if (d_UserDevice.InfoFillTime == null)
                        step = 3;
                    else
                        step = 4;
                }
                else
                {
                    step = 4;
                }
            }

            var ret = new
            {
                UserInfo = new
                {
                    NickName = d_User.NickName
                },
                OpenJsSdk = SdkFactory.Wx.GetJsApiConfigParams(app_Config, HttpUtility.UrlDecode(requestUrl)),
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
                Step = step
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitInfo(string operater, string userId)
        {
            var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            var d_UserDevices = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId).ToList();

            List<object> devices = new List<object>();

            foreach (var d_UserDevice in d_UserDevices)
            {
                var bindStatus = new FieldModel();
                if (d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.NotBind)
                {
                    bindStatus = new FieldModel(1, "未绑定");
                }
                else if (d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.Binded)
                {
                    bindStatus = new FieldModel(2, "已绑定");
                }
                else if (d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.UnBind)
                {
                    bindStatus = new FieldModel(3, "已解绑");
                }

                devices.Add(new
                {
                    Id = d_UserDevice.DeviceId,
                    UserName = d_User.NickName,
                    BindTime = d_UserDevice.BindTime.ToUnifiedFormatDateTime(),
                    UnBindTime = d_UserDevice.UnBindTime.ToUnifiedFormatDateTime(),
                    BindStatus = bindStatus
                });
            }

            var ret = new
            {
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
                Devices = devices,
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

        }

        public CustomJsonResult BindSerialNo(string operater, string userId, RopDeviceBindSerialNo rop)
        {

            if (string.IsNullOrEmpty(rop.DeviceId))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备号不能为空");

            var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();
            if (d_Device == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备号不存在");

            var app_Config = BizFactory.Senviv.GetWxAppConfigByUserId(userId);

            string wxPaOpenId = app_Config.Exts["WxPaOpenId"];

            var wx_UserInfo = SdkFactory.Wx.GetUserInfoByApiToken(app_Config, wxPaOpenId);

            if (wx_UserInfo == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注");
            }

            if (wx_UserInfo.subscribe <= 0)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注.");
            }

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_UserDevice == null)
            {
                d_UserDevice = new SenvivUserDevice();
                d_UserDevice.Id = IdWorker.Build(IdType.NewGuid);
                d_UserDevice.UserId = userId;
                d_UserDevice.DeviceId = rop.DeviceId;
                d_UserDevice.SvDeptId = d_Device.SvDeptId;
                d_UserDevice.BindDeviceIdTime = DateTime.Now;
                d_UserDevice.BindStatus = SenvivUserDeviceBindStatus.NotBind;
                d_UserDevice.Creator = operater;
                d_UserDevice.CreateTime = DateTime.Now;
                CurrentDb.SenvivUserDevice.Add(d_UserDevice);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_UserDevice.BindStatus = SenvivUserDeviceBindStatus.NotBind;
                d_UserDevice.BindDeviceIdTime = DateTime.Now;
                d_UserDevice.Mender = operater;
                d_UserDevice.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
        }

        public CustomJsonResult BindPhoneNumber(string operater, string userId, RopDeviceBindPhoneNumber rop)
        {
            if (string.IsNullOrEmpty(rop.TokenCode))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请先获取手机验证码");

            var phoneToken = RedisClient.Get<PhoneToken>(string.Format("phone_valid_code:{0}", rop.TokenCode));

            if (phoneToken == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请先获取手机验证码");

            if (phoneToken.ValidCode != rop.ValidCode)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "手机验证码错误");

            // if (string.IsNullOrEmpty(rop.PhoneNumber))
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "手机号不能为空");

            var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_UserDevice == null)
            {
                d_UserDevice = new SenvivUserDevice();
                d_UserDevice.Id = IdWorker.Build(IdType.NewGuid);
                d_UserDevice.UserId = userId;
                d_UserDevice.DeviceId = rop.DeviceId;
                d_UserDevice.SvDeptId = d_Device.SvDeptId;
                d_UserDevice.BindDeviceIdTime = DateTime.Now;
                d_UserDevice.BindPhoneTime = DateTime.Now;
                d_UserDevice.Creator = operater;
                d_UserDevice.CreateTime = DateTime.Now;
                CurrentDb.SenvivUserDevice.Add(d_UserDevice);
                CurrentDb.SaveChanges();
            }
            else
            {
                if (d_UserDevice.BindDeviceIdTime == null)
                    d_UserDevice.BindDeviceIdTime = DateTime.Now;
                d_UserDevice.BindPhoneTime = DateTime.Now;
                d_UserDevice.Mender = operater;
                d_UserDevice.MendTime = DateTime.Now;
            }

            var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            d_User.PhoneNumber = phoneToken.PhoneNumber;
            d_User.Mender = operater;
            d_User.MendTime = DateTime.Now;

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
        }

        public CustomJsonResult UnBind(string operater, string userId, RopDeviceUnBind rop)
        {
            var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.UserId == userId).FirstOrDefault();

            var config_Senviv = BizFactory.Senviv.GetConfig(d_SenvivUser.SvDeptId);

            var r_Api_BindBox = SdkFactory.Senviv.UnBindBox(config_Senviv, d_SenvivUser.Id, rop.DeviceId);

            if (r_Api_BindBox.Result != 1 && r_Api_BindBox.Result != 5)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解绑失败");

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_UserDevice != null)
            {
                d_UserDevice.BindDeviceIdTime = null;
                d_UserDevice.BindPhoneTime = null;
                d_UserDevice.InfoFillTime = null;
                d_UserDevice.UnBindTime = DateTime.Now;
                d_UserDevice.BindStatus = SenvivUserDeviceBindStatus.UnBind;
                d_UserDevice.Creator = operater;
                d_UserDevice.CreateTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            BizFactory.Senviv.SendDeviceUnBind(userId, "您已成功解绑设备，不再接收报告信息", rop.DeviceId, DateTime.Now.ToUnifiedFormatDateTime(), "感觉使用。");


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "解绑成功");
        }


        public string BuildValidCode()
        {
            VerifyCodeHelper v = new VerifyCodeHelper();
            v.CodeSerial = "0,1,2,3,4,5,6,7,8,9";
            v.Length = 4;
            string code = v.CreateVerifyCode(); //取随机码 

            return code;
        }


        public CustomJsonResult GetPhoneValidCode(string operater, string userId, RopOwnGetPhoneVaildCode rop)
        {

            var result = new CustomJsonResult();

            String product = "Dysmsapi";//短信API产品名称
            String domain = "dysmsapi.aliyuncs.com";//短信API产品域名

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAI4GHVbVRpJJ4h2kSAmVc6", "yipmZ8XZ0Bw4p2p2CvEZrirPre46b3");

            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);

            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();

            var smsTemplate = BizFactory.Senviv.GetSmsTemplateByBindPhone(userId);
            try
            {
                string validcode = BuildValidCode();
                string phoneNumber = rop.PhoneNumber;
                string templateCode = smsTemplate.TemplateCode;// "SMS_88990017";
                string templateParam = "{\"code\":\"" + validcode + "\"}";
                request.SignName = smsTemplate.SignName;//贩聚社团,//"管理控制台中配置的短信签名（状态必须是验证通过）"
                request.PhoneNumbers = phoneNumber;//"接收号码，多个号码可以逗号分隔"
                request.TemplateCode = templateCode;//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
                request.TemplateParam = templateParam;//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"

                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                if (sendSmsResponse.Code == "OK")
                {
                    string key = IdWorker.Build(IdType.NewGuid);

                    var key_val = new PhoneToken { PhoneNumber = phoneNumber, ValidCode = validcode };

                    RedisClient.Set(string.Format("phone_valid_code:{0}", key), key_val, new TimeSpan(0, 2, 0));

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发送成功", new { TokenCode = key });
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发送失败");
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("发送短信", ex);

                result = new CustomJsonResult(ResultType.Exception, ResultCode.Failure, "发送失败");
            }

            return result;
        }

    }
}
