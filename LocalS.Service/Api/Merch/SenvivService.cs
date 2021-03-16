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
                    return "其他";
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
    }
}
