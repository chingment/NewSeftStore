using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class StoreService : BaseDbContext
    {
        public string GetStatusText(bool isClose)
        {
            string text = "";
            if (isClose)
            {
                text = "关闭";
            }
            else
            {
                text = "正常";
            }

            return text;
        }

        public int GetStatusValue(bool isClose)
        {
            int text = 0;
            if (isClose)
            {
                text = 2;
            }
            else
            {
                text = 1;
            }

            return text;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupStoreGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Store
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, u.Name, u.MainImgUrl, u.IsClose, u.Description, u.Address, u.CreateTime });


            query = query.OrderByDescending(r => r.CreateTime);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    MainImgUrl = item.MainImgUrl,
                    Address = item.Address,
                    Status = new { text = GetStatusText(item.IsClose), value = GetStatusValue(item.IsClose) },
                    CreateTime = item.CreateTime,
                });
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;
        }

        public CustomJsonResult InitAdd(string operater, string merchId)
        {
            var result = new CustomJsonResult();
            var ret = new RetStoreInitAdd();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopStoreAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            return result;
        }

        public CustomJsonResult InitEdit(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitEdit();
        

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
        }

        public CustomJsonResult Edit(string operater, string merchId, RopStoreEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            return result;
        }
    }
}
