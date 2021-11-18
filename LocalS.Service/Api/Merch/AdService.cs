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
    public class AdService : BaseService
    {
        public StatusModel GetContentStatus(E_AdContentStatus status)
        {
            var statusModel = new StatusModel();

            switch (status)
            {
                case E_AdContentStatus.Normal:
                    statusModel.Value = 1;
                    statusModel.Text = "正常";
                    break;
                case E_AdContentStatus.Invalid:
                    statusModel.Value = 2;
                    statusModel.Text = "失效";
                    break;
            }


            return statusModel;
        }

        public StatusModel GetBelongStatus(E_AdContentStatus content_status, E_AdContentBelongStatus belong_status)
        {
            var statusModel = new StatusModel();

            switch (content_status)
            {
                case E_AdContentStatus.Normal:

                    switch (belong_status)
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
                    break;
                case E_AdContentStatus.Invalid:
                    statusModel.Value = 3;
                    statusModel.Text = "停用";
                    break;
            }


            return statusModel;
        }

        public CustomJsonResult GetSpaces(string operater, string merchId, RupAdGetSpaces rup)
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

            var ret = new RetAdInitRelease();

            var d_AdSpace = CurrentDb.AdSpace.Where(m => m.Id == adSpaceId).FirstOrDefault();

            if (d_AdSpace != null)
            {
                ret.AdSpaceId = d_AdSpace.Id;
                ret.AdSpaceName = d_AdSpace.Name;
                ret.AdSpaceDescription = d_AdSpace.Description;
                ret.AdSpaceSupportFormat = d_AdSpace.SupportFormat;
                if (d_AdSpace.BelongType == E_AdSpaceBelongType.App)
                {
                    var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();

                    foreach (var d_Store in d_Stores)
                    {
                        ret.Belongs.Add(new { Id = d_Store.Id, Name = string.Format("[店铺]{0}", d_Store.Name) });
                    }
                }
                else if (d_AdSpace.BelongType == E_AdSpaceBelongType.Device)
                {
                    var merchDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.CurUseStoreId != null).OrderBy(m => m.CurUseStoreId).ToList();

                    foreach (var merchDevice in merchDevices)
                    {
                        string storeName = "未绑定店铺";
                        var m_Store = BizFactory.Store.GetOne(merchDevice.CurUseStoreId);
                        if (m_Store != null)
                        {
                            storeName = m_Store.Name;
                        }

                        string code = MerchServiceFactory.Device.GetCode(merchDevice.DeviceId, merchDevice.CumCode);

                        ret.Belongs.Add(new { Id = merchDevice.DeviceId, Name = string.Format("[设备]{0}({1}))", code, storeName) });
                    }
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult InitContents(string operater, string merchId, E_AdSpaceId adSpaceId)
        {
            var result = new CustomJsonResult();

            var adSpace = CurrentDb.AdSpace.Where(m => m.Id == adSpaceId).FirstOrDefault();

            if (adSpace != null)
            {
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { AdSpaceId = adSpace.Id, AdSpaceName = adSpace.Name, AdSpaceDescription = adSpace.Description });
            }

            return result;
        }

        public CustomJsonResult GetSelBelongs(string operater, string merchId, string adContentId)
        {
            var result = new CustomJsonResult();


            var d_AdContent = CurrentDb.AdContent.Where(m => m.Id == adContentId).FirstOrDefault();

            var d_AdContentBelongIds = CurrentDb.AdContentBelong.Where(m => m.AdContentId == adContentId).Select(m => m.BelongId).ToList();

            List<object> objs = new List<object>();

            if (d_AdContent.BelongType == E_AdSpaceBelongType.App)
            {
                var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();
                foreach (var d_Store in d_Stores)
                {
                    if (!d_AdContentBelongIds.Contains(d_Store.Id))
                    {
                        objs.Add(new { Id = d_Store.Id, Name = string.Format("[店铺]{0}", d_Store.Name) });
                    }
                }
            }
            else if (d_AdContent.BelongType == E_AdSpaceBelongType.Device)
            {
                var merchDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.CurUseStoreId != null).OrderBy(m => m.CurUseStoreId).ToList();

                foreach (var merchDevice in merchDevices)
                {
                    string storeName = "未绑定店铺";
                    var m_Store = BizFactory.Store.GetOne(merchDevice.CurUseStoreId);
                    if (m_Store != null)
                    {
                        storeName = m_Store.Name;
                    }

                    if (!d_AdContentBelongIds.Contains(merchDevice.DeviceId))
                    {
                        var code = MerchServiceFactory.Device.GetCode(merchDevice.DeviceId, merchDevice.CumCode);

                        objs.Add(new { Id = merchDevice.DeviceId, Name = string.Format("[设备]{0}({1}))", code, storeName) });

                    }
                }
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", objs);


            return result;
        }

        public CustomJsonResult InitBelongs(string operater, string merchId, string adContentId)
        {
            var result = new CustomJsonResult();

            var d_AdContent = CurrentDb.AdContent.Where(m => m.Id == adContentId).FirstOrDefault();
            var d_AdSpace = CurrentDb.AdSpace.Where(m => m.Id == d_AdContent.AdSpaceId).FirstOrDefault();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { Url = d_AdContent.Url, Title = d_AdContent.Title, AdSpaceId = d_AdSpace.Id, AdSpaceName = d_AdSpace.Name, Status = GetContentStatus(d_AdContent.Status), AdSpaceDescription = d_AdSpace.Description });


            return result;
        }

        public CustomJsonResult Release(string operater, string merchId, RopAdRelease rop)
        {
            var result = new CustomJsonResult();

            List<string> deviceIds = new List<string>();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_AdSpace = CurrentDb.AdSpace.Where(m => m.Id == rop.AdSpaceId).FirstOrDefault();

                if (d_AdSpace == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发布失败");
                }

                var d_AdContent = new AdContent();
                d_AdContent.Id = IdWorker.Build(IdType.NewGuid);
                d_AdContent.AdSpaceId = rop.AdSpaceId;
                d_AdContent.BelongType = d_AdSpace.BelongType;
                d_AdContent.MerchId = merchId;
                d_AdContent.Priority = 0;
                d_AdContent.Title = rop.Title;
                d_AdContent.Url = rop.FileUrls[0].Url;
                d_AdContent.Status = E_AdContentStatus.Normal;
                d_AdContent.Creator = operater;
                d_AdContent.CreateTime = DateTime.Now;
                CurrentDb.AdContent.Add(d_AdContent);


                foreach (var belongId in rop.BelongIds)
                {
                    var d_AdContentBelong = new AdContentBelong();
                    d_AdContentBelong.Id = IdWorker.Build(IdType.NewGuid);
                    d_AdContentBelong.MerchId = d_AdContent.MerchId;
                    d_AdContentBelong.AdSpaceId = d_AdContent.AdSpaceId;
                    d_AdContentBelong.AdContentId = d_AdContent.Id;
                    d_AdContentBelong.BelongType = d_AdContent.BelongType;
                    d_AdContentBelong.BelongId = belongId;
                    d_AdContentBelong.Status = E_AdContentBelongStatus.Normal;
                    d_AdContentBelong.ValidStartTime = DateTime.Parse(rop.ValidDate[0]);
                    d_AdContentBelong.ValidEndTime = DateTime.Parse(rop.ValidDate[1]);
                    d_AdContentBelong.Creator = operater;
                    d_AdContentBelong.CreateTime = DateTime.Now;
                    CurrentDb.AdContentBelong.Add(d_AdContentBelong);

                    if (d_AdSpace.BelongType == E_AdSpaceBelongType.Device)
                    {
                        deviceIds.Add(belongId);
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.ad_release, string.Format("发布广告（{0}）成功", rop.Title), rop);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发布成功");
            }

            if (result.Result == ResultType.Success)
            {
                Task.Factory.StartNew(() =>
                {
                    BizFactory.Device.SendAds(operater, AppId.MERCH, merchId, deviceIds.ToArray());

                });
            }

            return result;
        }

        public CustomJsonResult GetContents(string operater, string merchId, RupAdGetContents rup)
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
                    Status = GetContentStatus(item.Status),
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult SetContentStatus(string operater, string merchId, RopAdSetContentStatus rop)
        {
            var result = new CustomJsonResult();

            List<string> deviceIds = new List<string>();

            var d_AdContent = CurrentDb.AdContent.Where(m => m.Id == rop.Id && m.MerchId == merchId).FirstOrDefault();
            var d_AdSpace = CurrentDb.AdSpace.Where(m => m.Id == d_AdContent.AdSpaceId).FirstOrDefault();

            if (d_AdContent == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设置失败");
            }


            d_AdContent.Status = rop.Status;
            d_AdContent.Mender = operater;
            d_AdContent.MendTime = DateTime.Now;

            if (d_AdSpace.BelongType == E_AdSpaceBelongType.Device)
            {
                deviceIds = CurrentDb.AdContentBelong.Where(m => m.AdContentId == d_AdContent.Id && m.MerchId == merchId && m.BelongType == E_AdSpaceBelongType.Device).Select(m => m.BelongId).Distinct().ToList();
            }

            CurrentDb.SaveChanges();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "设置成功");


            if (result.Result == ResultType.Success)
            {
                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.ad_set_status, string.Format("设置广告状态（{0}）成功", d_AdContent.Title), rop);

                Task.Factory.StartNew(() =>
                {
                    BizFactory.Device.SendAds(operater, AppId.MERCH, merchId, deviceIds.ToArray());

                });
            }

            return result;
        }

        public CustomJsonResult GetContentBelongs(string operater, string merchId, RupAdGetContentBelongs rup)
        {
            var result = new CustomJsonResult();

            var d_AdContent = CurrentDb.AdContent.Where(m => m.Id == rup.AdContentId).FirstOrDefault();


            var query = (from u in CurrentDb.AdContentBelong
                         where u.AdContentId == rup.AdContentId && u.MerchId == merchId
                         select new { u.Id, u.BelongType, u.AdSpaceId, u.Status, u.BelongId, u.ValidStartTime, u.ValidEndTime, u.CreateTime });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string belongName = "";
                if (item.BelongType == E_AdSpaceBelongType.Device)
                {
                    var code = item.BelongId;

                    var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.DeviceId == item.BelongId).FirstOrDefault();
                    if (d_MerchDevice != null)
                    {
                        code = MerchServiceFactory.Device.GetCode(d_MerchDevice.DeviceId, d_MerchDevice.CumCode);
                    }

                    belongName = string.Format("终端: {0}", code);
                }
                else if (item.BelongType == E_AdSpaceBelongType.App)
                {
                    var d_Store = CurrentDb.Store.Where(m => m.Id == item.BelongId).FirstOrDefault();
                    if (d_Store != null)
                    {
                        belongName = string.Format("店铺: {0}", d_Store.Name);
                    }
                }

                olist.Add(new
                {
                    Id = item.Id,
                    BelongId = item.BelongId,
                    BelongType = item.BelongType,
                    BelongName = belongName,
                    ValidStartTime = item.ValidStartTime.ToUnifiedFormatDate(),
                    ValidEndTime = item.ValidEndTime.ToUnifiedFormatDate(),
                    Status = GetBelongStatus(d_AdContent.Status, item.Status),
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;

        }

        public CustomJsonResult SetContentBelongStatus(string operater, string merchId, RopAdSetAdContentBelongStatus rop)
        {
            var result = new CustomJsonResult();

            List<string> deviceIds = new List<string>();

            var d_AdContentBelong = CurrentDb.AdContentBelong.Where(m => m.Id == rop.Id).FirstOrDefault();
            var d_AdSpace = CurrentDb.AdSpace.Where(m => m.Id == d_AdContentBelong.AdSpaceId).FirstOrDefault();
            if (d_AdContentBelong == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设置失败");
            }

            if (d_AdSpace.BelongType == E_AdSpaceBelongType.Device)
            {
                deviceIds.Add(d_AdContentBelong.BelongId);
            }

            d_AdContentBelong.Status = rop.Status;
            d_AdContentBelong.Mender = operater;
            d_AdContentBelong.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "设置成功");

            if (result.Result == ResultType.Success)
            {
                Task.Factory.StartNew(() =>
                {
                    BizFactory.Device.SendAds(operater, AppId.MERCH, merchId, deviceIds.ToArray());
                });
            }

            return result;
        }

        public CustomJsonResult AddContentBelong(string operater, string merchId, RopAdAddContentBelong rop)
        {

            var result = new CustomJsonResult();

            List<string> deviceIds = new List<string>();

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

                if (rop.BelongIds.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "对象不能为空");
                }

                foreach (var belongId in rop.BelongIds)
                {
                    var d_AdContentBelong = CurrentDb.AdContentBelong.Where(m => m.AdContentId == d_AdContent.Id && m.BelongId == belongId).FirstOrDefault();
                    if (d_AdContentBelong == null)
                    {
                        d_AdContentBelong = new AdContentBelong();
                        d_AdContentBelong.Id = IdWorker.Build(IdType.NewGuid);
                        d_AdContentBelong.MerchId = d_AdContent.MerchId;
                        d_AdContentBelong.AdSpaceId = d_AdContent.AdSpaceId;
                        d_AdContentBelong.AdContentId = d_AdContent.Id;
                        d_AdContentBelong.BelongType = d_AdSpace.BelongType;
                        d_AdContentBelong.BelongId = belongId;
                        d_AdContentBelong.ValidStartTime = DateTime.Parse(rop.ValidDate[0]);
                        d_AdContentBelong.ValidEndTime = DateTime.Parse(rop.ValidDate[1]);
                        d_AdContentBelong.Status = E_AdContentBelongStatus.Normal;
                        d_AdContentBelong.Creator = operater;
                        d_AdContentBelong.CreateTime = DateTime.Now;
                        CurrentDb.AdContentBelong.Add(d_AdContentBelong);

                        if (d_AdSpace.BelongType == E_AdSpaceBelongType.Device)
                        {
                            deviceIds.Add(belongId);
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            if (result.Result == ResultType.Success)
            {
                Task.Factory.StartNew(() =>
                {
                    BizFactory.Device.SendAds(operater, AppId.MERCH, merchId, deviceIds.ToArray());
                });


            }

            return result;
        }

        public CustomJsonResult EditContentBelong(string operater, string merchId, RopAdEditContentBelong rop)
        {
            var result = new CustomJsonResult();

            List<string> deviceIds = new List<string>();

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
                    if (d_AdContentBelong != null)
                    {
                        d_AdContentBelong.ValidStartTime = DateTime.Parse(rop.ValidDate[0]);
                        d_AdContentBelong.ValidEndTime = DateTime.Parse(rop.ValidDate[1]);
                    }
                }


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            if (result.Result == ResultType.Success)
            {
                Task.Factory.StartNew(() =>
                {
                    BizFactory.Device.SendAds(operater, AppId.MERCH, merchId, deviceIds.ToArray());
                });


            }

            return result;
        }
    }
}
