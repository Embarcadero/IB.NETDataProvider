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
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ConsoleApp3.Model
{
    public partial class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Emp_No {get; set;}
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Ext { get; set; }
        public DateTime Hire_Date { get; set; }
        public string Dept_No { get; set; }
        public string Job_Code { get; set; }
        public int Job_Grade { get; set; }
        public string Job_Country { get; set; }
        public decimal Salary { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Full_Name { get; }
    }
}
