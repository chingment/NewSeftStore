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

namespace LocalS.Service.Api.HealthApp
{
    public class DeviceService : BaseService
    {
        public CustomJsonResult InitBind(string operater, string userId, string deviceId)
        {
            var result = new CustomJsonResult();

            WxAppInfoConfig config = BizFactory.Senviv.GetWxAppInfoConfigByUserId(userId);

            int step = 1;

            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId && m.DeviceId == deviceId).FirstOrDefault();

            if (d_UserDevice != null)
            {
                if (d_UserDevice.BindDeviceTime == null)
                    step = 1;
                else if (d_UserDevice.BindPhoneTime == null)
                    step = 2;
                else if (d_UserDevice.BindInfoFillTime == null)
                    step = 3;
                else
                    step = 4;
            }

            var ret = new
            {
                UserInfo = new
                {
                    NickName = d_SenvivUser.NickName
                },
                PaInfo = new
                {

                },
                Step = step
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitInfo(string operater, string userId)
        {
            return null;
        }

        public CustomJsonResult BindSerialNo(string operater, string userId, RopDeviceBindSerialNo rop)
        {
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
                d_UserDevice.BindDeviceTime = DateTime.Now;
                d_UserDevice.Creator = operater;
                d_UserDevice.CreateTime = DateTime.Now;
                CurrentDb.SenvivUserDevice.Add(d_UserDevice);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_UserDevice.BindDeviceTime = DateTime.Now;
                d_UserDevice.Mender = operater;
                d_UserDevice.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
        }

        public CustomJsonResult BindPhoneNumber(string operater, string userId, RopDeviceBindPhoneNumber rop)
        {
            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_UserDevice == null)
            {
                d_UserDevice = new SenvivUserDevice();
                d_UserDevice.Id = IdWorker.Build(IdType.NewGuid);
                d_UserDevice.SvUserId = userId;
                d_UserDevice.DeviceId = rop.DeviceId;
                d_UserDevice.BindDeviceTime = DateTime.Now;
                d_UserDevice.BindPhoneTime = DateTime.Now;
                d_UserDevice.Creator = operater;
                d_UserDevice.CreateTime = DateTime.Now;
                CurrentDb.SenvivUserDevice.Add(d_UserDevice);
                CurrentDb.SaveChanges();
            }
            else
            {
                if(d_UserDevice.BindDeviceTime==null)
                    d_UserDevice.BindDeviceTime = DateTime.Now;
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

        public CustomJsonResult BindInfoFill(string operater, string userId, RopDeviceBindInfoFill rop)
        {
            return null;
        }
    }
}
