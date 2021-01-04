using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSlopeOne
{
 /// <summary>
     /// 评分实体类
     /// </summary>
     public class Rating
     {
         /// <summary>
         /// 记录差值
         /// </summary>
         public float Value { get; set; }
 
         /// <summary>
         /// 记录评分人数，方便公式中的 m 和 n 的值
         /// </summary>
         public int Freq { get; set; }
 
         /// <summary>
         /// 记录打分用户的ID
         /// </summary>
         public HashSet<int> hash_user = new HashSet<int>();
 
         /// <summary>
         /// 平均值
         /// </summary>
         public float AverageValue
         {
             get { return Value / Freq; }
         }
     }
}
