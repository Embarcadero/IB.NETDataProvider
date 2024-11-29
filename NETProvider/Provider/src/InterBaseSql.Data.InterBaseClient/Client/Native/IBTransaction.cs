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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Text;
using System.Runtime.InteropServices;

using InterBaseSql.Data.Common;
using InterBaseSql.Data.Client.Native.Handles;
using System.Threading.Tasks;
using System.Threading;

namespace InterBaseSql.Data.Client.Native;

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

	#region Fields

	private TransactionHandle _handle;
	private IBDatabase _database;
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

	#endregion

	#region Constructors

	public IBTransaction(IBDatabase database)
	{
		_database = database;
		_handle = new TransactionHandle();
		State = TransactionState.NoTransaction;
		_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
	}

	#endregion

	#region Dispose2

	public override void Dispose2()
	{
		if (!_disposed)
		{
			_disposed = true;
			if (State != TransactionState.NoTransaction)
			{
				Rollback();
			}
			_database = null;
			_handle.Dispose();
			State = TransactionState.NoTransaction;
			_statusVector = null;
			base.Dispose2();
		}
	}
	public override async ValueTask Dispose2Async(CancellationToken cancellationToken = default)
	{
		if (!_disposed)
		{
			_disposed = true;
			if (State != TransactionState.NoTransaction)
			{
				await RollbackAsync(cancellationToken).ConfigureAwait(false);
			}
			_database = null;
			_handle.Dispose();
			State = TransactionState.NoTransaction;
			_statusVector = null;
			await base.Dispose2Async(cancellationToken).ConfigureAwait(false);
		}
	}

	#endregion

	#region Methods

	public override void BeginTransaction(TransactionParameterBuffer tpb)
	{
		if (State != TransactionState.NoTransaction)
		{
			throw new InvalidOperationException();
		}

		var teb = new IscTeb();
		var tebData = IntPtr.Zero;

		try
		{
			ClearStatusVector();

			// Very important do not change in furture
			var pSize = Environment.Is64BitProcess ? 8 : 4;
			teb.dbb_ptr = Marshal.AllocHGlobal(pSize);
			if (Environment.Is64BitProcess)
				Marshal.WriteInt64(teb.dbb_ptr, _database.HandlePtr.DangerousGetHandle().ToInt64());
			else
				Marshal.WriteInt32(teb.dbb_ptr, _database.Handle);

			teb.tpb_len = tpb.Length;

			teb.tpb_ptr = Marshal.AllocHGlobal(tpb.Length);
			Marshal.Copy(tpb.ToArray(), 0, teb.tpb_ptr, tpb.Length);

			var size = Marshal.SizeOf<IscTeb>();
			tebData = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(teb, tebData, true);

			_database.IBClient.isc_start_multiple(
				_statusVector,
				ref _handle,
				1,
				tebData);

			_database.ProcessStatusVector(_statusVector);

			State = TransactionState.Active;

			_database.TransactionCount++;
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
	public override ValueTask BeginTransactionAsync(TransactionParameterBuffer tpb, CancellationToken cancellationToken = default)
	{
		if (State != TransactionState.NoTransaction)
		{
			throw new InvalidOperationException();
		}

		var teb = new IscTeb();
		var tebData = IntPtr.Zero;

		try
		{
			ClearStatusVector();
			// Very important do not change in furture
			var pSize = Environment.Is64BitProcess ? 8 : 4;
			teb.dbb_ptr = Marshal.AllocHGlobal(pSize);
			if (Environment.Is64BitProcess)
				Marshal.WriteInt64(teb.dbb_ptr, _database.HandlePtr.DangerousGetHandle().ToInt64());
			else
				Marshal.WriteInt32(teb.dbb_ptr, _database.Handle);

			teb.tpb_len = tpb.Length;

			teb.tpb_ptr = Marshal.AllocHGlobal(tpb.Length);
			Marshal.Copy(tpb.ToArray(), 0, teb.tpb_ptr, tpb.Length);

			var size = Marshal.SizeOf<IscTeb>();
			tebData = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(teb, tebData, true);

			_database.IBClient.isc_start_multiple(
				_statusVector,
				ref _handle,
				1,
				tebData);

			_database.ProcessStatusVector(_statusVector);

			State = TransactionState.Active;

			_database.TransactionCount++;
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
		return ValueTask2.CompletedTask;
	}

	public override void Commit()
	{
		EnsureActiveTransactionState();

		ClearStatusVector();

		_database.IBClient.isc_commit_transaction(_statusVector, ref _handle);

		_database.ProcessStatusVector(_statusVector);

		_database.TransactionCount--;

		OnUpdate(EventArgs.Empty);

		State = TransactionState.NoTransaction;
	}
	public override ValueTask CommitAsync(CancellationToken cancellationToken = default)
	{
		EnsureActiveTransactionState();

		ClearStatusVector();

		_database.IBClient.isc_commit_transaction(_statusVector, ref _handle);

		_database.ProcessStatusVector(_statusVector);

		_database.TransactionCount--;

		OnUpdate(EventArgs.Empty);

		State = TransactionState.NoTransaction;
		return ValueTask2.CompletedTask;
	}

	public override void Rollback()
	{
		EnsureActiveTransactionState();

		ClearStatusVector();

		_database.IBClient.isc_rollback_transaction(_statusVector, ref _handle);
		_database.ProcessStatusVector(_statusVector);
		_database.TransactionCount--;

		OnUpdate(EventArgs.Empty);
		State = TransactionState.NoTransaction;
	}
	public override ValueTask RollbackAsync(CancellationToken cancellationToken = default)
	{
		EnsureActiveTransactionState();

		ClearStatusVector();

		_database.IBClient.isc_rollback_transaction(_statusVector, ref _handle);
		_database.ProcessStatusVector(_statusVector);
		_database.TransactionCount--;

		OnUpdate(EventArgs.Empty);
		State = TransactionState.NoTransaction;
		return ValueTask2.CompletedTask;
	}

	public override void CommitRetaining()
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		_database.IBClient.isc_commit_retaining(_statusVector, ref _handle);
		_database.ProcessStatusVector(_statusVector);
		State = TransactionState.Active;
	}
	public override ValueTask CommitRetainingAsync(CancellationToken cancellationToken = default)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		_database.IBClient.isc_commit_retaining(_statusVector, ref _handle);
		_database.ProcessStatusVector(_statusVector);
		State = TransactionState.Active;
		return ValueTask2.CompletedTask;
	}

	public override void RollbackRetaining()
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		_database.IBClient.isc_rollback_retaining(_statusVector, ref _handle);
		_database.ProcessStatusVector(_statusVector);
		State = TransactionState.Active;
	}
	public override ValueTask RollbackRetainingAsync(CancellationToken cancellationToken = default)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		_database.IBClient.isc_rollback_retaining(_statusVector, ref _handle);
		_database.ProcessStatusVector(_statusVector);
		State = TransactionState.Active;
		return ValueTask2.CompletedTask;
	}

	public override void StartSavepoint(string name)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		var spName = Encoding.ASCII.GetBytes(name);
		_database.IBClient.isc_start_savepoint(_statusVector, ref _handle, spName);
		_database.ProcessStatusVector(_statusVector);
	}
	public override ValueTask StartSavepointAsync(string name, CancellationToken cancellationToken = default)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		var spName = Encoding.ASCII.GetBytes(name);
		_database.IBClient.isc_start_savepoint(_statusVector, ref _handle, spName);
		_database.ProcessStatusVector(_statusVector);
		return ValueTask2.CompletedTask;
	}

	public override void RollbackSavepoint(string name)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		var spName = Encoding.ASCII.GetBytes(name);
		_database.IBClient.isc_rollback_savepoint(_statusVector, ref _handle, spName, 0);
		_database.ProcessStatusVector(_statusVector);
	}
	public override ValueTask RollbackSavepointAsync(string name, CancellationToken cancellationToken = default)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		var spName = Encoding.ASCII.GetBytes(name);
		_database.IBClient.isc_rollback_savepoint(_statusVector, ref _handle, spName, 0);
		_database.ProcessStatusVector(_statusVector);
		return ValueTask2.CompletedTask;
	}

	public override void ReleaseSavepoint(string name)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		var spName = Encoding.ASCII.GetBytes(name);
		_database.IBClient.isc_release_savepoint(_statusVector, ref _handle, spName);
		_database.ProcessStatusVector(_statusVector);
	}
	public override ValueTask ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default)
	{
		EnsureActiveTransactionState();
		ClearStatusVector();
		var spName = Encoding.ASCII.GetBytes(name);
		_database.IBClient.isc_release_savepoint(_statusVector, ref _handle, spName);
		_database.ProcessStatusVector(_statusVector);
		return ValueTask2.CompletedTask;
	}

	public override void Prepare()
	{ }
	public override ValueTask PrepareAsync(CancellationToken cancellationToken = default)
	{
		return ValueTask2.CompletedTask;
	}

	public override void Prepare(byte[] buffer)
	{ }
	public override ValueTask PrepareAsync(byte[] buffer, CancellationToken cancellationToken = default)
	{
		return ValueTask2.CompletedTask;
	}

	#endregion

	#region Private Methods

	private void TransactionInfo(byte[] items, byte[] buffer, int bufferLength)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);

		_database.IBClient.isc_transaction_info(
			_statusVector,
			ref _handle,
			(short)items.Length,
			items,
			(short)bufferLength,
			buffer);

		ProcessStatusVector();
	}

	private void ClearStatusVector()
	{
		Array.Clear(_statusVector, 0, _statusVector.Length);
	}

	private void ProcessStatusVector()
	{
		StatusVectorHelper.ProcessStatusVector(_statusVector, _database.Charset, _database.WarningMessage);
	}

	private void ProcessStatusVector(Charset charset)
	{
		StatusVectorHelper.ProcessStatusVector(_statusVector, charset, _database.WarningMessage);
	}

	#endregion
}