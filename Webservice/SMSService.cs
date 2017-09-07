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
    public partial class SMSService : BaseService
    {

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

}