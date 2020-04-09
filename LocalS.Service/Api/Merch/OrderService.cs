using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
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
        public StatusModel GetExStatus(bool isHasEx, bool isHandleComplete)
        {
            var statusModel = new StatusModel();

            if (isHasEx)
            {
                if (isHandleComplete)
                {
                    statusModel.Value = 0;
                    statusModel.Text = "异常，已处理";
                }
                else
                {
                    statusModel.Value = 2;
                    statusModel.Text = "异常，未处理";
                }
            }
            else
            {
                statusModel.Value = 0;
                statusModel.Text = "否";
            }
            //switch (status)
            //{
            //    case E_AdContentStatus.Normal:
            //        statusModel.Value = 1;
            //        statusModel.Text = "正常";
            //        break;
            //    case E_AdContentStatus.Deleted:
            //        statusModel.Value = 2;
            //        statusModel.Text = "已删除";
            //        break;
            //}


            return statusModel;
        }

        public bool GetCanHandleEx(bool isHappen, bool isHandle)
        {
            if (isHappen && isHandle == false)
                return true;

            return false;
        }

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

        public StatusModel GetPickupStatus(E_OrderPickupStatus pickupStatus)
        {
            var status = new StatusModel();

            switch (pickupStatus)
            {
                case E_OrderPickupStatus.Submitted:
                    status.Value = 1000;
                    status.Text = "已提交";
                    break;
                case E_OrderPickupStatus.WaitPay:
                    status.Value = 2000;
                    status.Text = "待支付";
                    break;
                //case E_OrderDetailsChildSonStatus.Payed:
                //    status.Value = 3000;
                //    status.Text = "已支付";
                //    break;
                case E_OrderPickupStatus.WaitPickup:
                    status.Value = 3010;
                    status.Text = "待取货";
                    break;
                case E_OrderPickupStatus.SendPickupCmd:
                    status.Value = 3011;
                    status.Text = "取货中";
                    break;
                case E_OrderPickupStatus.Pickuping:
                    status.Value = 3012;
                    status.Text = "取货中";
                    break;
                case E_OrderPickupStatus.Taked:
                    status.Value = 4000;
                    status.Text = "已完成";
                    break;
                case E_OrderPickupStatus.Canceled:
                    status.Value = 5000;
                    status.Text = "已取消";
                    break;
                case E_OrderPickupStatus.Exception:
                    status.Value = 6000;
                    status.Text = "异常未处理";
                    break;
                case E_OrderPickupStatus.ExPickupSignTaked:
                    status.Value = 6010;
                    status.Text = "异常已处理，标记为已取货";
                    break;
                case E_OrderPickupStatus.ExPickupSignUnTaked:
                    status.Value = 6011;
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
                case E_OrderSource.Wxmp:
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
                         select new { o.Sn, o.Id, o.SellChannelRefIds, o.StoreId, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.StoreName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

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

            if (rup.IsHasEx)
            {
                query = query.Where(m => m.ExIsHappen == true && m.ExIsHandle == false);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == item.Id).ToList();

                List<object> sellChannelDetails = new List<object>();
                foreach (var orderDetail in orderSub)
                {
                    List<object> pickupSkus = new List<object>();
                    switch (orderDetail.SellChannelRefType)
                    {
                        case E_SellChannelRefType.Machine:

                            var prderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == item.Id).OrderByDescending(m => m.PickupStartTime).ToList();


                            foreach (var prderSubChildUnique in prderSubChildUniques)
                            {
                                var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == prderSubChildUnique.Id).OrderByDescending(m => m.CreateTime).ToList();

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
                                    Id = prderSubChildUnique.PrdProductSkuId,
                                    MainImgUrl = prderSubChildUnique.PrdProductSkuMainImgUrl,
                                    UniqueId = prderSubChildUnique.Id,
                                    ExPickupIsHandle = prderSubChildUnique.ExPickupIsHandle,
                                    Name = prderSubChildUnique.PrdProductSkuName,
                                    Quantity = prderSubChildUnique.Quantity,
                                    Status = GetPickupStatus(prderSubChildUnique.PickupStatus),
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
                    ExStatus = GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
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
            ret.CanHandleEx = GetCanHandleEx(order.ExIsHappen, order.ExIsHandle);
            ret.ExHandleRemark = order.ExHandleRemark;
            ret.ExIsHappen = order.ExIsHappen;
            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).ToList();

            List<RetOrderDetails.SellChannelDetail> sellChannelDetails = new List<RetOrderDetails.SellChannelDetail>();
            foreach (var orderSub in orderSubs)
            {
                var sellChannelDetail = new RetOrderDetails.SellChannelDetail();

                switch (orderSub.SellChannelRefType)
                {
                    case E_SellChannelRefType.Machine:
                        sellChannelDetail.Type = E_SellChannelRefType.Machine;
                        sellChannelDetail.Name = orderSub.SellChannelRefName;
                        sellChannelDetail.DetailType = 1;

                        var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderSubId == orderSub.Id).OrderByDescending(m => m.PickupStartTime).ToList();
                        var pickupSkus = new List<RetOrderDetails.PickupSku>();
                        foreach (var orderSubChildUnique in orderSubChildUniques)
                        {
                            var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChildUnique.Id).OrderByDescending(m => m.CreateTime).ToList();

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
                                Id = orderSubChildUnique.PrdProductSkuId,
                                ExPickupIsHandle = orderSubChildUnique.ExPickupIsHandle,
                                UniqueId = orderSubChildUnique.Id,
                                MainImgUrl = orderSubChildUnique.PrdProductSkuMainImgUrl,
                                Name = orderSubChildUnique.PrdProductSkuName,
                                Quantity = orderSubChildUnique.Quantity,
                                Status = GetPickupStatus(orderSubChildUnique.PickupStatus),
                                PickupLogs = pickupLogs,
                                SignStatus = 0
                            });
                        }

                        ret.SellChannelDetails.Add(sellChannelDetail);
                        break;
                }
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;

        }

        public CustomJsonResult HandleExOrder(string operater, string merchId, RopOrderHandleExOrder rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(rop.Id))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "订单ID不能为空");
                }

                if (rop.UniqueItems.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单要处理的信息为空");
                }

                var order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == rop.Id).FirstOrDefault();
                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单信息不存在");
                }

                if (!order.ExIsHappen)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不是异常状态，不能处理");
                }

                if (order.ExIsHandle)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该异常订单已经处理完毕");
                }


                var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == rop.Id && m.ExIsHappen == true && m.ExIsHandle == false).ToList();

                var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == rop.Id && m.ExPickupIsHappen == true && m.ExPickupIsHandle == false && m.PickupStatus == E_OrderPickupStatus.Exception).ToList();

                foreach (var orderSubChildUnique in orderSubChildUniques)
                {
                    var detailItem = rop.UniqueItems.Where(m => m.UniqueId == orderSubChildUnique.Id).FirstOrDefault();
                    if (detailItem == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单里对应商品异常记录未找到");
                    }

                    if (detailItem.SignStatus != 1 && detailItem.SignStatus != 2)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不能处理该异常状态:" + detailItem.SignStatus);
                    }

                    if (detailItem.SignStatus == 1)
                    {
                        orderSubChildUnique.ExPickupIsHandle = true;
                        orderSubChildUnique.ExPickupHandleTime = DateTime.Now;
                        orderSubChildUnique.ExPickupHandleSign = E_OrderExPickupHandleSign.Taked;
                        orderSubChildUnique.PickupStatus = E_OrderPickupStatus.ExPickupSignTaked;

                        BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderPickupOneManMadeSignTakeByNotComplete, orderSubChildUnique.MerchId, orderSubChildUnique.StoreId, orderSubChildUnique.SellChannelRefId, orderSubChildUnique.CabinetId, orderSubChildUnique.SlotId, orderSubChildUnique.PrdProductSkuId, 1);

                        var orderPickupLog = new OrderPickupLog();
                        orderPickupLog.Id = GuidUtil.New();
                        orderPickupLog.OrderId = orderSubChildUnique.OrderId;
                        orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                        orderPickupLog.SellChannelRefId = orderSubChildUnique.SellChannelRefId;
                        orderPickupLog.UniqueId = orderSubChildUnique.Id;
                        orderPickupLog.PrdProductSkuId = orderSubChildUnique.PrdProductSkuId;
                        orderPickupLog.SlotId = orderSubChildUnique.SlotId;
                        orderPickupLog.Status = E_OrderPickupStatus.Taked;
                        orderPickupLog.IsPickupComplete = true;
                        orderPickupLog.ActionRemark = "人为标识已取货";
                        orderPickupLog.Remark = "";
                        orderPickupLog.CreateTime = DateTime.Now;
                        orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(orderPickupLog);
                    }
                    else if (detailItem.SignStatus == 2)
                    {
                        orderSubChildUnique.ExPickupIsHandle = true;
                        orderSubChildUnique.ExPickupHandleTime = DateTime.Now;
                        orderSubChildUnique.ExPickupHandleSign = E_OrderExPickupHandleSign.UnTaked;
                        orderSubChildUnique.PickupStatus = E_OrderPickupStatus.ExPickupSignUnTaked;

                        BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderPickupOneManMadeSignNotTakeByNotComplete, orderSubChildUnique.MerchId, orderSubChildUnique.StoreId, orderSubChildUnique.SellChannelRefId, orderSubChildUnique.CabinetId, orderSubChildUnique.SlotId, orderSubChildUnique.PrdProductSkuId, 1);

                        var orderPickupLog = new OrderPickupLog();
                        orderPickupLog.Id = GuidUtil.New();
                        orderPickupLog.OrderId = orderSubChildUnique.OrderId;
                        orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                        orderPickupLog.SellChannelRefId = orderSubChildUnique.SellChannelRefId;
                        orderPickupLog.UniqueId = orderSubChildUnique.Id;
                        orderPickupLog.PrdProductSkuId = orderSubChildUnique.PrdProductSkuId;
                        orderPickupLog.SlotId = orderSubChildUnique.SlotId;
                        orderPickupLog.Status = E_OrderPickupStatus.Taked;
                        orderPickupLog.IsPickupComplete = false;
                        orderPickupLog.ActionRemark = "人为标识未取货";
                        orderPickupLog.Remark = "";
                        orderPickupLog.CreateTime = DateTime.Now;
                        orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(orderPickupLog);
                    }
                }

                foreach (var orderSub in orderSubs)
                {
                    orderSub.ExIsHandle = true;
                    orderSub.ExHandleTime = DateTime.Now;
                }

                order.ExIsHandle = true;
                order.ExHandleTime = DateTime.Now;
                order.ExHandleRemark = rop.Remark;
                order.CompletedTime = DateTime.Now;
                order.Status = E_OrderStatus.Completed;
                order.CompletedTime = DateTime.Now;


                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, order.MerchId, order.StoreId, "", EventCode.OrderHandleExOrder, string.Format("处理异常订单号:{0},备注:{1}", order.Sn, order.ExHandleRemark));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理成功");
            }

            return result;
        }


    }
}
