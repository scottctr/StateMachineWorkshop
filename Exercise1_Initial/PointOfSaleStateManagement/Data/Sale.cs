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

        public bool IsOpen => !IsOverpaid && !IsCancelled && !IsPaid;
        public bool IsOverpaid => Balance > 0;
        public bool IsCancelled { get; private set; }
        public bool IsPaid => Math.Abs(Balance) < 0.001 && SaleItems.Count > 0 && !IsCancelled;

        public double SubTotal { get; set; }

        public IList<SaleItem> SaleItems { get; } = new List<SaleItem>();
        public double Balance { get; private set; }
        public int TotalItems { get; private set; }

        public Sale(int id)
        {
            Id = id;
        }

        public ActionResult AddChange(Change change)
        {
            _change.Add(change);
            UpdateAmounts();

            return new ActionResult(wasSuccess: true);
        }

        public ActionResult AddItem(SaleItem item)
        {
            var selectedItem = SaleItems.FirstOrDefault(i => i.Product.Id == item.Product.Id);
            if (selectedItem != null)
            { selectedItem.Quantity += item.Quantity; }
            else
            { SaleItems.Add(item); }

            UpdateAmounts();

            return new ActionResult(wasSuccess: true);
        }

        public ActionResult AddPayment(Payment payment)
        {
            _payments.Add(payment);
            UpdateAmounts();

            return new ActionResult(wasSuccess: true);
        }

        public ActionResult Cancel()
        {
            IsCancelled = true;

            return new ActionResult(wasSuccess: true);
        }

        public ActionResult DeleteItem(int productId)
        {
            SaleItems.Remove(SaleItems.FirstOrDefault(i => i.Product.Id == productId));
            UpdateAmounts();

            return new ActionResult(wasSuccess: true);
        }

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var saleItem = SaleItems.FirstOrDefault(i => i.Product.Id == productId);
            if (saleItem == null)
            { return new ActionResult(wasSuccess: false, $"ProductId {productId} not found in sale items."); }

            saleItem.Quantity = newQuantity;
            UpdateAmounts();

            return new ActionResult(wasSuccess: true);
        }

        public ActionResult UpdateSaleItem()
        {
            UpdateAmounts();

            return new ActionResult(wasSuccess: true);
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
