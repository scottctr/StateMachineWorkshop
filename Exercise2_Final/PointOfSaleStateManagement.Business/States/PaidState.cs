namespace PointOfSaleStateManagement.Business.States
{
    public class PaidState : FinalState
    {
        public PaidState(Sale context) : base("Paid", context)
        { }
    }
}