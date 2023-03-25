namespace EFCore101
{
    partial class EmployeeForm
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
            components = new System.ComponentModel.Container();
            employeeBindingSource = new BindingSource(components);
            panel1 = new Panel();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)employeeBindingSource).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 50);
            panel1.TabIndex = 5;
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(143, 23);
            button1.TabIndex = 1;
            button1.Text = "Create Employee";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.DataSource = employeeBindingSource;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 50);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(800, 400);
            dataGridView1.TabIndex = 6;
            // 
            // EmployeeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dataGridView1);
            Controls.Add(panel1);
            Name = "EmployeeForm";
            Text = "EFCore 101";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)employeeBindingSource).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private BindingSource employeeBindingSource;
        private Panel panel1;
        private Button button1;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn empNoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn firstNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn lastNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn phoneExtDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn hireDateDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn deptNoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn jobCodeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn jobGradeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn jobCountryDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn salaryDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fullNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn deptNoNavigationDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn jobDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn departmentsDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn projectsDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn salaryHistoriesDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn salesDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn projsDataGridViewTextBoxColumn;
    }
}