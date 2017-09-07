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
    public partial class OrderService : BaseService
    {
        MallInterface service = new JDMallService();



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
        /// 下12580单
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="thirdOrder"></param>
        /// <param name="nums"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="invoiceType"></param>
        /// <param name="town"></param>
        /// <returns></returns>
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

    }

}