namespace PointOfSaleStateManagement.Data
{
    public enum SaleState
    {
        Open,
        Overpaid,
        Finalized, // Base state for all final states to prevent duplicating code
        Cancelled,
        Paid
    }
}