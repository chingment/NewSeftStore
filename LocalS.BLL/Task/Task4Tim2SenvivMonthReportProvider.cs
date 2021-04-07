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


            var d_Users = CurrentDb.SenvivUser.ToList();

            foreach (var d_User in d_Users)
            {

                string month = DateTime.Now.ToString("yyyy-MM");

                var d_MonthReport = CurrentDb.SenvivHealthMonthReport.Where(m => m.HealthDate == month && m.SvUserId == d_User.Id).FirstOrDefault();

                if (d_MonthReport == null)
                {

                    var d_DayReports = CurrentDb.SenvivHealthDayReport.Where(m => m.SvUserId == d_User.Id).ToList();

                    List<object> smSmscPt = new List<object>();
                    List<object> hrvXzznlPt = new List<object>();
                    List<object> hxztcsPt = new List<object>();

                    foreach (var d in d_DayReports)
                    {
                        smSmscPt.Add(new string[] { d.HealthDate.ToUnifiedFormatDate(), d.SmSmsc.ToString() });
                    }

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
                    d_MonthReport.SmRemsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmSemsmsc).Average().ToString());//
                    d_MonthReport.HrvXzznl = Decimal.Parse(d_DayReports.Select(m => m.HrvXzznl).Average().ToString());//
                    d_MonthReport.HrvXzznlPt = hrvXzznlPt.ToJsonString();//

                    d_MonthReport.HxPjhx = Decimal.Parse(d_DayReports.Select(m => m.HxDcpjhx).Average().ToString());//
                    d_MonthReport.XlPjxl = Decimal.Parse(d_DayReports.Select(m => m.XlDcpjxl).Average().ToString());//
                    d_MonthReport.HxztPjAhizs = Decimal.Parse(d_DayReports.Select(m => m.HxZtAhizs).Average().ToString());//


                    d_MonthReport.SmTdcs = Decimal.Parse(d_DayReports.Select(m => m.SmTdcs).Average().ToString());//

                    d_MonthReport.Hxztcs = Decimal.Parse(d_DayReports.Select(m => m.HxZtcs).Average().ToString());//
                    d_MonthReport.HxztcsPt = hxztcsPt.ToJsonString();//



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
}
