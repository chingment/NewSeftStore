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

    public class OrderService : BaseService
    {
        public CustomJsonResult Reserve(RopOrderReserve rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();

            if (d_Device == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未登记");
            }

            if (string.IsNullOrEmpty(d_Device.CurUseMerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定商户");
            }

            if (string.IsNullOrEmpty(d_Device.CurUseStoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定店铺");
            }

            if (string.IsNullOrEmpty(d_Device.CurUseShopId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定门店");
            }

            if (d_Device.RunStatus != E_DeviceRunStatus.Running)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备在维护状态");
            }

            var shop = CurrentDb.Shop.Where(m => m.Id == d_Device.CurUseShopId).FirstOrDefault();

            if (shop == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "门店信息异常");
            }

            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.AppId = AppId.STORETERM;
            bizRop.Source = E_OrderSource.Device;
            bizRop.StoreId = d_Device.CurUseStoreId;
            bizRop.ShopMethod = E_ShopMethod.Buy;
            bizRop.IsTestMode = d_Device.IsTestMode;

            LocalS.BLL.Biz.RopOrderReserve.BlockModel block = new LocalS.BLL.Biz.RopOrderReserve.BlockModel();

            block.ReceiveMode = E_ReceiveMode.SelfTakeByDevice;
            block.SelfTake.Mark.Id = d_Device.CurUseShopId;
            block.SelfTake.Mark.Name = shop.Name;
            block.SelfTake.Mark.Address = shop.Address;
            block.SelfTake.Mark.AreaCode = shop.AreaCode;
            block.SelfTake.Mark.AreaName = shop.AreaName;

            foreach (var sku in rop.Skus)
            {
                block.Skus.Add(new LocalS.BLL.Biz.RopOrderReserve.BlockModel.SkuModel() { Id = sku.SkuId, Quantity = sku.Quantity, ShopMode = E_ShopMode.Device, ShopId = d_Device.CurUseShopId, DeviceIds = new string[] { rop.DeviceId }, SvcConsulterId = sku.SvcConsulterId });
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

        public CustomJsonResult<RetOrderPayStatusQuery> PayStatusQuery(RopOrderPayStatusQuery rup)
        {
            CustomJsonResult<RetOrderPayStatusQuery> ret = new CustomJsonResult<RetOrderPayStatusQuery>();


            var ret_Biz = LocalS.BLL.Biz.BizFactory.Order.PayTransResultQuery(rup.DeviceId, rup.PayTransId);

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
                    ret.Data.Skus = BizFactory.Order.GetOrderSkuByPickup(ret_Biz.Data.OrderIds[0], rup.DeviceId);
                }
            }

            return ret;
        }

        public CustomJsonResult Cancle(RopOrderCancle rop)
        {

            var result = LocalS.BLL.Biz.BizFactory.Order.Cancle(IdWorker.Build(IdType.EmptyGuid), rop.OrderId, "", rop.Type, rop.Reason);

            return result;
        }

        public CustomJsonResult SearchByPickupCode(RopOrderSearchByPickupCode rup)
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
                pickupCode = MyDESCryptoUtil.DecodeQrcode2PickupCode(rup.PickupCode);
            }
            else
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效取货码");
            }

            LogUtil.Info("PickupCode2=>>" + pickupCode);

            var order = CurrentDb.Order.Where(m => m.DeviceId == rup.DeviceId && m.PickupCode == pickupCode).FirstOrDefault();

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


            ret.Skus = BizFactory.Order.GetOrderSkuByPickup(order.Id, rup.DeviceId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;
        }

        public CustomJsonResult PickupStatusQuery(RopOrderPickupStatusQuery rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderPickupStatusQuery();

            var orderSub = CurrentDb.OrderSub.Where(m => m.Id == rup.UniqueId).FirstOrDefault();

            if (orderSub != null)
            {
                ret.SkuId = orderSub.SkuId;
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
