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
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.btnBuild = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboDialect = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboPacket = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cboCharSet = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cboServerType = new System.Windows.Forms.ComboBox();
            this.chkSSL = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSEPPassword = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtServerPublicFile = new System.Windows.Forms.TextBox();
            this.txtServerPublicPath = new System.Windows.Forms.TextBox();
            this.txtClientCertFile = new System.Windows.Forms.TextBox();
            this.txtClientPassPhraseFile = new System.Windows.Forms.TextBox();
            this.txtClientPassPhrase = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtConnectionString.Location = new System.Drawing.Point(0, 326);
            this.txtConnectionString.Multiline = true;
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConnectionString.Size = new System.Drawing.Size(800, 78);
            this.txtConnectionString.TabIndex = 0;
            // 
            // btnBuild
            // 
            this.btnBuild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBuild.Location = new System.Drawing.Point(24, 297);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(75, 23);
            this.btnBuild.TabIndex = 1;
            this.btnBuild.Text = "Build String";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest.Location = new System.Drawing.Point(679, 297);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(109, 23);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test Connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(76, 15);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(118, 20);
            this.txtServer.TabIndex = 4;
            this.txtServer.Text = "localhost";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Database Path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Port";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(284, 15);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(504, 20);
            this.txtPath.TabIndex = 7;
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(76, 47);
            this.numPort.Maximum = new decimal(new int[] {
            64000,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(115, 20);
            this.numPort.TabIndex = 8;
            this.numPort.Value = new decimal(new int[] {
            3050,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(232, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "User";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(267, 47);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(147, 20);
            this.txtUser.TabIndex = 10;
            this.txtUser.Text = "sysdba";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(553, 47);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(116, 20);
            this.txtPassword.TabIndex = 11;
            this.txtPassword.Text = "masterkey";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(494, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(690, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Dialect";
            // 
            // cboDialect
            // 
            this.cboDialect.FormattingEnabled = true;
            this.cboDialect.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cboDialect.Location = new System.Drawing.Point(736, 47);
            this.cboDialect.Name = "cboDialect";
            this.cboDialect.Size = new System.Drawing.Size(52, 21);
            this.cboDialect.TabIndex = 14;
            this.cboDialect.Text = "3";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Packet Size";
            // 
            // cboPacket
            // 
            this.cboPacket.FormattingEnabled = true;
            this.cboPacket.Items.AddRange(new object[] {
            "1024",
            "2048",
            "4096",
            "8192",
            "16284"});
            this.cboPacket.Location = new System.Drawing.Point(76, 85);
            this.cboPacket.Name = "cboPacket";
            this.cboPacket.Size = new System.Drawing.Size(115, 21);
            this.cboPacket.TabIndex = 16;
            this.cboPacket.Text = "8192";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(211, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Char Set";
            // 
            // cboCharSet
            // 
            this.cboCharSet.FormattingEnabled = true;
            this.cboCharSet.Items.AddRange(new object[] {
            "None",
            "Octets",
            "Ascii",
            "UnicodeFss",
            "Utf8",
            "ShiftJis0208",
            "EucJapanese0208",
            "Iso2022Japanese",
            "Dos437",
            "Dos850",
            "Dos865",
            "Dos860",
            "Dos863",
            "Iso8859_1",
            "Iso8859_2",
            "Ksc5601",
            "Dos861",
            "Windows1250",
            "Windows1251",
            "Windows1252",
            "Windows1253",
            "Windows1254",
            "Big5",
            "Gb2312",
            "Windows1255",
            "Windows1256",
            "Windows1257",
            "Koi8R",
            "Koi8U",
            "TIS620"});
            this.cboCharSet.Location = new System.Drawing.Point(268, 85);
            this.cboCharSet.Name = "cboCharSet";
            this.cboCharSet.Size = new System.Drawing.Size(146, 21);
            this.cboCharSet.TabIndex = 18;
            this.cboCharSet.Text = "None";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(420, 89);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Server Type";
            // 
            // cboServerType
            // 
            this.cboServerType.FormattingEnabled = true;
            this.cboServerType.Items.AddRange(new object[] {
            "Default",
            "Embedded"});
            this.cboServerType.Location = new System.Drawing.Point(491, 86);
            this.cboServerType.Name = "cboServerType";
            this.cboServerType.Size = new System.Drawing.Size(121, 21);
            this.cboServerType.TabIndex = 20;
            this.cboServerType.Text = "Default";
            // 
            // chkSSL
            // 
            this.chkSSL.AutoSize = true;
            this.chkSSL.Location = new System.Drawing.Point(393, 134);
            this.chkSSL.Name = "chkSSL";
            this.chkSSL.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSSL.Size = new System.Drawing.Size(46, 17);
            this.chkSSL.TabIndex = 21;
            this.chkSSL.Text = "SSL";
            this.chkSSL.UseVisualStyleBackColor = true;
            this.chkSSL.CheckedChanged += new System.EventHandler(this.chkSSL_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(32, 184);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Server Public Path";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(53, 210);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(74, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Client Cert File";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(618, 89);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "SEP Password";
            // 
            // txtSEPPassword
            // 
            this.txtSEPPassword.Location = new System.Drawing.Point(693, 85);
            this.txtSEPPassword.Name = "txtSEPPassword";
            this.txtSEPPassword.Size = new System.Drawing.Size(95, 20);
            this.txtSEPPassword.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 237);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(114, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Client Pass Phrase File";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(32, 262);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(95, 13);
            this.label14.TabIndex = 27;
            this.label14.Text = "Client Pass Phrase";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(38, 160);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(89, 13);
            this.label15.TabIndex = 28;
            this.label15.Text = "Server Public File";
            // 
            // txtServerPublicFile
            // 
            this.txtServerPublicFile.Enabled = false;
            this.txtServerPublicFile.Location = new System.Drawing.Point(145, 157);
            this.txtServerPublicFile.Name = "txtServerPublicFile";
            this.txtServerPublicFile.Size = new System.Drawing.Size(643, 20);
            this.txtServerPublicFile.TabIndex = 29;
            // 
            // txtServerPublicPath
            // 
            this.txtServerPublicPath.Enabled = false;
            this.txtServerPublicPath.Location = new System.Drawing.Point(145, 181);
            this.txtServerPublicPath.Name = "txtServerPublicPath";
            this.txtServerPublicPath.Size = new System.Drawing.Size(643, 20);
            this.txtServerPublicPath.TabIndex = 30;
            // 
            // txtClientCertFile
            // 
            this.txtClientCertFile.Enabled = false;
            this.txtClientCertFile.Location = new System.Drawing.Point(145, 207);
            this.txtClientCertFile.Name = "txtClientCertFile";
            this.txtClientCertFile.Size = new System.Drawing.Size(643, 20);
            this.txtClientCertFile.TabIndex = 31;
            // 
            // txtClientPassPhraseFile
            // 
            this.txtClientPassPhraseFile.Enabled = false;
            this.txtClientPassPhraseFile.Location = new System.Drawing.Point(145, 233);
            this.txtClientPassPhraseFile.Name = "txtClientPassPhraseFile";
            this.txtClientPassPhraseFile.Size = new System.Drawing.Size(643, 20);
            this.txtClientPassPhraseFile.TabIndex = 32;
            // 
            // txtClientPassPhrase
            // 
            this.txtClientPassPhrase.Enabled = false;
            this.txtClientPassPhrase.Location = new System.Drawing.Point(145, 259);
            this.txtClientPassPhrase.Name = "txtClientPassPhrase";
            this.txtClientPassPhrase.Size = new System.Drawing.Size(643, 20);
            this.txtClientPassPhrase.TabIndex = 33;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 404);
            this.Controls.Add(this.txtClientPassPhrase);
            this.Controls.Add(this.txtClientPassPhraseFile);
            this.Controls.Add(this.txtClientCertFile);
            this.Controls.Add(this.txtServerPublicPath);
            this.Controls.Add(this.txtServerPublicFile);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtSEPPassword);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.chkSSL);
            this.Controls.Add(this.cboServerType);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cboCharSet);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cboPacket);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboDialect);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numPort);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.txtConnectionString);
            this.Name = "frmMain";
            this.Text = "IB Connection Builder";
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

