using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{

    public class SpecDes
    {
        public string Name { get; set; }
        public string Value { get; set; }


        public static string GetDescribe(string str)
        {

            if (string.IsNullOrEmpty(str))
                return null;

            string values = "";

            try
            {
                var items = str.ToJsonObject<List<SpecDes>>();

                return GetDescribe(items);
            }
            catch (Exception ex)
            {

            }

            return values;
        }

        public static string GetDescribe(List<SpecDes> items)
        {

            string values = "";

            try
            {
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (item.Name == "单规格")
                        {
                            values += item.Value + "";
                        }
                        else
                        {
                            values += item.Name + ":" + item.Value + " ";
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return values;
        }
    }

    public class SpecItem
    {
        public SpecItem()
        {
            this.Value = new List<SpecItemValue>();
        }

        public string Name { get; set; }
        public List<SpecItemValue> Value { get; set; }
    }

    public class SpecItemValue
    {
        public string Name { get; set; }
    }
}
