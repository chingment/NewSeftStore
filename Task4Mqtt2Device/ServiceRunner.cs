﻿using Common.Logging;
using LocalS.BLL.Task;
using Lumos;
using Lumos.Redis;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using System;
using Topshelf;

namespace Task4Mqtt2Device
{
    public sealed class ServiceRunner : ServiceControl, ServiceSuspend
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ServiceRunner));

        private string ServiceName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings.Get("ServiceName");
            }
        }

        public ServiceRunner()
        {

        }

        public bool Start(HostControl hostControl)
        {
            _logger.Info(string.Format("{0} Start", ServiceName));

            string taskProvider = System.Configuration.ConfigurationManager.AppSettings["custom:Task4Provider"];
            Task4Factory.Launcher.Launch(taskProvider);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _logger.Info(string.Format("{0} Stop", ServiceName));
            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            _logger.Info(string.Format("{0} Continue", ServiceName));
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            _logger.Info(string.Format("{0} Pause", ServiceName));
            return true;
        }

    }
}