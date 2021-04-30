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

// This file was originally ported from Jaybird

using System;

namespace InterBaseSql.Data.Common
{
	internal static class IscCodes
	{
		#region General

		public const int SQLDA_VERSION1 = 1;
		public const int SQLDA_VERSION2 = 2;
		public const int SQLDA_CURRENT_VERSION = SQLDA_VERSION2;
		public const int SQL_DIALECT_V5 = 1;
		public const int SQL_DIALECT_V6_TRANSITION = 2;
		public const int SQL_DIALECT_V6 = 3;
		public const int SQL_DIALECT_CURRENT = SQL_DIALECT_V6;
		public const int DSQL_close = 1;
		public const int DSQL_drop = 2;
		public const int ARRAY_DESC_COLUMN_MAJOR = 1;   /* Set for FORTRAN */
		public const int ISC_STATUS_LENGTH = 20;
		public const ushort INVALID_OBJECT = 0xFFFF;
		public const short ARR_DESC_VERSION2 = 2;
                public const short ARR_DESC_CURRENT_VERSION = ARR_DESC_VERSION2;

		#endregion

		#region Change Views

		public const int SQLIND_NULL = (1 << 15);
		public const int SQLIND_INSERT = (1 << 0);
		public const int SQLIND_UPDATE = (1 << 1);
		public const int SQLIND_DELETE = (1 << 2);
		public const int SQLIND_CHANGE = (1 << 3);
		public const int SQLIND_TRUNCATE = (1 << 4);
		public const int SQLIND_CHANGE_VIEW = (1 << 5);

		#endregion

		#region Buffer sizes

		public const int BUFFER_SIZE_128 = 128;
		public const int BUFFER_SIZE_256 = 256;
		public const int BUFFER_SIZE_32K = 32768;
		public const int DEFAULT_MAX_BUFFER_SIZE = 8192;
		public const int ROWS_AFFECTED_BUFFER_SIZE = 34;
		public const int STATEMENT_TYPE_BUFFER_SIZE = 8;
		public const int PREPARE_INFO_BUFFER_SIZE = 32768;

		#endregion

		#region Statement Flags

		public const int STMT_DEFER_EXECUTE = 4;

		#endregion

		#region Operation Codes

		// Operation (packet) types
		public const int op_void = 0;   // Packet has been voided
		public const int op_connect = 1;    // Connect to remote server
		public const int op_exit = 2;   // Remote end has exitted
		public const int op_accept = 3; // Server accepts connection
		public const int op_reject = 4; // Server rejects connection
		public const int op_protocol = 5;   // Protocol	selection
		public const int op_disconnect = 6; // Connect is going	away
		public const int op_credit = 7; // Grant (buffer) credits
		public const int op_continuation = 8;   // Continuation	packet
		public const int op_response = 9;   // Generic response	block

		// Page	server operations
		public const int op_open_file = 10; // Open	file for page service
		public const int op_create_file = 11;   // Create file for page	service
		public const int op_close_file = 12;    // Close file for page service
		public const int op_read_page = 13; // optionally lock and read	page
		public const int op_write_page = 14;    // write page and optionally release lock
		public const int op_lock = 15;  // sieze lock
		public const int op_convert_lock = 16;  // convert existing	lock
		public const int op_release_lock = 17;  // release existing	lock
		public const int op_blocking = 18;  // blocking	lock message

		// Full	context	server operations
		public const int op_attach = 19;    // Attach database
		public const int op_create = 20;    // Create database
		public const int op_detach = 21;    // Detach database
		public const int op_compile = 22;   // Request based operations
		public const int op_start = 23;
		public const int op_start_and_send = 24;
		public const int op_send = 25;
		public const int op_receive = 26;
		public const int op_unwind = 27;
		public const int op_release = 28;

		public const int op_transaction = 29;   // Transaction operations
		public const int op_commit = 30;
		public const int op_rollback = 31;
		public const int op_prepare = 32;
		public const int op_reconnect = 33;

		public const int op_create_blob = 34;   // Blob	operations //
		public const int op_open_blob = 35;
		public const int op_get_segment = 36;
		public const int op_put_segment = 37;
		public const int op_cancel_blob = 38;
		public const int op_close_blob = 39;

		public const int op_info_database = 40; // Information services
		public const int op_info_request = 41;
		public const int op_info_transaction = 42;
		public const int op_info_blob = 43;

		public const int op_batch_segments = 44;    // Put a bunch of blob segments

		public const int op_mgr_set_affinity = 45;  // Establish server	affinity
		public const int op_mgr_clear_affinity = 46;    // Break server	affinity
		public const int op_mgr_report = 47;    // Report on server

		public const int op_que_events = 48;    // Que event notification request
		public const int op_cancel_events = 49; // Cancel event	notification request
		public const int op_commit_retaining = 50;  // Commit retaining	(what else)
		public const int op_prepare2 = 51;  // Message form	of prepare
		public const int op_event = 52; // Completed event request (asynchronous)
		public const int op_connect_request = 53;   // Request to establish	connection
		public const int op_aux_connect = 54;   // Establish auxiliary connection
		public const int op_ddl = 55;   // DDL call
		public const int op_open_blob2 = 56;
		public const int op_create_blob2 = 57;
		public const int op_get_slice = 58;
		public const int op_put_slice = 59;
		public const int op_slice = 60; // Successful response to public const int op_get_slice
		public const int op_seek_blob = 61; // Blob	seek operation

		// DSQL	operations //
		public const int op_allocate_statement = 62;    // allocate	a statment handle
		public const int op_execute = 63;   // execute a prepared statement
		public const int op_exec_immediate = 64;    // execute a statement
		public const int op_fetch = 65; // fetch a record
		public const int op_fetch_response = 66;    // response	for	record fetch
		public const int op_free_statement = 67;    // free	a statement
		public const int op_prepare_statement = 68; // prepare a statement
		public const int op_set_cursor = 69;    // set a cursor	name
		public const int op_info_sql = 70;
		public const int op_dummy = 71; // dummy packet	to detect loss of client
		public const int op_response_piggyback = 72;    // response	block for piggybacked messages
		public const int op_start_and_receive = 73;
		public const int op_start_send_and_receive = 74;

		public const int op_exec_immediate2 = 75;   // execute an immediate	statement with msgs
		public const int op_execute2 = 76;  // execute a statement with	msgs
		public const int op_insert = 77;
		public const int op_sql_response = 78;  // response	from execute; exec immed; insert
		public const int op_transact = 79;
		public const int op_transact_response = 80;
		public const int op_drop_database = 81;
		public const int op_service_attach = 82;
		public const int op_service_detach = 83;
		public const int op_service_info = 84;
		public const int op_service_start = 85;
		public const int op_rollback_retaining = 86;

