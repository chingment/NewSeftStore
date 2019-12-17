using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class AppSoftwareService : BaseDbContext
    {
        public string GetAppSecretByAppKey(string appKey)
        {
            var term = CurrentDb.AppSoftware.Where(m => m.AppApiAppKey == appKey).FirstOrDefault();
            if (term == null)
            {
                return null;
            }

            return term.AppApiAppSecret;
        }
    }
}
