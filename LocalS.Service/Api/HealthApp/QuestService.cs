using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.HealthApp
{
    public class QuestService : BaseService
    {
        //初始页面-资料填写
        public CustomJsonResult InitFill(string operater, string userId)
        {

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
                    UserName = "",
                    Status = bindStatus
                });
            }

            var ret = new
            {
                Devices = devices,
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Fill(string operater, string userId, RopQuestFill rop)
        {
            var result = new CustomJsonResult();

            var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.UserId == userId).FirstOrDefault();

            var config_Senviv = BizFactory.Senviv.GetConfig(d_SenvivUser.DeptId);

            using (TransactionScope ts = new TransactionScope())
            {
                if (d_SenvivUser == null)
                {
                    var post = new
                    {
                        userid = "",
                        code = "",
                        mobile = "13800138000",
                        wechatid = "",
                        nick = d_User.NickName,
                        headimgurl = d_User.Avatar,
                        sex = d_User.Sex,
                        birthday = "2020-06-22T10:23:58.784Z",
                        height = 100,
                        weight = 100,
                        createtime = "2020-06-22T10:23:58.784Z", //创建时间
                        updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
                        SAS = d_User.Sex,
                        Perplex = "1", //目前困扰 （查看字典表）
                        OtherPerplex = "", //目前困扰输入其它 ,
                        Medicalhistory = "1", //既往史 （查看字典表）
                        OtherFamilyhistory = "", //既往史其它 ,
                        Medicine = "1", //用药情况 （查看字典表）
                        OtherMedicine = "", //用药情况其它 ,
                        deptid = config_Senviv.SenvivDeptId
                    };

                    var r_Api_UserCreate = SdkFactory.Senviv.UserCreate(config_Senviv, post);
                    if (string.IsNullOrEmpty(r_Api_UserCreate.userid))
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

                    d_SenvivUser = new Entity.SenvivUser();
                    d_SenvivUser.Id = r_Api_UserCreate.userid;
                    d_SenvivUser.MerchId = d_User.MerchId;
                    d_SenvivUser.UserId = d_User.Id;
                    d_SenvivUser.DeptId = config_Senviv.SenvivDeptId;
                    d_SenvivUser.NickName = d_User.NickName;
                    d_SenvivUser.Avatar = d_User.Avatar;
                    d_SenvivUser.PhoneNumber = d_User.PhoneNumber;
                    d_SenvivUser.CreateTime = DateTime.Now;
                    d_SenvivUser.Creator = d_User.Id;
                    CurrentDb.SenvivUser.Add(d_SenvivUser);
                    CurrentDb.SaveChanges();

                    ts.Complete();
                }
            }


            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();

            if (d_UserDevice == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

            var r_Api_BindBox = SdkFactory.Senviv.BindBox(config_Senviv, d_SenvivUser.Id, d_UserDevice.DeviceId);

            if (r_Api_BindBox.Result == 3)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备不存在");

            if (r_Api_BindBox.Result != 1)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

            d_UserDevice.SvUserId = d_SenvivUser.Id;
            d_UserDevice.InfoFillTime = DateTime.Now;
            d_UserDevice.BindTime = DateTime.Now;
            d_UserDevice.BindStatus = Entity.SenvivUserDeviceBindStatus.Binded;
            d_UserDevice.Mender = operater;
            d_UserDevice.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();


            BizFactory.Senviv.SendDeviceBind(userId, "您已成功绑定设备", "已绑定", DateTime.Now.ToUnifiedFormatDateTime(), "您好，您已成功绑定。");


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");


            return result;
        }
    }
}
