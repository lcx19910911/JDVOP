using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Model
{


    public class OrderInfo
    {
        public OrderStatus OrderStatus { get; set; }
        public string Address { get; set; }
        public string Tax { get; set; }

        public string CellPhone { get; set; }

        public string EmailAddress { get; set; }

        public string OrderId { get; set; }

        public int RegionId { get; set; }

        public string Remark { get; set; }

        public string TelPhone { get; set; }

        public string ShipTo { get; set; }

        public string ZipCode { get; set; }

        public string ShippingRegion { get; set; }
    }

    public enum JDOrderState
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New = 0,

        /// <summary>
        /// 妥投
        /// </summary>
        [Description("妥投")]
        Complete = 1,

        /// <summary>
        /// 拒收
        /// </summary>
        [Description("拒收")]
        Refund = 2,
    }

    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 0 - 所有
        /// </summary>
        [Description("所有")]
        All = 0,

        /// <summary>
        /// 1 - 等待买家付款
        /// </summary>
        [Description("等待买家付款")]
        WaitBuyerPay = 1,

        /// <summary>
        /// 2 - 买家已付款，等待发货
        /// </summary>
        [Description("买家已付款，等待发货")]
        BuyerAlreadyPaid = 2,

        /// <summary>
        /// 3 - 卖家已发货，等待确认
        /// </summary>
        [Description("卖家已发货，等待确认")]
        SellerAlreadySent = 3,

        /// <summary>
        /// 4 - 已关闭
        /// </summary>
        [Description("已关闭")]
        Closed = 4,

        /// <summary>
        /// 5 - 已完成
        /// </summary>
        [Description("已完成")]
        Finished = 5,

        /// <summary>
        /// 6 - 申请退款
        /// </summary>
        [Description("申请退款")]
        ApplyForRefund = 6,

        /// <summary>
        /// 7 - 申请退货
        /// </summary>
        [Description("申请退货")]
        ApplyForReturns = 7,

        /// <summary>
        /// 8 - 申请换货
        /// </summary>
        [Description("申请换货")]
        ApplyForReplacement = 8,

        /// <summary>
        /// 9 - 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 9,

        /// <summary>
        /// 10 - 已退货
        /// </summary>
        [Description("已退货")]
        Returned = 10,

        /// <summary>
        /// 11 - 今天
        /// </summary>
        [Description("今天")]
        Today = 11,

        /// <summary>
        /// 12 - 已删除
        /// </summary>
        [Description("已删除")]
        Deleted = 12,


        /// <summary>
        /// 99 - 历史订单
        /// </summary>
        [Description("历史订单")]
        History = 0x63,

        /// <summary>
        /// 已经支付定价(拼团）
        /// </summary>
        [Description("已经支付定价(拼团）")]
        GroupPaidDeposit = 20,

        /// <summary>
        /// 申请退款，拼团失败
        /// </summary>
        [Description("申请退款，拼团失败")]
        GroupFailReqRefund = 60,

        /// <summary>
        /// 拼团，退差价
        /// </summary>
        [Description("拼团，退差价")]
        GroupRefundDifference = 61


    }
}
