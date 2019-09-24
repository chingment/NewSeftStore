using LocalS.BLL;
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
    public class AdSpaceService : BaseDbContext
    {
        public CustomJsonResult GetList(string operater, string merchId, RupAdSpaceGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.AdSpace
                         select new { u.Id, u.Name, u.Description, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult InitRelease(string operater, string merchId, E_AdSpaceId adSpaceId)
        {
            var result = new CustomJsonResult();

            var ret = new RetAdSpaceInitRelease();

            var adSpace = CurrentDb.AdSpace.Where(m => m.Id == adSpaceId).FirstOrDefault();

            if (adSpace != null)
            {
                ret.AdSpaceId = adSpace.Id;
                ret.AdSpaceName = adSpace.Name;
                ret.AdSpaceDescription = adSpace.Description;

                if (adSpace.BelongType == E_AdSpaceBelongType.App)
                {
                    var stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();

                    foreach (var store in stores)
                    {
                        ret.Belongs.Add(new { Id = store.Id, Name = string.Format("[店铺]{0}", store.Name) });
                    }
                }
                else if (adSpace.BelongType == E_AdSpaceBelongType.Machine)
                {
                    var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).ToList();

                    foreach (var merchMachine in merchMachines)
                    {
                        string storeName = "未绑定店铺";
                        var store = CurrentDb.Store.Where(m => m.Id == merchMachine.StoreId).FirstOrDefault();
                        if (store != null)
                        {
                            storeName = store.Name;
                        }
                        ret.Belongs.Add(new { Id = merchMachine.Id, Name = string.Format("[机器({0})]{1}", storeName, merchMachine.Name) });
                    }
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Release(string operater, string merchId, RopAdSpaceRelease rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var adSpace = CurrentDb.AdSpace.Where(m => m.Id == rop.AdSpaceId).FirstOrDefault();

                var adSpaceContent = new AdSpaceContent();

                adSpaceContent.Id = GuidUtil.New();
                adSpaceContent.AdSpaceId = rop.AdSpaceId;
                adSpaceContent.MerchId = merchId;
                adSpaceContent.Priority = 0;
                adSpaceContent.Title = rop.Title;
                adSpaceContent.Url = rop.DispalyImgUrls[0].Url;
                adSpaceContent.Status = E_AdSpaceContentStatus.Normal;
                adSpaceContent.Creator = operater;
                adSpaceContent.CreateTime = DateTime.Now;
                CurrentDb.AdSpaceContent.Add(adSpaceContent);

                foreach (var belongId in rop.BelongIds)
                {
                    var adSpaceContentBelong = new AdSpaceContentBelong();
                    adSpaceContentBelong.Id = GuidUtil.New();
                    adSpaceContentBelong.AdSpaceContentId = adSpaceContent.Id;
                    adSpaceContentBelong.AdSpaceId = rop.AdSpaceId;
                    adSpaceContentBelong.MerchId = merchId;
                    adSpaceContentBelong.Priority = 0;
                    adSpaceContentBelong.Title = rop.Title;
                    adSpaceContentBelong.Url = rop.DispalyImgUrls[0].Url;
                    adSpaceContentBelong.BelongType = adSpace.BelongType;
                    adSpaceContentBelong.BelongId = belongId;
                    adSpaceContentBelong.Status = E_AdSpaceContentBelongStatus.Normal;
                    adSpaceContentBelong.Creator = operater;
                    adSpaceContentBelong.CreateTime = DateTime.Now;

                    CurrentDb.AdSpaceContentBelong.Add(adSpaceContentBelong);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发布成功");
            }
            return result;
        }
    }
}
