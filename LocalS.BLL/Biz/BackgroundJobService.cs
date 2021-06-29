using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class BackgroundJobService : BaseService
    {
        public CustomJsonResult SetStartOrStop(string operater, string id)
        {
            var result = new CustomJsonResult();
            var d_BackgroundJob = CurrentDb.BackgroundJob.Where(m => m.Id == id).FirstOrDefault();

            if (d_BackgroundJob == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该任务");
            }

            if (d_BackgroundJob.Status == E_BackgroundJobStatus.Runing)
            {
                SetStatus(operater, id, E_BackgroundJobStatus.Stoping);
            }
            else if (d_BackgroundJob.Status == E_BackgroundJobStatus.Stoped)
            {
                SetStatus(operater, id, E_BackgroundJobStatus.Starting);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

            return result;
        }

        public CustomJsonResult SetStatus(string operater, string id, E_BackgroundJobStatus status)
        {
            CustomJsonResult result = new CustomJsonResult();
            var backgroundJob = CurrentDb.BackgroundJob.Where(m => m.Id == id).FirstOrDefault();
            if (backgroundJob != null)
            {
                backgroundJob.Status = status;
                backgroundJob.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

            return result;
        }

        public bool UpdateInfo(string operater, string id, string jobName, DateTime lastRunTime, DateTime nextRunTime, decimal executionDuration, string runLog)
        {
            var backgroundJob = CurrentDb.BackgroundJob.Where(m => m.Id == id).FirstOrDefault();
            if (backgroundJob != null)
            {
                backgroundJob.RunCount += 1;
                backgroundJob.LastRunTime = lastRunTime;
                backgroundJob.NextRunTime = nextRunTime;
                backgroundJob.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            WriteLog(operater, id, jobName, DateTime.Now, executionDuration, runLog);

            return true;
        }

        public void WriteLog(string operater, string backgroundJobId, string jobName, DateTime executionTime, decimal executionDuration, string runLog)
        {
            var backgroundJobLog = new BackgroundJobLog();
            backgroundJobLog.Id = IdWorker.Build(IdType.NewGuid);
            backgroundJobLog.BackgroundJobId = backgroundJobId;
            backgroundJobLog.JobName = jobName;
            backgroundJobLog.ExecutionTime = executionTime;
            backgroundJobLog.ExecutionDuration = executionDuration;
            backgroundJobLog.CreateTime = DateTime.Now;
            backgroundJobLog.RunLog = runLog;
            CurrentDb.BackgroundJobLog.Add(backgroundJobLog);
            CurrentDb.SaveChanges();


        }

        public List<BackgroundJob> GeAllowScheduleJobInfoList()
        {
            var list = CurrentDb.BackgroundJob.Where(it => it.IsDelete == false).OrderBy(it => it.CreateTime).ToList();
            return list;
        }

    }
}
