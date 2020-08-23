using System;

namespace PointOfSaleStateManagement.Data
{
    public class SaleItem
    {
        public Sale Sale { get; set; }

        public Product Product { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = Math.Max(0, value);

                if (Product is null || Quantity == 0)
                { TotalPrice = 0; }
                else 
                { TotalPrice = Quantity * Product.UnitPrice; }

                Sale.UpdateSaleItem();
            }
        }

        public double TotalPrice { get; set; }
    }
}
