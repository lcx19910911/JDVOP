using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class CMCC_SMS_do
    {
        public int id { get; set; }
        public int sequenceId { get; set; }
        public string mobile { get; set; }
        public DateTime addtime { get; set; }

        public int state { get; set; }

        public string batchNum { get; set; }
    }
}
