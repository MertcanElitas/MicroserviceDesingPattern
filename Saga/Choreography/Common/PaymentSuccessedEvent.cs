using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PaymentSuccessedEvent
    {
        public string BuyerId { get; set; }
        public int OrderId { get; set; }

        public string successtwo { get; set; }
        public string successthree { get; set; }
    }
}
