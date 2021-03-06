﻿namespace PointOfSaleStateManagement.Business.States
{
    // Derived from OpenState
    internal class OverpaidState : OpenState
    {
        internal OverpaidState(Sale context) : base(context, "Overpaid")
        { }

        // Only overriding the methods that have Overpaid specific logic for processing or transitions
        // -- Giving change or adding items could cause the sale to go to Open or Paid

        internal override ActionResult AddChange(Change change)
        {
            var result = base.AddChange(change);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        internal override ActionResult AddItem(SaleItem newItem)
        {
            var result = base.AddItem(newItem);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        // Payments not allowed in Overpaid state

        internal override ActionResult AddPayment(Payment payment)
        {
            return new ActionResult(isSuccess: false, "Payments not allowed in Overpaid state");
        }

        internal override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var result = base.SetItemQuantity(productId, newQuantity);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        private void CheckForTransition()
        {
            if (Sale.IsPaid)
            { Sale.TransitionTo(new PaidState(Sale)); }
            else if (!Sale.HasPositiveBalance())
            { Sale.TransitionTo(new OpenState(Sale)); }
        }
    }
}