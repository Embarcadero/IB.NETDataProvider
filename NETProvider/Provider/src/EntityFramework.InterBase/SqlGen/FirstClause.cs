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
using System.Globalization;

namespace EntityFramework.InterBase.SqlGen
{
	internal class FirstClause : ISqlFragment
	{
		#region Fields

		private ISqlFragment _firstCount;
		private int _firstValue;

		#endregion

		#region Internal Properties

		/// <summary>
		/// How many first rows should be selected.
		/// </summary>
		internal ISqlFragment FirstCount
		{
			get { return _firstCount; }
		}

		public int FirstValue
		{
			get { return _firstValue; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a FirstClause with the given topCount and withTies.
		/// </summary>
		/// <param name="topCount"></param>
		internal FirstClause(ISqlFragment firstCount)
		{
			_firstCount = firstCount;
			var sb = firstCount as SqlBuilder;
			if (sb != null)
			    if (!sb.IsEmpty)
				    _firstValue = Int32.Parse(sb.FirstElement);
		}

		/// <summary>
		/// Creates a TopClause with the given topCount and withTies.
		/// </summary>
		/// <param name="topCount"></param>
		internal FirstClause(int firstCount)
		{
			var sqlBuilder = new SqlBuilder();
			sqlBuilder.Append(firstCount.ToString(CultureInfo.InvariantCulture));
			_firstCount = sqlBuilder;
			_firstValue = firstCount;
		}

		#endregion

		#region ISqlFragment Members

		/// <summary>
		/// Write out the FIRST part of sql select statement
		/// It basically writes FIRST (X).
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="sqlGenerator"></param>
		public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
		{
			writer.Write(" ROWS ");
			FirstCount.WriteSql(writer, sqlGenerator);

			writer.Write(" ");
		}

		#endregion
	}
}
