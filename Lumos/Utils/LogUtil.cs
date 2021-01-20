using log4net;
using System;
using System.Reflection;
using System.Web;

namespace Lumos
{
    public static class LogUtil
    {
        private static ILog log2 = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void SetTrackId()
        {
            SetTrackId(null);
        }

        public static void SetTrackId(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            }

            LogicalThreadContext.Properties["trackid"] = token;
        }
        private static ILog GetLog()
        {

            Type type = MethodBase.GetCurrentMethod().DeclaringType;

            var trace = new System.Diagnostics.StackTrace();

            string name = type.Name;
            if (trace.FrameCount >= 3)
            {
                System.Reflection.MethodBase mb = trace.GetFrame(2).GetMethod();
                type = mb.DeclaringType;

                name = string.Format("{0}.{1}", mb.DeclaringType.FullName, mb.Name);
            }

            return log4net.LogManager.GetLogger(name);
        }

        public static void Info(string tag, string msg)
        {
            string r_msg = "\r\n";
            GetLog().Info(r_msg + tag + msg);
        }

        public static void Info(string msg)
        {
            Info("", msg);
        }

        public static void Warn(string msg)
        {
            GetLog().Warn(msg);
        }

        public static void Error(string msg)
        {
            GetLog().Error(msg);
        }

        public static void Error(string msg, Exception ex)
        {
            Error("", msg, ex);
        }

        public static void Error(string tag, string msg, Exception ex)
        {
            GetLog().Error(tag + msg, ex);
        }
    }
}
