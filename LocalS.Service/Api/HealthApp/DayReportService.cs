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

            var userInfo = new
            {
                signName = "逍遥",
                avatar = "https://thirdwx.qlogo.cn/mmopen/vi_32/6zcicmSoM5yjdWG9MoHydE6suFUGaHsKATFUPU7yU4d7PhLcsKWj51NhxA4PichkuYg5uJAWWhagnkyqHhAfDKGg/132"
            };


            #region 
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
            #endregion

            #region smDvs
            var smDvs = new List<object>();
            smDvs.Add(SvDataJdUtil.GetSmSmsc(d_Rpt.SmSmsc));
            smDvs.Add(SvDataJdUtil.GetSmRsxs(d_Rpt.SmRsxs));
            smDvs.Add(SvDataJdUtil.GetSmSdsmsc(d_Rpt.SmSdsmsc));
            smDvs.Add(SvDataJdUtil.GetHxZtcs(d_Rpt.HxZtcs));
            smDvs.Add(SvDataJdUtil.GetXlDcjzxl(d_Rpt.XlDcjzxl));
            smDvs.Add(SvDataJdUtil.GetHrvXzznl(d_Rpt.HrvXzznl));
            #endregion

            var ret = new
            {
                rd = new
                {
                    HealthDate = d_Rpt.HealthDate.ToUnifiedFormatDate(),
                    HealthScore = 60,
                    HealthScoreTip = "您今天的健康值超过88%的人",
                    SmScore = 40,
                    SmScoreTip = "您的睡眠值已经打败77%的人",
                    GzTags = gzTags,//关注标签
                    SmTags = smTags,//睡眠标签
                    SmDvs = smDvs,//睡觉检测项
                    rptSuggest = d_Rpt.RptSuggest
                },
                userInfo = userInfo
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
