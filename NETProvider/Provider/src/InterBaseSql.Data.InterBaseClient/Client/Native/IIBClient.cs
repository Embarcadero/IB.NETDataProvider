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

//$Authors = Carlos Guzman Alvarez, Dean Harding, Jiri Cincura (jiri@cincura.net)

using System;
using System.Runtime.InteropServices;
using InterBaseSql.Data.Client.Native.Handle;
using InterBaseSql.Data.Client.Native.Marshalers;

namespace InterBaseSql.Data.Client.Native
{
	/// <summary>
	/// This is the interface that the dynamically-generated class uses to call the native library. 
	/// Each connection can specify different client library to use even on the same OS. 
	/// IIBClient and IBClientactory classes are implemented to support this feature.
	/// Public visibility added, because auto-generated assembly can't work with internal types
	/// </summary>
	///
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ibEventCallbackDelegate(IntPtr p, short Length, IntPtr updated);

	public interface IIBClient
	{
#pragma warning disable IDE1006
		string LibraryName();
		void LoadIBLibrary();
		void FreeIBLibrary();
		bool TryIBLoad();
		void CheckIBLoaded();
		string Platform { get; }
		string ServerType();
		decimal IBClientVersion { get; }
		short SQLDAVersion { get; }

		IntPtr isc_array_get_slice(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] destArray,
			ref int sliceLength);

