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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Data;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Common;

internal static class TypeHelper
{
	public static bool IsDBNull(object value)
	{
		return value == null || value == DBNull.Value;
	}

	public static short? GetSize(DbDataType type)
	{
		switch (type)
		{
			case DbDataType.Array:
			case DbDataType.Binary:
			case DbDataType.Text:
				return 8;

			case DbDataType.SmallInt:
				return 2;

			case DbDataType.Integer:
			case DbDataType.Float:
			case DbDataType.Date:
			case DbDataType.Time:
				return 4;

			case DbDataType.BigInt:
			case DbDataType.Double:
			case DbDataType.TimeStamp:
				return 8;

			case DbDataType.Guid:
				return 16;

			case DbDataType.Boolean:
				return 1;

			default:
				return null;
		}
	}

	public static int GetSqlTypeFromDbDataType(DbDataType type, bool isNullable)
	{
		var sqltype = 0;

		switch (type)
		{
			case DbDataType.Array:
				sqltype = IscCodes.SQL_ARRAY;
				break;

			case DbDataType.Binary:
			case DbDataType.Text:
				sqltype = IscCodes.SQL_BLOB;
				break;

			case DbDataType.Char:
				sqltype = IscCodes.SQL_TEXT;
				break;

			case DbDataType.VarChar:
				sqltype = IscCodes.SQL_VARYING;
				break;

			case DbDataType.SmallInt:
				sqltype = IscCodes.SQL_SHORT;
				break;

			case DbDataType.Integer:
				sqltype = IscCodes.SQL_LONG;
				break;

			case DbDataType.BigInt:
				sqltype = IscCodes.SQL_INT64;
				break;

			case DbDataType.Float:
				sqltype = IscCodes.SQL_FLOAT;
				break;

			case DbDataType.Guid:
				sqltype = IscCodes.SQL_TEXT;
				break;

			case DbDataType.Double:
				sqltype = IscCodes.SQL_DOUBLE;
				break;

			case DbDataType.Date:
				sqltype = IscCodes.SQL_TYPE_DATE;
				break;

			case DbDataType.Time:
				sqltype = IscCodes.SQL_TYPE_TIME;
				break;

			case DbDataType.TimeStamp:
				sqltype = IscCodes.SQL_TIMESTAMP;
				break;

			case DbDataType.Boolean:
				sqltype = IscCodes.SQL_BOOLEAN;
				break;

			default:
				throw InvalidDataType((int)type);
		}

		if (isNullable)
		{
			sqltype++;
		}

		return sqltype;
	}

	public static int GetSqlTypeFromBlrType(int type)
	{
		switch (type)
		{
			case IscCodes.blr_varying:
			case IscCodes.blr_varying2:
				return IscCodes.SQL_VARYING;

			case IscCodes.blr_text:
			case IscCodes.blr_text2:
			case IscCodes.blr_cstring:
			case IscCodes.blr_cstring2:
				return IscCodes.SQL_TEXT;

			case IscCodes.blr_short:
				return IscCodes.SQL_SHORT;

			case IscCodes.blr_long:
				return IscCodes.SQL_LONG;

			case IscCodes.blr_quad:
				return IscCodes.SQL_QUAD;

			case IscCodes.blr_int64:
			case IscCodes.blr_blob_id:
				return IscCodes.SQL_INT64;

			case IscCodes.blr_double:
				return IscCodes.SQL_DOUBLE;

			case IscCodes.blr_d_float:
				return IscCodes.SQL_D_FLOAT;

			case IscCodes.blr_float:
				return IscCodes.SQL_FLOAT;

			case IscCodes.blr_sql_date:
				return IscCodes.SQL_TYPE_DATE;

			case IscCodes.blr_sql_time:
				return IscCodes.SQL_TYPE_TIME;

			case IscCodes.blr_timestamp:
				return IscCodes.SQL_TIMESTAMP;

			case IscCodes.blr_blob:
				return IscCodes.SQL_BLOB;

			case IscCodes.blr_boolean_dtype:
			case IscCodes.blr_boolean:
				return IscCodes.SQL_BOOLEAN;

			default:
				throw InvalidDataType(type);
		}
	}

	public static string GetDataTypeName(DbDataType type)
	{
		switch (type)
		{
			case DbDataType.Array:
				return "ARRAY";

			case DbDataType.Binary:
				return "BLOB";

			case DbDataType.Text:
				return "BLOB SUB_TYPE 1";

			case DbDataType.Char:
			case DbDataType.Guid:
				return "CHAR";

			case DbDataType.VarChar:
				return "VARCHAR";

			case DbDataType.SmallInt:
				return "SMALLINT";

			case DbDataType.Integer:
				return "INTEGER";

			case DbDataType.Float:
				return "FLOAT";

			case DbDataType.Double:
				return "DOUBLE PRECISION";

			case DbDataType.BigInt:
				return "NUMERIC(18, 0)";

			case DbDataType.Numeric:
				return "NUMERIC";

			case DbDataType.Decimal:
				return "DECIMAL";

			case DbDataType.Date:
				return "DATE";

			case DbDataType.Time:
				return "TIME";

			case DbDataType.TimeStamp:
				return "TIMESTAMP";

			case DbDataType.Boolean:
				return "BOOLEAN";

			default:
				throw InvalidDataType((int)type);
		}
	}

