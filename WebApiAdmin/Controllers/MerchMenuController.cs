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
    public class MerchMenuController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupSysMenuGetList rup)
        {
            var result = AdminServiceFactory.SysMenu.GetList(this.CurrentUserId, Enumeration.BelongSite.Merch, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd([FromUri]string pId)
        {
            var result = AdminServiceFactory.SysMenu.InitAdd(this.CurrentUserId, Enumeration.BelongSite.Merch, pId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopSysMenuAdd rop)
        {
            var result = AdminServiceFactory.SysMenu.Add(this.CurrentUserId, Enumeration.BelongSite.Merch, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            var result = AdminServiceFactory.SysMenu.InitEdit(this.CurrentUserId, Enumeration.BelongSite.Merch, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopSysMenuEdit rop)
        {
            var result = AdminServiceFactory.SysMenu.Edit(this.CurrentUserId, Enumeration.BelongSite.Merch, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Sort([FromBody]RopSysMenuSort rop)
        {
            var result = AdminServiceFactory.SysMenu.Sort(this.CurrentUserId, Enumeration.BelongSite.Merch, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
