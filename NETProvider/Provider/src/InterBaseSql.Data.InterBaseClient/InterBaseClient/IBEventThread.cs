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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using InterBaseSql.Data.Client.Native;
using InterBaseSql.Data.Common;
using System.ComponentModel;

namespace InterBaseSql.Data.InterBaseClient;

public class IBEventThread
{
	internal class ProgressInfo
	{
		internal int WhichEvent { get; set; }
		internal int Count { get; set; }
	}

	internal const int MaxEventBlock = 15;
	// API use
	private int _eventID;
	private IntPtr _eventBuffer;
	private short _eventBufferLen;
	private IntPtr _resultBuffer;
	private IntPtr _thisCallback;
	private uint[] _status;
	private IntPtr[] _statusVector;
	private static Dictionary<IntPtr, IBEventThread> _instance = new Dictionary<IntPtr, IBEventThread>();
	private static ibEventCallbackDelegate callback;
	// local use
	private BackgroundWorker _bgWorker;
	private EventWaitHandle _signal;
	private bool _eventsReceived, _firstTime;
	private int _eventGroup;
	private ushort _eventCount;
	private IBEvents _parent;
	private bool _cancelAlerts;
	private bool _terminate = false;

	internal IIBClient IBClient => Parent.Connection.IBDatabase.IBClient;

	public IBEvents Parent
	{
		get { return _parent; }
		set { _parent = value; }
	}

	public EventWaitHandle Signal { get { return _signal; } }

	public bool ReturnValue { get; set; }
	public bool HandleExceptions { get; set; }

	internal bool ThreadRunning { get { return !_terminate; } }

	public IBEventThread(IBEvents owner, int eventGrp)
	{
		_status = new uint[15];
		_eventGroup = eventGrp;
		ReturnValue = false;
		HandleExceptions = true;
		_signal = new EventWaitHandle(false, EventResetMode.ManualReset);
		_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
		callback = new ibEventCallbackDelegate(IBEventThread.EventCallback);
		Parent = owner;
		_bgWorker = new System.ComponentModel.BackgroundWorker();
		_bgWorker.WorkerReportsProgress = true;
		_bgWorker.DoWork +=new DoWorkEventHandler(Execute);
		_bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
		_bgWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
		_bgWorker.RunWorkerAsync();
	}

	~IBEventThread()
	{
		_instance.Remove(_thisCallback);
	}

	protected void Execute(object sender, DoWorkEventArgs e)
	{
		RegisterEvents();
		QueueEvents();
		try
		{
			do
			{
				Signal.WaitOne();
				if (_eventsReceived)
				{
					ProcessEvents();
				    QueueEvents();
				}
			}
			while (!_terminate);
			ReturnValue = false;
			UnRegisterEvents();
		}
		catch
		{
			if (HandleExceptions)
				ReturnValue = true;
			else
				ReturnValue = false;
		}
		e.Result = ReturnValue;
	}

	protected void SignalEvent()
	{
		_eventsReceived = true;
		Signal.Set();
	}

	internal void SignalTerminate()
	{
		if (_terminate == false)
		{
			_terminate = true;
			Signal.Set();
		}
	}

	private byte[] EBP(int Index)
	{
        Index = Index + (_eventGroup * MaxEventBlock);
		if (Index >= Parent.Events.Count)
			return null;
		else
			return Encoding.ASCII.GetBytes(Parent.Events[Index]);
	}

	protected void RegisterEvents()
	{
		var EBPArray = new Byte[MaxEventBlock][];
		_eventBuffer = IntPtr.Zero;
		_resultBuffer = IntPtr.Zero;
		_eventBufferLen = 0;
		_firstTime = true;
		_eventCount = (ushort) (Parent.Events.Count - (_eventGroup * MaxEventBlock));
		if (_eventCount > MaxEventBlock)
			_eventCount = MaxEventBlock;
		for ( var i = 0; i < MaxEventBlock; i++)
			EBPArray[i] = EBP(i);
		_eventBufferLen = (short) IBClient.isc_event_block(ref _eventBuffer, ref _resultBuffer, _eventCount, EBPArray);
	}

