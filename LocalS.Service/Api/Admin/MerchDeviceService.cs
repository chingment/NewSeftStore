using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Admin
{
    public class MerchDeviceService : BaseService
    {
        public CustomJsonResult InitGetList(string operater)
        {
            var result = new CustomJsonResult();

            var merchs = CurrentDb.Merch.ToList();

            List<object> formSelectMerchs = new List<object>();
            foreach (var merch in merchs)
            {
                formSelectMerchs.Add(new { value = merch.Id, label = merch.Name });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { formSelectMerchs = formSelectMerchs });

            return result;
        }

        public CustomJsonResult GetList(string operater, RupMerchDeviceGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Device
                         where (rup.Id == null || u.Id == rup.Id)
                         select new { u.Id, u.Name, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderBy(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var l_Device = BizFactory.Device.GetOne(item.Id);
                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    MerchId = l_Device.MerchId,
                    MerchName = string.IsNullOrEmpty(l_Device.MerchId) == true ? "未绑定商户" : l_Device.MerchName,
                    CtrlSdkVersion = l_Device.CtrlSdkVersion,
                    AppVersion = l_Device.AppVersion,
                    LogoImgUrl = l_Device.LogoImgUrl,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }


        public CustomJsonResult InitEdit(string operater, RupMerchDeviceInitEdit rup)
        {
            var result = new CustomJsonResult();

            var device = CurrentDb.Device.Where(m => m.Id == rup.Id).FirstOrDefault();

            var deviceCabinets = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == rup.Id).ToList();

            List<object> cabinets = new List<object>();

            foreach (var deviceCabinet in deviceCabinets)
            {
                string pendantRows = "";

                if (deviceCabinet.CabinetId.Contains("ds"))
                {
                    var rowlayout = deviceCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                    if (rowlayout != null)
                    {
                        pendantRows = rowlayout.PendantRows.ToJsonString();
                    }
                }

                cabinets.Add(new { Id = deviceCabinet.CabinetId, Name = deviceCabinet.CabinetName, ComId = deviceCabinet.ComId, IsUse = deviceCabinet.IsUse, PendantRows = pendantRows });
            }

            var data = new
            {
                Id = device.Id,
                Name = device.Name,
                ImeiId = device.ImeiId,
                MacAddress = device.MacAddress,
                AppVersionCode = device.AppVersionCode,
                AppVersionName = device.AppVersionName,
                CtrlSdkVersionCode = device.CtrlSdkVersionCode,
                KindIsHidden = device.KindIsHidden,
                KindRowCellSize = device.KindRowCellSize,
                IsTestMode = device.IsTestMode,
                CameraByChkIsUse = device.CameraByChkIsUse,
                CameraByJgIsUse = device.CameraByJgIsUse,
                CameraByRlIsUse = device.CameraByRlIsUse,
                ExIsHas = device.ExIsHas,
                SannerIsUse = device.SannerIsUse,
                SannerComId = device.SannerComId,
                FingerVeinnerIsUse = device.FingerVeinnerIsUse,
                MstVern = device.MstVern,
                OstVern = device.OstVern,
                Cabinets = cabinets,
                ImIsUse = device.ImIsUse
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);

            return result;
        }

        public CustomJsonResult Edit(string operater, RopMerchDeviceEdit rop)
        {
            var result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var l_Device = CurrentDb.Device.Where(m => m.Id == rop.Id).FirstOrDefault();

                l_Device.CameraByChkIsUse = rop.CameraByChkIsUse;
                l_Device.CameraByJgIsUse = rop.CameraByJgIsUse;
                l_Device.CameraByRlIsUse = rop.CameraByRlIsUse;
                l_Device.ExIsHas = rop.ExIsHas;
                l_Device.SannerIsUse = rop.SannerIsUse;
                l_Device.SannerComId = rop.SannerComId;
                l_Device.FingerVeinnerIsUse = rop.FingerVeinnerIsUse;
                l_Device.MstVern = rop.MstVern;
                l_Device.OstVern = rop.OstVern;
                l_Device.KindIsHidden = rop.KindIsHidden;
                l_Device.KindRowCellSize = rop.KindRowCellSize;
                l_Device.ImIsUse = rop.ImIsUse;

                if (l_Device.ImIsUse)
                {
                    if (string.IsNullOrEmpty(l_Device.ImUserName))
                    {
                        l_Device.ImPartner = "Em";
                        l_Device.ImUserName = string.Format("MH_{0}", l_Device.Id);
                        l_Device.ImPassword = "1a2b3c4d";
                        var var1 = SdkFactory.Easemob.RegisterUser(l_Device.ImUserName, l_Device.ImPassword, l_Device.Id);
                        if (var1.Result != ResultType.Success)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，音频服务存在问题");
                        }
                    }
                }

                foreach (var cabinet in rop.Cabinets)
                {
                    var deviceCabinet = CurrentDb.DeviceCabinet.Where(m => m.CabinetId == cabinet.Id && m.DeviceId == rop.Id).FirstOrDefault();
                    if (deviceCabinet != null)
                    {
                        deviceCabinet.ComId = cabinet.ComId;
                        deviceCabinet.IsUse = cabinet.IsUse;
                        if (deviceCabinet.CabinetId.StartsWith("ds"))
                        {
                            if (!string.IsNullOrEmpty(deviceCabinet.RowColLayout))
                            {
                                var rowColLayout = deviceCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                                if (rowColLayout != null)
                                {
                                    rowColLayout.PendantRows = cabinet.PendantRows.ToJsonObject<List<int>>();

                                    deviceCabinet.RowColLayout = rowColLayout.ToJsonString();
                                }
                            }
                        }


                        CurrentDb.SaveChanges();
                    }

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            return result;
        }

        public CustomJsonResult BindOnMerch(string operater, RopMerchDeviceBindOnMerch rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();
                if (device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该设备");
                }

                if (!string.IsNullOrEmpty(device.CurUseMerchId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已绑定商户");
                }

                var merchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == rop.MerchId && m.DeviceId == rop.DeviceId).FirstOrDefault();
                if (merchDevice == null)
                {
                    merchDevice = new MerchDevice();
                    merchDevice.Id = IdWorker.Build(IdType.NewGuid);
                    merchDevice.Name = device.Name;
                    merchDevice.MerchId = rop.MerchId;
                    merchDevice.DeviceId = rop.DeviceId;
                    merchDevice.LogoImgUrl = null;
                    merchDevice.IsStopUse = false;
                    merchDevice.CreateTime = DateTime.Now;
                    merchDevice.Creator = operater;
                    CurrentDb.MerchDevice.Add(merchDevice);
                }
                else
                {
                    merchDevice.IsStopUse = false;
                    merchDevice.Mender = operater;
                    merchDevice.MendTime = DateTime.Now;
                }

                device.CurUseMerchId = rop.MerchId;
                device.CurUseStoreId = null;
                device.Mender = operater;
                device.MendTime = DateTime.Now;


                var deviceBindLog = new DeviceBindLog();
                deviceBindLog.Id = IdWorker.Build(IdType.NewGuid);
                deviceBindLog.DeviceId = rop.DeviceId;
                deviceBindLog.MerchId = rop.MerchId;
                deviceBindLog.StoreId = null;
                deviceBindLog.ShopId = null;
                deviceBindLog.BindType = E_DeviceBindType.BindOnMerch;
                deviceBindLog.CreateTime = DateTime.Now;
                deviceBindLog.Creator = operater;
                deviceBindLog.RemarkByDev = "绑定商户";
                CurrentDb.DeviceBindLog.Add(deviceBindLog);

                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
            }
            return result;
        }

        public CustomJsonResult BindOffMerch(string operater, RopMerchDeviceBindOffMerch rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();

                if (device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该设备");
                }

                var merchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == rop.MerchId && m.DeviceId == rop.DeviceId).FirstOrDefault();
                if (merchDevice != null)
                {
                    merchDevice.IsStopUse = true;
                    merchDevice.Mender = operater;
                    merchDevice.MendTime = DateTime.Now;
                }

                var deviceBindLog = new DeviceBindLog();
                deviceBindLog.Id = IdWorker.Build(IdType.NewGuid);
                deviceBindLog.DeviceId = rop.DeviceId;
                deviceBindLog.MerchId = device.CurUseMerchId;
                deviceBindLog.StoreId = device.CurUseStoreId;
                deviceBindLog.ShopId = device.CurUseShopId;
                deviceBindLog.BindType = E_DeviceBindType.BindOnMerch;
                deviceBindLog.CreateTime = DateTime.Now;
                deviceBindLog.Creator = operater;
                deviceBindLog.RemarkByDev = "解绑商户";
                CurrentDb.DeviceBindLog.Add(deviceBindLog);

                device.CurUseMerchId = null;
                device.CurUseStoreId = null;
                device.CurUseShopId = null;
                device.Mender = operater;
                device.MendTime = DateTime.Now;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");
            }
            return result;
        }
    }
}
