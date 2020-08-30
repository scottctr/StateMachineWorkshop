namespace PointOfSaleStateManagement.Data
{
    public class SaleItem
    {
        public SaleItem(Sale sale, Product product, int quantity)
        {
            Sale = sale;
            Product = product;
            Quantity = quantity;
            TotalPrice = product.UnitPrice * quantity;
        }

        public Sale Sale { get; }

        public Product Product { get; }

        public int Quantity { get; }

        public double TotalPrice { get; }
    }
}