		// Two following opcode are used in vulcan.
		// No plans to implement them completely for a while, but to
		// support protocol 11, where they are used, have them here.
		public const int op_update_account_info = 87;
		public const int op_authenticate_user = 88;

		public const int op_partial = 89;   // packet is not complete - delay processing
		public const int op_trusted_auth = 90;
		public const int op_cancel = 91;
		public const int op_cont_auth = 92;
		public const int op_ping = 93;
		public const int op_accept_data = 94;
		public const int op_abort_aux_connection = 95;
		public const int op_crypt = 96;
		public const int op_crypt_key_callback = 97;
		public const int op_cond_accept = 98;

		#endregion

		#region Database Parameter Block

		public const int isc_dpb_version1 = 1;
		public const int isc_dpb_cdd_pathname = 1;
		public const int isc_dpb_allocation = 2;
		public const int isc_dpb_journal = 3;
		public const int isc_dpb_page_size = 4;
		public const int isc_dpb_num_buffers = 5;
		public const int isc_dpb_buffer_length = 6;
		public const int isc_dpb_debug = 7;
		public const int isc_dpb_garbage_collect = 8;
		public const int isc_dpb_verify = 9;
		public const int isc_dpb_sweep = 10;
		public const int isc_dpb_enable_journal = 11;
		public const int isc_dpb_disable_journal = 12;
		public const int isc_dpb_dbkey_scope = 13;
		public const int isc_dpb_number_of_users = 14;
		public const int isc_dpb_trace = 15;
		public const int isc_dpb_no_garbage_collect = 16;
		public const int isc_dpb_damaged = 17;
		public const int isc_dpb_license = 18;
		public const int isc_dpb_sys_user_name = 19;
		public const int isc_dpb_encrypt_key = 20;
		public const int isc_dpb_activate_shadow = 21;
		public const int isc_dpb_sweep_interval = 22;
		public const int isc_dpb_delete_shadow = 23;
		public const int isc_dpb_force_write = 24;
		public const int isc_dpb_begin_log = 25;
		public const int isc_dpb_quit_log = 26;
		public const int isc_dpb_no_reserve = 27;
		public const int isc_dpb_user_name = 28;
		public const int isc_dpb_password = 29;
		public const int isc_dpb_password_enc = 30;
		public const int isc_dpb_sys_user_name_enc = 31;
		public const int isc_dpb_interp = 32;
		public const int isc_dpb_online_dump = 33;
		public const int isc_dpb_old_file_size = 34;
		public const int isc_dpb_old_num_files = 35;
		public const int isc_dpb_old_file = 36;
		public const int isc_dpb_old_start_page = 37;
		public const int isc_dpb_old_start_seqno = 38;
		public const int isc_dpb_old_start_file = 39;
		public const int isc_dpb_drop_walfile = 40;
		public const int isc_dpb_old_dump_id = 41;
		public const int isc_dpb_wal_backup_dir = 42;
		public const int isc_dpb_wal_chkptlen = 43;
		public const int isc_dpb_wal_numbufs = 44;
		public const int isc_dpb_wal_bufsize = 45;
		public const int isc_dpb_wal_grp_cmt_wait = 46;
		public const int isc_dpb_lc_messages = 47;
		public const int isc_dpb_lc_ctype = 48;
		public const int isc_dpb_cache_manager = 49;
		public const int isc_dpb_shutdown = 50;
		public const int isc_dpb_online = 51;
		public const int isc_dpb_shutdown_delay = 52;
		public const int isc_dpb_reserved = 53;
		public const int isc_dpb_overwrite = 54;
		public const int isc_dpb_sec_attach = 55;
		public const int isc_dpb_disable_wal = 56;
		public const int isc_dpb_connect_timeout = 57;
		public const int isc_dpb_dummy_packet_interval = 58;
		public const int isc_dpb_gbak_attach = 59;
		public const int isc_dpb_sql_role_name = 60;
		public const int isc_dpb_set_page_buffers = 61;
		public const int isc_dpb_working_directory = 62;
		public const int isc_dpb_sql_dialect = 63;
		public const int isc_dpb_set_db_readonly = 64;
		public const int isc_dpb_set_db_sql_dialect = 65;
		public const int isc_dpb_gfix_attach = 66;
		public const int isc_dpb_gstat_attach = 67;
		public const int isc_dpb_gbak_ods_version = 68;
		public const int isc_dpb_gbak_ods_minor_version = 69;
		public const int isc_dpb_set_group_commit = 70;
		public const int isc_dpb_gbak_validate = 71;
		public const int isc_dpb_client_interbase_var = 72;
		public const int isc_dpb_admin_option = 73;
		public const int isc_dpb_flush_interval = 74;
		public const int isc_dpb_instance_name = 75;
		public const int isc_dpb_old_overwrite = 76;
		public const int isc_dpb_archive_database = 77;
		public const int isc_dpb_archive_journals = 78;
		public const int isc_dpb_archive_sweep = 79;
		public const int isc_dpb_archive_dumps = 80;
		public const int isc_dpb_archive_recover = 81;
		public const int isc_dpb_recover_until = 82;
		public const int isc_dpb_force = 83;
		public const int isc_dpb_preallocate = 84;
		public const int isc_dpb_sys_encrypt_password = 85;
		public const int isc_dpb_eua_user_name = 86;
		public const int isc_dpb_transaction = 87;
		public const int isc_dpb_ods_version_major = 88;

		#endregion

		#region Transaction Parameter Block

		public const int isc_tpb_version1 = 1;
		public const int isc_tpb_version3 = 3;
		public const int isc_tpb_consistency = 1;
		public const int isc_tpb_concurrency = 2;
		public const int isc_tpb_shared = 3;
		public const int isc_tpb_protected = 4;
		public const int isc_tpb_exclusive = 5;
		public const int isc_tpb_wait = 6;
		public const int isc_tpb_nowait = 7;
		public const int isc_tpb_read = 8;
		public const int isc_tpb_write = 9;
		public const int isc_tpb_lock_read = 10;
		public const int isc_tpb_lock_write = 11;
		public const int isc_tpb_verb_time = 12;
		public const int isc_tpb_commit_time = 13;
		public const int isc_tpb_ignore_limbo = 14;
		public const int isc_tpb_read_committed = 15;
		public const int isc_tpb_autocommit = 16;
		public const int isc_tpb_rec_version = 17;
		public const int isc_tpb_no_rec_version = 18;
		public const int isc_tpb_restart_requests = 19;
		public const int isc_tpb_no_auto_undo = 20;
		public const int isc_tpb_no_savepoint = 21;
		public const int isc_tpb_exclusivity = 22;
		public const int isc_tpb_wait_time = 23;

		#endregion

		#region Services Parameter Block

		public const int isc_spb_version1 = 1;
		public const int isc_spb_current_version = 2;
		public const int isc_spb_version = isc_spb_current_version;

