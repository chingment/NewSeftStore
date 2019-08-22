using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{

    public class CartBlockModel
    {

        public string TagName { get; set; }

        public List<CartProductSkuModel> ProductSkus { get; set; }

        public E_ReceptionMode ReceptionMode { get; set; }

    }
}
