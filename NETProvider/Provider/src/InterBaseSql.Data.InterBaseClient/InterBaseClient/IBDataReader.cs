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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using InterBaseSql.Data.Client.Native;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.Schema;

namespace InterBaseSql.Data.InterBaseClient
{
	public sealed class IBDataReader : DbDataReader
	{
		#region Constants

		private const int StartPosition = -1;

		#endregion

		#region Fields

		private DataTable _schemaTable;
		private IBCommand _command;
		private IBConnection _connection;
		private DbValue[] _row;
		private Descriptor _fields;
		private CommandBehavior _commandBehavior;
		private bool _eof;
		private bool _isClosed;
		private int _position;
		private int _recordsAffected;
		private Dictionary<string, int> _columnsIndexesOrdinal;
		private Dictionary<string, int> _columnsIndexesOrdinalCI;

		#endregion

		#region DbDataReader Indexers

		public override object this[int i]
		{
			get { return GetValue(i); }
		}

		public override object this[string name]
		{
			get { return GetValue(GetOrdinal(name)); }
		}

		#endregion

		#region Constructors

		internal IBDataReader()
			: base()
		{
		}

		internal IBDataReader(
			IBCommand command,
			IBConnection connection,
			CommandBehavior commandBehavior)
		{
			_position = StartPosition;
			_command = command;
			_connection = connection;
			_commandBehavior = commandBehavior;
			_fields = _command.GetFieldsDescriptor();

			UpdateRecordsAffected();
		}

		#endregion

		#region DbDataReader overriden Properties

		public override int Depth
		{
			get
			{
				CheckState();

				return 0;
			}
		}

		public override bool HasRows
		{
			get { return _command.HasFields; }
		}

		public override bool IsClosed
		{
			get { return _isClosed; }
		}

		public override int FieldCount
		{
			get
			{
				CheckState();

				return _fields.Count;
			}
		}

		public override int RecordsAffected
		{
			get { return _recordsAffected; }
		}

		public override int VisibleFieldCount
		{
			get
			{
				CheckState();

				return _fields.Count;
			}
		}

		#endregion

		#region DbDataReader overriden methods

