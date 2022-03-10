using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class BaseService : BaseDbContext
    {
        public string GetAnswerValue(object obj)
        {
            string str = null;
            try
            {


                string t1 = obj.ToJsonString();

                string[] a1 = t1.ToJsonObject<List<string>>().ToArray();
                str = string.Join(",", a1);

                return str;
            }
            catch (Exception ex)
            {

            }

            return str;
        }


        public string[] GetAnswerValueArr(object obj)
        {
            string[] str = null;
            try
            {

                string t1 = obj.ToJsonString();

                string[] a1 = t1.ToJsonObject<List<string>>().ToArray();

                return a1;
            }
            catch (Exception ex)
            {

            }

            return str;
        }
    }
}