		public const int isc_spb_user_name = isc_dpb_user_name;
		public const int isc_spb_sys_user_name = isc_dpb_sys_user_name;
		public const int isc_spb_sys_user_name_enc = isc_dpb_sys_user_name_enc;
		public const int isc_spb_password = isc_dpb_password;
		public const int isc_spb_password_enc = isc_dpb_password_enc;
		public const int isc_spb_sys_encrypt_password = isc_dpb_sys_encrypt_password;
		public const int isc_spb_command_line = 105;
		public const int isc_spb_dbname = 106;
		public const int isc_spb_verbose = 107;
		public const int isc_spb_options = 108;
		public const int isc_spb_user_dbname = 109;
		public const int isc_spb_auth_dbname = 110;

		public const int isc_spb_connect_timeout = isc_dpb_connect_timeout;
		public const int isc_spb_dummy_packet_interval = isc_dpb_dummy_packet_interval;
		public const int isc_spb_sql_role_name = isc_dpb_sql_role_name;
		public const int isc_spb_instance_name = isc_dpb_instance_name;

		public const int isc_spb_num_att = 5;
		public const int isc_spb_num_db = 6;


		#endregion

		#region Services Actions

		public const int isc_action_svc_backup = 1; /* Starts database backup process on the server	*/
		public const int isc_action_svc_restore = 2;    /* Starts database restore process on the server */
		public const int isc_action_svc_repair = 3; /* Starts database repair process on the server	*/
		public const int isc_action_svc_add_user = 4;   /* Adds	a new user to the security database	*/
		public const int isc_action_svc_delete_user = 5;    /* Deletes a user record from the security database	*/
		public const int isc_action_svc_modify_user = 6;    /* Modifies	a user record in the security database */
		public const int isc_action_svc_display_user = 7;   /* Displays	a user record from the security	database */
		public const int isc_action_svc_properties = 8; /* Sets	database properties	*/
		public const int isc_action_svc_add_license = 9;    /* Adds	a license to the license file */
		public const int isc_action_svc_remove_license = 10;    /* Removes a license from the license file */
		public const int isc_action_svc_db_stats = 11;  /* Retrieves database statistics */
		public const int isc_action_svc_get_ib_log = 12;    /* Retrieves the InterBase log file	from the server	*/
		public const int isc_action_svc_add_db_alias = 13; /* Adds a new database alias */
		public const int isc_action_svc_delete_db_alias = 14; /* Deletes an existing database alias*/
		public const int isc_action_svc_display_db_alias = 15; /* Displays an existing database alias*/
		public const int isc_action_svc_dump = 16; /* Starts database dump process on the sever*/

		#endregion

		#region Services Information

		public const int isc_info_svc_svr_db_info = 50; /* Retrieves the number	of attachments and databases */
		public const int isc_info_svc_get_license = 51; /* Retrieves all license keys and IDs from the license file	*/
		public const int isc_info_svc_get_license_mask = 52;    /* Retrieves a bitmask representing	licensed options on	the	server */
		public const int isc_info_svc_get_config = 53;  /* Retrieves the parameters	and	values for IB_CONFIG */
		public const int isc_info_svc_version = 54; /* Retrieves the version of	the	services manager */
		public const int isc_info_svc_server_version = 55;  /* Retrieves the version of	the	InterBase server */
		public const int isc_info_svc_implementation = 56;  /* Retrieves the implementation	of the InterBase server	*/
		public const int isc_info_svc_capabilities = 57;    /* Retrieves a bitmask representing	the	server's capabilities */
		public const int isc_info_svc_user_dbpath = 58; /* Retrieves the path to the security database in use by the server	*/
		public const int isc_info_svc_get_env = 59; /* Retrieves the setting of	$INTERBASE */
		public const int isc_info_svc_get_env_lock = 60;    /* Retrieves the setting of	$INTERBASE_LCK */
		public const int isc_info_svc_get_env_msg = 61; /* Retrieves the setting of	$INTERBASE_MSG */
		public const int isc_info_svc_line = 62;    /* Retrieves 1 line	of service output per call */
		public const int isc_info_svc_to_eof = 63;  /* Retrieves as much of	the	server output as will fit in the supplied buffer */
		public const int isc_info_svc_timeout = 64; /* Sets	/ signifies	a timeout value	for	reading	service	information	*/
		public const int isc_info_svc_get_licensed_users = 65;  /* Retrieves the number	of users licensed for accessing	the	server */
		public const int isc_info_svc_limbo_trans = 66; /* Retrieve	the	limbo transactions */
		public const int isc_info_svc_running = 67; /* Checks to see if	a service is running on	an attachment */
		public const int isc_info_svc_get_users = 68;   /* Returns the user	information	from isc_action_svc_display_users */
		public const int isc_info_svc_get_db_alias = 69; /* Returns the database alias information from isc_action_svc_display_db_alias*/
		public const int isc_info_svc_product_identifier = 70; /* Returns embedding application's product identifier, if present in license */


		#endregion

		#region Services Properties

		public const int isc_spb_prp_page_buffers = 5;
		public const int isc_spb_prp_sweep_interval = 6;
		public const int isc_spb_prp_shutdown_db = 7;
		public const int isc_spb_prp_deny_new_attachments = 9;
		public const int isc_spb_prp_deny_new_transactions = 10;
		public const int isc_spb_prp_reserve_space = 11;
		public const int isc_spb_prp_write_mode = 12;
		public const int isc_spb_prp_access_mode = 13;
		public const int isc_spb_prp_set_sql_dialect = 14;
		public const int isc_spb_prp_archive_dumps = 42;
		public const int isc_spb_prp_archive_sweep = 43;

		// RESERVE_SPACE_PARAMETERS
		public const int isc_spb_prp_res_use_full = 35;
		public const int isc_spb_prp_res = 36;

		// WRITE_MODE_PARAMETERS
		public const int isc_spb_prp_wm_async = 37;
		public const int isc_spb_prp_wm_sync = 38;
		public const int isc_spb_prp_wm_direct = 41;

		// ACCESS_MODE_PARAMETERS
		public const int isc_spb_prp_am_readonly = 39;
		public const int isc_spb_prp_am_readwrite = 40;

		// Option Flags
		public const int isc_spb_prp_activate = 0x0100;
		public const int isc_spb_prp_db_online = 0x0200;

		#endregion

