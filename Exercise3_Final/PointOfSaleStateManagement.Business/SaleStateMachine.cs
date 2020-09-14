using NStateManager.Export;
using NStateManager.Sync;
using System;
using System.Diagnostics;

namespace PointOfSaleStateManagement.Business
{
    public class SaleStateMachine
    {
        // StateMachine requires type for context, states, events
        // -- Note that there is a StateMachineAsync if your solution needs to make async calls
        private readonly StateMachine<Sale, SaleState, SaleEvent> _stateMachine;
        private readonly Sale _sale;

        public SaleStateMachine(Sale sale)
        {
            _sale = sale;
            _stateMachine = new StateMachine<Sale, SaleState, SaleEvent>(
                stateAccessor: sale1 => sale1.State,                    // stateAccessor = Function to read context's current state
                stateMutator: (sale1, state) => sale1.State = state);   // stateMutator = Action to update the context's current state
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
            // Use builder pattern to configure each state
            _stateMachine.ConfigureState(SaleState.Open)
                // Add possible transitions
                .AddTransition(SaleEvent.Cancel, SaleState.Cancelled, condition: sale => sale.IsCancellable())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Overpaid, condition: sale => sale.HasPositiveBalance())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Paid, condition: sale => sale.IsPaid)
                .AddTransition(SaleEvent.Pay, SaleState.Overpaid, condition: sale => sale.HasPositiveBalance())
                .AddTransition(SaleEvent.Pay, SaleState.Paid, condition: sale => sale.IsPaid)
                .AddTransition(SaleEvent.GiveChange, SaleState.Paid, condition: sale => sale.IsPaid)
                // Add trigger actions -- how to process requests passed from Sale
                // - Include parameter type where required
                .AddTriggerAction<SaleItem>(SaleEvent.AddItem, (sale, item) => sale.AddItemInternal(item))
                .AddTriggerAction<SaleItem>(SaleEvent.SetItemQuantity, (sale, item) => sale.SetItemQuantityInternal(item))
                .AddTriggerAction<SaleItem>(SaleEvent.DeleteItem, (sale, item) => sale.DeleteItemInternal(item))
                .AddTriggerAction<Payment>(SaleEvent.Pay, (sale, payment) => sale.AddPaymentInternal(payment))
                .AddTriggerAction<Change>(SaleEvent.GiveChange, (sale, change) => sale.AddChangeInternal(change))
                .AddTriggerAction(SaleEvent.Cancel, sale => sale.CancelInternal());

            _stateMachine.ConfigureState(SaleState.Overpaid)
                .MakeSubstateOf(_stateMachine.ConfigureState(SaleState.Open)) // This inherits all of Open's configuration unless overridden
                // Overpaid can go back to Open if the balance goes negative
                .AddTransition(SaleEvent.GiveChange, SaleState.Open, condition: sale => sale.HasNegativeBalance())
                .AddTransition(SaleEvent.AddItem, SaleState.Open, condition: sale => sale.HasNegativeBalance())
                .AddTransition(SaleEvent.SetItemQuantity, SaleState.Open, condition: sale => sale.HasNegativeBalance())
                // Can't add payments when customer over paid
                .AddTriggerAction<Payment>(SaleEvent.Pay, (sale, payment) => throw new InvalidOperationException("Cannot pay on an overpaid sale"));

            _stateMachine.ConfigureState(SaleState.Finalized)
                // This is the parent class for any finalized state -- Paid and Cancelled
                // -- No actions or transitions allowed
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
            // Plug the output of the DotGraphExporter into https://dreampuf.github.io/GraphvizOnline to see graph of current configuration
#endif
        }
    }
}