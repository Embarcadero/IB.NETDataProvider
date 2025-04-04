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
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace InterBaseSql.EntityFrameworkCore.InterBase.ValueGeneration.Internal;

public class IBValueGeneratorCache : ValueGeneratorCache, IIBValueGeneratorCache
{
	readonly ConcurrentDictionary<string, IBSequenceValueGeneratorState> _sequenceGeneratorCache = new ConcurrentDictionary<string, IBSequenceValueGeneratorState>();

	public IBValueGeneratorCache(ValueGeneratorCacheDependencies dependencies)
		: base(dependencies)
	{ }

	public virtual IBSequenceValueGeneratorState GetOrAddSequenceState(IProperty property, IRelationalConnection connection)
	{
		var tableIdentifier = StoreObjectIdentifier.Create(property.DeclaringType, StoreObjectType.Table);
		var sequence = tableIdentifier != null
			? property.FindHiLoSequence(tableIdentifier.Value)
			: property.FindHiLoSequence();

		return _sequenceGeneratorCache.GetOrAdd(
			GetSequenceName(sequence, connection),
			_ => new IBSequenceValueGeneratorState(sequence));
	}

	static string GetSequenceName(ISequence sequence, IRelationalConnection connection)
	{
		var dbConnection = connection.DbConnection;

		return dbConnection.Database.ToUpperInvariant()
			+ "::"
			+ dbConnection.DataSource.ToUpperInvariant()
			+ "::"
			+ (sequence.Schema == null ? "" : sequence.Schema + ".")
			+ sequence.Name;
	}
}
