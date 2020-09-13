using NStateManager.Sync;
using System;
using System.Diagnostics;
using NStateManager.Export;

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

        public void AddChange(Change change)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.GiveChange, change);
        }

        public void AddItem(SaleItem newItem)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.AddItem, newItem);
        }

        public void AddPayment(Payment payment)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.Pay, payment);
        }

        public void Cancel()
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.Cancel);
        }

        public void DeleteItem(SaleItem item)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.DeleteItem, item);
        }

        public void SetItemQuantity(SaleItem existingItem)
        {
            _stateMachine.FireTrigger(_sale, SaleEvent.SetItemQuantity, existingItem);
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.ConfigureState(SaleState.Open)
                .AddTriggerAction<SaleItem>(SaleEvent.AddItem, (sale, item) => _sale.AddSaleItemRaw(item))
                .AddTriggerAction<SaleItem>(SaleEvent.SetItemQuantity, (sale, item) => _sale.SetItemQuantityRaw(item))
                .AddTriggerAction<SaleItem>(SaleEvent.DeleteItem, (sale, item) => _sale.DeleteSaleItemRaw(item))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (sale, payment) => _sale.AddPaymentRaw(payment))
                .AddTriggerAction<Change>(SaleEvent.GiveChange, (sale, change) => ProcessChangeRequest(change))
                .AddTriggerAction(SaleEvent.Cancel, sale => ProcessCancelRequest())
                .AddTransition(SaleEvent.Pay, SaleState.Overpaid, condition: _=> _sale.HasPositiveBalance())
                .AddTransition(SaleEvent.Pay, SaleState.Paid, _=> _sale.IsPaid())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Overpaid, condition: _=> _sale.HasPositiveBalance())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Paid, _ => _sale.IsPaid())
                .AddTransition(SaleEvent.GiveChange, SaleState.Paid, _ => _sale.IsPaid())
                .AddTransition(SaleEvent.Cancel, SaleState.Cancelled, _ => _sale.IsCancellable());

            _stateMachine.ConfigureState(SaleState.Overpaid)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Open))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (sale, payment) => throw new InvalidOperationException("Cannot pay on an overpaid sale"));

            _stateMachine.ConfigureState(SaleState.Finalized)
                .AddTriggerAction<SaleItem>(SaleEvent.AddItem, (sale, item) => throw new InvalidOperationException("Cannot add item to a finalized sale"))
                .AddTriggerAction<SaleItem>(SaleEvent.SetItemQuantity, (sale, item) => throw new InvalidOperationException("Cannot change item quantities a finalized sale"))
                .AddTriggerAction<SaleItem>(SaleEvent.DeleteItem, (sale, item) => throw new InvalidOperationException("Cannot delete item on a finalized sale"))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (sale, payment) => throw new InvalidOperationException("Cannot pay on a finalized sale"))
                .AddTriggerAction<Change>(SaleEvent.GiveChange, (sale, change) => throw new InvalidOperationException("Cannot give change on a finalized sale"))
                .AddTriggerAction(SaleEvent.Cancel, sale => throw new InvalidOperationException("Cannot cancel a finalized sale"));

            _stateMachine.ConfigureState(SaleState.Cancelled)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Finalized));

            _stateMachine.ConfigureState(SaleState.Paid)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Finalized));

#if DEBUG
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

            _sale.AddChangeRaw(change);
        }
    }
}
