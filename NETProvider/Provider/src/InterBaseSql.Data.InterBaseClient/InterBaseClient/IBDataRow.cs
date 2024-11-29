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

//$Authors = Jeff Overcash

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient;

public class IBDataRow : DataRow
{
	private readonly DataColumnCollection _csColumns;
	private Dictionary<DataColumn, IBChangeState> _changeStates;

	internal IBDataRow(DataRowBuilder builder) : base(builder)
	{
		_csColumns = Table.Columns;
		_changeStates = new Dictionary<DataColumn, IBChangeState>();
	}

	public IBChangeState ChangeState(int columnIndex)
	{
		DataColumn column = _csColumns[columnIndex];
		IBChangeState result;
		if (!_changeStates.TryGetValue(column, out result))
		{
			return IBChangeState.csUnknown;
		}
		else
		{
		return result;
		}
	}

	public IBChangeState ChangeState(string columnName)
	{
		DataColumn column = _csColumns[columnName];
		IBChangeState result;
		if (!_changeStates.TryGetValue(column, out result))
		{
			return IBChangeState.csUnknown;
		}
		else
		{
			return result;
		}
	}

	public IBChangeState ChangeState(DataColumn column)
	{
		IBChangeState result;
		if (!_changeStates.TryGetValue(column, out result))
		{
			return IBChangeState.csUnknown;
		}
		else
		{
			return result;
		}
	}

	internal void SetChangeState(DataColumn column, IBChangeState state)
	{
		_changeStates.Add(column, state);
	}

	internal void SetChangeState(string columnName, IBChangeState state)
	{
		_changeStates.Add(_csColumns[columnName], state);
	}
}
