using LocalS.Service.Api.Admin;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace WebApiAdmin.Controllers
{
    public class AdminMenuController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupSysMenuGetList rup)
        {
            IResult result = AdminServiceFactory.SysMenu.GetList(this.CurrentUserId, Enumeration.BelongSite.Admin, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd([FromUri]string pMenuId)
        {
            IResult result = AdminServiceFactory.SysMenu.InitAdd(this.CurrentUserId, Enumeration.BelongSite.Admin, pMenuId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopSysMenuAdd rop)
        {
            IResult result = AdminServiceFactory.SysMenu.Add(this.CurrentUserId, Enumeration.BelongSite.Admin, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string menuId)
        {
            IResult result = AdminServiceFactory.SysMenu.InitEdit(this.CurrentUserId, Enumeration.BelongSite.Admin, menuId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopSysMenuEdit rop)
        {
            IResult result = AdminServiceFactory.SysMenu.Edit(this.CurrentUserId, Enumeration.BelongSite.Admin, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
