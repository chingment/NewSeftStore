﻿using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lumos
{
    public sealed class Signature
    {
        ///// <summary>
        ///// 服务器可以接受的调用方时间戳与服务器时间的时差，单位秒
        ///// </summary>
        public const short MaxTimeDiff = 30;

        /// <summary>
        /// 时间戳计算的基准
        /// </summary>
        private readonly static DateTime TimestampBase = new DateTime(1970, 1, 1);

        /// <summary>
        /// 用于构建签名的有序参数列表
        /// </summary>
        public string Data;

        /// <summary>
        /// 时间戳，本地时间
        /// </summary>
        public long Timestamp;

        /// <summary>
        /// 用于混淆签名内容的，调用方唯一的128位字符串
        /// </summary>
        public string Secret;

        public string Key;

        public Signature(string key, string secret, long timestamp, string data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentOutOfRangeException("key");

            if (string.IsNullOrEmpty(secret))
                throw new ArgumentOutOfRangeException("secret");

            if (string.IsNullOrEmpty(timestamp.ToString()))
                throw new ArgumentOutOfRangeException("timestamp");

            this.Data = data;
            this.Timestamp = timestamp;
            this.Secret = secret;
            this.Key = key;
        }

        private string BuildMaterial()
        {
            var sb = new StringBuilder();


            sb.Append(this.Key);
            sb.Append(this.Secret);
            sb.Append(this.Timestamp.ToString());

            if (this.Data != null)
            {
                sb.Append(this.Data);
            }

            //Log.Info("签名原始数据:" + sb.ToString());

            //将字符串中字符按升序排序
            var sortStr = string.Concat(sb.ToString().OrderBy(c => c));
            //LogUtil.Info("签名排序后数据:" + sortStr);
            return sortStr;
        }

        private string ComputeHash(string material)
        {

            if (string.IsNullOrEmpty(material))
                throw new ArgumentOutOfRangeException();


            var input = Encoding.UTF8.GetBytes(material);

            var hash = SHA256Managed.Create().ComputeHash(input);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            string str = sb.ToString();

            //Log.Info("HASH数据后:" + str);

            var input2 = Encoding.UTF8.GetBytes(str);

            var output = Convert.ToBase64String(input2);

            //Log.Info("BASE64数据后:" + output);

            return output;
        }

        public string Compute()
        {
            var material = this.BuildMaterial();
            var hash = this.ComputeHash(material);

            return hash;
        }

        public static bool IsRequestTimeout(long app_timestamp)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime app_requestTime = startTime.AddSeconds(app_timestamp);

            var ts = DateTime.Now - app_requestTime;
            if (System.Math.Abs(ts.TotalMinutes) > MaxTimeDiff)
            {
                return true;
            }

            return false;
        }

        public static string Compute(string key, string secret, long timestamp, string data)
        {
            if (secret == null)
            {
                return null;
            }
            var signature = new Signature(key, secret, timestamp, data);
            return signature.Compute();
        }


        public static string GetQueryData(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return "";

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = HttpUtility.UrlEncode(dem.Current.Value, UTF8Encoding.UTF8);
                if (!string.IsNullOrEmpty(key))
                {
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }

            string s = queryStr.ToString().Substring(1, queryStr.Length - 1);

            return s;
        }

    }
}
