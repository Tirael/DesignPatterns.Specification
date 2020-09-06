using System;
using System.Collections.Generic;

namespace DesignPatterns.Specification.Tests.Entities
{
    internal class OrderItem
    {
        public string Name { get; }
        public OrderItemType Type { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }

        public OrderItem(string name, OrderItemType type, int quantity, decimal unitPrice)
        {
            Name = name;
            Type = type;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public enum OrderItemType : byte
        {
            Hardware = 0,
            Software = 1
        }

        internal static OrderItem RaspberryPi3ModelB() =>
            new OrderItem("Raspberry Pi 4 Model B", OrderItemType.Hardware, 1, 40);

        internal static OrderItem OrangePiOne() => new OrderItem("Orange Pi One", OrderItemType.Hardware, 1, 35);

        internal static OrderItem MicrosoftOffice2016() =>
            new OrderItem("Microsoft Office 2016", OrderItemType.Software, 1, 30);

        internal static OrderItem MicrosoftWindows10Professional64bitOEM() =>
            new OrderItem("Microsoft Windows 10 Professional 64-bit OEM", OrderItemType.Software, 1, 150);
    }

    internal class Order
    {
        public IEnumerable<OrderItem> Items { get; }
        public OrderStatus Status { get; }

        public Order(IEnumerable<OrderItem> items, OrderStatus orderStatus)
        {
            Items = items;
            Status = orderStatus;
        }

        public enum OrderStatus : byte
        {
            Created = 0,
            Approved = 1,
            Completed = 2,
            Canceled = 3,
            Revoked = 4
        }

        internal static Order HardwareOrder() =>
            new Order(new[] {OrderItem.RaspberryPi3ModelB(), OrderItem.OrangePiOne()}, OrderStatus.Created);
        internal static Order MicrosoftOfficeOrder() =>
            new Order(new[] {OrderItem.MicrosoftOffice2016(), OrderItem.MicrosoftWindows10Professional64bitOEM()},
                OrderStatus.Approved);
        internal static Order SoftwareOrder() =>
            new Order(new[] {OrderItem.MicrosoftOffice2016(), OrderItem.MicrosoftWindows10Professional64bitOEM()},
                OrderStatus.Canceled);
    }
}