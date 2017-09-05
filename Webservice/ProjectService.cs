﻿using Entity;
using JDVOP;
using JDVOP.Dto;
using JDVOP.Extensions;
using JDVOP.Helper;
using JDVOP.Interface;
using JDVOP.Model;
using JDVOP.Service;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webservice
{
    public partial class ProjectService
    {
        MallInterface service = new JDMallService();
        string projectKey = "jd_project";
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> Login(string loginName, string password)
        {
            if (loginName.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                return Result(false, "数据错误");
            }
            using (var db = new DbRepository())
            {
                if (loginName == "admin" && password == "dfqm2016@123")
                {
                    HttpCookie cookie = new HttpCookie("LoginAdmin");
                    cookie.Value = "admin";
                    cookie.Expires = DateTime.Now.AddDays(40);
                    // 写登录Cookie
                    HttpContext.Current.Response.Cookies.Remove(cookie.Name);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    return Result(true);
                }
                else
                {
                    return Result(false, "账号密码不正确");
                }
            }
        }

        string departmentKey = CacheHelper.RenderKey("11", "Department");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<CategoryInfo> Cache_Get_CategoryInfoList()
        {

            return CacheHelper.Get<List<CategoryInfo>>(departmentKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<CategoryInfo> list = db.CategoryInfo.ToList();
                    return list;
                }
            });
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ResultPageList<ProductInfo> Get_ProjectPageList(int pageIndex, int pageSize, string name, long? sku, string brandName, string firstCategory, string secondCategory, string thirdCategory, ProductStateEnum? state)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.ProductInfo.AsQueryable();
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.name.Contains(name));
                }
                if (brandName.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.brandName.Contains(brandName));
                }
                if (sku != null)
                {
                    query = query.Where(x => x.sku == sku);
                }
                if (firstCategory.IsNotNullOrEmpty() && firstCategory != "-1")
                {
                    var selectCategoryStr = $"{firstCategory};";
                    if (secondCategory.IsNotNullOrEmpty() && secondCategory != "-1")
                    {
                        if (thirdCategory.IsNotNullOrEmpty() && thirdCategory != "-1")
                        {
                            selectCategoryStr = $"{firstCategory};{secondCategory};{thirdCategory}";
                        }
                        else
                        {
                            selectCategoryStr = $"{firstCategory};{secondCategory};";
                        }
                    }
                    query = query.Where(x => x.category.Contains(selectCategoryStr));
                }

                if (state != null && (int)state != -1)
                {
                    query = query.Where(x => x.state == state);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CMCCId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var longList = new List<long>();
                list.Select(x => x.category).Distinct().ToList().ForEach(x =>
                {
                    foreach (var item in x.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (item.IsNotNullOrEmpty())
                        {
                            if (!longList.Contains(item.GetLong()))
                                longList.Add(item.GetLong());
                        }
                    }
                });
                var contentDic = Cache_Get_CategoryInfoList().Where(x => longList.Contains(x.catId)).Distinct().ToDictionary(x => x.catId.ToString());
                var skuNumList = list.Select(x => new SkuNum() { num = 1, skuId = x.sku }).ToList();

                var stockListResult = service.CheckStockForOne(skuNumList, 16, 1303, 48713, 48746);
                var skuIdList = list.Select(x => x.sku).ToList();
                var priceListResult = service.QueryProductPrice(skuIdList);
                //while (!stockListResult.success)
                //{
                //    stockListResult = service.CheckStockForOne(skuNumList, 16, 1303, 48713, 48746);
                //}
                //while (!priceListResult.success)
                //{
                //    priceListResult = service.QueryProductPrice(skuIdList);
                //}

                list.ForEach(x =>
                {
                    var product = service.FindProductDetial(x.sku)?.result;

                    if (product != null && !string.IsNullOrEmpty(product.name))
                    {
                        x.name = product?.name;
                    }
                    var obj = stockListResult.result.Where(y => y.skuId == x.sku).FirstOrDefault();
                    if (obj != null)
                    {
                        if (obj.stockStateld == StockState.HaveAndForDeliver || obj.stockStateld == StockState.HaveAndPayForFromStock || obj.stockStateld == StockState.HaveAndWaitArriveForDeliver || obj.stockStateDesc == "有货")
                        {
                            x.stock = 10;
                            x.state = ProductStateEnum.OnSale;
                        }
                        else if (obj.stockStateDesc == "无货")
                        {
                            x.stock = 0;
                            x.state = ProductStateEnum.OffSale;
                        }
                    }

                    var priceObj = priceListResult.result.Where(y => y.skuId == x.sku).FirstOrDefault();
                    if (priceObj != null)
                    {
                        x.JDprice = priceObj.jdPrice;
                        x.price = priceObj.price;
                    }

                    x.stateStr = x.state.GetDescription();
                    if (x.category.IsNotNullOrEmpty())
                    {
                        var nameList = new List<string>();
                        foreach (var item in x.category.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (item.IsNotNullOrEmpty() && contentDic.ContainsKey(item))
                            {
                                nameList.Add(contentDic[item].name);
                            }
                        }

                        x.categoryStr = string.Join(",", nameList);
                    }
                });
                entities.SaveChanges();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ResultPageList<JDOrderDto> GetOrderPageList(int pageIndex, int pageSize, string cmccOrderId,
            DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.JDOrderInfo.AsQueryable().AsNoTracking();
                if (cmccOrderId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.CMCCOrderId.Contains(cmccOrderId));
                }
                if (createdTimeStart != null)
                {
                    query = query.Where(x => x.CreateTime >= createdTimeStart);
                }
                if (createdTimeEnd != null)
                {
                    createdTimeEnd = createdTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.CreateTime < createdTimeEnd);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CMCCOrderId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    x.StateStr = x.State.GetDescription();
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public string Get_AreaList()
        {

            return new JDMallIBLL().QueryArea().Result;
        }


        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public List<SelectItem> Get_AreaList(string value)
        {
            var areaList = CacheHelper.Get<List<SelectItem>>("jd_area_list", CacheTimeOption.HalfDay, () =>
            {
                using (SqlConnection con = new SqlConnection(connectStr))
                {
                    con.Open();
                    return GetList("select id as Value,name as Text  from  [JD_area]", con);
                }
            });

            var areas = areaList.AsQueryable();
            if (!string.IsNullOrEmpty(value) && !value.Equals("0"))
                areas = areas.Where(x => !string.IsNullOrEmpty(x.ParentKey) && x.ParentKey.Trim().Equals(value));
            else
                areas = areas.Where(_ => string.IsNullOrEmpty(_.ParentKey));

            return areas.ToList();
        }

        /// <summary>
        /// 获取选择项
        /// </summary>
        /// <param name="enteredPointFlag">角色flag值</param>
        /// <returns></returns>
        public List<Tuple<long, string>> Get_CategorySelectItem(long parentId)
        {
            using (var db = new DbRepository())
            {
                var list = new List<CategoryInfo>() { new CategoryInfo { catId = -1, name = "请选择" } };
                if (parentId != 0)
                    list.AddRange(Cache_Get_CategoryInfoList().Where(x => x.parentId.Equals(parentId)).ToList());
                else
                {
                    list.AddRange(Cache_Get_CategoryInfoList().Where(x => x.catClass == CategoryClass.One).ToList());

                }
                return list.Select(x => new Tuple<long, string>(x.catId, x.name)).ToList();

            }
        }

        public WebResult<bool> CreateOrder(EnumShop shop, string thirdOrder, List<SkuNum> nums, int province, int city, int county, InvoiceType invoiceType, int town = 0)
        {

            string e_connectStr = "";

            if (shop == EnumShop.shop_12580)
            {
                e_connectStr = connectStr;
            }
            else if (shop == EnumShop.shop_KMI)
            {
                e_connectStr = KMIconnectStr;
            }

            using (SqlConnection con = new SqlConnection(e_connectStr))
            {
                con.Open();
                var order = GetResult<OrderInfo>($"select  OrderId,OrderStatus,Address,CellPhone,EmailAddress,RegionId,Remark,TelPhone,ShipTo,ZipCode from Hishop_Orders where OrderId='{thirdOrder}'", con);
                if (order == null)
                {
                    return Result(false, "商城订单不存在");
                }
                if (order.OrderStatus != OrderStatus.BuyerAlreadyPaid)
                {
                    return Result(false, "商城订单状态不是待发货状态不存在");
                }

                bool isComplete = false;
                List<int> regionList = new List<int>();

                int regionId = order.RegionId;
                if (order.RegionId > 200000)
                {
                    regionId = order.RegionId - 200000;
                }
                else if (order.RegionId > 100000)
                {
                    regionId = order.RegionId - 100000;
                }



                while (!isComplete)
                {
                    var area = GetResult<JD_area>("select *  from  [JD_area] where id =" + regionId, con);

                    if (area.parent_id == 0)
                    {
                        isComplete = true;
                    }

                    regionList.Insert(0, regionId);
                    regionId = area.parent_id;

                }
                var list = regionList.ToList();

                var result = new JDMallIBLL().CreateOrder(thirdOrder.ToString(), nums, order.ShipTo, order.CellPhone, order.Address, order.TelPhone, order.Remark, list[0], list[1], list[2], invoiceType, (list.Count == 4 ? list[3] : 0));
                if (result.IsSuccess)
                {
                    using (var db = new DbRepository())
                    {
                        //var orderModel = service.QueryJDOrderInfo(thirdOrder).result;
                        result.Result.ID = Guid.NewGuid().ToString("N");
                        result.Result.CMCCOrderId = thirdOrder;
                        result.Result.State = JDOrderState.New;
                        result.Result.CreateTime = DateTime.Now;
                        nums.ForEach(x =>
                        {
                            result.Result.DetailsJsonStr += "商品" + x.name + " 数量：" + x.num + ";\r\n";
                        });
                        db.JDOrderInfo.Add(result.Result);
                        db.SaveChanges();
                    }
                }
                return Result(result.IsSuccess, result.Msg);
            }
        }

        public ResultPageList<T> ResultPageList<T>(List<T> model, int pageIndex, int pageSize, int recoredCount)
        {
            List<string> operateList = new List<string>();
            return ConvertPageList<T>(model, pageIndex, pageSize, recoredCount);
        }

        /// <summary>
        /// list转换pageList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List">需要分页的数据</param>
        /// <returns></returns>
        private ResultPageList<T> ConvertPageList<T>(List<T> list, int pageIndex, int pageSize, int recoredCount)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            return new ResultPageList<T>(list, pageIndex, pageSize, recoredCount);
        }


        string connectStr = "data source=kmi.ppjd.cn;user id=CMCC_12580_sql;password=dfqm@201608sql;database=CMCC_12580";

        string KMIconnectStr = "data source=kmi.ppjd.cn;user id=KMI_sql;password=dfqm@201608sql;database=KMI";
        string jdvopConnectStr = "data source=kmi.ppjd.cn;user id=CMCC_12580_sql;password=dfqm@201608sql;database=jdvop";

        public void UpdateProductStock()
        {
            try
            {

                MallInterface service = new JDMallService();
                StringBuilder sb = new StringBuilder();
                var cmccIdSkuList = new List<KeyValue>();
                using (var db = new DbRepository())
                {
                    #region 刷新12306商品数据库数据

                    using (SqlConnection con = new SqlConnection(connectStr))
                    {
                        con.Open();
                        var cmccIdNumDic = GetListResult("select a.productId as [Key],b.Stock as Num from Hishop_Products a,Hishop_SKUs b where a.SupplierID=1 and a.productId=b.productId and a.ProductId not in (select ProductId from Hishop_SKUs  group by ProductId  having(count(ProductId))>1);", con).ToDictionary(x => x.Key, x => x.Num);
                        cmccIdSkuList = db.ProductInfo.Select(x => new KeyValue() { Key = x.CMCCId, Value = x.sku }).ToList();
                        var index = 0;
                        var ss = 0;
                        List<KeyValue> failedList = new List<KeyValue>();
                        var selectList = cmccIdSkuList.Skip(index).Take(100).ToList();
                        while (selectList != null && selectList.Count != 0)
                        {
                            var skuIdList = selectList.Select(x => x.Value).ToList();
                            var priceListResult = service.QueryProductPrice(skuIdList);
                            if (!priceListResult.success)
                            {
                                failedList.AddRange(selectList);
                                continue;
                            }
                            var stockListResult = service.CheckStockForOne(skuIdList.Select(x => new SkuNum() { num = 1, skuId = x }).ToList(), 16, 1303, 48713, 48746);
                            if (!stockListResult.success)
                            {
                                failedList.AddRange(selectList);
                                continue;
                            }
                            selectList.ForEach(x =>
                            {
                                var stock = 0;
                                decimal jdPrice = 0;
                                decimal price = 0;

                                var stockObj = stockListResult.result.Where(y => y.skuId == x.Value).FirstOrDefault();
                                var priceObj = priceListResult.result.Where(y => y.skuId == x.Value).FirstOrDefault();
                                if (stockObj != null)
                                {

                                    if (stockObj.remainNum > 0)
                                    {
                                        stock = stockObj.remainNum;
                                    }
                                    else
                                    {
                                        if (stockObj.stockStateld == StockState.HaveAndForDeliver || stockObj.stockStateld == StockState.HaveAndPayForFromStock || stockObj.stockStateld == StockState.HaveAndWaitArriveForDeliver || stockObj.stockStateDesc == "有货")
                                            stock = 10;
                                        else if (stockObj.stockStateDesc == "无货")
                                            stock = 0;
                                    }
                                }
                                if (priceObj != null)
                                {
                                    jdPrice = priceObj.jdPrice;
                                    price = priceObj.price;
                                }
                                if (cmccIdNumDic.ContainsKey(x.Key))
                                {
                                    if (cmccIdNumDic[x.Key] != stock)
                                    {
                                        sb.Append(GetUpdateProduct(x.Key, jdPrice, price, stock));
                                    }
                                }
                            });
                            index++;
                            selectList = cmccIdSkuList.Skip(index * 100).Take(100).ToList();
                            if (selectList.Count == 0)
                            {
                                ss++;
                                selectList = failedList.Skip(ss * 100).Take(100).ToList();
                            }
                        }
                        ExecuteNonQuer(sb.ToString(), con);
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateProductStockAndPrice()
        {
            try
            {
                MallInterface service = new JDMallService();
                StringBuilder sb = new StringBuilder();
                using (var db = new DbRepository())
                {
                    #region 刷新12306商品数据库数据

                    using (SqlConnection con = new SqlConnection(jdvopConnectStr))
                    {
                        con.Open();
                        var dic = GetListResult<KeyValue>("select sku as [Value] from ProductInfo where CMCCId!=0;", con);
                        var index = 0;
                        var cmccIdSkuList = dic.Select(x => x.Value).ToList();
                        var selectList = cmccIdSkuList.Skip(index).Take(100).ToList();
                        while (selectList != null && selectList.Count != 0)
                        {
                            var priceListResult = service.QueryProductPrice(selectList);
                            while (!priceListResult.success)
                            {
                                priceListResult = service.QueryProductPrice(selectList);
                            }
                            var stockListResult = service.CheckStockForOne(selectList.Select(x => new SkuNum() { num = 1, skuId = x }).ToList(), 16, 1303, 48713, 48746);
                            while (!stockListResult.success)
                            {
                                stockListResult = service.CheckStockForOne(selectList.Select(x => new SkuNum() { num = 1, skuId = x }).ToList(), 16, 1303, 48713, 48746);
                            }
                            selectList.ForEach(x =>
                            {
                                var stock = 0;
                                decimal price = 0;
                                decimal jdPrice = 0;
                                var stockObj = stockListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                                var priceObj = priceListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                                if (stockObj != null)
                                {

                                    if (stockObj.remainNum > 0)
                                    {
                                        stock = stockObj.remainNum;
                                    }
                                    else
                                    {
                                        if (stockObj.stockStateld == StockState.HaveAndForDeliver || stockObj.stockStateld == StockState.HaveAndPayForFromStock || stockObj.stockStateld == StockState.HaveAndWaitArriveForDeliver || stockObj.stockStateDesc == "有货")
                                            stock = 10;
                                        else if (stockObj.stockStateDesc == "无货")
                                            stock = 0;
                                    }
                                }
                                if (priceObj != null)
                                {
                                    price = priceObj.price;
                                    jdPrice = priceObj.jdPrice;
                                }
                                sb.AppendFormat("update ProductInfo set price={0},JDprice={1},stock={2},state={3} where sku={4};\r\n", price, jdPrice,stock, stock==10?1:0, x);

                            });
                            index++;
                            selectList = cmccIdSkuList.Skip(index * 100).Take(100).ToList();
                        }
                        ExecuteNonQuer(sb.ToString(), con);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SyncProduct()
        {
            MallInterface service = new JDMallService();


            using (var db = new DbRepository())
            {
                var dicProductDic = new Dictionary<long, ProductInfo>();

                List<ProductInfo> productList = new List<ProductInfo>();
                //获取所有的商品编号
                var skuHas = new HashSet<long>(db.ProductInfo.Select(x => x.sku).ToList());
                //获取商品池
                var poolList = service.QueryPageNum()?.result;
                if (poolList != null)
                {
                    poolList.ForEach(pool =>
                    {
                        //根据商品池编号获取商品编号
                        var skuArray = service.QueryProductSkuByPageNum(pool.page_num)?.result?.Split(',');
                        if (skuArray != null)
                        {
                            foreach (var skuStr in skuArray)
                            {
                                var sku = Convert.ToInt64(skuStr);
                                //判断商品是否存在如果不存在则新建入库
                                if (!skuHas.Contains(sku))
                                {
                                    var project = service.FindProductDetial(sku)?.result;
                                    if (project != null)
                                    {
                                        dicProductDic.Add(sku, project);
                                    }
                                }
                            }
                        }
                    });

                    var index = 0;

                    var skuIdList = dicProductDic.Select(x => x.Key).ToList();

                    var selectList = skuIdList.Skip(index).Take(100).Select(x => x.GetLong()).ToList();

                    while (selectList != null && selectList.Count != 0)
                    {
                        var priceListResult = service.QueryProductPrice(selectList);
                        while (!priceListResult.success)
                        {
                            priceListResult = service.QueryProductPrice(selectList);
                        }
                        selectList.ForEach(x =>
                        {
                            var obj = priceListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                            if (obj != null)
                            {
                                dicProductDic[x].price = obj.price;
                                dicProductDic[x].JDprice = obj.jdPrice;
                            }
                        });
                        var stockListResult = service.CheckStockForOne(selectList.Select(x => new SkuNum() { num = 1, skuId = x }).ToList(), 16, 1303, 48713, 48746);
                        while (!priceListResult.success)
                        {
                            stockListResult = service.CheckStockForOne(selectList.Select(x => new SkuNum() { num = 1, skuId = x }).ToList(), 16, 1303, 48713, 48746);
                        }

                        var imgListResult = service.QueryProductImage(selectList);
                        while (!imgListResult.success)
                        {
                            imgListResult = service.QueryProductImage(selectList);
                        }
                        selectList.ForEach(x =>
                        {
                            var product = dicProductDic[x];
                            var obj = stockListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                            if (obj != null)
                            {
                                if (obj.stockStateld == StockState.HaveAndForDeliver || obj.stockStateld == StockState.HaveAndPayForFromStock || obj.stockStateld == StockState.HaveAndWaitArriveForDeliver || obj.stockStateDesc == "有货")
                                {
                                    product.stock = 10;
                                }
                                else if (obj.stockStateDesc == "无货")
                                {
                                    product.stock = 0;
                                }
                            }
                            var priceObj = priceListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                            if (priceObj != null)
                            {
                                product.JDprice = priceObj.jdPrice;
                                product.price = priceObj.price;
                            }
                            var imgObj = imgListResult.result[x];
                            if (imgObj != null)
                            {
                                product.ThumbnailUrl40 = $"http://img13.360buyimg.com/n5/{product.imagePath}";
                                product.ThumbnailUrl60 = $"http://img13.360buyimg.com/n5/{product.imagePath}";
                                product.ThumbnailUrl100 = $"http://img13.360buyimg.com/n4/{product.imagePath}";
                                product.ThumbnailUrl160 = $"http://img13.360buyimg.com/n2/{product.imagePath}";
                                product.ThumbnailUrl180 = $"http://img13.360buyimg.com/n2/{product.imagePath}";
                                product.ThumbnailUrl220 = $"http://img13.360buyimg.com/n6/{product.imagePath}";
                                product.ThumbnailUrl310 = $"http://img13.360buyimg.com/n1/{product.imagePath}";
                                product.ThumbnailUrl410 = $"http://img13.360buyimg.com/n1/{product.imagePath}";

                                if (imgObj.Count == 1)
                                {
                                    product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                }
                                else if (imgObj.Count == 2)
                                {
                                    product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                    product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                }
                                else if (imgObj.Count == 3)
                                {
                                    product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                    product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                    product.ImageUrl3 = $"http://img13.360buyimg.com/n1/{imgObj[2].path}";
                                }
                                else if (imgObj.Count == 4)
                                {
                                    product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                    product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                    product.ImageUrl3 = $"http://img13.360buyimg.com/n1/{imgObj[2].path}";
                                    product.ImageUrl4 = $"http://img13.360buyimg.com/n1/{imgObj[3].path}";
                                }
                                else if (imgObj.Count >= 5)
                                {
                                    product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                    product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                    product.ImageUrl3 = $"http://img13.360buyimg.com/n1/{imgObj[2].path}";
                                    product.ImageUrl4 = $"http://img13.360buyimg.com/n1/{imgObj[3].path}";
                                    product.ImageUrl5 = $"http://img13.360buyimg.com/n1/{imgObj[4].path}";
                                }
                            }
                            if (product.brandName.IsNotNullOrEmpty())
                            {
                                product.ID = Guid.NewGuid().ToString("N");
                                product.CreateDate = DateTime.Now;
                                productList.Add(product);


                            }
                        });
                        index++;
                        selectList = skuIdList.Skip(index).Take(100).ToList();
                    }

                }

                db.ProductInfo.AddRange(productList);

                var mmm = db.SaveChanges();

                var z = mmm;
            }
        }


        public void GetProductFromPool()
        {
            try
            {

                MallInterface service = new JDMallService();
                StringBuilder sb = new StringBuilder();

                var dicProductDic = new Dictionary<long, ProductInfo>();
                var category_dic = new Dictionary<long, CategoryInfo>();
                using (var db = new DbRepository())
                {
                    #region 刷新12580商品数据库数据

                    using (SqlConnection con = new SqlConnection(connectStr))
                    {
                        con.Open();
                        var exitSkuIdList = db.ProductInfo.Select(x => x.sku.ToString()).ToList();
                        category_dic = db.CategoryInfo.ToDictionary(x => x.catId);
                        var index = 0;
                        var idList = new List<string>();
                        service.QueryPageNum().result.ForEach(x =>
                        {
                            idList.AddRange(service.QueryProductSkuByPageNum(x.page_num).result.Split(','));
                        });

                        var addSkuIdList = idList.Where(x => !exitSkuIdList.Contains(x)).ToList();
                        addSkuIdList.ForEach(x =>
                        {
                            var project = service.FindProductDetial(x.GetLong());
                            while (!project.success)
                            {
                                project = service.FindProductDetial(x.GetLong());
                            }
                            if (project.success && project.result != null)
                            {
                                project.result.ID = Guid.NewGuid().ToString("N");
                                dicProductDic.Add(x.GetLong(), project.result);
                            }
                        });
                        var skuIdList = dicProductDic.Select(x => x.Key).ToList();
                        var selectList = skuIdList.Skip(index).Take(100).Select(x => x.GetLong()).ToList();
                        while (selectList != null && selectList.Count != 0)
                        {
                            var priceListResult = service.QueryProductPrice(selectList);
                            while (!priceListResult.success)
                            {
                                priceListResult = service.QueryProductPrice(selectList);
                            }
                            selectList.ForEach(x =>
                            {
                                var obj = priceListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                                if (obj != null)
                                {
                                    dicProductDic[x].price = obj.price;
                                    dicProductDic[x].JDprice = obj.jdPrice;
                                }
                            });
                            var stockListResult = service.CheckStockForOne(selectList.Select(x => new SkuNum() { num = 1, skuId = x }).ToList(), 16, 1303, 48713, 48746);
                            while (!priceListResult.success)
                            {
                                stockListResult = service.CheckStockForOne(selectList.Select(x => new SkuNum() { num = 1, skuId = x }).ToList(), 16, 1303, 48713, 48746);
                            }

                            var imgListResult = service.QueryProductImage(selectList);
                            while (!imgListResult.success)
                            {
                                imgListResult = service.QueryProductImage(selectList);
                            }
                            selectList.ForEach(x =>
                            {
                                var product = dicProductDic[x];
                                var obj = stockListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                                if (obj != null)
                                {
                                    if (obj.stockStateld == StockState.HaveAndForDeliver || obj.stockStateld == StockState.HaveAndPayForFromStock || obj.stockStateld == StockState.HaveAndWaitArriveForDeliver || obj.stockStateDesc == "有货")
                                    {
                                        product.stock = 10;
                                    }
                                    else if (obj.stockStateDesc == "无货")
                                    {
                                        product.stock = 0;
                                    }
                                }
                                var priceObj = priceListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                                if (priceObj != null)
                                {
                                    product.JDprice = priceObj.jdPrice;
                                    product.price = priceObj.price;
                                }
                                var imgObj = imgListResult.result[x];
                                if (imgObj != null)
                                {
                                    product.ThumbnailUrl40 = $"http://img13.360buyimg.com/n5/{product.imagePath}";
                                    product.ThumbnailUrl60 = $"http://img13.360buyimg.com/n5/{product.imagePath}";
                                    product.ThumbnailUrl100 = $"http://img13.360buyimg.com/n4/{product.imagePath}";
                                    product.ThumbnailUrl160 = $"http://img13.360buyimg.com/n2/{product.imagePath}";
                                    product.ThumbnailUrl180 = $"http://img13.360buyimg.com/n2/{product.imagePath}";
                                    product.ThumbnailUrl220 = $"http://img13.360buyimg.com/n6/{product.imagePath}";
                                    product.ThumbnailUrl310 = $"http://img13.360buyimg.com/n1/{product.imagePath}";
                                    product.ThumbnailUrl410 = $"http://img13.360buyimg.com/n1/{product.imagePath}";

                                    if (imgObj.Count == 1)
                                    {
                                        product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                    }
                                    else if (imgObj.Count == 2)
                                    {
                                        product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                        product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                    }
                                    else if (imgObj.Count == 3)
                                    {
                                        product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                        product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                        product.ImageUrl3 = $"http://img13.360buyimg.com/n1/{imgObj[2].path}";
                                    }
                                    else if (imgObj.Count == 4)
                                    {
                                        product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                        product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                        product.ImageUrl3 = $"http://img13.360buyimg.com/n1/{imgObj[2].path}";
                                        product.ImageUrl4 = $"http://img13.360buyimg.com/n1/{imgObj[3].path}";
                                    }
                                    else if (imgObj.Count >= 5)
                                    {
                                        product.ImageUrl1 = $"http://img13.360buyimg.com/n1/{imgObj[0].path}";
                                        product.ImageUrl2 = $"http://img13.360buyimg.com/n1/{imgObj[1].path}";
                                        product.ImageUrl3 = $"http://img13.360buyimg.com/n1/{imgObj[2].path}";
                                        product.ImageUrl4 = $"http://img13.360buyimg.com/n1/{imgObj[3].path}";
                                        product.ImageUrl5 = $"http://img13.360buyimg.com/n1/{imgObj[4].path}";
                                    }
                                }
                                if (product.brandName.IsNotNullOrEmpty())
                                {
                                    SqlCommandAppend(new Hishop_Products()
                                    {
                                        ProductCode = product.sku.ToString(),
                                        Description = product.introduction,
                                        CategoryName = GetCategoryName(category_dic, product.category),
                                        AddedDate = DateTime.Now,
                                        ImageUrl1 = product.ImageUrl1,
                                        ImageUrl2 = product.ImageUrl2,
                                        ImageUrl3 = product.ImageUrl3,
                                        ImageUrl4 = product.ImageUrl4,
                                        ImageUrl5 = product.ImageUrl5,
                                        MarketPrice = product.price,
                                        MaxShowPrice = product.price,
                                        MinShowPrice = product.price,
                                        ProductName = product.name,
                                        ThumbnailUrl40 = product.ThumbnailUrl40,
                                        ThumbnailUrl60 = product.ThumbnailUrl60,
                                        ThumbnailUrl100 = product.ThumbnailUrl100,
                                        ThumbnailUrl160 = product.ThumbnailUrl160,
                                        ThumbnailUrl180 = product.ThumbnailUrl180,
                                        ThumbnailUrl220 = product.ThumbnailUrl220,
                                        ThumbnailUrl310 = product.ThumbnailUrl310,
                                        ThumbnailUrl410 = product.ThumbnailUrl410,
                                        BrandName = product.brandName,
                                        Stock = product.stock,
                                    }, con);
                                    product.ID = Guid.NewGuid().ToString("N");
                                    db.ProductInfo.Add(product);
                                }
                            });
                            index++;
                            db.SaveChanges();
                            selectList = skuIdList.Skip(index).Take(100).ToList();
                        }
                        //ExecuteNonQuer(sb.ToString(), con);
                        db.SaveChanges();
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SqlCommandAppend(Hishop_Products model, SqlConnection con)
        {
            SqlParameter[] parameters = new SqlParameter[10];
            //分类
            var categryList = model.CategoryName.Split(';');
            var firstCategory = GetCategory(categryList[0], 1, 0, con);
            //第一级判断是否存在
            if (firstCategory != null)
            {
                //第二级
                var secondCategory = GetCategory(categryList[1], 2, firstCategory.Key, con);
                if (secondCategory != null)
                {
                    if (categryList.Length == 3)
                    {
                        var thirdCategoryResult = GetCategory(categryList[2], 3, secondCategory.Key, con);
                        if (thirdCategoryResult != null)
                        {
                            model.CategoryId = thirdCategoryResult.Key;
                        }
                        else
                        {
                            var categorySql = $"DECLARE @DisplaySequence AS INT SELECT @DisplaySequence = (CASE WHEN MAX(DisplaySequence) IS NULL THEN 1 ELSE MAX(DisplaySequence) + 1 END) FROM Hishop_Categories where ParentCategoryId={secondCategory.Key};DECLARE @CategoryId AS INT Select @CategoryId = ISNULL(Max(CategoryId),0) From Hishop_Categories IF @CategoryId Is Not Null Set @CategoryId = @CategoryId+1 Else Set @CategoryId = 1;INSERT INTO Hishop_Categories(CategoryId,Name,DisplaySequence,ParentCategoryId,Depth,HasChildren,FirstCommission,SecondCommission,ThirdCommission,Path) VALUES(@CategoryId,'{categryList[2]}', @DisplaySequence,{secondCategory.Key},3,0,'0','0','0','{firstCategory.Key}|{secondCategory.Key}|'+convert(varchar(5),@CategoryId)); select @CategoryId;";
                            model.CategoryId = ExecuteScalar(categorySql, con);
                        }
                    }
                    else
                    {
                    }

                }
                else
                {
                    var categorySql = $"DECLARE @DisplaySequence AS INT SELECT @DisplaySequence = (CASE WHEN MAX(DisplaySequence) IS NULL THEN 1 ELSE MAX(DisplaySequence) + 1 END) FROM Hishop_Categories where ParentCategoryId={firstCategory.Key};DECLARE @CategoryId AS INT Select @CategoryId = ISNULL(Max(CategoryId),0) From Hishop_Categories IF @CategoryId Is Not Null Set @CategoryId = @CategoryId+1 Else Set @CategoryId = 1;INSERT INTO Hishop_Categories(CategoryId,Name,DisplaySequence,ParentCategoryId,Depth,HasChildren,FirstCommission,SecondCommission,ThirdCommission,Path) VALUES(@CategoryId,'{categryList[1]}', @DisplaySequence,{firstCategory.Key},2,1,'0','0','0','{firstCategory.Key}|'+convert(varchar(5),@CategoryId)); select @CategoryId;";
                    var secondCateId = ExecuteScalar(categorySql, con);
                    categorySql = $"DECLARE @DisplaySequence AS INT SELECT @DisplaySequence = (CASE WHEN MAX(DisplaySequence) IS NULL THEN 1 ELSE MAX(DisplaySequence) + 1 END) FROM Hishop_Categories where ParentCategoryId={secondCateId};DECLARE @CategoryId AS INT Select @CategoryId = ISNULL(Max(CategoryId),0) From Hishop_Categories IF @CategoryId Is Not Null Set @CategoryId = @CategoryId+1 Else Set @CategoryId = 1;INSERT INTO Hishop_Categories(CategoryId,Name,DisplaySequence,ParentCategoryId,Depth,HasChildren,FirstCommission,SecondCommission,ThirdCommission,Path) VALUES(@CategoryId,'{categryList[2]}', @DisplaySequence,{secondCateId},3,0,'0','0','0','{firstCategory.Key}|{secondCateId}|'+convert(varchar(5),@CategoryId)); select @CategoryId;";
                    model.CategoryId = ExecuteScalar(categorySql, con);
                }
            }
            else
            {
                //新增分类
                string categorySql = $"DECLARE @DisplaySequence AS INT SELECT @DisplaySequence = (CASE WHEN MAX(DisplaySequence) IS NULL THEN 1 ELSE MAX(DisplaySequence) + 1 END) FROM Hishop_Categories;DECLARE @CategoryId AS INT Select @CategoryId = ISNULL(Max(CategoryId),0) From Hishop_Categories IF @CategoryId Is Not Null Set @CategoryId = @CategoryId+1 Else Set @CategoryId = 1;INSERT INTO Hishop_Categories(CategoryId,Name,DisplaySequence,Depth,HasChildren,FirstCommission,SecondCommission,ThirdCommission,Path) VALUES(@CategoryId,'{categryList[0]}', @DisplaySequence,1,1,'0','0','0',''+convert(varchar(5),@CategoryId)); SELECT @@IDENTITY";
                var cateId = ExecuteScalar(categorySql, con);
                categorySql = $"DECLARE @DisplaySequence AS INT SELECT @DisplaySequence = (CASE WHEN MAX(DisplaySequence) IS NULL THEN 1 ELSE MAX(DisplaySequence) + 1 END) FROM Hishop_Categories where ParentCategoryId={cateId};DECLARE @CategoryId AS INT Select @CategoryId = ISNULL(Max(CategoryId),0) From Hishop_Categories IF @CategoryId Is Not Null Set @CategoryId = @CategoryId+1 Else Set @CategoryId = 1;INSERT INTO Hishop_Categories(CategoryId,Name,DisplaySequence,ParentCategoryId,Depth,HasChildren,FirstCommission,SecondCommission,ThirdCommission,Path) VALUES(@CategoryId,'{categryList[1]}', @DisplaySequence,{cateId},2,1,'0','0','0','{cateId}|'+convert(varchar(5),@CategoryId)); select @CategoryId;";
                var secondCateId = ExecuteScalar(categorySql, con);
                categorySql = $"DECLARE @DisplaySequence AS INT SELECT @DisplaySequence = (CASE WHEN MAX(DisplaySequence) IS NULL THEN 1 ELSE MAX(DisplaySequence) + 1 END) FROM Hishop_Categories where ParentCategoryId={secondCateId};DECLARE @CategoryId AS INT Select @CategoryId = ISNULL(Max(CategoryId),0) From Hishop_Categories IF @CategoryId Is Not Null Set @CategoryId = @CategoryId+1 Else Set @CategoryId = 1;INSERT INTO Hishop_Categories(CategoryId,Name,DisplaySequence,ParentCategoryId,Depth,HasChildren,FirstCommission,SecondCommission,ThirdCommission,Path) VALUES(@CategoryId,'{categryList[2]}', @DisplaySequence,{secondCateId},3,0,'0','0','0','{cateId}|{secondCateId}|'+convert(varchar(5),@CategoryId)); select @CategoryId;";
                model.CategoryId = ExecuteScalar(categorySql, con);
            }
            parameters = new SqlParameter[]{
                                           new SqlParameter("@brandname",model.BrandName)
                                      };
            model.BrandId = ExecuteScalar("select brandid from Hishop_BrandCategories where brandname=@brandname;", con, parameters);
            if (model.BrandId == 0)
            {
                parameters = new SqlParameter[]{
                                           new SqlParameter("@brandname",model.BrandName)
                                      };
                string brandSql = $"DECLARE @DisplaySequence AS INT SELECT @DisplaySequence = (CASE WHEN MAX(DisplaySequence) IS NULL THEN 1 ELSE MAX(DisplaySequence) + 1 END) FROM Hishop_BrandCategories;INSERT INTO Hishop_BrandCategories(BrandName,MetaKeywords,MetaDescription,DisplaySequence) VALUES(@brandname,@brandname,@brandname, @DisplaySequence); SELECT @@IDENTITY";
                model.BrandId = ExecuteScalar(brandSql, con, parameters);
            }

            parameters = new SqlParameter[]{
                                           new SqlParameter("@CategoryId",model.CategoryId),
                                           new SqlParameter("@ProductName",model.ProductName),
                                           new SqlParameter("@ProductCode",model.ProductCode),
                                           new SqlParameter("@Description",model.Description),
                                           new SqlParameter("@SaleStatus",model.SaleStatus),
                                           new SqlParameter("@AddedDate",model.AddedDate),
                                           new SqlParameter("@DisplaySequence",model.DisplaySequence),
                                           new SqlParameter("@ImageUrl1",model.ImageUrl1),
                                           new SqlParameter("@ImageUrl2",model.ImageUrl2),
                                           new SqlParameter("@ImageUrl3",model.ImageUrl3),
                                           new SqlParameter("@ImageUrl4",model.ImageUrl4),
                                           new SqlParameter("@ImageUrl5",model.ImageUrl5),
                                           new SqlParameter("@ThumbnailUrl40",model.ThumbnailUrl40),
                                           new SqlParameter("@ThumbnailUrl60",model.ThumbnailUrl60),
                                           new SqlParameter("@ThumbnailUrl100",model.ThumbnailUrl100),
                                           new SqlParameter("@ThumbnailUrl160",model.ThumbnailUrl160),
                                           new SqlParameter("@ThumbnailUrl180",model.ThumbnailUrl180),
                                           new SqlParameter("@ThumbnailUrl220",model.ThumbnailUrl220),
                                           new SqlParameter("@ThumbnailUrl310",model.ThumbnailUrl310),
                                           new SqlParameter("@ThumbnailUrl410",model.ThumbnailUrl410),
                                           new SqlParameter("@MarketPrice",model.MarketPrice),
                                           new SqlParameter("@BrandId",model.BrandId),
                                           new SqlParameter("@HasSKU",model.HasSKU),
                                           new SqlParameter("@IsfreeShipping",model.IsfreeShipping),
                                           new SqlParameter("@MinShowPrice",model.MinShowPrice),
                                           new SqlParameter("@MaxShowPrice",model.MaxShowPrice),
                                           new SqlParameter("@FreightTemplateId",model.FreightTemplateId),
                                           new SqlParameter("@FirstCommission",model.FirstCommission),
                                           new SqlParameter("@SecondCommission",model.SecondCommission),
                                           new SqlParameter("@ThirdCommission",model.ThirdCommission),
                                           new SqlParameter("@IsSetCommission",model.IsSetCommission),
                                           new SqlParameter("@CubicMeter",model.CubicMeter),
                                           new SqlParameter("@FreightWeight",model.FreightWeight),
                                           new SqlParameter("@ShowSaleCounts",model.ShowSaleCounts),
                                           new SqlParameter("@SupplierID",model.SupplierID)
                                      };
            string sqlComand = "INSERT INTO Hishop_Products" +
                         "(CategoryId, ProductName, ProductCode,[Description], SaleStatus, AddedDate, DisplaySequence," +
 "ImageUrl1, ImageUrl2, ImageUrl3, ImageUrl4, ImageUrl5, ThumbnailUrl40, ThumbnailUrl60, ThumbnailUrl100, ThumbnailUrl160, ThumbnailUrl180," +
 "ThumbnailUrl220, ThumbnailUrl310, ThumbnailUrl410," +
 "MarketPrice, BrandId, HasSKU, IsfreeShipping, MinShowPrice, MaxShowPrice,FreightTemplateId, FirstCommission, SecondCommission, ThirdCommission, IsSetCommission, CubicMeter, FreightWeight, ShowSaleCounts, SupplierID)" +
 "Values(" +
 $"@CategoryId, @ProductName, @ProductCode, @Description, @SaleStatus, @AddedDate, @DisplaySequence," +
 $"@ImageUrl1, @ImageUrl2, @ImageUrl3, @ImageUrl4, @ImageUrl5, @ThumbnailUrl40, @ThumbnailUrl60, @ThumbnailUrl100, @ThumbnailUrl160, @ThumbnailUrl180," +
 $"@ThumbnailUrl220, @ThumbnailUrl310, @ThumbnailUrl410," +
 $"@MarketPrice, @BrandId, @HasSKU, @IsfreeShipping, @MinShowPrice, @MaxShowPrice,@FreightTemplateId, @FirstCommission, @SecondCommission, @ThirdCommission, @IsSetCommission, @CubicMeter, @FreightWeight, @ShowSaleCounts, @SupplierID); select @@IDENTITY;";

            var productId = ExecuteScalar(sqlComand, con, parameters);

            string skuSqlCommand = $"INSERT INTO Hishop_SKUs(SkuId, ProductId, Stock, SalePrice) VALUES('{productId}_0', {productId}, {model.Stock},{model.MarketPrice})";
            ExecuteNonQuer(skuSqlCommand, con);
        }


        public KeyValue GetCategory(string name, int depth, int categoryId, SqlConnection con)
        {
            string categorySql = string.Empty;
            if (depth == 1)
            {
                categorySql = $"SELECT CategoryId as [Key], [Name] as [Value] FROM [CMCC_12580].[dbo].[Hishop_Categories]   where name = '{name}' and depth=1;";
            }
            else
            {
                categorySql = $"SELECT CategoryId as [Key], [Name] as [Value] FROM [CMCC_12580].[dbo].[Hishop_Categories] where depth={depth} and ParentCategoryId={categoryId} and name='{name}';";
            }
            return GetResult<KeyValue>(categorySql, con);
        }
        public string GetCategoryName(Dictionary<long, CategoryInfo> category_dic, string categoryId)
        {
            if (categoryId.IsNullOrEmpty())
                return "";
            List<string> nameList = new List<string>();
            categoryId.Split(';').ToList().ForEach(x =>
            {
                if (category_dic.ContainsKey(x.GetLong()))
                {
                    nameList.Add(category_dic[x.GetLong()].name);
                }
            });
            return string.Join(";", nameList);
        }
        public void UpdateProductCommentCount()
        {
            StringBuilder sb = new StringBuilder();
            //var cmccIdSkuList = new List<KeyValue>();
            using (var db = new DbRepository())
            {
                #region 刷新12306商品数据库数据

                using (SqlConnection con = new SqlConnection(connectStr))
                {
                    con.Open();
                    //var cmccIdNumDic = GetListResult("select a.productId as [Key],b.Stock as Num from Hishop_Products a,Hishop_SKUs b where a.SupplierID=1 and a.productId=b.productId;", con).ToDictionary(x => x.Key, x => x.Num);
                    var cmccIdSkuList = db.ProductInfo.ToList();
                    foreach (var item in cmccIdSkuList)
                    {
                        var result = string.Empty;
                        while (result.IsNullOrEmpty())
                        {
                            result = WebHelper.GetPage($"http://club.jd.com/ProductPageService.aspx?method=GetCommentSummaryBySkuId&referenceId={item.sku}&type=1");
                            if (result.Contains("302 Found"))
                                continue;
                            if (result.Contains("无法连接到远程服务器"))
                                result = string.Empty;
                        }
                        if (!result.Contains("302 Found"))
                        {
                            var obj = result.DeserializeJson<Comment>();
                            if (obj != null)
                            {
                                sb.Append($"update  ProductInfo set  CommentCount={obj.CommentCount} where ID='{item.ID}';\r\n");
                            }
                        }
                    }
                    ExecuteNonQuer(sb.ToString(), con);
                }

                #endregion
            }
        }

        public List<KeyValue> GetListResult(string sqlcomand, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, con);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            adapter.Fill(ds);//填充数据

            if (ds.Tables[0] != null)
                return ModelConvertHelper<KeyValue>.ToModels(ds.Tables[0]) as List<KeyValue>;
            else
                return null;
        }

        public List<T> GetListResult<T>(string sqlcomand, SqlConnection con) where T : new()
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, con);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            adapter.Fill(ds);//填充数据

            if (ds.Tables[0] != null)
                return ModelConvertHelper<T>.ToModels(ds.Tables[0]) as List<T>;
            else
                return null;
        }

        public List<SelectItem> GetList(string sqlcomand, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, con);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            adapter.Fill(ds);//填充数据

            if (ds.Tables[0] != null)
                return ModelConvertHelper<SelectItem>.ToModels(ds.Tables[0]) as List<SelectItem>;
            else
                return null;
        }


        public T GetResult<T>(string sqlcomand, SqlConnection conn) where T : new()
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            adapter.Fill(ds);//填充数据


            if (ds.Tables[0].Rows.Count > 0)
                return ModelConvertHelper<T>.ToModel(ds.Tables[0].Rows[0]);
            else
                return default(T);
        }

        public string GetUpdateProduct(int productId, int stock)
        {
            if (productId != 0)
            {
                return $"update  Hishop_Products set  SaleStatus={ (stock > 0 ? 1 : 2)} where ProductId={productId};" +
               $"update  Hishop_SKUs set  Stock={stock} where SkuId='{productId}_0';\r\n";
            }
            else
                return "";
        }
        public string GetUpdateProduct(int productId, decimal jdPrice, decimal price, int stock)
        {
            if (productId != 0)
            {
                return $"update  Hishop_Products set MarketPrice={jdPrice},MinShowPrice={jdPrice},MaxShowPrice={jdPrice},SaleStatus={ (stock > 0 ? 1 : 2)} where ProductId={productId} and MinShowPrice>{jdPrice};" +
                    $"update  Hishop_SKUs set SalePrice={jdPrice},BasicPrice={price},Stock={stock} where SkuId='{productId}_0' and SalePrice>{jdPrice};";
            }
            else
                return "";
        }
        public int ExecuteNonQuer(string sqlcomand, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, conn);
            return cmd.ExecuteNonQuery();
        }
        public int ExecuteScalar(string sqlcomand, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, conn);
            return cmd.ExecuteScalar().GetInt();
        }
        public int ExecuteScalar(string sqlcomand, SqlConnection conn, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, conn);
            PrepareCommand(cmd, conn, null, sqlcomand, cmdParms);
            object obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj.GetInt();
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        public WebResult<T> Result<T>(T model)
        {
            return new WebResult<T> { Result = model };
        }
        public WebResult<T> Result<T>(T model, string msg)
        {
            return new WebResult<T> { Result = model, Msg = msg };
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ResultPageList<SMSBatch> GetSMSPageList(int pageIndex, int pageSize, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.SMSBatch.AsQueryable();
                if (createdTimeStart != null)
                {
                    query = query.Where(x => x.CreatedTime >= createdTimeStart);
                }
                if (createdTimeEnd != null)
                {
                    createdTimeEnd = createdTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.CreatedTime < createdTimeEnd);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    using (SqlConnection coon = new SqlConnection(connectStr))
                    {
                        if (x.SuccessCount == 0)
                        {
                            x.SuccessCount = GetSMSResult(x.BatchNum, coon);
                        }
                    }
                });
                db.SaveChanges();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        public int GetSMSResult(string batchNum, SqlConnection coon)
        {
            coon.Open();
            var sql = $"SELECT COUNT(1) FROM [CMCC_12580].[dbo].[CMCC_SMS_do] where state = 1 and batchNum = '{batchNum}' ; ";
            return ExecuteScalar(sql, coon);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_SMSBatch(SMSBatch model)
        {
            using (DbRepository db = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.BatchNum = DateTime.Now.ToString("yyyyMMddhhmmss");
                using (SqlConnection coon = new SqlConnection(connectStr))
                {
                    coon.Open();

                    //非12580会员
                    //                    var sql = $" SELECT TOP  {model.SendCount} memberid,mobile  ";
                    //                    sql += @"FROM(
                    //        SELECT ROW_NUMBER() OVER(ORDER BY memberid) AS RowNumber,memberid ,MoBileId as mobile FROM
                    //          CMCC_Member_phone where MoBileId not in (select Mobile from[aspnet_12580Members]) and MoBileId not in (select Mobile from aspnet_free12580Member)
                    //         )   as a
                    //WHERE RowNumber >  " + model.SkipNum;

                    //12580会员
                    var sql = $" SELECT TOP  {model.SendCount} id,mobile  ";
                    sql += @"FROM(
        SELECT ROW_NUMBER() OVER(ORDER BY id) AS RowNumber,id ,mobile FROM
          aspnet_All12580Member
         )   as a
WHERE RowNumber >  " + model.SkipNum;

                    var idList = GetListResult<MemberPhone>(sql, coon);

                    if (idList.Count > 0)
                    {
                        model.StartUserID = idList[0].memberId;
                        model.EndUserID = idList[idList.Count - 1].memberId;
                    }
                    else
                    {
                        model.StartUserID = 0;
                        model.EndUserID = 0;
                    }



                    var sequenceId = ExecuteScalar("select top 1 sequenceId FROM [CMCC_12580].[dbo].[CMCC_SMS_do] order by id desc", coon);
                    if (model.AppendMibole.IsNotNullOrEmpty())
                    {
                        var sb = new StringBuilder();
                        model.AppendMibole.Split(',').ToList().ForEach(x =>
                        {
                            sequenceId++;

                            sb.Append($"INSERT INTO [CMCC_12580].[dbo].[CMCC_SMS_do]([sequenceId],[mobile],[message],[addtime],[state],[batchNum])VALUES({sequenceId},'{x}','{model.Content}','{DateTime.Now}',0,'{model.BatchNum}');\r\n");

                        });

                        if (sb.Length > 0)
                        {
                            ExecuteNonQuer(sb.ToString(), coon);
                        }
                    }

                    if (idList != null && idList.Count > 0)
                    {
                        var sb = new StringBuilder();

                        idList.ForEach(x =>
                        {
                            sequenceId++;
                            sb.Append($"INSERT INTO [CMCC_12580].[dbo].[CMCC_SMS_do]([sequenceId],[mobile],[message],[addtime],[state],[batchNum])VALUES({sequenceId},'{x.mobile}','{model.Content}','{DateTime.Now}',0,'{model.BatchNum}');\r\n");

                            if (sequenceId % 1000 == 0)
                            {
                                ExecuteNonQuer(sb.ToString(), coon);
                                sb = new StringBuilder();
                            }
                        });

                        if (sb.Length > 0)
                        {
                            ExecuteNonQuer(sb.ToString(), coon);
                        }
                    }



                }

                db.SMSBatch.Add(model);

                if (db.SaveChanges() > 0)
                {
                    return Result(true);
                }
                else
                {
                    return Result(false);
                }
            }

        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SMSBatch Find_SMSBatch(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository db = new DbRepository())
            {
                var model = db.SMSBatch.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
                return model;
            }
        }
    }

    /// <summary>
    /// 返回结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebResult<T>
    {

        /// <summary>
        /// 返回结果
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// 附加消息
        /// </summary>
        public string Msg { get; set; }


        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get { return Msg.IsNullOrEmpty(); }
        }
    }


    /// <summary>
    /// 选择项
    /// </summary>
    public class SelectItem
    {

        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected { get; set; } = false;

        public string ParentKey { get; set; }


    }

    public class JD_area
    {
        public int id { get; set; }

        public string name { get; set; }
        public int parent_id { get; set; }
    }


    public class Hishop_Products
    {
        public int Stock { get; set; }
        public int ProductId { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string Description { get; set; }
        /// <summary>
        ///  3仓库中 2售罄 1在售
        /// </summary>
        public int SaleStatus { get; set; } = 3;

        public DateTime AddedDate { get; set; }
        public int VistiCounts { get; set; } = 0;
        public int SaleCounts { get; set; } = 0;
        public int ShowSaleCounts { get; set; } = 0;

        public int DisplaySequence { get; set; }

        public string ImageUrl1 { get; set; }
        public string ImageUrl2 { get; set; }
        public string ImageUrl3 { get; set; }
        public string ImageUrl4 { get; set; }
        public string ImageUrl5 { get; set; }
        public string ThumbnailUrl40 { get; set; }
        public string ThumbnailUrl60 { get; set; }
        public string ThumbnailUrl100 { get; set; }
        public string ThumbnailUrl160 { get; set; }
        public string ThumbnailUrl180 { get; set; }

        public string ThumbnailUrl220 { get; set; }
        public string ThumbnailUrl310 { get; set; }
        public string ThumbnailUrl410 { get; set; }

        public decimal MarketPrice { get; set; }


        public int BrandId { get; set; }


        public int HasSKU { get; set; } = 0;
        public int IsfreeShipping { get; set; } = 0;


        public decimal MinShowPrice { get; set; }


        public decimal MaxShowPrice { get; set; }

        /// <summary>
        /// 0包邮  4京东
        /// </summary>
        public int FreightTemplateId { get; set; } = 4;

        public decimal FirstCommission { get; set; }

        public decimal SecondCommission { get; set; }

        public decimal ThirdCommission { get; set; }


        public int IsSetCommission { get; set; } = 0;


        public decimal CubicMeter { get; set; } = 0;

        public decimal FreightWeight { get; set; } = 0;

        public int SupplierID { get; set; } = 1;

    }

    public class MemberPhone
    {
        public int memberId { get; set; }
        public string mobile { get; set; }
    }
    public class KeyValue
    {
        public int Key { get; set; }

        public long Value { get; set; }
        public int Num { get; set; }
    }

    public class Comment
    {
        public int CommentCount { get; set; }

        public long SkuId { get; set; }
    }
}