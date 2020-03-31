using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Entity
{
    public static class EventCode
    {
        public const string Login = "Login";
        public const string Logout = "Logout";
        public const string HeartbeatBag = "HeartbeatBag";
        public const string ScanSlots = "ScanSlots";
        public const string Pickup = "Pickup";
        public const string MachineCabinetSlotSave = "MachineCabinetSlotSave";
        public const string MachineCabinetSlotRemove = "MachineCabinetSlotRemove";
        public const string MachineCabinetGetSlots = "MachineCabinetGetSlots";
        public const string MachineSaveCabinetRowColLayout = "MachineSaveCabinetRowColLayout";
        public const string AdjustStockQuantity = "AdjustStockQuantity";
        public const string AdjustStockSalePrice = "AdjustStockSalePrice";
        public const string OrderPickupOneManMadeSignNotTakeByNotComplete = "OrderPickupOneManMadeSignNotTakeByNotComplete";
        public const string OrderPickupOneManMadeSignNotTakeByComplete = "OrderPickupOneManMadeSignNotTakeByComplete";
        public const string OrderPickupOneSysMadeSignTake = "OrderPickupOneSysMadeSignTake";
        public const string OrderPickupOneManMadeSignTakeByNotComplete = "OrderPickupOneManMadeSignTakeByNotComplete";
        public const string OrderCancle = "OrderCancle";
        public const string OrderReserveSuccess = "OrderReserveSuccess";

        public static string GetEventName(string eventCode)
        {
            string name = "";
            switch (eventCode)
            {
                case EventCode.Login:
                    name = "登录";
                    break;
                case EventCode.Logout:
                    name = "退出";
                    break;
                case EventCode.HeartbeatBag:
                    name = "心跳包";
                    break;
                case EventCode.ScanSlots:
                    name = "扫描货道";
                    break;
                case EventCode.Pickup:
                    name = "商品取货";
                    break;
                case EventCode.AdjustStockQuantity:
                    name = "商品库存调整";
                    break;
                case EventCode.AdjustStockSalePrice:
                    name = "商品价格调整";
                    break;
                case EventCode.MachineCabinetSlotSave:
                    name = "商品货道保存";
                    break;
                case EventCode.MachineCabinetSlotRemove:
                    name = "商品货道移除";
                    break;
                case EventCode.MachineCabinetGetSlots:
                    name = "查看商品货道";
                    break;
                case EventCode.OrderCancle:
                    name = "订单取消";
                    break;
                case EventCode.OrderReserveSuccess:
                    name = "订单预定成功";
                    break;
                case EventCode.OrderPickupOneManMadeSignNotTakeByNotComplete:
                    name = "未完成，人工标识未取";
                    break;
                case EventCode.OrderPickupOneManMadeSignNotTakeByComplete:
                    name = "完成，人工标识未取";
                    break;
                case EventCode.OrderPickupOneSysMadeSignTake:
                    name = "系统标识已取";
                    break;
            }

            return name;
        }
    }
}
