using JDVOP.Extensions;
using System.Collections.Generic;

namespace JDVOP.Model
{
    public class JDApiResult<T>
    {
        public Dictionary<string, JDValueResult<T>> result { get; set; }
    }

    public class JDValueResult<T>
    {
        public bool success { get; set; } = false;

        public string resultMessage { get; set; }
        public string resultCode { get; set; }

        public T result { get; set; }

        public JDErrorCode code
        {
            get
            {
                if (string.IsNullOrEmpty(resultCode))
                    return JDErrorCode.sys_unknow_error;
                return (JDErrorCode)resultCode.GetInt();
            }
        }
    }

}