	public static Type GetTypeFromDbDataType(DbDataType type)
	{
		switch (type)
		{
			case DbDataType.Array:
				return typeof(System.Array);

			case DbDataType.Binary:
				return typeof(System.Byte[]);

			case DbDataType.Text:
			case DbDataType.Char:
			case DbDataType.VarChar:
				return typeof(System.String);

			case DbDataType.Guid:
				return typeof(System.Guid);

			case DbDataType.SmallInt:
				return typeof(System.Int16);

			case DbDataType.Integer:
				return typeof(System.Int32);

			case DbDataType.BigInt:
				return typeof(System.Int64);

			case DbDataType.Float:
				return typeof(System.Single);

			case DbDataType.Double:
				return typeof(System.Double);

			case DbDataType.Numeric:
			case DbDataType.Decimal:
				return typeof(System.Decimal);

			case DbDataType.Date:
			case DbDataType.TimeStamp:
				return typeof(System.DateTime);

			case DbDataType.Time:
				return typeof(System.TimeSpan);

			case DbDataType.Boolean:
				return typeof(System.Boolean);

			default:
				throw InvalidDataType((int)type);
		}
	}
	public static IBDbType GetIBDataTypeFromType(Type type)
	{
		if (type.IsEnum)
		{
			return GetIBDataTypeFromType(Enum.GetUnderlyingType(type));
		}

		if (type == typeof(System.DBNull))
		{
			return IBDbType.VarChar;
		}

		if (type == typeof(System.String))
		{
			return IBDbType.VarChar;
		}
		else if (type == typeof(System.Char))
		{
			return IBDbType.Char;
		}
		else if (type == typeof(System.Boolean))
		{
			return IBDbType.Boolean;
		}
		else if (type == typeof(System.Byte) || type == typeof(System.SByte) || type == typeof(System.Int16) || type == typeof(System.UInt16))
		{
			return IBDbType.SmallInt;
		}
		else if (type == typeof(System.Int32) || type == typeof(System.UInt32))
		{
			return IBDbType.Integer;
		}
		else if (type == typeof(System.Int64) || type == typeof(System.UInt64))
		{
			return IBDbType.BigInt;
		}
		else if (type == typeof(System.Single))
		{
			return IBDbType.Float;
		}
		else if (type == typeof(System.Double))
		{
			return IBDbType.Double;
		}
		else if (type == typeof(System.Decimal))
		{
			return IBDbType.Decimal;
		}
		else if (type == typeof(System.DateTime))
		{
			return IBDbType.TimeStamp;
		}
		else if (type == typeof(System.TimeSpan))
		{
			return IBDbType.Time;
		}
		else if (type == typeof(System.Guid))
		{
			return IBDbType.Guid;
		}
		else if (type == typeof(System.Byte[]))
		{
			return IBDbType.Binary;
		}
#if NET6_0_OR_GREATER
		else if (type == typeof(System.DateOnly))
		{
			return IBDbType.Date;
		}
#endif
#if NET6_0_OR_GREATER
		else if (type == typeof(System.TimeOnly))
		{
			return IBDbType.Time;
		}
#endif
		else
		{
			throw new ArgumentException($"Unknown type: {type}.");
		}
	}

	public static Type GetTypeFromBlrType(int type, int subType, int scale)
	{
		return GetTypeFromDbDataType(GetDbDataTypeFromBlrType(type, subType, scale));
	}

	public static DbType GetDbTypeFromDbDataType(DbDataType type)
	{
		switch (type)
		{
			case DbDataType.Array:
			case DbDataType.Binary:
				return DbType.Binary;

			case DbDataType.Text:
			case DbDataType.VarChar:
			case DbDataType.Char:
				return DbType.String;

			case DbDataType.SmallInt:
				return DbType.Int16;

			case DbDataType.Integer:
				return DbType.Int32;

			case DbDataType.BigInt:
				return DbType.Int64;

			case DbDataType.Date:
				return DbType.Date;

			case DbDataType.Time:
				return DbType.Time;

			case DbDataType.TimeStamp:
				return DbType.DateTime;

			case DbDataType.Numeric:
			case DbDataType.Decimal:
				return DbType.Decimal;

			case DbDataType.Float:
				return DbType.Single;

			case DbDataType.Double:
				return DbType.Double;

			case DbDataType.Guid:
				return DbType.Guid;

			case DbDataType.Boolean:
				return DbType.Boolean;

			default:
				throw InvalidDataType((int)type);
		}
	}

