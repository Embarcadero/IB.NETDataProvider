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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Text;
using System.Data;
//using InterBaseSql.Data.Client.Managed;

namespace InterBaseSql.Data.Common
{
	internal abstract class StatementBase : IDisposable
	{
		#region Protected Static Fields

		protected static readonly byte[] DescribePlanInfoItems = new byte[]
		{
			IscCodes.isc_info_sql_get_plan,
		};

		protected static readonly byte[] RowsAffectedInfoItems = new byte[]
		{
			IscCodes.isc_info_sql_records,
		};

		protected static readonly byte[] DescribeInfoAndBindInfoItems = new byte[]
		{
			IscCodes.isc_info_sql_select,
			IscCodes.isc_info_sql_describe_vars,
			IscCodes.isc_info_sql_sqlda_seq,
			IscCodes.isc_info_sql_type,
			IscCodes.isc_info_sql_sub_type,
			IscCodes.isc_info_sql_length,
			IscCodes.isc_info_sql_scale,
			IscCodes.isc_info_sql_field,
			IscCodes.isc_info_sql_relation,
			// IscCodes.isc_info_sql_owner,
			IscCodes.isc_info_sql_alias,
			IscCodes.isc_info_sql_describe_end,

			IscCodes.isc_info_sql_bind,
			IscCodes.isc_info_sql_describe_vars,
			IscCodes.isc_info_sql_sqlda_seq,
			IscCodes.isc_info_sql_type,
			IscCodes.isc_info_sql_sub_type,
			IscCodes.isc_info_sql_length,
			IscCodes.isc_info_sql_scale,
			IscCodes.isc_info_sql_field,
			IscCodes.isc_info_sql_relation,
			// IscCodes.isc_info_sql_owner,
			IscCodes.isc_info_sql_alias,
			IscCodes.isc_info_sql_describe_end,
		};

		protected static readonly byte[] StatementTypeInfoItems = new byte[]
		{
			IscCodes.isc_info_sql_stmt_type,
		};

		#endregion

		#region Protected Fields

		protected EventHandler TransactionUpdate;

		#endregion

		#region Properties

		public abstract IDatabase Database { get; }
		public abstract TransactionBase Transaction { get; set; }
		public abstract Descriptor Parameters { get; set; }
		public abstract Descriptor Fields { get; }
		public abstract int FetchSize { get; set; }

		public DbStatementType StatementType { get; protected set; } = DbStatementType.None;
		public StatementState State { get; protected set; } = StatementState.Deallocated;
		public int RecordsAffected { get; protected set; } = -1;

		public bool ReturnRecordsAffected { get; set; }

		public bool IsPrepared => !(State == StatementState.Deallocated || State == StatementState.Error);
		public bool DoRecordsAffected => ReturnRecordsAffected
			&& (StatementType == DbStatementType.Insert
				|| StatementType == DbStatementType.Delete
				|| StatementType == DbStatementType.Update
				|| StatementType == DbStatementType.StoredProcedure
				|| StatementType == DbStatementType.Select);

		#endregion

		#region IDisposable methods

		public virtual void Dispose()
		{ }

		#endregion

		#region Methods

		public string GetExecutionPlan()
		{
			return GetPlanInfo(DescribePlanInfoItems);
		}

		public virtual void Close()
		{
			if (State == StatementState.Executed ||
				State == StatementState.Error)
			{
				if (StatementType == DbStatementType.Select ||
					StatementType == DbStatementType.SelectForUpdate ||
					StatementType == DbStatementType.StoredProcedure)
				{
					if (State == StatementState.Allocated ||
						State == StatementState.Prepared ||
						State == StatementState.Executed)
					{
						try
						{
							Free(IscCodes.DSQL_close);
						}
						catch
						{ }
					}
					ClearArrayHandles();
					State = StatementState.Closed;
				}
			}
		}

		public virtual void Release()
		{
			if (Transaction != null && TransactionUpdate != null)
			{
				Transaction.Update -= TransactionUpdate;
				TransactionUpdate = null;
			}

			Free(IscCodes.DSQL_drop);

			ClearArrayHandles();
			State = StatementState.Deallocated;
			StatementType = DbStatementType.None;
		}

		#endregion

		#region Abstract Methods

		public abstract void Describe();
		public abstract void DescribeParameters();
		public abstract void Prepare(string commandText);
		public abstract void Execute();
		public abstract DbValue[] Fetch();
		public abstract DbValue[] GetOutputParameters();

		public abstract BlobBase CreateBlob();
		public abstract BlobBase CreateBlob(long handle);

