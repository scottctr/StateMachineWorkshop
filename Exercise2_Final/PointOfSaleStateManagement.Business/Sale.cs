using PointOfSaleStateManagement.Business.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSaleStateManagement.Business
{
    public class Sale
    {
        private readonly List<Change> _change = new List<Change>();
        private readonly List<Payment> _payments = new List<Payment>();
        private readonly List<SaleItem> _saleItems = new List<SaleItem>();

        // Reference to current state
        private SaleStateBase _state;

        public Sale(int id)
        {
            Id = id;

            // Initializing the Sale to the Open state
            // - Need to persist current state and initialize appropriately in real app
            TransitionTo(new OpenState(this));
        }

        // All action methods now forward request to the current state

        public ActionResult AddChange(Change change)
        {
            return _state.AddChange(change);
        }

        // The previous action methods have been converted to internal methods that are called by the state classes

        internal ActionResult AddChangeInternal(Change change)
        {
            // Non-state rules (i.e. invariant rules) are still encapsulated in the core Sale logic
            if (change.Amount > PaymentBalance)
            { return new ActionResult(isSuccess: false, "Change amount cannot exceed payment balance"); }

            _change.Add(change);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        public ActionResult AddItem(SaleItem newItem)
        {
            return _state.AddItem(newItem);
        }

        internal ActionResult AddItemInternal(SaleItem newItem)
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

        public ActionResult AddPayment(Payment payment)
        {
            return _state.AddPayment(payment);
        }

        internal ActionResult AddPaymentInternal(Payment payment)
        {
            if (Balance >= 0)
            { return new ActionResult(isSuccess: false, "Cannot add payment to sale with balance equal or greater than 0"); }

            _payments.Add(payment);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        public double AmountPaid { get; private set; }

        public double Balance { get; private set; }

        public ActionResult Cancel()
        {
            return _state.Cancel();
        }

        internal ActionResult CancelInternal()
        {
            return _payments.Sum(p => p.Amount) > _change.Sum(c => c.Amount) 
                ? new ActionResult(isSuccess: false, "Cannot cancel sale until payments returned") 
                : new ActionResult(isSuccess: true);
        }

        public double ChangeGiven { get; private set; }

        public ActionResult DeleteItem(int productId)
        {
            return _state.DeleteItem(productId);
        }

        internal ActionResult DeleteItemInternal(int productId)
        {
            _saleItems.Remove(SaleItems.FirstOrDefault(i => i.Product.Id == productId));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public int Id { get; }

        public bool IsComplete => _state.IsFinalState;

        public double PaymentBalance => AmountPaid - ChangeGiven;

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return _state.SetItemQuantity(productId, newQuantity);
        }

        internal ActionResult SetItemQuantityInternal(int productId, int newQuantity)
        {
            var existingItem = SaleItems.FirstOrDefault(i => i.Product.Id == productId);
            if (existingItem is null)
            { return new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items."); }

            ReplaceItem(existingItem, new SaleItem(existingItem.Product, newQuantity));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public string Status => _state.StatusName;

        public double SubTotal { get; set; }

        public IReadOnlyList<SaleItem> SaleItems => _saleItems.AsReadOnly();

        public int TotalItems { get; private set; }

        internal bool HasPositiveBalance()
        {
            return Balance > 0;
        }

        internal bool IsPaid => Math.Abs(Balance) < .001 && _payments.Any() && SaleItems.Any(i => i.Quantity > 0);

        private void ReplaceItem(SaleItem existingItem, SaleItem newItem)
        {
            _saleItems[_saleItems.IndexOf(existingItem)] = newItem;
        }

        // Used by the state classes to transition the Sale's current state

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
