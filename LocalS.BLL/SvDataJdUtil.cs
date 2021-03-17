using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SvDataJdUtil
    {
        public readonly string CA_1 = "#e80113";
        public readonly string CA_2 = "#f86872";
        public readonly string CA_3 = "#5fa5dc";
        public readonly string CA_4 = "#0368b8";
        public readonly string CA_5 = "#59c307";

        /// <summary>
        /// 免疫力指数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public SvDataJd GetMylzs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 29)
            {
                jd = new SvDataJd(val.ToString(), "差", "", CA_1);
            }
            else if (val >= 30 && val <= 49)
            {
                jd = new SvDataJd(val.ToString(), "较差", "", CA_2);
            }
            else if (val >= 50 && val <= 69)
            {
                jd = new SvDataJd(val.ToString(), "中等", "", CA_3);
            }
            else if (val >= 70 && val <= 89)
            {
                jd = new SvDataJd(val.ToString(), "较好", "", CA_4);
            }
            else if (val >= 90 && val <= 100)
            {
                jd = new SvDataJd(val.ToString(), "好", "", CA_5);
            }
            return jd;
        }

        public SvDataJd GetMylGrfx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 19)
            {
                jd = new SvDataJd(val.ToString(), "低", "", CA_5);
            }
            else if (val >= 20 && val <= 39)
            {
                jd = new SvDataJd(val.ToString(), "较低", "", CA_4);
            }
            else if (val >= 40 && val <= 69)
            {
                jd = new SvDataJd(val.ToString(), "中等", "", CA_3);
            }
            else if (val >= 70 && val <= 84)
            {
                jd = new SvDataJd(val.ToString(), "较高", "", CA_2);
            }
            else if (val >= 85 && val <= 100)
            {
                jd = new SvDataJd(val.ToString(), "高", "", CA_1);
            }
            return jd;
        }

        public SvDataJd GetMbGxygk(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 39)
            {
                jd = new SvDataJd(val.ToString(), "差", "", CA_1);
            }
            else if (val >= 40 && val <= 69)
            {
                jd = new SvDataJd(val.ToString(), "一般", "", CA_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd = new SvDataJd(val.ToString(), "好", "", CA_5);
            }
            return jd;
        }

        public SvDataJd GetMbGxbgk(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 39)
            {
                jd = new SvDataJd(val.ToString(), "差", "", CA_1);
            }
            else if (val >= 40 && val <= 69)
            {
                jd = new SvDataJd(val.ToString(), "一般", "", CA_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd = new SvDataJd(val.ToString(), "好", "", CA_5);
            }
            return jd;
        }

        public SvDataJd GetMbTlbgk(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 39)
            {
                jd = new SvDataJd(val.ToString(), "差", "", CA_1);
            }
            else if (val >= 40 && val <= 69)
            {
                jd = new SvDataJd(val.ToString(), "一般", "", CA_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd = new SvDataJd(val.ToString(), "好", "", CA_5);
            }
            return jd;
        }

        public SvDataJd GetQxxlKynl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 29)
            {
                jd = new SvDataJd(val.ToString(), "差", "", CA_1);
            }
            else if (val >= 30 && val <= 69)
            {
                jd = new SvDataJd(val.ToString(), "一般", "", CA_3);
            }
            else if (val >= 70 && val <= 100)
            {
                jd = new SvDataJd(val.ToString(), "好", "", CA_5);
            }
            return jd;
        }

        public SvDataJd GetJbfxXlscfx(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 29)
            {
                jd = new SvDataJd(val.ToString(), "高风险", "", CA_1);
            }
            else if (val >= 30 && val <= 49)
            {
                jd = new SvDataJd(val.ToString(), "中风险", "", CA_2);
            }
            else if (val >= 50 && val <= 180)
            {
                jd = new SvDataJd(val.ToString(), "低风险", "", CA_5);
            }
            else if (val >= 180 && val <= 220)
            {
                jd = new SvDataJd(val.ToString(), "中风险", "", CA_2);
            }
            else if (val >= 221)
            {
                jd = new SvDataJd(val.ToString(), "高风险", "", CA_1);
            }
            return jd;
        }

        public SvDataJd GetJbfxXljsl(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val <= 2.5m)
            {
                jd = new SvDataJd(val.ToString(), "过低", "", CA_1);
            }
            else if (val >= 2.5m && val <= 4.6m)
            {
                jd = new SvDataJd(val.ToString(), "偏低", "", CA_2);
            }
            else if (val >= 4.6m && val <= 12m)
            {
                jd = new SvDataJd(val.ToString(), "正常", "", CA_5);
            }
            else if (val >= 12m && val <= 20m)
            {
                jd = new SvDataJd(val.ToString(), "偏高", "", CA_2);
            }
            else if (val > 20m)
            {
                jd = new SvDataJd(val.ToString(), "过高", "", CA_1);
            }
            return jd;
        }

        public SvDataJd GetHxZtAhizs(decimal val)
        {
            var jd = new SvDataJd();
            jd.Value = val.ToString();

            if (val < 5)
            {
                jd = new SvDataJd(val.ToString(), "正常", "", CA_5);
            }
            else if (val >= 5 && val < 15)
            {
                jd = new SvDataJd(val.ToString(), "轻度", "", CA_3);
            }
            else if (val >= 15 && val < 30)
            {
                jd = new SvDataJd(val.ToString(), "中度", "", CA_2);
            }
            else if (val >= 30)
            {
                jd = new SvDataJd(val.ToString(), "重度", "", CA_1);
            }
            return jd;
        }
    }
}
