using Entity;
using JDVOP;
using JDVOP.Dto;
using JDVOP.Extensions;
using JDVOP.Helper;
using JDVOP.Interface;
using JDVOP.Model;
using JDVOP.Service;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Webservice
{
    public partial class CategoryService : BaseService
    {
        MallInterface service = new JDMallService();
        
        string categoryKey = CacheHelper.RenderKey("11", "categoryKey");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        public List<CategoryInfo> Cache_Get_CategoryInfoList()
        {
            return CacheHelper.Get<List<CategoryInfo>>(categoryKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<CategoryInfo> list = db.CategoryInfo.ToList();
                    return list;
                }
            });
        }

        
        /// <summary>
        /// 获取选择项
        /// </summary>
        /// <param name="enteredPointFlag">角色flag值</param>
        /// <returns></returns>
        public List<Tuple<long, string>> Get_CategorySelectItem(long parentId)
        {
            using (var db = new DbRepository())
            {
                var list = new List<CategoryInfo>() { new CategoryInfo { catId = -1, name = "请选择" } };
                if (parentId != 0)
                    list.AddRange(Cache_Get_CategoryInfoList().Where(x => x.parentId.Equals(parentId)).ToList());
                else
                {
                    list.AddRange(Cache_Get_CategoryInfoList().Where(x => x.catClass == CategoryClass.One).ToList());

                }
                return list.Select(x => new Tuple<long, string>(x.catId, x.name)).ToList();

            }
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="name"></param>
        /// <param name="depth"></param>
        /// <param name="categoryId"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public KeyValue GetCategory(string name, int depth, int categoryId, SqlConnection con)
        {
            string categorySql = string.Empty;
            if (depth == 1)
            {
                categorySql = $"SELECT CategoryId as [Key], [Name] as [Value] FROM [CMCC_12580].[dbo].[Hishop_Categories]   where name = '{name}' and depth=1;";
            }
            else
            {
                categorySql = $"SELECT CategoryId as [Key], [Name] as [Value] FROM [CMCC_12580].[dbo].[Hishop_Categories] where depth={depth} and ParentCategoryId={categoryId} and name='{name}';";
            }
            return GetResult<KeyValue>(categorySql, con);
        }

        /// <summary>
        /// 获取分类名
        /// </summary>
        /// <param name="category_dic"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public string GetCategoryName(Dictionary<long, CategoryInfo> category_dic, string categoryId)
        {
            if (categoryId.IsNullOrEmpty())
                return "";
            List<string> nameList = new List<string>();
            categoryId.Split(';').ToList().ForEach(x =>
            {
                if (category_dic.ContainsKey(x.GetLong()))
                {
                    nameList.Add(category_dic[x.GetLong()].name);
                }
            });
            return string.Join(";", nameList);
        }
    }

}