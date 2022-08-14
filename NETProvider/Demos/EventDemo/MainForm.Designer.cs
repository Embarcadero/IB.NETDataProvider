namespace EventDemo
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.btnCloseDatabase = new System.Windows.Forms.Button();
            this.btnOpenDatabase = new System.Windows.Forms.Button();
            this.grpEvents = new System.Windows.Forms.GroupBox();
            this.edEvent = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenerateEvent = new System.Windows.Forms.Button();
            this.grpRegistration = new System.Windows.Forms.GroupBox();
            this.moRegister = new System.Windows.Forms.TextBox();
            this.btnRegisterEvents = new System.Windows.Forms.Button();
            this.grpReceived = new System.Windows.Forms.GroupBox();
            this.lbReceived = new System.Windows.Forms.ListBox();
            this.btnClearEventsReceived = new System.Windows.Forms.Button();
            this.grpConnection.SuspendLayout();
            this.grpEvents.SuspendLayout();
            this.grpRegistration.SuspendLayout();
            this.grpReceived.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.btnCloseDatabase);
            this.grpConnection.Controls.Add(this.btnOpenDatabase);
            this.grpConnection.Location = new System.Drawing.Point(28, 15);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(200, 100);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Database Connection";
            // 
            // btnCloseDatabase
            // 
            this.btnCloseDatabase.Enabled = false;
            this.btnCloseDatabase.Location = new System.Drawing.Point(28, 63);
            this.btnCloseDatabase.Name = "btnCloseDatabase";
            this.btnCloseDatabase.Size = new System.Drawing.Size(150, 23);
            this.btnCloseDatabase.TabIndex = 1;
            this.btnCloseDatabase.Text = "Close Database";
            this.btnCloseDatabase.UseVisualStyleBackColor = true;
            this.btnCloseDatabase.Click += new System.EventHandler(this.btnCloseDatabase_Click);
            // 
            // btnOpenDatabase
            // 
            this.btnOpenDatabase.Location = new System.Drawing.Point(28, 34);
            this.btnOpenDatabase.Name = "btnOpenDatabase";
            this.btnOpenDatabase.Size = new System.Drawing.Size(150, 23);
            this.btnOpenDatabase.TabIndex = 0;
            this.btnOpenDatabase.Text = "Open Database";
            this.btnOpenDatabase.UseVisualStyleBackColor = true;
            this.btnOpenDatabase.Click += new System.EventHandler(this.btnOpenDatabase_Click);
            // 
            // grpEvents
            // 
            this.grpEvents.Controls.Add(this.edEvent);
            this.grpEvents.Controls.Add(this.label1);
            this.grpEvents.Controls.Add(this.btnGenerateEvent);
            this.grpEvents.Enabled = false;
            this.grpEvents.Location = new System.Drawing.Point(238, 15);
            this.grpEvents.Name = "grpEvents";
            this.grpEvents.Size = new System.Drawing.Size(200, 100);
            this.grpEvents.TabIndex = 1;
            this.grpEvents.TabStop = false;
            this.grpEvents.Text = "Generate Events";
            // 
            // edEvent
            // 
            this.edEvent.Location = new System.Drawing.Point(56, 27);
            this.edEvent.Name = "edEvent";
            this.edEvent.Size = new System.Drawing.Size(123, 23);
            this.edEvent.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Event";
            // 
            // btnGenerateEvent
            // 
            this.btnGenerateEvent.Enabled = false;
            this.btnGenerateEvent.Location = new System.Drawing.Point(26, 63);
            this.btnGenerateEvent.Name = "btnGenerateEvent";
            this.btnGenerateEvent.Size = new System.Drawing.Size(153, 23);
            this.btnGenerateEvent.TabIndex = 1;
            this.btnGenerateEvent.Text = "Generate Event";
            this.btnGenerateEvent.UseVisualStyleBackColor = true;
            this.btnGenerateEvent.Click += new System.EventHandler(this.btnGenerateEvent_Click);
            // 
            // grpRegistration
            // 
            this.grpRegistration.Controls.Add(this.moRegister);
            this.grpRegistration.Controls.Add(this.btnRegisterEvents);
            this.grpRegistration.Enabled = false;
            this.grpRegistration.Location = new System.Drawing.Point(28, 136);
            this.grpRegistration.Name = "grpRegistration";
            this.grpRegistration.Size = new System.Drawing.Size(200, 180);
            this.grpRegistration.TabIndex = 2;
            this.grpRegistration.TabStop = false;
            this.grpRegistration.Text = "Event Registration";
            // 
            // moRegister
            // 
            this.moRegister.Location = new System.Drawing.Point(6, 25);
            this.moRegister.Multiline = true;
            this.moRegister.Name = "moRegister";
            this.moRegister.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.moRegister.Size = new System.Drawing.Size(188, 121);
            this.moRegister.TabIndex = 0;
            this.moRegister.Text = "aa\r\nbb\r\n1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n10\r\n11\r\n12\r\n13";
            // 
            // btnRegisterEvents
            // 
            this.btnRegisterEvents.Enabled = false;
            this.btnRegisterEvents.Location = new System.Drawing.Point(28, 152);
            this.btnRegisterEvents.Name = "btnRegisterEvents";
            this.btnRegisterEvents.Size = new System.Drawing.Size(150, 22);
            this.btnRegisterEvents.TabIndex = 1;
            this.btnRegisterEvents.Text = "Register Events";
            this.btnRegisterEvents.UseVisualStyleBackColor = true;
            this.btnRegisterEvents.Click += new System.EventHandler(this.btnRegisterEvents_Click);
            // 
            // grpReceived
            // 
            this.grpReceived.Controls.Add(this.lbReceived);
            this.grpReceived.Controls.Add(this.btnClearEventsReceived);
            this.grpReceived.Enabled = false;
            this.grpReceived.Location = new System.Drawing.Point(238, 136);
            this.grpReceived.Name = "grpReceived";
            this.grpReceived.Size = new System.Drawing.Size(200, 180);
            this.grpReceived.TabIndex = 3;
            this.grpReceived.TabStop = false;
            this.grpReceived.Text = "Event Received";
            // 
            // lbReceived
            // 
            this.lbReceived.FormattingEnabled = true;
            this.lbReceived.ItemHeight = 15;
            this.lbReceived.Location = new System.Drawing.Point(8, 22);
            this.lbReceived.Name = "lbReceived";
            this.lbReceived.Size = new System.Drawing.Size(182, 124);
            this.lbReceived.TabIndex = 0;
            // 
            // btnClearEventsReceived
            // 
            this.btnClearEventsReceived.Enabled = false;
            this.btnClearEventsReceived.Location = new System.Drawing.Point(26, 151);
            this.btnClearEventsReceived.Name = "btnClearEventsReceived";
            this.btnClearEventsReceived.Size = new System.Drawing.Size(153, 23);
            this.btnClearEventsReceived.TabIndex = 1;
            this.btnClearEventsReceived.Text = "Clear Events Received";
            this.btnClearEventsReceived.UseVisualStyleBackColor = true;
            this.btnClearEventsReceived.Click += new System.EventHandler(this.btnClearEventsReceived_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 343);
            this.Controls.Add(this.grpReceived);
            this.Controls.Add(this.grpRegistration);
            this.Controls.Add(this.grpEvents);
            this.Controls.Add(this.grpConnection);
            this.Name = "MainForm";
            this.Text = "Event Alerter Demo";
            this.grpConnection.ResumeLayout(false);
            this.grpEvents.ResumeLayout(false);
            this.grpEvents.PerformLayout();
            this.grpRegistration.ResumeLayout(false);
            this.grpRegistration.PerformLayout();
            this.grpReceived.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox grpConnection;
        private GroupBox grpEvents;
        private GroupBox grpRegistration;
        private GroupBox grpReceived;
        private Button btnCloseDatabase;
        private Button btnOpenDatabase;
        private Button btnGenerateEvent;
        private Button btnRegisterEvents;
        private Button btnClearEventsReceived;
        private TextBox moRegister;
        private ListBox lbReceived;
        private TextBox edEvent;
        private Label label1;
    }
}