﻿using Lumos;
using Lumos.Redis;
using MyWeiXinSdk;
using SenvivSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SenvivConfig
    {
        public string AccessToken { get; set; }
        public string SenvivDeptId { get; set; }
        public string MerchId { get; set; }
    }

    public class Senviv4GProvider : BaseService
    {
        public string GetApiAccessToken(string name, string pwd)
        {
            string key = string.Format("Senviv_{0}_AccessToken", name);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {

                var loginRequest = new LoginRequest("", new { name = name, pwd = pwd });

                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

                var result = api.DoPost(loginRequest);

                if (result.Result == ResultType.Success)
                {

                    accessToken = result.Data.Data.AuthorizationCode;

                    redis.KSet(key, accessToken, new TimeSpan(0, 30, 0));

                }

                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));
            }
            else
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}", key, accessToken));
            }

            return accessToken;
        }

        public string GetWxPaAccessToken(SenvivConfig config)
        {
            string key = string.Format("Wx_AppId_{0}_AccessToken", config.SenvivDeptId);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));

                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

                var getAccessTokenRequest = new SenvivSdk.GetAccessTokenRequest(config.AccessToken, new { deptid = config.SenvivDeptId });

                var result = api.DoPost(getAccessTokenRequest);

                if (result.Result == ResultType.Success)
                {
                    accessToken = result.Data.Data.access_token;

                    string appId = result.Data.Data.appid;

                    var d_SenvivDept = CurrentDb.SenvivDept.Where(m => m.Id == config.SenvivDeptId).FirstOrDefault();

                    if (d_SenvivDept == null)
                    {
                        d_SenvivDept = new Entity.SenvivDept();
                        d_SenvivDept.Id = config.SenvivDeptId;
                        d_SenvivDept.AppId = appId;
                        CurrentDb.SenvivDept.Add(d_SenvivDept);
                        CurrentDb.SaveChanges();
                    }

                    LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}，已过期，重新获取成功", key, accessToken));
                    redis.KSet(key, accessToken, new TimeSpan(0, 30, 0));
                }
            }
            else
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}", key, accessToken));
            }

            return accessToken;
        }

        public List<SenvivSdk.UserListResult.DataModel> GetUserList(SenvivConfig config)
        {
            var list = new List<SenvivSdk.UserListResult.DataModel>();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1000;

            var userListRequest = new SenvivSdk.UserListRequest(config.AccessToken, new { deptid = config.SenvivDeptId, size = size, page = page });
            var result = api.DoPost(userListRequest);
            if (result.Result == ResultType.Success)
            {
                var data = result.Data;
                if (data != null)
                {
                    int count = data.Data.count;
                    int pageCount = (count + size - 1) / size;

                    list.AddRange(data.Data.data);

                    if (pageCount >= 2)
                    {
                        for (var i = 2; i <= pageCount; i++)
                        {
                            var userListRequest2 = new SenvivSdk.UserListRequest(config.AccessToken, new { deptid =config.SenvivDeptId, size = size, page = i });
                            var result2 = api.DoPost(userListRequest2);
                            if (result2.Result == ResultType.Success)
                            {
                                var data2 = result2.Data;
                                if (data2 != null)
                                {
                                    list.AddRange(data2.Data.data);
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        public List<SenvivSdk.BoxListResult.DataModel> GetBoxList(SenvivConfig config)
        {
            var list = new List<SenvivSdk.BoxListResult.DataModel>();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1000;

            var boxListRequest = new SenvivSdk.BoxListRequest(config.AccessToken, new { deptid = config.SenvivDeptId, size = size, page = page });
            var result = api.DoPost(boxListRequest);
            if (result.Result == ResultType.Success)
            {
                var data = result.Data;
                if (data != null)
                {
                    int count = data.Data.count;
                    int pageCount = (count + size - 1) / size;

                    list.AddRange(data.Data.data);

                    if (pageCount >= 2)
                    {
                        for (var i = 2; i <= pageCount; i++)
                        {
                            var userListRequest2 = new SenvivSdk.BoxListRequest(config.AccessToken, new { deptid = config.SenvivDeptId, size = size, page = i });
                            var result2 = api.DoPost(userListRequest2);
                            if (result2.Result == ResultType.Success)
                            {
                                var data2 = result2.Data;
                                if (data2 != null)
                                {
                                    list.AddRange(data2.Data.data);
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        public SenvivSdk.BoxListResult.DataModel GetBox(SenvivConfig config,string keyword)
        {
            SenvivSdk.BoxListResult.DataModel model = null;

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1;

            var boxListRequest = new SenvivSdk.BoxListRequest(config.AccessToken, new { deptid = config.SenvivDeptId, size = size, page = page, keyword = keyword });
            var result = api.DoPost(boxListRequest);
            if (result.Result == ResultType.Success)
            {
                var data = result.Data;
                if (data != null)
                {
                    model = data.Data.data[0];
                }

            }

            return model;
        }

        public ReportDetailListResult.DataModel GetUserHealthDayReport(SenvivConfig config, string userid)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            var requestReportDetailList = new SenvivSdk.ReportDetailListRequest(config.AccessToken, new { deptid = config.SenvivDeptId, userid = userid, size = 1, page = 1 });
            var resultReportDetailList = api.DoPost(requestReportDetailList);
            if (resultReportDetailList.Result == ResultType.Success)
            {
                if (resultReportDetailList.Data != null)
                {
                    if (resultReportDetailList.Data.Data != null)
                    {
                        var d = resultReportDetailList.Data.Data.data;
                        if (d != null && d.Count > 0)
                        {
                            return d[0];
                        }
                    }

                }
            }

            return null;
        }

        public UserCreateResult UserCreate(SenvivConfig config, object post)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            UserCreateRequest userCreateRequest = new UserCreateRequest(config.AccessToken, post);
            var result = api.DoPost(userCreateRequest);

            return result.Data.Data;
        }

        public BoxBindResult BindBox(SenvivConfig config, string userid, string sn)
        {
            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            BoxBindRequest request = new BoxBindRequest(config.AccessToken, new { sn = sn, userid = userid, deptid = config.SenvivDeptId });
            var result = api.DoPost(request);
            return result.Data.Data;
        }

        public BoxUnBindResult UnBindBox(SenvivConfig config, string userid, string sn)
        {
            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            BoxUnBindRequest request = new BoxUnBindRequest(config.AccessToken, new { sn = sn, userid = userid, deptid = config.SenvivDeptId });
            var result = api.DoPost(request);
            return result.Data.Data;
        }

    }

}