using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Dto
{
    /// <summary>
    /// 四级地址
    /// </summary>
    public class AreaDto
    {
        /// <summary>
        /// 京东一级地址编号
        /// </summary>
        public int province { get; set; }
        /// <summary>
        /// 京东二级地址编号
        /// </summary>
        public int city { get; set; }
        /// <summary>
        /// 京东三级地址编号
        /// </summary>
        public int county { get; set; }
        /// <summary>
        /// 京东四级地址编号
        /// </summary>
        public int town { get; set; } = 0;
    }

    /// <summary>
    /// 地区是否限制
    /// </summary>
    public class CheckAreaLimit
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public long skuId { get; set; }

        /// <summary>
        /// True 区域限制 false 不受区域限制
        /// </summary>
        public bool isAreaRestrict { get; set; }
    }

    public class Region
    {
        public string RegionId { get; set; }
        public string RegionName { get; set; }
    }

    public class RegionObject
    {
        public string Status { get; set; }
        public List<Region> Regions { get; set; }
    }

    public enum AreaType
    {
        Province=0,
        City=1,
        County=2,
        Town=3,
    }
}
