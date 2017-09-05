

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite
{
    /// <summary>
    /// 过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LoginFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var controllerName = filterContext.RouteData.Values["Controller"].ToString();
            string requestUrl = filterContext.HttpContext.Request.Url.ToString();
            HttpCookie cookie = HttpContext.Current.Request.Cookies["LoginAdmin"];
            if (cookie == null)
            {
                RedirectResult redirectResult = new RedirectResult("/Account/Login?redirecturl=" + requestUrl);
                filterContext.Result = redirectResult;
            }
        }


    }
}