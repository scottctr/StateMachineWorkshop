﻿using PointOfSaleStateManagement.Business;
using System.Linq;
using Xunit;

namespace PointOfSaleStateManagement.Tests
{
    public class SaleTests
    {
        [Fact]
        public void Test_1a_items_can_be_added()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            var item2 = new SaleItem(new Product(2, "product2", "product2", "product2s", 2.22, imageClassName: "test"), 2);

            var result1 = sut.AddItem(item1);
            var result2 = sut.AddItem(item2);

            Assert.True(result1.IsSuccess, "Unable to add item to sale");
            Assert.True(result2.IsSuccess, "Unable to add 2nd item to sale");
            Assert.Equal(2, sut.SaleItems.Count);
            Assert.Equal(item1, sut.SaleItems.First());
            Assert.Equal(item2, sut.SaleItems.Last());
        }

        [Fact]
        public void Test_1a_can_add_items_when_over_paid()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            var item2 = new SaleItem(new Product(2, "product2", "product2", "product2s", 2.22, imageClassName: "test"), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);
            sut.AddPayment(new Payment(4.44));

            var result = sut.AddItem(item1);

            Assert.True(result.IsSuccess, "Was not able to add items to over paid sale");
            Assert.Equal(4, sut.TotalItems);
        }

        [Fact]
        public void Test_1b_item_quantity_can_be_changed()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);

            var result = sut.SetItemQuantity(item1.Product.Id, 2);

            Assert.True(result.IsSuccess, "Unable to change item quantity");
            Assert.Equal(2, sut.SaleItems.First().Quantity);
            Assert.Equal(1, sut.SaleItems.Count);
        }

        [Fact]
        public void Test_1c_item_can_be_deleted()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);

            var result = sut.DeleteItem(item1.Product.Id);

            Assert.True(result.IsSuccess, "Unable to delete item");
            Assert.False(sut.SaleItems.Any());
        }

        [Fact]
        public void Test_2_can_pay()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);

            var result = sut.AddPayment(new Payment(1.11));

            Assert.True(result.IsSuccess, "Unable to pay");
            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Test_3_can_give_change()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(10));

            var result = sut.AddChange(new Change(9.00));

            Assert.True(result.IsSuccess, "Unable to give change");
            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Test_5_cannot_add_payment_when_balance_0()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(item1.TotalPrice));

            var result = sut.AddPayment(new Payment(10));

            Assert.False(result.IsSuccess, "Was able to add payment on paid sale");
            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Test_5_cannot_add_payment_when_balance_greater_than_0()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(10));

            var result = sut.AddPayment(new Payment(999));

            Assert.False(result.IsSuccess, "Was able to add payment to overpaid sale");
            Assert.Equal(9, sut.Balance);
        }

        [Fact]
        public void Test_6_can_give_change_when_amount_equals_payment_balance()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(50));

            var result = sut.AddChange(new Change(50));

            Assert.True(result.IsSuccess, "Unable to give change when change amount equals payment balance");
            Assert.Equal(item1.TotalPrice * -1, sut.Balance);
        }

        [Fact]
        public void Test_6_cannot_give_change_when_balance_0()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(item1.TotalPrice));

            var result = sut.AddChange(new Change(9.00));

            Assert.False(result.IsSuccess, "Was able to give change on a paid sale");
            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Test_6_cannot_give_change_when_payment_balance_0()
        {
            var sut = new Sale(1);

            var result = sut.AddChange(new Change(1.00));

            Assert.False(result.IsSuccess, "Able to add change to sale with negative balance");
            Assert.Equal(0, sut.ChangeGiven);
            Assert.Equal(0, sut.Balance);
            Assert.Equal(0, sut.PaymentBalance);
        }

        [Fact]
        public void Test_6_cannot_give_change_when_amount_exceeds_payment_balance()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(50));
            var expectedBalance = sut.Balance;

            var result = sut.AddChange(new Change(100));

            Assert.False(result.IsSuccess, "Able to add change to sale with negative balance");
            Assert.Equal(expectedBalance, sut.Balance);
        }

        [Fact]
        public void Test_7_cannot_add_item_to_paid_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(item1.TotalPrice));

            var result = sut.AddItem(new SaleItem(new Product(2, "product2", "product2", "product2s", 2.00, imageClassName: "test2"), 2));

            Assert.False(result.IsSuccess, "Was able to add item to paid sale");
            Assert.Equal(0, sut.Balance);
            Assert.Equal(1, sut.SaleItems.Count);
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Test_7_cannot_change_item_quantity_on_paid_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(item1.TotalPrice));

            var result = sut.SetItemQuantity(item1.Product.Id, 999);

            Assert.False(result.IsSuccess, "Was able to change item quantity on paid sale");
            Assert.Equal(0, sut.Balance);
            Assert.Equal(1, sut.SaleItems.Count);
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Test_7_cannot_delete_item_to_paid_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(item1.TotalPrice));

            var result = sut.DeleteItem(item1.Product.Id);

            Assert.False(result.IsSuccess, "Was able to delete item on paid sale");
            Assert.Equal(0, sut.Balance);
            Assert.Equal(1, sut.SaleItems.Count);
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Test_7_cannot_pay_on_paid_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(item1.TotalPrice));

            var result = sut.AddPayment(new Payment(item1.TotalPrice));

            Assert.False(result.IsSuccess, "Was able to pay on paid sale");
            Assert.Equal(0, sut.Balance);
            Assert.Equal(item1.TotalPrice, sut.AmountPaid);
        }

        [Fact]
        public void Test_7_cannot_give_change_on_paid_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(item1.TotalPrice));

            var result = sut.AddChange(new Change(5.55));

            Assert.False(result.IsSuccess, "Was able to give change on paid sale");
            Assert.Equal(0, sut.Balance);
            Assert.Equal(0, sut.ChangeGiven);
        }

        [Fact]
        public void Test_8_can_cancel()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            var item2 = new SaleItem(new Product(2, "product2", "product2", "product2s", 2.22, imageClassName: "test"), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);

            var result = sut.Cancel();

            Assert.True(result.IsSuccess, "Was unable to cancel open sale");
            Assert.True(sut.IsCancelled, "Successfully cancelled sale not marked cancelled");
        }

        [Fact]
        public void Test_8a_cannot_cancel_sale_with_payment_balance()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 10.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(5));

            var result = sut.Cancel();

            Assert.False(result.IsSuccess, "Cancelled sale with a payment balance");
            Assert.False(sut.IsCancelled, "Successfully cancelled sale not marked cancelled");
        }

        [Fact]
        public void Test_8b_change_amount_cannot_exceed_payment_balance()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 10.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(5));

            var result = sut.AddChange(new Change(6.00));

            Assert.False(result.IsSuccess, "Added change that exceeds payment balance");
            Assert.Equal(0, sut.ChangeGiven);
        }

        [Fact]
        public void Test_8c_cannot_add_items_on_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.AddItem(new SaleItem(new Product(2, "product2", "product2", "product2s", 2.22, "test2"), 2));

            Assert.False(result.IsSuccess, "Added items to cancelled sale");
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Test_8c_cannot_change_item_quantity_on_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.SetItemQuantity(item1.Product.Id, 123);

            Assert.False(result.IsSuccess, "Changed item quantity on cancelled sale");
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Test_8c_cannot_delete_item_on_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.DeleteItem(item1.Product.Id);

            Assert.False(result.IsSuccess, "Deleted item on cancelled sale");
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Test_8c_cannot_pay_on_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.AddPayment(new Payment(10));

            Assert.False(result.IsSuccess, "Added payment to cancelled sale");
            Assert.Equal(0, sut.AmountPaid);
        }

        [Fact]
        public void Test_8c_cannot_give_change_to_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.AddChange(new Change(1.00));

            Assert.False(result.IsSuccess, "Gave change on cancelled sale");
            Assert.Equal(0, sut.ChangeGiven);
        }

        [Fact]
        public void Test_8c_cannot_cancel_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.Cancel();

            Assert.False(result.IsSuccess, "Cancelled a previously cancelled sale");
        }
    }
}