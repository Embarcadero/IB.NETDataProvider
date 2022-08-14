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

using System.Security.Cryptography;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class IBBlobTests : IBTestsBase
	{
		#region Constructors

		public IBBlobTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region Unit Tests

		[Test]
		public void BinaryBlobTest()
		{
			var id_value = GetId();

			var selectText = "SELECT blob_field FROM TEST WHERE int_field = " + id_value.ToString();
			var insertText = "INSERT INTO TEST (int_field, blob_field) values(@int_field, @blob_field)";

			// Generate an array of temp data
                #if NET6_0_OR_GREATER
			byte[] insert_values =  RandomNumberGenerator.GetBytes(100000 * 4);
                #else
			var insert_values = new byte[100000 * 4];
			var rng = new RNGCryptoServiceProvider();
			rng.GetBytes(insert_values);
                #endif

			// Execute insert command
			var transaction = Connection.BeginTransaction();

			var insert = new IBCommand(insertText, Connection, transaction);
			insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
			insert.Parameters.Add("@blob_field", IBDbType.Binary).Value = insert_values;
			insert.ExecuteNonQuery();

			transaction.Commit();

			// Check that inserted values are correct
			var select = new IBCommand(selectText, Connection);
			var select_values = (byte[])select.ExecuteScalar();

			for (var i = 0; i < insert_values.Length; i++)
			{
				if (insert_values[i] != select_values[i])
				{
					Assert.Fail("differences at index " + i.ToString());
				}
			}
		}

		[Test]
		public void ReaderGetBytes()
		{
			var id_value = GetId();

			var selectText = "SELECT blob_field FROM TEST WHERE int_field = " + id_value.ToString();
			var insertText = "INSERT INTO TEST (int_field, blob_field) values(@int_field, @blob_field)";

			// Generate an array of temp data
                 #if NET6_0_OR_GREATER
			byte[] insert_values = RandomNumberGenerator.GetBytes(100000 * 4);
                 #else
			var insert_values = new byte[100000 * 4];
			var rng = new RNGCryptoServiceProvider();
			rng.GetBytes(insert_values);
                 #endif

			// Execute insert command
			var transaction = Connection.BeginTransaction();

			var insert = new IBCommand(insertText, Connection, transaction);
			insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
			insert.Parameters.Add("@blob_field", IBDbType.Binary).Value = insert_values;
			insert.ExecuteNonQuery();

			transaction.Commit();

			// Check that inserted values are correct
			var select = new IBCommand(selectText, Connection);

			var select_values = new byte[100000 * 4];

			using (var reader = select.ExecuteReader())
			{
				var index = 0;
				var segmentSize = 1000;
				while (reader.Read())
				{
					while (index < 400000)
					{
						reader.GetBytes(0, index, select_values, index, segmentSize);

						index += segmentSize;
					}
				}
			}

			for (var i = 0; i < insert_values.Length; i++)
			{
				if (insert_values[i] != select_values[i])
				{
					Assert.Fail("differences at index " + i.ToString());
				}
			}
		}

		#endregion
	}
}
