using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JDVOP.Extensions;
using JDVOP.Model;
using System.Web;

namespace JDVOP.Helper
{
    public class JDRequestHelper<T>
    {
        /// <summary>
        /// 获取返回结果 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="paramaJson"></param>
        /// <returns></returns>
        public static JDValueResult<T> GetResult(string method, string paramaJson,int type=0, string version = "1.0", string httpMethod = "post")
        {
            //失败返回体
            var failedResult = new JDValueResult<T>()
            {
                success = false,
                resultCode = JDErrorCode.sys_error.ToString()
            };
            try
            {
                string apiHost = "https://router.jd.com/api";
                string app_key = "53166537dc34480e95e256fc4b51192a";
                //参数为空值
                paramaJson = paramaJson.IsNullOrEmpty() ? "{}" : paramaJson;
                var paramsStr = string.Format("method={0}&app_key={1}&access_token={2}&timestamp={3}&v={4}&format=json&param_json={5}", method, app_key, GetToken(), GetTimestamp(), version, paramaJson);
                string resultStr = WebHelper.GetPage(apiHost, paramsStr, httpMethod);
                if (resultStr.IsNullOrEmpty())
                    return null;
                if (resultStr.Contains("token不存在"))
                {
                    CacheHelper.Remove("jd_token");
                    resultStr = WebHelper.GetPage(apiHost, paramsStr, httpMethod);
                    if (resultStr.IsNullOrEmpty())
                        return null;
                }
                //判断是否错误
                if (resultStr.Contains("errorResponse:"))
                {
                    var obj = resultStr.DeserializeJson<ErrorResponse>();
                    return new JDValueResult<T>()
                    {
                        success = false,
                        resultCode = obj.errorResponse.code,
                        resultMessage = obj.errorResponse.msg
                    };
                }
                //拼接返回json字符串为标准处理字符串
                resultStr = "{\"result\":" + resultStr + "}";
                if (type == 1)
                {
                    resultStr= resultStr.Replace("pOrder\":{", "pOrders\":{");
                }
                var resultObj = resultStr.DeserializeJson<JDApiResult<T>>();
                if (resultObj != null && resultObj.result.Keys.Count == 0)
                    return failedResult;
                return resultObj.result.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return failedResult;
            }
        }

        /// <summary>
        /// 获取token (缓存起来)
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            //缓存30天
            return CacheHelper.Get<string>("jd_token", CacheTimeOption.ThirtyDay, () =>
            {
                //string url = "https://kploauth.jd.com/oauth/token?grant_type=password&app_key={0}&app_secret={1}&state=0&username={2}&password={3}";

                //url = string.Format(url, "53166537dc34480e95e256fc4b51192a", "997cadf271e84d2ca6abca3ef184aed7", HttpUtility.UrlEncode("福建移动VOP"), System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile("111000", "MD5").ToLower());
                //string resultStr = WebHelper.GetPage(url);

                //if (resultStr.IsNullOrEmpty())
                //    return "";
                //var resultObj = resultStr.DeserializeJson<JDToken>();
                //return "55c8d46efb8d47aabf49c9164ffabaa68";
                //获取token
                string apiHost = "https://kploauth.jd.com/oauth/token";
                string app_key = "53166537dc34480e95e256fc4b51192a";
                string username = System.Web.HttpUtility.UrlEncode("福建移动VOP");
                string app_secret = "997cadf271e84d2ca6abca3ef184aed7";
                string password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile("111000", "MD5").ToLower();
                var url = string.Format("{0}?grant_type=password&app_key={1}&app_secret={2}&state=0&username={3}&password={4}", apiHost, app_key, app_secret, username, password);
                string resultStr = WebHelper.GetPage(url);
                if (resultStr.IsNullOrEmpty())
                    return string.Empty;
                var resultObj = resultStr.DeserializeJson<JDToken>();
                if (resultObj == null)
                    return string.Empty;
                //如果token过期了 刷新token
                if (resultObj.jDCode == JDErrorCode.sys_token_exprise)
                {
                    url = url.Replace("grant_type=password", "grant_type=refresh_token");
                    resultStr = WebHelper.GetPage(url);
                    if (resultStr.IsNullOrEmpty())
                        return string.Empty;
                    resultObj = resultStr.DeserializeJson<JDToken>();
                    if (resultObj != null && resultObj.code == "0")
                        return resultObj.access_token;
                    else
                        return string.Empty;
                }
                else if (resultObj.code == "0")
                    return resultObj.access_token;
                else
                    return string.Empty;

            });
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>

        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    public class ErrorResponse
    {
        public Msg errorResponse { get; set; }
    }

    public class Msg
    {
        public string code { get; set; }

        public string msg { get; set; }
    }
}
