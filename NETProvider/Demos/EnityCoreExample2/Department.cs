using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
            InverseHeadDeptNavigation = new HashSet<Department>();
            ProjDeptBudgets = new HashSet<ProjDeptBudget>();
        }

        public string DeptNo { get; set; } = null!;
        public string Department1 { get; set; } = null!;
        public string? HeadDept { get; set; }
        public short? MngrNo { get; set; }
        public decimal? Budget { get; set; }
        public string? Location { get; set; }
        public string? PhoneNo { get; set; }

        public virtual Department? HeadDeptNavigation { get; set; }
        public virtual Employee? MngrNoNavigation { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Department> InverseHeadDeptNavigation { get; set; }
        public virtual ICollection<ProjDeptBudget> ProjDeptBudgets { get; set; }
    }
}
