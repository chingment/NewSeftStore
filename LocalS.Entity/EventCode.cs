using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Entity
{
    public static class EventCode
    {
        [EventCodeRemark("A", "系统登录")]
        public const string Login = "Login";
        [EventCodeRemark("A", "系统退出")]
        public const string Logout = "Logout";
        [EventCodeRemark("D", "心跳包")]
        public const string MachineStatus = "machine_status";
        [EventCodeRemark("A", "机器扫描货道")]
        public const string ScanSlots = "ScanSlots";
        [EventCodeRemark("A", "机器商品取货")]
        public const string Pickup = "Pickup";
        [EventCodeRemark("D", "机器商品测试取货")]
        public const string PickupTest = "PickupTest";
        [EventCodeRemark("A", "库存改变-机器货道商品初始化")]
        public const string MachineCabinetSlotInit = "MachineCabinetSlotInit";
        [EventCodeRemark("A", "库存改变-机器货道商品替换")]
        public const string MachineCabinetSlotReplace = "MachineCabinetSlotReplace";
        [EventCodeRemark("A", "库存改变-机器货道商品保存")]
        public const string MachineCabinetSlotSave = "MachineCabinetSlotSave";
        [EventCodeRemark("A", "库存改变-机器货道商品移除")]
        public const string MachineCabinetSlotRemove = "MachineCabinetSlotRemove";
        [EventCodeRemark("A", "库存改变-机器货道商品调整库存数量")]
        public const string MachineCabinetSlotAdjustStockQuantity = "MachineCabinetSlotAdjustStockQuantity";
        [EventCodeRemark("A", "机器商品库存查看")]
        public const string MachineCabinetGetSlots = "MachineCabinetGetSlots";
        [EventCodeRemark("A", "机器货道保存扫成结果")]
        public const string MachineCabinetSaveRowColLayout = "MachineCabinetSaveRowColLayout";
        [EventCodeRemark("A", "机器货道商品调整价格")]
        public const string MachineAdjustStockSalePrice = "MachineAdjustStockSalePrice";
        [EventCodeRemark("A", "机器处理运行异常信息")]
        public const string MachineHandleRunEx = "MachineHandleRunEx";
        [EventCodeRemark("B", "库存改变-订单商品取货未完成人工标记未取状态")]
        public const string StockOrderPickupOneManMadeSignNotTakeByNotComplete = "StockOrderPickupOneManMadeSignNotTakeByNotComplete";
        [EventCodeRemark("B", "库存改变-订单商品取货已完成人工标记未取状态")]
        public const string StockOrderPickupOneManMadeSignNotTakeByComplete = "StockOrderPickupOneManMadeSignNotTakeByComplete";
        [EventCodeRemark("B", "库存改变-订单商品取货系统标记已取")]
        public const string StockOrderPickupOneSysMadeSignTake = "StockOrderPickupOneSysMadeSignTake";
        [EventCodeRemark("B", "库存改变-订单商品取货未取货完成系统标记已取")]
        public const string StockOrderPickupOneManMadeSignTakeByNotComplete = "StockOrderPickupOneManMadeSignTakeByNotComplete";
        [EventCodeRemark("B", "库存改变-订单取消")]
        public const string StockOrderCancle = "StockOrderCancle";
        [EventCodeRemark("B", "库存改变-订单支付成功")]
        public const string StockOrderPaySuccess = "StockOrderPaySuccess";
        [EventCodeRemark("B", "库存改变-订单预定成功")]
        public const string StockOrderReserveSuccess = "StockOrderReserveSuccess";
        [EventCodeRemark("A", "订单取消")]
        public const string OrderCancle = "OrderCancle";
        [EventCodeRemark("A", "订单支付成功")]
        public const string OrderPaySuccess = "OrderPaySuccess";
        [EventCodeRemark("A", "订单预定成功")]
        public const string OrderReserveSuccess = "OrderReserveSuccess";
        [EventCodeRemark("A", "订单异常处理")]
        public const string OrderHandleExOrder = "OrderHandleExOrder";
        [EventCodeRemark("A", "新增管理账号信息")]
        public const string AdminUserAdd = "AdminUserAdd";
        [EventCodeRemark("A", "修改管理账号信息")]
        public const string AdminUserEdit = "AdminUserEdit";
        [EventCodeRemark("A", "修改客户账号信息")]
        public const string ClientUserEdit = "ClientUserEdit";
        [EventCodeRemark("A", "发布广告")]
        public const string AdSpaceRelease = "AdSpaceRelease";
        [EventCodeRemark("A", "删除广告")]
        public const string AdSpaceDeleteAdContent = "AdSpaceDeleteAdContent";
        [EventCodeRemark("A", "保存机器信息")]
        public const string MachineEdit = "MachineEdit";
        [EventCodeRemark("A", "解绑门店机器")]
        public const string MachineUnBindShop = "MachineUnBindShop";
        [EventCodeRemark("A", "绑定门店机器")]
        public const string MachineBindShop = "MachineBindShop";
        [EventCodeRemark("A", "保存机器货道库存")]
        public const string MachineAdjustStockQuantity = "MachineAdjustStockQuantity";
        [EventCodeRemark("A", "新增商品分类信息")]
        public const string PrdKindAdd = "PrdKindAdd";
        [EventCodeRemark("A", "修改商品分类信息")]
        public const string PrdKindEdit = "PrdKindEdit";
        [EventCodeRemark("A", "删除商品分类信息")]
        public const string PrdKindDelete = "PrdKindDelete";
        [EventCodeRemark("A", "新增商品信息")]
        public const string PrdProductAdd = "PrdProductAdd";
        [EventCodeRemark("A", "修改商品信息")]
        public const string PrdProductEdit = "PrdProductEdit";
        [EventCodeRemark("A", "新增店铺信息")]
        public const string StoreAdd = "StoreAdd";
        [EventCodeRemark("A", "修改店铺信息")]
        public const string StoreEdit = "StoreEdit";
        [EventCodeRemark("A", "店铺添加门店")]
        public const string StoreAddShop = "StoreAddShop";
        [EventCodeRemark("A", "店铺移除门店")]
        public const string StoreRemoveShop = "StoreRemoveShop";
        [EventCodeRemark("A", "保存店铺分类")]
        public const string StoreSaveKind = "StoreSaveKind";
        [EventCodeRemark("A", "移除店铺分类")]
        public const string StoreRemoveKind = "StoreRemoveKind";
        [EventCodeRemark("A", "保存门店信息")]
        public const string ShopSave = "ShopSave";
        [EventCodeRemark("A", "店铺绑定机器")]
        public const string StoreAddMachine = "StoreAddMachine";
        [EventCodeRemark("A", "店铺移除机器")]
        public const string StoreRemoveMachine = "StoreRemoveMachine";
        [EventCodeRemark("B", "更新机器库存信息命令")]
        public const string MCmdUpdateProductSkuStock = "MCmdUpdateProductSkuStock";
        [EventCodeRemark("C", "更新机器首页广告命令")]
        public const string MCmdUpdateHomeBanners = "MCmdUpdateHomeBanners";
        [EventCodeRemark("C", "更新机器LOGO命令")]
        public const string MCmdUpdateHomeLogo = "MCmdUpdateHomeLogo";
        [EventCodeRemark("A", "重启系统命令")]
        public const string MCmdSysReboot = "MCmdSysReboot";
        [EventCodeRemark("A", "关闭系统命令")]
        public const string MCmdSysShutdown = "MCmdSysShutdown";
        [EventCodeRemark("A", "设置系统状态命令")]
        public const string MCmdSysSetStatus = "MCmdSysSetStatus";
        [EventCodeRemark("A", "打开DSX01设备取货门命令")]
        public const string MCmdDsx01OpenPickupDoor = "MCmdDsx01OpenPickupDoor";
        [EventCodeRemark("C", "支付成功命令")]
        public const string MCmdPaySuccess = "MCmdPaySuccess";
        [EventCodeRemark("A", "新建优惠券")]
        public const string CouponAdd = "CouponAdd";
        [EventCodeRemark("A", "修改优惠券信息")]
        public const string CouponEdit = "CouponEdit";

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
