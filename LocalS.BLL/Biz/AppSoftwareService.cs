using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class AppSoftwareService : BaseService
    {
        public string GetAppSecretByAppKey(string appId, string appKey)
        {
            var d_AppSoftware = CurrentDb.AppSoftware.Where(m => m.AppId == appId && m.AppKey == appKey).FirstOrDefault();
            if (d_AppSoftware == null)
            {
                return null;
            }

            return d_AppSoftware.AppSecret;
        }
    }
}
