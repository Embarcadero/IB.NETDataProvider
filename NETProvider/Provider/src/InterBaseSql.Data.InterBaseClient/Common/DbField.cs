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

using InterBaseSql.Data.Client.Native;
using System;
using System.Text;

namespace InterBaseSql.Data.Common
{
	internal sealed class DbField
	{
		#region Fields

		private short _dataType;
		private short _numericScale;
		private short _precisionScale;
		private short _subType;
		private short _length;
		private short _nullFlag;
		private string _name;
		private string _relation;
		private string _owner;
		private string _alias;
		private int _charCount;
		private DbValue _dbValue;
		private Charset _charset;
		private ArrayBase _arrayHandle;
		private IBDatabase _db;

		#endregion

		#region Properties

		public IBDatabase Database
		{
			get { return _db; }
			set { _db = value; }
		}

		public DbDataType DbDataType
		{
			get
			{
				return TypeHelper.GetDbDataTypeFromSqlType(SqlType, SubType, NumericScale, DbValue.Dialect, Length, Charset);
			}
		}

		public int SqlType
		{
			get { return _dataType & ~1; }
		}

		public short DataType
		{
			get { return _dataType; }
			set { _dataType = value; }
		}

		public short NumericScale
		{
			get { return _numericScale; }
			set { _numericScale = value; }
		}

		public short PrecisionScale
		{
			get { return _precisionScale;  }
			set { _precisionScale = value; }
		}

		public short SubType
		{
			get { return _subType; }
			set
			{
				_subType = value;
				if (IsCharacter())
				{
					// If the connection's charset is not the default (None) then the
					//  string data is converted on the server side to the lc_ctype.
					//  If it is then use the sql_sub_type for the information on the encoding
					//  Octets (1) needs to be treeated differently
					if ((_db.Charset != Charset.DefaultCharset) && (value != 1) )
					{
						_charset = _db.Charset;
					}
					else
					{
						// Bits 0-7 of sqlsubtype is charset_id (127 is a special value -
						// current attachment charset).
						// Bits 8-17 hold collation_id for this value.
						var cs = BitConverter.GetBytes(value);

						_charset = Charset.GetCharset(cs[0]);
					}

					if (_charset == null)
					{
						_charset = Charset.DefaultCharset;
					}
				}
			}
		}

		public short Length
		{
			get { return _length; }
			set
			{
				_length = value;
				if (IsCharacter())
				{
					_charCount = _length / _charset.BytesPerCharacter;
				}
			}
		}

		public short NullFlag
		{
			get { return _nullFlag; }
			set { _nullFlag = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value.Trim(); }
		}

		public string Relation
		{
			get { return _relation; }
			set { _relation = value.Trim(); }
		}

		public string Owner
		{
			get { return _owner; }
			set { _owner = value.Trim(); }
		}

		public string Alias
		{
			get { return _alias; }
			set { _alias = value.Trim(); }
		}

		public Charset Charset
		{
			get { return _charset; }
		}

		public int CharCount
		{
			get { return _charCount; }
		}

		public ArrayBase ArrayHandle
		{
			get
			{
				EnsureArray();
				return _arrayHandle;
			}

			set
			{
				EnsureArray();
				_arrayHandle = value;
			}
		}

		public DbValue DbValue
		{
			get { return _dbValue; }
		}

		public object Value
		{
			get { return _dbValue.Value; }
			set { _dbValue.Value = value; }
		}

		#endregion

		#region Constructors

		public DbField(IBDatabase DB)
		{
			_charCount = -1;
			_name = string.Empty;
			_relation = string.Empty;
			_owner = string.Empty;
			_alias = string.Empty;
			_db = DB;
			_dbValue = new DbValue(this, DBNull.Value);
		}

		#endregion

		#region Methods

		public bool IsNull()
		{
			return NullFlag < 0;
		}

