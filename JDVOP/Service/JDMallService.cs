using JDVOP.Dto;
using JDVOP.Extensions;
using JDVOP.Helper;
using JDVOP.Interface;
using JDVOP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Service
{
    public class JDMallService : MallInterface
    {

        #region 商品相关
        /// <summary>
        /// 查询商品池编号接口
        /// </summary>
        /// <returns></returns>
        public JDValueResult<List<ProductPoolDto>> QueryPageNum()
        {
            return JDRequestHelper<List<ProductPoolDto>>.GetResult("biz.product.PageNum.query", string.Empty);
        }

        /// <summary>
        /// 查询池内商品编号接口
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public JDValueResult<string> QueryProductSkuByPageNum(string pageNum)
        {
            if (pageNum.IsNullOrEmpty())
                return new JDValueResult<string>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<string>.GetResult("biz.product.sku.query", new { pageNum = pageNum }.ToJson());
        }

        /// <summary>
        /// 查询商品详情
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public JDValueResult<ProductInfo> FindProductDetial(long skuId)
        {
            if (skuId <= 0)
                return new JDValueResult<ProductInfo>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<ProductInfo>.GetResult("biz.product.detail.query", new { sku = skuId }.ToJson());
        }

        /// <summary>
        /// 查询图书商品详情
        /// </summary>
        /// <param name="skuId">商品ID</param>
        /// <returns></returns>
        public JDValueResult<ProductBook> FindBookDetial(long skuId)
        {
            if (skuId.ToString().Length != 8)
                return new JDValueResult<ProductBook>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<ProductBook>.GetResult("biz.product.detail.query", new { sku = skuId }.ToJson());
        }

        /// <summary>
        /// 查询商品状态
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        public JDValueResult<List<ProductState>> QueryProductState(List<long> skuIdList)
        {
            if (skuIdList == null || skuIdList.Count == 0 || skuIdList.Count > 100)
                return new JDValueResult<List<ProductState>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<List<ProductState>>.GetResult("biz.product.state.query", new { sku = string.Join(",", skuIdList) }.ToJson());

        }

        /// <summary>
        /// 查询商品价格
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        public JDValueResult<List<ProductPrice>> QueryProductPrice(List<long> skuIdList)
        {
            if (skuIdList == null || skuIdList.Count == 0 || skuIdList.Count > 100)
                return new JDValueResult<List<ProductPrice>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<List<ProductPrice>>.GetResult("biz.price.sellPrice.get", new { sku = string.Join(",", skuIdList) }.ToJson());

        }

        /// <summary>
        /// 查询商品图片
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        public JDValueResult<Dictionary<long, List<ProductImage>>> QueryProductImage(List<long> skuIdList)
        {
            if (skuIdList == null || skuIdList.Count == 0 || skuIdList.Count > 100)
                return new JDValueResult<Dictionary<long, List<ProductImage>>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<Dictionary<long, List<ProductImage>>>.GetResult("biz.product.skuImage.query", new { sku = string.Join(",", skuIdList) }.ToJson());
        }


        /// <summary>
        /// 查询商品好评度接口
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        public JDValueResult<List<CommentSummarys>> QueryProductCommentSummarys(List<long> skuIdList)
        {
            if (skuIdList == null || skuIdList.Count == 0 || skuIdList.Count > 50)
                return new JDValueResult<List<CommentSummarys>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<List<CommentSummarys>>.GetResult("biz.product.commentSummarys.query", new { sku = string.Join(",", skuIdList) }.ToJson());
        }


        /// <summary>
        /// 查询商品区域购买限制接口
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <param name="county">县区</param>
        /// <param name="town">城镇</param>
        /// <returns></returns>
        public JDValueResult<List<CheckAreaLimit>> CheckProductAreaLimit(List<long> skuIdList, int province, int city, int county, int town)
        {
            if (skuIdList == null || skuIdList.Count == 0 || skuIdList.Count > 50)
                return new JDValueResult<List<CheckAreaLimit>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<List<CheckAreaLimit>>.GetResult("biz.product.checkAreaLimit.query", new { skuIds = string.Join(",", skuIdList), province = province, city = city, county = county, town = town }.ToJson());
        }

        /// <summary>
        /// 商品可售验证接口
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        public JDValueResult<List<ProductCheckRep>> CheckProductCanSale(List<long> skuIdList)
        {
            if (skuIdList == null || skuIdList.Count == 0 || skuIdList.Count > 100)
                return new JDValueResult<List<ProductCheckRep>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<List<ProductCheckRep>>.GetResult("biz.product.sku.check", new { skuIds = string.Join(",", skuIdList) }.ToJson());
        }


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
        public JDValueResult<ResponseFreight> QueryProductFreight(List<SkuNum> skuNumList, int province, int city, int county, int town, PaymentType paymentType)
        {
            if (skuNumList == null || skuNumList.Count == 0 || skuNumList.Count > 50)
                return new JDValueResult<ResponseFreight>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            var requestModel = new RequestFreight()
            {
                province = province,
                city = city,
                county = county,
                town = town,
                paymentType = paymentType,
                sku = skuNumList
            };
            return JDRequestHelper<ResponseFreight>.GetResult("biz.order.freight.get", requestModel.ToJson());


        }

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
        public JDValueResult<CategoryPageList> QueryCategorys(int pageNo, int pageSize, int? parentId, CategoryClass? catClass)
        {
            if (pageNo < 1 || pageSize > 100 || pageSize < 1)
                return new JDValueResult<CategoryPageList>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<CategoryPageList>.GetResult("jd.biz.product.getcategorys", new { pageNo = pageNo, pageSize = pageSize, parentId = parentId??0, catClass = catClass }.ToJson());
        }



        /// <summary>
        /// 获取分类详情
        /// </summary>
        /// <param name="cid ">商品ID</param>
        /// <returns></returns>
        public JDValueResult<CategoryInfo> FindCategory(long cid)
        {
            if (cid <= 0)
                return new JDValueResult<CategoryInfo>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<CategoryInfo>.GetResult("jd.biz.product.getcategory", new { cid = cid }.ToJson());
        }
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
        public JDValueResult<SearchDto> QueryCategorys(string keyword, string catId, int pageIndex, int pageSize, string min, string max, List<string> brands)
        {
            if (pageIndex < 1 || pageSize < 1)
                return new JDValueResult<SearchDto>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            if (brands != null && brands.Count > 0)
            {
                brands.ForEach(x =>
                {
                    x = System.Web.HttpUtility.UrlEncode(x);
                });
            }
            return JDRequestHelper<SearchDto>.GetResult("jd.biz.search.search", new { pageIndex = pageIndex, pageSize = pageSize, keyword = keyword.IsNotNullOrEmpty() ? System.Web.HttpUtility.UrlEncode(keyword) : null, catId = catId, min = min, max = max, brands = string.Join(",", brands) }.ToJson());
        }
        #endregion

        #region 地区相关
        /// <summary>
        /// 查询省份地区
        /// </summary>
        /// <returns></returns>
        public JDValueResult<Dictionary<string, int>> QueryProvinces()
        {
            return JDRequestHelper<Dictionary<string, int>>.GetResult("biz.address.allProvinces.query", null);
        }
        /// <summary>
        /// 根据省份id查询市地区
        /// </summary>
        /// <returns></returns>
        public JDValueResult<Dictionary<string, int>> QueryCityByProvnicesId(int provniceId)
        {
            return JDRequestHelper<Dictionary<string, int>>.GetResult("biz.address.citysByProvinceId.query", new { id = provniceId }.ToJson());
        }
        /// <summary>
        /// 根据市id查询县区地区
        /// </summary>
        /// <returns></returns>
        public JDValueResult<Dictionary<string, int>> QueryCountysByCityId(int cityId)
        {
            return JDRequestHelper<Dictionary<string, int>>.GetResult("biz.address.countysByCityId.query", new { id = cityId }.ToJson());
        }

        /// <summary>
        /// 根据市id查询县区地区
        /// </summary>
        /// <returns></returns>
        public JDValueResult<Dictionary<string, int>> QueryTownsByCountyId(int countyId)
        {
            return JDRequestHelper<Dictionary<string, int>>.GetResult("biz.address.townsByCountyId.query", new { id = countyId }.ToJson());
        }

        #endregion

        #region 库存

        /// <summary>
        /// 检查商品库存
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        public JDValueResult<List<StockDto>> CheckStockForOne(List<SkuNum> skuNums, int provinceId, int cityId, int countyId, int? townId)
        {
            if (skuNums == null || skuNums.Count == 0)
                return new JDValueResult<List<StockDto>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            string areaStr = string.Empty;
            if (townId.HasValue)
            {
                areaStr = $"{provinceId}_{cityId}_{countyId}_{townId.Value}";
            }
            else
            {
                areaStr = $"{provinceId}_{cityId}_{countyId}";
            }
            return JDRequestHelper<List<StockDto>>.GetResult("biz.stock.fororder.batget", new { skuNums = skuNums, area = areaStr }.ToJson());
        }


        /// <summary>
        /// 检查列表的商品库存
        /// </summary>
        /// <param name="skuIdList">商品ID集合</param>
        /// <returns></returns>
        public JDValueResult<List<ListStockDto>> CheckStockForList(List<long> skuIdList, int provinceId, int cityId, int countyId, int? townId)
        {
            if (skuIdList == null || skuIdList.Count == 0)
                return new JDValueResult<List<ListStockDto>>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            string areaStr = string.Empty;
            if (townId.HasValue)
            {
                areaStr = $"{provinceId}_{cityId}_{countyId}_{townId.Value}";
            }
            else
            {
                areaStr = $"{provinceId}_{cityId}_{countyId}";
            }
            return JDRequestHelper<List<ListStockDto>>.GetResult("biz.stock.forList.batget", new { sku = string.Join(",", skuIdList), area = areaStr }.ToJson());
        }



        #endregion


        #region 订单
        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        public JDValueResult<JDOrderDto> CreateOrder(OrderDto model)
        {
            if (model == null)
                return new JDValueResult<JDOrderDto>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<JDOrderDto>.GetResult("biz.order.unite.submit", model.ToJson());
        }

        /// <summary>
        /// 确认预占库存订单接口
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        public JDValueResult<bool> ConfirmOccupyStock(long jdOrderId)
        {
            if (jdOrderId <= 0)
                return new JDValueResult<bool>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<bool>.GetResult("biz.order.occupyStock.confirm", new { jdOrderId = jdOrderId }.ToJson());
        }
        /// <summary>
        /// 取消未确认订单接口 根据京东订单号，取消未确认订单。与“确认预占库存订单接口“配套使用，用于取消未确认的订单
        /// 该接口仅能取消未确认的预占库存父订单单号。不能取消已经确认的订单单号。
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        public JDValueResult<bool> CancleOrder(long jdOrderId)
        {
            if (jdOrderId <= 0)
                return new JDValueResult<bool>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<bool>.GetResult("biz.order.cancelorder", new { jdOrderId = jdOrderId }.ToJson());
        }
        /// <summary>
        /// 根第三方订单id查询京东订单id
        /// </summary>
        /// <param name="orderID">第三方订单id</param>
        /// <returns></returns>
        public JDValueResult<long> QueryJDOrderID(string orderID)
        {
            if (orderID.IsNullOrEmpty())
                return new JDValueResult<long>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<long>.GetResult("biz.order.jdOrderIDByThridOrderID.query", new { thirdOrder = orderID }.ToJson());
        }

        /// <summary>
        /// 查询订单物流
        /// </summary>
        /// <param name="jdOrderId">订单id</param>
        /// <returns></returns>
        public JDValueResult<OrderTrack> QueryOrderTrack(long jdOrderId)
        {
            if (jdOrderId <= 0)
                return new JDValueResult<OrderTrack>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return JDRequestHelper<OrderTrack>.GetResult("biz.order.orderTrack.query", new { jdOrderId = jdOrderId }.ToJson());
        }


        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="jdOrderId">京东订单id</param>
        /// <returns></returns>
        public JDValueResult<JDOrderDto> QueryJDOrderInfo(long jdOrderId)
        {
            if (jdOrderId <= 0)
                return new JDValueResult<JDOrderDto>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            var result=JDRequestHelper<JDOrderDto>.GetResult("biz.order.jdOrder.query", new { jdOrderId = jdOrderId }.ToJson(),1);

            return result;
        }


        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderID">商城订单id</param>
        /// <returns></returns>
        public JDValueResult<JDOrderDto> QueryJDOrderInfo(string orderID)
        {
            var jdOrderIdResult = QueryJDOrderID(orderID);
            if (!jdOrderIdResult.success)
                return new JDValueResult<JDOrderDto>()
                {
                    success = false,
                    resultCode = JDErrorCode.sys_param_json_error.ToString()
                };
            return QueryJDOrderInfo(jdOrderIdResult.result);
        }


        #endregion


        #region 余额查询

        /// <summary>
        /// 余额查询
        /// </summary>
        /// <param name="type">支付方式（默认余额）</param>
        /// <returns></returns>
        public JDValueResult<decimal> QueryBalance(PaymentType type = PaymentType.OnLine)
        {
            return JDRequestHelper<decimal>.GetResult("biz.price.balance.get", new { payType = type }.ToJson());
        }

        #endregion

        #region 消息api
        /// <summary>
        /// 消息api
        /// </summary>
        /// <param name="action">处理方法</param>
        /// <param name="msgTypes">请求消息 1,2,3  1.拆单 2.价格变动 4商品上下架 5订单妥投 6添加商品、删除商品池商品 10订单取消 11申请开票 12配送单生成 13换新订单 14支付失败 15-7天未支付 16商品介绍和规格参数变化 50地址变更</param>
        /// <returns></returns>
        public void GetMessage<T>(Action<T> action,string msgTypes)
        {
            var msgResult=JDRequestHelper<T>.GetResult("biz.message.get", new { type = msgTypes }.ToJson());
            if (msgResult.success)
            {
                action(msgResult.result);
            }
        }
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="msgId">消息id</param>
        /// <returns></returns>
        public JDValueResult<bool> DeleteMessage(long msgId)
        {
            return JDRequestHelper<bool>.GetResult("biz.message.del", new { id = msgId }.ToJson());
        }
        
        #endregion

    }
}
