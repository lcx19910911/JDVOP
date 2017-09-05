using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Dto
{
    /// <summary>
    /// 搜索
    /// </summary>
    public class SearchDto
    {
        public int resultCount { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public int pageCount { get; set; }
         
        public int code { get; set;}

        /// <summary>
        ///商标结果
        /// </summary>
        public BrandAggregate brandAggregate { get; set; }
        /// <summary>
        /// 价格区间
        /// </summary>
        public List<IntervalAggregate> priceIntervalAggregate { get; set; }
        /// <summary>
        /// 分类结果
        /// </summary>
        public CategoryAggregate categoryAggregate { get; set; }

        public string expandAttrAggregate { get; set; }
        /// <summary>
        /// 搜索结果
        /// </summary>

        public List<searchResult> hitResult { get; set; }
    }

    /// <summary>
    /// 商标搜索接货
    /// </summary>
    public class BrandAggregate
    {
        public List<string> pinyinAggr { get; set; }

        public List<SearchBrand> brandList { get; set; }
    }
    /// <summary>
    /// 商标集合
    /// </summary>
    public class SearchBrand
    {
        public string id { get; set; }

        public string pinyin { get; set; }

        public string name { get; set; }
    }
    /// <summary>
    /// 价格区间
    /// </summary>
    public class IntervalAggregate
    {
        public int min { get; set; }
        public int max { get; set; }
    }

    /// <summary>
    /// 分类搜索统计
    /// </summary>
    public class  CategoryAggregate
    {

        public SearchCategory firstCategory { get; set; }
        public SearchCategory secondCategory { get; set; }
        public SearchCategory thridCategory { get; set; }

    }


    /// <summary>
    /// 分类对象
    /// </summary>
    public class SearchCategory
    {

        public long catId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 父类id
        /// </summary>
        public long parentId { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 分类名
        /// </summary>
        public string name { get; set; }
        

        public int weight { get; set; }
    }

    /// <summary>
    /// 搜索的结果
    /// </summary>
    public class searchResult
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public string wareId { get; set; }
        public string warePId { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string wareName { get; set; }

        /// <summary>
        /// 商标id
        /// </summary>
        public string brandId { get; set; }
        /// <summary>
        /// 商标名
        /// </summary>
        public string brand { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public string catId { get; set; }
        /// <summary>
        /// 分类名
        /// </summary>
        public string catName { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string imageUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string cid1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string cid2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string cid1Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string cid2Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string wstate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wyn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string synonyms { get; set; }
    }
}
