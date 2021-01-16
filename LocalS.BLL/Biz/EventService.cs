using LocalS.BLL.Mq;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class EventService : BaseService
    {
        public void Handle(EventNotifyModel model)
        {

            switch (model.EventCode)
            {
                case EventCode.Login:
                    var loginLogModel = model.EventContent.ToJsonObject<LoginLogModel>();
                    HandleByLogin(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, loginLogModel);
                    break;
                case EventCode.Logout:
                    var logoutLogModel = model.EventContent.ToJsonObject<LoginLogModel>();
                    HandleByLogout(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, logoutLogModel);
                    break;
                case EventCode.MachineStatus:
                    LogUtil.Info(">>>>>MachineStatus");
                    var machineStatusModel = model.EventContent.ToJsonObject<MachineEventByMachineStatusModel>();
                    HandleByMachineStatus(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, machineStatusModel);
                    break;
                case EventCode.Pickup:
                    var pickupModel = model.EventContent.ToJsonObject<MachineEventByPickupModel>();
                    HandleByPickup(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, pickupModel);
                    break;
                case EventCode.PickupTest:
                    var pickupTestModel = model.EventContent.ToJsonObject<MachineEventByPickupModel>();
                    HandleByPickupTest(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, pickupTestModel);
                    break;
                case EventCode.MachineCabinetSlotSave:
                case EventCode.MachineCabinetSlotRemove:
                case EventCode.MachineCabinetSlotAdjustStockQuantity:
                case EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete:
                case EventCode.StockOrderPickupOneManMadeSignNotTakeByComplete:
                case EventCode.StockOrderPickupOneSysMadeSignTake:
                case EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete:
                case EventCode.StockOrderCancle:
                case EventCode.StockOrderPaySuccess:
                case EventCode.StockOrderReserveSuccess:
                    var sellChannelStockChangeModel = model.EventContent.ToJsonObject<SellChannelStockChangeModel>();
                    HandleByStockChangeLog(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, sellChannelStockChangeModel);
                    break;
            }
        }

        private void HandleByLogin(string operater, string appId, string trgerId, string eventCode, string eventRemark, LoginLogModel model)
        {
            var userLoginHis = new SysUserLoginHis();
            userLoginHis.Id = IdWorker.Build(IdType.NewGuid);
            userLoginHis.UserId = operater;
            userLoginHis.AppId = appId;
            userLoginHis.LoginAccount = model.LoginAccount;
            userLoginHis.LoginFun = model.LoginFun;
            userLoginHis.LoginWay = model.LoginWay;
            userLoginHis.Ip = model.LoginIp;
            userLoginHis.LoginTime = DateTime.Now;
            userLoginHis.Result = model.LoginResult;
            userLoginHis.Description = eventRemark;
            userLoginHis.RemarkByDev = model.RemarkByDev;
            userLoginHis.CreateTime = DateTime.Now;
            userLoginHis.Creator = operater;
            CurrentDb.SysUserLoginHis.Add(userLoginHis);
            CurrentDb.SaveChanges();

            if (appId == AppId.MERCH || appId == AppId.STORETERM || appId == AppId.WXMINPRAGROM)
            {
                MqFactory.Global.PushOperateLog(IdWorker.Build(IdType.EmptyGuid), appId, trgerId, EventCode.Logout, model.RemarkByDev);
            }
        }

        private void HandleByLogout(string operater, string appId, string trgerId, string eventCode, string eventRemark, LoginLogModel model)
        {
            var userLoginHis = new SysUserLoginHis();
            userLoginHis.Id = IdWorker.Build(IdType.NewGuid);
            userLoginHis.UserId = operater;
            userLoginHis.AppId = appId;
            userLoginHis.LoginAccount = model.LoginAccount;
            userLoginHis.LoginFun = model.LoginFun;
            userLoginHis.LoginWay = model.LoginWay;
            userLoginHis.Ip = model.LoginIp;
            userLoginHis.LoginTime = DateTime.Now;
            userLoginHis.Result = model.LoginResult;
            userLoginHis.Description = eventRemark;
            userLoginHis.RemarkByDev = model.RemarkByDev;
            userLoginHis.CreateTime = DateTime.Now;
            userLoginHis.Creator = operater;
            CurrentDb.SysUserLoginHis.Add(userLoginHis);
            CurrentDb.SaveChanges();


            if (appId == AppId.MERCH || appId == AppId.STORETERM || appId == AppId.WXMINPRAGROM)
            {
                MqFactory.Global.PushOperateLog(IdWorker.Build(IdType.EmptyGuid), appId, trgerId, EventCode.Logout, model.RemarkByDev);
            }

        }

        private void HandleByMachineStatus(string operater, string appId, string trgerId, string eventCode, string eventRemark, MachineEventByMachineStatusModel model)
        {
            LogUtil.Info(">>>>>EventHandleByMachineStatus");

            string machineId = trgerId;
            var machine = CurrentDb.Machine.Where(m => m.Id == trgerId).FirstOrDefault();

            if (machine == null)
                return;

            string merchName = BizFactory.Merch.GetMerchName(machine.CurUseMerchId);
            string storeName = BizFactory.Merch.GetStoreName(machine.CurUseMerchId, machine.CurUseStoreId);

            string operaterUserName = BizFactory.Merch.GetClientName(machine.CurUseMerchId, operater);

            machine.LastRequestTime = DateTime.Now;

            bool isLog = false;
            switch (model.Status)
            {
                case "running":
                    eventRemark = string.Format("店铺：{0}，机器：{1}，运行正常", storeName, machineId);

                    if (machine.RunStatus != E_MachineRunStatus.Running)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Running;
                    break;
                case "setting":
                    eventRemark = string.Format("店铺：{0}，机器：{1}，维护中", storeName, machineId);

                    if (machine.RunStatus != E_MachineRunStatus.Setting)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Setting;
                    break;
                case "excepition":

                    eventRemark = string.Format("店铺：{0}，机器：{1}，异常", storeName, machineId);

                    if (machine.RunStatus != E_MachineRunStatus.Excepition)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Excepition;

                    break;
                default:
                    eventRemark = string.Format("店铺：{0}，机器：{1}，未知状态", storeName, machineId);
                    break;
            }

            CurrentDb.SaveChanges();

            if (isLog)
            {
                MqFactory.Global.PushOperateLog(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, machineId, EventCode.MachineStatus, eventRemark.ToString());
            }
        }

        private void HandleByPickup(string operater, string appId, string trgerId, string eventCode, string eventRemark, MachineEventByPickupModel model)
        {
            if (model == null)
                return;

            using (TransactionScope ts = new TransactionScope())
            {

                string machineId = trgerId;
                var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();

                if (machine == null)
                    return;

                machine.LastRequestTime = DateTime.Now;

                StringBuilder remark = new StringBuilder("");

                string productSkuName = "";
                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(machine.CurUseMerchId, model.ProductSkuId);
                if (bizProductSku != null)
                {
                    productSkuName = bizProductSku.Name;
                }

                if (model.PickupStatus == E_OrderPickupStatus.SendPickupCmd)
                {
                    remark.Append("发送命令");
                }
                else if (model.PickupStatus == E_OrderPickupStatus.Taked)
                {
                    remark.Append(string.Format("取货完成，用时：{0}", model.PickupUseTime));
                }
                else if (model.PickupStatus == E_OrderPickupStatus.Exception)
                {
                    remark.Append(string.Format("发生异常，原因：{0}", model.Remark));
                }
                else
                {
                    remark.Append(string.Format("当前动作：{0}，状态：{1}", model.ActionName, model.ActionStatusName));
                }

                var order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();
                if (order != null)
                {
                    var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == model.OrderId && m.ShopMode == E_ShopMode.Machine && m.MachineId == machine.Id).ToList();

                    //是否触发过取货
                    if (order.PickupTrgTime == null)
                    {
                        order.PickupIsTrg = true;
                        order.PickupTrgTime = DateTime.Now;
                        order.PickupFlowLastDesc = "商品取货，正在出货";
                        order.PickupFlowLastTime = DateTime.Now;

                        int timoutM = order.Quantity * 5;

                        Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPickupTimeout, order.Id, DateTime.Now.AddMinutes(timoutM), new OrderSub2CheckPickupTimeoutModel { OrderId = order.Id, MachineId = order.MachineId });
                    }

                    var orderPickupLog = new OrderPickupLog();
                    orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                    orderPickupLog.OrderId = model.OrderId;
                    orderPickupLog.MerchId = order.MerchId;
                    orderPickupLog.ShopMode = E_ShopMode.Machine;
                    orderPickupLog.StoreId = order.StoreId;
                    orderPickupLog.ShopId = order.ShopId;
                    orderPickupLog.MachineId = order.MachineId;
                    orderPickupLog.UniqueId = model.UniqueId;
                    orderPickupLog.UniqueType = E_UniqueType.OrderSub;
                    orderPickupLog.PrdProductSkuId = model.ProductSkuId;
                    orderPickupLog.CabinetId = model.CabinetId;
                    orderPickupLog.SlotId = model.SlotId;
                    orderPickupLog.Status = model.PickupStatus;
                    orderPickupLog.ActionId = model.ActionId;
                    orderPickupLog.ActionName = model.ActionName;
                    orderPickupLog.ActionStatusCode = model.ActionStatusCode;
                    orderPickupLog.ActionStatusName = model.ActionStatusName;
                    orderPickupLog.ImgId = model.ImgId;
                    orderPickupLog.ImgId2 = model.ImgId2;
                    orderPickupLog.PickupUseTime = model.PickupUseTime;
                    orderPickupLog.ActionRemark = remark.ToString();
                    orderPickupLog.Remark = model.Remark;
                    orderPickupLog.CreateTime = DateTime.Now;
                    orderPickupLog.Creator = operater;
                    CurrentDb.OrderPickupLog.Add(orderPickupLog);

                    if (model.PickupStatus == E_OrderPickupStatus.Exception)
                    {

                        order.ExIsHappen = true;
                        order.ExHappenTime = DateTime.Now;

                        List<ImgSet> exImgUrls = new List<ImgSet>();

                        if (!string.IsNullOrEmpty(model.ImgId))
                        {
                            exImgUrls.Add(new ImgSet { Url = BizFactory.Order.GetPickImgUrl(model.ImgId) });
                        }

                        if (!string.IsNullOrEmpty(model.ImgId2))
                        {
                            exImgUrls.Add(new ImgSet { Url = BizFactory.Order.GetPickImgUrl(model.ImgId2) });
                        }

                        if (exImgUrls.Count > 0)
                        {
                            order.ExImgUrls = exImgUrls.ToJsonString();
                        }

                        order.PickupFlowLastDesc = "取货动作发生异常";
                        order.PickupFlowLastTime = DateTime.Now;

                        foreach (var orderSub in orderSubs)
                        {
                            if (orderSub.PayStatus == E_PayStatus.PaySuccess)
                            {
                                if (orderSub.PickupStatus != E_OrderPickupStatus.Taked
                                    && orderSub.PickupStatus != E_OrderPickupStatus.Exception
                                    && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked
                                    && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                {
                                    orderSub.PickupStatus = E_OrderPickupStatus.Exception;
                                    orderSub.ExPickupReason = "取货动作发生异常";
                                    orderSub.ExPickupIsHappen = true;
                                    orderSub.ExPickupHappenTime = DateTime.Now;
                                }
                            }

                        }

                        machine.ExIsHas = true;
                        machine.ExReason = "取货动作发生异常";

                        Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, order.Id);
                    }
                    else
                    {

                        foreach (var orderSub in orderSubs)
                        {
                            if (orderSub.Id == model.UniqueId)
                            {
                                orderSub.PickupFlowLastDesc = model.ActionName + model.ActionStatusName;
                                orderSub.PickupFlowLastTime = DateTime.Now;

                                if (model.PickupStatus == E_OrderPickupStatus.Taked)
                                {
                                    if (orderSub.PickupStatus != E_OrderPickupStatus.Taked && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && orderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPickupOneSysMadeSignTake, orderSub.ShopMode, orderSub.MerchId, orderSub.StoreId, orderSub.ShopId, orderSub.MachineId, orderSub.CabinetId, orderSub.SlotId, orderSub.PrdProductSkuId, 1);
                                    }

                                    if (orderSub.PickupEndTime == null)
                                    {
                                        orderSub.PickupEndTime = DateTime.Now;
                                    }

                                }

                                if (orderSub.PickupStartTime == null)
                                {
                                    orderSub.PickupStartTime = DateTime.Now;
                                }


                                orderSub.PickupStatus = model.PickupStatus;
                                CurrentDb.SaveChanges();
                            }
                        }

                        var orderDetailsChildSonsCompeleteCount = orderSubs.Where(m => m.PickupStatus == E_OrderPickupStatus.Taked).Count();
                        //判断全部订单都是已完成
                        if (orderDetailsChildSonsCompeleteCount == orderSubs.Count)
                        {
                            order.PickupFlowLastDesc = "全部商品出货完成";
                            order.PickupFlowLastTime = DateTime.Now;
                            order.Status = E_OrderStatus.Completed;
                            order.CompletedTime = DateTime.Now;

                            Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, order.Id);
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();


                MqFactory.Global.PushOperateLog(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, machineId, EventCode.Pickup, remark.ToString());
            }
        }

        private void HandleByPickupTest(string operater, string appId, string trgerId, string eventCode, string eventRemark, MachineEventByPickupModel model)
        {
            if (model == null)
                return;

            string machineId = trgerId;
            var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();

            if (machine == null)
                return;

            machine.LastRequestTime = DateTime.Now;
            CurrentDb.SaveChanges();


            StringBuilder remark = new StringBuilder("");
            string productSkuName = "[测试]";
            var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(machine.CurUseMerchId, model.ProductSkuId);
            if (bizProductSku != null)
            {
                productSkuName += bizProductSku.Name;
            }

            if (model.PickupStatus == E_OrderPickupStatus.SendPickupCmd)
            {
                remark.Append("发送命令");
            }
            else if (model.PickupStatus == E_OrderPickupStatus.Taked)
            {
                remark.Append(string.Format("取货完成，用时：{0}", model.PickupUseTime));
            }
            else if (model.PickupStatus == E_OrderPickupStatus.Exception)
            {
                remark.Append(string.Format("发生异常，原因：{0}", model.Remark));
            }
            else
            {
                remark.Append(string.Format("当前动作：{0}，状态：{1}", model.ActionName, model.ActionStatusName));
            }

            MqFactory.Global.PushOperateLog(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, machineId, EventCode.PickupTest, remark.ToString());

        }

        private void HandleByStockChangeLog(string operater, string appId, string trgerId, string eventCode, string eventRemark, SellChannelStockChangeModel model)
        {
            string merchName = BizFactory.Merch.GetMerchName(model.MerchId);
            string storeName = BizFactory.Merch.GetStoreName(model.MerchId, model.StoreId);
            string operaterUserName = BizFactory.Merch.GetClientName(model.MerchId, operater);

            var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(model.MerchId, model.PrdProductSkuId);

            var sellChannelStockLog = new SellChannelStockLog();
            sellChannelStockLog.Id = IdWorker.Build(IdType.NewGuid);
            sellChannelStockLog.MerchId = model.MerchId;
            sellChannelStockLog.MerchName = merchName;
            sellChannelStockLog.StoreId = model.StoreId;
            sellChannelStockLog.StoreName = storeName;
            sellChannelStockLog.ShopId = model.ShopId;
            sellChannelStockLog.MachineId = model.MachineId;
            sellChannelStockLog.ShopMode = model.ShopMode;
            sellChannelStockLog.CabinetId = model.CabinetId;
            sellChannelStockLog.SlotId = model.SlotId;
            sellChannelStockLog.PrdProductId = bizProductSku.ProductId;
            sellChannelStockLog.PrdProductSkuId = model.PrdProductSkuId;
            sellChannelStockLog.PrdProductSkuName = bizProductSku.Name;
            sellChannelStockLog.SellQuantity = model.SellQuantity;
            sellChannelStockLog.WaitPayLockQuantity = model.WaitPayLockQuantity;
            sellChannelStockLog.WaitPickupLockQuantity = model.WaitPickupLockQuantity;
            sellChannelStockLog.SumQuantity = model.SumQuantity;
            sellChannelStockLog.EventCode = model.EventCode;
            sellChannelStockLog.EventName = EventCode.GetEventName(model.EventCode);
            sellChannelStockLog.ChangeQuantity = model.ChangeQuantity;
            sellChannelStockLog.Creator = operater;
            sellChannelStockLog.CreateTime = DateTime.Now;

            if (string.IsNullOrEmpty(model.MachineId))
            {
                sellChannelStockLog.Remark = eventRemark;
            }
            else
            {
                sellChannelStockLog.Remark = string.Format("店铺：{0}，机器：{1}，{2}", storeName, model.MachineId, eventRemark);
            }

            CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
            CurrentDb.SaveChanges();

        }
    }
}
