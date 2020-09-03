using Microsoft.AspNetCore.Components;
using PointOfSaleStateManagement.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PointOfSaleStateManagement.Business;

[assembly: InternalsVisibleTo("PointOfSaleStateManagement.Tests")]
namespace PointOfSaleStateManagement.Pages
{
    public partial class Index
    {
        private double _changeAmount;
        private double _paymentAmount;
        private readonly List<Product> _productList = new List<Product>();
        private readonly List<LogEntry> _log = new List<LogEntry>();
        private int _saleId;
        internal Sale Sale;

        public Index()
        {
            AddLogEntry("Application starting", skipBalance: true);
            InitializeProductList();
            StartNewSale();
        }

        internal void AddItem(int productId, int quantity)
        {
            var newSaleItem = new SaleItem(_productList[productId - 1], quantity);

            var result = Sale.AddItem(newSaleItem);
            if (result.IsSuccess)
            {
                AddLogEntry($"Added {newSaleItem.Quantity} {newSaleItem.Product.GetUnitName(newSaleItem.Quantity)} of {newSaleItem.Product.Name}");
                SetDefaultAmounts();
            }
            else
            { LogActionError("Add item", result); }
        }

        internal void AddPayment()
        {
            AddPayment(_paymentAmount);
        }

        internal void AddPayment(double paymentAmount)
        {
            var result = Sale.AddPayment(new Payment(paymentAmount));
            if (result.IsSuccess)
            {
                AddLogEntry($"Payment: {_paymentAmount:C}");
                SetDefaultChangeAmount();
            }
            else
            { LogActionError("Add payment", result); }
        }

        private void Cancel()
        {
            var result = Sale.Cancel();
            if (result.IsSuccess)
            { AddLogEntry("Sale cancelled"); }
            else
            { LogActionError("Cancel sale", result); }
        }

        private void ChangeItemQuantity(in int productId, int newQuantity)
        {
            var result = Sale.SetItemQuantity(productId, newQuantity);
            if (result.IsSuccess)
            {
                AddLogEntry($"{_productList[productId-1].Name} quantity changed to {newQuantity}");
                SetDefaultAmounts();
            }
            else
            { LogActionError("Change item quantity", result); }
        }

        private void DeleteItem(int productId)
        {
            var result = Sale.DeleteItem(productId);
            if (result.IsSuccess)
            {
                AddLogEntry($"Deleted {_productList[productId - 1].Name}");
                SetDefaultAmounts();
            }
            else
            { LogActionError("Delete item", result); }
        }

        private void GiveChange()
        {
            var result = Sale.AddChange(new Change(_changeAmount));
            if (result.IsSuccess)
            {
                AddLogEntry($"Change given: {_changeAmount:C}");
                SetDefaultAmounts();
            }
            else
            { LogActionError("Give change", result); }
        }

        private void AddLogEntry(string entry, bool skipBalance = false)
        {
            _log.Add(new LogEntry(entry));
            if (!skipBalance)
            { _log.Add(new LogEntry($"   -> Balance: { Sale.Balance:C}")); }
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
        }

        private void LogActionError(string action, ActionResult result)
        {
            if (!result.IsSuccess)
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
            _changeAmount = Math.Max(0, Sale.Balance);
        }

        private void SetDefaultPaymentAmount()
        {
            _paymentAmount = Math.Max(0, Sale.Balance * -1);
        }

        internal ActionResult StartNewSale()
        {
            if (Sale != null && !Sale.IsComplete)
            { return new ActionResult(isSuccess: false, "Current sale must be Paid or Cancelled before starting new sale"); }

            if (_saleId > 0)
            { AddLogEntry($"<<< Sale {_saleId} Completed >>>", skipBalance: true); }

            Sale = new Sale(++_saleId);
            SetDefaultAmounts();
            AddLogEntry($">>> Started new sale {_saleId} <<<");

            return new ActionResult(isSuccess: true);
        }
    }
}