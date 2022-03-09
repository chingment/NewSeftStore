using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class ArticleService : BaseService
    {
        public CustomJsonResult Details(string operater, string articleId, string svuid)
        {
            var d_SvArticle = CurrentDb.SvArticle.Where(m => m.Id == articleId).FirstOrDefault();
            if (d_SvArticle == null)
                new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到");


            var ret = new
            {
                Article = new
                {
                    Title = d_SvArticle.Title,
                    Content = d_SvArticle.Content
                }

            };


            var result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