		public abstract ArrayBase CreateArray(ArrayDesc descriptor);
		public abstract ArrayBase CreateArray(string tableName, string fieldName);
		public abstract ArrayBase CreateArray(long handle, string tableName, string fieldName);

		#endregion

		#region Protected Abstract Methods

		protected abstract void TransactionUpdated(object sender, EventArgs e);
		protected abstract byte[] GetSqlInfo(byte[] items, int bufferLength);
		protected abstract void Free(int option);

		#endregion

		#region Protected Methods

		protected byte[] GetSqlInfo(byte[] items)
		{
			return GetSqlInfo(items, IscCodes.DEFAULT_MAX_BUFFER_SIZE);
		}

		protected int GetRecordsAffected()
		{
			var buffer = GetSqlInfo(RowsAffectedInfoItems, IscCodes.ROWS_AFFECTED_BUFFER_SIZE);
			return ProcessRecordsAffectedBuffer(buffer);
		}

		protected int ProcessRecordsAffectedBuffer(byte[] buffer)
		{
			var insertCount = 0;
			var updateCount = 0;
			var deleteCount = 0;
			//var selectCount = 0;
			var pos = 0;

			int type;
			while ((type = buffer[pos++]) != IscCodes.isc_info_end)
			{
				var length = (int)IscHelper.VaxInteger(buffer, pos, 2);
				pos += 2;
				switch (type)
				{
					case IscCodes.isc_info_sql_records:
						int t;
						while ((t = buffer[pos++]) != IscCodes.isc_info_end)
						{
							var l = (int)IscHelper.VaxInteger(buffer, pos, 2);
							pos += 2;
							switch (t)
							{
								case IscCodes.isc_info_req_insert_count:
									insertCount = (int)IscHelper.VaxInteger(buffer, pos, l);
									break;
								case IscCodes.isc_info_req_update_count:
									updateCount = (int)IscHelper.VaxInteger(buffer, pos, l);
									break;
								case IscCodes.isc_info_req_delete_count:
									deleteCount = (int)IscHelper.VaxInteger(buffer, pos, l);
									break;
								// Select count is always 42 (I think).  anyways it is not correct for several reasons
								//case IscCodes.isc_info_req_select_count:
								//	selectCount = (int)IscHelper.VaxInteger(buffer, pos, l);
								//	break;
							}
							pos += l;
						}
						break;
					default:
						pos += length;
						break;
				}
			}

			return insertCount + updateCount + deleteCount;
		}

		protected DbStatementType GetStatementType()
		{
			var buffer = GetSqlInfo(StatementTypeInfoItems, IscCodes.STATEMENT_TYPE_BUFFER_SIZE);
			return ProcessStatementTypeInfoBuffer(buffer);
		}

		protected DbStatementType ProcessStatementTypeInfoBuffer(byte[] buffer)
		{
			var stmtType = DbStatementType.None;
			var pos = 0;
			int length;
			byte type;

			while ((type = buffer[pos++]) != IscCodes.isc_info_end)
			{
				length = (int)IscHelper.VaxInteger(buffer, pos, 2);
				pos += 2;
				switch (type)
				{
					case IscCodes.isc_info_sql_stmt_type:
						stmtType = (DbStatementType)IscHelper.VaxInteger(buffer, pos, length);
						pos += length;
						break;

					default:
						pos += length;
						break;
				}
			}

			return stmtType;
		}

		protected void ClearArrayHandles()
		{
			if (Fields != null && Fields.Count > 0)
			{
				for (var i = 0; i < Fields.Count; i++)
				{
					if (Fields[i].IsArray())
					{
						Fields[i].ArrayHandle = null;
					}
				}
			}
		}

		protected string GetPlanInfo(byte[] planInfoItems)
		{
			var count = 0;
			var bufferSize = IscCodes.DEFAULT_MAX_BUFFER_SIZE;
			var buffer = GetSqlInfo(planInfoItems, bufferSize);

			if (buffer[0] == IscCodes.isc_info_end)
			{
				return string.Empty;
			}

			while (buffer[0] == IscCodes.isc_info_truncated && count < 4)
			{
				bufferSize *= 2;
				buffer = GetSqlInfo(planInfoItems, bufferSize);
				count++;
			}
			if (count > 3)
			{
				return null;
			}

			int len = buffer[1];
			len += buffer[2] << 8;
			if (len > 0)
			{
				return Database.Charset.GetString(buffer, 4, --len);
			}
			else
			{
				return string.Empty;
			}
		}

		#endregion
	}
}
