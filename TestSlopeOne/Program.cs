using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSlopeOne
{
    class Program
    {
        static void Main(string[] args)
        {

            SlopeOne test = new SlopeOne();

            Dictionary<int, List<Product>> userRating = new Dictionary<int, List<Product>>();

            //第一位用户
            List<Product> list = new List<Product>()
            {
                new Product() { ProductID = 1, ProductName = "洗衣机", Score = 5 },
                new Product() { ProductID = 2, ProductName = "电冰箱", Score = 10 },
                new Product() { ProductID = 3, ProductName = "彩电", Score = 10 },
                new Product() { ProductID = 4, ProductName = "空调", Score = 5 },
            };

            userRating.Add(1000, list);

            test.AddUserRatings(userRating);

            userRating.Clear();
            userRating.Add(1000, list);

            test.AddUserRatings(userRating);

            //第二位用户
            list = new List<Product>()
            {
                new Product() { ProductID = 1, ProductName = "洗衣机", Score = 4 },
                new Product() { ProductID = 2, ProductName = "电冰箱", Score = 5 },
                new Product() { ProductID = 3, ProductName = "彩电", Score = 4 },
                new Product() { ProductID = 4, ProductName = "空调", Score = 10 },
            };

            userRating.Clear();
            userRating.Add(2000, list);

            test.AddUserRatings(userRating);

            //第三位用户
            list = new List<Product>()
            {
                new Product() { ProductID = 1, ProductName = "洗衣机", Score = 4 },
                new Product() { ProductID = 2, ProductName = "电冰箱", Score = 10 },
                new Product() { ProductID = 4, ProductName = "空调", Score = 5 },
            };

            userRating.Clear();
            userRating.Add(3000, list);

            test.AddUserRatings(userRating);

            //那么我们预测userID=3000这个人对 “彩电” 的打分会是多少？
            var userID = userRating.Keys.FirstOrDefault();
            var result = userRating[userID];

            var predictions = test.Predict(result);

            foreach (var rating in predictions)
                Console.WriteLine("ProductID= " + rating.Key + " Rating: " + rating.Value);
        }
    }
}
