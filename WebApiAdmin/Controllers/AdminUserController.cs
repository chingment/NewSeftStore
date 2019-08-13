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
            IResult result = AdminServiceFactory.AdminUser.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            IResult result = AdminServiceFactory.AdminUser.InitAdd(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopAdminUserAdd rop)
        {
            IResult result = AdminServiceFactory.AdminUser.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string userId)
        {
            IResult result = AdminServiceFactory.AdminUser.InitEdit(this.CurrentUserId, userId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopAdminUserEdit rop)
        {
            IResult result = AdminServiceFactory.AdminUser.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
