namespace PointOfSaleStateManagement.Business.States
{
    public class OverpaidState : OpenState
    {
        public OverpaidState(Sale context) : base(context, "Overpaid")
        { }

        public override ActionResult AddChange(Change change)
        {
            var result = base.AddChange(change);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        public override ActionResult AddItem(SaleItem newItem)
        {
            var result = base.AddItem(newItem);
            if (result.IsSuccess)
            { CheckForTransition(); }

            return result;
        }

        public override ActionResult SetItemQuantity(int productId, int newQuantity)
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