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

//$Authors = Siegfried Pammer (siegfried.pammer@gmail.com), Jiri Cincura (jiri@cincura.net)

using System;
using System.Data.Common;
using InterBaseSql.Data.InterBaseClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;

public class IBTimeSpanTypeMapping : TimeSpanTypeMapping
{
	readonly IBDbType _ibDbType;

	public IBTimeSpanTypeMapping(string storeType, IBDbType ibDbType)
	: base(storeType)
	{
		_ibDbType = ibDbType;
	}

	protected IBTimeSpanTypeMapping(RelationalTypeMappingParameters parameters, IBDbType ibDbType)
	: base(parameters)
	{
		_ibDbType = ibDbType;
	}

	protected override void ConfigureParameter(DbParameter parameter)
	{
		((IBParameter)parameter).IBDbType = _ibDbType;
	}

	protected override string GenerateNonNullSqlLiteral(object value)
	{
		switch (_ibDbType)
		{
			case IBDbType.Time:
				return $"CAST('{value:hh\\:mm\\:ss\\.ffff}' AS TIME)";
			default:
				throw new ArgumentOutOfRangeException(nameof(_ibDbType), $"{nameof(_ibDbType)}={_ibDbType}");
		}
	}

	protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
		=> new IBTimeSpanTypeMapping(parameters, _ibDbType);
}