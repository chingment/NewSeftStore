﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk.Tenpay
{
    public interface ITenpayPostApi
    {
        string ApiName { get; }

        string PostData { get; set; }


    }
}
