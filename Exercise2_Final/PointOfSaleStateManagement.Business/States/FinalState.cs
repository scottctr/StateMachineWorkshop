namespace PointOfSaleStateManagement.Business.States
{
    public abstract class FinalState : SaleStateBase
    {
        protected FinalState(string statusName, Sale context) : base(statusName, context, isFinalState: true)
        {}

        public override ActionResult AddChange(Change change)
        {
            return new ActionResult(isSuccess: false, $"Cannot give change on a {StatusName.ToLower()} sale");
        }

        public override ActionResult AddItem(SaleItem newItem)
        {
            return new ActionResult(isSuccess: false, $"Cannot add item to a {StatusName.ToLower()} sale");
        }

        public override ActionResult AddPayment(Payment payment)
        {
            return new ActionResult(isSuccess: false, $"Cannot add payment to a {StatusName.ToLower()} sale");
        }

        public override ActionResult Cancel()
        {
            return new ActionResult(isSuccess: false, $"Cannot cancel a {StatusName.ToLower()} sale");
        }

        public override ActionResult DeleteItem(int productId)
        {
            return new ActionResult(isSuccess: false, $"Cannot delete items from a {StatusName.ToLower()} sale");
        }

        public override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return new ActionResult(isSuccess: false, $"Cannot change item quantities on a {StatusName.ToLower()} sale");
        }
    }
}