using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
