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

            block.ShopMode = E_SellChannelRefType.Machine;
            block.ReceiveMode = E_ReceiveMode.MachineSelfTake;
            block.SelfTake.StoreName = machine.StoreName;
            block.SelfTake.StoreAddress = machine.StoreAddress;
            foreach (var productSku in rop.ProductSkus)
            {
                block.Skus.Add(new LocalS.BLL.Biz.OrderReserveBlockModel.ProductSkuModel() { Id = productSku.Id, Quantity = productSku.Quantity, ShopMode = E_SellChannelRefType.Machine, SellChannelRefIds = new string[] { machine.Id }, SvcConsulterId = productSku.SvcConsulterId });
            }

            bizRop.Blocks.Add(block);

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(IdWorker.Build(IdType.EmptyGuid), bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                RetOrderReserve ret = new RetOrderReserve();
                ret.OrderId = bizResult.Data.OrderId;
                ret.ChargeAmount = bizResult.Data.ChargeAmount;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
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


            var ret_Biz = LocalS.BLL.Biz.BizFactory.Order.PayResultQuery(rup.MachineId, rup.OrderId);

            ret.Result = ret_Biz.Result;
            ret.Code = ret_Biz.Code;
            ret.Message = ret_Biz.Message;

            if (ret_Biz.Data != null)
            {
                ret.Data = new RetOrderPayStatusQuery();
                ret.Data.Id = ret_Biz.Data.OrderId;
                ret.Data.Status = ret_Biz.Data.Status;
                if (ret_Biz.Data.Status == E_OrderPayStatus.PaySuccess)
                {
                    ret.Data.ProductSkus = BizFactory.Order.GetOrderProductSkuByPickup(rup.OrderId, rup.MachineId);
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

            var orderSub = CurrentDb.OrderSub.Where(m => m.SellChannelRefId == rup.MachineId && m.PickupCode == pickupCode).FirstOrDefault();

            if (orderSub == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单，请重新输入");
            }

            if (orderSub.PayStatus != E_OrderPayStatus.PaySuccess)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单未支付");
            }

            if (orderSub.PickupIsTrg)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经取货");
            }

            var ret = new RetOrderSearchByPickupCode();

            ret.Id = orderSub.OrderId;


            ret.ProductSkus = BizFactory.Order.GetOrderProductSkuByPickup(orderSub.OrderId, rup.MachineId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;
        }

        public CustomJsonResult PickupStatusQuery(RupOrderPickupStatusQuery rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderPickupStatusQuery();

            var orderSubChild = CurrentDb.OrderSubChild.Where(m => m.Id == rup.UniqueId).FirstOrDefault();

            if (orderSubChild != null)
            {
                ret.ProductSkuId = orderSubChild.PrdProductId;
                ret.SlotId = orderSubChild.SlotId;
                ret.UniqueId = orderSubChild.Id;
                ret.Status = orderSubChild.PickupStatus;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            }

            return result;
        }

        public CustomJsonResult BuildPayParams(RopOrderBuildPayParams rop)
        {
            LocalS.BLL.Biz.RopOrderBuildPayParams bizRop = new LocalS.BLL.Biz.RopOrderBuildPayParams();
            bizRop.OrderId = rop.OrderId;
            bizRop.PayCaller = rop.PayCaller;
            bizRop.PayPartner = rop.PayPartner;
            bizRop.CreateIp = rop.CreateIp;
            return BLL.Biz.BizFactory.Order.BuildPayParams(IdWorker.Build(IdType.EmptyGuid), bizRop);
        }

    }
}
