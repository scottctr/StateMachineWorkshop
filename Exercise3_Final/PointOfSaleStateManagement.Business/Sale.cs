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
        private readonly SaleStateMachine _stateMachine;

        public Sale(int id)
        {
            Id = id;
            _stateMachine = new SaleStateMachine(this);
        }

        public ActionResult AddChange(Change change)
        {
            return change.Amount > PaymentBalance
                ? new ActionResult(isSuccess: false, "Change amount cannot exceed payment balance")
                : ExecuteAction(() => _stateMachine.AddChange(change));
        }

        internal void AddChangeInternal(Change change)
        {
            _change.Add(change);
            UpdateAmounts();
        }

        public ActionResult AddItem(SaleItem newItem)
        {
            return ExecuteAction(() => _stateMachine.AddItem(newItem));
        }

        internal void AddItemInternal(SaleItem newItem)
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
        }

        public ActionResult AddPayment(Payment payment)
        {
            return Balance >= 0
                ? new ActionResult(isSuccess: false, "Cannot add payment to sale with balance equal or greater than 0")
                : ExecuteAction(() => _stateMachine.AddPayment(payment));
        }

        internal void AddPaymentInternal(Payment payment)
        {
            _payments.Add(payment);
            UpdateAmounts();
        }

        public double AmountPaid { get; private set; }

        public double Balance { get; private set; }

        public ActionResult Cancel()
        {
            return PaymentBalance > ChangeGiven
                ? new ActionResult(isSuccess: false, "Cannot cancel sale until payments returned")
                : ExecuteAction(() => _stateMachine.Cancel());
        }

        internal void CancelInternal()
        {
            // Checks done in public method
        }

        public double ChangeGiven { get; private set; }

        public ActionResult DeleteItem(int productId)
        {
            return ExecuteAction(() => _stateMachine.DeleteItem(_saleItems.FirstOrDefault(i => i.Product.Id == productId)));
        }

        internal void DeleteItemInternal(SaleItem item)
        {
            _saleItems.Remove(SaleItems.FirstOrDefault(i => i.Product.Id == item.Product.Id));
            UpdateAmounts();
        }

        public int Id { get; }

        public bool IsCancelled => State == SaleState.Cancelled;

        public bool IsComplete => IsPaid || IsCancelled;

        public bool IsPaid => Math.Abs(Balance) < .001 && _payments.Any() && SaleItems.Any(i => i.Quantity > 0);

        public double PaymentBalance => AmountPaid - ChangeGiven;

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var existingItem = _saleItems.FirstOrDefault(i => i.Product.Id == productId);

            return existingItem is null
                ? new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items.")
                : ExecuteAction(() => _stateMachine.SetItemQuantity(new SaleItem(existingItem.Product, newQuantity)));
        }

        internal void SetItemQuantityInternal(SaleItem item)
        {
            ReplaceItem(_saleItems.FirstOrDefault(i => i.Product.Id == item.Product.Id), item);
            UpdateAmounts();
        }

        public SaleState State { get; internal set; }

        public string Status => State.ToString();

        public double SubTotal { get; set; }

        public IReadOnlyList<SaleItem> SaleItems => _saleItems.AsReadOnly();

        public int TotalItems { get; private set; }

        private static ActionResult ExecuteAction(Action action)
        {
            try
            {
                action.Invoke();
                return new ActionResult(isSuccess: true);
            }
            catch (Exception e)
            {
                return new ActionResult(isSuccess: false, e.Message);
            }
        }

        internal bool HasNegativeBalance()
        {
            return Balance < 0;
        }

        internal bool HasPositiveBalance()
        {
            return Balance > 0;
        }

        internal bool IsCancellable()
        {
            return Math.Abs(PaymentBalance) < 0.001;
        }

        private void ReplaceItem(SaleItem existingItem, SaleItem newItem)
        {
            _saleItems[_saleItems.IndexOf(existingItem)] = newItem;
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