		IntPtr isc_array_put_slice(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] sourceArray,
			ref int sliceLength);

		IntPtr isc_create_blob2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			[MarshalAs(UnmanagedType.I4)] ref BlobHandle blobHandle,
			ref long blobId,
			short bpbLength,
			byte[] bpbAddress);

		IntPtr isc_open_blob2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			[MarshalAs(UnmanagedType.I4)] ref BlobHandle blobHandle,
			ref long blobId,
			short bpbLength,
			byte[] bpbAddress);

		IntPtr isc_put_segment(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref BlobHandle blobHandle,
			short segBufferLength,
			byte[] segBuffer);

		IntPtr isc_cancel_blob(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref BlobHandle blobHandle);

		IntPtr isc_close_blob(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref BlobHandle blobHandle);

		IntPtr isc_attach_database(
			[In, Out] IntPtr[] statusVector,
			short dbNameLength,
			byte[] dbName,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			short parmBufferLength,
			byte[] parmBuffer);

		IntPtr isc_detach_database(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle);

		IntPtr isc_database_info(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			short itemListBufferLength,
			byte[] itemListBuffer,
			short resultBufferLength,
			byte[] resultBuffer);

		IntPtr isc_create_database(
			[In, Out] IntPtr[] statusVector,
			short dbNameLength,
			byte[] dbName,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			short parmBufferLength,
			byte[] parmBuffer,
			short dbType);

		IntPtr isc_drop_database(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle);

		IntPtr isc_start_multiple(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			short dbHandleCount,
			IntPtr tebVectorAddress);

		IntPtr isc_start_transaction(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			short dbHandleCount,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			short tpbLength,
			IntPtr tebVectorAddress);

		IntPtr isc_commit_transaction(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle);

		IntPtr isc_commit_retaining(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle);

		IntPtr isc_rollback_transaction(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle);

		IntPtr isc_rollback_retaining(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle);

		IntPtr isc_dsql_describe(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);

		IntPtr isc_dsql_sql_info(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short itemsLength,
			byte[] items,
			short bufferLength,
			byte[] buffer);

		IntPtr isc_service_attach(
			[In, Out] IntPtr[] statusVector,
			short serviceLength,
			string service,
			ref ServiceHandle svcHandle,
			short spbLength,
			byte[] spb);

		IntPtr isc_service_start(
			[In, Out] IntPtr[] statusVector,
			ref ServiceHandle svcHandle,
			ref ServiceHandle reserved,
			short spbLength,
			byte[] spb);

		IntPtr isc_service_detach(
			[In, Out] IntPtr[] statusVector,
			ref ServiceHandle svcHandle);

		IntPtr isc_service_query(
			[In, Out] IntPtr[] statusVector,
			ref ServiceHandle svcHandle,
			ref ServiceHandle reserved,
			short sendSpbLength,
			byte[] sendSpb,
			short requestSpbLength,
			byte[] requestSpb,
			short bufferLength,
			byte[] buffer);

		IntPtr isc_array_gen_sdl(
			[In, Out] IntPtr[] statusVector,
			IntPtr desc,
			ref short isc_arg3,
			byte[] isc_arg4,
			ref short isc_arg5
			);

		IntPtr isc_array_gen_sdl2(
			[In, Out] IntPtr[] statusVector,
			IntPtr desc,
			ref short isc_arg3,
			byte[] isc_arg4,
			ref short isc_arg5
			);

		IntPtr isc_array_get_slice2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			ref long array_id,
			IntPtr desc,
			byte[] dest_array,
			ref int sliceLength);

		IntPtr isc_array_lookup_bounds(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);

		IntPtr isc_array_lookup_bounds2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);

		IntPtr isc_array_lookup_desc(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);

		IntPtr isc_array_lookup_desc2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);

		IntPtr isc_array_set_desc(
			[In, Out] IntPtr[] statusVector,
			byte[] tableName,
				  byte[] columnName,
			ref short sqlDtype,
			ref short sqlLength,
			ref short sqlDimensions,
				  IntPtr desc);

		IntPtr isc_array_set_desc2(
			[In, Out] IntPtr[] statusVector,
			byte[] tableName,
			byte[] columnName,
			ref short sqlDtype,
			ref short sqlLength,
			ref short sqlDimensions,
			IntPtr desc);

		IntPtr isc_array_put_slice2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] sourceArray,
			ref int sliceLength);

		IntPtr isc_blob_default_desc(
			IntPtr desc,
			byte[] tableName,
			byte[] columnName);

		IntPtr isc_blob_default_desc2(
			IntPtr desc,
			byte[] tableName,
			byte[] columnName);

		IntPtr isc_blob_gen_bpb(
			[In, Out] IntPtr[] statusVector,
			IntPtr toDescriptor,
			IntPtr fromDescriptor,
				  short bpbBufferLength,
			byte[] bpbBuffer,
				  ref short bpb_length);

		IntPtr isc_blob_gen_bpb2(
			[In, Out] IntPtr[] statusVector,
			IntPtr toDescriptor,
			IntPtr fromDescriptor,
			short bpbBufferLength,
			byte[] bpbBuffer,
			ref short bpb_length);

		IntPtr isc_blob_info(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref BlobHandle blobHandle,
				  short item_list_buffer_length,
			byte[] item_list_buffer,
			short result_buffer_length,
			byte result_buffer);

		IntPtr isc_blob_lookup_desc(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] table_name,
			byte[] column_name,
			IntPtr descriptor,
			byte[] global);

		IntPtr isc_blob_lookup_desc2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] table_name,
			byte[] column_name,
			IntPtr descriptor,
			byte[] global);

		IntPtr isc_blob_set_desc(
			[In, Out] IntPtr[] statusVector,
			byte[] table_name,
			byte[] column_name,
			short subtype,
			short charset,
			short segment_size,
			IntPtr descriptor);

		IntPtr isc_blob_set_desc2(
			[In, Out] IntPtr[] statusVector,
			byte[] table_name,
			byte[] column_name,
			short subtype,
			short charset,
			short segment_size,
			IntPtr descriptor);

		void isc_decode_date(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		void isc_decode_sql_date(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		void isc_decode_sql_time(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		void isc_decode_timestamp(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		IntPtr isc_dsql_alloc_statement2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle);

		IntPtr isc_dsql_describe_bind(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);

		IntPtr isc_dsql_exec_immed2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr inXSQLDA,
			IntPtr outXSQLDA);

		IntPtr isc_dsql_execute(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);

		IntPtr isc_dsql_execute2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short da_version,
			IntPtr inXsqlda,
			IntPtr outXsqlda);

		IntPtr isc_dsql_batch_execute_immed(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			short dialect,
			ulong noOfSQL,
			ref byte[] statements,
			ref long rowsAffected);

		IntPtr isc_dsql_batch_execute(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short dialect,
			IntPtr inSQLDA,
			short noOfRows,
			IntPtr batchVars,
			ref ulong rowsAffected);

		IntPtr isc_dsql_execute_immediate(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr XSQLDA);

		IntPtr isc_dsql_fetch(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);

		IntPtr isc_dsql_free_statement(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short option);

		IntPtr isc_dsql_prepare(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr xsqlda);

		IntPtr isc_dsql_set_cursor_name(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref StatementHandle stmtHandle,
			byte[] cursorName,
			short _type);

		void isc_encode_date(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		void isc_encode_sql_date(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		void isc_encode_sql_time(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		void isc_encode_timestamp(
			ref long ibDate,
			[MarshalAs(UnmanagedType.Struct)] ref CTimeStructure tmDate);

		IntPtr isc_cancel_events(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			ref int eventID
			);

		IntPtr isc_que_events(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref DatabaseHandle dbHandle,
			ref int eventId,
			short length,
			IntPtr eventBuffer,
			ibEventCallbackDelegate eventFunction,
			IntPtr eventFunctionArg);

		int isc_event_block(
			ref IntPtr event_buffer,
			ref IntPtr result_buffer,
			ushort id_count,
			byte[][] event_list);

		void isc_event_counts(
			[In, Out] uint[] status,
			short bufferLength,
			IntPtr eventBuffer,
			IntPtr resultBuffer);

		long isc_free(IntPtr isc_arg1);

		IntPtr isc_get_segment(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref BlobHandle blobHandle,
			ref short actualSegLength,
			short segBufferLength,
			byte[] segBuffer);

		IntPtr isc_interprete(
			byte[] buffer,
			 [In, Out] ref IntPtr[] statusVector);

		IntPtr isc_prepare_transaction([In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle);

		IntPtr isc_prepare_transaction2(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			short msgLength,
			byte[] msg);

		IntPtr isc_release_savepoint(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
				  byte[] savepointName);

		IntPtr isc_rollback_savepoint(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] savepointName,
			short option);

		IntPtr isc_start_savepoint(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			byte[] savepointName);

		long isc_sqlcode([In, Out] IntPtr[] statusVector);

		void isc_sql_interprete(
			short sqlcode,
			byte[] buffer,
			short bufferLength);

		IntPtr isc_transaction_info(
			[In, Out] IntPtr[] statusVector,
			[MarshalAs(UnmanagedType.I4)] ref TransactionHandle trHandle,
			short itemListBufferLength,
			byte[] itemListBuffer,
			short result_buffer_length,
			byte[] result_buffer);

		long isc_vax_integer(
			byte[] buffer,
			short length);

		long isc_portable_integer(
			byte[] buffer,
			short length);

		void isc_get_client_version(byte[] buffer);

		int isc_get_client_major_version();

		int isc_get_client_minor_version();

#pragma warning restore IDE1006
	}
}
