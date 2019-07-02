namespace EquityComponents
{
    public delegate void OrderPlacedEventHandler(OrderPlacedEventArgs e);

    public interface IOrderPlaced
    {
        event OrderPlacedEventHandler OrderPlaced;
    }
}
