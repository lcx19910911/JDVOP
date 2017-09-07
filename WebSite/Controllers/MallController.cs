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

namespace WebSite.Controllers
{
    public class MallController : Controller
    {
        ProjectService service = new ProjectService();
        AreaService areaService = new AreaService();
        CategoryService categoryService = new CategoryService();
        MallInterface jdService = new JDMallService();
        protected internal JsonResult JResult<T>(T model)
        {
            return Json(new
            {
                Code = 0,
                Result = model
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View(categoryService.Get_CategorySelectItem(0));
        }
        public ActionResult Order()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitOrder()
        {
            return JResult(areaService.Get_AreaList());
        }

        /// <summary>
        /// 获取地区下拉框
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAreaList(string value)
        {
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return JResult(areaService.Get_AreaList(value));
        }

    }
}