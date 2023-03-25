using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class Sale
    {
        public string PoNumber { get; set; } = null!;
        public int CustNo { get; set; }
        public short? SalesRep { get; set; }
        public string OrderStatus { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? DateNeeded { get; set; }
        public string? Paid { get; set; }
        public int QtyOrdered { get; set; }
        public decimal TotalValue { get; set; }
        public float Discount { get; set; }
        public string? ItemType { get; set; }
        public long? Aged { get; set; }

        public virtual Customer CustNoNavigation { get; set; } = null!;
        public virtual Employee? SalesRepNavigation { get; set; }
    }
}
