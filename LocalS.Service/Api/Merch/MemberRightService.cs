using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class MemberRightService : BaseService
    {
        public CustomJsonResult GetLevelSts(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var d_MemberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId).OrderBy(m => m.Level).ToList();

            List<object> levelSts = new List<object>();

            foreach (var d_MemberLevelSt in d_MemberLevelSts)
            {
                levelSts.Add(new
                {
                    Id = d_MemberLevelSt.Id,
                    Name = d_MemberLevelSt.Name,
                    MainImgUrl = d_MemberLevelSt.MainImgUrl,
                    Tag = d_MemberLevelSt.Tag,
                    Level = d_MemberLevelSt.Level
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { levelSts = levelSts });
            return result;
        }

        public CustomJsonResult InitManage(string operater, string merchId, string levelStId)
        {
            var ret = new { };

            List<object> levelSts = new List<object>();

            object curLevelSt = null;

            var d_MemberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId).ToList();
            foreach (var d_MemberLevelSt in d_MemberLevelSts)
            {

                if (d_MemberLevelSt.Id == levelStId)
                {
                    curLevelSt = new { Id = d_MemberLevelSt.Id, Name = d_MemberLevelSt.Name };
                }

                levelSts.Add(new { Id = d_MemberLevelSt.Id, Name = d_MemberLevelSt.Name });
            }

            if (curLevelSt == null)
            {
                curLevelSt = levelSts[0];
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { levelSts, curLevelSt });
        }

        public CustomJsonResult InitManageBaseInfo(string operater, string merchId, string levelId)
        {
            var result = new CustomJsonResult();

            var d_MemberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId && m.Id == levelId).FirstOrDefault();

            var ret = new { Id = d_MemberLevelSt.Id, Name = d_MemberLevelSt.Name };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }


        public CustomJsonResult GetFeeSts(string operater, string merchId, string levelStId)
        {
            var result = new CustomJsonResult();

            var d_MemberFeeSts = CurrentDb.MemberFeeSt.Where(m => m.MerchId == merchId && m.LevelStId == levelStId).OrderBy(m => m.FeeType).ToList();

            List<object> feeSts = new List<object>();

            foreach (var d_MemberFeeSt in d_MemberFeeSts)
            {
                feeSts.Add(new
                {
                    Id = d_MemberFeeSt.Id,
                    FeeTypeName = d_MemberFeeSt.FeeType,
                    FeeOriginalValue = d_MemberFeeSt.FeeOriginalValue,
                    FeeSaleValue = d_MemberFeeSt.FeeSaleValue,
                    Name = d_MemberFeeSt.Name,
                    MainImgUrl = d_MemberFeeSt.MainImgUrl,
                    Tag = d_MemberFeeSt.Tag
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { feeSts = feeSts });
            return result;
        }
    }
}
