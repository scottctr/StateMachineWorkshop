namespace PointOfSaleStateManagement.Business.States
{
    public class CancelState : SaleStateBase
    {
        public CancelState(Sale context) : base("Cancelled", context, isFinalState: true)
        {}

        public override ActionResult AddChange(Change change)
        {
            return new ActionResult(isSuccess: false, "Cannot give change on a cancelled sale");
        }

        public override ActionResult AddItem(SaleItem newItem)
        {
            return new ActionResult(isSuccess: false, "Cannot add item to a cancelled sale");
        }

        public override ActionResult AddPayment(Payment payment)
        {
            return new ActionResult(isSuccess: false, "Cannot add payment to a cancelled sale");
        }

        public override ActionResult Cancel()
        {
            return new ActionResult(isSuccess: false, "Cannot cancel a cancelled sale");
        }

        public override ActionResult DeleteItem(int productId)
        {
            return new ActionResult(isSuccess: false, "Cannot delete items from a cancelled sale");
        }

        public override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return new ActionResult(isSuccess: false, "Cannot change item quantities on a cancelled sale");
        }
    }
}