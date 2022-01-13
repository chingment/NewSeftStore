using LocalS.BLL.Biz;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Task
{
    public class Task4Tim2SenvivUsersProvider : BaseService, IJob
    {
        public readonly string TAG = "Task4Tim2SenvivUsersProvider";
        public DateTime Convert2DateTime(string str)
        {
            try
            {
                var dt1 = DateTime.Parse("1970-01-01T00:00:00+08:00");

                var dt = DateTime.Parse(str);

                if (dt < dt1)
                    return dt1;

                return dt;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string[] deptIds = new string[] { "32", "64" };

                foreach (var deptId in deptIds)
                {
                    var config_Senviv = BizFactory.Senviv.GetConfig(deptId);

                    var senvivUsers = SdkFactory.Senviv.GetUserList(config_Senviv);

                    LogUtil.Info(TAG, "SenvivUsers.Count:" + senvivUsers.Count);
                    if (deptId == "32")
                    {
                        #region "32"
                        foreach (var senvivUser in senvivUsers)
                        {
                            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == senvivUser.userid).FirstOrDefault();

                            if (d_SenvivUser == null)
                            {
                                d_SenvivUser = new Entity.SenvivUser();
                                d_SenvivUser.Id = senvivUser.userid;
                                d_SenvivUser.DeptId = senvivUser.deptid;
                                d_SenvivUser.WxOpenId = senvivUser.wechatid;
                                d_SenvivUser.PhoneNumber = senvivUser.mobile;
                                d_SenvivUser.NickName = senvivUser.nick;
                                d_SenvivUser.FullName = senvivUser.account;
                                d_SenvivUser.Avatar = senvivUser.headimgurl;
                                d_SenvivUser.Sex = senvivUser.sex;
                                d_SenvivUser.Birthday = Convert2DateTime(senvivUser.birthday);
                                d_SenvivUser.Height = senvivUser.height;
                                d_SenvivUser.Weight = senvivUser.weight;
                                d_SenvivUser.TargetValue = senvivUser.TargetValue;
                                d_SenvivUser.Sas = senvivUser.SAS;
                                d_SenvivUser.IsUseBreathMach = senvivUser.BreathingMachine == "1" ? true : false;
                                d_SenvivUser.Perplex = senvivUser.Perplex;
                                d_SenvivUser.PerplexOt = senvivUser.OtherPerplex;
                                d_SenvivUser.MedicalHis = senvivUser.Medicalhistory;
                                d_SenvivUser.MedicalHisOt = senvivUser.OtherFamilyhistory;
                                d_SenvivUser.Medicine = senvivUser.Medicine;
                                d_SenvivUser.MedicineOt = senvivUser.OtherMedicine;
                                d_SenvivUser.Creator = IdWorker.Build(IdType.EmptyGuid);
                                d_SenvivUser.CreateTime = Convert2DateTime(senvivUser.createtime);
                                CurrentDb.SenvivUser.Add(d_SenvivUser);
                                CurrentDb.SaveChanges();
                            }
                            else
                            {
                                d_SenvivUser.DeptId = senvivUser.deptid;
                                d_SenvivUser.WxOpenId = senvivUser.wechatid;
                                d_SenvivUser.PhoneNumber = senvivUser.mobile;
                                d_SenvivUser.NickName = senvivUser.nick;
                                d_SenvivUser.FullName = senvivUser.account;
                                d_SenvivUser.Avatar = senvivUser.headimgurl;
                                d_SenvivUser.Sex = senvivUser.sex;
                                d_SenvivUser.Birthday = Convert2DateTime(senvivUser.birthday);
                                d_SenvivUser.Height = senvivUser.height;
                                d_SenvivUser.Weight = senvivUser.weight;
                                d_SenvivUser.TargetValue = senvivUser.TargetValue;
                                d_SenvivUser.Sas = senvivUser.SAS;
                                d_SenvivUser.IsUseBreathMach = senvivUser.BreathingMachine == "1" ? true : false;
                                d_SenvivUser.Perplex = senvivUser.Perplex;
                                d_SenvivUser.PerplexOt = senvivUser.OtherPerplex;
                                d_SenvivUser.MedicalHis = senvivUser.Medicalhistory;
                                d_SenvivUser.MedicalHisOt = senvivUser.OtherFamilyhistory;
                                d_SenvivUser.Medicine = senvivUser.Medicine;
                                d_SenvivUser.MedicineOt = senvivUser.OtherMedicine;
                                d_SenvivUser.MendTime = DateTime.Now;
                                d_SenvivUser.Mender = IdWorker.Build(IdType.EmptyGuid);
                                CurrentDb.SaveChanges();
                            }

                            BizFactory.Senviv.BuildDayReport(d_SenvivUser.Id, d_SenvivUser.DeptId);
                        }

                        #endregion
                    }
                    else if (deptId == "46")
                    {

                    }

                    var senvivBoxs = SdkFactory.Senviv.GetBoxList(config_Senviv);

                    LogUtil.Info(TAG, "senvivBoxs.Length:" + senvivBoxs.Count);

                    foreach (var senvivBox in senvivBoxs)
                    {
                        var d_Device = CurrentDb.Device.Where(m => m.Id == senvivBox.sn).FirstOrDefault();
                        if (d_Device == null)
                        {
                            LogUtil.Info(TAG, senvivBox.sn + ",不存在");
                            d_Device = new Entity.Device();
                            d_Device.Id = senvivBox.sn;
                            d_Device.Name = "非接触式生命体征检测仪";
                            d_Device.Type = "senvivlite";
                            d_Device.ImeiId = senvivBox.imei;
                            d_Device.CurUseMerchId = "88273829";
                            d_Device.Model = senvivBox.model;
                            //d_Device.Lat = float.Parse(senvivBox.latitude);
                            //d_Device.Lng = float.Parse(senvivBox.longitude);
                            d_Device.AppVersionName = senvivBox.version;
                            d_Device.FingerVeinnerIsUse = false;
                            d_Device.ImIsUse = false;
                            d_Device.Creator = IdWorker.Build(IdType.EmptyGuid);
                            d_Device.CreateTime = DateTime.Now;
                            CurrentDb.Device.Add(d_Device);
                            CurrentDb.SaveChanges();

                            var d_MerchDevice = new Entity.MerchDevice();
                            d_MerchDevice.Id = IdWorker.Build(IdType.NewGuid);
                            d_MerchDevice.MerchId = d_Device.CurUseMerchId;
                            d_MerchDevice.DeviceId = d_Device.Id;
                            d_MerchDevice.Creator = d_Device.Creator;
                            d_MerchDevice.CreateTime = d_Device.CreateTime;
                            CurrentDb.MerchDevice.Add(d_MerchDevice);
                            CurrentDb.SaveChanges();
                        }
                        else
                        {
                            LogUtil.Info(TAG, senvivBox.sn + ",已存在");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }
    }
}
