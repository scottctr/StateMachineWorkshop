namespace PointOfSaleStateManagement.Business.States
{
    internal class PaidState : FinalState
    {
        internal PaidState(Sale context) : base("Paid", context)
        { }


        // No changes from Final state so no overrides here
        // - Only difference in Paid and Cancelled final states is how the sale is transitioned to each state
    }
}