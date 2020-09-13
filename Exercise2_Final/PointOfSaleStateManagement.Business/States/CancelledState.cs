namespace PointOfSaleStateManagement.Business.States
{
    public class CancelledState : FinalState
    {
        public CancelledState(Sale context) : base("Cancelled", context)
        {}
    }
}