		public IBChangeState ChangeState()
		{
			int s;
			// Turn off the NULL bit)
			UInt16 n = Convert.ToUInt16(IscCodes.SQLIND_NULL);
			n = Convert.ToUInt16(~n & 0x0000FFFF);
			s = NullFlag & n;
			// Is it the same?
			if (s == IscCodes.SQLIND_CHANGE_VIEW)
			{
				return IBChangeState.csSame;
			}
			// turn off the Change bit
			s = s & (~IscCodes.SQLIND_CHANGE_VIEW);
			// If all that is left is the SQLIND_CHANGE it is unknown
			if (s == IscCodes.SQLIND_CHANGE)
			{
				return IBChangeState.csUnknown;
			}
			// turn off the SQLIND_CHANGE bit
			s = s & (~(IscCodes.SQLIND_CHANGE));
			switch (s)
			{
				case IscCodes.SQLIND_INSERT:
					return IBChangeState.csInsert;
				case IscCodes.SQLIND_UPDATE:
					return IBChangeState.csUpdate; 
				case IscCodes.SQLIND_DELETE:
					return IBChangeState.csDelete; 
				case IscCodes.SQLIND_TRUNCATE:
					return IBChangeState.csTruncate; 
			}
			return IBChangeState.csUnknown;
		}

		public bool IsNumeric()
		{
			if (_dataType == 0)
			{
				return false;
			}

			switch (DbDataType)
			{
				case DbDataType.SmallInt:
				case DbDataType.Integer:
				case DbDataType.BigInt:
				case DbDataType.Numeric:
				case DbDataType.Decimal:
				case DbDataType.Float:
				case DbDataType.Double:
					return true;
				default:
					return false;
			}
		}

		public bool IsDecimal()
		{
			if (_dataType == 0)
			{
				return false;
			}

			switch (DbDataType)
			{
				case DbDataType.Numeric:
				case DbDataType.Decimal:
					return true;

				default:
					return false;
			}
		}

		public bool IsLong()
		{
			if (_dataType == 0)
			{
				return false;
			}

			switch (DbDataType)
			{
				case DbDataType.Binary:
				case DbDataType.Text:
					return true;

				default:
					return false;
			}
		}

		public bool IsCharacter()
		{
			if (_dataType == 0)
			{
				return false;
			}

			switch (DbDataType)
			{
				case DbDataType.Char:
				case DbDataType.VarChar:
				case DbDataType.Text:
					return true;

				default:
					return false;
			}
		}

		public bool IsArray()
		{
			if (_dataType == 0)
			{
				return false;
			}

			switch (DbDataType)
			{
				case DbDataType.Array:
					return true;

				default:
					return false;
			}
		}

		public bool IsAliased()
		{
			return (Name != Alias) ? true : false;
		}

		public int GetSize()
		{
			if (IsLong())
			{
				return System.Int32.MaxValue;
			}
			else
			{
				if (IsCharacter())
				{
					return CharCount;
				}
				else
				{
					return Length;
				}
			}
		}

		public bool AllowDBNull()
		{
			return ((DataType & 1) == 1);
		}

