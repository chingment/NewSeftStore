﻿using log4net;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApiStoreTerm.Controllers;

namespace WebApiStoreTerm
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            LogUtil.Info("应用程序开始");
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {

            HttpApplication ap = sender as HttpApplication;
            System.Exception ex = ap.Server.GetLastError();

            var httpStatusCode = (ex is HttpException) ? (ex as HttpException).GetHttpCode() : 500; //这里仅仅区分两种错误 
            switch (httpStatusCode)
            {
                case 404:
                    break;
                default:
                    LogUtil.Error("应用程序捕捉到异常", ex);
                    break;
            }
        }
    }
}
