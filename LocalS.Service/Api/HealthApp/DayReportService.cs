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


            var gzTags = new List<object>();

            gzTags.Add(new { Id = "1", Name = "孕期指数", Value = "80%" });
            gzTags.Add(new { Id = "1", Name = "水润指数", Value = "30%" });
            gzTags.Add(new { Id = "1", Name = "睡眠值", Value = "69" });
            gzTags.Add(new { Id = "1", Name = "焦虑指数", Value = "30" });
            gzTags.Add(new { Id = "1", Name = "免疫力", Value = "80" });
            gzTags.Add(new { Id = "1", Name = "高血压", Value = "0" });
            gzTags.Add(new { Id = "1", Name = "糖尿病", Value = "0" });

            var smTags = new List<object>();

            string[] names = new string[] { "消化力差", "轻度呼吸暂停", "易醒", "较难入睡" };
            var d_TagExplains = CurrentDb.SenvivHealthTagExplain.Where(m => names.Contains(m.TagName)).Take(4).ToList();

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

            var userInfo = new
            {
                signName = "逍遥",
                avatar = "https://thirdwx.qlogo.cn/mmopen/vi_32/6zcicmSoM5yjdWG9MoHydE6suFUGaHsKATFUPU7yU4d7PhLcsKWj51NhxA4PichkuYg5uJAWWhagnkyqHhAfDKGg/132"
            };


            var smDvs = new List<object>();
            smDvs.Add(SvDataJdUtil.GetSmSmsc(0));
            smDvs.Add(SvDataJdUtil.GetSmRsxs(0));
            smDvs.Add(SvDataJdUtil.GetSmSdsmsc(0));
            smDvs.Add(SvDataJdUtil.GetHxZtcs(0));
            smDvs.Add(SvDataJdUtil.GetXlDcjzxl(0));
            smDvs.Add(SvDataJdUtil.GetHrvXzznl(0));

            var ret = new
            {
                rd = new
                {
                    HealthDate="2012-12-12",
                    HealthScore = 60,
                    HealthScoreTip="您今天的健康值超过88%的人",
                    SmScore = 40,
                    SmScoreTip = "您的睡眠值已经打败77%的人",
                    GzTags = gzTags,//关注标签
                    SmTags = smTags,//睡眠标签
                    SmDvs = smDvs,//睡觉检测项
                },
                userInfo = userInfo
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
