namespace PointOfSaleStateManagement.Data
{
    //SaleItem is immutable to ensure all changes go through Sale so that we consolidate business rules there
    public class SaleItem
    {
        public SaleItem(Sale sale, Product product, int quantity)
        {
            Sale = sale;
            Product = product;
            Quantity = quantity;
            TotalPrice = quantity * product.UnitPrice;
        }

        public Sale Sale { get; }

        public Product Product { get; }

        public int Quantity { get; }

        public double TotalPrice { get; }
    }
}
