using JDVOP.Dto;
using JDVOP.Interface;
using JDVOP.Model;
using JDVOP.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JDVOP.Extensions;
using System.Web;

namespace JDVOP
{
    /// <summary>
    /// 京东业务调用
    /// </summary>
    public class JDMallIBLL
    {
        MallInterface service = new JDMallService();
        private OperateResult<T> Result<T>(bool isSuccess, string msg, T model)
        {
            return new OperateResult<T>() { IsSuccess = isSuccess, Msg = msg, Result = model };
        }

        private OperateResult<T> ErrorResult<T>(JDErrorCode code, T t)
        {
            return Result(false, code.GetDescription(), default(T));
        }
        private OperateResult<T> ErrorResult<T>(string msg, T t)
        {
            return Result(false, msg, default(T));
        }

        private OperateResult<T> Result<T>(T model)
        {
            return Result(true, string.Empty, model);
        }




        /// <summary>
        /// 下订单
        /// </summary>
        /// <param name="orderId">本地订单id</param>
        /// <param name="skuNums">商品id 数量集合</param>
        /// <param name="name">用户名</param>
        /// <param name="mobile">手机</param>
        /// <param name="email">邮箱</param>
        /// <param name="address">地址</param>
        /// <param name="zip">邮编</param>
        /// <param name="phone">座机</param>
        /// <param name="remark">备注</param>
        /// <param name="province">省份</param>
        /// <param name="city">市</param>
        /// <param name="county">县区</param>
        /// <param name="town">城镇</param>
        /// <returns></returns>
        public OperateResult<JDOrderDto> CreateOrder(string orderId, List<SkuNum> skuNums, string name, string mobile, string address, string phone, string remark, int province, int city, int county, InvoiceType invoiceType, int town = 0)
        {
            //检查参数
            if (skuNums == null || skuNums.Count == 0 || name.IsNullOrEmpty() || mobile.IsNullOrEmpty() || orderId.IsNullOrEmpty())
                return ErrorResult(JDErrorCode.sys_params_empty, default(JDOrderDto));

            if ((name.IsNotNullOrEmpty() && name.Length > 10) || (mobile.IsNotNullOrEmpty() && mobile.Length != 11) || (remark.IsNotNullOrEmpty() && remark.Length > 100))
                return ErrorResult(JDErrorCode.sys_params_invalid, default(JDOrderDto));


            var skuIdList = skuNums.Select(x => x.skuId).ToList();
            //检查商品可售性
            var saleResult = service.CheckProductCanSale(skuIdList);
            if (!saleResult.success)
                return ErrorResult(saleResult.code, default(JDOrderDto));
            //id name集合
            var skuDic = saleResult.result.ToDictionary(x => x.skuId, x => x.name);
            //判断商品是否全部可售
            if (saleResult.result.Where(x => x.saleState == YesOrNoCode.No).Any())
            {
                var cantSaleNames = saleResult.result.Where(x => x.saleState == YesOrNoCode.No).Select(x => x.name).ToList();
                return ErrorResult($"商品{string.Join(",", cantSaleNames)}不可售", default(JDOrderDto));
            }
            //库存
            var stockResult = service.CheckStockForOne(skuNums, province, city, county, town);
            if (stockResult.result.Where(x => x.stockStateDesc == "无货").Any())
            {
                var cantSaleNames = stockResult.result.Where(x => x.stockStateDesc == "无货").Select(x => skuDic.ContainsKey(x.skuId) ? skuDic[x.skuId] : "").ToList();
                return ErrorResult($"商品{string.Join(",", cantSaleNames)}无货", default(JDOrderDto));
            }

            //检查地区限制
            var areaLimitResult = service.CheckProductAreaLimit(skuIdList, province, city, county, town);
            if (!areaLimitResult.success)
                return ErrorResult(areaLimitResult.code, default(JDOrderDto));
            if (areaLimitResult.result.Where(x => x.isAreaRestrict == true).Any())
            {
                var cantSaleNames = areaLimitResult.result.Where(x => x.isAreaRestrict == true).Select(x => skuDic.ContainsKey(x.skuId) ? skuDic[x.skuId] : "").ToList();
                return ErrorResult($"商品{string.Join(",", cantSaleNames)}地区限制不可售", default(JDOrderDto));
            }

            //统计下单
            var orderModel = new OrderDto()
            {
                thirdOrder = orderId,
                name = name,
                mobile = mobile,
                phone = phone,
                address = address,
                province = province,
                city = city,
                county = county,
                town = town,
                sku = skuNums.Select(x => new OrderSku() { num = x.num, skuId = x.skuId }).ToList(),
                remark = remark,
                invoiceType= invoiceType
            };
            var orderResult = service.CreateOrder(orderModel);
            if (!orderResult.success)
                return ErrorResult(orderResult.code, default(JDOrderDto));

            //京东订单id
            var jdOrderId = orderResult.result.jdOrderId;

            //确认预占库存订单
            var occupyStockResult = service.ConfirmOccupyStock(jdOrderId);
            if (!occupyStockResult.success)
                return ErrorResult(orderResult.resultMessage, default(JDOrderDto));


            return Result(orderResult.result);

        }


