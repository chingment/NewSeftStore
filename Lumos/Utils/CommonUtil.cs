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
                return null;
            }

            if (phone.Length < 11)
            {
                return null;
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
    }
}
