using LocalS.BLL.Mq;
using LocalS.BLL.Task;
using LocalS.Entity;
using log4net;
using Lumos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Topshelf;

namespace Task4Mq2Global
{
    class Program
    {
        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log.InfoFormat("程序开始");

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.RunAsLocalSystem();
                x.Service<ServiceRunner>();
                x.SetDescription(string.Format("{0} Ver:{1}", System.Configuration.ConfigurationManager.AppSettings.Get("ServiceName"), System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
                x.SetDisplayName(System.Configuration.ConfigurationManager.AppSettings.Get("ServiceDisplayName"));
                x.SetServiceName(System.Configuration.ConfigurationManager.AppSettings.Get("ServiceName"));
                x.EnablePauseAndContinue();
            });
        }
    }
}

