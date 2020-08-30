using PointOfSaleStateManagement.Pages;
using System.Linq;
using Xunit;

namespace PointOfSaleStateManagement.Tests
{
    public class IndexTests
    {
        [Fact]
        public void Test_4_can_start_new_sale()
        {
            //Covered by Test_9
        }

        [Fact]
        public void Test_9_cannot_start_new_sale_when_current_sale_open()
        {
            var sut = new Index();
            sut.AddItem(productId: 1, quantity: 1);

            var result = sut.StartNewSale();

            Assert.False(result.IsSuccess, "Started a new sale when current sale was open");
        }

        [Fact]
        public void Test_9_cannot_start_new_sale_when_current_sale_overpaid()
        {
            var sut = new Index();
            sut.AddItem(productId: 1, quantity: 1);
            sut.AddPayment(sut.Sale.SaleItems.First().TotalPrice * 2);

            var result = sut.StartNewSale();

            Assert.False(result.IsSuccess, "Started a new sale when current sale was overpaid");
        }

        [Fact]
        public void Test_9_can_start_new_sale_when_current_sale_paid()
        {
            // expected to fail until exercise complete

            var sut = new Index();
            sut.AddItem(productId: 1, quantity: 1);
            sut.AddPayment(sut.Sale.SaleItems.First().TotalPrice);

            var result = sut.StartNewSale();

            Assert.True(result.IsSuccess, "Unable to start a new sale when current sale paid");
        }

        [Fact]
        public void Test_9_can_start_new_sale_when_current_sale_cancelled()
        {
            // expected to fail until exercise complete

            var sut = new Index();
            sut.AddItem(productId: 1, quantity: 1);
            sut.Sale.Cancel();

            var result = sut.StartNewSale();

            Assert.True(result.IsSuccess, "Was unable to start a new sale when current sale cancelled");
        }
    }
}