        /// <summary>
        /// 下订单
        /// </summary>
        /// <param name="orderId">本地订单id</param>
        /// <param name="skuNums">商品id 数量集合</param>
        /// <param name="name">用户名</param>
        /// <param name="mobile">手机</param>
        /// <param name="email">邮箱</param>
        /// <param name="address">地址</param>
        /// <param name="zip">邮编</param>
        /// <param name="phone">座机</param>
        /// <param name="remark">备注</param>
        /// <param name="province">省份</param>
        /// <param name="city">市</param>
        /// <param name="county">县区</param>
        /// <param name="town">城镇</param>
        /// <returns></returns>
        public OperateResult<bool> CheckProduct(List<SkuNum> skuNums, int province, int city, int county, int town = 0)
        {
            //检查参数
            if (skuNums == null || skuNums.Count == 0)
                return ErrorResult(JDErrorCode.sys_params_empty, false);

            var skuIdList = skuNums.Select(x => x.skuId).ToList();
            //检查商品可售性
            var saleResult = service.CheckProductCanSale(skuIdList);
            if (!saleResult.success)
                return ErrorResult(saleResult.code, false);
            //id name集合
            var skuDic = saleResult.result.ToDictionary(x => x.skuId, x => x.name);
            //判断商品是否全部可售
            if (saleResult.result.Where(x => x.saleState == YesOrNoCode.No).Any())
            {
                var cantSaleNames = saleResult.result.Where(x => x.saleState == YesOrNoCode.No).Select(x => x.name).ToList();
                return ErrorResult($"商品{string.Join(",", cantSaleNames)}不可售", false);
            }
            //库存
            var stockResult = service.CheckStockForOne(skuNums, province, city, county, town);
            if (stockResult.result.Where(x => x.stockStateDesc == "无货").Any())
            {
                var cantSaleNames = stockResult.result.Where(x => x.stockStateDesc == "无货").Select(x => skuDic.ContainsKey(x.skuId) ? skuDic[x.skuId] : "").ToList();
                return ErrorResult($"商品{string.Join(",", cantSaleNames)}无货", false);
            }

                //检查地区限制
                var areaLimitResult = service.CheckProductAreaLimit(skuIdList, province, city, county, town);
            if (!areaLimitResult.success)
                return ErrorResult(areaLimitResult.code, false);
            if (areaLimitResult.result.Where(x => x.isAreaRestrict == true).Any())
            {
                var cantSaleNames = areaLimitResult.result.Where(x => x.isAreaRestrict == true).Select(x => skuDic.ContainsKey(x.skuId) ? skuDic[x.skuId] : "").ToList();
                return ErrorResult($"商品{string.Join(",", cantSaleNames)}地区限制不可售", false);
            }

            return Result(true);

        }


