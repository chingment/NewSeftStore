using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreTerm.Controllers
{
    public class OwnController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse GetInfo([FromBody]LocalS.Service.Api.Account.RupOwnGetInfo rup)
        {
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.GetInfo(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UploadFingerVeinData([FromBody]LocalS.Service.Api.Account.RopUploadFingerVeinData rop)
        {
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.UploadFingerVeinData(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeleteFingerVeinData()
        {
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.DeleteFingerVeinData(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByFingerVein([FromBody]LocalS.Service.Api.Account.RopOwnLoginByFingerVein rop)
        {
            rop.Ip = CommonUtil.GetIP();
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.LoginByFingerVein(rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByAccount([FromBody]LocalS.Service.Api.Account.RopOwnLoginByAccount rop)
        {

            rop.Ip = CommonUtil.GetIP();

            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.LoginByAccount(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Logout([FromBody]LocalS.Service.Api.Account.RopOwnLogout rop)
        {
            if (rop == null)
            {
                rop = new LocalS.Service.Api.Account.RopOwnLogout();
            }
            rop.Ip = CommonUtil.GetIP();
            rop.AppId = AppId.STORETERM;
            rop.Token = this.Token;

            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.Logout(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
