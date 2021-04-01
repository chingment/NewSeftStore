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

                    d_MonthReport = new SenvivHealthMonthReport();
                    d_MonthReport.Id = IdWorker.Build(IdType.NewGuid);
                    d_MonthReport.HealthDate = month;
                    d_MonthReport.SvUserId = d_User.Id;
                    d_MonthReport.DayCount = d_DayReports.Count;
                    d_MonthReport.TotalScore = d_DayReports.Select(m => m.TotalScore).Average();
                    d_MonthReport.SmSmsc = 0;
                    d_MonthReport.SmQdsmsc = 0;
                    d_MonthReport.SmSdsmsc = 0;
                    d_MonthReport.SmRemsmsc = 0;
                    d_MonthReport.SmTdcs = 0;
                    d_MonthReport.HrvXzznl = 0;
                    d_MonthReport.HrvPjhx = 0;
                    d_MonthReport.HrvPjxl = 0;
                    d_MonthReport.HrvPjahizs = 0;
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
