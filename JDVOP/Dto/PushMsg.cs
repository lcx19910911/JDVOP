using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JDVOP.Dto
{
    public class PushMsg<T>
    {
        public int id { get; set; }

        public int type { get; set; }

        public string time { get; set; }

        public T result { get; set; }
    }
}
