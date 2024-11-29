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
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Services;

public sealed class IBLog : IBService
{
	public IBLog(string connectionString = null)
		: base(connectionString)
	{ }

	public void Execute()
	{
		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_get_ib_log);
				StartTask(startSpb);
				ProcessServiceOutput(new ServiceParameterBuffer(Service.ParameterBufferEncoding));
			}
			finally
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			throw IBException.Create(ex);
		}
	}
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_get_ib_log);
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
				await ProcessServiceOutputAsync(new ServiceParameterBuffer(Service.ParameterBufferEncoding), cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				await CloseAsync(cancellationToken).ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			throw IBException.Create(ex);
		}
	}
}