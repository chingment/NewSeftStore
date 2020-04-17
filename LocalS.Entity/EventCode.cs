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
        public const string MachineCabinetSlotAdjustStockQuantity = "MachineCabinetSlotAdjustStockQuantity";
        public const string MachineCabinetGetSlots = "MachineCabinetGetSlots";
        public const string MachineCabinetSaveRowColLayout = "MachineCabinetSaveRowColLayout";
        public const string MachineAdjustStockSalePrice = "MachineAdjustStockSalePrice";
        public const string MachineHandleRunEx = "MachineHandleRunEx";
        public const string OrderPickupOneManMadeSignNotTakeByNotComplete = "OrderPickupOneManMadeSignNotTakeByNotComplete";
        public const string OrderPickupOneManMadeSignNotTakeByComplete = "OrderPickupOneManMadeSignNotTakeByComplete";
        public const string OrderPickupOneSysMadeSignTake = "OrderPickupOneSysMadeSignTake";
        public const string OrderPickupOneManMadeSignTakeByNotComplete = "OrderPickupOneManMadeSignTakeByNotComplete";
        public const string OrderCancle = "OrderCancle";
        public const string OrderReserveSuccess = "OrderReserveSuccess";
        public const string OrderHandleExOrder = "OrderHandleExOrder";
        public const string AdminUserAdd = "AdminUserAdd";
        public const string AdminUserEdit = "AdminUserEdit";
        public const string AdSpaceRelease = "AdSpaceRelease";
        public const string AdSpaceDeleteAdContent = "AdSpaceDeleteAdContent";
        public const string MachineEdit = "MachineEdit";
        public const string PrdKindAdd = "PrdKindAdd";
        public const string PrdKindEdit = "PrdKindEdit";
        public const string PrdKindDelete = "PrdKindDelete";
        public const string PrdProductAdd = "PrdProductAdd";
        public const string PrdProductEdit = "PrdProductEdit";
        public const string StoreAdd = "StoreAdd";
        public const string StoreEdit = "StoreEdit";
        public const string StoreAddMachine = "StoreAddMachine";
        public const string StoreRemoveMachine = "StoreRemoveMachine";
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
                case EventCode.MachineCabinetSlotAdjustStockQuantity:
                    name = "商品库存调整";
                    break;
                case EventCode.MachineAdjustStockSalePrice:
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
                case EventCode.MachineHandleRunEx:
                    name = "机器异常处理";
                    break;
                case EventCode.AdminUserAdd:
                    name = "新建管理账号";
                    break;
                case EventCode.AdminUserEdit:
                    name = "保存管理账号信息";
                    break;
                case EventCode.AdSpaceRelease:
                    name = "广告发布";
                    break;
                case EventCode.AdSpaceDeleteAdContent:
                    name = "广告删除";
                    break;
                case EventCode.MachineEdit:
                    name = "保存机器信息";
                    break;
                case EventCode.PrdKindAdd:
                    name = "新建商品分类";
                    break;
                case EventCode.PrdKindEdit:
                    name = "保存商品分类信息";
                    break;
                case EventCode.PrdProductAdd:
                    name = "新建商品";
                    break;
                case EventCode.PrdProductEdit:
                    name = "保存商品信息";
                    break;
                case EventCode.StoreAdd:
                    name = "新建店铺";
                    break;
                case EventCode.StoreEdit:
                    name = "保存店铺信息";
                    break;
                case EventCode.StoreAddMachine:
                    name = "机器绑定店铺";
                    break;
                case EventCode.StoreRemoveMachine:
                    name = "机器解绑店铺";
                    break;
            }

            return name;
        }
    }
}
