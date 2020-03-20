using LocalS.BLL.Mq;
using LocalS.BLL.Task;
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
            model.RunStatus = machine.RunStatus;
            model.LastRequestTime = machine.LastRequestTime;
            model.AppVersion = machine.AppVersionName;
            model.CtrlSdkVersion = machine.CtrlSdkVersionCode;
            model.IsHiddenKind = machine.IsHiddenKind;
            model.KindRowCellSize = machine.KindRowCellSize;
            model.IsTestMode = machine.IsTestMode;
            model.IsOpenChkCamera = machine.IsOpenChkCamera;
            model.IsUseFingerVeinCtrl = machine.IsUseFingerVeinCtrl;
            model.IsUseSanCtrl = machine.IsUseSanCtrl;
            model.ExIsHas = machine.ExIsHas;

            var machineCabinets = CurrentDb.MachineCabinet.Where(m => m.MachineId == id && m.IsUse == true).OrderByDescending(m => m.Priority).ToList();

            foreach (var machineCabinet in machineCabinets)
            {
                var cabinet = new CabinetInfoModel();
                cabinet.Id = machineCabinet.CabinetId;
                cabinet.Name = machineCabinet.CabinetName;
                cabinet.RowColLayout = machineCabinet.RowColLayout;
                cabinet.Priority = machineCabinet.Priority;
                cabinet.FixSlotQuantity = machineCabinet.FixSlotQuantity;
                cabinet.ComId = machineCabinet.ComId;
                model.Cabinets.Add(cabinet.Id, cabinet);
            }

            var merch = CurrentDb.Merch.Where(m => m.Id == machine.CurUseMerchId).FirstOrDefault();

            if (merch != null)
            {
                model.MerchId = merch.Id;
                model.MerchName = merch.Name;
                model.CsrQrCode = merch.CsrQrCode;
                model.CsrPhoneNumber = merch.CsrPhoneNumber;
                model.CsrHelpTip = merch.CsrHelpTip;

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

        public static int[] GetLayout(string str)
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

        public static int[] GetPendantRows(string str)
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


        public CustomJsonResult EventNotify(string operater, string appId, string machineId, double lat, double lng, E_MachineEventType type, object content)
        {
            MqFactory.Global.PushMachineEventNotify(operater, appId, machineId, lat, lng, type, content);
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

                if (model.Lat > 0 && model.Lng > 0)
                {

                    machine.Lat = (float)model.Lat;
                    machine.Lng = (float)model.Lng;
                }

                machine.LastRequestTime = DateTime.Now;


                string eventName = "";
                StringBuilder eventRemark = new StringBuilder("");

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
                                    eventRemark.Append("正常");
                                    machine.RunStatus = E_MachineRunStatus.Running;
                                    break;
                                case "setting":
                                    eventRemark.Append("维护中");
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
                            eventRemark.Append(scanSlotsModel.Remark);
                        }
                        #endregion
                        break;
                    case E_MachineEventType.Pickup:
                        #region Pickup 商品取货
                        eventName = "商品取货";

                        var pickupModel = model.Content.ToJsonObject<MachineEventByPickupModel>();
                        if (pickupModel != null)
                        {
                            if (pickupModel.IsTest)
                            {
                                eventRemark.Append("[测试]");
                            }

                            var bizProduct = CacheServiceFactory.ProductSku.GetInfo(machine.CurUseMerchId, pickupModel.ProductSkuId);
                            if (bizProduct == null)
                            {
                                eventRemark.Append("商品:无");
                            }
                            else
                            {
                                eventRemark.Append("商品:" + bizProduct.Name);
                            }

                            eventRemark.Append(string.Format(",货槽:{0},当前动作:{1},状态:{2}", pickupModel.SlotId, pickupModel.ActionName, pickupModel.ActionStatusName));

                            if (pickupModel.IsPickupComplete)
                            {
                                eventRemark.Append(string.Format(",取货完成,用时:{0}", pickupModel.PickupUseTime));
                            }

                            if (!pickupModel.IsTest)
                            {
                                var order = CurrentDb.Order.Where(m => m.Id == pickupModel.OrderId).FirstOrDefault();
                                var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == pickupModel.OrderId && m.SellChannelRefId == machine.Id && m.SellChannelRefType == E_SellChannelRefType.Machine).FirstOrDefault();
                                var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == pickupModel.OrderId).ToList();

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
                                    if (pickupModel.Status == E_OrderPickupStatus.Exception)
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
                                        if (orderSubChildUnique.Id == pickupModel.UniqueId)
                                        {
                                            orderSubChildUnique.LastPickupActionId = pickupModel.ActionId;
                                            orderSubChildUnique.LastPickupActionStatusCode = pickupModel.ActionStatusCode;
                                            orderSubChildUnique.PickupStatus = pickupModel.Status;

                                            if (pickupModel.Status == E_OrderPickupStatus.Taked)
                                            {
                                                BizFactory.ProductSku.OperateStockQuantity(model.MachineId, OperateStockType.OrderPickupOneSysMadeSignTake, orderSubChildUnique.MerchId, orderSubChildUnique.StoreId, orderSubChildUnique.SellChannelRefId, orderSubChildUnique.CabinetId, orderSubChildUnique.SlotId, orderSubChildUnique.PrdProductSkuId, 1);
                                            }
                                        }
                                    }
                                }

                                if (pickupModel.Status == E_OrderPickupStatus.Exception)
                                {
                                    order.ExIsHappen = true;
                                    order.ExHappenTime = DateTime.Now;

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

                                        Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, orderSub.Id);
                                    }
                                }

                                var orderPickupLog = new OrderPickupLog();
                                orderPickupLog.Id = GuidUtil.New();
                                orderPickupLog.OrderId = pickupModel.OrderId;
                                orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                                orderPickupLog.SellChannelRefId = model.MachineId;
                                orderPickupLog.UniqueId = pickupModel.UniqueId;
                                orderPickupLog.PrdProductSkuId = pickupModel.ProductSkuId;
                                orderPickupLog.SlotId = pickupModel.SlotId;
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
                machineOperateLog.Remark = eventRemark.ToString();
                machineOperateLog.Creator = model.Operater;
                machineOperateLog.CreateTime = DateTime.Now;
                CurrentDb.MachineOperateLog.Add(machineOperateLog);
                CurrentDb.SaveChanges();
                ts.Complete();
            }
        }
    }
}
