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
            var d_Womens = CurrentDb.SenvivUserWomen.ToList();

            foreach (var d_Women in d_Womens)
            {
                var term = Lumos.CommonUtil.GetPregnancyWeeks(d_Women.PregnancyTime, DateTime.Now);
                string search_tag = string.Format("孕期{0}周", term.Week);
                string cdType = "wx_tpl_pregnancy_remind";
                string cdValue = term.Week.ToString();
                var pushMessageLog = CurrentDb.PushMessageLog.Where(m => m.UserId == d_Women.SvUserId && m.CdType == cdType && m.CdValue == cdValue).FirstOrDefault();
                if (pushMessageLog == null)
                {
                    var d_Article = CurrentDb.SenvivArticle.Where(m => m.Tags.StartsWith(search_tag)).FirstOrDefault();
                    if (d_Article != null)
                    {
                        string first = "您好";
                        string url = string.Format("http://health.17fanju.com/#/article/bw/details?id={0}&uid={1}", d_Article.Id, d_Women.SvUserId);
                        string keyword1 = "162天";
                        string keyword2 = d_Article.Title;
                        string remark = "感谢您的支持,祝您的宝宝健康成长";

                        bool isFlag = SdkFactory.Senviv.SendArticle(d_Women.SvUserId, first, keyword1, keyword2, remark, url);

                        if (isFlag)
                        {
                            pushMessageLog = new Entity.PushMessageLog();
                            pushMessageLog.Id = IdWorker.Build(IdType.NewGuid);
                            pushMessageLog.UserId = d_Women.SvUserId;
                            pushMessageLog.CdType = "wx_tpl_pregnancy_remind";
                            pushMessageLog.CdValue = cdValue;
                            pushMessageLog.Creator = IdWorker.Build(IdType.EmptyGuid);
                            pushMessageLog.CreateTime = DateTime.Now;
                            CurrentDb.PushMessageLog.Add(pushMessageLog);
                            CurrentDb.SaveChanges();
                        }

                    }
                }

            }

        }
    }
}