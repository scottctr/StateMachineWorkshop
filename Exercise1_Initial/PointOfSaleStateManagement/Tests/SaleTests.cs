using PointOfSaleStateManagement.Data;
using System.Linq;
using Xunit;

namespace PointOfSaleStateManagement.Tests
{
    public class SaleTests
    {
        [Fact]
        public void Items_can_be_added_to_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22, imageClassName: "test"), 2);

            var result1 = sut.AddItem(item1);
            var result2 = sut.AddItem(item2);

            Assert.True(result1.WasSuccess);
            Assert.True(result2.WasSuccess);
            Assert.Equal(2, sut.SaleItems.Count);
            Assert.Equal(item1, sut.SaleItems.First());
            Assert.Equal(item2, sut.SaleItems.Last());
        }

        [Fact]
        public void Can_pay()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);

            var result = sut.AddPayment(new Payment(1.11));

            Assert.True(result.WasSuccess);
            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Can_cancel()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22, imageClassName: "test"), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);

            var result = sut.Cancel();

            Assert.True(result.WasSuccess);
            Assert.True(sut.IsCancelled);
        }

        [Fact]
        public void Can_give_change_when_over_paid()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.00, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(10));

            var result = sut.AddChange(new Change(9.00));

            Assert.True(result.WasSuccess);
            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Can_add_items_when_over_paid()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22, imageClassName: "test"), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);
            sut.AddPayment(new Payment(4.44));

            var result = sut.AddItem(item1);

            Assert.True(result.WasSuccess);
            Assert.Equal(4, sut.TotalItems);
        }

        [Fact]
        public void Cannot_modify_items_on_paid_sale_with_0_balance()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(1.11));

            var result = sut.AddItem(item1);

            Assert.False(result.WasSuccess);
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Cannot_pay_on_paid_sale_with_0_balance()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(1.11));

            var result = sut.AddPayment(new Payment(1.11));

            Assert.False(result.WasSuccess);
            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Cannot_cancel_paid_sale_with_0_balance()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.AddPayment(new Payment(1.11));

            var result = sut.Cancel();

            Assert.False(result.WasSuccess);
            Assert.True(sut.IsCancelled);
        }

        [Fact]
        public void Cannot_modify_items_on_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.AddItem(item1);

            Assert.False(result.WasSuccess);
            Assert.Equal(1, sut.TotalItems);
        }

        [Fact]
        public void Cannot_give_change_to_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.AddChange(new Change(1.00));

            Assert.False(result.WasSuccess);
            Assert.Equal(0, sut.ChangeGiven);
        }

        [Fact]
        public void Cannot_cancel_cancelled_sale()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11, imageClassName: "test"), 1);
            sut.AddItem(item1);
            sut.Cancel();

            var result = sut.Cancel();

            Assert.False(result.WasSuccess);
        }
    }
}