using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using JDVOP.Dto;
using JDVOP;
using JDVOP.Extensions;
using JDVOP.Interface;
using JDVOP.Service;
using JDVOP.Helper;
using Webservice;

namespace Web
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        MallInterface service = new JDMallService();
        List<long> skuIdList = new List<long> { 2396431 };


        //仓山区地址路径 16,1303,48713,48746


        protected void Page_Load(object sender, EventArgs e)
        {
            //var model = CacheHelper.Get<List<ProductInfo>>("jd_project", CacheTimeOption.ThirtyDay, () =>
            //{
            //    var list = new List<ProductInfo>();
            //    var pools = service.QueryPageNum();

            //    foreach (var item in pools.result)
            //    {
            //        var poolProducts = service.QueryProductSkuByPageNum(item.page_num);
            //        if (poolProducts.result.IsNotNullOrEmpty())
            //        {
            //            foreach (var obj in poolProducts.result.Split(','))
            //            {
            //                var project = service.FindProductDetial(obj.GetLong());
            //                if (project.result != null)
            //                {
            //                    list.Add(project.result);
            //                }
            //            }
            //        }
            //    }
            //    return list;
            //});
        }


        protected void refreshButton_Click(object sender, EventArgs e)
        {

            new ProjectService().UpdateProductStockAndPrice();
            Response.Write("成功");

            //var poolProducts = service.QueryProductSkuByPageNum(pools.result[0].page_num);

            //var product = service.FindProductDetial(1);
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            var pools=service.QueryPageNum();

            Response.Write(pools.ToJson());

            //var poolProducts = service.QueryProductSkuByPageNum(pools.result[0].page_num);

            //var product = service.FindProductDetial(1);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var pools = service.QueryPageNum();
            var poolProducts = service.QueryProductSkuByPageNum(pools.result[0].page_num);

            Response.Write(poolProducts.ToJson());


        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var result = service.FindProductDetial(2396431);
            Response.Write(result.ToJson());
            Response.Write("<br />");
            //Response.Write(result.result.introduction);
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            var result = service.QueryProductState(skuIdList);
            Response.Write(result.ToJson());
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            var result = service.QueryProductImage(skuIdList);
            Response.Write(result.ToJson());


        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            var result = service.QueryProductCommentSummarys(skuIdList);
            Response.Write(result.ToJson());
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            var result = service.CheckProductAreaLimit(skuIdList, 16,1303, 48713, 48746);
            Response.Write(result.ToJson());
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            var result = service.QueryProductPrice(skuIdList);
            Response.Write(result.ToJson());
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
            var result = service.CheckProductCanSale(skuIdList);
            Response.Write(result.ToJson());
        }

        protected void Button10_Click(object sender, EventArgs e)
        {
            
            var result = service.QueryProvinces();
            Response.Write(result.ToJson());
            Response.Write("<br />");

            result = service.QueryCityByProvnicesId(16);
            Response.Write(result.ToJson());
            Response.Write("<br />");

            result = service.QueryCountysByCityId(1303);
            Response.Write(result.ToJson());
            Response.Write("<br />");

            result = service.QueryTownsByCountyId(48713);
            Response.Write(result.ToJson());
            Response.Write("<br />");
        }

        protected void Button11_Click(object sender, EventArgs e)
        {
            var result = service.CheckStockForList(skuIdList, 16, 1303, 48713, null);
            Response.Write(result.ToJson());
            
        }

        protected void Button12_Click(object sender, EventArgs e)
        {
            SkuNum skuNum = new SkuNum { skuId = 907654, num = 5 };

            var result = service.CheckStockForOne(new List<SkuNum> { skuNum }, 16, 1303, 48713, 48746);
            Response.Write(result.ToJson());
        }

        protected void Button13_Click(object sender, EventArgs e)
        {
            JDMallIBLL bll = new JDMallIBLL();
            var result = bll.QueryBalance();

            Response.Write(result.ToJson());
        }

        protected void Button14_Click(object sender, EventArgs e)
        {
            SkuNum skuNum = new SkuNum { skuId = 907654, num = 1 };

            JDMallIBLL bll = new JDMallIBLL();
           // var result = bll.CreateOrder(Guid.NewGuid().ToString("N"),new List<SkuNum> { skuNum }, "罗先生", "13809545632", "286557139@qq.com", "建新南路153号", "350000", "13809545632", "", 16, 1303, 48713, 48746);

            //Response.Write(result.ToJson());
        }
    }
}