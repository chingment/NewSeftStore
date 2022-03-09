﻿using LocalS.BLL.Biz;
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

                        var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == i_SenvivUser.userid).FirstOrDefault();
                        string svUserId = i_SenvivUser.userid;
                        string useId = null;

                        if (d_SvUser != null)
                        {
                            useId = d_SvUser.UserId;

                        }



                        if (i_SenvivUser.products == null)
                        {
                            if (d_SvUser != null)
                            {
                                d_SvUser.DeviceCount = 0;
                                CurrentDb.SaveChanges();
                            }
                        }
                        else
                        {
                            if (d_SvUser != null)
                            {
                                d_SvUser.DeviceCount = i_SenvivUser.products.Count;
                                CurrentDb.SaveChanges();
                            }

                            var products = i_SenvivUser.products;
                            foreach (var device in products)
                            {

                                if (string.IsNullOrEmpty(useId))
                                {
                                    var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == useId && m.DeviceId == device.sn).FirstOrDefault();
                                    if (d_SvUserDevice == null)
                                    {
                                        d_SvUserDevice = new Entity.SvUserDevice();
                                        d_SvUserDevice.Id = IdWorker.Build(IdType.NewGuid);
                                        d_SvUserDevice.SvUserId = svUserId;
                                        d_SvUserDevice.UserId = useId;
                                        d_SvUserDevice.DeviceId = device.sn;
                                        d_SvUserDevice.SvDeptId = svDeptId;
                                        d_SvUserDevice.WebUrl = device.webUrl;
                                        d_SvUserDevice.TcpAddress = device.tcpAddress;
                                        d_SvUserDevice.BindStatus = Entity.E_SvUserDeviceBindStatus.Unknow;
                                        d_SvUserDevice.CreateTime = DateTime.Now;
                                        d_SvUserDevice.Creator = IdWorker.Build(IdType.EmptyGuid);
                                        CurrentDb.SvUserDevice.Add(d_SvUserDevice);
                                        CurrentDb.SaveChanges();
                                    }
                                    else
                                    {
                                        d_SvUserDevice.WebUrl = device.webUrl;
                                        d_SvUserDevice.TcpAddress = device.tcpAddress;
                                        d_SvUserDevice.MendTime = DateTime.Now;
                                        d_SvUserDevice.Mender = IdWorker.Build(IdType.EmptyGuid);
                                        CurrentDb.SaveChanges();
                                    }
                                }
                                else
                                {
                                    var d_SvUserDevice = CurrentDb.SvUserDevice.Where(m => m.UserId == useId && m.DeviceId == device.sn).FirstOrDefault();
                                    if (d_SvUserDevice != null)
                                    {
                                        d_SvUserDevice.WebUrl = device.webUrl;
                                        d_SvUserDevice.TcpAddress = device.tcpAddress;
                                        d_SvUserDevice.MendTime = DateTime.Now;
                                        d_SvUserDevice.Mender = IdWorker.Build(IdType.EmptyGuid);
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
