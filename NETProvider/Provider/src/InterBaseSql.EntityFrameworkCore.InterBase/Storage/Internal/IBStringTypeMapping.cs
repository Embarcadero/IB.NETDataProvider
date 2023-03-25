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

using System.Data;
using System.Data.Common;
using InterBaseSql.Data.InterBaseClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;

public class IBStringTypeMapping : StringTypeMapping
{
	readonly IBDbType _ibDbType;

	public IBStringTypeMapping(string storeType, DbType dbType, IBDbType ibDbType, int? size = null, bool unicode = true)
		: base(storeType, dbType, unicode: unicode, size: size)
	{
		_ibDbType = ibDbType;
	}

	protected IBStringTypeMapping(RelationalTypeMappingParameters parameters, IBDbType IBDbType)
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
		var svalue = value.ToString();
		return IsUnicode
			? $"'{EscapeSqlLiteral(svalue)}'"
			: $"'{EscapeSqlLiteral(svalue)}'";
	}

	protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
		=> new IBStringTypeMapping(parameters, _ibDbType);
}
