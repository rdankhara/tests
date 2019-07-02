using EquityComponents;
using EquityComponents.Services;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class EquityOrderTests
    {
        EquityOrder _equityOrder;
        Mock<IOrderService> _orderService;
        Instrument _instrument;

        [SetUp]
        public void Setup()
        {
            _orderService = new Mock<IOrderService>();
            _instrument = new Instrument("MIC", 100, 100);
            _equityOrder = new EquityOrder(_orderService.Object, _instrument);

        }

        [Test]
        public void GivenPriceGoesBelowThresholdThenItPlacesOrder()
        {
            //Arrange

            //Act
            _equityOrder.ReceiveTick("MIC", 99.99M);

            //Assert
            _orderService.Verify(x => x.Buy("MIC", 100, 99.99M), Times.Once);
        }

        [Test]
        public void ItShutDownsAfterPlacingOneOrder()
        {
            //Arrange

            //Act
            Parallel.Invoke(
            () => _equityOrder.ReceiveTick("MIC", 99.99M),
            () => _equityOrder.ReceiveTick("MIC", 99.98M),
            () => _equityOrder.ReceiveTick("MIC", 99.97M));

            //Assert
            //Assert with exact parameters only once
            _orderService.Verify(x => x.Buy("MIC", 100, 99.99M), Times.Once);
            _orderService.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public void ItSignalsOrderPlacedHandler()
        {
            bool isOrderNotified = false;
            _equityOrder.OrderPlaced += e => isOrderNotified = true ;

            _equityOrder.ReceiveTick("MIC", 99.99M);
            Assert.IsTrue(isOrderNotified);
        }

        [Test]
        public void ItSignalsAnyErrorWhilePlacingOrder()
        {
            //Arrange
            bool isOrderPlaced = false;
            bool isErrorOccured = false;

            _orderService.Setup(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
                .Throws(new System.Exception());

            _equityOrder.OrderErrored += e => isErrorOccured = true;
            _equityOrder.OrderPlaced += e => isOrderPlaced = true;

            //Act
            _equityOrder.ReceiveTick("MIC", 99.98M);

            //Assert
            Assert.IsTrue(isErrorOccured);
            Assert.IsFalse(isOrderPlaced);
        }

        [Test]
        public void ItShutDownsAfterAnyErrorOccures()
        {
            int orderCount = 0;
            int errorCount = 0;
            _orderService.Setup(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
                .Throws(new System.Exception());

            _equityOrder.OrderErrored += e => errorCount++;
            _equityOrder.OrderPlaced += e => orderCount++;

            _equityOrder.ReceiveTick("MIC", 99.98M);
            _equityOrder.ReceiveTick("MIC", 99.97M);

            Assert.AreEqual(0, orderCount);
            Assert.AreEqual(1, errorCount);
        }
    }
}