using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class TermService : BaseDbContext
    {
        public string GetAppSecretByAppKey(string appKey)
        {
            var term = CurrentDb.Term.Where(m => m.AppKey == appKey).FirstOrDefault();
            if (term == null)
            {
                return null;
            }

            return term.AppSecret;
        }
    }
}
