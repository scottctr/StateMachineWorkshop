namespace PointOfSaleStateManagement.Business
{
    //SaleItem is immutable to ensure all changes go through Sale so that we consolidate business rules there
    public class SaleItem
    {
        public SaleItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
            TotalPrice = quantity * product.UnitPrice;
        }

        public Product Product { get; }

        public int Quantity { get; }

        public double TotalPrice { get; }
    }
}
