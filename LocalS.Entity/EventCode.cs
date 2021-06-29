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
        [EventCodeRemark("D", "设备心跳包")]
        public const string DeviceStatus = "DeviceStatus";
        [EventCodeRemark("A", "设备扫描货道")]
        public const string DeviceScanSlot = "DeviceScanSlot";
        [EventCodeRemark("D", "设备商品取货")]
        public const string DevicePickup = "DevicePickup";
        [EventCodeRemark("D", "设备商品测试取货")]
        public const string DevicePickupTest = "DevicePickupTest";
        [EventCodeRemark("A", "设备货道商品保存")]
        public const string DeviceCabinetSlotSave = "DeviceCabinetSlotSave";
        [EventCodeRemark("A", "设备货道商品移除")]
        public const string DeviceCabinetSlotRemove = "DeviceCabinetSlotRemove";
        [EventCodeRemark("A", "设备商品库存查看")]
        public const string DeviceCabinetGetSlots = "DeviceCabinetGetSlots";
        [EventCodeRemark("A", "设备货道商品调整价格")]
        public const string DeviceAdjustStockSalePrice = "DeviceAdjustStockSalePrice";
        [EventCodeRemark("A", "订单商品取货未完成人工标记未取状态")]
        public const string OrderPickupOneManMadeSignNotTakeByNotComplete = "OrderPickupOneManMadeSignNotTakeByNotComplete";
        [EventCodeRemark("A", "订单商品取货已完成人工标记未取状态")]
        public const string OrderPickupOneManMadeSignNotTakeByComplete = "OrderPickupOneManMadeSignNotTakeByComplete";
        [EventCodeRemark("A", "订单商品取货系统标记已取")]
        public const string OrderPickupOneSysMadeSignTake = "OrderPickupOneSysMadeSignTake";
        [EventCodeRemark("A", "订单商品取货未取货完成系统标记已取")]
        public const string OrderPickupOneManMadeSignTakeByNotComplete = "OrderPickupOneManMadeSignTakeByNotComplete";
        [EventCodeRemark("A", "订单取消")]
        public const string OrderCancle = "OrderCancle";
        [EventCodeRemark("A", "订单支付成功")]
        public const string OrderPaySuccess = "OrderPaySuccess";
        [EventCodeRemark("A", "订单预定成功")]
        public const string OrderReserveSuccess = "OrderReserveSuccess";
        [EventCodeRemark("A", "订单异常处理")]
        public const string OrderHandleException = "OrderHandleException";
        [EventCodeRemark("A", "新增管理账号信息")]
        public const string AdminUserAdd = "AdminUserAdd";
        [EventCodeRemark("A", "修改管理账号信息")]
        public const string AdminUserEdit = "AdminUserEdit";
        [EventCodeRemark("A", "修改客户账号信息")]
        public const string ClientUserEdit = "ClientUserEdit";
        [EventCodeRemark("A", "发布广告")]
        public const string AdRelease = "AdRelease";
        [EventCodeRemark("A", "删除广告")]
        public const string AdDeleteContent = "AdDeleteContent";
        [EventCodeRemark("A", "保存设备信息")]
        public const string DeviceEdit = "DeviceEdit";
        [EventCodeRemark("A", "导出Excel")]
        public const string ExportExcel = "ExportExcel";
        [EventCodeRemark("A", "解绑门店设备")]
        public const string DeviceUnBindShop = "DeviceUnBindShop";
        [EventCodeRemark("A", "绑定门店设备")]
        public const string DeviceBindShop = "DeviceBindShop";
        [EventCodeRemark("A", "保存设备货道库存")]
        public const string DeviceAdjustStockQuantity = "DeviceAdjustStockQuantity";
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
        [EventCodeRemark("A", "店铺绑定设备")]
        public const string StoreAddDevice = "StoreAddDevice";
        [EventCodeRemark("A", "店铺移除设备")]
        public const string StoreRemoveDevice = "StoreRemoveDevice";
        [EventCodeRemark("A", "新建优惠券")]
        public const string CouponAdd = "CouponAdd";
        [EventCodeRemark("A", "修改优惠券信息")]
        public const string CouponEdit = "CouponEdit";
        [EventCodeRemark("B", "更新设备库存信息命令")]
        public const string MCmdUpdateSkuStock = "MCmdUpdateSkuStock";
        [EventCodeRemark("C", "更新设备广告")]
        public const string MCmdUpdateAds = "MCmdUpdateAds";
        [EventCodeRemark("C", "更新设备LOGO命令")]
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
            //    case EventCode.DeviceCabinetSlotAdjustStockQuantity:
            //        name = "商品库存调整";
            //        break;
            //    case EventCode.DeviceAdjustStockSalePrice:
            //        name = "商品价格调整";
            //        break;
            //    case EventCode.DeviceCabinetSlotSave:
            //        name = "商品货道保存";
            //        break;
            //    case EventCode.DeviceCabinetSlotRemove:
            //        name = "商品货道移除";
            //        break;
            //    case EventCode.DeviceCabinetGetSlots:
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
            //    case EventCode.DeviceHandleRunEx:
            //        name = "设备异常处理";
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
            //    case EventCode.DeviceEdit:
            //        name = "保存设备信息";
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
            //    case EventCode.StoreAddDevice:
            //        name = "设备绑定店铺";
            //        break;
            //    case EventCode.StoreRemoveDevice:
            //        name = "设备解绑店铺";
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
