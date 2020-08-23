using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSaleStateManagement.Data;

namespace PointOfSaleStateManagement.Pages
{
    public partial class Index
    {
        private double _changeAmount;
        private double _paymentAmount;
        private int _saleId;
        private Sale _sale;

        public Index()
        {
            CreateNewSale();
        }

        private void AddPayment()
        {
            _sale.AddPayment(new Payment(_paymentAmount));
        }

        private void Cancel()
        {
            _sale.Cancel();
        }

        private void CreateNewSale()
        {
            _sale = new Sale(++_saleId);
        }

        private void GiveChange()
        {
            _sale.AddChange(new Change(_changeAmount));
        }

        private void ChangeAmountChanged(ChangeEventArgs args)
        {
            if (double.TryParse(args.Value.ToString(), out var changeAmount))
            { _changeAmount = changeAmount; }
        }

        private void PaymentAmountChanged(ChangeEventArgs args)
        {
            if (double.TryParse(args.Value.ToString(), out var paymentAmount))
            { _paymentAmount = paymentAmount; }
        }
    }
}