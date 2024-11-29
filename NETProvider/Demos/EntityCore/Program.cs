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

using System;
using ConsoleApp3.Model;
using System.Linq;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EmployeeContext())
            {
                // Creating a new Employee and saving it to the database
                var newEmp = new Employee()
                {
                    Emp_No = db.GetNextSequenceValue("EMP_NO_GEN"),
                    First_Name = "John", Last_Name = "Doe", Phone_Ext = "000", 
                    Hire_Date = DateTime.Now, Dept_No = "900", Job_Code = "Sales",
                    Job_Grade = 3, Job_Country = "USA", Salary = 45000
                };
                db.Employee.Add(newEmp);
                try
                { 
                    var count = db.SaveChanges(); 
                    Console.WriteLine("{0} records saved to database", count);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine(db.LastCommandText);
                }

                // Retrieving and displaying data
                Console.WriteLine();
                Console.WriteLine("All Employees in the database:");
                foreach (var emp in db.Employee)
                {
                    Console.WriteLine("{0} | {1} | {2}", emp.Full_Name, emp.Salary, emp.Dept_No);
                }
                }
        }
    }
}
