﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public interface IApiPostRequest<T>
    {
        string ApiUrl { get; }

        string PostData { get; }
    }
}
