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
using NUnit.Framework;

namespace EntityFramework.InterBase.Tests
{
	public class InfrastructureTests : EntityFrameworkTestsBase
	{
		[Test]
		public void DbProviderServicesTest()
		{
			object dbproviderservices = GetProviderServices();
			Assert.IsNotNull(dbproviderservices);
			Assert.IsInstanceOf<IBProviderServices>(dbproviderservices);
		}

		[Test]
		public void ProviderManifestTest()
		{
			var manifest = GetProviderServices().GetProviderManifest("foobar");
			Assert.IsNotNull(manifest);
		}

		[Test]
		public void ProviderManifestTokenTest()
		{
			var token = GetProviderServices().GetProviderManifestToken(Connection);
			Assert.IsNotNull(token);
			Assert.IsNotEmpty(token);
			var v = new Version(token);
			Assert.Greater(v.Major, 0);
			Assert.GreaterOrEqual(v.Minor, 0);
			Assert.AreEqual(v.Build, -1);
			Assert.AreEqual(v.Revision, -1);
		}
	}
}
