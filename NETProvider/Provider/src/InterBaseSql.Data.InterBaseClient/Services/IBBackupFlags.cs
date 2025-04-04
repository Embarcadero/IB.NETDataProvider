﻿/*
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

using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.Services;

[Flags]
public enum IBBackupFlags
{
	IgnoreChecksums = IscCodes.isc_spb_bkp_ignore_checksums,
	IgnoreLimbo = IscCodes.isc_spb_bkp_ignore_limbo,
	MetaDataOnly = IscCodes.isc_spb_bkp_metadata_only,
	NoGarbageCollect = IscCodes.isc_spb_bkp_no_garbage_collect,
	OldDescriptions = IscCodes.isc_spb_bkp_old_descriptions,
	NonTransportable = IscCodes.isc_spb_bkp_non_transportable,
	Convert = IscCodes.isc_spb_bkp_convert,
	Expand = IscCodes.isc_spb_bkp_expand,
}
