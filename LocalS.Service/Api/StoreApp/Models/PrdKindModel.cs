using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ListModel<T>
    {
        public ListModel()
        {
            this.Items = new List<T>();
        }

        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }

    public class PrdKindModel
    {
        public PrdKindModel()
        {
            this.List = new ListModel<PrdProductModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string MainImgUrl { get; set; }

        public bool Selected { get; set; }

        public ListModel<PrdProductModel> List { get; set; }


    }
}
