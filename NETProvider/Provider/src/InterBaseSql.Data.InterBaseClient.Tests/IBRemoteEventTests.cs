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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Security.Cryptography;
using System.Threading;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
public class IBRemoteEventTests : IBTestsBase
{
	public IBRemoteEventTests(IBServerType serverType)
		: base(serverType)
	{ }

	//[Test]
	//public void EventSimplyComesBackTest()
	//{
	//	var exception = (Exception)null;
	//	var triggered = false;
	//	var proccmd = Connection.CreateCommand();
	//	proccmd.CommandText = "create procedure event_test as begin begin post_event 'test'; end";
	//	proccmd.ExecuteNonQuery();
	//	try
	//	{
	//		using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//		{
	//			@event.RemoteEventError += (sender, e) =>
	//			{
	//				exception = e.Error;
	//			};
	//			@event.RemoteEventCounts += (sender, e) =>
	//			{
	//				triggered = e.Name == "test" && e.Counts == 1;
	//			};
	//			@event.QueueEvents(new[] { "test" });
	//			using (var cmd = Connection.CreateCommand())
	//			{
	//				cmd.CommandText = "execute procedure post_event";
	//				cmd.ExecuteNonQuery();
	//				Thread.Sleep(2000);
	//			}
	//			Assert.IsNull(exception);
	//			Assert.IsTrue(triggered);
	//		}
	//	}
	//	finally
	//	{
	//		proccmd.CommandText = "drop procedure event_test";
	//		proccmd.ExecuteNonQuery();
	//	}
	//}

	//[Test]
	//public void ProperCountsSingleTest()
	//{
	//	var exception = (Exception)null;
	//	var triggered = false;
	//	using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//	{
	//		@event.RemoteEventError += (sender, e) =>
	//		{
	//			exception = e.Error;
	//		};
	//		@event.RemoteEventCounts += (sender, e) =>
	//		{
	//			triggered = e.Name == "test" && e.Counts == 5;
	//		};
	//		@event.QueueEvents("test");
	//		using (var cmd = Connection.CreateCommand())
	//		{
	//			cmd.CommandText = "execute block as begin post_event 'test'; post_event 'test'; post_event 'test'; post_event 'test'; post_event 'test'; end";
	//			cmd.ExecuteNonQuery();
	//			Thread.Sleep(2000);
	//		}
	//		Assert.IsNull(exception);
	//		Assert.IsTrue(triggered);
	//	}
	//}

	//[Test]
	//public void EventNameSeparateSelectionTest()
	//{
	//	var exception = (Exception)null;
	//	var triggeredA = false;
	//	var triggeredB = false;
	//	using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//	{
	//		@event.RemoteEventError += (sender, e) =>
	//		{
	//			exception = e.Error;
	//		};
	//		@event.RemoteEventCounts += (sender, e) =>
	//		{
	//			switch (e.Name)
	//			{
	//				case "a":
	//					triggeredA = e.Counts == 1;
	//					break;
	//				case "b":
	//					triggeredB = e.Counts == 1;
	//					break;
	//			}
	//		};
	//		@event.QueueEvents("a", "b");
	//		using (var cmd = Connection.CreateCommand())
	//		{
	//			cmd.CommandText = "execute block as begin post_event 'b'; end";
	//			cmd.ExecuteNonQuery();
	//			cmd.CommandText = "execute block as begin post_event 'a'; end";
	//			cmd.ExecuteNonQuery();
	//			Thread.Sleep(2000);
	//		}
	//		Assert.IsNull(exception);
	//		Assert.IsTrue(triggeredA);
	//		Assert.IsTrue(triggeredB);
	//	}
	//}

	//[Test]
	//public void EventNameTogetherSelectionTest()
	//{
	//	var exception = (Exception)null;
	//	var triggeredA = false;
	//	var triggeredB = false;
	//	using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//	{
	//		@event.RemoteEventError += (sender, e) =>
	//		{
	//			exception = e.Error;
	//		};
	//		@event.RemoteEventCounts += (sender, e) =>
	//		{
	//			switch (e.Name)
	//			{
	//				case "a":
	//					triggeredA = e.Counts == 1;
	//					break;
	//				case "b":
	//					triggeredB = e.Counts == 1;
	//					break;
	//			}
	//		};
	//		@event.QueueEvents("a", "b");
	//		using (var cmd = Connection.CreateCommand())
	//		{
	//			cmd.CommandText = "execute block as begin post_event 'b'; post_event 'a'; end";
	//			cmd.ExecuteNonQuery();
	//			Thread.Sleep(2000);
	//		}
	//		Assert.IsNull(exception);
	//		Assert.IsTrue(triggeredA);
	//		Assert.IsTrue(triggeredB);
	//	}
	//}

	//[Test]
	//public void CancelTest()
	//{
	//	var exception = (Exception)null;
	//	var triggered = 0;
	//	using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//	{
	//		@event.RemoteEventError += (sender, e) =>
	//		{
	//			exception = e.Error;
	//		};
	//		@event.RemoteEventCounts += (sender, e) =>
	//		{
	//			triggered++;
	//		};
	//		@event.QueueEvents("test");
	//		using (var cmd = Connection.CreateCommand())
	//		{
	//			cmd.CommandText = "execute block as begin post_event 'test'; end";
	//			cmd.ExecuteNonQuery();
	//			Thread.Sleep(2000);
	//		}
	//		@event.CancelEvents();
	//		using (var cmd = Connection.CreateCommand())
	//		{
	//			cmd.CommandText = "execute block as begin post_event 'test'; end";
	//			cmd.ExecuteNonQuery();
	//			Thread.Sleep(2000);
	//		}
	//		Assert.IsNull(exception);
	//		Assert.AreEqual(1, triggered);
	//	}
	//}

	//[Test]
	//public void DoubleQueueingTest()
	//{
	//	using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//	{
	//		Assert.DoesNotThrow(() => @event.QueueEvents("test"));
	//		Assert.Throws<InvalidOperationException>(() => @event.QueueEvents("test"));
	//	}
	//}

	//[Test]
	//public void NoEventsAfterDispose()
	//{
	//	var triggered = 0;
	//	using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//	{
	//		@event.RemoteEventCounts += (sender, e) =>
	//		{
	//			triggered++;
	//		};
	//		@event.QueueEvents("test");
	//		Thread.Sleep(2000);
	//	}
	//	Thread.Sleep(2000);
	//	using (var cmd = Connection.CreateCommand())
	//	{
	//		cmd.CommandText = "execute block as begin post_event 'test'; end";
	//		cmd.ExecuteNonQuery();
	//		Thread.Sleep(2000);
	//	}
	//	Assert.AreEqual(0, triggered);
	//}

	//[Test]
	//public void NoExceptionWithDispose()
	//{
	//	var exception = (Exception)null;
	//	using (var @event = new IBRemoteEvent(Connection.ConnectionString))
	//	{
	//		@event.RemoteEventError += (sender, e) =>
	//		{
	//			exception = e.Error;
	//		};
	//		@event.QueueEvents("test");
	//		Thread.Sleep(2000);
	//	}
	//	Thread.Sleep(2000);
	//	Assert.IsNull(exception);
	//}
}
