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
                signName="逍遥"
            };

            var ret = new
            {
                rd = new
                {
                    smScore = 40,
                    smScoreTip = "您的睡眠值已经打败77%的人",
                    gzTags = gzTags,
                    smTags = smTags
                },
                userInfo = userInfo
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
