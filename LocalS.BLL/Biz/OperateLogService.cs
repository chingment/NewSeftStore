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
    public class OperateLogService : BaseDbContext
    {
        public void EventNotifyHandle(EventNotifyModel model)
        {


            switch (model.EventCode)
            {
                case EventCode.Login:
                    var loginLogModel = model.EventContent.ToJsonObject<LoginLogModel>();
                    EventHandleByLogin(model.Operater, model.AppId, model.MerchId, model.StoreId, model.MachineId, model.EventCode, model.EventRemark, loginLogModel);
                    break;
                case EventCode.Logout:
                    var logoutLogModel = model.EventContent.ToJsonObject<LoginLogModel>();
                    EventHandleByLogout(model.Operater, model.AppId, model.MerchId, model.StoreId, model.MachineId, model.EventCode, model.EventRemark, logoutLogModel);
                    break;
                case EventCode.HeartbeatBag:
                    var heartbeatBagModel = model.EventContent.ToJsonObject<MachineEventByHeartbeatBagModel>();
                    EventHandleByHeartbeatBag(model.Operater, model.AppId, model.MerchId, model.StoreId, model.MachineId, model.EventCode, model.EventRemark, heartbeatBagModel);
                    break;
                case EventCode.Pickup:
                    var pickupModel = model.EventContent.ToJsonObject<MachineEventByPickupModel>();
                    EventHandleByPickup(model.Operater, model.AppId, model.MerchId, model.StoreId, model.MachineId, model.EventCode, model.EventRemark, pickupModel);
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
                    EventHandleByStockChangeLog(model.Operater, model.AppId, model.MerchId, model.StoreId, model.MachineId, model.EventCode, model.EventRemark, sellChannelStockChangeModel);
                    break;
                default:
                    EventHandle(model.Operater, model.AppId, model.MerchId, model.StoreId, model.MachineId, model.EventCode, model.EventRemark);
                    break;
            }
        }

        private void EventHandle(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark)
        {
            string merchName = BizFactory.Merch.GetMerchName(merchId);
            string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
            string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
            string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

            if (!string.IsNullOrEmpty(operater) && operater != IdWorker.Build(IdType.EmptyGuid))
            {
                var sysUserOperateLog = new SysUserOperateLog();
                sysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
                sysUserOperateLog.UserId = operater;
                sysUserOperateLog.EventCode = eventCode;
                sysUserOperateLog.EventName = EventCode.GetEventName(eventCode);
                sysUserOperateLog.AppId = appId;
                sysUserOperateLog.Remark = eventRemark;
                sysUserOperateLog.CreateTime = DateTime.Now;
                sysUserOperateLog.Creator = operater;
                CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
                CurrentDb.SaveChanges();
            }

            var merchOperateLog = new MerchOperateLog();
            merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            merchOperateLog.AppId = appId;
            merchOperateLog.MerchId = merchId;
            merchOperateLog.MerchName = merchName;
            merchOperateLog.StoreId = storeId;
            merchOperateLog.StoreName = storeName;
            merchOperateLog.MachineId = machineId;
            merchOperateLog.MachineName = machineName;
            merchOperateLog.OperateUserId = operater;
            merchOperateLog.OperateUserName = operaterUserName;
            merchOperateLog.EventCode = eventCode;
            merchOperateLog.EventName = EventCode.GetEventName(eventCode);
            merchOperateLog.EventLevel = EventCode.GetEventLevel(eventCode);
            if (string.IsNullOrEmpty(machineId))
            {
                merchOperateLog.Remark = eventRemark;
            }
            else
            {
                merchOperateLog.Remark = string.Format("店铺：{0}，机器：{1}，{2}", storeName, machineName, eventRemark);
            }
            merchOperateLog.Creator = operater;
            merchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(merchOperateLog);
            CurrentDb.SaveChanges();

        }

        private void EventHandleByLogin(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, LoginLogModel model)
        {
            string merchName = BizFactory.Merch.GetMerchName(merchId);
            string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
            string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
            string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

            var sysUserOperateLog = new SysUserOperateLog();
            sysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            sysUserOperateLog.UserId = operater;
            sysUserOperateLog.EventCode = eventCode;
            sysUserOperateLog.EventName = EventCode.GetEventName(eventCode);
            sysUserOperateLog.AppId = appId;
            sysUserOperateLog.Remark = eventRemark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
            CurrentDb.SaveChanges();

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

            var merchOperateLog = new MerchOperateLog();
            merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            merchOperateLog.AppId = appId;
            merchOperateLog.MerchId = merchId;
            merchOperateLog.MerchName = merchName;
            merchOperateLog.StoreId = storeId;
            merchOperateLog.StoreName = storeName;
            merchOperateLog.MachineId = machineId;
            merchOperateLog.MachineName = machineName;
            merchOperateLog.OperateUserId = operater;
            merchOperateLog.OperateUserName = operaterUserName;
            merchOperateLog.EventCode = eventCode;
            merchOperateLog.EventLevel = EventCode.GetEventLevel(eventCode);
            merchOperateLog.EventName = EventCode.GetEventName(eventCode);

            if (string.IsNullOrEmpty(machineId))
            {
                merchOperateLog.Remark = string.Format("账号：{0}，{1}，进入站点：{2}", model.LoginAccount, eventRemark, appId);
            }
            else
            {
                merchOperateLog.Remark = string.Format("账号：{0}，{1}，进入店铺：{2}，机器：{3}", model.LoginAccount, eventRemark, storeName, machineName);
            }

            merchOperateLog.Creator = operater;
            merchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(merchOperateLog);
            CurrentDb.SaveChanges();
        }

        private void EventHandleByLogout(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, LoginLogModel model)
        {
            string merchName = BizFactory.Merch.GetMerchName(merchId);
            string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
            string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
            string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

            var sysUserOperateLog = new SysUserOperateLog();
            sysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            sysUserOperateLog.UserId = operater;
            sysUserOperateLog.EventCode = eventCode;
            sysUserOperateLog.EventName = EventCode.GetEventName(eventCode);
            sysUserOperateLog.AppId = appId;
            sysUserOperateLog.Remark = eventRemark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
            CurrentDb.SaveChanges();

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

            var merchOperateLog = new MerchOperateLog();
            merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            merchOperateLog.AppId = appId;
            merchOperateLog.MerchId = merchId;
            merchOperateLog.MerchName = merchName;
            merchOperateLog.StoreId = storeId;
            merchOperateLog.StoreName = storeName;
            merchOperateLog.MachineId = machineId;
            merchOperateLog.MachineName = machineName;
            merchOperateLog.OperateUserId = operater;
            merchOperateLog.OperateUserName = operaterUserName;
            merchOperateLog.EventCode = eventCode;
            merchOperateLog.EventLevel = EventCode.GetEventLevel(eventCode);
            merchOperateLog.EventName = EventCode.GetEventName(eventCode);
            merchOperateLog.Remark = string.Format("账号：{0}，{1}", model.LoginAccount, eventRemark);
            merchOperateLog.Creator = operater;
            merchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(merchOperateLog);
            CurrentDb.SaveChanges();
        }

        private void EventHandleByHeartbeatBag(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, MachineEventByHeartbeatBagModel model)
        {
            string merchName = BizFactory.Merch.GetMerchName(merchId);
            string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
            string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
            string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

            var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();

            if (machine == null)
                return;

            machine.LastRequestTime = DateTime.Now;

            bool isLog = false;
            switch (model.Status)
            {
                case "running":
                    eventRemark = string.Format("店铺：{0}，机器：{1}，运行正常", storeName, machineName);

                    if (machine.RunStatus != E_MachineRunStatus.Running)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Running;
                    break;
                case "setting":
                    eventRemark = string.Format("店铺：{0}，机器：{1}，维护中", storeName, machineName);

                    if (machine.RunStatus != E_MachineRunStatus.Setting)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Setting;
                    break;
                default:
                    eventRemark = string.Format("店铺：{0}，机器：{1}，未知状态", storeName, machineName);
                    break;
            }

            if (isLog)
            {
                var merchOperateLog = new MerchOperateLog();
                merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
                merchOperateLog.AppId = appId;
                merchOperateLog.MerchId = merchId;
                merchOperateLog.MerchName = merchName;
                merchOperateLog.StoreId = storeId;
                merchOperateLog.StoreName = storeName;
                merchOperateLog.MachineId = machineId;
                merchOperateLog.MachineName = machineName;
                merchOperateLog.OperateUserId = operater;
                merchOperateLog.OperateUserName = operaterUserName;
                merchOperateLog.EventCode = eventCode;
                merchOperateLog.EventLevel = EventCode.GetEventLevel(eventCode);
                merchOperateLog.EventName = EventCode.GetEventName(eventCode);
                merchOperateLog.Remark = eventRemark;
                merchOperateLog.Creator = operater;
                merchOperateLog.CreateTime = DateTime.Now;
                CurrentDb.MerchOperateLog.Add(merchOperateLog);
                CurrentDb.SaveChanges();
            }
        }

        private void EventHandleByPickup(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, MachineEventByPickupModel model)
        {
            if (model == null)
                return;

            using (TransactionScope ts = new TransactionScope())
            {
                string merchName = BizFactory.Merch.GetMerchName(merchId);
                string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
                string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
                string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

                var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();


                machine.LastRequestTime = DateTime.Now;

                StringBuilder remark = new StringBuilder("");
                string eventLevel = "A";
                string productSkuName = "";
                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(machine.CurUseMerchId, model.ProductSkuId);
                if (model.IsTest)
                {
                    productSkuName = "[测试]" + bizProductSku.Name;
                }

                if (model.Status == E_OrderPickupStatus.SendPickupCmd)
                {
                    eventLevel = "A";
                    remark.Append("发送命令");
                }
                else if (model.Status == E_OrderPickupStatus.Exception)
                {
                    eventLevel = "A";
                    remark.Append(string.Format("发生异常，原因：{0}", model.Remark));
                }
                else
                {
                    eventLevel = "D";
                    remark.Append(string.Format("当前动作：{0}，状态：{1}", model.ActionName, model.ActionStatusName));

                    if (model.IsPickupComplete)
                    {
                        eventLevel = "A";
                        remark.Append(string.Format("，取货完成，用时：{0}", model.PickupUseTime));
                    }
                }

                if (model.IsTest)
                {
                    eventLevel = "D";//测试全改为D
                }
                else
                {
                    var order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();
                    var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == model.OrderId && m.SellChannelRefId == machine.Id).FirstOrDefault();
                    var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == model.OrderId).ToList();

                    //是否触发过取货
                    if (orderSub.PickupTrgTime == null)
                    {
                        orderSub.PickupIsTrg = true;
                        orderSub.PickupTrgTime = DateTime.Now;
                        orderSub.PickupFlowLastDesc = "您在机器录入取货码，正在出货";
                        orderSub.PickupFlowLastTime = DateTime.Now;

                        int timoutM = orderSub.Quantity * 5;

                        Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPickupTimeout, orderSub.Id, DateTime.Now.AddMinutes(timoutM), new OrderSub2CheckPickupTimeoutModel { OrderId = orderSub.OrderId, OrderSubId = orderSub.Id, MachineId = orderSub.SellChannelRefId });


                        var l_orderPickupLog = new OrderPickupLog();
                        l_orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                        l_orderPickupLog.OrderId = order.Id;
                        l_orderPickupLog.OrderSubId = orderSub.Id;
                        l_orderPickupLog.SellChannelRefType = orderSub.SellChannelRefType;
                        l_orderPickupLog.SellChannelRefId = orderSub.SellChannelRefId;
                        l_orderPickupLog.UniqueId = orderSub.Id;
                        l_orderPickupLog.ActionRemark = orderSub.PickupFlowLastDesc;
                        l_orderPickupLog.ActionTime = orderSub.PickupFlowLastTime;
                        l_orderPickupLog.Remark = "";
                        l_orderPickupLog.CreateTime = DateTime.Now;
                        l_orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(l_orderPickupLog);

                    }

                    var orderPickupLog = new OrderPickupLog();
                    orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                    orderPickupLog.OrderId = model.OrderId;
                    orderPickupLog.OrderSubId = orderSub.Id;
                    orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                    orderPickupLog.SellChannelRefId = machineId;
                    orderPickupLog.UniqueId = model.UniqueId;
                    orderPickupLog.PrdProductSkuId = model.ProductSkuId;
                    orderPickupLog.CabinetId = model.CabinetId;
                    orderPickupLog.SlotId = model.SlotId;
                    orderPickupLog.Status = model.Status;
                    orderPickupLog.ActionId = model.ActionId;
                    orderPickupLog.ActionName = model.ActionName;
                    orderPickupLog.ActionStatusCode = model.ActionStatusCode;
                    orderPickupLog.ActionStatusName = model.ActionStatusName;
                    orderPickupLog.IsPickupComplete = model.IsPickupComplete;
                    orderPickupLog.ImgId = model.ImgId;
                    orderPickupLog.ImgId2 = model.ImgId2;

                    if (model.IsPickupComplete)
                    {
                        orderPickupLog.PickupUseTime = model.PickupUseTime;
                        orderPickupLog.ActionRemark = "取货完成";
                    }
                    else
                    {
                        if (model.Status == E_OrderPickupStatus.SendPickupCmd)
                        {
                            orderPickupLog.ActionRemark = "发送命令";
                        }
                        else if (model.Status == E_OrderPickupStatus.Exception)
                        {
                            orderPickupLog.ActionRemark = "发生异常";
                        }
                        else
                        {
                            orderPickupLog.ActionRemark = model.ActionName + model.ActionStatusName;
                        }
                    }

                    orderPickupLog.Remark = model.Remark;
                    orderPickupLog.CreateTime = DateTime.Now;
                    orderPickupLog.Creator = operater;
                    CurrentDb.OrderPickupLog.Add(orderPickupLog);


                    if (model.Status == E_OrderPickupStatus.Exception)
                    {

                        orderSub.ExIsHappen = true;
                        orderSub.ExHappenTime = DateTime.Now;
                        orderSub.PickupFlowLastDesc = "取货发生异常";
                        orderSub.PickupFlowLastTime = DateTime.Now;


                        foreach (var orderSubChild in orderSubChilds)
                        {
                            if (model.Status == E_OrderPickupStatus.Exception)
                            {
                                if (orderSubChild.PayStatus == E_OrderPayStatus.PaySuccess)
                                {
                                    if (orderSubChild.PickupStatus != E_OrderPickupStatus.Taked
                                        && orderSubChild.PickupStatus != E_OrderPickupStatus.Exception
                                        && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked
                                        && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        orderSubChild.PickupStatus = E_OrderPickupStatus.Exception;
                                        orderSubChild.ExPickupIsHappen = true;
                                        orderSubChild.ExPickupHappenTime = DateTime.Now;
                                    }
                                }
                            }
                        }

                        machine.ExIsHas = true;

                        Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, orderSub.Id);
                    }
                    else
                    {

                        foreach (var orderSubChild in orderSubChilds)
                        {
                            if (orderSubChild.Id == model.UniqueId)
                            {
                                orderSubChild.PickupFlowLastDesc = model.ActionName + model.ActionStatusName;
                                orderSubChild.PickupFlowLastTime = DateTime.Now;

                                if (model.Status == E_OrderPickupStatus.Taked)
                                {
                                    if (orderSubChild.PickupStatus != E_OrderPickupStatus.Taked && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPickupOneSysMadeSignTake, appId, orderSubChild.MerchId, orderSubChild.StoreId, orderSubChild.SellChannelRefId, orderSubChild.CabinetId, orderSubChild.SlotId, orderSubChild.PrdProductSkuId, 1);
                                    }
                                }

                                if (orderSubChild.PickupStartTime == null)
                                {
                                    orderSubChild.PickupStartTime = DateTime.Now;
                                }

                                if (model.IsPickupComplete)
                                {
                                    if (orderSubChild.PickupEndTime == null)
                                    {
                                        orderSubChild.PickupEndTime = DateTime.Now;
                                    }
                                }

                                orderSubChild.PickupStatus = model.Status;
                            }
                        }

                        var orderDetailsChildSonsCompeleteCount = orderSubChilds.Where(m => m.PickupStatus == E_OrderPickupStatus.Taked).Count();
                        //判断全部订单都是已完成
                        if (orderDetailsChildSonsCompeleteCount == orderSubChilds.Count)
                        {
                            order.Status = E_OrderStatus.Completed;
                            order.CompletedTime = DateTime.Now;
  

                            orderSub.PickupFlowLastDesc = "全部商品出货完成";
                            orderSub.PickupFlowLastTime = DateTime.Now;
                           

                            Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, orderSub.Id);
                        }
                    }
                }

                var merchOperateLog = new MerchOperateLog();
                merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
                merchOperateLog.AppId = appId;
                merchOperateLog.MerchId = merchId;
                merchOperateLog.MerchName = merchName;
                merchOperateLog.StoreId = storeId;
                merchOperateLog.StoreName = storeName;
                merchOperateLog.MachineId = machineId;
                merchOperateLog.MachineName = machineName;
                merchOperateLog.OperateUserId = operater;
                merchOperateLog.OperateUserName = operaterUserName;
                merchOperateLog.EventCode = eventCode;
                merchOperateLog.EventLevel = eventLevel;
                merchOperateLog.EventName = EventCode.GetEventName(eventCode);
                merchOperateLog.Remark = string.Format("店铺：{0}，机器：{1}，机柜：{2}，货道：{3}，商品：{4}，{5}", storeName, machineName, model.CabinetId, model.SlotId, productSkuName, remark.ToString());
                merchOperateLog.Creator = operater;
                merchOperateLog.CreateTime = DateTime.Now;
                CurrentDb.MerchOperateLog.Add(merchOperateLog);
                CurrentDb.SaveChanges();

                ts.Complete();
            }
        }

        private void EventHandleByStockChangeLog(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, SellChannelStockChangeModel model)
        {
            string merchName = BizFactory.Merch.GetMerchName(merchId);
            string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
            string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
            string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);
            if (!string.IsNullOrEmpty(operater) && operater != IdWorker.Build(IdType.EmptyGuid))
            {
                var sysUserOperateLog = new SysUserOperateLog();
                sysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
                sysUserOperateLog.UserId = operater;
                sysUserOperateLog.EventCode = eventCode;
                sysUserOperateLog.EventName = EventCode.GetEventName(eventCode);
                sysUserOperateLog.AppId = appId;
                sysUserOperateLog.Remark = eventRemark;
                sysUserOperateLog.CreateTime = DateTime.Now;
                sysUserOperateLog.Creator = operater;
                CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
                CurrentDb.SaveChanges();
            }

            var merchOperateLog = new MerchOperateLog();
            merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            merchOperateLog.AppId = appId;
            merchOperateLog.MerchId = merchId;
            merchOperateLog.MerchName = merchName;
            merchOperateLog.StoreId = storeId;
            merchOperateLog.StoreName = storeName;
            merchOperateLog.MachineId = machineId;
            merchOperateLog.MachineName = machineName;
            merchOperateLog.OperateUserId = operater;
            merchOperateLog.OperateUserName = operaterUserName;
            merchOperateLog.EventCode = eventCode;
            merchOperateLog.EventLevel = EventCode.GetEventLevel(eventCode);
            merchOperateLog.EventName = EventCode.GetEventName(eventCode);
            if (string.IsNullOrEmpty(machineId))
            {
                merchOperateLog.Remark = eventRemark;
            }
            else
            {
                merchOperateLog.Remark = string.Format("店铺：{0}，机器：{1}，{2}", storeName, machineName, eventRemark);
            }
            merchOperateLog.Creator = operater;
            merchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(merchOperateLog);
            CurrentDb.SaveChanges();

            if (model != null)
            {
                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, model.PrdProductSkuId);

                var sellChannelStockLog = new SellChannelStockLog();
                sellChannelStockLog.Id = IdWorker.Build(IdType.NewGuid);
                sellChannelStockLog.MerchId = merchId;
                sellChannelStockLog.MerchName = merchName;
                sellChannelStockLog.StoreId = storeId;
                sellChannelStockLog.StoreName = storeName;
                sellChannelStockLog.SellChannelRefId = model.SellChannelRefId;
                sellChannelStockLog.SellChannelRefName = machineName;
                sellChannelStockLog.SellChannelRefType = model.SellChannelRefType;
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
                if (string.IsNullOrEmpty(machineId))
                {
                    sellChannelStockLog.Remark = eventRemark;
                }
                else
                {
                    sellChannelStockLog.Remark = string.Format("店铺：{0}，机器：{1}，{2}", storeName, machineName, eventRemark);
                }
                CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                CurrentDb.SaveChanges();
            }
        }

    }
}
