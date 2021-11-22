using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.Service.UI;
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
    public class DevSenvivLiteService : DeviceService
    {
        public CustomJsonResult InitGetList(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { });
            return result;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupDeviceGetList rup)
        {
            var result = new CustomJsonResult();

            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var query = (from u in CurrentDb.MerchDevice
                         join m in CurrentDb.Device on u.DeviceId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where ((rup.Id == null || u.DeviceId.Contains(rup.Id)) || (rup.Id == null || u.CumCode.Contains(rup.Id)))
                         &&
                         merchIds.Contains(u.MerchId)
                         select new { u.Id, tt.Type, tt.Model, u.DeviceId, u.CumCode, tt.MainImgUrl, tt.CurUseStoreId, tt.CurUseShopId, tt.RunStatus, tt.LastRequestTime, tt.AppVersionCode, tt.CtrlSdkVersionCode, tt.ExIsHas, tt.Name, u.IsStopUse, u.CreateTime });

            if (!string.IsNullOrEmpty(rup.Type))
            {
                query = query.Where(m => m.Type == rup.Type);
            }


            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.CurUseStoreId == rup.StoreId);
            }

            if (!string.IsNullOrEmpty(rup.ShopId))
            {
                query = query.Where(m => m.CurUseShopId == rup.ShopId);
            }



            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.CurUseStoreId).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.OrderBy(m => m.IsStopUse).ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.DeviceId,
                    Name = item.Name,
                    Model = item.Model,
                    Code = GetCode(item.DeviceId, item.CumCode),
                    MainImgUrl = item.MainImgUrl,
                    LastRequestTime = item.LastRequestTime.ToUnifiedFormatDateTime()
                });

            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

    }
}
