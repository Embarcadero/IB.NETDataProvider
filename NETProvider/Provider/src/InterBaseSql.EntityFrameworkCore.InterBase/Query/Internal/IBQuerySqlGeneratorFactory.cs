﻿/*
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

using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;

public class IBQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
{
	readonly QuerySqlGeneratorDependencies _dependencies;
	readonly IIBOptions _ibOptions;

	public IBQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IIBOptions ibOptions)
	{
		_dependencies = dependencies;
		_ibOptions = ibOptions;
	}

	public QuerySqlGenerator Create()
		  => new IBQuerySqlGenerator(_dependencies, _ibOptions);
}
