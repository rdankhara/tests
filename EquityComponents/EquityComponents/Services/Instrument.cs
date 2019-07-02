namespace EquityComponents.Services
{
    public class Instrument
    {
        public Instrument(string code, decimal threshold, int quantity)
        {
            Code = code;
            Threshold = threshold;
            Quantity = quantity;
        }
        public string Code { get; private set; }

        public decimal Threshold { get; private set; }

        public int Quantity { get; private set; }
    }
}
