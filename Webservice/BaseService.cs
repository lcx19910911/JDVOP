using JDVOP.Extensions;
using JDVOP.Helper;
using JDVOP.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webservice
{
    public class BaseService
    {

        public string connectStr = "data source=kmi.ppjd.cn;user id=CMCC_12580_sql;password=dfqm@201608sql;database=CMCC_12580";

        public string KMIconnectStr = "data source=kmi.ppjd.cn;user id=KMI_sql;password=dfqm@2016sql;database=KMI";
        public string jdvopConnectStr = "data source=kmi.ppjd.cn;user id=CMCC_12580_sql;password=dfqm@201608sql;database=jdvop";

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
