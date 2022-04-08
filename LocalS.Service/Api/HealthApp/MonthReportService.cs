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

            var d_StageRpt = (from u in CurrentDb.SvHealthStageReport

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
                           tt.ReportCount,
                           u.SmTags,
                           u.HealthScore,
                           u.HealthDate,
                           u.MylGrfx,
                           u.MylMylzs,
                           u.MbGxbgk,
                           u.MbGxygk,
                           u.MbTnbgk,
                           u.QxxlJlqx,
                           u.QxxlKynl,
                           u.QxxlQxyj,
                           u.JbfxXljsl,
                           u.JbfxXlscfx,
                           u.HrvXzznl,
                           u.HrvXzznljzz,
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



            var sum_HealthScores = CurrentDb.SvHealthStageReport.Where(m => m.HealthDate == d_StageRpt.HealthDate).Select(m => m.HealthScore).ToList();
            int scoreRatio = 80;
            if (sum_HealthScores.Count > 0)
            {
                int a = sum_HealthScores.Where(m => m < d_StageRpt.HealthScore).Count();
                int b = sum_HealthScores.Count();
                double r = Math.Round((Convert.ToDouble(a) / Convert.ToDouble(b)), 2) * 100;
                scoreRatio = Convert.ToInt32(r);
            }

            var d_SmTags = CurrentDb.SvHealthStageReportTag.Where(m => m.ReportId == d_StageRpt.Id).OrderByDescending(m => m.TagCount).Take(4).ToList();

            List<object> smTags = new List<object>();

            foreach (var d_SmTag in d_SmTags)
            {
                smTags.Add(new { Id = d_SmTag.Id, Name = d_SmTag.TagName, Count = d_SmTag.TagCount });
            }

            var ret = new
            {
                Id = d_StageRpt.Id,
                UserInfo = new
                {
                    SignName = SvUtil.GetSignName("", d_StageRpt.FullName),
                    Avatar = d_StageRpt.Avatar,
                    Sex = SvUtil.GetSexName(d_StageRpt.Sex),
                    Age = SvUtil.GetAge(d_StageRpt.Birthday)
                },
                ReportData = new
                {
                    scoreRatio,
                    d_StageRpt.HealthScore,
                    d_StageRpt.HealthDate,
                    SmTags = smTags,
                    MylGrfx = SvUtil.GetMylGrfx(d_StageRpt.MylGrfx),
                    MylMylzs = SvUtil.GetMylzs(d_StageRpt.MylMylzs),
                    MbGxbgk = SvUtil.GetMbGxbgk(d_StageRpt.MbGxbgk),
                    MbGxygk = SvUtil.GetMbGxygk(d_StageRpt.MbGxygk),
                    MbTnbgk = SvUtil.GetMbTnbgk(d_StageRpt.MbTnbgk),
                    d_StageRpt.QxxlJlqx,
                    QxxlKynl = SvUtil.GetQxxlKynl(d_StageRpt.QxxlKynl),
                    d_StageRpt.QxxlQxyj,
                    JbfxXlscfx = SvUtil.GetJbfxXlscfx(d_StageRpt.JbfxXlscfx),
                    JbfxXljsl = SvUtil.GetJbfxXljsl(d_StageRpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvUtil.GetHrvXzznl(d_StageRpt.HrvXzznl,d_StageRpt.HrvXzznljzz, d_StageRpt.ReportCount),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvUtil.GetHrvJgsjzlzs(d_StageRpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvUtil.GetHrvMzsjzlzs(d_StageRpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvUtil.GetHrvZzsjzlzs(d_StageRpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvUtil.GetHrvHermzs(d_StageRpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvUtil.GetHrvTwjxgsszh(d_StageRpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvUtil.GetXlDcjzxl(d_StageRpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvUtil.GetXlCqjzxl(d_StageRpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvUtil.GetXlDcpjxl(d_StageRpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvUtil.GetHxDcjzhx(d_StageRpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvUtil.GetHxCqjzhx(d_StageRpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvUtil.GetHxDcpjhx(d_StageRpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvUtil.GetHxZtcs(d_StageRpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvUtil.GetHxZtahizs(d_StageRpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvUtil.GetSmSmsc(d_StageRpt.SmSmsc, "1"),
                    //深度睡眠时长
                    SmSdsmsc = SvUtil.GetSmSdsmsc(d_StageRpt.SmSdsmsc,"1"),
                    //浅度睡眠时长
                    SmQdsmsc = SvUtil.GetSmQdsmsc(d_StageRpt.SmQdsmsc, "1"),
                    //REM睡眠时长
                    SmRemsmsc = SvUtil.GetSmRemsmsc(d_StageRpt.SmRemsmsc, "1"),
                    //睡眠周期=
                    SmSmzq = SvUtil.GetSmSmzq(d_StageRpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvUtil.GetSmTdcs(d_StageRpt.SmTdcs),
                    RptSummary = d_StageRpt.RptSummary,
                    RptSuggest = d_StageRpt.RptSuggest
                }
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetEnergy(string operater, string rptId)
        {
            var result = new CustomJsonResult();

            var d_StageRpt = (from u in CurrentDb.SvHealthStageReport

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
                           tt.ReportCount,
                           u.HealthScore,
                           u.HealthDate,
                           u.SmTags,
                           u.SmSmsc,
                           u.SmSdsmsc,
                           u.SmQdsmsc,
                           u.SmRemsmsc,
                           u.HrvXzznl,
                           u.HrvXzznljzz,
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


            var d_SmTags = CurrentDb.SvHealthStageReportTag.Where(m => m.ReportId == d_StageRpt.Id).OrderByDescending(m => m.TagCount).Take(4).ToList();

            List<object> smTags = new List<object>();

            foreach (var d_SmTag in d_SmTags)
            {
                smTags.Add(new { Id = d_SmTag.Id, Name = d_SmTag.TagName, Count = d_SmTag.TagCount });
            }

            var ret = new
            {
                d_StageRpt.HealthScore,
                d_StageRpt.HealthDate,
                SmTags = smTags,
                SmSmsc = SvUtil.GetSmSmsc(d_StageRpt.SmSmsc, "1"),
                SmSdsmsc = SvUtil.GetSmSdsmsc(d_StageRpt.SmSdsmsc, "1"),
                SmQdsmsc = SvUtil.GetSmQdsmsc(d_StageRpt.SmQdsmsc, "1"),
                SmRemsmsc = SvUtil.GetSmRemsmsc(d_StageRpt.SmRemsmsc, "1"),
                HrvXzznl = SvUtil.GetHrvXzznl(d_StageRpt.HrvXzznl,d_StageRpt.HrvXzznljzz, d_StageRpt.ReportCount),
                HxDcpjhx = SvUtil.GetHxDcpjhx(d_StageRpt.HxDcpjhx),
                XlDcpjxl = SvUtil.GetXlDcpjxl(d_StageRpt.XlDcpjxl),
                HxZtcs = SvUtil.GetHxZtcs(d_StageRpt.HxZtcs),
                SmTdcs = SvUtil.GetSmTdcs(d_StageRpt.SmTdcs),
                HxZtahizs = SvUtil.GetHxZtahizs(d_StageRpt.HxZtahizs),
                DatePt = d_StageRpt.DatePt.ToJsonObject<List<object>>(),
                HealthScorePt = d_StageRpt.HealthScorePt.ToJsonObject<List<object>>(),
                SmSmscPt = d_StageRpt.SmSmscPt.ToJsonObject<List<object>>(),
                JbfxXlscfxPt = d_StageRpt.JbfxXlscfxPt.ToJsonObject<List<object>>(),
                HrvXzznlPt = d_StageRpt.HrvXzznlPt.ToJsonObject<List<object>>(),
                HxZtcsPt = d_StageRpt.HxZtcsPt.ToJsonObject<List<object>>(),
                HxZtahizsPt = d_StageRpt.HxZtahizsPt.ToJsonObject<List<object>>(),
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetAdvise(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var d_StageRpt = (from u in CurrentDb.SvHealthStageReport
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
                SugByYy = d_StageRpt.SugByYy.NullToEmpty(),
                SugByYd = d_StageRpt.SugByYd.NullToEmpty(),
                SugBySm = d_StageRpt.SugBySm.NullToEmpty(),
                SugByQxyl = d_StageRpt.SugByQxyl.NullToEmpty(),
                d_StageRpt.IsSend
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

            var tagExplain = CurrentDb.SvHealthTagExplain.Where(m => m.TagId == rptTag.TagId).FirstOrDefault();

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

            var d_StageRpt = CurrentDb.SvHealthStageReport.Where(m => m.Id == rptId).FirstOrDefault();
            if (d_StageRpt != null)
            {
                d_StageRpt.VisitCount += 1;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

            return result;
        }
    }
}
