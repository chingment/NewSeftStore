using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace System
{
    public static class DbContextDatabaseExtensions
    {
        public static List<T> ToList<T>(this DataTable dt, bool dateTimeToString = true) where T : class, new()
        {
            List<T> list = new List<T>();

            if (dt != null)
            {
                List<PropertyInfo> infos = new List<PropertyInfo>();

                Array.ForEach<PropertyInfo>(typeof(T).GetProperties(), p =>
                {
                    if (dt.Columns.Contains(p.Name) == true)
                    {
                        infos.Add(p);
                    }
                });//获取类型的属性集合

                SetList<T>(list, infos, dt, dateTimeToString);
            }

            return list;
        }

        private static void SetList<T>(List<T> list, List<PropertyInfo> infos, DataTable dt, bool dateTimeToString) where T : class, new()
        {
            foreach (DataRow dr in dt.Rows)
            {
                T model = new T();

                infos.ForEach(p =>
                {
                    if (dr[p.Name] != DBNull.Value)//判断属性在不为空
                    {

                        object tempValue = dr[p.Name];
                        Type type = tempValue.GetType();
                        //if判断就是解决办法
                        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))//判断convertsionType是否为nullable泛型类  
                        {
                            //如果type为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换  
                            System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(type);
                            //将type转换为nullable对的基础基元类型  
                            type = nullableConverter.UnderlyingType;
                        }

          
                        if (dr[p.Name].GetType() == typeof(DateTime) && dateTimeToString == true)//判断是否为时间
                        {
                            tempValue = dr[p.Name].ToString();
                        }
                        try
                        {
                            p.SetValue(model, Convert.ChangeType(tempValue, type), null);//设置
                        }
                        catch { }
                    }
                });
                list.Add(model);
            }
        }
    }
}
