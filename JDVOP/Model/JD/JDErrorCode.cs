using System.ComponentModel;

namespace JDVOP.Model
{
    public enum JDErrorCode
    {
        /// <summary>
        /// 无效token
        /// </summary>
        [Description("无效token.")]
        sys_token_invalid = 1003,

        /// <summary>
        /// token过期
        /// </summary>
        [Description("token过期.")]
        sys_token_exprise = 1004,
        /// <summary>
        /// 无效app_key
        /// </summary>
        [Description("无效app_key.")]
        sys_app_key_invalid =1005,
        /// <summary>
        /// 缺少app_key参数
        /// </summary>
        [Description("缺少app_key参数.")]
        sys_app_key_null =1020,

        /// <summary>
        /// 缺少token参数
        /// </summary>
        [Description("缺少token参数.")]
        sys_token_null = 1022,

        /// <summary>
        /// refresh_token过期
        /// </summary>
        [Description("refresh_token过期.")]
        sys_refresh_token_exprise =2011,

        /// <summary>
        /// refresh_token不存在
        /// </summary>
        [Description("refresh_token不存在.")]
        sys_refresh_token_null= 2012,

        /// <summary>
        /// 调用参数param_json为空
        /// </summary>
        [Description("调用参数param_json为空.")]
        sys_param_json_null = 3001,

        /// <summary>
        /// 检查param_json参数格式
        /// </summary>
        [Description("检查param_json参数格式.")]
        sys_param_json_invalid = 3002,
        /// <summary>
        /// 请检查param_json参数格式
        /// </summary>
        [Description("请检查param_json参数格式.")]
        sys_param_json_error = 3003,

        /// <summary>
        /// 调用 API接口连接超时
        /// </summary>
        [Description("调用 API接口连接超时.")]
        sys_time_out= 3004,

        /// <summary>
        /// 缺少配置参数api_host，调用失败
        /// </summary>
        [Description("缺少配置参数api_host，调用失败.")]
        sys_lose_api_host = 3020,

        /// <summary>
        /// 限制时间内调用失败次数
        /// </summary>
        [Description("限制时间内调用失败次数.")]
        sys_time_limit = 3021,
        /// <summary>
        /// 缺少配置参数api_host，调用失败
        /// </summary>
        [Description("缺少版本参数.")]
        sys_lose_version = 3022,
        /// <summary>
        /// 获取api信息调用异常
        /// </summary>
        [Description("获取api信息调用异常.")]
        sys_api_error = 3023,
        /// <summary>
        /// 缺少方法名参数
        /// </summary>
        [Description("缺少方法名参数.")]
        sys_lose_method = 3024,
        /// <summary>
        /// 不存在的方法名或者版本号
        /// </summary>
        [Description("不存在的方法名或者版本号.")]
        sys_version_or_method_invaild = 3025,
        /// <summary>
        /// jsf调用错误，参数param_json格式错误
        /// </summary>
        [Description("缺少配置参数api_host，调用失败.")]
        sys_jsf_error = 3030,

        /// <summary>
        /// jsf调用API接口连接超时
        /// </summary>
        [Description("jsf调用API接口连接超时.")]
        sys_jsf_time_out = 3035,
        /// <summary>
        /// API接口响应超时
        /// </summary>
        [Description("缺少方法名参数.")]
        sys_api_time_out = 3036,
        /// <summary>
        /// 服务器系统处理错误
        /// </summary>
        [Description("不存在的方法名或者版本号.")]
        sys_server_error = 3038,
        /// <summary>
        /// 无增值包权限，请申请开通
        /// </summary>
        [Description("无增值包权限，请申请开通.")]
        sys_package_author = 3039,
        /// <summary>
        /// appkey 已加入黑名单，被禁用
        /// </summary>
        [Description("appkey 已加入黑名单，被禁用.")]
        sys_disenabled = 3040,
        /// <summary>
        /// 已达到并发数限制
        /// </summary>
        [Description("已达到并发数限制.")]
        sys_count_limit = 3041,

        /// <summary>
        /// API 等级为 X
        /// </summary>
        [Description("API 等级为 X.")]
        sys_api_x = 3042,


        /// <summary>
        /// 获取分类信息失败
        /// </summary>
        [Description("获取分类信息失败.")]
        sys_query_category_failed = 3420,
        /// <summary>
        /// 分类不存在
        /// </summary>
        [Description("分类不存在.")]
        sys_category_not_exit = 3421,
        /// <summary>
        /// 获取分类列表信息失败
        /// </summary>
        [Description("获取分类列表信息失败.")]
        sys_query_category_list_failed = 3422,
        /// <summary>
        /// 分类列表不存在
        /// </summary>
        [Description("分类列表不存在.")]
        sys_query_category_list_not_exit= 3423,


