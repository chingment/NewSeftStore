using LocalS.BLL.Mq;
using Lumos;
using Lumos.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace WebApiMerch
{
    public class OwnStatisticsTrackerAttribute : BaseStatisticsTrackerAttribute
    {
        public override string CurrentUserId
        {
            get
            {
                return OwnApiRequest.TokenInfo.UserId;
            }
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            base.OnActionExecuted(actionContext);

            //MqFactory.Global.PushAccessLog(GuidUtil.New(), null);
        }
    }
}