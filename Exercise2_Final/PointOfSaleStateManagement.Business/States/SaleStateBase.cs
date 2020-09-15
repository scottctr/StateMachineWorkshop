namespace PointOfSaleStateManagement.Business.States
{
    // The base state class can't be used directly so this is an abstract class
    // - We didn't use an interface because there is some shared logic we don't want to duplicate
    internal abstract class SaleStateBase
    {
        internal string StatusName { get; }
        protected Sale Sale { get; }
        internal bool IsFinalState { get; }

        protected SaleStateBase(string statusName, Sale context, bool isFinalState)
        {
            StatusName = statusName;
            Sale = context;
            IsFinalState = isFinalState;
        }

        // Internal action methods Sale will forward requests to
        // - As the base class for all states, any common logic can be included here 
        // - To keep the core logic encapsulated in the Sale class, these methods call internal methods on the sale class

        internal virtual ActionResult AddChange(Change change)
        {
            return Sale.AddChangeInternal(change);
        }

        internal virtual ActionResult AddItem(SaleItem newItem)
        {
            return Sale.AddItemInternal(newItem);
        }

        internal virtual ActionResult AddPayment(Payment payment)
        {
            return Sale.AddPaymentInternal(payment);
        }

        internal virtual ActionResult Cancel()
        {
            // If Sale allows us to cancel, we take care of that here
            // -- This check could also go into the Open state

            var result = Sale.CancelInternal();
            if (result.IsSuccess)
            { Sale.TransitionTo(new CancelledState(Sale)); }

            return result;
        }

        internal virtual ActionResult DeleteItem(int productId)
        {
            return Sale.DeleteItemInternal(productId);
        }

        internal virtual ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return Sale.SetItemQuantityInternal(productId, newQuantity);
        }
    }
}
