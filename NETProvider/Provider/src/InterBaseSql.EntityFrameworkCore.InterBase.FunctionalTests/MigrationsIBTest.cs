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

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests
{
	public class MigrationsIBTest : MigrationsTestBase<MigrationsIBFixture>
	{
		public MigrationsIBTest(MigrationsIBFixture fixture)
			: base(fixture)
		{ }

		protected override void GiveMeSomeTime(DbContext db)
		{ }

		protected override Task GiveMeSomeTimeAsync(DbContext db)
			=> Task.CompletedTask;

		[Fact]
		public override void Can_generate_idempotent_up_scripts()
		{
			Assert.Throws<NotSupportedException>(base.Can_generate_idempotent_up_scripts);
		}

		[Fact]
		public override void Can_generate_idempotent_down_scripts()
		{
			Assert.Throws<NotSupportedException>(base.Can_generate_idempotent_down_scripts);
		}

		[Fact(Skip = "")]
		public override void Can_diff_against_2_2_model()
		{ }

		[Fact(Skip = "")]
		public override void Can_diff_against_3_0_ASP_NET_Identity_model()
		{ }

		[Fact(Skip = "")]
		public override void Can_diff_against_2_2_ASP_NET_Identity_model()
		{ }

		[Fact(Skip = "")]
		public override void Can_diff_against_2_1_ASP_NET_Identity_model()
		{ }
	}
}