        /// <summary>
        /// http服务返回结果json解析出错
        /// </summary>
        [Description("http服务返回结果json解析出错.")]
        sys_json_error = 4000,

        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功.")]
        sys_sucess = 0000,

        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("下单成功.")]
        sys_order_sucess = 0001,

        /// <summary>
        /// 取消订单成功
        /// </summary>
        [Description("取消订单成功.")]
        sys_cancle_order_sucess = 0002,

        /// <summary>
        /// 确认订单成功
        /// </summary>
        [Description("确认订单成功.")]
        sys_confirm_order_sucess = 0003,
        /// <summary>
        /// 申请开票成功
        /// </summary>
        [Description("申请开票成功.")]
        sys_apply_invoice_sucess = 0004,
        /// <summary>
        /// 全部开票成功
        /// </summary>
        [Description("全部开票成功.")]
        sys_create_all_invoice_sucess = 0005,
        /// <summary>
        /// 部分开票成功
        /// </summary>
        [Description("部分开票成功.")]
        sys_create_part_invoice_sucess = 0006,
        /// <summary>
        /// 取消开票成功
        /// </summary>
        [Description("取消开票成功.")]
        sys_cancle_invoice_sucess = 0007,
        /// <summary>
        /// 重复提交
        /// </summary>
        [Description("重复提交.")]
        sys_repeat_submit = 0008,
        /// <summary>
        /// 返回数据为空
        /// </summary>
        [Description("返回数据为空.")]
        sys_return_result_empty = 0010,
        /// <summary>
        /// 参数为空
        /// </summary>
        [Description("参数为空.")]
        sys_params_empty = 1001,

        /// <summary>
        /// 参数格式不正确
        /// </summary>
        [Description("参数格式不正确.")]
        sys_params_invalid = 1002,

        /// <summary>
        /// 参数值不正确
        /// </summary>
        [Description("参数值不正确.")]
        sys_params_value_invalid = 1003,
        /// <summary>
        /// 参数重复
        /// </summary>
        [Description("参数重复.")]
        sys_params_repeat = 1004,
        /// <summary>
        /// 入参转化错误
        /// </summary>
        [Description("入参转化错误.")]
        sys_params_convert_error = 1005,


        /// <summary>
        /// 用户权限不足
        /// </summary>
        [Description("用户权限不足.")]
        sys_user_perview_limit = 2001,
        /// <summary>
        /// 合同权限不足
        /// </summary>
        [Description("合同权限不足.")]
        sys_contractr_perview_limit = 2002,
        /// <summary>
        /// 企业权限不足
        /// </summary>
        [Description("企业权限不足.")]
        sys_company_perview_limit = 2003,
        /// <summary>
        /// 商品池权限不足
        /// </summary>
        [Description("商品池权限不足.")]
        sys_company_product_pool_perview_limit = 2004,

        /// <summary>
        /// 金彩权限问题
        /// </summary>
        [Description("金彩权限问题.")]
        sys_jx_perview = 2005,
        /// <summary>
        /// 无有效增票资质
        /// </summary>
        [Description("无有效增票资质.")]
        sys_pervire_ticket = 2006,
        /// <summary>
        /// token已过期
        /// </summary>
        [Description("token已过期.")]
        sys_token_had_exprise = 2007,


        /// <summary>
        /// 价格不存在
        /// </summary>
        [Description("价格不存在.")]
        sys_price_empty = 3001,
        /// <summary>
        /// 提交订单过快
        /// </summary>
        [Description("3002.")]
        sys_submit_order_fast = 3002,
        /// <summary>
        /// 订单类型不支持
        /// </summary>
        [Description("订单类型不支持.")]
        sys_order_type_no_surpot = 3003,
        /// <summary>
        /// 商品类型受限制
        /// </summary>
        [Description("商品类型受限制.")]
        sys_product_type_limit = 3004,
        /// <summary>
        /// 商品没查询到
        /// </summary>
        [Description("商品没查询到")]
        sys_oder_query_empty = 3005,
        /// <summary>
        /// 商品不能进行货到付款下单
        /// </summary>
        [Description("商品不能进行货到付款下单.")]
        sys_product_no_wait_pay = 3006,
        /// <summary>
        /// 地址不能进行货到付款下单
        /// </summary>
        [Description("地址不能进行货到付款下单.")]
        sys_area_cant_submit_order= 3007,
        /// <summary>
        /// 库存不足
        /// </summary>
        [Description("库存不足")]
        sys_stock_limit = 3008,
        /// <summary>
        /// 区域限制校验没通过
        /// </summary>
        [Description("区域限制校验没通过.")]
        sys_area_limt_error = 3009,
        /// <summary>
        /// 实体礼品卡和其他实物不能混合下单
        /// </summary>
        [Description("实体礼品卡和其他实物不能混合下单.")]
        sys_stlw_error = 3010,
        /// <summary>
        /// 大家电暂不支持公司转账 预占下单
        /// </summary>
        [Description("大家电暂不支持公司转账 预占下单")]
        sys_djt_error = 3011,
        /// <summary>
        /// 海尔仓大家电，不支持后款预占下单
        /// </summary>
        [Description("海尔仓大家电，不支持后款预占下单.")]
        sys_hr_error = 3012,

