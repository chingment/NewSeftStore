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
        public StatusModel GetReleaseStatus(E_AdContentStatus status)
        {
            var statusModel = new StatusModel();

            switch (status)
            {
                case E_AdContentStatus.Normal:
                    statusModel.Value = 1;
                    statusModel.Text = "正常";
                    break;
                case E_AdContentStatus.Deleted:
                    statusModel.Value = 2;
                    statusModel.Text = "已删除";
                    break;
            }


            return statusModel;
        }


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

        public CustomJsonResult GetReleaseList(string operater, string merchId, RupAdSpaceGetReleaseList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.AdContent
                         where u.AdSpaceId == rup.AdSpaceId
                         select new { u.Id, u.Title, u.Url, u.Status, u.CreateTime });


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
                    Title = item.Title,
                    Url = item.Url,
                    Status = GetReleaseStatus(item.Status),
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

                var adSpaceContent = new AdContent();

                adSpaceContent.Id = GuidUtil.New();
                adSpaceContent.AdSpaceId = rop.AdSpaceId;
                adSpaceContent.MerchId = merchId;
                adSpaceContent.Priority = 0;
                adSpaceContent.Title = rop.Title;
                adSpaceContent.Url = rop.DisplayImgUrls[0].Url;
                adSpaceContent.Status = E_AdContentStatus.Normal;
                adSpaceContent.Creator = operater;
                adSpaceContent.CreateTime = DateTime.Now;
                CurrentDb.AdContent.Add(adSpaceContent);

                foreach (var belongId in rop.BelongIds)
                {
                    var adSpaceContentBelong = new AdContentBelong();
                    adSpaceContentBelong.Id = GuidUtil.New();
                    adSpaceContentBelong.MerchId = adSpaceContent.MerchId;
                    adSpaceContentBelong.AdSpaceId = adSpaceContent.AdSpaceId;
                    adSpaceContentBelong.AdContentId = adSpaceContent.Id;
                    adSpaceContentBelong.BelongType = adSpace.BelongType;
                    adSpaceContentBelong.BelongId = belongId;
                    adSpaceContentBelong.Creator = operater;
                    adSpaceContentBelong.CreateTime = DateTime.Now;

                    CurrentDb.AdContentBelong.Add(adSpaceContentBelong);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发布成功");
            }
            return result;
        }


        public CustomJsonResult DeleteAdContent(string operater, string merchId, string adContentId)
        {
            var result = new CustomJsonResult();

            var adContent = CurrentDb.AdContent.Where(m => m.Id == adContentId).FirstOrDefault();
            if (adContent != null)
            {
                adContent.Status = E_AdContentStatus.Deleted;
                adContent.Mender = operater;
                adContent.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");

            return result;
        }
    }
}
