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


namespace LocalS.Service.Api.Merch
{
    public class OrderSubService : BaseDbContext
    {
        public CustomJsonResult GetList(string operater, string merchId, RupOrderSubGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.OrderSub
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         o.PayStatus == E_OrderPayStatus.PaySuccess
                         &&
                         (rup.OrderSubId == null || o.Id.Contains(rup.OrderSubId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.SellChannelRefId, o.StoreName, o.PickupIsTrg, o.ReceiveModeName, o.ReceiveMode, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

            if (rup.OrderStatus != Entity.E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.OrderStatus);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
            }

            if (!string.IsNullOrEmpty(rup.ClientUserId))
            {
                query = query.Where(m => m.ClientUserId == rup.ClientUserId);
            }

            if (rup.IsHasEx)
            {
                query = query.Where(m => m.ExIsHappen == true && m.ExIsHandle == false);
            }

            if (rup.PickupTrgStatus == 1)
            {
                query = query.Where(m => m.PickupIsTrg == false);
            }
            else if (rup.PickupTrgStatus == 2)
            {
                query = query.Where(m => m.PickupIsTrg == true);
            }

            if (rup.ReceiveMode != E_ReceiveMode.Unknow)
            {
                query = query.Where(m => m.ReceiveMode == rup.ReceiveMode);
            }

            if (!string.IsNullOrEmpty(rup.SellChannelRefId))
            {
                query = query.Where(m => m.SellChannelRefId == rup.SellChannelRefId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                List<object> receiveDetails = new List<object>();
                List<object> pickupSkus = new List<object>();

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == item.Id).OrderByDescending(m => m.PickupStartTime).ToList();

                foreach (var orderSubChild in orderSubChilds)
                {
                    var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChild.Id).OrderByDescending(m => m.CreateTime).ToList();

                    List<object> pickupLogs = new List<object>();

                    foreach (var orderPickupLog in orderPickupLogs)
                    {
                        string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                        string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                        List<string> imgUrls = new List<string>();
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            imgUrls.Add(imgUrl);
                        }

                        if (!string.IsNullOrEmpty(imgUrl2))
                        {
                            imgUrls.Add(imgUrl2);
                        }

                        pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                    }

                    pickupSkus.Add(new
                    {
                        Id = orderSubChild.PrdProductSkuId,
                        MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl,
                        UniqueId = orderSubChild.Id,
                        ExPickupIsHandle = orderSubChild.ExPickupIsHandle,
                        Name = orderSubChild.PrdProductSkuName,
                        Quantity = orderSubChild.Quantity,
                        Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus),
                        PickupLogs = pickupLogs
                    });
                }

                receiveDetails.Add(new
                {
                    Name = item.ReceiveModeName,
                    Mode = item.ReceiveMode,
                    DetailType = 1,
                    DetailItems = pickupSkus
                });

