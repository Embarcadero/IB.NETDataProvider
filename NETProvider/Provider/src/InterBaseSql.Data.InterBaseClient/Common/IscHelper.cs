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
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Common;

internal static class IscHelper
{
	public static List<object> ParseDatabaseInfo(byte[] buffer, Charset charset)
	{
		var info = new List<object>();

		var pos = 0;
		var length = 0;
		var type = 0;

		while ((type = buffer[pos++]) != IscCodes.isc_info_end)
		{
			length = (int)VaxInteger(buffer, pos, 2);
			pos += 2;

			switch (type)
			{
				case IscCodes.isc_info_error:
					throw IBException.Create("Received error response.");
				//
				// Database characteristics
				//
				case IscCodes.isc_info_allocation:
				case IscCodes.isc_info_ods_version:
				case IscCodes.isc_info_ods_minor_version:
				case IscCodes.isc_info_page_size:
				case IscCodes.isc_info_current_memory:
				case IscCodes.isc_info_max_memory:
				case IscCodes.isc_info_num_buffers:
				case IscCodes.isc_info_sweep_interval:
				case IscCodes.isc_info_fetches:
				case IscCodes.isc_info_marks:
				case IscCodes.isc_info_reads:
				case IscCodes.isc_info_writes:
				case IscCodes.isc_info_backout_count:
				case IscCodes.isc_info_delete_count:
				case IscCodes.isc_info_expunge_count:
				case IscCodes.isc_info_insert_count:
				case IscCodes.isc_info_purge_count:
				case IscCodes.isc_info_read_idx_count:
				case IscCodes.isc_info_read_seq_count:
				case IscCodes.isc_info_update_count:
				case IscCodes.isc_info_db_size_in_pages:
				case IscCodes.isc_info_db_sql_dialect:
					info.Add(VaxInteger(buffer, pos, length));
					break;

				case IscCodes.isc_info_no_reserve:
				case IscCodes.isc_info_db_read_only:
					info.Add(buffer[pos] == 1);
					break;

				case IscCodes.isc_info_user_names:
					info.Add(charset.GetString(buffer, pos + 1, buffer[pos]));
					break;

				case IscCodes.isc_info_base_level:
					info.Add(string.Format(CultureInfo.CurrentCulture, "{0}.{1}", buffer[pos], buffer[pos + 1]));
					break;

				case IscCodes.isc_info_db_id:
					{
						var dbFile = charset.GetString(buffer, pos + 2, buffer[pos + 1]);
						var sitePos = pos + 2 + buffer[pos + 1];
						int siteLength = buffer[sitePos];
						var siteName = charset.GetString(buffer, sitePos + 1, siteLength);

						sitePos += siteLength + 1;
						siteLength = buffer[sitePos];
						siteName += "." + charset.GetString(buffer, sitePos + 1, siteLength);

						info.Add(siteName + ":" + dbFile);
					}
					break;

				case IscCodes.isc_info_implementation:
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

				case IscCodes.isc_info_db_preallocate:
					info.Add(VaxInteger(buffer, pos, length));
					break;

				case IscCodes.isc_info_version:
					var messagePosition = pos;
					var count = buffer[messagePosition];
					for (var i = 0; i < count; i++)
					{
						var messageLength = buffer[messagePosition + 1];
						info.Add(charset.GetString(buffer, messagePosition + 2, messageLength));
						messagePosition += 1 + messageLength;
					}
					break;

				//
				// Environmental characteristics
				//

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