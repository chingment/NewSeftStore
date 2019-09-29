using LocalS.BLL;
using LocalS.Entity;
using Lumos;
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

            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.Source = E_OrderSource.Machine;
            bizRop.StoreId = machine.StoreId;
            bizRop.ReserveMode = E_ReserveMode.OffLine;
            bizRop.SellChannelRefId = machine.Id;
            bizRop.SellChannelRefType = E_SellChannelRefType.Machine;
            bizRop.PayWay = rop.PayWay;
            bizRop.PayCaller = rop.PayCaller;

            foreach (var productSku in rop.ProductSkus)
            {
                bizRop.ProductSkus.Add(new LocalS.BLL.Biz.RopOrderReserve.ProductSku() { Id = productSku.Id, Quantity = productSku.Quantity, ReceptionMode = E_ReceptionMode.Machine });
            }

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(machine.MerchId, bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                RetOrderReserve ret = new RetOrderReserve();
                ret.OrderId = bizResult.Data.OrderId;
                ret.OrderSn = bizResult.Data.OrderSn;
                ret.PayUrl = bizResult.Data.PayUrl;
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
                ret.Data.OrderId = ret_Biz.Data.OrderId;
                ret.Data.OrderSn = ret_Biz.Data.OrderSn;
                ret.Data.Status = ret_Biz.Data.Status;
                ret.Data.OrderDetails = GetOrderDetails(rup.MachineId, rup.OrderId);
            }

            return ret;
        }

        public CustomJsonResult Cancle(RopOrderCancle rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            result = LocalS.BLL.Biz.BizFactory.Order.Cancle(GuidUtil.Empty(), rop.OrderId, rop.Reason);

            return result;
        }

        private RetOrderDetails GetOrderDetails(string machineId, string orderId)
        {
            var ret = new RetOrderDetails();

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();
            var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == orderId).ToList();
            var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == orderId).ToList();

            ret.OrderSn = order.Sn;

            foreach (var orderDetailsChild in orderDetailsChilds)
            {
                var sku = new RetOrderDetails.ProductSku();
                sku.Id = orderDetailsChild.PrdProductSkuId;
                sku.Name = orderDetailsChild.PrdProductSkuName;
                sku.MainImgUrl = orderDetailsChild.PrdProductSkuMainImgUrl;
                sku.Quantity = orderDetailsChild.Quantity;


                var l_orderDetailsChildSons = orderDetailsChildSons.Where(m => m.PrdProductSkuId == orderDetailsChild.PrdProductSkuId).ToList();

                sku.QuantityBySuccess = l_orderDetailsChildSons.Where(m => m.Status == E_OrderDetailsChildSonStatus.Completed).Count();

                foreach (var orderDetailsChildSon in l_orderDetailsChildSons)
                {
                    var slot = new RetOrderDetails.Slot();
                    slot.UniqueId = orderDetailsChildSon.Id;
                    slot.SlotId = orderDetailsChildSon.SlotId;
                    slot.Status = orderDetailsChildSon.Status;

                    sku.Slots.Add(slot);
                }

                ret.ProductSkus.Add(sku);
            }

            return ret;
        }

        public CustomJsonResult Details(RupOrderDetails rup)
        {
            CustomJsonResult result = new CustomJsonResult();
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", GetOrderDetails(rup.MachineId, rup.OrderId));
            return result;
        }

        public CustomJsonResult PickupStatusQuery(RupOrderPickupStatusQuery rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderPickupStatusQuery();


            var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == rup.OrderId).ToList();
            var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == rup.OrderId).ToList();


            foreach (var orderDetailsChild in orderDetailsChilds)
            {
                var sku = new RetOrderPickupStatusQuery.ProductSku();
                sku.Id = orderDetailsChild.PrdProductSkuId;
                sku.Name = orderDetailsChild.PrdProductSkuName;
                sku.MainImgUrl = orderDetailsChild.PrdProductSkuMainImgUrl;
                sku.Quantity = orderDetailsChild.Quantity;


                var l_orderDetailsChildSons = orderDetailsChildSons.Where(m => m.PrdProductSkuId == orderDetailsChild.PrdProductSkuId).ToList();

                sku.QuantityBySuccess = l_orderDetailsChildSons.Where(m => m.Status == E_OrderDetailsChildSonStatus.Completed).Count();
                sku.QuantityByException = l_orderDetailsChildSons.Where(m => m.Status == E_OrderDetailsChildSonStatus.Exception).Count();
                foreach (var orderDetailsChildSon in l_orderDetailsChildSons)
                {
                    var slot = new RetOrderPickupStatusQuery.Slot();
                    slot.UniqueId = orderDetailsChildSon.Id;
                    slot.SlotId = orderDetailsChildSon.SlotId;
                    slot.Status = orderDetailsChildSon.Status;
                    sku.Slots.Add(slot);
                }

                ret.ProductSkus.Add(sku);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult PickupEventNotify(RopOrderPickupEventNotify rop)
        {
            CustomJsonResult result = new CustomJsonResult();

          

            return result;
        }

    }
}
