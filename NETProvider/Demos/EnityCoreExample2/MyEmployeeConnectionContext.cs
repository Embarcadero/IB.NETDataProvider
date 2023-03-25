using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFCore101
{
    public partial class MyEmployeeConnectionContext : DbContext
    {
        public MyEmployeeConnectionContext()
        {
        }

        public MyEmployeeConnectionContext(DbContextOptions<MyEmployeeConnectionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Job> Jobs { get; set; } = null!;
        public virtual DbSet<PhoneList> PhoneLists { get; set; } = null!;
        public virtual DbSet<ProjDeptBudget> ProjDeptBudgets { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<SalaryHistory> SalaryHistories { get; set; } = null!;
        public virtual DbSet<Sale> Sales { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseInterBase("data source=localhost;initial catalog=Employee;user id=sysdba;password=masterkey");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Country1)
                    .HasName("RDB$PRIMARY1");

                entity.ToTable("COUNTRY");

                entity.Property(e => e.Country1)
                    .HasMaxLength(15)
                    .HasColumnName("COUNTRY");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .HasColumnName("CURRENCY");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustNo)
                    .HasName("RDB$PRIMARY22");

                entity.ToTable("CUSTOMER");

                entity.HasIndex(e => e.Customer1, "CUSTNAMEX");

                entity.HasIndex(e => new { e.Country, e.City }, "CUSTREGION");

                entity.Property(e => e.CustNo).HasColumnName("CUST_NO");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(30)
                    .HasColumnName("ADDRESS_LINE1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(30)
                    .HasColumnName("ADDRESS_LINE2");

                entity.Property(e => e.City)
                    .HasMaxLength(25)
                    .HasColumnName("CITY");

                entity.Property(e => e.ContactFirst)
                    .HasMaxLength(15)
                    .HasColumnName("CONTACT_FIRST");

                entity.Property(e => e.ContactLast)
                    .HasMaxLength(20)
                    .HasColumnName("CONTACT_LAST");

                entity.Property(e => e.Country)
                    .HasMaxLength(15)
                    .HasColumnName("COUNTRY");

                entity.Property(e => e.Customer1)
                    .HasMaxLength(25)
                    .HasColumnName("CUSTOMER");

                entity.Property(e => e.OnHold)
                    .HasColumnType("CHAR(1)")
                    .HasColumnName("ON_HOLD")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(20)
                    .HasColumnName("PHONE_NO");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(12)
                    .HasColumnName("POSTAL_CODE");

                entity.Property(e => e.StateProvince)
                    .HasMaxLength(15)
                    .HasColumnName("STATE_PROVINCE");

                entity.HasOne(d => d.CountryNavigation)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.Country)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_61");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DeptNo)
                    .HasName("RDB$PRIMARY5");

                entity.ToTable("DEPARTMENT");

                entity.HasIndex(e => e.Budget, "BUDGETX");

                entity.HasIndex(e => e.Department1, "INTEG_15")
                    .IsUnique();

                entity.Property(e => e.DeptNo)
                    .HasColumnType("CHAR(3)")
                    .HasColumnName("DEPT_NO");

                entity.Property(e => e.Budget)
                    .HasColumnType("DECIMAL")
                    .HasColumnName("BUDGET")
                    .HasDefaultValueSql("50000");

                entity.Property(e => e.Department1)
                    .HasMaxLength(25)
                    .HasColumnName("DEPARTMENT");

                entity.Property(e => e.HeadDept)
                    .HasColumnType("CHAR(3)")
                    .HasColumnName("HEAD_DEPT");

                entity.Property(e => e.Location)
                    .HasMaxLength(15)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.MngrNo).HasColumnName("MNGR_NO");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(20)
                    .HasColumnName("PHONE_NO")
                    .HasDefaultValueSql("'555-1234'");

                entity.HasOne(d => d.HeadDeptNavigation)
                    .WithMany(p => p.InverseHeadDeptNavigation)
                    .HasForeignKey(d => d.HeadDept)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_17");

                entity.HasOne(d => d.MngrNoNavigation)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.MngrNo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_31");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpNo)
                    .HasName("RDB$PRIMARY7");

                entity.ToTable("EMPLOYEE");

                entity.HasIndex(e => new { e.LastName, e.FirstName }, "NAMEX");

                entity.Property(e => e.EmpNo).HasColumnName("EMP_NO");

                entity.Property(e => e.DeptNo)
                    .HasColumnType("CHAR(3)")
                    .HasColumnName("DEPT_NO");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(15)
                    .HasColumnName("FIRST_NAME");

                entity.Property(e => e.FullName)
                    .HasMaxLength(37)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("FULL_NAME");

                entity.Property(e => e.HireDate)
                    .HasColumnName("HIRE_DATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.JobCode)
                    .HasMaxLength(5)
                    .HasColumnName("JOB_CODE");

                entity.Property(e => e.JobCountry)
                    .HasMaxLength(15)
                    .HasColumnName("JOB_COUNTRY");

                entity.Property(e => e.JobGrade).HasColumnName("JOB_GRADE");

                entity.Property(e => e.LastName)
                    .HasMaxLength(20)
                    .HasColumnName("LAST_NAME");

                entity.Property(e => e.PhoneExt)
                    .HasMaxLength(4)
                    .HasColumnName("PHONE_EXT");

                entity.Property(e => e.Salary)
                    .HasColumnType("DECIMAL")
                    .HasColumnName("SALARY")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.DeptNoNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DeptNo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_28");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => new { d.JobCode, d.JobGrade, d.JobCountry })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_29");

                entity.HasMany(d => d.Projs)
                    .WithMany(p => p.EmpNos)
                    .UsingEntity<Dictionary<string, object>>(
                        "EmployeeProject",
                        l => l.HasOne<Project>().WithMany().HasForeignKey("ProjId").OnDelete(DeleteBehavior.Restrict).HasConstraintName("INTEG_41"),
                        r => r.HasOne<Employee>().WithMany().HasForeignKey("EmpNo").OnDelete(DeleteBehavior.Restrict).HasConstraintName("INTEG_40"),
                        j =>
                        {
                            j.HasKey("EmpNo", "ProjId").HasName("RDB$PRIMARY14");

                            j.ToTable("EMPLOYEE_PROJECT");

                            j.IndexerProperty<short>("EmpNo").HasColumnName("EMP_NO");

                            j.IndexerProperty<string>("ProjId").HasColumnType("CHAR(5)").HasColumnName("PROJ_ID");
                        });
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => new { e.JobCode, e.JobGrade, e.JobCountry })
                    .HasName("RDB$PRIMARY2");

                entity.ToTable("JOB");

                entity.HasIndex(e => new { e.JobCountry, e.MaxSalary }, "MAXSALX");

                entity.HasIndex(e => new { e.JobCountry, e.MinSalary }, "MINSALX");

                entity.Property(e => e.JobCode)
                    .HasMaxLength(5)
                    .HasColumnName("JOB_CODE");

                entity.Property(e => e.JobGrade).HasColumnName("JOB_GRADE");

                entity.Property(e => e.JobCountry)
                    .HasMaxLength(15)
                    .HasColumnName("JOB_COUNTRY");

                entity.Property(e => e.JobRequirement).HasColumnName("JOB_REQUIREMENT");

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(25)
                    .HasColumnName("JOB_TITLE");

                entity.Property(e => e.LanguageReq)
                    .HasMaxLength(15)
                    .HasColumnName("LANGUAGE_REQ");

                entity.Property(e => e.MaxSalary)
                    .HasColumnType("DECIMAL")
                    .HasColumnName("MAX_SALARY")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.MinSalary)
                    .HasColumnType("DECIMAL")
                    .HasColumnName("MIN_SALARY")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.JobCountryNavigation)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.JobCountry)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_11");
            });

            modelBuilder.Entity<PhoneList>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("PHONE_LIST");

                entity.Property(e => e.EmpNo).HasColumnName("EMP_NO");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(15)
                    .HasColumnName("FIRST_NAME");

                entity.Property(e => e.LastName)
                    .HasMaxLength(20)
                    .HasColumnName("LAST_NAME");

                entity.Property(e => e.Location)
                    .HasMaxLength(15)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.PhoneExt)
                    .HasMaxLength(4)
                    .HasColumnName("PHONE_EXT");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(20)
                    .HasColumnName("PHONE_NO");
            });

            modelBuilder.Entity<ProjDeptBudget>(entity =>
            {
                entity.HasKey(e => new { e.FiscalYear, e.ProjId, e.DeptNo })
                    .HasName("RDB$PRIMARY17");

                entity.ToTable("PROJ_DEPT_BUDGET");

                entity.Property(e => e.FiscalYear).HasColumnName("FISCAL_YEAR");

                entity.Property(e => e.ProjId)
                    .HasColumnType("CHAR(5)")
                    .HasColumnName("PROJ_ID");

                entity.Property(e => e.DeptNo)
                    .HasColumnType("CHAR(3)")
                    .HasColumnName("DEPT_NO");

                entity.Property(e => e.ProjectedBudget)
                    .HasColumnType("DECIMAL")
                    .HasColumnName("PROJECTED_BUDGET")
                    .HasDefaultValueSql("50000");

                entity.Property(e => e.QuartHeadCnt).HasColumnName("QUART_HEAD_CNT");

                entity.HasOne(d => d.DeptNoNavigation)
                    .WithMany(p => p.ProjDeptBudgets)
                    .HasForeignKey(d => d.DeptNo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_47");

                entity.HasOne(d => d.Proj)
                    .WithMany(p => p.ProjDeptBudgets)
                    .HasForeignKey(d => d.ProjId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_48");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.ProjId)
                    .HasName("RDB$PRIMARY12");

                entity.ToTable("PROJECT");

                entity.HasIndex(e => e.ProjName, "INTEG_34")
                    .IsUnique();

                entity.HasIndex(e => new { e.Product, e.ProjName }, "PRODTYPEX")
                    .IsUnique();

                entity.Property(e => e.ProjId)
                    .HasColumnType("CHAR(5)")
                    .HasColumnName("PROJ_ID");

                entity.Property(e => e.Product)
                    .HasMaxLength(12)
                    .HasColumnName("PRODUCT")
                    .HasDefaultValueSql("'software'");

                entity.Property(e => e.ProjDesc).HasColumnName("PROJ_DESC");

                entity.Property(e => e.ProjName)
                    .HasMaxLength(20)
                    .HasColumnName("PROJ_NAME");

                entity.Property(e => e.TeamLeader).HasColumnName("TEAM_LEADER");

                entity.HasOne(d => d.TeamLeaderNavigation)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.TeamLeader)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_36");
            });

            modelBuilder.Entity<SalaryHistory>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.ChangeDate, e.UpdaterId })
                    .HasName("RDB$PRIMARY20");

                entity.ToTable("SALARY_HISTORY");

                entity.HasIndex(e => e.ChangeDate, "CHANGEX");

                entity.HasIndex(e => e.UpdaterId, "UPDATERX");

                entity.Property(e => e.EmpNo).HasColumnName("EMP_NO");

                entity.Property(e => e.ChangeDate)
                    .HasColumnName("CHANGE_DATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdaterId)
                    .HasMaxLength(20)
                    .HasColumnName("UPDATER_ID");

                entity.Property(e => e.NewSalary)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("NEW_SALARY");

                entity.Property(e => e.OldSalary)
                    .HasColumnType("DECIMAL")
                    .HasColumnName("OLD_SALARY")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.PercentChange)
                    .HasColumnName("PERCENT_CHANGE")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.EmpNoNavigation)
                    .WithMany(p => p.SalaryHistories)
                    .HasForeignKey(d => d.EmpNo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_56");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.PoNumber)
                    .HasName("RDB$PRIMARY24");

                entity.ToTable("SALES");

                entity.HasIndex(e => e.DateNeeded, "NEEDX");

                entity.HasIndex(e => new { e.ItemType, e.QtyOrdered }, "QTYX");

                entity.HasIndex(e => new { e.OrderStatus, e.Paid }, "SALESTATX");

                entity.Property(e => e.PoNumber)
                    .HasColumnType("CHAR(8)")
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.Aged)
                    .HasColumnType("BIGINT")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("AGED");

                entity.Property(e => e.CustNo).HasColumnName("CUST_NO");

                entity.Property(e => e.DateNeeded).HasColumnName("DATE_NEEDED");

                entity.Property(e => e.Discount)
                    .HasColumnName("DISCOUNT")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ItemType)
                    .HasMaxLength(12)
                    .HasColumnName("ITEM_TYPE")
                    .HasDefaultValueSql("'software'");

                entity.Property(e => e.OrderDate)
                    .HasColumnName("ORDER_DATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(7)
                    .HasColumnName("ORDER_STATUS")
                    .HasDefaultValueSql("'new'");

                entity.Property(e => e.Paid)
                    .HasColumnType("CHAR(1)")
                    .HasColumnName("PAID")
                    .HasDefaultValueSql("'n'");

                entity.Property(e => e.QtyOrdered)
                    .HasColumnName("QTY_ORDERED")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.SalesRep).HasColumnName("SALES_REP");

                entity.Property(e => e.ShipDate).HasColumnName("SHIP_DATE");

                entity.Property(e => e.TotalValue)
                    .HasColumnType("DECIMAL")
                    .HasColumnName("TOTAL_VALUE");

                entity.HasOne(d => d.CustNoNavigation)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.CustNo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_77");

                entity.HasOne(d => d.SalesRepNavigation)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.SalesRep)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("INTEG_78");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
