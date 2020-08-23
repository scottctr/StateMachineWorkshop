using PointOfSaleStateManagement.Data;
using System.Collections.Generic;

namespace PointOfSaleStateManagement.Components
{
    public partial class AddItemDialog
    {
        private List<SaleItem> _saleItems;
        public List<SaleItem> SaleItems
        {
            get
            {
                if (_saleItems is null)
                { InitializeSaleItems(); }

                return _saleItems;
            }
            set
            {
                if (value is null)
                { InitializeSaleItems(); }
                else
                { _saleItems = value; }
            }
        }

        public bool ShowDialog { get; set; }

        public void Show()
        {
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void InitializeSaleItems()
        {
            SaleItems = new List<SaleItem>
            {
                new SaleItem { Quantity = 0, Product = new Product { Id = 1, Name = "Fuel", UnitName = "Gallons", UnitPrice = 2.00 } },
                new SaleItem { Quantity = 0, Product = new Product { Id = 2, Name = "Oil", UnitName = "Cans", UnitPrice = 3.50 } },
                new SaleItem { Quantity = 0, Product = new Product { Id = 3, Name = "Soda", UnitName = "Cans", UnitPrice = 1.00 } },
                new SaleItem { Quantity = 0, Product = new Product { Id = 4, Name = "Chips", UnitName = "Bags", UnitPrice = 1.50 } }
            };
        }
    }
}