		#region Backup Service
		/* options needing values... */
		public const int isc_spb_bkp_file = 5;
		public const int isc_spb_bkp_factor = 6;
		public const int isc_spb_bkp_length = 7;
		public const int isc_spb_bkp_preallocate = 13;
		public const int isc_spb_bkp_encrypt_name = 14;
		/* standalone options for backup operation... */
		public const int isc_spb_bkp_ignore_checksums = 0x01;
		public const int isc_spb_bkp_ignore_limbo = 0x02;
		public const int isc_spb_bkp_metadata_only = 0x04;
		public const int isc_spb_bkp_no_garbage_collect = 0x08;
		public const int isc_spb_bkp_old_descriptions = 0x10;
		public const int isc_spb_bkp_non_transportable = 0x20;
		public const int isc_spb_bkp_convert = 0x40;
		public const int isc_spb_bkp_expand = 0x80;
		/* standalone options for Archive backup operation... */
		public const int isc_spb_bkp_archive_database = 0x10000;
		public const int isc_spb_bkp_archive_journals = 0x20000;

		#endregion

		#region Restore Service

		public const int isc_spb_res_buffers = 9;
		public const int isc_spb_res_page_size = 10;
		public const int isc_spb_res_length = 11;
		public const int isc_spb_res_access_mode = 12;
		public const int isc_spb_res_preallocate = isc_spb_bkp_preallocate;
		public const int isc_spb_res_decrypt_password = 16;
		public const int isc_spb_res_eua_user_name = 17;
		public const int isc_spb_res_eua_password = 18;
		public const int isc_spb_res_write_mode = 19;
		public const int isc_spb_res_starting_trans = 21;  // requires 64bit integer value
		public const int isc_spb_res_ods_version_major =	22;
		public const int isc_spb_res_archive_recover_until = 23;
		public const int isc_spb_res_metadata_only = isc_spb_bkp_metadata_only;
		/* standalone options for restore operation... */
		public const int isc_spb_res_deactivate_idx = 0x0100;
		public const int isc_spb_res_no_shadow = 0x0200;
		public const int isc_spb_res_no_validity = 0x0400;
		public const int isc_spb_res_one_at_a_time = 0x0800;
		public const int isc_spb_res_replace = 0x1000;
		public const int isc_spb_res_create = 0x2000;
		public const int isc_spb_res_use_all_space = 0x4000;
		public const int isc_spb_res_validate = 0x8000;
		/* standalone options for Archive recover operation... */
		public const int isc_spb_res_archive_recover = 0x40000;
		/* standalone options for tablespace restore */
		public const int isc_spb_res_create_tablespace = 0x100000;
		public const int isc_spb_res_replace_tablespace = 0x200000;
		/* options needing string values */
		public const int isc_spb_tablespace_include = 24;
		public const int isc_spb_tablespace_exclude = 25;
		public const int isc_spb_tablespace_file = isc_spb_bkp_file;

		public const int isc_spb_dmp_file = isc_spb_bkp_file;
		public const int isc_spb_dmp_length = isc_spb_bkp_length;
		public const int isc_spb_dmp_overwrite = 20; // special case; does not require any values.
		/* standalone options for dump operation... */
		public const int isc_spb_dmp_create = 0x80000;

		public const int isc_spb_res_am_readonly = isc_spb_prp_am_readonly;
		public const int isc_spb_res_am_readwrite = isc_spb_prp_am_readwrite;

		public const int isc_spb_res_wm_async = isc_spb_prp_wm_async;
		public const int isc_spb_res_wm_sync = isc_spb_prp_wm_sync;
		public const int isc_spb_res_wm_direct = isc_spb_prp_wm_direct;

		#endregion

		#region DB Stats
		public const int isc_spb_sts_data_pages = 0x01;
		public const int isc_spb_sts_db_log = 0x02;
		public const int isc_spb_sts_hdr_pages = 0x04;
		public const int isc_spb_sts_idx_pages = 0x08;
		public const int isc_spb_sts_sys_relations = 0x10;
		public const int isc_spb_sts_record_versions = 0x20;
		public const int isc_spb_sts_table = 0x40;
		#endregion

		#region Validate Service
		public const int isc_spb_val_tab_incl = 1;  // include filter based on regular expression
		public const int isc_spb_val_tab_excl = 2;  // exclude filter based on regular expression
		public const int isc_spb_val_idx_incl = 3;  // regexp of indices to validate
		public const int isc_spb_val_idx_excl = 4;  // regexp of indices to NOT validate
		public const int isc_spb_val_lock_timeout = 5;  // how long to wait for table lock
		#endregion

		#region Repair Service
	    /* options needing values... */
		public const int isc_spb_rpr_commit_trans = 15;
		public const int isc_spb_rpr_rollback_trans = 34;
		public const int isc_spb_rpr_recover_two_phase = 17;
		public const int isc_spb_tra_id = 18;
		public const int isc_spb_single_tra_id = 19;
		public const int isc_spb_multi_tra_id = 20;
		public const int isc_spb_tra_state = 21;
		public const int isc_spb_tra_state_limbo = 22;
		public const int isc_spb_tra_state_commit = 23;
		public const int isc_spb_tra_state_rollback = 24;
		public const int isc_spb_tra_state_unknown = 25;
		public const int isc_spb_tra_host_site = 26;
		public const int isc_spb_tra_remote_site = 27;
		public const int isc_spb_tra_db_path = 28;
		public const int isc_spb_tra_advise = 29;
		public const int isc_spb_tra_advise_commit = 30;
		public const int isc_spb_tra_advise_rollback = 31;

		/* standalone options for repair operation... */
		public const int isc_spb_tra_advise_unknown = 33;
		public const int isc_spb_rpr_validate_db = 0x01;
		public const int isc_spb_rpr_sweep_db = 0x02;
		public const int isc_spb_rpr_mend_db = 0x04;
		public const int isc_spb_rpr_list_limbo_trans = 0x08;
		public const int isc_spb_rpr_check_db = 0x10;
		public const int isc_spb_rpr_ignore_checksum = 0x20;
		public const int isc_spb_rpr_kill_shadows = 0x40;
		public const int isc_spb_rpr_full = 0x80;

		#endregion

		#region Security Service

		public const int isc_spb_sec_userid = 5;
		public const int isc_spb_sec_groupid = 6;
		public const int isc_spb_sec_username = 7;
		public const int isc_spb_sec_password = 8;
		public const int isc_spb_sec_groupname = 9;
		public const int isc_spb_sec_firstname = 10;
		public const int isc_spb_sec_middlename = 11;
		public const int isc_spb_sec_lastname = 12;

		#endregion

		#region DBAlias Service

		public const int isc_spb_sec_db_alias_name = 20;
		public const int isc_spb_sec_db_alias_dbpath = 21;

		#endregion

		#region License Service

		public const int isc_spb_lic_key = 5;
		public const int isc_spb_lic_id = 6;
		public const int isc_spb_lic_desc = 7;

		#endregion

		#region Configuration Keys

