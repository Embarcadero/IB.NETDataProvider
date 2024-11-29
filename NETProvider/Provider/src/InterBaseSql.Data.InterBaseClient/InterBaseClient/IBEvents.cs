/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/Embarcadero/IB.NETDataProvider/blob/main/LICENSE.
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

//$Authors = Embarcadero, Jeff Overcash

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.Client.Native;


namespace InterBaseSql.Data.InterBaseClient;

public class IBEvents
{
	const int MaxEventNameLength = 255;
	const int MaxEpbLength = 65535;

	private List<string> _events;
	internal List<IBEventThread> _threads;
	private bool _registered;

	private IBConnection _connection;

	public IBConnection Connection
	{
		get { return _connection; }
		set
		{
			if ((Registered) && (value != _connection))
				throw new InvalidOperationException("Unregister events before changing the connection");
			_connection = value;
		}
	}

	public List<string> Events
	{
		get { return _events; }
		set
		{
			if (value.Count > 15)
				throw new InvalidOperationException("Events are currently restricted to a max 15");
			_events = value.ToList();
		}
	}

	protected void ValidateDatabase()
	{
		if (_connection == null)
			throw new InvalidOperationException("Connection not assigned.");
		if (_connection.State != ConnectionState.Open)
			throw new InvalidOperationException("Connection not open");
	}

	public bool Registered
	{
		get { return _registered; }
		set
		{
			_registered = value;
			if (value)
				RegisterEvents();
			else
				UnRegisterEvents();
		}
	}

	public IBEvents()
	{
		_events = new List<string>();
		_threads = new List<IBEventThread>();
	}

	~IBEvents()
	{
		if (Registered)
			UnRegisterEvents();
	}

	public void RegisterEvents()
	{
		if (!_registered)
		{
			for (int i = 0; i <= (_events.Count / IBEventThread.MaxEventBlock); i++)
			{
				_threads.Add(new IBEventThread(this, i));
				Thread.Sleep(100);
			}
		}
		else
			throw new InvalidOperationException("Unregister old events before registering new ones.");
		_registered = _threads.Count != 0;
	}

	public void UnRegisterEvents()
	{
		for (int i = _threads.Count - 1; i >= 0; i--)
		{
			IBEventThread t = _threads[i];
			t.SignalTerminate();
			while (t.ThreadRunning)
			{
				Thread.Sleep(100);
			}
			_threads.Remove(t);
		}
		_registered = _threads.Count != 0;
	}

	internal virtual void OnEventAlert(EventAlertArgs e)
	{
		EventAlertHandler handler = EventAlert;
		if (handler != null)
		{
			handler(this, e);
		}
	}

/*		internal virtual void OnEventError(EventErrorArgs e)
	{
		EventErrorHandler handler = EventError;
		if (handler != null)
		{
			handler(this, e);
		}
	}*/

	public event EventAlertHandler EventAlert;

//		public event EventErrorHandler EventError;

}

public delegate void EventAlertHandler(object sender, EventAlertArgs e);
//	public delegate void EventErrorHandler(object sender, EventErrorArgs e);

public class EventAlertArgs : EventArgs
{
	public string EventName { get; set; }

	public int Count { get; set; }

	public bool CancelAlerts { get; set; }
}

/*	public class EventErrorArgs : EventArgs
{
	public int ErrorCode { get; set; }
}*/
