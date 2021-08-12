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
        public const string login = "login";
        [EventCodeRemark("A", "系统退出")]
        public const string logout = "logout";
        [EventCodeRemark("D", "设备心跳包")]
        public const string device_status = "device_status";
        [EventCodeRemark("A", "设备扫描货道")]
        public const string vending_scan_slots = "vending_scan_slots";
        [EventCodeRemark("D", "设备商品取货")]
        public const string vending_pickup = "vending_pickup";
        [EventCodeRemark("D", "设备商品测试取货")]
        public const string vending_pickup_test = "vending_pickup_test";
        [EventCodeRemark("A", "设备货道商品保存")]
        public const string device_save_slot = "device_save_slot";
        [EventCodeRemark("A", "设备货道商品补货")]
        public const string device_slot_rsh = "device_slot_rsh";
        [EventCodeRemark("A", "设备货道商品移除")]
        public const string device_remove_slot = "device_remove_slot";
        [EventCodeRemark("A", "设备商品库存查看")]
        public const string device_saw_stock = "device_saw_stock";
        [EventCodeRemark("A", "设备货道商品调整价格")]
        public const string device_adjust_sale_price = "device_adjust_sale_price";
        [EventCodeRemark("A", "订单商品取货未完成人工标记未取状态")]
        public const string order_nocomplete_sign_notake = "order_nocomplete_sign_notake";
        [EventCodeRemark("A", "订单商品取货已完成人工标记未取状态")]
        public const string order_complete_sign_notake = "order_complete_sign_notake";
        [EventCodeRemark("A", "订单商品取货系统标记已取")]
        public const string order_sign_take = "order_sign_take";
        [EventCodeRemark("A", "订单商品取货未取货完成系统标记已取")]
        public const string order_nocomplete_sign_take = "order_nocomplete_sign_take";
        [EventCodeRemark("A", "订单取消")]
        public const string order_cancle = "order_cancle";
        [EventCodeRemark("A", "订单支付成功")]
        public const string order_pay_success = "order_pay_success";
        [EventCodeRemark("A", "订单预定成功")]
        public const string order_reserve_success = "order_reserve_success";
        [EventCodeRemark("A", "订单异常处理")]
        public const string order_handle_exception = "order_handle_exception";
        [EventCodeRemark("A", "新增管理账号信息")]
        public const string adminuser_add = "adminuser_add";
        [EventCodeRemark("A", "修改管理账号信息")]
        public const string adminuser_edit = "adminuser_edit";
        [EventCodeRemark("A", "修改客户账号信息")]
        public const string clientuser_edit = "clientuser_edit";
        [EventCodeRemark("A", "发布广告")]
        public const string ad_release = "ad_release";
        [EventCodeRemark("A", "设置广告状态")]
        public const string ad_set_status = "ad_set_status";
        [EventCodeRemark("A", "保存设备信息")]
        public const string device_edit = "device_edit";
        [EventCodeRemark("A", "导出Excel")]
        public const string export_excel = "export_excel";
        [EventCodeRemark("A", "解绑门店设备")]
        public const string device_unbind_shop = "device_unbind_shop";
        [EventCodeRemark("A", "绑定门店设备")]
        public const string device_bind_shop = "device_bind_shop";
        [EventCodeRemark("A", "保存设备货道库存")]
        public const string device_adjust_stock_quantity = "device_adjust_stock_quantity";
        [EventCodeRemark("A", "新增商品信息")]
        public const string product_add = "product_add";
        [EventCodeRemark("A", "修改商品信息")]
        public const string product_edit = "product_edit";
        [EventCodeRemark("A", "新增店铺信息")]
        public const string store_add = "store_add";
        [EventCodeRemark("A", "修改店铺信息")]
        public const string store_edit = "store_edit";
        [EventCodeRemark("A", "店铺添加门店")]
        public const string store_add_shop = "store_add_shop";
        [EventCodeRemark("A", "店铺移除门店")]
        public const string store_remove_shop = "store_remove_shop";
        [EventCodeRemark("A", "保存店铺分类")]
        public const string store_save_kind = "store_save_kind";
        [EventCodeRemark("A", "移除店铺分类")]
        public const string store_remove_kind = "store_remove_kind";
        [EventCodeRemark("A", "保存门店信息")]
        public const string shop_save = "shop_save";
        [EventCodeRemark("A", "店铺绑定设备")]
        public const string store_bind_device = "store_bind_device";
        [EventCodeRemark("A", "店铺移除设备")]
        public const string store_remove_device = "store_remove_device";
        [EventCodeRemark("A", "新建优惠券")]
        public const string coupon_add = "coupon_add";
        [EventCodeRemark("A", "修改优惠券信息")]
        public const string coupon_edit = "coupon_edit";
        [EventCodeRemark("A", "退款申请")]
        public const string pay_refund_apply = "pay_refund_apply";
        [EventCodeRemark("A", "退款处理")]
        public const string pay_refund_handle = "pay_refund_handle";

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
