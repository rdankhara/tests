using System;

namespace EquityComponents
{
    public class OrderPlacedEventArgs
    {
        public OrderPlacedEventArgs(string equityCode, decimal price)
        {
            EquityCode = equityCode;

            Price = price;
        }

        public string EquityCode { get; }

        public decimal Price { get; }

        public static implicit operator OrderPlacedEventArgs(OrderPlacedEventHandler v)
        {
            throw new NotImplementedException();
        }
    }

}
