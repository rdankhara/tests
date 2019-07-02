using System;
using System.Threading;

namespace EquityComponents.Services
{
    public class EquityOrder : IEquityOrder
    {
        public IOrderService OrderService { get; }
        public Instrument Instrument { get; }

        public event OrderPlacedEventHandler OrderPlaced;
        public event OrderErroredEventHandler OrderErrored;
        private int _orderProcessed;

        public EquityOrder(IOrderService orderService, Instrument instrument)
        {
            OrderService = orderService;
            Instrument = instrument;
        }

        public void ReceiveTick(string equityCode, decimal price)
        {

            if (equityCode == Instrument.Code && price < Instrument.Threshold
                && Interlocked.CompareExchange(ref _orderProcessed, 1, 0) == 0)
            {
                try
                {
                    OrderService.Buy(Instrument.Code, Instrument.Quantity, price);
                    NotifyOrderPlaced(equityCode, price);
                }
                catch (System.Exception exception)
                {
                    NotifyOrderFailed(equityCode, price, exception);
                }
            }
        }

        private void NotifyOrderFailed(string equityCode, decimal price, Exception exception)
        {
            OrderErroredEventHandler orderErrored = OrderErrored;

            if (orderErrored != null)
            {
                OrderErrored(new OrderErroredEventArgs(equityCode, price, exception));
            }
        }

        private void NotifyOrderPlaced(string equityCode, decimal price)
        {
            OrderPlaced?.Invoke(new OrderPlacedEventArgs(equityCode, price));
        }
    }
}
