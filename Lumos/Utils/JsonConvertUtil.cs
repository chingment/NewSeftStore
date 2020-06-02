using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public class JsonConvertUtil
    {
        public static string SerializeObject(Object obj)
        {

            try
            {
                var setting = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                return Newtonsoft.Json.JsonConvert.SerializeObject(obj, setting);
            }
            catch
            {
                return null;
            }

        }
    }
}
