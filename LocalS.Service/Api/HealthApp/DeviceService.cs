﻿using Aliyun.Acs.Core;
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
        public string GetAnswerValueStr(object obj)
        {
            if (obj == null)
                return "";

            return obj.ToString();
        }

        public string GetAnswerValueArrStr(object obj)
        {
            string str = null;
            try
            {


                string t1 = obj.ToJsonString();

                string[] a1 = t1.ToJsonObject<List<string>>().ToArray();
                str = string.Join(",", a1);

                return str;
            }
            catch (Exception ex)
            {

            }

            return str;
        }

        public string[] GetAnswerValueArrObj(object obj)
        {
            string[] str = null;
            try
            {

                string t1 = obj.ToJsonString();

                string[] a1 = t1.ToJsonObject<List<string>>().ToArray();

                return a1;
            }
            catch (Exception ex)
            {

            }

            return str;
        }



        public string[] ArrStrConvert2ArrObj(string val)
        {
            if (string.IsNullOrEmpty(val))
                return null;

            var arr = val.Split(',');

            return arr;

        }

        public CustomJsonResult InitBind(string operater, string userId, string deviceId, string requestUrl)
        {
            var result = new CustomJsonResult();

            var cfg_App = BizFactory.Senviv.GetWxAppConfigByUserId(userId);

            int step = 1;

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            if (string.IsNullOrEmpty(deviceId))
            {
                var d_ClientUserBindDeviceCount = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.BindStatus == E_SvUserDeviceBindStatus.Binded).Count();

                if (d_ClientUserBindDeviceCount > 0)
                {
                    step = 4;
                }
            }
            else
            {
                var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.DeviceId == deviceId).FirstOrDefault();

                if (d_SvUserDevice != null)
                {
                    if (d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.NotBind || d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.UnBind)
                    {
                        if (d_SvUserDevice.BindDeviceIdTime == null)
                            step = 1;
                        else if (d_SvUserDevice.BindPhoneTime == null)
                            step = 2;
                        else if (d_SvUserDevice.InfoFillTime == null)
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
                    NickName = d_ClientUser.NickName
                },
                OpenJsSdk = SdkFactory.Wx.GetJsApiConfigParams(cfg_App, HttpUtility.UrlDecode(requestUrl)),
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
                Step = step
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManage(string operater, string userId)
        {
            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            var d_SvUserDevices = CurrentDb.SvUserDevice.Where(m => m.UserId == userId).ToList();

            List<object> devices = new List<object>();

            foreach (var d_SvUserDevice in d_SvUserDevices)
            {
                var signName = d_ClientUser.NickName;

                if (!string.IsNullOrEmpty(d_SvUserDevice.SvUserId))
                {
                    var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == d_SvUserDevice.SvUserId).FirstOrDefault();
                    if (d_SvUser != null)
                    {
                        if (!string.IsNullOrEmpty(d_SvUser.FullName))
                        {
                            signName = d_SvUser.FullName;
                        }
                    }
                }

                var bindStatus = new FieldModel();
                if (d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.NotBind)
                {
                    bindStatus = new FieldModel(1, "未绑定");
                }
                else if (d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.Binded)
                {
                    bindStatus = new FieldModel(2, "已绑定");
                }
                else if (d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.UnBind)
                {
                    bindStatus = new FieldModel(3, "已解绑");
                }

                devices.Add(new
                {
                    Id = d_SvUserDevice.DeviceId,
                    SignName = signName,
                    BindTime = d_SvUserDevice.BindTime.ToUnifiedFormatDateTime(),
                    UnBindTime = d_SvUserDevice.UnBindTime.ToUnifiedFormatDateTime(),
                    BindStatus = bindStatus,
                    WebUrl = d_SvUserDevice.WebUrl,
                    OnLineStatus = new FieldModel(0, "连接中")
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
            var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.DeviceId == deviceId).FirstOrDefault();

            if (d_SvUserDevice == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");

            var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == d_SvUserDevice.SvUserId).FirstOrDefault();
            if (d_SvUser == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");

            var deliveryTime = "";
            FieldModel geyweek = new FieldModel(new int[] { 0, 0 }, "");
            FieldModel gmPeriod = new FieldModel(new string[] { "", "6", "28" }, "");
            if (d_SvUser.Sex == "2")
            {
                var d_SvUserWomen = CurrentDb.SvUserWomen.Where(m => m.SvUserId == d_SvUser.Id).FirstOrDefault();
                if (d_SvUserWomen != null)
                {
                    deliveryTime = d_SvUserWomen.DeliveryTime.ToUnifiedFormatDate();

                    if (d_SvUserWomen.PregnancyTime != null)
                    {
                        var week = Lumos.CommonUtil.GetDiffWeekDay(d_SvUserWomen.PregnancyTime.Value, DateTime.Now);
                        geyweek = new FieldModel(new int[] { week.Week, week.Day }, string.Format("{0}周+{1}", week.Week, week.Day));
                    }

                    gmPeriod = new FieldModel(new string[] { d_SvUserWomen.YjLastDay.ToUnifiedFormatDate(), d_SvUserWomen.YjDuration.ToString(), d_SvUserWomen.YjCycle.ToString() },
                        string.Format("月经周期:{0}天，经期:{1},末次时间:{2}", d_SvUserWomen.YjCycle.ToString(), d_SvUserWomen.YjDuration.ToString(), d_SvUserWomen.YjLastDay.ToUnifiedFormatDate()));
                }
            }

            var ret = new
            {
                DeviceInfo = new
                {
                    SvUserId = d_SvUser.Id,
                    DeviceId = deviceId,
                    FullName = d_SvUser.FullName,
                    Sex = new FieldModel(d_SvUser.Sex, SvUtil.GetSexName(d_SvUser.Sex)),
                    Birthday = d_SvUser.Birthday.ToUnifiedFormatDate(),
                    Height = d_SvUser.Height,
                    Weight = d_SvUser.Weight,
                    CareMode = d_SvUser.CareMode,
                    Geyweek = geyweek,
                    DeliveryTime = deliveryTime,
                    GmPeriod = gmPeriod,
                    Identity = SvUtil.GetIdentity(d_SvUser.CareMode),
                    Perplex = new FieldModel(ArrStrConvert2ArrObj(d_SvUser.Perplex), SvUtil.GetPerplexNames(d_SvUser.Perplex, d_SvUser.PerplexOt)),
                    Chronicdisease = new FieldModel(ArrStrConvert2ArrObj(d_SvUser.Chronicdisease), SvUtil.GetChronicdiseaseNames(d_SvUser.Chronicdisease, "")),
                    Medicalhis = new FieldModel(ArrStrConvert2ArrObj(d_SvUser.MedicalHis), SvUtil.GetMedicalHisNames(d_SvUser.MedicalHis, d_SvUser.MedicalHisOt)),
                    Medicine = new FieldModel(ArrStrConvert2ArrObj(d_SvUser.Medicine), SvUtil.GetMedicineNames(d_SvUser.Medicine, d_SvUser.MedicineOt)),
                    SubHealth = new FieldModel(ArrStrConvert2ArrObj(d_SvUser.SubHealth), SvUtil.GetSubHealthNames(d_SvUser.SubHealth, d_SvUser.SubHealthOt)),
                },
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);


        }

        public CustomJsonResult InfoEdit(string operater, string userId, RopDeviceInfoEdit rop)
        {
            var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == rop.SvUserId).FirstOrDefault();

            if (d_SvUser == null)
                new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败");

            if (rop.Answers.ContainsKey("fullName"))
            {
                d_SvUser.FullName = rop.Answers["fullName"].ToString();
            }

            if (rop.Answers.ContainsKey("sex"))
            {
                d_SvUser.Sex = rop.Answers["sex"].ToString();
            }

            if (rop.Answers.ContainsKey("birthday"))
            {
                d_SvUser.Birthday = Lumos.CommonUtil.ConverToDateTime(rop.Answers["birthday"].ToString());
            }

            if (rop.Answers.ContainsKey("height"))
            {
                d_SvUser.Height = rop.Answers["height"].ToString();
            }

            if (rop.Answers.ContainsKey("weight"))
            {
                d_SvUser.Weight = rop.Answers["weight"].ToString();
            }

            if (rop.Answers.ContainsKey("perplex"))
            {
                d_SvUser.Perplex = GetAnswerValueArrStr(rop.Answers["perplex"]);
            }

            if (rop.Answers.ContainsKey("subhealth"))
            {
                d_SvUser.SubHealth = GetAnswerValueArrStr(rop.Answers["subhealth"]);
            }

            if (rop.Answers.ContainsKey("chronicdisease"))
            {
                d_SvUser.Chronicdisease = GetAnswerValueArrStr(rop.Answers["chronicdisease"]);
            }

            if (rop.Answers.ContainsKey("medicalhis"))
            {
                d_SvUser.MedicalHis = GetAnswerValueArrStr(rop.Answers["medicalhis"]);
            }

            if (rop.Answers.ContainsKey("medicine"))
            {
                d_SvUser.Medicine = GetAnswerValueArrStr(rop.Answers["medicine"]);
            }

            if (rop.Answers.ContainsKey("identity"))
            {
                string identity = rop.Answers["identity"].ToString();

                d_SvUser.CareMode = GetCareMode(d_SvUser.Sex, identity);
            }


            if (rop.Answers.ContainsKey("geyweek"))
            {
                SaveSvUserWomenInfo(d_SvUser.Id, d_SvUser.SvDeptId, d_SvUser.CareMode, "geyweek", rop.Answers["geyweek"]);
            }

            if (rop.Answers.ContainsKey("deliveryTime"))
            {
                SaveSvUserWomenInfo(d_SvUser.Id, d_SvUser.SvDeptId, d_SvUser.CareMode, "deliveryTime", rop.Answers["deliveryTime"]);
            }

            if (rop.Answers.ContainsKey("gmPeriod"))
            {
                SaveSvUserWomenInfo(d_SvUser.Id, d_SvUser.SvDeptId, d_SvUser.CareMode, "gmPeriod", rop.Answers["gmPeriod"]);
            }



            var result_SaveSvUserInfo = SaveSvUserInfo(d_SvUser.Id, d_SvUser.SvDeptId, d_SvUser.FullName,
                  d_SvUser.Avatar, d_SvUser.Sex, d_SvUser.Birthday.ToUnifiedFormatDate(), d_SvUser.Height, d_SvUser.Weight,
                  d_SvUser.Perplex, d_SvUser.MedicalHis, d_SvUser.Medicine);

            if (result_SaveSvUserInfo.Result != ResultType.Success)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败");


            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

        }


        //初始页面-资料填写
        public CustomJsonResult InitFill(string operater, string userId)
        {

            var d_SvUserDevices = CurrentDb.SvUserDevice.Where(m => m.UserId == userId).ToList();

            List<object> devices = new List<object>();

            foreach (var d_SvUserDevice in d_SvUserDevices)
            {
                var bindStatus = new FieldModel();
                if (d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.NotBind)
                {
                    bindStatus = new FieldModel(1, "未绑定");
                }
                else if (d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.Binded)
                {
                    bindStatus = new FieldModel(2, "已绑定");
                }
                else if (d_SvUserDevice.BindStatus == E_SvUserDeviceBindStatus.UnBind)
                {
                    bindStatus = new FieldModel(3, "已解绑");
                }

                devices.Add(new
                {
                    Id = d_SvUserDevice.DeviceId,
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


        private E_SvUserCareMode GetCareMode(string sex, string identity)
        {
            E_SvUserCareMode careMode = E_SvUserCareMode.Normal;

            if (sex == "2")
            {
                if (identity == "21")
                {
                    careMode = E_SvUserCareMode.Lady;
                }
                else if (identity == "22")
                {
                    careMode = E_SvUserCareMode.PrePregnancy;
                }
                else if (identity == "23")
                {
                    careMode = E_SvUserCareMode.Pregnancy;
                }
                else if (identity == "24")
                {
                    careMode = E_SvUserCareMode.Postpartum;
                }
                else
                {
                    careMode = E_SvUserCareMode.Lady;
                }
            }

            return careMode;
        }

        public CustomJsonResult Fill(string operater, string userId, RopDeviceFill rop)
        {
            var result = new CustomJsonResult();

            var d_Device_HasBind = CurrentDb.SvUserDevice.Where(m => m.DeviceId == rop.DeviceId && m.BindStatus == E_SvUserDeviceBindStatus.Binded).FirstOrDefault();

            if (d_Device_HasBind != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定");
            }

            var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();

            if (d_SvUserDevice == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "信息为空");

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            var config_Senviv = BizFactory.Senviv.GetConfig(d_SvUserDevice.SvDeptId);

            string fullName = GetAnswerValueStr(rop.Answers["fullName"]);
            string sex = GetAnswerValueStr(rop.Answers["sex"]);
            string birthday = GetAnswerValueStr(rop.Answers["birthday"]);
            string height = GetAnswerValueStr(rop.Answers["height"]);
            string weight = GetAnswerValueStr(rop.Answers["weight"]);
            string perplex = GetAnswerValueArrStr(rop.Answers["perplex"]);
            string subhealth = GetAnswerValueArrStr(rop.Answers["subhealth"]);
            string chronicdisease = GetAnswerValueArrStr(rop.Answers["chronicdisease"]);
            string medicalhis = GetAnswerValueArrStr(rop.Answers["medicalhis"]);
            string medicine = GetAnswerValueArrStr(rop.Answers["medicine"]);
            string identity = GetAnswerValueStr(rop.Answers["identity"]);
            string deliveryTime = GetAnswerValueStr(rop.Answers["deliveryTime"]);
            string[] geyweek = GetAnswerValueArrObj(rop.Answers["geyweek"]);
            string[] gmPeriod = GetAnswerValueArrObj(rop.Answers["gmPeriod"]);

            var careMode = GetCareMode(sex, identity);

            var post = new
            {
                userid = d_SvUserDevice.SvUserId == null ? "" : d_SvUserDevice.SvUserId,
                code = "",
                mobile = "13800138000",
                wechatid = "",
                nick = fullName,
                headimgurl = d_ClientUser.Avatar,
                sex = sex,
                birthday = birthday,
                height = height,
                weight = weight,
                createtime = "2020-06-22T10:23:58.784Z", //创建时间
                updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
                SAS = d_ClientUser.Sex,
                Perplex = "4,5,6", //目前困扰 （查看字典表）
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

            var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == r_Api_UserCreate.userid).FirstOrDefault();
            if (d_SvUser == null)
            {
                d_SvUser = new Entity.SvUser();
                d_SvUser.Id = r_Api_UserCreate.userid;
                d_SvUser.MerchId = d_ClientUser.MerchId;
                d_SvUser.UserId = d_ClientUser.Id;
                d_SvUser.SvDeptId = config_Senviv.SvDeptId;
                d_SvUser.FullName = fullName;
                d_SvUser.Height = height;
                d_SvUser.Weight = weight;
                d_SvUser.Perplex = perplex;
                d_SvUser.MedicalHis = medicalhis;
                d_SvUser.Medicine = medicine;
                d_SvUser.SubHealth = subhealth;
                d_SvUser.Chronicdisease = chronicdisease;
                d_SvUser.Birthday = Lumos.CommonUtil.ConverToDateTime(birthday);
                d_SvUser.Avatar = d_ClientUser.Avatar;
                d_SvUser.PhoneNumber = d_ClientUser.PhoneNumber;
                d_SvUser.Sex = sex;
                d_SvUser.CareMode = careMode;
                d_SvUser.CreateTime = DateTime.Now;
                d_SvUser.Creator = d_ClientUser.Id;
                CurrentDb.SvUser.Add(d_SvUser);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_SvUser.FullName = fullName;
                d_SvUser.Sex = sex;
                d_SvUser.Birthday = Lumos.CommonUtil.ConverToDateTime(birthday);
                d_SvUser.Height = height;
                d_SvUser.Weight = weight;
                d_SvUser.Perplex = perplex;
                d_SvUser.MedicalHis = medicalhis;
                d_SvUser.Medicine = medicine;
                d_SvUser.SubHealth = subhealth;
                d_SvUser.Chronicdisease = chronicdisease;
                d_SvUser.CareMode = careMode;
                d_SvUser.MendTime = DateTime.Now;
                d_SvUser.Mender = d_ClientUser.Id;
            }

            d_SvUserDevice.SvUserId = d_SvUser.Id;

            CurrentDb.SaveChanges();

            #region 女性

            if (sex == "2")
            {
                var d_SvUserWomen = CurrentDb.SvUserWomen.Where(m => m.SvUserId == d_SvUser.Id).FirstOrDefault();
                if (d_SvUserWomen == null)
                {
                    d_SvUserWomen = new SvUserWomen();
                    d_SvUserWomen.Id = IdWorker.Build(IdType.NewGuid);
                    d_SvUserWomen.SvUserId = d_SvUser.Id;

                    if (Lumos.CommonUtil.IsDateTime(deliveryTime))
                    {
                        d_SvUserWomen.DeliveryTime = DateTime.Parse(deliveryTime);
                    }

                    if (geyweek != null && geyweek.Length == 2)
                    {
                        if (Lumos.CommonUtil.IsInt(geyweek[0]) && Lumos.CommonUtil.IsInt(geyweek[1]))
                        {
                            d_SvUserWomen.PregnancyTime = Lumos.CommonUtil.GetWeekDay2Time(int.Parse(geyweek[0]), int.Parse(geyweek[1]));
                        }
                    }

                    if (gmPeriod != null && gmPeriod.Length == 3)
                    {
                        if (Lumos.CommonUtil.IsDateTime(gmPeriod[0]))
                        {
                            d_SvUserWomen.YjLastDay = DateTime.Parse(gmPeriod[0]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[1]))
                        {
                            d_SvUserWomen.YjDuration = int.Parse(gmPeriod[1]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[2]))
                        {
                            d_SvUserWomen.YjCycle = int.Parse(gmPeriod[2]);
                        }
                    }

                    CurrentDb.SvUserWomen.Add(d_SvUserWomen);
                    CurrentDb.SaveChanges();
                }
                else
                {
                    if (Lumos.CommonUtil.IsDateTime(deliveryTime))
                    {
                        d_SvUserWomen.DeliveryTime = DateTime.Parse(deliveryTime);
                    }

                    if (geyweek != null && geyweek.Length == 2)
                    {
                        if (Lumos.CommonUtil.IsInt(geyweek[0]) && Lumos.CommonUtil.IsInt(geyweek[1]))
                        {
                            d_SvUserWomen.PregnancyTime = Lumos.CommonUtil.GetWeekDay2Time(int.Parse(geyweek[0]), int.Parse(geyweek[1]));
                        }
                    }

                    if (gmPeriod != null && gmPeriod.Length == 3)
                    {
                        if (Lumos.CommonUtil.IsDateTime(gmPeriod[0]))
                        {
                            d_SvUserWomen.YjLastDay = DateTime.Parse(gmPeriod[0]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[1]))
                        {
                            d_SvUserWomen.YjDuration = int.Parse(gmPeriod[1]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[2]))
                        {
                            d_SvUserWomen.YjCycle = int.Parse(gmPeriod[2]);
                        }
                    }

                    CurrentDb.SaveChanges();
                }


                if (gmPeriod != null && gmPeriod.Length == 3)
                {
                    SdkFactory.Senviv.SetGmPeriod(config_Senviv, d_SvUser.Id, careMode, d_SvUserWomen.YjLastDay, d_SvUserWomen.YjDuration, d_SvUserWomen.YjCycle);
                }
            }

            #endregion

            var result_BindBox = SdkFactory.Senviv.BindDevice(config_Senviv, d_SvUser.Id, d_SvUserDevice.DeviceId);

            if (result_BindBox.Result != ResultType.Success)
            {
                return result_BindBox;
            }



            d_SvUserDevice.InfoFillTime = DateTime.Now;
            d_SvUserDevice.BindTime = DateTime.Now;
            d_SvUserDevice.BindStatus = Entity.E_SvUserDeviceBindStatus.Binded;
            d_SvUserDevice.Mender = operater;
            d_SvUserDevice.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();

            BizFactory.Senviv.SendDeviceBind(d_SvUser.Id, "您已成功绑定设备", "已绑定", DateTime.Now.ToUnifiedFormatDateTime(), "您好，您已成功绑定。");

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

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

            var d_Device_HasBind = CurrentDb.SvUserDevice.Where(m => m.DeviceId == rop.DeviceId && m.BindStatus == E_SvUserDeviceBindStatus.Binded).FirstOrDefault();

            if (d_Device_HasBind != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定");
            }

            var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_SvUserDevice == null)
            {
                d_SvUserDevice = new SvUserDevice();
                d_SvUserDevice.Id = IdWorker.Build(IdType.NewGuid);
                d_SvUserDevice.UserId = userId;
                d_SvUserDevice.DeviceId = rop.DeviceId;
                d_SvUserDevice.SvDeptId = d_Device.SvDeptId;
                d_SvUserDevice.BindDeviceIdTime = DateTime.Now;
                d_SvUserDevice.BindStatus = E_SvUserDeviceBindStatus.NotBind;
                d_SvUserDevice.Creator = operater;
                d_SvUserDevice.CreateTime = DateTime.Now;
                CurrentDb.SvUserDevice.Add(d_SvUserDevice);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_SvUserDevice.BindStatus = E_SvUserDeviceBindStatus.NotBind;
                d_SvUserDevice.BindDeviceIdTime = DateTime.Now;
                d_SvUserDevice.Mender = operater;
                d_SvUserDevice.MendTime = DateTime.Now;
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

            var d_Device_HasBind = CurrentDb.SvUserDevice.Where(m => m.DeviceId == rop.DeviceId && m.BindStatus == E_SvUserDeviceBindStatus.Binded).FirstOrDefault();

            if (d_Device_HasBind != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定");
            }

            var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();

            var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();
            if (d_SvUserDevice == null)
            {
                d_SvUserDevice = new SvUserDevice();
                d_SvUserDevice.Id = IdWorker.Build(IdType.NewGuid);
                d_SvUserDevice.UserId = userId;
                d_SvUserDevice.DeviceId = rop.DeviceId;
                d_SvUserDevice.SvDeptId = d_Device.SvDeptId;
                d_SvUserDevice.BindDeviceIdTime = DateTime.Now;
                d_SvUserDevice.BindPhoneTime = DateTime.Now;
                d_SvUserDevice.Creator = operater;
                d_SvUserDevice.CreateTime = DateTime.Now;
                CurrentDb.SvUserDevice.Add(d_SvUserDevice);
                CurrentDb.SaveChanges();
            }
            else
            {


                if (d_SvUserDevice.BindDeviceIdTime == null)
                    d_SvUserDevice.BindDeviceIdTime = DateTime.Now;
                d_SvUserDevice.BindPhoneTime = DateTime.Now;
                d_SvUserDevice.Mender = operater;
                d_SvUserDevice.MendTime = DateTime.Now;
            }

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            d_ClientUser.PhoneNumber = phoneToken.PhoneNumber;
            d_ClientUser.Mender = operater;
            d_ClientUser.MendTime = DateTime.Now;

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
        }

        public CustomJsonResult UnBind(string operater, string userId, RopDeviceUnBind rop)
        {

            var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.DeviceId == rop.DeviceId).FirstOrDefault();

            var config_Senviv = BizFactory.Senviv.GetConfig(d_SvUserDevice.SvDeptId);

            var r_Api_BindBox = SdkFactory.Senviv.UnBindDevice(config_Senviv, d_SvUserDevice.SvUserId, rop.DeviceId);

            if (r_Api_BindBox.Result != ResultType.Success)
                return r_Api_BindBox;

            if (d_SvUserDevice != null)
            {
                d_SvUserDevice.BindDeviceIdTime = null;
                d_SvUserDevice.BindPhoneTime = null;
                d_SvUserDevice.InfoFillTime = null;
                d_SvUserDevice.UnBindTime = DateTime.Now;
                d_SvUserDevice.BindStatus = E_SvUserDeviceBindStatus.UnBind;
                d_SvUserDevice.Creator = operater;
                d_SvUserDevice.CreateTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            BizFactory.Senviv.SendDeviceUnBind(d_SvUserDevice.SvUserId, "您已成功解绑设备，不再接收报告信息", rop.DeviceId, DateTime.Now.ToUnifiedFormatDateTime(), "感谢使用。");


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

        public CustomJsonResult SaveSvUserInfo(string svUserId, string svDeptId, string fullName,
            string avatar, string sex, string birthday, string height, string weight, string perplex, string medicalhis, string medicine)
        {

            var post = new
            {
                userid = svUserId,
                code = "",
                mobile = "13800138000",
                wechatid = "",
                nick = fullName,
                headimgurl = avatar,
                sex = sex,
                birthday = birthday,
                height = height,
                weight = weight,
                createtime = "2020-06-22T10:23:58.784Z", //创建时间
                updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
                SAS = "4",
                Perplex = "4,5,6", //目前困扰 （查看字典表）
                OtherPerplex = "", //目前困扰输入其它 ,
                Medicalhistory = medicalhis, //既往史 （查看字典表）
                OtherFamilyhistory = "", //既往史其它 ,
                Medicine = medicine, //用药情况 （查看字典表）
                OtherMedicine = "", //用药情况其它 ,
                deptid = svDeptId
            };

            var config_Senviv = BizFactory.Senviv.GetConfig(svDeptId);

            var r_Api_UserCreate = SdkFactory.Senviv.UserCreate(config_Senviv, post);
            if (r_Api_UserCreate == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存信息失败");

            if (string.IsNullOrEmpty(r_Api_UserCreate.userid))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存信息失败");


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存信息成功");
        }

        public void SaveSvUserWomenInfo(string svUserId, string svDeptId, E_SvUserCareMode careMode, string field, object value)
        {
            var d_SvUserWomen = CurrentDb.SvUserWomen.Where(m => m.SvUserId == svUserId).FirstOrDefault();

            if (d_SvUserWomen == null)
            {
                d_SvUserWomen = new SvUserWomen();
                d_SvUserWomen.Id = IdWorker.Build(IdType.NewGuid);
                d_SvUserWomen.SvUserId = svUserId;

                if (field == "geyweek")
                {
                    string[] geyweek = GetAnswerValueArrObj(value);
                    if (geyweek != null && geyweek.Length == 2)
                    {
                        if (Lumos.CommonUtil.IsInt(geyweek[0]) && Lumos.CommonUtil.IsInt(geyweek[1]))
                        {
                            d_SvUserWomen.PregnancyTime = Lumos.CommonUtil.GetWeekDay2Time(int.Parse(geyweek[0]), int.Parse(geyweek[1]));
                        }
                    }
                }
                else if (field == "deliveryTime")
                {
                    string deliveryTime = value.ToString();
                    if (Lumos.CommonUtil.IsDateTime(deliveryTime))
                    {
                        d_SvUserWomen.DeliveryTime = DateTime.Parse(deliveryTime);
                    }

                }
                else if (field == "gmPeriod")
                {
                    string[] gmPeriod = GetAnswerValueArrObj(value);

                    if (gmPeriod != null && gmPeriod.Length == 3)
                    {
                        if (Lumos.CommonUtil.IsDateTime(gmPeriod[0]))
                        {
                            d_SvUserWomen.YjLastDay = DateTime.Parse(gmPeriod[0]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[1]))
                        {
                            d_SvUserWomen.YjDuration = int.Parse(gmPeriod[1]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[2]))
                        {
                            d_SvUserWomen.YjCycle = int.Parse(gmPeriod[2]);
                        }
                    }

                }

                CurrentDb.SvUserWomen.Add(d_SvUserWomen);
            }
            else
            {
                if (field == "geyweek")
                {
                    string[] geyweek = GetAnswerValueArrObj(value);
                    if (geyweek != null && geyweek.Length == 2)
                    {
                        if (Lumos.CommonUtil.IsInt(geyweek[0]) && Lumos.CommonUtil.IsInt(geyweek[1]))
                        {
                            d_SvUserWomen.PregnancyTime = Lumos.CommonUtil.GetWeekDay2Time(int.Parse(geyweek[0]), int.Parse(geyweek[1]));
                        }
                    }
                }
                else if (field == "deliveryTime")
                {
                    string deliveryTime = value.ToString();
                    if (Lumos.CommonUtil.IsDateTime(deliveryTime))
                    {
                        d_SvUserWomen.DeliveryTime = DateTime.Parse(deliveryTime);
                    }

                }
                else if (field == "gmPeriod")
                {
                    string[] gmPeriod = GetAnswerValueArrObj(value);

                    if (gmPeriod != null && gmPeriod.Length == 3)
                    {
                        if (Lumos.CommonUtil.IsDateTime(gmPeriod[0]))
                        {
                            d_SvUserWomen.YjLastDay = DateTime.Parse(gmPeriod[0]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[1]))
                        {
                            d_SvUserWomen.YjDuration = int.Parse(gmPeriod[1]);
                        }

                        if (Lumos.CommonUtil.IsInt(gmPeriod[2]))
                        {
                            d_SvUserWomen.YjCycle = int.Parse(gmPeriod[2]);
                        }
                    }

                }
            }

            var config_Senviv = BizFactory.Senviv.GetConfig(svDeptId);

            SdkFactory.Senviv.SetGmPeriod(config_Senviv, svUserId, careMode, d_SvUserWomen.YjLastDay, d_SvUserWomen.YjDuration, d_SvUserWomen.YjCycle);

            CurrentDb.SaveChanges();
        }
    }

}
