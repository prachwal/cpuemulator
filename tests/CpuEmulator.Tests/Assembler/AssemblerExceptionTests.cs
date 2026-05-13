using CpuEmulator.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpuEmulator.Tests.Assembler
{
    [TestClass]
    public class AssemblerExceptionTests
    {
        [TestMethod]
        public void AssemblerException_ShouldStoreLineAndColumn()
        {
            // Arrange
            var line = 42;
            var column = 10;
            var message = "Test error message";

            // Act
            var exception = new AssemblerException(message, line, column);

            // Assert
            exception.Line.Should().Be(line);
            exception.Column.Should().Be(column);
            exception.Message.Should().Be(message);
        }

        [TestMethod]
        public void AssemblerException_ShouldBeException()
        {
            // Arrange
            var exception = new AssemblerException("Test", 1, 1);

            // Act & Assert
            exception.Should().BeAssignableTo<Exception>();
        }
    }
}