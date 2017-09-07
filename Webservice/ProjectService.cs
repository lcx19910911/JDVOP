using Entity;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Webservice
{
    public partial class ProjectService :BaseService
    {
        MallInterface service = new JDMallService();
        CategoryService categoryService = new CategoryService();

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
            CategoryService categoryService = new CategoryService();
            using (DbRepository entities = new DbRepository())
            {
                var query = GetProductQuery(entities, name, sku, brandName, firstCategory, secondCategory, thirdCategory, state);

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
                var contentDic = categoryService.Cache_Get_CategoryInfoList().Where(x => longList.Contains(x.catId)).Distinct().ToDictionary(x => x.catId.ToString());
                var skuNumList = list.Select(x => new SkuNum() { num = 1, skuId = x.sku }).ToList();

                //var stockListResult = service.CheckStockForOne(skuNumList, 16, 1303, 48713, 48746);
                var stateListResult = service.CheckProductCanSale(list.Select(x => x.sku).ToList());

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
                    if (x.FreshDate < DateTime.Now.AddHours(-4))
                    {
                        var product = service.FindProductDetial(x.sku)?.result;

                        if (product != null && !string.IsNullOrEmpty(product.name))
                        {
                            x.name = product?.name;
                        }


                        //var obj = stockListResult.result.Where(y => y.skuId == x.sku).FirstOrDefault();
                        //if (obj != null)
                        //{
                        //    if (obj.stockStateld == StockState.HaveAndForDeliver || obj.stockStateld == StockState.HaveAndPayForFromStock || obj.stockStateld == StockState.HaveAndWaitArriveForDeliver || obj.stockStateDesc == "有货")
                        //    {
                        //        x.stock = 10;
                        //        x.state = ProductStateEnum.OnSale;
                        //    }
                        //    else if (obj.stockStateDesc == "无货")
                        //    {
                        //        x.stock = 0;
                        //        x.state = ProductStateEnum.OffSale;
                        //    }
                        //}


                        var stateObj = stateListResult.result.Where(y => y.skuId == x.sku).FirstOrDefault();

                        if (stateObj != null)
                        {
                            x.state = (ProductStateEnum)stateObj.saleState.GetInt();
                        }

                        var priceObj = priceListResult.result.Where(y => y.skuId == x.sku).FirstOrDefault();
                        if (priceObj != null)
                        {
                            x.JDprice = priceObj.jdPrice;
                            x.price = priceObj.price;
                        }

                        try
                        {
                            if (x.state == ProductStateEnum.OnSale)
                            {
                                //手机端 京东售价/京东秒杀
                                string result = WebHelper.GetPage("https://item.m.jd.com/product/" + x.sku + ".html");

                                //秒杀价
                                string pattern = "<span id=\"spec_price\">(.+?)</span>";
                                Match m = Regex.Match(result, pattern, RegexOptions.IgnoreCase);
                                if (m.Success)
                                {
                                    string str_spec_price = m.Groups[1].Value.Trim();
                                    decimal spec_price = decimal.Parse(str_spec_price);
                                    x.JDprice = Math.Min(x.JDprice, spec_price);
                                }
                            }
                        }
                        catch
                        {

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
                    }
                });

                entities.SaveChanges();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 获取搜索query
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="name"></param>
        /// <param name="sku"></param>
        /// <param name="brandName"></param>
        /// <param name="firstCategory"></param>
        /// <param name="secondCategory"></param>
        /// <param name="thirdCategory"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IQueryable<ProductInfo> GetProductQuery(DbRepository entities, string name, long? sku, string brandName, string firstCategory, string secondCategory, string thirdCategory, ProductStateEnum? state)
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
            return query;
        }

        /// <summary>
        /// 获取导出excle的集合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sku"></param>
        /// <param name="brandName"></param>
        /// <param name="firstCategory"></param>
        /// <param name="secondCategory"></param>
        /// <param name="thirdCategory"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<ProductInfo> GetExecleList(string name, long? sku, string brandName, string firstCategory, string secondCategory, string thirdCategory, ProductStateEnum? state)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = GetProductQuery(entities, name, sku, brandName, firstCategory, secondCategory, thirdCategory, state);
                var list = query.OrderBy(x => x.brandName).ThenBy(x => x.category).ToList();

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
                var contentDic = categoryService.Cache_Get_CategoryInfoList().Where(x => longList.Contains(x.catId)).Distinct().ToDictionary(x => x.catId.ToString());
                list.ForEach(x =>
                {
                    x.url = "https://item.m.jd.com/product/" + x.sku + ".html";
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

                return list;
            }
        }


        /// <summary>
        /// 修改产品库存
        /// </summary>
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

        /// <summary>
        /// 修改库存和价格
        /// </summary>
        public void UpdateProductStockAndPrice()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                MallInterface service = new JDMallService();

                using (var db = new DbRepository())
                {
                    #region 刷新商品数据库数据

                    using (SqlConnection con = new SqlConnection(jdvopConnectStr))
                    {
                        con.Open();
                        //4小时以上才更新
                        var dic = GetListResult<KeyValue>("select sku as [Value] from ProductInfo where (FreshDate is null or datediff(HOUR,FreshDate,getdate())>=4);", con);
                        //var dic = GetListResult<KeyValue>("select sku as [Value] from ProductInfo where (sku=2770504);", con);

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
                            var stateListResult = service.CheckProductCanSale(selectList);
                            while (!stateListResult.success)
                            {
                                stateListResult = service.CheckProductCanSale(selectList);
                            }
                            selectList.ForEach(x =>
                            {
                                var state = 0;
                                decimal price = 0;
                                decimal jdPrice = 0;
                                var stockObj = stateListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                                var priceObj = priceListResult.result.Where(y => y.skuId == x).FirstOrDefault();
                                if (stockObj != null)
                                {
                                    state = stockObj.saleState.GetInt();
                                }
                                if (priceObj != null)
                                {
                                    price = priceObj.price;
                                    jdPrice = priceObj.jdPrice;
                                }

                                if (state == 1)
                                {
                                    try
                                    {
                                        //手机端 京东售价/京东秒杀
                                        string result = WebHelper.GetPage("https://item.m.jd.com/product/" + x + ".html");

                                        //秒杀价
                                        string pattern = "<span id=\"spec_price\">(.+?)</span>";
                                        Match m = Regex.Match(result, pattern, RegexOptions.IgnoreCase);
                                        if (m.Success)
                                        {
                                            string str_spec_price = m.Groups[1].Value.Trim();
                                            decimal spec_price = decimal.Parse(str_spec_price);
                                            jdPrice = Math.Min(jdPrice, spec_price);
                                        }

                                        sb.AppendFormat("update ProductInfo set price={0},JDprice={1},state={2},FreshDate=getdate() where sku={3};\r\n", price, jdPrice, state, x);
                                    }
                                    catch
                                    {
                                        
                                    }
                                }else
                                {
                                    sb.AppendFormat("update ProductInfo set price={0},JDprice={1},state={2},FreshDate=getdate() where sku={3};\r\n", price, jdPrice, state, x);
                                }
                                

                            });
                            index++;
                            selectList = cmccIdSkuList.Skip(index * 100).Take(100).ToList();

                            //执行
                            ExecuteNonQuer(sb.ToString(), con);
                            sb = new StringBuilder();
                        }

                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
                                        CategoryName = categoryService.GetCategoryName(category_dic, product.category),
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
            var firstCategory = categoryService.GetCategory(categryList[0], 1, 0, con);
            //第一级判断是否存在
            if (firstCategory != null)
            {
                //第二级
                var secondCategory = categoryService.GetCategory(categryList[1], 2, firstCategory.Key, con);
                if (secondCategory != null)
                {
                    if (categryList.Length == 3)
                    {
                        var thirdCategoryResult = categoryService.GetCategory(categryList[2], 3, secondCategory.Key, con);
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
       
    }

}