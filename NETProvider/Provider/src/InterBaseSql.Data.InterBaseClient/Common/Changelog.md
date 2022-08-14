# Changes for 7.12.1

## ConnectionString
* Changed the default for CharSet from UTF8 to None
* Server/port information will now get parsed if only passed in the DataSource property.  Database property still holds precedence

## Extensions.cs
* suppressed a platform warning in TrySetKeepAlive.  There does not seem to be a platform neutral version of this.

## IDatabase.cs
* added a IBClient getter to be able to get at the underlying client library that a IBDatabase is using.

# Changes for 7.11.0

***Change View support added***

## ChangeState.cs (new)
* New enum, IBChangeState, that enumerates the different possible change states

## ConnectionString.cs
* Added support for truncate_char in a connection string to force char fields to be right truncated automatically

## DbField.cs
* New function ChangeState.  Returns the change state for the field.

## DbValue.cs
* New Function ChangeState.  Returns the underlying DbField's ChangeState.
* GetString updated to truncate Char fields automatically when the database componnet's TruncateChar is set to true

## IDatabase.cs
* New Property added to the interface - TruncateChar

# Changes for 7.10.2 

## General 
* All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.Client.Native to InterBaseSql.Data.Client.Native
* Uses InterBaseSql.Data.Common instead of FirebirdSql.Data.Common
* Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
## Charset.cs
* UTF8 Id switched to 59
* Added ISO8859_15, DOS852 and DOS857 to supported Charactersets
*	Removed WIN1255, WIN1256, WIN1257, KOI8U and TIS620 from supported charactersets
		
## ConnectionString.cs
* DefaultValueLibraryEmbedded default is now false (assume connecting to full server)
*	Over the wire Encryption is done in the connection string
*	Removed GetWireCrypt 
*	Added DefaultValueInstanceName
*	Added DefaultValueSEPPassword
*	Added DefaultValueServerPublicFile
*	Added DefaultValueClientCertFile
*	Added DefaultValueClientPassPhraseFile
*	Added DefaultValueClientPassPhrase
*	Added DefaultValueSSL
*	Following are added keys usable in the connection string
*	DefaultKeyClientLibrary changed to DefaultKeyLibraryEmbedded = "embedded".  This goes alogn with hte Client being the default now vs embedded
*	Added DefaultKeyEUAEnabled = "EUAEnabled"
*	Added DefaultKeyInstanceName = "instance name";
*	Added DefaultKeySEPPassword = "SEP Password";
*	Added DefaultKeyServerPublicFile = "ServerPublicFile";
*	Added DefaultKeyServerPublicPath = "ServerPublicPath";
*	Added DefaultKeyClientCertFile = "ClientCertFile";
*	Added DefaultKeyClientPassPhraseFile = "ClientPassPhraseFile";
*	Added DefaultKeyClientPassPhrase = "ClientPassPhrase";
*	Added DefaultKeySSL = "SSL";
*	Removed keys - DefaultKeyNoDbTriggers, DefaultKeyNoGarbageCollect, DefaultKeyCompression, DefaultKeyCryptKey and DefaultKeyWireCrypt
*	Added new Key/DefaultValue entries to the Dictionary and removed the Fb specific key/value combos
*	Following properties added to support new keys
		*	ClientEmbedded
		* EUAEnabled
		* InstanceName
		* SEPPassword
		* SSL
		* ServerPublicFile
		* ServerPublicPath
		* ClientCertFile
		* ClientPassPhraseFile
		* ClientPassPhrase
*	New public method ComposeDatabase that returns a string.  This builds an InterBase connection string passed through the DPB and includes the encryption stuff when used.
*	Validate
		*  removes the Fb specific NoDatabaseTriggers check
		*	Added encryption parameter exclusivity check
*	Load removed over the wire crypto items
		
## DbDataType.cs
* Removed Fb datatypes - TimeStampTZ, TimeStampTZEx, TimeTZ, TimeTZEx, Dec16, Dec34 and Int128

