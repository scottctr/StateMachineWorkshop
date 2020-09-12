using System;

namespace PointOfSaleStateManagement.Business.States
{
    public abstract class SaleStateBase
    {
        // Reference to the context
        protected Sale Sale { get; }

        // Exposing a name for each state is handy for UI and reporting
        public string StatusName { get; }

        // Handy property to help with UI
        public bool IsFinalState { get; }

        protected SaleStateBase(string statusName, Sale context, bool isFinalState)
        {
            StatusName = statusName;
            Sale = context;
            IsFinalState = isFinalState;
        }

        public virtual ActionResult AddChange(Change change)
        {
            throw new NotImplementedException();
        }

        public virtual ActionResult AddItem(SaleItem newItem)
        {
            throw new NotImplementedException();
        }

        public virtual ActionResult AddPayment(Payment payment)
        {
            throw new NotImplementedException();
        }

        public virtual ActionResult Cancel()
        {
            throw new NotImplementedException();
        }

        public virtual ActionResult DeleteItem(int productId)
        {
            throw new NotImplementedException();
        }

        public virtual ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            throw new NotImplementedException();
        }
    }
}