		public override void Close()
		{
			Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!IsClosed)
				{
					_isClosed = true;
					if (_command != null && !_command.IsDisposed)
					{
						if (_command.CommandType == CommandType.StoredProcedure)
						{
							_command.SetOutputParameters();
						}
						if (_command.HasImplicitTransaction)
						{
							_command.CommitImplicitTransaction();
						}
						_command.ActiveReader = null;
					}
					if (_connection != null && IsCommandBehavior(CommandBehavior.CloseConnection))
					{
						_connection.Close();
					}
					_position = StartPosition;
					_command = null;
					_connection = null;
					_row = null;
					_schemaTable = null;
					_fields = null;
				}
			}
		}

		public override bool Read()
		{
			CheckState();

			var retValue = false;

			if (IsCommandBehavior(CommandBehavior.SingleRow) && _position != StartPosition)
			{
			}
			else
			{
				if (IsCommandBehavior(CommandBehavior.SchemaOnly))
				{
				}
				else
				{
					_row = _command.Fetch();

					if (_row != null)
					{
						_position++;
						retValue = true;
					}
					else
					{
						_eof = true;
					}
				}
			}

			return retValue;
		}

		public override DataTable GetSchemaTable()
		{
			CheckState();

			if (_schemaTable != null)
			{
				return _schemaTable;
			}

			DataRow schemaRow = null;
			var tableCount = 0;
			var currentTable = string.Empty;

			_schemaTable = GetSchemaTableStructure();

			/* Prepare statement for schema fields information	*/
			var schemaCmd = new IBCommand(
				GetSchemaCommandText(),
				_command.Connection,
				_command.Connection.InnerConnection.ActiveTransaction);

			schemaCmd.Parameters.Add("@TABLE_NAME", IBDbType.Char, 68);
			schemaCmd.Parameters.Add("@COLUMN_NAME", IBDbType.Char, 68);
			schemaCmd.Prepare();

			_schemaTable.BeginLoadData();

			var emptyCount = 1;

			for (var i = 0; i < _fields.Count; i++)
			{
				var isKeyColumn = false;
				var isUnique = false;
				var isReadOnly = false;
				var precision = 0;
				var isExpression = false;
				var fieldType = 0;

				/* Get Schema data for the field	*/
				schemaCmd.Parameters[0].Value = _fields[i].Relation;
				schemaCmd.Parameters[1].Value = _fields[i].Name;

				using (var r = schemaCmd.ExecuteReader())
				{
					if (r.Read())
					{
						isReadOnly = (IsReadOnly(r) || IsExpression(r)) ? true : false;
						isKeyColumn = (r.GetInt32(2) == 1) ? true : false;
						isUnique = (r.GetInt32(3) == 1) ? true : false;
						precision = r.IsDBNull(4) ? -1 : r.GetInt32(4);
						isExpression = IsExpression(r);
						fieldType = r.GetInt32(5);
					}
				}

				/* Create new row for the Schema Table	*/
				schemaRow = _schemaTable.NewRow();

				schemaRow["ColumnName"] = GetName(i);
				schemaRow["ColumnOrdinal"] = i;
				schemaRow["ColumnSize"] = _fields[i].GetSize();
				if (_fields[i].IsDecimal())
				{
					schemaRow["NumericPrecision"] = schemaRow["ColumnSize"];
					if ((precision == -1) && (_fields[i].NumericScale != 0))
						precision = 15;	
					if (precision > 0)
					{
						schemaRow["NumericPrecision"] = precision;
					}
					schemaRow["NumericScale"] = _fields[i].NumericScale * (-1);
				}
				if ((_fields[i].DbDataType == DbDataType.Double) || (fieldType == IscCodes.blr_double))
				{
					if ((precision == -1) && (_fields[i].NumericScale != 0))
						schemaRow["NumericPrecision"] = 15;
					schemaRow["NumericScale"] = _fields[i].NumericScale * (-1);
				}
				schemaRow["DataType"] = GetFieldType(i);
				schemaRow["ProviderType"] = GetProviderType(i);
				schemaRow["IsLong"] = _fields[i].IsLong();
				schemaRow["AllowDBNull"] = _fields[i].AllowDBNull();
				schemaRow["IsRowVersion"] = false;
				schemaRow["IsAutoIncrement"] = false;
				schemaRow["IsReadOnly"] = isReadOnly;
				schemaRow["IsKey"] = isKeyColumn;
				schemaRow["IsUnique"] = isUnique;
				schemaRow["IsAliased"] = _fields[i].IsAliased();
				schemaRow["IsExpression"] = isExpression;
				schemaRow["BaseSchemaName"] = DBNull.Value;
				schemaRow["BaseCatalogName"] = DBNull.Value;
				schemaRow["BaseTableName"] = _fields[i].Relation;
				schemaRow["BaseColumnName"] = _fields[i].Name;

				if (IBDBXLegacyTypes.IncludeLegacySchemaType)
				{
					schemaRow["DbxDataType"] = IBDBXLegacyTypes.GetLegacyType(_connection.DBSQLDialect, GetLegacyProviderType(i));
					if (_fields[i].DbDataType == DbDataType.Char)
						schemaRow["DbxSubType"] = 31;
					else
						schemaRow["DbxSubType"] = 0;

					if (schemaRow["ColumnName"].ToString() == "")
					{
						schemaRow["ColumnName"] = "Mycolumn" + emptyCount;
						emptyCount = emptyCount + 1;
					}
					if ((_fields[i].IsDecimal() || (_fields[i].DbDataType == DbDataType.Double)))
					{
						if ((fieldType == IscCodes.blr_double) &&
						(_fields[i].NumericScale == 0))
							schemaRow["DbxPrecision"] = 8;
						else if (precision == -1)
							schemaRow["DbxPrecision"] = 15;
						else
							schemaRow["DbxPrecision"] = precision;
						    
					}
					else
						schemaRow["DbxPrecision"] = _fields[i].GetSize();
					if ((_fields[i].DbDataType == DbDataType.Binary) || (_fields[i].DbDataType == DbDataType.Text))
					{
						if (_fields[i].SubType == 1)
						{
							schemaRow["DbxSubType"] = 22;
							schemaRow["DbxPrecision"] = 1;
						}
						else
						{
							schemaRow["DbxSubType"] = 23;
							schemaRow["DbxPrecision"] = 1;
						}
					}
					schemaRow["IsHidden"] = false;
        			schemaRow["NumericScale"] = _fields[i].NumericScale * (-1);
					schemaRow["BaseSchemaName"] = DBNull.Value;
				}

				_schemaTable.Rows.Add(schemaRow);

				if (!string.IsNullOrEmpty(_fields[i].Relation) && currentTable != _fields[i].Relation)
				{
					tableCount++;
					currentTable = _fields[i].Relation;
				}

				/* Close statement	*/
				schemaCmd.Close();
			}

			if (tableCount > 1)
			{
				foreach (DataRow row in _schemaTable.Rows)
				{
					row["IsKey"] = false;
					row["IsUnique"] = false;
				}
			}

			_schemaTable.EndLoadData();

			/* Dispose command	*/
			schemaCmd.Dispose();

			return _schemaTable;
		}

		public override int GetOrdinal(string name)
		{
			CheckState();

			return GetColumnIndex(name);
		}

		public override string GetName(int i)
		{
			CheckState();
			CheckIndex(i);

			if (_fields[i].Alias.Length > 0)
			{
				return _fields[i].Alias;
			}
			else
			{
				return _fields[i].Name;
			}
		}

		public override string GetDataTypeName(int i)
		{
			CheckState();
			CheckIndex(i);

			return TypeHelper.GetDataTypeName(_fields[i].DbDataType);
		}

		public override Type GetFieldType(int i)
		{
			CheckState();
			CheckIndex(i);

			return _fields[i].GetSystemType();
		}

		public override Type GetProviderSpecificFieldType(int i)
		{
			return GetFieldType(i);
		}

		public override object GetProviderSpecificValue(int i)
		{
			return GetValue(i);
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			return GetValues(values);
		}

		public override object GetValue(int i)
		{
			// type coercions for EF
			if (_command.ExpectedColumnTypes != null)
			{
				var type = _command.ExpectedColumnTypes.ElementAtOrDefault(i);
				var nullableUnderlying = Nullable.GetUnderlyingType(type);
				if (nullableUnderlying != null)
				{
					if (IsDBNull(i))
					{
						return null;
					}
					if (nullableUnderlying == typeof(bool))
					{
						return GetBoolean(i);
					}
				}
				if (type == typeof(bool))
				{
					return GetBoolean(i);
				}
			}

			CheckState();
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].Value, i);
		}

		public override int GetValues(object[] values)
		{
			CheckState();
			CheckPosition();

			var count = Math.Min(_fields.Count, values.Length);
			for (var i = 0; i < count; i++)
			{
				values[i] = CheckedGetValue(x => GetValue(x), i);
			}
			return count;
		}

		public IBChangeState GetChangeState(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].ChangeState, i);
		}

		public IBChangeState GetChangeState(string fieldName)
		{
			return GetChangeState(GetOrdinal(fieldName));
		}


		public override bool GetBoolean(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetBoolean(), i);
		}

		public override byte GetByte(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetByte(), i);
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			CheckPosition();
			CheckIndex(i);

			var bytesRead = 0;
			var realLength = length;

			if (buffer == null)
			{
				if (IsDBNull(i))
				{
					return 0;
				}
				else
				{
					return CheckedGetValue(x => _row[x].GetBinary(), i).Length;
				}
			}
			else
			{
				var byteArray = CheckedGetValue(x => _row[x].GetBinary(), i);

				if (length > (byteArray.Length - dataIndex))
				{
					realLength = byteArray.Length - (int)dataIndex;
				}

				Array.Copy(byteArray, (int)dataIndex, buffer, bufferIndex, realLength);

				if ((byteArray.Length - dataIndex) < length)
				{
					bytesRead = byteArray.Length - (int)dataIndex;
				}
				else
				{
					bytesRead = length;
				}

				return bytesRead;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetChar(), i);
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			CheckPosition();
			CheckIndex(i);

			if (buffer == null)
			{
				if (IsDBNull(i))
				{
					return 0;
				}
				else
				{
					return CheckedGetValue(x => (string)GetValue(x), i).ToCharArray().Length;
				}
			}
			else
			{

				var charArray = CheckedGetValue(x => (string)GetValue(x), i).ToCharArray();

				var charsRead = 0;
				var realLength = length;

				if (length > (charArray.Length - dataIndex))
				{
					realLength = charArray.Length - (int)dataIndex;
				}

				System.Array.Copy(charArray, (int)dataIndex, buffer,
					bufferIndex, realLength);

				if ((charArray.Length - dataIndex) < length)
				{
					charsRead = charArray.Length - (int)dataIndex;
				}
				else
				{
					charsRead = length;
				}

				return charsRead;
			}
		}

		public override Guid GetGuid(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetGuid(), i);
		}

		public override Int16 GetInt16(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetInt16(), i);
		}

		public override Int32 GetInt32(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetInt32(), i);
		}

		public override Int64 GetInt64(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetInt64(), i);
		}

		public override float GetFloat(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetFloat(), i);
		}

		public override double GetDouble(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetDouble(), i);
		}

		public override string GetString(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetString(), i);
		}

		public override Decimal GetDecimal(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetDecimal(), i);
		}

		public override DateTime GetDateTime(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return CheckedGetValue(x => _row[x].GetDateTime(), i);
		}

		public override bool IsDBNull(int i)
		{
			CheckPosition();
			CheckIndex(i);

			return _row[i].IsDBNull();
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this, IsCommandBehavior(CommandBehavior.CloseConnection));
		}

		public override bool NextResult()
		{
			return false;
		}

		#endregion

		#region Private Methods

		private void CheckPosition()
		{
			if (_eof || _position == StartPosition)
				throw new InvalidOperationException("There are no data to read.");
		}

		private void CheckState()
		{
			if (IsClosed)
				throw new InvalidOperationException("Invalid attempt of read when the reader is closed.");
		}

		private void CheckIndex(int i)
		{
			if (i < 0 || i >= FieldCount)
				throw new IndexOutOfRangeException("Could not find specified column in results.");
		}

		private IBDbType GetProviderType(int i)
		{
			return (IBDbType)_fields[i].DbDataType;
		}

		private IBDbType GetLegacyProviderType(int i)
		{
			switch (_fields[i].SqlType)
			{
				case IscCodes.SQL_TEXT:
				case IscCodes.SQL_VARYING:
				case IscCodes.SQL_FLOAT:
				case IscCodes.SQL_TIMESTAMP:
				case IscCodes.SQL_TYPE_TIME:
				case IscCodes.SQL_TYPE_DATE:
				case IscCodes.SQL_ARRAY:
				case IscCodes.SQL_BOOLEAN:
				case IscCodes.SQL_SHORT:
				case IscCodes.SQL_LONG:
				case IscCodes.SQL_QUAD:
				case IscCodes.SQL_INT64:
				case IscCodes.SQL_BLOB:
					{
						return GetProviderType(i);
					}

				case IscCodes.SQL_DOUBLE:
				case IscCodes.SQL_D_FLOAT:
					if (_fields[i].SubType == 2)
					{
						return IBDbType.Decimal;
					}
					else if (_fields[i].SubType == 1)
					{
						return IBDbType.Numeric;
					}
					else if (_fields[i].NumericScale < 0)
					{
						return IBDbType.Decimal;
					}
					else
					{
						return IBDbType.Double;
					}
				default:
					return GetProviderType(i);
			}
		}
		private void UpdateRecordsAffected()
		{
			if (_command != null && !_command.IsDisposed)
			{
				if (_command.RecordsAffected != -1)
				{
					_recordsAffected = _recordsAffected == -1 ? 0 : _recordsAffected;
					_recordsAffected += _command.RecordsAffected;
				}
			}
		}

		private bool IsCommandBehavior(CommandBehavior behavior)
		{
			return _commandBehavior.HasFlag(behavior);
		}

		private void InitializeColumnsIndexes()
		{
			_columnsIndexesOrdinal = new Dictionary<string, int>(_fields.Count, StringComparer.Ordinal);
			_columnsIndexesOrdinalCI = new Dictionary<string, int>(_fields.Count, StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < _fields.Count; i++)
			{
				var fieldName = _fields[i].Alias;
				if (!_columnsIndexesOrdinal.ContainsKey(fieldName))
					_columnsIndexesOrdinal.Add(fieldName, i);
				if (!_columnsIndexesOrdinalCI.ContainsKey(fieldName))
					_columnsIndexesOrdinalCI.Add(fieldName, i);
			}
		}

		private int GetColumnIndex(string name)
		{
			if (_columnsIndexesOrdinal == null || _columnsIndexesOrdinalCI == null)
			{
				InitializeColumnsIndexes();
			}
			if (!_columnsIndexesOrdinal.TryGetValue(name, out var index))
				if (!_columnsIndexesOrdinalCI.TryGetValue(name, out index))
					throw new IndexOutOfRangeException($"Could not find specified column '{name}' in results.");
			return index;
		}

		#endregion

		#region Static Methods

		private static bool IsReadOnly(IBDataReader r)
		{
			return IsExpression(r);
		}

		public static bool IsExpression(IBDataReader r)
		{
			/* [0] = COMPUTED_BLR
			 * [1] = COMPUTED_SOURCE
			 */
			if (!r.IsDBNull(0) || !r.IsDBNull(1))
			{
				return true;
			}

			return false;
		}

		private static DataTable GetSchemaTableStructure()
		{
			var schema = new DataTable("Schema");

			// Schema table structure
			schema.Columns.Add("ColumnName", System.Type.GetType("System.String"));
			schema.Columns.Add("ColumnOrdinal", System.Type.GetType("System.Int32"));
			schema.Columns.Add("ColumnSize", System.Type.GetType("System.Int32"));
			schema.Columns.Add("NumericPrecision", System.Type.GetType("System.Int32"));
			schema.Columns.Add("NumericScale", System.Type.GetType("System.Int32"));
			schema.Columns.Add("DataType", System.Type.GetType("System.Type"));
			schema.Columns.Add("ProviderType", System.Type.GetType("System.Int32"));
			schema.Columns.Add("IsLong", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("AllowDBNull", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("IsReadOnly", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("IsRowVersion", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("IsUnique", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("IsKey", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("IsAutoIncrement", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("IsAliased", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("IsExpression", System.Type.GetType("System.Boolean"));
			schema.Columns.Add("BaseSchemaName", System.Type.GetType("System.String"));
			schema.Columns.Add("BaseCatalogName", System.Type.GetType("System.String"));
			schema.Columns.Add("BaseTableName", System.Type.GetType("System.String"));
			schema.Columns.Add("BaseColumnName", System.Type.GetType("System.String"));

			if (IBDBXLegacyTypes.IncludeLegacySchemaType)
			{
				schema.Columns.Add("DbxDataType", System.Type.GetType("System.Int32"));
				schema.Columns.Add("DbxSubType", System.Type.GetType("System.Int32"));
				schema.Columns.Add("DbxPrecision", System.Type.GetType("System.Int32"));
				schema.Columns.Add("IsHidden", System.Type.GetType("System.Boolean"));
			}

			return schema;
		}

		private static string GetSchemaCommandText()
		{
			const string sql =
				@"SELECT
					fld.rdb$computed_blr AS computed_blr,
					fld.rdb$computed_source AS computed_source,
					(SELECT COUNT(*) FROM rdb$relation_constraints rel
					  INNER JOIN rdb$indices idx ON rel.rdb$index_name = idx.rdb$index_name
					  INNER JOIN rdb$index_segments seg ON idx.rdb$index_name = seg.rdb$index_name
					WHERE rel.rdb$constraint_type = 'PRIMARY KEY'
					  AND rel.rdb$relation_name = rfr.rdb$relation_name
					  AND seg.rdb$field_name = rfr.rdb$field_name) AS primary_key,
					(SELECT COUNT(*) FROM rdb$relation_constraints rel
					  INNER JOIN rdb$indices idx ON rel.rdb$index_name = idx.rdb$index_name
					  INNER JOIN rdb$index_segments seg ON idx.rdb$index_name = seg.rdb$index_name
					WHERE rel.rdb$constraint_type = 'UNIQUE'
					  AND rel.rdb$relation_name = rfr.rdb$relation_name
					  AND seg.rdb$field_name = rfr.rdb$field_name) AS unique_key,
					fld.rdb$field_precision AS numeric_precision,
					fld.rdb$field_type AS FIELD_TYPE
				  FROM rdb$relation_fields rfr
					INNER JOIN rdb$fields fld ON rfr.rdb$field_source = fld.rdb$field_name
				  WHERE rfr.rdb$relation_name = ?
					AND rfr.rdb$field_name = ?
				  ORDER BY rfr.rdb$relation_name, rfr.rdb$field_position";

			return sql;
		}

		private static T CheckedGetValue<T>(Func<int, T> f, int index)
		{
			try
			{
				return f(index);
			}
			catch (IscException ex)
			{
				throw new IBException(ex.Message, ex);
			}
		}

		#endregion
	}
}
