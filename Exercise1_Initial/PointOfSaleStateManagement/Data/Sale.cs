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

        public IReadOnlyList<SaleItem> SaleItems { get; set; }
        public double Balance { get; private set; }
        public int TotalItems { get; private set; }

        public Sale(int id)
        {
            Id = id;
            InitializeSaleItems();
        }

        public void AddChange(Change change)
        {
            _change.Add(change);
            UpdateAmounts();
        }

        public void AddPayment(Payment payment)
        {
            _payments.Add(payment);
            UpdateAmounts();
        }

        public void Cancel()
        {
            //!!!
        }

        public void UpdateSaleItem()
        {
            if (SaleItems == null)
            { return; }

            UpdateAmounts();
        }

        private void InitializeSaleItems()
        {
            SaleItems = new List<SaleItem>
            {
                new SaleItem { Sale = this, Quantity = 0, Product = new Product { Id = 1, Name = "Fuel", UnitName = "Gallons", UnitPrice = 2.00 } },
                new SaleItem { Sale = this, Quantity = 0, Product = new Product { Id = 2, Name = "Oil", UnitName = "Cans", UnitPrice = 3.50 } },
                new SaleItem { Sale = this, Quantity = 0, Product = new Product { Id = 3, Name = "Soda", UnitName = "Cans", UnitPrice = 1.00 } },
                new SaleItem { Sale = this, Quantity = 0, Product = new Product { Id = 4, Name = "Chips", UnitName = "Bags", UnitPrice = 1.50 } }
            };
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
