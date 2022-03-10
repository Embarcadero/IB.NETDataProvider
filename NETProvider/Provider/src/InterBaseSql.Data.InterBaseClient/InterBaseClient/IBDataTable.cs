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

//$Authors = Jeff Overcash

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient
{
	public class IBDataTable : DataTable
	{
		private bool _changeStateColumns = false;
		private DataColumn[] _changeColumns;

		protected override Type GetRowType()
		{
			return typeof(IBDataRow);
		}

		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new IBDataRow(builder);
		}

		public IBDataRow this[int idx]
		{
			get { return (IBDataRow)Rows[idx]; }
		}

		public void Add(IBDataRow row)
		{
			Rows.Add(row);
		}

		public void Remove(IBDataRow row)
		{
			Rows.Remove(row);
		}

		public IBDataRow GetNewRow()
		{
			IBDataRow row = (IBDataRow)NewRow();
                        foreach (DataColumn column in Columns)
			{
				row.SetChangeState(column, Common.IBChangeState.csUnknown);
			}
			return row;
		}

		public bool ChangeStateColumns
		{
			get { return _changeStateColumns; }

			set
			{
				// Do nothing cause data has not been filled
				if (value && (Columns.Count == 0) )
				{
					_changeStateColumns = false;					
				}
				else
				// turning it on.  Build the new columns and fill the data
				if (value && !_changeStateColumns)
				{
					_changeStateColumns = value;
					_changeColumns = new DataColumn[Columns.Count];
					for (int i = Columns.Count - 1; i >= 0; i--)
					{
						var col = Columns.Add(Columns[i].ColumnName + " ChangeState", typeof(IBChangeState));
						col.SetOrdinal(i + 1);
						_changeColumns[i] = col;
					}
					IBDataRow aRow;
					for (int i = 0; i < Rows.Count; i++)
					{
						aRow = this[i];
						for (int j = 0; j < _changeColumns.Length; j++)
							aRow[(2 * j) + 1] = (IBChangeState) aRow.ChangeState(2 * j);
					}
					for (int j = 0; j < _changeColumns.Length; j++)
						_changeColumns[j].ReadOnly = true;
				}
				else
				if (!value)
				{
					_changeStateColumns = value;
					foreach (DataColumn col in _changeColumns)
						Columns.Remove(col);
					_changeColumns = null;
				}
			}
		}
	}
}
