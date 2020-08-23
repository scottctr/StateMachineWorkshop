using PointOfSaleStateManagement.Data;

namespace PointOfSaleStateManagement.Pages
{
    public partial class Index
    {
        private int _saleId;
        private Sale _sale;

        public Index()
        {
            CreateNewSale();
        }

        public void Cancel()
        {
            _sale.Cancel();
        }

        public void CreateNewSale()
        {
            _sale = new Sale(++_saleId);
        }
    }
}