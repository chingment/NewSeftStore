using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class MachineService : BaseService
    {

        public StatusModel GetStatus(string curUseShopId, bool isStopUse, bool isEx, E_MachineRunStatus runstatus, DateTime? lastRequestTime)
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
                case E_MachineRunStatus.Running:
                    status.Text = "运行中";
                    status.Value = 2;
                    break;
                case E_MachineRunStatus.Setting:
                    status.Text = "维护中";
                    status.Value = 4;
                    break;
                //case E_MachineRunStatus.Stoped:
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

            var machineCount = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).Count();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { machineCount = machineCount });
            return result;
        }


        public CustomJsonResult GetList(string operater, string merchId, RupMachineGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchMachine
                         join m in CurrentDb.Machine on u.MachineId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where (rup.Id == null || u.MachineId.Contains(rup.Id))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, u.MachineId, tt.MainImgUrl, tt.CurUseStoreId, tt.CurUseShopId, tt.RunStatus, tt.LastRequestTime, tt.AppVersionCode, tt.CtrlSdkVersionCode, tt.ExIsHas, u.Name, u.IsStopUse, u.CreateTime });


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
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CurUseStoreId).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.OrderBy(m => m.IsStopUse).ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string shopName = "未绑定门店";

                if (!string.IsNullOrEmpty(item.CurUseShopId))
                {
                    var store = CurrentDb.Store.Where(m => m.Id == item.CurUseStoreId).FirstOrDefault();
                    var shop = CurrentDb.Shop.Where(m => m.Id == item.CurUseShopId).FirstOrDefault();

                    shopName = string.Format("{0}/{1}", store.Name, shop.Name);
                }

                olist.Add(new
                {
                    Id = item.MachineId,
                    Name = item.MachineId,
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

        public CustomJsonResult InitManage(string operater, string merchId, string machineId)
        {
            var ret = new RetMachineInitManage();

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).OrderByDescending(r => r.CurUseStoreId).ToList();

            merchMachines = merchMachines.OrderBy(m => m.IsStopUse).ToList();

            foreach (var merchMachine in merchMachines)
            {
                string name = "";
                if (merchMachine.IsStopUse)
                {
                    name = string.Format("{0} [{1}]", merchMachine.MachineId, "已停止使用");
                }
                else
                {
                    if (string.IsNullOrEmpty(merchMachine.CurUseStoreId))
                    {
                        name = string.Format("{0} [未绑定店铺]", merchMachine.MachineId);
                    }
                    else if (string.IsNullOrEmpty(merchMachine.CurUseShopId))
                    {
                        name = string.Format("{0} [未绑定门店]", merchMachine.MachineId);
                    }
                    else
                    {
                        var store = CurrentDb.Store.Where(m => m.Id == merchMachine.CurUseStoreId).FirstOrDefault();

                        var shop = CurrentDb.Shop.Where(m => m.Id == merchMachine.CurUseShopId).FirstOrDefault();

                        if (store != null && shop != null)
                        {
                            name = string.Format("{0} [{1}/{2}]", merchMachine.MachineId, store.Name, shop.Name);
                        }
                        else
                        {
                            name = "未知";
                        }
                    }

                }

                if (merchMachine.MachineId == machineId)
                {
                    ret.CurMachine = new MachineModel();
                    ret.CurMachine.Id = merchMachine.MachineId;
                    ret.CurMachine.Name = name;
                }



                ret.Machines.Add(new MachineModel { Id = merchMachine.MachineId, Name = name });
            }


            if (ret.CurMachine == null)
            {
                if (ret.Machines.Count > 0)
                {
                    ret.CurMachine = ret.Machines[0];
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManageBaseInfo(string operater, string merchId, string machineId)
        {
            var result = new CustomJsonResult();

            var ret = new RetMachineInitManageBaseInfo();

            var d_machine = (from s in CurrentDb.MerchMachine
                             join m in CurrentDb.Machine on s.MachineId equals m.Id into temp
                             from u in temp.DefaultIfEmpty()
                             where
                             s.MerchId == merchId
                             &&
                             s.MachineId == machineId
                             select new { s.MachineId, u.AppVersionCode, u.CtrlSdkVersionCode, u.Name, s.LogoImgUrl, s.CurUseStoreId, s.CurUseShopId, u.RunStatus, u.LastRequestTime, u.ExIsHas, s.IsStopUse }).FirstOrDefault();

            ret.Id = d_machine.MachineId;
            ret.Name = d_machine.Name;
            ret.LogoImgUrl = d_machine.LogoImgUrl;
            ret.Status = GetStatus(d_machine.CurUseShopId, d_machine.IsStopUse, d_machine.ExIsHas, d_machine.RunStatus, d_machine.LastRequestTime);
            ret.LastRequestTime = d_machine.LastRequestTime.ToUnifiedFormatDateTime();
            ret.AppVersion = d_machine.AppVersionCode;
            ret.CtrlSdkVersion = d_machine.CtrlSdkVersionCode;
            ret.IsStopUse = d_machine.IsStopUse;


            if (string.IsNullOrEmpty(d_machine.CurUseStoreId) || string.IsNullOrEmpty(d_machine.CurUseShopId))
            {
                ret.ShopName = "未绑定店铺门店";
            }
            else
            {
                var store = CurrentDb.Store.Where(m => m.Id == d_machine.CurUseStoreId).FirstOrDefault();
                var shop = CurrentDb.Shop.Where(m => m.Id == d_machine.CurUseShopId).FirstOrDefault();

                ret.ShopName = string.Format("{0}/{1}", store.Name, shop.Name);
            }




            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult InitManageStock(string operater, string merchId, string machineId)
        {
            var result = new CustomJsonResult();

            var ret = new RetMachineInitManageStock();

            var machineCabinets = CurrentDb.MachineCabinet.Where(m => m.MachineId == machineId && m.IsUse).ToList();


            foreach (var machineCabinet in machineCabinets)
            {
                var optionNode = new OptionNode();

                optionNode.Value = machineCabinet.CabinetId;
                optionNode.Label = machineCabinet.CabinetName;

                ret.OptionsCabinets.Add(optionNode);

            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult ManageStockGetStocks(string operater, string merchId, string machineId, string cabinetId)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(machineId);

            var machineCabinet = CurrentDb.MachineCabinet.Where(m => m.MachineId == machineId && m.CabinetId == cabinetId).FirstOrDefault();

            if (machineCabinet == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置机柜，请联系管理员");
            }
            if (string.IsNullOrEmpty(machineCabinet.RowColLayout))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "识别不到机器列数");
            }


            List<object> olist = new List<object>();

            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.CabinetId == cabinetId && m.StoreId == machine.StoreId && m.ShopId == machine.ShopId && m.MachineId == machineId && m.ShopMode == E_ShopMode.Machine).ToList();

            List<SlotRowModel> rows = new List<SlotRowModel>();


            switch (cabinetId)
            {
                case "dsx01n01":
                    #region zsx01n01
                    var dsCabinetRowColLayout = machineCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                    if (dsCabinetRowColLayout != null)
                    {
                        int rowsLength = dsCabinetRowColLayout.Rows.Count;

                        for (int i = rowsLength - 1; i >= 0; i--)
                        {
                            SlotRowModel row = new SlotRowModel();
                            row.No = i;

                            int cols = dsCabinetRowColLayout.Rows[i];
                            for (int j = 0; j < cols; j++)
                            {
                                var slotId = string.Format("r{0}c{1}", i, j);

                                var col = new SlotColModel();
                                col.No = j;
                                col.SlotId = slotId;

                                var slotStock = sellChannelStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                                if (slotStock != null)
                                {
                                    var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, slotStock.PrdProductSkuId);
                                    col.ProductSkuId = bizProductSku.Id;
                                    col.Name = bizProductSku.Name;
                                    col.MainImgUrl = bizProductSku.MainImgUrl;
                                    col.SumQuantity = slotStock.SumQuantity;
                                    col.LockQuantity = slotStock.WaitPayLockQuantity + slotStock.WaitPickupLockQuantity;
                                    col.SellQuantity = slotStock.SellQuantity;
                                    col.MaxQuantity = 10;
                                    col.SalePrice = slotStock.SalePrice;
                                    col.IsOffSell = slotStock.IsOffSell;
                                    col.Version = slotStock.Version;
                                }

                                row.Cols.Add(col);
                            }

                            rows.Add(row);
                        }
                    }
                    #endregion
                    break;
                case "zsx01n01":
                case "zsx01n02":
                    #region zsx01n01
                    var zsCabinetRowColLayout = machineCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByZSModel>();
                    if (zsCabinetRowColLayout != null)
                    {
                        if (zsCabinetRowColLayout.Rows != null)
                        {
                            int rowsLength = zsCabinetRowColLayout.Rows.Count;
                            LogUtil.Info("rowsLength：" + rowsLength);

                            for (int i = 0; i < rowsLength; i++)
                            {
                                if (zsCabinetRowColLayout.Rows[i].Cols != null)
                                {
                                    SlotRowModel row = new SlotRowModel();
                                    row.No = i;

                                    int cols = zsCabinetRowColLayout.Rows[i].Cols.Count;
                                    LogUtil.Info("cols.length：" + cols);
                                    for (int j = 0; j < cols; j++)
                                    {

                                        var slotId = zsCabinetRowColLayout.Rows[i].Cols[j].Id;

                                        var col = new SlotColModel();
                                        col.No = j;
                                        col.SlotId = slotId;

                                        var slotStock = sellChannelStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                                        if (slotStock != null)
                                        {
                                            var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, slotStock.PrdProductSkuId);

                                            col.ProductSkuId = bizProductSku.Id;
                                            col.Name = bizProductSku.Name;
                                            col.MainImgUrl = bizProductSku.MainImgUrl;
                                            col.SumQuantity = slotStock.SumQuantity;
                                            col.LockQuantity = slotStock.WaitPayLockQuantity + slotStock.WaitPickupLockQuantity;
                                            col.SellQuantity = slotStock.SellQuantity;
                                            col.MaxQuantity = 10;
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

        public CustomJsonResult ManageStockEditStock(string operater, string merchId, RopMachineEditStock rop)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            result = BizFactory.ProductSku.AdjustStockQuantity(operater, E_ShopMode.Machine, merchId, machine.StoreId, machine.ShopId, rop.MachineId, rop.CabinetId, rop.SlotId, rop.ProductSkuId, rop.Version, rop.SumQuantity);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopMachineEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {

                //var isExist = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId != rop.Id && m.Name == rop.Name).FirstOrDefault();
                //if (isExist != null)
                //{
                //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在,请使用其它");
                //}

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == rop.Id).FirstOrDefault();
                if (merchMachine.IsStopUse)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器已停止使用");
                }
                //merchMachine.Name = rop.Name;
                merchMachine.LogoImgUrl = rop.LogoImgUrl;
                merchMachine.MendTime = DateTime.Now;
                merchMachine.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.MachineEdit, string.Format("保存机器（{0}）信息成功", merchMachine.MachineId));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            if (result.Result == ResultType.Success)
            {
                BizFactory.Machine.SendHomeLogo(operater, AppId.MERCH, merchId, rop.Id, rop.LogoImgUrl);
            }

            return result;
        }



        public CustomJsonResult SysReboot(string operater, string merchId, RopMachineRebootSys rop)
        {
            CustomJsonResult result = new CustomJsonResult();



            result = BizFactory.Machine.SendSysReboot(operater, AppId.MERCH, merchId, rop.Id);

            return result;
        }

        public CustomJsonResult SysShutdown(string operater, string merchId, RopMachineShutdownSys rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            result = BizFactory.Machine.SendSysShutdown(operater, AppId.MERCH, merchId, rop.Id);

            return result;
        }

        public CustomJsonResult SysSetStatus(string operater, string merchId, RopMachineSetSysStatus rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            result = BizFactory.Machine.SendSysSetStatus(operater, AppId.MERCH, merchId, rop.Id, rop.Status, rop.HelpTip);

            return result;
        }

        public CustomJsonResult Dsx01OpenPickupDoor(string operater, string merchId, RopMachineShutdownSys rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            result = BizFactory.Machine.SendDsx01OpenPickupDoor(operater, AppId.MERCH, merchId, rop.Id);

            return result;
        }


        public CustomJsonResult QueryMsgPushResult(string operater, string merchId, RopMachineQueryMsgPushResult rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            result = BizFactory.Machine.QueryMsgPushResult(operater, AppId.MERCH, merchId, rop.MachineId, rop.msg_id);

            return result;
        }
    }
}
