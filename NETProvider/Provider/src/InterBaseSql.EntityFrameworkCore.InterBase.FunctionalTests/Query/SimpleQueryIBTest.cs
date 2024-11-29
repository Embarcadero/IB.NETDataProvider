/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/raw/master/license.txt.
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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class SimpleQueryIBTest : SimpleQueryRelationalTestBase
{
	protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

	[DoesNotHaveTheDataTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task StoreType_for_UDF_used(bool async)
	{
		return base.StoreType_for_UDF_used(async);
	}

	[Theory(Skip = "Not interesting for InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Multiple_different_entity_type_from_different_namespaces(bool async)
	{
		return base.Multiple_different_entity_type_from_different_namespaces(async);
	}

	[HasDataInTheSameTransactionAsDDLTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Multiple_nested_reference_navigations(bool async)
	{
		return base.Multiple_nested_reference_navigations(async);
	}

	[HasDataInTheSameTransactionAsDDLTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Comparing_byte_column_to_enum_in_vb_creating_double_cast(bool async)
	{
		return base.Comparing_byte_column_to_enum_in_vb_creating_double_cast(async);
	}

	[HasDataInTheSameTransactionAsDDLTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Comparing_enum_casted_to_byte_with_int_constant(bool async)
	{
		return base.Comparing_enum_casted_to_byte_with_int_constant(async);
	}

	[HasDataInTheSameTransactionAsDDLTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Comparing_enum_casted_to_byte_with_int_parameter(bool async)
	{
		return base.Comparing_enum_casted_to_byte_with_int_parameter(async);
	}

	[HasDataInTheSameTransactionAsDDLTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Null_check_removal_in_ternary_maintain_appropriate_cast(bool async)
	{
		return base.Null_check_removal_in_ternary_maintain_appropriate_cast(async);
	}

	public override async Task Aggregate_over_subquery_in_group_by_projection(bool async)
	{
		var contextFactory = await InitializeAsync<IBContext27083>(seed: c => c.Seed());
		using var context = contextFactory.CreateContext();

		Expression<Func<Order, bool>> someFilterFromOutside = x => x.Number != "A1";

		var query = context
			.Set<Order>()
			.Where(someFilterFromOutside)
			.GroupBy(x => new { x.CustomerId, x.Number })
			.Select(
				x => new
				{
					x.Key.CustomerId,
					CustomerMinHourlyRate = context.Set<Order>().Where(n => n.CustomerId == x.Key.CustomerId).Min(h => h.HourlyRate),
					HourlyRate = x.Min(f => f.HourlyRate),
					Count = x.Count()
				});

		var orders = async
			? await query.ToListAsync()
			: query.ToList();

		Assert.Collection(
			orders.OrderBy(x => x.CustomerId),
			t =>
			{
				Assert.Equal(1, t.CustomerId);
				Assert.Equal(10, t.CustomerMinHourlyRate);
				Assert.Equal(11, t.HourlyRate);
				Assert.Equal(1, t.Count);
			},
			t =>
			{
				Assert.Equal(2, t.CustomerId);
				Assert.Equal(20, t.CustomerMinHourlyRate);
				Assert.Equal(20, t.HourlyRate);
				Assert.Equal(1, t.Count);
			});
	}

	protected class IBContext27083 : DbContext
	{
		public IBContext27083(DbContextOptions options)
			: base(options)
		{
		}

		public DbSet<TimeSheet> TimeSheets { get; set; }
		public DbSet<Customer> Customers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			ModelHelpers.SetStringLengths(modelBuilder);
			ModelHelpers.SetPrimaryKeyGeneration(modelBuilder);	
		}

		public void Seed()
		{
			var customerA = new Customer { Name = "Customer A" };
			var customerB = new Customer { Name = "Customer B" };

			var projectA = new Project { Customer = customerA };
			var projectB = new Project { Customer = customerB };

			var orderA1 = new Order
			{
				Number = "A1",
				Customer = customerA,
				HourlyRate = 10
			};
			var orderA2 = new Order
			{
				Number = "A2",
				Customer = customerA,
				HourlyRate = 11
			};
			var orderB1 = new Order
			{
				Number = "B1",
				Customer = customerB,
				HourlyRate = 20
			};

			var timeSheetA = new TimeSheet { Order = orderA1, Project = projectA };
			var timeSheetB = new TimeSheet { Order = orderB1, Project = projectB };

			AddRange(customerA, customerB);
			AddRange(projectA, projectB);
			AddRange(orderA1, orderA2, orderB1);
			AddRange(timeSheetA, timeSheetB);
			SaveChanges();
		}
	}

}
