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

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string[] svDeptIds = new string[] { "32", "46" };

                foreach (var svDeptId in svDeptIds)
                {
                    var config_Senviv = BizFactory.Senviv.GetConfig(svDeptId);

                    var i_Devices = SdkFactory.Senviv.GetDevices(config_Senviv);

                    foreach (var i_Device in i_Devices)
                    {
                        var d_Device = CurrentDb.Device.Where(m => m.Id == i_Device.sn).FirstOrDefault();
                        if (d_Device == null)
                        {
                            d_Device = new Entity.Device();
                            d_Device.Id = i_Device.sn;
                            d_Device.Name = "非接触式生命体征检测仪";
                            d_Device.Type = "senvivlite";
                            d_Device.ImeiId = i_Device.imei;
                            d_Device.CurUseMerchId = "88273829";
                            d_Device.Model = i_Device.model;
                            d_Device.AppVersionName = i_Device.version;
                            d_Device.Creator = IdWorker.Build(IdType.EmptyGuid);
                            d_Device.CreateTime = DateTime.Now;
                            d_Device.SvDeptId = svDeptId;
                            CurrentDb.Device.Add(d_Device);
                            CurrentDb.SaveChanges();

                            var d_MerchDevice = new Entity.MerchDevice();
                            d_MerchDevice.Id = IdWorker.Build(IdType.NewGuid);
                            d_MerchDevice.MerchId = d_Device.CurUseMerchId;
                            d_MerchDevice.DeviceId = d_Device.Id;
                            d_MerchDevice.IsStopUse = false;
                            d_MerchDevice.Creator = d_Device.Creator;
                            d_MerchDevice.CreateTime = d_Device.CreateTime;
                            CurrentDb.MerchDevice.Add(d_MerchDevice);
                            CurrentDb.SaveChanges();
                        }
                    }


                    var i_SvUsers = SdkFactory.Senviv.GetUsers(config_Senviv);

                    foreach (var i_SvUser in i_SvUsers)
                    {
                        var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == i_SvUser.userid).FirstOrDefault();
                        if (d_SvUser != null)
                        {
                            if (i_SvUser.products == null || i_SvUser.products.Count == 0)
                            {
                                d_SvUser.DeviceCount = 0;
                            }
                            else
                            {
                                d_SvUser.DeviceCount = i_SvUser.products.Count;
                            }

                            CurrentDb.SaveChanges();
                        }
                    }
                }

                var d_SvUserDevices = CurrentDb.SvUserDevice.Where(m => m.BindStatus == Entity.E_SvUserDeviceBindStatus.Binded).ToList();
                foreach (var d_SvUserDevice in d_SvUserDevices)
                {
                    BizFactory.Senviv.BuildDayReport(d_SvUserDevice.SvUserId, d_SvUserDevice.DeviceId, d_SvUserDevice.SvDeptId, d_SvUserDevice.IsStopSend);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }
    }
}
