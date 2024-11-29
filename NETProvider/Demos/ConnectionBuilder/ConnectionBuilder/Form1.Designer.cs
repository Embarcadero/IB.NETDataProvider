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

//$Authors = Embarcadero, Jeff Overcash

namespace ConnectionBuilder
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			txtConnectionString = new System.Windows.Forms.TextBox();
			btnBuild = new System.Windows.Forms.Button();
			btnTest = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			txtServer = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			txtPath = new System.Windows.Forms.TextBox();
			numPort = new System.Windows.Forms.NumericUpDown();
			label4 = new System.Windows.Forms.Label();
			txtUser = new System.Windows.Forms.TextBox();
			txtPassword = new System.Windows.Forms.TextBox();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			cboDialect = new System.Windows.Forms.ComboBox();
			label7 = new System.Windows.Forms.Label();
			cboPacket = new System.Windows.Forms.ComboBox();
			label8 = new System.Windows.Forms.Label();
			cboCharSet = new System.Windows.Forms.ComboBox();
			label9 = new System.Windows.Forms.Label();
			cboServerType = new System.Windows.Forms.ComboBox();
			chkSSL = new System.Windows.Forms.CheckBox();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			txtSEPPassword = new System.Windows.Forms.TextBox();
			label13 = new System.Windows.Forms.Label();
			label14 = new System.Windows.Forms.Label();
			label15 = new System.Windows.Forms.Label();
			txtServerPublicFile = new System.Windows.Forms.TextBox();
			txtServerPublicPath = new System.Windows.Forms.TextBox();
			txtClientCertFile = new System.Windows.Forms.TextBox();
			txtClientPassPhraseFile = new System.Windows.Forms.TextBox();
			txtClientPassPhrase = new System.Windows.Forms.TextBox();
			button1 = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
			SuspendLayout();
			// 
			// txtConnectionString
			// 
			txtConnectionString.Dock = System.Windows.Forms.DockStyle.Bottom;
			txtConnectionString.Location = new System.Drawing.Point(0, 377);
			txtConnectionString.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtConnectionString.Multiline = true;
			txtConnectionString.Name = "txtConnectionString";
			txtConnectionString.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtConnectionString.Size = new System.Drawing.Size(933, 89);
			txtConnectionString.TabIndex = 0;
			// 
			// btnBuild
			// 
			btnBuild.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			btnBuild.Location = new System.Drawing.Point(28, 343);
			btnBuild.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnBuild.Name = "btnBuild";
			btnBuild.Size = new System.Drawing.Size(88, 27);
			btnBuild.TabIndex = 1;
			btnBuild.Text = "Build String";
			btnBuild.UseVisualStyleBackColor = true;
			btnBuild.Click += btnBuild_Click;
			// 
			// btnTest
			// 
			btnTest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			btnTest.Location = new System.Drawing.Point(792, 343);
			btnTest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnTest.Name = "btnTest";
			btnTest.Size = new System.Drawing.Size(127, 27);
			btnTest.TabIndex = 2;
			btnTest.Text = "Test Connection";
			btnTest.UseVisualStyleBackColor = true;
			btnTest.Click += btnTest_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(37, 22);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(39, 15);
			label1.TabIndex = 3;
			label1.Text = "Server";
			// 
			// txtServer
			// 
			txtServer.Location = new System.Drawing.Point(89, 17);
			txtServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtServer.Name = "txtServer";
			txtServer.Size = new System.Drawing.Size(137, 23);
			txtServer.TabIndex = 4;
			txtServer.Text = "localhost";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(233, 22);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(82, 15);
			label2.TabIndex = 5;
			label2.Text = "Database Path";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(51, 59);
			label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(29, 15);
			label3.TabIndex = 6;
			label3.Text = "Port";
			// 
			// txtPath
			// 
			txtPath.Location = new System.Drawing.Point(331, 17);
			txtPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtPath.Name = "txtPath";
			txtPath.Size = new System.Drawing.Size(587, 23);
			txtPath.TabIndex = 7;
			// 
			// numPort
			// 
			numPort.Location = new System.Drawing.Point(89, 54);
			numPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			numPort.Maximum = new decimal(new int[] { 64000, 0, 0, 0 });
			numPort.Name = "numPort";
			numPort.Size = new System.Drawing.Size(134, 23);
			numPort.TabIndex = 8;
			numPort.Value = new decimal(new int[] { 3050, 0, 0, 0 });
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(271, 59);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(30, 15);
			label4.TabIndex = 9;
			label4.Text = "User";
			// 
			// txtUser
			// 
			txtUser.Location = new System.Drawing.Point(312, 54);
			txtUser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtUser.Name = "txtUser";
			txtUser.Size = new System.Drawing.Size(171, 23);
			txtUser.TabIndex = 10;
			txtUser.Text = "sysdba";
			// 
			// txtPassword
			// 
			txtPassword.Location = new System.Drawing.Point(645, 54);
			txtPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtPassword.Name = "txtPassword";
			txtPassword.Size = new System.Drawing.Size(135, 23);
			txtPassword.TabIndex = 11;
			txtPassword.Text = "masterkey";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(576, 59);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(57, 15);
			label5.TabIndex = 12;
			label5.Text = "Password";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(805, 59);
			label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(43, 15);
			label6.TabIndex = 13;
			label6.Text = "Dialect";
			// 
			// cboDialect
			// 
			cboDialect.FormattingEnabled = true;
			cboDialect.Items.AddRange(new object[] { "1", "2", "3" });
			cboDialect.Location = new System.Drawing.Point(859, 54);
			cboDialect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cboDialect.Name = "cboDialect";
			cboDialect.Size = new System.Drawing.Size(60, 23);
			cboDialect.TabIndex = 14;
			cboDialect.Text = "3";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(7, 102);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(65, 15);
			label7.TabIndex = 15;
			label7.Text = "Packet Size";
			// 
			// cboPacket
			// 
			cboPacket.FormattingEnabled = true;
			cboPacket.Items.AddRange(new object[] { "1024", "2048", "4096", "8192", "16284" });
			cboPacket.Location = new System.Drawing.Point(89, 98);
			cboPacket.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cboPacket.Name = "cboPacket";
			cboPacket.Size = new System.Drawing.Size(134, 23);
			cboPacket.TabIndex = 16;
			cboPacket.Text = "8192";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(246, 103);
			label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(51, 15);
			label8.TabIndex = 17;
			label8.Text = "Char Set";
			// 
			// cboCharSet
			// 
			cboCharSet.FormattingEnabled = true;
			cboCharSet.Items.AddRange(new object[] { "None", "Octets", "Ascii", "UnicodeFss", "Utf8", "ShiftJis0208", "EucJapanese0208", "Iso2022Japanese", "Dos437", "Dos850", "Dos865", "Dos860", "Dos863", "Iso8859_1", "Iso8859_2", "Ksc5601", "Dos861", "Windows1250", "Windows1251", "Windows1252", "Windows1253", "Windows1254", "Big5", "Gb2312", "Windows1255", "Windows1256", "Windows1257", "Koi8R", "Koi8U", "TIS620" });
			cboCharSet.Location = new System.Drawing.Point(313, 98);
			cboCharSet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cboCharSet.Name = "cboCharSet";
			cboCharSet.Size = new System.Drawing.Size(170, 23);
			cboCharSet.TabIndex = 18;
			cboCharSet.Text = "None";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(490, 103);
			label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(66, 15);
			label9.TabIndex = 19;
			label9.Text = "Server Type";
			// 
			// cboServerType
			// 
			cboServerType.FormattingEnabled = true;
			cboServerType.Items.AddRange(new object[] { "Default", "Embedded" });
			cboServerType.Location = new System.Drawing.Point(573, 99);
			cboServerType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cboServerType.Name = "cboServerType";
			cboServerType.Size = new System.Drawing.Size(140, 23);
			cboServerType.TabIndex = 20;
			cboServerType.Text = "Default";
			// 
			// chkSSL
			// 
			chkSSL.AutoSize = true;
			chkSSL.Location = new System.Drawing.Point(458, 155);
			chkSSL.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkSSL.Name = "chkSSL";
			chkSSL.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			chkSSL.Size = new System.Drawing.Size(44, 19);
			chkSSL.TabIndex = 21;
			chkSSL.Text = "SSL";
			chkSSL.UseVisualStyleBackColor = true;
			chkSSL.CheckedChanged += chkSSL_CheckedChanged;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(37, 212);
			label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(102, 15);
			label10.TabIndex = 22;
			label10.Text = "Server Public Path";
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(62, 242);
			label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(84, 15);
			label11.TabIndex = 23;
			label11.Text = "Client Cert File";
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(721, 103);
			label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(79, 15);
			label12.TabIndex = 24;
			label12.Text = "SEP Password";
			// 
			// txtSEPPassword
			// 
			txtSEPPassword.Location = new System.Drawing.Point(808, 98);
			txtSEPPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtSEPPassword.Name = "txtSEPPassword";
			txtSEPPassword.Size = new System.Drawing.Size(110, 23);
			txtSEPPassword.TabIndex = 25;
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(15, 273);
			label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(123, 15);
			label13.TabIndex = 26;
			label13.Text = "Client Pass Phrase File";
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(37, 302);
			label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(102, 15);
			label14.TabIndex = 27;
			label14.Text = "Client Pass Phrase";
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(44, 185);
			label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(96, 15);
			label15.TabIndex = 28;
			label15.Text = "Server Public File";
			// 
			// txtServerPublicFile
			// 
			txtServerPublicFile.Enabled = false;
			txtServerPublicFile.Location = new System.Drawing.Point(169, 181);
			txtServerPublicFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtServerPublicFile.Name = "txtServerPublicFile";
			txtServerPublicFile.Size = new System.Drawing.Size(750, 23);
			txtServerPublicFile.TabIndex = 29;
			// 
			// txtServerPublicPath
			// 
			txtServerPublicPath.Enabled = false;
			txtServerPublicPath.Location = new System.Drawing.Point(169, 209);
			txtServerPublicPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtServerPublicPath.Name = "txtServerPublicPath";
			txtServerPublicPath.Size = new System.Drawing.Size(750, 23);
			txtServerPublicPath.TabIndex = 30;
			// 
			// txtClientCertFile
			// 
			txtClientCertFile.Enabled = false;
			txtClientCertFile.Location = new System.Drawing.Point(169, 239);
			txtClientCertFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtClientCertFile.Name = "txtClientCertFile";
			txtClientCertFile.Size = new System.Drawing.Size(750, 23);
			txtClientCertFile.TabIndex = 31;
			// 
			// txtClientPassPhraseFile
			// 
			txtClientPassPhraseFile.Enabled = false;
			txtClientPassPhraseFile.Location = new System.Drawing.Point(169, 269);
			txtClientPassPhraseFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtClientPassPhraseFile.Name = "txtClientPassPhraseFile";
			txtClientPassPhraseFile.Size = new System.Drawing.Size(750, 23);
			txtClientPassPhraseFile.TabIndex = 32;
			// 
			// txtClientPassPhrase
			// 
			txtClientPassPhrase.Enabled = false;
			txtClientPassPhrase.Location = new System.Drawing.Point(169, 299);
			txtClientPassPhrase.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtClientPassPhrase.Name = "txtClientPassPhrase";
			txtClientPassPhrase.Size = new System.Drawing.Size(750, 23);
			txtClientPassPhrase.TabIndex = 33;
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(257, 347);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(106, 21);
			button1.TabIndex = 34;
			button1.Text = "Load from XML";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// button2
			// 
			button2.Location = new System.Drawing.Point(141, 346);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(96, 22);
			button2.TabIndex = 35;
			button2.Text = "Write To XML";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// frmMain
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(933, 466);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(txtClientPassPhrase);
			Controls.Add(txtClientPassPhraseFile);
			Controls.Add(txtClientCertFile);
			Controls.Add(txtServerPublicPath);
			Controls.Add(txtServerPublicFile);
			Controls.Add(label15);
			Controls.Add(label14);
			Controls.Add(label13);
			Controls.Add(txtSEPPassword);
			Controls.Add(label12);
			Controls.Add(label11);
			Controls.Add(label10);
			Controls.Add(chkSSL);
			Controls.Add(cboServerType);
			Controls.Add(label9);
			Controls.Add(cboCharSet);
			Controls.Add(label8);
			Controls.Add(cboPacket);
			Controls.Add(label7);
			Controls.Add(cboDialect);
			Controls.Add(label6);
			Controls.Add(label5);
			Controls.Add(txtPassword);
			Controls.Add(txtUser);
			Controls.Add(label4);
			Controls.Add(numPort);
			Controls.Add(txtPath);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(txtServer);
			Controls.Add(label1);
			Controls.Add(btnTest);
			Controls.Add(btnBuild);
			Controls.Add(txtConnectionString);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "frmMain";
			Text = "IB Connection Builder";
			((System.ComponentModel.ISupportInitialize)numPort).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboDialect;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboPacket;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cboCharSet;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboServerType;
        private System.Windows.Forms.CheckBox chkSSL;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtSEPPassword;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtServerPublicFile;
        private System.Windows.Forms.TextBox txtServerPublicPath;
        private System.Windows.Forms.TextBox txtClientCertFile;
        private System.Windows.Forms.TextBox txtClientPassPhraseFile;
        private System.Windows.Forms.TextBox txtClientPassPhrase;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}

