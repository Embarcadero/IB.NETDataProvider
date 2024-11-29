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
using System.Globalization;
using System.Threading;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[NoServerCategory]
public class IBParameterCollectionTests
{
	[Test]
	public void AddTest()
	{
		var command = new IBCommand();

		command.Parameters.Add(new IBParameter("@p292", 10000));
		command.Parameters.Add("@p01", IBDbType.Integer);
		command.Parameters.Add("@p02", 289273);
		command.Parameters.Add("#p3", IBDbType.SmallInt, 2, "sourceColumn");
	}

	[Test]
	public void DNET532_CheckCultureAwareIndexOf()
	{
		var curCulture = Thread.CurrentThread.CurrentCulture;
		try
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
			var command = new IBCommand();
			// \u0131 is turkish symbol "i without dot" that uppercases to "I" symbol.
			// see https://msdn.microsoft.com/en-us/library/ms973919.aspx#stringsinnet20_topic5 for more information
			var parameterName = "Turkish\u0131Parameter";
			command.Parameters.Add(parameterName, IBDbType.Char);
			Assert.AreNotEqual(-1, command.Parameters.IndexOf("turkishIParameter"));
		}
		finally
		{
			Thread.CurrentThread.CurrentCulture = curCulture;
		}
	}

	[Test]
	public void DNET532_CheckFlagForUsingOrdinalIgnoreCase()
	{
		var command = new IBCommand();
		command.Parameters.IndexOf("SomeField");

		for (var i = 0; i < 100; ++i)
		{
			command.Parameters.Add("FIELD" + i.ToString(), IBDbType.Integer);
		}

		const string probeParameterName = "FIELD0";
		const int noMatterValue = 12345;
		const int deleteIndex = 12;
		command.Parameters[probeParameterName].Value = noMatterValue;
		Assert.IsFalse(command.Parameters.HasParameterWithNonAsciiName);

		command.Parameters.Remove(command.Parameters[deleteIndex]);
		command.Parameters[probeParameterName].Value = noMatterValue;

		command.Parameters.RemoveAt(deleteIndex);
		command.Parameters[probeParameterName].Value = noMatterValue;

		command.Parameters.Insert(deleteIndex, new IBParameter("FIELD101", IBDbType.Integer));
		command.Parameters[probeParameterName].Value = noMatterValue;

		command.Parameters.Clear();
	}

	[Test]
	public void DNET532_CheckFlagForUsingOrdinalIgnoreCaseWithOuterChanges()
	{
		var collection = new IBParameterCollection();
		var parameter = new IBParameter() { ParameterName = "test" };
		collection.Add(parameter);
		var dummy1 = collection.IndexOf("dummy");
		Assert.IsFalse(collection.HasParameterWithNonAsciiName);
		parameter.ParameterName = "řčšřčšřčš";
		var dummy2 = collection.IndexOf("dummy");
		Assert.IsTrue(parameter.IsUnicodeParameterName);
		Assert.IsTrue(collection.HasParameterWithNonAsciiName);
	}

	[Test]
	public void CheckIBParameterParentPropertyInvariant()
	{
		var collection = new IBParameterCollection();
		var parameter = collection.Add("Name", IBDbType.Array);
		Assert.AreEqual(collection, parameter.Parent);
		Assert.Throws<ArgumentException>(() => collection.Add(parameter));
		Assert.Throws<ArgumentException>(() => collection.AddRange(new IBParameter[] { parameter }));

		collection.Remove(parameter);
		Assert.IsNull(parameter.Parent);

		Assert.Throws<ArgumentException>(() => collection.Remove(parameter));

		collection.Insert(0, parameter);
		Assert.AreEqual(collection, parameter.Parent);
		Assert.Throws<ArgumentException>(() => collection.Insert(0, parameter));
	}

	[Test]
	public void DNET635_ResetsParentOnClear()
	{
		var collection = new IBParameterCollection();
		var parameter = collection.Add("test", 0);
		Assert.IsNotNull(parameter.Parent);
		collection.Clear();
		Assert.IsNull(parameter.Parent);
	}
	// Note this does not need a Dialect 1 version of the tests as no DB is involved.
}