using LocalS.Service.Api.Account;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAccount.Controllers
{
    public class OwnController : OwnApiBaseController
    {

        public Lumos.DbRelay.Enumeration.AppId GetAppIdByRedirectUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return Lumos.DbRelay.Enumeration.AppId.Account;

            url = url.ToLower();

            if (url.IndexOf("admin.17fanju.com") > -1)
            {
                return Lumos.DbRelay.Enumeration.AppId.Admin;
            }
            else if (url.IndexOf("merch.17fanju.com") > -1)
            {
                return Lumos.DbRelay.Enumeration.AppId.Merch;
            }
            else if (url.IndexOf("agent.17fanju.com") > -1)
            {
                return Lumos.DbRelay.Enumeration.AppId.Agent;
            }

            return Lumos.DbRelay.Enumeration.AppId.Account;
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse LoginByAccount([FromBody]RopOwnLoginByAccountInWebSite rop)
        {

            var myRop = new RopOwnLoginByAccount();
            myRop.UserName = rop.UserName;
            myRop.Password = rop.Password;
            myRop.Ip = rop.Ip;
            myRop.LoginPms = rop.LoginPms;
            myRop.AppId = GetAppIdByRedirectUrl(rop.RedirectUrl);

            IResult result = AccountServiceFactory.Own.LoginByAccount(myRop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetInfo([FromUri]RupOwnGetInfo rup)
        {
            IResult result = AccountServiceFactory.Own.GetInfo(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Logout([FromBody] RopOwnLogout rop)
        {
            IResult result = AccountServiceFactory.Own.Logout(rop.AppId, this.CurrentUserId, this.CurrentUserId, this.Token);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse CheckPermission([FromUri]RupOwnCheckPermission rup)
        {
            IResult result = AccountServiceFactory.Own.CheckPermission(this.CurrentUserId, this.CurrentUserId, this.Token, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UploadFingerVeinData([FromUri]LocalS.Service.Api.Account.RopUploadFingerVeinData rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.UploadFingerVeinData(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
