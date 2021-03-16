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
                        var d_SenvivHealthDay = CurrentDb.SenvivHealthDay.Where(m => m.Id == d1.reportId).FirstOrDefault();

                        if (d_SenvivHealthDay == null)
                        {
                            d_SenvivHealthDay = new Entity.SenvivHealthDay();
                            d_SenvivHealthDay.Id = d1.reportId;
                            d_SenvivHealthDay.SvUserId = d_SenvivUser.Id;
                            d_SenvivHealthDay.HealthDate = Convert2DateTime(d1.createtime);
                            d_SenvivHealthDay.TotalScore = d1.Report.TotalScore;

                            var xl = d1.ReportOfHeartBeat;

                            if (xl != null)
                            {
                                d_SenvivHealthDay.XlAvg = xl.DayCurBenchmark;//平均呼吸率
                                d_SenvivHealthDay.XlStd = xl.Benchmark;//基准心率值
                                d_SenvivHealthDay.XlMin = xl.HeartbeatMin;//当夜最小心率值
                                d_SenvivHealthDay.XlMax = xl.HeartbeatMax;//当夜最大心率值
                            }

                            var hx = d1.ReportOfBreath;
                            if (hx != null)
                            {
                                d_SenvivHealthDay.HxAvg = hx.Average;//	平均呼吸
                                d_SenvivHealthDay.HxStd = hx.Benchmark;//基准呼吸值
                                d_SenvivHealthDay.HxMin = hx.BreathMin;//当夜最小呼吸率
                                d_SenvivHealthDay.HxMax = hx.BreathMax;//当夜最大呼吸率
                                d_SenvivHealthDay.HxTd = hx.BreathMax;
                                d_SenvivHealthDay.HxZtNum = hx.PauseSum;//呼吸暂停次数
                                d_SenvivHealthDay.HxZtStd = hx.LowerCounts;
                            }

                            var hrv = d1.ReportOfHRV;
                            if (hrv != null)
                            {
                                d_SenvivHealthDay.HrvXzznl = hrv.HeartIndex;
                                // d_SenvivHealthDay.HrvXljsl = d1.Report.TotalScore;
                                d_SenvivHealthDay.HrvMzsjzl = hrv.HF;
                                d_SenvivHealthDay.HrvJgsjzl = hrv.LF;
                                d_SenvivHealthDay.HrvZzsjzl = hrv.LFHF;
                                d_SenvivHealthDay.HrvXgss = hrv.temperature;
                                d_SenvivHealthDay.HrvSDNN = hrv.SDNN;
                                //d_SenvivHealthDay.HrvPNN50 = ;
                                //d_SenvivHealthDay.HrvRMSSD = d1.Report.TotalScore;
                            }

                            d_SenvivHealthDay.CreateTime = DateTime.Now;
                            d_SenvivHealthDay.Creator = IdWorker.Build(IdType.EmptyGuid);
                            CurrentDb.SenvivHealthDay.Add(d_SenvivHealthDay);
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
