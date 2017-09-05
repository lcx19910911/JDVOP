using JDVOP.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Dto
{
    /// <summary>
    /// 分类
    /// </summary>
    [Table("CategoryInfo")]
    public class CategoryInfo
    {
        [Key]
        [Required]
        [MaxLength(32)]
        public string ID { get; set; }

        public long catId { get; set; }
        public long parentId { get; set; }

        [MaxLength(32)]
        public string name { get; set; }
        public CategoryClass catClass { get; set; }
        public int state { get; set; }
    }


    /// <summary>
    /// 分类等级
    /// </summary>
    public enum CategoryClass
    {
        /// <summary>
        /// 一级
        /// </summary>
        One = 0,
        /// <summary>
        /// 二级
        /// </summary>
        Two = 1,
        /// <summary>
        /// 三级
        /// </summary>
        Three = 2
    }

    /// <summary>
    /// 分类分页列表
    /// </summary>
    public class CategoryPageList:PageList
    {
        public List<CategoryInfo> categorys { get; set; }
    }
}
