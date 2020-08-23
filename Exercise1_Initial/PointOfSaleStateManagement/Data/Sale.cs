using System.Collections.Generic;
using System.Linq;

namespace PointOfSaleStateManagement.Data
{
    public class Sale
    {
        public int Id { get; }
        public IReadOnlyList<SaleItem> SaleItems { get; set; }
        public SaleState State = SaleState.Open;
        public double TotalAmount { get; private set; }
        public int TotalItems { get; private set; }

        public Sale(int id)
        {
            Id = id;
            InitializeSaleItems();
        }

        public void Cancel()
        {
            State = SaleState.Cancelled;
        }

        public void UpdateSaleItem()
        {
            if (SaleItems == null)
            { return; }

            UpdateTotalAmount();
            UpdateTotalItems();
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

        private void UpdateTotalAmount()
        {
            TotalAmount = SaleItems.Sum(i => i.TotalPrice);
        }

        private void UpdateTotalItems()
        {
            TotalItems = SaleItems.Sum(i => i.Quantity);
        }
    }
}
