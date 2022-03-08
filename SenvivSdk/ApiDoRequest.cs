using Lumos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class ApiDoRequest
    {
        private string responseString = null;
        private readonly string TAG = "SenvivSdk.ApiDoRequest";

        public string GetSeviceUrl()
        {
            return "http://api.ryouhu.senviv.com";
        }


        public ApiDoRequest()
        {

        }

        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }



        public CustomJsonResult<T> DoPost<T>(IApiPostRequest<T> request)
        {
            var result = new CustomJsonResult<T>();
            try
            {
                string requestUrl = GetSeviceUrl() + "/" + request.ApiUrl;
                WebUtils webUtils = new WebUtils();
                //LogUtil.Info(TAG, requestUrl);

                string str_PostData = request.PostData.ToString();
                if (request.PostData.GetType() != typeof(String))
                {
                    str_PostData = request.PostData.ToJsonString();
                }
                //LogUtil.Info(TAG, str_PostData);

                var obj_PostData = new { Token = request.Token, Data = GetBase64(str_PostData) };

                var str_PostData2 = obj_PostData.ToJsonString();

                //LogUtil.Info(TAG, str_PostData2);

                var doPost = webUtils.DoPost(requestUrl, str_PostData2);

                if (doPost == null)
                {
                    result.Result = ResultType.Failure;
                    result.Code = ResultCode.Failure;
                    result.Message = "请求数据失败";
                    return result;
                }

                if (doPost.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    result.Result = ResultType.Failure;
                    result.Code = ResultCode.Failure;
                    result.Message = "请求数据失败：" + doPost.StatusCode;
                    return result;
                }

                this.responseString = doPost.ResponseString;

                //LogUtil.Info(TAG, responseString);

                if ( request.ApiUrl.IndexOf("Boxbind") > -1)
                {
                    responseString = "{code:0,data:{\"result\":" + this.responseString + "}}";
                }
                else if(request.ApiUrl.IndexOf("BoxUnbind") > -1)
                {
                    responseString = "{code:0,data:{\"result\":1}}";
                }


                T data = JsonConvert.DeserializeObject<T>(responseString);

                result.Result = ResultType.Success;
                result.Code = ResultCode.Success;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Result = ResultType.Exception;
                result.Code = ResultCode.Exception;
                result.Message = "请求数据发生异常";
            }

            return result;
        }

        public string GetBase64(string data)
        {
            if (data == null)
                return null;

            byte[] buffer = System.Text.UTF8Encoding.UTF8.GetBytes(data);
            //压缩后的byte数组
            byte[] compressedbuffer = null;
            //Compress buffer,压缩缓存
            MemoryStream ms = new MemoryStream();
            using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zs.Write(buffer, 0, buffer.Length);

                //下面两句被注释掉的代码有问题, 对应的compressedbuffer的长度只有10--该10字节应该只是压缩buffer的header

                //zs.Flush();
                //compressedbuffer = ms.ToArray();           

            }

            //只有GZipStream在Dispose后调应对应MemoryStream.ToArray()所得到的Buffer才是我们需要的结果
            compressedbuffer = ms.ToArray();
            //将压缩后的byte数组basse64字符串
            string text64 = Convert.ToBase64String(compressedbuffer);

            return text64;
        }
    }
}
