using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class MonthReportService : BaseService
    {
        public CustomJsonResult GetMonitor(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SvHealthStageReport

                       join s in CurrentDb.SvUser on u.SvUserId equals s.Id into temp
                       from tt in temp.DefaultIfEmpty()
                       where u.Id == rptId
                       select new
                       {
                           u.Id,
                           tt.Sex,
                           tt.FullName,
                           tt.Birthday,
                           tt.Avatar,
                           u.SmTags,
                           u.HealthScore,
                           u.HealthDate,
                           u.MylGrfx,
                           u.MylMylzs,
                           u.MbGxbgk,
                           u.MbGxygk,
                           u.MbTlbgk,
                           u.QxxlJlqx,
                           u.QxxlKynl,
                           u.QxxlQxyj,
                           u.JbfxXljsl,
                           u.JbfxXlscfx,
                           u.HrvXzznl,
                           u.HrvJgsjzlzs,
                           u.HrvMzsjzlzs,
                           u.HrvZzsjzlzs,
                           u.HrvHermzs,
                           u.HrvTwjxgsszs,
                           u.XlDcjzxl,
                           u.XlCqjzxl,
                           u.XlDcpjxl,
                           u.HxDcjzhx,
                           u.HxCqjzhx,
                           u.HxDcpjhx,
                           u.HxZtcs,
                           u.HxZtahizs,
                           u.HxZtpjsc,
                           u.SmSmsc,
                           u.SmSdsmsc,
                           u.SmQdsmsc,
                           u.SmRemsmsc,
                           u.SmTdcs,
                           u.SmSmzq,
                           u.IsBuild,
                           u.IsSend,
                           u.VisitCount,
                           u.Status,
                           u.SvUserId,
                           u.RptSummary,
                           u.RptSuggest,
                           u.CreateTime
                       }).FirstOrDefault();



            var sum_HealthScores = CurrentDb.SvHealthStageReport.Where(m => m.HealthDate == rpt.HealthDate).Select(m => m.HealthScore).ToList();
            int scoreRatio = 80;
            if (sum_HealthScores.Count > 0)
            {
                int a = sum_HealthScores.Where(m => m < rpt.HealthScore).Count();
                int b = sum_HealthScores.Count();
                double r = Math.Round((Convert.ToDouble(a) / Convert.ToDouble(b)), 2) * 100;
                scoreRatio = Convert.ToInt32(r);
            }

            var d_SmTags = CurrentDb.SvHealthStageReportTag.Where(m => m.ReportId == rpt.Id).OrderByDescending(m => m.TagCount).Take(4).ToList();

            List<object> smTags = new List<object>();

            foreach (var d_SmTag in d_SmTags)
            {
                smTags.Add(new { Id = d_SmTag.Id, Name = d_SmTag.TagName, Count = d_SmTag.TagCount });
            }

            var ret = new
            {
                Id = rpt.Id,
                UserInfo = new
                {
                    SignName = SvUtil.GetSignName("", rpt.FullName),
                    Avatar = rpt.Avatar,
                    Sex = SvUtil.GetSexName(rpt.Sex),
                    Age = SvUtil.GetAge(rpt.Birthday)
                },
                ReportData = new
                {
                    scoreRatio,
                    rpt.HealthScore,
                    rpt.HealthDate,
                    SmTags = smTags,
                    MylGrfx = SvUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvUtil.GetSmSmsc(rpt.SmSmsc, "1"),
                    //深度睡眠时长
                    SmSdsmsc = SvUtil.GetSmSdsmsc(rpt.SmSdsmsc,"1"),
                    //浅度睡眠时长
                    SmQdsmsc = SvUtil.GetSmQdsmsc(rpt.SmQdsmsc, "1"),
                    //REM睡眠时长
                    SmRemsmsc = SvUtil.GetSmRemsmsc(rpt.SmRemsmsc, "1"),
                    //睡眠周期=
                    SmSmzq = SvUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvUtil.GetSmTdcs(rpt.SmTdcs),
                    RptSummary = rpt.RptSummary,
                    RptSuggest = rpt.RptSuggest
                }
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetEnergy(string operater, string rptId)
        {
            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SvHealthStageReport
                       where u.Id == rptId
                       select new
                       {
                           u.Id,
                           u.HealthScore,
                           u.HealthDate,
                           u.SmTags,
                           u.SmSmsc,
                           u.SmSdsmsc,
                           u.SmQdsmsc,
                           u.SmRemsmsc,
                           u.HrvXzznl,
                           u.XlDcpjxl,
                           u.HxDcpjhx,
                           u.HxZtcs,
                           u.HxZtahizs,
                           u.SmTdcs,
                           u.DatePt,
                           u.HealthScorePt,
                           u.SmSmscPt,
                           u.JbfxXlscfxPt,
                           u.HrvXzznlPt,
                           u.HxZtcsPt,
                           u.HxZtahizsPt,
                           u.CreateTime
                       }).FirstOrDefault();


            var d_SmTags = CurrentDb.SvHealthStageReportTag.Where(m => m.ReportId == rpt.Id).OrderByDescending(m => m.TagCount).Take(4).ToList();

            List<object> smTags = new List<object>();

            foreach (var d_SmTag in d_SmTags)
            {
                smTags.Add(new { Id = d_SmTag.Id, Name = d_SmTag.TagName, Count = d_SmTag.TagCount });
            }

            var ret = new
            {
                rpt.HealthScore,
                rpt.HealthDate,
                SmTags = smTags,
                SmSmsc = SvUtil.GetSmSmsc(rpt.SmSmsc, "1"),
                SmSdsmsc = SvUtil.GetSmSdsmsc(rpt.SmSdsmsc, "1"),
                SmQdsmsc = SvUtil.GetSmQdsmsc(rpt.SmQdsmsc, "1"),
                SmRemsmsc = SvUtil.GetSmRemsmsc(rpt.SmRemsmsc, "1"),
                HrvXzznl = SvUtil.GetHrvXzznl(rpt.HrvXzznl),
                HxDcpjhx = SvUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                XlDcpjxl = SvUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                HxZtcs = SvUtil.GetHxZtcs(rpt.HxZtcs),
                SmTdcs = SvUtil.GetSmTdcs(rpt.SmTdcs),
                HxZtahizs = SvUtil.GetHxZtahizs(rpt.HxZtahizs),
                DatePt = rpt.DatePt.ToJsonObject<List<object>>(),
                HealthScorePt = rpt.HealthScorePt.ToJsonObject<List<object>>(),
                SmSmscPt = rpt.SmSmscPt.ToJsonObject<List<object>>(),
                JbfxXlscfxPt = rpt.JbfxXlscfxPt.ToJsonObject<List<object>>(),
                HrvXzznlPt = rpt.HrvXzznlPt.ToJsonObject<List<object>>(),
                HxZtcsPt = rpt.HxZtcsPt.ToJsonObject<List<object>>(),
                HxZtahizsPt = rpt.HxZtahizsPt.ToJsonObject<List<object>>(),
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetAdvise(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SvHealthStageReport
                       where u.Id == rptId
                       select new
                       {
                           u.Id,
                           u.SugByYy,
                           u.SugByYd,
                           u.SugBySm,
                           u.SugByQxyl,
                           u.IsSend,
                           u.Status,
                           u.CreateTime
                       }).FirstOrDefault();

            var ret = new
            {
                SugByYy = rpt.SugByYy.NullToEmpty(),
                SugByYd = rpt.SugByYd.NullToEmpty(),
                SugBySm = rpt.SugBySm.NullToEmpty(),
                SugByQxyl = rpt.SugByQxyl.NullToEmpty(),
                rpt.IsSend
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetTagAdvise(string operater, string tagId)
        {

            var result = new CustomJsonResult();

            var rptTag = (from u in CurrentDb.SvHealthStageReportTag
                          where u.Id == tagId
                          select new
                          {
                              u.Id,
                              u.ReportId,
                              u.TagId,
                              u.TagName,
                              u.TagCount
                          }).FirstOrDefault();

            var tagExplain = CurrentDb.SenvivHealthTagExplain.Where(m => m.TagId == rptTag.TagId).FirstOrDefault();

            string proExplain = "";
            string tcmExplain = "";
            string suggest = "";

            if (tagExplain != null)
            {
                proExplain = tagExplain.ProExplain;
                tcmExplain = tagExplain.TcmExplain;
                suggest = tagExplain.Suggest;
            }

            var ret = new
            {
                TagName = rptTag.TagName,
                TagCount = rptTag.TagCount,
                ProExplain = proExplain,
                TcmExplain = tcmExplain,
                Suggest = suggest
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetSugProducts(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SvHealthStageReportSugSku
                         where u.ReportId == rptId
                         select new { u.Id, u.MerchId, u.SkuId, u.CreateTime });


            int total = query.Count();

            int pageIndex = 0;
            int pageSize = int.MaxValue;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(item.MerchId, item.SkuId);
                if (r_Sku != null)
                {
                    olist.Add(new
                    {
                        Id = r_Sku.Id,
                        SpuId = r_Sku.SpuId,
                        Name = r_Sku.Name,
                        BriefDes = r_Sku.BriefDes,
                        MainImgUrl = r_Sku.MainImgUrl
                    });
                }
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult UpdateVisitCount(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var rpt = CurrentDb.SvHealthStageReport.Where(m => m.Id == rptId).FirstOrDefault();
            if (rpt != null)
            {
                rpt.VisitCount += 1;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

            return result;
        }
    }
}
