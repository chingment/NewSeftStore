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

            var ret = new RetImServiceSeats();

            var imSeatModel = new ImSeatModel();
            imSeatModel.UserId = "1";
            imSeatModel.UserName = "张医生";
            imSeatModel.Avatar = "http://file.17fanju.com/Upload/Avatar_default.png";
            imSeatModel.BriefDes = "又多年";
            imSeatModel.CharTags.Add("特约专家");
            imSeatModel.CharTags.Add("医疗服务");
            imSeatModel.ImUserName = "adadasda";
            imSeatModel.ImStatus = "idle";

            ret.Seats.Add(imSeatModel);

            var imSeatModel2 = new ImSeatModel();
            imSeatModel2.UserId = "2";
            imSeatModel2.UserName = "黄之风";
            imSeatModel2.Avatar = "http://file.17fanju.com/Upload/Avatar_default.png";
            imSeatModel2.BriefDes = "又多年";
            imSeatModel2.CharTags.Add("中医院");
            imSeatModel2.CharTags.Add("营养治疗师");
            imSeatModel2.ImUserName = "adadasda";
            imSeatModel2.ImStatus = "busy";

            ret.Seats.Add(imSeatModel);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "机器未登记", ret);
        }
    }
}
