using System;
using System.Collections.Generic;

namespace EFCore101
{
    public partial class ProjDeptBudget
    {
        public int FiscalYear { get; set; }
        public string ProjId { get; set; } = null!;
        public string DeptNo { get; set; } = null!;
        public int? QuartHeadCnt { get; set; }
        public decimal? ProjectedBudget { get; set; }

        public virtual Department DeptNoNavigation { get; set; } = null!;
        public virtual Project Proj { get; set; } = null!;
    }
}
