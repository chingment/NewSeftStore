using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
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

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();


            if (string.IsNullOrEmpty(d_Merch.SenvivDepts))
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new PageEntity());

            var deptIds = d_Merch.SenvivDepts.Split(',');

            var query = (from u in CurrentDb.SenvivUser
                         where
                         deptIds.Contains(u.DeptId)
                         && ((rup.Name == null || u.Nick.Contains(rup.Name)) ||
                         (rup.Name == null || u.Account.Contains(rup.Name)))
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

                string signName = item.Nick;

                if (!string.IsNullOrEmpty(item.Account) && item.Nick != item.Account)
                {
                    signName += "(" + item.Account + ")";
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
                    HeadImgurl = item.HeadImgurl,
                    SAS = SvUtil.GetSASName(item.SAS),
                    BreathingMachine = SvUtil.GetBreathingMachineName(item.BreathingMachine),
                    SignTags = SvUtil.GetSignTags(item.Perplex, item.OtherPerplex),
                    Medicalhistory = SvUtil.GetMedicalhistoryName(item.Medicalhistory),
                    Medicine = SvUtil.GetMedicineNames(item.Medicine),
                    Sex = SvUtil.GetSexName(item.Sex),
                    Age = age,
                    Height = item.Height,
                    Weight = item.Weight,
                    Mobile = item.Mobile,
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
                                select new { u.Id, u.Nick, u.HeadImgurl, u.Birthday, u.SAS, u.Perplex, u.Height, u.Weight, u.Medicalhistory, u.Medicine, u.OtherPerplex, u.BreathingMachine, u.Account, u.Sex, u.Mobile, u.LastReportId, u.LastReportTime, u.CreateTime }).FirstOrDefault();

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
                SignName = SvUtil.GetSignName(d_SenvivUser.Nick, d_SenvivUser.Account),
                Age = SvUtil.GetAge(d_SenvivUser.Birthday),
                HeadImgurl = d_SenvivUser.HeadImgurl,
                SAS = SvUtil.GetSASName(d_SenvivUser.SAS),
                BreathingMachine = SvUtil.GetBreathingMachineName(d_SenvivUser.BreathingMachine),
                SignTags = SvUtil.GetSignTags(d_SenvivUser.Perplex, d_SenvivUser.OtherPerplex),
                Medicalhistory = SvUtil.GetMedicalhistoryName(d_SenvivUser.Medicalhistory),
                Medicine = SvUtil.GetMedicineNames(d_SenvivUser.Medicine),
                Sex = SvUtil.GetSexName(d_SenvivUser.Sex),
                Height = d_SenvivUser.Height,
                Weight = d_SenvivUser.Weight,
                Mobile = d_SenvivUser.Mobile,
                LastReportId = d_SenvivUser.LastReportId,
                LastReportTime = d_SenvivUser.LastReportTime
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetDayReports(string operater, string merchId, RupSenvivGetDayReports rup)
        {
            var result = new CustomJsonResult();

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();


            if (string.IsNullOrEmpty(d_Merch.SenvivDepts))
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new PageEntity());

            var deptIds = d_Merch.SenvivDepts.Split(',');

            var query = (from u in CurrentDb.SenvivHealthDayReport

                         join s in CurrentDb.SenvivUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.IsValid == true
                         select new
                         {
                             u.Id,
                             tt.Nick,
                             tt.Sex,
                             u.SvUserId,
                             tt.Account,
                             tt.Birthday,
                             tt.HeadImgurl,
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
                query = query.Where(m => ((rup.Name == null || m.Nick.Contains(rup.Name)) || (rup.Name == null || m.Account.Contains(rup.Name))));
            }

            if (rup.HealthDate != null && rup.HealthDate.Length == 2)
            {

                DateTime? startTime = CommonUtil.ConverToStartTime(rup.HealthDate[0]);
                DateTime? endTime = CommonUtil.ConverToEndTime(rup.HealthDate[1]);

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
                    SignName = SvUtil.GetSignName(rpt.Nick, rpt.Account),
                    HeadImgurl = rpt.HeadImgurl,
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
                             tt.Nick,
                             tt.Sex,
                             tt.Account,
                             tt.Birthday,
                             tt.HeadImgurl,
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
                    SignName = SvUtil.GetSignName(d_Rpt.Nick, d_Rpt.Account),
                    HeadImgurl = d_Rpt.HeadImgurl,
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

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();


            if (string.IsNullOrEmpty(d_Merch.SenvivDepts))
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new PageEntity());

            var deptIds = d_Merch.SenvivDepts.Split(',');

            var query = (from u in CurrentDb.SenvivHealthMonthReport

                         join s in CurrentDb.SenvivUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()

                         select new
                         {
                             u.Id,
                             tt.Nick,
                             tt.Sex,
                             tt.Account,
                             tt.Birthday,
                             tt.HeadImgurl,
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
                query = query.Where(m => ((rup.Name == null || m.Nick.Contains(rup.Name)) || (rup.Name == null || m.Account.Contains(rup.Name))));
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
                    SignName = SvUtil.GetSignName(rpt.Nick, rpt.Account),
                    HeadImgurl = rpt.HeadImgurl,
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
                           tt.Nick,
                           tt.Sex,
                           tt.Account,
                           tt.Birthday,
                           tt.HeadImgurl,
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
                    SignName = SvUtil.GetSignName(rpt.Nick, rpt.Account),
                    HeadImgurl = rpt.HeadImgurl,
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

            var ret = new
            {
                rpt.RptSummary,
                rpt.RptSuggest,
                rpt.IsSend
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

            if (rop.IsSend)
            {

                var isSend = SdkFactory.Senviv.SendMonthReport(rop.ReportId);
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
    }
}
