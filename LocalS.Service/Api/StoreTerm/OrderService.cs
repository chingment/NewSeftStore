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
                    ret.Data.OrderDetails = BizFactory.Order.GetOrderDetails(rup.OrderId, rup.MachineId);
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

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", BizFactory.Order.GetOrderDetails(order.Id, rup.MachineId));
            return result;
        }

        public CustomJsonResult PickupStatusQuery(RupOrderPickupStatusQuery rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderPickupStatusQuery();

            var orderDetailsChildSon = CurrentDb.OrderDetailsChildSon.Where(m => m.Id == rup.UniqueId).FirstOrDefault();

            if (orderDetailsChildSon != null)
            {
                ret.ProductSkuId = orderDetailsChildSon.PrdProductId;
                ret.SlotId = orderDetailsChildSon.SlotId;
                ret.UniqueId = orderDetailsChildSon.Id;
                ret.Status = orderDetailsChildSon.Status;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            }

            return result;
        }

        public CustomJsonResult PickupEventNotify(RopOrderPickupEventNotify rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var orderDetailsChildSon = CurrentDb.OrderDetailsChildSon.Where(m => m.Id == rop.UniqueId).FirstOrDefault();
                if (orderDetailsChildSon != null)
                {
                    orderDetailsChildSon.LastPickupActionId = rop.ActionId;
                    orderDetailsChildSon.LastPickupActionStatusCode = rop.ActionStatusCode;
                    orderDetailsChildSon.Status = rop.Status;
                    CurrentDb.SaveChanges();

                    var orderDetailsChildSonsNoCompeleteCount = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == orderDetailsChildSon.OrderId && m.Status != E_OrderDetailsChildSonStatus.Completed).Count();

                    if (orderDetailsChildSonsNoCompeleteCount == 0)
                    {
                        var order = CurrentDb.Order.Where(m => m.Id == orderDetailsChildSon.OrderId).FirstOrDefault();
                        if (order != null)
                        {
                            order.Status = E_OrderStatus.Completed;
                            order.CompletedTime = DateTime.Now;

                            var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == orderDetailsChildSon.OrderId).ToList();
                            foreach (var orderDetail in orderDetails)
                            {
                                orderDetail.Status = E_OrderStatus.Completed;
                                orderDetail.CompletedTime = DateTime.Now;

                                var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == orderDetailsChildSon.OrderId).ToList();

                                foreach (var orderDetailsChild in orderDetailsChilds)
                                {
                                    orderDetailsChild.Status = E_OrderStatus.Completed;
                                    orderDetailsChild.CompletedTime = DateTime.Now;
                                }
                            }
                        }
                    }

                    var orderPickupLog = new OrderPickupLog();
                    orderPickupLog.Id = GuidUtil.New();
                    orderPickupLog.OrderId = orderDetailsChildSon.OrderId;
                    orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                    orderPickupLog.SellChannelRefId = rop.MachineId;
                    orderPickupLog.UniqueId = rop.UniqueId;
                    orderPickupLog.ProductSkuId = orderDetailsChildSon.PrdProductSkuId;
                    orderPickupLog.SlotId = orderDetailsChildSon.SlotId;
                    orderPickupLog.Status = rop.Status;
                    orderPickupLog.ActionId = rop.ActionId;
                    orderPickupLog.ActionName = rop.ActionName;
                    orderPickupLog.ActionStatusCode = rop.ActionStatusCode;
                    orderPickupLog.ActionStatusName = rop.ActionStatusName;
                    orderPickupLog.IsPickupComplete = rop.IsPickupComplete;
                    if (rop.IsPickupComplete)
                    {
                        orderPickupLog.ImgUrlByCHK = "http://file.17fanju.com/upload/common/" + rop.UniqueId + ".jpg";
                        orderPickupLog.PickupUseTime = rop.PickupUseTime;
                        orderPickupLog.ActionRemark = "取货完成";
                    }
                    else
                    {
                        if (rop.Status == E_OrderDetailsChildSonStatus.SendPick)
                        {
                            orderPickupLog.ActionRemark = "发送命令";
                        }
                        else
                        {
                            orderPickupLog.ActionRemark = rop.ActionName + rop.ActionStatusName;
                        }
                    }
                    orderPickupLog.Remark = rop.Remark;
                    orderPickupLog.CreateTime = DateTime.Now;
                    orderPickupLog.Creator = rop.MachineId;
                    CurrentDb.OrderPickupLog.Add(orderPickupLog);

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
            bizRop.PayCaller = rop.PayCaller;
            bizRop.PayPartner = rop.PayPartner;
            return BLL.Biz.BizFactory.Order.BuildPayParams(GuidUtil.Empty(), bizRop);
        }


    }
}
