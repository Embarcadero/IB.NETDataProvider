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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Data.Common;
using System.Linq;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query
{
	public class FromSqlQueryIBTest : FromSqlQueryTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
	{
		public FromSqlQueryIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
			: base(fixture)
		{ }

		[NotSupportedOnInterBaseFact]
		public override void Bad_data_error_handling_invalid_cast_key()
		{
			base.Bad_data_error_handling_invalid_cast_key();
		}

		[NotSupportedOnInterBaseFact]
		public override void Bad_data_error_handling_invalid_cast_no_tracking()
		{
			base.Bad_data_error_handling_invalid_cast_no_tracking();
		}

		[NotSupportedOnInterBaseFact]
		public override void Bad_data_error_handling_invalid_cast_projection()
		{
			base.Bad_data_error_handling_invalid_cast_projection();
		}

		[NotSupportedOnInterBaseFact]
		public override void Bad_data_error_handling_invalid_cast()
		{
			base.Bad_data_error_handling_invalid_cast();
		}

		[DoesNotHaveTheDataFact]
		public override void FromSqlRaw_queryable_simple_projection_composed()
		{
			base.FromSqlRaw_queryable_simple_projection_composed();
		}

		protected override DbParameter CreateDbParameter(string name, object value)
			=> new IBParameter
			{
				ParameterName = name,
				Value = value
			};
	}
}
