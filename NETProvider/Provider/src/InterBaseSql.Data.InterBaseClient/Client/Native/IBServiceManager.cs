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
using InterBaseSql.Data.Client.Native.Handles;
using InterBaseSql.Data.InterBaseClient;
using System.Runtime.InteropServices;

namespace InterBaseSql.Data.Client.Native;

internal sealed class IBServiceManager : ServiceManagerBase
{
	#region Fields

	private readonly IIBClient _ibClient;
	private IntPtr[] _statusVector;

	#endregion

	#region Constructors

	public IBServiceManager(IBServerType clientType, Charset charset)
		: base(charset)
	{
		_ibClient = IBClientFactory.GetGDSLibrary(clientType);
		HandlePtr = new ServiceHandle();
		_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
	}

	#endregion

	#region Methods

	public override void Attach(ServiceParameterBufferBase spb, string dataSource, int port, string service)
	{

		StatusVectorHelper.ClearStatusVector(_statusVector);

		var svcHandle = HandlePtr;
		string Service;
		if ((port > 0) || (dataSource != ""))
			Service = dataSource + ((port > 0) ? "/" + port.ToString() : "") + ":" + service;
		else
			Service = service;
		_ibClient.isc_service_attach(
			_statusVector,
			(short)Service.Length,
			Service,
			ref svcHandle,
			spb.Length,
			spb.ToArray());

		ProcessStatusVector(Charset.DefaultCharset);

		HandlePtr = svcHandle;
	}
	public override ValueTask AttachAsync(ServiceParameterBufferBase spb, string dataSource, int port, string service, CancellationToken cancellationToken = default)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);
		var svcHandle = HandlePtr;

		_ibClient.isc_service_attach(
			_statusVector,
			(short)service.Length,
			service,
			ref svcHandle,
			spb.Length,
			spb.ToArray());

		ProcessStatusVector(Charset.DefaultCharset);
		HandlePtr = svcHandle;
		return ValueTask2.CompletedTask;
	}

	public override void Detach()
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);
		var svcHandle = HandlePtr;
		_ibClient.isc_service_detach(_statusVector, ref svcHandle);
		ProcessStatusVector();
		HandlePtr = svcHandle;
	}
	public override ValueTask DetachAsync(CancellationToken cancellationToken = default)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);
		var svcHandle = HandlePtr;
		_ibClient.isc_service_detach(_statusVector, ref svcHandle);

		ProcessStatusVector();
		HandlePtr = svcHandle;
		return ValueTask2.CompletedTask;
	}

	public override void Start(ServiceParameterBufferBase spb)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);

		var svcHandle = HandlePtr;
		ServiceHandle reserved = new ServiceHandle();

		_ibClient.isc_service_start(
			_statusVector,
			ref svcHandle,
			ref reserved,
			spb.Length,
			spb.ToArray());

		ProcessStatusVector();
	}
	public override ValueTask StartAsync(ServiceParameterBufferBase spb, CancellationToken cancellationToken = default)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);
		var svcHandle = HandlePtr;
		var reserved = new ServiceHandle();

		_ibClient.isc_service_start(
			_statusVector,
			ref svcHandle,
			ref reserved,
			spb.Length,
			spb.ToArray());

		ProcessStatusVector();
		return ValueTask2.CompletedTask;
	}

	public override void Query(ServiceParameterBufferBase spb, int requestLength, byte[] requestBuffer, int bufferLength, byte[] buffer)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);

		var svcHandle = HandlePtr;
		ServiceHandle reserved = new ServiceHandle();

		_ibClient.isc_service_query(
			_statusVector,
			ref svcHandle,
			ref reserved,
			spb.Length,
			spb.ToArray(),
			(short)requestLength,
			requestBuffer,
			(short)buffer.Length,
			buffer);

		ProcessStatusVector();
	}

	public override ValueTask QueryAsync(ServiceParameterBufferBase spb, int requestLength, byte[] requestBuffer, int bufferLength, byte[] buffer, CancellationToken cancellationToken = default)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);

		var svcHandle = HandlePtr;
		var reserved = new ServiceHandle();

		_ibClient.isc_service_query(
			_statusVector,
			ref svcHandle,
			ref reserved,
			spb.Length,
			spb.ToArray(),
			(short)requestLength,
			requestBuffer,
			(short)buffer.Length,
			buffer);

		ProcessStatusVector();

		return ValueTask2.CompletedTask;
	}

	public override ServiceParameterBufferBase CreateServiceParameterBuffer()
	{
		return new ServiceParameterBuffer(ParameterBufferEncoding);
	}

	#endregion

	#region Private Methods

	private void ProcessStatusVector()
	{
		StatusVectorHelper.ProcessStatusVector(_statusVector, Charset, WarningMessage);
	}

	private void ProcessStatusVector(Charset charset)
	{
		StatusVectorHelper.ProcessStatusVector(_statusVector, charset, WarningMessage);
	}

	#endregion
}