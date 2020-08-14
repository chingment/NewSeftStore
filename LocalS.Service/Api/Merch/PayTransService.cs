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
    public class PayTransService : BaseDbContext
    {

        public CustomJsonResult GetList(string operater, string merchId, RupPayTransGetList rup)
        {
            var result = new CustomJsonResult();


            var query = (from o in CurrentDb.PayTrans
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         (rup.OrderId == null || o.OrderIds.Contains(rup.OrderId)) &&
                              (rup.PayTransId == null || o.Id.Contains(rup.PayTransId)) &&
                                          (rup.PayPartnerOrderId == null || o.PayPartnerOrderId.Contains(rup.PayPartnerOrderId)) &&
                                          o.PayStatus == E_PayStatus.PaySuccess&&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.StoreName, o.Description, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.Quantity, o.AppId, o.IsTestMode, o.ClientUserId, o.SubmittedTime, o.ClientUserName, o.Source, o.OrderIds, o.PayedTime, o.PayWay, o.PayCaller, o.PayPartner, o.CreateTime, o.PayStatus, o.PayPartnerOrderId });

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    AppId = item.AppId,
                    OrderIds = item.OrderIds,
                    IsTestMode = item.IsTestMode ? "是" : "否",
                    SubmittedTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    PayStatus = BizFactory.Order.GetPayStatus(item.PayStatus),
                    PayedTime = item.PayedTime,
                    PayWay = BizFactory.Order.GetPayWay(item.PayWay),
                    PayPartner = BizFactory.Order.GetPayPartner(item.PayPartner),
                    PayPartnerOrderId = item.PayPartnerOrderId,
                    Description = item.Description
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
    }
}
