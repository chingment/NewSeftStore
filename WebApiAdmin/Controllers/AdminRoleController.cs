﻿using LocalS.Service.Api.Admin;
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
    public class AdminRoleController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupSysRoleGetList rup)
        {
            IResult result = AdminServiceFactory.SysRole.GetList(this.CurrentUserId,Enumeration.BelongSite.Admin,rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            IResult result = AdminServiceFactory.SysRole.InitAdd(this.CurrentUserId, Enumeration.BelongSite.Admin);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopSysRoleAdd rop)
        {
            IResult result = AdminServiceFactory.SysRole.Add(this.CurrentUserId, Enumeration.BelongSite.Admin, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            IResult result = AdminServiceFactory.SysRole.InitEdit(this.CurrentUserId, Enumeration.BelongSite.Admin, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopSysRoleEdit rop)
        {
            IResult result = AdminServiceFactory.SysRole.Edit(this.CurrentUserId, Enumeration.BelongSite.Admin, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
