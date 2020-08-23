using Microsoft.AspNetCore.Components;
using PointOfSaleStateManagement.Data;
using System;
using System.Collections.Generic;

namespace PointOfSaleStateManagement.Pages
{
    public partial class Index
    {
        private double _changeAmount;
        private double _paymentAmount;
        private readonly List<LogEntry> _log = new List<LogEntry>();
        private int _saleId;
        private Sale _sale;

        public Index()
        {
            AddLogEntry("Application starting", skipBalance: true);

            CreateNewSale();
        }

        private void AddLogEntry(string entry, bool skipBalance = false)
        {
            _log.Add(new LogEntry(entry));
            if (!skipBalance)
            { _log.Add(new LogEntry($"   -> New balance: { _sale.Balance:C}")); }
        }

        private void AddPayment()
        {
            _sale.AddPayment(new Payment(_paymentAmount));
            AddLogEntry($"Payment: {_paymentAmount:C}");

            SetChangeAmount();
        }

        private void Cancel()
        {
            _sale.Cancel();
            AddLogEntry("Sale cancelled");
        }

        private void CreateNewSale()
        {
            _sale = new Sale(++_saleId);
            AddLogEntry("Created new sale", skipBalance: true);
        }

        private void GiveChange()
        {
            _sale.AddChange(new Change(_changeAmount));
            AddLogEntry($"Change given: {_changeAmount:C}");
        }

        private void ChangeAmountChanged(ChangeEventArgs args)
        {
            if (double.TryParse(args.Value.ToString(), out var changeAmount))
            { _changeAmount = changeAmount; }
        }

        private void ItemQuantityChanged(object newValue, SaleItem saleItem)
        {
            if (!int.TryParse(newValue.ToString(), out var newQuantity))
            { return; }

            saleItem.Quantity = newQuantity;
            AddLogEntry($"{saleItem.Product.Name} quantity changed to {saleItem.Quantity} (@{saleItem.Product.UnitPrice:C}/{saleItem.Product.UnitName})");

            _paymentAmount = Math.Max(0, _sale.Balance * -1); //DIY!!!
            SetChangeAmount();
        }

        private void PaymentAmountChanged(ChangeEventArgs args)
        {
            if (double.TryParse(args.Value.ToString(), out var paymentAmount))
            { _paymentAmount = paymentAmount; }
        }

        private void SetChangeAmount()
        {
            _changeAmount = Math.Max(0, _sale.Balance);
        }
    }
}