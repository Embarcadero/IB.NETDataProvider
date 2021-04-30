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

using InterBaseSql.Data.Common;
using InterBaseSql.Data.Client.Native.Handle;

namespace InterBaseSql.Data.Client.Native
{
	internal sealed class IBBlob : BlobBase
	{
		#region Fields

		private IBDatabase _db;
		private IntPtr[] _statusVector;
		private BlobHandle _blobHandle;

		#endregion

		#region Properties

		public override IDatabase Database
		{
			get { return _db; }
		}

		public override int Handle
		{
			get { return _blobHandle.DangerousGetHandle().AsInt(); }
		}

		#endregion

		#region Constructors

		public IBBlob(IDatabase db, TransactionBase transaction)
			: this(db, transaction, 0)
		{
		}

		public IBBlob(IDatabase db, TransactionBase transaction, long blobId)
			: base(db)
		{
			if (!(db is IBDatabase))
			{
				throw new ArgumentException($"Specified argument is not of {nameof(IBDatabase)} type.");
			}
			if (!(transaction is IBTransaction))
			{
				throw new ArgumentException($"Specified argument is not of {nameof(IBTransaction)} type.");
			}

			_db = (IBDatabase)db;
			_transaction = (IBTransaction)transaction;
			_position = 0;
			_blobHandle = new BlobHandle();
			_blobId = blobId;
			_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
		}

		#endregion

		#region Protected Methods

		protected override void Create()
		{
			ClearStatusVector();

			var dbHandle = _db.HandlePtr;
			var trHandle = ((IBTransaction)_transaction).HandlePtr;

			_db.IBClient.isc_create_blob2(
				_statusVector,
				ref dbHandle,
				ref trHandle,
				ref _blobHandle,
				ref _blobId,
				0,
				new byte[0]);

			_db.ProcessStatusVector(_statusVector);

			RblAddValue(IscCodes.RBL_create);
		}

		protected override void Open()
		{
			ClearStatusVector();

			var dbHandle = _db.HandlePtr;
			var trHandle = ((IBTransaction)_transaction).HandlePtr;

			_db.IBClient.isc_open_blob2(
				_statusVector,
				ref dbHandle,
				ref trHandle,
				ref _blobHandle,
				ref _blobId,
				0,
				new byte[0]);

			_db.ProcessStatusVector(_statusVector);
		}

		protected override void GetSegment(Stream stream)
		{
			var requested = (short)SegmentSize;
			short segmentLength = 0;

			ClearStatusVector();

			var tmp = new byte[requested];

			var status = _db.IBClient.isc_get_segment(
				_statusVector,
				ref _blobHandle,
				ref segmentLength,
				requested,
				tmp);


			RblRemoveValue(IscCodes.RBL_segment);

			if (_statusVector[1] == new IntPtr(IscCodes.isc_segstr_eof))
			{
				RblAddValue(IscCodes.RBL_eof_pending);
				return;
			}
			else
			{
				if (status == IntPtr.Zero || _statusVector[1] == new IntPtr(IscCodes.isc_segment))
				{
					RblAddValue(IscCodes.RBL_segment);
				}
				else
				{
					_db.ProcessStatusVector(_statusVector);
				}
			}

			stream.Write(tmp, 0, segmentLength);
		}

		protected override void PutSegment(byte[] buffer)
		{
			ClearStatusVector();

			_db.IBClient.isc_put_segment(
				_statusVector,
				ref _blobHandle,
				(short)buffer.Length,
				buffer);

			_db.ProcessStatusVector(_statusVector);
		}

		protected override void Seek(int position)
		{
			throw new NotSupportedException();
		}

		protected override void GetBlobInfo()
		{
			throw new NotSupportedException();
		}

		protected override void Close()
		{
			ClearStatusVector();

			_db.IBClient.isc_close_blob(_statusVector, ref _blobHandle);

			_db.ProcessStatusVector(_statusVector);
		}

		protected override void Cancel()
		{
			ClearStatusVector();

			_db.IBClient.isc_cancel_blob(_statusVector, ref _blobHandle);

			_db.ProcessStatusVector(_statusVector);
		}

		#endregion

		#region Private Methods

		private void ClearStatusVector()
		{
			Array.Clear(_statusVector, 0, _statusVector.Length);
		}

		#endregion
	}
}
