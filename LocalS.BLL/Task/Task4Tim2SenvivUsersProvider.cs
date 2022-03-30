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

                    var i_SenvivBoxs = SdkFactory.Senviv.GetBoxList(config_Senviv);

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

                }

                var d_SvUsers = CurrentDb.SvUser.ToList();

                foreach (var d_SvUser in d_SvUsers)
                {
                    BizFactory.Senviv.BuildDayReport(d_SvUser.Id, "", d_SvUser.SvDeptId);
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }
    }
}
