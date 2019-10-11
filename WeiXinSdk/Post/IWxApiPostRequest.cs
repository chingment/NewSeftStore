﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk
{
    public interface IWxApiPostRequest<T> where T : WxApiBaseResult
    {
        string ApiUrl { get; }

        IDictionary<string, string> GetUrlParameters();

        WxPostDataType PostDataTpye { get; set; }

        object PostData { get; set; }

    }
}