	public static DbDataType GetDbDataTypeFromDbType(DbType type)
	{
		switch (type)
		{
			case DbType.String:
			case DbType.AnsiString:
				return DbDataType.VarChar;

			case DbType.StringFixedLength:
			case DbType.AnsiStringFixedLength:
				return DbDataType.Char;

			case DbType.Byte:
			case DbType.SByte:
			case DbType.Int16:
			case DbType.UInt16:
				return DbDataType.SmallInt;

			case DbType.Int32:
			case DbType.UInt32:
				return DbDataType.Integer;

			case DbType.Int64:
			case DbType.UInt64:
				return DbDataType.BigInt;

			case DbType.Date:
				return DbDataType.Date;

			case DbType.Time:
				return DbDataType.Time;

			case DbType.DateTime:
				return DbDataType.TimeStamp;

			case DbType.Object:
			case DbType.Binary:
				return DbDataType.Binary;

			case DbType.Decimal:
				return DbDataType.Decimal;

			case DbType.Double:
				return DbDataType.Double;

			case DbType.Single:
				return DbDataType.Float;

			case DbType.Guid:
				return DbDataType.Guid;

			case DbType.Boolean:
				return DbDataType.Boolean;

			default:
				throw InvalidDataType((int)type);
		}
	}

	public static DbDataType GetDbDataTypeFromBlrType(int type, int subType, int scale)
	{
		return GetDbDataTypeFromSqlType(GetSqlTypeFromBlrType(type), subType, scale);
	}

	public static DbDataType GetDbDataTypeFromSqlType(int type, int subType, int scale, int? length = null, Charset charset = null)
	{
		// Special case for Guid handling
		if ((type == IscCodes.SQL_TEXT || type == IscCodes.SQL_VARYING)  && length == 16 && (charset?.IsOctetsCharset ?? false))
		{
			return DbDataType.Guid;
		}

		switch (type)
		{
			case IscCodes.SQL_TEXT:
				return DbDataType.Char;

			case IscCodes.SQL_VARYING:
				return DbDataType.VarChar;

			case IscCodes.SQL_SHORT:
				if (subType == 2)
				{
					return DbDataType.Decimal;
				}
				else if (subType == 1)
				{
					return DbDataType.Numeric;
				}
				else if (scale < 0)
				{
					return DbDataType.Decimal;
				}
				else
				{
					return DbDataType.SmallInt;
				}

			case IscCodes.SQL_LONG:
				if (subType == 2)
				{
					return DbDataType.Decimal;
				}
				else if (subType == 1)
				{
					return DbDataType.Numeric;
				}
				else if (scale < 0)
				{
					return DbDataType.Decimal;
				}
				else
				{
					return DbDataType.Integer;
				}

			case IscCodes.SQL_QUAD:
			case IscCodes.SQL_INT64:
				if (subType == 2)
				{
					return DbDataType.Decimal;
				}
				else if (subType == 1)
				{
					return DbDataType.Numeric;
				}
				else if (scale < 0)
				{
					return DbDataType.Decimal;
				}
				else
				{
					return DbDataType.BigInt;
				}

			case IscCodes.SQL_FLOAT:
				return DbDataType.Float;

			// Only 2 ways to get these two.
			//   First is double precision no matter the dialect
			//   Second is a Numeric/decimal(10+,x) in dialect 1
			//     even after upgrading to D3 this will continue to be a double precision.
			//   Internally the value will always be a double, do not represent it as a decimal or numeric
			case IscCodes.SQL_DOUBLE:
			case IscCodes.SQL_D_FLOAT:
				//if (subType == 2)
				//{
				//	return DbDataType.Decimal;
				//}
				//else if (subType == 1)
				//{
				//	return DbDataType.Numeric;
				//}
				//else if (scale < 0)
				//{
				//	return DbDataType.Decimal;
				//}
				//else
				//{
				//	return DbDataType.Double;
				//}
				return DbDataType.Double;
			case IscCodes.SQL_BLOB:
				if (subType == 1)
				{
					return DbDataType.Text;
				}
				else
				{
					return DbDataType.Binary;
				}

			case IscCodes.SQL_TIMESTAMP:
				return DbDataType.TimeStamp;

			case IscCodes.SQL_TYPE_TIME:
				return DbDataType.Time;

			case IscCodes.SQL_TYPE_DATE:
				return DbDataType.Date;

			case IscCodes.SQL_ARRAY:
				return DbDataType.Array;

			case IscCodes.SQL_BOOLEAN:
				return DbDataType.Boolean;

			default:
				throw InvalidDataType(type);
		}
	}

	public static DbDataType GetDbDataTypeFromIBDbType(IBDbType type)
	{
		// these are aligned for this conversion
		return (DbDataType)type;
	}

	public static TimeSpan DateTimeToTimeSpan(DateTime d)
	{
		return TimeSpan.FromTicks(d.Subtract(d.Date).Ticks);
	}

	public static Exception InvalidDataType(int type)
	{
		return new ArgumentException($"Invalid data type: {type}.");
	}
	public static int BlrAlign(int current, int alignment)
	{
		return (current + alignment - 1) & ~(alignment - 1);
	}
}
