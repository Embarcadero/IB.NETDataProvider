/*/*
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

public sealed class IBConfiguration : IBService
{
	public IBConfiguration(string connectionString = null)
		: base(connectionString)
	{ }

	public void SetSqlDialect(int sqlDialect)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_set_sql_dialect, sqlDialect);
				StartTask(startSpb);
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
	public async Task SetSqlDialectAsync(int sqlDialect, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_set_sql_dialect, sqlDialect);
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void SetSweepInterval(int sweepInterval)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_sweep_interval, sweepInterval);
				StartTask(startSpb);
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
	public async Task SetSweepIntervalAsync(int sweepInterval, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_sweep_interval, sweepInterval);
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void SetPageBuffers(int pageBuffers)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_page_buffers, pageBuffers);
				StartTask(startSpb);
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
	public async Task SetPageBuffersAsync(int pageBuffers, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_page_buffers, pageBuffers);
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void DatabaseShutdown(IBShutdownMode mode, int seconds)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				switch (mode)
				{
					case IBShutdownMode.Forced:
						startSpb.Append(IscCodes.isc_spb_prp_shutdown_db, seconds);
						break;
					case IBShutdownMode.DenyTransaction:
						startSpb.Append(IscCodes.isc_spb_prp_deny_new_transactions, seconds);
						break;
					case IBShutdownMode.DenyConnection:
						startSpb.Append(IscCodes.isc_spb_prp_deny_new_attachments, seconds);
						break;
				}
				StartTask(startSpb);
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
	public async Task DatabaseShutdownAsync(IBShutdownMode mode, int seconds, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				switch (mode)
				{
					case IBShutdownMode.Forced:
						startSpb.Append(IscCodes.isc_spb_prp_shutdown_db, seconds);
						break;
					case IBShutdownMode.DenyTransaction:
						startSpb.Append(IscCodes.isc_spb_prp_deny_new_transactions, seconds);
						break;
					case IBShutdownMode.DenyConnection:
						startSpb.Append(IscCodes.isc_spb_prp_deny_new_attachments, seconds);
						break;
				}
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void DatabaseOnline()
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_options, IscCodes.isc_spb_prp_db_online);
				StartTask(startSpb);
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
	public async Task DatabaseOnlineAsync(CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_options, IscCodes.isc_spb_prp_db_online);
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void ActivateShadows()
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_options, IscCodes.isc_spb_prp_activate);
				StartTask(startSpb);
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
	public async Task ActivateShadowsAsync(CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_options, IscCodes.isc_spb_prp_activate);
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void SetForcedWrites(bool forcedWrites)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				if (forcedWrites)
				{
					startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_sync);
				}
				else
				{
					startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_async);
				}
				StartTask(startSpb);
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
	public async Task SetForcedWritesAsync(bool forcedWrites, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				if (forcedWrites)
				{
					startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_sync);
				}
				else
				{
					startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_async);
				}
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void SetReserveSpace(bool reserveSpace)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				if (reserveSpace)
				{
					startSpb.Append(IscCodes.isc_spb_prp_reserve_space, (byte)IscCodes.isc_spb_prp_res);
				}
				else
				{
					startSpb.Append(IscCodes.isc_spb_prp_reserve_space, (byte)IscCodes.isc_spb_prp_res_use_full);
				}
				StartTask(startSpb);
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
	public async Task SetReserveSpaceAsync(bool reserveSpace, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				if (reserveSpace)
				{
					startSpb.Append(IscCodes.isc_spb_prp_reserve_space, (byte)IscCodes.isc_spb_prp_res);
				}
				else
				{
					startSpb.Append(IscCodes.isc_spb_prp_reserve_space, (byte)IscCodes.isc_spb_prp_res_use_full);
				}
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void SetAccessMode(bool readOnly)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_access_mode, (byte)(readOnly ? IscCodes.isc_spb_prp_am_readonly : IscCodes.isc_spb_prp_am_readwrite));
				StartTask(startSpb);
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
	public async Task SetAccessModeAsync(bool readOnly, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				startSpb.Append(IscCodes.isc_spb_prp_access_mode, (byte)(readOnly ? IscCodes.isc_spb_prp_am_readonly : IscCodes.isc_spb_prp_am_readwrite));
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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

	public void SetWriteMode(IBWriteMode Value)
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				switch (Value)
				{
					case IBWriteMode.wmSync:
						startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_sync);
						break;
					case IBWriteMode.wmASync:
						startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_async);
						break;
					case IBWriteMode.wmDirect:
						startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_direct);
						break;
				}
				StartTask(startSpb);
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
	public async Task SetWriteModeAsync(IBWriteMode Value, CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_properties);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				switch (Value)
				{
					case IBWriteMode.wmSync:
						startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_sync);
						break;
					case IBWriteMode.wmASync:
						startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_async);
						break;
					case IBWriteMode.wmDirect:
						startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_direct);
						break;
				}
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
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
}