using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Dto
{
    /// <summary>
    /// 请求商品运费
    /// </summary>
    public class RequestFreight:AreaDto
    {
        public List<SkuNum> sku { get; set; }

        public PaymentType paymentType { get; set; }
    }


    /// <summary>
    /// 返回运费结果
    /// </summary>
    public class ResponseFreight
    {

        /// <summary>
        /// 总运费
        /// </summary>
        public decimal freight { get; set; }

        /// <summary>
        /// 基础运费
        /// </summary>
        public decimal baseFreight { get; set; }


        /// <summary>
        /// 偏远运费
        /// </summary>
        public decimal remoteRegionFreight { get; set; }


        /// <summary>
        /// 需收取偏远运费的sku
        /// </summary>
        public decimal remoteSku { get; set; }
    }

    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PaymentType
    {
        //1：货到付款，2：邮局付款，4：在线支付（余额支付），5：公司转账，6：银行转账，7：网银钱包， 101：金采支付

        /// <summary>
        /// 货到付款
        /// </summary>
        Delivery = 1,

        /// <summary>
        /// 邮局付款
        /// </summary>
        PostOffice = 2,
        /// <summary>
        /// 在线支付（余额支付）
        /// </summary>
        OnLine = 4,

        /// <summary>
        /// 公司转账
        /// </summary>
        CompanyTransfer = 5,
        /// <summary>
        /// 银行转账
        /// </summary>
        BankTransfer = 6,

        /// <summary>
        /// 网银钱包
        /// </summary>
        NetSilverWallet = 7,

        /// <summary>
        /// 金采支付
        /// </summary>
        PayOnGold = 101,
    }
}
