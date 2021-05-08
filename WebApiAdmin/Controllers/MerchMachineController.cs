﻿using LocalS.Service.Api.Admin;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAdmin.Controllers
{
    public class MerchMachineController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse InitGetList()
        {
            var result = AdminServiceFactory.MerchMachine.InitGetList(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupMerchMachineGetList rup)
        {
            var result = AdminServiceFactory.MerchMachine.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]RupMerchMachineInitEdit rup)
        {
            var result = AdminServiceFactory.MerchMachine.InitEdit(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopMerchMachineEdit rop)
        {
            var result = AdminServiceFactory.MerchMachine.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindOffMerch([FromBody]RopMerchMachineBindOffMerch rop)
        {
            var result = AdminServiceFactory.MerchMachine.BindOffMerch(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindOnMerch([FromBody]RopMerchMachineBindOnMerch rop)
        {
            var result = AdminServiceFactory.MerchMachine.BindOnMerch(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