	protected void UnRegisterEvents()
	{
		var handle = ((IBDatabase)Parent.Connection.IBDatabase).HandlePtr;
		IBClient.isc_cancel_events(_statusVector, ref handle, ref _eventID);
		IBClient.isc_free(_eventBuffer);
		_eventBuffer = IntPtr.Zero;
		IBClient.isc_free(_resultBuffer);
		_resultBuffer = IntPtr.Zero;
	}

	protected void QueueEvents()
	{
		_eventsReceived = false;
		Signal.Reset();
		SQueEvents();
	}
	
 		private static void EventCallback(IntPtr p, short Length, IntPtr updated)
	{
		if ((p != IntPtr.Zero) && (updated != IntPtr.Zero))
		{
			IBEventThread iBEventThread = _instance[p];
			if (iBEventThread != null)
			{
				byte[] managedArray = new byte[Length];
				Marshal.Copy(updated, managedArray, 0, Length);
				iBEventThread.UpdateResultBuffer(Length, managedArray);
				iBEventThread._eventsReceived = true;
				iBEventThread.Signal.Set();
			}
		}
	}

	protected void SQueEvents()
	{
		try
		{
			_instance.Remove(_thisCallback);
			var handle = ((IBDatabase)Parent.Connection.IBDatabase).HandlePtr;
			GCHandle handle1 = GCHandle.Alloc(this);
			_thisCallback = GCHandle.ToIntPtr(handle1);
			handle1.Free();
			_instance.Add(_thisCallback, this);
			int status = (int) IBClient.isc_que_events(_statusVector, ref handle, ref _eventID, _eventBufferLen, _eventBuffer, callback, _thisCallback);
		}
		catch
		{
			/*				on E: Exception do
								if Assigned(Parent.OnError) then


					if E is EIBError then

					  Parent.OnError(Parent, EIBError(E).IBErrorCode)

					else
								Parent.OnError(Parent, 0);
							end;*/

		}
	}

	protected void ProcessEvents()
	{
		int _whichEvent, _countForEvent;
		IBClient.isc_event_counts(_status, _eventBufferLen, _eventBuffer, _resultBuffer);
		if (!_firstTime)
		{
			_cancelAlerts = false;
			for (int i = 0; i < _eventCount; i++)
			{
				if (_status[i] != 0)
				{
					_whichEvent = i;
					_countForEvent = (int)_status[_whichEvent];
					// This just forces the event into the main thread
					_bgWorker.ReportProgress(0, new ProgressInfo() { WhichEvent = _whichEvent, Count = _countForEvent});
				}
			}
		}
		_firstTime = false;
	}

	private void backgroundWorker1_ProgressChanged(object sender,
		ProgressChangedEventArgs e)
	{
		// 0 is for an event, 1 is for errors. This is used to get this into the main thread
		if (e.ProgressPercentage == 0)
		{
			ProgressInfo pi = e.UserState as ProgressInfo;
			var eventName = Parent.Events[((_eventGroup * MaxEventBlock) + pi.WhichEvent)];
			var args = new EventAlertArgs() { CancelAlerts = false, Count = pi.Count, EventName = eventName };
			Parent.OnEventAlert(args);
			_cancelAlerts = args.CancelAlerts;
		}
	}

	private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		Parent._threads.Remove(this);
	}

	protected void DoHandleException()
	{

	}

	protected bool HandleException()
	{
/*			if (!Parent.ThreadException)
		{
			Parent.ThreadException = true;
			if (FExceptObject is EAbort)
				Parent.OnEventError; // need synchronizing?
			return true;
		}
		else */
			return false;
	}

	internal void UpdateResultBuffer(short length, byte[] Updated)
	{
		unsafe
		{
			Span<byte> byteArray = new Span<byte>(_resultBuffer.ToPointer(), length);
			for (int i = 0; i < Updated.Length; i++)
			{
				byteArray[i] = Updated[i];
			}
		}
	}
}
