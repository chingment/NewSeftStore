using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.Service.UI;
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
    public class DevVendingService : DeviceService
    {
        public StatusModel GetStatus(string curUseShopId, bool isStopUse, bool isEx, E_DeviceRunStatus runstatus, DateTime? lastRequestTime)
        {
            var status = new StatusModel();

            if (isStopUse)
            {
                return new StatusModel(1, "停止使用");
            }

            if (string.IsNullOrEmpty(curUseShopId))
            {
                return new StatusModel(1, "未绑定门店");
            }

            if (isEx)
            {
                return new StatusModel(3, "异常");
            }

            if (lastRequestTime != null)
            {
                if ((DateTime.Now - lastRequestTime.Value).TotalMinutes > 15)
                {
                    return new StatusModel(3, "离线");
                }
            }

            switch (runstatus)
            {
                case E_DeviceRunStatus.Running:
                    status.Text = "运行中";
                    status.Value = 2;
                    break;
                case E_DeviceRunStatus.Setting:
                    status.Text = "维护中";
                    status.Value = 4;
                    break;
                //case E_DeviceRunStatus.Stoped:
                //    status.Text = "停止";
                //    status.Value = 1;
                //    break;
                default:
                    status.Text = "未知状态";
                    status.Value = 1;
                    break;
            }

            return status;
        }

        public CustomJsonResult InitGetList(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var deviceCount = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId).Count();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { deviceCount = deviceCount });
            return result;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupDeviceGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchDevice
                         join m in CurrentDb.Device on u.DeviceId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where ((rup.Id == null || u.DeviceId.Contains(rup.Id)) || (rup.Id == null || u.CumCode.Contains(rup.Id)))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, tt.Type, tt.Model, u.DeviceId, u.CumCode, tt.MainImgUrl, tt.CurUseStoreId, tt.CurUseShopId, tt.RunStatus, tt.LastRequestTime, tt.AppVersionCode, tt.CtrlSdkVersionCode, tt.ExIsHas, tt.Name, u.IsStopUse, u.CreateTime });

            if (!string.IsNullOrEmpty(rup.Type))
            {
                query = query.Where(m => m.Type == rup.Type);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.CurUseStoreId == rup.StoreId);
            }

            if (!string.IsNullOrEmpty(rup.ShopId))
            {
                query = query.Where(m => m.CurUseShopId == rup.ShopId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.OrderByDescending(m => m.CurUseShopId).ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string shopName = "未绑定门店";

                if (!string.IsNullOrEmpty(item.CurUseShopId))
                {
                    var d_Store = CurrentDb.Store.Where(m => m.Id == item.CurUseStoreId).FirstOrDefault();
                    var d_Shop = CurrentDb.Shop.Where(m => m.Id == item.CurUseShopId).FirstOrDefault();
                    shopName = string.Format("{0}/{1}", d_Store.Name, d_Shop.Name);
                }

                olist.Add(new
                {
                    Id = item.DeviceId,
                    Name = item.Name,
                    Model = item.Model,
                    Code = GetCode(item.DeviceId, item.CumCode),
                    StoreId = item.CurUseStoreId,
                    ShopId = item.CurUseShopId,
                    ShopName = shopName,
                    MainImgUrl = item.MainImgUrl,
                    Status = GetStatus(item.CurUseShopId, item.IsStopUse, item.ExIsHas, item.RunStatus, item.LastRequestTime),
                    LastRequestTime = item.LastRequestTime.ToUnifiedFormatDateTime(),
                });

            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult GetListByShop(string operater, string merchId, RupDeviceGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchDevice
                         join m in CurrentDb.Device on u.DeviceId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where ((rup.Id == null || u.DeviceId.Contains(rup.Id)) || (rup.Id == null || u.CumCode.Contains(rup.Id)))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, tt.Type, tt.Model, u.DeviceId, u.CumCode, tt.MainImgUrl, tt.CurUseStoreId, tt.CurUseShopId, tt.RunStatus, tt.LastRequestTime, tt.AppVersionCode, tt.CtrlSdkVersionCode, tt.ExIsHas, tt.Name, u.IsStopUse, u.CreateTime });

            if (!string.IsNullOrEmpty(rup.Type))
            {
                query = query.Where(m => m.Type == rup.Type);
            }

            query = query.Where(m => m.CurUseStoreId == rup.StoreId && m.CurUseShopId == rup.ShopId);


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.CurUseStoreId).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.OrderBy(m => m.IsStopUse).ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string shopName = "未绑定门店";

                if (!string.IsNullOrEmpty(item.CurUseShopId))
                {
                    var d_Store = CurrentDb.Store.Where(m => m.Id == item.CurUseStoreId).FirstOrDefault();
                    var d_Shop = CurrentDb.Shop.Where(m => m.Id == item.CurUseShopId).FirstOrDefault();

                    shopName = string.Format("{0}/{1}", d_Store.Name, d_Shop.Name);
                }

                olist.Add(new
                {
                    Id = item.DeviceId,
                    Name = item.Name,
                    Model = item.Model,
                    Code = GetCode(item.DeviceId, item.CumCode),
                    StoreId = item.CurUseStoreId,
                    ShopId = item.CurUseShopId,
                    MainImgUrl = item.MainImgUrl,
                    Status = GetStatus(item.CurUseShopId, item.IsStopUse, item.ExIsHas, item.RunStatus, item.LastRequestTime),
                    LastRequestTime = item.LastRequestTime.ToUnifiedFormatDateTime(),
                    ShopName = shopName
                });

            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult GetListBySbShop(string operater, string merchId, RupDeviceGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchDevice
                         join m in CurrentDb.Device on u.DeviceId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where ((rup.Id == null || u.DeviceId.Contains(rup.Id)) || (rup.Id == null || u.CumCode.Contains(rup.Id)))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, tt.Type, tt.Model, u.DeviceId, u.CumCode, tt.MainImgUrl, tt.CurUseStoreId, tt.CurUseShopId, tt.RunStatus, tt.LastRequestTime, tt.AppVersionCode, tt.CtrlSdkVersionCode, tt.ExIsHas, tt.Name, u.IsStopUse, u.CreateTime });

            if (!string.IsNullOrEmpty(rup.Type))
            {
                query = query.Where(m => m.Type == rup.Type);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.OrderBy(m => m.CurUseShopId).ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string shopName = "未绑定门店";

                if (!string.IsNullOrEmpty(item.CurUseShopId))
                {
                    var d_Store = CurrentDb.Store.Where(m => m.Id == item.CurUseStoreId).FirstOrDefault();
                    var d_Shop = CurrentDb.Shop.Where(m => m.Id == item.CurUseShopId).FirstOrDefault();

                    shopName = string.Format("{0}/{1}", d_Store.Name, d_Shop.Name);
                }

                string opTips = "";

                bool isCanSelect = false;


                if (string.IsNullOrEmpty(item.CurUseShopId))
                {
                    isCanSelect = true;
                }
                else
                {
                    opTips = "已绑定";
                }

                olist.Add(new
                {
                    Id = item.DeviceId,
                    Name = item.Name,
                    Model = item.Model,
                    Code = GetCode(item.DeviceId, item.CumCode),
                    StoreId = item.CurUseStoreId,
                    ShopId = item.CurUseShopId,
                    MainImgUrl = item.MainImgUrl,
                    Status = GetStatus(item.CurUseShopId, item.IsStopUse, item.ExIsHas, item.RunStatus, item.LastRequestTime),
                    LastRequestTime = item.LastRequestTime.ToUnifiedFormatDateTime(),
                    ShopName = shopName,
                    IsCanSelect = isCanSelect,
                    OpTips = opTips
                });

            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult InitManage(string operater, string merchId, string deviceId)
        {
            var ret = new RetDeviceInitManage();

            var d_MerchDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId).OrderByDescending(r => r.CurUseStoreId).ToList();

            d_MerchDevices = d_MerchDevices.OrderBy(m => m.IsStopUse).ToList();

            foreach (var d_MerchDevice in d_MerchDevices)
            {
                string name = "";
                if (d_MerchDevice.IsStopUse)
                {
                    name = string.Format("{0} [{1}]", d_MerchDevice.DeviceId, "已停止使用");
                }
                else
                {
                    if (string.IsNullOrEmpty(d_MerchDevice.CurUseStoreId))
                    {
                        name = string.Format("{0} [未绑定店铺]", d_MerchDevice.DeviceId);
                    }
                    else if (string.IsNullOrEmpty(d_MerchDevice.CurUseShopId))
                    {
                        name = string.Format("{0} [未绑定门店]", d_MerchDevice.DeviceId);
                    }
                    else
                    {
                        var d_Store = CurrentDb.Store.Where(m => m.Id == d_MerchDevice.CurUseStoreId).FirstOrDefault();

                        var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_MerchDevice.CurUseShopId).FirstOrDefault();

                        if (d_Store != null && d_Shop != null)
                        {
                            name = string.Format("{0} [{1}/{2}]", GetCode(d_MerchDevice.DeviceId, d_MerchDevice.CumCode), d_Store.Name, d_Shop.Name);
                        }
                        else
                        {
                            name = "未知";
                        }
                    }

                }

                if (d_MerchDevice.DeviceId == deviceId)
                {
                    ret.CurDevice = new DeviceModel();
                    ret.CurDevice.Id = d_MerchDevice.DeviceId;
                    ret.CurDevice.Name = name;
                }



                ret.Devices.Add(new DeviceModel { Id = d_MerchDevice.DeviceId, Name = name });
            }


            if (ret.CurDevice == null)
            {
                if (ret.Devices.Count > 0)
                {
                    ret.CurDevice = ret.Devices[0];
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManageBaseInfo(string operater, string merchId, string deviceId)
        {
            var result = new CustomJsonResult();

            var ret = new RetDeviceInitManageBaseInfo();

            var d_Device = (from s in CurrentDb.MerchDevice
                            join m in CurrentDb.Device on s.DeviceId equals m.Id into temp
                            from u in temp.DefaultIfEmpty()
                            where
                            s.MerchId == merchId
                            &&
                            s.DeviceId == deviceId
                            select new { s.DeviceId, u.AppVersionCode, u.CtrlSdkVersionCode, s.CumCode, u.Name, s.LogoImgUrl, s.CurUseStoreId, s.CurUseShopId, u.RunStatus, u.LastRequestTime, u.ExIsHas, s.IsStopUse }).FirstOrDefault();

            ret.Id = d_Device.DeviceId;
            ret.Name = d_Device.Name;
            ret.CumCode = d_Device.CumCode;
            ret.LogoImgUrl = d_Device.LogoImgUrl;
            ret.Status = GetStatus(d_Device.CurUseShopId, d_Device.IsStopUse, d_Device.ExIsHas, d_Device.RunStatus, d_Device.LastRequestTime);
            ret.LastRequestTime = d_Device.LastRequestTime.ToUnifiedFormatDateTime();
            ret.AppVersion = d_Device.AppVersionCode;
            ret.CtrlSdkVersion = d_Device.CtrlSdkVersionCode;
            ret.IsStopUse = d_Device.IsStopUse;


            if (string.IsNullOrEmpty(d_Device.CurUseStoreId) || string.IsNullOrEmpty(d_Device.CurUseShopId))
            {
                ret.ShopName = "未绑定店铺门店";
            }
            else
            {
                var d_Store = CurrentDb.Store.Where(m => m.Id == d_Device.CurUseStoreId).FirstOrDefault();
                var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_Device.CurUseShopId).FirstOrDefault();

                ret.ShopName = string.Format("{0}/{1}", d_Store.Name, d_Shop.Name);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult InitManageStock(string operater, string merchId, string deviceId)
        {
            var result = new CustomJsonResult();

            var ret = new RetDeviceInitManageStock();

            var d_DeviceCabinets = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == deviceId && m.IsUse).ToList();


            foreach (var d_DeviceCabinet in d_DeviceCabinets)
            {
                var optionNode = new OptionNode();

                optionNode.Value = d_DeviceCabinet.CabinetId;
                optionNode.Label = d_DeviceCabinet.CabinetName;

                ret.OptionsCabinets.Add(optionNode);

            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult ManageStockGetStocks(string operater, string merchId, string deviceId, string cabinetId)
        {
            var result = new CustomJsonResult();

            var m_Device = BizFactory.Device.GetOne(deviceId);

            var d_DeviceCabinet = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == deviceId && m.CabinetId == cabinetId).FirstOrDefault();

            if (d_DeviceCabinet == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置机柜，请联系管理员");
            }
            if (string.IsNullOrEmpty(d_DeviceCabinet.RowColLayout))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "识别不到设备列数");
            }


            List<object> olist = new List<object>();

            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.CabinetId == cabinetId && m.StoreId == m_Device.StoreId && m.ShopId == m_Device.ShopId && m.DeviceId == deviceId && m.ShopMode == E_ShopMode.Device).ToList();

            List<SlotRowModel> rows = new List<SlotRowModel>();


            switch (cabinetId)
            {
                case "dsx01n01":
                    #region zsx01n01
                    var dsCabinetRowColLayout = d_DeviceCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                    if (dsCabinetRowColLayout != null)
                    {
                        int rowsLength = dsCabinetRowColLayout.Rows.Count;

                        int num = 1;
                        for (int i = rowsLength - 1; i >= 0; i--)
                        {
                            SlotRowModel row = new SlotRowModel();
                            row.No = i;

                            int cols = dsCabinetRowColLayout.Rows[i];
                            for (int j = cols - 1; j >= 0; j--)
                            {
                                var slotId = string.Format("{0}-{1}-{2}", i, j, num);

                                var col = new SlotColModel();
                                col.No = j;
                                col.SlotId = slotId;

                                var slotStock = sellChannelStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                                if (slotStock != null)
                                {
                                    var r_Sku = CacheServiceFactory.Product.GetSkuInfo(merchId, slotStock.SkuId);
                                    col.SkuId = r_Sku.Id;
                                    col.Name = r_Sku.Name;
                                    col.MainImgUrl = r_Sku.MainImgUrl;
                                    col.SumQuantity = slotStock.SumQuantity;
                                    col.LockQuantity = slotStock.WaitPayLockQuantity + slotStock.WaitPickupLockQuantity;
                                    col.SellQuantity = slotStock.SellQuantity;
                                    col.WarnQuantity = slotStock.WarnQuantity;
                                    col.MaxQuantity = slotStock.MaxQuantity;
                                    col.HoldQuantity = slotStock.HoldQuantity;
                                    col.SalePrice = slotStock.SalePrice;
                                    col.IsOffSell = slotStock.IsOffSell;
                                    col.Version = slotStock.Version;
                                }

                                row.Cols.Add(col);

                                num++;
                            }

                            rows.Add(row);
                        }
                    }
                    #endregion
                    break;
                case "zsx01n01":
                case "zsx01n02":
                    #region zsx01n01
                    var zsCabinetRowColLayout = d_DeviceCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByZSModel>();
                    if (zsCabinetRowColLayout != null)
                    {
                        if (zsCabinetRowColLayout.Rows != null)
                        {
                            int rowsLength = zsCabinetRowColLayout.Rows.Count;
                            LogUtil.Info("rowsLength：" + rowsLength);

                            for (int i = 0; i < rowsLength; i++)
                            {
                                if (zsCabinetRowColLayout.Rows[i] != null)
                                {
                                    SlotRowModel row = new SlotRowModel();
                                    row.No = i;

                                    int cols = zsCabinetRowColLayout.Rows[i].Count;
                                    LogUtil.Info("cols.length：" + cols);
                                    for (int j = 0; j < cols; j++)
                                    {

                                        var slotId = zsCabinetRowColLayout.Rows[i][j].ToString();

                                        var col = new SlotColModel();
                                        col.No = j;
                                        col.SlotId = slotId;

                                        var slotStock = sellChannelStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                                        if (slotStock != null)
                                        {
                                            var r_Sku = CacheServiceFactory.Product.GetSkuInfo(merchId, slotStock.SkuId);

                                            col.SkuId = r_Sku.Id;
                                            col.Name = r_Sku.Name;
                                            col.MainImgUrl = r_Sku.MainImgUrl;
                                            col.SumQuantity = slotStock.SumQuantity;
                                            col.LockQuantity = slotStock.WaitPayLockQuantity + slotStock.WaitPickupLockQuantity;
                                            col.SellQuantity = slotStock.SellQuantity;
                                            col.WarnQuantity = slotStock.WarnQuantity;
                                            col.MaxQuantity = slotStock.MaxQuantity;
                                            col.HoldQuantity = slotStock.HoldQuantity;
                                            col.SalePrice = slotStock.SalePrice;
                                            col.IsOffSell = slotStock.IsOffSell;
                                            col.Version = slotStock.Version;

                                        }

                                        row.Cols.Add(col);
                                    }

                                    rows.Add(row);
                                }
                            }
                        }
                    }
                    #endregion
                    break;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", rows);

            return result;
        }

        public CustomJsonResult ManageStockEditStock(string operater, string merchId, RopDeviceEditStock rop)
        {
            var m_Device = BizFactory.Device.GetOne(rop.DeviceId);

            var result = BizFactory.ProductSku.AdjustStockQuantity(operater, E_ShopMode.Device, merchId, m_Device.StoreId, m_Device.ShopId, rop.DeviceId, rop.CabinetId, rop.SlotId, rop.SkuId, rop.Version, rop.SumQuantity, rop.MaxQuantity, rop.WarnQuantity, rop.HoldQuantity);

            if (result.Result == ResultType.Success)
            {
                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.device_adjust_stock_quantity, string.Format("店铺：{0}，门店：{1}，设备：{2}，机柜：{3}，货道：{4}，调整库存", m_Device.StoreName, m_Device.ShopName, m_Device.DeviceId, rop.CabinetId, rop.SlotId), rop);
            }

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopDeviceEdit rop)
        {
            var result = new CustomJsonResult();

            var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.DeviceId == rop.Id).FirstOrDefault();
            if (d_MerchDevice.IsStopUse)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            string cumCode = null;
            if (!string.IsNullOrEmpty(rop.CumCode))
            {
                cumCode = rop.CumCode.Trim();

                var d_ExistCumCode = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.DeviceId != rop.Id && m.CumCode == cumCode).FirstOrDefault();
                if (d_ExistCumCode != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备自定义编码已经存在");
                }
            }

            d_MerchDevice.CumCode = cumCode;
            //d_MerchDevice.LogoImgUrl = rop.LogoImgUrl;
            d_MerchDevice.MendTime = DateTime.Now;
            d_MerchDevice.Mender = operater;
            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            if (result.Result == ResultType.Success)
            {
                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.device_edit, string.Format("设备：{0}，信息修改，保存成功", d_MerchDevice.DeviceId), rop);
            }

            return result;
        }

        public CustomJsonResult RebootSys(string operater, string merchId, RopDeviceRebootSys rop)
        {
            var result = BizFactory.Device.SendRebootSys(operater, AppId.MERCH, merchId, rop.Id);
            return result;
        }

        public CustomJsonResult ShutdownSys(string operater, string merchId, RopDeviceShutdownSys rop)
        {
            var result = BizFactory.Device.SendShutdownSys(operater, AppId.MERCH, merchId, rop.Id);
            return result;
        }

        public CustomJsonResult SetSysStatus(string operater, string merchId, RopDeviceSetSysStatus rop)
        {
            var m_Device = CurrentDb.Device.Where(m => m.Id == rop.Id && m.CurUseMerchId == merchId).FirstOrDefault();

            if (m_Device == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该找不到记录");
            }

            if (rop.Status == 1)
            {
                m_Device.ExIsHas = false;
                m_Device.ExReason = "";
            }
            else if (rop.Status == 2)
            {
                m_Device.ExIsHas = true;
                m_Device.ExReason = "后台人员设置维护中";
            }

            CurrentDb.SaveChanges();

            var result = BizFactory.Device.SendSetSysStatus(operater, AppId.MERCH, merchId, rop.Id, rop.Status, rop.HelpTip);

            return result;
        }

        public CustomJsonResult GetSysParams(string operater, string merchId, string deviceId)
        {
            var m_Device = CurrentDb.Device.Where(m => m.Id == deviceId && m.CurUseMerchId == merchId).FirstOrDefault();

            if (m_Device == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该找不到记录");
            }

            var ret = new
            {
                Id = m_Device.Id,
                CbLight = m_Device.CbLight.ToJsonObject<List<object>>()
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该找不到记录", ret);
        }

        public CustomJsonResult SetSysParams(string operater, string merchId, RopDeviceSetSysParams rop)
        {
            var m_Device = CurrentDb.Device.Where(m => m.Id == rop.Id && m.CurUseMerchId == merchId).FirstOrDefault();

            if (m_Device == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该找不到记录");
            }

            m_Device.CbLight = rop.CbLight.ToJsonString();

            CurrentDb.SaveChanges();

            Dictionary<string, string> lights = new Dictionary<string, string>();
            lights.Add("cb", m_Device.CbLight);

            var result = BizFactory.Device.SendSetSysParams(operater, AppId.MERCH, merchId, rop.Id, new { Lights = lights });

            return result;
        }

        public CustomJsonResult OpenPickupDoor(string operater, string merchId, RopDeviceOpenPickupDoor rop)
        {

            var result = BizFactory.Device.SendOpenPickupDoor(operater, AppId.MERCH, merchId, rop.Id);

            return result;
        }

        public CustomJsonResult UpdateApp(string operater, string merchId, RopDeviceOpenPickupDoor rop)
        {

            var result = BizFactory.Device.SendUpdateApp(operater, AppId.MERCH, merchId, rop.Id);

            return result;
        }

        public CustomJsonResult QueryMsgPushResult(string operater, string merchId, RopDeviceQueryMsgPushResult rop)
        {

            var result = BizFactory.Device.QueryMsgPushResult(operater, AppId.MERCH, merchId, rop.DeviceId, rop.MsgId);

            return result;
        }

        public CustomJsonResult UnBindShop(string operater, string merchId, RopDeviceUnBindShop rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_Device = CurrentDb.Device.Where(m => m.CurUseMerchId == merchId && m.Id == rop.DeviceId && m.CurUseStoreId == rop.StoreId && m.CurUseShopId == rop.ShopId).FirstOrDefault();

                if (d_Device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已解绑门店");
                }

                var d_LockCount = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.ShopId == rop.ShopId && (m.WaitPayLockQuantity > 0 || m.WaitPickupLockQuantity > 0)).Count();

                if (d_LockCount > 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "存在已锁定的商品数量，不能解绑，请先确保数量未被锁定");
                }

                d_Device.CurUseStoreId = null;
                d_Device.CurUseShopId = null;
                d_Device.Mender = operater;
                d_Device.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();

                var d_Store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();
                var d_Shop = CurrentDb.Shop.Where(m => m.Id == rop.ShopId).FirstOrDefault();
                var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.DeviceId == rop.DeviceId && m.MerchId == merchId).FirstOrDefault();

                if (d_MerchDevice != null)
                {
                    d_MerchDevice.CurUseShopId = null;
                    d_MerchDevice.CurUseStoreId = null;
                    d_MerchDevice.Mender = operater;
                    d_MerchDevice.MendTime = DateTime.Now;
                    CurrentDb.SaveChanges();
                }

                var d_Stocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.ShopId == rop.ShopId && m.DeviceId == rop.DeviceId).ToList();
                foreach (var d_Stock in d_Stocks)
                {
                    CurrentDb.SellChannelStock.Remove(d_Stock);
                    CurrentDb.SaveChanges();
                }

                var d_DeviceBindLog = new DeviceBindLog();
                d_DeviceBindLog.Id = IdWorker.Build(IdType.NewGuid);
                d_DeviceBindLog.DeviceId = rop.DeviceId;
                d_DeviceBindLog.MerchId = merchId;
                d_DeviceBindLog.StoreId = rop.StoreId;
                d_DeviceBindLog.ShopId = rop.ShopId;
                d_DeviceBindLog.BindType = E_DeviceBindType.BindOffShop;
                d_DeviceBindLog.CreateTime = DateTime.Now;
                d_DeviceBindLog.Creator = operater;
                d_DeviceBindLog.RemarkByDev = "解绑门店";
                CurrentDb.DeviceBindLog.Add(d_DeviceBindLog);
                CurrentDb.SaveChanges();

                CurrentDb.SaveChanges();
                ts.Complete();


                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.device_unbind_shop, string.Format("将设备（{0}）从店铺（{1}）门店（{2}）移除成功", rop.DeviceId, d_Store.Name, d_Shop.Name), rop);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "解绑成功");

            }
            return result;
        }
        public CustomJsonResult BindShop(string operater, string merchId, RopDeviceUnBindShop rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_Device = CurrentDb.Device.Where(m => m.CurUseMerchId == merchId && m.Id == rop.DeviceId).FirstOrDefault();

                if (d_Device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到设备");
                }

                if (!string.IsNullOrEmpty(d_Device.CurUseStoreId) || !string.IsNullOrEmpty(d_Device.CurUseShopId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已被绑定，请先解除绑定");
                }

                d_Device.CurUseStoreId = rop.StoreId;
                d_Device.CurUseShopId = rop.ShopId;
                d_Device.Mender = operater;
                d_Device.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();

                var d_Store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();
                var d_Shop = CurrentDb.Shop.Where(m => m.Id == rop.ShopId).FirstOrDefault();
                var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.DeviceId == rop.DeviceId && m.MerchId == merchId).FirstOrDefault();

                if (d_MerchDevice == null)
                {
                    d_MerchDevice = new MerchDevice();
                    d_MerchDevice.Id = IdWorker.Build(IdType.NewGuid);
                    d_MerchDevice.MerchId = merchId;
                    d_MerchDevice.DeviceId = rop.DeviceId;
                    d_MerchDevice.CurUseStoreId = rop.StoreId;
                    d_MerchDevice.CurUseShopId = rop.ShopId;
                    d_MerchDevice.LogoImgUrl = d_Device.LogoImgUrl;
                    d_MerchDevice.Creator = operater;
                    d_MerchDevice.CreateTime = DateTime.Now;
                    CurrentDb.MerchDevice.Add(d_MerchDevice);
                    CurrentDb.SaveChanges();
                }
                else
                {
                    d_MerchDevice.CurUseStoreId = rop.StoreId;
                    d_MerchDevice.CurUseShopId = rop.ShopId;
                    d_MerchDevice.Mender = operater;
                    d_MerchDevice.MendTime = DateTime.Now;
                    CurrentDb.SaveChanges();
                }

                var d_DeviceBindLog = new DeviceBindLog();
                d_DeviceBindLog.Id = IdWorker.Build(IdType.NewGuid);
                d_DeviceBindLog.DeviceId = rop.DeviceId;
                d_DeviceBindLog.MerchId = merchId;
                d_DeviceBindLog.StoreId = rop.StoreId;
                d_DeviceBindLog.ShopId = rop.ShopId;
                d_DeviceBindLog.BindType = E_DeviceBindType.BindOnShop;
                d_DeviceBindLog.CreateTime = DateTime.Now;
                d_DeviceBindLog.Creator = operater;
                d_DeviceBindLog.RemarkByDev = "绑定门店";
                CurrentDb.DeviceBindLog.Add(d_DeviceBindLog);
                CurrentDb.SaveChanges();

                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.device_bind_shop, string.Format("选择设备（{0}）到店铺（{1}）门店（{2}）添加成功", rop.DeviceId, d_Store.Name, d_Shop.Name), rop);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");

            }

            return result;
        }

    }
}
