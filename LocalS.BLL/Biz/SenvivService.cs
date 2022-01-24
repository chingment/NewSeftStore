using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using SenvivSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class DateValuePoint
    {
        public List<long> DataTime { get; set; }

        public List<int> DataValue { get; set; }
    }

    public class SmPointModel
    {
        public long StartTime { get; set; }
        public long EndTime { get; set; }

        public List<DataValueModel> DataValue { get; set; }

        public class DataValueModel
        {
            public long starttime { get; set; }

            public long endtime { get; set; }

            public int type { get; set; }
        }
    }

    public class WxPaTplModel
    {
        public string OpenId { get; set; }
        public string AccessToken { get; set; }
        public string TemplateId { get; set; }
    }

    public class SmsTemplateModel
    {
        public string SignName { get; set; }

        public string TemplateCode { get; set; }
    }

    public class WxAppInfo
    {
        public string AppName { get; set; }
        public string PaQrCode { get; set; }
    }


    public class SenvivService : BaseService
    {
        public readonly string TAG = "SenvivService";

        public DateTime TicksToDate(long time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000).AddHours(8);

        }

        public DateTime Convert2DateTime(string str)
        {
            try
            {
                var dt1 = DateTime.Parse("1970-01-01T00:00:00+08:00");

                var dt = DateTime.Parse(str);

                if (dt < dt1)
                    return dt1;

                return dt;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }



        private void BuildTask(string operater, string userId, E_SenvivTaskType taskType, Dictionary<string, object> taskParams)
        {
            string rptId = "";

            if (taskType == E_SenvivTaskType.Health_Monitor_FisrtDay)
            {
                rptId = taskParams["rpt_id"].ToString();
            }
            else
            {
                #region build
                DateTime rptStartTime = (DateTime)taskParams["start_time"];
                DateTime rptEndTime = (DateTime)taskParams["end_time"];
                string rptType = "";
                switch (taskType)
                {
                    case E_SenvivTaskType.Health_Monitor_SeventhDay:
                        rptType = "seventh_day";
                        break;
                    case E_SenvivTaskType.Health_Monitor_FourteenthDay:
                        rptType = "fourteenth_day:";
                        break;
                    case E_SenvivTaskType.Health_Monitor_PerMonth:
                        rptType = "per_month";
                        break;
                }

                var d_StageReport = CurrentDb.SenvivHealthStageReport.Where(m => m.SvUserId == userId && m.RptType == rptType && m.RptStartTime == rptStartTime && m.RptEndTime == rptEndTime).FirstOrDefault();

                if (d_StageReport != null)
                    return;

                LogUtil.Info("userId：" + userId + ",rptType:" + rptType + ",d_StageReport is null");

                var d_DayReports = CurrentDb.SenvivHealthDayReport.Where(m => m.SvUserId == userId && m.IsValid == true && m.HealthDate >= rptStartTime && m.HealthDate <= rptEndTime).ToList();
                if (d_DayReports.Count > 0)
                {
                    LogUtil.Info("userId：" + userId + ",rptType:" + rptType + ",d_DayReports:" + d_DayReports.Count);

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

                    var t1ByQd = 0;
                    var t2ByQd = 0;
                    var t3ByQd = 0;
                    var t4ByQd = 0;
                    var t5ByQd = 0;
                    var t6ByQd = 0;
                    var t7ByQd = 0;

                    var t1BySd = 0;
                    var t2BySd = 0;
                    var t3BySd = 0;
                    var t4BySd = 0;
                    var t5BySd = 0;
                    var t6BySd = 0;
                    var t7BySd = 0;

                    var t1ByRem = 0;
                    var t2ByRem = 0;
                    var t3ByRem = 0;
                    var t4ByRem = 0;
                    var t5ByRem = 0;
                    var t6ByRem = 0;
                    var t7ByRem = 0;
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

                        #region 深度，浅度，REM

                        var smPoint = dayReport.SmPoint.ToJsonObject<SmPointModel>();
                        if (smPoint != null)
                        {
                            if (smPoint.DataValue != null)
                            {
                                if (smPoint.DataValue.Count > 0)
                                {
                                    foreach (var item in smPoint.DataValue)
                                    {
                                        var t1 = TicksToDate(item.starttime * 1000);

                                        //LogUtil.Info("datatime=>:" + t1.ToUnifiedFormatDateTime() + "type=>:" + item.type);

                                        if (Lumos.CommonUtil.GetTimeSpan(t1, "21:00", "23:00"))
                                        {
                                            switch (item.type)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    t1ByQd++;
                                                    break;
                                                case 2:
                                                    break;
                                                case 3:
                                                    break;
                                                case 4:
                                                    t1ByRem++;
                                                    break;
                                                case 5:
                                                    t1BySd++;
                                                    break;
                                                case 6:
                                                    t1ByQd++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "23:00", "01:00"))
                                        {
                                            switch (item.type)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    t2ByQd++;
                                                    break;
                                                case 2:
                                                    break;
                                                case 3:
                                                    break;
                                                case 4:
                                                    t2ByRem++;
                                                    break;
                                                case 5:
                                                    t2BySd++;
                                                    break;
                                                case 6:
                                                    t2ByQd++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "01:00", "03:00"))
                                        {
                                            switch (item.type)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    t3ByQd++;
                                                    break;
                                                case 2:
                                                    break;
                                                case 3:
                                                    break;
                                                case 4:
                                                    t3ByRem++;
                                                    break;
                                                case 5:
                                                    t3BySd++;
                                                    break;
                                                case 6:
                                                    t3ByQd++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "03:00", "05:00"))
                                        {
                                            switch (item.type)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    t4ByQd++;
                                                    break;
                                                case 2:
                                                    break;
                                                case 3:
                                                    break;
                                                case 4:
                                                    t4ByRem++;
                                                    break;
                                                case 5:
                                                    t4BySd++;
                                                    break;
                                                case 6:
                                                    t4ByQd++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "05:00", "07:00"))
                                        {
                                            switch (item.type)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    t5ByQd++;
                                                    break;
                                                case 2:
                                                    break;
                                                case 3:
                                                    break;
                                                case 4:
                                                    t5ByRem++;
                                                    break;
                                                case 5:
                                                    t5BySd++;
                                                    break;
                                                case 6:
                                                    t5ByQd++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else if (Lumos.CommonUtil.GetTimeSpan(t1, "07:00", "09:00"))
                                        {

                                            switch (item.type)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    t6ByQd++;
                                                    break;
                                                case 2:
                                                    break;
                                                case 3:
                                                    break;
                                                case 4:
                                                    t6ByRem++;
                                                    break;
                                                case 5:
                                                    t6BySd++;
                                                    break;
                                                case 6:
                                                    t6ByQd++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (item.type)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    t7ByQd++;
                                                    break;
                                                case 2:
                                                    break;
                                                case 3:
                                                    break;
                                                case 4:
                                                    t7ByRem++;
                                                    break;
                                                case 5:
                                                    t7BySd++;
                                                    break;
                                                case 6:
                                                    t7ByQd++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    var timeFrameStaPt = new
                    {
                        t1 = new { sccs = t1BySccs, rscs = t1ByRscs, qxcs = t1ByQxcs, lccs = t1ByLccs, hxZtcs = t1ByHxZtcs, tdcs = t1ByTdcs, pjXl = t1ByPjXl.Count == 0 ? 0 : Math.Round(t1ByPjXl.Average(), 2), pjHx = t1ByPjHx.Count == 0 ? 0 : Math.Round(t1ByPjHx.Average(), 2), qd = t1ByQd, sd = t1BySd, rem = t1ByRem },
                        t2 = new { sccs = t2BySccs, rscs = t2ByRscs, qxcs = t2ByQxcs, lccs = t2ByLccs, hxZtcs = t2ByHxZtcs, tdcs = t2ByTdcs, pjXl = t2ByPjXl.Count == 0 ? 0 : Math.Round(t2ByPjXl.Average(), 2), pjHx = t2ByPjHx.Count == 0 ? 0 : Math.Round(t2ByPjHx.Average(), 2), qd = t2ByQd, sd = t2BySd, rem = t2ByRem },
                        t3 = new { sccs = t3BySccs, rscs = t3ByRscs, qxcs = t3ByQxcs, lccs = t3ByLccs, hxZtcs = t3ByHxZtcs, tdcs = t3ByTdcs, pjXl = t3ByPjXl.Count == 0 ? 0 : Math.Round(t3ByPjXl.Average(), 2), pjHx = t3ByPjHx.Count == 0 ? 0 : Math.Round(t3ByPjHx.Average(), 2), qd = t3ByQd, sd = t3BySd, rem = t3ByRem },
                        t4 = new { sccs = t4BySccs, rscs = t4ByRscs, qxcs = t4ByQxcs, lccs = t4ByLccs, hxZtcs = t4ByHxZtcs, tdcs = t4ByTdcs, pjXl = t4ByPjXl.Count == 0 ? 0 : Math.Round(t4ByPjXl.Average(), 2), pjHx = t4ByPjHx.Count == 0 ? 0 : Math.Round(t4ByPjHx.Average(), 2), qd = t4ByQd, sd = t4BySd, rem = t4ByRem },
                        t5 = new { sccs = t5BySccs, rscs = t5ByRscs, qxcs = t5ByQxcs, lccs = t5ByLccs, hxZtcs = t5ByHxZtcs, tdcs = t5ByTdcs, pjXl = t5ByPjXl.Count == 0 ? 0 : Math.Round(t5ByPjXl.Average(), 2), pjHx = t5ByPjHx.Count == 0 ? 0 : Math.Round(t5ByPjHx.Average(), 2), qd = t5ByQd, sd = t5BySd, rem = t5ByRem },
                        t6 = new { sccs = t6BySccs, rscs = t6ByRscs, qxcs = t6ByQxcs, lccs = t6ByLccs, hxZtcs = t6ByHxZtcs, tdcs = t6ByTdcs, pjXl = t6ByPjXl.Count == 0 ? 0 : Math.Round(t6ByPjXl.Average(), 2), pjHx = t6ByPjHx.Count == 0 ? 0 : Math.Round(t6ByPjHx.Average(), 2), qd = t6ByQd, sd = t6BySd, rem = t6ByRem },
                        t7 = new { sccs = t7BySccs, rscs = t7ByRscs, qxcs = t7ByQxcs, lccs = t7ByLccs, hxZtcs = t7ByHxZtcs, tdcs = t7ByTdcs, pjXl = t7ByPjXl.Count == 0 ? 0 : Math.Round(t7ByPjXl.Average(), 2), pjHx = t7ByPjHx.Count == 0 ? 0 : Math.Round(t7ByPjHx.Average(), 2), qd = t7ByQd, sd = t7BySd, rem = t7ByRem },
                    };

                    d_StageReport = new SenvivHealthStageReport();
                    d_StageReport.Id = IdWorker.Build(IdType.NewGuid);
                    d_StageReport.TimeFrameStaPt = timeFrameStaPt.ToJsonString();
                    d_StageReport.RptStartTime = rptStartTime;
                    d_StageReport.RptEndTime = rptEndTime;
                    d_StageReport.RptType = rptType;
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


                    d_StageReport.SvUserId = userId;
                    d_StageReport.DayCount = d_DayReports.Count;
                    d_StageReport.TotalScore = d_DayReports.Select(m => m.TotalScore).Average();

                    d_StageReport.MylGrfx = Decimal.Parse(d_DayReports.Select(m => m.MylGrfx).Average().ToString());//
                    d_StageReport.MylMylzs = Decimal.Parse(d_DayReports.Select(m => m.MylMylzs).Average().ToString());//
                    d_StageReport.MylMylzs = Decimal.Parse(d_DayReports.Select(m => m.MylMylzs).Average().ToString());//
                    d_StageReport.MbGxbgk = Decimal.Parse(d_DayReports.Select(m => m.MbGxbgk).Average().ToString());//
                    d_StageReport.MbGxygk = Decimal.Parse(d_DayReports.Select(m => m.MbGxygk).Average().ToString());//
                    d_StageReport.MbTlbgk = Decimal.Parse(d_DayReports.Select(m => m.MbTlbgk).Average().ToString());//
                    d_StageReport.SmSmsc = Decimal.Parse(d_DayReports.Select(m => m.SmSmsc).Average().ToString());//
                    d_StageReport.SmQdsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmQdsmsc).Average().ToString());//
                    d_StageReport.SmQdsmbl = Decimal.Parse(d_DayReports.Select(m => m.SmQdsmbl).Average().ToString());//
                    d_StageReport.SmSdsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmSdsmsc).Average().ToString());//
                    d_StageReport.SmSdsmbl = Decimal.Parse(d_DayReports.Select(m => m.SmSdsmbl).Average().ToString());//
                    d_StageReport.SmRemsmsc = Decimal.Parse(d_DayReports.Select(m => m.SmRemsmsc).Average().ToString());//
                    d_StageReport.SmRemsmbl = Decimal.Parse(d_DayReports.Select(m => m.SmRemsmbl).Average().ToString());//
                    d_StageReport.SmTdcs = Decimal.Parse(d_DayReports.Select(m => m.SmTdcs).Average().ToString());//
                    d_StageReport.SmSmzq = Decimal.Parse(d_DayReports.Select(m => m.SmSmzq).Average().ToString());//

                    d_StageReport.QxxlQxyj = Decimal.Parse(d_DayReports.Select(m => m.QxxlQxyj).Average().ToString());//
                    d_StageReport.QxxlKynl = Decimal.Parse(d_DayReports.Select(m => m.QxxlKynl).Average().ToString());//
                    d_StageReport.HrvXzznl = Decimal.Parse(d_DayReports.Select(m => m.HrvXzznl).Average().ToString());//
                    d_StageReport.HxDcpjhx = Decimal.Parse(d_DayReports.Select(m => m.HxDcpjhx).Average().ToString());//
                    d_StageReport.HxDcjzhx = Decimal.Parse(d_DayReports.Select(m => m.HxDcjzhx).Average().ToString());//
                    d_StageReport.HxCqjzhx = Decimal.Parse(d_DayReports.Select(m => m.HxCqjzhx).Average().ToString());//
                    d_StageReport.XlDcpjxl = Decimal.Parse(d_DayReports.Select(m => m.XlDcpjxl).Average().ToString());//
                    d_StageReport.XlDcjzxl = Decimal.Parse(d_DayReports.Select(m => m.XlDcjzxl).Average().ToString());//

                    d_StageReport.XlCqjzxl = Decimal.Parse(d_DayReports.Select(m => m.XlCqjzxl).Average().ToString());//

                    d_StageReport.HxZtahizs = Decimal.Parse(d_DayReports.Select(m => m.HxZtahizs).Average().ToString());//
                    d_StageReport.HxZtcs = Decimal.Parse(d_DayReports.Select(m => m.HxZtcs).Average().ToString());//


                    d_StageReport.HrvJgsjzlzs = Decimal.Parse(d_DayReports.Select(m => m.HrvJgsjzlzs).Average().ToString());//
                    d_StageReport.HrvMzsjzlzs = Decimal.Parse(d_DayReports.Select(m => m.HrvMzsjzlzs).Average().ToString());//
                    d_StageReport.HrvZzsjzlzs = Decimal.Parse(d_DayReports.Select(m => m.HrvZzsjzlzs).Average().ToString());//
                    d_StageReport.HrvHermzs = Decimal.Parse(d_DayReports.Select(m => m.HrvHermzs).Average().ToString());//
                    d_StageReport.HrvTwjxgsszs = Decimal.Parse(d_DayReports.Select(m => m.HrvTwjxgsszs).Average().ToString());//

                    d_StageReport.JbfxXljsl = Decimal.Parse(d_DayReports.Select(m => m.JbfxXljsl).Average().ToString());//
                    d_StageReport.JbfxXlscfx = Decimal.Parse(d_DayReports.Select(m => m.JbfxXlscfx).Average().ToString());//

                    d_StageReport.DatePt = d_DayReports.Select(m => m.HealthDate.ToUnifiedFormatDate()).ToJsonString();//


                    d_StageReport.TotalScorePt = d_DayReports.Select(m => m.TotalScore).ToJsonString();

                    d_StageReport.SmSmscPt = d_DayReports.Select(m => Math.Round(m.SmSmsc / 3600m, 2)).ToJsonString();

                    // d_MonthReport.SmDtqcsPt = d_DayReports.Select(m => m.SmDtqcs).ToJsonString();
                    d_StageReport.XlDcjzxlPt = d_DayReports.Select(m => m.XlDcjzxl).ToJsonString();//
                    d_StageReport.XlCqjzxlPt = d_DayReports.Select(m => m.XlCqjzxl).ToJsonString();//
                    d_StageReport.HrvXzznlPt = d_DayReports.Select(m => m.HrvXzznl).ToJsonString();//
                    d_StageReport.HxZtcsPt = d_DayReports.Select(m => m.HxZtcs).ToJsonString();//
                    d_StageReport.HxDcjzhxPt = d_DayReports.Select(m => m.HxDcjzhx).ToJsonString();//
                    d_StageReport.HxCqjzhxPt = d_DayReports.Select(m => m.HxCqjzhx).ToJsonString();//
                    d_StageReport.HxZtahizsPt = d_DayReports.Select(m => m.HxZtahizs).ToJsonString();//
                    d_StageReport.HrvJgsjzlzsPt = d_DayReports.Select(m => m.HrvJgsjzlzs).ToJsonString();//
                    d_StageReport.HrvMzsjzlzsPt = d_DayReports.Select(m => m.HrvMzsjzlzs).ToJsonString();//
                    d_StageReport.HrvZzsjzlzsPt = d_DayReports.Select(m => m.HrvZzsjzlzs).ToJsonString();//
                    d_StageReport.HrvHermzsPt = d_DayReports.Select(m => m.HrvHermzs).ToJsonString();//
                    d_StageReport.HrvTwjxgsszsPt = d_DayReports.Select(m => m.HrvTwjxgsszs).ToJsonString();//
                    d_StageReport.JbfxXlscfxPt = d_DayReports.Select(m => m.JbfxXlscfx).ToJsonString();//
                    d_StageReport.JbfxXljslPt = d_DayReports.Select(m => m.JbfxXljsl).ToJsonString();//

                    var smTags_Count = smTags.GroupBy(s => s).Select(group => new { Name = group.Key, Count = group.Count() });


                    foreach (var smTag in smTags_Count)
                    {
                        var d_smTag = new SenvivHealthStageReportTag();
                        d_smTag.Id = IdWorker.Build(IdType.NewGuid);
                        d_smTag.SvUserId = userId;
                        d_smTag.ReportId = d_StageReport.Id;

                        var d_tag = CurrentDb.SenvivHealthTagExplain.Where(m => m.TagName == smTag.Name).FirstOrDefault();
                        if (d_tag != null)
                        {
                            d_smTag.TagId = d_tag.TagId;
                        }

                        d_smTag.TagName = smTag.Name;
                        d_smTag.TagCount = smTag.Count;

                        CurrentDb.SenvivHealthStageReportTag.Add(d_smTag);
                    }

                    d_StageReport.SmTags = smTags_Count.OrderByDescending(m => m.Count).ToJsonString();

                    d_StageReport.IsSend = false;
                    d_StageReport.VisitCount = 0;
                    d_StageReport.Status = E_SenvivHealthReportStatus.WaitSend;
                    d_StageReport.CreateTime = DateTime.Now;
                    d_StageReport.Creator = IdWorker.Build(IdType.NewGuid);
                    CurrentDb.SenvivHealthStageReport.Add(d_StageReport);
                    CurrentDb.SaveChanges();

                    rptId = d_StageReport.Id;
                }

                #endregion
            }

            if (!string.IsNullOrEmpty(rptId))
            {
                if (taskType == E_SenvivTaskType.Health_Monitor_FisrtDay || taskType == E_SenvivTaskType.Health_Monitor_SeventhDay || taskType == E_SenvivTaskType.Health_Monitor_FourteenthDay)
                {
                    var d_Task = CurrentDb.SenvivTask.Where(m => m.SvUserId == userId && m.TaskType == taskType).FirstOrDefault();
                    if (d_Task != null)
                        return;
                }


                SenvivTask d_SenvivTask = new SenvivTask();
                d_SenvivTask.Id = IdWorker.Build(IdType.NewGuid);
                d_SenvivTask.SvUserId = userId;
                d_SenvivTask.TaskType = taskType;
                var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();
                var signName = SvUtil.GetSignName(d_User.FullName, d_User.NickName);

                DateTime rptStartTime = (DateTime)taskParams["start_time"];
                DateTime rptEndTime = (DateTime)taskParams["end_time"];

                string title = "";
                switch (taskType)
                {
                    case E_SenvivTaskType.Health_Monitor_FisrtDay:
                        title = string.Format("客户[{0}]的首份报告已生成，需进行回访", signName);
                        break;
                    case E_SenvivTaskType.Health_Monitor_SeventhDay:
                        title = string.Format("客户[{0}]的首次7天报告({1}~{2})已生成，需进行回访", signName, rptStartTime.ToUnifiedFormatDate(), rptEndTime.ToUnifiedFormatDate());
                        break;
                    case E_SenvivTaskType.Health_Monitor_FourteenthDay:
                        title = string.Format("客户[{0}]的首次14天报告({1}~{2})已生成，需进行回访", signName, rptStartTime.ToUnifiedFormatDate(), rptEndTime.ToUnifiedFormatDate());
                        break;
                    case E_SenvivTaskType.Health_Monitor_PerMonth:
                        title = string.Format("客户[{0}]的{1}月报告已生成，需进行回访", signName, rptStartTime.ToString("yyyy-MM"));
                        break;
                }
                d_SenvivTask.Title = title;
                d_SenvivTask.ReportId = rptId;
                d_SenvivTask.Status = E_SenvivTaskStatus.WaitHandle;
                d_SenvivTask.CreateTime = DateTime.Now;
                d_SenvivTask.Creator = IdWorker.Build(IdType.NewGuid);
                CurrentDb.SenvivTask.Add(d_SenvivTask);
                CurrentDb.SaveChanges();

            }
        }

        public void BuildDayReport32(string userId, string deptId)
        {
            try
            {
                var config_Senviv = GetConfig(deptId);

                LogUtil.Info(TAG, "BuildDayReport32.UserId:" + userId + ",DeptId:" + deptId);

                var d1 = SdkFactory.Senviv.GetUserHealthDayReport32(config_Senviv, userId);

                if (d1 == null)
                {
                    LogUtil.Info(TAG, "DayReport Is Null");

                    return;
                }

                var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

                if (d_User.FisrtReportTime == null)
                {
                    d_User.FisrtReportTime = DateTime.Now;
                }

                DateTime? fisrtReportTime = d_User.FisrtReportTime;
                DateTime? lastReportTime = d_User.LastReportTime;

                d_User.LastReportTime = DateTime.Now;
                d_User.LastReportId = d1.reportId;

                var d_DayReport = CurrentDb.SenvivHealthDayReport.Where(m => m.Id == d1.reportId).FirstOrDefault();

                if (d_DayReport == null)
                {
                    #region DayReport
                    d_DayReport = new SenvivHealthDayReport();
                    d_DayReport.Id = d1.reportId;
                    d_DayReport.SvUserId = userId;
                    d_DayReport.HealthDate = Convert2DateTime(d1.createtime);
                    d_DayReport.TotalScore = d1.Report.TotalScore;

                    var x2 = d1.Report;

                    if (x2 != null)
                    {
                        var indexs = x2.indexs;

                        #region indexs
                        if (indexs != null)
                        {
                            foreach (var index in indexs)
                            {
                                SenvivHealthDayReportLabel d_Label = null;
                                switch (index.type)
                                {
                                    //情绪心理-情绪应激
                                    case "emostress":
                                        d_DayReport.QxxlQxyj = index.score;

                                        d_Label = new SenvivHealthDayReportLabel();
                                        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                        d_Label.ReportId = d_DayReport.Id;
                                        d_Label.SvUserId = d_DayReport.SvUserId;
                                        d_Label.TypeCode = "QxxlQxyj";
                                        d_Label.TypeName = "情绪应激";
                                        d_Label.TypeClass = "2";
                                        d_Label.Explain = index.explain;
                                        d_Label.Suggest = index.suggest.ToJsonString();
                                        d_Label.Score = index.score;
                                        break;
                                    //情绪心理-抗压能力
                                    case "compressionability":
                                        d_DayReport.QxxlKynl = index.score;

                                        d_Label = new SenvivHealthDayReportLabel();
                                        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                        d_Label.ReportId = d_DayReport.Id;
                                        d_Label.SvUserId = d_DayReport.SvUserId;
                                        d_Label.TypeCode = "QxxlKynl";
                                        d_Label.TypeName = "抗压能力";
                                        d_Label.TypeClass = "2";
                                        d_Label.Explain = index.explain;
                                        d_Label.Suggest = index.suggest.ToJsonString();
                                        d_Label.Score = index.score;
                                        break;
                                    //免疫力-免疫力指数
                                    case "Immunity":
                                        d_DayReport.MylMylzs = index.score;

                                        d_Label = new SenvivHealthDayReportLabel();
                                        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                        d_Label.ReportId = d_DayReport.Id;
                                        d_Label.SvUserId = d_DayReport.SvUserId;
                                        d_Label.TypeCode = "MylMylZs";
                                        d_Label.TypeName = "免疫力指数";
                                        d_Label.TypeClass = "2";
                                        d_Label.Explain = index.explain;
                                        d_Label.Suggest = index.suggest.ToJsonString();
                                        d_Label.Score = index.score;
                                        break;
                                    //免疫力-感染风险
                                    case "感染风险":

                                        d_DayReport.MylGrfx = index.score;

                                        d_Label = new SenvivHealthDayReportLabel();
                                        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                        d_Label.ReportId = d_DayReport.Id;
                                        d_Label.SvUserId = d_DayReport.SvUserId;
                                        d_Label.TypeCode = "MylGrfx";
                                        d_Label.TypeName = "感染风险";
                                        d_Label.TypeClass = "2";
                                        d_Label.Explain = index.explain;
                                        d_Label.Suggest = index.suggest.ToJsonString();
                                        d_Label.Score = index.score;

                                        break;
                                    //慢病管理-高血压管控
                                    case "高血压管控":
                                        d_DayReport.MbGxygk = index.score;

                                        d_Label = new SenvivHealthDayReportLabel();
                                        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                        d_Label.ReportId = d_DayReport.Id;
                                        d_Label.SvUserId = d_DayReport.SvUserId;
                                        d_Label.TypeCode = "MbGxbgk";
                                        d_Label.TypeName = "高血压管控";
                                        d_Label.TypeClass = "2";
                                        d_Label.Explain = index.explain;
                                        d_Label.Suggest = index.suggest.ToJsonString();
                                        d_Label.Score = index.score;
                                        break;
                                    //慢病管理-糖尿病管控
                                    case "糖尿病管控":
                                        d_DayReport.MbTlbgk = index.score;

                                        d_Label = new SenvivHealthDayReportLabel();
                                        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                        d_Label.ReportId = d_DayReport.Id;
                                        d_Label.SvUserId = d_DayReport.SvUserId;
                                        d_Label.TypeCode = "MbTlbgk";
                                        d_Label.TypeName = "糖尿病管控";
                                        d_Label.TypeClass = "2";
                                        d_Label.Explain = index.explain;
                                        d_Label.Suggest = index.suggest.ToJsonString();
                                        d_Label.Score = index.score;

                                        break;
                                    //情绪心理-焦虑情绪
                                    case "Anxiety":

                                        d_DayReport.QxxlJlqx = index.explain;

                                        d_Label = new SenvivHealthDayReportLabel();
                                        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                        d_Label.ReportId = d_DayReport.Id;
                                        d_Label.SvUserId = d_DayReport.SvUserId;
                                        d_Label.TypeCode = "QxxlJlqx";
                                        d_Label.TypeName = "焦虑情绪";
                                        d_Label.TypeClass = "2";
                                        d_Label.Explain = index.explain;
                                        d_Label.Suggest = index.suggest.ToJsonString();
                                        d_Label.Score = index.score;

                                        break;
                                    default:
                                        d_Label = null;
                                        break;
                                }

                                if (d_Label != null)
                                {
                                    CurrentDb.SenvivHealthDayReportLabel.Add(d_Label);
                                    CurrentDb.SaveChanges();
                                }
                            }

                        }
                        #endregion

                        var labels = x2.labels;

                        #region
                        if (labels != null)
                        {
                            List<string> smTags = new List<string>();

                            foreach (var label in labels)
                            {
                                //SenvivHealthDayReportLabel d_Label = null;

                                //switch (label.TagName)
                                //{
                                //    case "消化力差":
                                //        d_Label = new SenvivHealthDayReportLabel();
                                //        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                //        d_Label.DayReportId = d_DayReport.Id;
                                //        d_Label.SvUserId = d_DayReport.SvUserId;
                                //        d_Label.TypeCode = "Xhnl";
                                //        d_Label.TypeName = "消化力差";
                                //        d_Label.Explain = label.Explain;
                                //        d_Label.Suggest = label.suggest.ToJsonString();
                                //        d_Label.Level = label.level;

                                //        break;
                                //    case "睡眠不安":

                                //        d_Label = new SenvivHealthDayReportLabel();
                                //        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                //        d_Label.DayReportId = d_DayReport.Id;
                                //        d_Label.SvUserId = d_DayReport.SvUserId;
                                //        d_Label.TypeCode = "Smba";
                                //        d_Label.TypeName = "睡眠不安";
                                //        d_Label.Explain = label.Explain;
                                //        d_Label.Suggest = label.suggest.ToJsonString();
                                //        d_Label.Level = label.level;

                                //        break;
                                //    case "易醒":

                                //        d_Label = new SenvivHealthDayReportLabel();
                                //        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                //        d_Label.DayReportId = d_DayReport.Id;
                                //        d_Label.SvUserId = d_DayReport.SvUserId;
                                //        d_Label.TypeCode = "Yx";
                                //        d_Label.TypeName = "易醒";
                                //        d_Label.Explain = label.Explain;
                                //        d_Label.Suggest = label.suggest.ToJsonString();
                                //        d_Label.Level = label.level;

                                //        break;
                                //    case "较难入睡":

                                //        d_Label = new SenvivHealthDayReportLabel();
                                //        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                //        d_Label.DayReportId = d_DayReport.Id;
                                //        d_Label.SvUserId = d_DayReport.SvUserId;
                                //        d_Label.TypeCode = "Jxrs";
                                //        d_Label.TypeName = "较难入睡";
                                //        d_Label.Explain = label.Explain;
                                //        d_Label.Suggest = label.suggest.ToJsonString();
                                //        d_Label.Level = label.level;

                                //        break;
                                //    default:
                                //        d_Label = new SenvivHealthDayReportLabel();
                                //        d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                //        d_Label.DayReportId = d_DayReport.Id;
                                //        d_Label.SvUserId = d_DayReport.SvUserId;
                                //        d_Label.TypeName = label.TagName;
                                //        d_Label.Explain = label.Explain;
                                //        d_Label.Suggest = label.suggest.ToJsonString();
                                //        d_Label.Level = label.level;
                                //        break;

                                //}


                                //if (d_Label != null)
                                //{
                                var d_Label = new SenvivHealthDayReportLabel();
                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                d_Label.ReportId = d_DayReport.Id;
                                d_Label.SvUserId = d_DayReport.SvUserId;
                                d_Label.TypeName = label.TagName;
                                d_Label.Explain = label.Explain;
                                d_Label.Suggest = label.suggest.ToJsonString();
                                d_Label.Level = label.level;
                                d_Label.TypeClass = "1";
                                CurrentDb.SenvivHealthDayReportLabel.Add(d_Label);
                                CurrentDb.SaveChanges();
                                //}
                            }

                            d_DayReport.SmTags = labels.Select(m => m.TagName).ToList().ToJsonString();

                        }

                        #endregion

                        var advices = x2.advices;

                        #region advices

                        foreach (var advice in advices)
                        {
                            var d_Advice = new SenvivHealthDayReportAdvice();
                            d_Advice.Id = IdWorker.Build(IdType.NewGuid);
                            d_Advice.ReportId = d_DayReport.Id;
                            d_Advice.SvUserId = d_DayReport.SvUserId;
                            d_Advice.SuggestCode = advice.suggestcode;
                            d_Advice.SuggestName = advice.suggestion;
                            d_Advice.SuggestDirection = advice.suggestdirection;
                            d_Advice.Summary = advice.summarystr;
                            CurrentDb.SenvivHealthDayReportAdvice.Add(d_Advice);
                            CurrentDb.SaveChanges();
                        }

                        #endregion
                    }

                    var xl = d1.ReportOfHeartBeat;


                    #region ReportOfHeartBeat
                    if (xl != null)
                    {
                        d_DayReport.XlDcjzxl = xl.DayCurBenchmark;//当次基准心率
                        d_DayReport.XlCqjzxl = xl.DayLongterm;//长期基准心率
                        d_DayReport.XlDcpjxl = xl.Average;//当次平均心率
                        d_DayReport.XlZg = xl.HeartbeatMax;//最高心率
                        d_DayReport.XlZd = xl.HeartbeatMin;//最低心率
                        d_DayReport.XlXdgksc = xl.Higher;//心动过快时长
                        d_DayReport.XlXdgmsc = xl.Lower;//心动过慢时长
                        d_DayReport.Xlcg125 = 0;//todo 
                        d_DayReport.Xlcg115 = 0;//todo 
                        d_DayReport.Xlcg085 = 0;//todo 
                        d_DayReport.Xlcg075 = 0;//todo 
                    }
                    #endregion

                    var hx = d1.ReportOfBreath;

                    #region ReportOfBreath
                    if (hx != null)
                    {
                        d_DayReport.HxDcpjhx = hx.Average;//	平均呼吸
                        d_DayReport.HxDcjzhx = hx.Benchmark;//基准呼吸值
                        d_DayReport.HxZdhx = hx.BreathMin;//当夜最低呼吸率
                        d_DayReport.HxZghx = hx.BreathMax;//当夜最高呼吸率
                        d_DayReport.HxCqjzhx = hx.Longterm; //长期基准呼吸
                        d_DayReport.HxGksc = 0;//todo 
                        d_DayReport.HxGmsc = 0;//todo 


                        d_DayReport.HxZtahizs = hx.AHI;//AHI指数
                        d_DayReport.HxZtcs = hx.HigherCounts;//呼吸暂停次数
                        d_DayReport.HxZtcsPoint = hx.ReportOfBreathPause.ToJsonString();
                        d_DayReport.HxZtpjsc = hx.AvgPause;//呼吸暂停平均时长
                    }
                    #endregion

                    var hrv = d1.ReportOfHRV;

                    #region ReportOfHRV
                    if (hrv != null)
                    {
                        d_DayReport.HrvXzznl = hrv.HeartIndex;//心脏总能量
                        d_DayReport.HrvXzznljzz = hrv.BaseTP;//心脏总能量基准值
                        d_DayReport.HrvJgsjzlzs = hrv.LF;//交感神经张力指数
                        d_DayReport.HrvJgsjzlzsjzz = hrv.BaseLF;// 交感神经张力基准值
                        d_DayReport.HrvMzsjzlzs = hrv.HF;//迷走神经张力指数
                        d_DayReport.HrvMzsjzlzsjzz = hrv.BaseHF;//迷走神经张力基准值
                        d_DayReport.HrvZzsjzlzs = hrv.LFHF;//自主神经平衡
                        d_DayReport.HrvZzsjzlzsjzz = hrv.BaseLFHF;//自主神经平衡基准值
                        d_DayReport.HrvHermzs = hrv.endocrine;//荷尔蒙指数
                        d_DayReport.HrvHermzsjzz = 0;//荷尔蒙指数基准值
                        d_DayReport.HrvTwjxgsszs = hrv.temperature;//体温及血管舒缩指数
                        d_DayReport.HrvTwjxgsszhjzz = 0;//体温及血管舒缩基准值
                        d_DayReport.SmScore = hrv.SleepValue;
                        d_DayReport.JbfxXlscfx = hrv.SDNN;//心律失常风险指数

                        var d2 = d1.UserBaseInfo;
                        if (d2 != null)
                        {
                            d_DayReport.JbfxXljsl = d1.UserBaseInfo.DcValue;
                        }
                    }
                    #endregion

                    var sm = d1.ReportOfSleep;
                    if (sm != null)
                    {
                        d_DayReport.SmScsj = TicksToDate(x2.StartTime);//上床时间
                        d_DayReport.SmLcsj = TicksToDate(x2.FinishTime);//离床时间
                        d_DayReport.SmZcsc = (long)(d_DayReport.SmLcsj - d_DayReport.SmScsj).TotalSeconds;//起床时刻
                        d_DayReport.SmRssj = TicksToDate(x2.OnbedTime);//入睡时间
                        d_DayReport.SmQxsj = TicksToDate(x2.OffbedTime);//清醒时间
                        d_DayReport.SmSmsc = (long)(d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalSeconds;//睡眠时长
                        d_DayReport.SmRsxs = (long)(d_DayReport.SmRssj - d_DayReport.SmScsj).TotalSeconds;//入睡需时
                        d_DayReport.SmLzsc = (long)(d_DayReport.SmLcsj - d_DayReport.SmQxsj).TotalSeconds;//离枕时长
                        d_DayReport.SmLzscbl = sm.OffbedRatio;
                        d_DayReport.SmSmzq = sm.SleepCounts;//睡眠周期

                        d_DayReport.SmSdsmsc = sm.Deep;//深睡时长
                        d_DayReport.SmSdsmbl = sm.DeepRatio;//深睡期比例

                        d_DayReport.SmQdsmsc = sm.Shallow;//浅睡期时长
                        d_DayReport.SmQdsmbl = sm.ShallowRatio;//浅睡期比例

                        d_DayReport.SmRemsmsc = sm.Rem;//REM期时长
                        d_DayReport.SmRemsmbl = sm.RemRatio;//REM期比例

                        d_DayReport.SmQxsksc = sm.Sober;//REM期时长
                        d_DayReport.SmQxskbl = sm.SoberRatio;//REM期比例

                        d_DayReport.SmLzcs = 0;

                        d_DayReport.SmTdcs = sm.MoveCounts;//体动次数
                        d_DayReport.SmTdcsPoint = sm.Moves.ToJsonString();
                        d_DayReport.SmPjtdsc = sm.MovingAverageLength;//平均体动时长

                    }
                    var rc = d1.ReportCharts;
                    if (rc != null)
                    {
                        var trendCharts = rc.ReportTrendChart;

                        if (trendCharts != null)
                        {
                            foreach (var chart in trendCharts)
                            {
                                if (chart.type == 2107)
                                {
                                    d_DayReport.HxPoint = (new { DataTime = chart.XDataTime, DataValue = chart.XDataValue }).ToJsonString();
                                }
                                else if (chart.type == 2106)
                                {
                                    d_DayReport.XlPoint = (new { DataTime = chart.XDataTime, DataValue = chart.XDataValue }).ToJsonString();
                                }
                            }
                        }

                        var barCharts = rc.ReportBarChart;
                        if (barCharts != null)
                        {
                            foreach (var chart in barCharts)
                            {
                                var items = chart.Items;
                                if (items != null)
                                {
                                    if (chart.ChartTypeId == 2110)
                                    {
                                        foreach (var item in items)
                                        {
                                            d_DayReport.SmPoint = (new { StartTime = item.starttime, EndTime = item.endtime, DataValue = item.subitems }).ToJsonString();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    d_DayReport.IsSend = true;
                    d_DayReport.Status = E_SenvivHealthReportStatus.SendSuccess;
                    d_DayReport.CreateTime = DateTime.Now;
                    d_DayReport.Creator = IdWorker.Build(IdType.EmptyGuid);

                    if ((d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalHours >= 4)
                    {
                        d_DayReport.IsValid = true;
                    }

                    CurrentDb.SenvivHealthDayReport.Add(d_DayReport);
                    CurrentDb.SaveChanges();

                    #endregion
                }

                if (fisrtReportTime != null)
                {
                    Dictionary<string, object> taskParams = new Dictionary<string, object>();
                    DateTime? rptStartTime = null;
                    DateTime? rptEndTime = null;
                    if ((DateTime.Now - fisrtReportTime).Value.Days >= 1)
                    {
                        LogUtil.Info(TAG, "Health_Monitor_FisrtDay");

                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                        taskParams.Add("rpt_id", d_DayReport.Id);
                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);
                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_FisrtDay, taskParams);
                    }

                    if ((DateTime.Now - fisrtReportTime).Value.Days >= 7)
                    {
                        LogUtil.Info(TAG, "Health_Monitor_SeventhDay");

                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(fisrtReportTime.ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);

                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_SeventhDay, taskParams);
                    }


                    if ((DateTime.Now - fisrtReportTime).Value.Days >= 14)
                    {
                        LogUtil.Info(TAG, "Health_Monitor_FourteenthDay");

                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(fisrtReportTime.ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);

                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_FourteenthDay, taskParams);
                    }

                    LogUtil.Info(TAG, "Health_Monitor_PerMonth:" + lastReportTime.Value.ToUnifiedFormatDateTime());

                    DateTime dt1 = lastReportTime.Value;
                    DateTime dt2 = DateTime.Now;
                    int month = (dt2.Year - dt1.Year) * 12 + (dt2.Month - dt1.Month);
                    if (month >= 1)
                    {
                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(new DateTime(dt1.Year, dt1.Month, 1).ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime((rptStartTime.Value.AddMonths(1).AddDays(-1)).ToUnifiedFormatDateTime()).Value;

                        taskParams = new Dictionary<string, object>();
                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);

                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_PerMonth, taskParams);
                    }

                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }

        public void BuildDayReport46(string userId, string sn, string deptId)
        {
            try
            {
                var config_Senviv = GetConfig(deptId);

                LogUtil.Info(TAG, "BuildDayReport46.sn" + sn + ",UserId:" + userId + ",DeptId:" + deptId);

                var d1 = SdkFactory.Senviv.GetUserHealthDayReport64(config_Senviv, sn);

                if (d1 == null)
                {
                    LogUtil.Info(TAG, "DayReport Is Null");

                    return;
                }

                var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

                if (d_User.FisrtReportTime == null)
                {
                    d_User.FisrtReportTime = DateTime.Now;
                }

                var reportpar = d1.reportpar;

                DateTime? fisrtReportTime = d_User.FisrtReportTime;
                DateTime? lastReportTime = d_User.LastReportTime;

                d_User.LastReportTime = DateTime.Now;
                d_User.LastReportId = reportpar.ReportId;

                var d_DayReport = CurrentDb.SenvivHealthDayReport.Where(m => m.Id == reportpar.ReportId).FirstOrDefault();

                if (d_DayReport == null)
                {
                    #region DayReport
                    d_DayReport = new SenvivHealthDayReport();
                    d_DayReport.Id = reportpar.ReportId;
                    d_DayReport.SvUserId = userId;
                    d_DayReport.HealthDate = TicksToDate(reportpar.CreateTime);
                    d_DayReport.TotalScore = SvUtil.D46Decimal(reportpar.hv);//健康值

                    d_DayReport.SmTags = reportpar.AbnormalLabel.ToJsonString();
                    d_DayReport.QxxlQxyj = SvUtil.D46Int(reportpar.emotion);
                    d_DayReport.QxxlKynl = SvUtil.D46Decimal(reportpar.press);
                    d_DayReport.MylMylzs = SvUtil.D46Decimal(reportpar.im);
                    d_DayReport.MylGrfx = 100 - SvUtil.D46Decimal(reportpar.gr);
                    d_DayReport.MbGxygk = SvUtil.D46Decimal(reportpar.hc);
                    d_DayReport.MbTlbgk = 100 - SvUtil.D46Decimal(reportpar.tc);
                    d_DayReport.MbGxbgk = SvUtil.D46Decimal(reportpar.mc);

                    d_DayReport.QxxlJlqx = reportpar.qxxl;


                    d_DayReport.XlDcjzxl = SvUtil.D46Int(reportpar.hr);//当次基准心率
                    d_DayReport.XlCqjzxl = SvUtil.D46Int(reportpar.lhr);//长期基准心率
                    d_DayReport.XlDcpjxl = SvUtil.D46Int(reportpar.avg);//当次平均心率
                    d_DayReport.XlZg = SvUtil.D46Int(reportpar.max);//最高心率
                    d_DayReport.XlZd = SvUtil.D46Int(reportpar.min);//最低心率
                    d_DayReport.XlXdgksc = SvUtil.D46Int(reportpar.hrfast);//心动过快时长
                    d_DayReport.XlXdgmsc = SvUtil.D46Int(reportpar.hrslow);//心动过慢时长
                    d_DayReport.Xlcg125 = 0;//todo 
                    d_DayReport.Xlcg115 = 0;//todo 
                    d_DayReport.Xlcg085 = 0;//todo 
                    d_DayReport.Xlcg075 = 0;//todo


                    d_DayReport.HxDcpjhx = SvUtil.D46Int(reportpar.bavg);//	平均呼吸
                    d_DayReport.HxDcjzhx = SvUtil.D46Int(reportpar.br);//基准呼吸值
                    d_DayReport.HxZdhx = SvUtil.D46Int(reportpar.bmin);//当夜最低呼吸率
                    d_DayReport.HxZghx = SvUtil.D46Int(reportpar.bmax);//当夜最高呼吸率
                    d_DayReport.HxCqjzhx = SvUtil.D46Int(reportpar.lbr); //长期基准呼吸
                    d_DayReport.HxGksc = 0;//todo 
                    d_DayReport.HxGmsc = 0;//todo 


                    d_DayReport.HxZtahizs = SvUtil.D46Decimal(reportpar.AHI);//AHI指数
                    d_DayReport.HxZtcs = SvUtil.D46Int(reportpar.brz);//呼吸暂停次数
                                                                      //d_DayReport.HxZtcsPoint = hx.ReportOfBreathPause.ToJsonString();
                                                                      //d_DayReport.HxZtpjsc = hx.AvgPause;//呼吸暂停平均时长


                    d_DayReport.HrvXzznl = SvUtil.D46Int(reportpar.TP);//心脏总能量
                    d_DayReport.HrvXzznljzz = SvUtil.D46Int(reportpar.BaseTP);//心脏总能量基准值
                    d_DayReport.HrvJgsjzlzs = SvUtil.D46Int(reportpar.LF);//交感神经张力指数
                    d_DayReport.HrvJgsjzlzsjzz = SvUtil.D46Int(reportpar.BaseLF);// 交感神经张力基准值
                    d_DayReport.HrvMzsjzlzs = SvUtil.D46Int(reportpar.HF);//迷走神经张力指数
                    d_DayReport.HrvMzsjzlzsjzz = SvUtil.D46Int(reportpar.BaseHF);//迷走神经张力基准值
                    d_DayReport.HrvZzsjzlzs = SvUtil.D46Decimal(reportpar.LFHF);//自主神经平衡
                    d_DayReport.HrvZzsjzlzsjzz = SvUtil.D46Decimal(reportpar.BaseLFHF);//自主神经平衡基准值
                    d_DayReport.HrvHermzs = SvUtil.D46Decimal(reportpar.ulf);//荷尔蒙指数
                    d_DayReport.HrvHermzsjzz = SvUtil.D46Decimal(reportpar.Baseulf); //荷尔蒙指数基准值
                    d_DayReport.HrvTwjxgsszs = SvUtil.D46Decimal(reportpar.vlf);//体温及血管舒缩指数
                    d_DayReport.HrvTwjxgsszhjzz = SvUtil.D46Decimal(reportpar.Basevlf);//体温及血管舒缩基准值

                    d_DayReport.JbfxXlscfx = SvUtil.D46Int(reportpar.sdnn);//心律失常风险指数

                    d_DayReport.JbfxXljsl = SvUtil.D46Decimal(reportpar.dc);

                    d_DayReport.SmScore = SvUtil.D46Decimal(reportpar.sleepValue);//睡眠分数
                    d_DayReport.SmScsj = TicksToDate(reportpar.StartTime);//上床时间
                    d_DayReport.SmLcsj = TicksToDate(reportpar.FinishTime);//离床时间
                    d_DayReport.SmZcsc = (long)(d_DayReport.SmLcsj - d_DayReport.SmScsj).TotalSeconds;//起床时刻
                    d_DayReport.SmRssj = TicksToDate(reportpar.OnbedTime);//入睡时间
                    d_DayReport.SmQxsj = TicksToDate(reportpar.OffbedTime);//清醒时间
                    d_DayReport.SmSmsc = (long)(d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalSeconds;//睡眠时长
                    d_DayReport.SmRsxs = (long)(d_DayReport.SmRssj - d_DayReport.SmScsj).TotalSeconds;//入睡需时
                    d_DayReport.SmLzsc = (long)(d_DayReport.SmLcsj - d_DayReport.SmQxsj).TotalSeconds;//离枕时长
                                                                                                      //d_DayReport.SmLzscbl = sm.OffbedRatio;
                    d_DayReport.SmSmzq = SvUtil.D46Int(reportpar.sct);//睡眠周期

                    d_DayReport.SmSdsmsc = SvUtil.D46Long(reportpar.dp);//深睡时长
                    d_DayReport.SmSdsmbl = SvUtil.D46Decimal(reportpar.dpr);//深睡期比例

                    d_DayReport.SmQdsmsc = SvUtil.D46Long(reportpar.sl);//浅睡期时长
                    d_DayReport.SmQdsmbl = SvUtil.D46Decimal(reportpar.slr);//浅睡期比例

                    d_DayReport.SmRemsmsc = SvUtil.D46Long(reportpar.rem);//REM期时长
                    d_DayReport.SmRemsmbl = SvUtil.D46Decimal(reportpar.remr);//REM期比例

                    d_DayReport.SmQxsksc = SvUtil.D46Long(reportpar.sr);//REM期时长
                    d_DayReport.SmQxskbl = SvUtil.D46Decimal(reportpar.srr);//REM期比例

                    d_DayReport.SmLzcs = SvUtil.D46Int(reportpar.ofbdc);

                    d_DayReport.SmTdcs = SvUtil.D46Int(reportpar.mct);//体动次数
                    //d_DayReport.SmTdcsPoint = sm.Moves.ToJsonString();
                    d_DayReport.SmPjtdsc = SvUtil.D46Int(reportpar.mvavg);//平均体动时长


                    var trendcharts = d1.trendchart;
                    if (trendcharts != null)
                    {

                        foreach (var chart in trendcharts)
                        {
                            if (chart.type == 2107)
                            {
                                d_DayReport.HxPoint = (new { DataTime = chart.xdatatime, DataValue = chart.xdatavalue }).ToJsonString();
                            }
                            else if (chart.type == 2106)
                            {
                                d_DayReport.XlPoint = (new { DataTime = chart.xdatatime, DataValue = chart.xdatavalue }).ToJsonString();
                            }
                        }
                    }

                    var barchart = d1.barchart;
                    if (barchart != null)
                    {
                        if (barchart.type == 2110)
                        {
                            var items = barchart.items;
                            if (items != null && items.Count > 0)
                            {
                                var sub = items[0];
                                var item2s = sub.sub;
                                var dataValues = new List<object>();
                                foreach (var item in item2s)
                                {
                                    dataValues.Add(new { endtime = SvUtil.D46Long(item.et), starttime = SvUtil.D46Long(item.st), type = SvUtil.D46Int(item.type) });
                                }
                                d_DayReport.SmPoint = (new { StartTime = SvUtil.D46Long(sub.st), EndTime = SvUtil.D46Long(sub.et), DataValue = dataValues }).ToJsonString();
                            }
                        }
                    }

                    d_DayReport.IsSend = true;
                    d_DayReport.Status = E_SenvivHealthReportStatus.SendSuccess;
                    d_DayReport.CreateTime = DateTime.Now;
                    d_DayReport.Creator = IdWorker.Build(IdType.EmptyGuid);

                    if ((d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalHours >= 4)
                    {
                        d_DayReport.IsValid = true;
                    }

                    CurrentDb.SenvivHealthDayReport.Add(d_DayReport);
                    CurrentDb.SaveChanges();

                    #endregion
                }

                if (fisrtReportTime != null)
                {
                    Dictionary<string, object> taskParams = new Dictionary<string, object>();
                    DateTime? rptStartTime = null;
                    DateTime? rptEndTime = null;
                    if ((DateTime.Now - fisrtReportTime).Value.Days >= 1)
                    {
                        LogUtil.Info(TAG, "Health_Monitor_FisrtDay");

                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                        taskParams.Add("rpt_id", d_DayReport.Id);
                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);
                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_FisrtDay, taskParams);
                    }

                    if ((DateTime.Now - fisrtReportTime).Value.Days >= 7)
                    {
                        LogUtil.Info(TAG, "Health_Monitor_SeventhDay");

                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(fisrtReportTime.ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);

                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_SeventhDay, taskParams);
                    }


                    if ((DateTime.Now - fisrtReportTime).Value.Days >= 14)
                    {
                        LogUtil.Info(TAG, "Health_Monitor_FourteenthDay");

                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(fisrtReportTime.ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);

                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_FourteenthDay, taskParams);
                    }

                    LogUtil.Info(TAG, "Health_Monitor_PerMonth:" + lastReportTime.Value.ToUnifiedFormatDateTime());

                    DateTime dt1 = lastReportTime.Value;
                    DateTime dt2 = DateTime.Now;
                    int month = (dt2.Year - dt1.Year) * 12 + (dt2.Month - dt1.Month);
                    if (month >= 1)
                    {
                        rptStartTime = Lumos.CommonUtil.ConverToStartTime(new DateTime(dt1.Year, dt1.Month, 1).ToUnifiedFormatDateTime()).Value;
                        rptEndTime = Lumos.CommonUtil.ConverToEndTime((rptStartTime.Value.AddMonths(1).AddDays(-1)).ToUnifiedFormatDateTime()).Value;

                        taskParams = new Dictionary<string, object>();
                        taskParams.Add("start_time", rptStartTime);
                        taskParams.Add("end_time", rptEndTime);

                        BuildTask(IdWorker.Build(IdType.EmptyGuid), userId, E_SenvivTaskType.Health_Monitor_PerMonth, taskParams);
                    }

                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }

        //优先级别MerchId,若Merch为空再取deviceId
        public WxAppConfig GetWxAppInfoConfigByMerchIdOrDeviceId(string merchId, string deviceId)
        {
            LogUtil.Info("merchId:" + merchId + ",deviceId:" + deviceId);

            if (string.IsNullOrEmpty(merchId) && string.IsNullOrEmpty(deviceId))
                return null;

            if (string.IsNullOrEmpty(merchId))
            {
                return GetWxAppConfigByDeviceId(deviceId);
            }
            else
            {
                return GetWxAppConfigByMerchId(merchId);
            }
        }
        public WxAppInfo GetWxAppInfoByUserId(string userId)
        {
            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            if (d_ClientUser == null)
                return null;

            var d_SenvivMerch = CurrentDb.SenvivMerch.Where(m => m.MerchId == d_ClientUser.MerchId).FirstOrDefault();

            var appInfo = new WxAppInfo();
            appInfo.AppName = d_SenvivMerch.WxPaAppName;
            appInfo.PaQrCode = d_SenvivMerch.WxPaQrCode;
            return appInfo;
        }
        public WxAppConfig GetWxAppConfigByMerchId(string merchId)
        {
            var d_SenvivMerch = CurrentDb.SenvivMerch.Where(m => m.MerchId == merchId).FirstOrDefault();
            if (d_SenvivMerch == null)
            {
                return null;
            }


            var config = new WxAppConfig();
            config.AppId = d_SenvivMerch.WxPaAppId;
            config.AppSecret = d_SenvivMerch.WxPaAppSecret;

            Dictionary<string, string> exts = new Dictionary<string, string>();
            exts.Add("MerchId", merchId);
            config.Exts = exts;

            return config;
        }
        public WxAppConfig GetWxAppConfigByDeviceId(string deviceId)
        {
            LogUtil.Info("GetWxAppConfigByDeviceId:" + deviceId);
            var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.DeviceId == deviceId && m.IsStopUse == false).FirstOrDefault();

            if (d_MerchDevice == null)
                return null;

            return GetWxAppConfigByMerchId(d_MerchDevice.MerchId);
        }
        public WxAppConfig GetWxAppConfigByUserId(string userId)
        {
            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            if (d_ClientUser == null)
                return null;

            var d_SenvivMerch = CurrentDb.SenvivMerch.Where(m => m.MerchId == d_ClientUser.MerchId).FirstOrDefault();

            if (d_SenvivMerch == null)
                return null;

            var appConfig = new WxAppConfig();
            appConfig.AppId = d_SenvivMerch.WxPaAppId;
            appConfig.AppSecret = d_SenvivMerch.WxPaAppSecret;


            Dictionary<string, string> exts = new Dictionary<string, string>();
            exts.Add("MerchId", d_SenvivMerch.MerchId);
            exts.Add("WxPaOpenId", d_ClientUser.WxPaOpenId);
            appConfig.Exts = exts;

            return appConfig;

            //WxAppConfig config = new WxAppConfig();
            //config.AppId = "wxc6e80f8c575cf3f5";
            //config.AppSecret = "fee895c9923da26a4d42d9c435202b37";

            //return config;
        }

        public SenvivConfig GetConfig(string deptId)
        {
            var config = new SenvivConfig();
            if (deptId == "32")
            {
                config.AccessToken = SdkFactory.Senviv.GetApiAccessToken("qxtadmin", "zkxz123");
                config.SvDeptId = "32";
            }
            else if (deptId == "46")
            {
                config.AccessToken = SdkFactory.Senviv.GetApiAccessToken("全线通月子会所", "qxt123456");
                config.SvDeptId = "46";
            }
            return config;
        }

        public bool SendMonthReport(string userId, string first, string keyword1, string keyword2, string remark, string url)
        {
            var template = GetWxPaTpl(userId, "month_report");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"" + url + "\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public bool SendArticle(string userId, string first, string keyword1, string keyword2, string remark, string url)
        {
            var template = GetWxPaTpl(userId, "pregnancy_remind");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"" + url + "\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public bool SendHealthMonitor(string userId, string first, string keyword1, string keyword2, string keyword3, string remark)
        {
            var template = GetWxPaTpl(userId, "health_monitor");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword3\":{ \"value\":\"" + keyword3 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public bool SendDeviceBind(string userId, string first, string keyword1, string keyword2, string remark)
        {
            var template = GetWxPaTpl(userId, "device_bind");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public bool SendDeviceUnBind(string userId, string first, string keyword1, string keyword2, string remark)
        {
            var template = GetWxPaTpl(userId, "device_unbind");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public WxPaTplModel GetWxPaTpl(string userId, string template)
        {
            var model = new WxPaTplModel();

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.UserId == userId).FirstOrDefault();
            var d_SenvivMerch = CurrentDb.SenvivMerch.Where(m => m.MerchId == d_SenvivUser.MerchId).FirstOrDefault();

            model.OpenId = d_ClientUser.WxPaOpenId;
            //model.OpenId = "on0dM51JLVry0lnKT4Q8nsJBRXNs";


            if (d_SenvivUser.SvDeptId == "32")
            {
                var cofig = GetConfig("32");
                model.AccessToken = SdkFactory.Senviv.GetWxPaAccessToken(cofig);
            }
            else
            {
                WxAppConfig config = new WxAppConfig();
                config.AppId = "wxc6e80f8c575cf3f5";
                config.AppSecret = "fee895c9923da26a4d42d9c435202b37";
                model.AccessToken = SdkFactory.Wx.GetApiAccessToken(config);
            }

            switch (template)
            {
                case "month_report":
                    model.TemplateId = "GpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKY";
                    break;
                case "health_monitor":
                    model.TemplateId = "4rfsYerDDF7aVGuETQ3n-Kn84mjIHLBn0H6H8giz7Ac";
                    break;
                case "pregnancy_remind":
                    model.TemplateId = "gB4vyZuiziivwyYm3b1qyooZI2g2okxm4b92tEej7B4";
                    break;
                case "device_bind":
                    model.TemplateId = "fKFTJV_022tp2bhKkjBSPSIr91soiiOH5wwnbG4ZbUE";
                    break;
                case "device_unbind":
                    model.TemplateId = "czt-rzvyJnYpMK06Kv0hMcEtmJgD5vx5_mShiMGbkmo";
                    break;
            }

            return model;
        }

        public SmsTemplateModel GetSmsTemplateByBindPhone(string userId)
        {
            var tmp = new SmsTemplateModel();

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            if (d_ClientUser == null)
                return null;

            var d_SenvivMerch = CurrentDb.SenvivMerch.Where(m => m.MerchId == d_ClientUser.MerchId).FirstOrDefault();

            if (d_SenvivMerch == null)
                return null;

            tmp.SignName = d_SenvivMerch.SmsSignName;
            tmp.TemplateCode = d_SenvivMerch.SmsTemplateCode;

            return tmp;
        }
    }
}
