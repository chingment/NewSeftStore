using Lumos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ObjectExtensions
    {
        public static string ToJsonString(this Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string rt = null;
            try
            {
                rt = JsonConvertUtil.SerializeObject(obj);
            }
            catch
            {

            }

            if (rt == "[]" || rt == "{}")
            {
                rt = null;
            }

            return rt;
        }

        public static T ToJsonObject<T>(this Object s)
        {

            if (s == null)
                return default(T);

            try
            {
                if (s is String)
                {
                    T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s.ToString());

                    return t;
                }
                else
                {
                    T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(s));

                    return t;
                }

            }
            catch
            {
                return default(T);
            }

        }
    }
}
