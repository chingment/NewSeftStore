using LocalS.BLL;
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

        public string GetAppIdByRedirectUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return AppId.ACCOUNT;

            url = url.ToLower();

            if (url.IndexOf("admin.17fanju.com") > -1)
            {
                return AppId.ADMIN;
            }
            else if (url.IndexOf("merch.17fanju.com") > -1 || url.IndexOf("172.24.144.1") > -1)
            {
                return AppId.MERCH;
            }
            else if (url.IndexOf("agent.17fanju.com") > -1)
            {
                return AppId.AGENT;
            }

            return AppId.ACCOUNT;
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse LoginByAccount([FromBody]RopOwnLoginByAccountInWebSite rop)
        {

            LogUtil.Info("RedirectUrl:" + rop.RedirectUrl);

            var myRop = new RopOwnLoginByAccount();
            myRop.UserName = rop.UserName;
            myRop.Password = rop.Password;
            myRop.Ip = rop.Ip;
            myRop.LoginPms = rop.LoginPms;
            myRop.AppId = GetAppIdByRedirectUrl(rop.RedirectUrl);
            myRop.LoginWay = Lumos.DbRelay.Enumeration.LoginWay.Website;
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
        public OwnApiHttpResponse Logout([FromBody]RopOwnLogout rop)
        {
            if (rop == null)
            {
                rop = new RopOwnLogout();
            }

            rop.Token = this.Token;

            IResult result = AccountServiceFactory.Own.Logout(this.CurrentUserId, this.CurrentUserId, rop);
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
