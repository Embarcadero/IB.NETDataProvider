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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Data;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class IBParameterTests : IBTestsBase
	{
		#region Constructors

		public IBParameterTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region Unit Tests

		[Test]
		public void ConstructorsTest()
		{
			var ctor01 = new IBParameter();
			var ctor02 = new IBParameter("ctor2", 10);
			var ctor03 = new IBParameter("ctor3", IBDbType.Char);
			var ctor04 = new IBParameter("ctor4", IBDbType.Integer, 4);
			var ctor05 = new IBParameter("ctor5", IBDbType.Integer, 4, "int_field");
			var ctor06 = new IBParameter(
				"ctor6",
				IBDbType.Integer,
				4,
				ParameterDirection.Input,
				false,
				0,
				0,
				"int_field",
				DataRowVersion.Original,
				100);

			ctor01 = null;
			ctor02 = null;
			ctor03 = null;
			ctor04 = null;
			ctor05 = null;
			ctor06 = null;
		}

		[Test]
		public void CloneTest()
		{
			var p = new IBParameter("@p1", IBDbType.Integer);
			p.Value = 1;
			p.Charset = IBCharset.Dos850;

			var p1 = ((ICloneable)p).Clone() as IBParameter;

			Assert.AreEqual(p1.ParameterName, p.ParameterName);
			Assert.AreEqual(p1.IBDbType, p.IBDbType);
			Assert.AreEqual(p1.DbType, p.DbType);
			Assert.AreEqual(p1.Direction, p.Direction);
			Assert.AreEqual(p1.SourceColumn, p.SourceColumn);
			Assert.AreEqual(p1.SourceVersion, p.SourceVersion);
			Assert.AreEqual(p1.Charset, p.Charset);
			Assert.AreEqual(p1.IsNullable, p.IsNullable);
			Assert.AreEqual(p1.Size, p.Size);
			Assert.AreEqual(p1.Scale, p.Scale);
			Assert.AreEqual(p1.Precision, p.Precision);
			Assert.AreEqual(p1.Value, p.Value);
		}

		[Test]
		public void FbDbTypeFromEnumAsValueTest()
		{
			var p = new IBParameter();
			p.Value = IBServerType.Embedded;
			Assert.AreEqual(IBDbType.Integer, p.IBDbType);
		}

		[Test]
		public void FbDbTypeFromDBNullAsValueTest()
		{
			var p = new IBParameter();
			p.Value = DBNull.Value;
			Assert.AreEqual(IBDbType.VarChar, p.IBDbType);
		}


		#endregion
	}

	public class IBParameterTestsDialect1 : IBParameterTests
	{
		public IBParameterTestsDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1;
		}
	}


}
