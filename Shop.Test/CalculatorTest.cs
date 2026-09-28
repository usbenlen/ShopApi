using MyCalculator;

namespace Shop.Test;

public class CalculatorTest
{
    [Fact]
    public void SumTest()
    {
        //A - Arrange
        int a = 10, b = 20;
        Calculator calculator = new Calculator();

        //A - Act
        int result = calculator.Sum(a, b);

        //Assert
        Assert.Equal(30, result);
    }

    [Fact]
    public void SubtractTest()
    {
        int a = 20, b = 10;
        Calculator calculator = new Calculator();

        int result = calculator.Subtract(a, b);

        Assert.Equal(10, result);
    }

    [Fact]
    public void MultiplyTest()
    {
        int a = 10, b = 20;
        Calculator calculator = new Calculator();

        int result = calculator.Multiply(a, b);

        Assert.Equal(200, result);
    }
}
