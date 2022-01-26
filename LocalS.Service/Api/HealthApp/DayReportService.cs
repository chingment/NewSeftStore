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


            var togetherDays = (int)(d_Rpt.HealthDate - d_SvUser.CreateTime).TotalDays;

            var userInfo = new
            {
                SignName = d_SvUser.NickName,
                Avatar = d_SvUser.Avatar,
                TogetherDays = togetherDays
            };



            #region  gzTags
            var gzTags = new List<object>();

            gzTags.Add(SvDataJdUtil.GetMylzs(d_Rpt.MylMylzs));
            gzTags.Add(SvDataJdUtil.GetMylGrfx(d_Rpt.MylGrfx));
            gzTags.Add(SvDataJdUtil.GetMbGxygk(d_Rpt.MbGxygk));
            gzTags.Add(SvDataJdUtil.GetMbGxbgk(d_Rpt.MbGxbgk));
            gzTags.Add(SvDataJdUtil.GetMbTlbgk(d_Rpt.MbTlbgk));
            gzTags.Add(SvDataJdUtil.GetQxxlJlqx(d_Rpt.QxxlJlqx));
            gzTags.Add(SvDataJdUtil.GetQxxlKynl(d_Rpt.QxxlKynl));
            gzTags.Add(SvDataJdUtil.GetQxxlQxyj(d_Rpt.QxxlQxyj));
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
            smDvs.Add(SvDataJdUtil.GetSmSmsc(d_Rpt.SmSmsc));
            smDvs.Add(SvDataJdUtil.GetSmRsxs(d_Rpt.SmRsxs));
            smDvs.Add(SvDataJdUtil.GetSmSdsmsc(d_Rpt.SmSdsmsc));
            smDvs.Add(SvDataJdUtil.GetHxZtahizs(d_Rpt.HxZtahizs));
            smDvs.Add(SvDataJdUtil.GetXlDcjzxl(d_Rpt.XlDcjzxl));
            smDvs.Add(SvDataJdUtil.GetHrvXzznl(d_Rpt.HrvXzznl));
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
                    HealthScore = SvDataJdUtil.GetHealthScore(d_Rpt.HealthScore),
                    HealthScoreTip = "您今天的健康值超过88%的人",
                    SmScore = SvDataJdUtil.GetSmScore(d_Rpt.SmScore),
                    SmScoreTip = "您的睡眠值已经打败77%的人",
                    GzTags = gzTags,//关注标签
                    SmTags = smTags,//睡眠标签
                    SmDvs = smDvs,//睡觉检测项
                    RptSuggest = d_Rpt.RptSuggest,
                    SmScoreByLast = smScoreByLast,
                    HxZtahizs= SvDataJdUtil.GetHxZtahizs(d_Rpt.HxZtahizs),
                    HrvXzznl = SvDataJdUtil.GetHrvXzznl(d_Rpt.HrvXzznl),
                    SmSmxl = SvDataJdUtil.GetSmSmxl(d_Rpt.SmSmxl),
                    SmSmlxx = SvDataJdUtil.GetSmSmlxx(d_Rpt.SmSmlxx),
                    SmSdsmbl = SvDataJdUtil.GetSmSdsmbl(d_Rpt.SmSdsmbl),
                    XlDcjzxl = SvDataJdUtil.GetXlCqjzxl(d_Rpt.XlDcjzxl),
                    XlCqjzxl = SvDataJdUtil.GetXlCqjzxl(d_Rpt.XlCqjzxl),
                    HxDcjzhx = SvDataJdUtil.GetHxDcjzhx(d_Rpt.HxDcjzhx),
                    SmSmsc = SvDataJdUtil.GetSmSmsc(d_Rpt.SmSmsc)
                },
                userInfo = userInfo
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
