using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class Country
    {
        public Country()
        {
            Customers = new HashSet<Customer>();
            Jobs = new HashSet<Job>();
        }

        public string Country1 { get; set; } = null!;
        public string Currency { get; set; } = null!;

        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
