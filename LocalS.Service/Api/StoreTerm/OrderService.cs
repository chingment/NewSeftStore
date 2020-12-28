using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.StoreTerm
{

    public class OrderService : BaseDbContext
    {
        public CustomJsonResult Reserve(RopOrderReserve rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }

            if (string.IsNullOrEmpty(machine.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(machine.StoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户店铺");
            }

            if (machine.RunStatus != E_MachineRunStatus.Running)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器在维护状态");
            }

            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.AppId = AppId.STORETERM;
            bizRop.Source = E_OrderSource.Machine;
            bizRop.StoreId = machine.StoreId;
            bizRop.IsTestMode = machine.IsTestMode;


            OrderReserveBlockModel block = new OrderReserveBlockModel();

            block.ReceiveMode = E_ReceiveMode.MachineSelfTake;
            block.SelfTake.Id = machine.StoreId;
            block.SelfTake.MarkName = machine.StoreName;
            block.SelfTake.Address = machine.StoreAddress;
            foreach (var productSku in rop.ProductSkus)
            {
                block.Skus.Add(new LocalS.BLL.Biz.OrderReserveBlockModel.ProductSkuModel() { Id = productSku.ProductSkuId, Quantity = productSku.Quantity, ShopMode = E_SellChannelRefType.Machine, SellChannelRefIds = new string[] { machine.MachineId }, SvcConsulterId = productSku.SvcConsulterId });
            }

            bizRop.Blocks.Add(block);

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(IdWorker.Build(IdType.EmptyGuid), bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                var order = bizResult.Data.Orders[0];
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", new { OrderId = order.Id, ChargeAmount = order.ChargeAmount });
            }
            else
            {
                result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, bizResult.Message);
            }


            return result;

        }

        public CustomJsonResult<RetOrderPayStatusQuery> PayStatusQuery(RupOrderPayStatusQuery rup)
        {
            CustomJsonResult<RetOrderPayStatusQuery> ret = new CustomJsonResult<RetOrderPayStatusQuery>();


            var ret_Biz = LocalS.BLL.Biz.BizFactory.Order.PayTransResultQuery(rup.MachineId, rup.PayTransId);

            ret.Result = ret_Biz.Result;
            ret.Code = ret_Biz.Code;
            ret.Message = ret_Biz.Message;

            if (ret_Biz.Data != null)
            {
                ret.Data = new RetOrderPayStatusQuery();
                ret.Data.PayTransId = ret_Biz.Data.PayTransId;
                ret.Data.PayStatus = ret_Biz.Data.PayStatus;
                if (ret_Biz.Data.PayStatus == E_PayStatus.PaySuccess)
                {
                    ret.Data.OrderId = ret_Biz.Data.OrderIds[0];
                    ret.Data.ProductSkus = BizFactory.Order.GetOrderProductSkuByPickup(ret_Biz.Data.OrderIds[0], rup.MachineId);
                }
            }

            return ret;
        }

        public CustomJsonResult Cancle(RopOrderCancle rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            result = LocalS.BLL.Biz.BizFactory.Order.Cancle(IdWorker.Build(IdType.EmptyGuid), rop.OrderId, rop.Type, rop.Reason);

            return result;
        }

        public CustomJsonResult SearchByPickupCode(RupOrderSearchByPickupCode rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rup.PickupCode))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效取货码");
            }
            LogUtil.Info("PickupCode=>>" + rup.PickupCode);

            string pickupCode = "";
            if (rup.PickupCode.IndexOf("pickupcode@v1:") > -1)
            {
                pickupCode = rup.PickupCode.Split(':')[1];
            }
            else if (rup.PickupCode.IndexOf("pickupcode@v2:") > -1)
            {
                pickupCode = BizFactory.Order.DecodeQrcode2PickupCode(rup.PickupCode);
            }
            else
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效取货码");
            }

            LogUtil.Info("PickupCode2=>>" + pickupCode);

            var order = CurrentDb.Order.Where(m => m.SellChannelRefId == rup.MachineId && m.PickupCode == pickupCode).FirstOrDefault();

            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单，请重新输入");
            }

            if (order.PayStatus != E_PayStatus.PaySuccess)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单未支付");
            }

            if (order.PickupIsTrg)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经取货");
            }

            var ret = new RetOrderSearchByPickupCode();

            ret.OrderId = order.Id;


            ret.ProductSkus = BizFactory.Order.GetOrderProductSkuByPickup(order.Id, rup.MachineId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;
        }

        public CustomJsonResult PickupStatusQuery(RupOrderPickupStatusQuery rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderPickupStatusQuery();

            var orderSub = CurrentDb.OrderSub.Where(m => m.Id == rup.UniqueId).FirstOrDefault();

            if (orderSub != null)
            {
                ret.ProductSkuId = orderSub.PrdProductId;
                ret.SlotId = orderSub.SlotId;
                ret.UniqueId = orderSub.Id;
                ret.Status = orderSub.PickupStatus;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            }

            return result;
        }

        public CustomJsonResult BuildPayParams(RopOrderBuildPayParams rop)
        {
            LocalS.BLL.Biz.RopOrderBuildPayParams bizRop = new LocalS.BLL.Biz.RopOrderBuildPayParams();
            bizRop.OrderIds.Add(rop.OrderId);
            bizRop.PayCaller = rop.PayCaller;
            bizRop.PayPartner = rop.PayPartner;
            bizRop.CreateIp = rop.CreateIp;
            return BLL.Biz.BizFactory.Order.BuildPayParams(IdWorker.Build(IdType.EmptyGuid), bizRop);
        }

    }
}