## DbField.cs
* Added private _precisionScale
*	Added public property PrecisionScale
*	Added public property IsNull checking for the NullFlag being < 0 for change views
*	FixNull changed to support ChangeViews

## DbValue.cs	
* Remoced Fb specific data types GetTimeZoneId, GetDec16, GetDec34 and GetInt128	
*	GetBytes removed TimeStampTZ, TimeStampTZEx, TimeTZ, TimeTZEx, Dec16, Dec34 and Int128 Fb specific datatypes
*	GetNumericBytes removed SQL_INT128 support
		
## Descriptor.cs
* Descriptor's constructor defaults the version to the SQLDA_CURRENT_VERSION
*	ToBlrArray changed boolean write from IscCodes.blr_bool to IscCodes.blr_boolean
*	Removed SQL_TIMESTAMP_TZ_EX, SQL_TIMESTAMP_TZ, SQL_TIME_TZ, SQL_TIME_TZ_EX, SQL_DEC16, SQL_DEC34, SQL_INT128 and SQL_NULL (Fb specific items) 
		
## IscCodes.cs - Long story short, removed the Fb specific code (particularly when they overlapped) and added all the IB missing ones.
* Added SQLDA_VERSION2 and SQLDA_CURRENT_VERSION  
* Added ARR_DESC_VERSION2 and ARR_DESC_CURRENT_VERSION
* Added SQLIND_NULL, SQLIND_INSERT, SQLIND_UPDATE, SQLIND_DELETE, SQLIND_CHANGE, SQLIND_TRUNCATE and SQLIND_CHANGE_VIEW	for change views
*	Removed the Fb Protocol codes
*	Removed Fb Server class
*	removed isc_dpb_version2
*	Added isc_dpb_wal_backup_dir, isc_dpb_wal_chkptlen, isc_dpb_wal_numbufs, isc_dpb_wal_bufsize, isc_dpb_wal_grp_cmt_wait and isc_dpb_disable_wal
*	Removed Fb overlapped constants isc_dpb_set_db_charset, isc_dpb_process_id, isc_dpb_no_db_triggers, isc_dpb_trusted_auth, isc_dpb_process_name, isc_dpb_utf8_filename, isc_dpb_client_version and isc_dpb_specific_auth_data
*	Added isc_dpb_gbak_ods_version, isc_dpb_gbak_ods_minor_version, isc_dpb_set_group_commit, isc_dpb_gbak_validate, isc_dpb_client_interbase_var, isc_dpb_admin_option, isc_dpb_flush_interval, isc_dpb_instance_name,	isc_dpb_old_overwrite, isc_dpb_archive_database, isc_dpb_archive_journals,isc_dpb_archive_sweep, isc_dpb_archive_dumps, isc_dpb_archive_recover, isc_dpb_recover_until,t isc_dpb_force,
*	isc_dpb_preallocate, isc_dpb_sys_encrypt_password, isc_dpb_eua_user_name, isc_dpb_transaction and isc_dpb_ods_version_major
* Removed overlapped values for isc_tpb_lock_timeout, isc_tpb_read_consistency and isc_tpb_at_snapshot_number
* Added isc_tpb_no_savepoint, isc_tpb_exclusivity, int isc_tpb_wait_time
*	Removed isc_spb_version3
*	Removed overlap values for isc_spb_address_path, isc_spb_process_id, isc_spb_trusted_auth, isc_spb_process_name, isc_spb_trusted_role, isc_spb_verbint, isc_spb_auth_block, isc_spb_auth_plugin_name,	isc_spb_auth_plugin_list, isc_spb_utf8_filename, isc_spb_client_version, isc_spb_remote_protocol, isc_spb_host_name, isc_spb_os_user, isc_spb_config, isc_spb_expected_db and isc_spb_specific_auth_data
*	Added isc_spb_sys_encrypt_password, isc_spb_user_dbname, isc_spb_auth_dbname and isc_spb_instance_name
* Removed overlapped isc_action_svc_nbak, isc_action_svc_nrest, isc_action_svc_trace_start, isc_action_svc_trace_stop, isc_action_svc_trace_suspend, isc_action_svc_trace_resume, isc_action_svc_trace_list, isc_action_svc_set_mapping, isc_action_svc_drop_mapping, isc_action_svc_display_user_adm and isc_action_svc_validate
*	Added isc_action_svc_add_db_alias,isc_action_svc_delete_db_alias, isc_action_svc_display_db_alias and isc_action_svc_dump
*	Removed overlapped isc_info_svc_stdin
*	Added isc_info_svc_get_db_alias and isc_info_svc_product_identifier
*	Removed overlapped isc_spb_prp_force_shutdown, isc_spb_prp_attachments_shutdown, isc_spb_prp_transactions_shutdown, isc_spb_prp_shutdown_mode, isc_spb_prp_online_mode, isc_spb_prp_sm_normal, isc_spb_prp_sm_multi, isc_spb_prp_sm_single, int isc_spb_prp_sm_full
*	Added isc_spb_prp_archive_dumps and isc_spb_prp_archive_sweep
*	Added isc_spb_prp_wm_direct
*	Removed isc_spb_prp_nolinger
*	Removed isc_spb_bkp_skip_data, isc_spb_bkp_stat and isc_spb_bkp_no_triggers
*	Added isc_spb_bkp_preallocate, isc_spb_bkp_encrypt_name, isc_spb_bkp_archive_database and isc_spb_bkp_archive_journals
*	Removed isc_spb_bkp_skip_data, isc_spb_res_fix_fss_data, isc_spb_res_fix_fss_metadata and isc_spb_res_stat
*	Added isc_spb_res_preallocate, isc_spb_res_decrypt_password, isc_spb_res_eua_user_name, isc_spb_res_eua_password, isc_spb_res_write_mode, isc_spb_res_starting_trans, isc_spb_res_ods_version_major, isc_spb_res_archive_recover_until, isc_spb_res_validate, isc_spb_res_archive_recover, isc_spb_res_create_tablespace, isc_spb_res_replace_tablespace, isc_spb_tablespace_include, isc_spb_tablespace_exclude, isc_spb_tablespace_file, isc_spb_dmp_file, isc_spb_dmp_length, isc_spb_dmp_overwrite, isc_spb_dmp_create, isc_spb_res_wm_async, isc_spb_res_wm_sync and isc_spb_res_wm_direct
* Added isc_spb_sts_data_pages, isc_spb_sts_db_log, isc_spb_sts_hdr_pages, isc_spb_sts_idx_pages, isc_spb_sts_sys_relations, isc_spb_sts_record_versions and isc_spb_sts_table	
* Added isc_spb_sec_db_alias_name and isc_spb_sec_db_alias_dbpath		
*	Added isc_spb_lic_key, isc_spb_lic_id and isc_spb_lic_desc
*	Removed isc_spb_nbk_level, isc_spb_nbk_file, isc_spb_nbk_direct and isc_spb_nbk_no_triggers
*	Removed isc_spb_trc_id, isc_spb_trc_name and isc_spb_trc_cfg
*	Added ISCCFG_CPU_AFFINITY_KEY,t ISCCFG_SWEEP_QUANTUM_KEY, ISCCFG_USER_QUANTUM_KEY, ISCCFG_SLEEP_TIME_KEY, ISCCFG_MAX_THREADS_KEY, ISCCFG_ADMIN_DB_KEY, ISCCFG_USE_SANCTUARY_KEY, ISCCFG_ENABLE_HT_KEY, ISCCFG_USE_ROUTER_KEY, ISCCFG_SORTMEM_BUFFER_SIZE_KEY, ISCCFG_SQL_CMP_RECURSION_KEY, ISCCFG_SOL_BOUND_THREADS_KEY, ISCCFG_SOL_SYNC_SCOPE_KEY, ISCCFG_IDX_RECNUM_MARKER_KEY, ISCCFG_IDX_GARBAGE_COLLECTION_KEY, ISCCFG_WIN_LOCAL_CONNECT_RETRIES_KEY, ISCCFG_EXPAND_MOUNTPOINT_KEY, ISCCFG_LOOPBACK_CONNECTION_KEY, ISCCFG_THREAD_STACK_SIZE_KEY, ISCCFG_MAX_DB_VIRMEM_USE_KEY, ISCCFG_MAX_ASSISTANTS_KEY, ISCCFG_APPDATA_DIR_KEY, ISCCFG_MEMORY_RECLAMATION_KEY, ISCCFG_PAGE_CACHE_EXPANSION_KEY, ISCCFG_STARTING_TRANSACTION_ID_KEY, ISCCFG_DATABASE_ODS_VERSION_KEY, ISCCFG_HOSTLIC_IMPORT_DIR_KEY, ISCCFG_HOSTLIC_INFO_DIR_KEY, ISCCFG_ENABLE_PARTIAL_INDEX_SELECTIVITY_KEY and ISCCFG_PREDICTIVE_IO_PAGES_KEY
*	Removed isc_info_sql_relation_alias, isc_info_sql_explain_plan and isc_info_sql_stmt_flags
*	Added isc_info_sql_precision
*	Added isc_info_sql_stmt_set_subscription and isc_info_sql_stmt_truncate
*	isc_info_isc_version changed to isc_info_version
*	Removed (these did not overlap though) frb_info_att_charset, isc_info_db_class, isc_info_firebird_version, isc_info_oldest_transaction, isc_info_oldest_active, isc_info_oldest_snapshot, isc_info_next_transaction, isc_info_db_provider and isc_info_active_transactions 
*	Added isc_info_db_reads, isc_info_db_writes, isc_info_db_fetches, isc_info_db_marks, isc_info_db_group_commit, isc_info_svr_min_ver, isc_info_ib_env_var, isc_info_server_tcp_port,t isc_info_db_preallocate, isc_info_db_encrypted, isc_info_db_encryptions, isc_info_db_sep_external, isc_info_db_eua_active and isc_info_db_creation_timestamp
*	Added isc_bpb_target_relation_name and isc_bpb_target_field_name
*	Added isc_blob_untyped, isc_blob_text, isc_blob_blr, isc_blob_acl, isc_blob_ranges, isc_blob_summary, isc_blob_format, isc_blob_tra, isc_blob_extfile, isc_blob_formatted_memo, isc_blob_paradox_ole, isc_blob_graphic, isc_blob_dbase_ole and isc_blob_typed_binary
*	Removed P_REQ_async and EPB_version1
*	isc_arg_sql_state changed to isc_arg_sql and added isc_arg_int64
*	Removed blr_begin, blr_message, blr_bool, blr_dec64, blr_dec128, blr_int128, blr_sql_time_tz, blr_timestamp_tz, blr_ex_time_tz and blr_ex_timestamp_tz
*	Added blr_boolean_dtype, blr_date, blr_inner, blr_left, blr_right, blr_full, blr_gds_code, blr_sql_code, blr_exception, blr_trigger_code, blr_default_code, blr_immediate, blr_deferred, blr_restrict, blr_cascade and blr_version4
*	Added all blr constants from the current (Ib2020) ibase.h
*	Removed SQL_TIMESTAMP_TZ_EX, SQL_TIME_TZ_EX, SQL_INT128, SQL_TIMESTAMP_TZ, SQL_TIME_TZ, SQL_DEC16, SQL_DEC34, SQL_BOOLEAN and SQL_NULL
*	Added SQL_BOOLEAN
*	Removed fb_cancel_disable, fb_cancel_enable, fb_cancel_raise and fb_cancel_abort
*	Removed CNCT_user, CNCT_passwd, CNCT_host, CNCT_group, CNCT_user_verification, CNCT_specific_data, CNCT_plugin_name, CNCT_login, CNCT_plugin_list and CNCT_client_crypt
		
