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
    public partial class AreaService :BaseService
    {
       
        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public string Get_AreaList()
        {
            return new JDMallIBLL().QueryArea().Result;
        }


        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public List<SelectItem> Get_AreaList(string value)
        {
            var areaList = CacheHelper.Get<List<SelectItem>>("jd_area_list", CacheTimeOption.HalfDay, () =>
            {
                using (SqlConnection con = new SqlConnection(connectStr))
                {
                    con.Open();
                    return GetList("select id as Value,name as Text  from  [JD_area]", con);
                }
            });

            var areas = areaList.AsQueryable();
            if (!string.IsNullOrEmpty(value) && !value.Equals("0"))
                areas = areas.Where(x => !string.IsNullOrEmpty(x.ParentKey) && x.ParentKey.Trim().Equals(value));
            else
                areas = areas.Where(_ => string.IsNullOrEmpty(_.ParentKey));

            return areas.ToList();
        }
    }

}