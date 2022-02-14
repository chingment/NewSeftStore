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
                string[] svDeptIds = new string[] { "32", "46" };

                foreach (var svDeptId in svDeptIds)
                {
                    var config_Senviv = BizFactory.Senviv.GetConfig(svDeptId);


                    #region 获取所有设备
                    //var i_SenvivBoxs = SdkFactory.Senviv.GetBoxList(config_Senviv);

                    //LogUtil.Info(TAG, string.Format("DeptId:{0},SenvivBoxs.Count:{1}", svDeptId, i_SenvivBoxs.Count));

                    //foreach (var i_SenvivBox in i_SenvivBoxs)
                    //{
                    //    var d_Device = CurrentDb.Device.Where(m => m.Id == i_SenvivBox.sn).FirstOrDefault();
                    //    if (d_Device == null)
                    //    {
                    //        d_Device = new Entity.Device();
                    //        d_Device.Id = i_SenvivBox.sn;
                    //        d_Device.Name = "非接触式生命体征检测仪";
                    //        d_Device.Type = "senvivlite";
                    //        d_Device.ImeiId = i_SenvivBox.imei;
                    //        d_Device.CurUseMerchId = "88273829";
                    //        d_Device.Model = i_SenvivBox.model;
                    //        d_Device.AppVersionName = i_SenvivBox.version;
                    //        d_Device.Creator = IdWorker.Build(IdType.EmptyGuid);
                    //        d_Device.CreateTime = DateTime.Now;
                    //        d_Device.SvDeptId = svDeptId;
                    //        CurrentDb.Device.Add(d_Device);
                    //        CurrentDb.SaveChanges();

                    //        var d_MerchDevice = new Entity.MerchDevice();
                    //        d_MerchDevice.Id = IdWorker.Build(IdType.NewGuid);
                    //        d_MerchDevice.MerchId = d_Device.CurUseMerchId;
                    //        d_MerchDevice.DeviceId = d_Device.Id;
                    //        d_MerchDevice.IsStopUse = false;
                    //        d_MerchDevice.Creator = d_Device.Creator;
                    //        d_MerchDevice.CreateTime = d_Device.CreateTime;
                    //        CurrentDb.MerchDevice.Add(d_MerchDevice);
                    //        CurrentDb.SaveChanges();
                    //    }
                    //}

                    #endregion


                    #region 获取所有用户 
                    var i_SenvivUsers = SdkFactory.Senviv.GetUserList(config_Senviv);

                    LogUtil.Info(TAG, string.Format("DeptId:{0},SenvivUsers.Count:{1}", svDeptId, i_SenvivUsers.Count));

                    foreach (var i_SenvivUser in i_SenvivUsers)
                    {

                        var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == i_SenvivUser.userid).FirstOrDefault();
                        string svUserId = i_SenvivUser.userid;
                        string useId = null;

                        if (d_SenvivUser != null)
                        {
                            useId = d_SenvivUser.UserId;
                        }

                        //if (d_SenvivUser == null)
                        //{
                        //    var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.WxPaOpenId == i_SenvivUser.wechatid).FirstOrDefault();
                        //    if (d_ClientUser == null)
                        //    {
                        //        d_ClientUser = new SysClientUser();
                        //        d_ClientUser.Id = IdWorker.Build(IdType.NewGuid);
                        //        d_ClientUser.UserName = IdWorker.Build(IdType.NewGuid);
                        //        d_ClientUser.PasswordHash = PassWordHelper.HashPassword("sfsfsffds.3pg");
                        //        d_ClientUser.SecurityStamp = IdWorker.Build(IdType.NewGuid);
                        //        d_ClientUser.NickName = i_SenvivUser.nick;
                        //        d_ClientUser.Avatar = i_SenvivUser.headimgurl;
                        //        //d_ClientUser.MerchId = "88273829";
                        //        d_ClientUser.WxPaOpenId = i_SenvivUser.wechatid;
                        //        d_ClientUser.BelongType = Enumeration.BelongType.Client;
                        //        d_ClientUser.RegisterTime = DateTime.Now;
                        //        d_ClientUser.CreateTime = DateTime.Now;
                        //        d_ClientUser.Creator = d_ClientUser.Id;
                        //        CurrentDb.SysClientUser.Add(d_ClientUser);
                        //        CurrentDb.SaveChanges();
                        //    }

                        //    useId = d_ClientUser.Id;

                        //    d_SenvivUser = new Entity.SenvivUser();
                        //    d_SenvivUser.Id = i_SenvivUser.userid;
                        //    d_SenvivUser.MerchId = d_ClientUser.MerchId;
                        //    d_SenvivUser.SvDeptId = i_SenvivUser.deptid;
                        //    d_SenvivUser.UserId = d_ClientUser.Id;
                        //    d_SenvivUser.PhoneNumber = i_SenvivUser.mobile;
                        //    d_SenvivUser.NickName = i_SenvivUser.nick;
                        //    d_SenvivUser.FullName = i_SenvivUser.account;
                        //    d_SenvivUser.Avatar = i_SenvivUser.headimgurl;
                        //    d_SenvivUser.Sex = i_SenvivUser.sex;
                        //    d_SenvivUser.Birthday = Convert2DateTime(i_SenvivUser.birthday);
                        //    d_SenvivUser.Height = i_SenvivUser.height;
                        //    d_SenvivUser.Weight = i_SenvivUser.weight;
                        //    d_SenvivUser.SmTargetVal = i_SenvivUser.TargetValue;
                        //    d_SenvivUser.Sas = i_SenvivUser.SAS;
                        //    d_SenvivUser.IsUseBreathMach = i_SenvivUser.BreathingMachine == "1" ? true : false;
                        //    d_SenvivUser.Perplex = i_SenvivUser.Perplex;
                        //    d_SenvivUser.PerplexOt = i_SenvivUser.OtherPerplex;
                        //    d_SenvivUser.MedicalHis = i_SenvivUser.Medicalhistory;
                        //    d_SenvivUser.MedicalHisOt = i_SenvivUser.OtherFamilyhistory;
                        //    d_SenvivUser.Medicine = i_SenvivUser.Medicine;
                        //    d_SenvivUser.MedicineOt = i_SenvivUser.OtherMedicine;
                        //    d_SenvivUser.Creator = IdWorker.Build(IdType.EmptyGuid);
                        //    d_SenvivUser.CreateTime = Convert2DateTime(i_SenvivUser.createtime);
                        //    CurrentDb.SenvivUser.Add(d_SenvivUser);
                        //    CurrentDb.SaveChanges();
                        //}
                        //else
                        //{
                        //    var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.WxPaOpenId == i_SenvivUser.wechatid).FirstOrDefault();
                        //    if (d_ClientUser == null)
                        //    {
                        //        d_ClientUser = new SysClientUser();
                        //        d_ClientUser.Id = IdWorker.Build(IdType.NewGuid);
                        //        d_ClientUser.UserName = IdWorker.Build(IdType.NewGuid);
                        //        d_ClientUser.PasswordHash = PassWordHelper.HashPassword("sfsfsffds.3pg");
                        //        d_ClientUser.SecurityStamp = IdWorker.Build(IdType.NewGuid);
                        //        d_ClientUser.NickName = i_SenvivUser.nick;
                        //        d_ClientUser.Avatar = i_SenvivUser.headimgurl;
                        //        //d_ClientUser.MerchId = "88273829";
                        //        d_ClientUser.WxPaOpenId = i_SenvivUser.wechatid;
                        //        d_ClientUser.BelongType = Enumeration.BelongType.Client;
                        //        d_ClientUser.RegisterTime = DateTime.Now;
                        //        d_ClientUser.CreateTime = DateTime.Now;
                        //        d_ClientUser.Creator = d_ClientUser.Id;
                        //        CurrentDb.SysClientUser.Add(d_ClientUser);
                        //        CurrentDb.SaveChanges();
                        //    }

                        //    useId = d_ClientUser.Id;

                        //    d_SenvivUser.SvDeptId = i_SenvivUser.deptid;
                        //    d_SenvivUser.UserId = d_ClientUser.Id;
                        //    d_SenvivUser.PhoneNumber = i_SenvivUser.mobile;
                        //    d_SenvivUser.NickName = i_SenvivUser.nick;
                        //    d_SenvivUser.FullName = i_SenvivUser.account;
                        //    d_SenvivUser.Avatar = i_SenvivUser.headimgurl;
                        //    d_SenvivUser.Sex = i_SenvivUser.sex;
                        //    d_SenvivUser.Birthday = Convert2DateTime(i_SenvivUser.birthday);
                        //    d_SenvivUser.Height = i_SenvivUser.height;
                        //    d_SenvivUser.Weight = i_SenvivUser.weight;
                        //    d_SenvivUser.SmTargetVal = i_SenvivUser.TargetValue;
                        //    d_SenvivUser.Sas = i_SenvivUser.SAS;
                        //    d_SenvivUser.IsUseBreathMach = i_SenvivUser.BreathingMachine == "1" ? true : false;
                        //    d_SenvivUser.Perplex = i_SenvivUser.Perplex;
                        //    d_SenvivUser.PerplexOt = i_SenvivUser.OtherPerplex;
                        //    d_SenvivUser.MedicalHis = i_SenvivUser.Medicalhistory;
                        //    d_SenvivUser.MedicalHisOt = i_SenvivUser.OtherFamilyhistory;
                        //    d_SenvivUser.Medicine = i_SenvivUser.Medicine;
                        //    d_SenvivUser.MedicineOt = i_SenvivUser.OtherMedicine;
                        //    d_SenvivUser.MendTime = DateTime.Now;
                        //    d_SenvivUser.Mender = IdWorker.Build(IdType.EmptyGuid);
                        //    CurrentDb.SaveChanges();
                        //}

                        if (i_SenvivUser.products != null)
                        {
                            var products = i_SenvivUser.products;
                            foreach (var device in products)
                            {
                                if (string.IsNullOrEmpty(useId))
                                {
                                    var d_SenvivUserDevice = CurrentDb.SenvivUserDevice.Where(m => m.UserId == useId && m.DeviceId == device.sn).FirstOrDefault();
                                    if (d_SenvivUserDevice == null)
                                    {
                                        d_SenvivUserDevice = new Entity.SenvivUserDevice();
                                        d_SenvivUserDevice.Id = IdWorker.Build(IdType.NewGuid);
                                        d_SenvivUserDevice.SvUserId = svUserId;
                                        d_SenvivUserDevice.UserId = useId;
                                        d_SenvivUserDevice.DeviceId = device.sn;
                                        d_SenvivUserDevice.SvDeptId = svDeptId;
                                        d_SenvivUserDevice.BindStatus = Entity.SenvivUserDeviceBindStatus.Unknow;
                                        d_SenvivUserDevice.CreateTime = DateTime.Now;
                                        d_SenvivUserDevice.Creator = IdWorker.Build(IdType.EmptyGuid);
                                        CurrentDb.SenvivUserDevice.Add(d_SenvivUserDevice);
                                        CurrentDb.SaveChanges();
                                    }
                                }

                                if (svDeptId == "32")
                                {
                                    BizFactory.Senviv.BuildDayReport32(svUserId, device.sn, svDeptId);
                                }
                                else if (svDeptId == "46")
                                {
                                    BizFactory.Senviv.BuildDayReport46(svUserId, device.sn, svDeptId);
                                }
                            }
                        }


                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }
    }
}
