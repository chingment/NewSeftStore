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
        public readonly string TAG = "Task4Tim2SenvivArticlePushProvider";

        public void Execute(IJobExecutionContext context)
        {
            LogUtil.Info(TAG, "start");

            try
            {
                var d_Users = (from u in CurrentDb.SenvivUser
                               join m in CurrentDb.SenvivUserWomen on u.Id equals m.SvUserId
                               where u.Sex == "2"
                               select new { m.PregnancyTime, u.UserId, u.MerchId, m.SvUserId, m.DeliveryTime, u.CareMode, u.FullName }).ToList();


                LogUtil.Info(TAG, "d_Users.Count:" + d_Users.Count);

                foreach (var d_User in d_Users)
                {
                    LogUtil.Info(TAG, "d_User.FullName:" + d_User.FullName + ",CareMode:" + d_User.CareMode + ",DeliveryTime:" + d_User.FullName);

                    if (d_User.CareMode == Entity.E_SenvivUserCareMode.Pregnancy)
                    {
                        #region    //怀孕中

                        var term = Lumos.CommonUtil.GetDiffWeekDay(d_User.PregnancyTime, DateTime.Now);
                        string search_tag = string.Format("孕期{0}周", term.Week);
                        string cdType = "wx_tpl_pregnancy_remind";
                        string cdValue = term.Week.ToString();
                        var pushMessageLog = CurrentDb.PushMessageLog.Where(m => m.UserId == d_User.SvUserId && m.CdType == cdType && m.CdValue == cdValue).FirstOrDefault();
                        if (pushMessageLog == null)
                        {
                            var d_Article = CurrentDb.SenvivArticle.Where(m => m.Tags.StartsWith(search_tag)).FirstOrDefault();
                            if (d_Article != null)
                            {
                                string first = "您好";
                                string url = string.Format("http://health.17fanju.com/article/bw/details?id={0}&uid={1}", d_Article.Id, d_User.SvUserId);
                                string keyword1 = "162天";
                                string keyword2 = d_Article.Title;
                                string remark = "感谢您的支持,祝您的宝宝健康成长";

                                bool isFlag = BizFactory.Senviv.SendArticle(d_User.SvUserId, first, keyword1, keyword2, remark, url);

                                if (isFlag)
                                {
                                    pushMessageLog = new Entity.PushMessageLog();
                                    pushMessageLog.Id = IdWorker.Build(IdType.NewGuid);
                                    pushMessageLog.UserId = d_User.SvUserId;
                                    pushMessageLog.CdType = "wx_tpl_pregnancy_remind";
                                    pushMessageLog.CdValue = cdValue;
                                    pushMessageLog.Creator = IdWorker.Build(IdType.EmptyGuid);
                                    pushMessageLog.CreateTime = DateTime.Now;
                                    CurrentDb.PushMessageLog.Add(pushMessageLog);
                                    CurrentDb.SaveChanges();
                                }
                            }
                        }


                        #endregion

                    }
                    else if (d_User.CareMode == Entity.E_SenvivUserCareMode.Postpartum)
                    {
                        #region 分娩后

                        var search_term = Lumos.CommonUtil.GetDiffWeekDay(d_User.DeliveryTime, DateTime.Now);

                        string[] search_tags = new string[] { string.Format("产后第{0}天", search_term.Day), string.Format("产后第{0}周", search_term.Week) };

                        LogUtil.Info(TAG, "search_tags.Count:" + search_tags.Length + string.Format("产后第{0}天", search_term.Day) + "," + string.Format("产后第{0}周", search_term.Week));

                        foreach (var search_tag in search_tags)
                        {
                            string cdType = "article_postpartum";
                            var d_SendLog = CurrentDb.PushMessageLog.Where(m => m.SvUserId == d_User.SvUserId && m.CdType == cdType && m.CdValue == search_tag).FirstOrDefault();
                            if (d_SendLog != null)
                                return;

                            var d_Article = CurrentDb.SenvivArticle.Where(m => m.Tags.StartsWith(search_tag)).FirstOrDefault();
                            if (d_Article == null)
                                return;

                            string title = string.Format("您好,{0}", search_tag);
                            string url = string.Format("http://health.17fanju.com/article/details?id={0}&svuid={1}", d_Article.Id, d_User.SvUserId);
                            string remark = "感谢您的支持";

                            bool isSend = BizFactory.Senviv.SendArticleByPostpartum(d_User.SvUserId, title, remark, url);

                            if (isSend)
                            {
                                d_SendLog = new Entity.PushMessageLog();
                                d_SendLog.Id = IdWorker.Build(IdType.NewGuid);
                                d_SendLog.MerchId = d_User.MerchId;
                                d_SendLog.UserId = d_User.UserId;
                                d_SendLog.SvUserId = d_User.SvUserId;
                                d_SendLog.CdType = cdType;
                                d_SendLog.CdValue = search_tag;
                                d_SendLog.Creator = IdWorker.Build(IdType.EmptyGuid);
                                d_SendLog.CreateTime = DateTime.Now;
                                CurrentDb.PushMessageLog.Add(d_SendLog);
                                CurrentDb.SaveChanges();
                            }
                        }

                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }

        }
    }
}