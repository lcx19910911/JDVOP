using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDVOP;
using JDVOP.Interface;
using JDVOP.Service;
using JDVOP.Helper;
using JDVOP.Dto;
using JDVOP.Extensions;
using Webservice;
using Entity;
using Core;
using System.Collections;
using System.IO;

namespace WebSite.Controllers
{
    [LoginFilter]
    public class HomeController : Controller
    {

        ProjectService service = new ProjectService();
        CategoryService categoryService = new CategoryService();
        OrderService orderService = new OrderService();
        AreaService areaService = new AreaService();
        MallInterface jdService = new JDMallService();
        SMSService smsService = new SMSService();
        protected internal JsonResult JResult<T>(T model)
        {
            return Json(new
            {
                Code = 0,
                Result = model
            }, JsonRequestBehavior.AllowGet);
        }

        public string GetToken()
        {
            return JDVOP.Helper.JDRequestHelper<string>.GetToken();
        }


        public ActionResult Index()
        {
            return View(categoryService.Get_CategorySelectItem(0));
        }

        public ActionResult SMS()
        {
            return View();
        }
        public ActionResult Order()
        {
            return View();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string name, long? sku, string brandName, string firstCategory, string secondCategory, string thirdCategory, ProductStateEnum? state)
        {
            return JResult(service.Get_ProjectPageList(pageIndex, pageSize, name, sku, brandName, firstCategory, secondCategory, thirdCategory, state));
        }

        public ActionResult GetExecle(string name, long? sku, string brandName, string firstCategory, string secondCategory, string thirdCategory, ProductStateEnum? state)
        {
            var list=service.GetExecleList(name, sku, brandName, firstCategory, secondCategory, thirdCategory, state);

            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" );
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath +=  fileName;
            NPOIHelper<ProductInfo>.GetExcel(list, GetHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }

        private Dictionary<string, string> GetHT()
        {
            Dictionary<string,string> hs = new Dictionary<string, string>();
            hs["sku"] = "京东sku";
            hs["name"] = "标题";
            hs["categoryStr"] = "分类";
            hs["stateStr"] = "上下架状态";
            hs["brandName"] = "品牌";
            hs["CommentCount"] = "评论数";
            hs["JDprice"] = "京东促销价";
            hs["price"] = "成本价";
            hs["url"] = "网址";
            return hs;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetOrderPageList(int pageIndex, int pageSize, string cmccOrderId,
            DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            return JResult(orderService.GetOrderPageList(pageIndex, pageSize, cmccOrderId, createdTimeStart, createdTimeEnd));
        }


        /// <summary>
        /// 获取下拉框 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSelectItem(long value)
        {
            return JResult(categoryService.Get_CategorySelectItem(value));
        }
        /// <summary>
        /// 获取下拉框 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAreaSelectItem()
        {
            return JResult(areaService.Get_AreaList());
        }
        /// <summary>
        /// 更新12580商品库存
        /// </summary>
        /// <returns></returns>
        public ActionResult Refresh()
        {
            service.UpdateProductStock();
            return JResult(new Webservice.WebResult<bool>() { Result = true });
        }

        /// <summary>
        /// 更新分类
        /// </summary>
        /// <returns></returns>
        public ActionResult ReCategory()
        {
            categoryService.RefreshCategory();
            return JResult(new Webservice.WebResult<bool>() { Result = true });
        }

        
        /// <summary>
        /// 更新商品
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProduct()
        {
            service.GetProductFromPool();
            return JResult(new Webservice.WebResult<bool>() { Result = true });
        }
        /// <summary>
        /// 同步商品
        /// </summary>
        /// <returns></returns>
        public ActionResult SyncProduct()
        {
            service.SyncProduct();
            return JResult(new Webservice.WebResult<bool>() { Result = true });
        }

        public ActionResult CreateOrder(OrderDto model)
        {
            return JResult(orderService.CreateOrder(model.shop, model.thirdOrder, model.skuNum, model.province, model.city, model.county, model.invoiceType, model.town));
        }

        public ActionResult QueryJdorder(string cmccOrderId)
        {
            return JResult(jdService.QueryJDOrderInfo(cmccOrderId));
        }

        public ActionResult QueryJdorderTrack(long jdOrderId)
        {
            return JResult(jdService.QueryOrderTrack(jdOrderId));
        }

        public ActionResult QueryBalance()
        {
            return JResult(jdService.QueryBalance());
        }


        public ActionResult UpdateLocal()
        {
            service.UpdateProductStockAndPrice();
            return JResult(new Webservice.WebResult<bool>() { Result = true });
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetSMSPageList(int pageIndex, int pageSize,
            DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            return JResult(smsService.GetSMSPageList(pageIndex, pageSize, createdTimeStart, createdTimeEnd));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult FindSMS(string id)
        {
            return JResult<SMSBatch>(smsService.Find_SMSBatch(id));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSMSBatch(SMSBatch model)
        {
            return JResult(smsService.Add_SMSBatch(model));
        }
        public ActionResult ClearCache()
        {
            CacheHelper.Clear();
            return JResult("");
        }
    }
}