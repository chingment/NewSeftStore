using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.BLL.UI;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class DevSenvivLiteService : DeviceService
    {
        public CustomJsonResult InitGetList(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var d_CMerchs = CurrentDb.Merch.Where(m => m.PId == merchId).ToList();

            var optionsByMerch = new List<object>();

            foreach (var d_CMerch in d_CMerchs)
            {
                optionsByMerch.Add(new { Id = d_CMerch.Id, Name = d_CMerch.Name });
            }

            var ret = new { optionsByMerch = optionsByMerch };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupDeviceGetList rup)
        {
            var result = new CustomJsonResult();

            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var query = (from u in CurrentDb.MerchDevice
                         join m in CurrentDb.Device on u.DeviceId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         join q in CurrentDb.Merch on u.MerchId equals q.Id into temp2
                         from tt1 in temp2.DefaultIfEmpty()
                         where ((rup.Id == null || u.DeviceId.Contains(rup.Id)) || (rup.Id == null || u.CumCode.Contains(rup.Id)))
                         &&
                         merchIds.Contains(u.MerchId)
                         &&
                         tt.Type == "senvivlite"
                         &&
                         u.IsStopUse == false
                         select new { u.Id, tt.Type, tt.CurUseMerchId, MerchName = tt1.Name, u.MerchId, tt.Model, u.DeviceId, u.CumCode, tt.MainImgUrl, tt.CurUseStoreId, tt.CurUseShopId, tt.RunStatus, tt.LastRequestTime, tt.AppVersionCode, tt.CtrlSdkVersionCode, tt.ExIsHas, tt.Name, u.IsStopUse, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.DeviceId).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var distributeStatus = new FieldModel();

                if (item.CurUseMerchId == merchId)
                {
                    distributeStatus = new FieldModel(1, "未分配");
                }
                else
                {
                    distributeStatus = new FieldModel(2, "已分配");
                }

                olist.Add(new
                {
                    Id = item.DeviceId,
                    Name = item.Name,
                    MerchId = item.MerchId,
                    MerchName = item.MerchName,
                    Model = item.Model,
                    Code = GetCode(item.DeviceId, item.CumCode),
                    MainImgUrl = item.MainImgUrl,
                    LastRequestTime = item.LastRequestTime.ToUnifiedFormatDateTime(),
                    DistributeStatus = distributeStatus
                });

            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult UnBindMerch(string operater, string merchId, RopDeviceUnBindMerch rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_Device = CurrentDb.Device.Where(m => m.CurUseMerchId == rop.MerchId && m.Id == rop.DeviceId).FirstOrDefault();

                if (d_Device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已解绑门店");
                }

                d_Device.CurUseMerchId = merchId;
                d_Device.CurUseStoreId = null;
                d_Device.CurUseShopId = null;
                d_Device.Mender = operater;
                d_Device.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();

                var d_MerchDevices = CurrentDb.MerchDevice.Where(m => m.DeviceId == rop.DeviceId).ToList();

                foreach (var d_MerchDevice in d_MerchDevices)
                {
                    if (d_MerchDevice.MerchId == merchId)
                    {
                        d_MerchDevice.IsStopUse = false;
                    }
                    else
                    {
                        d_MerchDevice.IsStopUse = true;
                    }

                    d_MerchDevice.Mender = operater;
                    d_MerchDevice.MendTime = DateTime.Now;
                }


                CurrentDb.SaveChanges();
                ts.Complete();


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "解绑成功");

            }
            return result;
        }

        public CustomJsonResult BindMerch(string operater, string merchId, RopDeviceBindMerch rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();

                if (d_Device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到设备");
                }
                string old_CurUseMerchId = d_Device.CurUseMerchId;

                d_Device.CurUseMerchId = rop.MerchId;
                d_Device.Mender = operater;
                d_Device.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();

                var d_CMerchDevice = CurrentDb.MerchDevice.Where(m => m.DeviceId == rop.DeviceId && m.MerchId == rop.MerchId).FirstOrDefault();

                if (d_CMerchDevice == null)
                {
                    d_CMerchDevice = new MerchDevice();
                    d_CMerchDevice.Id = IdWorker.Build(IdType.NewGuid);
                    d_CMerchDevice.MerchId = rop.MerchId;
                    d_CMerchDevice.DeviceId = rop.DeviceId;
                    d_CMerchDevice.LogoImgUrl = d_Device.LogoImgUrl;
                    d_CMerchDevice.IsStopUse = false;
                    d_CMerchDevice.Creator = operater;
                    d_CMerchDevice.CreateTime = DateTime.Now;
                    CurrentDb.MerchDevice.Add(d_CMerchDevice);
                    CurrentDb.SaveChanges();
                }

                var d_PMerchDevice = CurrentDb.MerchDevice.Where(m => m.DeviceId == rop.DeviceId && m.MerchId == old_CurUseMerchId).FirstOrDefault();
                if (d_PMerchDevice != null)
                {
                    d_PMerchDevice.IsStopUse = true;
                    d_PMerchDevice.Creator = operater;
                    d_PMerchDevice.CreateTime = DateTime.Now;
                }



                var box = SdkFactory.Senviv.GetBox(null, d_Device.Id);
                if (box != null)
                {
                    var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == box.userid).FirstOrDefault();
                    if (d_SenvivUser != null)
                    {
                        d_SenvivUser.MerchId = rop.MerchId;
                    }
                }

                CurrentDb.SaveChanges();

                ts.Complete();



                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");

            }

            return result;
        }
    }
}