        /// <summary>
        /// 取消未确认订单(自己订单id)
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <returns></returns>
        public OperateResult<bool> CancleOrder(string orderId)
        {
            if (orderId.IsNullOrEmpty())
                return ErrorResult(JDErrorCode.sys_params_empty, false);
            var jdOrderResult = service.QueryJDOrderID(orderId);
            if (!jdOrderResult.success)
                return ErrorResult(jdOrderResult.resultMessage, false);
            //确认预占库存订单
            var orderResult = service.CancleOrder(jdOrderResult.result);
            if (!orderResult.success)
                return ErrorResult(orderResult.resultMessage, false);
            return Result(orderResult.result);
        }

        /// <summary>
        /// 查询订单物流(自己订单id)
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <returns></returns>
        public OperateResult<List<TrackDetial>> QueryOrderTrack(string orderId)
        {
            if (orderId.IsNullOrEmpty())
                return ErrorResult(JDErrorCode.sys_params_empty, default(List<TrackDetial>));
            var jdOrderResult = service.QueryJDOrderID(orderId);
            if (!jdOrderResult.success)
                return ErrorResult(jdOrderResult.resultMessage, default(List<TrackDetial>));
            //查询订单物流
            var orderResult = service.QueryOrderTrack(jdOrderResult.result);
            if (!orderResult.success)
                return ErrorResult(orderResult.resultMessage, default(List<TrackDetial>));
            return Result(orderResult.result.orderTrack);
        }

        /// <summary>
        /// 查询地区数据
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <returns></returns>
        public OperateResult<string> QueryArea()
        {

            int areaType, parentId = 0;
            int.TryParse(HttpContext.Current.Request["type"], out areaType);
            int.TryParse(HttpContext.Current.Request["parentId"], out parentId);

            var type = (AreaType)areaType;
            var model = new RegionObject();
            switch (type)
            {
                case AreaType.Province:
                    var provinceResult = service.QueryProvinces();
                    if (!provinceResult.success)
                        return ErrorResult(provinceResult.resultMessage, string.Empty);
                    else
                    {
                        model.Regions = provinceResult.result.Select(x => new Region() { RegionId = x.Value.ToString(), RegionName = x.Key }).ToList();
                        model.Status = "OK";
                        return Result(model.ToJson());
                    }
                case AreaType.City:
                    var cityResult = service.QueryCityByProvnicesId(parentId);
                    if (!cityResult.success)
                        return ErrorResult(cityResult.resultMessage, model.ToJson());
                    else
                    {
                        model.Regions = cityResult.result.Select(x => new Region() { RegionId = x.Value.ToString(), RegionName = x.Key }).ToList();
                        model.Status = "OK";
                        return Result(model.ToJson());
                    }
                case AreaType.County:
                    var countyResult = service.QueryCountysByCityId(parentId);
                    if (!countyResult.success)
                        return ErrorResult(countyResult.resultMessage, model.ToJson());
                    else
                    {
                        model.Regions = countyResult.result.Select(x => new Region() { RegionId = x.Value.ToString(), RegionName = x.Key }).ToList();
                        model.Status = "OK";
                        return Result(model.ToJson());
                    }
                case AreaType.Town:
                    var townResult = service.QueryTownsByCountyId(parentId);
                    if (!townResult.success)
                        return ErrorResult(townResult.resultMessage, model.ToJson());
                    else
                    {
                        model.Regions = townResult.result.Select(x => new Region() { RegionId = x.Value.ToString(), RegionName = x.Key }).ToList();
                        model.Status = "OK";
                        return Result(model.ToJson());
                    }
                default:
                    return Result(model.ToJson());
            }
        }


        /// <summary>
        /// 余额查询
        /// </summary>
        /// <param name="type">支付方式（默认余额）</param>
        /// <returns></returns>
        public OperateResult<decimal> QueryBalance(PaymentType type = PaymentType.OnLine)
        {
            var orderResult = service.QueryBalance(type);
            if (!orderResult.success)
                return ErrorResult(orderResult.resultMessage, default(decimal));
            return Result(orderResult.result);
        }
    }
}
