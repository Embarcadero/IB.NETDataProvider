/*
 *  Visual Studio DDEX Provider for FirebirdClient
 * 
 *     The contents of this file are subject to the Initial 
 *     Developer's Public License Version 1.0 (the "License"); 
 *     you may not use this file except in compliance with the 
 *     License. You may obtain a copy of the License at 
 *     http://www.firebirdsql.org/index.php?op=doc&id=idpl
 *
 *     Software distributed under the License is distributed on 
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either 
 *     express or implied.  See the License for the specific 
 *     language governing rights and limitations under the License.
 * 
 *  Copyright (c) 2005 Carlos Guzman Alvarez
 *  Portions created by Embarcadero are Copyright (C) Embarcadero.
 *
 *  All Rights Reserved.
 *   
 *  Contributors:
 *    Jiri Cincura (jiri@cincura.net)
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data;
using System.Globalization;

namespace InterBaseSql.VisualStudio.DataTools
{
    public partial class IBDataConnectionUIControl : DataConnectionUIControl
    {
        #region · Static Methods ·

        // This	is somethig	that should	be needed in .NET 2.0
        // for use with	the	DbConnectionOptions	or DbConnectionString classes.
        private static Hashtable GetSynonyms()
        {
            Hashtable synonyms = new Hashtable(StringComparer.Create(CultureInfo.InvariantCulture, true))
            {
                { "data source", "data source" },
                { "datasource", "data source" },
                { "server", "data source" },
                { "host", "data source" },
                { "port", "port number" },
                { "port number", "port number" },
                { "database", "initial catalog" },
                { "initial catalog", "initial catalog" },
                { "user id", "user id" },
                { "userid", "user id" },
                { "uid", "user id" },
                { "user", "user id" },
                { "user name", "user id" },
                { "username", "user id" },
                { "password", "password" },
                { "user password", "password" },
                { "userpassword", "password" },
                { "dialect", "dialect" },
                { "pooling", "pooling" },
                { "max pool size", "max pool size" },
                { "maxpoolsize", "max pool size" },
                { "min pool size", "min pool size" },
                { "minpoolsize", "min pool size" },
                { "character set", "character set" },
                { "charset", "character set" },
                { "connection lifetime", "connection lifetime" },
                { "connectionlifetime", "connection lifetime" },
                { "timeout", "connection timeout" },
                { "connection timeout", "connection timeout" },
                { "connectiontimeout", "connection timeout" },
                { "packet size", "packet size" },
                { "packetsize", "packet size" },
                { "role", "role name" },
                { "role name", "role name" },
                { "fetch size", "fetch size" },
                { "fetchsize", "fetch size" },
                { "server type", "server type" },
                { "servertype", "server type" },
                { "isolation level", "isolation level" },
                { "isolationlevel", "isolation level" },
                { "records affected", "records affected" },
                { "context connection", "context connection" }
            };

            return synonyms;
        }

        #endregion

        #region · Constructors ·

        public IBDataConnectionUIControl()
        {
            TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(myWriter);
            System.Diagnostics.Trace.WriteLine("IBDataConnectionUIControl()");
            InitializeComponent();
        }

        #endregion

        #region · Methods ·

        public override void LoadProperties()
        {
            System.Diagnostics.Trace.WriteLine("IBDataConnectionUIControl::LoadProperties()");

            try
            {
                this.txtDataSource.Text = (string)ConnectionProperties["Data Source"];
                this.txtUserName.Text   = (string)ConnectionProperties["User ID"];
                this.txtDatabase.Text   = (string)ConnectionProperties["Initial Catalog"];
                this.txtPassword.Text   = (string)ConnectionProperties["Password"];
                this.txtRole.Text       = (string)ConnectionProperties["Role"];
                this.cboCharset.Text    = (string)ConnectionProperties["Character Set"];
                if (this.ConnectionProperties.Contains("Port Number"))
                {
                    this.txtPort.Text = ConnectionProperties["Port Number"].ToString();
                }

                if (this.ConnectionProperties.Contains("Dialect"))
                {
                    if (Convert.ToInt32(ConnectionProperties["Dialect"]) == 1)
                    {
                        this.cboDialect.SelectedIndex = 0;
                    }
                    else
                    {
                        this.cboDialect.SelectedIndex = 1;
                    }
                }

                if (this.ConnectionProperties.Contains("Server Type"))
                {
                    if (Convert.ToInt32(ConnectionProperties["Server Type"]) == 0)
                    {
                        this.cboServerType.SelectedIndex = 0;
                    }
                    else
                    {
                        this.cboServerType.SelectedIndex = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region · Private Methods ·

        private void SetProperty(string propertyName, object value)
        {
            this.ConnectionProperties[propertyName] = value;
        }

        #endregion

        #region · Event Handlers ·

        private void SetProperty(object sender, EventArgs e)
        {
            if (sender.Equals(this.txtDataSource))
            {
                this.SetProperty("Data Source", this.txtDataSource.Text);
            } 
            else if (sender.Equals(this.txtDatabase))
            {
                this.SetProperty("Initial Catalog", this.txtDatabase.Text);
            }
            else if (sender.Equals(this.txtUserName))
            {
                this.SetProperty("User ID", this.txtUserName.Text);
            }
            else if (sender.Equals(this.txtPassword))
            {
                this.SetProperty("Password", this.txtPassword.Text);
            }
            else if (sender.Equals(this.txtRole))
            {
                this.SetProperty("Role", this.txtRole.Text);
            }
            else if (sender.Equals(this.txtPort))
            {
                if (!String.IsNullOrEmpty(this.txtPort.Text))
                {
                    this.SetProperty("Port Number", Convert.ToInt32(this.txtPort.Text));
                }
            }
            else if (sender.Equals(this.cboCharset))
            {
                this.SetProperty("Character Set", this.cboCharset.Text);
            }
            else if (sender.Equals(this.cboDialect))
            {
                if (!String.IsNullOrEmpty(this.cboDialect.Text))
                {
                    this.SetProperty("Dialect", Convert.ToInt32(this.cboDialect.Text));
                }
            }
            else if (sender.Equals(this.cboServerType))
            {
                if (this.cboServerType.SelectedIndex != -1)
                {
                    this.SetProperty("Server Type", Convert.ToInt32(this.cboServerType.SelectedIndex));
                }
            }
        }

        private void CmdGetFile_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtDatabase.Text = this.openFileDialog.FileName;
            }
        }

        #endregion
    }
}
