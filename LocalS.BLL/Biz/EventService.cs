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
                    var pickupTestModel = model.EventContent.ToJsonObject<MachineEventByPickupTestModel>();
                    HandleByPickupTest(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, pickupTestModel);
                    break;
                case EventCode.MachineCabinetSlotSave:
                case EventCode.MachineCabinetSlotRemove:
                case EventCode.MachineCabinetSlotAdjustStockQuantity:
                    //var sellChannelStockChangeModel = model.EventContent.ToJsonObject<SellChannelStockChangeModel>();
                    //HandleByStockChangeLog(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, sellChannelStockChangeModel);
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
            userLoginHis.Remark = model.Remark;
            userLoginHis.CreateTime = DateTime.Now;
            userLoginHis.Creator = operater;
            CurrentDb.SysUserLoginHis.Add(userLoginHis);
            CurrentDb.SaveChanges();

            if (appId == AppId.MERCH || appId == AppId.STORETERM || appId == AppId.WXMINPRAGROM)
            {
                MqFactory.Global.PushOperateLog(operater, appId, trgerId, EventCode.Login, eventRemark, model);
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
            userLoginHis.Remark = model.Remark;
            userLoginHis.CreateTime = DateTime.Now;
            userLoginHis.Creator = operater;
            CurrentDb.SysUserLoginHis.Add(userLoginHis);
            CurrentDb.SaveChanges();


            if (appId == AppId.MERCH || appId == AppId.STORETERM || appId == AppId.WXMINPRAGROM)
            {
                MqFactory.Global.PushOperateLog(operater, appId, trgerId, EventCode.Logout, eventRemark, model);
            }

        }

        private void HandleByMachineStatus(string operater, string appId, string trgerId, string eventCode, string eventRemark, MachineEventByMachineStatusModel model)
        {
            LogUtil.Info(">>>>>EventHandleByMachineStatus");

            string machineId = trgerId;
            var machine = CurrentDb.Machine.Where(m => m.Id == trgerId).FirstOrDefault();

            if (machine == null)
                return;

            string storeName = BizFactory.Merch.GetStoreName(machine.CurUseMerchId, machine.CurUseStoreId);
            string shopName = BizFactory.Merch.GetShopName(machine.CurUseMerchId, machine.CurUseShopId);
            string operaterUserName = BizFactory.Merch.GetOperaterUserName(machine.CurUseMerchId, operater);

            machine.LastRequestTime = DateTime.Now;

            bool isLog = false;
            switch (model.Status)
            {
                case "running":
                    eventRemark = "运行正常";

                    if (machine.RunStatus != E_MachineRunStatus.Running)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Running;
                    break;
                case "setting":
                    eventRemark = "维护中";

                    if (machine.RunStatus != E_MachineRunStatus.Setting)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Setting;
                    break;
                case "excepition":

                    eventRemark = "异常";

                    if (machine.RunStatus != E_MachineRunStatus.Excepition)
                    {
                        isLog = true;
                    }

                    machine.RunStatus = E_MachineRunStatus.Excepition;

                    break;
                default:
                    eventRemark = "未知状态";
                    break;
            }

            CurrentDb.SaveChanges();

            if (isLog)
            {
                MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, machineId, EventCode.MachineStatus, string.Format("店铺：{0}，门店：{1}，机器：{2}，{3}", storeName, shopName, machineId, eventRemark), model);
            }
        }

        private void HandleByPickup(string operater, string appId, string trgerId, string eventCode, string eventRemark, MachineEventByPickupModel model)
        {
            if (model == null)
                return;

            List<Order> s_Orders = new List<Order>();
            List<OrderSub> s_OrderSubs = new List<OrderSub>();
            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(model.SignId))
                {
                    model.SignId = IdWorker.Build(IdType.NewGuid);
                }

                var orderPickupLog = CurrentDb.OrderPickupLog.Where(m => m.Id == model.SignId).FirstOrDefault();

                if (orderPickupLog != null)
                    return;

                string machineId = trgerId;
                var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();

                if (machine == null)
                    return;

                string storeName = BizFactory.Merch.GetStoreName(machine.CurUseMerchId, machine.CurUseStoreId);
                string shopName = BizFactory.Merch.GetShopName(machine.CurUseMerchId, machine.CurUseShopId);
                string operaterUserName = BizFactory.Merch.GetOperaterUserName(machine.CurUseMerchId, operater);

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

                var d_Order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();
                if (d_Order != null)
                {

                    if (d_Order.Status != E_OrderStatus.Completed && !d_Order.ExIsHappen)
                    {
                        var d_OrderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == model.OrderId && m.ShopMode == E_ShopMode.Machine && m.MachineId == machine.Id).ToList();


                        //是否触发过取货
                        if (d_Order.PickupTrgTime == null)
                        {
                            d_Order.PickupIsTrg = true;
                            d_Order.PickupTrgTime = DateTime.Now;
                            d_Order.PickupFlowLastDesc = "商品取货，正在出货";
                            d_Order.PickupFlowLastTime = DateTime.Now;

                            int timoutM = d_Order.Quantity * 5;

                            Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPickupTimeout, d_Order.Id, DateTime.Now.AddMinutes(timoutM), new Order2CheckPickupTimeoutModel { OrderId = d_Order.Id, MachineId = d_Order.MachineId });
                        }

                        orderPickupLog = new OrderPickupLog();
                        orderPickupLog.Id = model.SignId;
                        orderPickupLog.OrderId = model.OrderId;
                        orderPickupLog.MerchId = d_Order.MerchId;
                        orderPickupLog.ShopMode = E_ShopMode.Machine;
                        orderPickupLog.StoreId = d_Order.StoreId;
                        orderPickupLog.ShopId = d_Order.ShopId;
                        orderPickupLog.MachineId = d_Order.MachineId;
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
                        orderPickupLog.ImgId3 = model.ImgId3;
                        orderPickupLog.PickupUseTime = model.PickupUseTime;
                        orderPickupLog.ActionRemark = remark.ToString();
                        orderPickupLog.Remark = model.Remark;
                        orderPickupLog.CreateTime = DateTime.Now;
                        orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(orderPickupLog);

                        if (model.PickupStatus == E_OrderPickupStatus.Exception)
                        {

                            d_Order.ExIsHappen = true;
                            d_Order.ExHappenTime = DateTime.Now;

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
                                d_Order.ExImgUrls = exImgUrls.ToJsonString();
                            }

                            d_Order.PickupFlowLastDesc = "取货动作发生异常";
                            d_Order.PickupFlowLastTime = DateTime.Now;

                            foreach (var d_OrderSub in d_OrderSubs)
                            {
                                if (d_OrderSub.PayStatus == E_PayStatus.PaySuccess)
                                {
                                    if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked
                                        && d_OrderSub.PickupStatus != E_OrderPickupStatus.Exception
                                        && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked
                                        && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        d_OrderSub.PickupStatus = E_OrderPickupStatus.Exception;
                                        d_OrderSub.ExPickupReason = "取货动作发生异常";
                                        d_OrderSub.ExPickupIsHappen = true;
                                        d_OrderSub.ExPickupHappenTime = DateTime.Now;
                                    }
                                }

                            }

                            machine.ExIsHas = true;
                            machine.ExReason = "取货动作发生异常";

                            Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, d_Order.Id);
                        }
                        else
                        {

                            foreach (var d_OrderSub in d_OrderSubs)
                            {

                                if (d_OrderSub.Id == model.UniqueId)
                                {
                                    d_OrderSub.PickupFlowLastDesc = model.ActionName + model.ActionStatusName;
                                    d_OrderSub.PickupFlowLastTime = DateTime.Now;

                                    if (model.PickupStatus == E_OrderPickupStatus.Taked)
                                    {
                                        if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                        {
                                            BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.OrderPickupOneSysMadeSignTake, d_OrderSub.ShopMode, d_OrderSub.MerchId, d_OrderSub.StoreId, d_OrderSub.ShopId, d_OrderSub.MachineId, d_OrderSub.CabinetId, d_OrderSub.SlotId, d_OrderSub.PrdProductSkuId, 1);

                                            s_OrderSubs.Add(d_OrderSub);
                                        }

                                        if (d_OrderSub.PickupEndTime == null)
                                        {
                                            d_OrderSub.PickupEndTime = DateTime.Now;
                                        }

                                    }

                                    if (d_OrderSub.PickupStartTime == null)
                                    {
                                        d_OrderSub.PickupStartTime = DateTime.Now;
                                    }

                                    if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        d_OrderSub.PickupStatus = model.PickupStatus;
                                    }

                                    CurrentDb.SaveChanges();
                                }
                            }

                            var orderDetailsChildSonsCompeleteCount = d_OrderSubs.Where(m => m.PickupStatus == E_OrderPickupStatus.Taked).Count();
                            //判断全部订单都是已完成
                            if (orderDetailsChildSonsCompeleteCount == d_OrderSubs.Count)
                            {
                                d_Order.PickupFlowLastDesc = "全部商品出货完成";
                                d_Order.PickupFlowLastTime = DateTime.Now;
                                d_Order.Status = E_OrderStatus.Completed;
                                d_Order.CompletedTime = DateTime.Now;

                                Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, d_Order.Id);
                            }
                        }
                    }

                    s_Orders.Add(d_Order);
                }

                CurrentDb.SaveChanges();
                ts.Complete();


                MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, machineId, EventCode.Pickup, string.Format("店铺：{0}，门店：{1}，机器：{2}，{3}", storeName, shopName, machine.Id, remark.ToString()), model);

                MqFactory.Global.PushEventNotify(operater, AppId.STORETERM, machineId, EventCode.OrderPickupOneSysMadeSignTake, string.Format("店铺：{0}，门店：{1}，机器：{2}，{3}", storeName, shopName, machine.Id, remark.ToString()), new {
                    Orders= s_Orders,
                    OrderSubs= s_OrderSubs
                });
            }
        }

        private void HandleByPickupTest(string operater, string appId, string trgerId, string eventCode, string eventRemark, MachineEventByPickupTestModel model)
        {
            if (model == null)
                return;

            string machineId = trgerId;
            var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();

            if (machine == null)
                return;

            machine.LastRequestTime = DateTime.Now;
            CurrentDb.SaveChanges();

            string storeName = BizFactory.Merch.GetStoreName(machine.CurUseMerchId, machine.CurUseStoreId);
            string shopName = BizFactory.Merch.GetShopName(machine.CurUseMerchId, machine.CurUseShopId);
            string operaterUserName = BizFactory.Merch.GetOperaterUserName(machine.CurUseMerchId, operater);

            StringBuilder remark = new StringBuilder("");
            string productSkuName = "[测试]";

            var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(machine.CurUseMerchId, model.ProductSkuId);

            if (r_ProductSku != null)
            {
                productSkuName += r_ProductSku.Name;
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

            MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, machineId, EventCode.PickupTest, string.Format("店铺：{0}，门店：{1}，机器：{2}，{3}", storeName, shopName, machine.Id, remark.ToString()), model);

        }

        //private void HandleByStockChangeLog(string operater, string appId, string trgerId, string eventCode, string eventRemark, SellChannelStockChangeModel model)
        //{
        //    string merchName = BizFactory.Merch.GetMerchName(model.MerchId);
        //    string storeName = BizFactory.Merch.GetStoreName(model.MerchId, model.StoreId);
        //    string operaterUserName = BizFactory.Merch.GetClientName(model.MerchId, operater);

        //    var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(model.MerchId, model.PrdProductSkuId);

        //    var sellChannelStockLog = new SellChannelStockLog();
        //    sellChannelStockLog.Id = IdWorker.Build(IdType.NewGuid);
        //    sellChannelStockLog.MerchId = model.MerchId;
        //    sellChannelStockLog.MerchName = merchName;
        //    sellChannelStockLog.StoreId = model.StoreId;
        //    sellChannelStockLog.StoreName = storeName;
        //    sellChannelStockLog.ShopId = model.ShopId;
        //    sellChannelStockLog.MachineId = model.MachineId;
        //    sellChannelStockLog.ShopMode = model.ShopMode;
        //    sellChannelStockLog.CabinetId = model.CabinetId;
        //    sellChannelStockLog.SlotId = model.SlotId;
        //    sellChannelStockLog.PrdProductId = bizProductSku.ProductId;
        //    sellChannelStockLog.PrdProductSkuId = model.PrdProductSkuId;
        //    sellChannelStockLog.PrdProductSkuName = bizProductSku.Name;
        //    sellChannelStockLog.SellQuantity = model.SellQuantity;
        //    sellChannelStockLog.WaitPayLockQuantity = model.WaitPayLockQuantity;
        //    sellChannelStockLog.WaitPickupLockQuantity = model.WaitPickupLockQuantity;
        //    sellChannelStockLog.SumQuantity = model.SumQuantity;
        //    sellChannelStockLog.EventCode = model.EventCode;
        //    sellChannelStockLog.EventName = EventCode.GetEventName(model.EventCode);
        //    sellChannelStockLog.ChangeQuantity = model.ChangeQuantity;
        //    sellChannelStockLog.Creator = operater;
        //    sellChannelStockLog.CreateTime = DateTime.Now;

        //    if (string.IsNullOrEmpty(model.MachineId))
        //    {
        //        sellChannelStockLog.Remark = eventRemark;
        //    }
        //    else
        //    {
        //        sellChannelStockLog.Remark = string.Format("店铺：{0}，机器：{1}，{2}", storeName, model.MachineId, eventRemark);
        //    }

        //    CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
        //    CurrentDb.SaveChanges();

        //}
    }
}
