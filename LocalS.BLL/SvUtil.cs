using LocalS.BLL.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SvUtil
    {
        //性别
        public static string GetSexName(string value)
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
        public static string GetSasName(string value)
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
        public static string GetIsUseBreathMachName(bool value)
        {
            if (value)
            {
                return "是";
            }
            else
            {
                return "否";
            }
        }

        //目前困扰
        public static string GetPerplexName(string value)
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

        public static List<EleTag> GetSignTags(string perplex, string otherPerplex)
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
        private static string GetMedicalHisName(string value)
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

        public static string GetMedicalHisNames(string value)
        {
            List<string> names = new List<string>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] arrs = value.Split(',');

                if (arrs != null)
                {
                    foreach (var val in arrs)
                    {
                        var name = GetMedicalHisName(val);
                        if (!string.IsNullOrEmpty(name))
                        {
                            names.Add(name);
                        }
                    }
                }
            }

            return string.Join(",", names);

        }

        //用药情况
        private static string GetMedicineName(string value)
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

        public static string GetMedicineNames(string value)
        {
            List<string> names = new List<string>();

            if (!string.IsNullOrEmpty(value))
            {

                string[] arrs = value.Split(',');

                if (arrs != null)
                {
                    foreach (var val in arrs)
                    {
                        var name = GetMedicineName(val);
                        if (!string.IsNullOrEmpty(name))
                        {
                            names.Add(name);
                        }
                    }
                }
            }

            return string.Join(",", names);
        }

        //肺炎情况
        public static string GetFieyanName(string value)
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

        public static string GetSignName(string nick, string account)
        {
            string signName = nick;

            if (!string.IsNullOrEmpty(account) && nick != account)
            {
                signName += "(" + account + ")";
            }


            return signName;
        }

        public static string GetAge(DateTime? bithday)
        {
            string age = "-";

            if (bithday != null)
            {
                age = Lumos.CommonUtil.GetAgeByBirthdate(bithday.Value).ToString();
            }

            return age;
        }

        public static int D46Int(object obj)
        {
            if (obj == null)
                return 0;
            try
            {
                int c = Convert.ToInt32(obj);
                return c;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static long D46Long(object obj)
        {
            if (obj == null)
                return 0;

            try
            {
                long c = Convert.ToInt64(obj);
                return c;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static decimal D46Decimal(object obj)
        {
            if (obj == null)
                return 0;

            try
            {
                decimal c = decimal.Parse(obj.ToString());
                return c;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
