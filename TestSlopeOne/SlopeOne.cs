using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestSlopeOne
{

    #region Slope One 算法
    /// <summary>
    /// Slope One 算法
    /// </summary>
    public class SlopeOne
    {
        /// <summary>
        /// 评分系统
        /// </summary>
        public static Dictionary<int, Product> dicRatingSystem = new Dictionary<int, Product>();

        public Dictionary<string, Rating> dic_Martix = new Dictionary<string, Rating>();

        public HashSet<int> hash_items = new HashSet<int>();

        #region 接收一个用户的打分记录
        /// <summary>
        /// 接收一个用户的打分记录
        /// </summary>
        /// <param name="userRatings"></param>
        public void AddUserRatings(IDictionary<int, List<Product>> userRatings)
        {
            foreach (var user1 in userRatings)
            {
                //遍历所有的Item
                foreach (var item1 in user1.Value)
                {
                    //该产品的编号（具有唯一性）
                    int item1Id = item1.ProductID;

                    //该项目的评分
                    float item1Rating = item1.Score;

                    //将产品编号字存放在hash表中
                    hash_items.Add(item1.ProductID);

                    foreach (var user2 in userRatings)
                    {
                        //再次遍历item，用于计算俩俩 Item 之间的差值
                        foreach (var item2 in user2.Value)
                        {
                            //过滤掉同名的项目
                            if (item2.ProductID <= item1Id)
                                continue;

                            //该产品的名字
                            int item2Id = item2.ProductID;

                            //该项目的评分
                            float item2Rating = item2.Score;

                            Rating ratingDiff;

                            //用表的形式构建矩阵
                            var key = Tools.GetKey(item1Id, item2Id);

                            //将俩俩 Item 的差值 存放到 Rating 中
                            if (dic_Martix.Keys.Contains(key))
                                ratingDiff = dic_Martix[key];
                            else
                            {
                                ratingDiff = new Rating();
                                dic_Martix[key] = ratingDiff;
                            }

                            //方便以后以后userrating的编辑操作，（add)
                            if (!ratingDiff.hash_user.Contains(user1.Key))
                            {
                                //value保存差值
                                ratingDiff.Value += item1Rating - item2Rating;

                                //说明计算过一次
                                ratingDiff.Freq += 1;
                            }

                            //记录操作人的ID，方便以后再次添加评分
                            ratingDiff.hash_user.Add(user1.Key);
                        }
                    }
                }
            }
        }
        #endregion

        #region 根据矩阵的值，预测出该Rating中的值
        /// <summary>
        /// 根据矩阵的值，预测出该Rating中的值
        /// </summary>
        /// <param name="userRatings"></param>
        /// <returns></returns>
        public IDictionary<int, float> Predict(List<Product> userRatings)
        {
            Dictionary<int, float> predictions = new Dictionary<int, float>();

            var productIDs = userRatings.Select(i => i.ProductID).ToList();

            //循环遍历_Items中所有的Items
            foreach (var itemId in this.hash_items)
            {
                //过滤掉不需要计算的产品编号
                if (productIDs.Contains(itemId))
                    continue;

                Rating itemRating = new Rating();

                // 内层遍历userRatings
                foreach (var userRating in userRatings)
                {
                    if (userRating.ProductID == itemId)
                        continue;

                    int inputItemId = userRating.ProductID;

                    //获取该key对应项目的两组AVG的值
                    var key = Tools.GetKey(itemId, inputItemId);

                    if (dic_Martix.Keys.Contains(key))
                    {
                        Rating diff = dic_Martix[key];

                        //关键点：运用公式求解（这边为了节省空间，对角线两侧的值呈现奇函数的特性）
                        itemRating.Value += diff.Freq * (userRating.Score + diff.AverageValue * ((itemId < inputItemId) ? 1 : -1));

                        //关键点：运用公式求解 累计每两组的人数
                        itemRating.Freq += diff.Freq;
                    }
                }

                predictions.Add(itemId, itemRating.AverageValue);
            }

            return predictions;
        }
        #endregion
    }
    #endregion

    #region 工具类
    /// <summary>
    /// 工具类
    /// </summary>
    public class Tools
    {
        public static string GetKey(int Item1Id, int Item2Id)
        {
            return (Item1Id < Item2Id) ? Item1Id + "->" + Item2Id : Item2Id + "->" + Item1Id;
        }
    }
    #endregion
}

