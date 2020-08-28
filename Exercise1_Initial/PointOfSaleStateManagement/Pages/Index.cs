using Microsoft.AspNetCore.Components;
using PointOfSaleStateManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSaleStateManagement.Pages
{
    public partial class Index
    {
        private double _changeAmount;
        private double _paymentAmount;
        private readonly List<Product> _productList = new List<Product>();
        private readonly List<LogEntry> _log = new List<LogEntry>();
        private int _saleId;
        private Sale _sale;
        private int _selectedProductId;

        public Index()
        {
            AddLogEntry("Application starting", skipBalance: true);
            InitializeProductList();
            CreateNewSale();
        }

        private void AddItem()
        {
            AddItem(_selectedProductId - 1, quantity: 1);
        }

        private void AddItem(int productId, int quantity)
        {
            var newSaleItem = new SaleItem(_sale, _productList[productId - 1], quantity);

            var result = _sale.AddItem(newSaleItem);
            if (result.WasSuccess)
            {
                SetDefaultAmounts();
                AddLogEntry($"Added a {newSaleItem.Product.SingularUnitName} of {newSaleItem.Product.Name}");
            }
            else
            { LogActionError("Add item", result); }
        }

        private void AddPayment()
        {
            var result = _sale.AddPayment(new Payment(_paymentAmount));
            if (result.WasSuccess)
            {
                SetDefaultChangeAmount();
                AddLogEntry($"Payment: {_paymentAmount:C}");
            }
            else
            { LogActionError("Add payment", result); }
        }

        private void Cancel()
        {
            var result = _sale.Cancel();
            if (result.WasSuccess)
            { AddLogEntry("Sale cancelled"); }
            else
            { LogActionError("Cancel sale", result); }
        }

        private void ChangeItemQuantity(in int productId, int newQuantity)
        {
            var result = _sale.SetItemQuantity(productId, newQuantity);
            if (result.WasSuccess)
            {
                SetDefaultAmounts();
                AddLogEntry($"{_productList[productId-1].Name} quantity changed to {newQuantity}");
            }
            else
            { LogActionError("Change item quantity", result); }
        }

        private void CreateNewSale()
        {
            _sale = new Sale(++_saleId);
            SetDefaultAmounts();
            AddLogEntry("Created new sale", skipBalance: true);
        }

        private void DeleteItem(int productId)
        {
            var result = _sale.DeleteItem(productId);
            if (result.WasSuccess)
            {
                SetDefaultAmounts();
                AddLogEntry($"Deleted {_productList[productId - 1].Name}");
            }
            else
            { LogActionError("Delete item", result); }
        }

        private void GiveChange()
        {
            var result = _sale.AddChange(new Change(_changeAmount));
            if (result.WasSuccess)
            {
                SetDefaultAmounts();
                AddLogEntry($"Change given: {_changeAmount:C}");
            }
            else
            { LogActionError("Give change", result); }
        }

        private void AddLogEntry(string entry, bool skipBalance = false)
        {
            _log.Add(new LogEntry(entry));
            if (!skipBalance)
            { _log.Add(new LogEntry($"   -> New balance: { _sale.Balance:C}")); }
        }

        private void ChangeAmountChanged(ChangeEventArgs args)
        {
            if (double.TryParse(args.Value.ToString(), out var changeAmount))
            { _changeAmount = changeAmount; }
        }

        private void InitializeProductList()
        {
            _productList.Add(new Product(1, "Fuel", "Gallon", "Gallons", 2.00, imageClassName: "gas-pump"));
            _productList.Add(new Product(2, "Oil", "Can", "Cans", 3.50, imageClassName: "oil-can"));
            _productList.Add(new Product(3, "Soda", "Can", "Cans", 1.00, imageClassName: "beer"));
            _productList.Add(new Product(4, "Chips", "Bag", "Bags", 1.50, imageClassName: "cookie"));

            _selectedProductId = _productList.First().Id;
        }

        private void LogActionError(string action, ActionResult result)
        {
            if (!result.WasSuccess)
            { AddLogEntry($"{action} failed - {result.ErrorMessage}", skipBalance: true); }
        }

        private void PaymentAmountChanged(ChangeEventArgs args)
        {
            if (double.TryParse(args.Value.ToString(), out var paymentAmount))
            { _paymentAmount = paymentAmount; }
        }

        private void SetDefaultAmounts()
        {
            SetDefaultPaymentAmount();
            SetDefaultChangeAmount();
        }

        private void SetDefaultChangeAmount()
        {
            _changeAmount = Math.Max(0, _sale.Balance);
        }

        private void SetDefaultPaymentAmount()
        {
            _paymentAmount = Math.Max(0, _sale.Balance * -1);
        }
    }
}