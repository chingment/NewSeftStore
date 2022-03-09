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

                        LogUtil.Info(TAG, "search_tag:" + search_tag);

                        string cdType = "article_pregnancy";

                        var d_SendLog = CurrentDb.PushMessageLog.Where(m => m.SvUserId == d_User.SvUserId && m.CdType == cdType && m.CdValue == search_tag).FirstOrDefault();
                        if (d_SendLog != null)
                            return;

                        var d_Article = CurrentDb.SenvivArticle.Where(m => m.Tags.StartsWith(search_tag)).FirstOrDefault();
                        if (d_Article == null)
                            return;

                        string title = string.Format("您好,{0}", search_tag);
                        string url = string.Format("http://health.17fanju.com/article/details?id={0}&svuid={1}", d_Article.Id, d_User.SvUserId);
                        string remark = "感谢您的支持";

                        bool isSend = BizFactory.Senviv.SendArticleByPregnancy(d_User.SvUserId, title, "", "", remark, url);

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

                        #endregion

                    }
                    else if (d_User.CareMode == Entity.E_SenvivUserCareMode.Postpartum)
                    {
                        #region 分娩后

                        string search_tag = "";

                        double totalDays = (DateTime.Now - d_User.DeliveryTime).TotalDays;

                        if (totalDays <= 30)
                        {
                            search_tag = string.Format("产后第{0}天", totalDays);
                        }
                        else
                        {
                            double week = totalDays / 7;
                            search_tag = string.Format("产后第{0}周", (int)Math.Floor(week));
                        }

                        LogUtil.Info(TAG, "search_tag:" + search_tag);

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