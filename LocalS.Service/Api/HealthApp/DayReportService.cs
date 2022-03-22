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

            var d_DayRpt = CurrentDb.SvHealthDayReport.Where(m => m.Id == rptId).FirstOrDefault();
            if (d_DayRpt == null)
                new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "报告找不到");

            var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == d_DayRpt.SvUserId).FirstOrDefault();

            var togetherDays = (int)(d_DayRpt.HealthDate - d_SvUser.CreateTime).TotalDays + 1;


            var pregnancy = new { birthLastDays = 0, gesWeek = 0, gesDay = 0 };

            if (d_SvUser.CareMode == Entity.E_SvUserCareMode.Pregnancy)
            {
                var d_Women = CurrentDb.SvUserWomen.Where(m => m.SvUserId == d_SvUser.Id).FirstOrDefault();
                if (d_Women != null)
                {
                    if (d_Women.PregnancyTime != null && d_Women.DeliveryTime.Value != null)
                    {
                        var week = Lumos.CommonUtil.GetDiffWeekDay(DateTime.Parse(d_Women.PregnancyTime.Value.ToString("yyyy-MM-dd")),DateTime.Parse(d_DayRpt.CreateTime.ToString("yyyy-MM-dd")));
                        var birthLastDays = Convert.ToInt32((DateTime.Parse(d_Women.DeliveryTime.Value.ToString("yyyy-MM-dd")) - DateTime.Parse(d_DayRpt.CreateTime.ToString("yyyy-MM-dd"))).TotalDays);
                        pregnancy = new { birthLastDays = birthLastDays, gesWeek = week.Week, gesDay = week.Day };
                    }
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


            #region last7DayData


            List<object> smScoreByLast = new List<object>();
            List<object> mylMylzsByLast = new List<object>();
            List<object> mylGrfxByLast = new List<object>();
            List<object> mbGxygkByLast = new List<object>();
            List<object> mbGxbgkByLast = new List<object>();
            List<object> mbTlbgkByLast = new List<object>();
            List<object> qxxlKynlByLast = new List<object>();
            List<object> qxxlQxyjByLast = new List<object>();
            List<ChatDataByStr> qxxlJlqxByLast = new List<ChatDataByStr>();
            List<object> zsGmYqByLast = new List<object>();
            List<object> zsGmYpByLast = new List<object>();
            List<object> zsGmSrByLast = new List<object>();


            var d_DayReportsByLast = (from u in CurrentDb.SvHealthDayReport
                                      where u.SvUserId == d_DayRpt.SvUserId && u.IsValid == true
                                      && u.HealthDate <= d_DayRpt.HealthDate
                                      select new { u.HealthDate, u.SmScore, u.MylMylzs, u.MylGrfx, u.MbGxygk, u.MbTlbgk, u.MbGxbgk, u.QxxlKynl, u.QxxlQxyj, u.QxxlJlqx, u.ZsGmSr, u.ZsGmYp, u.ZsGmYq, u.CreateTime }).OrderByDescending(m => m.HealthDate).Take(7).ToList();

            d_DayReportsByLast.Reverse();

            foreach (var d_DayReportByLast in d_DayReportsByLast)
            {
                smScoreByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.SmScore });
                mylMylzsByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.MylMylzs });
                mylGrfxByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.MylGrfx });
                mbGxygkByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.MbGxygk });
                mbGxbgkByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.MbGxbgk });
                mbTlbgkByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.MbTlbgk });
                qxxlKynlByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.QxxlKynl });
                qxxlQxyjByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.QxxlQxyj });
                qxxlJlqxByLast.Add(new ChatDataByStr { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.QxxlJlqx });
                zsGmYqByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.ZsGmYq });
                zsGmYpByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.ZsGmYp });
                zsGmSrByLast.Add(new { xData = d_DayReportByLast.HealthDate.ToString("MM-dd"), yData = d_DayReportByLast.ZsGmSr });
            }

            #endregion

            #region  gzTags
            var gzTags = new List<object>();


            if (d_DayRpt.ZsGmYp > 0 && d_SvUser.Sex == "2")
            {
                if (d_SvUser.CareMode == Entity.E_SvUserCareMode.Pregnancy)
                {
                    gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_DayRpt.MylGrfx), true, mylGrfxByLast));
                }
                else
                {
                    gzTags.Add(SvUtil.GetZsGmYq(decimal.Floor(d_DayRpt.ZsGmYq), true, zsGmYqByLast));
                }

                gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_DayRpt.MylMylzs), true, mylMylzsByLast));
                gzTags.Add(SvUtil.GetQxxlJlqx(d_DayRpt.QxxlJlqx, true, qxxlJlqxByLast));
                gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_DayRpt.QxxlKynl), true, qxxlKynlByLast));
                gzTags.Add(SvUtil.GetZsGmYp(decimal.Floor(d_DayRpt.ZsGmYp), true, zsGmYpByLast));
                gzTags.Add(SvUtil.GetZsGmSr(decimal.Floor(d_DayRpt.ZsGmSr), true, zsGmSrByLast));
                gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_DayRpt.QxxlQxyj), true, qxxlQxyjByLast));
            }
            else
            {
                gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_DayRpt.MylMylzs), true, mylMylzsByLast));
                gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_DayRpt.MylGrfx), true, mylGrfxByLast));
                gzTags.Add(SvUtil.GetMbGxygk(decimal.Floor(d_DayRpt.MbGxygk), true, mbGxygkByLast));
                gzTags.Add(SvUtil.GetMbGxbgk(decimal.Floor(d_DayRpt.MbGxbgk), true, mbGxbgkByLast));
                gzTags.Add(SvUtil.GetMbTlbgk(decimal.Floor(d_DayRpt.MbTlbgk), true, mbTlbgkByLast));
                gzTags.Add(SvUtil.GetQxxlJlqx(d_DayRpt.QxxlJlqx, true, qxxlJlqxByLast));
                gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_DayRpt.QxxlKynl), true, qxxlKynlByLast));
                gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_DayRpt.QxxlQxyj), true, qxxlQxyjByLast));
            }
            #endregion

            #region smTags
            var smTags = new List<object>();

            var arr_SmTags = d_DayRpt.SmTags.ToJsonObject<string[]>();
            if (arr_SmTags != null && arr_SmTags.Length > 0)
            {
                var d_TagExplains = CurrentDb.SvHealthTagExplain.Where(m => arr_SmTags.Contains(m.TagName)).Take(4).ToList();

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
            smDvs.Add(SvUtil.GetSmSmsc(d_DayRpt.SmSmsc, "2"));
            smDvs.Add(SvUtil.GetSmRsxs(d_DayRpt.SmRsxs, "2"));
            smDvs.Add(SvUtil.GetSmSdsmsc(d_DayRpt.SmSdsmsc, "2"));
            smDvs.Add(SvUtil.GetHxZtahizs(d_DayRpt.HxZtahizs));
            smDvs.Add(SvUtil.GetXlDcjzxl(d_DayRpt.XlDcjzxl));
            smDvs.Add(SvUtil.GetHrvXzznl(d_DayRpt.HrvXzznl));
            #endregion


            var consult = new { isOpen = false, tmpImg = "" };

            if (d_SvUser.MerchId == "46120614" || d_SvUser.MerchId == "94718084")
            {
                consult = new { isOpen = true, tmpImg = "http://file.17fanju.com/upload/yuyi_consult.png" };
            }

            //var sum_HealthScores = CurrentDb.SvHealthDayReport.Select(m => m.HealthScore).ToList();
            //int scoreRatio = 80;
            //if (sum_HealthScores.Count > 0)
            //{
            //    int a = sum_HealthScores.Where(m => m < d_DayRpt.HealthScore).Count();
            //    int b = sum_HealthScores.Count();
            //    double r = Math.Round((Convert.ToDouble(a) / Convert.ToDouble(b)), 2) * 100;
            //    scoreRatio = Convert.ToInt32(r);
            //}

            var ret = new
            {
                rd = new
                {
                    HealthDate = d_DayRpt.CreateTime.ToUnifiedFormatDate(),
                    HealthScore = SvUtil.GetHealthScore(d_DayRpt.HealthScore),
                    HealthScoreTip = "您今天的健康值超过" + d_DayRpt.HealthScoreRatio + "%的人",
                    SmScoreByLast = smScoreByLast,
                    SmScore = SvUtil.GetSmScore(d_DayRpt.SmScore, true, smScoreByLast),
                    SmScoreTip = "您的睡眠值已经打败" + d_DayRpt.SmScoreRatio + "%的人",
                    GzTags = gzTags,//关注标签
                    SmTags = smTags,//睡眠标签
                    SmDvs = smDvs,//睡觉检测项
                    RptSuggest = d_DayRpt.RptSuggest,
                    HxZtahizs = SvUtil.GetHxZtahizs(d_DayRpt.HxZtahizs),
                    HrvXzznl = SvUtil.GetHrvXzznl(d_DayRpt.HrvXzznl),
                    SmSmxl = SvUtil.GetSmSmxl(d_DayRpt.SmSmxl),
                    SmSmlxx = SvUtil.GetSmSmlxx(d_DayRpt.SmSmlxx),
                    SmSdsmbl = SvUtil.GetSmSdsmbl(d_DayRpt.SmSdsmbl),
                    XlDcjzxl = SvUtil.GetXlCqjzxl(d_DayRpt.XlDcjzxl),
                    XlCqjzxl = SvUtil.GetXlCqjzxl(d_DayRpt.XlCqjzxl),
                    HxDcjzhx = SvUtil.GetHxDcjzhx(d_DayRpt.HxDcjzhx),
                    SmSmsc = SvUtil.GetSmSmsc(d_DayRpt.SmSmsc, "2"),
                    SmZcsc = SvUtil.GetSmSmsc(d_DayRpt.SmZcsc, "2"),
                    SmZcsjfw = SvUtil.GetSmZcsjfw(d_DayRpt.SmScsj, d_DayRpt.SmLcsj),
                },
                userInfo = userInfo,
                consult = consult
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult UpdateVisitCount(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var d_DayRpt = CurrentDb.SvHealthDayReport.Where(m => m.Id == rptId).FirstOrDefault();
            if (d_DayRpt != null)
            {
                d_DayRpt.VisitCount += 1;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

            return result;
        }
    }
}
