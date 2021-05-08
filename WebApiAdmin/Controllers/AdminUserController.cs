using LocalS.Service.Api.Admin;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAdmin.Controllers
{
    public class AdminUserController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupAdminUserGetList rup)
        {
            var result = AdminServiceFactory.AdminUser.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            var result = AdminServiceFactory.AdminUser.InitAdd(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopAdminUserAdd rop)
        {
            var result = AdminServiceFactory.AdminUser.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            var result = AdminServiceFactory.AdminUser.InitEdit(this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopAdminUserEdit rop)
        {
            var result = AdminServiceFactory.AdminUser.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
