using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class SenvivService : BaseService
    {
        //性别
        public string GetSexName(string value)
        {
            switch (value)
            {
                case "1":
                    return "男";
                case "2":
                    return "女";
                default:
                    return "";
            }
        }

        //呼吸暂停综合证
        public string GetSASName(string value)
        {
            switch (value)
            {
                case "1":
                    return "轻度";
                case "2":
                    return "中度";
                case "3":
                    return "重度";
                case "4":
                    return "无";
                default:
                    return "";
            }
        }

        //呼吸机
        public string GetBreathingMachineName(string value)
        {
            switch (value)
            {
                case "1":
                    return "是";
                case "2":
                    return "否";
                default:
                    return "";
            }
        }

        //目前困扰
        public string GetPerplexName(string value)
        {
            switch (value)
            {
                case "1":
                    return "没有困扰";
                case "2":
                    return "睡眠呼吸暂停综合症";
                case "3":
                    return "打鼾";
                case "4":
                    return "糖尿病";
                case "5":
                    return "高血压";
                case "6":
                    return "冠心病";
                case "7":
                    return "心脏病";
                case "8":
                    return "心衰";
                case "9":
                    return "慢性阻塞性肺疾病";
                case "10":
                    return "脑梗塞/脑卒中";
                case "11":
                    return "长期失眠";
                case "12":
                    return "癫痫";
                case "13":
                    return "不宁腿综合症";
                case "14":
                    return "其它";
                default:
                    return "";
            }
        }


        public List<EleTag> GetSignTags(string perplex, string otherPerplex)
        {
            var tags = new List<EleTag>();

            if (!string.IsNullOrEmpty(perplex))
            {
                string[] arrs = perplex.Split(',');

                foreach (var val in arrs)
                {
                    var name = GetPerplexName(val);
                    if (!string.IsNullOrEmpty(name) && name != "其它")
                    {
                        tags.Add(new EleTag(name, ""));
                    }
                }
            }

            if (!string.IsNullOrEmpty(otherPerplex) && otherPerplex != "其它")
            {
                tags.Add(new EleTag(otherPerplex, ""));
            }

            return tags;
        }

        //既往史
        public string GetMedicalhistoryName(string value)
        {
            switch (value)
            {
                case "1":
                    return "重大手术史";
                case "2":
                    return "输血史(非献血)";
                case "3":
                    return "传染病史";
                case "4":
                    return "都没有";
                default:
                    return "";
            }
        }

        //用药情况
        public string GetMedicineName(string value)
        {
            switch (value)
            {
                case "0":
                    return "未服用药物";
                case "1":
                    return "高血压药物";
                case "2":
                    return "心脏病药物";
                case "3":
                    return "糖尿病药物";
                case "4":
                    return "脑梗塞药物";
                case "5":
                    return "治疗失眠药物";
                case "6":
                    return "其它";
                default:
                    return "";
            }
        }

        public List<string> GetMedicineNames(string medicine)
        {
            List<string> names = new List<string>();

            if (!string.IsNullOrEmpty(medicine))
            {
                string[] arrs = medicine.Split(',');

                foreach (var val in arrs)
                {
                    var name = GetMedicineName(val);
                    if (!string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }
            }

            return names;
        }

        //肺炎情况
        public string GetFieyanName(string value)
        {
            switch (value)
            {
                case "1":
                    return "抗疫一线医务人员";
                case "2":
                    return "湖北旅行史或居住史";
                case "3":
                    return "病例报告社区旅行史或接触史";
                case "4":
                    return "不确定有无接触史";
                case "5":
                    return "无接触史";
                case "6":
                    return "确诊患者";
                case "7":
                    return "确诊患者密切接触史";
                default:
                    return "";
            }
        }


        public string GetSignName(string nick, string account)
        {
            string signName = nick;

            if (!string.IsNullOrEmpty(account) && nick != account)
            {
                signName += "(" + account + ")";
            }


            return signName;
        }

        public string GetAge(DateTime? bithday)
        {
            string age = "-";

            if (bithday != null)
            {
                age = CommonUtil.GetAgeByBirthdate(bithday.Value).ToString();
            }

            return age;
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
                         select new { u.Id, u.Nick, u.HeadImgurl, u.Birthday, u.SAS, u.Perplex, u.Height, u.Weight, u.Medicalhistory, u.Medicine, u.OtherPerplex, u.BreathingMachine, u.Account, u.Sex, u.Mobile, u.LastReportId, u.LastReportTime, u.CreateTime });

            if (rup.Sas != "0")
            {
                query = query.Where(m => m.SAS == rup.Sas);
            }


            if (rup.Chronic != "0")
            {
                query = query.Where(m => m.Perplex.Contains(rup.Chronic));
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
                        var name = GetPerplexName(val);
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
                    SAS = GetSASName(item.SAS),
                    BreathingMachine = GetBreathingMachineName(item.BreathingMachine),
                    SignTags = GetSignTags(item.Perplex, item.OtherPerplex),
                    Medicalhistory = GetMedicalhistoryName(item.Medicalhistory),
                    Medicine = GetMedicineNames(item.Medicine),
                    Sex = GetSexName(item.Sex),
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
                    var name = GetPerplexName(val);
                    if (!string.IsNullOrEmpty(name))
                    {
                        perplex.Add(name);
                    }
                }
            }


            var ret = new
            {
                Id = d_SenvivUser.Id,
                SignName = GetSignName(d_SenvivUser.Nick, d_SenvivUser.Account),
                Age = GetAge(d_SenvivUser.Birthday),
                HeadImgurl = d_SenvivUser.HeadImgurl,
                SAS = GetSASName(d_SenvivUser.SAS),
                BreathingMachine = GetBreathingMachineName(d_SenvivUser.BreathingMachine),
                SignTags = GetSignTags(d_SenvivUser.Perplex, d_SenvivUser.OtherPerplex),
                Medicalhistory = GetMedicalhistoryName(d_SenvivUser.Medicalhistory),
                Medicine = GetMedicineNames(d_SenvivUser.Medicine),
                Sex = GetSexName(d_SenvivUser.Sex),
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
                             u.SmRssj,
                             u.SmQxsj,
                             u.MylGrfx,
                             u.MylMylZs,
                             u.MbGxbgk,
                             u.MbGxygk,
                             u.MbTlbgk,
                             u.QxxlJlqx,
                             u.QxxlKynl,
                             u.QxxlQxyj,
                             u.JbfxXljsl,
                             u.JbfxXlscfx,
                             u.HrvXzznl,
                             u.HrvXzznlJzz,
                             u.HrvJgsjzlzs,
                             u.HrvJgsjzlzsJzz,
                             u.HrvMzsjzlzs,
                             u.HrvMzsjzlzsJzz,
                             u.HrvZzsjzlzs,
                             u.HrvZzsjzlzsJzz,
                             u.HrvHermzs,
                             u.HrvHermzsJzz,
                             u.HrvTwjxgsszh,
                             u.HrvTwjxgsszhJzz,
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
                             u.HxDcPj,
                             //呼吸最高呼吸
                             u.HxZgHx,
                             //呼吸最低呼吸
                             u.HxZdHx,
                             //呼吸过快时长
                             u.HxGksc,
                             //呼吸过慢时长
                             u.HxGmsc,
                             //呼吸暂停次数
                             u.HxZtcs,
                             //呼吸暂停AHI指数
                             u.HxZtAhizs,
                             //呼吸暂停平均时长
                             u.HxZtPjsc,
                             u.SmScsj,
                             u.SmLcsj,
                             u.SmZcsc,
                             //睡眠时长
                             u.SmSmsc,
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
                             u.SmSemqsc,
                             //REM睡眠比例
                             u.SmSemqbl,
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
                             u.SvUserId,
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

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    SignName = GetSignName(item.Nick, item.Account),
                    HeadImgurl = item.HeadImgurl,
                    Sex = GetSexName(item.Sex),
                    Age = GetAge(item.Birthday),
                    HealthDate = item.HealthDate.ToUnifiedFormatDate(),
                    TotalScore = item.TotalScore,
                    SmRssj = item.SmRssj.ToUnifiedFormatDateTime(),
                    SmQxsj = item.SmQxsj.ToUnifiedFormatDateTime(),
                    item.MylGrfx,
                    item.MylMylZs,
                    item.MbGxbgk,
                    item.MbGxygk,
                    item.MbTlbgk,
                    item.QxxlJlqx,
                    item.QxxlKynl,
                    item.QxxlQxyj,
                    item.JbfxXlscfx,
                    item.JbfxXljsl,
                    //心脏总能量
                    item.HrvXzznl,
                    //心脏总能量基准值
                    item.HrvXzznlJzz,
                    //交感神经张力指数
                    item.HrvJgsjzlzs,
                    //交感神经张力指数基准值
                    item.HrvJgsjzlzsJzz,
                    //迷走神经张力指数
                    item.HrvMzsjzlzs,
                    //迷走神经张力指数基准值
                    item.HrvMzsjzlzsJzz,
                    //自主神经平衡指数
                    item.HrvZzsjzlzs,
                    //自主神经平衡指数基准值
                    item.HrvZzsjzlzsJzz,
                    //荷尔蒙指数
                    item.HrvHermzs,
                    //荷尔蒙指数基准值
                    item.HrvHermzsJzz,
                    //体温及血管舒缩指数
                    item.HrvTwjxgsszh,
                    //体温及血管舒缩指数基准值
                    item.HrvTwjxgsszhJzz,
                    //当次基准心率
                    item.XlDcjzxl,
                    //长期基准心率
                    item.XlCqjzxl,
                    //当次平均心率
                    item.XlDcpjxl,
                    //最高心率
                    item.XlZg,
                    //最低心率
                    item.XlZd,
                    //心动过快时长
                    item.XlXdgksc,
                    //心动过慢时长
                    item.XlXdgmsc,
                    //心率超过1.25时长
                    item.Xlcg125,
                    //心率超过1.15时长
                    item.Xlcg115,
                    //心率超过0.85时长
                    item.Xlcg085,
                    //心率超过075时长
                    item.Xlcg075,
                    //呼吸当次基准呼吸
                    item.HxDcjzhx,
                    //呼吸长期基准呼吸
                    item.HxCqjzhx,
                    //呼吸平均呼吸
                    item.HxDcPj,
                    //呼吸最高呼吸
                    item.HxZgHx,
                    //呼吸最低呼吸
                    item.HxZdHx,
                    //呼吸过快时长
                    item.HxGksc,
                    //呼吸过慢时长
                    item.HxGmsc,
                    //呼吸暂停次数
                    item.HxZtcs,
                    //呼吸暂停AHI指数
                    item.HxZtAhizs,
                    //呼吸暂停平均时长
                    item.HxZtPjsc,
                    item.SmZcsc,
                    //睡眠时长
                    item.SmSmsc,
                    //入睡需时
                    item.SmRsxs,
                    //深度睡眠时长
                    item.SmSdsmsc,
                    //深度睡眠比例
                    item.SmSdsmbl,
                    //浅度睡眠时长
                    item.SmQdsmsc,
                    //浅度睡眠比例
                    item.SmQdsmbl,
                    //REM睡眠时长
                    item.SmSemqsc,
                    //REM睡眠比例
                    item.SmSemqbl,
                    //清醒时刻时长
                    item.SmQxsksc,
                    //清醒时刻比例
                    item.SmQxskbl,
                    //离真次数
                    item.SmLzcs,
                    //离真时长
                    item.SmLzsc,
                    //体动次数
                    item.SmTdcs,
                    //平均体动时长
                    item.SmPjtdsc,
                    item.SmLcsj,
                    item.SmScsj
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
                             u.SmRssj,
                             u.SmQxsj,
                             u.MylGrfx,
                             u.MylMylZs,
                             u.MbGxbgk,
                             u.MbGxygk,
                             u.MbTlbgk,
                             u.QxxlJlqx,
                             u.QxxlKynl,
                             u.QxxlQxyj,
                             u.JbfxXljsl,
                             u.JbfxXlscfx,
                             u.HrvXzznl,
                             u.HrvXzznlJzz,
                             u.HrvJgsjzlzs,
                             u.HrvJgsjzlzsJzz,
                             u.HrvMzsjzlzs,
                             u.HrvMzsjzlzsJzz,
                             u.HrvZzsjzlzs,
                             u.HrvZzsjzlzsJzz,
                             u.HrvHermzs,
                             u.HrvHermzsJzz,
                             u.HrvTwjxgsszh,
                             u.HrvTwjxgsszhJzz,
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
                             u.HxDcPj,
                             //呼吸最高呼吸
                             u.HxZgHx,
                             //呼吸最低呼吸
                             u.HxZdHx,
                             //呼吸过快时长
                             u.HxGksc,
                             //呼吸过慢时长
                             u.HxGmsc,
                             //呼吸暂停次数
                             u.HxZtcs,
                             //呼吸暂停AHI指数
                             u.HxZtAhizs,
                             //呼吸暂停平均时长
                             u.HxZtPjsc,
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
                             u.SmSemqsc,
                             //REM睡眠比例
                             u.SmSemqbl,
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
                             u.SvUserId,
                             u.CreateTime
                         }).FirstOrDefault();



            var ret = new
            {

                Id = d_Rpt.Id,
                UserInfo = new
                {
                    SignName = GetSignName(d_Rpt.Nick, d_Rpt.Account),
                    HeadImgurl = d_Rpt.HeadImgurl,
                    Sex = GetSexName(d_Rpt.Sex),
                    Age = GetAge(d_Rpt.Birthday)
                },
                ReportData = new
                {
                    HealthDate = d_Rpt.HealthDate.ToUnifiedFormatDate(),
                    TotalScore = d_Rpt.TotalScore,
                    SmRssj = d_Rpt.SmRssj.ToUnifiedFormatDateTime(),
                    SmQxsj = d_Rpt.SmQxsj.ToUnifiedFormatDateTime(),
                    d_Rpt.MylGrfx,
                    d_Rpt.MylMylZs,
                    d_Rpt.MbGxbgk,
                    d_Rpt.MbGxygk,
                    d_Rpt.MbTlbgk,
                    d_Rpt.QxxlJlqx,
                    d_Rpt.QxxlKynl,
                    d_Rpt.QxxlQxyj,
                    d_Rpt.JbfxXlscfx,
                    d_Rpt.JbfxXljsl,
                    //心脏总能量
                    d_Rpt.HrvXzznl,
                    //心脏总能量基准值
                    d_Rpt.HrvXzznlJzz,
                    //交感神经张力指数
                    d_Rpt.HrvJgsjzlzs,
                    //交感神经张力指数基准值
                    d_Rpt.HrvJgsjzlzsJzz,
                    //迷走神经张力指数
                    d_Rpt.HrvMzsjzlzs,
                    //迷走神经张力指数基准值
                    d_Rpt.HrvMzsjzlzsJzz,
                    //自主神经平衡指数
                    d_Rpt.HrvZzsjzlzs,
                    //自主神经平衡指数基准值
                    d_Rpt.HrvZzsjzlzsJzz,
                    //荷尔蒙指数
                    d_Rpt.HrvHermzs,
                    //荷尔蒙指数基准值
                    d_Rpt.HrvHermzsJzz,
                    //体温及血管舒缩指数
                    d_Rpt.HrvTwjxgsszh,
                    //体温及血管舒缩指数基准值
                    d_Rpt.HrvTwjxgsszhJzz,
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
                    HxDcjzhx= SvDataJdUtil.GetHxDcjzhx(d_Rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx= SvDataJdUtil.GetHxCqjzhx(d_Rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcPj= SvDataJdUtil.GetHxDcPj(d_Rpt.HxDcPj),
                    //呼吸最高呼吸
                    d_Rpt.HxZgHx,
                    //呼吸最低呼吸
                    d_Rpt.HxZdHx,
                    //呼吸过快时长
                    d_Rpt.HxGksc,
                    //呼吸过慢时长
                    d_Rpt.HxGmsc,
                    //呼吸暂停次数
                    HxZtcs= SvDataJdUtil.GetHxZtcs(d_Rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtAhizs= SvDataJdUtil.GetHxZtAhizs(d_Rpt.HxZtAhizs),
                    //呼吸暂停平均时长
                    d_Rpt.HxZtPjsc,
                    d_Rpt.SmZcsc,
                    //睡眠时长
                    SmSmsc = SvDataJdUtil.GetSmSmsc(d_Rpt.SmSmsc),
                    //深度睡眠时长
                    SmSdsmsc = SvDataJdUtil.GetSmSdsmsc(d_Rpt.SmSdsmsc),
                    //浅度睡眠时长
                    SmQdsmsc = SvDataJdUtil.GetSmQdsmsc(d_Rpt.SmQdsmsc),
                    //REM睡眠时长
                    SmSemqsc = SvDataJdUtil.GetSmSemqsc(d_Rpt.SmSemqsc),
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
                    d_Rpt.SmLcsj,
                    d_Rpt.SmScsj
                }
            };



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }
    }
}
