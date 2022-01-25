using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SvDataJdUtil
    {
        public readonly static string CA_0 = "#666666";
        public readonly static string CA_1 = "#e80113";
        public readonly static string CA_2 = "#f86872";
        public readonly static string CA_3 = "#5fa5dc";
        public readonly static string CA_4 = "#0368b8";
        public readonly static string CA_5 = "#59c307";

        public static decimal Covevt2Hour(decimal seconds)
        {
            decimal hour = seconds / 3600m;

            return hour;
        }

        public static string GetHourText(decimal hour)
        {
            if (hour <= 0)
                return "0";

            return hour.ToString("0.00");
        }

        /// <summary>
        /// 免疫力指数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
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

        public static SvDataJd GetSmSmsc(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "睡眠总时长";
            var hour = Covevt2Hour(val);
            jd.Value = GetHourText(hour);
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

        public static SvDataJd GetSmRsxs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "入睡需时";
            var hour = Covevt2Hour(val);
            jd.Value = GetHourText(hour);
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

        public static SvDataJd GetSmQdsmsc(decimal val)
        {
            var jd = new SvDataJd();

            var hour = Covevt2Hour(val);
            jd.Value = GetHourText(hour);

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

        public static SvDataJd GetSmSdsmsc(decimal val)
        {
            var jd = new SvDataJd();
            jd.Name = "深度睡眠时长";
            var hour = Covevt2Hour(val);
            jd.Value = GetHourText(hour);
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

            var hour = Covevt2Hour(val);

            jd.Value = GetHourText(hour);
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
            jd.Value = val;
            jd.RefRange = "0~100";
            if (val < 29)
            {
                jd.Set("差", "", "#e16d6d");
            }
            else if (val >= 30 && val <= 49)
            {
                jd.Set("较差", "", "#e68a8b");
            }
            else if (val >= 50 && val <= 69)
            {
                jd.Set("中等", "", "#f1b46d");
            }
            else if (val >= 70 && val <= 89)
            {
                jd.Set("较好", "", "#96a2dc");
            }
            else 
            {
                jd.Set("好", "", "#628DF2");
            }

            return jd;
        }
    }
}
