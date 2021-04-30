﻿/*
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

using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers
{
	public class HasDataInTheSameTransactionAsDDLFactAttribute : FactAttribute
	{
		public HasDataInTheSameTransactionAsDDLFactAttribute()
		{
			Skip = "HasData is called in the same transaction as DDL commands.";
		}
	}
	public class HasDataInTheSameTransactionAsDDLTheoryAttribute : TheoryAttribute
	{
		public HasDataInTheSameTransactionAsDDLTheoryAttribute()
		{
			Skip = "HasData is called in the same transaction as DDL commands.";
		}
	}

#warning Can I somehow handle it in SQL generation?
	public class GeneratedNameTooLongFactAttribute : FactAttribute
	{
		public GeneratedNameTooLongFactAttribute()
		{
			Skip = "Generated name in the query is too long.";
		}
	}
	public class GeneratedNameTooLongTheoryAttribute : TheoryAttribute
	{
		public GeneratedNameTooLongTheoryAttribute()
		{
			Skip = "Generated name in the query is too long.";
		}
	}

	public class NotSupportedOnInterBaseFactAttribute : FactAttribute
	{
		public NotSupportedOnInterBaseFactAttribute()
		{
			Skip = "Not supported on InterBase.";
		}
	}
	public class NotSupportedOnInterBaseTheoryAttribute : TheoryAttribute
	{
		public NotSupportedOnInterBaseTheoryAttribute()
		{
			Skip = "Not supported on InterBase.";
		}
	}

	public class DoesNotHaveTheDataFactAttribute : FactAttribute
	{
		public DoesNotHaveTheDataFactAttribute()
		{
			Skip = "Does not have the data.";
		}
	}
	public class DoesNotHaveTheDataTheoryAttribute : TheoryAttribute
	{
		public DoesNotHaveTheDataTheoryAttribute()
		{
			Skip = "Does not have the data.";
		}
	}
}
