using LocalS.BLL;
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
                    return "睡眠呼吸暂停综合征";
                case "3":
                    return "打鼾";
                case "4":
                    return "糖尿病";
                case "5":
                    return "高血压";
                case "6":
                    return "冠心病";
                case "7":
                    return "其他心脏病";
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
                    return "不宁腿综合征";
                case "14":
                    return "其他";
                default:
                    return "";
            }
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


        public CustomJsonResult GetUsers(string operater, string merchId, RupClientGetList rup)
        {
            var result = new CustomJsonResult();

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();


            if (string.IsNullOrEmpty(d_Merch.SenvivDepts))
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new PageEntity());

            var deptIds = d_Merch.SenvivDepts.Split(',');

            var query = (from u in CurrentDb.SenvivUser
                         where
                         deptIds.Contains(u.DeptId)
                         select new { u.Id, u.Nick, u.HeadImgurl, u.Account, u.Sex, u.Mobile, u.LastReportId, u.LastReportTime, u.CreateTime });


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
                    Account = item.Account,
                    Nick = item.Nick,
                    HeadImgurl = item.HeadImgurl,
                    Sex = item.Sex,
                    Mobile = item.Mobile,
                    LastReportId = item.LastReportId,
                    LastReportTime = item.LastReportTime,
                    CreateTime = item.CreateTime
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
    }
}
