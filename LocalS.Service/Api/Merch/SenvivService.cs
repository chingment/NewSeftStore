using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using LocalS.BLL.UI;
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
        public FieldModel GetReportStatus(E_SvHealthReportStatus status)
        {
            var statusModel = new FieldModel();

            switch (status)
            {
                case E_SvHealthReportStatus.WaitBuild:
                    statusModel = new FieldModel(1, "待生成");
                    break;
                case E_SvHealthReportStatus.Building:
                    statusModel = new FieldModel(2, "生成中");
                    break;
                case E_SvHealthReportStatus.BuildSuccess:
                    statusModel = new FieldModel(3, "已生成");
                    break;
                case E_SvHealthReportStatus.BuildFailure:
                    statusModel = new FieldModel(4, "生成失败");
                    break;
                case E_SvHealthReportStatus.WaitSend:
                    statusModel = new FieldModel(5, "待评价");
                    break;
                case E_SvHealthReportStatus.Sending:
                    statusModel = new FieldModel(6, "发送中");
                    break;
                case E_SvHealthReportStatus.SendSuccess:
                    statusModel = new FieldModel(7, "已发送");
                    break;
                case E_SvHealthReportStatus.SendFailure:
                    statusModel = new FieldModel(8, "发送失败");
                    break;
            }

            return statusModel;
        }

        public List<EleTag> GetSignTags(SvUser user)
        {
            List<EleTag> signTags = new List<EleTag>();

            if (user.CareMode == E_SvUserCareMode.Normal)
            {
                signTags = SvUtil.GetSignTags(user.Perplex, user.PerplexOt);
            }
            else if (user.CareMode == E_SvUserCareMode.Pregnancy)
            {
                var d_Women = CurrentDb.SvUserWomen.Where(m => m.SvUserId == user.Id).FirstOrDefault();
                if (d_Women != null)
                {
                    if (d_Women.PregnancyTime != null)
                    {
                        var week = Lumos.CommonUtil.GetDiffWeekDay(d_Women.PregnancyTime.Value, DateTime.Now);

                        signTags.Add(new EleTag(string.Format("{0}周+{1}", week.Week, week.Day), ""));
                    }
                }

                //if (d_Gravida.DeliveryWay == SenvivUserGravidaDeliveryWay.NaturalLabour)
                //{
                //    signTags.Add(new EleTag("自然顺产", ""));
                //}
                //else if (d_Gravida.DeliveryWay == SenvivUserGravidaDeliveryWay.Cesarean)
                //{
                //    signTags.Add(new EleTag("剖腹产", ""));
                //}

            }

            return signTags;
        }

        public CustomJsonResult GetUsers(string operater, string merchId, RupSenvivGetUsers rup)
        {
            var result = new CustomJsonResult();

            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var query = (from u in CurrentDb.SvUser
                         where
                         merchIds.Contains(u.MerchId) &&
                         u.DeviceCount > 0
                         && (rup.Name == null || u.FullName.Contains(rup.Name))
                         select u);

            if (rup.Sas != "0")
            {
                query = query.Where(m => m.Sas == rup.Sas);
            }

            if (rup.Chronic != "0")
            {
                var pred = PredicateExtensionses.False<SvUser>();
                pred = pred.Or(m => m.Perplex.Contains(rup.Chronic));
                query = query.Where(pred);
            }


            if (rup.Perplex != "0")
            {
                var pred = PredicateExtensionses.False<SvUser>();
                pred = pred.Or(m => m.Perplex.Contains(rup.Perplex));
                query = query.Where(pred);
            }

            if (rup.CareLevel != E_SvUserCareLevel.None)
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
                olist.Add(new
                {
                    Id = item.Id,
                    SignName = SvUtil.GetSignName("", item.FullName),
                    Avatar = item.Avatar,
                    SignTags = GetSignTags(item),
                    Sex = new FieldModel(item.Sex, SvUtil.GetSexName(item.Sex)),
                    Age = SvUtil.GetAge(item.Birthday),
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

        public CustomJsonResult GetUserDetail(string operater, string merchId, string svUserId)
        {
            var result = new CustomJsonResult();


            var d_SvUser = (from u in CurrentDb.SvUser
                            where u.Id == svUserId
                            select u).FirstOrDefault();


            object pregnancy = null;

            if (d_SvUser.CareMode == E_SvUserCareMode.Pregnancy)
            {
                var d_Women = CurrentDb.SvUserWomen.Where(m => m.SvUserId == d_SvUser.Id).FirstOrDefault();
                if (d_Women != null)
                {
                    if (d_Women.PregnancyTime != null)
                    {
                        var ges = Lumos.CommonUtil.GetDiffWeekDay(d_Women.PregnancyTime.Value, DateTime.Now);

                        pregnancy = new
                        {
                            GesWeek = ges.Week,
                            GesDay = ges.Day,
                            DeliveryTime = d_Women.DeliveryTime.ToUnifiedFormatDate()
                        };
                    }
                }
            }

            var ret = new
            {
                SvUserId = d_SvUser.Id,
                SignName = SvUtil.GetSignName("", d_SvUser.FullName),
                SignTags = GetSignTags(d_SvUser),
                Age = SvUtil.GetAge(d_SvUser.Birthday),
                Birthday = d_SvUser.Birthday.ToUnifiedFormatDate(),
                Height = d_SvUser.Height,
                Weight = d_SvUser.Weight,
                Avatar = d_SvUser.Avatar,
                FullName = d_SvUser.FullName,
                CareMode = GetCareMode(d_SvUser.CareMode),
                Sex = new FieldModel(d_SvUser.Sex, SvUtil.GetSexName(d_SvUser.Sex)),
                Sas = new FieldModel(d_SvUser.Sas, SvUtil.GetSasName(d_SvUser.Sas)),
                IsUseBreathMach = new FieldModel(d_SvUser.IsUseBreathMach, SvUtil.GetIsUseBreathMachName(d_SvUser.IsUseBreathMach)),
                MedicalHis = new FieldModel(d_SvUser.MedicalHis, SvUtil.GetMedicalHisNames(d_SvUser.MedicalHis, d_SvUser.MedicalHisOt)),
                Medicine = new FieldModel(d_SvUser.Medicine, SvUtil.GetMedicineNames(d_SvUser.Medicine, d_SvUser.MedicineOt)),
                PhoneNumber = d_SvUser.PhoneNumber,
                LastReportId = d_SvUser.LastReportId,
                LastReportTime = d_SvUser.LastReportTime,
                Pregnancy = pregnancy
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult SaveUserDetail(string operater, string merchId, RopSenvivSaveUserDetail rop)
        {
            var result = new CustomJsonResult();

            var d_User = CurrentDb.SvUser.Where(m => m.Id == rop.SvUserId).FirstOrDefault();

            if (d_User != null)
            {
                d_User.FullName = rop.FullName;
                d_User.Sex = rop.Sex;
                d_User.Birthday = rop.Birthday;
                d_User.Height = rop.Height;
                d_User.Weight = rop.Weight;
                d_User.CareMode = rop.CareMode;
                if (rop.Pregnancy != null)
                {
                    Dictionary<string, string> pregnancy = new Dictionary<string, string>();


                    var d_Women = CurrentDb.SvUserWomen.Where(m => m.SvUserId == d_User.Id).FirstOrDefault();
                    if (d_Women == null)
                    {

                    }
                    else
                    {
                        d_Women.PregnancyTime = Lumos.CommonUtil.GetWeekDay2Time(int.Parse(pregnancy["gesWeek"].ToString()), int.Parse(pregnancy["gesDay"].ToString()));
                        d_Women.DeliveryTime = DateTime.Parse(pregnancy["deliveryTime"]);
                    }
                }

                CurrentDb.SaveChanges();
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

        public CustomJsonResult GetDayReports(string operater, string merchId, RupSenvivGetDayReports rup)
        {
            var result = new CustomJsonResult();

            var merchIds = BizFactory.Merch.GetRelIds(merchId);


            var query = (from u in CurrentDb.SvHealthDayReport

                         join s in CurrentDb.SvUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.IsValid == true
                         &&
                         merchIds.Contains(tt.MerchId)
                         select new
                         {
                             u.Id,
                             tt.Sex,
                             u.SvUserId,
                             tt.FullName,
                             tt.Birthday,
                             tt.Avatar,
                             u.HealthScore,
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
                query = query.Where(m => (rup.Name == null || m.FullName.Contains(rup.Name)));
            }

            if (rup.HealthDate != null && rup.HealthDate.Length == 2)
            {

                DateTime? startTime = Lumos.CommonUtil.ConverToStartTime(rup.HealthDate[0]);
                DateTime? endTime = Lumos.CommonUtil.ConverToEndTime(rup.HealthDate[1]);

                query = query.Where(m => m.HealthDate >= startTime && m.HealthDate <= endTime);
            }

            if (!string.IsNullOrEmpty(rup.SvUserId))
            {
                query = query.Where(m => m.SvUserId == rup.SvUserId);
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
                    SignName = SvUtil.GetSignName("", rpt.FullName),
                    Avatar = rpt.Avatar,
                    Sex = new FieldModel(rpt.Sex, SvUtil.GetSexName(rpt.Sex)),
                    Age = SvUtil.GetAge(rpt.Birthday),
                    HealthDate = rpt.HealthDate.ToUnifiedFormatDate(),
                    HealthScore = rpt.HealthScore,
                    SmRssj = rpt.SmRssj.ToString("HH:mm:ss"),
                    SmQxsj = rpt.SmQxsj.ToString("HH:mm:ss"),
                    DsTags = rpt.SmTags.ToJsonObject<List<string>>(),
                    MylGrfx = SvUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvUtil.GetSmSmsc(rpt.SmSmsc, "1"),
                    //深度睡眠时长
                    SmSdsmsc = SvUtil.GetSmSdsmsc(rpt.SmSdsmsc, "1"),
                    //浅度睡眠时长
                    SmQdsmsc = SvUtil.GetSmQdsmsc(rpt.SmQdsmsc, "1"),
                    //REM睡眠时长
                    SmRemsmsc = SvUtil.GetSmRemsmsc(rpt.SmRemsmsc, "1"),
                    //睡眠周期=
                    SmSmzq = SvUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvUtil.GetSmTdcs(rpt.SmTdcs),
                    IsSend = rpt.IsSend,
                    Status = GetReportStatus(rpt.Status),
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;

        }

        public CustomJsonResult GetDayReportDetail(string operater, string merchId, string reportId, string taskId)
        {
            var result = new CustomJsonResult();

            var d_Task = CurrentDb.SvTask.Where(m => m.Id == taskId).FirstOrDefault();

            object task = null;

            if (d_Task != null)
            {
                task = new
                {
                    TaskId = d_Task.Id,
                    Title = d_Task.Title,
                    Status = d_Task.Status
                };
            }

            var d_Rpt = (from u in CurrentDb.SvHealthDayReport
                         join s in CurrentDb.SvUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.Id == reportId
                         select new
                         {
                             u.Id,
                             tt.Sex,
                             tt.FullName,
                             tt.Birthday,
                             tt.Avatar,
                             u.HealthScore,
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
                Task = task,
                UserInfo = new
                {
                    SvUserId = d_Rpt.SvUserId,
                    SignName = SvUtil.GetSignName("", d_Rpt.FullName),
                    Avatar = d_Rpt.Avatar,
                    Sex = new FieldModel(d_Rpt.Sex, SvUtil.GetSexName(d_Rpt.Sex)),
                    Age = SvUtil.GetAge(d_Rpt.Birthday)
                },
                ReportData = new
                {
                    HealthDate = d_Rpt.HealthDate.ToUnifiedFormatDate(),
                    HealthScore = d_Rpt.HealthScore,
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
                    JbfxXlscfx = SvUtil.GetJbfxXlscfx(d_Rpt.JbfxXlscfx),
                    JbfxXljsl = SvUtil.GetJbfxXljsl(d_Rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvUtil.GetHrvXzznl(d_Rpt.HrvXzznl),
                    //心脏总能量基准值
                    d_Rpt.HrvXzznljzz,
                    //交感神经张力指数
                    HrvJgsjzlzs = SvUtil.GetHrvJgsjzlzs(d_Rpt.HrvJgsjzlzs),
                    //交感神经张力指数基准值
                    d_Rpt.HrvJgsjzlzsjzz,
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvUtil.GetHrvMzsjzlzs(d_Rpt.HrvMzsjzlzs),
                    //迷走神经张力指数基准值
                    d_Rpt.HrvMzsjzlzsjzz,
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvUtil.GetHrvZzsjzlzs(d_Rpt.HrvZzsjzlzs),
                    //自主神经平衡指数基准值
                    d_Rpt.HrvZzsjzlzsjzz,
                    //荷尔蒙指数
                    HrvHermzs = SvUtil.GetHrvHermzs(d_Rpt.HrvHermzs),
                    //荷尔蒙指数基准值
                    d_Rpt.HrvHermzsjzz,
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvUtil.GetHrvTwjxgsszh(d_Rpt.HrvTwjxgsszs),
                    //体温及血管舒缩指数基准值
                    d_Rpt.HrvTwjxgsszhjzz,
                    //当次基准心率
                    XlDcjzxl = SvUtil.GetXlDcjzxl(d_Rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvUtil.GetXlCqjzxl(d_Rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvUtil.GetXlDcpjxl(d_Rpt.XlDcpjxl),
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
                    HxDcjzhx = SvUtil.GetHxDcjzhx(d_Rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvUtil.GetHxCqjzhx(d_Rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvUtil.GetHxDcpjhx(d_Rpt.HxDcpjhx),
                    //呼吸最高呼吸
                    d_Rpt.HxZghx,
                    //呼吸最低呼吸
                    d_Rpt.HxZdhx,
                    //呼吸过快时长
                    d_Rpt.HxGksc,
                    //呼吸过慢时长
                    d_Rpt.HxGmsc,
                    //呼吸暂停次数
                    HxZtcs = SvUtil.GetHxZtcs(d_Rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvUtil.GetHxZtahizs(d_Rpt.HxZtahizs),
                    //呼吸暂停平均时长
                    d_Rpt.HxZtpjsc,
                    d_Rpt.SmZcsc,
                    //睡眠时长
                    SmSmsc = SvUtil.GetSmSmsc(d_Rpt.SmSmsc, "1"),
                    //深度睡眠时长
                    SmSdsmsc = SvUtil.GetSmSdsmsc(d_Rpt.SmSdsmsc, "1"),
                    //浅度睡眠时长
                    SmQdsmsc = SvUtil.GetSmQdsmsc(d_Rpt.SmQdsmsc, "1"),
                    //REM睡眠时长
                    SmRemsmsc = SvUtil.GetSmRemsmsc(d_Rpt.SmRemsmsc, "1"),
                    //睡眠周期=
                    SmSmzq = SvUtil.GetSmSmzq(d_Rpt.SmSmzq),
                    //清醒时刻时长
                    d_Rpt.SmQxsksc,
                    //清醒时刻比例
                    d_Rpt.SmQxskbl,
                    //离真次数
                    d_Rpt.SmLzcs,
                    //离真时长
                    d_Rpt.SmLzsc,
                    //体动次数
                    SmTdcs = SvUtil.GetSmTdcs(d_Rpt.SmTdcs),
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

        public CustomJsonResult GetStageReports(string operater, string merchId, RupSenvivGetDayReports rup)
        {
            var result = new CustomJsonResult();

            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var query = (from u in CurrentDb.SvHealthStageReport
                         join s in CurrentDb.SvUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where
                         merchIds.Contains(tt.MerchId)
                         select new
                         {
                             u.Id,
                             tt.Sex,
                             tt.FullName,
                             tt.Birthday,
                             tt.Avatar,
                             u.HealthScore,
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
                             u.RptType,
                             u.RptStartTime,
                             u.RptEndTime,
                             u.CreateTime
                         });

            if (!string.IsNullOrEmpty(rup.RptType))
            {
                query = query.Where(m => m.RptType == rup.RptType);
            }

            if (!string.IsNullOrEmpty(rup.Name))
            {
                query = query.Where(m => (rup.Name == null || m.FullName.Contains(rup.Name)));
            }


            if (!string.IsNullOrEmpty(rup.SvUserId))
            {
                query = query.Where(m => m.SvUserId == rup.SvUserId);
            }

            if (rup.HealthDate != null && rup.HealthDate.Length == 2)
            {
                var d1 = CommonUtil.MonthMinDateTime(rup.HealthDate[0]);
                var d2 = CommonUtil.MonthMaxDateTime(rup.HealthDate[1]);

                LogUtil.Info("d1：" + d1.ToUnifiedFormatDateTime());
                LogUtil.Info("d2：" + d2.ToUnifiedFormatDateTime());
                query = query.Where(m => m.RptStartTime >= d1 && m.RptEndTime <= d2);

            }



            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.RptStartTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var rpt in list)
            {
                string healthDate = "";
                if (rpt.RptType == "per_month")
                {
                    if (rpt.RptStartTime.Value != null)
                    {
                        healthDate = rpt.RptStartTime.Value.ToString("yyyy-MM");
                    }
                }

                olist.Add(new
                {
                    Id = rpt.Id,
                    SvUserId = rpt.SvUserId,
                    SignName = SvUtil.GetSignName("", rpt.FullName),
                    Avatar = rpt.Avatar,
                    Sex = new FieldModel(rpt.Sex, SvUtil.GetSexName(rpt.Sex)),
                    Age = SvUtil.GetAge(rpt.Birthday),
                    rpt.HealthScore,
                    HealthDate = healthDate,
                    MylGrfx = SvUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvUtil.GetSmSmsc(rpt.SmSmsc, "1"),
                    //深度睡眠时长
                    SmSdsmsc = SvUtil.GetSmSdsmsc(rpt.SmSdsmsc, "1"),
                    //浅度睡眠时长
                    SmQdsmsc = SvUtil.GetSmQdsmsc(rpt.SmQdsmsc, "1"),
                    //REM睡眠时长
                    SmRemsmsc = SvUtil.GetSmRemsmsc(rpt.SmRemsmsc, "1"),
                    //睡眠周期=
                    SmSmzq = SvUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvUtil.GetSmTdcs(rpt.SmTdcs),
                    Status = GetReportStatus(rpt.Status),
                    IsBuild = rpt.IsBuild,
                    IsSend = rpt.IsSend
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;

        }

        public CustomJsonResult GetStageReportDetail(string operater, string merchId, string reportId, string taskId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SvHealthStageReport

                       join s in CurrentDb.SvUser on u.SvUserId equals s.Id into temp
                       from tt in temp.DefaultIfEmpty()
                       where u.Id == reportId
                       select new
                       {
                           u.Id,
                           tt.Sex,
                           tt.FullName,
                           tt.Birthday,
                           tt.Avatar,
                           u.SmTags,
                           u.HealthScore,
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
                    SvUserId = rpt.SvUserId,
                    SignName = SvUtil.GetSignName("", rpt.FullName),
                    Avatar = rpt.Avatar,
                    Sex = new FieldModel(rpt.Sex, SvUtil.GetSexName(rpt.Sex)),
                    Age = SvUtil.GetAge(rpt.Birthday)
                },
                ReportData = new
                {
                    rpt.HealthScore,
                    rpt.HealthDate,
                    SmTags = rpt.SmTags.ToJsonObject<List<object>>(),
                    MylGrfx = SvUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvUtil.GetSmSmsc(rpt.SmSmsc, "1"),
                    //深度睡眠时长
                    SmSdsmsc = SvUtil.GetSmSdsmsc(rpt.SmSdsmsc, "1"),
                    //浅度睡眠时长
                    SmQdsmsc = SvUtil.GetSmQdsmsc(rpt.SmQdsmsc, "1"),
                    //REM睡眠时长
                    SmRemsmsc = SvUtil.GetSmRemsmsc(rpt.SmRemsmsc, "1"),
                    //睡眠周期=
                    SmSmzq = SvUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvUtil.GetSmTdcs(rpt.SmTdcs),
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

        public CustomJsonResult GetStageReportSug(string operater, string merchId, string reportId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SvHealthStageReport
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

            var d_SugSkus = CurrentDb.SvHealthStageReportSugSku.Where(m => m.ReportId == reportId).ToList();

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

        public CustomJsonResult SaveStageReportSug(string operater, string merchId, SenvivSaveMonthReportSug rop)
        {

            var result = new CustomJsonResult();

            var rpt = CurrentDb.SvHealthStageReport.Where(m => m.Id == rop.ReportId).FirstOrDefault();

            rpt.RptSummary = rop.RptSummary;
            rpt.RptSuggest = rop.RptSuggest;

            var d_SugSkus = CurrentDb.SvHealthStageReportSugSku.Where(m => m.ReportId == rop.ReportId).ToList();

            foreach (var d_SugSku in d_SugSkus)
            {
                CurrentDb.SvHealthStageReportSugSku.Remove(d_SugSku);
            }

            if (rop.SugSkus != null)
            {
                foreach (var sugSku in rop.SugSkus)
                {
                    var d_SugSku = new SvHealthStageReportSugSku();
                    d_SugSku.Id = IdWorker.Build(IdType.NewGuid);
                    d_SugSku.ReportId = rop.ReportId;
                    d_SugSku.MerchId = merchId;
                    d_SugSku.SkuId = sugSku.Id;
                    d_SugSku.CreateTime = DateTime.Now;
                    d_SugSku.Creator = operater;
                    CurrentDb.SvHealthStageReportSugSku.Add(d_SugSku);
                }
            }

            if (rop.IsSend)
            {
                string first = "您好，" + rpt.HealthDate + "月健康报告已生成，详情如下";
                string url = "http://health.17fanju.com/report/month/monitor?rptId=" + rpt.Id;
                string keyword1 = DateTime.Now.ToUnifiedFormatDateTime();
                string keyword2 = "总体评分" + rpt.HealthScore + "分";
                string remark = "感谢您的支持，如需查看详情报告信息请点击";
                var isSend = BizFactory.Senviv.SendMonthReport(rpt.SvUserId, first, keyword1, keyword2, remark, url);
                if (isSend)
                {
                    rpt.IsSend = true;
                    rpt.Status = E_SvHealthReportStatus.SendSuccess;
                }
                else
                {
                    rpt.Status = E_SvHealthReportStatus.SendFailure;
                }

                CurrentDb.SaveChanges();

                if (isSend)
                {
                    SignTaskStatus(operater, rop.TaskId, E_SvTaskStatus.Handled);

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

        public CustomJsonResult GetDayReportSug(string operater, string merchId, string reportId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SvHealthDayReport
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

        public CustomJsonResult SaveDayReportSug(string operater, string merchId, SenvivSaveMonthReportSug rop)
        {
            var result = new CustomJsonResult();

            if (!rop.IsSend)
            {
                var d_DayReport = CurrentDb.SvHealthDayReport.Where(m => m.Id == rop.ReportId).FirstOrDefault();
                d_DayReport.RptSummary = rop.RptSummary;
                d_DayReport.RptSuggest = rop.RptSuggest;
                CurrentDb.SaveChanges();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }
            else
            {
                var isSend = BizFactory.Senviv.SendDayReport(rop.ReportId, rop.RptSummary, rop.RptSuggest);
                if (isSend)
                {
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发送成功");
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发送失败");
                }
            }

            return result;

        }

        public CustomJsonResult GetTagExplains(string operater, string merchId, RupSenvivGetTags rup)
        {

            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SvHealthTagExplain
                         where u.MerchId == merchId
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

            var tagExplain = CurrentDb.SvHealthTagExplain.Where(m => m.Id == rop.Id).FirstOrDefault();
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

        public CustomJsonResult GetImitateLyingIns(string operater, string merchId, RupSenvivGetImitateLyingIns rup)
        {

            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SvImitateLyingIn
                         where u.MerchId == merchId
                         select new { u.Id, u.Title, u.Content, u.StartTimeQt, u.EndTimeQt, u.CreateTime });


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
                    Title = item.Title,
                    Content = item.Content,
                    StartTimeQt = item.StartTimeQt.ToString("HH:mm:ss"),
                    EndTimeQt = item.EndTimeQt.ToString("HH:mm:ss"),
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;

        }

        public CustomJsonResult SaveImitateLyingIn(string operater, string merchId, RopSenvivSaveImitateLyingIn rop)
        {

            var result = new CustomJsonResult();

            SvImitateLyingIn d_SvImitateLyingIn;


            string endTimeQt = rop.EndTimeQt == "24:00" ? "23:59:59" : rop.EndTimeQt;

            if (string.IsNullOrEmpty(rop.Id))
            {
                d_SvImitateLyingIn = new SvImitateLyingIn();
                d_SvImitateLyingIn.Id = IdWorker.Build(IdType.NewGuid);
                d_SvImitateLyingIn.MerchId = merchId;
                d_SvImitateLyingIn.Title = rop.Title;
                d_SvImitateLyingIn.Content = rop.Content;
                d_SvImitateLyingIn.Title = rop.Title;
                d_SvImitateLyingIn.StartTimeQt = DateTime.Parse("2020-01-01 " + rop.StartTimeQt);
                d_SvImitateLyingIn.EndTimeQt = DateTime.Parse("2020-01-01 " + endTimeQt);
                d_SvImitateLyingIn.CreateTime = DateTime.Now;
                d_SvImitateLyingIn.Creator = operater;

                CurrentDb.SvImitateLyingIn.Add(d_SvImitateLyingIn);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_SvImitateLyingIn = CurrentDb.SvImitateLyingIn.Where(m => m.Id == rop.Id).FirstOrDefault();
                d_SvImitateLyingIn.Title = rop.Title;
                d_SvImitateLyingIn.Content = rop.Content;
                d_SvImitateLyingIn.Title = rop.Title;
                d_SvImitateLyingIn.StartTimeQt = DateTime.Parse("2020-01-01 " + rop.StartTimeQt);
                d_SvImitateLyingIn.EndTimeQt = DateTime.Parse("2020-01-01 " + endTimeQt);
                d_SvImitateLyingIn.MendTime = DateTime.Now;
                d_SvImitateLyingIn.Mender = operater;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;

        }

        public CustomJsonResult GetVisitRecords(string operater, string merchId, RupSenvivGetVisitRecords rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SvVisitRecord
                         select new { u.Id, u.SvUserId, u.VisitType, u.TaskId, u.ReportId, u.VisitTemplate, u.VisitContent, u.VisitTime, u.NextTime, u.CreateTime });

            if (!string.IsNullOrEmpty(rup.SvUserId))
            {
                query = query.Where(m => m.SvUserId == rup.SvUserId);
            }

            if (!string.IsNullOrEmpty(rup.TaskId))
            {
                query = query.Where(m => m.TaskId == rup.TaskId);
            }

            if (!string.IsNullOrEmpty(rup.ReportId))
            {
                query = query.Where(m => m.ReportId == rup.ReportId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.VisitTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string visitType = "";
                if (item.VisitType == E_SvVisitRecordVisitType.Callout)
                {
                    visitType = "电话回访";
                }
                else if (item.VisitType == E_SvVisitRecordVisitType.WxPa)
                {
                    visitType = "公众号告知";
                }

                List<object> arr_VisitContent = new List<object>();

                if (item.VisitTemplate == E_SvVisitRecordVisitTemplate.CalloutRecord)
                {
                    Dictionary<string, string> dic_Content = item.VisitContent.ToJsonObject<Dictionary<string, string>>();

                    string remark = "";
                    if (dic_Content.ContainsKey("remark"))
                    {
                        remark = dic_Content["remark"].ToString();
                    }

                    arr_VisitContent.Add(new { key = "回访记录", value = remark });
                }
                else if (item.VisitTemplate == E_SvVisitRecordVisitTemplate.WxPaByHealthMonitor)
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


            var d_SvVisitRecord = new SvVisitRecord();
            d_SvVisitRecord.Id = IdWorker.Build(IdType.NewGuid);
            d_SvVisitRecord.SvUserId = rop.SvUserId;
            d_SvVisitRecord.ReportId = rop.ReportId;
            d_SvVisitRecord.TaskId = rop.TaskId;
            d_SvVisitRecord.VisitType = E_SvVisitRecordVisitType.Callout;
            d_SvVisitRecord.VisitTemplate = E_SvVisitRecordVisitTemplate.CalloutRecord;
            d_SvVisitRecord.VisitContent = rop.VisitContent.ToJsonString();
            d_SvVisitRecord.VisitTime = CommonUtil.ConverToDateTime(rop.VisitTime).Value;
            d_SvVisitRecord.NextTime = CommonUtil.ConverToDateTime(rop.NextTime);
            d_SvVisitRecord.Creator = operater;
            d_SvVisitRecord.CreateTime = DateTime.Now;
            CurrentDb.SvVisitRecord.Add(d_SvVisitRecord);
            CurrentDb.SaveChanges();

            SignTaskStatus(operater, rop.TaskId, E_SvTaskStatus.Handled);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

        public CustomJsonResult SaveVisitRecordByPapush(string operater, string merchId, RopSenvivSaveVisitRecordByPapush rop)
        {
            var result = new CustomJsonResult();

            var d_SvVisitRecord = new SvVisitRecord();
            d_SvVisitRecord.Id = IdWorker.Build(IdType.NewGuid);
            d_SvVisitRecord.SvUserId = rop.SvUserId;
            d_SvVisitRecord.ReportId = rop.ReportId;
            d_SvVisitRecord.TaskId = rop.TaskId;
            d_SvVisitRecord.VisitType = E_SvVisitRecordVisitType.WxPa;
            d_SvVisitRecord.VisitTemplate = rop.VisitTemplate;

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

            d_SvVisitRecord.VisitContent = rop.VisitContent.ToJsonString();
            d_SvVisitRecord.VisitTime = DateTime.Now;
            d_SvVisitRecord.Creator = operater;
            d_SvVisitRecord.CreateTime = DateTime.Now;
            CurrentDb.SvVisitRecord.Add(d_SvVisitRecord);
            CurrentDb.SaveChanges();

            if (rop.VisitTemplate == E_SvVisitRecordVisitTemplate.WxPaByHealthMonitor)
            {
                var dic = rop.VisitContent.ToJsonObject<Dictionary<string, string>>();
                string first = "亲，您近期的监测结果显示异常";
                string keyword1 = dic.ContainsKey("keyword1") ? dic["keyword1"] : "";
                string keyword2 = dic.ContainsKey("keyword2") ? dic["keyword2"] : "";
                string keyword3 = dic.ContainsKey("keyword3") ? dic["keyword3"] : "";
                string remark = dic.ContainsKey("remark") ? dic["remark"] : "";
                string url = "";
                BizFactory.Senviv.SendHealthMonitor(rop.SvUserId, first, keyword1, keyword2, keyword3, remark);
            }

            SignTaskStatus(operater, rop.TaskId, E_SvTaskStatus.Handled);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

        public CustomJsonResult GetTasks(string operater, string merchId, RupSenvivGetTasks rup)
        {
            var result = new CustomJsonResult();


            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var query = (from u in CurrentDb.SvTask
                         join s in CurrentDb.SvUser on u.SvUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where
                         merchIds.Contains(tt.MerchId)
                         select new { u.Id, u.ReportId, u.TaskType, u.Params, u.Title, u.Status, u.CreateTime, u.Handler, u.HandleTime });

            var waitHandle = query.Where(m => m.Status == E_SvTaskStatus.WaitHandle || m.Status == E_SvTaskStatus.Handling).Count();
            var handled = query.Where(m => m.Status == E_SvTaskStatus.Handled).Count();


            if (rup.Status == 0)
            {
                query = query.Where(m => m.Status == E_SvTaskStatus.Handling || m.Status == E_SvTaskStatus.WaitHandle);
            }
            else if (rup.Status == 1)
            {
                query = query.Where(m => m.Status == E_SvTaskStatus.Handled);
            }


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
                    TaskType = GetTaskType(item.TaskType),
                    Title = item.Title,
                    Status = GetTaskStatus(item.Status),
                    ReportId = item.ReportId,
                    Params = item.Params.ToJsonObject<Dictionary<string, string>>(),
                    CreateTime = item.CreateTime
                });
            }


            var pageEntity = new
            {
                PageSize = pageSize,
                Total = total,
                Items = olist,
                Count = new
                {
                    WaitHandle = waitHandle,
                    Handled = handled
                }
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult SignTaskStatus(string operater, string taskId, E_SvTaskStatus status)
        {
            var result = new CustomJsonResult();

            var d_Task = CurrentDb.SvTask.Where(m => m.Id == taskId).FirstOrDefault();
            if (d_Task != null)
            {
                d_Task.Status = status;
                d_Task.Mender = operater;
                d_Task.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

        public FieldModel GetTaskStatus(E_SvTaskStatus status)
        {
            var statusModel = new FieldModel();

            switch (status)
            {
                case E_SvTaskStatus.WaitHandle:
                    statusModel = new FieldModel(1, "待处理");
                    break;
                case E_SvTaskStatus.Handling:
                    statusModel = new FieldModel(2, "处理中");
                    break;
                case E_SvTaskStatus.Handled:
                    statusModel = new FieldModel(3, "已处理");
                    break;
            }

            return statusModel;
        }

        public FieldModel GetTaskType(E_SvTaskType type)
        {
            var statusModel = new FieldModel();

            switch (type)
            {
                case E_SvTaskType.Health_Monitor_FisrtDay:
                    statusModel = new FieldModel(1, "首次回访");
                    break;
                case E_SvTaskType.Health_Monitor_SeventhDay:
                    statusModel = new FieldModel(2, "一周回访");
                    break;
                case E_SvTaskType.Health_Monitor_FourteenthDay:
                    statusModel = new FieldModel(3, "两周回访");
                    break;
                case E_SvTaskType.Health_Monitor_PerMonth:
                    statusModel = new FieldModel(5, "每月回访");
                    break;
            }

            return statusModel;
        }

        public FieldModel GetCareMode(E_SvUserCareMode mode)
        {
            var statusModel = new FieldModel();

            switch (mode)
            {
                case E_SvUserCareMode.Normal:
                    statusModel = new FieldModel(1, "正常模式");
                    break;
                case E_SvUserCareMode.PrePregnancy:
                    statusModel = new FieldModel(24, "备孕中");
                    break;
                case E_SvUserCareMode.Pregnancy:
                    statusModel = new FieldModel(25, "怀孕中");
                    break;
                case E_SvUserCareMode.Postpartum:
                    statusModel = new FieldModel(26, "产后");
                    break;
            }

            return statusModel;
        }

        public CustomJsonResult GetArticles(string operater, string merchId, RupSenvivGetTasks rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SvArticle
                         where u.MerchId == merchId
                         select new { u.Id, u.Title, u.Tags, u.CreateTime, u.Creator });

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
                    Title = item.Title,
                    Tags = item.Tags,
                    CreateTime = item.CreateTime
                });
            }


            var pageEntity = new
            {
                PageSize = pageSize,
                Total = total,
                Items = olist
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetArticle(string operater, string merchId, string articleId)
        {
            var result = new CustomJsonResult();

            var d_SvArticle = CurrentDb.SvArticle.Where(m => m.Id == articleId).FirstOrDefault();

            var ret = new
            {

                Id = d_SvArticle.Id,
                Title = d_SvArticle.Title,
                Tags = string.IsNullOrEmpty(d_SvArticle.Tags) == true ? null : d_SvArticle.Tags.Split(new char[] { ',' }),
                Content = d_SvArticle.Content
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult SaveArticle(string operater, string merchId, RopSenvivSaveArticle rop)
        {
            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.Title))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "标题不能为空");

            if (string.IsNullOrEmpty(rop.Content))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "内容不能为空");


            if (string.IsNullOrEmpty(rop.Id))
            {
                var d_SvArticle = new SvArticle();
                d_SvArticle.Id = IdWorker.Build(IdType.NewGuid);
                d_SvArticle.MerchId = merchId;
                d_SvArticle.Title = rop.Title;
                d_SvArticle.Tags = rop.Tags == null ? null : string.Join(",", rop.Tags.ToArray());
                d_SvArticle.Content = rop.Content;
                d_SvArticle.CreateTime = DateTime.Now;
                d_SvArticle.Creator = operater;
                CurrentDb.SvArticle.Add(d_SvArticle);
                CurrentDb.SaveChanges();
            }
            else
            {
                var d_SvArticle = CurrentDb.SvArticle.Where(m => m.Id == rop.Id).FirstOrDefault();
                d_SvArticle.Title = rop.Title;
                d_SvArticle.Tags = rop.Tags == null ? null : string.Join(",", rop.Tags.ToArray());
                d_SvArticle.Content = rop.Content;
                d_SvArticle.MendTime = DateTime.Now;
                d_SvArticle.Mender = operater;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

    }
}
