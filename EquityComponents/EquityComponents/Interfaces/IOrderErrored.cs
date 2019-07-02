namespace EquityComponents
{
    public delegate void OrderErroredEventHandler(OrderErroredEventArgs e);

    public interface IOrderErrored
    {
        event OrderErroredEventHandler OrderErrored;
    }
}
