using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class Job
    {
        public Job()
        {
            Employees = new HashSet<Employee>();
        }

        public string JobCode { get; set; } = null!;
        public short JobGrade { get; set; }
        public string JobCountry { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string? JobRequirement { get; set; }
        public string? LanguageReq { get; set; }

        public virtual Country JobCountryNavigation { get; set; } = null!;
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