## IscError.cs	
*	  StrParam rmeoved isc_arg_sql_state and added isc_arg_sql to the _strParam return section of the switched
	
## IscErrorMessages.cs
*	Replaced the String "Firebird" with "InterBase" in the error messages.
		
## IscException.cs
*	Removed BuildSqlState and ForSQLSTATE (Fb feature)
*	Removed all to BuildSqlState in BuildExceptionData
		
## IscHelper.cs
*	ParseDatabaseInfo got support for isc_info_db_encrypted, isc_info_db_encryptions, isc_info_db_sep_external, isc_info_db_preallocate and isc_info_db_eua_active
*	ParseDatabaseInfo got enhanced update for isc_info_forced_writes
*	ParseDatabaseInfo removed isc_info_firebird_version, isc_info_db_class, isc_info_oldest_transaction, isc_info_oldest_active, isc_info_oldest_snapshot, isc_info_next_transaction and isc_info_active_transactions support
		
## IServiceManager.cs
* Removed crypto from Attach method.

## PageSizeHelper.cs
* Removed 32768 size

## ParameterBuffer.cs
* Added a Write(long) override

## RemoteEvent.cs
* Removed adding EPB_version1 when building the Epb	
		
## ServiceParameterBuffer.cs
* Added an Append overload accepting (int, long)
		
