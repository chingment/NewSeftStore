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
using System.Transactions;
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

            if (string.IsNullOrEmpty(deviceId))
            {
                var d_UserBindDeviceCount = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.BindStatus == SenvivUserDeviceBindStatus.Binded).Count();

                if (d_UserBindDeviceCount > 0)
                {
                    step = 4;
                }
            }
            else
            {
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

        public CustomJsonResult InitManage(string operater, string userId)
        {
            var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            var d_UserDevices = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId).ToList();

            List<object> devices = new List<object>();

            foreach (var d_UserDevice in d_UserDevices)
            {
                var signName = d_User.NickName;

                if (!string.IsNullOrEmpty(d_UserDevice.SvUserId))
                {
                    var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == d_UserDevice.SvUserId).FirstOrDefault();
                    if (d_SenvivUser != null)
                    {
                        if (!string.IsNullOrEmpty(d_SenvivUser.FullName))
                        {
                            signName = d_SenvivUser.FullName;
                        }
                    }
                }

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
                    SignName = signName,
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

        public CustomJsonResult InitInfo(string operater, string userId, string deviceId)
        {
            var d_SenvivUserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == deviceId).FirstOrDefault();

            if (d_SenvivUserDevice == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");

            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == d_SenvivUserDevice.SvUserId).FirstOrDefault();
            if (d_SenvivUser == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");

            var ret = new
            {
                DeviceInfo = new
                {
                    SvUserId = d_SenvivUser.Id,
                    DeviceId = deviceId,
                    FullName = d_SenvivUser.FullName,
                    Sex = new FieldModel(d_SenvivUser.Sex, SvUtil.GetSexName(d_SenvivUser.Sex)),
                    Birthday = d_SenvivUser.Birthday.ToUnifiedFormatDate(),
                    Height = d_SenvivUser.Height,
                    Weight = d_SenvivUser.Weight,
                    Perplex = new FieldModel(GetValue(d_SenvivUser.Perplex), SvUtil.GetPerplexNames(d_SenvivUser.Perplex, d_SenvivUser.PerplexOt)),
                    Chronicdisease = new FieldModel(GetValue(d_SenvivUser.Chronicdisease), SvUtil.GetChronicdiseaseNames(d_SenvivUser.Chronicdisease, "")),
                    Medicalhis = new FieldModel(GetValue(d_SenvivUser.MedicalHis), SvUtil.GetMedicalHisNames(d_SenvivUser.MedicalHis, d_SenvivUser.MedicalHisOt)),
                    Medicine = new FieldModel(GetValue(d_SenvivUser.Medicine), SvUtil.GetMedicineNames(d_SenvivUser.Medicine, d_SenvivUser.MedicineOt)),
                    SubHealth = new FieldModel(GetValue(d_SenvivUser.SubHealth), SvUtil.GetSubHealthNames(d_SenvivUser.SubHealth, d_SenvivUser.SubHealthOt)),
                },
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);


        }

        public string[] GetValue(string val)
        {
            if (string.IsNullOrEmpty(val))
                return null;

            var arr = val.Split(',');

            return arr;

        }

        public CustomJsonResult InfoEdit(string operater, string userId, RopDeviceInfoEdit rop)
        {
            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == rop.SvUserId).FirstOrDefault();

            if (d_SenvivUser == null)
                new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败");

            if (rop.Answers.ContainsKey("fullName"))
            {
                d_SenvivUser.FullName = rop.Answers["fullName"].ToString();
            }

            if (rop.Answers.ContainsKey("sex"))
            {
                d_SenvivUser.Sex = rop.Answers["sex"].ToString();
            }

            if (rop.Answers.ContainsKey("birthday"))
            {
                d_SenvivUser.Birthday =Lumos.CommonUtil.ConverToDateTime(rop.Answers["birthday"].ToString());
            }

            if (rop.Answers.ContainsKey("sex"))
            {
                d_SenvivUser.Sex = rop.Answers["sex"].ToString();
            }

            if (rop.Answers.ContainsKey("height"))
            {
                d_SenvivUser.Height = rop.Answers["height"].ToString();
            }

            if (rop.Answers.ContainsKey("weight"))
            {
                d_SenvivUser.Weight = rop.Answers["weight"].ToString();
            }

            if (rop.Answers.ContainsKey("perplex"))
            {
                d_SenvivUser.Perplex = GetAnswerValue(rop.Answers["perplex"]);
            }

            if (rop.Answers.ContainsKey("subhealth"))
            {
                d_SenvivUser.SubHealth = GetAnswerValue(rop.Answers["subhealth"]);
            }


            if (rop.Answers.ContainsKey("chronicdisease"))
            {
                d_SenvivUser.Chronicdisease = GetAnswerValue(rop.Answers["chronicdisease"]);
            }

            if (rop.Answers.ContainsKey("medicalhis"))
            {
                d_SenvivUser.MedicalHis = GetAnswerValue(rop.Answers["medicalhis"]);
            }

            if (rop.Answers.ContainsKey("medicine"))
            {
                d_SenvivUser.Medicine = GetAnswerValue(rop.Answers["medicine"]);
            }


            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");


        }


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
                    BindStatus = bindStatus
                });
            }

            var ret = new
            {
                Devices = devices,
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Fill(string operater, string userId, RopDeviceFill rop)
        {
            var result = new CustomJsonResult();

            var d_Device_HasBind = CurrentDb.SenvivUserDevice.Where(m => m.DeviceId == rop.DeviceId && m.BindStatus == SenvivUserDeviceBindStatus.Binded).FirstOrDefault();

            if (d_Device_HasBind != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定");
            }

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();

            if (d_UserDevice == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "信息为空");

            var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();


            var config_Senviv = BizFactory.Senviv.GetConfig(d_UserDevice.SvDeptId);

            SenvivUser d_SenvivUser;

            string fullName = rop.Answers["fullName"].ToString();
            string sex = rop.Answers["sex"].ToString();
            string birthday = rop.Answers["birthday"].ToString();
            string height = rop.Answers["height"].ToString();
            string weight = rop.Answers["weight"].ToString();
            string perplex = GetAnswerValue(rop.Answers["perplex"]);
            string subhealth = GetAnswerValue(rop.Answers["subhealth"]);
            string chronicdisease = GetAnswerValue(rop.Answers["chronicdisease"]);
            string medicalhis = GetAnswerValue(rop.Answers["medicalhis"]);
            string medicine = GetAnswerValue(rop.Answers["medicine"]);
            string ladyidentity = rop.Answers["ladyidentity"].ToString();


            E_SenvivUserCareMode careMode = E_SenvivUserCareMode.Normal;

            if (sex == "2")
            {
                if (ladyidentity == "3")
                {
                    careMode = E_SenvivUserCareMode.Pregnancy;
                }
                else
                {
                    careMode = E_SenvivUserCareMode.Lady;
                }
            }

            LogUtil.Info("perplex:" + perplex);

            if (string.IsNullOrEmpty(d_UserDevice.SvUserId))
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    var post = new
                    {
                        userid = "",
                        code = "",
                        mobile = "13800138000",
                        wechatid = "",
                        nick = fullName,
                        headimgurl = d_User.Avatar,
                        sex = sex,
                        birthday = birthday,
                        height = height,
                        weight = weight,
                        createtime = "2020-06-22T10:23:58.784Z", //创建时间
                        updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
                        SAS = d_User.Sex,
                        Perplex = perplex, //目前困扰 （查看字典表）
                        OtherPerplex = "", //目前困扰输入其它 ,
                        Medicalhistory = medicalhis, //既往史 （查看字典表）
                        OtherFamilyhistory = "", //既往史其它 ,
                        Medicine = medicine, //用药情况 （查看字典表）
                        OtherMedicine = "", //用药情况其它 ,
                        deptid = config_Senviv.SvDeptId
                    };

                    var r_Api_UserCreate = SdkFactory.Senviv.UserCreate(config_Senviv, post);
                    if (string.IsNullOrEmpty(r_Api_UserCreate.userid))
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

                    d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == r_Api_UserCreate.userid).FirstOrDefault();
                    if (d_SenvivUser == null)
                    {
                        d_SenvivUser = new Entity.SenvivUser();
                        d_SenvivUser.Id = r_Api_UserCreate.userid;
                        d_SenvivUser.MerchId = d_User.MerchId;
                        d_SenvivUser.UserId = d_User.Id;
                        d_SenvivUser.SvDeptId = config_Senviv.SvDeptId;
                        d_SenvivUser.FullName = fullName;
                        d_SenvivUser.Height = height;
                        d_SenvivUser.Weight = weight;
                        d_SenvivUser.Perplex = perplex;
                        d_SenvivUser.MedicalHis = medicalhis;
                        d_SenvivUser.Medicine = medicine;
                        d_SenvivUser.SubHealth = subhealth;
                        d_SenvivUser.Chronicdisease = chronicdisease;
                        d_SenvivUser.Birthday = Lumos.CommonUtil.ConverToDateTime(birthday);
                        d_SenvivUser.NickName = null;
                        d_SenvivUser.Avatar = d_User.Avatar;
                        d_SenvivUser.PhoneNumber = d_User.PhoneNumber;
                        d_SenvivUser.Sex = sex;
                        d_SenvivUser.CareMode = careMode;
                        d_SenvivUser.CreateTime = DateTime.Now;
                        d_SenvivUser.Creator = d_User.Id;
                        CurrentDb.SenvivUser.Add(d_SenvivUser);
                        CurrentDb.SaveChanges();
                    }
                    else
                    {
                        d_SenvivUser.NickName = null;
                        d_SenvivUser.FullName = fullName;
                        d_SenvivUser.Sex = sex;
                        d_SenvivUser.Birthday = Lumos.CommonUtil.ConverToDateTime(birthday);
                        d_SenvivUser.Height = height;
                        d_SenvivUser.Weight = weight;
                        d_SenvivUser.Perplex = perplex;
                        d_SenvivUser.MedicalHis = medicalhis;
                        d_SenvivUser.Medicine = medicine;
                        d_SenvivUser.SubHealth = subhealth;
                        d_SenvivUser.Chronicdisease = chronicdisease;
                        d_SenvivUser.CareMode = careMode;
                        d_SenvivUser.MendTime = DateTime.Now;
                        d_SenvivUser.Mender = d_User.Id;
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();
                }
            }
            else
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var post = new
                    {
                        userid = d_UserDevice.SvUserId,
                        code = "",
                        mobile = "13800138000",
                        wechatid = "",
                        nick = fullName,
                        headimgurl = d_User.Avatar,
                        sex = sex,
                        birthday = birthday,
                        height = height,
                        weight = weight,
                        createtime = "2020-06-22T10:23:58.784Z", //创建时间
                        updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
                        SAS = sex,
                        Perplex = perplex, //目前困扰 （查看字典表）
                        OtherPerplex = "", //目前困扰输入其它 ,
                        Medicalhistory = medicalhis, //既往史 （查看字典表）
                        OtherFamilyhistory = "", //既往史其它 ,
                        Medicine = medicine, //用药情况 （查看字典表）
                        OtherMedicine = "", //用药情况其它 ,
                        deptid = config_Senviv.SvDeptId
                    };

                    var r_Api_UserCreate = SdkFactory.Senviv.UserCreate(config_Senviv, post);
                    if (string.IsNullOrEmpty(r_Api_UserCreate.userid))
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

                    d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == d_UserDevice.SvUserId).FirstOrDefault();
                    d_SenvivUser.NickName = null;
                    d_SenvivUser.FullName = fullName;
                    d_SenvivUser.Sex = sex;
                    d_SenvivUser.Birthday =Lumos.CommonUtil.ConverToDateTime(birthday);
                    d_SenvivUser.Height = height;
                    d_SenvivUser.Weight = weight;
                    d_SenvivUser.Perplex = perplex;
                    d_SenvivUser.MedicalHis = medicalhis;
                    d_SenvivUser.Medicine = medicine;
                    d_SenvivUser.SubHealth = subhealth;
                    d_SenvivUser.Chronicdisease = chronicdisease;
                    d_SenvivUser.CareMode = careMode;
                    d_SenvivUser.MendTime = DateTime.Now;
                    d_SenvivUser.Mender = d_User.Id;
                    CurrentDb.SaveChanges();
                    ts.Complete();
                }
            }

            if (sex == "2")
            {
                if (ladyidentity == "3")
                {
                    #region  孕妈
                    string[] geyweek = GetAnswerValue(rop.Answers["geyweek"]).Split(',');
                    string deliveryTime = rop.Answers["deliveryTime"].ToString();

                    var d_SenvivUserWomen = CurrentDb.SenvivUserWomen.Where(m => m.SvUserId == d_SenvivUser.Id).FirstOrDefault();
                    if (d_SenvivUserWomen == null)
                    {
                        d_SenvivUserWomen = new SenvivUserWomen();
                        d_SenvivUserWomen.Id = IdWorker.Build(IdType.NewGuid);
                        d_SenvivUserWomen.SvUserId = d_SenvivUser.Id;
                        d_SenvivUserWomen.DeliveryTime = DateTime.Parse(deliveryTime);
                        d_SenvivUserWomen.PregnancyTime = Lumos.CommonUtil.GetPregnancyTime(int.Parse(geyweek[0].ToString()), int.Parse(geyweek[1].ToString()));
                        CurrentDb.SenvivUserWomen.Add(d_SenvivUserWomen);
                        CurrentDb.SaveChanges();
                    }
                    else
                    {
                        d_SenvivUserWomen.DeliveryTime = DateTime.Parse(deliveryTime);
                        d_SenvivUserWomen.PregnancyTime = Lumos.CommonUtil.GetPregnancyTime(int.Parse(geyweek[0].ToString()), int.Parse(geyweek[1].ToString()));
                        CurrentDb.SaveChanges();
                    }

                    #endregion
                }
            }



            var r_Api_BindBox = SdkFactory.Senviv.BindBox(config_Senviv, d_SenvivUser.Id, d_UserDevice.DeviceId);

            if (r_Api_BindBox.Result == 3)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备不存在");

            if (r_Api_BindBox.Result == 5)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定");

            if (r_Api_BindBox.Result == 6)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被冻结");

            if (r_Api_BindBox.Result != 1)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("绑定失败[{0}]", r_Api_BindBox.Result));


            d_UserDevice.SvUserId = d_SenvivUser.Id;
            d_UserDevice.InfoFillTime = DateTime.Now;
            d_UserDevice.BindTime = DateTime.Now;
            d_UserDevice.BindStatus = Entity.SenvivUserDeviceBindStatus.Binded;
            d_UserDevice.Mender = operater;
            d_UserDevice.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();

            BizFactory.Senviv.SendDeviceBind(d_SenvivUser.Id, "您已成功绑定设备", "已绑定", DateTime.Now.ToUnifiedFormatDateTime(), "您好，您已成功绑定。");

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            //var d_User = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            //var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.UserId == userId).FirstOrDefault();

            //var config_Senviv = BizFactory.Senviv.GetConfig(d_SenvivUser.DeptId);

            //using (TransactionScope ts = new TransactionScope())
            //{
            //    if (d_SenvivUser == null)
            //    {
            //        var post = new
            //        {
            //            userid = "",
            //            code = "",
            //            mobile = "13800138000",
            //            wechatid = "",
            //            nick = d_User.NickName,
            //            headimgurl = d_User.Avatar,
            //            sex = d_User.Sex,
            //            birthday = "2020-06-22T10:23:58.784Z",
            //            height = 100,
            //            weight = 100,
            //            createtime = "2020-06-22T10:23:58.784Z", //创建时间
            //            updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
            //            SAS = d_User.Sex,
            //            Perplex = "1", //目前困扰 （查看字典表）
            //            OtherPerplex = "", //目前困扰输入其它 ,
            //            Medicalhistory = "1", //既往史 （查看字典表）
            //            OtherFamilyhistory = "", //既往史其它 ,
            //            Medicine = "1", //用药情况 （查看字典表）
            //            OtherMedicine = "", //用药情况其它 ,
            //            deptid = config_Senviv.SenvivDeptId
            //        };

            //        var r_Api_UserCreate = SdkFactory.Senviv.UserCreate(config_Senviv, post);
            //        if (string.IsNullOrEmpty(r_Api_UserCreate.userid))
            //            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

            //        d_SenvivUser = new Entity.SenvivUser();
            //        d_SenvivUser.Id = r_Api_UserCreate.userid;
            //        d_SenvivUser.MerchId = d_User.MerchId;
            //        d_SenvivUser.UserId = d_User.Id;
            //        d_SenvivUser.DeptId = config_Senviv.SenvivDeptId;
            //        d_SenvivUser.NickName = d_User.NickName;
            //        d_SenvivUser.Avatar = d_User.Avatar;
            //        d_SenvivUser.PhoneNumber = d_User.PhoneNumber;
            //        d_SenvivUser.CreateTime = DateTime.Now;
            //        d_SenvivUser.Creator = d_User.Id;
            //        CurrentDb.SenvivUser.Add(d_SenvivUser);
            //        CurrentDb.SaveChanges();

            //        ts.Complete();
            //    }
            //}

            //var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();

            //if (d_UserDevice == null)
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

            //var r_Api_BindBox = SdkFactory.Senviv.BindBox(config_Senviv, d_SenvivUser.Id, d_UserDevice.DeviceId);

            //if (r_Api_BindBox.Result == 3)
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备不存在");

            //if (r_Api_BindBox.Result != 1)
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "绑定失败");

            //d_UserDevice.SvUserId = d_SenvivUser.Id;
            //d_UserDevice.InfoFillTime = DateTime.Now;
            //d_UserDevice.BindTime = DateTime.Now;
            //d_UserDevice.BindStatus = Entity.SenvivUserDeviceBindStatus.Binded;
            //d_UserDevice.Mender = operater;
            //d_UserDevice.MendTime = DateTime.Now;
            //CurrentDb.SaveChanges();


            //BizFactory.Senviv.SendDeviceBind(userId, "您已成功绑定设备", "已绑定", DateTime.Now.ToUnifiedFormatDateTime(), "您好，您已成功绑定。");


            //result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");


            return result;
        }

        public CustomJsonResult BindSerialNo(string operater, string userId, RopDeviceBindSerialNo rop)
        {

            if (string.IsNullOrEmpty(rop.DeviceId))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备号不能为空");

            var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();
            if (d_Device == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备号不存在[1]");


            var app_Config = BizFactory.Senviv.GetWxAppConfigByUserId(userId);

            string merchId = app_Config.Exts["MerchId"];

            string wxPaOpenId = app_Config.Exts["WxPaOpenId"];


            var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.DeviceId == rop.DeviceId && m.IsStopUse == false).FirstOrDefault();

            if (d_MerchDevice == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备号不存在.");
            }

            var wx_UserInfo = SdkFactory.Wx.GetUserInfoByApiToken(app_Config, wxPaOpenId);

            if (wx_UserInfo == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注");
            }

            if (wx_UserInfo.subscribe <= 0)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注.");
            }

            var d_Device_HasBind = CurrentDb.SenvivUserDevice.Where(m => m.DeviceId == rop.DeviceId && m.BindStatus == SenvivUserDeviceBindStatus.Binded).FirstOrDefault();

            if (d_Device_HasBind != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定");
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

            var d_Device_HasBind = CurrentDb.SenvivUserDevice.Where(m => m.DeviceId == rop.DeviceId && m.BindStatus == SenvivUserDeviceBindStatus.Binded).FirstOrDefault();

            if (d_Device_HasBind != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定");
            }

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

            var d_SenvivUserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();

            var config_Senviv = BizFactory.Senviv.GetConfig(d_SenvivUserDevice.SvDeptId);

            var r_Api_BindBox = SdkFactory.Senviv.UnBindBox(config_Senviv, d_SenvivUserDevice.SvUserId, rop.DeviceId);

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

            BizFactory.Senviv.SendDeviceUnBind(d_UserDevice.SvUserId, "您已成功解绑设备，不再接收报告信息", rop.DeviceId, DateTime.Now.ToUnifiedFormatDateTime(), "感谢使用。");


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

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "验证码发送成功", new { TokenCode = key });
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "验证码发送失败");
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("发送短信", ex);

                result = new CustomJsonResult(ResultType.Exception, ResultCode.Failure, "验证码发送失败");
            }

            return result;
        }

    }
}
