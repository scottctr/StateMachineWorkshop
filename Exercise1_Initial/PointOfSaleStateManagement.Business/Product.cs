namespace PointOfSaleStateManagement.Business
{
    public class Product
    {
        public Product(int id, string name, string singularUnitName, string pluralUnitName, double unitPrice, string imageClassName)
        {
            Id = id;
            Name = name;
            SingularUnitName = singularUnitName;
            PluralUnitName = pluralUnitName;
            UnitPrice = unitPrice;
            ImageClassName = imageClassName;
        }

        public int Id { get; }
        public string Name { get; }
        public string SingularUnitName { get; }
        public string PluralUnitName { get; }
        public double UnitPrice { get; }
        public string ImageClassName { get; }

        public string GetUnitName(int quantity)
        {
            return quantity == 1 ? SingularUnitName : PluralUnitName;
        }
    }
}
