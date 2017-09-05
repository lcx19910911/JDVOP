using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDVOP.Dto
{
    /// <summary>
    /// 订单日历
    /// </summary>
    public class CalendarDto:AreaDto
    {

        public List<SkuNum> sku { get; set; }

        public PaymentType paymentType { get; set; }
    }
}
