namespace PointOfSaleStateManagement.Data
{
    public class Payment
    {
        public Payment(double amount)
        {
            Amount = amount;
        }

        public double Amount { get; }
    }
}