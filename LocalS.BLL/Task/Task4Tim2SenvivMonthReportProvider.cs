using LocalS.Entity;
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
    public class Task4Tim2SenvivMonthReportProvider : BaseService, IJob
    {
        public void Execute(IJobExecutionContext context)
        {

            try
            {
                var d_Users = CurrentDb.SenvivUser.ToList();

                foreach (var d_User in d_Users)
                {
                    LogUtil.Info("UserId：" + d_User.Id);

                    string month = DateTime.Now.ToString("yyyy-MM");

                    var d_MonthReport = CurrentDb.SenvivHealthMonthReport.Where(m => m.HealthDate == month && m.SvUserId == d_User.Id).FirstOrDefault();

                    if (d_MonthReport == null)
                    {
                        LogUtil.Info("UserId：" + d_User.Id + ",d_MonthReport is null");

                        var d_DayReports = CurrentDb.SenvivHealthDayReport.Where(m => m.SvUserId == d_User.Id && m.IsValid == true).ToList();
                        if (d_DayReports.Count > 0)
                        {
                            List<object> smSmscPt = new List<object>();
                            List<object> hrvXzznlPt = new List<object>();
                            List<object> hxZtcsPt = new List<object>();

                            foreach (var d in d_DayReports)
                            {
                                smSmscPt.Add(new object[] { d.HealthDate.ToUnifiedFormatDate(), d.SmSmsc });
                                hrvXzznlPt.Add(new object[] { d.HealthDate.ToUnifiedFormatDate(), d.HrvXzznl });
                                hxZtcsPt.Add(new object[] { d.HealthDate.ToUnifiedFormatDate(), d.HxZtcs });
                            }

                            var _smTags = d_DayReports.Select(m => m.SmTags).ToList();
                            List<string> smTags = new List<string>();
                            foreach (var s in _smTags)
                            {
                                var arr = s.ToJsonObject<List<string>>();
                                if (arr != null)
                                {
                                    smTags.AddRange(arr);
                                }
                            }

                            var smTags_Count = smTags.GroupBy(s => s).OrderByDescending(s => s.Count()).ToList();

                           // var a= from s in smTags group

                            d_MonthReport = new SenvivHealthMonthReport();
                            d_MonthReport.Id = IdWorker.Build(IdType.NewGuid);
                            d_MonthReport.HealthDate = month;
                            d_MonthReport.SvUserId = d_User.Id;
                            d_MonthReport.DayCount = d_DayReports.Count;
                            d_MonthReport.TotalScore = d_DayReports.Select(m => m.TotalScore).Average();
                            d_MonthReport.SmSmsc = Decimal.Parse(d_DayReports.Select(m => m.SmSmsc).Average().ToString());//
                            d_MonthReport.SmSmscPt = smSmscPt.ToJsonString();//
                            d_MonthReport.SmQdsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmQdsmsc).Average().ToString());//
                            d_MonthReport.SmSdsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmSdsmsc).Average().ToString());//
                            d_MonthReport.SmRemsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmRemsmsc).Average().ToString());//
                            d_MonthReport.HrvXzznl = Decimal.Parse(d_DayReports.Select(m => m.HrvXzznl).Average().ToString());//
                            d_MonthReport.HrvXzznlPt = hrvXzznlPt.ToJsonString();//

                            d_MonthReport.HxPjhx = Decimal.Parse(d_DayReports.Select(m => m.HxDcpjhx).Average().ToString());//
                            d_MonthReport.XlPjxl = Decimal.Parse(d_DayReports.Select(m => m.XlDcpjxl).Average().ToString());//
                            d_MonthReport.HxZtpjahizs = Decimal.Parse(d_DayReports.Select(m => m.HxZtahizs).Average().ToString());//


                            d_MonthReport.SmTdcs = Decimal.Parse(d_DayReports.Select(m => m.SmTdcs).Average().ToString());//

                            d_MonthReport.HxZtcs = Decimal.Parse(d_DayReports.Select(m => m.HxZtcs).Average().ToString());//
                            d_MonthReport.HxZtcsPt = hxZtcsPt.ToJsonString();//

                            d_MonthReport.SmTags = smTags_Count.ToJsonString();

                            d_MonthReport.IsSend = false;
                            d_MonthReport.VisitCount = 0;
                            d_MonthReport.CreateTime = DateTime.Now;
                            d_MonthReport.Creator = IdWorker.Build(IdType.NewGuid);
                            CurrentDb.SenvivHealthMonthReport.Add(d_MonthReport);
                            CurrentDb.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("报告生成异常", ex);
            }
        }
    }
}
