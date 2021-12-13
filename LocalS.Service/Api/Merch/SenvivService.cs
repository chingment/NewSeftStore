using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    /// <summary>
    /// 统一ParameterExpression
    /// </summary>
    public class ParameterReplacer : ExpressionVisitor
    {
        public ParameterReplacer(ParameterExpression paramExpr)
        {
            this.ParameterExpression = paramExpr;
        }

        public ParameterExpression ParameterExpression { get; private set; }

        public Expression Replace(Expression expr)
        {
            return this.Visit(expr);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return this.ParameterExpression;
        }
    }

    public static class PredicateExtensionses
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp_left, Expression<Func<T, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "candidate");
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.And(left, right);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp_left, Expression<Func<T, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "candidate");
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.Or(left, right);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }
    }


    public class SenvivService : BaseService
    {
        public StatusModel GetReportStatus(E_SenvivHealthReportStatus status)
        {
            var statusModel = new StatusModel();

            switch (status)
            {
                case E_SenvivHealthReportStatus.WaitBuild:
                    statusModel = new StatusModel(1, "待生成");
                    break;
                case E_SenvivHealthReportStatus.Building:
                    statusModel = new StatusModel(2, "生成中");
                    break;
                case E_SenvivHealthReportStatus.BuildSuccess:
                    statusModel = new StatusModel(3, "已生成");
                    break;
                case E_SenvivHealthReportStatus.BuildFailure:
                    statusModel = new StatusModel(4, "生成失败");
                    break;
                case E_SenvivHealthReportStatus.WaitSend:
                    statusModel = new StatusModel(5, "待评价");
                    break;
                case E_SenvivHealthReportStatus.Sending:
                    statusModel = new StatusModel(6, "发送中");
                    break;
                case E_SenvivHealthReportStatus.SendSuccess:
                    statusModel = new StatusModel(7, "已发送");
                    break;
                case E_SenvivHealthReportStatus.SendFailure:
                    statusModel = new StatusModel(8, "发送失败");
                    break;
            }

            return statusModel;
        }

        public CustomJsonResult GetUsers(string operater, string merchId, RupSenvivGetUsers rup)
        {
            var result = new CustomJsonResult();


            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            ///var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            //if (string.IsNullOrEmpty(d_Merch.SenvivDepts))
            //    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new PageEntity());
            //var deptIds = d_Merch.SenvivDepts.Split(',');

            var query = (from u in CurrentDb.SenvivUser
                         where
                         merchIds.Contains(u.MerchId)
                         && ((rup.Name == null || u.NickName.Contains(rup.Name)) ||
                         (rup.Name == null || u.FullName.Contains(rup.Name)))
                         select u);

            if (rup.Sas != "0")
            {
                query = query.Where(m => m.SAS == rup.Sas);
            }



            if (rup.Chronic != "0")
            {
                var pred = PredicateExtensionses.False<SenvivUser>();
                pred = pred.Or(m => m.Perplex.Contains(rup.Chronic));
                query = query.Where(pred);
            }


            if (rup.Perplex != "0")
            {
                var pred = PredicateExtensionses.False<SenvivUser>();
                pred = pred.Or(m => m.Perplex.Contains(rup.Perplex));
                query = query.Where(pred);
            }

            if (rup.CareLevel != E_SenvivUserCareLevel.None)
            {
                query = query.Where(m => m.CareLevel == rup.CareLevel);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                List<string> perplex = new List<string>();

                if (!string.IsNullOrEmpty(item.Perplex))
                {
                    string[] arrs = item.Perplex.Split(',');

                    foreach (var val in arrs)
                    {
                        var name = SvUtil.GetPerplexName(val);
                        if (!string.IsNullOrEmpty(name))
                        {
                            perplex.Add(name);
                        }
                    }
                }

                string signName = item.NickName;

                if (!string.IsNullOrEmpty(item.FullName) && item.NickName != item.FullName)
                {
                    signName += "(" + item.FullName + ")";
                }


                string age = "-";

                if (item.Birthday != null)
                {
                    age = CommonUtil.GetAgeByBirthdate(item.Birthday.Value).ToString();
                }

                olist.Add(new
                {
                    Id = item.Id,
                    SignName = signName,
                    Avatar = item.Avatar,
                    SAS = SvUtil.GetSASName(item.SAS),
                    BreathingMachine = SvUtil.GetBreathingMachineName(item.BreathingMachine),
                    SignTags = SvUtil.GetSignTags(item.Perplex, item.OtherPerplex),
                    Medicalhistory = SvUtil.GetMedicalhistoryName(item.Medicalhistory),
                    Medicine = SvUtil.GetMedicineNames(item.Medicine),
                    Sex = SvUtil.GetSexName(item.Sex),
                    Age = age,
                    Height = item.Height,
                    Weight = item.Weight,
                    PhoneNumber = item.PhoneNumber,
                    LastReportId = item.LastReportId,
                    LastReportTime = item.LastReportTime
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetUserDetail(string operater, string merchId, string userId)
        {
            var result = new CustomJsonResult();

            var d_SenvivUser = (from u in CurrentDb.SenvivUser
                                where u.Id == userId
                                select new { u.Id, u.NickName, u.Avatar, u.Birthday, u.SAS, u.Perplex, u.Height, u.Weight, u.Medicalhistory, u.Medicine, u.OtherPerplex, u.BreathingMachine, u.FullName, u.Sex, u.PhoneNumber, u.LastReportId, u.LastReportTime, u.CreateTime }).FirstOrDefault();

            List<string> perplex = new List<string>();

            if (!string.IsNullOrEmpty(d_SenvivUser.Perplex))
            {
                string[] arrs = d_SenvivUser.Perplex.Split(',');

                foreach (var val in arrs)
                {
                    var name = SvUtil.GetPerplexName(val);
                    if (!string.IsNullOrEmpty(name))
                    {
                        perplex.Add(name);
                    }
                }
            }


            var ret = new
            {
                Id = d_SenvivUser.Id,
                SignName = SvUtil.GetSignName(d_SenvivUser.NickName, d_SenvivUser.FullName),
                Age = SvUtil.GetAge(d_SenvivUser.Birthday),
                Avatar = d_SenvivUser.Avatar,
                SAS = SvUtil.GetSASName(d_SenvivUser.SAS),
                BreathingMachine = SvUtil.GetBreathingMachineName(d_SenvivUser.BreathingMachine),
                SignTags = SvUtil.GetSignTags(d_SenvivUser.Perplex, d_SenvivUser.OtherPerplex),
                Medicalhistory = SvUtil.GetMedicalhistoryName(d_SenvivUser.Medicalhistory),
                Medicine = SvUtil.GetMedicineNames(d_SenvivUser.Medicine),
                Sex = SvUtil.GetSexName(d_SenvivUser.Sex),
                Height = d_SenvivUser.Height,
                Weight = d_SenvivUser.Weight,
                PhoneNumber = d_SenvivUser.PhoneNumber,
                LastReportId = d_SenvivUser.LastReportId,
                LastReportTime = d_SenvivUser.LastReportTime
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetDayReports(string operater, string merchId, RupSenvivGetDayReports rup)
        {
            var result = new CustomJsonResult();

            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            //var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();


            //if (string.IsNullOrEmpty(d_Merch.SenvivDepts))
            //    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new PageEntity());

            //var deptIds = d_Merch.SenvivDepts.Split(',');

            var query = (from u in CurrentDb.SenvivHealthDayReport

                         join s in CurrentDb.SenvivUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.IsValid == true
                         &&
                         merchIds.Contains(tt.MerchId)
                         select new
                         {
                             u.Id,
                             tt.NickName,
                             tt.Sex,
                             u.SvUserId,
                             tt.FullName,
                             tt.Birthday,
                             tt.Avatar,
                             u.TotalScore,
                             u.HealthDate,
                             u.SmTags,
                             u.SmRssj,
                             u.SmQxsj,
                             u.MylGrfx,
                             u.MylMylzs,
                             u.MbGxbgk,
                             u.MbGxygk,
                             u.MbTlbgk,
                             u.QxxlJlqx,
                             u.QxxlKynl,
                             u.QxxlQxyj,
                             u.JbfxXljsl,
                             u.JbfxXlscfx,
                             u.HrvXzznl,
                             u.HrvJgsjzlzs,
                             u.HrvMzsjzlzs,
                             u.HrvZzsjzlzs,
                             u.HrvHermzs,
                             u.HrvTwjxgsszs,
                             u.XlDcjzxl,
                             u.XlCqjzxl,
                             u.XlDcpjxl,
                             u.HxDcjzhx,
                             u.HxCqjzhx,
                             u.HxDcpjhx,
                             u.HxZtcs,
                             u.HxZtahizs,
                             u.HxZtpjsc,
                             u.SmSmsc,
                             u.SmSdsmsc,
                             u.SmQdsmsc,
                             u.SmRemsmsc,
                             u.SmTdcs,
                             u.SmSmzq,
                             u.Status,
                             u.IsSend,
                             u.CreateTime
                         });

            if (!string.IsNullOrEmpty(rup.Name))
            {
                query = query.Where(m => ((rup.Name == null || m.NickName.Contains(rup.Name)) || (rup.Name == null || m.FullName.Contains(rup.Name))));
            }

            if (rup.HealthDate != null && rup.HealthDate.Length == 2)
            {

                DateTime? startTime = Lumos.CommonUtil.ConverToStartTime(rup.HealthDate[0]);
                DateTime? endTime = Lumos.CommonUtil.ConverToEndTime(rup.HealthDate[1]);

                query = query.Where(m => m.HealthDate >= startTime && m.HealthDate <= endTime);
            }

            if (!string.IsNullOrEmpty(rup.UserId))
            {
                query = query.Where(m => m.SvUserId == rup.UserId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.HealthDate).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var rpt in list)
            {
                olist.Add(new
                {
                    Id = rpt.Id,
                    SvUserId = rpt.SvUserId,
                    SignName = SvUtil.GetSignName(rpt.NickName, rpt.FullName),
                    Avatar = rpt.Avatar,
                    Sex = SvUtil.GetSexName(rpt.Sex),
                    Age = SvUtil.GetAge(rpt.Birthday),
                    HealthDate = rpt.HealthDate.ToUnifiedFormatDate(),
                    TotalScore = rpt.TotalScore,
                    SmRssj = rpt.SmRssj.ToUnifiedFormatDateTime(),
                    SmQxsj = rpt.SmQxsj.ToUnifiedFormatDateTime(),
                    DsTags = rpt.SmTags.ToJsonObject<List<string>>(),
                    MylGrfx = SvDataJdUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvDataJdUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvDataJdUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvDataJdUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvDataJdUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvDataJdUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvDataJdUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvDataJdUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvDataJdUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvDataJdUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvDataJdUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvDataJdUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvDataJdUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvDataJdUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvDataJdUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvDataJdUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvDataJdUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvDataJdUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvDataJdUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvDataJdUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvDataJdUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvDataJdUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvDataJdUtil.GetSmSmsc(rpt.SmSmsc),
                    //深度睡眠时长
                    SmSdsmsc = SvDataJdUtil.GetSmSdsmsc(rpt.SmSdsmsc),
                    //浅度睡眠时长
                    SmQdsmsc = SvDataJdUtil.GetSmQdsmsc(rpt.SmQdsmsc),
                    //REM睡眠时长
                    SmRemsmsc = SvDataJdUtil.GetSmRemsmsc(rpt.SmRemsmsc),
                    //睡眠周期=
                    SmSmzq = SvDataJdUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvDataJdUtil.GetSmTdcs(rpt.SmTdcs),
                    IsSend = rpt.IsSend,
                    Status = GetReportStatus(rpt.Status),
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;

        }

        public CustomJsonResult GetDayReportDetail(string operater, string merchId, string reportId)
        {
            var result = new CustomJsonResult();

            var d_Rpt = (from u in CurrentDb.SenvivHealthDayReport

                         join s in CurrentDb.SenvivUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.Id == reportId
                         select new
                         {
                             u.Id,
                             tt.NickName,
                             tt.Sex,
                             tt.FullName,
                             tt.Birthday,
                             tt.Avatar,
                             u.TotalScore,
                             u.HealthDate,
                             u.SmTags,
                             u.SmRssj,
                             u.SmQxsj,
                             u.MylGrfx,
                             u.MylMylzs,

                             u.MbGxbgk,
                             u.MbGxygk,
                             u.MbTlbgk,
                             u.QxxlJlqx,
                             u.QxxlKynl,
                             u.QxxlQxyj,
                             u.JbfxXljsl,
                             u.JbfxXlscfx,
                             u.HrvXzznl,
                             u.HrvXzznljzz,
                             u.HrvJgsjzlzs,
                             u.HrvJgsjzlzsjzz,
                             u.HrvMzsjzlzs,
                             u.HrvMzsjzlzsjzz,
                             u.HrvZzsjzlzs,
                             u.HrvZzsjzlzsjzz,
                             u.HrvHermzs,
                             u.HrvHermzsjzz,
                             u.HrvTwjxgsszs,
                             u.HrvTwjxgsszhjzz,
                             //当次基准心率
                             u.XlDcjzxl,
                             //长期基准心率
                             u.XlCqjzxl,
                             //当次平均心率
                             u.XlDcpjxl,
                             //最高心率
                             u.XlZg,
                             //最低心率
                             u.XlZd,
                             //心动过快时长
                             u.XlXdgksc,
                             //心动过慢时长
                             u.XlXdgmsc,
                             //心率超过1.25时长
                             u.Xlcg125,
                             //心率超过1.15时长
                             u.Xlcg115,
                             //心率超过0.85时长
                             u.Xlcg085,
                             //心率超过075时长
                             u.Xlcg075,
                             //呼吸当次基准呼吸
                             u.HxDcjzhx,
                             //呼吸长期基准呼吸
                             u.HxCqjzhx,
                             //呼吸平均呼吸
                             u.HxDcpjhx,
                             //呼吸最高呼吸
                             u.HxZghx,
                             //呼吸最低呼吸
                             u.HxZdhx,
                             //呼吸过快时长
                             u.HxGksc,
                             //呼吸过慢时长
                             u.HxGmsc,
                             //呼吸暂停次数
                             u.HxZtcs,
                             //呼吸暂停AHI指数
                             u.HxZtahizs,
                             //呼吸暂停平均时长
                             u.HxZtpjsc,
                             u.SmScsj,
                             u.SmLcsj,
                             u.SmZcsc,
                             //睡眠时长
                             u.SmSmsc,
                             //睡眠周期
                             u.SmSmzq,
                             //入睡需时
                             u.SmRsxs,
                             //深度睡眠时长
                             u.SmSdsmsc,
                             //深度睡眠比例
                             u.SmSdsmbl,
                             //浅度睡眠时长
                             u.SmQdsmsc,
                             //浅度睡眠比例
                             u.SmQdsmbl,
                             //REM睡眠时长
                             u.SmRemsmsc,
                             //REM睡眠比例
                             u.SmRemsmbl,
                             //清醒时刻时长
                             u.SmQxsksc,
                             //清醒时刻比例
                             u.SmQxskbl,
                             //离真次数
                             u.SmLzcs,
                             //离真时长
                             u.SmLzsc,
                             //体动次数
                             u.SmTdcs,
                             //平均体动时长
                             u.SmPjtdsc,
                             u.SmLzscbl,
                             u.SvUserId,
                             u.SmPoint,
                             u.HxPoint,
                             u.XlPoint,
                             u.CreateTime
                         }).FirstOrDefault();



            var ret = new
            {
                Id = d_Rpt.Id,
                UserInfo = new
                {
                    SignName = SvUtil.GetSignName(d_Rpt.NickName, d_Rpt.FullName),
                    Avatar = d_Rpt.Avatar,
                    Sex = SvUtil.GetSexName(d_Rpt.Sex),
                    Age = SvUtil.GetAge(d_Rpt.Birthday)
                },
                ReportData = new
                {
                    HealthDate = d_Rpt.HealthDate.ToUnifiedFormatDate(),
                    TotalScore = d_Rpt.TotalScore,
                    SmScsj = d_Rpt.SmScsj.ToString("yyyy/MM/dd HH:mm"),
                    SmRssj = d_Rpt.SmRssj.ToString("yyyy/MM/dd HH:mm"),
                    SmQxsj = d_Rpt.SmQxsj.ToString("yyyy/MM/dd HH:mm"),
                    SmLcsj = d_Rpt.SmLcsj.ToString("yyyy/MM/dd HH:mm"),
                    SmPie = new
                    {
                        Data = new List<object>() {
new {  Name = "浅度", Value = d_Rpt.SmQdsmbl},
new {  Name = "深度", Value = d_Rpt.SmSdsmbl},
new {  Name = "REM", Value = d_Rpt.SmRemsmbl},
new {  Name = "清醒", Value = d_Rpt.SmQxskbl},
new {  Name = "离床", Value = d_Rpt.SmLzscbl} }
                    },
                    DsTags = d_Rpt.SmTags.ToJsonObject<List<string>>(),
                    d_Rpt.MylGrfx,
                    d_Rpt.MylMylzs,
                    d_Rpt.MbGxbgk,
                    d_Rpt.MbGxygk,
                    d_Rpt.MbTlbgk,
                    d_Rpt.QxxlJlqx,
                    d_Rpt.QxxlKynl,
                    d_Rpt.QxxlQxyj,
                    JbfxXlscfx = SvDataJdUtil.GetJbfxXlscfx(d_Rpt.JbfxXlscfx),
                    JbfxXljsl = SvDataJdUtil.GetJbfxXljsl(d_Rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvDataJdUtil.GetHrvXzznl(d_Rpt.HrvXzznl),
                    //心脏总能量基准值
                    d_Rpt.HrvXzznljzz,
                    //交感神经张力指数
                    HrvJgsjzlzs = SvDataJdUtil.GetHrvJgsjzlzs(d_Rpt.HrvJgsjzlzs),
                    //交感神经张力指数基准值
                    d_Rpt.HrvJgsjzlzsjzz,
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvDataJdUtil.GetHrvMzsjzlzs(d_Rpt.HrvMzsjzlzs),
                    //迷走神经张力指数基准值
                    d_Rpt.HrvMzsjzlzsjzz,
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvDataJdUtil.GetHrvZzsjzlzs(d_Rpt.HrvZzsjzlzs),
                    //自主神经平衡指数基准值
                    d_Rpt.HrvZzsjzlzsjzz,
                    //荷尔蒙指数
                    HrvHermzs = SvDataJdUtil.GetHrvHermzs(d_Rpt.HrvHermzs),
                    //荷尔蒙指数基准值
                    d_Rpt.HrvHermzsjzz,
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvDataJdUtil.GetHrvTwjxgsszh(d_Rpt.HrvTwjxgsszs),
                    //体温及血管舒缩指数基准值
                    d_Rpt.HrvTwjxgsszhjzz,
                    //当次基准心率
                    XlDcjzxl = SvDataJdUtil.GetXlDcjzxl(d_Rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvDataJdUtil.GetXlCqjzxl(d_Rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvDataJdUtil.GetXlDcpjxl(d_Rpt.XlDcpjxl),
                    //最高心率
                    d_Rpt.XlZg,
                    //最低心率
                    d_Rpt.XlZd,
                    //心动过快时长
                    d_Rpt.XlXdgksc,
                    //心动过慢时长
                    d_Rpt.XlXdgmsc,
                    //心率超过1.25时长
                    d_Rpt.Xlcg125,
                    //心率超过1.15时长
                    d_Rpt.Xlcg115,
                    //心率超过0.85时长
                    d_Rpt.Xlcg085,
                    //心率超过075时长
                    d_Rpt.Xlcg075,
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvDataJdUtil.GetHxDcjzhx(d_Rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvDataJdUtil.GetHxCqjzhx(d_Rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvDataJdUtil.GetHxDcpjhx(d_Rpt.HxDcpjhx),
                    //呼吸最高呼吸
                    d_Rpt.HxZghx,
                    //呼吸最低呼吸
                    d_Rpt.HxZdhx,
                    //呼吸过快时长
                    d_Rpt.HxGksc,
                    //呼吸过慢时长
                    d_Rpt.HxGmsc,
                    //呼吸暂停次数
                    HxZtcs = SvDataJdUtil.GetHxZtcs(d_Rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvDataJdUtil.GetHxZtahizs(d_Rpt.HxZtahizs),
                    //呼吸暂停平均时长
                    d_Rpt.HxZtpjsc,
                    d_Rpt.SmZcsc,
                    //睡眠时长
                    SmSmsc = SvDataJdUtil.GetSmSmsc(d_Rpt.SmSmsc),
                    //深度睡眠时长
                    SmSdsmsc = SvDataJdUtil.GetSmSdsmsc(d_Rpt.SmSdsmsc),
                    //浅度睡眠时长
                    SmQdsmsc = SvDataJdUtil.GetSmQdsmsc(d_Rpt.SmQdsmsc),
                    //REM睡眠时长
                    SmRemsmsc = SvDataJdUtil.GetSmRemsmsc(d_Rpt.SmRemsmsc),
                    //睡眠周期=
                    SmSmzq = SvDataJdUtil.GetSmSmzq(d_Rpt.SmSmzq),
                    //清醒时刻时长
                    d_Rpt.SmQxsksc,
                    //清醒时刻比例
                    d_Rpt.SmQxskbl,
                    //离真次数
                    d_Rpt.SmLzcs,
                    //离真时长
                    d_Rpt.SmLzsc,
                    //体动次数
                    SmTdcs = SvDataJdUtil.GetSmTdcs(d_Rpt.SmTdcs),
                    //平均体动时长
                    d_Rpt.SmPjtdsc,
                    SmPoint = d_Rpt.SmPoint.ToJsonObject<object>(),
                    XlPoint = d_Rpt.XlPoint.ToJsonObject<object>(),
                    HxPoint = d_Rpt.HxPoint.ToJsonObject<object>()
                }
            };



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult GetMonthReports(string operater, string merchId, RupSenvivGetDayReports rup)
        {
            var result = new CustomJsonResult();

            //var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();


            //if (string.IsNullOrEmpty(d_Merch.SenvivDepts))
            //    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new PageEntity());

            //var deptIds = d_Merch.SenvivDepts.Split(',');

            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var query = (from u in CurrentDb.SenvivHealthMonthReport

                         join s in CurrentDb.SenvivUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where
                         merchIds.Contains(tt.MerchId)
                         select new
                         {
                             u.Id,
                             tt.NickName,
                             tt.Sex,
                             tt.FullName,
                             tt.Birthday,
                             tt.Avatar,
                             u.TotalScore,
                             u.HealthDate,
                             u.MylGrfx,
                             u.MylMylzs,
                             u.MbGxbgk,
                             u.MbGxygk,
                             u.MbTlbgk,
                             u.QxxlJlqx,
                             u.QxxlKynl,
                             u.QxxlQxyj,
                             u.JbfxXljsl,
                             u.JbfxXlscfx,
                             u.HrvXzznl,
                             u.HrvJgsjzlzs,
                             u.HrvMzsjzlzs,
                             u.HrvZzsjzlzs,
                             u.HrvHermzs,
                             u.HrvTwjxgsszs,
                             u.XlDcjzxl,
                             u.XlCqjzxl,
                             u.XlDcpjxl,
                             u.HxDcjzhx,
                             u.HxCqjzhx,
                             u.HxDcpjhx,
                             u.HxZtcs,
                             u.HxZtahizs,
                             u.HxZtpjsc,
                             u.SmSmsc,
                             u.SmSdsmsc,
                             u.SmQdsmsc,
                             u.SmRemsmsc,
                             u.SmTdcs,
                             u.SmSmzq,
                             u.IsBuild,
                             u.IsSend,
                             u.VisitCount,
                             u.Status,
                             u.SvUserId,
                             u.CreateTime
                         });

            if (!string.IsNullOrEmpty(rup.Name))
            {
                query = query.Where(m => ((rup.Name == null || m.NickName.Contains(rup.Name)) || (rup.Name == null || m.FullName.Contains(rup.Name))));
            }


            if (!string.IsNullOrEmpty(rup.UserId))
            {
                query = query.Where(m => m.SvUserId == rup.UserId);
            }

            if (rup.HealthDate != null && rup.HealthDate.Length == 2)
            {
                var d1 = rup.HealthDate[0];
                var d2 = rup.HealthDate[1];
                if (d1 == d2)
                {
                    query = query.Where(m => m.HealthDate == d1);
                }
            }



            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.HealthDate).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var rpt in list)
            {
                olist.Add(new
                {
                    Id = rpt.Id,
                    SvUserId = rpt.SvUserId,
                    SignName = SvUtil.GetSignName(rpt.NickName, rpt.FullName),
                    Avatar = rpt.Avatar,
                    Sex = SvUtil.GetSexName(rpt.Sex),
                    Age = SvUtil.GetAge(rpt.Birthday),
                    rpt.TotalScore,
                    rpt.HealthDate,
                    MylGrfx = SvDataJdUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvDataJdUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvDataJdUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvDataJdUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvDataJdUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvDataJdUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvDataJdUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvDataJdUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvDataJdUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvDataJdUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvDataJdUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvDataJdUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvDataJdUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvDataJdUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvDataJdUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvDataJdUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvDataJdUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvDataJdUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvDataJdUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvDataJdUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvDataJdUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvDataJdUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvDataJdUtil.GetSmSmsc(rpt.SmSmsc),
                    //深度睡眠时长
                    SmSdsmsc = SvDataJdUtil.GetSmSdsmsc(rpt.SmSdsmsc),
                    //浅度睡眠时长
                    SmQdsmsc = SvDataJdUtil.GetSmQdsmsc(rpt.SmQdsmsc),
                    //REM睡眠时长
                    SmRemsmsc = SvDataJdUtil.GetSmRemsmsc(rpt.SmRemsmsc),
                    //睡眠周期=
                    SmSmzq = SvDataJdUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvDataJdUtil.GetSmTdcs(rpt.SmTdcs),
                    Status = GetReportStatus(rpt.Status),
                    IsBuild = rpt.IsBuild,
                    IsSend = rpt.IsSend
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;

        }

        public CustomJsonResult GetMonthReportDetail(string operater, string merchId, string reportId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SenvivHealthMonthReport

                       join s in CurrentDb.SenvivUser on u.SvUserId equals s.Id into temp
                       from tt in temp.DefaultIfEmpty()
                       where u.Id == reportId
                       select new
                       {
                           u.Id,
                           tt.NickName,
                           tt.Sex,
                           tt.FullName,
                           tt.Birthday,
                           tt.Avatar,
                           u.SmTags,
                           u.TotalScore,
                           u.HealthDate,
                           u.MylGrfx,
                           u.MylMylzs,
                           u.MbGxbgk,
                           u.MbGxygk,
                           u.MbTlbgk,
                           u.QxxlJlqx,
                           u.QxxlKynl,
                           u.QxxlQxyj,
                           u.JbfxXljsl,
                           u.JbfxXlscfx,
                           u.HrvXzznl,
                           u.HrvJgsjzlzs,
                           u.HrvMzsjzlzs,
                           u.HrvZzsjzlzs,
                           u.HrvHermzs,
                           u.HrvTwjxgsszs,
                           u.XlDcjzxl,
                           u.XlCqjzxl,
                           u.XlDcpjxl,
                           u.HxDcjzhx,
                           u.HxCqjzhx,
                           u.HxDcpjhx,
                           u.HxZtcs,
                           u.HxZtahizs,
                           u.HxZtpjsc,
                           u.SmSmsc,
                           u.SmSdsmsc,
                           u.SmQdsmsc,
                           u.SmRemsmsc,
                           u.SmTdcs,
                           u.SmSmzq,
                           u.IsBuild,
                           u.IsSend,
                           u.VisitCount,
                           u.Status,
                           u.SvUserId,
                           u.DatePt,
                           u.SmSmscPt,
                           u.XlDcjzxlPt,
                           u.XlCqjzxlPt,
                           u.HrvXzznlPt,
                           u.HxZtcsPt,
                           u.HxDcjzhxPt,
                           u.HxCqjzhxPt,
                           u.HxZtahizsPt,
                           u.HrvJgsjzlzsPt,
                           u.HrvMzsjzlzsPt,
                           u.HrvZzsjzlzsPt,
                           u.HrvHermzsPt,
                           u.HrvTwjxgsszsPt,
                           u.JbfxXljslPt,
                           u.JbfxXlscfxPt,
                           u.TimeFrameStaPt,
                           u.CreateTime
                       }).FirstOrDefault();



            var ret = new
            {
                Id = rpt.Id,
                UserInfo = new
                {
                    SignName = SvUtil.GetSignName(rpt.NickName, rpt.FullName),
                    Avatar = rpt.Avatar,
                    Sex = SvUtil.GetSexName(rpt.Sex),
                    Age = SvUtil.GetAge(rpt.Birthday)
                },
                ReportData = new
                {
                    rpt.TotalScore,
                    rpt.HealthDate,
                    SmTags = rpt.SmTags.ToJsonObject<List<object>>(),
                    MylGrfx = SvDataJdUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvDataJdUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvDataJdUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvDataJdUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvDataJdUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvDataJdUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvDataJdUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvDataJdUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvDataJdUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvDataJdUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvDataJdUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvDataJdUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvDataJdUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvDataJdUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvDataJdUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvDataJdUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvDataJdUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvDataJdUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvDataJdUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvDataJdUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvDataJdUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvDataJdUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvDataJdUtil.GetSmSmsc(rpt.SmSmsc),
                    //深度睡眠时长
                    SmSdsmsc = SvDataJdUtil.GetSmSdsmsc(rpt.SmSdsmsc),
                    //浅度睡眠时长
                    SmQdsmsc = SvDataJdUtil.GetSmQdsmsc(rpt.SmQdsmsc),
                    //REM睡眠时长
                    SmRemsmsc = SvDataJdUtil.GetSmRemsmsc(rpt.SmRemsmsc),
                    //睡眠周期=
                    SmSmzq = SvDataJdUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvDataJdUtil.GetSmTdcs(rpt.SmTdcs),
                    DatePt = rpt.DatePt.ToJsonObject<List<object>>(),
                    SmSmscPt = rpt.SmSmscPt.ToJsonObject<List<object>>(),
                    XlDcjzxlPt = rpt.XlDcjzxlPt.ToJsonObject<List<object>>(),
                    XlCqjzxlPt = rpt.XlCqjzxlPt.ToJsonObject<List<object>>(),
                    HrvXzznlPt = rpt.HrvXzznlPt.ToJsonObject<List<object>>(),
                    HxZtcsPt = rpt.HxZtcsPt.ToJsonObject<List<object>>(),
                    HxDcjzhxPt = rpt.HxDcjzhxPt.ToJsonObject<List<object>>(),
                    HxCqjzhxPt = rpt.HxCqjzhxPt.ToJsonObject<List<object>>(),
                    HxZtahizsPt = rpt.HxZtahizsPt.ToJsonObject<List<object>>(),
                    HrvJgsjzlzsPt = rpt.HrvJgsjzlzsPt.ToJsonObject<List<object>>(),
                    HrvMzsjzlzsPt = rpt.HrvMzsjzlzsPt.ToJsonObject<List<object>>(),
                    HrvZzsjzlzsPt = rpt.HrvZzsjzlzsPt.ToJsonObject<List<object>>(),
                    HrvHermzsPt = rpt.HrvHermzsPt.ToJsonObject<List<object>>(),
                    HrvTwjxgsszsPt = rpt.HrvTwjxgsszsPt.ToJsonObject<List<object>>(),
                    JbfxXljslPt = rpt.JbfxXljslPt.ToJsonObject<List<object>>(),
                    JbfxXlscfxPt = rpt.JbfxXlscfxPt.ToJsonObject<List<object>>(),
                    TimeFrameStaPt = rpt.TimeFrameStaPt.ToJsonObject<object>()
                }
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult GetMonthReportSug(string operater, string merchId, string reportId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SenvivHealthMonthReport
                       where u.Id == reportId
                       select new
                       {
                           u.Id,
                           u.RptSummary,
                           u.RptSuggest,
                           u.IsSend,
                           u.Status,
                           u.CreateTime
                       }).FirstOrDefault();

            var d_SugSkus = CurrentDb.SenvivHealthMonthReportSugSku.Where(m => m.ReportId == reportId).ToList();

            var sugSkus = new List<object>();

            if (d_SugSkus != null)
            {
                foreach (var d_SugSku in d_SugSkus)
                {
                    var r_Sku = CacheServiceFactory.Product.GetSkuInfo(merchId, d_SugSku.SkuId);

                    if (r_Sku != null)
                    {
                        sugSkus.Add(new { Id = r_Sku.Id, Name = r_Sku.Name, CumCode = r_Sku.CumCode });
                    }
                }
            }

            var ret = new
            {
                rpt.RptSummary,
                rpt.RptSuggest,
                rpt.IsSend,
                sugSkus
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult SaveMonthReportSug(string operater, string merchId, SenvivSaveMonthReportSug rop)
        {

            var result = new CustomJsonResult();

            var rpt = CurrentDb.SenvivHealthMonthReport.Where(m => m.Id == rop.ReportId).FirstOrDefault();

            rpt.RptSummary = rop.RptSummary;
            rpt.RptSuggest = rop.RptSuggest;

            var d_SugSkus = CurrentDb.SenvivHealthMonthReportSugSku.Where(m => m.ReportId == rop.ReportId).ToList();

            foreach (var d_SugSku in d_SugSkus)
            {
                CurrentDb.SenvivHealthMonthReportSugSku.Remove(d_SugSku);
            }

            if (rop.SugSkus != null)
            {
                foreach (var sugSku in rop.SugSkus)
                {
                    var d_SugSku = new SenvivHealthMonthReportSugSku();
                    d_SugSku.Id = IdWorker.Build(IdType.NewGuid);
                    d_SugSku.ReportId = rop.ReportId;
                    d_SugSku.MerchId = merchId;
                    d_SugSku.SkuId = sugSku.Id;
                    d_SugSku.CreateTime = DateTime.Now;
                    d_SugSku.Creator = operater;
                    CurrentDb.SenvivHealthMonthReportSugSku.Add(d_SugSku);
                }
            }

            if (rop.IsSend)
            {
                string first = "您好，" + rpt.HealthDate + "月健康报告已生成，详情如下";
                string url = "http://health.17fanju.com/#/report/month/monitor?rptId=" + rpt.Id;
                string keyword1 = DateTime.Now.ToUnifiedFormatDateTime();
                string keyword2 = "总体评分" + rpt.TotalScore + "分";
                string remark = "感谢您的支持，如需查看详情报告信息请点击";
                var isSend = SdkFactory.Senviv.SendMonthReport(rpt.SvUserId, first, keyword1, keyword2, remark, url);
                if (isSend)
                {
                    rpt.IsSend = true;
                    rpt.Status = E_SenvivHealthReportStatus.SendSuccess;
                }
                else
                {
                    rpt.Status = E_SenvivHealthReportStatus.SendFailure;
                }

                CurrentDb.SaveChanges();

                if (isSend)
                {
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发送成功");
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发送失败");
                }
            }
            else
            {
                CurrentDb.SaveChanges();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;

        }

        public CustomJsonResult GetTagExplains(string operater, string merchId, RupSenvivGetTags rup)
        {

            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SenvivHealthTagExplain

                         select new { u.Id, u.TagId, u.TagName, u.ProExplain, u.TcmExplain, u.Suggest });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderBy(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    TagId = item.TagId,
                    TagName = item.TagName,
                    ProExplain = item.ProExplain,
                    TcmExplain = item.TcmExplain,
                    Suggest = item.Suggest
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;

        }

        public CustomJsonResult SaveTagExplain(string operater, string merchId, RopSenvivSaveTagExplain rop)
        {

            var result = new CustomJsonResult();

            var tagExplain = CurrentDb.SenvivHealthTagExplain.Where(m => m.Id == rop.Id).FirstOrDefault();
            if (tagExplain != null)
            {
                tagExplain.ProExplain = rop.ProExplain;
                tagExplain.TcmExplain = rop.TcmExplain;
                tagExplain.Suggest = rop.Suggest;
                CurrentDb.SaveChanges();
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;

        }

        public CustomJsonResult GetVisitRecords(string operater, string merchId, RupSenvivGetVisitRecords rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SenvivVisitRecord
                         where
                         u.MerchId == merchId
                         &&
                         u.SvUserId == rup.UserId
                         select new { u.Id, u.VisitType, u.VisitTemplate, u.VisitContent, u.VisitTime, u.NextTime, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.VisitTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string visitType = "";
                if (item.VisitType == E_SenvivVisitRecordVisitType.Callout)
                {
                    visitType = "电话回访";
                }
                else if (item.VisitType == E_SenvivVisitRecordVisitType.WxPa)
                {
                    visitType = "公众号告知";
                }

                List<object> arr_VisitContent = new List<object>();

                if (item.VisitTemplate == E_SenvivVisitRecordVisitTemplate.CalloutRecord)
                {
                    Dictionary<string, string> dic_Content = item.VisitContent.ToJsonObject<Dictionary<string, string>>();

                    string remark = "";
                    if (dic_Content.ContainsKey("remark"))
                    {
                        remark = dic_Content["remark"].ToString();
                    }

                    arr_VisitContent.Add(new { key = "回访记录", value = remark });
                }
                else if (item.VisitTemplate == E_SenvivVisitRecordVisitTemplate.WxPaByHealthMonitor)
                {
                    Dictionary<string, string> dic_Content = item.VisitContent.ToJsonObject<Dictionary<string, string>>();

                    string remark = "";
                    if (dic_Content.ContainsKey("remark"))
                    {
                        remark = dic_Content["remark"].ToString();
                    }

                    string keyword1 = "";
                    if (dic_Content.ContainsKey("keyword1"))
                    {
                        keyword1 = dic_Content["keyword1"].ToString();
                    }

                    string keyword2 = "";
                    if (dic_Content.ContainsKey("keyword2"))
                    {
                        keyword2 = dic_Content["keyword2"].ToString();
                    }

                    string keyword3 = "";
                    if (dic_Content.ContainsKey("keyword3"))
                    {
                        keyword3 = dic_Content["keyword3"].ToString();
                    }

                    arr_VisitContent.Add(new { key = "异常结果", value = keyword1 });
                    arr_VisitContent.Add(new { key = "风险因素", value = keyword2 });
                    arr_VisitContent.Add(new { key = "健康建议", value = keyword3 });
                    arr_VisitContent.Add(new { key = "备注", value = remark });
                }


                string s_Operater = "";
                var d_Operater = CurrentDb.SysMerchUser.Where(m => m.Id == operater).FirstOrDefault();
                if (d_Operater != null)
                {
                    s_Operater = d_Operater.FullName;
                }

                olist.Add(new
                {
                    Id = item.Id,
                    VisitType = visitType,
                    VisitContent = arr_VisitContent,
                    VisitTime = item.VisitTime.ToUnifiedFormatDateTime(),
                    NextTime = item.NextTime.ToUnifiedFormatDateTime(),
                    CreateTime = item.CreateTime,
                    Operater = s_Operater,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult SaveVisitRecordByTelePhone(string operater, string merchId, RopSenvivSaveVisitRecordByTelePhone rop)
        {
            var result = new CustomJsonResult();


            var d_SenvivVisitRecord = new SenvivVisitRecord();
            d_SenvivVisitRecord.Id = IdWorker.Build(IdType.NewGuid);
            d_SenvivVisitRecord.MerchId = merchId;
            d_SenvivVisitRecord.SvUserId = rop.UserId;
            d_SenvivVisitRecord.VisitType = E_SenvivVisitRecordVisitType.Callout;
            d_SenvivVisitRecord.VisitTemplate = E_SenvivVisitRecordVisitTemplate.CalloutRecord;
            d_SenvivVisitRecord.VisitContent = rop.VisitContent.ToJsonString();
            d_SenvivVisitRecord.VisitTime = CommonUtil.ConverToDateTime(rop.VisitTime).Value;
            d_SenvivVisitRecord.NextTime = CommonUtil.ConverToDateTime(rop.NextTime);
            d_SenvivVisitRecord.Creator = operater;
            d_SenvivVisitRecord.CreateTime = DateTime.Now;
            CurrentDb.SenvivVisitRecord.Add(d_SenvivVisitRecord);
            CurrentDb.SaveChanges();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

        public CustomJsonResult SaveVisitRecordByPapush(string operater, string merchId, RopSenvivSaveVisitRecordByPapush rop)
        {
            var result = new CustomJsonResult();

            var d_SenvivVisitRecord = new SenvivVisitRecord();
            d_SenvivVisitRecord.Id = IdWorker.Build(IdType.NewGuid);
            d_SenvivVisitRecord.MerchId = merchId;
            d_SenvivVisitRecord.SvUserId = rop.UserId;
            d_SenvivVisitRecord.VisitType = E_SenvivVisitRecordVisitType.WxPa;
            d_SenvivVisitRecord.VisitTemplate = rop.VisitTemplate;

            //string str_Content = "";
            //if(rop.VisitTemplate== E_SenvivVisitRecordVisitTemplate.WxPaByHealthException)
            //{
            //    Dictionary<string, string> dic_Content = rop.Content.ToJsonObject<>();

            //    str_Content += "检测异常提醒：\n";
            //    str_Content += "异常结果："+ dic_Content[""]+ "\n";
            //    str_Content += "风险因素：" + dic_Content[""] + "\n";
            //    str_Content += "健康建议：" + dic_Content[""] + "\n";
            //    str_Content += "备注：" + dic_Content[""] + "\n";
            //}

            d_SenvivVisitRecord.VisitContent = rop.VisitContent.ToJsonString();
            d_SenvivVisitRecord.VisitTime = DateTime.Now;
            d_SenvivVisitRecord.Creator = operater;
            d_SenvivVisitRecord.CreateTime = DateTime.Now;
            CurrentDb.SenvivVisitRecord.Add(d_SenvivVisitRecord);
            CurrentDb.SaveChanges();

            if (rop.VisitTemplate == E_SenvivVisitRecordVisitTemplate.WxPaByHealthMonitor)
            {
                var dic = rop.VisitContent.ToJsonObject<Dictionary<string, string>>();
                string first = "亲，您近期的监测结果显示异常";
                string keyword1 = dic["keyword1"];
                string keyword2 = dic["keyword2"];
                string keyword3 = dic["keyword3"];
                string remark = dic["remark"];
                string url = "";
                SdkFactory.Senviv.SendHealthMonitor(rop.UserId, first, keyword1, keyword2, keyword3, remark);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }


        public CustomJsonResult GetTasks(string operater, string merchId, RupSenvivGetUsers rup)
        {
            var result = new CustomJsonResult();


            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var query = (from u in CurrentDb.SenvivTask
                         where
                         merchIds.Contains(u.MerchId)
                         select new { u.Id, u.TaskType, u.Title, u.Status, u.CreateTime, u.Handler, u.HandleTime });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {



                olist.Add(new
                {
                    Id = item.Id,
                    TaskType = item.TaskType,
                    Title = item.Title,
                    Status = GetTaskStatus(item.Status),
                    CreateTime = item.CreateTime
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public StatusModel GetTaskStatus(E_SenvivTaskStatus status)
        {
            var statusModel = new StatusModel();

            switch (status)
            {
                case E_SenvivTaskStatus.WaitHandle:
                    statusModel = new StatusModel(1, "待处理");
                    break;
                case E_SenvivTaskStatus.Handling:
                    statusModel = new StatusModel(2, "处理中");
                    break;
                case E_SenvivTaskStatus.Handled:
                    statusModel = new StatusModel(3, "已处理");
                    break;
            }

            return statusModel;
        }

    }
}