		public const int ISCCFG_LOCKMEM_KEY = 0;
		public const int ISCCFG_LOCKSEM_KEY = 1;
		public const int ISCCFG_LOCKSIG_KEY = 2;
		public const int ISCCFG_EVNTMEM_KEY = 3;
		public const int ISCCFG_DBCACHE_KEY = 4;
		public const int ISCCFG_PRIORITY_KEY = 5;
		public const int ISCCFG_IPCMAP_KEY = 6;
		public const int ISCCFG_MEMMIN_KEY = 7;
		public const int ISCCFG_MEMMAX_KEY = 8;
		public const int ISCCFG_LOCKORDER_KEY = 9;
		public const int ISCCFG_ANYLOCKMEM_KEY = 10;
		public const int ISCCFG_ANYLOCKSEM_KEY = 11;
		public const int ISCCFG_ANYLOCKSIG_KEY = 12;
		public const int ISCCFG_ANYEVNTMEM_KEY = 13;
		public const int ISCCFG_LOCKHASH_KEY = 14;
		public const int ISCCFG_DEADLOCK_KEY = 15;
		public const int ISCCFG_LOCKSPIN_KEY = 16;
		public const int ISCCFG_CONN_TIMEOUT_KEY = 17;
		public const int ISCCFG_DUMMY_INTRVL_KEY = 18;
		public const int ISCCFG_TRACE_POOLS_KEY = 19; /* Internal Use only	*/
		public const int ISCCFG_REMOTE_BUFFER_KEY = 20;
		public const int ISCCFG_CPU_AFFINITY_KEY = 21;
		public const int ISCCFG_SWEEP_QUANTUM_KEY = 22;
		public const int ISCCFG_USER_QUANTUM_KEY = 23;
		public const int ISCCFG_SLEEP_TIME_KEY = 24;
		public const int ISCCFG_MAX_THREADS_KEY = 25;
		public const int ISCCFG_ADMIN_DB_KEY = 26;
		public const int ISCCFG_USE_SANCTUARY_KEY = 27;
		public const int ISCCFG_ENABLE_HT_KEY = 28;
		public const int ISCCFG_USE_ROUTER_KEY = 29;
		public const int ISCCFG_SORTMEM_BUFFER_SIZE_KEY = 30;
		public const int ISCCFG_SQL_CMP_RECURSION_KEY = 31;
		public const int ISCCFG_SOL_BOUND_THREADS_KEY = 32;
		public const int ISCCFG_SOL_SYNC_SCOPE_KEY = 33;
		public const int ISCCFG_IDX_RECNUM_MARKER_KEY = 34;
		public const int ISCCFG_IDX_GARBAGE_COLLECTION_KEY = 35;
		public const int ISCCFG_WIN_LOCAL_CONNECT_RETRIES_KEY = 36;
		public const int ISCCFG_EXPAND_MOUNTPOINT_KEY = 37;
		public const int ISCCFG_LOOPBACK_CONNECTION_KEY = 38;
		public const int ISCCFG_THREAD_STACK_SIZE_KEY = 39;
		public const int ISCCFG_MAX_DB_VIRMEM_USE_KEY = 40;
		public const int ISCCFG_MAX_ASSISTANTS_KEY = 41;
		public const int ISCCFG_APPDATA_DIR_KEY	= 42;
		public const int ISCCFG_MEMORY_RECLAMATION_KEY = 43;
		public const int ISCCFG_PAGE_CACHE_EXPANSION_KEY = 44;
		public const int ISCCFG_STARTING_TRANSACTION_ID_KEY = 45;	/* Used internally to test 64-bit transaction ID*/
		public const int ISCCFG_DATABASE_ODS_VERSION_KEY = 46; /* Used internally to test creating databases with older ODS versions */
		public const int ISCCFG_HOSTLIC_IMPORT_DIR_KEY = 47;
		public const int ISCCFG_HOSTLIC_INFO_DIR_KEY = 48;
		public const int ISCCFG_ENABLE_PARTIAL_INDEX_SELECTIVITY_KEY = 49;
		public const int ISCCFG_PREDICTIVE_IO_PAGES_KEY = 50;

		#endregion

		#region Common Structural Codes

		public const int isc_info_end = 1;
		public const int isc_info_truncated = 2;
		public const int isc_info_error = 3;
		public const int isc_info_data_not_ready = 4;
		public const int isc_info_flag_end = 127;

		#endregion

		#region SQL Information

		public const int isc_info_sql_select = 4;
		public const int isc_info_sql_bind = 5;
		public const int isc_info_sql_num_variables = 6;
		public const int isc_info_sql_describe_vars = 7;
		public const int isc_info_sql_describe_end = 8;
		public const int isc_info_sql_sqlda_seq = 9;
		public const int isc_info_sql_message_seq = 10;
		public const int isc_info_sql_type = 11;
		public const int isc_info_sql_sub_type = 12;
		public const int isc_info_sql_scale = 13;
		public const int isc_info_sql_length = 14;
		public const int isc_info_sql_null_ind = 15;
		public const int isc_info_sql_field = 16;
		public const int isc_info_sql_relation = 17;
		public const int isc_info_sql_owner = 18;
		public const int isc_info_sql_alias = 19;
		public const int isc_info_sql_sqlda_start = 20;
		public const int isc_info_sql_stmt_type = 21;
		public const int isc_info_sql_get_plan = 22;
		public const int isc_info_sql_records = 23;
		public const int isc_info_sql_batch_fetch = 24;
		public const int isc_info_sql_precision = 25;

		#endregion

		#region SQL Information Return Values

		public const int isc_info_sql_stmt_select = 1;
		public const int isc_info_sql_stmt_insert = 2;
		public const int isc_info_sql_stmt_update = 3;
		public const int isc_info_sql_stmt_delete = 4;
		public const int isc_info_sql_stmt_ddl = 5;
		public const int isc_info_sql_stmt_get_segment = 6;
		public const int isc_info_sql_stmt_put_segment = 7;
		public const int isc_info_sql_stmt_exec_procedure = 8;
		public const int isc_info_sql_stmt_start_trans = 9;
		public const int isc_info_sql_stmt_commit = 10;
		public const int isc_info_sql_stmt_rollback = 11;
		public const int isc_info_sql_stmt_select_for_upd = 12;
		public const int isc_info_sql_stmt_set_generator = 13;
		public const int isc_info_sql_stmt_savepoint = 14;
		public const int isc_info_sql_stmt_set_subscription = 15;
		public const int isc_info_sql_stmt_truncate = 16;
		#endregion

		#region Database Information

		public const int isc_info_db_id = 4;
		public const int isc_info_reads = 5;
		public const int isc_info_writes = 6;
		public const int isc_info_fetches = 7;
		public const int isc_info_marks = 8;

