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
using System.Data.Common;
using InterBaseSql.Data.InterBaseClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal
{
	public class IBDateTimeTypeMapping : DateTimeTypeMapping
	{
		readonly IBDbType _ibDbType;

		public IBDateTimeTypeMapping(string storeType, IBDbType IBDbType)
			: base(storeType)
		{
			_ibDbType = IBDbType;
		}

		protected IBDateTimeTypeMapping(RelationalTypeMappingParameters parameters, IBDbType IBDbType)
			: base(parameters)
		{
			_ibDbType = IBDbType;
		}

		protected override void ConfigureParameter(DbParameter parameter)
		{
			((IBParameter)parameter).IBDbType = _ibDbType;
		}

		protected override string GenerateNonNullSqlLiteral(object value)
		{
			switch (_ibDbType)
			{
				case IBDbType.TimeStamp:
					return $"CAST('{value:yyyy-MM-dd HH:mm:ss}' AS TIMESTAMP)";
				case IBDbType.Date:
					return $"CAST('{value:yyyy-MM-dd}' AS DATE)";
				case IBDbType.Time:
					return $"CAST('{value:HH:mm:ss}' AS TIME)";
				default:
					throw new ArgumentOutOfRangeException(nameof(_ibDbType), $"{nameof(_ibDbType)}={_ibDbType}");
			}
		}

		protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
			=> new IBDateTimeTypeMapping(parameters, _ibDbType);
	}
}
