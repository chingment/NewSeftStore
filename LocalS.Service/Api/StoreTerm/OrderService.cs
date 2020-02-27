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
            bizRop.Source = E_OrderSource.Machine;
            bizRop.StoreId = machine.StoreId;
            bizRop.SellChannelRefType = E_SellChannelRefType.Machine;
            bizRop.SellChannelRefIds = new string[] { machine.Id };//指定机器
            bizRop.IsTestMode = machine.IsTestMode;
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
                ret.Data.Id = ret_Biz.Data.OrderId;
                ret.Data.Sn = ret_Biz.Data.OrderSn;
                ret.Data.Status = ret_Biz.Data.Status;
                if (ret_Biz.Data.Status == E_OrderStatus.Payed)
                {
                    ret.Data.ProductSkus = BizFactory.Order.GetOrderProductSkuByPickup(rup.OrderId, rup.MachineId);
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

            if (string.IsNullOrEmpty(rup.PickupCode))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效取货码");
            }

            var order = CurrentDb.OrderSub.Where(m => m.PickupCode == rup.PickupCode).FirstOrDefault();

            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单，请重新输入");
            }

            result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效订单");
            //if (order.Status != E_OrderStatus.Payed)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效订单");
            //}

            // result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", BizFactory.Order.GetOrderDetailsByPickup(order.Id, rup.MachineId));
            return result;
        }

        public CustomJsonResult PickupStatusQuery(RupOrderPickupStatusQuery rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderPickupStatusQuery();

            var orderSubChildUnique = CurrentDb.OrderSubChildUnique.Where(m => m.Id == rup.UniqueId).FirstOrDefault();

            if (orderSubChildUnique != null)
            {
                ret.ProductSkuId = orderSubChildUnique.PrdProductId;
                ret.SlotId = orderSubChildUnique.SlotId;
                ret.UniqueId = orderSubChildUnique.Id;
                ret.Status = orderSubChildUnique.Status;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            }

            return result;
        }

        //public CustomJsonResult PickupEventNotify(RopOrderPickupEventNotify rop)
        //{
        //    CustomJsonResult result = new CustomJsonResult();

        //    using (TransactionScope ts = new TransactionScope())
        //    {
        //        var orderSubChildUnique = CurrentDb.OrderSubChildUnique.Where(m => m.Id == rop.UniqueId).FirstOrDefault();
        //        if (orderSubChildUnique != null)
        //        {
        //            orderSubChildUnique.LastPickupActionId = rop.ActionId;
        //            orderSubChildUnique.LastPickupActionStatusCode = rop.ActionStatusCode;
        //            orderSubChildUnique.Status = rop.Status;
        //            CurrentDb.SaveChanges();


        //            //如果某次取货异常 剩下所有取货都标识为订单取货异常
        //            var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == orderSubChildUnique.OrderId).ToList();

        //            if (rop.Status == E_OrderPickupStatus.Exception)
        //            {
        //                var order = CurrentDb.Order.Where(m => m.Id == orderSubChildUnique.OrderId).FirstOrDefault();
        //                if (order != null)
        //                {
        //                    order.ExIsHappen = true;
        //                    order.ExHappenTime = DateTime.Now;
        //                    CurrentDb.SaveChanges();
        //                }

        //                foreach (var item in orderSubChildUniques)
        //                {
        //                    if (item.Status != E_OrderPickupStatus.Completed && item.Status != E_OrderPickupStatus.Canceled)
        //                    {
        //                        item.Status = E_OrderPickupStatus.Exception;
        //                        item.ExPickupIsHappen = true;
        //                        item.ExPickupHappenTime = DateTime.Now;
        //                        CurrentDb.SaveChanges();
        //                    }
        //                }
        //            }


        //            var orderDetailsChildSonsCompeleteCount = orderSubChildUniques.Where(m => m.Status == E_OrderPickupStatus.Completed).Count();
        //            //判断全部订单都是已完成
        //            if (orderDetailsChildSonsCompeleteCount == orderSubChildUniques.Count)
        //            {
        //                var order = CurrentDb.Order.Where(m => m.Id == orderSubChildUnique.OrderId).FirstOrDefault();
        //                if (order != null)
        //                {
        //                    order.Status = E_OrderStatus.Completed;
        //                    order.CompletedTime = DateTime.Now;
        //                }
        //            }

        //            var orderPickupLog = new OrderPickupLog();
        //            orderPickupLog.Id = GuidUtil.New();
        //            orderPickupLog.OrderId = orderSubChildUnique.OrderId;
        //            orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
        //            orderPickupLog.SellChannelRefId = rop.MachineId;
        //            orderPickupLog.UniqueId = rop.UniqueId;
        //            orderPickupLog.PrdProductSkuId = orderSubChildUnique.PrdProductSkuId;
        //            orderPickupLog.SlotId = orderSubChildUnique.SlotId;
        //            orderPickupLog.Status = rop.Status;
        //            orderPickupLog.ActionId = rop.ActionId;
        //            orderPickupLog.ActionName = rop.ActionName;
        //            orderPickupLog.ActionStatusCode = rop.ActionStatusCode;
        //            orderPickupLog.ActionStatusName = rop.ActionStatusName;
        //            orderPickupLog.IsPickupComplete = rop.IsPickupComplete;
        //            orderPickupLog.ImgId = rop.ImgId;
        //            if (rop.IsPickupComplete)
        //            {
        //                orderPickupLog.PickupUseTime = rop.PickupUseTime;
        //                orderPickupLog.ActionRemark = "取货完成";

        //                BizFactory.ProductSku.OperateStockQuantity(rop.MachineId, OperateStockType.OrderPickupOneSysMadeSignTake, orderSubChildUnique.MerchId, orderSubChildUnique.StoreId, orderSubChildUnique.SellChannelRefId, orderSubChildUnique.SlotId, orderSubChildUnique.PrdProductSkuId, 1);
        //            }
        //            else
        //            {
        //                if (rop.Status == E_OrderPickupStatus.SendPickupCmd)
        //                {
        //                    orderPickupLog.ActionRemark = "发送命令";
        //                }
        //                else if (rop.Status == E_OrderPickupStatus.Exception)
        //                {
        //                    orderPickupLog.ActionRemark = "发生异常";
        //                }
        //                else
        //                {
        //                    orderPickupLog.ActionRemark = rop.ActionName + rop.ActionStatusName;
        //                }
        //            }

        //            orderPickupLog.Remark = rop.Remark;
        //            orderPickupLog.CreateTime = DateTime.Now;
        //            orderPickupLog.Creator = rop.MachineId;
        //            CurrentDb.OrderPickupLog.Add(orderPickupLog);

        //            MqFactory.Global.PushOperateLog(AppId.STORETERM, orderSubChildUnique.ClientUserId, rop.MachineId, "OrderPickup", orderSubChildUnique.PrdProductSkuName + "," + orderPickupLog.ActionRemark);

        //        }

        //        CurrentDb.SaveChanges();
        //        ts.Complete();

        //        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

        //    }

        //    return result;
        //}

        public CustomJsonResult BuildPayParams(RopOrderBuildPayParams rop)
        {
            LocalS.BLL.Biz.RopOrderBuildPayParams bizRop = new LocalS.BLL.Biz.RopOrderBuildPayParams();
            bizRop.OrderId = rop.OrderId;
            bizRop.PayCaller = rop.PayCaller;
            bizRop.PayPartner = rop.PayPartner;
            bizRop.CreateIp = rop.CreateIp;
            return BLL.Biz.BizFactory.Order.BuildPayParams(GuidUtil.Empty(), bizRop);
        }


        public CustomJsonResult GetExOrder(RupOrderGetExOrder rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderGetExOrder();

            string orderId = "";

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

            ret.OrderId = order.Id;
            ret.OrderSn = order.Sn;

            var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == orderId).ToList();

            foreach (var orderSubChildUnique in orderSubChildUniques)
            {
                var productSku = new RetOrderGetExOrder.ProductSku();
                productSku.Id = orderSubChildUnique.PrdProductId;
                productSku.UniqueId = orderSubChildUnique.Id;
                productSku.SlotId = orderSubChildUnique.SlotId;
                productSku.Quantity = orderSubChildUnique.Quantity;
                productSku.Name = orderSubChildUnique.PrdProductSkuName;
                productSku.MainImgUrl = orderSubChildUnique.PrdProductSkuMainImgUrl;

                if (orderSubChildUnique.Status == E_OrderPickupStatus.Completed)
                {
                    productSku.CanHandle = false;
                }
                else
                {
                    productSku.CanHandle = true;
                }

                ret.ProductSkus.Add(productSku);
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult HandleExOrder(RopOrderHandleOrder rop)
        {
            var result = new CustomJsonResult();





            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
