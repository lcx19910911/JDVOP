using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Webservice;

namespace WebSite.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        
        /// <summary>
        /// 登录提交
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param> 
        /// <returns></returns>
        public JsonResult Submit(string account, string password)
        {
            return Json(new ProjectService().Login(account, password),JsonRequestBehavior.AllowGet);
        }
    }
}