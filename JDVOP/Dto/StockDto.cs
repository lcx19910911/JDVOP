using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Dto
{
    /// <summary>
    /// 库存表
    /// </summary>
    public class StockDto
    {

        /// <summary>
        /// 配送地址ID
        /// </summary>
        public string areaId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public long skuId { get; set; }

        /// <summary>
        /// 库存状态编号
        /// </summary>
        public StockState stockStateld { get; set; }

        /// <summary>
        /// 库存状态描述
        /// </summary>
        public string stockStateDesc { get; set; }


        /// <summary>
        /// 返回真实数量  返回-1，代表未知
        /// </summary>
        public int remainNum { get; set; }

    }

    /// <summary>
    /// 库存表(列表)
    /// </summary>
    public class ListStockDto
    {

        /// <summary>
        /// 配送地址ID
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public long sku { get; set; }

        /// <summary>
        /// 库存状态编号
        /// </summary>
        public StockState state { get; set; }

        /// <summary>
        /// 库存状态描述
        /// </summary>
        public string desc { get; set; }


    }

    /// <summary>
    /// 库存状态
    /// </summary>
    public enum StockState
    {
        Nono=0,

        /// <summary>
        ///  有货 现货-下单立即发货
        /// </summary>
        HaveAndForDeliver = 33,

        /// <summary>
        /// 有货 在途-正在内部配货，预计2 ~6天到达本仓库
        /// </summary>
        HaveAndWaitArriveForDeliver = 39,

        /// <summary>
        ///  可配货-下单后从有货仓库配货
        /// </summary>
        HaveAndPayForFromStock = 40,
        /// <summary>
        /// 预订
        /// </summary>
        Reserver = 36,
        /// <summary>
        /// 无货
        /// </summary>
        NoHaveStock = 34,
    }

}