        /// <summary>
        /// 厂家直送商品只能下先款订单
        /// </summary>
        [Description("厂家直送商品只能下先款订单.")]
        sys_chzs_error = 3013,
        /// <summary>
        /// 厂家直送商品不能使用普票随货下单
        /// </summary>
        [Description("厂家直送商品不能使用普票随货下单")]
        sys_chzs_ticket_error = 3014,
        /// <summary>
        /// 实物礼品卡订单只能下普票订单
        /// </summary>
        [Description("实物礼品卡订单只能下普票订单.")]
        sys_stlw_ticket_error = 3015,
        /// <summary>
        /// 配额不足或者已被锁定
        /// </summary>
        [Description("配额不足或者已被锁定")]
        sys_pay_integer_limit = 3016,
        /// <summary>
        /// 余额不足
        /// </summary>
        [Description("余额不足.")]
        sys_integer_limit = 3017,

        /// <summary>
        /// 价格获取失败
        /// </summary>
        [Description("价格获取失败.")]
        sys_query_price_failed = 3051,
        /// <summary>
        /// 主数据接口业务异常
        /// </summary>
        [Description("主数据接口业务异常.")]
        sys_api_service_error = 3052,
        /// <summary>
        /// 商品基本信息接口调用失败
        /// </summary>
        [Description("商品基本信息接口调用失败.")]
        sys_find_product_api_error = 3053,
        /// <summary>
        /// 商品扩展接口调用失败
        /// </summary>
        [Description("商品扩展接口调用失败.")]
        sys_find_product_extend_api_error = 3054,

        /// <summary>
        /// 大家电接口调用失败
        /// </summary>
        [Description("大家电接口调用失败.")]
        sys_djd_api_error = 3055,
        /// <summary>
        /// 赠品附件接口调用失败
        /// </summary>
        [Description("赠品附件接口调用失败.")]
        sys_zpfj_api_error = 3056,
        /// <summary>
        /// 区分大家电和中小件商品失败
        /// </summary>
        [Description("区分大家电和中小件商品失败.")]
        sys_find_djdzxjsp_failed = 3057,
        /// <summary>
        /// 下单失败，请重新提交订单
        /// </summary>
        [Description("下单失败，请重新提交订单.")]
        sys_make_order_error_resubmit = 3058,


        /// <summary>
        /// 确认下单最终失败，请重新确认订单
        /// </summary>
        [Description("确认下单最终失败，请重新确认订单.")]
        sys_confiem_make_order_error = 3101,

        /// <summary>
        /// jdOrderId不存在
        /// </summary>
        [Description("jdOrderId不存在.")]
        sys_jdOrderId_had_exit = 3102,
        /// <summary>
        /// 该订单已确认下单
        /// </summary>
        [Description("该订单已确认下单.")]
        sys_order_had_confirem = 3103,
        /// <summary>
        /// 不能单独确认子订单
        /// </summary>
        [Description("不能单独确认子订单.")]
        sys_cant_confirm_children_order_only = 3104,
        /// <summary>
        /// 订单对应子单已取消，不能确认！
        /// </summary>
        [Description("订单对应子单已取消，不能确认！.")]
        sys_children_order_had_cancle_cant_confirm = 3105,

        /// <summary>
        /// 查询子单异常
        /// </summary>
        [Description("查询子单异常.")]
        sys_query_children_order_error = 3106,
        /// <summary>
        /// 本地子单与ERP子单不一致
        /// </summary>
        [Description("本地子单与ERP子单不一致.")]
        sys_odrer_no_not_equal_erp_order_no = 3107,
        /// <summary>
        /// 确认订单操作失败
        /// </summary>
        [Description("确认订单操作失败.")]
        sys_confirm_failed= 3108,



