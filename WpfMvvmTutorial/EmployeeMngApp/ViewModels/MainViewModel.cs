using Caliburn.Micro;
using EmployeeMngApp.Models;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace EmployeeMngApp.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private readonly string connstring = "Data Source=localhost;Initial Catalog=EMS;Integrated Security=True";
        private BindableCollection<Employees> employees;

        public BindableCollection<Employees> Employees
        {
            get => employees;
            set
            {
                employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }

        private int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        private string empName;
        public string EmpName
        {
            get => empName;
            set
            {
                empName = value;
                NotifyOfPropertyChange(() => empName);
            }
        }

        private decimal salary;
        public decimal Salary
        {
            get => salary;
            set
            {
                salary = value;
                NotifyOfPropertyChange(() => Salary);
            }
        }

        private string deptName;
        public string DeptName
        {
            get => deptName;
            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => DeptName);
            }
        }

        private string destination;
        public string Destination
        {
            get => destination;
            set
            {
                destination = value;
                NotifyOfPropertyChange(() => Destination);
            }
        }

        private Employees selectedEmployee;
        public Employees SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                selectedEmployee = value;
            }
        }

        public MainViewModel()
        {
            DisplayName = "Employee Management App";

            // DB연결
            GetEmployees();
        }

        private void GetEmployees()
        {
            using (SqlConnection conn = new SqlConnection(connstring))
            {
                conn.Open();
                string selquery = @"SELECT Id
                                         , EmpName
                                         , Salary
                                         , DeptName
                                         , Destination
                                      FROM Employees";
                SqlCommand cmd = new SqlCommand(selquery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                Employees = new BindableCollection<Employees>();

                while (reader.Read())
                {
                    var empTmp = new Employees
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Destination = reader["Destination"].ToString()
                    };
                    Employees.Add(empTmp);
                }

            }
        }
    }
}
