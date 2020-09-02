using PointOfSaleStateManagement.Data.States;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSaleStateManagement.Data
{
    public class Sale
    {
        private readonly List<Change> _change = new List<Change>();
        private readonly List<Payment> _payments = new List<Payment>();
        private readonly List<SaleItem> _saleItems = new List<SaleItem>();
        private SaleStateBase _state;

        public Sale(int id)
        {
            Id = id;
            TransitionTo(new OpenState(this));
        }

        public ActionResult AddChange(Change change)
        {
            return _state.AddChange(change);
        }

        public ActionResult AddItem(SaleItem newItem)
        {
            return _state.AddItem(newItem);
        }

        public ActionResult AddPayment(Payment payment)
        {
            return _state.AddPayment(payment);
        }

        public double AmountPaid { get; internal set; }

        public double Balance { get; internal set; }

        public double ChangeGiven { get; internal set; }

        public ActionResult Cancel()
        {
            return _state.Cancel();
        }

        public ActionResult DeleteItem(int productId)
        {
            return _state.DeleteItem(productId);
        }

        public bool IsComplete => _state.IsFinalState;

        public int Id { get; }

        public double PaymentBalance => AmountPaid - ChangeGiven;

        public IReadOnlyList<SaleItem> SaleItems => _saleItems;

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return _state.SetItemQuantity(productId, newQuantity);
        }

        public string Status => _state.StatusName;

        public double SubTotal { get; set; }

        public int TotalItems { get; internal set; }

        internal ActionResult AddChangeRaw(Change change)
        {
            if (change.Amount > PaymentBalance)
            { return new ActionResult(isSuccess: false, "Change amount cannot exceed payment balance"); }

            _change.Add(change);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        internal ActionResult AddItemRaw(SaleItem newItem)
        {
            var existingItem = SaleItems.FirstOrDefault(i => i.Product.Id == newItem.Product.Id);

            if (existingItem != null)
            {
                newItem = new SaleItem(newItem.Product, newItem.Quantity + existingItem.Quantity);
                ReplaceItem(existingItem, newItem);
            }
            else
            { _saleItems.Add(newItem); }

            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        internal ActionResult AddPaymentRaw(Payment payment)
        {
            if (Balance >= 0)
            { return new ActionResult(isSuccess: false, "Cannot add payment to sale with balance equal or greater than 0"); }

            if (Balance >= 0)
            { return new ActionResult(isSuccess: false, "Cannot add payment to sale with balance equal or greater than 0"); }

            _payments.Add(payment);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        internal ActionResult CancelRaw()
        {
            return PaymentBalance > 0 
                ? new ActionResult(isSuccess: false, "Cannot cancel sale until payments returned") 
                : new ActionResult(isSuccess: true);
        }

        internal ActionResult DeleteItemRaw(int productId)
        {
            _saleItems.Remove(SaleItems.FirstOrDefault(i => i.Product.Id == productId));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        internal ActionResult SetItemQuantityRaw(int productId, int newQuantity)
        {
            var existingItem = SaleItems.FirstOrDefault(i => i.Product.Id == productId);
            if (existingItem is null)
            { return new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items."); }

            ReplaceItem(existingItem, new SaleItem(existingItem.Product, newQuantity));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        private void ReplaceItem(SaleItem existingItem, SaleItem newItem)
        {
            _saleItems[_saleItems.IndexOf(existingItem)] = newItem;
        }

        internal void TransitionTo(SaleStateBase newState)
        {
            _state = newState;
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
            AmountPaid = _payments.Sum(i => i.Amount);
        }

        private void UpdateChangeGiven()
        {
            ChangeGiven = _change.Sum(i => i.Amount);
        }

        private void UpdateBalance()
        {
            Balance = AmountPaid - SubTotal - ChangeGiven;
        }

        private void UpdateSubTotal()
        {
            SubTotal = SaleItems.Sum(i => i.TotalPrice);
        }

        private void UpdateTotalItems()
        {
            TotalItems = SaleItems.Sum(i => i.Quantity);
        }
    }
}