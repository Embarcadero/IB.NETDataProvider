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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;

using InterBaseSql.Data.Common;
using InterBaseSql.Data.Client.Native.Handle;
using System.Text;

namespace InterBaseSql.Data.Client.Native
{
	internal sealed class IBTransaction : TransactionBase
	{
		#region Inner Structs

		[StructLayout(LayoutKind.Sequential)]
		public struct IscTeb
		{
			public IntPtr dbb_ptr;
			public int tpb_len;
			public IntPtr tpb_ptr;
		}

		#endregion

		#region Events

		public override event EventHandler Update;

		#endregion

		#region Fields

		private TransactionHandle _handle;
		private IBDatabase _db;
		private TransactionState _state;
		private bool _disposed;
		private IntPtr[] _statusVector;

		#endregion

		#region Properties

		public override int Handle
		{
			get { return _handle.DangerousGetHandle().AsInt(); }
		}

		public TransactionHandle HandlePtr
		{
			get { return _handle; }
		}

		public override TransactionState State
		{
			get { return _state; }
		}

		#endregion

		#region Constructors

		public IBTransaction(IDatabase db)
		{
			if (!(db is IBDatabase))
			{
				throw new ArgumentException($"Specified argument is not of {nameof(IBDatabase)} type.");
			}

			_db = (IBDatabase)db;
			_handle = new TransactionHandle();
			_state = TransactionState.NoTransaction;
			_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
		}

		#endregion

		#region IDisposable methods

		public override void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				if (_state != TransactionState.NoTransaction)
				{
					Rollback();
				}
				_db = null;
				_handle.Dispose();
				_state = TransactionState.NoTransaction;
				_statusVector = null;
				base.Dispose();
			}
		}

		#endregion

		#region Methods

		public override void BeginTransaction(TransactionParameterBuffer tpb)
		{
			if (_state != TransactionState.NoTransaction)
			{
				throw new InvalidOperationException();
			}

			var teb = new IscTeb();
			var tebData = IntPtr.Zero;

			try
			{
				ClearStatusVector();

				var pSize = Environment.Is64BitProcess ? 8 : 4;
				teb.dbb_ptr = Marshal.AllocHGlobal(pSize);
				if (Environment.Is64BitProcess)
					Marshal.WriteInt64(teb.dbb_ptr, _db.HandlePtr.DangerousGetHandle().ToInt64());
				else
					Marshal.WriteInt32(teb.dbb_ptr, _db.Handle);

				teb.tpb_len = tpb.Length;

				teb.tpb_ptr = Marshal.AllocHGlobal(tpb.Length);
				var b = tpb.ToArray();
				Marshal.Copy(tpb.ToArray(), 0, teb.tpb_ptr, tpb.Length);

				var size = Marshal.SizeOf<IscTeb>();
				tebData = Marshal.AllocHGlobal(size);

				Marshal.StructureToPtr(teb, tebData, true);

				_db.IBClient.isc_start_multiple(
					_statusVector,
					ref _handle,
					1,
					tebData);

				_db.ProcessStatusVector(_statusVector);

				_state = TransactionState.Active;

				_db.TransactionCount++;
			}
			finally
			{
				if (teb.dbb_ptr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(teb.dbb_ptr);
				}
				if (teb.tpb_ptr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(teb.tpb_ptr);
				}
				if (tebData != IntPtr.Zero)
				{
					Marshal.DestroyStructure<IscTeb>(tebData);
					Marshal.FreeHGlobal(tebData);
				}
			}
		}

		public override void Commit()
		{
			EnsureActiveTransactionState();

			ClearStatusVector();

			_db.IBClient.isc_commit_transaction(_statusVector, ref _handle);

			_db.ProcessStatusVector(_statusVector);

			_db.TransactionCount--;

			Update?.Invoke(this, new EventArgs());

			_state = TransactionState.NoTransaction;
		}

		public override void Rollback()
		{
			EnsureActiveTransactionState();

			ClearStatusVector();

			_db.IBClient.isc_rollback_transaction(_statusVector, ref _handle);

			_db.ProcessStatusVector(_statusVector);

			_db.TransactionCount--;

			Update?.Invoke(this, new EventArgs());

			_state = TransactionState.NoTransaction;
		}

		public override void CommitRetaining()
		{
			EnsureActiveTransactionState();

			ClearStatusVector();

			_db.IBClient.isc_commit_retaining(_statusVector, ref _handle);

			_db.ProcessStatusVector(_statusVector);

			_state = TransactionState.Active;
		}

		public override void RollbackRetaining()
		{
			EnsureActiveTransactionState();

			ClearStatusVector();

			_db.IBClient.isc_rollback_retaining(_statusVector, ref _handle);

			_db.ProcessStatusVector(_statusVector);

			_state = TransactionState.Active;
		}

		public override void StartSavepoint(string name)
		{
			EnsureActiveTransactionState();
			ClearStatusVector();
			var spName = Encoding.ASCII.GetBytes(name); 
			_db.IBClient.isc_start_savepoint(_statusVector, ref _handle, spName);
			_db.ProcessStatusVector(_statusVector);
		}

		public override void RollbackSavepoint(string name)
		{
			EnsureActiveTransactionState();
			ClearStatusVector();
			var spName = Encoding.ASCII.GetBytes(name);
			_db.IBClient.isc_rollback_savepoint(_statusVector, ref _handle, spName, 0);
			_db.ProcessStatusVector(_statusVector);
		}

		public override void ReleaseSavepoint(string name)
		{
			EnsureActiveTransactionState();
			ClearStatusVector();
			var spName = Encoding.ASCII.GetBytes(name);
			_db.IBClient.isc_release_savepoint(_statusVector, ref _handle, spName);
			_db.ProcessStatusVector(_statusVector);
		}

		#endregion

		#region Two Phase Commit Methods

		public override void Prepare()
		{ }

		public override void Prepare(byte[] buffer)
		{ }

		#endregion

		#region Private Methods

		private void ClearStatusVector()
		{
			Array.Clear(_statusVector, 0, _statusVector.Length);
		}

		#endregion
	}
}
