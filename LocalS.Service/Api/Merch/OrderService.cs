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

namespace LocalS.Service.Api.Merch
{
    public class OrderService : BaseDbContext
    {
        public StatusModel GetStatus(E_OrderStatus orderStatus)
        {
            var status = new StatusModel();

            switch (orderStatus)
            {
                case E_OrderStatus.Submitted:
                    status.Value = 1000;
                    status.Text = "已提交";
                    break;
                case E_OrderStatus.WaitPay:
                    status.Value = 2000;
                    status.Text = "待支付";
                    break;
                case E_OrderStatus.Payed:
                    status.Value = 3000;
                    status.Text = "已支付";
                    break;
                case E_OrderStatus.Completed:
                    status.Value = 4000;
                    status.Text = "已完成";
                    break;
                case E_OrderStatus.Canceled:
                    status.Value = 5000;
                    status.Text = "已取消";
                    break;
            }
            return status;
        }

        public StatusModel GetSonStatus(E_OrderDetailsChildSonStatus orderStatus)
        {
            var status = new StatusModel();

            switch (orderStatus)
            {
                case E_OrderDetailsChildSonStatus.Submitted:
                    status.Value = 1000;
                    status.Text = "已提交";
                    break;
                case E_OrderDetailsChildSonStatus.WaitPay:
                    status.Value = 2000;
                    status.Text = "待支付";
                    break;
                //case E_OrderDetailsChildSonStatus.Payed:
                //    status.Value = 3000;
                //    status.Text = "已支付";
                //    break;
                case E_OrderDetailsChildSonStatus.WaitPick:
                    status.Value = 3010;
                    status.Text = "待取货";
                    break;
                case E_OrderDetailsChildSonStatus.SendPick:
                    status.Value = 3011;
                    status.Text = "取货中";
                    break;
                case E_OrderDetailsChildSonStatus.Picking:
                    status.Value = 3012;
                    status.Text = "取货中";
                    break;
                case E_OrderDetailsChildSonStatus.Completed:
                    status.Value = 4000;
                    status.Text = "已完成";
                    break;
                case E_OrderDetailsChildSonStatus.Canceled:
                    status.Value = 5000;
                    status.Text = "已取消";
                    break;
                case E_OrderDetailsChildSonStatus.Exception:
                    status.Value = 6000;
                    status.Text = "异常未处理";
                    break;
                case E_OrderDetailsChildSonStatus.ExPickupSignTaked:
                    status.Value = 6010;
                    status.Text = "异常已处理，标记为已取货";
                    break;
                case E_OrderDetailsChildSonStatus.ExPickupSignUnTaked:
                    status.Value = 6010;
                    status.Text = "异常已处理，标记为未取货";
                    break;
            }
            return status;
        }

        public string GetSourceName(E_OrderSource orderSource)
        {
            string name = "";
            switch (orderSource)
            {
                case E_OrderSource.Api:
                    name = "开放接口";
                    break;
                case E_OrderSource.WechatMiniProgram:
                    name = "微信小程序";
                    break;
                case E_OrderSource.Machine:
                    name = "终端机器";
                    break;
            }
            return name;
        }

        public string GetPickImgUrl(string imgId)
        {
            if (string.IsNullOrEmpty(imgId))
                return null;


            return string.Format("http://file.17fanju.com/upload/pickup/{0}.jpg", imgId);
        }

