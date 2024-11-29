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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

namespace InterBaseSql.Data.InterBaseClient;

[Serializable]
[ListBindable(false)]
public sealed class IBErrorCollection : ICollection<IBError>
{
	#region Fields

	private List<IBError> _errors;

	#endregion

	#region Constructors

	internal IBErrorCollection()
	{
		_errors = new List<IBError>();
	}

	#endregion

	#region Properties

	public int Count
	{
		get { return _errors.Count; }
	}

	public bool IsReadOnly
	{
		get { return true; }
	}

	#endregion

	#region Methods

	internal int IndexOf(string errorMessage)
	{
		return _errors.FindIndex(x => string.Equals(x.Message, errorMessage, StringComparison.CurrentCultureIgnoreCase));
	}

	internal IBError Add(IBError error)
	{
		_errors.Add(error);

		return error;
	}

	internal IBError Add(string errorMessage, int number)
	{
		return Add(new IBError(errorMessage, number));
	}

	void ICollection<IBError>.Add(IBError item)
	{
		throw new NotSupportedException();
	}

	void ICollection<IBError>.Clear()
	{
		throw new NotSupportedException();
	}

	public bool Contains(IBError item)
	{
		return _errors.Contains(item);
	}

	public void CopyTo(IBError[] array, int arrayIndex)
	{
		_errors.CopyTo(array, arrayIndex);
	}

	bool ICollection<IBError>.Remove(IBError item)
	{
		throw new NotSupportedException();
	}

	public IEnumerator<IBError> GetEnumerator()
	{
		return _errors.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	#endregion
}
