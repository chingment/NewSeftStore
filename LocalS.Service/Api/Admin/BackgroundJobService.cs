using LocalS.BLL;
using LocalS.BLL.Task;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class BackgroundJobService : BaseService
    {
        public CustomJsonResult GetDetails(string operater, string id)
        {
            var ret = new RetBackgroundJobGetDetails();
            var backgroundJob = CurrentDb.BackgroundJob.Where(m => m.Id == id).FirstOrDefault();

            if (backgroundJob == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空", ret);
            }

            if (backgroundJob != null)
            {
                ret.Id = backgroundJob.Id;
                ret.Name = backgroundJob.Name;
                ret.AssemblyName = backgroundJob.AssemblyName;
                ret.ClassName = backgroundJob.ClassName;
                ret.Description = backgroundJob.Description;
                ret.CronExpression = backgroundJob.CronExpression;
                ret.CronExpressionDescription = backgroundJob.CronExpressionDescription;
                ret.JobArgs = backgroundJob.JobArgs;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
        }

        public CustomJsonResult Add(string operater, RopBackgroundJobAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var isExists = CurrentDb.BackgroundJob.Where(m => m.AssemblyName == rop.AssemblyName && m.ClassName == rop.ClassName).FirstOrDefault();
            if (isExists != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("程序集:{0},类名:{1},已存在"));
            }

            var type = QuartzManager.GetClassInfo(rop.AssemblyName, rop.ClassName);
            if (type == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "程序集或类型无效，无法映射");
            }

            if (!QuartzManager.ValidExpression(rop.CronExpression))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效的Cron表达式");
            }

            var backgroundJob = new BackgroundJob();
            backgroundJob.Id = IdWorker.Build(IdType.NewGuid);
            backgroundJob.Name = rop.Name;
            backgroundJob.AssemblyName = rop.AssemblyName;
            backgroundJob.ClassName = rop.ClassName;
            backgroundJob.CronExpression = rop.CronExpression;
            backgroundJob.CronExpressionDescription = rop.CronExpressionDescription;
            backgroundJob.Description = rop.Description;
            backgroundJob.DisplayOrder = 0;
            backgroundJob.RunCount = 0;
            backgroundJob.Status = E_BackgroundJobStatus.Stoped;
            backgroundJob.IsDelete = false;
            backgroundJob.Creator = operater;
            backgroundJob.CreateTime = DateTime.Now;


            CurrentDb.BackgroundJob.Add(backgroundJob);
            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

            return result;
        }

        public CustomJsonResult Edit(string operater, RopBackgroundJobEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (!QuartzManager.ValidExpression(rop.CronExpression))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "无效的Cron表达式");
            }

            var backgroundJob = CurrentDb.BackgroundJob.Where(m => m.Id == rop.Id).FirstOrDefault();

            if (backgroundJob == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
            }

            backgroundJob.CronExpression = rop.CronExpression;
            backgroundJob.CronExpressionDescription = rop.CronExpressionDescription;
            backgroundJob.Description = rop.Description;
            backgroundJob.Mender = operater;
            backgroundJob.MendTime = DateTime.Now;

            CurrentDb.SaveChanges();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

            return result;
        }

    }
}
