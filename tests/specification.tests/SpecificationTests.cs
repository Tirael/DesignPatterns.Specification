using DesignPatterns.Specification.Tests.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace DesignPatterns.Specification.Tests
{
    [TestFixture]
    public class SpecificationTests
    {
        [Test]
        public void HardwareAndRaspberryPi()
        {
            // Arrange
            var rpi = OrderItem.RaspberryPi3ModelB();
            var orangePiOne = OrderItem.OrangePiOne();
            ISpecification<OrderItem> hwSpec = new Spec<OrderItem>(x => x.Type == OrderItem.OrderItemType.Hardware);
            ISpecification<OrderItem> rpiSpec = new Spec<OrderItem>(x => x.Name.ToLower().Contains("raspberry"));

            // Act
            var hwAndRpiSpec = hwSpec.And(rpiSpec);
            
            // Assert
            hwAndRpiSpec.IsSatisfiedBy(rpi).Should().BeTrue();
            hwAndRpiSpec.IsSatisfiedBy(orangePiOne).Should().BeFalse();
        }
        
        [Test]
        public void AnyHardwareAndRpiOrOrange()
        {
            // Arrange
            var rpi = OrderItem.RaspberryPi3ModelB();
            var orangePiOne = OrderItem.OrangePiOne();
            var msOffice = OrderItem.MicrosoftOffice2016();
            ISpecification<OrderItem> hwSpec = new Spec<OrderItem>(x => x.Type == OrderItem.OrderItemType.Hardware);
            ISpecification<OrderItem> rpiSpec = new Spec<OrderItem>(x => x.Name.ToLower().Contains("raspberry"));
            ISpecification<OrderItem> orangeSpec = new Spec<OrderItem>(x => x.Name.ToLower().Contains("orange"));

            // Act
            var anyHwSpecAndRpiOrOrange = hwSpec.And(rpiSpec.Or(orangeSpec));
            
            // Assert
            anyHwSpecAndRpiOrOrange.IsSatisfiedBy(rpi).Should().BeTrue();
            anyHwSpecAndRpiOrOrange.IsSatisfiedBy(orangePiOne).Should().BeTrue();
            anyHwSpecAndRpiOrOrange.IsSatisfiedBy(msOffice).Should().BeFalse();
        }
        
        [Test]
        public void NotHardware()
        {
            // Arrange
            var rpi = OrderItem.RaspberryPi3ModelB();
            var orangePiOne = OrderItem.OrangePiOne();
            var msOffice = OrderItem.MicrosoftOffice2016();
            ISpecification<OrderItem> hwSpec = new Spec<OrderItem>(x => x.Type == OrderItem.OrderItemType.Hardware);

            // Act
            var notHwSpec = hwSpec.Not();
            
            // Assert
            notHwSpec.IsSatisfiedBy(rpi).Should().BeFalse();
            notHwSpec.IsSatisfiedBy(orangePiOne).Should().BeFalse();
            notHwSpec.IsSatisfiedBy(msOffice).Should().BeTrue();
        }
    }
}