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
    public class AdSpaceService : BaseService
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

        public StatusModel GetBelongStatus(E_AdContentBelongStatus status)
        {
            var statusModel = new StatusModel();

            switch (status)
            {
                case E_AdContentBelongStatus.Normal:
                    statusModel.Value = 1;
                    statusModel.Text = "正常";
                    break;
                case E_AdContentBelongStatus.Invalid:
                    statusModel.Value = 2;
                    statusModel.Text = "失效";
                    break;
            }


            return statusModel;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupAdSpaceGetList rup)
        {
            var result = new CustomJsonResult();


            var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();

            var query = (from u in CurrentDb.AdSpace
                         select new { u.Id, u.Name, u.Description, u.CreateTime });



            if (!merch.MctMode.Contains("K"))
            {
                query = query.Where(m => m.Id != E_AdSpaceId.MachineHomeBanner);
            }

            if (!merch.MctMode.Contains("F"))
            {
                query = query.Where(m => m.Id != E_AdSpaceId.AppHomeTopBanner);
            }

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
                    var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.CurUseStoreId != null).OrderBy(m => m.CurUseStoreId).ToList();

                    foreach (var merchMachine in merchMachines)
                    {
                        string storeName = "未绑定店铺";
                        var store = BizFactory.Store.GetOne(merchMachine.CurUseStoreId);
                        if (store != null)
                        {
                            storeName = store.Name;
                        }
                        ret.Belongs.Add(new { Id = merchMachine.MachineId, Name = string.Format("[机器]{0}({1}))", merchMachine.MachineId, storeName) });
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

                LogUtil.Info("adSpace.Id)" + adSpace.Id);
                var adSpaceContent = new AdContent();

                adSpaceContent.Id = IdWorker.Build(IdType.NewGuid);
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
                    LogUtil.Info("belongId.Id)" + belongId);
                    var adSpaceContentBelong = new AdContentBelong();
                    adSpaceContentBelong.Id = IdWorker.Build(IdType.NewGuid);
                    adSpaceContentBelong.MerchId = adSpaceContent.MerchId;
                    adSpaceContentBelong.AdSpaceId = adSpaceContent.AdSpaceId;
                    adSpaceContentBelong.AdContentId = adSpaceContent.Id;
                    adSpaceContentBelong.BelongType = adSpace.BelongType;
                    adSpaceContentBelong.BelongId = belongId;
                    adSpaceContentBelong.Status = E_AdContentBelongStatus.Normal;
                    adSpaceContentBelong.Creator = operater;
                    adSpaceContentBelong.CreateTime = DateTime.Now;

                    CurrentDb.AdContentBelong.Add(adSpaceContentBelong);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.AdSpaceRelease, string.Format("发布广告（{0}）成功", rop.Title), rop);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发布成功");
            }

            if (result.Result == ResultType.Success)
            {
                if (rop.AdSpaceId == E_AdSpaceId.MachineHomeBanner)
                {
                    string[] machineIds = rop.BelongIds.ToArray();
                    BizFactory.Machine.SendHomeBanners(operater, AppId.MERCH, merchId, machineIds);
                }
            }

            return result;
        }

        public CustomJsonResult GetAdContents(string operater, string merchId, RupAdSpaceGetReleaseList rup)
        {
            var result = new CustomJsonResult();

            string d_AdSpace_Name = "";
            var d_AdSpace = CurrentDb.AdSpace.Where(m => m.Id == rup.AdSpaceId).FirstOrDefault();
            if (d_AdSpace != null)
            {
                d_AdSpace_Name = d_AdSpace.Name;
            }

            var query = (from u in CurrentDb.AdContent
                         where u.AdSpaceId == rup.AdSpaceId && u.MerchId == merchId
                         select new { u.Id, u.Title, u.Url, u.Status, u.CreateTime });


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
                    Title = item.Title,
                    AdSpaceName = d_AdSpace_Name,
                    Url = item.Url,
                    Status = GetReleaseStatus(item.Status),
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult DeleteAdContent(string operater, string merchId, string adContentId)
        {
            var result = new CustomJsonResult();

            List<string> machineIds = new List<string>();

            var d_AdContent = CurrentDb.AdContent.Where(m => m.Id == adContentId && m.MerchId == merchId).FirstOrDefault();
            if (d_AdContent != null)
            {
                d_AdContent.Status = E_AdContentStatus.Deleted;
                d_AdContent.Mender = operater;
                d_AdContent.MendTime = DateTime.Now;

                if (d_AdContent.AdSpaceId == E_AdSpaceId.MachineHomeBanner)
                {
                    machineIds = CurrentDb.AdContentBelong.Where(m => m.AdSpaceId == E_AdSpaceId.MachineHomeBanner && m.AdContentId == d_AdContent.Id && m.MerchId == merchId && m.BelongType == E_AdSpaceBelongType.Machine).Select(m => m.BelongId).Distinct().ToList();
                }

                CurrentDb.SaveChanges();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.AdSpaceDeleteAdContent, string.Format("删除广告（{0}）成功", d_AdContent.Title), new { adContentId = adContentId });

            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");


            if (result.Result == ResultType.Success)
            {
                BizFactory.Machine.SendHomeBanners(operater, AppId.MERCH, merchId, machineIds.ToArray());
            }

            return result;
        }

        public CustomJsonResult GetAdContentBelongs(string operater, string merchId, RupAdSpaceGetAdContentBelongs rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.AdContentBelong
                         where u.AdContentId == rup.AdContentId && u.MerchId == merchId
                         select new { u.Id, u.BelongType, u.AdSpaceId, u.Status, u.BelongId, u.CreateTime });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string belongName = "";
                string adSpaceName = "";
                if (item.BelongType == E_AdSpaceBelongType.Machine)
                {
                    belongName = string.Format("终端: {0}", item.BelongId);
                }
                else if (item.BelongType == E_AdSpaceBelongType.App)
                {
                    var d_Store = CurrentDb.Store.Where(m => m.Id == item.BelongId).FirstOrDefault();
                    if (d_Store != null)
                    {
                        belongName = string.Format("店铺: {0}", d_Store.Name);
                    }
                }

                var adSpace = CurrentDb.AdSpace.Where(m => m.Id == item.AdSpaceId).FirstOrDefault();
                if (adSpace != null)
                {
                    adSpaceName = adSpace.Name;
                }

                olist.Add(new
                {
                    Id = item.Id,
                    BelongId = item.BelongId,
                    AdSpaceName = adSpaceName,
                    AdSpaceId = item.AdSpaceId,
                    BelongType = item.BelongType,
                    BelongName = belongName,
                    Status = GetBelongStatus(item.Status),
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;

        }

        public CustomJsonResult SetAdContentBelongStatus(string operater, string merchId, RopAdSpaceSetAdContentBelongStatus rop)
        {
            var result = new CustomJsonResult();

            List<string> machineIds = new List<string>();

            var d_AdContentBelong = CurrentDb.AdContentBelong.Where(m => m.Id == rop.Id).FirstOrDefault();

            if (d_AdContentBelong == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设置失败");
            }

            if (d_AdContentBelong.AdSpaceId == E_AdSpaceId.MachineHomeBanner)
            {
                machineIds.Add(d_AdContentBelong.BelongId);
            }

            d_AdContentBelong.Status = rop.Status;
            d_AdContentBelong.Mender = operater;
            d_AdContentBelong.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "设置成功");

            if (result.Result == ResultType.Success)
            {
                BizFactory.Machine.SendHomeBanners(operater, AppId.MERCH, merchId, machineIds.ToArray());
            }

            return result;
        }

        public CustomJsonResult CopyAdContent2Belongs(string operater, string merchId, RopAdContentCopy2Belongs rop)
        {
            var result = new CustomJsonResult();

            List<string> machineIds = new List<string>();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_AdContent = CurrentDb.AdContent.Where(m => m.Id == rop.AdContentId).FirstOrDefault();

                if (d_AdContent == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到信息");
                }

                var d_AdSpace = CurrentDb.AdSpace.Where(m => m.Id == d_AdContent.AdSpaceId).FirstOrDefault();

                if (d_AdSpace == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到信息");
                }

                foreach (var belongId in rop.BelongIds)
                {
                    var d_AdContentBelong = CurrentDb.AdContentBelong.Where(m => m.AdContentId == d_AdContent.Id && m.BelongId == belongId).FirstOrDefault();
                    if (d_AdContentBelong == null)
                    {
                        var adSpaceContentBelong = new AdContentBelong();
                        adSpaceContentBelong.Id = IdWorker.Build(IdType.NewGuid);
                        adSpaceContentBelong.MerchId = d_AdContent.MerchId;
                        adSpaceContentBelong.AdSpaceId = d_AdContent.AdSpaceId;
                        adSpaceContentBelong.AdContentId = d_AdContent.Id;
                        adSpaceContentBelong.BelongType = d_AdSpace.BelongType;
                        adSpaceContentBelong.BelongId = belongId;
                        adSpaceContentBelong.Status = E_AdContentBelongStatus.Normal;
                        adSpaceContentBelong.Creator = operater;
                        adSpaceContentBelong.CreateTime = DateTime.Now;
                        CurrentDb.AdContentBelong.Add(adSpaceContentBelong);

                        if (d_AdContentBelong.AdSpaceId == E_AdSpaceId.MachineHomeBanner)
                        {
                            machineIds.Add(belongId);
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "同步成功");

            }

            if (result.Result == ResultType.Success)
            {
                BizFactory.Machine.SendHomeBanners(operater, AppId.MERCH, merchId, machineIds.ToArray());
            }

            return result;
        }
    }
}
