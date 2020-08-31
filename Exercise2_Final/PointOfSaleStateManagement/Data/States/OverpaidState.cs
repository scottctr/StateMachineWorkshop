using System;
using System.Linq;

namespace PointOfSaleStateManagement.Data.States
{
    public class OverpaidState : SaleStateBase
    {
        public OverpaidState(Sale context) : base("Overpaid", context, isFinalState: false)
        { }

        public override ActionResult AddChange(Change change)
        {
            var result = base.AddChange(change);
            if (!result.IsSuccess)
            { return result; }

            CheckForTransition();

            return result;
        }

        public override ActionResult AddItem(SaleItem newItem)
        {
            var result = base.AddItem(newItem);
            if (!result.IsSuccess)
            { return result; }

            CheckForTransition();

            return result;
        }

        public override ActionResult AddPayment(Payment payment)
        {
            return new ActionResult(isSuccess: false, "Cannot add payments to over paid sales");
        }

        public override ActionResult Cancel()
        {
            return Sale.Payments.Sum(p => p.Amount) > Sale.Change.Sum(c => c.Amount) 
                ? new ActionResult(isSuccess: false, "Cannot cancel sale until payments returned") 
                : base.Cancel();
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
            if (Math.Abs(Sale.Balance) < 0.001)
            { Sale.TransitionTo(new PaidState(Sale)); }
            else if (Sale.Balance < 0)
            { Sale.TransitionTo(new OpenState(Sale)); }
        }
    }
}