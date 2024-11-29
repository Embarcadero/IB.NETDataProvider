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
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.Serialization;

using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient;

[Serializable]
public sealed class IBException : DbException
{
	#region Fields

	private IBErrorCollection _errors;

	#endregion

	#region Properties

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public IBErrorCollection Errors => _errors ??= new IBErrorCollection();
	public override int ErrorCode => (InnerException as IscException)?.ErrorCode ?? 0;
	public string SQLSTATE => (InnerException as IscException)?.SQLSTATE;

	#endregion

	#region Constructors

	private IBException(string message, Exception innerException)
		: base(message, innerException)
	{ }

	private IBException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		_errors = (IBErrorCollection)info.GetValue("errors", typeof(IBErrorCollection));
	}

	#endregion

	#region Methods

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);

		info.AddValue("errors", _errors);
	}

	#endregion

	#region Private Methods

	private void ProcessIscExceptionErrors(IscException innerException)
	{
		foreach (var error in innerException.Errors)
		{
			Errors.Add(error.Message, error.ErrorCode);
		}
	}

	#endregion


	internal static Exception Create(string message) => Create(message, null);
	internal static Exception Create(Exception innerException) => Create(null, innerException);
	internal static Exception Create(string message, Exception innerException)
	{
		message ??= innerException?.Message;
		if (innerException is IscException iscException)
		{
			var result = new IBException(message, innerException);
			result.ProcessIscExceptionErrors(iscException);
			return result;
		}
		else
		{
			return new IBException(message, innerException);
		}
	}
}
