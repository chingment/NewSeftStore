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

                    var i_SenvivUsers = SdkFactory.Senviv.GetUserList(config_Senviv);

                    LogUtil.Info(TAG, string.Format("DeptId:{0},SenvivUsers.Count:{1}", deptId, i_SenvivUsers.Count));

                    if (deptId == "32")
                    {
                        #region "32"
                        foreach (var i_SenvivUser in i_SenvivUsers)
                        {
                            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == i_SenvivUser.userid).FirstOrDefault();

                            if (d_SenvivUser == null)
                            {
                                SysClientUser d_ClientUser = new SysClientUser();
                                d_ClientUser.Id = IdWorker.Build(IdType.NewGuid);
                                d_ClientUser.UserName= IdWorker.Build(IdType.NewGuid);
                                d_ClientUser.PasswordHash = PassWordHelper.HashPassword("fedgsgseee.3ff");
                                d_ClientUser.Avatar = i_SenvivUser.headimgurl;
                                d_ClientUser.NickName = i_SenvivUser.nick;
                                d_ClientUser.PhoneNumber = i_SenvivUser.mobile;
                                d_ClientUser.Sex = i_SenvivUser.sex;
                                d_ClientUser.WxPaOpenId = i_SenvivUser.wechatid;
                                d_ClientUser.CreateTime = DateTime.Now;
                                d_ClientUser.Creator= IdWorker.Build(IdType.EmptyGuid);
                                CurrentDb.SysClientUser.Add(d_ClientUser);
                                CurrentDb.SaveChanges();

                                d_SenvivUser = new Entity.SenvivUser();
                                d_SenvivUser.Id = i_SenvivUser.userid;
                                d_SenvivUser.DeptId = i_SenvivUser.deptid;
                                d_SenvivUser.UserId = d_ClientUser.Id;
                                //d_SenvivUser.WxOpenId = senvivUser.wechatid;
                                d_SenvivUser.PhoneNumber = i_SenvivUser.mobile;
                                d_SenvivUser.NickName = i_SenvivUser.nick;
                                d_SenvivUser.FullName = i_SenvivUser.account;
                                d_SenvivUser.Avatar = i_SenvivUser.headimgurl;
                                d_SenvivUser.Sex = i_SenvivUser.sex;
                                d_SenvivUser.Birthday = Convert2DateTime(i_SenvivUser.birthday);
                                d_SenvivUser.Height = i_SenvivUser.height;
                                d_SenvivUser.Weight = i_SenvivUser.weight;
                                d_SenvivUser.TargetValue = i_SenvivUser.TargetValue;
                                d_SenvivUser.Sas = i_SenvivUser.SAS;
                                d_SenvivUser.IsUseBreathMach = i_SenvivUser.BreathingMachine == "1" ? true : false;
                                d_SenvivUser.Perplex = i_SenvivUser.Perplex;
                                d_SenvivUser.PerplexOt = i_SenvivUser.OtherPerplex;
                                d_SenvivUser.MedicalHis = i_SenvivUser.Medicalhistory;
                                d_SenvivUser.MedicalHisOt = i_SenvivUser.OtherFamilyhistory;
                                d_SenvivUser.Medicine = i_SenvivUser.Medicine;
                                d_SenvivUser.MedicineOt = i_SenvivUser.OtherMedicine;
                                d_SenvivUser.Creator = IdWorker.Build(IdType.EmptyGuid);
                                d_SenvivUser.CreateTime = Convert2DateTime(i_SenvivUser.createtime);
                                CurrentDb.SenvivUser.Add(d_SenvivUser);
                                CurrentDb.SaveChanges();
                            }
                            else
                            {
                                d_SenvivUser.DeptId = i_SenvivUser.deptid;
                                d_SenvivUser.PhoneNumber = i_SenvivUser.mobile;
                                d_SenvivUser.NickName = i_SenvivUser.nick;
                                d_SenvivUser.FullName = i_SenvivUser.account;
                                d_SenvivUser.Avatar = i_SenvivUser.headimgurl;
                                d_SenvivUser.Sex = i_SenvivUser.sex;
                                d_SenvivUser.Birthday = Convert2DateTime(i_SenvivUser.birthday);
                                d_SenvivUser.Height = i_SenvivUser.height;
                                d_SenvivUser.Weight = i_SenvivUser.weight;
                                d_SenvivUser.TargetValue = i_SenvivUser.TargetValue;
                                d_SenvivUser.Sas = i_SenvivUser.SAS;
                                d_SenvivUser.IsUseBreathMach = i_SenvivUser.BreathingMachine == "1" ? true : false;
                                d_SenvivUser.Perplex = i_SenvivUser.Perplex;
                                d_SenvivUser.PerplexOt = i_SenvivUser.OtherPerplex;
                                d_SenvivUser.MedicalHis = i_SenvivUser.Medicalhistory;
                                d_SenvivUser.MedicalHisOt = i_SenvivUser.OtherFamilyhistory;
                                d_SenvivUser.Medicine = i_SenvivUser.Medicine;
                                d_SenvivUser.MedicineOt = i_SenvivUser.OtherMedicine;
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

                    var i_SenvivBoxs = SdkFactory.Senviv.GetBoxList(config_Senviv);

                    LogUtil.Info(TAG, string.Format("DeptId:{0},SenvivBoxs.Count:{1}", deptId, i_SenvivBoxs.Count));

 
                    foreach (var i_SenvivBox in i_SenvivBoxs)
                    {
                        var d_Device = CurrentDb.Device.Where(m => m.Id == i_SenvivBox.sn).FirstOrDefault();
                        if (d_Device == null)
                        {
                            d_Device = new Entity.Device();
                            d_Device.Id = i_SenvivBox.sn;
                            d_Device.Name = "非接触式生命体征检测仪";
                            d_Device.Type = "senvivlite";
                            d_Device.ImeiId = i_SenvivBox.imei;
                            d_Device.CurUseMerchId = "88273829";
                            d_Device.Model = i_SenvivBox.model;
                            d_Device.AppVersionName = i_SenvivBox.version;
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