        /// <summary>
        /// 取消订单失败，请重新取消订单
        /// </summary>
        [Description("取消订单失败，请重新取消订单.")]
        sys_cancle_order_error = 3201,
        /// <summary>
        /// jdOrderId不存在
        /// </summary>
        [Description("jdOrderId不存在.")]
        sys_jdorderId_error = 3302,
        /// <summary>
        /// 该订单已经被取消
        /// </summary>
        [Description("该订单已经被取消.")]
        sys_oder_had_cancle = 3203,
        /// <summary>
        /// 不能取消已经生产订单
        /// </summary>
        [Description("不能取消已经生产订单.")]
        sys_cant_cancle_had_make_order = 3204,
        /// <summary>
        /// 不能取消未确认订单
        /// </summary>
        [Description("不能取消未确认订单.")]
        sys_cant_cancle_no_confirm_order= 3205,
        /// <summary>
        /// 不能取消预占并且已确认订单
        /// </summary>
        [Description("不能取消预占并且已确认订单.")]
        sys_cant_cancle_prehold_and_confirm_order = 3206,
        /// <summary>
        /// 不能取消父订单
        /// </summary>
        [Description("不能取消父订单.")]
        sys_cant_cancle_parent_order = 3207,
        /// <summary>
        /// 不能取消已确认订单
        /// </summary>
        [Description("不能取消已确认订单.")]
        sys_cant_cancle_confirmed_order = 3208,
        /// <summary>
        /// 不能取消子订单
        /// </summary>
        [Description("不能取消子订单.")]
        sys_cant_cancle_children_order = 3209,
        /// <summary>
        /// 查询子单异常
        /// </summary>
        [Description("查询子单异常.")]
        sys_query_children_order_failed = 3210,
        /// <summary>
        /// 取消订单失败，存在已确认的子单
        /// </summary>
        [Description("取消订单失败，存在已确认的子单.")]
        sys_cancle_confiemd_order_error = 3211,
        /// <summary>
        /// 取消订单操作失败
        /// </summary>
        [Description("取消订单操作失败.")]
        sys_cancle_order_failed = 3212,


        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核.")]
        sys_wait_audit = 3301,
        /// <summary>
        /// 驳回
        /// </summary>
        [Description("驳回.")]
        sys_reject = 3302,
        /// <summary>
        /// 通过待开票
        /// </summary>
        [Description("通过待开票.")]
        sys_pass_wait_tikect = 3403,
        /// <summary>
        /// 申请单不存在
        /// </summary>
        [Description("申请单不存在.")]
        sys_apply_order_invalid = 3304,



        /// <summary>
        /// 订单不存在
        /// </summary>
        [Description("订单不存在.")]
        sys_order_not_exit = 3401,
        /// <summary>
        /// 订单配送信息不存在
        /// </summary>
        [Description("订单配送信息不存在.")]
        sys_order_give_not_exit = 3402,
        /// <summary>
        /// 支付时余额不足
        /// </summary>
        [Description("支付时余额不足.")]
        sys_pay_no_have_enough_money = 3403,
        /// <summary>
        /// 订单不能发起支付
        /// </summary>
        [Description("订单不能发起支付.")]
        sys_order_cant_pay = 3404,
        /// <summary>
        /// 没查询到对应地址
        /// </summary>
        [Description("没查询到对应地址.")]
        sys_order_no_have_adress = 3405,
        /// <summary>
        /// 价格不存在
        /// </summary>
        [Description("价格不存在.")]
        sys_no_have_price = 3406,
        /// <summary>
        /// 获取余额业务异常
        /// </summary>
        [Description("获取余额业务异常.")]
        sys_find_intger_error = 3407,
        /// <summary>
        /// 该支付类型不支持余额查询
        /// </summary>
        [Description("该支付类型不支持余额查询.")]
        sys_cant_query_intger = 3408,
        /// <summary>
        /// 区分大家电和中小件商品失败
        /// </summary>
        [Description("区分大家电和中小件商品失败.")]
        sys_cant_find_type = 3409,
        /// <summary>
        /// 订单未被挂起
        /// </summary>
        [Description("订单未被挂起.")]
        sys_order_holdon_error = 3410,
        /// <summary>
        /// 服务异常，请稍后重试
        /// </summary>
        [Description("服务异常，请稍后重试.")]
        sys_error = 5001,
        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("未知错误.")]
        sys_unknow_error= 5002,
    }
}
