using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Lumos.Web.Mvc
{


    public abstract class BaseController : Controller
    {
        public virtual string CurrentUserId { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            LogUtil.SetTrackId();
        }

        public BaseController()
        {

        }
    }
}
