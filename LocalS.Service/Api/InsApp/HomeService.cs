﻿using LocalS.BLL;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class HomeService : BaseDbContext
    {
        public CustomJsonResult GetIndexPageData(string mId, string uId)
        {
            var result = new CustomJsonResult();


            var ret = new RetHomeGetIndexPageData();


            ret.Swiper.Imgs.Add(new ImgModel { Src = "http://file.17fanju.com/Upload/Banner/1.png" });
            ret.Swiper.Imgs.Add(new ImgModel { Src = "http://file.17fanju.com/Upload/Banner/2.png" });


            var lNavGridByInsCar = new LNavGridModel();

            lNavGridByInsCar.Title = "车务服务";

            var sysAgentUser = CurrentDb.SysAgentUser.Where(m => m.AgentId == mId && m.Id == uId).FirstOrDefault();

            LogUtil.Info("YbInsureSign:" + sysAgentUser.YbInsureSign);
            string url = string.Format("https://www.ybinsure.com/spa/newCarTest/autoLogin?channelCode=QXT-001&sign={0}", System.Web.HttpUtility.UrlEncode(sysAgentUser.YbInsureSign));
            LogUtil.Info("UrlL:" + url);

            lNavGridByInsCar.Items.Add(new LNavGridItemModel { Title = "车险报价", OpType = "HURL", OpContent = url });

            //lNavGridByInsCar.Items.Add(new LNavGridItemModel { Title = "车险报价", OpType = "HURL", OpContent = string.Format("http://weixin.implus100.com/agent-new/channel_redirect.jsp?channelAccount=ff8080816be268a8016be3f449d10076&userId={0}&type=insure", uId) });
            //lNavGridByInsCar.Items.Add(new LNavGridItemModel { Title = "车险订单", OpType = "HURL", OpContent = string.Format("http://weixin.implus100.com/agent-new/channel_redirect.jsp?channelAccount=ff8080816be268a8016be3f449d10076&userId={0}&type=order", uId) });
            lNavGridByInsCar.Items.Add(new LNavGridItemModel { Title = "理赔服务", OpType = "PURL", OpContent = "/Error/NonOpen" });
            lNavGridByInsCar.Items.Add(new LNavGridItemModel { Title = "车辆定损", OpType = "PURL", OpContent = "/Error/NonOpen" });

            ret.LNavGrids.Add(lNavGridByInsCar);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }


    }
}
