using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace JDVOP.Helper
{
    /// <summary>
    /// 数据库表转化为实体帮助类，使用此类注意数据库表的字段和实体属性名称一样，并且实体存在显式定义的无参构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ModelConvertHelper<T> where T : new()
    {
        /// <summary>
        /// 将DataTable转为Model
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>泛型实体集合</returns>
        public static IList<T> ToModels(DataTable dt)
        {
            IList<T> ts = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                ts.Add(ToModel(dr));
            }
            return ts;
        }

        /// <summary>
        /// 将SqlDataReader读取的内容转为Model，结束后不会自动关闭Reader
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <returns>泛型实体集合</returns>
        public static IList<T> ToModels(SqlDataReader dr)
        {
            IList<T> ts = new List<T>();
            while (dr.Read())
            {
                ts.Add(ToModel(dr));
            }
            return ts;
        }

        /// <summary>
        /// 将DataRow读取到的一行 转为 Model
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <returns>泛型实体</returns>
        public static T ToModel(DataRow dr)
        {
            // 获得此模型的类型
            Type type = typeof(T);
            string tempName = "";
            T t = new T();
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();
            DataTable dt = dr.Table;
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name;
                if (dt.Columns.Contains(tempName))
                {
                    // 判断此属性是否有Setter6
                    if (!pi.CanWrite)
                        continue;
                    object value = dr[tempName];
                    if (value != DBNull.Value)
                    {
                        if (pi.PropertyType.IsEnum)
                            pi.SetValue(t, Enum.Parse(pi.PropertyType, value.ToString().Trim(), true), null);
                        else if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                            pi.SetValue(t, Convert.ToDateTime(value.ToString()), null);
                        else
                            pi.SetValue(t, value, null);
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 将 SqlDataReader 转为Model, 如果 SqlDataReader.read() 有值 ，返回对象，否则返回Null
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <returns>泛型实体</returns>
        public static T ToModel(SqlDataReader dr)
        {
            // 获得此模型的类型
            Type type = typeof(T);
            string tempName = "";
            T t = new T();
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();
            int clen = dr.FieldCount;
            Dictionary<string, object> nv = new Dictionary<string, object>();
            for (int i = 0; i < clen; i++)
            {
                string fieldname = dr.GetName(i).ToLower();
                nv[fieldname] = dr[i];
            }
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name.ToLower();
                if (nv.ContainsKey(tempName))
                {
                    if (!pi.CanWrite)
                        continue;
                    object value = nv[tempName];
                    if (value != DBNull.Value)
                    {
                        if (pi.PropertyType.IsEnum)
                            pi.SetValue(t, Enum.Parse(pi.PropertyType, value.ToString().Trim(), true), null);
                        else if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                            pi.SetValue(t, Convert.ToDateTime(value.ToString()), null);
                        else
                            pi.SetValue(t, value, null);
                    }
                }
            }
            return t;
        }

    }
}
