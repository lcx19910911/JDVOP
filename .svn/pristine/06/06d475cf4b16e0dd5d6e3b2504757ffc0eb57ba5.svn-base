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
    /// 订单
    /// </summary>
    public class OrderDto
    {

        public EnumShop shop { get; set; }

        /// <summary>
        /// 商品相关
        /// </summary>
        public List<SkuNum> skuNum { get; set; }

        /// <summary>
        ///  第三方的订单单号（客户根据自己规则定义，不超20位数字）
        /// </summary>
        [Required]
        public string thirdOrder { get; set; }

        /// <summary>
        /// 商品相关
        /// </summary>
        public List<OrderSku> sku { get; set; }

        /// <summary>
        /// 收货人（少于10字）
        /// </summary>   
        [Required]
        public string name { get; set; }


        /// <summary>
        /// 一级地址id
        /// </summary>   
        [Required]
        public int province { get; set; }


        /// <summary>
        /// 二级地址id
        /// </summary>   
        [Required]
        public int city { get; set; }

        /// <summary>
        /// 三级地址id
        /// </summary>   
        [Required]
        public int county { get; set; }

        /// <summary>
        /// 四级地址id（地址接口查询）  (如果该地区有四级地址，则必须传递四级地址，没有四级地址则传0)
        /// </summary>  
        public int town { get; set; } = 0;

        /// <summary>
        /// 详细地址（少于30字）
        /// </summary>   
        [Required]
        public string address { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>   
        public string zip { get; set; }
        /// <summary>
        /// 座机号
        /// </summary>   
        public string phone { get; set; }
        /// <summary>
        /// 手机
        /// </summary>   
        [Required]
        public string mobile { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>   
        [Required]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; } = "286557139@qq.com";
        /// <summary>
        /// 座机号
        /// </summary>   
        public string remark { get; set; }
        /// <summary>
        /// 开票方式
        /// </summary>   
        [Required]
        public InvoiceState invoiceState { get; set; } = InvoiceState.Centred;

        /// <summary>
        /// 开票类型
        /// </summary>   
        [Required]
        public InvoiceType invoiceType { get; set; } = InvoiceType.VATInvoice;


        public string companyName { get; set; } = "福建东方启明科技有限公司";

        /// <summary>
        /// 开票对象
        /// </summary>   
        [Required]
        public InvoiceTitle selectedInvoiceTitle { get; set; } = InvoiceTitle.Company;
        /// <summary>
        /// 发票抬头(如果selectedInvoiceTitle= 5则此字段Y)
        /// </summary>   
        //public string companyName { get; set; }
        /// <summary>
        /// 发票详情1:明细，3：电脑配件，19:耗材，22：办公用品
        //           备注:若增值发票则只能选1 明细
        /// </summary>   
        [Required]
        public InvoiceContent invoiceContent { get; set; } = InvoiceContent.Detial;
        /// <summary>
        /// 支付方式
        /// </summary>   
        [Required]
        public PaymentType paymentType { get; set; } = PaymentType.OnLine;

        /// <summary>
        /// 使用余额   预存款下单固定传1， 使用余额 非预存款下单固定0 不使用余额
        /// </summary>
        public YesOrNoCode isUseBalance { get; set; } = YesOrNoCode.Yes;

        /// <summary>
        /// 是否预占库存  是否预占库存，0是预占库存（需要调用确认订单接口），1是不预占库存
        /// </summary>
        public YesOrNoCode submitState { get; set; } = YesOrNoCode.No;

        //bookInvoiceContent int N   bookInvoiceContent=4(图书普票随货开必传，其他不传)
        //16, 1303, 48713, 48746
        /// <summary>
        /// 增值票收票人姓名 当invoiceType=2 且invoiceState=1时则此字段必填
        /// </summary>   
        public string invoiceName { get; set; } = "福建东方启明科技有限公司";
        /// <summary>
        /// 增值票收票人姓名 当invoiceType=2且invoiceState=1 时则此字段必填
        /// </summary>   
        public string invoicePhone { get; set; } = "13809545632";
        /// <summary>
        /// 增值票收票人所在省(京东地址编码)  当invoiceType=2 且invoiceState=1时则此字段必填
        /// </summary>   
        public string invoiceProvice { get; set; } = "16";
        /// <summary>
        /// 增值票收票人所在市  当invoiceType=2 且invoiceState=1时则此字段必填
        /// </summary>   
        public string invoiceCity { get; set; } = "1303";
        /// <summary>
        /// 增值票收票人所在区/县 当invoiceType=2 且invoiceState=1时则此字段必填
        /// </summary>   
        public string invoiceCounty { get; set; } = "3484";
        /// <summary>
        /// 增值票收票人所在地址 当invoiceType=2 且invoiceState=1时则此字段必填
        /// </summary>   
        public string invoiceAddress { get; set; } = "软件大道89号福州软件园E区10号楼401室";

        ///// <summary>
        ///// 大家电配送日期 默认值为-1，0表示当天，1表示明天，2：表示后天; 如果为-1表示不使用大家电预约日历
        ///// </summary>
        //public int reservingDate { get; set; }

        ///// <summary>
        ///// 大家电安装日期 不支持默认按-1处理，0表示当天，1表示明天，2：表示后天
        ///// </summary>
        //public int installDate { get; set; }
        ///// <summary>
        ///// 大家电是否选择了安装 默认为true，选择了“暂缓安装”，此为必填项，必填值为false。
        ///// </summary>
        //public bool needInstall { get; set; } = false;

        ///// <summary>
        ///// 中小件配送预约日期  格式：yyyy-MM-dd
        ///// </summary>   
        //public string promiseDate { get; set; }
        ///// <summary>
        ///// 中小件配送预约时间段 时间段如： 9:00-15:00
        ///// </summary>   
        //public string promiseTimeRange { get; set; }

        ///// <summary>
        ///// 中小件预约时间段的标记
        ///// </summary>
        //public int promiseTimeRangeCode { get; set; }

        ///// <summary>
        ///// 下单价格模式  no: 客户端订单价格快照不做验证对比，还是以京东端价格正常下单; yes:必需验证客户端订单价格快照，如果快照与京东端价格不一致返回下单失败，需要更新商品价格后，重新下单;
        ///// </summary>
        //public YesOrNoCode doOrderPriceMode { get; set; }

        ///// <summary>
        ///// 客户端订单价格快照
        ///// </summary>
        //public List<PriceSnap> orderPriceSnap { get; set; }

        //extContent String  N 买断模式下，该参数必传
        //扩展节点字段（extContent）说明
        //　{
        //    "paymentType": "1",
        //    "mobile": "13551061234”,
        //    “skus”:”{“107810”:”50.30”,”181818”:”22.30”}”,
        //    “orderPrice”:”72.60”
        //}

        //			字段名	说明
        //			paymentType	是否由京东代收货款：1：是；0：否
        //			mobile	C用户手机号码
        //			skus	订单商品在C客户系统的单价:
        //如："{"107810":"50.30","181818":"22.30"}"
        //107810：京东SkuId---50.30：商品在C客户价格
        //181818：京东SkuId---22.30：商品在C客户价格
        //			orderPrice	C用户在客户系统下单的订单金额。
        //			备注：买断模式下，订单基础信息上必须传客户自己的手机号，与扩展节点里的C用户手机号不一致。

    }

    [Table("JDOrderInfo")]
    public class JDOrderDto
    {
        [NotMapped]
        public long pOrder { get; set; }
        [NotMapped]
        public JDOrderDto pOrders { get; set; }
        /// <summary>
        /// 子订单
        /// </summary>
        [NotMapped]
        public List<JDOrderDto> cOrder { get; set; }

        /// <summary>
        /// 京东订单号
        /// </summary>
        [Key]
        [Required]
        [MaxLength(32)]
        public string ID { get; set; }

        /// <summary>
        /// 京东订单号
        /// </summary>
        [Required]
        public long jdOrderId { get; set; }

        [MaxLength(32)]
        public string CMCCOrderId { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal orderPrice { get; set; }
        /// <summary>
        /// 订单裸价
        /// </summary>
        public decimal orderNakedPrice { get; set; }
        /// <summary>
        /// 订单税额
        /// </summary>
        public decimal orderTaxPrice { get; set; }
       
        /// <summary>
        /// 运费（合同有运费配置才会返回，否则不会返回该字段）
        /// </summary>
        public decimal? freight { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        [NotMapped]
        public List<OrderDetailSku> sku { get; set; }

        [MaxLength(1024)]
        public string DetailsJsonStr { get; set; }

        public JDOrderState State { get; set; }
        [NotMapped]
        public int orderState { get; set; }
        [NotMapped]
        public string StateStr { get; set; }

        public int type { get; set;}

        public DateTime CreateTime { get; set; }
    }
    /// <summary>
    /// 商品信息
    /// </summary>

    public class OrderDetailSku
    {
        /// <summary>
        /// 货品编号
        /// </summary>
        public long skuId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public long category { get; set; }

        /// <summary>
        /// 税种
        /// </summary>
        public decimal tax { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal taxPrice { get; set; }
        /// <summary>
        /// 裸价
        /// </summary>
        public decimal nakedPrice { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public SkuType type { get; set; }
        /// <summary>
        /// 父商品ID  oid为主商品skuid，如果身是主商品，则oid为0
        /// </summary>
        public int oid { get; set; }

    }

    /// <summary>
    /// 商品类别
    /// </summary>
    public enum SkuType
    {
        /// <summary>
        /// 0普通
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 1附件
        /// </summary>
        Attachments = 1,
        /// <summary>
        /// 2赠品
        /// </summary>
        Gifts = 2,
    }

    /// <summary>
    /// 开票方式
    /// </summary>
    public enum InvoiceState
    {
        /// <summary>
        /// 0为订单预借
        /// </summary>
        OrderToBorrow = 0,
        /// <summary>
        /// 1为随货开票
        /// </summary>
        WithProduct = 1,
        /// <summary>
        /// 2为集中开票
        /// </summary>
        Centred = 2,
    }

    public enum EnumShop
    {
        /// <summary>
        /// 普通发票
        /// </summary>
        shop_12580 = 1,
        /// <summary>
        /// 增值税发票
        /// </summary>
        shop_KMI = 2,
    }

    /// <summary>
    /// 开票类型
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// 普通发票
        /// </summary>
        OrdinaryInvoice = 1,
        /// <summary>
        /// 增值税发票
        /// </summary>
        VATInvoice = 2,
    }

    /// <summary>
    /// 开票类型
    /// </summary>
    public enum InvoiceContent
    {
        //1:明细，3：电脑配件，19:耗材，22：办公用品
        /// <summary>
        /// 明细
        /// </summary>
        Detial = 1,
        /// <summary>
        /// 电脑配件
        /// </summary>
        ComputerAccess = 3,
        /// <summary>
        /// 耗材
        /// </summary>
        Supplies = 19,
        /// <summary>
        /// 办公用品
        /// </summary>
        OfficeSupplies = 22,
    }
    


    /// <summary>
    /// 发票对象
    /// </summary>
    public enum InvoiceTitle
    {
        /// <summary>
        /// 个人
        /// </summary>
        Person = 4,
        /// <summary>
        /// 单位
        /// </summary>
        Company = 5,
    }
}
