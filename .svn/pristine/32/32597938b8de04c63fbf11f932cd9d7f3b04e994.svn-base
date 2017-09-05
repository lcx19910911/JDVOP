using JDVOP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Model
{
    public class JDToken
    {
        /// <summary>
        /// 接口调用使用
        /// </summary>
        public string access_token { get; set; }
        public string code { get; set; }
        /// <summary>
        ///  授权到期时间（倒计时，单位秒）
        /// </summary>
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        /// <summary>
        /// 获取access_token时间戳 
        /// </summary>
        public string time { get; set; }
        public string token_type { get; set; }
        public string uid { get; set; }
        /// <summary>
        /// 获取token的京东账号
        /// </summary>
        public string user_nick { get; set; }


        public JDErrorCode jDCode
        {
            get
            {
                if (string.IsNullOrEmpty(code))
                    return JDErrorCode.sys_unknow_error;
                return (JDErrorCode)code.GetInt();
            }
        }
    }
}
