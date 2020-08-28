namespace PointOfSaleStateManagement.Data
{
    public class Product
    {
        public Product(int id, string name, string singularUnitName, string pluralUnitName, double unitPrice)
        {
            Id = id;
            Name = name;
            SingularUnitName = singularUnitName;
            PluralUnitName = pluralUnitName;
            UnitPrice = unitPrice;
        }

        public int Id { get; }
        public string Name { get; }
        public string SingularUnitName { get; }
        public string PluralUnitName { get; }
        public double UnitPrice { get; }
    }
}