                olist.Add(new
                {
                    Id = item.Id,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    SubmitTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = BizFactory.Order.GetStatus(item.Status),
                    SourceName = BizFactory.Order.GetSourceName(item.Source),
                    ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    ReceiveDetails = receiveDetails,
                    ReceiveModeName = item.ReceiveModeName,
                    TrgStatus = item.PickupIsTrg == false ? "未触发" : "已触发"
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetListByDelivery(string operater, string merchId, RupOrderSubGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.OrderSub
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         o.ReceiveMode == E_ReceiveMode.Delivery
                         &&
                         (rup.OrderId == null || o.Id.Contains(rup.OrderId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.StoreName, o.ReceiveModeName, o.ReceiveMode, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

            if (rup.OrderStauts != Entity.E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.OrderStauts);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
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
                List<object> receiveDetails = new List<object>();
                List<object> pickupSkus = new List<object>();

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == item.Id).OrderByDescending(m => m.PickupStartTime).ToList();

                foreach (var orderSubChild in orderSubChilds)
                {
                    var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChild.Id).OrderByDescending(m => m.CreateTime).ToList();

                    List<object> pickupLogs = new List<object>();

                    foreach (var orderPickupLog in orderPickupLogs)
                    {
                        string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                        string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                        List<string> imgUrls = new List<string>();
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            imgUrls.Add(imgUrl);
                        }

                        if (!string.IsNullOrEmpty(imgUrl2))
                        {
                            imgUrls.Add(imgUrl2);
                        }

                        pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                    }

                    pickupSkus.Add(new
                    {
                        Id = orderSubChild.PrdProductSkuId,
                        MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl,
                        UniqueId = orderSubChild.Id,
                        ExPickupIsHandle = orderSubChild.ExPickupIsHandle,
                        Name = orderSubChild.PrdProductSkuName,
                        Quantity = orderSubChild.Quantity,
                        Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus),
                        PickupLogs = pickupLogs
                    });
                }

                receiveDetails.Add(new
                {
                    Name = item.ReceiveModeName,
                    Mode = item.ReceiveMode,
                    DetailType = 1,
                    DetailItems = pickupSkus
                });

                olist.Add(new
                {
                    Id = item.Id,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    SubmitTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = BizFactory.Order.GetStatus(item.Status),
                    SourceName = BizFactory.Order.GetSourceName(item.Source),
                    ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    ReceiveDetails = receiveDetails,
                    ReceiveModeName = item.ReceiveModeName
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetListByStoreSelfTake(string operater, string merchId, RupOrderSubGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.OrderSub
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         o.ReceiveMode == E_ReceiveMode.StoreSelfTake
                         &&
                         (rup.OrderSubId == null || o.Id.Contains(rup.OrderSubId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.StoreName, o.ReceiveModeName, o.ReceiveMode, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

            if (rup.OrderStauts != Entity.E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.OrderStauts);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
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
                List<object> receiveDetails = new List<object>();
                List<object> pickupSkus = new List<object>();

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == item.Id).OrderByDescending(m => m.PickupStartTime).ToList();

                foreach (var orderSubChild in orderSubChilds)
                {
                    var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChild.Id).OrderByDescending(m => m.CreateTime).ToList();

                    List<object> pickupLogs = new List<object>();

                    foreach (var orderPickupLog in orderPickupLogs)
                    {
                        string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                        string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                        List<string> imgUrls = new List<string>();
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            imgUrls.Add(imgUrl);
                        }

                        if (!string.IsNullOrEmpty(imgUrl2))
                        {
                            imgUrls.Add(imgUrl2);
                        }

                        pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                    }

                    pickupSkus.Add(new
                    {
                        Id = orderSubChild.PrdProductSkuId,
                        MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl,
                        UniqueId = orderSubChild.Id,
                        ExPickupIsHandle = orderSubChild.ExPickupIsHandle,
                        Name = orderSubChild.PrdProductSkuName,
                        Quantity = orderSubChild.Quantity,
                        Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus),
                        PickupLogs = pickupLogs
                    });
                }

                receiveDetails.Add(new
                {
                    Name = item.ReceiveModeName,
                    Mode = item.ReceiveMode,
                    DetailType = 1,
                    DetailItems = pickupSkus
                });

                olist.Add(new
                {
                    Id = item.Id,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    SubmitTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = BizFactory.Order.GetStatus(item.Status),
                    SourceName = BizFactory.Order.GetSourceName(item.Source),
                    ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    ReceiveDetails = receiveDetails,
                    ReceiveModeName = item.ReceiveModeName
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetListByMachineSelfTake(string operater, string merchId, RupOrderSubGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.OrderSub
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         o.ReceiveMode == E_ReceiveMode.MachineSelfTake
                         &&
                         o.PayStatus == E_OrderPayStatus.PaySuccess
                         &&
                         (rup.OrderId == null || o.Id.Contains(rup.OrderId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.StoreName, o.PickupIsTrg, o.ReceiveModeName, o.ReceiveMode, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

            if (rup.OrderStauts != Entity.E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.OrderStauts);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
            }

            if (!string.IsNullOrEmpty(rup.ClientUserId))
            {
                query = query.Where(m => m.ClientUserId == rup.ClientUserId);
            }

            if (rup.IsHasEx)
            {
                query = query.Where(m => m.ExIsHappen == true && m.ExIsHandle == false);
            }

            if (rup.PickupTrgStatus == 1)
            {
                query = query.Where(m => m.PickupIsTrg == false);
            }
            else if (rup.PickupTrgStatus == 2)
            {
                query = query.Where(m => m.PickupIsTrg == true);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                List<object> receiveDetails = new List<object>();
                List<object> pickupSkus = new List<object>();

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == item.Id).OrderByDescending(m => m.PickupStartTime).ToList();

                foreach (var orderSubChild in orderSubChilds)
                {
                    var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChild.Id).OrderByDescending(m => m.CreateTime).ToList();

                    List<object> pickupLogs = new List<object>();

                    foreach (var orderPickupLog in orderPickupLogs)
                    {
                        string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                        string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                        List<string> imgUrls = new List<string>();
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            imgUrls.Add(imgUrl);
                        }

                        if (!string.IsNullOrEmpty(imgUrl2))
                        {
                            imgUrls.Add(imgUrl2);
                        }

                        pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                    }

                    pickupSkus.Add(new
                    {
                        Id = orderSubChild.PrdProductSkuId,
                        MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl,
                        UniqueId = orderSubChild.Id,
                        ExPickupIsHandle = orderSubChild.ExPickupIsHandle,
                        Name = orderSubChild.PrdProductSkuName,
                        Quantity = orderSubChild.Quantity,
                        Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus),
                        PickupLogs = pickupLogs
                    });
                }

                receiveDetails.Add(new
                {
                    Name = item.ReceiveModeName,
                    Mode = item.ReceiveMode,
                    DetailType = 1,
                    DetailItems = pickupSkus
                });

                olist.Add(new
                {
                    Id = item.Id,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    SubmitTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = BizFactory.Order.GetStatus(item.Status),
                    SourceName = BizFactory.Order.GetSourceName(item.Source),
                    ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    ReceiveDetails = receiveDetails,
                    ReceiveModeName = item.ReceiveModeName,
                    PickupIsTrg = item.PickupIsTrg == false ? "未触发" : "已触发"
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetDetailsByMachineSelfTake(string operater, string merchId, string orderSubId)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderDetailsByMachineSelfTake();

            var orderSub = CurrentDb.OrderSub.Where(m => m.MerchId == merchId && m.Id == orderSubId).FirstOrDefault();
            if (orderSub == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            ret.Id = orderSub.Id;
            ret.ClientUserName = orderSub.ClientUserName;
            ret.ClientUserId = orderSub.ClientUserId;
            ret.StoreName = orderSub.StoreName;
            ret.SubmitTime = orderSub.SubmittedTime.ToUnifiedFormatDateTime();
            ret.ChargeAmount = orderSub.ChargeAmount.ToF2Price();
            ret.DiscountAmount = orderSub.DiscountAmount.ToF2Price();
            ret.OriginalAmount = orderSub.OriginalAmount.ToF2Price();
            ret.Quantity = orderSub.Quantity;
            ret.CreateTime = orderSub.CreateTime.ToUnifiedFormatDateTime();
            ret.Status = BizFactory.Order.GetStatus(orderSub.Status);
            ret.SourceName = BizFactory.Order.GetSourceName(orderSub.Source);
            ret.CanHandleEx = BizFactory.Order.GetCanHandleEx(orderSub.ExIsHappen, orderSub.ExIsHandle);
            ret.ExHandleRemark = orderSub.ExHandleRemark;
            ret.ExIsHappen = orderSub.ExIsHappen;

            var receiveMode = new RetOrderDetailsByMachineSelfTake.ReceiveMode();
            receiveMode.Mode = orderSub.ReceiveMode;
            receiveMode.Name = orderSub.ReceiveModeName;
            receiveMode.Type = 1;

            var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == orderSub.Id).OrderByDescending(m => m.PickupStartTime).ToList();
            var pickupSkus = new List<RetOrderDetailsByMachineSelfTake.PickupSku>();
            foreach (var orderSubChild in orderSubChilds)
            {
                var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChild.Id).OrderByDescending(m => m.CreateTime).ToList();

                List<RetOrderDetailsByMachineSelfTake.PickupLog> pickupLogs = new List<RetOrderDetailsByMachineSelfTake.PickupLog>();

                foreach (var orderPickupLog in orderPickupLogs)
                {
                    string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                    string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                    List<string> imgUrls = new List<string>();
                    if (!string.IsNullOrEmpty(imgUrl))
                    {
                        imgUrls.Add(imgUrl);
                    }
                    if (!string.IsNullOrEmpty(imgUrl2))
                    {
                        imgUrls.Add(imgUrl2);
                    }
                    pickupLogs.Add(new RetOrderDetailsByMachineSelfTake.PickupLog { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                }

                receiveMode.Items.Add(new RetOrderDetailsByMachineSelfTake.PickupSku
                {
                    ExPickupIsHandle = orderSubChild.ExPickupIsHandle,
                    UniqueId = orderSubChild.Id,
                    MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl,
                    Name = orderSubChild.PrdProductSkuName,
                    Quantity = orderSubChild.Quantity,
                    Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus),
                    PickupLogs = pickupLogs,
                    SignStatus = 0
                });
            }

            ret.ReceiveModes.Add(receiveMode);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;

        }

        public CustomJsonResult HandleExOrderByMachineSelfTake(string operater, string merchId, RopOrderSubHandleExOrderByMachineSelfTake rop)
        {
            var bizRop = new BLL.Biz.RopOrderHandleExOrderByMachineSelfTake();
            bizRop.IsRunning = rop.IsRunning;
            bizRop.Remark = rop.Remark;
            bizRop.Items.Add(new ExItem { Id = rop.Id, Uniques = rop.Uniques });
            var result = BizFactory.Order.HandleExOrderByMachineSelfTake(operater, bizRop);
            return result;
        }
    }
}
