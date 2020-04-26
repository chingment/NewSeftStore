using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Test;

namespace System
{

    public class ValueAndCnName
    {
        public int Value { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// 备注特性
    /// </summary>
    public class RemarkAttribute : Attribute
    {
        private string _CnName;
        public RemarkAttribute(string cnname)
        {
            this._CnName = cnname;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string CnName
        {
            get { return _CnName; }
            set { _CnName = value; }
        }
    }

    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的备注信息
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string GetCnName(this Enum em)
        {

            Type type = em.GetType();
            FieldInfo fd = type.GetField(em.ToString());
            if (fd == null)
                return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(RemarkAttribute), false);
            string name = string.Empty;
            foreach (RemarkAttribute attr in attrs)
            {
                name = attr.CnName;
            }
            return name;
        }

        public static string GetCnName(this String em)
        {

         
            Type type2 = em.GetType();
         
            var atts=typeof(TestA).GetField("Login").GetCustomAttributes(typeof(RemarkAttribute), false);

            Type type = em.GetType();

            PropertyInfo fd = type.GetProperty(em);
            //if (fd == null)
            //    return string.Empty;
            object[] attrs = type.GetCustomAttributes(typeof(RemarkAttribute), false);
            string name = string.Empty;
            foreach (RemarkAttribute attr in attrs)
            {
                name = attr.CnName;
            }
            return name;
        }

    }
}
