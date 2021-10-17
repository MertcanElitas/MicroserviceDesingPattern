using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StockNotReserverdEvent
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
    }
}
