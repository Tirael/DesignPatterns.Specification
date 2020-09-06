using System.Linq;
using DesignPatterns.Specification.Tests.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace DesignPatterns.Specification.Tests
{
    [TestFixture]
    public class SpecTests
    {
        [Test]
        public void HardwareAndRaspberryPi()
        {
            // Arrange
            var rpi = OrderItem.RaspberryPi3ModelB();
            var orangePiOne = OrderItem.OrangePiOne();
            SpecBase<OrderItem> hwSpec = new Spec<OrderItem>(x => x.Type == OrderItem.OrderItemType.Hardware);
            SpecBase<OrderItem> rpiSpec = new Spec<OrderItem>(x => x.Name.ToLower().Contains("raspberry"));

            // Act
            var hwAndRpiSpec = hwSpec & rpiSpec;
            
            // Assert
            hwAndRpiSpec.IsSatisfiedBy(rpi).Should().BeTrue();
            hwAndRpiSpec.IsSatisfiedBy(orangePiOne).Should().BeFalse();
            rpi.Is(hwAndRpiSpec).Should().BeTrue();
            orangePiOne.Is(hwAndRpiSpec).Should().BeFalse();
        }

        [Test]
        public void AnyHardwareAndRpiOrOrange()
        {
            // Arrange
            var rpi = OrderItem.RaspberryPi3ModelB();
            var orangePiOne = OrderItem.OrangePiOne();
            var msOffice = OrderItem.MicrosoftOffice2016();
            SpecBase<OrderItem> hwSpec = new Spec<OrderItem>(x => x.Type == OrderItem.OrderItemType.Hardware);
            SpecBase<OrderItem> rpiSpec = new Spec<OrderItem>(x => x.Name.ToLower().Contains("raspberry"));
            SpecBase<OrderItem> orangeSpec = new Spec<OrderItem>(x => x.Name.ToLower().Contains("orange"));

            // Act
            var anyHwSpecAndRpiOrOrange = hwSpec & (rpiSpec | orangeSpec);
            
            // Assert
            anyHwSpecAndRpiOrOrange.IsSatisfiedBy(rpi).Should().BeTrue();
            anyHwSpecAndRpiOrOrange.IsSatisfiedBy(orangePiOne).Should().BeTrue();
            anyHwSpecAndRpiOrOrange.IsSatisfiedBy(msOffice).Should().BeFalse();
            new[] {rpi, orangePiOne}.Are(anyHwSpecAndRpiOrOrange).Should().BeTrue();
            msOffice.Is(anyHwSpecAndRpiOrOrange).Should().BeFalse();
        }

        [Test]
        public void Any()
        {
            // Arrange
            var rpi = OrderItem.RaspberryPi3ModelB();
            var orangePiOne = OrderItem.OrangePiOne();
            var msOffice = OrderItem.MicrosoftOffice2016();

            // Act/Assert
            new[] {rpi, orangePiOne, msOffice}.Are(Spec.Any<OrderItem>()).Should().BeTrue();
        }

        [Test]
        public void Not()
        {
            // Arrange
            var rpi = OrderItem.RaspberryPi3ModelB();
            var orangePiOne = OrderItem.OrangePiOne();
            var msOffice = OrderItem.MicrosoftOffice2016();

            // Act/Assert
            new[] {rpi, orangePiOne, msOffice}.Are(Spec.Not<OrderItem>()).Should().BeFalse();
        }
    }
}