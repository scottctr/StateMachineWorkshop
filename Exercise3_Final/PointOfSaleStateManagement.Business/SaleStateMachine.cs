using NStateManager.Export;
using NStateManager.Sync;
using System;
using System.Diagnostics;

namespace PointOfSaleStateManagement.Business
{
    internal class SaleStateMachine
    {
        private readonly StateMachine<Sale, SaleState, SaleEvent> _stateMachine;
        private readonly Sale _sale;

        public SaleStateMachine(Sale sale)
        {
            _sale = sale;

            _stateMachine = new StateMachine<Sale, SaleState, SaleEvent>(
                stateAccessor: aSale => aSale.State,
                stateMutator: (aSale, newState) => aSale.State = newState);
            ConfigureStateMachine();
        }

        internal void AddChange(Change change)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.GiveChange, change);
        }

        internal void AddItem(SaleItem newItem)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.AddItem, newItem);
        }

        internal void AddPayment(Payment payment)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.Pay, payment);
        }

        internal void Cancel()
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.Cancel);
        }

        internal void DeleteItem(SaleItem item)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.DeleteItem, item);
        }

        internal void SetItemQuantity(SaleItem existingItem)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.SetItemQuantity, existingItem);
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.ConfigureState(SaleState.Open)
                .AddTransition(SaleEvent.Pay, SaleState.Paid, _ => _sale.IsPaid())
                .AddTransition(SaleEvent.Pay, SaleState.Overpaid, condition: _ => _sale.HasPositiveBalance())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Paid, _ => _sale.IsPaid())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Overpaid, condition: _ => _sale.HasPositiveBalance())
                .AddTransition(SaleEvent.GiveChange, SaleState.Paid, _ => _sale.IsPaid())
                .AddTransition(SaleEvent.Cancel, SaleState.Cancelled, _ => _sale.IsCancellable())
                .AddTriggerAction<SaleItem>(SaleEvent.AddItem, (_, item) => _sale.AddSaleItemInternal(item))
                .AddTriggerAction<SaleItem>(SaleEvent.SetItemQuantity, (_, item) => _sale.SetItemQuantityInternal(item))
                .AddTriggerAction<SaleItem>(SaleEvent.DeleteItem, (_, item) => _sale.DeleteSaleItemInternal(item))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (_, payment) => _sale.AddPaymentInternal(payment))
                .AddTriggerAction<Change>(SaleEvent.GiveChange, (_, change) => ProcessChangeRequest(change))
                .AddTriggerAction(SaleEvent.Cancel, sale => ProcessCancelRequest());

            _stateMachine.ConfigureState(SaleState.Overpaid)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Open))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (_, payment) => throw new InvalidOperationException("Cannot pay on an overpaid sale"));

            _stateMachine.ConfigureState(SaleState.Finalized)
                .AddTriggerAction<SaleItem>(SaleEvent.AddItem, (_, item) => throw new InvalidOperationException("Cannot add item to a finalized sale"))
                .AddTriggerAction<SaleItem>(SaleEvent.SetItemQuantity, (_, item) => throw new InvalidOperationException("Cannot change item quantities a finalized sale"))
                .AddTriggerAction<SaleItem>(SaleEvent.DeleteItem, (_, item) => throw new InvalidOperationException("Cannot delete item on a finalized sale"))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (_, payment) => throw new InvalidOperationException("Cannot pay on a finalized sale"))
                .AddTriggerAction<Change>(SaleEvent.GiveChange, (_, change) => throw new InvalidOperationException("Cannot give change on a finalized sale"))
                .AddTriggerAction(SaleEvent.Cancel, sale => throw new InvalidOperationException("Cannot cancel a finalized sale"));

            _stateMachine.ConfigureState(SaleState.Cancelled)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Finalized));

            _stateMachine.ConfigureState(SaleState.Paid)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Finalized));

#if DEBUG
            // This is how to export state configuration to Csv and/or DotGraph
            var configSummary = _stateMachine.GetSummary();
            Debug.WriteLine(CsvExporter<SaleState, SaleEvent>.Export(configSummary));
            Debug.WriteLine(DotGraphExporter<SaleState, SaleEvent>.Export(configSummary));
#endif
        }

        private void ProcessCancelRequest()
        {
            if (_sale.PaymentBalance > 0)
            { throw new InvalidOperationException("Cannot cancel a sale until payments returned"); }
        }

        private void ProcessChangeRequest(Change change)
        {
            if (change.Amount > _sale.PaymentBalance)
            { throw new InvalidOperationException("Change amount cannot exceed payment balance."); }

            _sale.AddChangeInternal(change);
        }
    }
}
