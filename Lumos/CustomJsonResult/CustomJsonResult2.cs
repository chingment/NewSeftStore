using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lumos
{
    #region CustomJsonResult 自定义JsonResult
    /// <summary>
    /// 扩展JsonResult
    /// </summary>
    public class CustomJsonResult2<T> : ActionResult, IResult2<T>
    {

        private string _code = "";
        private string _msg = "";
        private T _data;


        /// <summary>
        /// 信息默认返回空字符串
        /// </summary>
        public string Msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

        /// <summary>
        /// 信息默认返回空字符串
        /// </summary>
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        /// <summary>
        /// 内容默认为null
        /// </summary>
        public T Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public JsonConverter[] JsonConverter
        {
            get;
            set;
        }

        public JsonSerializerSettings JsonSerializerSettings
        {
            get;
            set;
        }

        public CustomJsonResult2()
        {
            this.JsonSerializerSettings = new JsonSerializerSettings();
        }

        private void SetCustomJsonResult(string contenttype, string code, string msg, T data, JsonSerializerSettings settings, params JsonConverter[] converters)
        {

            this._code = code;
            this._msg = msg;
            this._data = data;
            this.JsonSerializerSettings = settings;
            this.JsonConverter = converters;
        }

        public CustomJsonResult2(string contenttype, string code, string msg, T content, JsonSerializerSettings settings, params JsonConverter[] converters)
        {
            SetCustomJsonResult(contenttype, code, msg, content, settings, converters);
        }


        public CustomJsonResult2(string code, string msg, T content)
        {
            SetCustomJsonResult(null, code, msg, content, null, null);
        }


        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            //if (!String.IsNullOrEmpty(ContentType))
            //{
            //    response.ContentType = ContentType;
            //}
            //else
            //{
            //    response.ContentType = "application/json";
            //}

            //if (ContentEncoding != null)
            //{
            //    response.ContentEncoding = ContentEncoding;
            //}

            response.Write(GetResultJson());

        }

        public override string ToString()
        {
            return GetResultJson();
        }


        public string GetResultJson()
        {
            StringBuilder json = new StringBuilder();
            json.Append("{");

            try
            {

                if (this._data != null)
                {
                    if (this._data is string)
                    {
                        if (!string.IsNullOrWhiteSpace(this._data.ToString()))
                        {
                            json.Append("\"data\":" + this._data + ",");
                        }
                    }
                    else
                    {

                        if (this.JsonSerializerSettings == null)
                        {
                            this.JsonSerializerSettings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };

                        }

                        JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
                        {
                            //日期类型默认格式化处理
                            this.JsonSerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
                            this.JsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                            return this.JsonSerializerSettings;
                        });
                        this.JsonSerializerSettings.Converters = this.JsonConverter;
                        this.JsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        json.Append("\"data\":" + JsonConvert.SerializeObject(this._data, Formatting.None, this.JsonSerializerSettings) + ",");
                    }
                }

                if (CommonUtil.IsNumber(this._code))
                {
                    json.Append("\"code\": " + this._code + ",");
                }
                else
                {
                    json.Append("\"code\": \"" + this._code + "\",");
                }

                json.Append("\"msg\":" + JsonConvert.SerializeObject(this._msg) + "");


            }
            catch (Exception ex)
            {
                json.Append("\"code\":3000,");
                json.Append("\"msg\":\"" + string.Format("CustomJsonResult转换发生异常:{0}", ex.Message) + "\"");
                //转换失败记录日志
            }
            json.Append("}");

            string s = json.ToString();

            return s;
        }
    }

    public class CustomJsonResult2 : CustomJsonResult2<object>, IResult2
    {

        public CustomJsonResult2()
        {

        }

        public CustomJsonResult2(string code, string message) : base(code, message, null)
        {

        }

        public CustomJsonResult2(string code, string message, object content) : base(code, message, content)
        {

        }
    }

    #endregion
}
