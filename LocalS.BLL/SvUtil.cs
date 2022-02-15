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
        public readonly static string CA_0 = "#628DF2";//666666
        public readonly static string CA_1 = "#e68a8b";
        public readonly static string CA_2 = "#f1b46d";
        public readonly static string CA_3 = "#e16d6d";
        public readonly static string CA_4 = "#96a2dc";
        public readonly static string CA_5 = "#628DF2";

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

        public static string GetPerplexNames(string value,string ot)
        {
            List<string> names = new List<string>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] arrs = value.Split(',');

                if (arrs != null)
                {
                    foreach (var val in arrs)
                    {
                        var name = GetPerplexName(val);
                        if (!string.IsNullOrEmpty(name))
                        {
                            names.Add(name);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(ot))
            {
                names.Add(ot);
            }
            return string.Join(",", names);
        }

        public static string GetSubHealthName(string value)
        {
            return "";
        }

        public static string GetSubHealthNames(string value, string ot)
        {
            List<string> names = new List<string>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] arrs = value.Split(',');

                if (arrs != null)
                {
                    foreach (var val in arrs)
                    {
                        var name = GetSubHealthName(val);
                        if (!string.IsNullOrEmpty(name))
                        {
                            names.Add(name);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(ot))
            {
                names.Add(ot);
            }
            return string.Join(",", names);

        }

        public static string GetChronicdiseaseName(string value)
        {
            return "";
        }

        public static string GetChronicdiseaseNames(string value, string ot)
        {
            List<string> names = new List<string>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] arrs = value.Split(',');

                if (arrs != null)
                {
                    foreach (var val in arrs)
                    {
                        var name = GetChronicdiseaseName(val);
                        if (!string.IsNullOrEmpty(name))
                        {
                            names.Add(name);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(ot))
            {
                names.Add(ot);
            }
            return string.Join(",", names);

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
        public static string GetMedicalHisNames(string value,string ot)
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
            if (!string.IsNullOrEmpty(ot))
            {
                names.Add(ot);
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
        public static string GetMedicineNames(string value, string ot)
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
            if (!string.IsNullOrEmpty(ot))
            {
                names.Add(ot);
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
                if (string.IsNullOrEmpty(nick))
                {
                    signName = account;
                }
                else
                {
                    signName += "(" + account + ")";
                }
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
                int c = Convert.ToInt32(Convert.ToDecimal(obj));
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
        public static DateTime D32LongToDateTime(long time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000).AddHours(8);

        }
        public static long D32DateTimeToLong(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
            return timeStamp / 1000;
        }


        public static decimal Covevt2Hour(decimal seconds)
        {
            decimal hour = seconds / 3600m;

            return hour;
        }
        public static string GetTimeText(decimal scends, string valformat)
        {
            TimeSpan t = TimeSpan.FromSeconds(double.Parse(scends.ToString()));

            if (valformat == "1")
            {
                if (t.Hours > 0)
                {
                    if (t.Minutes > 0)
                    {
                        return t.Hours + "h" + t.Minutes + "m";
                    }
                    else
                    {
                        return t.Hours + "h";
                    }
                }
                else
                {
                    return t.Minutes + "m";
                }
            }
            else
            {
                if (t.Hours > 0)
                {
                    if (t.Minutes > 0)
                    {
                        return t.Hours + "小时" + t.Minutes + "分钟";
                    }
                    else
                    {
                        return t.Hours + "小时";
                    }
                }
                else
                {
                    return t.Minutes + "分钟";
                }
            }


            //if (hour <= 0)
            //    return "0";

            //return hour.ToString("0.00");
        }
        // 免疫力指数
        public static SvDataJd GetMylzs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Id = "1";
            jd.Name = "免疫力";
            jd.Value = val.ToString("0.#####");

            if (val <= 29)
            {
                jd.Set("差", "↓↓", CA_1);
            }
            else if (val >= 30 && val <= 49)
            {
                jd.Set("较差", "↓", CA_2);
            }
            else if (val >= 50 && val <= 69)
            {
                jd.Set("中等", "-", CA_3);
            }
            else if (val >= 70 && val <= 89)
            {
                jd.Set("较好", "-", CA_4);
            }
            else if (val >= 90 && val <= 100)
            {
                jd.Set("好", "-", CA_0);
            }
            return jd;
        }
        public static SvDataJd GetMylGrfx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Id = "2";
            jd.Name = "感染风险";
            jd.Value = val.ToString("0.#####");

            if (val <= 19)
            {
                jd.Set("低", "-", CA_0);
            }
            else if (val >= 20 && val <= 39)
            {
                jd.Set("较低", "-", CA_4);
            }
            else if (val >= 40 && val <= 69)
            {
                jd.Set("中等", "↓", CA_3);
            }
            else if (val >= 70 && val <= 84)
            {
                jd.Set("较高", "↓↓", CA_2);
            }
            else if (val >= 85 && val <= 100)
            {
                jd.Set("高", "↓↓", CA_1);
            }
            return jd;
        }
        public static SvDataJd GetMbGxygk(decimal val)
        {
            var jd = new SvDataJd();
            val = 100 - val;
            jd.Id = "3";
            jd.Name = "高血压管控";
            jd.Value = val.ToString("0.#####");

            if (val <= 39)
            {
                jd.Set("差", "↓↓", CA_1);
            }
            else if (val >= 40 && val <= 69)
            {
                jd.Set("一般", "↓", CA_2);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CA_0);
            }
            return jd;
        }
        public static SvDataJd GetMbGxbgk(decimal val)
        {
            var jd = new SvDataJd();
            jd.Id = "4";
            jd.Name = "冠心病管控";
            if (val == 0)
            {
                jd.Value = "";
                jd.Set("", "", CA_1);

                return jd;
            }

            jd.Value = val.ToString("0.#####");

            if (val <= 39)
            {
                jd.Set("差", "↓↓", CA_1);
            }
            else if (val >= 40 && val <= 69)
            {
                jd.Set("一般", "↓", CA_2);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CA_0);
            }
            return jd;
        }
        public static SvDataJd GetMbTlbgk(decimal val)
        {
            var jd = new SvDataJd();
            jd.Id = "5";
            jd.Name = "冠心病管控";

            val = 100 - val;
            jd.Value = val.ToString("0.#####");

            if (val <= 39)
            {
                jd.Set("差", "↓↓", CA_1);
            }
            else if (val >= 40 && val <= 69)
            {
                jd.Set("一般", "↓", CA_2);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CA_0);
            }
            return jd;
        }
        public static SvDataJd GetQxxlKynl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Id = "7";
            jd.Name = "抗压能力";
            jd.Value = val.ToString("0.#####");

            if (val <= 29)
            {
                jd.Set("差", "↓↓", CA_1);
            }
            else if (val >= 30 && val <= 69)
            {
                jd.Set("一般", "↓", CA_2);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CA_0);
            }
            return jd;
        }
        public static SvDataJd GetQxxlJlqx(string val)
        {
            var jd = new SvDataJd();
            jd.Id = "6";
            jd.Name = "焦虑情绪";
            jd.Value = val.ToString();


            return jd;
        }
        public static SvDataJd GetQxxlQxyj(decimal val)
        {
            var jd = new SvDataJd();
            jd.Id = "8";
            jd.Name = "情绪应激";
            jd.Value = val.ToString("0.#####");


            return jd;
        }
        public static SvDataJd GetJbfxXlscfx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "50~80";
            if (val <= 29)
            {
                jd.Set("高风险", "↓↓", CA_1);
            }
            else if (val >= 30 && val <= 49)
            {
                jd.Set("中风险", "↓", CA_2);
            }
            else if (val >= 50 && val <= 180)
            {
                jd.Set("低风险", "-", CA_0);
            }
            else if (val >= 180 && val <= 220)
            {
                jd.Set("中风险", "↑", CA_2);
            }
            else if (val >= 221)
            {
                jd.Set("高风险", "↑↑", CA_1);
            }
            return jd;
        }
        public static SvDataJd GetJbfxXljsl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "4.6~12";
            if (val <= 2.5m)
            {
                jd.Set("过低", "↓↓", CA_1);
            }
            else if (val >= 2.5m && val <= 4.6m)
            {
                jd.Set("偏低", "↓", CA_2);
            }
            else if (val >= 4.6m && val <= 12m)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val >= 12m && val <= 20m)
            {
                jd.Set("偏高", "↑", CA_2);
            }
            else if (val > 20m)
            {
                jd.Set("过高", "↑↑", CA_1);
            }
            return jd;
        }
        public static SvDataJd GetHxZtahizs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.Name = "呼吸紊乱指数";
            jd.RefRange = "0~5次/h";
            if (val < 5)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val >= 5 && val < 15)
            {
                jd.Set("轻度", "↑", CA_3);
            }
            else if (val >= 15 && val < 30)
            {
                jd.Set("中度", "↑↑", CA_2);
            }
            else if (val >= 30)
            {
                jd.Set("重度", "↑↑↑", CA_1);
            }
            return jd;
        }
        public static SvDataJd GetSmSmsc(decimal val, string valFormat)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠总时长";
            var hour = Covevt2Hour(val);
            jd.Value = GetTimeText(val, valFormat);
            jd.RefRange = "6~9h";
            if (hour < 6)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (hour >= 6 && hour <= 9)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (hour > 9)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmZcsjfw(DateTime d1, DateTime d2)
        {
            var jd = new SvDataJd();
            jd.Name = "在床时间段";
            jd.Value = d1.ToString("HH:mm") + "-" + d2.ToString("HH:mm");

            return jd;

        }
        public static SvDataJd GetSmRsxs(decimal val, string valFormat)
        {
            var jd = new SvDataJd();
            jd.Name = "入睡需时";

            TimeSpan ts = TimeSpan.FromSeconds(double.Parse(val.ToString()));

            jd.Value = GetTimeText(val, valFormat);
            jd.RefRange = "0~30min";
            if (ts.TotalMinutes <= 30)
            {
                jd.Set("正常", "-", CA_5);
            }
            else if (ts.TotalMinutes > 30 && ts.TotalMinutes <= 60)
            {
                jd.Set("偏多", "↑", CA_2);
            }
            else if (ts.TotalMinutes > 60)
            {
                jd.Set("过多", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmQdsmsc(decimal val, string valFormat)
        {
            var jd = new SvDataJd();

            var hour = Covevt2Hour(val);
            jd.Value = GetTimeText(val, valFormat);

            jd.RefRange = "3.5~5.4h";
            if (hour < 3.5m)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (hour >= 3.5m && hour <= 5.4m)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (hour > 5.4m)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmSdsmsc(decimal val, string valFormat)
        {
            var jd = new SvDataJd();
            jd.Name = "深度睡眠时长";
            var hour = Covevt2Hour(val);
            jd.Value = GetTimeText(val, valFormat);
            jd.RefRange = "1.05~2.25h";
            if (hour < 1.05m)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (hour >= 1.05m && hour <= 2.25m)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (hour > 2.25m)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmRemsmsc(decimal val, string valFormat)
        {
            var jd = new SvDataJd();

            var hour = Covevt2Hour(val);

            jd.Value = GetTimeText(val, valFormat);
            jd.RefRange = "1.05~2.25h";
            if (hour < 1.05m)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (hour >= 1.05m && hour <= 2.25m)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (hour > 2.25m)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmSmzq(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "4~5次";
            if (val < 4)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 4 && val <= 5)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 5)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmTdcs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "0~200次";
            if (val <= 200)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 200)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetXlDcjzxl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "基准心率";
            jd.Value = val.ToString();
            jd.RefRange = "50~83次/min";
            if (val < 50)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 50 && val <= 83)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 83)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetXlCqjzxl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "50~65次/min";
            if (val < 50)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 50 && val <= 65)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 65)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetXlDcpjxl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "50~65次/min";
            if (val < 50)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 50 && val <= 65)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 65)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHxDcjzhx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "12~20次/min";
            if (val < 12)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 12 && val <= 20)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 20)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHxCqjzhx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "10~18次/min";
            if (val < 10)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 10 && val <= 18)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 18)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHxDcpjhx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "10~18次/min";
            if (val < 10)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 10 && val <= 18)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 18)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHxZtcs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "呼吸暂停次数";
            jd.Value = val.ToString();
            jd.RefRange = "0~30次";
            if (val <= 30)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 30)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHrvXzznl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "心脏总能量";
            jd.Value = val.ToString();
            jd.RefRange = "2300~5000";
            if (val < 2300)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 2300 && val <= 5000)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 30)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHrvJgsjzlzs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "500~1200";
            if (val < 500)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 500 && val <= 1200)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 1200)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHrvMzsjzlzs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "520~1200";
            if (val < 520)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 520 && val <= 1200)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 1200)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHrvZzsjzlzs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "0.7~1.3";
            if (val < 0.7m)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 0.7m && val <= 1.3m)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 1.3m)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;

        }
        public static SvDataJd GetHrvHermzs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "480~1100";
            if (val < 480)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 480 && val <= 1100)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 1100)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHrvTwjxgsszh(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "600~1800";
            if (val < 480)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 600 && val <= 1800)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val > 1800)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmScore(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠值";
            jd.Value = val;
            jd.RefRange = "0~100";
            if (val < 29)
            {
                jd.Set("差", "", CA_1);
            }
            else if (val >= 30 && val <= 49)
            {
                jd.Set("较差", "", CA_2);
            }
            else if (val >= 50 && val <= 69)
            {
                jd.Set("中等", "", CA_3);
            }
            else if (val >= 70 && val <= 89)
            {
                jd.Set("较好", "", CA_4);
            }
            else
            {
                jd.Set("好", "", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetHealthScore(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "健康值";
            jd.Value = val;
            jd.RefRange = "0~100";
            return jd;
        }
        public static SvDataJd GetSmSmxl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠效率";
            jd.Value = Convert.ToInt32(val * 100).ToString();
            jd.RefRange = "85~100";

            if (val <= 0.5m)
            {
                jd.Set("低", "↓↓", CA_1);
            }
            else if (val > 0.5m && val <= 0.85m)
            {
                jd.Set("偏低", "↓", CA_2);
            }
            else if (val > 0.85m)
            {
                jd.Set("正常", "-", CA_5);
            }

            return jd;

        }
        public static SvDataJd GetSmSmlxx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠连续性";
            jd.Value = Convert.ToInt32(val * 100).ToString();
            jd.RefRange = "0~100";

            if (val <= 0.75m)
            {
                jd.Set("低", "↓↓", CA_1);
            }
            else if (val > 0.75m && val <= 0.9m)
            {
                jd.Set("偏低", "↓", CA_2);
            }
            else if (val > 0.9m)
            {
                jd.Set("正常", "-", CA_5);
            }

            return jd;
        }
        public static SvDataJd GetSmSdsmbl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "深度睡眠比例";
            jd.Value = val;
            jd.RefRange = "15~25";

            if (val <= 15m)
            {
                jd.Set("少", "↓", CA_2);
            }
            else if (val > 15m && val <= 25m)
            {
                jd.Set("正常", "-", CA_5);
            }
            else if (val > 25m)
            {
                jd.Set("多", "-", CA_2);
            }

            return jd;

            return jd;
        }

    }
}
