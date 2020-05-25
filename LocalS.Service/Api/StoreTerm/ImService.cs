using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class ImService : BaseDbContext
    {
        public CustomJsonResult Seats(RopImServiceSeats rop)
        {
            var result = new CustomJsonResult();


            List<object> objs = new List<object>();


            objs.Add(new { name = "", avatar = "http://file.17fanju.com/Upload/Avatar_default.png", imUserName = "", imPassword = "", tags = "", briefDes = "", imStatus = "" });
            objs.Add(new { name = "", avatar = "http://file.17fanju.com/Upload/Avatar_default.png", imUserName = "", imPassword = "", tags = "", briefDes = "", imStatus = "" });

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "机器未登记", objs);
        }
    }
}
