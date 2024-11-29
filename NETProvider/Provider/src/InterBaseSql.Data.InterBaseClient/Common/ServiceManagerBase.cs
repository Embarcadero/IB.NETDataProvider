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
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using InterBaseSql.Data.Client.Native.Handles;

namespace InterBaseSql.Data.Common;

internal abstract class ServiceManagerBase
{
	public Action<IscException> WarningMessage { get; set; }

	public Encoding ParameterBufferEncoding => Encoding2.Default;

	public Charset Charset { get; }
	public int Handle => HandlePtr.DangerousGetHandle().AsInt();

	public ServiceHandle HandlePtr;

	public ServiceManagerBase(Charset charset)
	{
		Charset = charset;
	}

	public abstract void Attach(ServiceParameterBufferBase spb, string dataSource, int port, string service);
	public abstract ValueTask AttachAsync(ServiceParameterBufferBase spb, string dataSource, int port, string service, CancellationToken cancellationToken = default);

	public abstract void Detach();
	public abstract ValueTask DetachAsync(CancellationToken cancellationToken = default);

	public abstract void Start(ServiceParameterBufferBase spb);
	public abstract ValueTask StartAsync(ServiceParameterBufferBase spb, CancellationToken cancellationToken = default);

	public abstract void Query(ServiceParameterBufferBase spb, int requestLength, byte[] requestBuffer, int bufferLength, byte[] buffer);
	public abstract ValueTask QueryAsync(ServiceParameterBufferBase spb, int requestLength, byte[] requestBuffer, int bufferLength, byte[] buffer, CancellationToken cancellationToken = default);

	public abstract ServiceParameterBufferBase CreateServiceParameterBuffer();
}
