using LocalS.BLL;
using LocalS.BLL.Biz;
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



            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.Source = E_OrderSource.Machine;
            bizRop.StoreId = machine.StoreId;
            bizRop.SellChannelRefType = E_SellChannelRefType.Machine;
            bizRop.SellChannelRefIds = new string[] { machine.Id };//指定机器

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
                if (ret_Biz.Data.Status == E_OrderStatus.Payed)
                {
                    ret.Data.OrderDetails = GetOrderDetails(rup.MachineId, rup.OrderId);
                }
            }

            return ret;
        }

        public CustomJsonResult Cancle(RopOrderCancle rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            result = LocalS.BLL.Biz.BizFactory.Order.Cancle(GuidUtil.Empty(), rop.OrderId, rop.Reason);

            return result;
        }

        public CustomJsonResult Search(RupOrderSearch rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            Order order = null;

            if (!string.IsNullOrEmpty(rup.PickCode))
            {
                order = CurrentDb.Order.Where(m => m.PickCode == rup.PickCode).FirstOrDefault();
            }

            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单，请重新输入");
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", GetOrderDetails(rup.MachineId, order.Id));
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

            using (TransactionScope ts = new TransactionScope())
            {
                var orderPickupLog = new OrderPickupLog();
                orderPickupLog.Id = GuidUtil.New();
                orderPickupLog.OrderId = rop.OrderId;
                orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                orderPickupLog.SellChannelRefId = rop.MachineId;
                orderPickupLog.UniqueId = rop.UniqueId;
                orderPickupLog.ProductSkuId = rop.ProductSkuId;
                orderPickupLog.SlotId = rop.SlotId;
                orderPickupLog.EventCode = rop.EventCode;
                orderPickupLog.EventRemark = rop.EventRemark;
                orderPickupLog.CreateTime = DateTime.Now;
                orderPickupLog.Creator = rop.MachineId;
                CurrentDb.OrderPickupLog.Add(orderPickupLog);

                var orderDetailsChildSon = CurrentDb.OrderDetailsChildSon.Where(m => m.Id == rop.UniqueId).FirstOrDefault();
                if (orderDetailsChildSon != null)
                {
                    switch (rop.EventCode)
                    {
                        case "1000":
                            orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.SendPick;
                            break;
                        case "2000":
                            orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.Picking;
                            break;
                        case "3000":
                            orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.Completed;
                            break;
                        case "4000":
                            orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.Exception;
                            break;
                    }
                }

                var orderDetailsChildSonsNoCompeleteCount = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == rop.OrderId && m.Status != E_OrderDetailsChildSonStatus.Completed).Count();

                if (orderDetailsChildSonsNoCompeleteCount == 0)
                {
                    var order = CurrentDb.Order.Where(m => m.Id == rop.OrderId).FirstOrDefault();
                    if (order != null)
                    {
                        order.Status = E_OrderStatus.Completed;
                        order.CompletedTime = DateTime.Now;

                        var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == rop.OrderId).ToList();
                        foreach (var orderDetail in orderDetails)
                        {
                            orderDetail.Status = E_OrderStatus.Completed;
                            orderDetail.CompletedTime = DateTime.Now;

                            var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == rop.OrderId).ToList();

                            foreach (var orderDetailsChild in orderDetails)
                            {
                                orderDetailsChild.Status = E_OrderStatus.Completed;
                                orderDetailsChild.CompletedTime = DateTime.Now;
                            }
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            }

            return result;
        }

        public CustomJsonResult BuildPayParams(RopOrderBuildPayParams rop)
        {
            LocalS.BLL.Biz.RopOrderBuildPayParams bizRop = new LocalS.BLL.Biz.RopOrderBuildPayParams();
            bizRop.OrderId = rop.OrderId;
            bizRop.PayWay = rop.PayWay;
            bizRop.PayCaller = rop.PayCaller;

            return BLL.Biz.BizFactory.Order.BuildPayParams(GuidUtil.Empty(), bizRop);
        }

        private RetOrderDetails GetOrderDetails(string machineId, string orderId)
        {
            var ret = new RetOrderDetails();

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();
            var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == orderId && m.SellChannelRefId == machineId && m.SellChannelRefType == E_SellChannelRefType.Machine).ToList();
            var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == orderId).ToList();

            ret.OrderId = order.Id;
            ret.OrderSn = order.Sn;

            foreach (var orderDetailsChild in orderDetailsChilds)
            {
                var sku = new RetOrderDetails.ProductSku();
                sku.Id = orderDetailsChild.PrdProductSkuId;
                sku.Name = orderDetailsChild.PrdProductSkuName;
                sku.MainImgUrl = orderDetailsChild.PrdProductSkuMainImgUrl;
                sku.Quantity = orderDetailsChild.Quantity;


                var l_orderDetailsChildSons = orderDetailsChildSons.Where(m => m.OrderDetailsChildId == orderDetailsChild.Id && m.PrdProductSkuId == orderDetailsChild.PrdProductSkuId).ToList();

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


    }
}
