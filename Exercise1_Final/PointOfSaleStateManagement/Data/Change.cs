namespace PointOfSaleStateManagement.Data
{
    public class Change
    {
        public Change(double amount)
        {
            Amount = amount;
        }

        public double Amount { get; }
    }
}