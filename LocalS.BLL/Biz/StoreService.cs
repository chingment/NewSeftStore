using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class StoreService : BaseService
    {
        public StoreModel GetOne(string id)
        {
            var m_Store = new StoreModel();

            var d_Store = CurrentDb.Store.Where(m => m.Id == id).FirstOrDefault();

            if (d_Store == null)
                return null;

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == d_Store.MerchId).FirstOrDefault();

            m_Store.StoreId = d_Store.Id;
            m_Store.Name = d_Store.Name;
            m_Store.MerchId = d_Store.MerchId;
            m_Store.MerchName = d_Merch.Name;
            m_Store.BriefDes = d_Store.BriefDes;
            m_Store.IsDelete = d_Store.IsDelete;
            m_Store.IsTestMode = d_Store.IsTestMode;
            m_Store.SctMode = d_Store.SctMode;
            return m_Store;
        }
    }
}
