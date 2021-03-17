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
    public class Task4Tim2SenvivDayReportProvider : BaseService, IJob
    {
        public readonly string TAG = "Task4Tim2SenvivDayReportProvider";
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

        public DateTime TicksToDate(long time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000).AddHours(8);

        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var d_SenvivUsers = CurrentDb.SenvivUser.ToList();

                foreach (var d_SenvivUser in d_SenvivUsers)
                {
                    var d1 = SdkFactory.Senviv.GetUserHealthDayReport(d_SenvivUser.DeptId, d_SenvivUser.Id);

                    if (d1 != null)
                    {
                        var d_DayReport = CurrentDb.SenvivHealthDayReport.Where(m => m.Id == d1.reportId).FirstOrDefault();

                        if (d_DayReport == null)
                        {
                            d_DayReport = new SenvivHealthDayReport();
                            d_DayReport.Id = d1.reportId;
                            d_DayReport.SvUserId = d_SenvivUser.Id;
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
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "QxxlQxyj";
                                                d_Label.TypeName = "情绪应激";
                                                d_Label.Explain = index.explain;
                                                d_Label.Suggest = index.suggest.ToJsonString();
                                                d_Label.Score = index.score;
                                                break;
                                            //情绪心理-抗压能力
                                            case "compressionability":
                                                d_DayReport.QxxlKynl = index.score;

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "QxxlKynl";
                                                d_Label.TypeName = "抗压能力";
                                                d_Label.Explain = index.explain;
                                                d_Label.Suggest = index.suggest.ToJsonString();
                                                d_Label.Score = index.score;
                                                break;
                                            //免疫力-免疫力指数
                                            case "Immunity":
                                                d_DayReport.MylMylZs = index.score;

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "MylMylZs";
                                                d_Label.TypeName = "免疫力指数";
                                                d_Label.Explain = index.explain;
                                                d_Label.Suggest = index.suggest.ToJsonString();
                                                d_Label.Score = index.score;
                                                break;
                                            //免疫力-感染风险
                                            case "感染风险":

                                                d_DayReport.MylGrfx = index.score;

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "MylGrfx";
                                                d_Label.TypeName = "感染风险";
                                                d_Label.Explain = index.explain;
                                                d_Label.Suggest = index.suggest.ToJsonString();
                                                d_Label.Score = index.score;

                                                break;
                                            //慢病管理-高血压管控
                                            case "高血压管控":
                                                d_DayReport.MbGxygk = index.score;

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "MbGxbgk";
                                                d_Label.TypeName = "高血压管控";
                                                d_Label.Explain = index.explain;
                                                d_Label.Suggest = index.suggest.ToJsonString();
                                                d_Label.Score = index.score;
                                                break;
                                            //慢病管理-糖尿病管控
                                            case "糖尿病管控":
                                                d_DayReport.MbTlbgk = index.score;

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "MbTlbgk";
                                                d_Label.TypeName = "糖尿病管控";
                                                d_Label.Explain = index.explain;
                                                d_Label.Suggest = index.suggest.ToJsonString();
                                                d_Label.Score = index.score;

                                                break;
                                            //情绪心理-焦虑情绪
                                            case "Anxiety":

                                                d_DayReport.QxxlJlqx = index.score;

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "QxxlJlqx";
                                                d_Label.TypeName = "焦虑情绪";
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
                                        SenvivHealthDayReportLabel d_Label = null;

                                        switch (label.TagName)
                                        {
                                            case "消化力差":
                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "Xhnl";
                                                d_Label.TypeName = "消化力差";
                                                d_Label.Explain = label.Explain;
                                                d_Label.Suggest = label.suggest.ToJsonString();
                                                d_Label.Level = label.level;

                                                break;
                                            case "睡眠不安":

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "Smba";
                                                d_Label.TypeName = "睡眠不安";
                                                d_Label.Explain = label.Explain;
                                                d_Label.Suggest = label.suggest.ToJsonString();
                                                d_Label.Level = label.level;

                                                break;
                                            case "易醒":

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "Yx";
                                                d_Label.TypeName = "易醒";
                                                d_Label.Explain = label.Explain;
                                                d_Label.Suggest = label.suggest.ToJsonString();
                                                d_Label.Level = label.level;

                                                break;
                                            case "较难入睡":

                                                d_Label = new SenvivHealthDayReportLabel();
                                                d_Label.Id = IdWorker.Build(IdType.NewGuid);
                                                d_Label.DayReportId = d_DayReport.Id;
                                                d_Label.SvUserId = d_DayReport.SvUserId;
                                                d_Label.TypeCode = "Jxrs";
                                                d_Label.TypeName = "较难入睡";
                                                d_Label.Explain = label.Explain;
                                                d_Label.Suggest = label.suggest.ToJsonString();
                                                d_Label.Level = label.level;

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

                                    d_DayReport.SmTags = labels.Select(m => m.TagName).ToList().ToJsonString();

                                }

                                #endregion

                                var advices = x2.advices;

                                #region advices

                                foreach (var advice in advices)
                                {
                                    var d_Advice = new SenvivHealthDayReportAdvice();
                                    d_Advice.Id = IdWorker.Build(IdType.NewGuid);
                                    d_Advice.DayReportId = d_DayReport.Id;
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
                                d_DayReport.HxDcPj = hx.Average;//	平均呼吸
                                d_DayReport.HxDcjzhx = hx.Benchmark;//基准呼吸值
                                d_DayReport.HxZdHx = hx.BreathMin;//当夜最低呼吸率
                                d_DayReport.HxZgHx = hx.BreathMax;//当夜最高呼吸率
                                d_DayReport.HxCqjzhx = hx.Longterm; //长期基准呼吸
                                d_DayReport.HxGksc = 0;//todo 
                                d_DayReport.HxGmsc = 0;//todo 


                                d_DayReport.HxZtAhizs = hx.AHI;//AHI指数
                                d_DayReport.HxZtCs = hx.HigherCounts;//呼吸暂停次数
                                d_DayReport.HxZtPjsc = hx.AvgPause;//呼吸暂停平均时长
                            }
                            #endregion

                            var hrv = d1.ReportOfHRV;

                            #region ReportOfHRV
                            if (hrv != null)
                            {
                                d_DayReport.HrvXzznl = hrv.HeartIndex;//心脏总能量
                                d_DayReport.HrvXzznlJzz = hrv.BaseTP;//心脏总能量基准值
                                d_DayReport.HrvJgsjzlzs = hrv.LF;//交感神经张力指数
                                d_DayReport.HrvJgsjzlzsJzz = hrv.BaseLF;// 交感神经张力基准值
                                d_DayReport.HrvMzsjzlzs = hrv.HF;//迷走神经张力指数
                                d_DayReport.HrvMzsjzlzsJzz = hrv.BaseHF;//迷走神经张力基准值
                                d_DayReport.HrvZzsjzlzs = hrv.LFHF;//自主神经平衡
                                d_DayReport.HrvZzsjzlzsJzz = hrv.BaseLFHF;//自主神经平衡基准值
                                d_DayReport.HrvHermzs = hrv.endocrine;//荷尔蒙指数
                                d_DayReport.HrvHermzsJzz = 0;//荷尔蒙指数基准值
                                d_DayReport.HrvTwjxgsszh = hrv.temperature;//体温及血管舒缩指数
                                d_DayReport.HrvTwjxgsszhJzz = 0;//体温及血管舒缩基准值
                            }
                            #endregion

                            var sm = d1.ReportOfSleep;
                            if (sm != null)
                            {
                                d_DayReport.SmScsj = TicksToDate(x2.StartTime);//上床时间
                                d_DayReport.SmLcsj = TicksToDate(x2.FinishTime);//离床时间
                                d_DayReport.SmZcsc = (long)(d_DayReport.SmScsj - d_DayReport.SmLcsj).TotalSeconds;//起床时刻
                                d_DayReport.SmRssj = TicksToDate(x2.OnbedTime);//入睡时间
                                d_DayReport.SmQxsj = TicksToDate(x2.OffbedTime);//清醒时间
                                d_DayReport.SmSmsc = (long)(d_DayReport.SmRssj - d_DayReport.SmQxsj).TotalSeconds;//睡眠时长
                                d_DayReport.SmRsxs = (long)(d_DayReport.SmRssj - d_DayReport.SmScsj).TotalSeconds;//入睡需时

                                d_DayReport.SmSdsmsc = sm.Deep;//深睡时长
                                d_DayReport.SmSdsmbl = sm.DeepRatio;//深睡期比例

                                d_DayReport.SmQxsksc = sm.Shallow;//浅睡期时长
                                d_DayReport.SmQxskbl = sm.ShallowRatio;//浅睡期比例

                                d_DayReport.SmSemqsc = sm.Rem;//REM期时长
                                d_DayReport.SmSemqbl = sm.RemRatio;//REM期比例

                                d_DayReport.SmQxsksc = sm.Sober;//REM期时长
                                d_DayReport.SmQxskbl = sm.SoberRatio;//REM期比例

                                d_DayReport.SmLzcs = 0;

                                d_DayReport.SmTdcs = sm.MoveCounts;//体动次数
                                d_DayReport.SmPjtdsc = sm.MovingAverageLength;//平均体动时长

                            }

                            d_DayReport.CreateTime = DateTime.Now;
                            d_DayReport.Creator = IdWorker.Build(IdType.EmptyGuid);
                            CurrentDb.SenvivHealthDayReport.Add(d_DayReport);
                            CurrentDb.SaveChanges();
                        }
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
