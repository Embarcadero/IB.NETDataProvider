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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Data;
using InterBaseSql.Data.InterBaseClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal
{
	public class IBTypeMappingSource : RelationalTypeMappingSource
	{
		public const int BinaryMaxSize = Int32.MaxValue;
		public const int VarcharMaxSize = 32765 / 4;
		public const int DefaultDecimalPrecision = 18;
		public const int DefaultDecimalScale = 2;

		readonly IBBoolTypeMapping _boolean = new IBBoolTypeMapping();

		readonly ShortTypeMapping _smallint = new ShortTypeMapping("SMALLINT", DbType.Int16);
		readonly IntTypeMapping _integer = new IntTypeMapping("INTEGER", DbType.Int32);
		readonly LongTypeMapping _bigint = new LongTypeMapping("NUMERIC(18, 0)", DbType.Int64);

		readonly IBStringTypeMapping _char = new IBStringTypeMapping("CHAR", IBDbType.Char);
		readonly IBStringTypeMapping _varchar = new IBStringTypeMapping("VARCHAR", IBDbType.VarChar);
		readonly IBStringTypeMapping _clob = new IBStringTypeMapping("BLOB SUB_TYPE TEXT", IBDbType.Text);

		readonly IBByteArrayTypeMapping _binary = new IBByteArrayTypeMapping();

		readonly FloatTypeMapping _float = new FloatTypeMapping("FLOAT");
		readonly DoubleTypeMapping _double = new DoubleTypeMapping("DOUBLE PRECISION");
		readonly DecimalTypeMapping _decimal = new DecimalTypeMapping($"DECIMAL({DefaultDecimalPrecision},{DefaultDecimalScale})");

		readonly IBDateTimeTypeMapping _timestamp = new IBDateTimeTypeMapping("TIMESTAMP", IBDbType.TimeStamp);
		readonly IBDateTimeTypeMapping _date = new IBDateTimeTypeMapping("DATE", IBDbType.Date);

		readonly IBTimeSpanTypeMapping _time = new IBTimeSpanTypeMapping("TIME", IBDbType.Time);

		readonly IBGuidTypeMapping _guid = new IBGuidTypeMapping();

		readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
		readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
		readonly HashSet<string> _disallowedMappings;

		public IBTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relationalDependencies)
			: base(dependencies, relationalDependencies)
		{
			_storeTypeMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
			{
				{ "BOOLEAN", _boolean },
				{ "SMALLINT", _smallint },
				{ "INTEGER", _integer },
				{ "BIGINT", _bigint },
				{ "CHAR", _char },
				{ "VARCHAR", _varchar },
				{ "BLOB SUB_TYPE TEXT", _clob },
				{ "BLOB SUB_TYPE BINARY", _binary },
				{ "FLOAT", _float },
				{ "DOUBLE PRECISION", _double },
				{ "DECIMAL", _decimal },
				{ "TIMESTAMP", _timestamp },
				{ "DATE", _date },
				{ "TIME", _time },
				{ "CHAR(16) CHARACTER SET OCTETS", _guid },
			};

			_clrTypeMappings = new Dictionary<Type, RelationalTypeMapping>()
			{
				{ typeof(bool), _boolean },
				{ typeof(short), _smallint },
				{ typeof(int), _integer },
				{ typeof(long), _bigint },
				{ typeof(float), _float },
				{ typeof(double), _double},
				{ typeof(decimal), _decimal },
				{ typeof(DateTime), _timestamp },
				{ typeof(TimeSpan), _time },
				{ typeof(Guid), _guid },
			};

			_disallowedMappings = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
					"CHARACTER",
					"CHAR",
					"VARCHAR",
					"CHARACTER VARYING",
					"CHAR VARYING",
			};
		}

		protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
		{
			return FindRawMapping(mappingInfo)?.Clone(mappingInfo) ?? base.FindMapping(mappingInfo);
		}

		protected override void ValidateMapping(CoreTypeMapping mapping, IProperty property)
		{
			var relationalMapping = mapping as RelationalTypeMapping;

			if (_disallowedMappings.Contains(relationalMapping?.StoreType))
			{
				if (property == null)
				{
					throw new ArgumentException($"Data type '{relationalMapping.StoreType}' is not supported in this form. Either specify the length explicitly in the type name or remove the data type and use APIs such as HasMaxLength.");
				}

				throw new ArgumentException($"Data type '{relationalMapping.StoreType}' for property '{property}' is not supported in this form. Either specify the length explicitly in the type name or remove the data type and use APIs such as HasMaxLength.");
			}
		}

		RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
		{
			var clrType = mappingInfo.ClrType;
			var storeTypeName = mappingInfo.StoreTypeName;
			var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

			if (storeTypeName != null)
			{
				if (clrType == typeof(float)
					&& mappingInfo.Size != null
					&& mappingInfo.Size <= 24
					&& (storeTypeNameBase.Equals("FLOAT", StringComparison.OrdinalIgnoreCase)
						|| storeTypeNameBase.Equals("DOUBLE PRECISION", StringComparison.OrdinalIgnoreCase)))
				{
					return _float;
				}

				if (_storeTypeMappings.TryGetValue(storeTypeName, out var mapping) || _storeTypeMappings.TryGetValue(storeTypeNameBase, out mapping))
				{
					return clrType == null || mapping.ClrType == clrType
						? mapping
						: null;
				}
			}

			if (clrType != null)
			{
				if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
				{
					return mapping;
				}

				if (clrType == typeof(string))
				{
					var isFixedLength = mappingInfo.IsFixedLength == true;
					var size = mappingInfo.Size ?? (mappingInfo.IsKeyOrIndex ? 256 : (int?)null);

					if (size > VarcharMaxSize)
					{
						size = isFixedLength ? VarcharMaxSize : (int?)null;
					}

					if (size == null)
					{
						return _clob;
					}
					else
					{
						if (!isFixedLength)
						{
							return new IBStringTypeMapping($"VARCHAR({size})", IBDbType.VarChar, size);
						}
						else
						{
							return new IBStringTypeMapping($"CHAR({size})", IBDbType.Char, size);
						}
					}
				}

				if (clrType == typeof(byte[]))
				{
					return _binary;
				}
			}

			return null;
		}
	}
}
