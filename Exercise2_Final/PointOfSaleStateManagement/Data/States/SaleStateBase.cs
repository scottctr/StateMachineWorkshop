using System.Linq;

namespace PointOfSaleStateManagement.Data.States
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
            Sale.Change.Add(change);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        public virtual ActionResult AddItem(SaleItem newItem)
        {
            var existingItem = Sale.SaleItems.FirstOrDefault(i => i.Product.Id == newItem.Product.Id);

            if (existingItem != null)
            {
                newItem = new SaleItem(newItem.Product, newItem.Quantity + existingItem.Quantity);
                ReplaceItem(existingItem, newItem);
            }
            else
            { Sale.SaleItems.Add(newItem); }

            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public virtual ActionResult AddPayment(Payment payment)
        {
            if (Sale.Balance >= 0)
            { return new ActionResult(isSuccess: false, "Cannot add payment to sale with balance equal or greater than 0"); }

            if (Sale.Balance >= 0)
            { return new ActionResult(isSuccess: false, "Cannot add payment to sale with balance equal or greater than 0"); }

            Sale.Payments.Add(payment);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        public virtual ActionResult Cancel()
        {
            if (Sale.Payments.Sum(p => p.Amount) > Sale.Change.Sum(c => c.Amount))
            { return new ActionResult(isSuccess: false, "Cannot cancel sale until payments returned"); }

            Sale.TransitionTo(new CancelState(Sale));
            return new ActionResult(isSuccess: true);
        }

        public virtual ActionResult DeleteItem(int productId)
        {
            Sale.SaleItems.Remove(Sale.SaleItems.FirstOrDefault(i => i.Product.Id == productId));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public virtual ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var existingItem = Sale.SaleItems.FirstOrDefault(i => i.Product.Id == productId);
            if (existingItem is null)
            { return new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items."); }

            ReplaceItem(existingItem, new SaleItem(existingItem.Product, newQuantity));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        private void ReplaceItem(SaleItem existingItem, SaleItem newItem)
        {
            Sale.SaleItems[Sale.SaleItems.IndexOf(existingItem)] = newItem;
        }

        private void UpdateAmounts()
        {
            UpdateTotalItems();
            UpdateSubTotal();
            UpdateAmountPaid();
            UpdateChangeGiven();
            UpdateBalance();
        }

        private void UpdateAmountPaid()
        {
            Sale.AmountPaid = Sale.Payments.Sum(i => i.Amount);
        }

        private void UpdateChangeGiven()
        {
            Sale.ChangeGiven = Sale.Change.Sum(i => i.Amount);
        }

        private void UpdateBalance()
        {
            Sale.Balance = Sale.AmountPaid - Sale.SubTotal - Sale.ChangeGiven;
        }

        private void UpdateSubTotal()
        {
            Sale.SubTotal = Sale.SaleItems.Sum(i => i.TotalPrice);
        }

        private void UpdateTotalItems()
        {
            Sale.TotalItems = Sale.SaleItems.Sum(i => i.Quantity);
        }
    }
}