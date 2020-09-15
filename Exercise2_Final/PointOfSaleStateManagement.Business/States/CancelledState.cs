namespace PointOfSaleStateManagement.Business.States
{
    internal class CancelledState : FinalState
    {
        internal CancelledState(Sale context) : base("Cancelled", context)
        {}

        // No changes from Final state so no overrides here
        // - Only difference in Paid and Cancelled final states is how the sale is transitioned to each state
    }
}