namespace PointOfSaleStateManagement.Business.States
{
    public class OpenState : SaleStateBase
    {
        public OpenState(Sale context) : base("Open", context, isFinalState: false)
        {}

        public override ActionResult AddPayment(Payment payment)
        {
            var result = base.AddPayment(payment);
            if (!result.IsSuccess)
            { return result; }
            
            CheckForTransition();

            return result;
        }

        public override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var result = base.SetItemQuantity(productId, newQuantity);
            if (!result.IsSuccess)
            { return result; }

            CheckForTransition();

            return result;
        }

        private void CheckForTransition()
        {
            if (Sale.HasPositiveBalance())
            { Sale.TransitionTo(new OverpaidState(Sale)); }
            else if (Sale.IsPaid())
            { Sale.TransitionTo(new PaidState(Sale)); }
        }
    }
}