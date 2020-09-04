using System;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSaleStateManagement.Business
{
    public class Sale
    {
        private readonly List<Change> _change = new List<Change>();
        private readonly List<Payment> _payments = new List<Payment>();
        private readonly IList<SaleItem> _saleItems = new List<SaleItem>();
        private readonly SaleStateMachine _saleStateMachine;

        public Sale(int id)
        {
            Id = id;
            _saleStateMachine = new SaleStateMachine(this);
        }

        public double AmountPaid { get; internal set; }

        public ActionResult AddChange(Change change)
        {
            return ExecuteAction(() => _saleStateMachine.AddChange(change));
        }

        public ActionResult AddItem(SaleItem newItem)
        {
            return ExecuteAction(() => _saleStateMachine.AddItem(newItem));
        }

        public ActionResult AddPayment(Payment payment)
        {
            return ExecuteAction(() => _saleStateMachine.AddPayment(payment));
        }
        public double Balance { get; internal set; }

        public ActionResult Cancel()
        {
            return ExecuteAction(() => _saleStateMachine.Cancel());
        }

        public double ChangeGiven { get; internal set; }

        public ActionResult DeleteItem(int productId)
        {
            return ExecuteAction(() => _saleStateMachine.DeleteItem(_saleItems.FirstOrDefault(i => i.Product.Id == productId)));
        }

        public int Id { get; }

        public bool IsComplete => State == SaleState.Paid || State == SaleState.Cancelled;

        public double PaymentBalance => AmountPaid - ChangeGiven;

        public IReadOnlyList<SaleItem> SaleItems => _saleItems as IReadOnlyList<SaleItem>;

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var existingItem = _saleItems.FirstOrDefault(i => i.Product.Id == productId);

            return existingItem is null 
                ? new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items.") 
                : ExecuteAction(() => _saleStateMachine.SetItemQuantity(new SaleItem(existingItem.Product, newQuantity)));
        }

        public SaleState State { get; internal set; }

        public double SubTotal { get; set; }

        public int TotalItems { get; internal set; }

        internal void AddSaleItemRaw(SaleItem newItem)
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

        internal void AddChangeRaw(Change change)
        {
            _change.Add(change);
            UpdateAmounts();
        }

        internal void AddPaymentRaw(Payment payment)
        {
            _payments.Add(payment);
            UpdateAmounts();
        }

        internal void DeleteSaleItemRaw(SaleItem item)
        {
            _saleItems.Remove(item);
            UpdateAmounts();
        }

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

        internal void ReplaceItem(SaleItem existingItem, SaleItem newItem)
        {
            _saleItems[_saleItems.IndexOf(existingItem)] = newItem;
        }

        internal void SetItemQuantityRaw(SaleItem updatedItem)
        {
            ReplaceItem(SaleItems.First(i => i.Product.Id == updatedItem.Product.Id), updatedItem);
            UpdateAmounts();
        }

        internal void UpdateAmounts()
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
            SubTotal = _saleItems.Sum(i => i.TotalPrice);
        }

        private void UpdateTotalItems()
        {
            TotalItems = _saleItems.Sum(i => i.Quantity);
        }
    }
}
