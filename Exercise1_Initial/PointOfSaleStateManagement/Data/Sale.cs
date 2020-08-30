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

        public Sale(int id)
        {
            Id = id;
        }

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
                newItem = new SaleItem(this, newItem.Product, newItem.Quantity + existingItem.Quantity);
                ReplaceItem(existingItem, newItem);
            }
            else
            { SaleItems.Add(newItem); }

            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public ActionResult AddPayment(Payment payment)
        {
            _payments.Add(payment);
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public ActionResult Cancel()
        {
            return new ActionResult(isSuccess: false, "Not implemented");
        }

        public ActionResult DeleteItem(int productId)
        {
            SaleItems.Remove(SaleItems.FirstOrDefault(i => i.Product.Id == productId));
            UpdateAmounts();

            return new ActionResult(isSuccess: true);
        }

        public bool IsComplete => true; //TODO Sale only complete when paid or cancelled

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            var existingItem = SaleItems.FirstOrDefault(i => i.Product.Id == productId);

            if (existingItem is null)
            { return new ActionResult(isSuccess: false, $"ProductId {productId} not found in sale items."); }

            ReplaceItem(existingItem, new SaleItem(this, existingItem.Product, newQuantity));
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