		public const int isc_info_implementation = 11;
		public const int isc_info_version = 12;
		public const int isc_info_base_level = 13;
		public const int isc_info_page_size = 14;
		public const int isc_info_num_buffers = 15;
		public const int isc_info_limbo = 16;
		public const int isc_info_current_memory = 17;
		public const int isc_info_max_memory = 18;
		public const int isc_info_window_turns = 19;
		public const int isc_info_license = 20;

		public const int isc_info_allocation = 21;
		public const int isc_info_attachment_id = 22;
		public const int isc_info_read_seq_count = 23;
		public const int isc_info_read_idx_count = 24;
		public const int isc_info_insert_count = 25;
		public const int isc_info_update_count = 26;
		public const int isc_info_delete_count = 27;
		public const int isc_info_backout_count = 28;
		public const int isc_info_purge_count = 29;
		public const int isc_info_expunge_count = 30;

		public const int isc_info_sweep_interval = 31;
		public const int isc_info_ods_version = 32;
		public const int isc_info_ods_minor_version = 33;
		public const int isc_info_no_reserve = 34;
		public const int isc_info_logfile = 35;
		public const int isc_info_cur_logfile_name = 36;
		public const int isc_info_cur_log_part_offset = 37;
		public const int isc_info_num_wal_buffers = 38;
		public const int isc_info_wal_buffer_size = 39;
		public const int isc_info_wal_ckpt_length = 40;

		public const int isc_info_wal_cur_ckpt_interval = 41;
		public const int isc_info_wal_prv_ckpt_fname = 42;
		public const int isc_info_wal_prv_ckpt_poffset = 43;
		public const int isc_info_wal_recv_ckpt_fname = 44;
		public const int isc_info_wal_recv_ckpt_poffset = 45;
		public const int isc_info_wal_grpc_wait_usecs = 47;
		public const int isc_info_wal_num_io = 48;
		public const int isc_info_wal_avg_io_size = 49;
		public const int isc_info_wal_num_commits = 50;

		public const int isc_info_wal_avg_grpc_size = 51;
		public const int isc_info_forced_writes = 52;
		public const int isc_info_user_names = 53;
		public const int isc_info_page_errors = 54;
		public const int isc_info_record_errors = 55;
		public const int isc_info_bpage_errors = 56;
		public const int isc_info_dpage_errors = 57;
		public const int isc_info_ipage_errors = 58;
		public const int isc_info_ppage_errors = 59;
		public const int isc_info_tpage_errors = 60;

		public const int isc_info_set_page_buffers = 61;
		public const int isc_info_db_sql_dialect = 62;
		public const int isc_info_db_read_only = 63;
		public const int isc_info_db_size_in_pages = 64;

		public const int isc_info_db_reads = 65;
		public const int isc_info_db_writes = 66;
		public const int isc_info_db_fetches = 67;
		public const int isc_info_db_marks = 68;
		public const int isc_info_db_group_commit = 69;
		public const int isc_info_svr_min_ver = 71;
		public const int isc_info_ib_env_var = 72;
		public const int isc_info_server_tcp_port = 73;
		public const int isc_info_db_preallocate = 74;
		public const int isc_info_db_encrypted = 75;
		public const int isc_info_db_encryptions = 76;
		public const int isc_info_db_sep_external = 77;
		public const int isc_info_db_eua_active = 78;
		public const int isc_info_db_creation_timestamp = 79;


		#endregion

		#region Information Request

		public const int isc_info_number_messages = 4;
		public const int isc_info_max_message = 5;
		public const int isc_info_max_send = 6;
		public const int isc_info_max_receive = 7;
		public const int isc_info_state = 8;
		public const int isc_info_message_number = 9;
		public const int isc_info_message_size = 10;
		public const int isc_info_request_cost = 11;
		public const int isc_info_access_path = 12;
		public const int isc_info_req_select_count = 13;
		public const int isc_info_req_insert_count = 14;
		public const int isc_info_req_update_count = 15;
		public const int isc_info_req_delete_count = 16;

		#endregion

		#region Array Slice Description Language

		public const int isc_sdl_version1 = 1;
		public const int isc_sdl_eoc = 255;
		public const int isc_sdl_relation = 2;
		public const int isc_sdl_rid = 3;
		public const int isc_sdl_field = 4;
		public const int isc_sdl_fid = 5;
		public const int isc_sdl_struct = 6;
		public const int isc_sdl_variable = 7;
		public const int isc_sdl_scalar = 8;
		public const int isc_sdl_tiny_integer = 9;
		public const int isc_sdl_short_integer = 10;
		public const int isc_sdl_long_integer = 11;
		public const int isc_sdl_literal = 12;
		public const int isc_sdl_add = 13;
		public const int isc_sdl_subtract = 14;
		public const int isc_sdl_multiply = 15;
		public const int isc_sdl_divide = 16;
		public const int isc_sdl_negate = 17;
		public const int isc_sdl_eql = 18;
		public const int isc_sdl_neq = 19;
		public const int isc_sdl_gtr = 20;
		public const int isc_sdl_geq = 21;
		public const int isc_sdl_lss = 22;
		public const int isc_sdl_leq = 23;
		public const int isc_sdl_and = 24;
		public const int isc_sdl_or = 25;
		public const int isc_sdl_not = 26;
		public const int isc_sdl_while = 27;
		public const int isc_sdl_assignment = 28;
		public const int isc_sdl_label = 29;
		public const int isc_sdl_leave = 30;
		public const int isc_sdl_begin = 31;
		public const int isc_sdl_end = 32;
		public const int isc_sdl_do3 = 33;
		public const int isc_sdl_do2 = 34;
		public const int isc_sdl_do1 = 35;
		public const int isc_sdl_element = 36;

		#endregion

		#region Blob Parameter Block

		public const int isc_bpb_version1 = 1;
		public const int isc_bpb_source_type = 1;
		public const int isc_bpb_target_type = 2;
		public const int isc_bpb_type = 3;
		public const int isc_bpb_source_interp = 4;
		public const int isc_bpb_target_interp = 5;
		public const int isc_bpb_filter_parameter = 6;
		public const int isc_bpb_target_relation_name = 7;
		public const int isc_bpb_target_field_name = 8;

		public const int isc_bpb_type_segmented = 0;
		public const int isc_bpb_type_stream = 1;

		public const int RBL_eof = 1;
		public const int RBL_segment = 2;
		public const int RBL_eof_pending = 4;
		public const int RBL_create = 8;

		#endregion

		#region Blob Subtypes

		public const int isc_blob_untyped = 0;

		/* internal subtypes*/

		public const int isc_blob_text = 1;
		public const int isc_blob_blr = 2;
		public const int isc_blob_acl = 3;
		public const int isc_blob_ranges = 4;
		public const int isc_blob_summary = 5;
		public const int isc_blob_format = 6;
		public const int isc_blob_tra = 7;
		public const int isc_blob_extfile = 8;

