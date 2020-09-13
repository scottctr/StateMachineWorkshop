namespace PointOfSaleStateManagement.Business.States
{
    public abstract class SaleStateBase
    {
        public string StatusName { get; }
        protected Sale Sale { get; }
        public bool IsFinalState { get; }

        protected SaleStateBase(string statusName, Sale context, bool isFinalState)
        {
            StatusName = statusName;
            Sale = context;
            IsFinalState = isFinalState;
        }

        public virtual ActionResult AddChange(Change change)
        {
            return Sale.AddChangeInternal(change);
        }

        public virtual ActionResult AddItem(SaleItem newItem)
        {
            return Sale.AddItemInternal(newItem);
        }

        public virtual ActionResult AddPayment(Payment payment)
        {
            return Sale.AddPaymentInternal(payment);
        }

        public virtual ActionResult Cancel()
        {
            var result = Sale.CancelInternal();
            if (result.IsSuccess)
            { Sale.TransitionTo(new CancelledState(Sale)); }

            return result;
        }

        public virtual ActionResult DeleteItem(int productId)
        {
            return Sale.DeleteItemInternal(productId);
        }

        public virtual ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return Sale.SetItemQuantityInternal(productId, newQuantity);
        }
    }
}