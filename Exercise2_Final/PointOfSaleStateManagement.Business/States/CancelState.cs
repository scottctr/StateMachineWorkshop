namespace PointOfSaleStateManagement.Business.States
{
    public class CancelState : FinalState
    {
        public CancelState(Sale context) : base("Cancelled", context)
        {}
    }
}