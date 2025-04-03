/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/Embarcadero/IB.NETDataProvider/raw/main/NETProvider/license.txt.
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

//$Authors = Jeff Overcash

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient;

// The purpose of this is to include the old Dbx Based ADO.NET drivers types in schema outputs.
// By default they won't be included.  Set IncludeLegacySchemaType to true if you want IBDataReaders's
//   GetSchema to include the DbxXxxx type of output in addition to the normal output.
//   This is still experimental, the mappings are not always obvious.
//   Also some Dialect 1/3 issues to be resolved around Date, Time, Timestamp as in Dialect 1 Date and Timestamp
//   both map to Timestamp, in Dialect 3 they map to 2 different types
public class IBDBXLegacyTypes
{
	public static bool IncludeLegacySchemaType = false;

	// Enum representing the old Dbx based drivers DbxDataTypes not all map to DbDataTypes
	public const byte UnknownType = 0;
	public const byte TimeStampType = 24;
	public const byte CurrencyType = 25;
	public const byte WideStringType = 26;
	public const byte SingleType = 27;
	public const byte Int8Type = 28;
	public const byte UInt8Type = 29;
	public const byte ObjectType = 30;
	public const byte TableType = 23;
	public const byte CharArrayType = 31;
	public const byte BinaryBlobType = 33;
	public const byte DBXConnectionType = 34;
	public const byte VariantType = 35;
	public const byte TimeStampOffsetType = 36;
	public const byte JsonValueType = 37;
	public const byte CallbackType = 38;
	public const byte MaxBaseTypes = 39;
	public const byte IntervalType = 32;
	public const byte RefType = 22;
	public const byte ArrayType = 21;
	public const byte AdtType = 20;
	public const byte AnsiStringType = 1;
	public const byte DateType = 2;
	public const byte BlobType = 3;
	public const byte BooleanType = 4;
	public const byte Int16Type = 5;
	public const byte Int32Type = 6;
	public const byte DoubleType = 7;
	public const byte BcdType = 8;
	public const byte BytesType = 9;
	public const byte TimeType = 10;
	public const byte DateTimeType = 11;
	public const byte UInt16Type = 12;
	public const byte UInt32Type = 13;
	public const byte VarBytesType = 15;
	public const byte CursorType = 17;
	public const byte Int64Type = 18;
	public const byte UInt64Type = 19;

	internal static IBDbType GetLegacyProviderType(IBDbType i, int SubType, int NumericScale)
	{
		switch ((int) i)
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
					return (IBDbType) i;
				}

