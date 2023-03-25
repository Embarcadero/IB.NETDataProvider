using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class Customer
    {
        public Customer()
        {
            Sales = new HashSet<Sale>();
        }

        public int CustNo { get; set; }
        public string Customer1 { get; set; } = null!;
        public string? ContactFirst { get; set; }
        public string? ContactLast { get; set; }
        public string? PhoneNo { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateProvince { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? OnHold { get; set; }

        public virtual Country? CountryNavigation { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
