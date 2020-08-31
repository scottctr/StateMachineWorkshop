using System;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSaleStateManagement.Data
{
    public class Sale
    {
        private readonly List<Change> _change = new List<Change>();
        private readonly List<Payment> _payments = new List<Payment>();

        public int Id { get; }
        public double AmountPaid { get; private set; }
        public double ChangeGiven { get; private set; }

        public double SubTotal { get; set; }

        public IList<SaleItem> SaleItems { get; } = new List<SaleItem>();
        public double Balance { get; private set; }
        public int TotalItems { get; private set; }

        public bool IsCancelled { get; private set; }
        public bool IsPaid => Math.Abs(Balance) < .001 && _payments.Any() && SaleItems.Any(i => i.Quantity > 0);
        public bool IsOverpaid => Balance > 0 && !IsCancelled;
        public bool IsOpen => !IsCancelled && !IsPaid && !IsOverpaid;
        public bool IsComplete => IsPaid || IsCancelled;

        public Sale(int id)
        {
            Id = id;
        }

        public ActionResult AddChange(Change change)
        {
            if (IsPaid || IsCancelled)
            { return new ActionResult(isSuccess: false, "Cannot give change on paid or cancelled sale"); }

            _change.Add(change);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        public ActionResult AddItem(SaleItem newItem)
        {
            if (IsPaid || IsCancelled)
            { return new ActionResult(isSuccess: false, "Cannot add items to paid or cancelled sales"); }

            var existingItem = SaleItems.FirstOrDefault(i => i.Product.Id == newItem.Product.Id);

            if (existingItem != null)
            {
                newItem = new SaleItem(newItem.Product, newItem.Quantity + existingItem.Quantity);
                ReplaceItem(existingItem, newItem);
            }
            else
            { SaleItems.Add(newItem); }

            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public ActionResult AddPayment(Payment payment)
        {
            if (IsPaid || IsCancelled)
            { return new ActionResult(isSuccess: false, "Cannot add payment to cancelled or paid sales"); }

            if (Balance >= 0)
            { return new ActionResult(isSuccess: false, "Cannot add payment to sale with balance equal or greater than 0"); }

            _payments.Add(payment);
            UpdateAmounts();
            return new ActionResult(isSuccess: true);
        }

        public ActionResult Cancel()
        {
            if (IsPaid || IsCancelled)
            { return new ActionResult(isSuccess: false, "Cannot cancel paid or cancelled sales"); }

            if (_payments.Sum(p => p.Amount) > _change.Sum(c => c.Amount))
            { return new ActionResult(isSuccess: false, "Cannot cancel sale until payments returned"); }

            IsCancelled = true;
            return new ActionResult(isSuccess: true);
        }

        public ActionResult DeleteItem(int productId)
        {
            if (IsPaid || IsCancelled)
            { return new ActionResult(isSuccess: false, "Cannot delete items from paid or cancelled sales"); }

            SaleItems.Remove(SaleItems.FirstOrDefault(i => i.Product.Id == productId));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            if (IsPaid || IsCancelled)
            { return new ActionResult(isSuccess: false, "Cannot change item quantity on paid or cancelled sales"); }

            var existingItem = SaleItems.FirstOrDefault(i => i.Product.Id == productId);
            if (existingItem is null)
            { return new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items."); }

            ReplaceItem(existingItem, new SaleItem(existingItem.Product, newQuantity));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        private void ReplaceItem(SaleItem existingItem, SaleItem newItem)
        {
            SaleItems[SaleItems.IndexOf(existingItem)] = newItem;
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
