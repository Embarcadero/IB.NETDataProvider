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

using InterBaseSql.Data.InterBaseClient;
using System.IO;
using InterBaseSql.Data.Common;

namespace EventDemo
{
    public partial class MainForm : Form
    {
        private IBConnection _connection;
        private IBEvents _events;

        public MainForm()
        {
            InitializeComponent();
            _connection = new IBConnection();
            _events = new IBEvents();
            _events.Connection = _connection;
            _events.EventAlert += EventFired;
        }

        private void btnOpenDatabase_Click(object sender, EventArgs e)
        {
            CreateAndConnectDatabase();
            btnOpenDatabase.Enabled = false;
            btnClearEventsReceived.Enabled = true;
            btnCloseDatabase.Enabled = true;
            btnGenerateEvent.Enabled = true;
            btnRegisterEvents.Enabled = true;
            grpEvents.Enabled = true;
            grpReceived.Enabled = true;
            grpRegistration.Enabled = true;
        }

        private static IBConnectionStringBuilder BuildConnectionStringBuilder()
        {
            var builder = new IBConnectionStringBuilder();
            builder.UserID = "SYSDBA";
            builder.Password = "masterkey";
            builder.DataSource = "localhost";
            builder.Database = AppDomain.CurrentDomain.BaseDirectory + "EventsDemo.ib";
            builder.Port = 3050;
            return builder;
        }

		private void CreateAndConnectDatabase()
		{
			var cs = BuildConnectionStringBuilder().ToString();
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "EventsDemo.ib"))
            {
                IBConnection.CreateDatabase(cs, 8192, true, true);
                ConnectToDB();
                CreateSP();
            }
            else
            {
                ConnectToDB();
            }

			void ConnectToDB()

			{
				_connection.ConnectionString = cs;
				_connection.Open();
			}

            void CreateSP()
            {
                using (var command = new IBCommand(@"CREATE PROCEDURE EVENTDEMO 
    (
        EVENT VARCHAR(40)
    )
    AS
    begin
        post_event :event;
    end", _connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void btnCloseDatabase_Click(object sender, EventArgs e)
        {
            if (_events.Registered)
                _events.UnRegisterEvents();
            _connection!.Close();
            btnOpenDatabase.Enabled = true;
            btnClearEventsReceived.Enabled = false;
            btnCloseDatabase.Enabled = false;
            btnGenerateEvent.Enabled = false;
            btnRegisterEvents.Enabled = false;
            grpEvents.Enabled = false;
            grpReceived.Enabled = false;
            grpRegistration.Enabled = false;
        }

        private void EventFired(object sender, EventAlertArgs e)
        {
            if (e.Count == 1)
                lbReceived.Items.Add(e.EventName);
            else
                lbReceived.Items.Add(e.EventName + "(" + e.Count.ToString() + ")");
        }

        private void btnRegisterEvents_Click(object sender, EventArgs e)
        {
            _events.UnRegisterEvents();
            try
            {
                _events.Events = new List<string>(moRegister.Text.Split(Environment.NewLine));
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            _events.RegisterEvents();
        }

        private void btnGenerateEvent_Click(object sender, EventArgs e)
        {
            var command = new IBCommand("execute procedure eventdemo(?)", _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add("@Event", IBDbType.VarChar).Direction = System.Data.ParameterDirection.Input;
            command.Parameters[0].Value = edEvent.Text;
            command.ExecuteNonQuery();
        }

        private void btnClearEventsReceived_Click(object sender, EventArgs e)
        {
            lbReceived.Items.Clear();
        }
    }
}