		/* the range 20-30 is reserved for dBASE and Paradox types */

		public const int isc_blob_formatted_memo = 20;
		public const int isc_blob_paradox_ole = 21;
		public const int isc_blob_graphic = 22;
		public const int isc_blob_dbase_ole = 23;
		public const int isc_blob_typed_binary = 24;

		#endregion

		#region Blob Information

		public const int isc_info_blob_num_segments = 4;
		public const int isc_info_blob_max_segment = 5;
		public const int isc_info_blob_total_length = 6;
		public const int isc_info_blob_type = 7;

		#endregion

		#region ISC Error codes

		public const int isc_facility = 20;
		public const int isc_err_base = 335544320;
		public const int isc_err_factor = 1;
		public const int isc_arg_end = 0;    // end of argument list
		public const int isc_arg_gds = 1;    // generic DSRI	status value
		public const int isc_arg_string = 2;    // string argument
		public const int isc_arg_cstring = 3;   // count & string argument
		public const int isc_arg_number = 4;    // numeric argument	(long)
		public const int isc_arg_interpreted = 5;   // interpreted status code (string)
		public const int isc_arg_vms = 6;   // VAX/VMS status code (long)
		public const int isc_arg_unix = 7;  // UNIX	error code
		public const int isc_arg_domain = 8;    // Apollo/Domain error code
		public const int isc_arg_dos = 9;   // MSDOS/OS2 error code
		public const int isc_arg_mpexl = 10;    // HP MPE/XL error code
		public const int isc_arg_mpexl_ipc = 11;    // HP MPE/XL IPC error code
		public const int isc_arg_next_mach = 15;    // NeXT/Mach error code
		public const int isc_arg_netware = 16;  // NetWare error code
		public const int isc_arg_win32 = 17;    // Win32 error code
		public const int isc_arg_warning = 18;  // warning argument
		public const int isc_arg_sql = 19;    
		public const int isc_arg_int64 = 20;

		public const int isc_open_trans = 335544357;
		public const int isc_segment = 335544366;
		public const int isc_segstr_eof = 335544367;
		public const int isc_connect_reject = 335544421;
		public const int isc_invalid_dimension = 335544458;
		public const int isc_tra_state = 335544468;
		public const int isc_except = 335544517;
		public const int isc_dsql_sqlda_err = 335544583;
		public const int isc_network_error = 335544721;
		public const int isc_net_read_err = 335544726;
		public const int isc_net_write_err = 335544727;
		public const int isc_stack_trace = 335544842;
		public const int isc_except2 = 335544848;
		public const int isc_arith_except = 335544321;
		public const int isc_string_truncation = 335544914;
		public const int isc_formatted_exception = 335545016;
		public const int isc_wirecrypt_incompatible = 335545064;

		#endregion

		#region BLR Codes

		public const int blr_text                       =         14;
		public const int blr_text2                      =         15;
		public const int blr_short                      =          7;
		public const int blr_long                       =          8;
		public const int blr_quad                       =          9;
		public const int blr_float                      =         10;
		public const int blr_double                     =         27;
		public const int blr_d_float                    =         11;
		public const int blr_timestamp                  =         35;
		public const int blr_varying                    =         37;
		public const int blr_varying2                   =         38;
		public const int blr_blob                       =        261;
		public const int blr_cstring                    =         40;
		public const int blr_cstring2                   =         41;
		public const int blr_blob_id                    =         45;
		public const int blr_sql_date                   =         12;
		public const int blr_sql_time                   =         13;
		public const int blr_int64                      =         16;
		public const int blr_boolean_dtype              =         17;
		public const int blr_date                       =         blr_timestamp;

		public const int blr_inner                      =          0;
		public const int blr_left                       =          1;
		public const int blr_right                      =          2;
		public const int blr_full                       =          3;

		public const int blr_gds_code                   =          0;
		public const int blr_sql_code                   =          1;
		public const int blr_exception                  =          2;
		public const int blr_trigger_code               =          3;
		public const int blr_default_code               =          4;

		public const int blr_immediate                  =          0;
		public const int blr_deferred                   =          1;

		public const int blr_restrict                   =          0;
		public const int blr_cascade                    =          1;

		public const int blr_version4                   =          4;
		public const int blr_version5                   =          5;
		public const int blr_eoc                        =         76;
		public const int blr_end                        =         255;

		public const int blr_assignment                 =          1;
		public const int blr_begin                      =          2;
		public const int blr_dcl_variable               =          3;
		public const int blr_message                    =          4;
		public const int blr_erase                      =          5;
		public const int blr_fetch                      =          6;
		public const int blr_for                        =          7;
		public const int blr_if                         =          8;
		public const int blr_loop                       =          9;
		public const int blr_modify                     =         10;
		public const int blr_handler                    =         11;
		public const int blr_receive                    =         12;
		public const int blr_select                     =         13;
		public const int blr_send                       =         14;
		public const int blr_store                      =         15;
		public const int blr_truncate                   =         16;
		public const int blr_label                      =         17;
		public const int blr_leave                      =         18;
		public const int blr_store2                     =         19;
		public const int blr_post                       =         20;

		public const int blr_literal                    =         21;
		public const int blr_dbkey                      =         22;
		public const int blr_field                      =         23;
		public const int blr_fid                        =         24;
		public const int blr_parameter                  =         25;
		public const int blr_variable                   =         26;
		public const int blr_average                    =         27;
		public const int blr_count                      =         28;
		public const int blr_maximum                    =         29;
		public const int blr_minimum                    =         30;
		public const int blr_total                      =         31;
		public const int blr_add                        =         34;
		public const int blr_subtract                   =         35;
		public const int blr_multiply                   =         36;
		public const int blr_divide                     =         37;
		public const int blr_negate                     =         38;
		public const int blr_concatenate                =         39;
		public const int blr_substring                  =         40;
		public const int blr_parameter2                 =         41;
		public const int blr_from                       =         42;
		public const int blr_via                        =         43;
		public const int blr_user_name                  =         44;
		public const int blr_null                       =         45;

		public const int blr_eql                        =         47;
		public const int blr_neq                        =         48;
		public const int blr_gtr                        =         49;
		public const int blr_geq                        =         50;
		public const int blr_lss                        =         51;
		public const int blr_leq                        =         52;
		public const int blr_containing                 =         53;
		public const int blr_matching                   =         54;
		public const int blr_starting                   =         55;
		public const int blr_between                    =         56;
		public const int blr_or                         =         57;
		public const int blr_and                        =         58;
		public const int blr_not                        =         59;
		public const int blr_any                        =         60;
		public const int blr_missing                    =         61;
		public const int blr_unique                     =         62;
		public const int blr_like                       =         63;
		public const int blr_with                       =         64;

