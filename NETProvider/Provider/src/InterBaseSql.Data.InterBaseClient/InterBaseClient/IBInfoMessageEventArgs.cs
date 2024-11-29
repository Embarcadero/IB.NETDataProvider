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

//$Authors = Carlos Guzman Alvarez

using System;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient;

public sealed class IBInfoMessageEventArgs : EventArgs
{
	#region Fields

	private IBErrorCollection _errors;
	private string _message;

	#endregion

	#region Properties

	public IBErrorCollection Errors
	{
		get { return _errors; }
	}

	public string Message
	{
		get { return _message; }
	}

	#endregion

	#region Constructors

	internal IBInfoMessageEventArgs(IscException ex)
	{
		_message = ex.Message;
		_errors = new IBErrorCollection();
		foreach (var error in ex.Errors)
		{
			_errors.Add(error.Message, error.ErrorCode);
		}
	}

	#endregion
}
