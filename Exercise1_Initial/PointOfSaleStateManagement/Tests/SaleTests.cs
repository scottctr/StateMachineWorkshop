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
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22), 2);

            sut.AddItem(item1);
            sut.AddItem(item2);

            Assert.Equal(2, sut.SaleItems.Count);
            Assert.Equal(item1, sut.SaleItems.First());
            Assert.Equal(item2, sut.SaleItems.Last());
        }

        [Fact]
        public void Can_pay()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);

            sut.AddPayment(new Payment(3.33));

            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Can_cancel()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);

            sut.Cancel();

            //!!!!Assert
        }

        [Fact]
        public void Can_give_change_when_over_paid()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);
            sut.AddPayment(new Payment(10));

            sut.AddChange(new Change(6.67));

            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Can_add_items_when_over_paid()
        {
            var sut = new Sale(1);
            var item1 = new SaleItem(sut, new Product(1, "product1", "product1", "product1s", 1.11), 1);
            var item2 = new SaleItem(sut, new Product(2, "product2", "product2", "product2s", 2.22), 2);
            sut.AddItem(item1);
            sut.AddItem(item2);
            sut.AddPayment(new Payment(4.44));

            sut.AddItem(item1);

            Assert.Equal(0, sut.Balance);
        }

        [Fact]
        public void Cannot_modify_items_on_paid_sale_with_0_balance()
        {
            //!!! how to remove item or change quantity???
            //!!!
        }

        [Fact]
        public void Cannot_pay_on_paid_sale_with_0_balance()
        {
            //!!!
        }

        [Fact]
        public void Cannot_cancel_paid_sale_with_0_balance()
        {
            //!!!
        }

        [Fact]
        public void Cannot_pay_paid_sale_with_0_balance()
        {
            //!!!
        }

        [Fact]
        public void Cannot_modify_items_to_cancelled_sale()
        {
            //!!!
        }

        [Fact]
        public void Cannot_give_change_to_cancelled_sale()
        {
            //!!!
        }

        [Fact]
        public void Cannot_cannot_cancelled_sale()
        {
            //!!!
        }

        [Fact]
        public void Cannot_give_change_with_negative_balance()
        {
            //!!!
        }
    }
}