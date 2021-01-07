using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSlopeOne
{
    class Program
    {
        /// <summary>
        /// 输出指定的二维数组
        /// </summary>
        /// <param name="array"></param>
        public static void OutPutArray<T>(T[,] array)
        {
            bool isDouble = false;
            if (array[0, 0] is double)
                isDouble = true;
            int len1 = array.GetLength(0);
            int len2 = array.GetLength(1);
            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {

                    //if (j >= i)
                    switch (isDouble)
                    {
                        case false:
                            Console.Write(array[i, j] + "    ");
                            break;

                        case true:
                            if (array[i, j].ToString().Length == 1)
                                Console.Write(array[i, j] + ".00    ");
                            else
                                Console.Write(array[i, j] + "    ");
                            break;
                    }
                    /*
                    else
                        Console.Write(array[i, j] + "    ");
                        */
                }
                Console.WriteLine();
            }

        }

        public static List<string> ListSort(List<string> strList)
        {
            if (strList != null && strList.Count > 0)
            {
                //strList.Sort((x, y) => -x.CompareTo(y)); //逆序
                strList.Sort((x, y) => x.CompareTo(y)); //顺序
            }
            return strList;
        }

        public List<BI_UserSkuHitsNum> bi_UserSkuHitsNums;

        List<string> user_Ids;
        List<string> sku_Ids;
        int user_Count = 0;
        int sku_Count = 0;
        int[,] user_Sku_Matrix;
        int[,] sku_Cooccurrence_Matrix;
        //存储物品的总用户数
        int[] sku_UserNum;
        //物品之间的余弦相似矩阵二维数组
        double[,] cosine_Similar_Matrix;

        Dictionary<string, string> likeSkus = new Dictionary<string, string>();
        //用户没有购买过或喜欢过对应的物品 key为users中的下标，value所有物品字符串，中间用，隔开
        Dictionary<string, string> recommendSkus = new Dictionary<string, string>();

        public int[,] Get_User_Goods_Matrix()
        {
            user_Ids = ListSort(bi_UserSkuHitsNums.Select(m => m.UserId).Distinct().ToList());
            sku_Ids = ListSort(bi_UserSkuHitsNums.Select(m => m.SkuId).Distinct().ToList());

            user_Count = user_Ids.Count();
            sku_Count = sku_Ids.Count();

            user_Sku_Matrix = new int[user_Count, sku_Count];


            foreach (var bi_UserSkuHitsNum in bi_UserSkuHitsNums)
            {
                //得到原始数据的第i行的用户值和物品值在users中的索引
                int a = user_Ids.IndexOf(bi_UserSkuHitsNum.UserId);
                int b = sku_Ids.IndexOf(bi_UserSkuHitsNum.SkuId);
                //将倒排矩阵中的[a,b]值置为1
                user_Sku_Matrix[a, b] = 1;
            }
            Console.WriteLine("用户->物品的倒排矩阵:");

            OutPutArray<int>(user_Sku_Matrix);

            return user_Sku_Matrix;
        }

        public int[,] Get_Cooccurrence_Matrix(string userId)
        {
            int i, j, k, y, CompareCount;
            //物品与物品的同现矩阵二维数组
            Get_User_Goods_Matrix();

            //同现矩阵
            sku_Cooccurrence_Matrix = new int[sku_Count, sku_Count];
            //存储物品的总用户数
            sku_UserNum = new int[sku_Count];
            for (i = 0; i < user_Count; i++)
            {
                string userLikeGoodsStr = "";
                string recommendGoodsStr = "";
                for (j = 0; j < sku_Count; j++)
                {
                    /* 判断起始对比值是否为1，
                     * 如果为0的话说明user_Goods_Matrix[i, j]的值与第i行中的任何一个数据一定是对比不成功的则直接跳过。
                     */
                    if (user_Sku_Matrix[i, j] == 1)
                    {
                        if (user_Ids[i] == userId)
                            userLikeGoodsStr = userLikeGoodsStr + j + ",";
                        sku_UserNum[j] = sku_UserNum[j] + 1;
                        //统计物品总用户数结束
                        //实际对比次数=CompareCount-1
                        CompareCount = sku_Count - j;
                        for (k = 1; k < CompareCount; k++)
                        {
                            y = j + k;
                            if (user_Sku_Matrix[i, y] == 1)
                            {
                                sku_Cooccurrence_Matrix[j, y]++;
                                //Cooccurrence_Matrix[y, j]++; 放弃对角线值
                            }
                        }
                    }
                    else
                       if (user_Ids[i] == userId)
                        recommendGoodsStr = recommendGoodsStr + j + ",";
                }
                if (user_Ids[i] == userId)
                {
                    likeSkus.Add(userId, userLikeGoodsStr);
                    recommendSkus.Add(userId, recommendGoodsStr);
                }
            }

            Console.WriteLine("物品与物品的同现矩阵（www.b0c0.com）:");
            OutPutArray<int>(sku_Cooccurrence_Matrix);
            return sku_Cooccurrence_Matrix;
        }

        public double[,] Get_Cosine_Similar_Matrix(string userId)
        {
            Get_Cooccurrence_Matrix(userId);


            var sss = bi_UserSkuHitsNums.Where(m => m.UserId == userId).GroupBy(t => t.SkuId)
.Select(g => new
{
    SkuId = g.Key,
    Sum = g.Sum(x => x.HitsNum)
}
).ToList();

            //string skuId = sku_Ids[m[i]];

            //int rate = 1;
            //var s = sss.Where(g => g.SkuId == skuId).FirstOrDefault();
            //if (s != null)
            //{
            //    rate = s.Sum;
            //}

            cosine_Similar_Matrix = new double[sku_Count, sku_Count];
            int i = 0, j = 0;
            for (i = 0; i < sku_Count; i++)
            {
                if (i <= j)
                    for (j = 0; j < sku_Count; j++)
                    {
                        if (sku_Cooccurrence_Matrix[i, j] != 0)
                        {
                            string skuId = sku_Ids[i];

                            int rate = 1;


                            var s = sss.Where(m => m.SkuId == skuId).FirstOrDefault();
                            if (s != null)
                            {
                                rate = s.Sum;
                            }


                            double a = rate * Math.Round((sku_Cooccurrence_Matrix[i, j] / Math.Sqrt(sku_UserNum[i] * sku_UserNum[j])), 2);

                            cosine_Similar_Matrix[i, j] = a;
                        }
                    }
            }
            Console.WriteLine("物品之间的余弦相似矩阵(www.b0c0.com):");
            OutPutArray<double>(cosine_Similar_Matrix);
            return cosine_Similar_Matrix;
        }


        public void Get_Similarity(string userId)
        {

            //from u in bi_UserSkuHitsNums select u.SkuId,hi

            Get_Cosine_Similar_Matrix(userId);

            string likeGoodsStr = "";
            string recommendGoodsStr = "";
            //存储为用户推荐的物品集合，key为推荐物品Id，value推荐度
            Dictionary<int, double> tes = new Dictionary<int, double>();
            likeSkus.TryGetValue(userId, out likeGoodsStr);
            recommendSkus.TryGetValue(userId, out recommendGoodsStr);
            if (!string.IsNullOrEmpty(recommendGoodsStr))
            {
                int[] m = Array.ConvertAll(recommendGoodsStr.Substring(0, recommendGoodsStr.Length - 1).Split(','), int.Parse);
                if (!string.IsNullOrEmpty(likeGoodsStr))
                {
                    int[] n = Array.ConvertAll(likeGoodsStr.Substring(0, likeGoodsStr.Length - 1).Split(','), int.Parse);
                    for (int i = 0; i < m.Count(); i++)
                    {
                        int x = m[i];
                        double goodSimilarity = 0.00;
                        for (int j = 0; j < n.Count(); j++)
                        {
                            int y = n[j];
                            if (x < y)
                                goodSimilarity += cosine_Similar_Matrix[x, y];
                            else
                                goodSimilarity += cosine_Similar_Matrix[y, x];
                        }
                        tes.Add(m[i], goodSimilarity);
                    }
                }
            }
            tes = tes.OrderByDescending(p => p.Value).ToDictionary(o => o.Key, p => p.Value);

            int top10 = 0;
            Console.WriteLine("为用户【" + userId + "】推荐：");
            foreach (KeyValuePair<int, double> k in tes)
            {
                if (top10 != 10)
                {
                    //goodsName.TryGetValue(goods[k.Key], out va);
                    Console.WriteLine("物品:" + sku_Ids[k.Key] + "推荐度：" + k.Value);
                    top10++;
                }
                else
                    break;
            }

        }

        static void Main(string[] args)
        {

            List<BI_UserSkuHitsNum> bi_UserSkuHitsNums = new List<BI_UserSkuHitsNum>();

            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "2", SkuId = "15", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "2", SkuId = "39", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "2", SkuId = "57", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "6", SkuId = "24", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "6", SkuId = "57", HitsNum = 11 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "6", SkuId = "60", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "20", SkuId = "15", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "20", SkuId = "39", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "20", SkuId = "60", HitsNum = 12 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "31", SkuId = "15", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "31", SkuId = "24", HitsNum = 14222 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "35", SkuId = "39", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "35", SkuId = "41", HitsNum = 1 });
            bi_UserSkuHitsNums.Add(new BI_UserSkuHitsNum { UserId = "35", SkuId = "60", HitsNum = 1 });

            Program pm = new Program();

            pm.bi_UserSkuHitsNums = bi_UserSkuHitsNums;

            pm.Get_Similarity("2");

            //List<string> user_Ids = new List<string>();
            //List<string> sku_Ids = new List<string>();
            //int user_Count = 0;
            //int sku_Count = 0;

            //user_Ids = ListSort(bi_UserSkuHitsNums.Select(m => m.UserId).Distinct().ToList());
            //sku_Ids = ListSort(bi_UserSkuHitsNums.Select(m => m.SkuId).Distinct().ToList());


            //user_Count = user_Ids.Count();
            //sku_Count = sku_Ids.Count();

            ////用户->物品的倒排矩阵二维数组
            //int[,] user_Sku_Matrix = new int[user_Count, sku_Count];

            //foreach (var bi_UserSkuHitsNum in bi_UserSkuHitsNums)
            //{
            //    //得到原始数据的第i行的用户值和物品值在users中的索引
            //    int a = user_Ids.IndexOf(bi_UserSkuHitsNum.UserId);
            //    int b = sku_Ids.IndexOf(bi_UserSkuHitsNum.SkuId);
            //    //将倒排矩阵中的[a,b]值置为1
            //    user_Sku_Matrix[a, b] = 1;
            //}
            //Console.WriteLine("用户->物品的倒排矩阵:");
            //OutPutArray<int>(user_Sku_Matrix);

            ////同现矩阵
            //int[,] sku_Cooccurrence_Matrix = new int[sku_Count, sku_Count];

            ////存储物品的总用户数
            //int[] sku_UserNum = new int[sku_Count];

            //int compareCount = 0;

            ////用户购买过或喜欢过对应的物品 key为users中的下标，value所有物品字符串，中间用，隔开
            //Dictionary<int, string> likeSkus = new Dictionary<int, string>();
            ////用户没有购买过或喜欢过对应的物品 key为users中的下标，value所有物品字符串，中间用，隔开
            //Dictionary<int, string> recommendSkus = new Dictionary<int, string>();

            //for (int i = 0; i < user_Count; i++)
            //{
            //    string userLikeSkuStr = "";
            //    string recommendSkuStr = "";
            //    for (int j = 0; j < sku_Count; j++)
            //    {
            //        /* 判断起始对比值是否为1，
            //         * 如果为0的话说明user_Goods_Matrix[i, j]的值与第i行中的任何一个数据一定是对比不成功的则直接跳过。
            //         */
            //        if (user_Sku_Matrix[i, j] == 1)
            //        {
            //            userLikeSkuStr = userLikeSkuStr + j + ",";
            //            sku_UserNum[j] = sku_UserNum[j] + 1;
            //            //统计物品总用户数结束
            //            //实际对比次数=CompareCount-1
            //            compareCount = sku_Count - j;
            //            for (int k = 1; k < compareCount; k++)
            //            {
            //                int y = j + k;
            //                if (user_Sku_Matrix[i, y] == 1)
            //                {
            //                    sku_Cooccurrence_Matrix[j, y]++;
            //                    //Cooccurrence_Matrix[y, j]++; 放弃对角线值
            //                }
            //            }
            //        }
            //        else
            //            recommendSkuStr = recommendSkuStr + j + ",";
            //    }
            //    likeSkus.Add(i, userLikeSkuStr);
            //    recommendSkus.Add(i, recommendSkuStr);
            //}
            //Console.WriteLine("物品与物品的同现矩阵:");
            //OutPutArray<int>(sku_Cooccurrence_Matrix);


            ////物品之间的余弦相似矩阵二维数组
            //double[,] cosine_Similar_Matrix=new double[sku_Count, sku_Count];

            //int z = 0;
            //for (int i = 0; i < sku_Count;  i++)
            //{
            //    if (i <= z)
            //        for (z = 0; z < sku_Count; z++)
            //        {
            //            if (sku_Cooccurrence_Matrix[i, z] != 0)
            //            {
            //                double a = Math.Round((sku_Cooccurrence_Matrix[i, z] / Math.Sqrt(sku_UserNum[i] * sku_UserNum[z])), 2);
            //                cosine_Similar_Matrix[i, z] = a;
            //            }
            //        }
            //}
            //Console.WriteLine("物品之间的余弦相似矩阵:");
            //OutPutArray<double>(cosine_Similar_Matrix);

            //SlopeOne test = new SlopeOne();

            //Dictionary<int, List<Product>> userRating = new Dictionary<int, List<Product>>();

            ////第一位用户
            //List<Product> list = new List<Product>()
            //{
            //    new Product() { ProductID = 1, ProductName = "洗衣机", Score = 5 },
            //    new Product() { ProductID = 2, ProductName = "电冰箱", Score = 10 },
            //    new Product() { ProductID = 3, ProductName = "彩电", Score = 10 },
            //    new Product() { ProductID = 4, ProductName = "空调", Score = 5 },
            //};

            //userRating.Add(1000, list);

            //test.AddUserRatings(userRating);

            //userRating.Clear();
            //userRating.Add(1000, list);

            //test.AddUserRatings(userRating);

            ////第二位用户
            //list = new List<Product>()
            //{
            //    new Product() { ProductID = 1, ProductName = "洗衣机", Score = 4 },
            //    new Product() { ProductID = 2, ProductName = "电冰箱", Score = 5 },
            //    new Product() { ProductID = 3, ProductName = "彩电", Score = 4 },
            //    new Product() { ProductID = 4, ProductName = "空调", Score = 10 },
            //};

            //userRating.Clear();
            //userRating.Add(2000, list);

            //test.AddUserRatings(userRating);

            ////第三位用户
            //list = new List<Product>()
            //{
            //    new Product() { ProductID = 1, ProductName = "洗衣机", Score = 4 },
            //    new Product() { ProductID = 2, ProductName = "电冰箱", Score = 10 },
            //    new Product() { ProductID = 4, ProductName = "空调", Score = 5 },
            //};

            //userRating.Clear();
            //userRating.Add(3000, list);

            //test.AddUserRatings(userRating);

            ////那么我们预测userID=3000这个人对 “彩电” 的打分会是多少？
            //var userID = userRating.Keys.FirstOrDefault();
            //var result = userRating[userID];

            //var predictions = test.Predict(result);

            //foreach (var rating in predictions)
            //    Console.WriteLine("ProductID= " + rating.Key + " Rating: " + rating.Value);
        }
    }
}
