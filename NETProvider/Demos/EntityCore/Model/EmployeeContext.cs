/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    The Initial Developer(s) of the Original Code are listed below.
 *    Portions created by Embarcadero are Copyright (C) Embarcadero.
 *
 *    All Rights Reserved.
 */

//$Authors = Embarcadero, Jeff Overcash

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp3.Model
{
    public partial class EmployeeContext : DbContext
    {
        class LastCommandTextCommandInterceptor : DbCommandInterceptor
        {
            public string LastCommandText { get; private set; }

            public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
            {
                LastCommandText = command.CommandText;
                return base.NonQueryExecuted(command, eventData, result);
            }

            public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
            {
                LastCommandText = command.CommandText;
                return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
            }

            public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
            {
                LastCommandText = command.CommandText;
                return base.ReaderExecuted(command, eventData, result);
            }

            public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
            {
                LastCommandText = command.CommandText;
                return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
            }

            public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
            {
                LastCommandText = command.CommandText;
                return base.ScalarExecuted(command, eventData, result);
            }

            public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
            {
                LastCommandText = command.CommandText;
                return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
            }
        }

        LastCommandTextCommandInterceptor _lastCommandTextInterceptor;

        public DbSet<Employee> Employee { get; set; }

        public EmployeeContext()
        {
            _lastCommandTextInterceptor = new LastCommandTextCommandInterceptor();
        }

        public EmployeeContext(DbContextOptions<EmployeeContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseInterBase("data source=localhost;initial catalog=Employee;user id=sysdba;password=masterkey");
                optionsBuilder.AddInterceptors(_lastCommandTextInterceptor);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Emp_No).HasName("EMP_NO");
                entity.Property(e => e.Emp_No).HasColumnName("EMP_NO");
                entity.Property(e => e.First_Name).HasColumnName("FIRST_NAME");
                entity.Property(e => e.Last_Name).HasColumnName("LAST_NAME");
                entity.Property(e => e.Phone_Ext).HasColumnName("PHONE_EXT");
                entity.Property(e => e.Hire_Date).HasColumnName("HIRE_DATE");
                entity.Property(e => e.Dept_No).HasColumnName("DEPT_NO");
                entity.Property(e => e.Job_Code).HasColumnName("JOB_CODE");
                entity.Property(e => e.Job_Grade).HasColumnName("JOB_GRADE");
                entity.Property(e => e.Job_Country).HasColumnName("JOB_COUNTRY");
                entity.Property(e => e.Salary).HasColumnName("SALARY");
                entity.Property(e => e.Full_Name).HasColumnName("FULL_NAME");

                entity.ToTable("EMPLOYEE");
            });
        }        

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public string LastCommandText => _lastCommandTextInterceptor.LastCommandText;

        public long GetNextSequenceValue(string genName)
        {
            using (var cmd = Database.GetDbConnection().CreateCommand())
            {
                Database.GetDbConnection().Open();
                cmd.CommandText = "SELECT gen_id(" + genName + ", 1) from rdb$database";
                var obj = cmd.ExecuteScalar();
                return (long)obj;
            }
        }
    }
}