		public void SetValue(byte[] buffer)
		{
			if (buffer == null || NullFlag == -1)
			{
				Value = DBNull.Value;
			}
			else
			{
				switch (SqlType)
				{
					case IscCodes.SQL_TEXT:
					case IscCodes.SQL_VARYING:
						if (DbDataType == DbDataType.Guid)
						{
							Value = TypeDecoder.DecodeGuid(buffer);
						}
						else
						{
							if (Charset.IsOctetsCharset)
							{
								Value = buffer;
							}
							else
							{
								var s = Charset.GetString(buffer, 0, buffer.Length);

								if ((Length % Charset.BytesPerCharacter) == 0 &&
									s.Length > CharCount)
								{
									s = s.Substring(0, CharCount);
								}

								Value = s;
							}
						}
						break;

					case IscCodes.SQL_SHORT:
						if (_numericScale < 0)
						{
							Value = TypeDecoder.DecodeDecimal(
								BitConverter.ToInt16(buffer, 0),
								_numericScale,
								_dataType);
						}
						else
						{
							Value = BitConverter.ToInt16(buffer, 0);
						}
						break;

					case IscCodes.SQL_LONG:
						if (NumericScale < 0)
						{
							Value = TypeDecoder.DecodeDecimal(
								BitConverter.ToInt32(buffer, 0),
								_numericScale,
								_dataType);
						}
						else
						{
							Value = BitConverter.ToInt32(buffer, 0);
						}
						break;

					case IscCodes.SQL_FLOAT:
						Value = BitConverter.ToSingle(buffer, 0);
						break;

					case IscCodes.SQL_DOUBLE:
					case IscCodes.SQL_D_FLOAT:
						Value = BitConverter.ToDouble(buffer, 0);
						break;

					case IscCodes.SQL_QUAD:
					case IscCodes.SQL_INT64:
					case IscCodes.SQL_BLOB:
					case IscCodes.SQL_ARRAY:
						if (NumericScale < 0)
						{
							Value = TypeDecoder.DecodeDecimal(
								BitConverter.ToInt64(buffer, 0),
								_numericScale,
								_dataType);
						}
						else
						{
							Value = BitConverter.ToInt64(buffer, 0);
						}
						break;

					case IscCodes.SQL_TIMESTAMP:
						var date = TypeDecoder.DecodeDate(BitConverter.ToInt32(buffer, 0));
						var time = TypeDecoder.DecodeTime(BitConverter.ToInt32(buffer, 4));

						Value = date.Add(time);
						break;

					case IscCodes.SQL_TYPE_TIME:
						Value = TypeDecoder.DecodeTime(BitConverter.ToInt32(buffer, 0));
						break;

					case IscCodes.SQL_TYPE_DATE:
						Value = TypeDecoder.DecodeDate(BitConverter.ToInt32(buffer, 0));
						break;

					case IscCodes.SQL_BOOLEAN:
						Value = TypeDecoder.DecodeBoolean(buffer);
						break;

					default:
						throw TypeHelper.InvalidDataType(SqlType);
				}
			}
		}

		public void FixNull()
		{
			if (NullFlag <= -1 && _dbValue.IsDBNull())
			{
				switch (DbDataType)
				{
					case DbDataType.Char:
					case DbDataType.VarChar:
						Value = string.Empty;
						break;

					case DbDataType.Guid:
						Value = Guid.Empty;
						break;

					case DbDataType.SmallInt:
						Value = (short)0;
						break;

					case DbDataType.Integer:
						Value = (int)0;
						break;

					case DbDataType.BigInt:
					case DbDataType.Binary:
					case DbDataType.Array:
					case DbDataType.Text:
						Value = (long)0;
						break;

					case DbDataType.Numeric:
					case DbDataType.Decimal:
						Value = (decimal)0;
						break;

					case DbDataType.Float:
						Value = (float)0;
						break;

					case DbDataType.Double:
						Value = (double)0;
						break;

					case DbDataType.Date:
					case DbDataType.TimeStamp:
						Value = new DateTime(0 * 10000L + 621355968000000000);
						break;

					case DbDataType.Time:
						Value = TimeSpan.Zero;
						break;

					case DbDataType.Boolean:
						Value = false;
						break;

					default:
						throw IscException.ForStrParam($"Unknown sql data type: {DataType}.");
				}
			}
		}

		public Type GetSystemType()
		{
			return TypeHelper.GetTypeFromDbDataType(DbDataType);
		}

		public bool HasDataType()
		{
			return _dataType != 0;
		}

		#endregion

		#region Private Methods

		private void EnsureArray()
		{
			if (!IsArray())
				throw IscException.ForStrParam("Field is not an array type.");
		}

		#endregion
	}
}