        public CustomJsonResult GetList(string operater, string merchId, RupOrderGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.Order
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         (rup.OrderSn == null || o.Sn.Contains(rup.OrderSn)) &&
                         o.MerchId == merchId
                         select new { o.Sn, o.Id, o.SellChannelRefIds, o.StoreId, o.ClientUserId, o.ClientUserName, o.StoreName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

            if (rup.OrderStauts != Entity.E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.OrderStauts);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
            }

            if (!string.IsNullOrEmpty(rup.MachineId))
            {
                query = query.Where(m => m.SellChannelRefIds.Contains(rup.MachineId));
            }

            if (!string.IsNullOrEmpty(rup.ClientUserId))
            {
                query = query.Where(m => m.ClientUserId == rup.ClientUserId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == item.Id).ToList();

                List<object> sellChannelDetails = new List<object>();
                foreach (var orderDetail in orderDetails)
                {
                    List<object> pickupSkus = new List<object>();
                    switch (orderDetail.SellChannelRefType)
                    {
                        case E_SellChannelRefType.Machine:

                            var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == item.Id).ToList();


                            foreach (var orderDetailsChildSon in orderDetailsChildSons)
                            {
                                var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderDetailsChildSon.Id).OrderByDescending(m => m.CreateTime).ToList();

                                List<object> pickupLogs = new List<object>();

                                foreach (var orderPickupLog in orderPickupLogs)
                                {
                                    string imgUrl = GetPickImgUrl(orderPickupLog.ImgId);
                                    List<string> imgUrls = new List<string>();
                                    if (!string.IsNullOrEmpty(imgUrl))
                                    {
                                        imgUrls.Add(imgUrl);
                                    }

                                    pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                                }

                                pickupSkus.Add(new
                                {
                                    Id = orderDetailsChildSon.PrdProductSkuId,
                                    MainImgUrl = orderDetailsChildSon.PrdProductSkuMainImgUrl,
                                    UniqueId = orderDetailsChildSon.Id,
                                    ExPickupIsHandled = orderDetailsChildSon.ExPickupIsHandled,
                                    Name = orderDetailsChildSon.PrdProductSkuName,
                                    Quantity = orderDetailsChildSon.Quantity,
                                    Status = GetSonStatus(orderDetailsChildSon.Status),
                                    PickupLogs = pickupLogs
                                });
                            }

                            sellChannelDetails.Add(new
                            {
                                Name = orderDetail.SellChannelRefName,
                                Type = orderDetail.SellChannelRefType,
                                DetailType = 1,
                                DetailItems = pickupSkus
                            });

                            break;
                    }
                }

                olist.Add(new
                {
                    Id = item.Id,
                    Sn = item.Sn,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    SubmitTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = GetStatus(item.Status),
                    SourceName = GetSourceName(item.Source),
                    SellChannelDetails = sellChannelDetails
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetDetails(string operater, string merchId, string orderId)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderDetails();

            var order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == orderId).FirstOrDefault();
            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            ret.Id = order.Id;
            ret.Sn = order.Sn;
            ret.ClientUserName = order.ClientUserName;
            ret.ClientUserId = order.ClientUserId;
            ret.StoreName = order.StoreName;
            ret.SubmitTime = order.SubmittedTime.ToUnifiedFormatDateTime();
            ret.ChargeAmount = order.ChargeAmount.ToF2Price();
            ret.DiscountAmount = order.DiscountAmount.ToF2Price();
            ret.OriginalAmount = order.OriginalAmount.ToF2Price();
            ret.Quantity = order.Quantity;
            ret.CreateTime = order.CreateTime.ToUnifiedFormatDateTime();
            ret.Status = GetStatus(order.Status);
            ret.SourceName = GetSourceName(order.Source);



            var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).ToList();

