using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PaymentMessage
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }



        public string varone { get; set; }
        public string vartwo { get; set; }

        public string varthree { get; set; }
        public string varfour { get; set; }

    }
}
