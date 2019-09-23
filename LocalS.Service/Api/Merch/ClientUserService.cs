using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class ClientUserService : BaseDbContext
    {
        public CustomJsonResult GetList(string operater, string merchId, RupClientGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SysClientUser
                         where (rup.NickName == null || u.UserName.Contains(rup.NickName)) &&
                         (rup.PhoneNumber == null || u.PhoneNumber.Contains(rup.PhoneNumber)) &&
                         u.IsDelete == false &&
                         u.MerchId == merchId
                         select new { u.Id, u.UserName, u.NickName, u.Avatar, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete, u.IsDisable });


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
                    UserName = item.UserName,
                    FullName = item.FullName,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Avatar = item.Avatar,
                    NickName = item.NickName,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult InitDetails(string operater, string merchId, string clientUserId)
        {
            var result = new CustomJsonResult();

            var ret = new RetClientUserInitManageBaseInfo();

            //var store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

            //ret.Id = store.Id;
            //ret.Name = store.Name;
            //ret.Address = store.Address;
            //ret.BriefDes = store.BriefDes;
            //ret.DispalyImgUrls = store.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
            //ret.IsOpen = store.IsOpen;


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult InitDetailsBaseInfo(string operater, string merchId, string clientUserId)
        {
            var ret = new RetClientUserInitManageProduct();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitDetailsOrders(string operater, string merchId, string clientUserId)
        {
            var ret = new RetClientUserInitDetailsOrders();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult DetailsOrdersGetOrderList(string operater, string merchId, RupClientDetailsOrdersGetOrderList rup)
        {
            var result = new CustomJsonResult();
            return MerchServiceFactory.Order.GetList(operater, merchId, new RupOrderGetList { ClientUserId = rup.ClientUserId, OrderSn = rup.OrderSn });
        }
    }
}