            List<RetOrderDetails.SellChannelDetail> sellChannelDetails = new List<RetOrderDetails.SellChannelDetail>();
            foreach (var orderDetail in orderDetails)
            {
                var sellChannelDetail = new RetOrderDetails.SellChannelDetail();

                switch (orderDetail.SellChannelRefType)
                {
                    case E_SellChannelRefType.Machine:
                        sellChannelDetail.Type = E_SellChannelRefType.Machine;
                        sellChannelDetail.Name = orderDetail.SellChannelRefName;
                        sellChannelDetail.DetailType = 1;

                        var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderDetailsId == orderDetail.Id).ToList();
                        var pickupSkus = new List<RetOrderDetails.PickupSku>();
                        foreach (var orderDetailsChildSon in orderDetailsChildSons)
                        {
                            var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderDetailsChildSon.Id).OrderByDescending(m => m.CreateTime).ToList();

                            List<RetOrderDetails.PickupLog> pickupLogs = new List<RetOrderDetails.PickupLog>();

                            foreach (var orderPickupLog in orderPickupLogs)
                            {
                                string imgUrl = GetPickImgUrl(orderPickupLog.ImgId);
                                List<string> imgUrls = new List<string>();
                                if (!string.IsNullOrEmpty(imgUrl))
                                {
                                    imgUrls.Add(imgUrl);
                                }

                                pickupLogs.Add(new RetOrderDetails.PickupLog { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                            }

                            sellChannelDetail.DetailItems.Add(new RetOrderDetails.PickupSku
                            {
                                Id = orderDetailsChildSon.PrdProductSkuId,
                                UniqueId = orderDetailsChildSon.Id,
                                ExPickupIsHandled = orderDetailsChildSon.ExPickupIsHandled,
                                MainImgUrl = orderDetailsChildSon.PrdProductSkuMainImgUrl,
                                Name = orderDetailsChildSon.PrdProductSkuName,
                                Quantity = orderDetailsChildSon.Quantity,
                                Status = GetSonStatus(orderDetailsChildSon.Status),
                                PickupLogs = pickupLogs,
                            });
                        }

                        ret.SellChannelDetails.Add(sellChannelDetail);
                        break;
                }
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;

        }

        public CustomJsonResult PickupExceptionHandle(string operater, string merchId, RopOrderPickupExceptionHandle rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var orderDetailsChildSon = CurrentDb.OrderDetailsChildSon.Where(m => m.Id == rop.UniqueId).FirstOrDefault();

                if (orderDetailsChildSon == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该取货物品");
                }

                if (orderDetailsChildSon.ExPickupIsHandled)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已经标识过");
                }


                E_OrderDetailsChildSonStatus old_Status = orderDetailsChildSon.Status;


                var orderPickupLog = new OrderPickupLog();

                switch (rop.HandleMethod)
                {
                    case RopOrderPickupExceptionHandle.ExceptionHandleMethod.SignTaked:
                        orderDetailsChildSon.ExPickupIsHandled = true;
                        orderDetailsChildSon.ExPickupHandleSign = E_OrderDetailsChildSonExPickupHandleSign.Taked;
                        orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.ExPickupSignTaked;

                        if (old_Status == E_OrderDetailsChildSonStatus.Exception)
                        {
                            BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderPickupOneManMadeSignTakeByNotComplete, orderDetailsChildSon.MerchId, orderDetailsChildSon.StoreId, orderDetailsChildSon.SellChannelRefId, orderDetailsChildSon.SlotId, orderDetailsChildSon.PrdProductSkuId, 1);

                            orderPickupLog.Id = GuidUtil.New();
                            orderPickupLog.OrderId = orderDetailsChildSon.OrderId;
                            orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                            orderPickupLog.SellChannelRefId = orderDetailsChildSon.SellChannelRefId;
                            orderPickupLog.UniqueId = rop.UniqueId;
                            orderPickupLog.PrdProductSkuId = orderDetailsChildSon.PrdProductSkuId;
                            orderPickupLog.SlotId = orderDetailsChildSon.SlotId;
                            orderPickupLog.Status = E_OrderDetailsChildSonStatus.Completed;
                            orderPickupLog.IsPickupComplete = true;
                            orderPickupLog.ActionRemark = "人为标识已取货";
                            orderPickupLog.Remark = rop.Remark;
                            orderPickupLog.CreateTime = DateTime.Now;
                            orderPickupLog.Creator = operater;
                            CurrentDb.OrderPickupLog.Add(orderPickupLog);
                        }

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "标识成功");
                        break;
                    case RopOrderPickupExceptionHandle.ExceptionHandleMethod.SignUnTaked:
                        orderDetailsChildSon.ExPickupIsHandled = true;
                        orderDetailsChildSon.ExPickupHandleSign = E_OrderDetailsChildSonExPickupHandleSign.UnTaked;
                        orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.ExPickupSignUnTaked;
                        if (old_Status == E_OrderDetailsChildSonStatus.Completed)
                        {
                            BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderPickupOneManMadeSignNotTakeByComplete, orderDetailsChildSon.MerchId, orderDetailsChildSon.StoreId, orderDetailsChildSon.SellChannelRefId, orderDetailsChildSon.SlotId, orderDetailsChildSon.PrdProductSkuId, 1);

