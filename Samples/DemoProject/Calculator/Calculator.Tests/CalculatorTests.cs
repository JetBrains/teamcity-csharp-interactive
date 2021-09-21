using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void ShouldAdd()
        {
            // Given
            var calculator = new Calculator();

            // When
            var result = calculator.Add(1, 2);

            // Then
            Assert.AreEqual(3, result);
        }
    }
}