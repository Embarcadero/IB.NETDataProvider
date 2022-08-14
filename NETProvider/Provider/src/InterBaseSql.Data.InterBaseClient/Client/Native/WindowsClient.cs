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

//$Authors = Embarcadero, Jeff Overcash

using System;
using System.Runtime.InteropServices;
using InterBaseSql.Data.Client.Native.Handle;
using InterBaseSql.Data.Client.Native.Marshalers;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.Client.Native
{
	internal static class UnsafeNativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
	}


	class WindowsClient : IIBClient
	{

		//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void Tisc_get_client_version(byte[] buffer);
		internal delegate int Tisc_get_client_major_version();
		internal delegate int Tisc_get_client_minor_version();
		internal delegate void Tisc_sql_interprete(short sqlcode, byte[] buffer, short bufferLength);
		internal delegate IntPtr Tisc_array_get_slice(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] destArray,
			ref int sliceLength);
		internal delegate IntPtr Tisc_array_put_slice(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] sourceArray,
			ref int sliceLength);

		internal delegate IntPtr Tisc_create_blob2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref BlobHandle blobHandle,
			ref long blobId,
			short bpbLength,
			byte[] bpbAddress);
		internal delegate IntPtr Tisc_open_blob2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref BlobHandle blobHandle,
			ref long blobId,
			short bpbLength,
			byte[] bpbAddress);
		internal delegate IntPtr Tisc_get_segment(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle,
			ref short actualSegLength,
			short segBufferLength,
			byte[] segBuffer);
		internal delegate IntPtr Tisc_put_segment(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle,
			short segBufferLength,
			byte[] segBuffer);
		internal delegate IntPtr Tisc_cancel_blob(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle);
		internal delegate IntPtr Tisc_close_blob(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle);
		internal delegate IntPtr Tisc_attach_database(
			[In, Out] IntPtr[] statusVector,
			short dbNameLength,
			byte[] dbName,
			ref DatabaseHandle dbHandle,
			short parmBufferLength,
			byte[] parmBuffer);
		internal delegate long Tisc_sqlcode([In, Out] IntPtr[] statusVector);
		internal delegate IntPtr Tisc_interprete(byte[] buffer, [In, Out] ref IntPtr[] statusVector);
		internal delegate long Tisc_vax_integer(byte[] buffer, short length);
		internal delegate long Tisc_portable_integer(byte[] buffer, short length);
		internal delegate IntPtr Tisc_blob_info(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle,
			short item_list_buffer_length,
			byte[] item_list_buffer,
			short result_buffer_length,
			byte result_buffer);
		internal delegate IntPtr Tisc_create_database(
			[In, Out] IntPtr[] statusVector,
			short dbNameLength,
			byte[] dbName,
			ref DatabaseHandle dbHandle,
			short parmBufferLength,
			byte[] parmBuffer,
			short dbType);
		internal delegate IntPtr Tisc_array_gen_sdl(
			[In, Out] IntPtr[] statusVector,
			IntPtr desc,
			ref short isc_arg3,
			byte[] isc_arg4,
			ref short isc_arg5);
		internal delegate IntPtr Tisc_array_lookup_bounds(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);
		internal delegate IntPtr Tisc_array_lookup_desc(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);
		internal delegate IntPtr Tisc_array_set_desc(
			[In, Out] IntPtr[] statusVector,
			byte[] tableName,
				  byte[] columnName,
			ref short sqlDtype,
			ref short sqlLength,
			ref short sqlDimensions,
				  IntPtr desc);
		internal delegate IntPtr Tisc_blob_default_desc(IntPtr desc, byte[] tableName, byte[] columnName);
		internal delegate IntPtr Tisc_blob_gen_bpb(
			[In, Out] IntPtr[] statusVector,
			IntPtr toDescriptor,
			IntPtr fromDescriptor,
					short bpbBufferLength,
			byte[] bpbBuffer,
					ref short bpb_length);
		internal delegate IntPtr Tisc_blob_lookup_desc(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] table_name,
			byte[] column_name,
			IntPtr descriptor,
			byte[] global);
		internal delegate IntPtr Tisc_blob_set_desc(
			[In, Out] IntPtr[] statusVector,
			byte[] table_name,
			byte[] column_name,
			short subtype,
			short charset,
			short segment_size,
			IntPtr descriptor);
		internal delegate void Tisc_decode_date(
			ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate void Tisc_encode_date(
			ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate IntPtr Tisc_dsql_free_statement(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short option);
		internal delegate IntPtr Tisc_dsql_execute2(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short da_version,
			IntPtr inXsqlda,
			IntPtr outXsqlda);
		internal delegate IntPtr Tisc_dsql_execute(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);
		internal delegate IntPtr Tisc_dsql_set_cursor_name(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			byte[] cursorName,
			short _type);
		internal delegate IntPtr Tisc_dsql_fetch(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);
		internal delegate IntPtr Tisc_dsql_sql_info(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short itemsLength,
			byte[] items,
			short bufferLength,
			byte[] buffer);
		internal delegate IntPtr Tisc_dsql_alloc_statement2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref StatementHandle stmtHandle);
		internal delegate IntPtr Tisc_dsql_prepare(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr xsqlda);
		internal delegate IntPtr Tisc_dsql_describe_bind(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);
		internal delegate IntPtr Tisc_dsql_exec_immed2(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr inXSQLDA,
			IntPtr outXSQLDA);
		internal delegate IntPtr Tisc_dsql_describe(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda);
		internal delegate IntPtr Tisc_dsql_execute_immediate(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr XSQLDA);
		internal delegate IntPtr Tisc_drop_database(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle);
		internal delegate IntPtr Tisc_detach_database(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle);
		internal delegate IntPtr Tisc_database_info(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			short itemListBufferLength,
			byte[] itemListBuffer,
			short resultBufferLength,
			byte[] resultBuffer);
		internal delegate IntPtr Tisc_start_multiple(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short dbHandleCount,
			IntPtr tebVectorAddress);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate IntPtr Tisc_start_transaction(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short dbHandleCount,
			ref DatabaseHandle dbHandle,
			short tpbLength,
			IntPtr tebVectorAddress);
		internal delegate IntPtr Tisc_commit_transaction(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle);
		internal delegate IntPtr Tisc_commit_retaining(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle);
		internal delegate IntPtr Tisc_rollback_transaction(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle);
		internal delegate IntPtr Tisc_rollback_retaining(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle);

		internal delegate IntPtr Tisc_cancel_events(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref int eventID);
		internal delegate IntPtr Tisc_que_events(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref int eventId,
			short length,
			IntPtr eventBuffer,
			ibEventCallbackDelegate eventFunction,
			IntPtr eventFunctionArg);
		internal delegate void Tisc_event_counts(
			[In, Out] uint[] status,
			short bufferLength,
			IntPtr eventBuffer,
			IntPtr resultBuffer);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int Tisc_event_block(
			ref IntPtr event_buffer,
			ref IntPtr result_buffer,
			ushort id_count,
			byte[] v1, byte[] v2, byte[] v3, byte[] v4, byte[] v5, byte[] v6, byte[] v7,
			byte[] v8, byte[] v9, byte[] v10, byte[] v11, byte[] v12, byte[] v13, byte[] v14, byte[] v15);
		internal delegate long Tisc_free(IntPtr isc_arg1);
		internal delegate IntPtr Tisc_prepare_transaction([In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle);
		internal delegate IntPtr Tisc_prepare_transaction2(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short msgLength,
			byte[] msg);
		internal delegate IntPtr Tisc_transaction_info(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short itemListBufferLength,
			byte[] itemListBuffer,
			short result_buffer_length,
			byte[] result_buffer);
		internal delegate IntPtr Tisc_service_attach(
			[In, Out] IntPtr[] statusVector,
			short serviceLength,
			string service,
			ref ServiceHandle svcHandle,
			short spbLength,
			byte[] spb);
		internal delegate IntPtr Tisc_service_detach([In, Out] IntPtr[] statusVector, ref ServiceHandle svcHandle);
		internal delegate IntPtr Tisc_service_query(
			[In, Out] IntPtr[] statusVector,
			ref ServiceHandle svcHandle,
			ref ServiceHandle reserved,
			short sendSpbLength,
			byte[] sendSpb,
			short requestSpbLength,
			byte[] requestSpb,
			short bufferLength,
			byte[] buffer);
		internal delegate IntPtr Tisc_service_start(
			[In, Out] IntPtr[] statusVector,
			ref ServiceHandle svcHandle,
			ref ServiceHandle reserved,
			short spbLength,
			byte[] spb);
		internal delegate void Tisc_decode_sql_date(
			ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate void Tisc_decode_sql_time(
			ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate void Tisc_decode_timestamp(
		 	ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate void Tisc_encode_sql_date(
			ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate void Tisc_encode_sql_time(
			ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate void Tisc_encode_timestamp(
			ref long ibDate,
			ref CTimeStructure tmDate);
		internal delegate IntPtr Tisc_array_gen_sdl2(
			[In, Out] IntPtr[] statusVector,
			IntPtr desc,
			ref short isc_arg3,
			byte[] isc_arg4,
			ref short isc_arg5);
		internal delegate IntPtr Tisc_array_get_slice2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref long array_id,
			IntPtr desc,
			byte[] dest_array,
			ref int sliceLength);
		internal delegate IntPtr Tisc_array_lookup_bounds2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);
		internal delegate IntPtr Tisc_array_lookup_desc2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc);
		internal delegate IntPtr Tisc_array_set_desc2(
			[In, Out] IntPtr[] statusVector,
			byte[] tableName,
			byte[] columnName,
			ref short sqlDtype,
			ref short sqlLength,
			ref short sqlDimensions,
			IntPtr desc);
		internal delegate IntPtr Tisc_array_put_slice2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] sourceArray,
			ref int sliceLength);
		internal delegate IntPtr Tisc_blob_default_desc2(
			IntPtr desc,
			byte[] tableName,
			byte[] columnName);
		internal delegate IntPtr Tisc_blob_gen_bpb2(
			[In, Out] IntPtr[] statusVector,
			IntPtr toDescriptor,
			IntPtr fromDescriptor,
			short bpbBufferLength,
			byte[] bpbBuffer,
			ref short bpb_length);
		internal delegate IntPtr Tisc_blob_lookup_desc2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] table_name,
			byte[] column_name,
			IntPtr descriptor,
			byte[] global);
		internal delegate IntPtr Tisc_blob_set_desc2(
			[In, Out] IntPtr[] statusVector,
			byte[] table_name,
			byte[] column_name,
			short subtype,
			short charset,
			short segment_size,
			IntPtr descriptor);
		internal delegate IntPtr Tisc_release_savepoint(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			byte[] savepointName);
		internal delegate IntPtr Tisc_rollback_savepoint(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			byte[] savepointName,
			short option);
		internal delegate IntPtr Tisc_start_savepoint(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			byte[] savepointName);
		internal delegate IntPtr Tisc_dsql_batch_execute_immed(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			short dialect,
			ulong noOfSQL,
			ref byte[] statements,
			ref long rowsAffected);
		internal delegate IntPtr Tisc_dsql_batch_execute(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short dialect,
			IntPtr inSQLDA,
			short noOfRows,
			IntPtr batchVars,
			ref ulong rowsAffected);

		IntPtr FIBLibrary;
		Tisc_get_client_version _isc_get_client_version;
		Tisc_get_client_major_version _isc_get_client_major_version;
		Tisc_get_client_minor_version _isc_get_client_minor_version;
		Tisc_sql_interprete _isc_sql_interprete;
		Tisc_array_get_slice _isc_array_get_slice;
		Tisc_array_put_slice _isc_array_put_slice;
		Tisc_create_blob2 _isc_create_blob2;
		Tisc_open_blob2 _isc_open_blob2;
		Tisc_get_segment _isc_get_segment;
		Tisc_put_segment _isc_put_segment;
		Tisc_cancel_blob _isc_cancel_blob;
		Tisc_close_blob _isc_close_blob;
		Tisc_attach_database _isc_attach_database;
		Tisc_sqlcode _isc_sqlcode;
		Tisc_interprete _isc_interprete;
		Tisc_vax_integer _isc_vax_integer;
		Tisc_portable_integer _isc_portable_integer;
		Tisc_blob_info _isc_blob_info;
		Tisc_create_database _isc_create_database;
		Tisc_array_gen_sdl _isc_array_gen_sdl;
		Tisc_array_lookup_bounds _isc_array_lookup_bounds;
		Tisc_array_lookup_desc _isc_array_lookup_desc;
		Tisc_array_set_desc _isc_array_set_desc;
		Tisc_blob_default_desc _isc_blob_default_desc;
		Tisc_blob_gen_bpb _isc_blob_gen_bpb;
		Tisc_blob_lookup_desc _isc_blob_lookup_desc;
		Tisc_blob_set_desc _isc_blob_set_desc;
		Tisc_decode_date _isc_decode_date;
		Tisc_encode_date _isc_encode_date;
		Tisc_dsql_free_statement _isc_dsql_free_statement;
		Tisc_dsql_execute2 _isc_dsql_execute2;
		Tisc_dsql_execute _isc_dsql_execute;
		Tisc_dsql_set_cursor_name _isc_dsql_set_cursor_name;
		Tisc_dsql_fetch _isc_dsql_fetch;
		Tisc_dsql_sql_info _isc_dsql_sql_info;
		Tisc_dsql_alloc_statement2 _isc_dsql_alloc_statement2;
		Tisc_dsql_prepare _isc_dsql_prepare;
		Tisc_dsql_describe_bind _isc_dsql_describe_bind;
		Tisc_dsql_exec_immed2 _isc_dsql_exec_immed2;
		Tisc_dsql_describe _isc_dsql_describe;
		Tisc_dsql_execute_immediate _isc_dsql_execute_immediate;
		Tisc_drop_database _isc_drop_database;
		Tisc_detach_database _isc_detach_database;
		Tisc_database_info _isc_database_info;
		Tisc_start_multiple _isc_start_multiple;
		Tisc_start_transaction _isc_start_transaction;
		Tisc_commit_transaction _isc_commit_transaction;
		Tisc_commit_retaining _isc_commit_retaining;
		Tisc_rollback_transaction _isc_rollback_transaction;
		Tisc_cancel_events _isc_cancel_events;
		Tisc_que_events _isc_que_events;
		Tisc_event_counts _isc_event_counts;
		Tisc_event_block _isc_event_block;
		Tisc_free _isc_free;
		Tisc_prepare_transaction _isc_prepare_transaction;
		Tisc_prepare_transaction2 _isc_prepare_transaction2;
		Tisc_transaction_info _isc_transaction_info;
		Tisc_rollback_retaining _isc_rollback_retaining;
		Tisc_service_attach _isc_service_attach;
		Tisc_service_detach _isc_service_detach;
		Tisc_service_query _isc_service_query;
		Tisc_service_start _isc_service_start;
		Tisc_decode_sql_date _isc_decode_sql_date;
		Tisc_decode_sql_time _isc_decode_sql_time;
		Tisc_decode_timestamp _isc_decode_timestamp;
		Tisc_encode_sql_date _isc_encode_sql_date;
		Tisc_encode_sql_time _isc_encode_sql_time;
		Tisc_encode_timestamp _isc_encode_timestamp;
		Tisc_array_gen_sdl2 _isc_array_gen_sdl2;
		Tisc_array_get_slice2 _isc_array_get_slice2;
		Tisc_array_lookup_bounds2 _isc_array_lookup_bounds2;
		Tisc_array_lookup_desc2 _isc_array_lookup_desc2;
		Tisc_array_set_desc2 _isc_array_set_desc2;
		Tisc_array_put_slice2 _isc_array_put_slice2;
		Tisc_blob_default_desc2 _isc_blob_default_desc2;
		Tisc_blob_gen_bpb2 _isc_blob_gen_bpb2;
		Tisc_blob_lookup_desc2 _isc_blob_lookup_desc2;
		Tisc_blob_set_desc2 _isc_blob_set_desc2;
		Tisc_release_savepoint _isc_release_savepoint;
		Tisc_rollback_savepoint _isc_rollback_savepoint;
		Tisc_start_savepoint _isc_start_savepoint;
		Tisc_dsql_batch_execute_immed _isc_dsql_batch_execute_immed;
		Tisc_dsql_batch_execute _isc_dsql_batch_execute;


		public WindowsClient()
		{
			FIBLibrary = IntPtr.Zero;
		}

		public string Platform { get { return "Windows"; } }
		public virtual string ServerType()
		{ return "Default"; }

		~WindowsClient()
		{
			FIBLibrary = IntPtr.Zero;
		}

		public virtual string LibraryName()
		{
			return Environment.Is64BitProcess ? "ibclient64" : "gds32.dll";
		}

		private IntPtr TryGetProcAddess(string ProcName)
		{
			return UnsafeNativeMethods.GetProcAddress(FIBLibrary, ProcName);
		}

		private IntPtr GetProcAddress(string ProcName)
		{
			IntPtr Result = UnsafeNativeMethods.GetProcAddress(FIBLibrary, ProcName);
			if (Result != IntPtr.Zero)
				return Result;
			else
				throw new Exception(ProcName + " not found in " + LibraryName());
		}

		public void LoadIBLibrary()
		{
			FIBLibrary = UnsafeNativeMethods.LoadLibrary(LibraryName());
			if (FIBLibrary != IntPtr.Zero)
			{
				_isc_sqlcode = (Tisc_sqlcode)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_sqlcode"), typeof(Tisc_sqlcode));
				_isc_interprete = (Tisc_interprete)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_interprete"), typeof(Tisc_interprete));
				_isc_sql_interprete = (Tisc_sql_interprete)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_sql_interprete"), typeof(Tisc_sql_interprete));
				_isc_vax_integer = (Tisc_vax_integer)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_vax_integer"), typeof(Tisc_vax_integer));
				_isc_portable_integer = (Tisc_portable_integer)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_portable_integer"), typeof(Tisc_portable_integer));
				_isc_blob_info = (Tisc_blob_info)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_info"), typeof(Tisc_blob_info));
				_isc_open_blob2 = (Tisc_open_blob2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_open_blob2"), typeof(Tisc_open_blob2));
				_isc_close_blob = (Tisc_close_blob)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_close_blob"), typeof(Tisc_close_blob));
				_isc_get_segment = (Tisc_get_segment)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_get_segment"), typeof(Tisc_get_segment));
				_isc_put_segment = (Tisc_put_segment)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_put_segment"), typeof(Tisc_put_segment));
				_isc_create_blob2 = (Tisc_create_blob2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_create_blob2"), typeof(Tisc_create_blob2));
				_isc_create_database = (Tisc_create_database)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_create_database"), typeof(Tisc_create_database));
				_isc_cancel_blob = (Tisc_cancel_blob)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_cancel_blob"), typeof(Tisc_cancel_blob));
				_isc_array_gen_sdl = (Tisc_array_gen_sdl)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_gen_sdl"), typeof(Tisc_array_gen_sdl));
				_isc_array_get_slice = (Tisc_array_get_slice)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_get_slice"), typeof(Tisc_array_get_slice));
				_isc_array_lookup_bounds = (Tisc_array_lookup_bounds)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_lookup_bounds"), typeof(Tisc_array_lookup_bounds));
				_isc_array_lookup_desc = (Tisc_array_lookup_desc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_lookup_desc"), typeof(Tisc_array_lookup_desc));
				_isc_array_set_desc = (Tisc_array_set_desc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_set_desc"), typeof(Tisc_array_set_desc));
				_isc_array_put_slice = (Tisc_array_put_slice)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_put_slice"), typeof(Tisc_array_put_slice));
				_isc_blob_default_desc = (Tisc_blob_default_desc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_default_desc"), typeof(Tisc_blob_default_desc));
				_isc_blob_gen_bpb = (Tisc_blob_gen_bpb)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_gen_bpb"), typeof(Tisc_blob_gen_bpb));
				_isc_blob_lookup_desc = (Tisc_blob_lookup_desc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_lookup_desc"), typeof(Tisc_blob_lookup_desc));
				_isc_blob_set_desc = (Tisc_blob_set_desc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_set_desc"), typeof(Tisc_blob_set_desc));
				_isc_decode_date = (Tisc_decode_date)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_decode_date"), typeof(Tisc_decode_date));
				_isc_encode_date = (Tisc_encode_date)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_encode_date"), typeof(Tisc_encode_date));
				_isc_dsql_free_statement = (Tisc_dsql_free_statement)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_free_statement"), typeof(Tisc_dsql_free_statement));
				_isc_dsql_execute2 = (Tisc_dsql_execute2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_execute2"), typeof(Tisc_dsql_execute2));
				_isc_dsql_execute = (Tisc_dsql_execute)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_execute"), typeof(Tisc_dsql_execute));
				_isc_dsql_set_cursor_name = (Tisc_dsql_set_cursor_name)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_set_cursor_name"), typeof(Tisc_dsql_set_cursor_name));
				_isc_dsql_fetch = (Tisc_dsql_fetch)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_fetch"), typeof(Tisc_dsql_fetch));
				_isc_dsql_sql_info = (Tisc_dsql_sql_info)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_sql_info"), typeof(Tisc_dsql_sql_info));
				_isc_dsql_alloc_statement2 = (Tisc_dsql_alloc_statement2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_alloc_statement2"), typeof(Tisc_dsql_alloc_statement2));
				_isc_dsql_prepare = (Tisc_dsql_prepare)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_prepare"), typeof(Tisc_dsql_prepare));
				_isc_dsql_describe_bind = (Tisc_dsql_describe_bind)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_describe_bind"), typeof(Tisc_dsql_describe_bind));
				_isc_dsql_exec_immed2 = (Tisc_dsql_exec_immed2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_exec_immed2"), typeof(Tisc_dsql_exec_immed2));
				_isc_dsql_describe = (Tisc_dsql_describe)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_describe"), typeof(Tisc_dsql_describe));
				_isc_dsql_execute_immediate = (Tisc_dsql_execute_immediate)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_execute_immediate"), typeof(Tisc_dsql_execute_immediate));
				_isc_drop_database = (Tisc_drop_database)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_drop_database"), typeof(Tisc_drop_database));
				_isc_detach_database = (Tisc_detach_database)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_detach_database"), typeof(Tisc_detach_database));
				_isc_attach_database = (Tisc_attach_database)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_attach_database"), typeof(Tisc_attach_database));
				_isc_database_info = (Tisc_database_info)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_database_info"), typeof(Tisc_database_info));
				_isc_start_multiple = (Tisc_start_multiple)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_start_multiple"), typeof(Tisc_start_multiple));
				_isc_start_transaction = (Tisc_start_transaction)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_start_transaction"), typeof(Tisc_start_transaction));
				_isc_commit_transaction = (Tisc_commit_transaction)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_commit_transaction"), typeof(Tisc_commit_transaction));
				_isc_commit_retaining = (Tisc_commit_retaining)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_commit_retaining"), typeof(Tisc_commit_retaining));
				_isc_rollback_transaction = (Tisc_rollback_transaction)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_rollback_transaction"), typeof(Tisc_rollback_transaction));
				_isc_cancel_events = (Tisc_cancel_events)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_cancel_events"), typeof(Tisc_cancel_events));
				_isc_que_events = (Tisc_que_events)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_que_events"), typeof(Tisc_que_events));
				_isc_event_counts = (Tisc_event_counts)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_event_counts"), typeof(Tisc_event_counts));
				_isc_event_block = (Tisc_event_block)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_event_block"), typeof(Tisc_event_block));
				_isc_free = (Tisc_free)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_free"), typeof(Tisc_free));
				_isc_prepare_transaction = (Tisc_prepare_transaction)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_prepare_transaction"), typeof(Tisc_prepare_transaction));
				_isc_prepare_transaction2 = (Tisc_prepare_transaction2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_prepare_transaction2"), typeof(Tisc_prepare_transaction2));
				_isc_transaction_info = (Tisc_transaction_info)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_transaction_info"), typeof(Tisc_transaction_info));
				_isc_rollback_retaining = (Tisc_rollback_retaining)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_rollback_retaining"), typeof(Tisc_rollback_retaining));
				_isc_service_attach = (Tisc_service_attach)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_service_attach"), typeof(Tisc_service_attach));
				_isc_service_detach = (Tisc_service_detach)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_service_detach"), typeof(Tisc_service_detach));
				_isc_service_query = (Tisc_service_query)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_service_query"), typeof(Tisc_service_query));
				_isc_service_start = (Tisc_service_start)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_service_start"), typeof(Tisc_service_start));
				_isc_decode_sql_date = (Tisc_decode_sql_date)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_decode_sql_date"), typeof(Tisc_decode_sql_date));
				_isc_decode_sql_time = (Tisc_decode_sql_time)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_decode_sql_time"), typeof(Tisc_decode_sql_time));
				_isc_decode_timestamp = (Tisc_decode_timestamp)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_decode_timestamp"), typeof(Tisc_decode_timestamp));
				_isc_encode_sql_date = (Tisc_encode_sql_date)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_encode_sql_date"), typeof(Tisc_encode_sql_date));
				_isc_encode_sql_time = (Tisc_encode_sql_time)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_encode_sql_time"), typeof(Tisc_encode_sql_time));
				_isc_encode_timestamp = (Tisc_encode_timestamp)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_encode_timestamp"), typeof(Tisc_encode_timestamp));


				//FBLOB_get := GetProcAddr('BLOB_get'); {do not localize}
				//FBLOB_put := GetProcAddr('BLOB_put'); {do not localize}

				//Fisc_add_user := GetProcAddr('isc_add_user'); {do not localize}
				//Fisc_delete_user := GetProcAddr('isc_delete_user'); {do not localize}
				//Fisc_modify_user := GetProcAddr('isc_modify_user'); {do not localize}
				IBClientVersion = 6;
				SQLDAVersion = IscCodes.SQLDA_VERSION1;
				if (TryGetProcAddess("isc_get_client_version") != IntPtr.Zero)
				{
					_isc_get_client_version = (Tisc_get_client_version)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_get_client_version"), typeof(Tisc_get_client_version));
					_isc_get_client_major_version = (Tisc_get_client_major_version)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_get_client_major_version"), typeof(Tisc_get_client_major_version));
					_isc_get_client_minor_version = (Tisc_get_client_minor_version)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_get_client_minor_version"), typeof(Tisc_get_client_minor_version));
					IBClientVersion = isc_get_client_major_version() + (isc_get_client_minor_version() / 10);
					SQLDAVersion = IscCodes.SQLDA_VERSION2;
				}
				if (IBClientVersion >= 7)
				{
					_isc_array_gen_sdl2 = (Tisc_array_gen_sdl2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_gen_sdl2"), typeof(Tisc_array_gen_sdl2));
					_isc_array_get_slice2 = (Tisc_array_get_slice2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_get_slice2"), typeof(Tisc_array_get_slice2));
					_isc_array_lookup_bounds2 = (Tisc_array_lookup_bounds2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_lookup_bounds2"), typeof(Tisc_array_lookup_bounds2));
					_isc_array_lookup_desc2 = (Tisc_array_lookup_desc2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_lookup_desc2"), typeof(Tisc_array_lookup_desc2));
					_isc_array_set_desc2 = (Tisc_array_set_desc2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_set_desc2"), typeof(Tisc_array_set_desc2));
					_isc_array_put_slice2 = (Tisc_array_put_slice2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_array_put_slice2"), typeof(Tisc_array_put_slice2));
					_isc_blob_default_desc2 = (Tisc_blob_default_desc2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_default_desc2"), typeof(Tisc_blob_default_desc2));
					_isc_blob_gen_bpb2 = (Tisc_blob_gen_bpb2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_gen_bpb2"), typeof(Tisc_blob_gen_bpb2));
					_isc_blob_lookup_desc2 = (Tisc_blob_lookup_desc2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_lookup_desc2"), typeof(Tisc_blob_lookup_desc2));
					_isc_blob_set_desc2 = (Tisc_blob_set_desc2)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_blob_set_desc2"), typeof(Tisc_blob_set_desc2));

					if (IBClientVersion >= (decimal)7.1)
					{
						_isc_release_savepoint = (Tisc_release_savepoint)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_release_savepoint"), typeof(Tisc_release_savepoint));
						_isc_rollback_savepoint = (Tisc_rollback_savepoint)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_rollback_savepoint"), typeof(Tisc_rollback_savepoint));
						_isc_start_savepoint = (Tisc_start_savepoint)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_start_savepoint"), typeof(Tisc_start_savepoint));

						if (IBClientVersion >= 8)
						{
							_isc_dsql_batch_execute_immed = (Tisc_dsql_batch_execute_immed)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_batch_execute_immed"), typeof(Tisc_dsql_batch_execute_immed));
							_isc_dsql_batch_execute = (Tisc_dsql_batch_execute)Marshal.GetDelegateForFunctionPointer(GetProcAddress("isc_dsql_batch_execute"), typeof(Tisc_dsql_batch_execute));
						}
					}
				}
			}
			else
				throw new Exception("Client library - " + LibraryName() + " not found.");
		}

		public void FreeIBLibrary()
		{
		}

		public bool TryIBLoad()
		{
			return true;
		}

		public void CheckIBLoaded()
		{
			if (!TryIBLoad())
				throw new Exception("Unable to load ClientLibrary");
		}

		public decimal IBClientVersion { get; internal set; } 

		public short SQLDAVersion { get; internal set; }

		public IntPtr isc_array_get_slice(
					[In, Out] IntPtr[] statusVector,
					ref DatabaseHandle dbHandle,
					ref TransactionHandle trHandle,
					ref long arrayId,
					IntPtr desc,
					byte[] destArray,
					ref int sliceLength)
		{
			return _isc_array_get_slice(statusVector, ref dbHandle, ref trHandle, ref arrayId, desc, destArray, ref sliceLength);
		}

		public IntPtr isc_array_put_slice(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] sourceArray,
			ref int sliceLength)
		{
			return _isc_array_put_slice(statusVector, ref dbHandle, ref trHandle, ref arrayId, desc, sourceArray, ref sliceLength);
		}

		public IntPtr isc_create_blob2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref BlobHandle blobHandle,
			ref long blobId,
			short bpbLength,
			byte[] bpbAddress)
		{
			return _isc_create_blob2(statusVector, ref dbHandle, ref trHandle, ref blobHandle, ref blobId, bpbLength, bpbAddress);
		}

		public IntPtr isc_open_blob2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref BlobHandle blobHandle,
			ref long blobId,
			short bpbLength,
			byte[] bpbAddress)
		{
			return _isc_open_blob2(statusVector, ref dbHandle, ref trHandle, ref blobHandle, ref blobId, bpbLength, bpbAddress);
		}

		public IntPtr isc_get_segment(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle,
			ref short actualSegLength,
			short segBufferLength,
			byte[] segBuffer)
		{
			return _isc_get_segment(statusVector, ref blobHandle, ref actualSegLength, segBufferLength, segBuffer);
		}

		public IntPtr isc_put_segment(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle,
			short segBufferLength,
			byte[] segBuffer)
		{
			return _isc_put_segment(statusVector, ref blobHandle, segBufferLength, segBuffer);
		}

		public IntPtr isc_cancel_blob(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle)
		{
			return _isc_cancel_blob(statusVector, ref blobHandle);
		}

		public IntPtr isc_close_blob(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle)
		{
			return _isc_close_blob(statusVector, ref blobHandle);
		}

		public IntPtr isc_attach_database(
			[In, Out] IntPtr[] statusVector,
			short dbNameLength,
			byte[] dbName,
			ref DatabaseHandle dbHandle,
			short parmBufferLength,
			byte[] parmBuffer)
		{
			return _isc_attach_database(statusVector, dbNameLength, dbName, ref dbHandle, parmBufferLength, parmBuffer);
		}

		public IntPtr isc_detach_database(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle)
		{
			return _isc_detach_database(statusVector, ref dbHandle);
		}

		public IntPtr isc_database_info(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			short itemListBufferLength,
			byte[] itemListBuffer,
			short resultBufferLength,
			byte[] resultBuffer)
		{
			return _isc_database_info(statusVector, ref dbHandle, itemListBufferLength, itemListBuffer, resultBufferLength, resultBuffer);
		}

		public IntPtr isc_create_database(
			[In, Out] IntPtr[] statusVector,
			short dbNameLength,
			byte[] dbName,
			ref DatabaseHandle dbHandle,
			short parmBufferLength,
			byte[] parmBuffer,
			short dbType)
		{
			return _isc_create_database(statusVector, dbNameLength, dbName, ref dbHandle, parmBufferLength, parmBuffer, dbType);
		}

		public IntPtr isc_drop_database(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle)
		{
			return _isc_drop_database(statusVector, ref dbHandle);
		}

		public IntPtr isc_start_multiple(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short dbHandleCount,
			IntPtr tebVectorAddress)
		{
			var teb = Marshal.PtrToStructure<IBTransaction.IscTeb>(tebVectorAddress);
			return _isc_start_multiple(statusVector, ref trHandle, dbHandleCount, tebVectorAddress);
		}

		public IntPtr isc_start_transaction(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short dbHandleCount,
			ref DatabaseHandle dbHandle,
			short tpbLength,
			IntPtr tebVectorAddress)
		{
			return _isc_start_transaction(statusVector, ref trHandle, dbHandleCount, ref dbHandle, tpbLength, tebVectorAddress);
		}

		public IntPtr isc_commit_transaction(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle)
		{
			return _isc_commit_transaction(statusVector, ref trHandle);
		}

		public IntPtr isc_commit_retaining(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle)
		{
			return _isc_commit_retaining(statusVector, ref trHandle);
		}

		public IntPtr isc_rollback_transaction(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle)
		{
			return _isc_rollback_transaction(statusVector, ref trHandle);
		}

		public IntPtr isc_rollback_retaining(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle)
		{
			return _isc_rollback_retaining(statusVector,ref trHandle);
		}

		public IntPtr isc_dsql_describe(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda)
		{
			return _isc_dsql_describe(statusVector, ref stmtHandle, daVersion, xsqlda);
		}

		public IntPtr isc_dsql_describe_bind(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda)
		{
			return _isc_dsql_describe_bind(statusVector, ref stmtHandle, daVersion, xsqlda);
		}

		public IntPtr isc_dsql_prepare(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr xsqlda)
		{
			return _isc_dsql_prepare(statusVector, ref trHandle, ref stmtHandle, length, statement, dialect, xsqlda);
		}

		public IntPtr isc_dsql_execute(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda)
		{
			return _isc_dsql_execute(statusVector, ref trHandle, ref stmtHandle, daVersion, xsqlda);
		}

		public IntPtr isc_dsql_execute2(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short da_version,
			IntPtr inXsqlda,
			IntPtr outXsqlda)
		{
			return _isc_dsql_execute2(statusVector, ref trHandle, ref stmtHandle, da_version, inXsqlda, outXsqlda);
		}

		public IntPtr isc_dsql_fetch(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short daVersion,
			IntPtr xsqlda)
		{
			return _isc_dsql_fetch(statusVector, ref stmtHandle, daVersion, xsqlda);
		}

		public IntPtr isc_dsql_free_statement(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short option)
		{
			return _isc_dsql_free_statement(statusVector, ref stmtHandle, option);
		}

		public IntPtr isc_dsql_sql_info(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			short itemsLength,
			byte[] items,
			short bufferLength,
			byte[] buffer)
		{
			return _isc_dsql_sql_info(statusVector, ref stmtHandle, itemsLength, items, bufferLength, buffer);
		}

		public IntPtr isc_service_attach(
			[In, Out] IntPtr[] statusVector,
			short serviceLength,
			string service,
			ref ServiceHandle svcHandle,
			short spbLength,
			byte[] spb)
		{
			return _isc_service_attach(statusVector, serviceLength, service, ref svcHandle, spbLength, spb);
		}

		public IntPtr isc_service_start(
			[In, Out] IntPtr[] statusVector,
			ref ServiceHandle svcHandle,
			ref ServiceHandle reserved,
			short spbLength,
			byte[] spb)
		{
			return _isc_service_start(statusVector, ref svcHandle, ref reserved, spbLength, spb);
		}

		public IntPtr isc_service_detach([In, Out] IntPtr[] statusVector, ref ServiceHandle svcHandle)
		{
			return _isc_service_detach(statusVector, ref svcHandle);
		}

		public IntPtr isc_service_query(
			[In, Out] IntPtr[] statusVector,
			ref ServiceHandle svcHandle,
			ref ServiceHandle reserved,
			short sendSpbLength,
			byte[] sendSpb,
			short requestSpbLength,
			byte[] requestSpb,
			short bufferLength,
			byte[] buffer)
		{
			return _isc_service_query(statusVector, ref svcHandle, ref reserved, sendSpbLength, sendSpb, requestSpbLength, requestSpb, bufferLength, buffer);
		}

		public IntPtr isc_array_gen_sdl(
			[In, Out] IntPtr[] statusVector,
			IntPtr desc,
			ref short isc_arg3,
			byte[] isc_arg4,
			ref short isc_arg5)
		{
			return _isc_array_gen_sdl(statusVector, desc, ref isc_arg3, isc_arg4, ref isc_arg5);
		}

		public IntPtr isc_array_gen_sdl2(
			[In, Out] IntPtr[] statusVector,
			IntPtr desc,
			ref short isc_arg3,
			byte[] isc_arg4,
			ref short isc_arg5)
		{
			return _isc_array_gen_sdl2(statusVector, desc, ref isc_arg3, isc_arg4, ref isc_arg5);
		}

		public IntPtr isc_array_get_slice2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref long array_id,
			IntPtr desc,
			byte[] dest_array,
			ref int sliceLength)
		{
			return _isc_array_get_slice2(statusVector, ref dbHandle,ref trHandle, ref array_id,	desc, dest_array,ref sliceLength);
		}

		public IntPtr isc_array_lookup_bounds(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc)
		{
			return _isc_array_lookup_bounds(statusVector, ref dbHandle, ref trHandle, tableName, columnName, desc);
		}

		public IntPtr isc_array_lookup_bounds2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc)
		{
			return _isc_array_lookup_bounds2(statusVector, ref dbHandle, ref trHandle, tableName, columnName, desc);
		}

		public IntPtr isc_array_lookup_desc(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc)
		{
			return _isc_array_lookup_desc(statusVector, ref dbHandle, ref trHandle, tableName, columnName, desc);
		}

		public IntPtr isc_array_lookup_desc2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] tableName,
			byte[] columnName,
			IntPtr desc)
		{
			return _isc_array_lookup_desc2(statusVector, ref dbHandle, ref trHandle, tableName, columnName, desc);
		}

		public IntPtr isc_array_set_desc(
			[In, Out] IntPtr[] statusVector,
			byte[] tableName,
				  byte[] columnName,
			ref short sqlDtype,
			ref short sqlLength,
			ref short sqlDimensions,
				  IntPtr desc)
		{
			return _isc_array_set_desc(statusVector, tableName, columnName, ref sqlDtype, ref sqlLength, ref sqlDimensions, desc);
		}

		public IntPtr isc_array_set_desc2(
			[In, Out] IntPtr[] statusVector,
			byte[] tableName,
			byte[] columnName,
			ref short sqlDtype,
			ref short sqlLength,
			ref short sqlDimensions,
			IntPtr desc)
		{
			return _isc_array_set_desc2(statusVector, tableName, columnName, ref sqlDtype,ref sqlLength,ref sqlDimensions, desc);
		}

		public IntPtr isc_array_put_slice2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			ref long arrayId,
			IntPtr desc,
			byte[] sourceArray,
			ref int sliceLength)
		{
			return _isc_array_put_slice2(statusVector, ref dbHandle,ref trHandle,ref arrayId, desc, sourceArray,ref sliceLength);
		}


		public IntPtr isc_blob_default_desc(
			IntPtr desc,
			byte[] tableName,
			byte[] columnName)
		{
			return _isc_blob_default_desc(desc, tableName, columnName);
		}

		public IntPtr isc_blob_default_desc2(
			IntPtr desc,
			byte[] tableName,
			byte[] columnName)
		{
			return _isc_blob_default_desc2(desc, tableName, columnName);
		}

		public IntPtr isc_blob_gen_bpb(
			[In, Out] IntPtr[] statusVector,
			IntPtr toDescriptor,
			IntPtr fromDescriptor,
				  short bpbBufferLength,
			byte[] bpbBuffer,
				  ref short bpb_length)
		{
			return _isc_blob_gen_bpb(statusVector, toDescriptor, fromDescriptor, bpbBufferLength, bpbBuffer, ref bpb_length);
		}

		public IntPtr isc_blob_gen_bpb2(
			[In, Out] IntPtr[] statusVector,
			IntPtr toDescriptor,
			IntPtr fromDescriptor,
			short bpbBufferLength,
			byte[] bpbBuffer,
			ref short bpb_length)
		{
			return _isc_blob_gen_bpb2(statusVector, toDescriptor, fromDescriptor, bpbBufferLength, bpbBuffer,ref bpb_length);
		}

		public IntPtr isc_blob_info(
			[In, Out] IntPtr[] statusVector,
			ref BlobHandle blobHandle,
				  short item_list_buffer_length,
			byte[] item_list_buffer,
			short result_buffer_length,
			byte result_buffer)
		{
			return _isc_blob_info(statusVector, ref blobHandle, item_list_buffer_length, item_list_buffer, result_buffer_length, result_buffer);
		}

		public IntPtr isc_blob_lookup_desc(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] table_name,
			byte[] column_name,
			IntPtr descriptor,
			byte[] global)
		{
			return _isc_blob_lookup_desc(statusVector, ref dbHandle, ref trHandle, table_name, column_name, descriptor, global);
		}

		public IntPtr isc_blob_lookup_desc2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			byte[] table_name,
			byte[] column_name,
			IntPtr descriptor,
			byte[] global)
		{
			return _isc_blob_lookup_desc2(statusVector, ref dbHandle, ref trHandle, table_name, column_name, descriptor, global);
		}

		public IntPtr isc_blob_set_desc(
			[In, Out] IntPtr[] statusVector,
			byte[] table_name,
			byte[] column_name,
			short subtype,
			short charset,
			short segment_size,
			IntPtr descriptor)
		{
			return _isc_blob_set_desc(statusVector, table_name, column_name, subtype, charset, segment_size, descriptor);
		}

		public IntPtr isc_blob_set_desc2(
			[In, Out] IntPtr[] statusVector,
			byte[] table_name,
			byte[] column_name,
			short subtype,
			short charset,
			short segment_size,
			IntPtr descriptor)
		{
			return _isc_blob_set_desc2(statusVector, table_name, column_name, subtype, charset, segment_size, descriptor);
		}

		public IntPtr isc_cancel_events(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref int eventID)
		{
			return _isc_cancel_events(statusVector, ref dbHandle, ref eventID);
		}

		public void isc_decode_date(
			ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_decode_date(ref ibDate, ref tmDate);
		}

		public void isc_decode_sql_date(
			ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_decode_sql_date(ref ibDate, ref tmDate);
		}

		public void isc_decode_sql_time(
			ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_decode_sql_time(ref ibDate, ref tmDate);
		}


		public void isc_decode_timestamp(
		 	ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_decode_timestamp(ref ibDate, ref tmDate);
		}

		public IntPtr isc_dsql_alloc_statement2(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref StatementHandle stmtHandle)
		{
			return _isc_dsql_alloc_statement2(statusVector,ref dbHandle,ref stmtHandle);
		}

		public IntPtr isc_dsql_exec_immed2(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr inXSQLDA,
			IntPtr outXSQLDA)
		{
			return _isc_dsql_exec_immed2(statusVector,ref trHandle,ref stmtHandle, length, statement, dialect, inXSQLDA, outXSQLDA);
		}

		public IntPtr isc_dsql_batch_execute_immed(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			short dialect,
			ulong noOfSQL,
			ref byte[] statements,
			ref long rowsAffected)
		{
			if (_isc_dsql_batch_execute_immed != null)
				return _isc_dsql_batch_execute_immed(statusVector, ref dbHandle, ref trHandle, dialect, noOfSQL, ref statements, ref rowsAffected);
			else
				throw new Exception("isc_dsql_batch_execute_immed requires InterBase client 8.0 or higher");
		}

		public IntPtr isc_dsql_batch_execute(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			ref StatementHandle stmtHandle,
			short dialect,
			IntPtr inSQLDA,
			short noOfRows,
			IntPtr batchVars,
			ref ulong rowsAffected)
		{
			if (_isc_dsql_batch_execute != null)
				return _isc_dsql_batch_execute(statusVector, ref trHandle, ref stmtHandle, dialect, inSQLDA, noOfRows, batchVars, ref rowsAffected);
			else
				throw new Exception("isc_dsql_batch_execute_immed requires InterBase client 8.0 or higher");
		}

		public IntPtr isc_dsql_execute_immediate(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref TransactionHandle trHandle,
			short length,
			byte[] statement,
			short dialect,
			IntPtr XSQLDA)
		{
			return _isc_dsql_execute_immediate(statusVector, ref dbHandle, ref trHandle, length, statement, dialect, XSQLDA);
		}

		public IntPtr isc_dsql_set_cursor_name(
			[In, Out] IntPtr[] statusVector,
			ref StatementHandle stmtHandle,
			byte[] cursorName,
			short _type)
		{
			return _isc_dsql_set_cursor_name(statusVector, ref stmtHandle, cursorName, _type);
		}

		public void isc_encode_date(
			ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_encode_date( ref ibDate, ref tmDate);
		}

		public void isc_encode_sql_date(
			ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_encode_sql_date(ref ibDate, ref tmDate);
		}

		public void isc_encode_sql_time(
			ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_encode_sql_time(ref ibDate, ref tmDate);
		}

		public void isc_encode_timestamp(
			ref long ibDate,
			ref CTimeStructure tmDate)
		{
			_isc_encode_timestamp(ref ibDate, ref tmDate);
		}

		public int isc_event_block(
			ref IntPtr event_buffer,
			ref IntPtr result_buffer,
			ushort id_count,
			byte[][] event_list)
		{
			return _isc_event_block(ref event_buffer, ref result_buffer, id_count, event_list[0], event_list[1],
				event_list[2], event_list[3], event_list[4], event_list[5], event_list[6], event_list[7], event_list[8],
				event_list[9], event_list[10], event_list[11], event_list[12], event_list[13], event_list[14]);
		}

		public void isc_event_counts(
			[In, Out] uint[] status,
			short bufferLength,
			IntPtr eventBuffer,
			IntPtr resultBuffer)
		{
			_isc_event_counts(status, bufferLength, eventBuffer, resultBuffer);
		}

		public long isc_free(IntPtr isc_arg1)
		{
			return _isc_free(isc_arg1);
		}

		public IntPtr isc_interprete(
			byte[] buffer,
			 [In, Out] ref IntPtr[] statusVector)
		{
			return _isc_interprete(buffer, ref statusVector);
		}

		public IntPtr isc_prepare_transaction([In, Out] IntPtr[] statusVector,
	        ref TransactionHandle trHandle)
		{
			return _isc_prepare_transaction(statusVector, ref trHandle);
		}

		public IntPtr isc_prepare_transaction2(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short msgLength,
			byte[] msg)
		{
			return _isc_prepare_transaction2(statusVector, ref trHandle, msgLength, msg);
		}

		public IntPtr isc_que_events(
			[In, Out] IntPtr[] statusVector,
			ref DatabaseHandle dbHandle,
			ref int eventId,
			short length,
			IntPtr eventBuffer,
			ibEventCallbackDelegate eventFunction,
			IntPtr eventFunctionArg)
		{
			return _isc_que_events(statusVector, ref dbHandle, ref eventId, length, eventBuffer, eventFunction, eventFunctionArg);
		}

		public IntPtr isc_release_savepoint(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
				  byte[] savepointName)
		{
			if (_isc_release_savepoint != null)
				return _isc_release_savepoint(statusVector, ref trHandle, savepointName);
			else
				throw new Exception("isc_release_savepoint requires InterBase Client 7.1 or higher");
		}

		public IntPtr isc_rollback_savepoint(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			byte[] savepointName,
			short option)
		{
			if (_isc_rollback_savepoint != null)
			    return _isc_rollback_savepoint(statusVector, ref trHandle, savepointName, option);
			else
				throw new Exception("isc_rollback_savepoint requires InterBase Client 7.1 or higher");
		}

		public IntPtr isc_start_savepoint(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			byte[] savepointName)
		{
			if (_isc_start_savepoint != null)
			return _isc_start_savepoint(statusVector,ref trHandle, savepointName);
			else
				throw new Exception("isc_start_savepoint requires InterBase Client 7.1 or higher");
		}

		public long isc_sqlcode([In, Out] IntPtr[] statusVector)
		{
			return _isc_sqlcode(statusVector);
		}

		public void isc_sql_interprete(short sqlcode, byte[] buffer, short bufferLength)
		{
			_isc_sql_interprete(sqlcode, buffer, bufferLength);
		}


		public IntPtr isc_transaction_info(
			[In, Out] IntPtr[] statusVector,
			ref TransactionHandle trHandle,
			short itemListBufferLength,
			byte[] itemListBuffer,
			short result_buffer_length,
			byte[] result_buffer)
		{
			return _isc_transaction_info(statusVector, ref trHandle, itemListBufferLength, itemListBuffer, result_buffer_length, result_buffer);
		}

		public long isc_vax_integer(
			byte[] buffer,
			short length)
		{
			return _isc_vax_integer(buffer, length);
		}

		public long isc_portable_integer(
			byte[] buffer,
			short length)
		{
			return _isc_portable_integer(buffer, length);
		}

		public void isc_get_client_version(byte[] buffer)
		{
			if (_isc_get_client_version != null)
				_isc_get_client_version(buffer);
			else
				throw new Exception("isc_get_client_version requires IB client 7.0 or higher");
		}

		public int isc_get_client_major_version()
		{
			if (_isc_get_client_major_version != null)
			    return _isc_get_client_major_version();
		    else
				throw new Exception("isc_get_client_major_version requires IB client 7.0 or higher");
		}

		public int isc_get_client_minor_version()
		{
			if (_isc_get_client_minor_version != null)
			    return _isc_get_client_minor_version();
			else
				throw new Exception("isc_get_client_minor_version requires IB client 7.0 or higher");
		}
	}
}
