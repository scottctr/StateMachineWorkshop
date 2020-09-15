namespace PointOfSaleStateManagement.Business.States
{
    // Final isn't an actual state so this is an abstract class
    // - There is some shared logic used by all classes that inherit from Final so not using an interface

    internal abstract class FinalState : SaleStateBase
    {
        protected internal FinalState(string statusName, Sale context) : base(statusName, context, isFinalState: true)
        {}

        // No actions or transitions allowed from any final state so all actions return an unsuccessful ActionResult

        internal override ActionResult AddChange(Change change)
        {
            return new ActionResult(isSuccess: false, $"Cannot give change on a {StatusName.ToLower()} sale");
        }

        internal override ActionResult AddItem(SaleItem newItem)
        {
            return new ActionResult(isSuccess: false, $"Cannot add item to a {StatusName.ToLower()} sale");
        }

        internal override ActionResult AddPayment(Payment payment)
        {
            return new ActionResult(isSuccess: false, $"Cannot add payment to a {StatusName.ToLower()} sale");
        }

        internal override ActionResult Cancel()
        {
            return new ActionResult(isSuccess: false, $"Cannot cancel a {StatusName.ToLower()} sale");
        }

        internal override ActionResult DeleteItem(int productId)
        {
            return new ActionResult(isSuccess: false, $"Cannot delete items from a {StatusName.ToLower()} sale");
        }

        internal override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return new ActionResult(isSuccess: false, $"Cannot change item quantities on a {StatusName.ToLower()} sale");
        }
    }
}