			case IscCodes.SQL_DOUBLE:
			case IscCodes.SQL_D_FLOAT:
				if (SubType == 2)
				{
					return IBDbType.Decimal;
				}
				else if (SubType == 1)
				{
					return IBDbType.Numeric;
				}
				else if (NumericScale < 0)
				{
					return IBDbType.Decimal;
				}
				else
				{
					return IBDbType.Double;
				}
			default:
				return (IBDbType)i;
		}
	}

	internal static Dictionary<IBDbType, int> LegacyTypeMapping_D1 { get; } = new Dictionary<IBDbType, int>();
	internal static Dictionary<IBDbType, int> LegacyTypeMapping_D3 { get; } = new Dictionary<IBDbType, int>();

	internal static Dictionary<string, string> LegacyColumnNames { get; } = new Dictionary<string, string>();

	internal static HashSet<IBDbType> FixedLength { get; } = new HashSet<IBDbType>();
	internal static HashSet<IBDbType> IsLong { get; } = new HashSet<IBDbType>();

	public static int GetLegacyType(int SQLDialect, IBDbType type)
	{
		if (SQLDialect == 1)
			return LegacyTypeMapping_D1[type];
		else
			return -1;
	}

	public static DataTable UpdateColumnNames(DataTable dt)
	{
		foreach (DataColumn column in dt.Columns)
		{
			column.ColumnName = LegacyColumnNames.TryGetValue(column.ColumnName, out string value) ? value : column.ColumnName;
		}
		return dt;
	}

	static IBDBXLegacyTypes()
	{
		LegacyTypeMapping_D1.Add(IBDbType.Array, ArrayType);
		LegacyTypeMapping_D1.Add(IBDbType.BigInt, Int64Type);
		LegacyTypeMapping_D1.Add(IBDbType.Binary, BlobType);
		LegacyTypeMapping_D1.Add(IBDbType.Boolean, BooleanType);
		LegacyTypeMapping_D1.Add(IBDbType.Char, AnsiStringType);
		LegacyTypeMapping_D1.Add(IBDbType.Date, DateType);
		LegacyTypeMapping_D1.Add(IBDbType.Decimal, BcdType);
		LegacyTypeMapping_D1.Add(IBDbType.Double, DoubleType);
		LegacyTypeMapping_D1.Add(IBDbType.Float, SingleType);
		LegacyTypeMapping_D1.Add(IBDbType.Guid, BytesType); // Note IB does not have a GUID type so probably not used
		LegacyTypeMapping_D1.Add(IBDbType.Integer, Int32Type);
		LegacyTypeMapping_D1.Add(IBDbType.Numeric, BcdType);
		LegacyTypeMapping_D1.Add(IBDbType.SmallInt, Int16Type);
		LegacyTypeMapping_D1.Add(IBDbType.Text, BlobType);
		LegacyTypeMapping_D1.Add(IBDbType.Time, TimeType);
		LegacyTypeMapping_D1.Add(IBDbType.TimeStamp, TimeStampType);
		LegacyTypeMapping_D1.Add(IBDbType.VarChar, AnsiStringType);

		LegacyColumnNames.Add("PROCEDURE_CATALOG","CatalogName");
		LegacyColumnNames.Add("PROCEDURE_SCHEMA", "SchemaName");
		LegacyColumnNames.Add("PROCEDURE_NAME", "ProcedureName");
		LegacyColumnNames.Add("PARAMETER_NAME", "ParameterName");
		LegacyColumnNames.Add("PARAMETER_DATA_TYPE", "TypeName");
		LegacyColumnNames.Add("PARAMETER_SIZE", "Precision");
		LegacyColumnNames.Add("NUMERIC_SCALE", "Scale");
		LegacyColumnNames.Add("ORDINAL_POSITION", "Ordinal");
		LegacyColumnNames.Add("IS_NULLABLE", "IsNullable");
		LegacyColumnNames.Add("TABLE_NAME", "TableName");
		LegacyColumnNames.Add("COLUMN_NAME", "ColumnName");
		LegacyColumnNames.Add("COLUMN_DATA_TYPE", "TypeName");
		LegacyColumnNames.Add("COLUMN_SIZE", "Precision");
		LegacyColumnNames.Add("CONSTRAINT_CATALOG", "CatalogName");
		LegacyColumnNames.Add("CONSTRAINT_SCHEMA", "SchemaName");
		LegacyColumnNames.Add("CONSTRAINT_NAME", "ForeignKeyName");
		LegacyColumnNames.Add("REFERENCED_TABLE_NAME", "PrimaryTableName");
		LegacyColumnNames.Add("REFERENCED_COLUMN_NAME", "PrimaryColumnName");
		LegacyColumnNames.Add("REFERENCED_KEY_NAME", "PrimaryKeyName");
		LegacyColumnNames.Add("TABLE_CATALOG", "CatalogName");
		LegacyColumnNames.Add("TABLE_SCHEMA", "SchemaName");
		LegacyColumnNames.Add("INDEX_NAME", "IndexName");
		LegacyColumnNames.Add("IS_PRIMARY", "IsPrimary");
		LegacyColumnNames.Add("LEG_CONSTRAINT_NAME", "ConstraintName");
		LegacyColumnNames.Add("ROLE_NAME", "RoleName");
		LegacyColumnNames.Add("TABLE_TYPE", "TableType");
		LegacyColumnNames.Add("VIEW_CATALOG", "CatalogName");
		LegacyColumnNames.Add("VIEW_SCHEMA", "SchemaName");
		LegacyColumnNames.Add("VIEW_NAME", "ViewName");
		LegacyColumnNames.Add("DEFINITION", "Definition");
		//LegacyColumnNames.Add("", "");

		FixedLength.Add(IBDbType.BigInt);
		FixedLength.Add(IBDbType.Decimal);
		FixedLength.Add(IBDbType.Double);
		FixedLength.Add(IBDbType.Float);
		FixedLength.Add(IBDbType.Integer);
		FixedLength.Add(IBDbType.Numeric);
		FixedLength.Add(IBDbType.SmallInt);

		IsLong.Add(IBDbType.VarChar);
		IsLong.Add(IBDbType.Char);
	}
}