## StatementBase.cs
*	 Removed DescribeExplaindPlanInfoItems
*	Removed GetExecutionExplainedPlan
*	ProcessRecordsAffectedBuffer does not include selects because IB does not support selects (always 42)
		
## TransactionBase.cs
*	Added to the Abstract class StartSavepoint(string name), RollbackSavepoint(string name) and ReleaseSavepoint(string name)
		
## TypeDecoder.cs
* Removed DecodeDec16, DecodeDec34 and DecodeInt128	decode methods (Fb specific items)
		
## TypeEncoder.cs	
* Removed encodeDec16, EncodeDec34 and encodeInt128	decode methods (Fb specific items)
	
## TypeHelper.cs
* Removed from GetSize DbDataType.Dec16, DbDataType.TimeTZ, DbDataType.TimeStampTZEx, DbDataType.Dec34, DbDataType.Int128, DbDataType.TimeStampTZ and DbDataType.TimeTZEx
*	GetSqlTypeFromDbDataType removed DbDataType.Dec16, DbDataType.TimeTZ, DbDataType.TimeStampTZEx, DbDataType.Dec34, DbDataType.Int128, DbDataType.TimeStampTZ and DbDataType.TimeTZEx 
*	Renamed GetFbDataTypeFromType to GetIBDataTypeFromType which returns the IBDbType that corresponds to the passed .NET types
*	GetSqlTypeFromBlrType 
		* Added support for blr_boolean_dtype and blr_boolean
		*	Removed support for blr_bool, blr_ex_timestamp_tz, blr_timestamp_tz, blr_sql_time_tz, blr_ex_time_tz, blr_dec64, blr_dec128 and blr_int128 
