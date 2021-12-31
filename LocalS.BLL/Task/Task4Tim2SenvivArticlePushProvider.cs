using LocalS.BLL.Biz;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Task
{
    public class Task4Tim2SenvivArticlePushProvider : BaseService, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var d_Gravidas = CurrentDb.SenvivUserGravida.ToList();

            foreach (var d_Gravida in d_Gravidas)
            {
                var week = Lumos.CommonUtil.GetPregnancyWeeks(d_Gravida.PregnancyTime, DateTime.Now);

                string startsWith = string.Format("孕期{0}周", week);

                var d_Article = CurrentDb.SenvivArticle.Where(m => m.Tags.StartsWith(startsWith)).FirstOrDefault();

                if (d_Article != null)
                {
                    string first = "您好";
                    string url = "http://health.17fanju.com/#/article/bw/details?id=" + d_Article.Id;
                    string keyword1 = "162天";
                    string keyword2 = d_Article.Title;
                    string remark = "感谢您的支持,祝您的宝宝健康成长";

                    bool isFlag = SdkFactory.Senviv.SendArticle(d_Gravida.SvUserId, first, keyword1, keyword2, remark, url);

                }

            }

        }
    }
}