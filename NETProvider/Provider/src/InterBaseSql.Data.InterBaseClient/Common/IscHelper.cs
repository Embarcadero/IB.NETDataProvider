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
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace InterBaseSql.Data.Common
{
	internal static class IscHelper
	{
		public static List<object> ParseDatabaseInfo(byte[] buffer)
		{
			var info = new List<object>();

			var pos = 0;
			int length;
			int type;

			while ((type = buffer[pos++]) != IscCodes.isc_info_end)
			{
				length = (int)VaxInteger(buffer, pos, 2);
				pos += 2;

				switch (type)
				{
					//
					// Database characteristics
					//
					case IscCodes.isc_info_allocation:
						// Number of database pages allocated
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_base_level:
						/* Database version (level) number:
						 * - 1 byte containing the number 1
						 * - 1 byte containing the version number
						 */
						info.Add(string.Format(CultureInfo.CurrentCulture, "{0}.{1}", buffer[pos], buffer[pos + 1]));
						break;

					case IscCodes.isc_info_db_id:
						/* Database file name and site name:
						 * - 1 byte containing the number 2
						 * - 1 byte containing the length, d, of the database file name in bytes
						 * - A string of d bytes, containing the database file name
						 * - 1 byte containing the length, l, of the site name in bytes
						 * - A string of l bytes, containing the site name
						 */
						var dbFile = Encoding.Default.GetString(buffer, pos + 2, buffer[pos + 1]);
						var sitePos = pos + 2 + buffer[pos + 1];
						int siteLength = buffer[sitePos];
						var siteName = Encoding.Default.GetString(buffer, sitePos + 1, siteLength);

						sitePos += siteLength + 1;
						siteLength = buffer[sitePos];
						siteName += "." + Encoding.Default.GetString(buffer, sitePos + 1, siteLength);

						info.Add(siteName + ":" + dbFile);
						break;

					case IscCodes.isc_info_implementation:
						/* Database implementation number:
						 * - 1 byte containing a 1
						 * - 1 byte containing the implementation number
						 * - 1 byte containing a class number, either 1 or 12
						 */
						info.Add(string.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2}", buffer[pos], buffer[pos + 1], buffer[pos + 2]));
						break;

					case IscCodes.isc_info_db_encrypted:
						/* 0 or 1
						 * - 0 not encrypted
						 * - 1 encrypted
						 */
						info.Add(buffer[pos] == 1 ? true : false);
						break;

					case IscCodes.isc_info_db_encryptions:
						/* Number of encryptions */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_db_sep_external:
						/* Is Sep Encryption external
						 * - 0 not encrypted
						 * - 1 encrypted
						 */
						info.Add(buffer[pos] == 1 ? true : false);
						break;

					case IscCodes.isc_info_db_eua_active:
						/* 0 or 1
						 * - 0 not eua off
						 * - 1 eua on
						 */
						info.Add(buffer[pos] == 1 ? true : false);
						break;

					case IscCodes.isc_info_no_reserve:
						/* 0 or 1
						 * - 0 indicates space is reserved on each database page for holding
						 *     backup versions of modified records [Default]
						 * - 1 indicates no space is reserved for such records
						 */
						info.Add(buffer[pos] == 1 ? true : false);
						break;

					case IscCodes.isc_info_ods_version:
						/* ODS major version number
						 * - Databases with different major version numbers have different
						 *   physical layouts; a database engine can only access databases
						 *   with a particular ODS major version number
						 * - Trying to attach to a database with a different ODS number
						 *   results in an error
						 */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_ods_minor_version:
						/* On-disk structure (ODS) minor version number; an increase in a
						 * minor version number indicates a non-structural change, one that
						 * still allows the database to be accessed by database engines with
						 * the same major version number but possibly different minor
						 * version numbers
						 */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_page_size:
						/* Number of bytes per page of the attached database; use with
						 * isc_info_allocation to determine the size of the database
						 */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_db_preallocate:
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_version:
						/* Version identification string of the database implementation:
						 * - 1 byte containing the number number of message
						 * - 1 byte specifying the length, of the following string
						 * - n bytes containing the string
						 */
						var messagePosition = pos;
						var count = buffer[messagePosition];
						for (var i = 0; i < count; i++)
						{
							var messageLength = buffer[messagePosition + 1];
							info.Add(Encoding.Default.GetString(buffer, messagePosition + 2, messageLength));
							messagePosition += 1 + messageLength;
						}
						break;

					//
					// Environmental characteristics
					//

					case IscCodes.isc_info_current_memory:
						// Amount of server memory (in bytes) currently in use
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_forced_writes:
						/* Number specifying the mode in which database writes are performed
						 * (0 for asynchronous, 1 for synchronous, 2 for direct)
						 */
						switch (buffer[pos])
						{
							case 0:
							    info.Add("ASync");
								break;
							case 1:
								info.Add("Sync");
								break;
							case 2:
								info.Add("Direct");
								break;
							default:
								info.Add("Unknown");
								break;
						}
						break;

					case IscCodes.isc_info_max_memory:
						/* Maximum amount of memory (in bytes) used at one time since the first
						 * process attached to the database
						 */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_num_buffers:
						// Number of memory buffers currently allocated
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_sweep_interval:
						/* Number of transactions that are committed between sweeps to
						 * remove database record versions that are no longer needed
						 */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					//
					// Performance statistics
					//

					case IscCodes.isc_info_fetches:
						// Number of reads from the memory buffer cache
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_marks:
						// Number of writes to the memory buffer cache
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_reads:
						// Number of page reads
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_writes:
						// Number of page writes
						info.Add(VaxInteger(buffer, pos, length));
						break;

					//
					// Database operation counts
					//

					case IscCodes.isc_info_backout_count:
						// Number of removals of a version of a record
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_delete_count:
						// Number of database deletes since the database was last attached
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_expunge_count:
						/* Number of removals of a record and all of its ancestors, for records
						 * whose deletions have been committed
						 */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_insert_count:
						// Number of inserts into the database since the database was last attached
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_purge_count:
						// Number of removals of old versions of fully mature records
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_read_idx_count:
						// Number of reads done via an index since the database was last attached
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_read_seq_count:
						/* Number of sequential sequential table scans (row reads) done on each
						 * table since the database was last attached
						 */
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_update_count:
						// Number of database updates since the database was last attached
						info.Add(VaxInteger(buffer, pos, length));
						break;

					//
					// Misc
					//

					case IscCodes.isc_info_db_read_only:
						info.Add(buffer[pos] == 1 ? true : false);
						break;

					case IscCodes.isc_info_db_size_in_pages:
						// Database size in pages.
						info.Add(VaxInteger(buffer, pos, length));
						break;

					case IscCodes.isc_info_user_names:
						// Active user name
						info.Add(Encoding.Default.GetString(buffer, pos + 1, buffer[pos]));
						break;
				}

				pos += length;
			}

			return info;
		}

		public static long VaxInteger(byte[] buffer, int index, int length)
		{
			var value = 0L;
			var shift = 0;
			var i = index;
			while (--length >= 0)
			{
				value += (buffer[i++] & 0xffL) << shift;
				shift += 8;
			}
			return value;
		}
	}
}
