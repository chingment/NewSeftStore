using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                case E_OrderStatus.Cancled:
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
                case E_OrderDetailsChildSonStatus.Payed:
                    status.Value = 3000;
                    status.Text = "已支付";
                    break;
                case E_OrderDetailsChildSonStatus.WaitPick:
                    status.Value = 3010;
                    status.Text = "待取货";
                    break;
                case E_OrderDetailsChildSonStatus.Picking:
                    status.Value = 3011;
                    status.Text = "取货中";
                    break;
                case E_OrderDetailsChildSonStatus.Completed:
                    status.Value = 4000;
                    status.Text = "已完成";
                    break;
                case E_OrderDetailsChildSonStatus.Cancled:
                    status.Value = 5000;
                    status.Text = "已取消";
                    break;
                case E_OrderDetailsChildSonStatus.Exception:
                    status.Value = 6000;
                    status.Text = "异常";
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

        public CustomJsonResult GetList(string operater, string merchId, RupOrderGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.Order
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         (rup.OrderSn == null || o.Sn.Contains(rup.OrderSn)) &&
                         o.MerchId == merchId
                         select new { o.Sn, o.Id, o.SellChannelRefIds, o.StoreId, o.ClientUserId, o.ClientUserName, o.StoreName, o.Source, o.SubmitTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

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

                List<object> olist_Details = new List<object>();
                foreach (var orderDetail in orderDetails)
                {
                    List<object> sub_Skus = new List<object>();
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
                                    string imgUrl = null;
                                    List<string> imgUrls = new List<string>();
                                    if (!string.IsNullOrEmpty(orderPickupLog.ImgUrlByCHK))
                                    {
                                        imgUrl = orderPickupLog.ImgUrlByCHK;
                                        imgUrls.Add(orderPickupLog.ImgUrlByCHK);
                                    }

                                    pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                                }

                                sub_Skus.Add(new
                                {
                                    PrdProductSkuId = orderDetailsChildSon.PrdProductSkuId,
                                    PrdProductSkuMainImgUrl = orderDetailsChildSon.PrdProductSkuMainImgUrl,
                                    PrdProductSkuName = orderDetailsChildSon.PrdProductSkuName,
                                    Quantity = orderDetailsChildSon.Quantity,
                                    Status = GetSonStatus(orderDetailsChildSon.Status),
                                    PickupLogs = pickupLogs
                                });
                            }

                            break;
                    }
                    olist_Details.Add(new
                    {
                        SellChannelRefName = orderDetail.SellChannelRefName,
                        SellChannelRefType = orderDetail.SellChannelRefType,
                        Detials = sub_Skus
                    });
                }

                olist.Add(new
                {
                    Id = item.Id,
                    Sn = item.Sn,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    SubmitTime = item.SubmitTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = GetStatus(item.Status),
                    SourceName = GetSourceName(item.Source),
                    Details = olist_Details
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
    }
}
