using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class Employee
    {
        public Employee()
        {
            Departments = new HashSet<Department>();
            Projects = new HashSet<Project>();
            SalaryHistories = new HashSet<SalaryHistory>();
            Sales = new HashSet<Sale>();
            Projs = new HashSet<Project>();
        }

        public short EmpNo { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneExt { get; set; }
        public DateTime HireDate { get; set; }
        public string DeptNo { get; set; } = null!;
        public string JobCode { get; set; } = null!;
        public short JobGrade { get; set; }
        public string JobCountry { get; set; } = null!;
        public decimal Salary { get; set; }
        public string? FullName { get; set; }

        public virtual Department DeptNoNavigation { get; set; } = null!;
        public virtual Job Job { get; set; } = null!;
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<SalaryHistory> SalaryHistories { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }

        public virtual ICollection<Project> Projs { get; set; }
    }
}
