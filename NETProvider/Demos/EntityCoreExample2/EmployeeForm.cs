using Microsoft.EntityFrameworkCore;

namespace EFCore101
{
    public partial class EmployeeForm : Form
    {
        MyEmployeeConnectionContext dbContext = new MyEmployeeConnectionContext();

        public EmployeeForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = dbContext.Employees.ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rnd = new Random();
            var emp = new Employee()
            {
                EmpNo = (short)dbContext.GetNextSequenceValue("emp_no_gen"),
                FirstName = "Testing " + rnd.Next(10000),
                LastName = "EFCore101",
                PhoneExt = "123",
                HireDate = DateTime.Now,
                DeptNo = "600",
                JobCode = "VP",
                JobGrade = 2,
                JobCountry = "USA",
                Salary = 80000 + rnd.Next(20000),
            };

            dbContext.Add<Employee>(emp);
            dbContext.SaveChanges();
            Form1_Load(sender, e);
        }
    }

}