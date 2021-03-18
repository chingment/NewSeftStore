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

        public static double Covevt2Hour(long seconds)
        {
            double hour = seconds / 3600;

            return hour;
        }

        public static string GetHourText(double hour)
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
            jd.Value = val.ToString();

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
            jd.Value = val.ToString();

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
            jd.Value = val.ToString();

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

            if (val == 0)
            {
                jd.Value = "";
                jd.Set("", "", CA_1);

                return jd;
            }

            jd.Value = val.ToString();

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


            val = 100 - val;
            jd.Value = val.ToString();

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
            jd.Value = val.ToString();

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

        public static SvDataJd GetHxZtAhizs(decimal val)
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

        public static SvDataJd GetSmSmsc(long val)
        {
            var jd = new SvDataJd();

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

        public static SvDataJd GetSmQdsmsc(long val)
        {
            var jd = new SvDataJd();

            var hour = Covevt2Hour(val);
            jd.Value = GetHourText(hour);

            jd.RefRange = "3.5~5.4h";
            if (hour < 3.5)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (hour >= 3.5 && hour <= 5.4)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (hour > 5.4)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }

        public static SvDataJd GetSmSdsmsc(long val)
        {
            var jd = new SvDataJd();

            var hour = Covevt2Hour(val);
            jd.Value = GetHourText(hour);
            jd.RefRange = "1.05~2.25h";
            if (hour < 1.05)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (hour >= 1.05 && hour <= 2.25)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (hour > 2.25)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }

        public static SvDataJd GetSmSemqsc(long val)
        {
            var jd = new SvDataJd();

            var hour = Covevt2Hour(val);

            jd.Value = GetHourText(hour);
            jd.RefRange = "1.05~2.25h";
            if (hour < 1.05)
            {
                jd.Set("低", "↓", CA_1);
            }
            else if (hour >= 1.05 && hour <= 2.25)
            {
                jd.Set("正常", "-", CA_0);
            }
            else if (hour > 2.25)
            {
                jd.Set("高", "↑", CA_1);
            }

            return jd;
        }

        public static SvDataJd GetSmSmzq(long val)
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

        public static SvDataJd GetSmTdcs(long val)
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

        public static SvDataJd GetXlDcjzxl(int val)
        {
            var jd = new SvDataJd();
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

        public static SvDataJd GetXlCqjzxl(int val)
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

        public static SvDataJd GetXlDcpjxl(int val)
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


        public static SvDataJd GetHxDcjzhx(int val)
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

        public static SvDataJd GetHxCqjzhx(int val)
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

        public static SvDataJd GetHxDcPj(int val)
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

        public static SvDataJd GetHxZtcs(int val)
        {
            var jd = new SvDataJd();
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


        public static SvDataJd GetHrvXzznl(int val)
        {
            var jd = new SvDataJd();
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

        public static SvDataJd GetHrvJgsjzlzs(int val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "500~1200";
            if (val < 2300)
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

        public static SvDataJd GetHrvMzsjzlzs(int val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();
            jd.RefRange = "520~1200";
            if (val < 2300)
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
    }
}
