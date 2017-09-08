using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JDVOP.Extensions;
using JDVOP.Model;
using JDVOP.Dto;
using System.Linq;
using System.Collections.Generic;
using JDVOP.Interface;
using JDVOP.Service;
using System.Text;
using JDVOP;
using Repository;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using JDVOP.Helper;
using Webservice;


namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        
        string connectStr = "data source=120.76.155.106;user id=CMCC_12580_sql;password=dfqm@201608sql;database=CMCC_12580";
        //string connectStr = "data source=.;user id=sa;password=123456;database=CMCC_12580";
        string ids = "383871,594183,2396502,1857759,1058052,2894163,874205,167546,595936,1506778,2264597,1605651,968111,1399668,2734186,1989015,2599660,2687146,3102734,2263714,2232084,2425676,1058054,2263698,595926,861100,2881770,1370589,1365533,2723770,2829222,376056,2734304,2283996,1365527,1058010,2264657,2302898,1153067,538729,1504775,3039102,1057237,603496,577083,916143,1350524,1504812,2354650,829764,997908,2599499,153363,2734194,1093078,608054,153360,874203,1077119,857977,1069555,365888,968110,2734200,2723754,2861719,666422,1102138,1576618,2377411";
        [TestMethod]
        public void TestMethod1()
        {
            var categoryService = new CategoryService();
            categoryService.RefreshCategory();

            var ttt= "{\"pOrder\":0,\"pOrders\":null,\"cOrder\":null,\"ID\":\"9653cff735d24cf2b88c39d4cbf4a787\",\"jdOrderId\":50742331509,\"CMCCOrderId\":\"170329103556269\",\"orderPrice\":99.0,\"orderNakedPrice\":84.62,\"orderTaxPrice\":14.38,\"freight\":0.0,\"sku\":[{\"skuId\":1509606,\"num\":1,\"category\":967,\"tax\":17.0,\"name\":\"怡禾康 YH-999 颈椎按摩器按摩靠垫温热红光按摩枕\",\"price\":97.0,\"taxPrice\":14.09,\"nakedPrice\":82.91,\"type\":0,\"oid\":0},{\"skuId\":1310316,\"num\":1,\"category\":6232,\"tax\":17.0,\"name\":\"【京东超市】宜洁 牙签竹制袋装200枚Y-9892\",\"price\":2.0,\"taxPrice\":0.29,\"nakedPrice\":1.71,\"type\":0,\"oid\":0}],\"DetailsJsonStr\":\"商品怡禾康 YH-999 颈椎按摩器按摩靠垫温热红光按摩枕 数量：1;\r\n商品【京东超市】宜洁 牙签竹制袋装200枚Y-9892 数量：1;\r\n\",\"State\":0,\"orderState\":1,\"StateStr\":null,\"type\":2,\"CreateTime\":\"2017 - 03 - 29T11: 29:47.5851408 + 08:00\"}";
            MallInterface service = new JDMallService();
            using (var db = new DbRepository())
            {
                var odfd = ttt.DeserializeJson<JDOrderDto>();
                db.JDOrderInfo.Add(odfd);
                db.SaveChanges();
                var sssss = service.QueryJDOrderInfo("170325192137315").result;

                sssss.ID = Guid.NewGuid().ToString("N");
                sssss.State = JDOrderState.Complete;
                sssss.CMCCOrderId = "170308154134731";
                db.JDOrderInfo.Add(sssss);
                db.SaveChanges();
            }

            //    var result = new JDMallIBLL().QueryOrderTrack(thirdOrder.ToString(), nums, order.ShipTo, order.CellPhone, order.Address, order.TelPhone, order.Remark, province, city, county, town);
            //if (result.IsSuccess)
            //{
            //    using (var db = new DbRepository())
            //    {
            //        result.Result.ID = Guid.NewGuid().ToString("N");
            //        result.Result.CMCCOrderId = thirdOrder;
            //        result.Result.State = JDOrderState.New;
            //        result.Result.DetailsJsonStr = result.Result.sku.ToJson();
            //        db.JDOrderInfo.Add(result.Result);
            //    }
            //}



            //new ProjectService().UpdateProductStockAndPrice();
          
        }

        List<string> loseIdList = new List<string>();
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

        public void GetPriceMsg(List<PushMsg<MsgProductPrice>> model)
        {
            string ss = "";
        }
        public void GetStateMsg(List<PushMsg<SkuNum>> model)
        {
            string ss = "";
        }

        public string GetUpdateProduct(long sku, int stock)
        {
            if (sku != 0)
            {
                return $"update  Hishop_Products set  SaleStatus={(stock > 0 ? 1 : 2)} where ProductId in (select ProductId from Hishop_Products where ProductCode='{sku}');\r\n" +
                    $"update  Hishop_SKUs set  Stock={stock} where ProductId in  (select ProductId from Hishop_Products where ProductCode='{sku}');\r\n";
            }
            return "";
            
        }

        public void UpdateProduct(SqlConnection con, long sku, decimal price, int stock)
        {
            SqlParameter[] parameters = new SqlParameter[10];
            int ProductId = ExecuteScalar("select ProductId from Hishop_Products where ProductCode='" + sku + "'", con);
            if (ProductId != 0)
            {
                parameters = new SqlParameter[]{
                                           new SqlParameter("@ProductId",ProductId),
                                           new SqlParameter("@Price",price),
                                           new SqlParameter("@Stock",stock),
                                           new SqlParameter("@SkuId",ProductId+"_0"),
                                      };


                string sqlComand = "update  Hishop_Products set  MarketPrice=@Price,MinShowPrice=@Price,MaxShowPrice=@Price,SaleStatus=" + (stock > 0 ? 1 : 2) + " where ProductId=@ProductId;" +
                    "update  Hishop_SKUs set  SalePrice=@Price,Stock=@Stock where SkuId=@SkuId;";
                int i = ExecuteNonQuer(sqlComand, con, parameters);
            }
        }

        public void SqlCommandAppend(Hishop_Products model)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                SqlParameter[] parameters = new SqlParameter[10];
                con.Open();
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
                            loseIdList.Add(model.ProductCode);
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
        }

        public void ChangePath()
        {

            using (SqlConnection con = new SqlConnection(connectStr))
            {
                var result = GetListResult("SELECT  [CategoryId] as [Key],[Path] as [Value] FROM[CMCC_12580].[dbo].[Hishop_Categories]", con).ToDictionary(x => x.Key.ToString());
                var productResult = GetListResult("SELECT  [ProductId] as [Key],convert(varchar(5),[CategoryId]) as [Value] FROM [CMCC_12580].[dbo].[Hishop_Products]", con);
                StringBuilder sb = new StringBuilder();
                foreach (var item in productResult)
                {
                    if (item.Value.IsNotNullOrEmpty() && result.ContainsKey(item.Value))
                    {
                        sb.AppendFormat("update [CMCC_12580].[dbo].[Hishop_Products] set MainCategoryPath='{0}' where ProductId={1};\r\n", result[item.Value].Value, item.Key);
                    }
                }

                string ss = sb.ToString();
                ss = "";
            }
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
            return GetResult(categorySql, con);
        }

        public int ExecuteNonQuer(string sqlcomand, SqlConnection conn, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, conn);
            PrepareCommand(cmd, conn, null, sqlcomand, cmdParms);
            return cmd.ExecuteNonQuery();
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
        public KeyValue GetResult(string sqlcomand, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            adapter.Fill(ds);//填充数据

            if (ds.Tables[0].Rows.Count > 0)
                return ModelConvertHelper<KeyValue>.ToModel(ds.Tables[0].Rows[0]);
            else
                return null;
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
    }

    public class KeyValue
    {
        public int Key { get; set; }

        public string Value { get; set; }
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
}
