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
    public class DeviceService : BaseService
    {
        public CustomJsonResult InitBind(string operater, string userId, string deviceId, string requestUrl)
        {
            var result = new CustomJsonResult();

            WxAppInfoConfig config = BizFactory.Senviv.GetWxAppInfoConfigByUserId(userId);

            int step = 1;

            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId && m.DeviceId == deviceId).FirstOrDefault();

            if (d_UserDevice != null)
            {
                if (d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.Bind)
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
                    step = 1;
                }
            }

            var ret = new
            {
                UserInfo = new
                {
                    NickName = d_SenvivUser.NickName
                },
                OpenJsSdk = SdkFactory.Wx.GetJsApiConfigParams(config, HttpUtility.UrlDecode(requestUrl)),
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
                Step = step
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitInfo(string operater, string userId)
        {
            var d_UserDevices = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId).ToList();

            List<object> devices = new List<object>();

            foreach (var d_UserDevice in d_UserDevices)
            {
                var bindStatus = new FieldModel();
                if (d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.Bind)
                {
                    bindStatus = new FieldModel(1, "已绑定");
                }
                else if (d_UserDevice.BindStatus == SenvivUserDeviceBindStatus.UnBind)
                {
                    bindStatus = new FieldModel(2, "已解绑");
                }

                devices.Add(new
                {
                    Id = d_UserDevice.DeviceId,
                    UserSignName = "ddd",
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
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备号未生效");

            WxAppInfoConfig config = BizFactory.Senviv.GetWxAppInfoConfigByUserId(userId);

            var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

            var wx_UserInfo = SdkFactory.Wx.GetUserInfoByApiToken(config, d_User.WxOpenId);

            if (wx_UserInfo == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注");
            }

            if (wx_UserInfo.subscribe <= 0)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注.");
            }

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_UserDevice == null)
            {
                d_UserDevice = new SenvivUserDevice();
                d_UserDevice.Id = IdWorker.Build(IdType.NewGuid);
                d_UserDevice.SvUserId = userId;
                d_UserDevice.DeviceId = rop.DeviceId;
                d_UserDevice.BindDeviceIdTime = DateTime.Now;
                d_UserDevice.Creator = operater;
                d_UserDevice.CreateTime = DateTime.Now;
                CurrentDb.SenvivUserDevice.Add(d_UserDevice);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_UserDevice.BindDeviceIdTime = DateTime.Now;
                d_UserDevice.Mender = operater;
                d_UserDevice.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
        }

        public CustomJsonResult BindPhoneNumber(string operater, string userId, RopDeviceBindPhoneNumber rop)
        {
            if (string.IsNullOrEmpty(rop.PhoneNumber))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "手机号不能为空");

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_UserDevice == null)
            {
                d_UserDevice = new SenvivUserDevice();
                d_UserDevice.Id = IdWorker.Build(IdType.NewGuid);
                d_UserDevice.SvUserId = userId;
                d_UserDevice.DeviceId = rop.DeviceId;
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

            var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

            d_User.PhoneNumber = rop.PhoneNumber;
            d_User.Mender = operater;
            d_User.MendTime = DateTime.Now;

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
        }

        public CustomJsonResult UnBind(string operater, string userId, RopDeviceUnBind rop)
        {
            var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

            var config_Senviv = BizFactory.Senviv.GetConfig(d_User.MerchId);

            var r_Api_BindBox = SdkFactory.Senviv.UnBindBox(config_Senviv,d_User.TrdUserId, rop.DeviceId);

            if (r_Api_BindBox.Result != 1 && r_Api_BindBox.Result != 5)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解绑失败");

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_UserDevice != null)
            {
                d_UserDevice.UnBindTime = DateTime.Now;
                d_UserDevice.BindStatus = SenvivUserDeviceBindStatus.UnBind;
                d_UserDevice.Creator = operater;
                d_UserDevice.CreateTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "解绑成功");
        }
    }
}