*	GetDataTypeName removed DbDataType.TimeStampTZ, DbDataType.TimeStampTZEx, DbDataType.TimeTZ, DbDataType.TimeTZEx, DbDataType.Dec16, DbDataType.Dec34 and DbDataType.Int128
*	GetTypeFromDbDataType removed DbDataType.TimeStampTZ, DbDataType.TimeStampTZEx, DbDataType.TimeTZ, DbDataType.TimeTZEx, DbDataType.Dec16, DbDataType.Dec34 and DbDataType.Int128
*	GetDbTypeFromDbDataType removed DbDataType.TimeStampTZ, DbDataType.TimeStampTZEx, DbDataType.TimeTZ, DbDataType.TimeTZEx, DbDataType.Dec16, DbDataType.Dec34 and DbDataType.Int128
*	GetDbDataTypeFromSqlType removed IscCodes.SQL_NULL, IscCodes.TimeStampTZ, IscCodes.TimeStampTZEx, IscCodes.TimeTZ, IscCodes.TimeTZEx, IscCodes.Dec16, IscCodes.Dec34 and IscCodes.Int128
*	GetDbDataTypeFromFbDbType renamed GetDbDataTypeFromIBDbType
*	Removed CreateZonedDateTime
*	Removed CreateZonedTime
		
