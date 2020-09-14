using NStateManager.Export;
using NStateManager.Sync;
using System;
using System.Diagnostics;

namespace PointOfSaleStateManagement.Business
{
    public class SaleStateMachine
    {
        private readonly StateMachine<Sale, SaleState, SaleEvent> _stateMachine;
        private readonly Sale _sale;

        public SaleStateMachine(Sale sale)
        {
            _sale = sale;
            _stateMachine = new StateMachine<Sale, SaleState, SaleEvent>(
                stateAccessor: sale1 => sale1.State,
                stateMutator: (sale1, state) => sale1.State = state);
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

        internal void DeleteItem(SaleItem saleItem)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.DeleteItem, saleItem);
        }

        internal void SetItemQuantity(SaleItem saleItem)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.SetItemQuantity, saleItem);
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.ConfigureState(SaleState.Open)
                .AddTransition(SaleEvent.Cancel, SaleState.Cancelled, _ => _sale.IsCancellable())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Overpaid, condition: sale => sale.HasPositiveBalance())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Paid, condition: sale => sale.IsPaid)
                .AddTransition(SaleEvent.Pay, SaleState.Overpaid, condition: sale => sale.HasPositiveBalance())
                .AddTransition(SaleEvent.Pay, SaleState.Paid, condition: sale => sale.IsPaid)
                .AddTransition(SaleEvent.GiveChange, SaleState.Paid, condition: sale => sale.IsPaid)
                .AddTriggerAction<SaleItem>(SaleEvent.AddItem, (sale, item) => sale.AddItemInternal(item))
                .AddTriggerAction<SaleItem>(SaleEvent.SetItemQuantity, (sale, item) => sale.SetItemQuantityInternal(item))
                .AddTriggerAction<SaleItem>(SaleEvent.DeleteItem, (sale, item) => sale.DeleteItemInternal(item))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (sale, payment) => sale.AddPaymentInternal(payment))
                .AddTriggerAction<Change>(SaleEvent.GiveChange, (sale, change) => sale.AddChangeInternal(change))
                .AddTriggerAction(SaleEvent.Cancel, sale => sale.CancelInternal());

            _stateMachine.ConfigureState(SaleState.Overpaid)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Open))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (sale, payment) => throw new InvalidOperationException("Cannot pay on an overpaid sale"));

            _stateMachine.ConfigureState(SaleState.Finalized)
                .AddTriggerAction<SaleItem>(SaleEvent.AddItem, (_, item) => throw new InvalidOperationException("Cannot add item to a finalized sale"))
                .AddTriggerAction<SaleItem>(SaleEvent.SetItemQuantity, (_, item) => throw new InvalidOperationException("Cannot change item quantities a finalized sale"))
                .AddTriggerAction<SaleItem>(SaleEvent.DeleteItem, (_, item) => throw new InvalidOperationException("Cannot delete item on a finalized sale"))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (_, payment) => throw new InvalidOperationException("Cannot pay on a finalized sale"))
                .AddTriggerAction<Change>(SaleEvent.GiveChange, (_, change) => throw new InvalidOperationException("Cannot give change on a finalized sale"))
                .AddTriggerAction(SaleEvent.Cancel, sale => throw new InvalidOperationException("Cannot cancel a finalized sale"));

            _stateMachine.ConfigureState(SaleState.Paid)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Finalized));

            _stateMachine.ConfigureState(SaleState.Cancelled)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Finalized));

#if DEBUG
            // This is how to export state configuration to Csv and/or DotGraph
            var configSummary = _stateMachine.GetSummary();
            Debug.WriteLine(CsvExporter<SaleState, SaleEvent>.Export(configSummary));
            Debug.WriteLine(DotGraphExporter<SaleState, SaleEvent>.Export(configSummary));
#endif
        }
    }
}