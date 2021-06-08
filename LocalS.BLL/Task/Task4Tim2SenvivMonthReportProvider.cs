using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using Quartz;
using SenvivSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace LocalS.BLL.Task
{
    public class DateValuePoint
    {
        public List<long> DataTime { get; set; }

        public List<int> DataValue { get; set; }
    }

    public class Task4Tim2SenvivMonthReportProvider : BaseService, IJob
    {
        public int InTimeSpan(DateTime t1)
        {
            if (Lumos.CommonUtil.GetTimeSpan(t1, "21:00", "23:00"))
            {
                return 1;
            }
            else if (Lumos.CommonUtil.GetTimeSpan(t1, "23:00", "01:00"))
            {
                return 1;
            }
            else if (Lumos.CommonUtil.GetTimeSpan(t1, "01:00", "03:00"))
            {
                return 3;
            }
            else if (Lumos.CommonUtil.GetTimeSpan(t1, "03:00", "05:00"))
            {
                return 4;
            }
            else if (Lumos.CommonUtil.GetTimeSpan(t1, "05:00", "07:00"))
            {
                return 5;
            }
            else if (Lumos.CommonUtil.GetTimeSpan(t1, "07:00", "09:00"))
            {
                return 6;
            }
            else
            {
                return 7;
            }
        }

        public DateTime TicksToDate(long time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000).AddHours(8);

        }


        public void Execute(IJobExecutionContext context)
        {

            try
            {
                var d_Users = CurrentDb.SenvivUser.ToList();

                foreach (var d_User in d_Users)
                {
                    LogUtil.Info("UserId：" + d_User.Id);

                    string month = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");

                    var d_MonthReport = CurrentDb.SenvivHealthMonthReport.Where(m => m.HealthDate == month && m.SvUserId == d_User.Id).FirstOrDefault();

                    if (d_MonthReport == null)
                    {
                        LogUtil.Info("UserId：" + d_User.Id + ",d_MonthReport is null");

                        var d_DayReports = CurrentDb.SenvivHealthDayReport.Where(m => m.SvUserId == d_User.Id && m.IsValid == true && System.Data.Entity.DbFunctions.DiffMonths(m.HealthDate, DateTime.Now) == 1).ToList();
                        if (d_DayReports.Count > 0)
                        {
                            var t1BySccs = 0;
                            var t2BySccs = 0;
                            var t3BySccs = 0;
                            var t4BySccs = 0;
                            var t5BySccs = 0;
                            var t6BySccs = 0;
                            var t7BySccs = 0;

                            var t1ByRscs = 0;
                            var t2ByRscs = 0;
                            var t3ByRscs = 0;
                            var t4ByRscs = 0;
                            var t5ByRscs = 0;
                            var t6ByRscs = 0;
                            var t7ByRscs = 0;

                            var t1ByQxcs = 0;
                            var t2ByQxcs = 0;
                            var t3ByQxcs = 0;
                            var t4ByQxcs = 0;
                            var t5ByQxcs = 0;
                            var t6ByQxcs = 0;
                            var t7ByQxcs = 0;

                            var t1ByLccs = 0;
                            var t2ByLccs = 0;
                            var t3ByLccs = 0;
                            var t4ByLccs = 0;
                            var t5ByLccs = 0;
                            var t6ByLccs = 0;
                            var t7ByLccs = 0;

                            var t1ByHxZtcs = 0;
                            var t2ByHxZtcs = 0;
                            var t3ByHxZtcs = 0;
                            var t4ByHxZtcs = 0;
                            var t5ByHxZtcs = 0;
                            var t6ByHxZtcs = 0;
                            var t7ByHxZtcs = 0;

                            var t1ByTdcs = 0;
                            var t2ByTdcs = 0;
                            var t3ByTdcs = 0;
                            var t4ByTdcs = 0;
                            var t5ByTdcs = 0;
                            var t6ByTdcs = 0;
                            var t7ByTdcs = 0;

                            var t1ByPjXl = new List<int>();
                            var t2ByPjXl = new List<int>();
                            var t3ByPjXl = new List<int>();
                            var t4ByPjXl = new List<int>();
                            var t5ByPjXl = new List<int>();
                            var t6ByPjXl = new List<int>();
                            var t7ByPjXl = new List<int>();

                            var t1ByPjHx = new List<int>();
                            var t2ByPjHx = new List<int>();
                            var t3ByPjHx = new List<int>();
                            var t4ByPjHx = new List<int>();
                            var t5ByPjHx = new List<int>();
                            var t6ByPjHx = new List<int>();
                            var t7ByPjHx = new List<int>();

                            foreach (var dayReport in d_DayReports)
                            {
                                #region 上床时间
                                if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmScsj, "21:00", "23:00"))
                                {
                                    t1BySccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmScsj, "23:00", "01:00"))
                                {
                                    t2BySccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmScsj, "01:00", "03:00"))
                                {
                                    t3BySccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmScsj, "03:00", "05:00"))
                                {
                                    t4BySccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmScsj, "05:00", "07:00"))
                                {
                                    t5BySccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmScsj, "07:00", "09:00"))
                                {
                                    t6BySccs++;
                                }
                                else
                                {
                                    t7BySccs++;
                                }
                                #endregion

                                #region 入睡时间
                                if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmRssj, "21:00", "23:00"))
                                {
                                    t1ByRscs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmRssj, "23:00", "01:00"))
                                {
                                    t2ByRscs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmRssj, "01:00", "03:00"))
                                {
                                    t3ByRscs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmRssj, "03:00", "05:00"))
                                {
                                    t4ByRscs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmRssj, "05:00", "07:00"))
                                {
                                    t5ByRscs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmRssj, "07:00", "09:00"))
                                {
                                    t6ByRscs++;
                                }
                                else
                                {
                                    t7ByRscs++;
                                }
                                #endregion

                                #region 清醒时间

                                if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmQxsj, "21:00", "23:00"))
                                {
                                    t1ByQxcs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmQxsj, "23:00", "01:00"))
                                {
                                    t2ByQxcs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmQxsj, "01:00", "03:00"))
                                {
                                    t3ByQxcs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmQxsj, "03:00", "05:00"))
                                {
                                    t4ByQxcs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmQxsj, "05:00", "07:00"))
                                {
                                    t5ByQxcs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmQxsj, "07:00", "09:00"))
                                {
                                    t6ByQxcs++;
                                }
                                else
                                {
                                    t7ByQxcs++;
                                }

                                #endregion

                                #region 离床时间

                                if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmLcsj, "21:00", "23:00"))
                                {
                                    t1ByLccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmLcsj, "23:00", "01:00"))
                                {
                                    t2ByLccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmLcsj, "01:00", "03:00"))
                                {
                                    t3ByLccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmLcsj, "03:00", "05:00"))
                                {
                                    t4ByLccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmLcsj, "05:00", "07:00"))
                                {
                                    t5ByLccs++;
                                }
                                else if (Lumos.CommonUtil.GetTimeSpan(dayReport.SmLcsj, "07:00", "09:00"))
                                {
                                    t6ByLccs++;
                                }
                                else
                                {
                                    t7ByLccs++;
                                }

                                #endregion

                                #region 呼吸暂停
                                var hxZtcsPoint = dayReport.HxZtcsPoint.ToJsonObject<List<ReportDetailListResult.D_ReportOfBreathPause>>();
                                if (hxZtcsPoint != null)
                                {
                                    foreach (var item in hxZtcsPoint)
                                    {
                                        var t1 = TicksToDate(item.StartTime);

                                        if (Lumos.CommonUtil.GetTimeSpan(t1, "21:00", "23:00"))
                                        {
                                            t1ByHxZtcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "23:00", "01:00"))
                                        {
                                            t2ByHxZtcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "01:00", "03:00"))
                                        {
                                            t3ByHxZtcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "03:00", "05:00"))
                                        {
                                            t4ByHxZtcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "05:00", "07:00"))
                                        {
                                            t5ByHxZtcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "07:00", "09:00"))
                                        {
                                            t6ByHxZtcs++;
                                        }
                                        else
                                        {
                                            t7ByHxZtcs++;
                                        }
                                    }
                                }

                                #endregion

                                #region 体动次数
                                var smTdcsPoint = dayReport.SmTdcsPoint.ToJsonObject<List<ReportDetailListResult.D_Move>>();
                                if (smTdcsPoint != null)
                                {
                                    foreach (var item in smTdcsPoint)
                                    {
                                        var t1 = TicksToDate(item.starttime);

                                        if (Lumos.CommonUtil.GetTimeSpan(t1, "21:00", "23:00"))
                                        {
                                            t1ByTdcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "23:00", "01:00"))
                                        {
                                            t2ByTdcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "01:00", "03:00"))
                                        {
                                            t3ByTdcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "03:00", "05:00"))
                                        {
                                            t4ByTdcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "05:00", "07:00"))
                                        {
                                            t5ByTdcs++;
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "07:00", "09:00"))
                                        {
                                            t6ByTdcs++;

                                        }
                                        else
                                        {
                                            t7ByTdcs++;
                                        }
                                    }
                                }

                                #endregion

                                #region 平均心率
                                var xlPoint = dayReport.XlPoint.ToJsonObject<DateValuePoint>();

                                if (xlPoint != null)
                                {
                                    if (xlPoint.DataTime != null)
                                    {
                                        for (int i = 0; i < xlPoint.DataTime.Count; i++)
                                        {
                                            DateTime t1 = TicksToDate(xlPoint.DataTime[i] * 1000);

                                            if (Lumos.CommonUtil.GetTimeSpan(t1, "21:00", "23:00"))
                                            {
                                                t1ByPjXl.Add(xlPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "23:00", "01:00"))
                                            {
                                                t2ByPjXl.Add(xlPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "01:00", "03:00"))
                                            {
                                                t3ByPjXl.Add(xlPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "03:00", "05:00"))
                                            {
                                                t4ByPjXl.Add(xlPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "05:00", "07:00"))
                                            {
                                                t5ByPjXl.Add(xlPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "07:00", "09:00"))
                                            {
                                                t6ByPjXl.Add(xlPoint.DataValue[i]);
                                            }
                                            else
                                            {
                                                t7ByPjXl.Add(xlPoint.DataValue[i]);
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region 平均呼吸
                                var hxPoint = dayReport.HxPoint.ToJsonObject<DateValuePoint>();

                                if (hxPoint != null)
                                {
                                    if (hxPoint.DataTime != null)
                                    {
                                        for (int i = 0; i < hxPoint.DataTime.Count; i++)
                                        {
                                            DateTime t1 = TicksToDate(hxPoint.DataTime[i] * 1000);

                                            if (Lumos.CommonUtil.GetTimeSpan(t1, "21:00", "23:00"))
                                            {
                                                t1ByPjHx.Add(hxPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "23:00", "01:00"))
                                            {
                                                t2ByPjHx.Add(hxPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "01:00", "03:00"))
                                            {
                                                t3ByPjHx.Add(hxPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "03:00", "05:00"))
                                            {
                                                t4ByPjHx.Add(hxPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "05:00", "07:00"))
                                            {
                                                t5ByPjHx.Add(hxPoint.DataValue[i]);
                                            }
                                            else if (Lumos.CommonUtil.GetTimeSpan(t1, "07:00", "09:00"))
                                            {
                                                t6ByPjHx.Add(hxPoint.DataValue[i]);
                                            }
                                            else
                                            {
                                                t7ByPjHx.Add(hxPoint.DataValue[i]);
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }

                            var timeFrameStaPt = new
                            {
                                t1 = new { sccs = t1BySccs, rscs = t1ByRscs, qxcs = t1ByQxcs, lccs = t1ByLccs, hxZtcs = t1ByHxZtcs, tdcs = t1ByTdcs, pjXl = t1ByPjXl.Count == 0 ? 0 : t1ByPjXl.Average(), pjHx = t1ByPjHx.Count == 0 ? 0 : t1ByPjHx.Average() },
                                t2 = new { sccs = t2BySccs, rscs = t2ByRscs, qxcs = t2ByQxcs, lccs = t2ByLccs, hxZtcs = t2ByHxZtcs, tdcs = t2ByTdcs, pjXl = t2ByPjXl.Count == 0 ? 0 : t2ByPjXl.Average(), pjHx = t2ByPjHx.Count == 0 ? 0 : t2ByPjHx.Average() },
                                t3 = new { sccs = t3BySccs, rscs = t3ByRscs, qxcs = t3ByQxcs, lccs = t3ByLccs, hxZtcs = t3ByHxZtcs, tdcs = t3ByTdcs, pjXl = t3ByPjXl.Count == 0 ? 0 : t3ByPjXl.Average(), pjHx = t3ByPjHx.Count == 0 ? 0 : t3ByPjHx.Average() },
                                t4 = new { sccs = t4BySccs, rscs = t4ByRscs, qxcs = t4ByQxcs, lccs = t4ByLccs, hxZtcs = t4ByHxZtcs, tdcs = t4ByTdcs, pjXl = t4ByPjXl.Count == 0 ? 0 : t4ByPjXl.Average(), pjHx = t4ByPjHx.Count == 0 ? 0 : t4ByPjHx.Average() },
                                t5 = new { sccs = t5BySccs, rscs = t5ByRscs, qxcs = t5ByQxcs, lccs = t5ByLccs, hxZtcs = t5ByHxZtcs, tdcs = t5ByTdcs, pjXl = t5ByPjXl.Count == 0 ? 0 : t5ByPjXl.Average(), pjHx = t5ByPjHx.Count == 0 ? 0 : t5ByPjHx.Average() },
                                t6 = new { sccs = t6BySccs, rscs = t6ByRscs, qxcs = t6ByQxcs, lccs = t6ByLccs, hxZtcs = t6ByHxZtcs, tdcs = t6ByTdcs, pjXl = t6ByPjXl.Count == 0 ? 0 : t6ByPjXl.Average(), pjHx = t6ByPjHx.Count == 0 ? 0 : t6ByPjHx.Average() },
                                t7 = new { sccs = t7BySccs, rscs = t7ByRscs, qxcs = t7ByQxcs, lccs = t7ByLccs, hxZtcs = t7ByHxZtcs, tdcs = t7ByTdcs, pjXl = t7ByPjXl.Count == 0 ? 0 : t7ByPjXl.Average(), pjHx = t7ByPjHx.Count == 0 ? 0 : t7ByPjHx.Average() },
                            };

                            d_MonthReport = new SenvivHealthMonthReport();
                            d_MonthReport.Id = IdWorker.Build(IdType.NewGuid);
                            d_MonthReport.TimeFrameStaPt = timeFrameStaPt.ToJsonString();

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


                            d_MonthReport.TotalScorePt = d_DayReports.Select(m => m.TotalScore).ToJsonString();

                            d_MonthReport.SmSmscPt = d_DayReports.Select(m => Math.Round(m.SmSmsc / 3600m, 2)).ToJsonString();

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


                            foreach (var smTag in smTags_Count)
                            {
                                var d_smTag = new SenvivHealthMonthReportTag();
                                d_smTag.Id = IdWorker.Build(IdType.NewGuid);
                                d_smTag.SvUserId = d_User.Id;
                                d_smTag.ReportId = d_MonthReport.Id;

                                var d_tag = CurrentDb.SenvivHealthTagExplain.Where(m => m.TagName == smTag.Name).FirstOrDefault();
                                if (d_tag != null)
                                {
                                    d_smTag.TagId = d_tag.TagId;
                                }

                                d_smTag.TagName = smTag.Name;
                                d_smTag.TagCount = smTag.Count;

                                CurrentDb.SenvivHealthMonthReportTag.Add(d_smTag);
                            }

                            d_MonthReport.SmTags = smTags_Count.OrderByDescending(m => m.Count).ToJsonString();

                            d_MonthReport.IsSend = false;
                            d_MonthReport.VisitCount = 0;
                            d_MonthReport.Status = E_SenvivHealthReportStatus.WaitSend;
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