                            orderPickupLog.Id = GuidUtil.New();
                            orderPickupLog.OrderId = orderDetailsChildSon.OrderId;
                            orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                            orderPickupLog.SellChannelRefId = orderDetailsChildSon.SellChannelRefId;
                            orderPickupLog.UniqueId = rop.UniqueId;
                            orderPickupLog.PrdProductSkuId = orderDetailsChildSon.PrdProductSkuId;
                            orderPickupLog.SlotId = orderDetailsChildSon.SlotId;
                            orderPickupLog.Status = E_OrderDetailsChildSonStatus.Completed;
                            orderPickupLog.IsPickupComplete = false;
                            orderPickupLog.ActionRemark = "人为标识未取货";
                            orderPickupLog.Remark = rop.Remark;
                            orderPickupLog.CreateTime = DateTime.Now;
                            orderPickupLog.Creator = operater;
                            CurrentDb.OrderPickupLog.Add(orderPickupLog);
                        }
                        else if (old_Status == E_OrderDetailsChildSonStatus.Exception)
                        {
                            BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderPickupOneManMadeSignNotTakeByNotComplete, orderDetailsChildSon.MerchId, orderDetailsChildSon.StoreId, orderDetailsChildSon.SellChannelRefId, orderDetailsChildSon.SlotId, orderDetailsChildSon.PrdProductSkuId, 1);

                            orderPickupLog.Id = GuidUtil.New();
                            orderPickupLog.OrderId = orderDetailsChildSon.OrderId;
                            orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                            orderPickupLog.SellChannelRefId = orderDetailsChildSon.SellChannelRefId;
                            orderPickupLog.UniqueId = rop.UniqueId;
                            orderPickupLog.PrdProductSkuId = orderDetailsChildSon.PrdProductSkuId;
                            orderPickupLog.SlotId = orderDetailsChildSon.SlotId;
                            orderPickupLog.Status = E_OrderDetailsChildSonStatus.Completed;
                            orderPickupLog.IsPickupComplete = false;
                            orderPickupLog.ActionRemark = "人为标识未取货";
                            orderPickupLog.Remark = rop.Remark;
                            orderPickupLog.CreateTime = DateTime.Now;
                            orderPickupLog.Creator = operater;
                            CurrentDb.OrderPickupLog.Add(orderPickupLog);
                        }


                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "标识成功");
                        break;

                }

                CurrentDb.SaveChanges();

                var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.ToList();
                var orderDetailsChildSonsCompeleteCount = orderDetailsChildSons.Where(m => m.Status == E_OrderDetailsChildSonStatus.Completed || m.Status == E_OrderDetailsChildSonStatus.ExPickupSignTaked || m.Status == E_OrderDetailsChildSonStatus.ExPickupSignUnTaked).Count();
                //判断全部订单都是已完成
                if (orderDetailsChildSonsCompeleteCount == orderDetailsChildSons.Count)
                {
                    var order = CurrentDb.Order.Where(m => m.Id == orderDetailsChildSon.OrderId).FirstOrDefault();
                    if (order != null)
                    {
                        order.Status = E_OrderStatus.Completed;
                        order.CompletedTime = DateTime.Now;
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();
            }


            return result;
        }
    }
}
