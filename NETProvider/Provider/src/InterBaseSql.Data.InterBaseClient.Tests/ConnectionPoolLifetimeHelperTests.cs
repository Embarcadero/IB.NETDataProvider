﻿/*
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

//$Authors = @realic, Jiri Cincura (jiri@cincura.net)

using InterBaseSql.Data.Common;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

public class ConnectionPoolLifetimeHelperTests
{
	[Test]
	public void IsAliveTrueIfLifetimeNotExceed()
	{
		var now = 1_000_000;
		var timeAgo = now - (10 * 1000);
		var isAlive = ConnectionPoolLifetimeHelper.IsAlive(20, timeAgo, now);
		Assert.IsTrue(isAlive);
	}

	[Test]
	public void IsAliveFalseIfLifetimeIsExceed()
	{
		var now = 1_000_000;
		var timeAgo = now - (30 * 1000);
		var isAlive = ConnectionPoolLifetimeHelper.IsAlive(20, timeAgo, now);
		Assert.IsFalse(isAlive);
	}
}
