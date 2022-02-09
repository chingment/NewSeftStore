using NPinyin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Lumos
{
    public class PregnancyWeekModel
    {
        public int Week { get; set; }

        public int Day { get; set; }
    }

    public static class CommonUtil
    {
        #region "获取Ip"
        /// <summary>
        /// 获取Ip
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        public static string GetIP()
        {
            string userIP = "";
            try
            {
                HttpContext rq = HttpContext.Current;
                HttpRequest Request = HttpContext.Current.Request;
                // 如果使用代理，获取真实IP
                if (rq.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                {
                    userIP = rq.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    userIP = rq.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                if (userIP == null || userIP == "")
                {
                    userIP = rq.Request.UserHostAddress;
                }
            }
            catch
            {
                userIP = "error ip";
            }
            return userIP;

        }
        #endregion

        #region "获取浏览器Agent"
        /// <summary>
        /// 获取Ip
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        public static string GetBrowserInfo()
        {

            HttpContext rq = HttpContext.Current;

            string info = "";
            if (rq != null)
            {
                info = rq.Request.ServerVariables["HTTP_USER_AGENT"];
            }
            return info;
        }
        #endregion

        public static bool CanViewErrorStackTrace()
        {
            string[] canViewIp = new string[] { "127.0.0.1", "::1" };


            string ip = CommonUtil.GetIP();

            if (canViewIp.Contains(ip))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsDateTime(string strDate)
        {
            if (strDate == null)
            {
                return false;
            }
            else
            {
                DateTime dtDate;
                bool bValid = true;
                try
                {
                    dtDate = DateTime.Parse(strDate);
                }
                catch (FormatException)
                {
                    // 如果解析方法失败则表示不是日期性数据
                    bValid = false;
                }
                return bValid;
            }
        }

        public static bool IsInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            else
            {
                Int32 dtDate;
                bool bValid = true;
                try
                {
                    dtDate = Int32.Parse(str);
                }
                catch (FormatException)
                {
                    // 如果解析方法失败则表示不是日期性数据
                    bValid = false;
                }
                return bValid;
            }
        }

        public static bool IsEmpty(string strDate)
        {
            if (strDate == null)
            {
                return true;
            }

            strDate = strDate.Trim();

            if (string.IsNullOrEmpty(strDate))
            {
                return true;
            }

            return false;


        }

        public static DateTime? ConverToStartTime(string strDate)
        {
            DateTime? d = null;
            try
            {
                if (strDate.Trim() != "")
                {
                    if (CommonUtil.IsDateTime(strDate))
                    {
                        strDate = DateTime.Parse(strDate).ToShortDateString();
                    }

                    d = DateTime.Parse(strDate + " 00:00:00.000");
                }
            }
            catch
            {

            }
            return d;

        }

        public static DateTime? ConverToEndTime(string strDate)
        {
            DateTime? d = null;
            try
            {
                if (strDate.Trim() != "")
                {
                    if (CommonUtil.IsDateTime(strDate))
                    {
                        strDate = DateTime.Parse(strDate).ToShortDateString();
                    }

                    d = DateTime.Parse(strDate + " 23:59:59");
                }
            }
            catch
            {

            }
            return d;

        }

        public static DateTime? ConverToDateTime(string strDate)
        {
            DateTime? d = null;
            try
            {
                d = DateTime.Parse(strDate);
            }
            catch
            {

            }
            return d;

        }

        public static bool IsNumber(string str)
        {
            if (str == null || str.Length == 0)    //验证这个参数是否为空  
                return false;                           //是，就返回False  
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例  
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里  

            foreach (byte c in bytestr)                   //遍历这个数组里的内容  
            {
                if (c < 48 || c > 57)                          //判断是否为数字  
                {
                    return false;                              //不是，就返回False  
                }
            }
            return true;                                        //是，就返回True  
        }

        public static string GetNumberAlpha(string source)
        {
            string pattern = "[A-Za-z0-9]";
            string strRet = "";
            MatchCollection results = Regex.Matches(source, pattern);
            foreach (var v in results)
            {
                strRet += v.ToString();
            }
            return strRet;
        }

        public static string GetPingYin(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;


            string str1 = Pinyin.GetPinyin(str, Encoding.GetEncoding("GB2312"));
            if (string.IsNullOrEmpty(str1))
                return null;

            str1 = GetNumberAlpha(str1);

            if (!string.IsNullOrEmpty(str))
                str1 = str1.ToUpper();

            return str1;

        }

        public static string GetPingYinIndex(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;


            string str1 = Pinyin.GetInitials(str);
            if (string.IsNullOrEmpty(str1))
                return null;

            str1 = GetNumberAlpha(str1);

            if (!string.IsNullOrEmpty(str))
                str1 = str1.ToUpper();

            return str1;

        }

        public static string GetIpAddress(HttpRequestBase myRequest)
        {
            string userHostAddress = myRequest.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(userHostAddress))
            {
                if (myRequest.ServerVariables["HTTP_VIA"] != null)
                    userHostAddress = myRequest.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = myRequest.UserHostAddress;
            }

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }


        public static string GetEncryptionPhoneNumber(string phone)
        {

            if (string.IsNullOrEmpty(phone))
            {
                return "";
            }

            if (phone.Length < 11)
            {
                return "";
            }

            try
            {
                return Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            catch (Exception)
            {
                return "未知号码";
            }

        }

        public static string GetCnWeekDayName(DateTime startTime)
        {
            DateTime dateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

            int days = Convert.ToInt32(Math.Floor((startTime - dateTime).TotalDays));

            string week = "";
            if (days == 0)
            {
                week = "今天";
            }
            else if (days == 1)
            {
                week = "明天";
            }
            else if (days == 2)
            {
                week = "后天";
            }
            else
            {
                string[] cnWeekDayNames = { "日", "一", "二", "三", "四", "五", "六" };

                week = "星期" + cnWeekDayNames[(int)startTime.DayOfWeek];
            }

            return week;
        }


        public static string ConvetMD5IN32B(string str)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] bytStr = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            string encryptStr = "";
            for (int i = 0; i < bytStr.Length; i++)
            {
                encryptStr = encryptStr + bytStr[i].ToString("x").PadLeft(2, '0');
            }

            return encryptStr;
        }

        public static int GetAgeByBirthdate(DateTime birthdate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthdate.Year;
            if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
            {
                age--;
            }
            return age < 0 ? 0 : age;
        }

        public static bool GetTimeSpan(string timeStr, string time1, string time2)
        {
            DateTime t1 = Convert.ToDateTime(timeStr);

            return GetTimeSpan(t1, time1, time2);

        }

        public static bool GetTimeSpan(DateTime timeStr, string time1, string time2)
        {
            //判断当前时间是否在工作时间段内
            string _strWorkingDayAM = time1;//工作时间上午08:30
            string _strWorkingDayPM = time2;
            TimeSpan dspWorkingDayAM = DateTime.Parse(_strWorkingDayAM).TimeOfDay;
            TimeSpan dspWorkingDayPM = DateTime.Parse(_strWorkingDayPM).TimeOfDay;

            //string time1 = "2017-2-17 8:10:00";
            DateTime t1 = timeStr;

            TimeSpan dspNow = t1.TimeOfDay;
            if (dspWorkingDayAM.Hours < dspWorkingDayPM.Hours)
            {
                if (dspNow > dspWorkingDayAM && dspNow < dspWorkingDayPM)
                {
                    return true;
                }
            }
            else
            {
                if (dspNow > dspWorkingDayPM && dspNow > dspWorkingDayAM)
                {
                    return true;
                }
                else if (dspNow < dspWorkingDayPM)
                {
                    return true;
                }
            }

            return false;
        }


        public static PregnancyWeekModel GetPregnancyWeeks(DateTime dtBegin, DateTime dtEnd)
        {
            var weekModel = new PregnancyWeekModel();

            if (dtEnd <= dtBegin)
            {
                return weekModel;
            }

            double totalDays = (dtEnd - dtBegin).TotalDays;

            double week = totalDays / 7;

            weekModel.Week = (int)Math.Floor(week);
            weekModel.Day = (int)(totalDays - weekModel.Week * 7);

            return weekModel;
        }

        public static DateTime GetPregnancyTime(int week, int day)
        {


            int toal = week * 7 + 2;

            DateTime t = DateTime.Now.AddDays(toal);
            //double week = totalDays / 7;

            //weekModel.Week = (int)Math.Floor(week);
            //weekModel.Day = (int)(totalDays - weekModel.Week * 7);

            return t;
        }

        public static DateTime MonthMinDateTime(string month)
        {
            return DateTime.Parse(month + "-01");
        }

        public static DateTime MonthMaxDateTime(string month)
        {
            DateTime t1 = DateTime.Parse(month + "-01");
            int days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(t1.Year, t1.Month);

            DateTime t2 = DateTime.Parse(month + "-" + days+ " 23:59:59");


            return t2;
        }
    }
}
