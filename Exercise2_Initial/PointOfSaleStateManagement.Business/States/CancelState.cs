﻿using System;

namespace PointOfSaleStateManagement.Business.States
{
    public class CancelState : SaleStateBase
    {
        public CancelState(Sale context) : base("Cancelled", context, isFinalState: true)
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