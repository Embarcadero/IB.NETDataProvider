using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class Project
    {
        public Project()
        {
            ProjDeptBudgets = new HashSet<ProjDeptBudget>();
            EmpNos = new HashSet<Employee>();
        }

        public string ProjId { get; set; } = null!;
        public string ProjName { get; set; } = null!;
        public string? ProjDesc { get; set; }
        public short? TeamLeader { get; set; }
        public string? Product { get; set; }

        public virtual Employee? TeamLeaderNavigation { get; set; }
        public virtual ICollection<ProjDeptBudget> ProjDeptBudgets { get; set; }

        public virtual ICollection<Employee> EmpNos { get; set; }
    }
}
