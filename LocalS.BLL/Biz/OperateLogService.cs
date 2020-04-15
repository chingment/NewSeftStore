﻿using LocalS.BLL.Mq;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
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
                default:
                    EventHandle(model.Operater, model.AppId, model.MerchId, model.StoreId, model.MachineId, model.EventCode, model.EventRemark);
                    break;
            }
        }

        private void EventHandle(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string merchName = BizFactory.Merch.GetMerchName(merchId);
                string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
                string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
                string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

                if (!string.IsNullOrEmpty(operater) && operater != GuidUtil.Empty())
                {
                    var sysUserOperateLog = new SysUserOperateLog();
                    sysUserOperateLog.Id = GuidUtil.New();
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
                merchOperateLog.Id = GuidUtil.New();
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

                ts.Complete();
            }
        }

        private void EventHandleByLogin(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, LoginLogModel model)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string merchName = BizFactory.Merch.GetMerchName(merchId);
                string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
                string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
                string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

                var sysUserOperateLog = new SysUserOperateLog();
                sysUserOperateLog.Id = GuidUtil.New();
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
                userLoginHis.Id = GuidUtil.New();
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
                merchOperateLog.Id = GuidUtil.New();
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

                ts.Complete();
            }
        }

        private void EventHandleByLogout(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, LoginLogModel model)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string merchName = BizFactory.Merch.GetMerchName(merchId);
                string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
                string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
                string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

                var sysUserOperateLog = new SysUserOperateLog();
                sysUserOperateLog.Id = GuidUtil.New();
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
                userLoginHis.Id = GuidUtil.New();
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
                merchOperateLog.Id = GuidUtil.New();
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
                merchOperateLog.Remark = string.Format("账号：{0}，{1}", model.LoginAccount, eventRemark);
                merchOperateLog.Creator = operater;
                merchOperateLog.CreateTime = DateTime.Now;
                CurrentDb.MerchOperateLog.Add(merchOperateLog);
                CurrentDb.SaveChanges();

                ts.Complete();
            }
        }

        private void EventHandleByHeartbeatBag(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, MachineEventByHeartbeatBagModel model)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string merchName = BizFactory.Merch.GetMerchName(merchId);
                string storeName = BizFactory.Merch.GetStoreName(merchId, storeId);
                string machineName = BizFactory.Merch.GetMachineName(merchId, machineId);
                string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

                var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();

                if (machine == null)
                    return;

                machine.LastRequestTime = DateTime.Now;

                switch (model.Status)
                {
                    case "running":
                        eventRemark = string.Format("店铺：{0}，机器：{1}，运行正常", storeName, machineName);
                        machine.RunStatus = E_MachineRunStatus.Running;
                        break;
                    case "setting":
                        eventRemark = string.Format("店铺：{0}，机器：{1}，维护中", storeName, machineName);
                        machine.RunStatus = E_MachineRunStatus.Setting;
                        break;
                    default:
                        eventRemark = string.Format("店铺：{0}，机器：{1}，未知状态", storeName, machineName);
                        break;
                }

                var merchOperateLog = new MerchOperateLog();
                merchOperateLog.Id = GuidUtil.New();
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
                merchOperateLog.Remark = eventRemark;
                merchOperateLog.Creator = operater;
                merchOperateLog.CreateTime = DateTime.Now;
                CurrentDb.MerchOperateLog.Add(merchOperateLog);
                CurrentDb.SaveChanges();
                ts.Complete();
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

                string productSkuName = "";
                var bizProduct = CacheServiceFactory.ProductSku.GetInfo(machine.CurUseMerchId, model.ProductSkuId);
                if (bizProduct == null)
                {
                    productSkuName = "无";
                }
                else
                {
                    productSkuName = bizProduct.Name;
                }

                if (model.IsTest)
                {
                    productSkuName = "[测试]" + productSkuName;
                }

                if (model.Status == E_OrderPickupStatus.SendPickupCmd)
                {
                    remark.Append("发送命令");
                }
                else if (model.Status == E_OrderPickupStatus.Exception)
                {
                    remark.Append(string.Format("发生异常，原因：{0}", model.Remark));
                }
                else
                {
                    remark.Append(string.Format("当前动作：{0}，状态：{1}", model.ActionName, model.ActionStatusName));

                    if (model.IsPickupComplete)
                    {
                        remark.Append(string.Format("，取货完成，用时：{0}", model.PickupUseTime));
                    }
                }


                if (!model.IsTest)
                {
                    var order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();
                    var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == model.OrderId && m.SellChannelRefId == machine.Id && m.SellChannelRefType == E_SellChannelRefType.Machine).FirstOrDefault();
                    var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == model.OrderId).ToList();

                    //是否触发过取货
                    if (orderSub.PickupTrgTime == null)
                    {
                        orderSub.PickupIsTrg = true;
                        orderSub.PickupTrgTime = DateTime.Now;

                        int timoutM = orderSub.Quantity * 5;

                        Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPickupTimeout, orderSub.Id, DateTime.Now.AddMinutes(timoutM), new OrderSub2CheckPickupTimeoutModel { OrderId = orderSub.OrderId, OrderSubId = orderSub.Id, MachineId = orderSub.SellChannelRefId });
                    }

                    foreach (var orderSubChildUnique in orderSubChildUniques)
                    {
                        if (model.Status == E_OrderPickupStatus.Exception)
                        {
                            if (orderSubChildUnique.PayStatus == E_OrderPayStatus.PaySuccess)
                            {
                                if (orderSubChildUnique.PickupStatus != E_OrderPickupStatus.Taked
                                    && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.Exception
                                    && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked
                                    && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                {
                                    orderSubChildUnique.PickupStatus = E_OrderPickupStatus.Exception;
                                    orderSubChildUnique.ExPickupIsHappen = true;
                                    orderSubChildUnique.ExPickupHappenTime = DateTime.Now;
                                }
                            }
                        }
                        else
                        {
                            if (orderSubChildUnique.Id == model.UniqueId)
                            {
                                orderSubChildUnique.LastPickupActionId = model.ActionId;
                                orderSubChildUnique.LastPickupActionStatusCode = model.ActionStatusCode;

                                if (model.Status == E_OrderPickupStatus.Taked)
                                {
                                    if (orderSubChildUnique.PickupStatus != E_OrderPickupStatus.Taked && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        BizFactory.ProductSku.OperateStockQuantity(machineId, OperateStockType.OrderPickupOneSysMadeSignTake, orderSubChildUnique.MerchId, orderSubChildUnique.StoreId, orderSubChildUnique.SellChannelRefId, orderSubChildUnique.CabinetId, orderSubChildUnique.SlotId, orderSubChildUnique.PrdProductSkuId, 1);
                                    }
                                }

                                if (orderSubChildUnique.PickupStartTime == null)
                                {
                                    orderSubChildUnique.PickupStartTime = DateTime.Now;
                                }

                                if (model.IsPickupComplete)
                                {
                                    if (orderSubChildUnique.PickupEndTime == null)
                                    {
                                        orderSubChildUnique.PickupEndTime = DateTime.Now;
                                    }
                                }

                                orderSubChildUnique.PickupStatus = model.Status;
                            }
                        }
                    }

                    if (model.Status == E_OrderPickupStatus.Exception)
                    {
                        order.ExIsHappen = true;
                        order.ExHappenTime = DateTime.Now;

                        orderSub.ExIsHappen = true;
                        orderSub.ExHappenTime = DateTime.Now;

                        machine.ExIsHas = true;

                        Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, orderSub.Id);
                    }
                    else
                    {
                        var orderDetailsChildSonsCompeleteCount = orderSubChildUniques.Where(m => m.PickupStatus == E_OrderPickupStatus.Taked).Count();
                        //判断全部订单都是已完成
                        if (orderDetailsChildSonsCompeleteCount == orderSubChildUniques.Count)
                        {
                            order.Status = E_OrderStatus.Completed;
                            order.CompletedTime = DateTime.Now;
                            order.ExIsHappen = false;
                            Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, orderSub.Id);
                        }
                    }

                    var orderPickupLog = new OrderPickupLog();
                    orderPickupLog.Id = GuidUtil.New();
                    orderPickupLog.OrderId = model.OrderId;
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
                    orderPickupLog.Creator = machineId;
                    CurrentDb.OrderPickupLog.Add(orderPickupLog);
                }

                var merchOperateLog = new MerchOperateLog();
                merchOperateLog.Id = GuidUtil.New();
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
                merchOperateLog.Remark = string.Format("店铺：{0}，机器：{1}，机柜：{2}，货道：{3}，商品：{4}，{5}", storeName, machineName, model.CabinetId, model.SlotId, remark.ToString());
                merchOperateLog.Creator = operater;
                merchOperateLog.CreateTime = DateTime.Now;
                CurrentDb.MerchOperateLog.Add(merchOperateLog);
                CurrentDb.SaveChanges();

                ts.Complete();
            }
        }
    }
}
