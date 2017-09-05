namespace DFshop.UI.Common.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Xml;
    using JDVOP.Dto;
    using JDVOP;
    using JDVOP.Extensions;

    public class RegionHandler : IHttpHandler
    {


        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string str2 = context.Request["action"];
                switch (str2)
                {
                    case "getregions":
                        context.Response.Write(new JDMallIBLL().QueryArea());
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