		public const int blr_stream                     =         65;
		public const int blr_set_index                  =         66;
		public const int blr_rse                        =         67;
		public const int blr_first                      =         68;
		public const int blr_project                    =         69;
		public const int blr_sort                       =         70;
		public const int blr_boolean                    =         71;
		public const int blr_ascending                  =         72;
		public const int blr_descending                 =         73;
		public const int blr_relation                   =         74;
		public const int blr_rid                        =         75;
		public const int blr_union                      =         76;
		public const int blr_map                        =         77;
		public const int blr_group_by                   =         78;
		public const int blr_aggregate                  =         79;
		public const int blr_join_type                  =         80;
		public const int blr_rows                       =         81;
		public const int blr_derived_relation           =         82;

		/* sub parameters for public const int blr_rows */
		public const int blr_ties                       =          0;
		public const int blr_percent                    =          1;

		public const int blr_agg_count                  =         83;
		public const int blr_agg_max                    =         84;
		public const int blr_agg_min                    =         85;
		public const int blr_agg_total                  =         86;
		public const int blr_agg_average                =         87;
		public const int blr_parameter3                 =         88;
		public const int blr_run_count                  =        118;
		public const int blr_run_max                    =         89;
		public const int blr_run_min                    =         90;
		public const int blr_run_total                  =         91;
		public const int blr_run_average                =         92;
		public const int blr_agg_count2                 =         93;
		public const int blr_agg_count_distinct         =         94;
		public const int blr_agg_total_distinct         =         95;
		public const int blr_agg_average_distinct       =         96;

		public const int blr_function                   =        100;
		public const int blr_gen_id                     =        101;
		public const int blr_prot_mask                  =        102;
		public const int blr_upcase                     =        103;
		public const int blr_lock_state                 =        104;
		public const int blr_value_if                   =        105;
		public const int blr_matching2                  =        106;
		public const int blr_index                      =        107;
		public const int blr_ansi_like                  =        108;
		public const int blr_bookmark                   =        109;
		public const int blr_crack                      =        110;
		public const int blr_force_crack                =        111;
		public const int blr_seek                       =        112;
		public const int blr_find                       =        113;

		public const int blr_continue                   =          0;
		public const int blr_forward                    =          1;
		public const int blr_backward                   =          2;
		public const int blr_bof_forward                =          3;
		public const int blr_eof_backward               =          4;

		public const int blr_lock_relation              =        114;
		public const int blr_lock_record                =        115;
		public const int blr_set_bookmark               =        116;
		public const int blr_get_bookmark               =        117;
		public const int blr_rs_stream                  =        119;
		public const int blr_exec_proc                  =        120;
		public const int blr_begin_range                =        121;
		public const int blr_end_range                  =        122;
		public const int blr_delete_range               =        123;
		public const int blr_procedure                  =        124;
		public const int blr_pid                        =        125;
		public const int blr_exec_pid                   =        126;
		public const int blr_singular                   =        127;
		public const int blr_abort                      =        128;
		public const int blr_block                      =        129;
		public const int blr_error_handler              =        130;
		public const int blr_cast                       =        131;
		public const int blr_release_lock               =        132;
		public const int blr_release_locks              =        133;
		public const int blr_start_savepoint            =        134;
		public const int blr_end_savepoint              =        135;
		public const int blr_find_dbkey                 =        136;
		public const int blr_range_relation             =        137;
		public const int blr_delete_ranges              =        138;

		public const int blr_plan                       =        139;
		public const int blr_merge                      =        140;
		public const int blr_join                       =        141;
		public const int blr_sequential                 =        142;
		public const int blr_navigational               =        143;
		public const int blr_indices                    =        144;
		public const int blr_retrieve                   =        145;

		public const int blr_relation2                  =        146;
		public const int blr_rid2                       =        147;
		public const int blr_reset_stream               =        148;
		public const int blr_release_bookmark           =        149;
		public const int blr_set_generator              =        150;
		public const int blr_ansi_any                   =        151;
		public const int blr_exists                     =        152;
		public const int blr_cardinality                =        153;

		public const int blr_record_version             =        154;		/* get tid of record */
		public const int blr_stall                      =        155;		/* fake server stall*/
		public const int blr_seek_no_warn               =        156;
		public const int blr_find_dbkey_version         =        157;
		public const int blr_ansi_all                   =        158;

		public const int blr_extract                    = 159;

		/* sub parameters for public const int blr_extract*/
		public const int blr_extract_year               = 0;
		public const int blr_extract_month              = 1;
		public const int blr_extract_day	         = 2;
		public const int blr_extract_hour               = 3;
		public const int blr_extract_minute             = 4;
		public const int blr_extract_second             = 5;
		public const int blr_extract_weekday            = 6;
		public const int blr_extract_yearday            = 7;

		public const int blr_current_date               = 160;
		public const int blr_current_timestamp          = 161;
		public const int blr_current_time               = 162;

		/* These verbs were added in 6.0, primarily to support 64-bit integers */
		public const int blr_add2	            = 163;
		public const int blr_subtract2	            = 164;
		public const int blr_multiply2             = 165;
		public const int blr_divide2	            = 166;
		public const int blr_agg_total2            = 167;
		public const int blr_agg_total_distinct2   = 168;
		public const int blr_agg_average2          = 169;
		public const int blr_agg_average_distinct2 = 170;
		public const int blr_average2		    = 171;
		public const int blr_gen_id2		    = 172;
		public const int blr_set_generator2        = 173;

		/* These verbs were added in 7.0 for BOOLEAN dtype supprt*/
		public const int blr_boolean_true = 174;
		public const int blr_boolean_false = 175;

		/* These verbs were added in 7.1 for SQL savepoint support*/
		public const int blr_start_savepoint2      = 176;
		public const int blr_release_savepoint     = 177;
		public const int blr_rollback_savepoint    = 178;
		/* added for EXECUTE STATEMENT in 10.0 */
		public const int blr_exec_stmt             = 179;
		public const int blr_exec_stmt2            = 180;
		#endregion

		#region DataType Definitions

		public const int SQL_VARYING = 448;
		public const int SQL_TEXT = 452;
		public const int SQL_DOUBLE = 480;
		public const int SQL_FLOAT = 482;
		public const int SQL_LONG = 496;
		public const int SQL_SHORT = 500;
		public const int SQL_TIMESTAMP = 510;
		public const int SQL_BLOB = 520;
		public const int SQL_D_FLOAT = 530;
		public const int SQL_ARRAY = 540;
		public const int SQL_QUAD = 550;
		public const int SQL_TYPE_TIME = 560;
		public const int SQL_TYPE_DATE = 570;
		public const int SQL_INT64 = 580;
		public const int SQL_BOOLEAN = 590;

		// Historical alias	for	pre	V6 applications
		public const int SQL_DATE = SQL_TIMESTAMP;

		#endregion
	}
}
