using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Entity
{
    public static class EventCode
    {
        [EventCodeRemark("I", "登录")]
        public const string Login = "Login";
        [EventCodeRemark("I", "退出")]
        public const string Logout = "Logout";
        [EventCodeRemark("I", "心跳包")]
        public const string HeartbeatBag = "HeartbeatBag";
        [EventCodeRemark("I", "扫描货道")]
        public const string ScanSlots = "ScanSlots";
        [EventCodeRemark("I", "商品取货")]
        public const string Pickup = "Pickup";
        [EventCodeRemark("I", "机器货道初始化")]
        public const string MachineCabinetSlotInit = "MachineCabinetSlotInit";
        [EventCodeRemark("I", "机器货道替换")]
        public const string MachineCabinetSlotReplace = "MachineCabinetSlotReplace";
        [EventCodeRemark("I", "机器货道保存")]
        public const string MachineCabinetSlotSave = "MachineCabinetSlotSave";
        [EventCodeRemark("I", "机器货道移除")]
        public const string MachineCabinetSlotRemove = "MachineCabinetSlotRemove";
        [EventCodeRemark("I", "机器货道调整库存数量")]
        public const string MachineCabinetSlotAdjustStockQuantity = "MachineCabinetSlotAdjustStockQuantity";
        [EventCodeRemark("I", "机器货道调整库存查看")]
        public const string MachineCabinetGetSlots = "MachineCabinetGetSlots";
        [EventCodeRemark("I", "机器货道保存扫成结果")]
        public const string MachineCabinetSaveRowColLayout = "MachineCabinetSaveRowColLayout";
        [EventCodeRemark("I", "机器货道商品价格调整")]
        public const string MachineAdjustStockSalePrice = "MachineAdjustStockSalePrice";
        [EventCodeRemark("I", "机器处理运行异常信息")]
        public const string MachineHandleRunEx = "MachineHandleRunEx";
        [EventCodeRemark("I", "商品取货人工标记未取状态")]
        public const string OrderPickupOneManMadeSignNotTakeByNotComplete = "OrderPickupOneManMadeSignNotTakeByNotComplete";
        [EventCodeRemark("I", "商品取货人工标记未取状态")]
        public const string OrderPickupOneManMadeSignNotTakeByComplete = "OrderPickupOneManMadeSignNotTakeByComplete";
        [EventCodeRemark("I", "商品取货系统标记已取")]
        public const string OrderPickupOneSysMadeSignTake = "OrderPickupOneSysMadeSignTake";
        [EventCodeRemark("I", "商品取货系统标记未取")]
        public const string OrderPickupOneManMadeSignTakeByNotComplete = "OrderPickupOneManMadeSignTakeByNotComplete";
        [EventCodeRemark("I", "订单取消")]
        public const string OrderCancle = "OrderCancle";
        [EventCodeRemark("I", "订单支付成功")]
        public const string OrderPaySuccess = "OrderPaySuccess";
        [EventCodeRemark("I", "订单预定成功")]
        public const string OrderReserveSuccess = "OrderReserveSuccess";
        [EventCodeRemark("I", "订单异常处理")]
        public const string OrderHandleExOrder = "OrderHandleExOrder";
        [EventCodeRemark("I", "新增管理账号")]
        public const string AdminUserAdd = "AdminUserAdd";
        [EventCodeRemark("I", "修改管理账号信息")]
        public const string AdminUserEdit = "AdminUserEdit";
        [EventCodeRemark("I", "发布广告")]
        public const string AdSpaceRelease = "AdSpaceRelease";
        [EventCodeRemark("I", "删除广告")]
        public const string AdSpaceDeleteAdContent = "AdSpaceDeleteAdContent";
        [EventCodeRemark("I", "保存机器信息")]
        public const string MachineEdit = "MachineEdit";
        [EventCodeRemark("I", "新增商品分类信息")]
        public const string PrdKindAdd = "PrdKindAdd";
        [EventCodeRemark("I", "修改商品分类信息")]
        public const string PrdKindEdit = "PrdKindEdit";
        [EventCodeRemark("I", "删除商品分类信息")]
        public const string PrdKindDelete = "PrdKindDelete";
        [EventCodeRemark("I", "新增商品信息")]
        public const string PrdProductAdd = "PrdProductAdd";
        [EventCodeRemark("I", "修改商品信息")]
        public const string PrdProductEdit = "PrdProductEdit";
        [EventCodeRemark("I", "新增店铺信息")]
        public const string StoreAdd = "StoreAdd";
        [EventCodeRemark("I", "修改店铺信息")]
        public const string StoreEdit = "StoreEdit";
        [EventCodeRemark("I", "店铺绑定机器")]
        public const string StoreAddMachine = "StoreAddMachine";
        [EventCodeRemark("I", "店铺移除机器")]
        public const string StoreRemoveMachine = "StoreRemoveMachine";
        [EventCodeRemark("I", "发送更新机器库存信息命令")]
        public const string MCmdUpdateProductSkuStock = "mcmd:update:ProductSkuStock";
        [EventCodeRemark("I", "发送更新机器首页广告")]
        public const string MCmdUpdateHomeBanners = "mcmd:update:HomeBanners";
        [EventCodeRemark("I", "发送更新机器LOGO")]
        public const string MCmdUpdateHomeLogo = "mcmd:update:HomeLogo";
        [EventCodeRemark("I", "发送重启系统命令")]
        public const string MCmdSysReboot = "mcmd:sys:Reboot";
        [EventCodeRemark("I", "发送关闭系统命令")]
        public const string MCmdSysShutdown = "mcmd:sys:Shutdown";
        [EventCodeRemark("I", "发送设置系统状态命令")]
        public const string MCmdSysSetStatus = "mcmd:sys:SetStatus";
        [EventCodeRemark("I", "发送打开DSX01设备取货门命令")]
        public const string MCmdDsx01OpenPickupDoor = "mcmd:dsx01:OpenPickupDoor";
        [EventCodeRemark("I", "发送支付成功命令")]
        public const string MCmdPaySuccess = "mcmd:pay:Success";

        public static string GetEventName(string eventCode)
        {

            string name = string.Empty;

            try
            {
                var atts = typeof(EventCode).GetField(eventCode).GetCustomAttributes(typeof(EventCodeRemarkAttribute), false);
                if (atts != null)
                {
                    if (atts.Length > 0)
                    {
                        var att = (EventCodeRemarkAttribute)atts[0];

                        name = att.Name;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return name;

            //string name = "";
            //switch (eventCode)
            //{
            //    case EventCode.Login:
            //        name = "登录";
            //        break;
            //    case EventCode.Logout:
            //        name = "退出";
            //        break;
            //    case EventCode.HeartbeatBag:
            //        name = "心跳包";
            //        break;
            //    case EventCode.ScanSlots:
            //        name = "扫描货道";
            //        break;
            //    case EventCode.Pickup:
            //        name = "商品取货";
            //        break;
            //    case EventCode.MachineCabinetSlotAdjustStockQuantity:
            //        name = "商品库存调整";
            //        break;
            //    case EventCode.MachineAdjustStockSalePrice:
            //        name = "商品价格调整";
            //        break;
            //    case EventCode.MachineCabinetSlotSave:
            //        name = "商品货道保存";
            //        break;
            //    case EventCode.MachineCabinetSlotRemove:
            //        name = "商品货道移除";
            //        break;
            //    case EventCode.MachineCabinetGetSlots:
            //        name = "查看商品货道";
            //        break;
            //    case EventCode.OrderCancle:
            //        name = "订单取消";
            //        break;
            //    case EventCode.OrderReserveSuccess:
            //        name = "订单预定成功";
            //        break;
            //    case EventCode.OrderPaySuccess:
            //        name = "订单支付成功";
            //        break;
            //    case EventCode.OrderPickupOneManMadeSignNotTakeByNotComplete:
            //        name = "未完成，人工标识未取";
            //        break;
            //    case EventCode.OrderPickupOneManMadeSignNotTakeByComplete:
            //        name = "完成，人工标识未取";
            //        break;
            //    case EventCode.OrderPickupOneSysMadeSignTake:
            //        name = "系统标识已取";
            //        break;
            //    case EventCode.MachineHandleRunEx:
            //        name = "机器异常处理";
            //        break;
            //    case EventCode.AdminUserAdd:
            //        name = "新建管理账号";
            //        break;
            //    case EventCode.AdminUserEdit:
            //        name = "保存管理账号信息";
            //        break;
            //    case EventCode.AdSpaceRelease:
            //        name = "广告发布";
            //        break;
            //    case EventCode.AdSpaceDeleteAdContent:
            //        name = "广告删除";
            //        break;
            //    case EventCode.MachineEdit:
            //        name = "保存机器信息";
            //        break;
            //    case EventCode.PrdKindAdd:
            //        name = "新建商品分类";
            //        break;
            //    case EventCode.PrdKindEdit:
            //        name = "保存商品分类信息";
            //        break;
            //    case EventCode.PrdProductAdd:
            //        name = "新建商品";
            //        break;
            //    case EventCode.PrdProductEdit:
            //        name = "保存商品信息";
            //        break;
            //    case EventCode.StoreAdd:
            //        name = "新建店铺";
            //        break;
            //    case EventCode.StoreEdit:
            //        name = "保存店铺信息";
            //        break;
            //    case EventCode.StoreAddMachine:
            //        name = "机器绑定店铺";
            //        break;
            //    case EventCode.StoreRemoveMachine:
            //        name = "机器解绑店铺";
            //        break;
            //}

            //return name;
        }

        public static string GetEventLevel(string eventCode)
        {
            string name = string.Empty;

            try
            {
                var atts = typeof(EventCode).GetField(eventCode).GetCustomAttributes(typeof(EventCodeRemarkAttribute), false);
                if (atts != null)
                {
                    if (atts.Length > 0)
                    {
                        var att = (EventCodeRemarkAttribute)atts[0];

                        name = att.Level;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return name;
        }
    }
}
