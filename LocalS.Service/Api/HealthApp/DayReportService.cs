using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class DayReportService : BaseService
    {
        public CustomJsonResult GetDetails(string operater, string rptId)
        {
            var result = new CustomJsonResult();

            var d_Rpt = CurrentDb.SenvivHealthDayReport.Where(m => m.Id == rptId).FirstOrDefault();
            if (d_Rpt == null)
                new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "报告找不到");

            var d_SvUser = CurrentDb.SenvivUser.Where(m => m.Id == d_Rpt.SvUserId).FirstOrDefault();

            var togetherDays = (int)(d_Rpt.HealthDate - d_SvUser.CreateTime).TotalDays + 1;


            var pregnancy = new { birthLastDays = 0, gesWeek = 0, gesDay = 0 };

            if (d_SvUser.CareMode == Entity.E_SenvivUserCareMode.Pregnancy)
            {
                var d_Women = CurrentDb.SenvivUserWomen.Where(m => m.SvUserId == d_SvUser.Id).FirstOrDefault();
                if (d_Women != null)
                {
                    var week = Lumos.CommonUtil.GetPregnancyWeeks(d_Women.PregnancyTime, DateTime.Now);
                    var birthLastDays = Convert.ToInt32((d_Women.DeliveryTime - DateTime.Now).TotalDays + 1);
                    pregnancy = new { birthLastDays = birthLastDays, gesWeek = week.Week, gesDay = week.Day };
                }
            }

            var userInfo = new
            {
                SignName = d_SvUser.FullName,
                Avatar = d_SvUser.Avatar,
                TogetherDays = togetherDays,
                CareMode = d_SvUser.CareMode,
                Pregnancy = pregnancy
            };



            #region  gzTags
            var gzTags = new List<object>();

            if (d_SvUser.Sex == "2")
            {
                if (d_Rpt.ZsYp > 0)
                {
                    if (d_SvUser.CareMode == Entity.E_SenvivUserCareMode.Pregnancy)
                    {
                        gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_Rpt.MylGrfx)));
                    }
                    else
                    {
                        gzTags.Add(SvUtil.GetZsYq(decimal.Floor(d_Rpt.ZsYq)));
                    }

                    gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_Rpt.MylMylzs)));
                    gzTags.Add(SvUtil.GetQxxlJlqx(d_Rpt.QxxlJlqx));
                    gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_Rpt.QxxlKynl)));
                    gzTags.Add(SvUtil.GetZsYp(decimal.Floor(d_Rpt.ZsYp)));
                    gzTags.Add(SvUtil.GetZsSr(decimal.Floor(d_Rpt.ZsSr)));
                    gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_Rpt.QxxlQxyj)));
                }
                else
                {
                    gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_Rpt.MylMylzs)));
                    gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_Rpt.MylGrfx)));
                    gzTags.Add(SvUtil.GetMbGxygk(decimal.Floor(d_Rpt.MbGxygk)));
                    gzTags.Add(SvUtil.GetMbGxbgk(decimal.Floor(d_Rpt.MbGxbgk)));
                    gzTags.Add(SvUtil.GetMbTlbgk(decimal.Floor(d_Rpt.MbTlbgk)));
                    gzTags.Add(SvUtil.GetQxxlJlqx(d_Rpt.QxxlJlqx));
                    gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_Rpt.QxxlKynl)));
                    gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_Rpt.QxxlQxyj)));
                }
            }
            else
            {
                gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_Rpt.MylMylzs)));
                gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_Rpt.MylGrfx)));
                gzTags.Add(SvUtil.GetMbGxygk(decimal.Floor(d_Rpt.MbGxygk)));
                gzTags.Add(SvUtil.GetMbGxbgk(decimal.Floor(d_Rpt.MbGxbgk)));
                gzTags.Add(SvUtil.GetMbTlbgk(decimal.Floor(d_Rpt.MbTlbgk)));
                gzTags.Add(SvUtil.GetQxxlJlqx(d_Rpt.QxxlJlqx));
                gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_Rpt.QxxlKynl)));
                gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_Rpt.QxxlQxyj)));
            }
            #endregion

            #region smTags
            var smTags = new List<object>();

            var arr_SmTags = d_Rpt.SmTags.ToJsonObject<string[]>();
            if (arr_SmTags != null && arr_SmTags.Length > 0)
            {
                var d_TagExplains = CurrentDb.SenvivHealthTagExplain.Where(m => arr_SmTags.Contains(m.TagName)).Take(4).ToList();

                foreach (var d_TagExplain in d_TagExplains)
                {
                    smTags.Add(new
                    {
                        Id = d_TagExplain.TagId,
                        Name = d_TagExplain.TagName,
                        ProExplain = d_TagExplain.ProExplain,
                        TcmExplain = d_TagExplain.TcmExplain,
                        Suggest = d_TagExplain.Suggest
                    });
                }
            }
            #endregion

            #region smDvs
            var smDvs = new List<object>();
            smDvs.Add(SvUtil.GetSmSmsc(d_Rpt.SmSmsc, "2"));
            smDvs.Add(SvUtil.GetSmRsxs(d_Rpt.SmRsxs, "2"));
            smDvs.Add(SvUtil.GetSmSdsmsc(d_Rpt.SmSdsmsc, "2"));
            smDvs.Add(SvUtil.GetHxZtahizs(d_Rpt.HxZtahizs));
            smDvs.Add(SvUtil.GetXlDcjzxl(d_Rpt.XlDcjzxl));
            smDvs.Add(SvUtil.GetHrvXzznl(d_Rpt.HrvXzznl));
            #endregion

            #region smScoreByLast


            List<object> smScoreByLast = new List<object>();
            var d_DayReportSmScores = (from u in CurrentDb.SenvivHealthDayReport
                                       where u.SvUserId == d_Rpt.SvUserId && u.IsValid == true
                                       && u.HealthDate <= d_Rpt.HealthDate
                                       select new { u.CreateTime, u.HealthDate, u.SmScore }).OrderByDescending(m => m.HealthDate).Take(7).ToList();

            d_DayReportSmScores.Reverse();

            foreach (var d_hDayReportSmScore in d_DayReportSmScores)
            {
                smScoreByLast.Add(new { xData = d_hDayReportSmScore.HealthDate.ToString("MM-dd"), yData = d_hDayReportSmScore.SmScore });
            }

            #endregion

            var ret = new
            {
                rd = new
                {
                    HealthDate = d_Rpt.HealthDate.ToUnifiedFormatDate(),
                    HealthScore = SvUtil.GetHealthScore(d_Rpt.HealthScore),
                    HealthScoreTip = "您今天的健康值超过88%的人",
                    SmScore = SvUtil.GetSmScore(d_Rpt.SmScore),
                    SmScoreTip = "您的睡眠值已经打败77%的人",
                    GzTags = gzTags,//关注标签
                    SmTags = smTags,//睡眠标签
                    SmDvs = smDvs,//睡觉检测项
                    RptSuggest = d_Rpt.RptSuggest,
                    SmScoreByLast = smScoreByLast,
                    HxZtahizs = SvUtil.GetHxZtahizs(d_Rpt.HxZtahizs),
                    HrvXzznl = SvUtil.GetHrvXzznl(d_Rpt.HrvXzznl),
                    SmSmxl = SvUtil.GetSmSmxl(d_Rpt.SmSmxl),
                    SmSmlxx = SvUtil.GetSmSmlxx(d_Rpt.SmSmlxx),
                    SmSdsmbl = SvUtil.GetSmSdsmbl(d_Rpt.SmSdsmbl),
                    XlDcjzxl = SvUtil.GetXlCqjzxl(d_Rpt.XlDcjzxl),
                    XlCqjzxl = SvUtil.GetXlCqjzxl(d_Rpt.XlCqjzxl),
                    HxDcjzhx = SvUtil.GetHxDcjzhx(d_Rpt.HxDcjzhx),
                    SmSmsc = SvUtil.GetSmSmsc(d_Rpt.SmSmsc, "2"),
                    SmZcsc = SvUtil.GetSmSmsc(d_Rpt.SmZcsc, "2"),
                    SmZcsjfw = SvUtil.GetSmZcsjfw(d_Rpt.SmScsj, d_Rpt.SmLcsj),
                },
                userInfo = userInfo
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult UpdateVisitCount(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var rpt = CurrentDb.SenvivHealthDayReport.Where(m => m.Id == rptId).FirstOrDefault();
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
