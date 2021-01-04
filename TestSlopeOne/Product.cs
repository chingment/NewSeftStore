using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSlopeOne
{
// <summary>
     /// 产品类
     /// </summary>
     public class Product
     {
         public int ProductID { get; set; }
 
         public string ProductName { get; set; }
 
         /// <summary>
         /// 对产品的打分
         /// </summary>
         public float Score { get; set; }
     }
}
