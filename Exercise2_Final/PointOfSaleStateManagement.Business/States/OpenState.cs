namespace PointOfSaleStateManagement.Business.States
{
    internal class OpenState : SaleStateBase
    {
        internal OpenState(Sale context, string statusName = "Open") : base(statusName, context, isFinalState: false)
        { }

        // Only overriding the methods that have Open specific logic for processing or transitions
        // -- Adding payments or removing items could cause the sale to go to Paid or Overpaid

        internal override ActionResult AddPayment(Payment payment)
        {
            var result = base.AddPayment(payment);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        internal override ActionResult DeleteItem(int productId)
        {
            var result = base.DeleteItem(productId);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        internal override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var result = base.SetItemQuantity(productId, newQuantity);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        // It's possible that any of the methods above could require changing to the Paid or Overpaid state so we're centralizing those checks here

        private void CheckForTransition()
        {
            if (Sale.HasPositiveBalance())
            { Sale.TransitionTo(new OverpaidState(Sale)); }
            else if (Sale.IsPaid)
            { Sale.TransitionTo(new PaidState(Sale)); }
        }
    }
}