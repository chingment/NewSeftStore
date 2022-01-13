using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
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
            var ret = new
            {
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Fill(string operater, string userId, RopQuestFill rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();
                var config_Senviv = BizFactory.Senviv.GetConfig(d_User.MerchId);
                if (d_User.TrdUserId == null)
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
                        birthday = d_User.Birthday.ToUnifiedFormatDateTime(),
                        height = 100,
                        weight = 100,
                        createtime = "2020-06-22T10:23:58.784Z", //创建时间
                        updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
                        SAS = d_User.Sex,
                        Perplex = d_User.Perplex, //目前困扰 （查看字典表）
                        OtherPerplex = d_User.PerplexOt, //目前困扰输入其它 ,
                        Medicalhistory = d_User.MedicalHis, //既往史 （查看字典表）
                        OtherFamilyhistory = d_User.MedicalHisOt, //既往史其它 ,
                        Medicine = d_User.Medicine, //用药情况 （查看字典表）
                        OtherMedicine = d_User.MedicalHisOt, //用药情况其它 ,
                        deptid = "46"
                    };

                    var r_Api_UserCreate = SdkFactory.Senviv.UserCreate(config_Senviv, post);
                    if (string.IsNullOrEmpty(r_Api_UserCreate.userid))
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

                    d_User.TrdUserId = r_Api_UserCreate.userid;
                    CurrentDb.SaveChanges();
                }

                var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId).FirstOrDefault();

                if (d_UserDevice == null)
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

                var r_Api_BindBox = SdkFactory.Senviv.BindBox(config_Senviv,d_User.TrdUserId, d_UserDevice.DeviceId);

                if (r_Api_BindBox.Result != 1)
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

                d_UserDevice.InfoFillTime = DateTime.Now;
                d_UserDevice.BindTime = DateTime.Now;
                d_UserDevice.BindStatus = Entity.SenvivUserDeviceBindStatus.Bind;
                d_UserDevice.Mender = operater;
                d_UserDevice.MendTime = DateTime.Now;

                CurrentDb.SaveChanges();

                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }


            return result;
        }
    }
}
