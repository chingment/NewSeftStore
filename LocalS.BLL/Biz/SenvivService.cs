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
        public string MerchName { get; set; }
        public string FullName { get; set; }
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
        private void BuildTask(string operater, string svUserId, E_SvTaskType taskType, Dictionary<string, object> taskParams)
        {
            string rptId = "";
            string rptType = "";
            switch (taskType)
            {
                case E_SvTaskType.Health_Monitor_PerMonth:
                    rptType = "per_month";
                    break;
                case E_SvTaskType.Health_Monitor_FisrtDay:
                    rptType = "firtst_day";
                    break;
                case E_SvTaskType.Health_Monitor_SeventhDay:
                    rptType = "seventh_day";
                    break;
                case E_SvTaskType.Health_Monitor_FourteenthDay:
                    rptType = "fourteenth_day:";
                    break;
            }


            if (taskType == E_SvTaskType.Health_Monitor_FisrtDay)
            {
                rptId = taskParams["rpt_id"].ToString();
            }
            else
            {
                #region build
                DateTime rptStartTime = (DateTime)taskParams["start_time"];
                DateTime rptEndTime = (DateTime)taskParams["end_time"];

                var d_StageReport = CurrentDb.SvHealthStageReport.Where(m => m.SvUserId == svUserId && m.RptType == rptType && m.RptStartTime == rptStartTime && m.RptEndTime == rptEndTime).FirstOrDefault();

                if (d_StageReport != null)
                    return;

                LogUtil.Info("userId：" + svUserId + ",rptType:" + rptType + ",d_StageReport is null");

                var d_DayReports = CurrentDb.SvHealthDayReport.Where(m => m.SvUserId == svUserId && m.IsValid == true && m.ReportTime >= rptStartTime && m.ReportTime <= rptEndTime).ToList();
                if (d_DayReports.Count > 0)
                {
                    LogUtil.Info("userId：" + svUserId + ",rptType:" + rptType + ",d_DayReports:" + d_DayReports.Count);

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
                                var t1 = SvUtil.D32LongToDateTime(item.StartTime);

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
                                var t1 = SvUtil.D32LongToDateTime(item.starttime);

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
                                    DateTime t1 = SvUtil.D32LongToDateTime(xlPoint.DataTime[i] * 1000);

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
                                    DateTime t1 = SvUtil.D32LongToDateTime(hxPoint.DataTime[i] * 1000);

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
                                        var t1 = SvUtil.D32LongToDateTime(item.starttime * 1000);

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

                    d_StageReport = new SvHealthStageReport();
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


                    d_StageReport.SvUserId = svUserId;
                    d_StageReport.DayCount = d_DayReports.Count;
                    d_StageReport.HealthScore = d_DayReports.Select(m => m.HealthScore).Average();
                    d_StageReport.SmScore = d_DayReports.Select(m => m.SmScore).Average();
                    d_StageReport.MylGrfx = Decimal.Parse(d_DayReports.Select(m => m.MylGrfx).Average().ToString());//
                    d_StageReport.MylMylzs = Decimal.Parse(d_DayReports.Select(m => m.MylMylzs).Average().ToString());//
                    d_StageReport.MylMylzs = Decimal.Parse(d_DayReports.Select(m => m.MylMylzs).Average().ToString());//
                    d_StageReport.MbGxbgk = Decimal.Parse(d_DayReports.Select(m => m.MbGxbgk).Average().ToString());//
                    d_StageReport.MbGxygk = Decimal.Parse(d_DayReports.Select(m => m.MbGxygk).Average().ToString());//
                    d_StageReport.MbTnbgk = Decimal.Parse(d_DayReports.Select(m => m.MbTnbgk).Average().ToString());//
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

                    d_StageReport.DatePt = d_DayReports.Select(m => m.ReportTime.ToUnifiedFormatDate()).ToJsonString();//


                    d_StageReport.HealthScorePt = d_DayReports.Select(m => m.HealthScore).ToJsonString();
                    d_StageReport.SmScorePt = d_DayReports.Select(m => m.SmScore).ToJsonString();

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
                        var d_smTag = new SvHealthStageReportTag();
                        d_smTag.Id = IdWorker.Build(IdType.NewGuid);
                        d_smTag.SvUserId = svUserId;
                        d_smTag.ReportId = d_StageReport.Id;

                        var d_tag = CurrentDb.SvHealthTagExplain.Where(m => m.TagName == smTag.Name).FirstOrDefault();
                        if (d_tag != null)
                        {
                            d_smTag.TagId = d_tag.TagId;
                        }

                        d_smTag.TagName = smTag.Name;
                        d_smTag.TagCount = smTag.Count;

                        CurrentDb.SvHealthStageReportTag.Add(d_smTag);
                    }

                    if (rptType == "per_month")
                    {
                        d_StageReport.HealthDate = rptStartTime.ToString("yyyy-MM");
                    }
                    else
                    {
                        d_StageReport.HealthDate = rptStartTime.ToString("yyyy-MM-dd") + "~" + rptEndTime.ToString("yyyy-MM-dd");
                    }

                    d_StageReport.SmTags = smTags_Count.OrderByDescending(m => m.Count).ToJsonString();

                    d_StageReport.IsSend = false;
                    d_StageReport.VisitCount = 0;
                    d_StageReport.Status = E_SvHealthReportStatus.WaitSend;
                    d_StageReport.CreateTime = DateTime.Now;
                    d_StageReport.Creator = IdWorker.Build(IdType.NewGuid);
                    CurrentDb.SvHealthStageReport.Add(d_StageReport);
                    CurrentDb.SaveChanges();

                    rptId = d_StageReport.Id;
                }

                #endregion
            }

            if (!string.IsNullOrEmpty(rptId))
            {
                if (taskType == E_SvTaskType.Health_Monitor_FisrtDay || taskType == E_SvTaskType.Health_Monitor_SeventhDay || taskType == E_SvTaskType.Health_Monitor_FourteenthDay)
                {
                    var d_Task = CurrentDb.SvTask.Where(m => m.SvUserId == svUserId && m.TaskType == taskType).FirstOrDefault();
                    if (d_Task != null)
                        return;
                }
                else if (taskType == E_SvTaskType.Health_Monitor_PerMonth)
                {
                    LogUtil.Info("月报生成1");
                    var d_Task = CurrentDb.SvTask.Where(m => m.SvUserId == svUserId && m.ReportId == rptId).FirstOrDefault();
                    if (d_Task != null)
                        return;
                    LogUtil.Info("月报生成2");
                }


                SvTask d_SvTask = new SvTask();
                d_SvTask.Id = IdWorker.Build(IdType.NewGuid);
                d_SvTask.SvUserId = svUserId;
                d_SvTask.TaskType = taskType;

                var d_User = CurrentDb.SvUser.Where(m => m.Id == svUserId).FirstOrDefault();

                var signName = SvUtil.GetSignName("", d_User.FullName);

                DateTime rptStartTime = (DateTime)taskParams["start_time"];
                DateTime rptEndTime = (DateTime)taskParams["end_time"];

                string title = "";
                switch (taskType)
                {
                    case E_SvTaskType.Health_Monitor_FisrtDay:
                        title = string.Format("客户[{0}]的首份报告已生成，需进行回访", signName);
                        break;
                    case E_SvTaskType.Health_Monitor_SeventhDay:
                        title = string.Format("客户[{0}]的首次7天报告({1}~{2})已生成，需进行回访", signName, rptStartTime.ToUnifiedFormatDate(), rptEndTime.ToUnifiedFormatDate());
                        break;
                    case E_SvTaskType.Health_Monitor_FourteenthDay:
                        title = string.Format("客户[{0}]的首次14天报告({1}~{2})已生成，需进行回访", signName, rptStartTime.ToUnifiedFormatDate(), rptEndTime.ToUnifiedFormatDate());
                        break;
                    case E_SvTaskType.Health_Monitor_PerMonth:
                        title = string.Format("客户[{0}]的{1}月报告已生成，需进行回访", signName, rptStartTime.ToString("yyyy-MM"));
                        break;
                }
                d_SvTask.Title = title;
                d_SvTask.ReportId = rptId;
                d_SvTask.Status = E_SvTaskStatus.WaitHandle;
                d_SvTask.CreateTime = DateTime.Now;
                d_SvTask.Creator = IdWorker.Build(IdType.NewGuid);
                CurrentDb.SvTask.Add(d_SvTask);
                CurrentDb.SaveChanges();

            }
        }

        public void BuildDayReport(string svUserId, string svDeviceId, string svDeptId, bool isStopSend)
        {
            SvHealthDayReport r_DayReport = null;


            if (svDeptId == "32")
            {
                r_DayReport = BuildDayReport32(svUserId, svDeviceId, svDeptId);
            }
            else if (svDeptId == "46")
            {
                r_DayReport = BuildDayReport46(svUserId, svDeviceId, svDeptId);
            }

            if (r_DayReport == null)
                return;


            #region DayReport
            SvHealthDayReport d_DayReport = new SvHealthDayReport();
            d_DayReport.Id = r_DayReport.Id;
            d_DayReport.SvUserId = r_DayReport.SvUserId;
            d_DayReport.ReportTime = r_DayReport.ReportTime;
            d_DayReport.HealthScore = r_DayReport.HealthScore;
            d_DayReport.SmTags = r_DayReport.SmTags;
            d_DayReport.MylMylzs = r_DayReport.MylMylzs;
            d_DayReport.MylGrfx = r_DayReport.MylGrfx;
            d_DayReport.MbGxygk = r_DayReport.MbGxygk;
            d_DayReport.MbTnbgk = r_DayReport.MbTnbgk;
            d_DayReport.MbGxbgk = r_DayReport.MbGxbgk;
            d_DayReport.MbXytjjn = r_DayReport.MbXytjjn;
            d_DayReport.MbGzdmjn = r_DayReport.MbGzdmjn;
            d_DayReport.MbXtphjn = r_DayReport.MbXtphjn;

            d_DayReport.QxxlQxyj = r_DayReport.QxxlQxyj;
            d_DayReport.QxxlKynl = r_DayReport.QxxlKynl;
            d_DayReport.QxxlJlqx = r_DayReport.QxxlJlqx;
            d_DayReport.QxxlQxxl = r_DayReport.QxxlQxxl;
            d_DayReport.XlDcjzxl = r_DayReport.XlDcjzxl;//当次基准心率
            d_DayReport.XlCqjzxl = r_DayReport.XlCqjzxl;//长期基准心率
            d_DayReport.XlDcpjxl = r_DayReport.XlDcpjxl;//当次平均心率
            d_DayReport.XlZgxl = r_DayReport.XlZgxl;//最高心率
            d_DayReport.XlZdxl = r_DayReport.XlZdxl;//最低心率
            d_DayReport.XlXdgksc = r_DayReport.XlXdgksc;//心动过快时长
            d_DayReport.XlXdgmsc = r_DayReport.XlXdgmsc;//心动过慢时长
            d_DayReport.Xlcg125 = r_DayReport.Xlcg125;//todo 
            d_DayReport.Xlcg115 = r_DayReport.Xlcg115;//todo 
            d_DayReport.Xlcg085 = r_DayReport.Xlcg085;//todo 
            d_DayReport.Xlcg075 = r_DayReport.Xlcg075;//todo
            d_DayReport.HxDcpjhx = r_DayReport.HxDcpjhx;//	平均呼吸
            d_DayReport.HxDcjzhx = r_DayReport.HxDcjzhx;//基准呼吸值
            d_DayReport.HxZdhx = r_DayReport.HxZdhx;//当夜最低呼吸率
            d_DayReport.HxZghx = r_DayReport.HxZghx;//当夜最高呼吸率
            d_DayReport.HxCqjzhx = r_DayReport.HxCqjzhx; //长期基准呼吸
            d_DayReport.HxGksc = r_DayReport.HxGksc;//todo 
            d_DayReport.HxGmsc = r_DayReport.HxGmsc;//todo 
            d_DayReport.HxZtahizs = r_DayReport.HxZtahizs;//AHI指数
            d_DayReport.HxZtcs = r_DayReport.HxZtcs;//呼吸暂停次数
            d_DayReport.HxZtcsPoint = r_DayReport.HxZtcsPoint;
            d_DayReport.HxZtpjsc = r_DayReport.HxZtpjsc;//呼吸暂停平均时长
            d_DayReport.HrvXzznl = r_DayReport.HrvXzznl;//心脏总能量
            d_DayReport.HrvXzznljzz = r_DayReport.HrvXzznljzz;//心脏总能量基准值
            d_DayReport.HrvJgsjzlzs = r_DayReport.HrvJgsjzlzs;//交感神经张力指数
            d_DayReport.HrvJgsjzlzsjzz = r_DayReport.HrvJgsjzlzsjzz;// 交感神经张力基准值
            d_DayReport.HrvMzsjzlzs = r_DayReport.HrvMzsjzlzs;//迷走神经张力指数
            d_DayReport.HrvMzsjzlzsjzz = r_DayReport.HrvMzsjzlzsjzz;//迷走神经张力基准值
            d_DayReport.HrvZzsjzlzs = r_DayReport.HrvZzsjzlzs;//自主神经平衡
            d_DayReport.HrvZzsjzlzsjzz = r_DayReport.HrvZzsjzlzsjzz;//自主神经平衡基准值
            d_DayReport.HrvHermzs = r_DayReport.HrvHermzs;//荷尔蒙指数
            d_DayReport.HrvHermzsjzz = r_DayReport.HrvHermzsjzz; //荷尔蒙指数基准值
            d_DayReport.HrvTwjxgsszs = r_DayReport.HrvTwjxgsszs;//体温及血管舒缩指数
            d_DayReport.HrvTwjxgsszhjzz = r_DayReport.HrvTwjxgsszhjzz;//体温及血管舒缩基准值
            d_DayReport.JbfxXlscfx = r_DayReport.JbfxXlscfx;//心律失常风险指数
            d_DayReport.JbfxXljsl = r_DayReport.JbfxXljsl;
            d_DayReport.SmLzsc = r_DayReport.SmLzsc;
            d_DayReport.SmScore = r_DayReport.SmScore;//睡眠分数
            d_DayReport.SmScsj = r_DayReport.SmScsj;//上床时间
            d_DayReport.SmLcsj = r_DayReport.SmLcsj;//离床时间
            d_DayReport.SmZcsc = r_DayReport.SmZcsc;//起床时刻
            d_DayReport.SmRssj = r_DayReport.SmRssj;//入睡时间
            d_DayReport.SmQxsj = r_DayReport.SmQxsj;//清醒时间
            d_DayReport.SmSmsc = r_DayReport.SmSmsc;//睡眠时长
            d_DayReport.SmRsxs = r_DayReport.SmRsxs;//入睡需时
            d_DayReport.SmLzsc = r_DayReport.SmLzsc; //离枕时长
            d_DayReport.SmLzscbl = r_DayReport.SmLzscbl;
            d_DayReport.SmSmzq = r_DayReport.SmSmzq;//睡眠周期
            d_DayReport.SmSdsmsc = r_DayReport.SmSdsmsc;//深睡时长
            d_DayReport.SmSdsmbl = r_DayReport.SmSdsmbl;//深睡期比例
            d_DayReport.SmQdsmsc = r_DayReport.SmQdsmsc;//浅睡期时长
            d_DayReport.SmQdsmbl = r_DayReport.SmQdsmbl;//浅睡期比例
            d_DayReport.SmRemsmsc = r_DayReport.SmRemsmsc;//REM期时长
            d_DayReport.SmRemsmbl = r_DayReport.SmRemsmbl;//REM期比例
            d_DayReport.SmQxsc = r_DayReport.SmQxsc;//REM期时长
            d_DayReport.SmQxscbl = r_DayReport.SmQxscbl;//REM期比例
            d_DayReport.SmLzcs = r_DayReport.SmLzcs;
            d_DayReport.SmTdcs = r_DayReport.SmTdcs;//体动次数
            d_DayReport.SmTdcsPoint = r_DayReport.SmTdcsPoint;
            d_DayReport.SmPjtdsc = r_DayReport.SmPjtdsc;//平均体动时长
            d_DayReport.SmSmxl = r_DayReport.SmSmxl;
            d_DayReport.SmSmlxx = r_DayReport.SmSmlxx;
            d_DayReport.ZsGmSr = r_DayReport.ZsGmSr;
            d_DayReport.ZsGmYp = r_DayReport.ZsGmYp;
            d_DayReport.ZsGmYq = r_DayReport.ZsGmYq;
            d_DayReport.ZsGmMl = r_DayReport.ZsGmMl;
            d_DayReport.HxPoint = r_DayReport.HxPoint;
            d_DayReport.XlPoint = r_DayReport.XlPoint;
            d_DayReport.SmPoint = r_DayReport.SmPoint;
            d_DayReport.SmTdcsPoint = r_DayReport.SmTdcsPoint;
            d_DayReport.HxZtcsPoint = r_DayReport.HxZtcsPoint;
            d_DayReport.IsSend = false;
            d_DayReport.Status = E_SvHealthReportStatus.WaitSend;

            if ((d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalHours >= 4)
            {
                d_DayReport.IsValid = true;
            }

            //todo 暂时一个随机值
            ThreadSafeRandom r1 = new ThreadSafeRandom();
            int healthScoreRatio = r1.Next(80, 90);
            ThreadSafeRandom r2 = new ThreadSafeRandom();
            int smScoreRatio = r2.Next(80, 95);

            d_DayReport.HealthScoreRatio = healthScoreRatio;
            d_DayReport.SmScoreRatio = smScoreRatio;

            d_DayReport.CreateTime = DateTime.Now;
            d_DayReport.Creator = IdWorker.Build(IdType.EmptyGuid);
            CurrentDb.SvHealthDayReport.Add(d_DayReport);
            CurrentDb.SaveChanges();

            #endregion

            if (!d_DayReport.IsValid)
                return;

            SendDayReport(d_DayReport.Id, d_DayReport.RptSummary, d_DayReport.RptSuggest);

            var d_User = CurrentDb.SvUser.Where(m => m.Id == svUserId).FirstOrDefault();

            if (d_User.FisrtReportTime == null)
            {
                d_User.FisrtReportTime = DateTime.Now;
            }

            d_User.LastReportTime = DateTime.Now;
            d_User.LastReportId = d_DayReport.Id;
            d_User.Mender = IdWorker.Build(IdType.EmptyGuid);
            d_User.MendTime = DateTime.Now;

            var reportCount = CurrentDb.SvHealthDayReport.Where(m => m.Id == svUserId && m.IsValid == true).Count();

            d_User.ReportCount = reportCount;

            CurrentDb.SaveChanges();

            if (d_User.FisrtReportTime == null)
                return;

            Dictionary<string, object> taskParams = new Dictionary<string, object>();
            DateTime? rptStartTime = null;
            DateTime? rptEndTime = null;
            if ((DateTime.Now - d_User.FisrtReportTime).Value.Days == 0)
            {
                rptStartTime = Lumos.CommonUtil.ConverToStartTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;
                rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                taskParams.Add("rpt_id", d_DayReport.Id);
                taskParams.Add("start_time", rptStartTime);
                taskParams.Add("end_time", rptEndTime);
                BuildTask(IdWorker.Build(IdType.EmptyGuid), svUserId, E_SvTaskType.Health_Monitor_FisrtDay, taskParams);
            }

            if ((DateTime.Now - d_User.FisrtReportTime).Value.Days == 7)
            {
                rptStartTime = Lumos.CommonUtil.ConverToStartTime(d_User.FisrtReportTime.ToUnifiedFormatDateTime()).Value;
                rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                taskParams.Add("start_time", rptStartTime);
                taskParams.Add("end_time", rptEndTime);

                BuildTask(IdWorker.Build(IdType.EmptyGuid), svUserId, E_SvTaskType.Health_Monitor_SeventhDay, taskParams);
            }


            if ((DateTime.Now - d_User.FisrtReportTime).Value.Days == 14)
            {
                rptStartTime = Lumos.CommonUtil.ConverToStartTime(d_User.FisrtReportTime.ToUnifiedFormatDateTime()).Value;
                rptEndTime = Lumos.CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).Value;

                taskParams.Add("start_time", rptStartTime);
                taskParams.Add("end_time", rptEndTime);

                BuildTask(IdWorker.Build(IdType.EmptyGuid), svUserId, E_SvTaskType.Health_Monitor_FourteenthDay, taskParams);
            }

            DateTime dt1 = d_User.LastReportTime.Value;
            DateTime dt2 = DateTime.Now;
            int month = (dt2.Year - dt1.Year) * 12 + (dt2.Month - dt1.Month);
            if (month >= 1)
            {
                rptStartTime = Lumos.CommonUtil.ConverToStartTime(new DateTime(dt1.Year, dt1.Month, 1).ToUnifiedFormatDateTime()).Value;
                rptEndTime = Lumos.CommonUtil.ConverToEndTime((rptStartTime.Value.AddMonths(1).AddDays(-1)).ToUnifiedFormatDateTime()).Value;

                taskParams = new Dictionary<string, object>();
                taskParams.Add("start_time", rptStartTime);
                taskParams.Add("end_time", rptEndTime);

                BuildTask(IdWorker.Build(IdType.EmptyGuid), svUserId, E_SvTaskType.Health_Monitor_PerMonth, taskParams);
            }

        }

        private SvHealthDayReport BuildDayReport32(string svUserId, string svDeviceId, string svDeptId)
        {
            try
            {


                var config_Senviv = GetConfig(svDeptId);

                LogUtil.Info(TAG, "BuildDayReport32.UserId:" + svUserId + ",DeptId:" + svDeptId);

                var d1 = SdkFactory.Senviv.GetUserHealthDayReport32(config_Senviv, svUserId);

                if (d1 == null)
                {
                    return null;
                }


                if (d1.userid != svUserId)
                {
                    LogUtil.Info(TAG, "reportpar.ReportId ：" + d1.reportId + ",reportpar.userid:" + d1.userid + ",svUserId:" + svUserId + ",is not");
                    return null;
                }

                if (d1.sn != svDeviceId)
                {
                    LogUtil.Info(TAG, "reportpar.ReportId ：" + d1.reportId + ",reportpar.sn:" + d1.userid + ",svDeviceId:" + svDeviceId + ",is not");
                    return null;
                }

                var d_DayReport = CurrentDb.SvHealthDayReport.Where(m => m.Id == d1.reportId).FirstOrDefault();

                if (d_DayReport != null)
                    return null;

                #region DayReport
                d_DayReport = new SvHealthDayReport();
                d_DayReport.Id = d1.reportId;
                d_DayReport.SvUserId = svUserId;
                d_DayReport.HealthScore = d1.Report.TotalScore;

                var x2 = d1.Report;

                if (x2 != null)
                {
                    var indexs = x2.indexs;

                    #region indexs
                    if (indexs != null)
                    {
                        foreach (var index in indexs)
                        {
                            SvHealthDayReportLabel d_Label = null;
                            switch (index.type)
                            {
                                //情绪心理-情绪应激
                                case "emostress":
                                    d_DayReport.QxxlQxyj = index.score;

                                    d_Label = new SvHealthDayReportLabel();
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

                                    d_Label = new SvHealthDayReportLabel();
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

                                    d_Label = new SvHealthDayReportLabel();
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

                                    d_Label = new SvHealthDayReportLabel();
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
                                    d_DayReport.MbGxygk = 100 - index.score;

                                    d_Label = new SvHealthDayReportLabel();
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
                                    d_DayReport.MbTnbgk = 100 - index.score;

                                    d_Label = new SvHealthDayReportLabel();
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

                                    d_Label = new SvHealthDayReportLabel();
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
                                case "冠心病管控":
                                    d_DayReport.MbGxbgk = 100 - index.score;
                                    break;
                                default:
                                    d_Label = null;
                                    break;
                            }

                            if (d_Label != null)
                            {
                                CurrentDb.SvHealthDayReportLabel.Add(d_Label);
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
                            var d_Label = new SvHealthDayReportLabel();
                            d_Label.Id = IdWorker.Build(IdType.NewGuid);
                            d_Label.ReportId = d_DayReport.Id;
                            d_Label.SvUserId = d_DayReport.SvUserId;
                            d_Label.TypeName = label.TagName;
                            d_Label.Explain = label.Explain;
                            d_Label.Suggest = label.suggest.ToJsonString();
                            d_Label.Level = label.level;
                            d_Label.TypeClass = "1";
                            CurrentDb.SvHealthDayReportLabel.Add(d_Label);
                            CurrentDb.SaveChanges();

                        }

                        d_DayReport.SmTags = labels.Select(m => m.TagName).ToList().ToJsonString();

                    }

                    #endregion

                    var advices = x2.advices;

                    #region advices

                    foreach (var advice in advices)
                    {
                        var d_Advice = new SvHealthDayReportAdvice();
                        d_Advice.Id = IdWorker.Build(IdType.NewGuid);
                        d_Advice.ReportId = d_DayReport.Id;
                        d_Advice.SvUserId = d_DayReport.SvUserId;
                        d_Advice.SuggestCode = advice.suggestcode;
                        d_Advice.SuggestName = advice.suggestion;
                        d_Advice.SuggestDirection = advice.suggestdirection;
                        d_Advice.Summary = advice.summarystr;
                        CurrentDb.SvHealthDayReportAdvice.Add(d_Advice);
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
                    d_DayReport.XlZgxl = xl.HeartbeatMax;//最高心率
                    d_DayReport.XlZdxl = xl.HeartbeatMin;//最低心率
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

                    var bps = hx.ReportOfBreathPause;
                    if (bps != null)
                    {
                        var l_bps = new List<object>();

                        foreach (var bp in bps)
                        {
                            var startTime = bp.StartTime / 1000;
                            var endTime = bp.EndTime / 1000;
                            var longerVal = endTime - startTime;
                            l_bps.Add(new { startTime = startTime, endTime = endTime, longerVal = longerVal });
                        }

                        d_DayReport.HxZtcsPoint = l_bps.ToJsonString();
                    }


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
                    d_DayReport.ReportTime = SvUtil.D32LongToDateTime(x2.FinishTime);

                    d_DayReport.SmScsj = SvUtil.D32LongToDateTime(x2.StartTime);//上床时间
                    d_DayReport.SmLcsj = SvUtil.D32LongToDateTime(x2.FinishTime);//离床时间
                    d_DayReport.SmZcsc = (long)(d_DayReport.SmLcsj - d_DayReport.SmScsj).TotalSeconds;//起床时刻
                    d_DayReport.SmRssj = SvUtil.D32LongToDateTime(x2.OnbedTime);//入睡时间
                    d_DayReport.SmQxsj = SvUtil.D32LongToDateTime(x2.OffbedTime);//清醒时间
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

                    d_DayReport.SmQxsc = sm.Sober;//REM期时长
                    d_DayReport.SmQxscbl = sm.SoberRatio;//REM期比例

                    d_DayReport.SmLzcs = 0;

                    d_DayReport.SmTdcs = sm.MoveCounts;//体动次数

                    List<object> moves = new List<object>();
                    if (sm.Moves != null)
                    {
                        foreach (var move in sm.Moves)
                        {
                            if (move.starttime != 0 && move.endtime != 0)
                            {
                                moves.Add(new { startTime = move.starttime / 1000, endTime = move.endtime / 1000, score = move.score });
                            }
                        }

                        d_DayReport.SmTdcsPoint = moves.ToJsonString();

                    }


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
                                        List<object> dataValue = new List<object>();
                                        foreach (var subitem in item.subitems)
                                        {
                                            dataValue.Add(new { StartTime = subitem.starttime, EndTime = subitem.endtime, Type = subitem.type });
                                        }

                                        d_DayReport.SmPoint = (new { StartTime = item.starttime, EndTime = item.endtime, DataValue = dataValue }).ToJsonString();
                                    }
                                }
                            }
                        }
                    }
                }

                if ((d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalHours >= 4)
                {
                    d_DayReport.IsValid = true;
                }

                if (d_DayReport.SmZcsc > 0)
                {
                    d_DayReport.SmSmxl = Math.Round(((decimal)(d_DayReport.SmSdsmsc + d_DayReport.SmQdsmsc + d_DayReport.SmRemsmsc) / d_DayReport.SmZcsc), 2);
                }

                if (d_DayReport.SmSmsc > 0)
                {
                    d_DayReport.SmSmlxx = Math.Round(((decimal)(d_DayReport.SmSdsmsc + d_DayReport.SmQdsmsc + d_DayReport.SmRemsmsc) / d_DayReport.SmSmsc), 2);
                }

                #endregion

                return d_DayReport;
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);

                return null;
            }
        }

        private SvHealthDayReport BuildDayReport46(string svUserId, string svDeviceId, string svDeptId)
        {
            try
            {
                var config_Senviv = GetConfig(svDeptId);

                LogUtil.Info(TAG, "BuildDayReport46.svDeviceId" + svDeviceId + ",UserId:" + svUserId + ",DeptId:" + svDeptId);

                var d1 = SdkFactory.Senviv.GetUserHealthDayReport46(config_Senviv, svDeviceId);

                if (d1 == null)
                {
                    return null;
                }

                var reportpar = d1.reportpar;

                if (reportpar.userid != svUserId)
                {
                    LogUtil.Info(TAG, "reportpar.ReportId ：" + reportpar.ReportId + ",reportpar.userid:" + reportpar.userid + ",svUserId:" + svUserId + ",is not");
                    return null;
                }

                if (reportpar.sn != svDeviceId)
                {
                    LogUtil.Info(TAG, "reportpar.ReportId ：" + reportpar.ReportId + ",reportpar.sn:" + reportpar.userid + ",svDeviceId:" + svDeviceId + ",is not");
                    return null;
                }

                var d_DayReport = CurrentDb.SvHealthDayReport.Where(m => m.Id == reportpar.ReportId).FirstOrDefault();

                if (d_DayReport != null)
                    return null;


                #region DayReport
                d_DayReport = new SvHealthDayReport();
                d_DayReport.Id = reportpar.ReportId;
                d_DayReport.SvUserId = svUserId;
                d_DayReport.ReportTime = SvUtil.D32LongToDateTime(reportpar.CreateTime);
                d_DayReport.HealthScore = SvUtil.D46Decimal(reportpar.hv);//健康值

                d_DayReport.SmTags = reportpar.AbnormalLabel.ToJsonString();

                d_DayReport.MylMylzs = SvUtil.D46Decimal(reportpar.im);
                d_DayReport.MylGrfx = SvUtil.D46Decimal(reportpar.gr);
                d_DayReport.MbGxygk = SvUtil.D46Decimal(reportpar.hc);
                d_DayReport.MbTnbgk = SvUtil.D46Decimal(reportpar.tc);
                d_DayReport.MbGxbgk = SvUtil.D46Decimal(reportpar.mc);
                d_DayReport.MbXytjjn = SvUtil.D46Decimal(reportpar.hcNot);
                d_DayReport.MbGzdmjn = SvUtil.D46Decimal(reportpar.mcNot);
                d_DayReport.MbXtphjn = SvUtil.D46Decimal(reportpar.tcNot);
                d_DayReport.QxxlQxyj = SvUtil.D46Int(reportpar.emotion);
                d_DayReport.QxxlKynl = SvUtil.D46Decimal(reportpar.press);
                d_DayReport.QxxlJlqx = reportpar.Sc_an;
                d_DayReport.QxxlQxxl = SvUtil.D46Decimal(reportpar.qxxl);

                d_DayReport.XlDcjzxl = SvUtil.D46Int(reportpar.hr);//当次基准心率
                d_DayReport.XlCqjzxl = SvUtil.D46Int(reportpar.lhr);//长期基准心率
                d_DayReport.XlDcpjxl = SvUtil.D46Int(reportpar.avg);//当次平均心率
                d_DayReport.XlZgxl = SvUtil.D46Int(reportpar.max);//最高心率
                d_DayReport.XlZdxl = SvUtil.D46Int(reportpar.min);//最低心率
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
                d_DayReport.SmLzsc = SvUtil.D46Int(reportpar.of);
                d_DayReport.SmScore = SvUtil.D46Decimal(reportpar.sleepValue);//睡眠分数
                d_DayReport.SmScsj = SvUtil.D32LongToDateTime(reportpar.StartTime);//上床时间
                d_DayReport.SmLcsj = SvUtil.D32LongToDateTime(reportpar.FinishTime);//离床时间
                d_DayReport.SmZcsc = (long)(d_DayReport.SmLcsj - d_DayReport.SmScsj).TotalSeconds;//起床时刻
                d_DayReport.SmRssj = SvUtil.D32LongToDateTime(reportpar.OnbedTime);//入睡时间
                d_DayReport.SmQxsj = SvUtil.D32LongToDateTime(reportpar.OffbedTime);//清醒时间
                d_DayReport.SmSmsc = (long)(d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalSeconds - SvUtil.D46Long(reportpar.of);//睡眠时长
                d_DayReport.SmRsxs = (long)(d_DayReport.SmRssj - d_DayReport.SmScsj).TotalSeconds;//入睡需时
                d_DayReport.SmLzsc = SvUtil.D46Int(reportpar.of); //离枕时长
                                                                  //d_DayReport.SmLzscbl = sm.OffbedRatio;
                d_DayReport.SmSmzq = SvUtil.D46Int(reportpar.sct);//睡眠周期

                d_DayReport.SmSdsmsc = SvUtil.D46Long(reportpar.dp);//深睡时长
                d_DayReport.SmSdsmbl = SvUtil.D46Decimal(reportpar.dpr);//深睡期比例

                d_DayReport.SmQdsmsc = SvUtil.D46Long(reportpar.sl);//浅睡期时长
                d_DayReport.SmQdsmbl = SvUtil.D46Decimal(reportpar.slr);//浅睡期比例

                d_DayReport.SmRemsmsc = SvUtil.D46Long(reportpar.rem);//REM期时长
                d_DayReport.SmRemsmbl = SvUtil.D46Decimal(reportpar.remr);//REM期比例

                d_DayReport.SmQxsc = SvUtil.D46Long(reportpar.sr);//REM期时长
                d_DayReport.SmQxscbl = SvUtil.D46Decimal(reportpar.srr);//REM期比例

                d_DayReport.SmLzcs = SvUtil.D46Int(reportpar.ofbdc);
                d_DayReport.SmTdcs = SvUtil.D46Int(reportpar.mct);//体动次数
                                                                  //d_DayReport.SmTdcsPoint = sm.Moves.ToJsonString();
                d_DayReport.SmPjtdsc = SvUtil.D46Int(reportpar.mvavg);//平均体动时长
                d_DayReport.SmSmxl = SvUtil.D46Decimal(reportpar.sffcy2);
                d_DayReport.SmSmlxx = SvUtil.D46Decimal(reportpar.SleepContinuity);
                d_DayReport.ZsGmSr = SvUtil.D46Decimal(reportpar.gmsr) * 100;
                d_DayReport.ZsGmYp = SvUtil.D46Decimal(reportpar.gmyp) * 100;
                d_DayReport.ZsGmYq = SvUtil.D46Decimal(reportpar.gmyq) * 100;
                d_DayReport.ZsGmMl = SvUtil.D46Decimal(reportpar.gmml);

                //todo 暂时一个随机值
                ThreadSafeRandom r1 = new ThreadSafeRandom();
                int healthScoreRatio = r1.Next(80, 90);
                ThreadSafeRandom r2 = new ThreadSafeRandom();
                int smScoreRatio = r2.Next(80, 95);

                d_DayReport.HealthScoreRatio = healthScoreRatio;
                d_DayReport.SmScoreRatio = smScoreRatio;

                var trendcharts = d1.trendchart;
                if (trendcharts != null)
                {
                    var smScsj = SvUtil.D32DateTimeToLong(d_DayReport.SmScsj);


                    foreach (var chart in trendcharts)
                    {
                        if (chart.type == 2107)
                        {
                            var xdatatimes = new List<long>();
                            foreach (var i in chart.xdatatime)
                            {
                                xdatatimes.Add(SvUtil.D46Long(smScsj + i));
                            }

                            d_DayReport.HxPoint = (new { DataTime = xdatatimes, DataValue = chart.xdatavalue }).ToJsonString();
                        }
                        else if (chart.type == 2106)
                        {
                            var xdatatimes = new List<long>();
                            foreach (var i in chart.xdatatime)
                            {
                                xdatatimes.Add(SvUtil.D46Long(smScsj + i));
                            }

                            d_DayReport.XlPoint = (new { DataTime = xdatatimes, DataValue = chart.xdatavalue }).ToJsonString();
                        }
                    }
                }

                var barchart = d1.barchart;
                if (barchart != null)
                {
                    if (barchart.type == 2110)
                    {
                        var smScsj = SvUtil.D32DateTimeToLong(d_DayReport.SmScsj);
                        var smLcjs = SvUtil.D32DateTimeToLong(d_DayReport.SmLcsj);

                        var items = barchart.items;
                        if (items != null && items.Count > 0)
                        {
                            var sub = items[0];
                            var item2s = sub.sub;
                            var dataValues = new List<object>();
                            foreach (var item in item2s)
                            {
                                dataValues.Add(new { StartTime = smScsj + SvUtil.D46Long(item.st), EndTime = smScsj + SvUtil.D46Long(item.et), Type = SvUtil.D46Int(item.type) });
                            }
                            d_DayReport.SmPoint = (new { StartTime = smScsj, EndTime = smLcjs, DataValue = dataValues }).ToJsonString();
                        }
                    }
                }

                var mvs = d1.mv;
                if (mvs != null)
                {
                    var smScsj = SvUtil.D32DateTimeToLong(d_DayReport.SmScsj);
                    var smLcjs = SvUtil.D32DateTimeToLong(d_DayReport.SmLcsj);
                    var tdcsPoints = new List<object>();
                    foreach (var mv in mvs)
                    {
                        tdcsPoints.Add(new { startTime = smScsj + SvUtil.D46Long(mv.s), endTime = smScsj + SvUtil.D46Long(mv.e), score = 0 });
                    }

                    d_DayReport.SmTdcsPoint = tdcsPoints.ToJsonString();

                }
                var ps = d1.p;
                if (ps != null)
                {
                    var smScsj = SvUtil.D32DateTimeToLong(d_DayReport.SmScsj);
                    var smLcjs = SvUtil.D32DateTimeToLong(d_DayReport.SmLcsj);
                    var hxztPoints = new List<object>();
                    foreach (var p in ps)
                    {
                        hxztPoints.Add(new { startTime = smScsj + SvUtil.D46Long(p.s), endTime = smScsj + SvUtil.D46Long(p.e), longerVal = SvUtil.D46Long(p.i) });
                    }

                    d_DayReport.HxZtcsPoint = hxztPoints.ToJsonString();
                }

                d_DayReport.IsSend = false;
                d_DayReport.Status = E_SvHealthReportStatus.WaitSend;
                d_DayReport.CreateTime = DateTime.Now;
                d_DayReport.Creator = IdWorker.Build(IdType.EmptyGuid);

                if ((d_DayReport.SmQxsj - d_DayReport.SmRssj).TotalHours >= 4)
                {
                    d_DayReport.IsValid = true;
                }

                CurrentDb.SvHealthDayReport.Add(d_DayReport);
                CurrentDb.SaveChanges();

                if (d_DayReport.IsValid)
                {
                    SendDayReport(d_DayReport.Id, d_DayReport.RptSummary, d_DayReport.RptSuggest);
                }

                #endregion

                return d_DayReport;
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);

                return null;
            }
        }

        public WxAppConfig GetWxAppInfoConfigByMerchIdOrDeviceId(string merchId, string svDeviceId)
        {
            LogUtil.Info("merchId:" + merchId + ",deviceId:" + svDeviceId);

            if (string.IsNullOrEmpty(merchId) && string.IsNullOrEmpty(svDeviceId))
                return null;

            if (string.IsNullOrEmpty(merchId))
            {
                return GetWxAppConfigByDeviceId(svDeviceId);
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

            var d_SvMerch = CurrentDb.SvMerch.Where(m => m.MerchId == d_ClientUser.MerchId).FirstOrDefault();

            var appInfo = new WxAppInfo();
            appInfo.AppName = d_SvMerch.WxPaAppName;
            appInfo.PaQrCode = d_SvMerch.WxPaQrCode;
            return appInfo;
        }
        public WxAppConfig GetWxAppConfigByMerchId(string merchId)
        {
            var d_SvMerch = CurrentDb.SvMerch.Where(m => m.MerchId == merchId).FirstOrDefault();
            if (d_SvMerch == null)
            {
                LogUtil.Info("SvMerch：" + merchId + ",is NULL");
                return null;
            }


            var config = new WxAppConfig();
            config.AppId = d_SvMerch.WxPaAppId;
            config.AppSecret = d_SvMerch.WxPaAppSecret;

            Dictionary<string, string> exts = new Dictionary<string, string>();
            exts.Add("MerchId", merchId);
            config.Exts = exts;

            return config;
        }
        public WxAppConfig GetWxAppConfigByDeviceId(string svDeviceId)
        {
            LogUtil.Info("GetWxAppConfigByDeviceId:" + svDeviceId);
            var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.DeviceId == svDeviceId && m.IsStopUse == false).FirstOrDefault();

            if (d_MerchDevice == null)
                return null;

            return GetWxAppConfigByMerchId(d_MerchDevice.MerchId);
        }
        public WxAppConfig GetWxAppConfigByUserId(string userId)
        {
            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            if (d_ClientUser == null)
                return null;

            var d_SvMerch = CurrentDb.SvMerch.Where(m => m.MerchId == d_ClientUser.MerchId).FirstOrDefault();

            if (d_SvMerch == null)
                return null;

            var appConfig = new WxAppConfig();
            appConfig.AppId = d_SvMerch.WxPaAppId;
            appConfig.AppSecret = d_SvMerch.WxPaAppSecret;


            Dictionary<string, string> exts = new Dictionary<string, string>();
            exts.Add("MerchId", d_SvMerch.MerchId);
            exts.Add("WxPaOpenId", d_ClientUser.WxPaOpenId);
            appConfig.Exts = exts;

            return appConfig;
        }
        public SenvivConfig GetConfig(string svDeptId)
        {
            var config = new SenvivConfig();

            string account = "";
            string pwd = "";
            if (svDeptId == "32")
            {
                account = "qxtadmin";
                pwd = "zkxz123";
            }
            else if (svDeptId == "46")
            {
                account = "全线通月子会所";
                pwd = "qxt123456";
            }

            config.AccessToken = SdkFactory.Senviv.GetApiAccessToken(account, pwd);
            config.SvDeptId = svDeptId;

            return config;
        }
        public bool SendMonthReport(string svUserId, string first, string keyword1, string keyword2, string remark, string url)
        {
            var template = GetWxPaTpl(svUserId, "month_report");

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
        public bool SendDayReport(string rptId, string rptSummary, string rptSuggest)
        {
            var d_DayReport = CurrentDb.SvHealthDayReport.Where(m => m.Id == rptId).FirstOrDefault();
            var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == d_DayReport.SvUserId).FirstOrDefault();

            string theme = "green";

            if (d_SvUser.Sex == "2")
            {
                theme = "pink";
            }

            d_DayReport.RptSummary = rptSummary;
            d_DayReport.RptSuggest = rptSuggest;

            var template = GetWxPaTpl(d_DayReport.SvUserId, "day_report");

            string first = "您好，" + d_DayReport.ReportTime.ToUnifiedFormatDate() + "日健康报告已生成，详情如下";
            string url = "http://health.17fanju.com/report/day?rptId=" + rptId + "&theme=" + theme;
            string keyword1 = DateTime.Now.ToUnifiedFormatDateTime();
            string keyword2 = "总体评分" + d_DayReport.HealthScore + "分";
            string remark = "感谢您的支持，如需查看详情报告信息请点击";

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
            {
                d_DayReport.IsSend = false;
                d_DayReport.Status = E_SvHealthReportStatus.SendFailure;
                CurrentDb.SaveChanges();
                return false;
            }
            else
            {
                d_DayReport.IsSend = true;
                d_DayReport.Status = E_SvHealthReportStatus.SendSuccess;
                CurrentDb.SaveChanges();
                return true;
            }

        }
        public bool SendArticleByPregnancy(string svUserId, string first, string keyword1, string keyword2, string remark, string url)
        {
            var template = GetWxPaTpl(svUserId, "article_pregnancy");

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
        public bool SendArticleByPostpartum(string svUserId, string title, string remark, string url)
        {
            var template = GetWxPaTpl(svUserId, "article_postpartum");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"" + url + "\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + title + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + template.FullName + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + template.MerchName + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword3\":{ \"value\":\"" + DateTime.Now.ToUnifiedFormatDateTime() + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }
        public bool SendHealthMonitor(string svUserId, string first, string keyword1, string keyword2, string keyword3, string remark)
        {
            var template = GetWxPaTpl(svUserId, "health_monitor");

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
        public bool SendDeviceBind(string svUserId, string first, string keyword1, string keyword2, string remark)
        {
            var template = GetWxPaTpl(svUserId, "device_bind");

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
        public bool SendDeviceUnBind(string svUserId, string first, string keyword1, string keyword2, string remark)
        {
            var template = GetWxPaTpl(svUserId, "device_unbind");

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
        public WxPaTplModel GetWxPaTpl(string svUserId, string template)
        {
            var model = new WxPaTplModel();

            var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == svUserId).FirstOrDefault();
            var d_SvMerch = CurrentDb.SvMerch.Where(m => m.MerchId == d_SvUser.MerchId).FirstOrDefault();
            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == d_SvUser.UserId).FirstOrDefault();

            model.OpenId = d_ClientUser.WxPaOpenId;
            model.FullName = d_SvUser.FullName;
            model.MerchName = d_SvMerch.MerchName;
            //model.OpenId = "on0dM51JLVry0lnKT4Q8nsJBRXNs";


            WxAppConfig config = new WxAppConfig();
            config.AppId = d_SvMerch.WxPaAppId;
            config.AppSecret = d_SvMerch.WxPaAppSecret;
            model.AccessToken = SdkFactory.Wx.GetApiAccessToken(config);

            switch (template)
            {
                case "day_report":
                    //model.TemplateId = "GpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKY";
                    model.TemplateId = d_SvMerch.WxPaTplIdDayReport;
                    break;
                case "month_report":
                    //model.TemplateId = "GpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKY";
                    model.TemplateId = d_SvMerch.WxPaTplIdMonthReport;
                    break;
                case "health_monitor":
                    //model.TemplateId = "4rfsYerDDF7aVGuETQ3n-Kn84mjIHLBn0H6H8giz7Ac";
                    model.TemplateId = d_SvMerch.WxPaTplIdHealthMonitor;
                    break;
                case "article_pregnancy":
                    //model.TemplateId = "gB4vyZuiziivwyYm3b1qyooZI2g2okxm4b92tEej7B4";
                    model.TemplateId = d_SvMerch.WxPaTplIdPregnancyRemind;
                    break;
                case "device_bind":
                    //model.TemplateId = "fKFTJV_022tp2bhKkjBSPSIr91soiiOH5wwnbG4ZbUE";
                    model.TemplateId = d_SvMerch.WxPaTplIdDeviceBind;
                    break;
                case "device_unbind":
                    // model.TemplateId = "czt-rzvyJnYpMK06Kv0hMcEtmJgD5vx5_mShiMGbkmo";
                    model.TemplateId = d_SvMerch.WxPaTplIdDeviceUnBind;
                    break;
                case "article_postpartum":
                    model.TemplateId = d_SvMerch.WxpaTplIdPostpartumArticle;
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

            var d_SvMerch = CurrentDb.SvMerch.Where(m => m.MerchId == d_ClientUser.MerchId).FirstOrDefault();

            if (d_SvMerch == null)
                return null;

            tmp.SignName = d_SvMerch.SmsSignName;
            tmp.TemplateCode = d_SvMerch.SmsTemplateCode;

            return tmp;
        }
    }
}
