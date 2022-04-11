using LocalS.Entity;
using Lumos;
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
        public string SvDeptId { get; set; }
        //public string MerchId { get; set; }
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
            string key = string.Format("Wx_AppId_{0}_AccessToken", config.SvDeptId);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));

                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

                var getAccessTokenRequest = new SenvivSdk.GetAccessTokenRequest(config.AccessToken, new { deptid = config.SvDeptId });

                var result = api.DoPost(getAccessTokenRequest);

                if (result.Result == ResultType.Success)
                {
                    accessToken = result.Data.Data.access_token;
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

        public List<SenvivSdk.UserListResult.DataModel> GetUsers(SenvivConfig config)
        {
            var list = new List<SenvivSdk.UserListResult.DataModel>();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1000;

            var userListRequest = new SenvivSdk.UserListRequest(config.AccessToken, new { deptid = config.SvDeptId, size = size, page = page });
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
                            var userListRequest2 = new SenvivSdk.UserListRequest(config.AccessToken, new { deptid = config.SvDeptId, size = size, page = i });
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

        public List<SenvivSdk.BoxListResult.DataModel> GetDevices(SenvivConfig config)
        {
            return GetDevices(config, "");
        }

        public List<SenvivSdk.BoxListResult.DataModel> GetDevices(SenvivConfig config, string deviceId)
        {
            var list = new List<SenvivSdk.BoxListResult.DataModel>();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1000;

            var boxListRequest = new SenvivSdk.BoxListRequest(config.AccessToken, new { deptid = config.SvDeptId, size = size, page = page, keyword = deviceId });
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
                            var userListRequest2 = new SenvivSdk.BoxListRequest(config.AccessToken, new { deptid = config.SvDeptId, size = size, page = i });
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

        public SenvivSdk.BoxListResult.DataModel GetBox(SenvivConfig config, string keyword)
        {
            SenvivSdk.BoxListResult.DataModel model = null;

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1;

            var boxListRequest = new SenvivSdk.BoxListRequest(config.AccessToken, new { deptid = config.SvDeptId, size = size, page = page, keyword = keyword });
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

        public ReportDetailListResult.DataModel GetUserHealthDayReport32(SenvivConfig config, string userid)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            var requestReportDetailList = new SenvivSdk.ReportDetailListRequest(config.AccessToken, new { deptid = config.SvDeptId, userid = userid, size = 1, page = 1 });
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

        public ReportParDetailResult.DataModel GetUserHealthDayReport46(SenvivConfig config, string sn)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            ReportParDetailRequest requestReportParDetail = new SenvivSdk.ReportParDetailRequest(config.AccessToken, new { deptid = config.SvDeptId, sn = sn, size = 1, page = 1 });
            var resultReportParDetail = api.DoPost(requestReportParDetail);


            if (resultReportParDetail.Data != null)
            {
                if (resultReportParDetail.Data.count > 0)
                {
                    return resultReportParDetail.Data.data[0];
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

        public CustomJsonResult BindDevice(SenvivConfig config, string userid, string sn)
        {
            var restult = new CustomJsonResult();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            BoxBindRequest request = new BoxBindRequest(config.AccessToken, new { sn = sn, userid = userid, deptid = config.SvDeptId });
            var r_api = api.DoPost(request);

            if (r_api.Data == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备不存在[1]");
            }
            var d = r_api.Data.Data;

            if (d == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备不存在[2]");
            }

            if (d.Result == 3)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备不存在[3]");

            if (d.Result == 5)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被绑定[4]");

            if (d.Result == 6)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此设备已经被冻结[5]");

            if (d.Result != 1)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("绑定失败[{0}]", d.Result));


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");

        }

        public CustomJsonResult UnBindDevice(SenvivConfig config, string userid, string sn)
        {
            var restult = new CustomJsonResult();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            BoxUnBindRequest request = new BoxUnBindRequest(config.AccessToken, new { sn = sn, userid = userid, deptid = config.SvDeptId });
            var r_api = api.DoPost(request);

            if (r_api.Data == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解绑失败[1]");
            }
            var d = r_api.Data.Data;

            if (d == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解绑失败[2]");
            }

            if (d.Result != 1 && d.Result != 5)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解绑失败[3]");

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "解绑成功");
        }

        public CustomJsonResult SetGmPeriod(SenvivConfig config, string userid, E_SvUserCareMode careMode, DateTime? lastTime, int duration, int cycle)
        {
            string currentState = "1";
            if (careMode == E_SvUserCareMode.PrePregnancy)
            {
                currentState = "2";
            }

            string theLastTime = "";
            if (lastTime != null)
            {
                theLastTime = lastTime.Value.ToUnifiedFormatDate();
            }
            var result = new CustomJsonResult();
            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            SetGmPeriodRequest request = new SetGmPeriodRequest(config.AccessToken, new { userid = userid, deptid = config.SvDeptId, currentState = currentState, theLastTime = theLastTime, duration = duration, cycle = cycle });
            var r_api = api.DoPost(request);


            return result;
        }

    }

}
