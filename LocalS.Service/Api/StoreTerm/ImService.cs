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
            imSeatModel.NickName = "黄雯";
            imSeatModel.Avatar = "http://file.17fanju.com/Upload/Avatar_default.png";
            imSeatModel.BriefDes = "神经内科临床营养师 擅长脑血管疾病营养饮食指导 擅长危重患者胃肠营养 各特殊生理阶段人群营养咨询";
            imSeatModel.CharTags.Add("特约专家");
            imSeatModel.CharTags.Add("医疗服务");
            imSeatModel.ImUserName = "15989287032";
            imSeatModel.ImStatus = "idle";

            ret.Seats.Add(imSeatModel);

            var imSeatModel2 = new ImSeatModel();
            imSeatModel2.UserId = "2";
            imSeatModel2.NickName = "钟国峰";
            imSeatModel2.Avatar = "http://file.17fanju.com/Upload/Avatar_default.png";
            imSeatModel2.BriefDes = "临床营养治疗方案";
            imSeatModel2.CharTags.Add("中医院");
            imSeatModel2.CharTags.Add("营养治疗师");
            imSeatModel2.ImUserName = "15989287032";
            imSeatModel2.ImStatus = "busy";

            ret.Seats.Add(imSeatModel2);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }
    }
}
