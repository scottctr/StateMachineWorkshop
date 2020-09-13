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

        public Sale(int id)
        {
            Id = id;
        }
        
        public double AmountPaid { get; private set; }

        public ActionResult AddChange(Change change)
        {
            _change.Add(change);
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }


        public ActionResult AddItem(SaleItem newItem)
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
            _payments.Add(payment);
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public double Balance { get; private set; }

        public ActionResult Cancel()
        {
            return new ActionResult(isSuccess: false, "Not implemented");
        }

        public double ChangeGiven { get; private set; }

        public ActionResult DeleteItem(int productId)
        {
            _saleItems.Remove(SaleItems.FirstOrDefault(i => i.Product.Id == productId));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public int Id { get; }

        public bool IsComplete => true; //TODO Sale only complete when paid or cancelled

        public double PaymentBalance => AmountPaid - ChangeGiven;

        public IReadOnlyList<SaleItem> SaleItems => _saleItems.AsReadOnly();

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var existingItem = SaleItems.FirstOrDefault(i => i.Product.Id == productId);

            if (existingItem is null)
            { return new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items."); }

            ReplaceItem(existingItem, new SaleItem(existingItem.Product, newQuantity));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public string Status { get; private set; }

        public double SubTotal { get; set; }

        public int TotalItems { get; private set; }

        internal bool HasPositiveBalance()
        {
            return Balance > 0;
        }

        internal bool IsCancellable()
        {
            return Math.Abs(PaymentBalance) < 0.001;
        }

        internal bool IsPaid()
        {
            return Math.Abs(Balance) < 0.001 && SaleItems.Any() && AmountPaid > 0;
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
