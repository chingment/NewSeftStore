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
                        var week = Lumos.CommonUtil.GetDiffWeekDay(d_Women.PregnancyTime.Value, DateTime.Now);
                        var birthLastDays = Convert.ToInt32((d_Women.DeliveryTime.Value - DateTime.Now).TotalDays + 1);
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



            #region  gzTags
            var gzTags = new List<object>();

            if (d_SvUser.Sex == "2")
            {
                if (d_DayRpt.ZsYp > 0)
                {
                    if (d_SvUser.CareMode == Entity.E_SvUserCareMode.Pregnancy)
                    {
                        gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_DayRpt.MylGrfx)));
                    }
                    else
                    {
                        gzTags.Add(SvUtil.GetZsYq(decimal.Floor(d_DayRpt.ZsYq)));
                    }

                    gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_DayRpt.MylMylzs)));
                    gzTags.Add(SvUtil.GetQxxlJlqx(d_DayRpt.QxxlJlqx));
                    gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_DayRpt.QxxlKynl)));
                    gzTags.Add(SvUtil.GetZsYp(decimal.Floor(d_DayRpt.ZsYp)));
                    gzTags.Add(SvUtil.GetZsSr(decimal.Floor(d_DayRpt.ZsSr)));
                    gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_DayRpt.QxxlQxyj)));
                }
                else
                {
                    gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_DayRpt.MylMylzs)));
                    gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_DayRpt.MylGrfx)));
                    gzTags.Add(SvUtil.GetMbGxygk(decimal.Floor(d_DayRpt.MbGxygk)));
                    gzTags.Add(SvUtil.GetMbGxbgk(decimal.Floor(d_DayRpt.MbGxbgk)));
                    gzTags.Add(SvUtil.GetMbTlbgk(decimal.Floor(d_DayRpt.MbTlbgk)));
                    gzTags.Add(SvUtil.GetQxxlJlqx(d_DayRpt.QxxlJlqx));
                    gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_DayRpt.QxxlKynl)));
                    gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_DayRpt.QxxlQxyj)));
                }
            }
            else
            {
                gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_DayRpt.MylMylzs)));
                gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_DayRpt.MylGrfx)));
                gzTags.Add(SvUtil.GetMbGxygk(decimal.Floor(d_DayRpt.MbGxygk)));
                gzTags.Add(SvUtil.GetMbGxbgk(decimal.Floor(d_DayRpt.MbGxbgk)));
                gzTags.Add(SvUtil.GetMbTlbgk(decimal.Floor(d_DayRpt.MbTlbgk)));
                gzTags.Add(SvUtil.GetQxxlJlqx(d_DayRpt.QxxlJlqx));
                gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_DayRpt.QxxlKynl)));
                gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_DayRpt.QxxlQxyj)));
            }
            #endregion

            #region smTags
            var smTags = new List<object>();

            var arr_SmTags = d_DayRpt.SmTags.ToJsonObject<string[]>();
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
            smDvs.Add(SvUtil.GetSmSmsc(d_DayRpt.SmSmsc, "2"));
            smDvs.Add(SvUtil.GetSmRsxs(d_DayRpt.SmRsxs, "2"));
            smDvs.Add(SvUtil.GetSmSdsmsc(d_DayRpt.SmSdsmsc, "2"));
            smDvs.Add(SvUtil.GetHxZtahizs(d_DayRpt.HxZtahizs));
            smDvs.Add(SvUtil.GetXlDcjzxl(d_DayRpt.XlDcjzxl));
            smDvs.Add(SvUtil.GetHrvXzznl(d_DayRpt.HrvXzznl));
            #endregion

            #region smScoreByLast


            List<object> smScoreByLast = new List<object>();
            var d_DayReportSmScores = (from u in CurrentDb.SvHealthDayReport
                                       where u.SvUserId == d_DayRpt.SvUserId && u.IsValid == true
                                       && u.HealthDate <= d_DayRpt.HealthDate
                                       select new { u.CreateTime, u.HealthDate, u.SmScore }).OrderByDescending(m => m.HealthDate).Take(7).ToList();

            d_DayReportSmScores.Reverse();

            foreach (var d_hDayReportSmScore in d_DayReportSmScores)
            {
                smScoreByLast.Add(new { xData = d_hDayReportSmScore.HealthDate.ToString("MM-dd"), yData = d_hDayReportSmScore.SmScore });
            }

            #endregion

            var consult = new { isOpen = false, tmpImg = "" };

            if (d_SvUser.MerchId == "46120614" || d_SvUser.MerchId == "94718084")
            {
                consult = new { isOpen = true, tmpImg = "http://file.17fanju.com/upload/yuyi_consult.jpg" };
            }

            var ret = new
            {
                rd = new
                {
                    HealthDate = d_DayRpt.HealthDate.ToUnifiedFormatDate(),
                    HealthScore = SvUtil.GetHealthScore(d_DayRpt.HealthScore),
                    HealthScoreTip = "您今天的健康值超过88%的人",
                    SmScore = SvUtil.GetSmScore(d_DayRpt.SmScore),
                    SmScoreTip = "您的睡眠值已经打败77%的人",
                    GzTags = gzTags,//关注标签
                    SmTags = smTags,//睡眠标签
                    SmDvs = smDvs,//睡觉检测项
                    RptSuggest = d_DayRpt.RptSuggest,
                    SmScoreByLast = smScoreByLast,
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
