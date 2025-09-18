namespace Shared
{
    public class RabbitMqSettingsConst
    {
        public const string OrderSaga = "order-saga-queue";
        public const string PaymentStockReservedRequestQueueName = "payment-stock-reserved-request-queue";
        public const string OrderRequestCompletedQueueName = "order-request-completed-queue";
        public const string OrderRequestFailedQueueName = "order-request-failed-queue";

        public const string StockRollBackMessageQueueName = "stock-roolback-queue";


        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
        public const string StockReservedEventQueueName = "stock-reserved-queue";
        public const string OrderPaymentCompletedEventQueueName = "order-payment-completed-queue";
        public const string OrderPaymentFailEventQueueName = "order-payment-fail-queue";
        public const string StockPaymentFailEventQueueName = "stock-payment-fail-queue";
        public const string OrderStockNotReservedEventQueueName = "order-stock-not-reserved-queue";


    }
}
