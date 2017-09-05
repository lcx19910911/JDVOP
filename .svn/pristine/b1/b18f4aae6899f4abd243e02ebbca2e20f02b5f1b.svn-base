using JDVOP.Dto;
using JDVOP.Model;
using System;
using System.Collections.Generic;

namespace JDVOP.Interface
{
    public interface MallInterface
    {

        #region 商品

        /// <summary>
        /// 查询商品池编号接口
        /// </summary>
        /// <returns></returns>
        JDValueResult<List<ProductPoolDto>> QueryPageNum();

        /// <summary>
        /// 查询池内商品编号接口
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        JDValueResult<string> QueryProductSkuByPageNum(string pageNum);

        /// <summary>
        /// 查询商品详情
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<ProductInfo> FindProductDetial(long skuId);

        /// <summary>
        /// 查询图书商品详情
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<ProductBook> FindBookDetial(long skuId);

        /// <summary>
        /// 查询商品状态
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<List<ProductState>> QueryProductState(List<long> skuIdList);

        /// <summary>
        /// 查询商品价格
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<List<ProductPrice>> QueryProductPrice(List<long> skuIdList);

        /// <summary>
        /// 查询商品图片
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<Dictionary<long, List<ProductImage>>> QueryProductImage(List<long> skuIdList);

        /// <summary>
        /// 查询商品好评度接口
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<List<CommentSummarys>> QueryProductCommentSummarys(List<long> skuIdList);


        /// <summary>
        /// 查询商品区域购买限制接口
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <param name="county">县区</param>
        /// <param name="town">城镇</param>
        /// <returns></returns>
        JDValueResult<List<CheckAreaLimit>> CheckProductAreaLimit(List<long> skuIdList, int province, int city, int county, int town);

        /// <summary>
        /// 运费查询
        /// </summary>
        /// <param name="skuNumList">商品ID数量集合</param>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <param name="county">县区</param>
        /// <param name="town">城镇</param>
        /// <param name="paymentType">支付方式</param>
        /// <returns></returns>
        JDValueResult<ResponseFreight> QueryProductFreight(List<SkuNum> skuNumList, int province, int city, int county, int town, PaymentType paymentType);


        /// <summary>
        /// 商品可售验证接口
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<List<ProductCheckRep>> CheckProductCanSale(List<long> skuIdList);


        #endregion


        #region 分类
        /// <summary>
        /// 分页查询分类
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">数量</param>
        /// <param name="parentId">父类ID</param>
        /// <param name="catClass">分类登记</param>
        /// <returns></returns>
        JDValueResult<CategoryPageList> QueryCategorys(int pageNo, int pageSize, int? parentId, CategoryClass? catClass);

        /// <summary>
        /// 获取分类详情
        /// </summary>
        /// <param name="cid ">商品ID</param>
        /// <returns></returns>
        JDValueResult<CategoryInfo> FindCategory(long cid);
        #endregion

        #region 库存

        /// <summary>
        /// 检查商品库存
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<List<StockDto>> CheckStockForOne(List<SkuNum> skuNums, int provinceId, int cityId, int countyId, int? townId);

        /// <summary>
        /// 检查列表的商品库存
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        JDValueResult<List<ListStockDto>> CheckStockForList(List<long> skuIdList, int provinceId, int cityId, int countyId, int? townId);



        #endregion

        #region 搜索
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="catId">分类id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <param name="min">最小价格</param>
        /// <param name="max">最大价格</param>
        /// <param name="brands">商标集合</param>
        /// <returns></returns>
        JDValueResult<SearchDto> QueryCategorys(string keyword, string catId, int pageIndex, int pageSize, string min, string max, List<string> brands);

        #endregion


        #region 地区相关
        /// <summary>
        /// 查询省份地区
        /// </summary>
        /// <returns></returns>
        JDValueResult<Dictionary<string, int>> QueryProvinces();

        /// <summary>
        /// 根据省份id查询市地区
        /// </summary>
        /// <returns></returns>
        JDValueResult<Dictionary<string, int>> QueryCityByProvnicesId(int provniceId);

        /// <summary>
        /// 根据市id查询县区地区
        /// </summary>
        /// <returns></returns>
        JDValueResult<Dictionary<string, int>> QueryCountysByCityId(int cityId);

        /// <summary>
        /// 根据市id查询县区地区
        /// </summary>
        /// <returns></returns>
        JDValueResult<Dictionary<string, int>> QueryTownsByCountyId(int countyId);
        #endregion

        #region 订单
        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        JDValueResult<JDOrderDto> CreateOrder(OrderDto model);

        /// <summary>
        /// 确认预占库存订单接口
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        JDValueResult<bool> ConfirmOccupyStock(long jdOrderId);
        /// <summary>
        /// 取消未确认订单接口 根据京东订单号，取消未确认订单。与“确认预占库存订单接口“配套使用，用于取消未确认的订单
        /// 该接口仅能取消未确认的预占库存父订单单号。不能取消已经确认的订单单号。
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        JDValueResult<bool> CancleOrder(long jdOrderId);

        /// <summary>
        /// 根第三方订单id查询京东订单id
        /// </summary>
        /// <param name="orderID">第三方订单id</param>
        /// <returns></returns>
        JDValueResult<long> QueryJDOrderID(string orderID);

        /// <summary>
        /// 查询订单物流
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        JDValueResult<OrderTrack> QueryOrderTrack(long jdOrderId);


        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        JDValueResult<JDOrderDto> QueryJDOrderInfo(long jdOrderId);


        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        JDValueResult<JDOrderDto> QueryJDOrderInfo(string CMCCOrderId);

        #endregion

        #region 余额查询

        /// <summary>
        /// 余额查询
        /// </summary>
        /// <param name="type">支付方式（默认余额）</param>
        /// <returns></returns>
        JDValueResult<decimal> QueryBalance(PaymentType type = PaymentType.OnLine);

        #endregion


        #region 消息api
        /// <summary>
        /// 消息api
        /// </summary>
        /// <param name="action">处理方法</param>
        /// <param name="msgTypes">请求消息</param>
        /// <returns></returns>
        void GetMessage<T>(Action<T> action, string msgTypes);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="msgId">消息id</param>
        /// <returns></returns>
        JDValueResult<bool> DeleteMessage(long msgId);
        #endregion
    }
}
