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
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using InterBaseSql.Data.InterBaseClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private IBDataAdapter dataAdapter = new IBDataAdapter();

        public Form1()
        {
            InitializeComponent();
        }

        private void GetData(string selectCommand)
        {
            try
            {// Create a new data adapter based on the specified query.
                dataAdapter = new IBDataAdapter(selectCommand, textBox1.Text);

                // Create a command builder to generate SQL update, insert, and
                // delete commands based on selectCommand.
                IBCommandBuilder commandBuilder = new IBCommandBuilder(dataAdapter);

                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new DataTable { Locale = CultureInfo.InvariantCulture };

                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;
                dataGridView1.DataSource = bindingSource1;

                // Resize the DataGridView columns to fit the newly loaded content.
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
             GetData(textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var c = new IBConnection();
            c.ConnectionString = textBox1.Text;
            c.Open();
            MessageBox.Show(c.ConnectionString);
        }
    }
}
