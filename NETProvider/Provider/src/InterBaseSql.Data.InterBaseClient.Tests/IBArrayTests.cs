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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBArrayTests : IBTestsBase
{
	#region Constructors

	public IBArrayTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void IntegerArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new int[] { 10, 20, 30, 40 };

		var selectText = "SELECT	iarray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, iarray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();
		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new int[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void ShortArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new short[] { 50, 60, 70, 80 };

		var selectText = "SELECT	sarray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, sarray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new short[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public virtual void BigIntArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new long[] { 50, 60, 70, 80 };

		var selectText = "SELECT	larray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, larray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new long[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void FloatArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new float[] { 130.10F, 140.20F, 150.30F, 160.40F };
		var selectText = "SELECT	farray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, farray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new float[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void DoubleArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new double[] { 170.10, 180.20, 190.30, 200.40 };

		var selectText = "SELECT	barray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, barray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new double[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public virtual void NumericArrayTest()
	{
		Transaction = Connection.BeginTransaction();
		var insert_values = new decimal[] { 210.10M, 220.20M, 230.30M, 240.40M };
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);

		var selectText = "SELECT	narray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, narray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new decimal[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void DateArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new DateTime[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20), DateTime.Today.AddDays(30), DateTime.Today.AddDays(40) };

		var selectText = "SELECT	darray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, darray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new DateTime[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public virtual void TimeArrayTest()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new TimeSpan[] { new TimeSpan(3, 9, 10), new TimeSpan(4, 11, 12), new TimeSpan(6, 13, 14), new TimeSpan(8, 15, 16) };

		var selectText = "SELECT	tarray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, tarray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new TimeSpan[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void TimeStampArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new DateTime[] { DateTime.Now.AddSeconds(10), DateTime.Now.AddSeconds(20), DateTime.Now.AddSeconds(30), DateTime.Now.AddSeconds(40) };

		var selectText = "SELECT	tsarray_field FROM TEST	WHERE int_field	= " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, tsarray_field) values(@int_field,	@array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new DateTime[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				insert_values = insert_values.Select(x => new DateTime(x.Ticks / 1000 * 1000)).ToArray();
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void CharArrayTest()
	{
		Transaction = Connection.BeginTransaction();
		var insert_values = new string[] { "abc", "abcdef", "abcdefghi", "abcdefghijkl" };

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);

		var selectText = "SELECT	carray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, carray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new string[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				select_values = select_values.Select(x => x.TrimEnd(' ')).ToArray();
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void VarCharArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new string[] { "abc", "abcdef", "abcdefghi", "abcdefghijkl" };

		var selectText = "SELECT	varray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, varray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new string[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void IntegerArrayPartialUpdateTest()
	{
		var new_values = new int[] { 100, 200 };
		var updateText = "update	TEST set iarray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void ShortArrayPartialUpdateTest()
	{
		var new_values = new short[] { 500, 600 };
		var updateText = "update	TEST set sarray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void BigIntArrayPartialUpdateTest()
	{
		var new_values = new long[] { 900, 1000, 1100, 1200 };
		var updateText = "update	TEST set larray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void FloatArrayPartialUpdateTest()
	{
		var new_values = new long[] { 900, 1000, 1100, 1200 };
		var updateText = "update	TEST set farray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void DoubleArrayPartialUpdateTest()
	{
		var new_values = new double[] { 1700.10, 1800.20 };
		var updateText = "update	TEST set barray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public virtual void NumericArrayPartialUpdateTest()
	{
		var new_values = new decimal[] { 2100.10M, 2200.20M };
		var updateText = "update	TEST set narray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void DateArrayPartialUpdateTest()
	{
		var new_values = new DateTime[] { DateTime.Now.AddDays(100), DateTime.Now.AddDays(200) };
		var updateText = "update	TEST set darray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public virtual void TimeArrayPartialUpdateTest()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		var new_values = new TimeSpan[] { new TimeSpan(11, 13, 14), new TimeSpan(12, 15, 16) };
		var updateText = "update	TEST set tarray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}
	[Test]
	public void TimeStampArrayPartialUpdateTest()
	{
		var new_values = new DateTime[] { DateTime.Now.AddSeconds(100), DateTime.Now.AddSeconds(200) };
		var updateText = "update	TEST set tsarray_field = @array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void CharArrayPartialUpdateTest()
	{
		var new_values = new string[] { "abc", "abcdef" };
		var updateText = "update	TEST set carray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void VarCharArrayPartialUpdateTest()
	{
		var new_values = new string[] { "abc", "abcdef" };
		var updateText = "update	TEST set varray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public void BigArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		int elements = short.MaxValue;

		var selectText = "SELECT	big_array FROM TEST	WHERE int_field	= " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, big_array) values(@int_field,	@array_field)";

#if NET6_0_OR_GREATER
		var bytes = RandomNumberGenerator.GetBytes(elements * 4);
#else
		var bytes = new byte[elements * 4];
		using (var rng = new RNGCryptoServiceProvider())
		{
			rng.GetBytes(bytes);
		}
#endif
		var insert_values = new int[elements];
		Buffer.BlockCopy(bytes, 0, insert_values, 0, bytes.Length);

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new int[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}

		reader.Close();
		select.Dispose();
	}

	[Test]
	public void PartialUpdatesTest()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var elements = 16384;
#if NET6_0_OR_GREATER
		var bytes = RandomNumberGenerator.GetBytes(elements * 4);
#else
		var bytes = new byte[elements * 4];
		using (var rng = new RNGCryptoServiceProvider())
		{
			rng.GetBytes(bytes);
		}
#endif
		var insert_values = new int[elements];
		Buffer.BlockCopy(bytes, 0, insert_values, 0, bytes.Length);

		using (var transaction = Connection.BeginTransaction())
		{
			using (var insert = new IBCommand("INSERT INTO TEST (int_field, big_array) values(@int_field, @array_field)", Connection, transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				insert.ExecuteNonQuery();
			}
			transaction.Commit();
		}

		using (var select = new IBCommand($"SELECT big_array FROM TEST WHERE int_field = {id_value}", Connection))
		{
			using (var reader = select.ExecuteReader())
			{
				if (reader.Read())
				{
					if (!reader.IsDBNull(0))
					{
						var select_values = new int[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	#endregion

	#region Async Tests

	[Test]
	public virtual async Task IntegerArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new int[] { 10, 20, 30, 40 };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, iarray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT iarray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new int[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task ShortArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new short[] { 50, 60, 70, 80 };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, sarray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}

			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT sarray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new short[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public virtual async Task BigIntArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new long[] { 50, 60, 70, 80 };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, larray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT larray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new long[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task FloatArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new float[] { 130.10F, 140.20F, 150.30F, 160.40F };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, farray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT farray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new float[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task DoubleArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new double[] { 170.10, 180.20, 190.30, 200.40 };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, barray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT barray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new double[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public virtual async Task NumericArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new decimal[] { 210.10M, 220.20M, 230.30M, 240.40M };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, narray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT narray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new decimal[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task DateArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new DateTime[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20), DateTime.Today.AddDays(30), DateTime.Today.AddDays(40) };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, darray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT darray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new DateTime[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public virtual async Task TimeArrayTestAsync()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new TimeSpan[] { new TimeSpan(3, 9, 10), new TimeSpan(4, 11, 12), new TimeSpan(6, 13, 14), new TimeSpan(8, 15, 16) };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, tarray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT tarray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new TimeSpan[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task TimeStampArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new DateTime[] { DateTime.Now.AddSeconds(10), DateTime.Now.AddSeconds(20), DateTime.Now.AddSeconds(30), DateTime.Now.AddSeconds(40) };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, tsarray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT tsarray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new DateTime[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						insert_values = insert_values.Select(x => new DateTime(x.Ticks / 1000 * 1000)).ToArray();
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task CharArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new string[] { "abc", "abcdef", "abcdefghi", "abcdefghijkl" };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, carray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT carray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new string[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						select_values = select_values.Select(x => x.TrimEnd(' ')).ToArray();
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task VarCharArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new string[] { "abc", "abcdef", "abcdefghi", "abcdefghijkl" };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, varray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT varray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new string[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task IntegerArrayPartialUpdateTestAsync()
	{
		var new_values = new int[] { 100, 200 };

		await using (var update = new IBCommand("update TEST set iarray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task ShortArrayPartialUpdateTestAsync()
	{
		var new_values = new short[] { 500, 600 };

		await using (var update = new IBCommand("update TEST set sarray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task BigIntArrayPartialUpdateTestAsync()
	{
		var new_values = new long[] { 900, 1000, 1100, 1200 };

		await using (var update = new IBCommand("update TEST set larray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task FloatArrayPartialUpdateTestAsync()
	{
		var new_values = new float[] { 1300.10F, 1400.20F };

		await using (var update = new IBCommand("update TEST set farray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task DoubleArrayPartialUpdateTestAsync()
	{
		var new_values = new double[] { 1700.10, 1800.20 };

		await using (var update = new IBCommand("update TEST set barray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public virtual async Task NumericArrayPartialUpdateTestAsync()
	{
		var new_values = new decimal[] { 2100.10M, 2200.20M };

		await using (var update = new IBCommand("update TEST set narray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task DateArrayPartialUpdateTestAsync()
	{
		var new_values = new DateTime[] { DateTime.Now.AddDays(100), DateTime.Now.AddDays(200) };

		await using (var update = new IBCommand("update TEST set darray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public virtual async Task TimeArrayPartialUpdateTestAsync()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		var new_values = new TimeSpan[] { new TimeSpan(11, 13, 14), new TimeSpan(12, 15, 16) };

		await using (var update = new IBCommand("update TEST set tarray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task TimeStampArrayPartialUpdateTestAsync()
	{
		var new_values = new DateTime[] { DateTime.Now.AddSeconds(100), DateTime.Now.AddSeconds(200) };

		await using (var update = new IBCommand("update TEST set tsarray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task CharArrayPartialUpdateTestAsync()
	{
		var new_values = new string[] { "abc", "abcdef" };

		await using (var update = new IBCommand("update TEST set carray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task VarCharArrayPartialUpdateTestAsync()
	{
		var new_values = new string[] { "abc", "abcdef" };

		await using (var update = new IBCommand("update TEST set varray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task BigArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		int elements = short.MaxValue;
#if NET6_0_OR_GREATER
		var bytes = RandomNumberGenerator.GetBytes(elements * 4);
#else
		var bytes = new byte[elements * 4];
		using (var rng = new RNGCryptoServiceProvider())
		{
			rng.GetBytes(bytes);
		}
#endif
		var insert_values = new int[elements];
		Buffer.BlockCopy(bytes, 0, insert_values, 0, bytes.Length);

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, big_array) values(@int_field, @array_field)", Connection, transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT big_array FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new int[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public async Task PartialUpdatesTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var elements = 16384;
#if NET6_0_OR_GREATER
		var bytes = RandomNumberGenerator.GetBytes(elements * 4);
#else
		var bytes = new byte[elements * 4];
		using (var rng = new RNGCryptoServiceProvider())
		{
			rng.GetBytes(bytes);
		}
#endif
		var insert_values = new int[elements];
		Buffer.BlockCopy(bytes, 0, insert_values, 0, bytes.Length);

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, big_array) values(@int_field, @array_field)", Connection, transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT big_array FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new int[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}
	#endregion
}

public class IBArrayTestsDialect1 : IBArrayTests
{
	public IBArrayTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}

	[Test]
	public override void BigIntArrayTest()
	{
		Transaction = Connection.BeginTransaction();

		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new double[] { 50, 60, 70, 80 };

		var selectText = "SELECT	larray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, larray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				var select_values = new double[insert_values.Length];
				Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
				CollectionAssert.AreEqual(insert_values, select_values);
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public override void NumericArrayPartialUpdateTest()
	{
		var new_values = new double[] { (double)2100.10M, (double)2200.20M };
		var updateText = "update	TEST set narray_field =	@array_field " +
							"WHERE int_field = 1";

		var update = new IBCommand(updateText, Connection);
		update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
		update.ExecuteNonQuery();
		update.Dispose();
	}

	[Test]
	public override void NumericArrayTest()
	{
		Transaction = Connection.BeginTransaction();
		var insert_values = new double[] { (double)210.10M, (double)220.20M, (double)230.30M, (double)240.40M };
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);

		var selectText = "SELECT	narray_field FROM TEST WHERE int_field = " + id_value.ToString();
		var insertText = "INSERT	INTO TEST (int_field, narray_field)	values(@int_field, @array_field)";

		var insert = new IBCommand(insertText, Connection, Transaction);
		insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
		insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
		insert.ExecuteNonQuery();
		insert.Dispose();

		Transaction.Commit();

		var select = new IBCommand(selectText, Connection);
		var reader = select.ExecuteReader();
		if (reader.Read())
		{
			if (!reader.IsDBNull(0))
			{
				if (IBTestsSetup.Dialect == 3)
				{
					var select_values = new decimal[insert_values.Length];
					Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
					CollectionAssert.AreEqual(insert_values, select_values);
				}
				else
				{
					var select_values = new double[insert_values.Length];
					Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
					CollectionAssert.AreEqual(insert_values, select_values);
				}
			}
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public override async Task BigIntArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new double[] { 50, 60, 70, 80 };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, larray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT larray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						var select_values = new double[insert_values.Length];
						Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
						CollectionAssert.AreEqual(insert_values, select_values);
					}
				}
			}
		}
	}

	[Test]
	public override async Task NumericArrayPartialUpdateTestAsync()
	{
		var new_values = new double[] { (double)2100.10M, (double)2200.20M };

		await using (var update = new IBCommand("update TEST set narray_field = @array_field WHERE int_field = 1", Connection))
		{
			update.Parameters.Add("@array_field", IBDbType.Array).Value = new_values;
			await update.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public override async Task NumericArrayTestAsync()
	{
		var id_value = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert_values = new double[] { (double)210.10M, (double)220.20M, (double)230.30M, (double)240.40M };

		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var insert = new IBCommand("INSERT INTO TEST (int_field, narray_field) values(@int_field, @array_field)", Connection, Transaction))
			{
				insert.Parameters.Add("@int_field", IBDbType.Integer).Value = id_value;
				insert.Parameters.Add("@array_field", IBDbType.Array).Value = insert_values;
				await insert.ExecuteNonQueryAsync();
			}
			await Transaction.CommitAsync();
		}

		await using (var select = new IBCommand($"SELECT narray_field FROM TEST WHERE int_field = {id_value}", Connection))
		{
			await using (var reader = await select.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					if (!await reader.IsDBNullAsync(0))
					{
						if (IBTestsSetup.Dialect == 3)
						{
							var select_values = new decimal[insert_values.Length];
							Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
							CollectionAssert.AreEqual(insert_values, select_values);
						}
						else
						{
							var select_values = new double[insert_values.Length];
							Array.Copy((Array)reader.GetValue(0), select_values, select_values.Length);
							CollectionAssert.AreEqual(insert_values, select_values);
						}
					}
				}
			}
		}
	}



}
