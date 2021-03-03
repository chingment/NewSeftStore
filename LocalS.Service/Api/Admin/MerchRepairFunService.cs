﻿using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class MerchRepairFunService: BaseService
    {
        public CustomJsonResult ReLoadSpuCache(string operater)
        {
            var result = new CustomJsonResult();


            CacheServiceFactory.Product.ReLoad();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "执行成功");

            return result;
        }
    }
}
