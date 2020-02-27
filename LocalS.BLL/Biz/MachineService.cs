using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public enum E_MachineEventType
    {
        Unknow = 0,
        SendHeartbeatBag = 1,
        Pickup = 2,
        ScanSlots = 3
    }

    public class MachineService : BaseDbContext
    {
        public MachineInfoModel GetOne(string id)
        {
            var model = new MachineInfoModel();

            var machine = CurrentDb.Machine.Where(m => m.Id == id).FirstOrDefault();

            if (machine == null)
                return null;

            model.Id = machine.Id;
            model.DeviceId = machine.DeviceId;
            model.Name = machine.Name;
            model.MainImgUrl = machine.MainImgUrl;
            model.LogoImgUrl = machine.LogoImgUrl;
            model.JPushRegId = machine.JPushRegId;
            model.CabinetId_1 = machine.CabinetId_1;
            model.CabinetName_1 = machine.CabinetName_1;
            model.CabinetRowColLayout_1 = GetLayout(machine.CabinetRowColLayout_1);
            model.CabinetPendantRows_1 = GetPendantRows(machine.CabinetPendantRows_1);
            model.RunStatus = machine.RunStatus;
            model.LastRequestTime = machine.LastRequestTime;
            model.AppVersion = machine.AppVersionName;
            model.CtrlSdkVersion = machine.CtrlSdkVersionCode;
            model.IsHiddenKind = machine.IsHiddenKind;
            model.KindRowCellSize = machine.KindRowCellSize;
            model.IsTestMode = machine.IsTestMode;

            var merch = CurrentDb.Merch.Where(m => m.Id == machine.CurUseMerchId).FirstOrDefault();

            if (merch != null)
            {
                model.MerchId = merch.Id;
                model.MerchName = merch.Name;
                model.CsrQrCode = merch.CsrQrCode;
                model.CsrPhoneNumber = merch.CsrPhoneNumber;
                model.PayOptions = merch.TermAppPayOptions.ToJsonObject<List<PayOption>>();

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == machine.CurUseMerchId && m.MachineId == id).FirstOrDefault();
                if (merchMachine != null)
                {
                    model.Name = merchMachine.Name;
                    model.LogoImgUrl = merchMachine.LogoImgUrl;
                }

                var merchStore = BizFactory.Store.GetOne(machine.CurUseStoreId);
                if (merchStore != null)
                {
                    model.StoreId = merchStore.Id;
                    model.StoreName = merchStore.Name;
                }
            }

            return model;
        }


        public List<BannerModel> GetHomeBanners(string id)
        {
            var bannerModels = new List<BannerModel>();

            var machine = BizFactory.Machine.GetOne(id);

            LogUtil.Info("MerchId：" + machine.MerchId);
            LogUtil.Info("BelongId：" + id);
            var adContentIds = CurrentDb.AdContentBelong.Where(m => m.MerchId == machine.MerchId && m.AdSpaceId == E_AdSpaceId.MachineHomeBanner && m.BelongType == E_AdSpaceBelongType.Machine && m.BelongId == id).Select(m => m.AdContentId).ToArray();

            if (adContentIds != null && adContentIds.Length > 0)
            {
                var adContents = CurrentDb.AdContent.Where(m => adContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();


                foreach (var item in adContents)
                {
                    bannerModels.Add(new BannerModel { Url = item.Url });
                }
            }

            return bannerModels;
        }

        public void SendUpdateProductSkuStock(string id, UpdateMachineProdcutSkuStockModel updateProdcutSkuStock)
        {
            if (updateProdcutSkuStock != null)
            {
                var machine = BizFactory.Machine.GetOne(id);
                PushService.SendUpdateProductSkuStock(machine.JPushRegId, updateProdcutSkuStock);
            }
        }

        public void SendUpdateHomeBanners(string id)
        {
            var machine = BizFactory.Machine.GetOne(id);
            var banners = BizFactory.Machine.GetHomeBanners(id);
            PushService.SendUpdateMachineHomeBanners(machine.JPushRegId, banners);
        }

        public void SendUpdateHomeLogo(string id, string logoImgUrl)
        {
            var machine = BizFactory.Machine.GetOne(id);
            var content = new { url = logoImgUrl };
            PushService.SendUpdateMachineHomeLogo(machine.JPushRegId, content);
        }

        public void SendPaySuccess(string id, string orderId, string orderSn)
        {
            //var machine = BizFactory.Machine.GetOne(id);
            //var orderDetails = BizFactory.Order.GetOrderDetailsByPickup(orderId, id);
            //var content = new { orderId = orderId, orderSn = orderSn, status = E_OrderStatus.Payed, OrderDetails = orderDetails };
            //PushService.SendPaySuccess(machine.JPushRegId, content);
        }

        private static int[] GetLayout(string str)
        {
            int[] layout = null;

            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                string[] data = str.Split(',');
                if (data.Length > 0)
                {

                    layout = new int[data.Length];

                    for (int i = 0; i < data.Length; i++)
                    {
                        layout[i] = int.Parse(data[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }


            return layout;
        }

        private static int[] GetPendantRows(string str)
        {

            int[] layout = null;

            try
            {
                string[] sNums = str.Split(',');
                layout = Array.ConvertAll(sNums, int.Parse);
                return layout;
            }
            catch (Exception ex)
            {
                return layout;
            }
        }


        public CustomJsonResult EventNotify(string operater, string appId, string machineId, E_MachineEventType type, object content)
        {
            MqFactory.Global.PushMachineEventNotify(operater, appId, machineId, type, content);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }


        public void EventHandle(MachineEventNotifyModel model)
        {
            if (model == null)
                return;

            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == model.MachineId).FirstOrDefault();

                if (machine == null)
                    return;

                machine.LastRequestTime = DateTime.Now;

                string eventName = "";
                string eventRemark = "";

                switch (model.Type)
                {
                    case E_MachineEventType.SendHeartbeatBag:
                        #region SendHeartbeatBag 发送心跳包
                        eventName = "发送心跳包";
                        var heartbeatBagModel = model.Content.ToJsonObject<MachineEventByHeartbeatBagModel>();
                        if (heartbeatBagModel != null)
                        {
                            switch (heartbeatBagModel.Status)
                            {
                                case "running":
                                    eventRemark = "正常";
                                    machine.RunStatus = E_MachineRunStatus.Running;
                                    break;
                                case "setting":
                                    eventRemark = "维护中";
                                    machine.RunStatus = E_MachineRunStatus.Setting;
                                    break;
                            }
                        }
                        #endregion
                        break;
                    case E_MachineEventType.ScanSlots:
                        #region ScanSlots 扫描货道
                        eventName = "扫描货道";

                        var scanSlotsModel = model.Content.ToJsonObject<MachineEventByScanSlotsModel>();
                        if (scanSlotsModel != null)
                        {
                            eventRemark = scanSlotsModel.Remark;
                        }
                        #endregion
                        break;
                    case E_MachineEventType.Pickup:
                        #region Pickup 商品取货
                        eventName = "商品取货";

                        var pickupModel = model.Content.ToJsonObject<MachineEventByPickupModel>();
                        if (pickupModel != null)
                        {
                            if (!pickupModel.IsTest)
                            {
                                var orderSubChildUnique = CurrentDb.OrderSubChildUnique.Where(m => m.Id == pickupModel.UniqueId).FirstOrDefault();
                                if (orderSubChildUnique != null)
                                {
                                    orderSubChildUnique.LastPickupActionId = pickupModel.ActionId;
                                    orderSubChildUnique.LastPickupActionStatusCode = pickupModel.ActionStatusCode;
                                    orderSubChildUnique.Status = pickupModel.Status;
                                    CurrentDb.SaveChanges();


                                    //如果某次取货异常 剩下所有取货都标识为订单取货异常
                                    var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == orderSubChildUnique.OrderId).ToList();

                                    if (pickupModel.Status == E_OrderPickupStatus.Exception)
                                    {
                                        var order = CurrentDb.Order.Where(m => m.Id == orderSubChildUnique.OrderId).FirstOrDefault();
                                        if (order != null)
                                        {
                                            order.ExIsHappen = true;
                                            order.ExHappenTime = DateTime.Now;
                                            CurrentDb.SaveChanges();
                                        }

                                        foreach (var item in orderSubChildUniques)
                                        {
                                            if (item.Status != E_OrderPickupStatus.Completed && item.Status != E_OrderPickupStatus.Canceled)
                                            {
                                                item.Status = E_OrderPickupStatus.Exception;
                                                item.ExPickupIsHappen = true;
                                                item.ExPickupHappenTime = DateTime.Now;
                                                CurrentDb.SaveChanges();
                                            }
                                        }
                                    }


                                    var orderDetailsChildSonsCompeleteCount = orderSubChildUniques.Where(m => m.Status == E_OrderPickupStatus.Completed).Count();
                                    //判断全部订单都是已完成
                                    if (orderDetailsChildSonsCompeleteCount == orderSubChildUniques.Count)
                                    {
                                        var order = CurrentDb.Order.Where(m => m.Id == orderSubChildUnique.OrderId).FirstOrDefault();
                                        if (order != null)
                                        {
                                            order.Status = E_OrderStatus.Completed;
                                            order.CompletedTime = DateTime.Now;
                                        }
                                    }

                                    var orderPickupLog = new OrderPickupLog();
                                    orderPickupLog.Id = GuidUtil.New();
                                    orderPickupLog.OrderId = orderSubChildUnique.OrderId;
                                    orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                                    orderPickupLog.SellChannelRefId = model.MachineId;
                                    orderPickupLog.UniqueId = pickupModel.UniqueId;
                                    orderPickupLog.PrdProductSkuId = orderSubChildUnique.PrdProductSkuId;
                                    orderPickupLog.SlotId = orderSubChildUnique.SlotId;
                                    orderPickupLog.Status = pickupModel.Status;
                                    orderPickupLog.ActionId = pickupModel.ActionId;
                                    orderPickupLog.ActionName = pickupModel.ActionName;
                                    orderPickupLog.ActionStatusCode = pickupModel.ActionStatusCode;
                                    orderPickupLog.ActionStatusName = pickupModel.ActionStatusName;
                                    orderPickupLog.IsPickupComplete = pickupModel.IsPickupComplete;
                                    orderPickupLog.ImgId = pickupModel.ImgId;
                                    if (pickupModel.IsPickupComplete)
                                    {
                                        orderPickupLog.PickupUseTime = pickupModel.PickupUseTime;
                                        orderPickupLog.ActionRemark = "取货完成";
                                        BizFactory.ProductSku.OperateStockQuantity(model.MachineId, OperateStockType.OrderPickupOneSysMadeSignTake, orderSubChildUnique.MerchId, orderSubChildUnique.StoreId, orderSubChildUnique.SellChannelRefId, orderSubChildUnique.SlotId, orderSubChildUnique.PrdProductSkuId, 1);
                                    }
                                    else
                                    {
                                        if (pickupModel.Status == E_OrderPickupStatus.SendPickupCmd)
                                        {
                                            orderPickupLog.ActionRemark = "发送命令";
                                        }
                                        else if (pickupModel.Status == E_OrderPickupStatus.Exception)
                                        {
                                            orderPickupLog.ActionRemark = "发生异常";
                                        }
                                        else
                                        {
                                            orderPickupLog.ActionRemark = pickupModel.ActionName + pickupModel.ActionStatusName;
                                        }
                                    }

                                    orderPickupLog.Remark = pickupModel.Remark;
                                    orderPickupLog.CreateTime = DateTime.Now;
                                    orderPickupLog.Creator = model.MachineId;
                                    CurrentDb.OrderPickupLog.Add(orderPickupLog);
                                }
                            }

                            if (string.IsNullOrEmpty(pickupModel.ProductSkuId))
                            {
                                if (pickupModel.IsPickupComplete)
                                {
                                    eventRemark = string.Format("商品:无,货槽:{0},当前动作({1}):{2},状态({3}):{4},取货完成,用时:{5}", pickupModel.SlotId, pickupModel.ActionId, pickupModel.ActionName, pickupModel.ActionStatusCode, pickupModel.ActionStatusName, pickupModel.PickupUseTime);
                                }
                                else
                                {
                                    eventRemark = string.Format("商品:无,货槽:{0},当前动作({1}):{2},状态({3}):{4}", pickupModel.SlotId, pickupModel.ActionId, pickupModel.ActionName, pickupModel.ActionStatusCode, pickupModel.ActionStatusName);
                                }
                            }
                            else
                            {

                                string productSkuId = "";
                                string productSkuName = "";

                                var bizProduct = CacheServiceFactory.ProductSku.GetInfo(machine.CurUseMerchId, pickupModel.ProductSkuId);
                                if (bizProduct != null)
                                {
                                    productSkuId = bizProduct.Id;
                                    productSkuName = bizProduct.Name;
                                }

                                if (pickupModel.IsPickupComplete)
                                {
                                    eventRemark = string.Format("商品({0}):{1},货槽:{1},当前动作({3}):{4},状态({5}):{6},取货完成,用时:{7}", productSkuId, productSkuName, pickupModel.SlotId, pickupModel.ActionId, pickupModel.ActionName, pickupModel.ActionStatusCode, pickupModel.ActionStatusName, pickupModel.PickupUseTime);
                                }
                                else
                                {
                                    eventRemark = string.Format("商品({0}):{1},货槽:{2},当前动作({3}):{4},状态({5}):{6}", productSkuId, productSkuName, pickupModel.SlotId, pickupModel.ActionId, pickupModel.ActionName, pickupModel.ActionStatusCode, pickupModel.ActionStatusName);
                                }
                            }
                        }

                        #endregion
                        break;
                }

                var machineOperateLog = new MachineOperateLog();
                machineOperateLog.Id = GuidUtil.New();
                machineOperateLog.AppId = model.AppId;
                machineOperateLog.MerchId = machine.CurUseMerchId;
                machineOperateLog.StoreId = machine.CurUseStoreId;
                machineOperateLog.MachineId = model.MachineId;
                machineOperateLog.OperateUserId = model.Operater;
                machineOperateLog.EventCode = (int)model.Type;
                machineOperateLog.EventName = eventName;
                machineOperateLog.Remark = eventRemark;
                machineOperateLog.Creator = model.Operater;
                machineOperateLog.CreateTime = DateTime.Now;
                CurrentDb.MachineOperateLog.Add(machineOperateLog);

                CurrentDb.SaveChanges();
                ts.Complete();
            }
        }
    }
}
