/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.ComponentModel;

namespace InterBaseSql.Data.InterBaseClient
{
	public class IBConnectionStringBuilder : DbConnectionStringBuilder
	{
		#region Properties

		[Category("Security")]
		[DisplayName("User ID")]
		[Description("Indicates the User ID to be used when connecting to the data source.")]
		[DefaultValue(Common.ConnectionString.DefaultValueUserId)]
		public string UserID
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyUserId), base.TryGetValue, Common.ConnectionString.DefaultValueUserId); }
			set { SetValue(Common.ConnectionString.DefaultKeyUserId, value); }
		}

		[Category("Security")]
		[DisplayName("Password")]
		[Description("Indicates the password to be used when connecting to the data source.")]
		[PasswordPropertyText(true)]
		[DefaultValue(Common.ConnectionString.DefaultValuePassword)]
		public string Password
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyPassword), base.TryGetValue, Common.ConnectionString.DefaultValuePassword); }
			set { SetValue(Common.ConnectionString.DefaultKeyPassword, value); }
		}

		[Category("Source")]
		[DisplayName("DataSource")]
		[Description("The name of the InterBase server to which to connect.")]
		[DefaultValue(Common.ConnectionString.DefaultValueDataSource)]
		public string DataSource
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyDataSource), base.TryGetValue, Common.ConnectionString.DefaultValueDataSource); }
			set { SetValue(Common.ConnectionString.DefaultKeyDataSource, value); }
		}

		[Category("Source")]
		[DisplayName("Database")]
		[Description("The name of the actual database or the database to be used when a connection is open. It is normally the path to an .IB file or an alias.")]
		[DefaultValue(Common.ConnectionString.DefaultValueCatalog)]
		public string Database
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyCatalog), base.TryGetValue, Common.ConnectionString.DefaultValueCatalog); }
			set { SetValue(Common.ConnectionString.DefaultKeyCatalog, value); }
		}

		[Category("Source")]
		[DisplayName("Port")]
		[Description("Port to use for TCP/IP connections")]
		[DefaultValue(Common.ConnectionString.DefaultValuePortNumber)]
		public int Port
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyPortNumber), base.TryGetValue, Common.ConnectionString.DefaultValuePortNumber); }
			set { SetValue(Common.ConnectionString.DefaultKeyPortNumber, value); }
		}

		[Category("Advanced")]
		[DisplayName("PacketSize")]
		[Description("The size (in bytes) of network packets. PacketSize may be in the range 512-32767 bytes.")]
		[DefaultValue(Common.ConnectionString.DefaultValuePacketSize)]
		public int PacketSize
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyPacketSize), base.TryGetValue, Common.ConnectionString.DefaultValuePacketSize); }
			set { SetValue(Common.ConnectionString.DefaultKeyPacketSize, value); }
		}

		[Category("Security")]
		[DisplayName("Role")]
		[Description("The user role.")]
		[DefaultValue(Common.ConnectionString.DefaultValueRoleName)]
		public string Role
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyRoleName), base.TryGetValue, Common.ConnectionString.DefaultValueRoleName); }
			set { SetValue(Common.ConnectionString.DefaultKeyRoleName, value); }
		}

		[Category("Advanced")]
		[DisplayName("Dialect")]
		[Description("The database SQL dialect.")]
		[DefaultValue(Common.ConnectionString.DefaultValueDialect)]
		public int Dialect
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyDialect), base.TryGetValue, Common.ConnectionString.DefaultValueDialect); }
			set { SetValue(Common.ConnectionString.DefaultKeyDialect, value); }
		}

		[Category("Advanced")]
		[DisplayName("Character Set")]
		[Description("The connection character set encoding.")]
		[DefaultValue(Common.ConnectionString.DefaultValueCharacterSet)]
		public string Charset
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyCharacterSet), base.TryGetValue, Common.ConnectionString.DefaultValueCharacterSet); }
			set { SetValue(Common.ConnectionString.DefaultKeyCharacterSet, value); }
		}

		[Category("Connection")]
		[DisplayName("Connection Timeout")]
		[Description("The time (in seconds) to wait for a connection to open.")]
		[DefaultValue(Common.ConnectionString.DefaultValueConnectionTimeout)]
		public int ConnectionTimeout
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyConnectionTimeout), base.TryGetValue, Common.ConnectionString.DefaultValueConnectionTimeout); }
			set { SetValue(Common.ConnectionString.DefaultKeyConnectionTimeout, value); }
		}

		[Category("Pooling")]
		[DisplayName("Pooling")]
		[Description("When true the connection is grabbed from a pool or, if necessary, created and added to the appropriate pool.")]
		[DefaultValue(Common.ConnectionString.DefaultValuePooling)]
		public bool Pooling
		{
			get { return Common.ConnectionString.GetBoolean(GetKey(Common.ConnectionString.DefaultKeyPooling), base.TryGetValue, Common.ConnectionString.DefaultValuePooling); }
			set { SetValue(Common.ConnectionString.DefaultKeyPooling, value); }
		}

		[Category("Connection")]
		[DisplayName("Connection LifeTime")]
		[Description("When a connection is returned to the pool, its creation time is compared with the current time, and the connection is destroyed if that time span (in seconds) exceeds the value specified by connection lifetime.")]
		[DefaultValue(Common.ConnectionString.DefaultValueConnectionLifetime)]
		public int ConnectionLifeTime
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyConnectionLifetime), base.TryGetValue, Common.ConnectionString.DefaultValueConnectionLifetime); }
			set { SetValue(Common.ConnectionString.DefaultKeyConnectionLifetime, value); }
		}

		[Category("Pooling")]
		[DisplayName("MinPoolSize")]
		[Description("The minimum number of connections allowed in the pool.")]
		[DefaultValue(Common.ConnectionString.DefaultValueMinPoolSize)]
		public int MinPoolSize
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyMinPoolSize), base.TryGetValue, Common.ConnectionString.DefaultValueMinPoolSize); }
			set { SetValue(Common.ConnectionString.DefaultKeyMinPoolSize, value); }
		}

		[Category("Pooling")]
		[DisplayName("MaxPoolSize")]
		[Description("The maximum number of connections allowed in the pool.")]
		[DefaultValue(Common.ConnectionString.DefaultValueMaxPoolSize)]
		public int MaxPoolSize
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyMaxPoolSize), base.TryGetValue, Common.ConnectionString.DefaultValueMaxPoolSize); }
			set { SetValue(Common.ConnectionString.DefaultKeyMaxPoolSize, value); }
		}

		[Category("Advanced")]
		[DisplayName("FetchSize")]
		[Description("The maximum number of rows to be fetched in a single call to read into the internal row buffer.")]
		[DefaultValue(Common.ConnectionString.DefaultValueFetchSize)]
		public int FetchSize
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyFetchSize), base.TryGetValue, Common.ConnectionString.DefaultValueFetchSize); }
			set { SetValue(Common.ConnectionString.DefaultKeyFetchSize, value); }
		}

		[Category("Source")]
		[DisplayName("ServerType")]
		[Description("The type of server used.")]
		[DefaultValue(Common.ConnectionString.DefaultValueServerType)]
		public IBServerType ServerType
		{
			get { return GetServerType(Common.ConnectionString.DefaultKeyServerType, Common.ConnectionString.DefaultValueServerType); }
			set { SetValue(Common.ConnectionString.DefaultKeyServerType, value); }
		}

		[Category("Advanced")]
		[DisplayName("IsolationLevel")]
		[Description("The default Isolation Level for implicit transactions.")]
		[DefaultValue(Common.ConnectionString.DefaultValueIsolationLevel)]
		public IsolationLevel IsolationLevel
		{
			get { return GetIsolationLevel(Common.ConnectionString.DefaultKeyIsolationLevel, Common.ConnectionString.DefaultValueIsolationLevel); }
			set { SetValue(Common.ConnectionString.DefaultKeyIsolationLevel, value); }
		}

		[Category("Advanced")]
		[DisplayName("Records Affected")]
		[Description("Get the number of rows affected by a command when true.")]
		[DefaultValue(Common.ConnectionString.DefaultValueRecordsAffected)]
		public bool ReturnRecordsAffected
		{
			get { return Common.ConnectionString.GetBoolean(GetKey(Common.ConnectionString.DefaultKeyRecordsAffected), base.TryGetValue, Common.ConnectionString.DefaultValueRecordsAffected); }
			set { SetValue(Common.ConnectionString.DefaultKeyRecordsAffected, value); }
		}

		[Category("Pooling")]
		[DisplayName("Enlist")]
		[Description("If true, enlists the connections in the current transaction.")]
		[DefaultValue(Common.ConnectionString.DefaultValuePooling)]
		public bool Enlist
		{
			get { return Common.ConnectionString.GetBoolean(GetKey(Common.ConnectionString.DefaultKeyEnlist), base.TryGetValue, Common.ConnectionString.DefaultValueEnlist); }
			set { SetValue(Common.ConnectionString.DefaultKeyEnlist, value); }
		}

		[Category("Advanced")]
		[DisplayName("DB Cache Pages")]
		[Description("How many cache buffers to use for this session.")]
		[DefaultValue(Common.ConnectionString.DefaultValueDbCachePages)]
		public int DbCachePages
		{
			get { return Common.ConnectionString.GetInt32(GetKey(Common.ConnectionString.DefaultKeyDbCachePages), base.TryGetValue, Common.ConnectionString.DefaultValueDbCachePages); }
			set { SetValue(Common.ConnectionString.DefaultKeyDbCachePages, value); }
		}

		[Category("Advanced")]
		[DisplayName("EUA Enabled DB")]
		[Description("Set true when EUA is enabled.")]
		[DefaultValue(Common.ConnectionString.DefaultValueEUAEnabled)]
		public bool EUAEnabled
		{
			get { return Common.ConnectionString.GetBoolean(GetKey(Common.ConnectionString.DefaultKeyEUAEnabled), base.TryGetValue, Common.ConnectionString.DefaultValueEUAEnabled); }
			set { SetValue(Common.ConnectionString.DefaultKeyEUAEnabled, value); }
		}

		[Category("Advanced")]
		[DisplayName("Instance Name")]
		[Description("Instance alias name.  Can be used instead of a port number.")]
		[DefaultValue(Common.ConnectionString.DefaultValueDbCachePages)]
		public string InstanceName
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyInstanceName), base.TryGetValue, Common.ConnectionString.DefaultValueInstanceName); }
			set { SetValue(Common.ConnectionString.DefaultKeyInstanceName, value); }
		}

		[Category("Security")]
		[DisplayName("SEP Password")]
		[Description("SEP Password to use on connections.")]
		[DefaultValue(Common.ConnectionString.DefaultValueSEPPassword)]
		public string SEPPassword
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeySEPPassword), base.TryGetValue, Common.ConnectionString.DefaultValueSEPPassword); }
			set { SetValue(Common.ConnectionString.DefaultKeySEPPassword, value); }
		}

		[Category("Security")]
		[DisplayName("SSL")]
		[Description("Set true to make SSL connections.")]
		[DefaultValue(Common.ConnectionString.DefaultValueSSL)]
		public bool SSL
		{
			get { return Common.ConnectionString.GetBoolean(GetKey(Common.ConnectionString.DefaultKeySSL), base.TryGetValue, Common.ConnectionString.DefaultValueSSL); }
			set { SetValue(Common.ConnectionString.DefaultKeySSL, value); }
		}

		[Category("Security")]
		[DisplayName("Server Public File")]
		[Description("Server Public File to use on SSL connections.")]
		[DefaultValue(Common.ConnectionString.DefaultValueServerPublicFile)]
		public string ServerPublicFile
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyServerPublicFile), base.TryGetValue, Common.ConnectionString.DefaultValueServerPublicFile); }
			set { SetValue(Common.ConnectionString.DefaultKeyServerPublicFile, value); }
		}

		[Category("Security")]
		[DisplayName("Server Public Path")]
		[Description("Server Public Path to use on SSL connections.")]
		[DefaultValue(Common.ConnectionString.DefaultValueServerPublicPath)]
		public string ServerPublicPath
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyServerPublicPath), base.TryGetValue, Common.ConnectionString.DefaultValueServerPublicPath); }
			set { SetValue(Common.ConnectionString.DefaultKeyServerPublicPath, value); }
		}

		[Category("Security")]
		[DisplayName("Client Certificate File")]
		[Description("Client Certificate File to use on SSL connections.")]
		[DefaultValue(Common.ConnectionString.DefaultValueClientCertFile)]
		public string ClientCertFile
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyClientCertFile), base.TryGetValue, Common.ConnectionString.DefaultValueClientCertFile); }
			set { SetValue(Common.ConnectionString.DefaultKeyClientCertFile, value); }
		}

		[Category("Security")]
		[DisplayName("Client Pass Phrase File")]
		[Description("Client Pass Phrase File to use on SSL connections.")]
		[DefaultValue(Common.ConnectionString.DefaultValueClientCertFile)]
		public string ClientPassPhraseFile
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyClientPassPhraseFile), base.TryGetValue, Common.ConnectionString.DefaultValueClientPassPhraseFile); }
			set { SetValue(Common.ConnectionString.DefaultKeyClientPassPhraseFile, value); }
		}

		[Category("Security")]
		[DisplayName("Client Pass Phrase")]
		[Description("Client Pass Phrase to use on SSL connections.")]
		[DefaultValue(Common.ConnectionString.DefaultValueClientCertFile)]
		public string ClientPassPhrase
		{
			get { return Common.ConnectionString.GetString(GetKey(Common.ConnectionString.DefaultKeyClientPassPhrase), base.TryGetValue, Common.ConnectionString.DefaultValueClientPassPhrase); }
			set { SetValue(Common.ConnectionString.DefaultKeyClientPassPhrase, value); }
		}

		#endregion

		#region Constructors

		public IBConnectionStringBuilder()
		{ }

		public IBConnectionStringBuilder(string connectionString)
			: this()
		{
			ConnectionString = connectionString;
		}

		#endregion

		#region Private methods

		private IBServerType GetServerType(string keyword, IBServerType defaultValue)
		{
			var key = GetKey(keyword);
			if (!TryGetValue(key, out var value))
				return defaultValue;
			switch (value)
			{
				case IBServerType IBServerType:
					return IBServerType;
				case string s when Enum.TryParse<IBServerType>(s, true, out var enumResult):
					return enumResult;
				default:
					return Common.ConnectionString.GetServerType(key, base.TryGetValue, defaultValue);
			}
		}

		private IsolationLevel GetIsolationLevel(string keyword, IsolationLevel defaultValue)
		{
			var key = GetKey(keyword);
			if (!TryGetValue(key, out var value))
				return defaultValue;
			switch (value)
			{
				case IsolationLevel isolationLevel:
					return isolationLevel;
				case string s when Enum.TryParse<IsolationLevel>(s, true, out var enumResult):
					return enumResult;
				default:
					return Common.ConnectionString.GetIsolationLevel(key, base.TryGetValue, defaultValue);
			}
		}

		private byte[] GetBytes(string keyword, byte[] defaultValue)
		{
			var key = GetKey(keyword);
			if (!TryGetValue(key, out var value))
				return defaultValue;
			switch (value)
			{
				case byte[] bytes:
					return bytes;
				case string s:
					return Convert.FromBase64String(s);
				default:
					return defaultValue;
			}
		}

		private void SetValue<T>(string keyword, T value)
		{
			var key = GetKey(keyword);
			if (value is byte[] bytes)
			{
				this[key] = Convert.ToBase64String(bytes);
			}
			else
			{
				this[key] = value;
			}
		}

		private string GetKey(string keyword)
		{
			var synonymKey = Common.ConnectionString.Synonyms[keyword];
			foreach (string key in Keys)
			{
				if (Common.ConnectionString.Synonyms.ContainsKey(key) && Common.ConnectionString.Synonyms[key] == synonymKey)
				{
					synonymKey = key;
					break;
				}
			}
			return synonymKey;
		}

		#endregion
	}
}
