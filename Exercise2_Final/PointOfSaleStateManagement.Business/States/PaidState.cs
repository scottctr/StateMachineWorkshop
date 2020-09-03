namespace PointOfSaleStateManagement.Business.States
{
    public class PaidState : SaleStateBase
    {
        public PaidState(Sale context) : base("Paid", context, isFinalState: true)
        { }

        public override ActionResult AddChange(Change change)
        {
            return new ActionResult(isSuccess: false, "Cannot give change on a paid sale");
        }

        public override ActionResult AddItem(SaleItem newItem)
        {
            return new ActionResult(isSuccess: false, "Cannot add item to a paid sale");
        }

        public override ActionResult AddPayment(Payment payment)
        {
            return new ActionResult(isSuccess: false, "Cannot add payment to a paid sale");
        }

        public override ActionResult Cancel()
        {
            return new ActionResult(isSuccess: false, "Cannot cancel a paid sale");
        }

        public override ActionResult DeleteItem(int productId)
        {
            return new ActionResult(isSuccess: false, "Cannot delete an item from a paid sale");
        }

        public override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return new ActionResult(isSuccess: false, "Cannot change item quantities on a paid sale");
        }
    }
}