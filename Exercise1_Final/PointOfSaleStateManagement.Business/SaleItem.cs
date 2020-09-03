namespace PointOfSaleStateManagement.Business
{
    public class SaleItem
    {
        public SaleItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
            TotalPrice = product.UnitPrice * quantity;
        }

        public Product Product { get; }

        public int Quantity { get; }

        public double TotalPrice { get; }
    }
}
