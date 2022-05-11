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

            int reportCount = 0;

            var togetherDays = (int)(d_DayRpt.ReportTime - d_SvUser.CreateTime).TotalDays + 1;


            var pregnancy = new { birthLastDays = 0, gesWeek = 0, gesDay = 0 };

            if (d_SvUser.CareMode == Entity.E_SvUserCareMode.Pregnancy)
            {
                var d_Women = CurrentDb.SvUserWomen.Where(m => m.SvUserId == d_SvUser.Id).FirstOrDefault();
                if (d_Women != null)
                {
                    if (d_Women.PregnancyTime != null && d_Women.DeliveryTime.Value != null)
                    {
                        var week = Lumos.CommonUtil.GetDiffWeekDay(DateTime.Parse(d_Women.PregnancyTime.Value.ToString("yyyy-MM-dd")), DateTime.Parse(d_DayRpt.CreateTime.ToString("yyyy-MM-dd")));
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
            List<object> mbTnbgkByLast = new List<object>();
            List<object> qxxlKynlByLast = new List<object>();
            List<object> qxxlQxyjByLast = new List<object>();
            List<ChatDataByStr> qxxlJlqxByLast = new List<ChatDataByStr>();
            List<object> zsGmYqByLast = new List<object>();
            List<object> zsGmYpByLast = new List<object>();
            List<object> zsGmSrByLast = new List<object>();
            List<object> hrvHermzsByLast = new List<object>();
            List<object> jbfxXlscfxByLast = new List<object>();

            var d_DayReportsByLast = (from u in CurrentDb.SvHealthDayReport
                                      where u.SvUserId == d_DayRpt.SvUserId && u.IsValid == true
                                      && u.ReportTime <= d_DayRpt.ReportTime
                                      select new { u.ReportTime, u.SmScore, u.MylMylzs, u.MylGrfx, u.MbGxygk, u.MbTnbgk, u.JbfxXlscfx, u.HrvHermzs, u.MbGxbgk, u.QxxlKynl, u.QxxlQxyj, u.QxxlJlqx, u.ZsGmSr, u.ZsGmYp, u.ZsGmYq, u.CreateTime }).OrderByDescending(m => m.ReportTime).Take(7).ToList();

            d_DayReportsByLast.Reverse();

            foreach (var d_DayReportByLast in d_DayReportsByLast)
            {
                smScoreByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmScore });
                mylMylzsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.MylMylzs });
                mylGrfxByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.MylGrfx });
                mbGxygkByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.MbGxygk });
                mbGxbgkByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.MbGxbgk });
                mbTnbgkByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.MbTnbgk });
                qxxlKynlByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.QxxlKynl });
                qxxlQxyjByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.QxxlQxyj });
                qxxlJlqxByLast.Add(new ChatDataByStr { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.QxxlJlqx });
                zsGmYqByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.ZsGmYq });
                zsGmYpByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.ZsGmYp });
                zsGmSrByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.ZsGmSr });
                hrvHermzsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HrvHermzs });
                jbfxXlscfxByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.JbfxXlscfx });
            }

            #endregion

            #region  gzTags
            var gzTags = new List<object>();

            gzTags.Add(SvUtil.GetMylzs(decimal.Floor(d_DayRpt.MylMylzs), true, mylMylzsByLast));
            gzTags.Add(SvUtil.GetMylGrfx(decimal.Floor(d_DayRpt.MylGrfx), true, mylGrfxByLast));
            gzTags.Add(SvUtil.GetQxxlQxyj(decimal.Floor(d_DayRpt.QxxlQxyj), true, qxxlQxyjByLast));
            gzTags.Add(SvUtil.GetQxxlJlqx(d_DayRpt.QxxlJlqx, true, qxxlJlqxByLast));
            gzTags.Add(SvUtil.GetQxxlKynl(decimal.Floor(d_DayRpt.QxxlKynl), true, qxxlKynlByLast));
            #endregion

            #region  nxTags
            var nxTags = new List<object>();
            if (d_SvUser.Sex == "2")
            {
                if (d_SvUser.CareMode != Entity.E_SvUserCareMode.Pregnancy)
                {
                    nxTags.Add(SvUtil.GetZsGmYq(decimal.Floor(d_DayRpt.ZsGmYq), true, zsGmYqByLast));
                }

                nxTags.Add(SvUtil.GetZsGmYp(decimal.Floor(d_DayRpt.ZsGmYp), true, zsGmYpByLast));
                nxTags.Add(SvUtil.GetZsGmSr(decimal.Floor(d_DayRpt.ZsGmSr), true, zsGmSrByLast));
            }
            #endregion

            #region  mbTags
            var mbTags = new List<object>();
            string chronicdisease = d_SvUser.Chronicdisease == null ? "" : d_SvUser.Chronicdisease;

            if (chronicdisease.IndexOf("4") > -1)
            {
                mbTags.Add(SvUtil.GetMbTnbgk(decimal.Floor(d_DayRpt.MbTnbgk), true, mbTnbgkByLast));
            }

            if (chronicdisease.IndexOf("5") > -1)
            {
                mbTags.Add(SvUtil.GetMbGxygk(decimal.Floor(d_DayRpt.MbGxygk), true, mbGxygkByLast));
            }

            if (chronicdisease.IndexOf("6") > -1)
            {
                mbTags.Add(SvUtil.GetMbGxbgk(decimal.Floor(d_DayRpt.MbGxbgk), true, mbGxbgkByLast));
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

            smDvs.Add(SvUtil.GetSmZcsc(d_DayRpt.SmZcsc));
            smDvs.Add(SvUtil.GetSmSmsc(d_DayRpt.SmSmsc));
            smDvs.Add(SvUtil.GetSmRsxs(d_DayRpt.SmRsxs));
            smDvs.Add(SvUtil.GetSmSmzq(d_DayRpt.SmSmzq));
            smDvs.Add(SvUtil.GetSmSdsmsc(d_DayRpt.SmSdsmsc));
            smDvs.Add(SvUtil.GetSmQdsmsc(d_DayRpt.SmQdsmsc));
            smDvs.Add(SvUtil.GetSmRemsmsc(d_DayRpt.SmRemsmsc));
            smDvs.Add(SvUtil.GetSmLzcs(d_DayRpt.SmLzcs));
            smDvs.Add(SvUtil.GetSmTdcs(d_DayRpt.SmTdcs));
            smDvs.Add(SvUtil.GetHxZtahizs(d_DayRpt.HxZtahizs));

            //smDvs.Add(SvUtil.GetXlDcjzxl(d_DayRpt.XlDcjzxl));
            //smDvs.Add(SvUtil.GetHrvXzznl(d_DayRpt.HrvXzznl, d_DayRpt.HrvXzznljzz, d_SvUser.ReportCount));
            #endregion


            var consult = new { isOpen = true, tmpImg = "http://file.17fanju.com/upload/yuyi_consult.png" };

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
                    NxTags = nxTags,//女性标签
                    SmTags = smTags,//睡眠标签
                    MbTags = mbTags,//慢病标签
                    SmDvs = smDvs,//睡觉检测项
                    RptSuggest = d_DayRpt.RptSuggest,
                    HxZtahizs = SvUtil.GetHxZtahizs(d_DayRpt.HxZtahizs),
                    HrvXzznl = SvUtil.GetHrvXzznl(d_DayRpt.HrvXzznl, d_DayRpt.HrvXzznljzz, d_SvUser.ReportCount),
                    SmSmxl = SvUtil.GetSmSmxl(d_DayRpt.SmSmxl),
                    SmSmlxx = SvUtil.GetSmSmlxx(d_DayRpt.SmSmlxx),
                    SmSdsmbl = SvUtil.GetSmSdsmbl(d_DayRpt.SmSdsmbl),
                    XlDcjzxl = SvUtil.GetXlCqjzxl(d_DayRpt.XlDcjzxl),
                    XlCqjzxl = SvUtil.GetXlCqjzxl(d_DayRpt.XlCqjzxl),
                    HxDcjzhx = SvUtil.GetHxDcjzhx(d_DayRpt.HxDcjzhx),
                    SmSmsc = SvUtil.GetSmSmsc(d_DayRpt.SmSmsc),
                    SmZcsc = SvUtil.GetSmZcsc(d_DayRpt.SmZcsc),
                    SmZcsjfw = SvUtil.GetSmZcsjfw(d_DayRpt.SmScsj, d_DayRpt.SmLcsj),
                },
                userInfo = userInfo,
                consult = consult
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetIndicator(string operater, string rptId)
        {
            var result = new CustomJsonResult();

            var d_Rpt = CurrentDb.SvHealthDayReport.Where(m => m.Id == rptId).FirstOrDefault();


            #region last7DayData


            List<object> smSmscByLast = new List<object>();
            List<object> smSmxlByLast = new List<object>();
            List<object> smRsxsByLast = new List<object>();
            List<object> smSmlxxByLast = new List<object>();
            List<object> smSmzqByLast = new List<object>();
            List<object> smQdsmblByLast = new List<object>();
            List<object> smSdsmblByLast = new List<object>();
            List<object> smRemsmblByLast = new List<object>();
            List<object> smLzcsByLast = new List<object>();
            List<object> smTdcsByLast = new List<object>();
            List<object> xlDcjzxlByLast = new List<object>();
            List<object> xlCqjzxlByLast = new List<object>();
            List<object> xlDcpjxlByLast = new List<object>();
            List<object> hxDcjzhxByLast = new List<object>();
            List<object> hxCqjzhxByLast = new List<object>();
            List<object> hxDcpjhxByLast = new List<object>();
            List<object> hxZtahizsByLast = new List<object>();
            List<object> hxZtcsByLast = new List<object>();
            List<object> hrvXzznlByLast = new List<object>();
            List<object> hrvJgsjzlzsByLast = new List<object>();
            List<object> hrvMzsjzlzsByLast = new List<object>();
            List<object> hrvZzsjzlzsByLast = new List<object>();
            List<object> jbfxXlscfxByLast = new List<object>();
            List<object> jbfxXljslByLast = new List<object>();



            var d_DayReportsByLast = (from u in CurrentDb.SvHealthDayReport
                                      where u.SvUserId == d_Rpt.SvUserId && u.IsValid == true
                                      && u.ReportTime <= d_Rpt.ReportTime
                                      select new { u.ReportTime, u.SmSmsc, u.SmSmxl, u.SmRsxs, u.SmSmlxx, u.SmSmzq, u.SmQdsmbl, u.SmSdsmbl, u.SmRemsmbl, u.SmLzcs,
                                          u.SmTdcs,
                                          u.XlDcjzxl,
                                          u.XlCqjzxl,
                                          u.XlDcpjxl,
                                          u.HxDcjzhx,
                                          u.HxCqjzhx,
                                          u.HxDcpjhx,
                                          u.HxZtahizs,
                                          u.HxZtcs,
                                          u.HrvXzznl,
                                          u.HrvJgsjzlzs,
                                          u.HrvMzsjzlzs,
                                          u.HrvZzsjzlzs,
                                          u.JbfxXlscfx,
                                          u.JbfxXljsl
                                      }).OrderByDescending(m => m.ReportTime).Take(7).ToList();

            d_DayReportsByLast.Reverse();

            foreach (var d_DayReportByLast in d_DayReportsByLast)
            {
                smSmscByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = SvUtil.Covevt2Hour(d_DayReportByLast.SmSmsc) });
                smSmxlByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmSmxl });
                smRsxsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = TimeSpan.FromSeconds(double.Parse(d_DayReportByLast.SmRsxs.ToString())).TotalMinutes });
                smSmlxxByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmSmlxx });
                smSmzqByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmSmzq });
                smQdsmblByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmQdsmbl });
                smSdsmblByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmSdsmbl });
                smRemsmblByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmRemsmbl });
                smLzcsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmLzcs });
                smTdcsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.SmTdcs });
                xlDcjzxlByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.XlDcjzxl });
                xlCqjzxlByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.XlCqjzxl });
                xlDcpjxlByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.XlDcpjxl });
                hxDcjzhxByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HxDcjzhx });
                hxCqjzhxByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HxCqjzhx });
                hxDcpjhxByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HxDcpjhx });
                hxZtahizsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HxZtahizs });
                hxZtcsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HxZtcs });
                hrvXzznlByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HrvXzznl });
                hrvJgsjzlzsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HrvJgsjzlzs });
                hrvMzsjzlzsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HrvMzsjzlzs});
                hrvZzsjzlzsByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.HrvZzsjzlzs });
                jbfxXlscfxByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.JbfxXlscfx });
                jbfxXljslByLast.Add(new { xData = d_DayReportByLast.ReportTime.ToString("MM-dd"), yData = d_DayReportByLast.JbfxXljsl });


            }

            #endregion


            var ret = new
            {

                rd = new
                {
                    SmSmsc = SvUtil.GetSmSmsc(d_Rpt.SmSmsc, true, smSmscByLast),
                    SmSmxl = SvUtil.GetSmSmxl(d_Rpt.SmSmxl, true, smSmxlByLast),
                    SmRsxs = SvUtil.GetSmRsxs(d_Rpt.SmRsxs, true, smRsxsByLast),
                    SmSmlxx = SvUtil.GetSmSmlxx(d_Rpt.SmSmlxx, true, smSmlxxByLast),
                    SmSmzq = SvUtil.GetSmSmzq(d_Rpt.SmSmzq, true, smSmzqByLast),
                    SmQdsmbl = SvUtil.GetSmQdsmbl(d_Rpt.SmQdsmbl, true, smQdsmblByLast),
                    SmSdsmbl = SvUtil.GetSmSdsmbl(d_Rpt.SmSdsmbl, true, smSdsmblByLast),
                    SmRemsmbl = SvUtil.GetSmRemsmbl(d_Rpt.SmRemsmbl, true, smRemsmblByLast),
                    SmLzcs = SvUtil.GetSmLzcs(d_Rpt.SmLzcs, true, smLzcsByLast),
                    SmTdcs = SvUtil.GetSmTdcs(d_Rpt.SmTdcs, true, smTdcsByLast),


                    XlDcjzxl = SvUtil.GetXlDcjzxl(d_Rpt.XlDcjzxl, true, xlDcjzxlByLast),
                    XlCqjzxl = SvUtil.GetXlCqjzxl(d_Rpt.XlCqjzxl, true, xlCqjzxlByLast),
                    XlDcpjxl = SvUtil.GetXlDcpjxl(d_Rpt.XlDcpjxl, true, xlDcpjxlByLast),

                    HxDcjzhx = SvUtil.GetHxDcjzhx(d_Rpt.HxDcjzhx, true, hxDcjzhxByLast),
                    HxCqjzhx = SvUtil.GetHxCqjzhx(d_Rpt.HxCqjzhx, true, hxCqjzhxByLast),
                    HxDcpjhx = SvUtil.GetHxDcpjhx(d_Rpt.HxDcpjhx, true, hxDcpjhxByLast),
                    HxZtahizs = SvUtil.GetHxZtahizs(d_Rpt.HxZtahizs, true, hxZtahizsByLast),
                    HxZtcs = SvUtil.GetHxZtcs(d_Rpt.HxZtcs, true, hxZtcsByLast),


                    HrvXzznl = SvUtil.GetHrvXzznl(d_Rpt.HrvXzznl, d_Rpt.HrvJgsjzlzsjzz, 0, true, hrvXzznlByLast),
                    HrvJgsjzlzs = SvUtil.GetHrvJgsjzlzs(d_Rpt.HrvJgsjzlzs, true, hrvJgsjzlzsByLast),
                    HrvMzsjzlzs = SvUtil.GetHrvMzsjzlzs(d_Rpt.HrvMzsjzlzs, true, hrvMzsjzlzsByLast),
                    HrvZzsjzlzs = SvUtil.GetHrvZzsjzlzs(d_Rpt.HrvZzsjzlzs, true, hrvZzsjzlzsByLast),
                    JbfxXlscfx = SvUtil.GetJbfxXlscfx(d_Rpt.JbfxXlscfx, true, jbfxXlscfxByLast),
                    JbfxXljsl = SvUtil.GetJbfxXljsl(d_Rpt.JbfxXljsl, true, jbfxXljslByLast),

                    SmScsj = d_Rpt.SmScsj.ToString("yyyy/MM/dd HH:mm"),
                    SmRssj = d_Rpt.SmRssj.ToString("yyyy/MM/dd HH:mm"),
                    SmQxsj = d_Rpt.SmQxsj.ToString("yyyy/MM/dd HH:mm"),
                    SmLcsj = d_Rpt.SmLcsj.ToString("yyyy/MM/dd HH:mm"),
                    SmPoint = d_Rpt.SmPoint.ToJsonObject<object>(),
                    SmTdcsPoint= d_Rpt.SmTdcsPoint.ToJsonObject<object>(),
                    XlPoint = d_Rpt.XlPoint.ToJsonObject<object>(),
                    HxPoint = d_Rpt.HxPoint.ToJsonObject<object>()

                }
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
