using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RabbitMQConstants
    {
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
        public const string StockReserverdEventQueueName = "stock-reserved-queue";
        public const string StockPayentFailedEventQueueName = "stock-payment-failed-queue";

        public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue";

        public const string OrderPaymentSuccessededEventQueueName = "order-payment-successed-queue";
        public const string OrderPaymentFailEventQueueName = "order-payment-fail-queue";
        public const string OrderStockNotReservedEventQueueName = "order-stocknotreserved-queue";


        #region " Orchestrator "

        public const string OrderSaga = "order-saga";
        public const string PaymentStockReservedRequestQueueName = "payment-stock-reserved-request-queue";
        public const string OrderCompletedRequestQueueName = "order-completed-request-queue";
        public const string OrderStockNotReservedEventRequestQueueName = "order-stock-not-reserved-queue";
        public const string StockRollBackMessageQueueName = "stock-rollback-queue";

        #endregion
    }
}
