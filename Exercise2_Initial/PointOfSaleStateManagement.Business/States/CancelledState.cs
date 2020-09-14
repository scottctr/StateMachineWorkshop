using System;

namespace PointOfSaleStateManagement.Business.States
{
    public class CancelledState : FinalizedState
    {
        public CancelledState(Sale context) : base(context, "Cancelled")
        {
            throw new NotImplementedException();
        }

        public override ActionResult AddChange(Change change)
        {
            throw new NotImplementedException();
        }

        public override ActionResult AddItem(SaleItem newItem)
        {
            throw new NotImplementedException();
        }

        public override ActionResult AddPayment(Payment payment)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Cancel()
        {
            throw new NotImplementedException();
        }

        public override ActionResult DeleteItem(int productId)
        {
            throw new NotImplementedException();
        }

        public override ActionResult SetItemQuantity(int productId, int newQuantity)
        {
            throw new NotImplementedException();
        }
    }
}