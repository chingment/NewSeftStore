using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{

    public class OrderService : BaseService
    {
        public IResult2 Reserve(string merchId, RopOrderReserve rop)
        {
            var result = new CustomJsonResult2();

            if (string.IsNullOrEmpty(rop.device_id))
            {
                return new CustomJsonResult2(ResultCode.Failure, "设备Id不能为空");
            }

            var d_machine = CurrentDb.Machine.Where(m => m.Id == rop.device_id).FirstOrDefault();

            if (d_machine == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "机器未登记");
            }

            if (string.IsNullOrEmpty(d_machine.CurUseMerchId))
            {
                return new CustomJsonResult2(ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(d_machine.CurUseStoreId))
            {
                return new CustomJsonResult2(ResultCode.Failure, "机器未绑定店铺");
            }

            if (string.IsNullOrEmpty(d_machine.CurUseShopId))
            {
                return new CustomJsonResult2(ResultCode.Failure, "机器未绑定门店");
            }

            if (d_machine.RunStatus != E_MachineRunStatus.Running)
            {
                return new CustomJsonResult2(ResultCode.Failure, "机器在维护状态");
            }


            if (string.IsNullOrEmpty(rop.low_order_id))
            {
                return new CustomJsonResult2(ResultCode.Failure, "商户订单编号不能为空");
            }

            if (string.IsNullOrEmpty(rop.low_order_id))
            {
                return new CustomJsonResult2(ResultCode.Failure, "商户订单号不能为空");
            }

            if (string.IsNullOrEmpty(rop.notify_url))
            {
                return new CustomJsonResult2(ResultCode.Failure, "通知URL不能为空");
            }

            var shop = CurrentDb.Shop.Where(m => m.Id == d_machine.CurUseShopId).FirstOrDefault();

            if (shop == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "门店信息异常");
            }

            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.AppId = AppId.STORETERM;
            bizRop.Source = E_OrderSource.Api;
            bizRop.StoreId = d_machine.CurUseStoreId;
            bizRop.ShopMethod = E_ShopMethod.Buy;
            bizRop.IsTestMode = d_machine.IsTestMode;
            bizRop.CumOrderId = rop.low_order_id;

            LocalS.BLL.Biz.RopOrderReserve.BlockModel block = new LocalS.BLL.Biz.RopOrderReserve.BlockModel();

            block.ReceiveMode = E_ReceiveMode.SelfTakeByMachine;
            block.SelfTake.Mark.Id = d_machine.CurUseShopId;
            block.SelfTake.Mark.Name = shop.Name;
            block.SelfTake.Mark.Address = shop.Address;
            block.SelfTake.Mark.AreaCode = shop.AreaCode;
            block.SelfTake.Mark.AreaName = shop.AreaName;

            foreach (var detail in rop.detail)
            {

                if (string.IsNullOrEmpty(detail.sku_id) && string.IsNullOrEmpty(detail.sku_cum_code))
                    return new CustomJsonResult2(ResultCode.Failure, "sku_id,sku_cum_code 不能同时为空");

                if (string.IsNullOrEmpty(detail.sku_id))
                {
                    var d_Sku = CurrentDb.PrdSku.Where(m => m.CumCode == detail.sku_cum_code && m.MerchId == merchId).FirstOrDefault();
                    if (d_Sku == null)
                    {
                        return new CustomJsonResult2(ResultCode.Failure, "商品编码不存在");
                    }
                    else
                    {
                        detail.sku_id = d_Sku.Id;
                    }
                }

                block.Skus.Add(new LocalS.BLL.Biz.RopOrderReserve.BlockModel.SkuModel() { Id = detail.sku_id, Quantity = detail.quantity, ShopMode = E_ShopMode.Machine, ShopId = d_machine.CurUseShopId, MachineIds = new string[] { rop.device_id }, SvcConsulterId = "" });
            }

            bizRop.Blocks.Add(block);

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(IdWorker.Build(IdType.EmptyGuid), bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                var order = bizResult.Data.Orders[0];

                result = new CustomJsonResult2(ResultCode.Success, "", new { low_order_id = order.CumId, up_order_id = order.Id });
            }
            else
            {
                result = new CustomJsonResult2(ResultCode.Failure, bizResult.Message);
            }


            return result;
        }

        public IResult2 Cancle(string merchId, RopOrderCancle rop)
        {
            var result = BizFactory.Order.Cancle(IdWorker.Build(IdType.EmptyGuid), null, rop.low_order_id, E_OrderCancleType.PayCancle, "订单支付取消");

            return new CustomJsonResult2(result.Code, result.Message, result.Data);
        }

        public IResult2 Query(string merchId, RopOrderQuery rop)
        {
            var result = new CustomJsonResult2();

            var d_Order = CurrentDb.Order.Where(m => m.CumId == rop.low_order_id).FirstOrDefault();

            if (d_Order == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "该商户订单号信息不存在");
            }

            var d_OrderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_Order.Id).ToList();

            var sku_Ships = new List<object>();

            foreach (var item in d_OrderSubs)
            {
                sku_Ships.Add(new
                {
                    unique_id = item.Id,
                    cabinet_id = item.CabinetId,
                    slot_id = item.SlotId,
                    sku_id = item.SkuId,
                    sku_cum_code = item.SkuCumCode,
                    status = item.PickupStatus,
                    tips = item.ExPickupReason,
                });
            }

            var ret = new
            {
                low_order_id = d_Order.CumId,
                up_order_id = d_Order.Id,
                business_type = "shipment",
                detail = new
                {
                    sku_ships = sku_Ships
                }
            };

            result = new CustomJsonResult2(ResultCode.Success, "", ret);

            return result;
        }
    }
}
