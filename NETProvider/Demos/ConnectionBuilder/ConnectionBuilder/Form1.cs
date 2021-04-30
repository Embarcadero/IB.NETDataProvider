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

//$Authors = Embarcadero, Jeff Overcash

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InterBaseSql.Data.InterBaseClient;

namespace ConnectionBuilder
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            var cs = new IBConnectionStringBuilder();
            if (txtServer.Text != "")
                cs.DataSource = txtServer.Text;
            cs.Database = txtPath.Text;
            if (numPort.Value != 3050)
                cs.Port = (int) numPort.Value;
            cs.UserID = txtUser.Text;
            cs.Password = txtPassword.Text;
            if (cboDialect.Text != "3")
                cs.Dialect = Int32.Parse(cboDialect.Text);
            if (cboPacket.Text != "8192")
                cs.PacketSize = Int32.Parse(cboPacket.Text);
            if (cboCharSet.Text != "None")
                cs.Charset = cboCharSet.Text;
            if (cboServerType.Text != "Default")
                cs.ServerType = IBServerType.Embedded;
            if (txtSEPPassword.Text != "")
                cs.SEPPassword = txtSEPPassword.Text;
            if (chkSSL.Checked)
            {
                cs.SSL = true;
                if (txtServerPublicFile.Text != "")
                    cs.ServerPublicFile = txtServerPublicFile.Text;
                if (txtServerPublicPath.Text != "")
                    cs.ServerPublicPath = txtServerPublicPath.Text;
                if (txtClientCertFile.Text != "")
                    cs.ClientCertFile = txtClientCertFile.Text;
                if (txtClientPassPhraseFile.Text != "")
                    cs.ClientPassPhraseFile = txtClientPassPhraseFile.Text;
                if (txtClientPassPhrase.Text != "")
                    cs.ClientPassPhrase = txtClientPassPhrase.Text;
            }

            txtConnectionString.Text = cs.ConnectionString;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var conn = new IBConnection(txtConnectionString.Text);
            try
            {
                conn.Open();
                MessageBox.Show("Successful connection!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failure {ex.Message}", "Failed");
            }

        }

        private void chkSSL_CheckedChanged(object sender, EventArgs e)
        {
            txtServerPublicFile.Enabled = ((CheckBox)sender).Checked;
            txtServerPublicPath.Enabled = ((CheckBox)sender).Checked;
            txtClientCertFile.Enabled = ((CheckBox)sender).Checked;
            txtClientPassPhraseFile.Enabled = ((CheckBox)sender).Checked;
            txtClientPassPhrase.Enabled = ((CheckBox)sender).Checked;
        }
    }
}
