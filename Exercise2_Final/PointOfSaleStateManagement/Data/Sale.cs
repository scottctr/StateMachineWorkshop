using System.Collections.Generic;
using PointOfSaleStateManagement.Data.States;

namespace PointOfSaleStateManagement.Data
{
    public class Sale
    {
        internal readonly List<Change> Change = new List<Change>();
        internal readonly List<Payment> Payments = new List<Payment>();
        private SaleStateBase _state;

        public int Id { get; }
        public double AmountPaid { get; internal set; }
        public double ChangeGiven { get; internal set; }

        public double SubTotal { get; set; }

        public IList<SaleItem> SaleItems { get; } = new List<SaleItem>();
        public double Balance { get; internal set; }
        public int TotalItems { get; internal set; }

        public Sale(int id)
        {
            Id = id;
            TransitionTo(new OpenState(this));
        }

        public ActionResult AddChange(Change change)
        {
            return _state.AddChange(change);
        }

        public ActionResult AddItem(SaleItem newItem)
        {
            return _state.AddItem(newItem);
        }

        public ActionResult AddPayment(Payment payment)
        {
            return _state.AddPayment(payment);
        }

        public ActionResult Cancel()
        {
            return _state.Cancel();
        }

        public ActionResult DeleteItem(int productId)
        {
            return _state.DeleteItem(productId);
        }

        public bool IsComplete => _state.IsFinalState;

        public ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            return _state.SetItemQuantity(productId, newQuantity);
        }

        public string Status => _state.StatusName;

        internal void TransitionTo(SaleStateBase newState)
        {
            _state = newState;
        }
    }
}