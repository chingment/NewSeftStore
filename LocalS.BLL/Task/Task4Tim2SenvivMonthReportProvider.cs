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

                        var d_DayReports = CurrentDb.SenvivHealthDayReport.Where(m => m.SvUserId == d_User.Id && m.IsValid == true && System.Data.Entity.DbFunctions.DiffMonths(m.HealthDate, DateTime.Now) == 0).ToList();
                        if (d_DayReports.Count > 0)
                        {

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


                            d_MonthReport = new SenvivHealthMonthReport();
                            d_MonthReport.Id = IdWorker.Build(IdType.NewGuid);
                            d_MonthReport.HealthDate = month;
                            d_MonthReport.SvUserId = d_User.Id;
                            d_MonthReport.DayCount = d_DayReports.Count;
                            d_MonthReport.TotalScore = d_DayReports.Select(m => m.TotalScore).Average();

                            d_MonthReport.MylGrfx = Decimal.Parse(d_DayReports.Select(m => m.MylGrfx).Average().ToString());//
                            d_MonthReport.MylMylzs = Decimal.Parse(d_DayReports.Select(m => m.MylMylzs).Average().ToString());//
                            d_MonthReport.MylMylzs = Decimal.Parse(d_DayReports.Select(m => m.MylMylzs).Average().ToString());//
                            d_MonthReport.MbGxbgk = Decimal.Parse(d_DayReports.Select(m => m.MbGxbgk).Average().ToString());//
                            d_MonthReport.MbGxygk = Decimal.Parse(d_DayReports.Select(m => m.MbGxygk).Average().ToString());//
                            d_MonthReport.MbTlbgk = Decimal.Parse(d_DayReports.Select(m => m.MbTlbgk).Average().ToString());//

                            d_MonthReport.SmSmsc = Decimal.Parse(d_DayReports.Select(m => m.SmSmsc).Average().ToString());//
                            d_MonthReport.SmQdsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmQdsmsc).Average().ToString());//
                            d_MonthReport.SmQdsmbl = Decimal.Parse(d_DayReports.Select(m => m.SmQdsmbl).Average().ToString());//
                            d_MonthReport.SmSdsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmSdsmsc).Average().ToString());//
                            d_MonthReport.SmSdsmbl = Decimal.Parse(d_DayReports.Select(m => m.SmSdsmbl).Average().ToString());//
                            d_MonthReport.SmRemsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmRemsmsc).Average().ToString());//
                            d_MonthReport.SmRemsmbl = Decimal.Parse(d_DayReports.Select(m => m.SmRemsmbl).Average().ToString());//
                            d_MonthReport.SmTdcs = Decimal.Parse(d_DayReports.Select(m => m.SmTdcs).Average().ToString());//
                            d_MonthReport.SmSmzq = Decimal.Parse(d_DayReports.Select(m => m.SmSmzq).Average().ToString());//

                            d_MonthReport.QxxlQxyj = Decimal.Parse(d_DayReports.Select(m => m.QxxlQxyj).Average().ToString());//
                            d_MonthReport.QxxlKynl = Decimal.Parse(d_DayReports.Select(m => m.QxxlKynl).Average().ToString());//
                            d_MonthReport.HrvXzznl = Decimal.Parse(d_DayReports.Select(m => m.HrvXzznl).Average().ToString());//
                            d_MonthReport.HxDcpjhx = Decimal.Parse(d_DayReports.Select(m => m.HxDcpjhx).Average().ToString());//
                            d_MonthReport.HxDcjzhx = Decimal.Parse(d_DayReports.Select(m => m.HxDcjzhx).Average().ToString());//
                            d_MonthReport.HxCqjzhx = Decimal.Parse(d_DayReports.Select(m => m.HxCqjzhx).Average().ToString());//
                            d_MonthReport.XlDcpjxl = Decimal.Parse(d_DayReports.Select(m => m.XlDcpjxl).Average().ToString());//
                            d_MonthReport.XlDcjzxl = Decimal.Parse(d_DayReports.Select(m => m.XlDcjzxl).Average().ToString());//
                            d_MonthReport.HxZtahizs = Decimal.Parse(d_DayReports.Select(m => m.HxZtahizs).Average().ToString());//
                            d_MonthReport.HxZtcs = Decimal.Parse(d_DayReports.Select(m => m.HxZtcs).Average().ToString());//


                            d_MonthReport.HrvJgsjzlzs = Decimal.Parse(d_DayReports.Select(m => m.HrvJgsjzlzs).Average().ToString());//
                            d_MonthReport.HrvMzsjzlzs = Decimal.Parse(d_DayReports.Select(m => m.HrvMzsjzlzs).Average().ToString());//
                            d_MonthReport.HrvZzsjzlzs = Decimal.Parse(d_DayReports.Select(m => m.HrvZzsjzlzs).Average().ToString());//
                            d_MonthReport.HrvHermzs = Decimal.Parse(d_DayReports.Select(m => m.HrvHermzs).Average().ToString());//
                            d_MonthReport.HrvTwjxgsszs = Decimal.Parse(d_DayReports.Select(m => m.HrvTwjxgsszs).Average().ToString());//

                            d_MonthReport.JbfxXljsl = Decimal.Parse(d_DayReports.Select(m => m.JbfxXljsl).Average().ToString());//
                            d_MonthReport.JbfxXlscfx = Decimal.Parse(d_DayReports.Select(m => m.JbfxXlscfx).Average().ToString());//

                            d_MonthReport.DatePt = d_DayReports.Select(m => m.HealthDate.ToUnifiedFormatDate()).ToJsonString();//
                            d_MonthReport.SmSmscPt = d_DayReports.Select(m => m.SmSmsc).ToJsonString();//
                            // d_MonthReport.SmDtqcsPt = d_DayReports.Select(m => m.SmDtqcs).ToJsonString();
                            d_MonthReport.XlDcjzxlPt = d_DayReports.Select(m => m.XlDcjzxl).ToJsonString();//
                            d_MonthReport.XlCqjzxlPt = d_DayReports.Select(m => m.XlCqjzxl).ToJsonString();//
                            d_MonthReport.HrvXzznlPt = d_DayReports.Select(m => m.HrvXzznl).ToJsonString();//
                            d_MonthReport.HxZtcsPt = d_DayReports.Select(m => m.HxZtcs).ToJsonString();//
                            d_MonthReport.HxDcjzhxPt = d_DayReports.Select(m => m.HxDcjzhx).ToJsonString();//
                            d_MonthReport.HxCqjzhxPt = d_DayReports.Select(m => m.HxCqjzhx).ToJsonString();//
                            d_MonthReport.HxZtahizsPt = d_DayReports.Select(m => m.HxZtahizs).ToJsonString();//
                            d_MonthReport.HrvJgsjzlzsPt = d_DayReports.Select(m => m.HrvJgsjzlzs).ToJsonString();//
                            d_MonthReport.HrvMzsjzlzsPt = d_DayReports.Select(m => m.HrvMzsjzlzs).ToJsonString();//
                            d_MonthReport.HrvZzsjzlzsPt = d_DayReports.Select(m => m.HrvZzsjzlzs).ToJsonString();//
                            d_MonthReport.HrvHermzsPt = d_DayReports.Select(m => m.HrvHermzs).ToJsonString();//
                            d_MonthReport.HrvTwjxgsszsPt = d_DayReports.Select(m => m.HrvTwjxgsszs).ToJsonString();//
                            d_MonthReport.JbfxXlscfxPt = d_DayReports.Select(m => m.JbfxXlscfx).ToJsonString();//
                            d_MonthReport.JbfxXljslPt = d_DayReports.Select(m => m.JbfxXljsl).ToJsonString();//

                            var smTags_Count = smTags.GroupBy(s => s).Select(group => new { Name = group.Key, Count = group.Count() });

                            d_MonthReport.SmTags = smTags_Count.OrderByDescending(m => m.Count).ToJsonString();

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
