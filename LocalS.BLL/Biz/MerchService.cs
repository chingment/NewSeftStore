using MyAlipaySdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWeiXinSdk;
using TgPaySdk;
using XrtPaySdk;
using LocalS.Entity;

namespace LocalS.BLL.Biz
{
    public class MerchService : BaseService
    {

        public string GetMerchName(string merchId)
        {

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();

            if (d_Merch == null)
                return null;

            return d_Merch.Name;
        }

        public string GetClientName(string merchId, string userId)
        {
            string userName = "匿名";
            var d_SysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();
            if (d_SysUser != null)
            {
                if (!string.IsNullOrEmpty(d_SysUser.FullName))
                {
                    return d_SysUser.FullName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.NickName))
                {
                    return d_SysUser.NickName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.UserName))
                {
                    return d_SysUser.UserName;
                }
            }

            return userName;
        }

        public string GetOperaterUserName(string merchId, string userId)
        {
            string userName = "匿名";
            var d_SysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();
            if (d_SysUser != null)
            {
                if (!string.IsNullOrEmpty(d_SysUser.FullName))
                {
                    return d_SysUser.FullName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.NickName))
                {
                    return d_SysUser.NickName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.UserName))
                {
                    return d_SysUser.UserName;
                }
            }

            return userName;
        }

        public string GetStoreName(string merchId, string storeId)
        {

            var d_Store = CurrentDb.Store.Where(m => m.Id == storeId && m.MerchId == merchId).FirstOrDefault();

            if (d_Store == null)
                return null;

            return d_Store.Name;
        }

        public string GetShopName(string merchId, string shopId)
        {

            var d_Shop = CurrentDb.Shop.Where(m => m.Id == shopId && m.MerchId == merchId).FirstOrDefault();

            if (d_Shop == null)
                return null;

            return d_Shop.Name;
        }

        public string GetIotApiSecret(string merchId)
        {
            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();

            if (d_Merch == null)
                return null;

            return d_Merch.IotApiSecret;
        }

        public string[] GetRelIds(string merchId)
        {
            var list = CurrentDb.Merch.ToList();

            var query = list.Where(m => m.Id == merchId).ToList();

            var list2 = query.Concat(GetSonList(list, merchId));

            var arr = list2.Select(m => m.Id).ToArray();

            return arr;
        }

        private IEnumerable<Merch> GetSonList(List<Merch> list, string id)
        {
            var query = list.Where(m => m.PId == id).ToList();

            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonList(list, t.Id)));
        }
    }
}
