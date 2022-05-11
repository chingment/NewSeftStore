using LocalS.BLL.Biz;
using LocalS.BLL.UI;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ChatDataByStr
    {
        public string xData { get; set; }

        public string yData { get; set; }
    }

    public class SvUtil
    {
        public readonly static string CB_1 = "#ed6869";//深红
        public readonly static string CB_2 = "#ee8b8a";//浅红
        public readonly static string CB_3 = "#f8b260";//橙色
        public readonly static string CB_4 = "#95a1e9";//紫色
        public readonly static string CB_5 = "#628DF2";//蓝色


        public readonly static string CA_0 = "#628DF2";//蓝色
        public readonly static string CA_1 = "#e68a8b";//红色
        public readonly static string CA_2 = "#f1b46d";//橙色
        public readonly static string CA_3 = "#e16d6d";//红色
        public readonly static string CA_4 = "#96a2dc";//紫色
        public readonly static string CA_5 = "#628DF2";//蓝色

        public static FieldModel GetIdentity(E_SvUserCareMode value)
        {
            switch (value)
            {
                case E_SvUserCareMode.Lady:
                    return new FieldModel(21, "暂无孕产计划");
                case E_SvUserCareMode.PrePregnancy:
                    return new FieldModel(22, "备孕");
                case E_SvUserCareMode.Pregnancy:
                    return new FieldModel(23, "孕妈");
                case E_SvUserCareMode.Postpartum:
                    return new FieldModel(24, "宝妈");
                default:
                    return new FieldModel(21, "暂无孕产计划");
            }
        }

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
                case "0":
                case "1":
                    return "无";
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
                case "21":
                    return "易醒";
                default:
                    return "";
            }
        }

        public static string GetPerplexNames(string value, string ot)
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
            switch (value)
            {
                case "0":
                    return "无";
                case "1":
                    return "疲乏无力";
                case "2":
                    return "情绪波动";
                case "3":
                    return "精力不足";
                case "4":
                    return "怕冷怕冷";
                case "5":
                    return "头昏头痛";
                case "6":
                    return "易于感冒";
                case "7":
                    return "记忆力下降";
                case "8":
                    return "胸闷";
                case "9":
                    return "肠胃问题";
                default:
                    return "";
            }
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
            switch (value)
            {
                case "0":
                    return "无";
                case "4":
                    return "糖尿病";
                case "5":
                    return "高血压";
                case "6":
                    return "冠心病";
                default:
                    return "";
            }
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

        public static List<EleTag> GetSignTags(string chronicdisease, string chronicdiseaseOt)
        {
            var tags = new List<EleTag>();

            if (!string.IsNullOrEmpty(chronicdisease))
            {
                string[] arrs = chronicdisease.Split(',');

                foreach (var val in arrs)
                {
                    var name = GetChronicdiseaseName(val);
                    if (!string.IsNullOrEmpty(name) && name != "无" && name != "其它")
                    {
                        tags.Add(new EleTag(name, ""));
                    }
                }
            }

            if (!string.IsNullOrEmpty(chronicdiseaseOt) && chronicdiseaseOt != "无" && chronicdiseaseOt != "其它")
            {
                tags.Add(new EleTag(chronicdiseaseOt, ""));
            }

            return tags;
        }
        //既往史
        private static string GetMedicalHisName(string value)
        {
            switch (value)
            {
                case "0":
                    return "无";
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
        public static string GetMedicalHisNames(string value, string ot)
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
                    return "无";
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
                long c = Convert.ToInt64(Convert.ToDecimal(obj));
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
                decimal c = decimal.Parse(decimal.Parse(obj.ToString()).ToString("#0.00"));
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
            decimal hour = decimal.Parse((seconds / 3600m).ToString("#0.00"));
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
        public static SvDataJd GetMylzs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();

            jd.Id = "1";
            jd.Name = "免疫力";
            jd.Value = Lumos.CommonUtil.ToInt(val);
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 30)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 30 && val < 50)
            {
                jd.Set("较差", "↓", CB_2);
            }
            else if (val >= 50 && val < 70)
            {
                jd.Set("中等", "-", CB_3);
            }
            else if (val >= 70 && val < 90)
            {
                jd.Set("较好", "-", CB_4);
            }
            else if (val >= 90 && val <= 100)
            {
                jd.Set("好", "-", CB_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 30, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 30, Max = 50, Color = CB_2, Tips = "较差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 50, Max = 70, Color = CB_3, Tips = "中等" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 90, Color = CB_4, Tips = "较好" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 90, Max = 100, Color = CB_5, Tips = "好" });
            }

            return jd;
        }
        public static SvDataJd GetMylGrfx(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "2";
            jd.Name = "感染风险";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 20)
            {
                jd.Set("低", "-", CB_5);
            }
            else if (val >= 20 && val < 40)
            {
                jd.Set("较低", "-", CB_4);
            }
            else if (val >= 40 && val < 70)
            {
                jd.Set("中等", "-", CB_3);
            }
            else if (val >= 70 && val < 85)
            {
                jd.Set("较高", "↑", CB_2);
            }
            else if (val >= 85 && val <= 100)
            {
                jd.Set("高", "↑↑", CB_1);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 20, Color = CB_5, Tips = "低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 20, Max = 40, Color = CB_4, Tips = "较低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 70, Color = CB_3, Tips = "中等" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 85, Color = CB_2, Tips = "较高" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 85, Max = 100, Color = CB_1, Tips = "高" });
            }


            return jd;
        }



        public static SvDataJd GetMbGxygk(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "3";
            jd.Name = "高血压管控";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 40)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 40 && val < 70)
            {
                jd.Set("一般", "↓", CB_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CB_5);
            }


            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 40, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 70, Color = CB_3, Tips = "一般" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CB_5, Tips = "好" });

            }

            return jd;
        }
        public static SvDataJd GetMbGxbgk(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "4";
            jd.Name = "冠心病管控";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };

            if (val < 40)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 40 && val < 70)
            {
                jd.Set("一般", "↓", CA_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CA_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 40, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 70, Color = CB_3, Tips = "一般" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CB_5, Tips = "好" });

            }

            return jd;
        }
        public static SvDataJd GetMbTnbgk(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "5";
            jd.Name = "糖尿病管控";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 40)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 40 && val < 70)
            {
                jd.Set("一般", "↓", CB_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CB_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 40, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 70, Color = CB_3, Tips = "一般" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CB_5, Tips = "好" });

            }

            return jd;
        }

        public static SvDataJd GetMbXytjjn(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "5";
            jd.Name = "血压调节机能";
            jd.Value = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 40)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 40 && val < 70)
            {
                jd.Set("一般", "↓", CB_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CB_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 40, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 70, Color = CB_3, Tips = "一般" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CB_5, Tips = "好" });

            }

            return jd;
        }

        public static SvDataJd GetMbGzdmjn(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "5";
            jd.Name = "冠状动脉机能";
            jd.Value = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 40)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 40 && val < 70)
            {
                jd.Set("一般", "↓", CB_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CB_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 40, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 70, Color = CB_3, Tips = "一般" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CB_5, Tips = "好" });

            }

            return jd;
        }

        public static SvDataJd GetMbXtphjn(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "5";
            jd.Name = "血糖平衡机能";
            jd.Value = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 40)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 40 && val < 70)
            {
                jd.Set("一般", "↓", CB_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CB_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 40, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 70, Color = CB_3, Tips = "一般" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CB_5, Tips = "好" });

            }

            return jd;
        }

        public static SvDataJd GetQxxlKynl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "7";
            jd.Name = "抗压能力";
            jd.Value = Lumos.CommonUtil.ToInt(val);
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } };
            if (val < 30)
            {
                jd.Set("差", "↓↓", CB_1);
            }
            else if (val >= 30 && val < 70)
            {
                jd.Set("一般", "↓", CB_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("好", "-", CB_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 30, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 30, Max = 70, Color = CB_3, Tips = "一般" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CB_5, Tips = "好" });

            }

            return jd;
        }
        public static SvDataJd GetQxxlJlqx(string val, bool isGetRefRanges = false, List<ChatDataByStr> lastVals = null)
        {
            string s_val = val.ToString();
            int i_val = 0;
            if (s_val == "恐慌")
            {
                i_val = 10;
            }
            else if (s_val == "重度")
            {
                i_val = 30;
            }
            else if (s_val == "中度")
            {
                i_val = 50;
            }
            else if (s_val == "轻度")
            {
                i_val = 70;
            }
            else if (s_val == "安康")
            {
                i_val = 90;
            }

            List<object> i_lastVals = new List<object>();

            foreach (var t in lastVals)
            {
                string c = t.yData;
                if (c == "恐慌")
                {
                    i_lastVals.Add(new { xData = t.xData, yData = 10 });
                }
                else if (c == "重度")
                {
                    i_lastVals.Add(new { xData = t.xData, yData = 30 });
                }
                else if (c == "中度")
                {
                    i_lastVals.Add(new { xData = t.xData, yData = 50 });
                }
                else if (c == "轻度")
                {
                    i_lastVals.Add(new { xData = t.xData, yData = 70 });
                }
                else if (c == "安康")
                {
                    i_lastVals.Add(new { xData = t.xData, yData = 90 });
                }
            }

            var jd = new SvDataJd();
            jd.Id = "6";
            jd.Name = "焦虑情绪";
            jd.Value = i_val;
            jd.ValueText = val;
            jd.Chat = new { Data = i_lastVals, yAxisLabel = new int[] { 0, 10, 30, 50, 90 }, yAxisLabelExt = new string[] { "", "恐慌", "重度", "中度", "轻度", "安康" }, markLine = new { yAxis = "" } };
            jd.IsHidValue = true;
            if (i_val <= 20)
            {
                jd.Set("恐慌", "", CA_3);
            }
            else if (i_val > 20 && i_val <= 40)
            {
                jd.Set("重度", "", CA_1);
            }
            else if (i_val > 40 && i_val <= 60)
            {
                jd.Set("中度", "", CA_2);
            }
            else if (i_val > 60 && i_val <= 80)
            {
                jd.Set("轻度", "", CA_4);
            }
            else if (i_val > 80 && i_val <= 100)
            {
                jd.Set("安康", "↓", CA_0);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 20, Color = CA_3, Tips = "恐慌" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 20, Max = 40, Color = CA_1, Tips = "重度" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 60, Color = CA_2, Tips = "中度" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 60, Max = 80, Color = CA_4, Tips = "轻度" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 80, Max = 100, Color = CA_0, Tips = "安康" });
            }

            return jd;
        }
        public static SvDataJd GetQxxlQxyj(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "8";
            jd.Name = "情绪应激";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 } };

            return jd;
        }



        public static SvDataJd GetJbfxXlscfx(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "心律失常风险";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.RefRange = "50~80";
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 13, 26, 39, 52, 65, 78, 91 }, markLine = new { yAxis = 78 } };
            jd.Pph = "心律失常风险指数，反映了心脏跳动节律的总体变异性，可用于预测心律失常发生风险，参考范围为50~180，指数值升高或降低都可能出现心律失常。 当心律失常风险指数降低时，提示快速性心律失常风险增加，原患高血压、冠心病或其他心脏病者，可能出现心动过速、房扑等。当心律失常风险指数升高，提示慢速性心律失常风险增加，心动过缓、停搏、房室传导阻滞的风险增加。";
            if (val < 30)
            {
                jd.Set("过低", "↓↓", CB_1);
            }
            else if (val >= 30 && val < 50)
            {
                jd.Set("偏低", "↓", CB_3);
            }
            else if (val >= 50 && val < 180)
            {
                jd.Set("正常", "-", CB_5);
            }
            else if (val >= 180 && val < 220)
            {
                jd.Set("偏高", "↑", CB_3);
            }
            else if (val >= 220)
            {
                jd.Set("过高", "↑↑", CB_1);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 30, Color = CB_1, Tips = "过低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 30, Max = 50, Color = CB_3, Tips = "偏低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 50, Max = 180, Color = CB_5, Tips = "正常" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 180, Max = 220, Color = CB_3, Tips = "偏高" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 220, Max = 1000, Color = CB_1, Tips = "过高" });
            }

            return jd;
        }
        public static SvDataJd GetJbfxXljsl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "心律减速理";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.RefRange = "4.6~12";
            jd.Pph = "心率减速力是迷走神经的负性作用对心率的减速调节结果，即降低心率，对心脏起到保护性作用。心率减速力对预测心脏性猝死有重要意义，参考范围为4.6~12，心率减速力越低，心脏性猝死风险越高。相反的情况，当心率减速力过大，停搏、房室传导阻滞的风险会升高。";
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
        public static SvDataJd GetHxZtahizs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次/h";

            jd.Name = "呼吸紊乱指数";
            jd.RefRange = "0~5次/h";
            jd.Pph = "也叫睡眠呼吸暂停和低通气指数，是指每小时发生呼吸暂停和低通气的平均次数，正常范围是0~5次/小时。超过5次/小时则可判断为睡眠呼吸暂停，5~15次/小时为轻度，15~30次/小时为中度，大于30次/小时为重度。中重度呼吸暂停会增加高血压、心律失常、冠心病、心衰、卒中、认知异常等疾病的风险。";
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

        public static SvDataJd GetSmZcsc(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "在床总时长";
            var hour = Covevt2Hour(val);
            jd.Value = hour;
            jd.ValueText = GetTimeText(val, "2");
            jd.RefRange = "";
            jd.Set("", "-", CA_0);

            return jd;
        }

        public static SvDataJd GetSmSmsc(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "实际睡眠总时长";
            var hour = Covevt2Hour(val);
            jd.Value = hour;
            jd.ValueText = GetTimeText(val, "2");
            jd.RefRange = "6~9h";
            jd.Pph = "保证充足的睡眠有助于维持身心健康，美国睡眠医学会和睡眠研究学会建议：成年人每晚最佳睡眠时间是7-8小时。但睡眠时间不是评价睡眠质量的唯一标准，只要睡醒后感觉神清气爽、精神饱满即可，习惯性短睡者不需因为睡眠总时长低于推荐标准而过分担忧。另外，一味为了延长睡眠时间而赖在床上并不能弥补睡眠不足，反而更不利于获得高质量睡眠；可以通过短暂午睡来弥补夜间睡眠不足。";
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 2, 3, 5, 6, 8, 9, 11 }, yAxisMin = 0, yAxisMax = 11, yAxisSplitNumber = 8, yAxisMarkLine = 7 };

            if (hour < 5)
            {
                jd.Set("过少", "↓↓", CB_1);
            }
            else if (hour >= 5 && hour < 7)
            {
                jd.Set("偏少", "↓", CB_3);
            }
            else if (hour >= 7 && hour < 8)
            {
                jd.Set("正常", "-", CB_5);
            }
            else if (hour >= 8 && hour < 9)
            {
                jd.Set("偏多", "↑", CB_3);
            }
            else if (hour >= 9)
            {
                jd.Set("过多", "↑↑", CB_1);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 5, Color = CB_1, Tips = "过少" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 5, Max = 7, Color = CB_3, Tips = "偏少" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 7, Max = 8, Color = CB_5, Tips = "正常" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 8, Max = 9, Color = CB_3, Tips = "偏多" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 9, Max = "∞", Color = CB_1, Tips = "过多" });
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
        public static SvDataJd GetSmRsxs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "入睡需时";
            jd.Pph = "从躺在床上至入睡所需要的时长，正常人入睡应该在0.5小时以内。如果超过半小时仍然睡不着就可能存在失眠，失眠是睡眠障碍中最常见的一种。随着生活压力的增大和一系列社会事件的增多，失眠发生率升高；此外电子产品的广泛应用，也大大增加了入睡困难的发生率，与此同时也导致焦虑发生率升高。白天注意运动锻炼，晚上减少使用电子产品能避免入睡困难，如果睡不着建议起来到客厅或者书房看书或做其它活动再到床上来睡觉，不要使劲在床上睡。";
            TimeSpan ts = TimeSpan.FromSeconds(double.Parse(val.ToString()));
            var hour = Covevt2Hour(val);
            jd.Value = hour;
            jd.ValueText = GetTimeText(val, "2");
            jd.RefRange = "0~30min";
            jd.Chat = new { Data = lastVals, yAxisMin = 0, yAxisMax = 11, yAxisSplitNumber = 8, yAxisMarkLine = 30 };
            if (ts.TotalMinutes < 30)
            {
                jd.Set("正常", "-", CB_5);
            }
            else if (ts.TotalMinutes >= 30 && ts.TotalMinutes < 60)
            {
                jd.Set("偏多", "↑", CB_3);
            }
            else if (ts.TotalMinutes >= 60)
            {
                jd.Set("过多", "↑", CB_1);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 30, Color = CB_5, Tips = "正常" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 30, Max = 60, Color = CB_3, Tips = "偏多" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 60, Max = "∞", Color = CB_1, Tips = "过多" });
            }

            return jd;
        }
        public static SvDataJd GetSmQdsmsc(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "浅度睡眠时长";
            var hour = Covevt2Hour(val);
            jd.Value = hour;
            jd.ValueText = GetTimeText(val, "2");
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

        public static SvDataJd GetSmSmlxx(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠连续性";
            jd.Value = val;
            jd.ValueText = val.ToString("0.#####") + "%";
            jd.RefRange = "90~100";
            jd.Pph = "睡眠过程不间断，则为睡眠连续性好，是评价睡眠质量的其中一个标准，对体力恢复、情绪调节和增强记忆力都有重要作用。典型的睡眠状态转换为“浅睡-深睡-浅睡-REM（快速眼动）”，为一个睡眠周期，随即进入下一次的睡眠周期。但如果睡眠过程中觉醒次数较多，醒后难以再次入睡，则直接影响睡眠的连续性。";
            jd.Chat = new { Data = lastVals, yAxisMarkLine = 90 };

            if (val <75)
            {
                jd.Set("低", "↓↓", CB_1);
            }
            else if (val >= 75 && val < 90)
            {
                jd.Set("偏低", "↓", CB_3);
            }
            else if (val >= 90)
            {
                jd.Set("正常", "-", CB_5);
            }


            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 75, Color = CB_1, Tips = "低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 75, Max = 90, Color = CB_3, Tips = "偏低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 90, Max = 100, Color = CB_5, Tips = "正常" });
            }

            return jd;
        }

        public static SvDataJd GetSmQdsmbl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "浅睡眠";
            jd.Value = val;
            jd.ValueText = val.ToString("0.#####") + "%";
            jd.RefRange = "45~65";
            jd.Pph = "浅睡眠阶段的心率和呼吸比清醒时减慢，人在浅睡眠时容易被唤醒，对人体疲劳消除、精力恢复的作用不如深睡眠，但浅睡眠是人从深睡眠过度到清醒的保护机制，是一种必须的生理需求。浅睡眠的正常比例为45%~65%。";
            if (val < 45m)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 45m && val < 65m)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val >= 65m)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }

        public static SvDataJd GetSmSdsmsc(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "深度睡眠时长";
            var hour = Covevt2Hour(val);
            jd.Value = hour;
            jd.ValueText = GetTimeText(val, "2");
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
        public static SvDataJd GetSmRemsmsc(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "REM睡眠时长";
            var hour = Covevt2Hour(val);
            jd.Value = hour;
            jd.ValueText = GetTimeText(val, "2");
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

        public static SvDataJd GetSmLzcs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "离枕次数";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.RefRange = "4~5次";
            jd.Pph = "睡眠过程中体动次数的多少反映了睡眠质量的高低，正常人体动次数为50~200次，体动过多通常是因为睡眠不安、早醒、难入睡等，而睡眠呼吸暂停患者由于翻身较多、身体抽动等原因，体动次数通常也较多。";
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

        public static SvDataJd GetSmSmzq(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠周期";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.RefRange = "4~5次";
            jd.Pph = "典型的睡眠状态转换为浅睡-深睡-浅睡-REM（快速眼动），为一个睡眠周期，随即进入下一次的睡眠周期。但实际过程中不一定会经历所有的睡眠状态，睡眠状态的转换也不一定是完全规律的。7小时睡眠中3-6个周期是正常的。";
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
        public static SvDataJd GetSmTdcs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "体动次数";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
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
        public static SvDataJd GetXlDcjzxl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "当次基准心率";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次/min";
            jd.RefRange = "50~83次/min";
            jd.Pph = "基准心率反映心脏基础功能，夜间心率的参考范围为50~65次/分钟。长期夜间心率太高，会使心血管疾病风险升高。心率高的几种可能情况：感染发热、长期不运动、饮酒、疲劳、高血压/糖尿病/冠心病/心衰患者、服用药物如阿托品、肾上腺素等。心率低的几种可能情况：长期运动人群、窦性心动过缓、停搏、传导阻滞。";
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
        public static SvDataJd GetXlCqjzxl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "长期基准心率";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次/min";
            jd.RefRange = "50~65次/min";
            jd.Pph = "长期基准心率反映了一段时间内心脏的总体功能，其参考范围为50~65次/分钟。长期不运动、经常饮酒、吸烟、睡眠障碍者，患高血压、冠心病、心衰的患者等长期基准心率可能会较正常人要高，发生心血管意外的风险也会相应增加。";
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
        public static SvDataJd GetXlDcpjxl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "平均心率";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次/min";
            jd.RefRange = "50~65次/min";
            jd.Pph = "";
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
        public static SvDataJd GetHxDcjzhx(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "当次基准呼吸";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次/min";
            jd.RefRange = "12~20次/min";
            jd.Pph = "基准呼吸是评价肺功能的基础指标之一，其参考范围为9~18次/分钟，感染发热、睡眠呼吸障碍、缺血缺氧或某些药物的影响可能会导致呼吸加快。";
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
        public static SvDataJd GetHxCqjzhx(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "长期基准呼吸";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次/min";
            jd.RefRange = "10~18次/min";
            jd.Pph = "长期基准呼吸反映了一段时间内的肺功能情况，参考范围为9~18次/分钟，长期运动者肺活量大，呼吸频率比普通人要低；而长期不运动者、吸烟、有心肺功能受损的患者长期基准呼吸通常较高。";
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
        public static SvDataJd GetHxDcpjhx(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "平均呼吸";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次/min";
            jd.RefRange = "10~18次/min";
            jd.Pph = "";
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
        public static SvDataJd GetHxZtcs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "呼吸暂停次数";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString() + "次";
            jd.RefRange = "0~30次";
            jd.Pph = "睡眠过程中呼吸运动停止10秒钟以上记录为呼吸暂停，主要特征表现为打鼾。引起睡眠呼吸暂停的因素中最常见的是肥胖，呼吸道炎症（如感冒、鼻炎等）和气道解剖学异常（如鼻甲肿大、肿物等）也会导致呼吸暂停。呼吸暂停不超过15次是最好的，呼吸暂停较多时会导致血氧饱和度下降，血液黏稠度增加，高血压、冠心病的发病风险升高。";
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
        public static SvDataJd GetHrvXzznl(decimal val, decimal jzz, int reportCount, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "心脏总能量";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.Pph = "心脏总能量反映了心脏的整体储备功能，过低提示心脏储备能量不足，过高提示心脏负荷过大。人群的参考范围为3000~6000，并随着年龄的增加，心脏总能量降低。 心功能不全、心衰、高血压长期用药者，糖尿病患者，久坐不动者心脏总能量降低；高血压未用药者，停搏患者，心律失常患者，过量运动者心脏总能量升高。";
            long var1 = 2000;
            long var2 = 3000;
            long var3 = 6000;
            long var4 = 8000;

            if (reportCount >= 8)
            {
                var1 = D46Long(jzz * 0.5m);
                var2 = D46Long(jzz * 0.75m);
                var3 = D46Long(jzz * 1.5m);
                var4 = D46Long(jzz * 2m);
            }


            jd.RefRange = var2 + "~" + var3;

            if (val < var1)
            {
                jd.Set("偏低", "↓↓", CA_3);
            }
            else if (val >= var1 && val < var2)
            {
                jd.Set("低", "↓", CA_2);
            }
            else if (val >= var2 && val < var3)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val >= var3 && val < var4)
            {
                jd.Set("高", "↑", CA_2);
            }
            else if (val >= var4)
            {
                jd.Set("偏高", "↑↑", CA_3);
            }

            return jd;
        }
        public static SvDataJd GetHrvJgsjzlzs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "交感神经张力";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.RefRange = "500~1200";
            jd.Pph = "反映了交感神经的调节张力，交感神经是保证人在紧张状态下生理功能的重要因素，参考范围为600~1800，随着年龄的增大，交感神经张力降低。 交感神经张力过高时能引起末梢血管收缩、心跳加快、新陈代谢亢进，过低则容易出现乏力、精神萎靡。交感神经张力指数过高通常发生在高血压患者血压控制不佳时，房颤、停搏、房室传导阻滞发生时，情绪过激或运动过量时。而交感神经张力指数降低则发生在高血压、糖尿病患者发生自主神经损伤时，心律失常、冠心病患者，年龄增大心功能减弱者也会降低。";
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
        public static SvDataJd GetHrvMzsjzlzs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "迷走神经张力";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.RefRange = "520~1200";
            jd.Pph = "反映了迷走神经的调节张力，其与交感神经的功能是相互拮抗的，迷走神经保持人在安静时的平衡状态，迷走神经张力指数的参考范围为600-1200，并随着年龄的增大而降低。 迷走神经的主要的生理功能是使心跳减慢、血压降低、促进胃肠蠕动和消化腺分泌。迷走神经张力过高时可能导致胃肠过激、肠易激综合征等，导致便秘、腹泻等；高血压、冠心病患者这个值太高可能出现早搏、房颤或其他心律不齐。迷走神经张力指数过低则可能出现胃肠功能减弱、便秘，糖尿病患者则提示出现了自主神经受损，喝酒、服用抑制神经的药物则会导致迷走神经张力降低。";
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
        public static SvDataJd GetHrvZzsjzlzs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "自主神经张力";
            jd.Value = val.ToString();
            jd.ValueText = val.ToString();
            jd.RefRange = "0.7~1.3";
            jd.Pph = "自主神经平衡指数，反映了交感神经和迷走神经的平衡性，正常人在睡眠状态下，交感神经活性降低，迷走神经活性升高，参考范围为0.8~1.2。 自主神经平衡指数升高，有以下情况：提示高血压风险升高，高血压患者血压波动、情绪烦躁不安、焦虑恐惧等。对于自主神经平衡指数降低的，原患糖尿病者提示出现明显自主神经受损，可能出现血糖控制不佳。血压降低或低血压者此值也会降低。";
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
        public static SvDataJd GetHrvHermzs(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "12";
            jd.Value = val.ToString();
            jd.Name = "内分泌指数";
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
            if (val < 600)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (val >= 600 && val < 1800)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (val >= 1800)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }
        public static SvDataJd GetSmScore(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠值";
            jd.Value = val;
            jd.ValueText = val.ToString();
            jd.RefRange = "0~100";
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, yAxisMin = 0, yAxisMax = 100, yAxisSplitNumber = 10, markLine = new { yAxis = 70 } };

            if (val < 30)
            {
                jd.Set("低", "↓↓", CB_1);
            }
            else if (30 <= val && val < 60)
            {
                jd.Set("偏低", "↓", CB_2);
            }
            else if (60 <= val && val < 80)
            {
                jd.Set("中等", "-", CB_3);
            }
            else if (80 <= val && val < 90)
            {
                jd.Set("良好", "-", CB_4);
            }
            else if (90 <= val && val <= 100)
            {
                jd.Set("优秀", "-", CB_5);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 30, Color = CB_1, Tips = "差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 30, Max = 60, Color = CB_2, Tips = "较差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 60, Max = 80, Color = CB_3, Tips = "中等" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 80, Max = 90, Color = CB_4, Tips = "良好" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 90, Max = 100, Color = CB_5, Tips = "优秀" });
            }

            return jd;
        }
        public static SvDataJd GetHealthScore(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "健康值";
            jd.Value = val;
            jd.RefRange = "0~100";

            if (val < 30)
            {
                jd.Set("低", "↓↓", CB_1);
            }
            else if (30 <= val && val < 60)
            {
                jd.Set("偏低", "↓", CB_2);
            }
            else if (60 <= val && val < 80)
            {
                jd.Set("中等", "-", CB_3);
            }
            else if (80 <= val && val < 90)
            {
                jd.Set("良好", "-", CB_4);
            }
            else if (90 <= val && val <= 100)
            {
                jd.Set("优秀", "-", CB_5);
            }

            return jd;
        }
        public static SvDataJd GetSmSmxl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠效率";
            jd.Value = val;
            jd.ValueText = val.ToString("0.#####") + "%";
            jd.RefRange = "85~100";
            jd.Pph = "高效的睡眠，对于增强智力和体力起着重要作用，睡眠效率达到85%为正常，大于90%为优秀。难入睡者入睡需时太长，易醒者在睡眠中清醒次数增多，都是导致睡眠效率不高的直接原因。";
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 25, 50, 85, 100 }, yAxisMin = 0, yAxisMax = 100, yAxisSplitNumber = 5, yAxisMarkLine = 80 };
            if (val < 50)
            {
                jd.Set("低", "↓↓", CB_1);
            }
            else if (val >= 50 && val < 85)
            {
                jd.Set("偏低", "↓", CB_3);
            }
            else if (val >= 85)
            {
                jd.Set("正常", "-", CB_5);
            }


            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 50, Color = CB_1, Tips = "低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 50, Max = 85, Color = CB_3, Tips = "偏低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 85, Max = 100, Color = CB_5, Tips = "正常" });
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
        public static SvDataJd GetSmSdsmbl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "深睡眠";
            jd.Value = val;
            jd.ValueText = val.ToString("0.#####") + "%";
            jd.RefRange = "15~25";
            jd.Pph = "深睡眠，也称为“黄金睡眠”，深睡眠对机体细胞修复、生长激素分泌、增强免疫、消除疲劳、精力恢复有重要作用。正常人的深睡眠比例为15%~25%，缺乏深睡眠将可能出现代谢紊乱、免疫力下降、精神疲劳等；深睡眠比例长期过高则可能是某些疾病发作的信号，需寻找专业医生的指导。";
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
        }

        public static SvDataJd GetSmRemsmbl(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Name = "REM睡眠";
            jd.Value = val;
            jd.ValueText = val.ToString("0.#####") + "%";
            jd.RefRange = "15~25";
            jd.Pph = "REM睡眠，即快速眼动睡眠，得名于此睡眠期内眼球的特征性快速运动。睡梦多发生在这个阶段，如果在这个阶段被唤醒，大多数人都可能说自己在做梦。快速眼动睡眠对记忆力形成、情绪调节、维持精神健康起着重要作用。正常人的REM睡眠比例为15%~25%，REM睡眠不足会影响记忆形成，导致记忆力下降，并且增加全因死亡风险和老年痴呆风险。REM睡眠过多则会影响身体而得不到完全恢复，易怒很难控制自己情绪。";
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
        }

        public static SvDataJd GetZsGmYq(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "9";
            jd.Name = "孕气指数";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } }; ;
            if (val < 20)
            {
                jd.Set("好", "↓↓", CA_1);
            }
            else if (val >= 20 && val < 40)
            {
                jd.Set("较好", "↓", CA_2);
            }
            else if (val >= 40 && val < 55)
            {
                jd.Set("中等", "-", CA_0);
            }
            else if (val >= 50 && val < 70)
            {
                jd.Set("偏差", "-", CA_0);
            }
            else if (val >= 70 && val < 85)
            {
                jd.Set("较差", "-", CA_0);
            }
            else if (val >= 85 && val <= 100)
            {
                jd.Set("差", "-", CA_0);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 20, Color = CA_1, Tips = "好" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 20, Max = 40, Color = CA_2, Tips = "较好" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 40, Max = 55, Color = CA_3, Tips = "中等" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 50, Max = 70, Color = CA_4, Tips = "偏差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 85, Color = CA_3, Tips = "较差" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 85, Max = 100, Color = CA_4, Tips = "差" });
            }

            return jd;
        }

        public static SvDataJd GetZsGmYp(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "10";
            jd.Name = "易胖指数";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } }; ;
            if (val < 30)
            {
                jd.Set("较低", "-", CA_0);
            }
            else if (val >= 30 && val < 50)
            {
                jd.Set("中等", "↑", CA_2);
            }
            else if (val > 50 && val <= 70)
            {
                jd.Set("偏高", "↑", CA_3);
            }
            else if (val > 70 && val <= 100)
            {
                jd.Set("较高", "↑", CA_4);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 30, Color = CA_0, Tips = "较低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 30, Max = 50, Color = CA_2, Tips = "中等" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 50, Max = 70, Color = CA_3, Tips = "偏高" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CA_4, Tips = "较高" });
            }

            return jd;
        }

        public static SvDataJd GetZsGmSr(decimal val, bool isGetRefRanges = false, object lastVals = null)
        {
            var jd = new SvDataJd();
            jd.Id = "11";
            jd.Name = "水润指数";
            jd.Value = val.ToString("0.#####");
            jd.ValueText = val.ToString("0.#####");
            jd.Chat = new { Data = lastVals, yAxisLabel = new int[] { 0, 30, 50, 70, 90, 100 }, markLine = new { yAxis = 70 } }; ;
            if (val < 30)
            {
                jd.Set("较低", "-", CA_1);
            }
            else if (val >= 30 && val < 50)
            {
                jd.Set("偏低", "↓", CA_2);
            }
            else if (val >= 50 && val < 70)
            {
                jd.Set("中等", "↑", CA_0);
            }
            else if (val >= 70 && val <= 100)
            {
                jd.Set("较高", "↑", CA_0);
            }

            if (isGetRefRanges)
            {
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 0, Max = 30, Color = CA_1, Tips = "较低" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 30, Max = 50, Color = CA_2, Tips = "中等" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 50, Max = 70, Color = CA_3, Tips = "偏高" });
                jd.RefRanges.Add(new SvDataJd.RefRangeArea { Min = 70, Max = 100, Color = CA_4, Tips = "较高" });
            }
            return jd;
        }


    }
}
