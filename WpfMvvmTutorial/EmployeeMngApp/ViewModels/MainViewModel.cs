using Caliburn.Micro;
using EmployeeMngApp.Models;
using System.Data.SqlClient;
using System.Windows;

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
                NotifyOfPropertyChange(() => EmpName);
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

                if (selectedEmployee != null)
                {
                    Id = value.Id;
                    EmpName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Destination = value.Destination;

                    NotifyOfPropertyChange(() => SelectedEmployee);
                    /*NotifyOfPropertyChange(() => Id);          // "" != null
                    NotifyOfPropertyChange(() => EmpName);
                    NotifyOfPropertyChange(() => Salary);
                    NotifyOfPropertyChange(() => DeptName);
                    NotifyOfPropertyChange(() => Destination);*/ // 주석을 풀어도 무방
                }
            }
        }

        public MainViewModel()
        {
            DisplayName = "Employee Management App";

            // DB연결
            GetEmployees();
        }

        public void GetEmployees()
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

        public void SaveEmployee()
        {
            int resultRow = 0;  // UPDATE 기본 1, INSERT 기본 1

            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    conn.Open();
                    var upquery = @"UPDATE Employees
                                       SET EmpName = @empName
                                         , Salary = @salary
                                         , DeptName = @deptName
                                         , Destination = @destination
                                     WHERE Id = @id";

                    var inquery = @"INSERT INTO Employees
                                              ( EmpName
                                              , Salary
                                              , DeptName
                                              , Destination)
                                         VALUES
                                              ( @empName
                                              , @salary
                                              , @deptName
                                              , @destination)";

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    if (Id == 0) // insert
                        cmd.CommandText = inquery;
                    else // update 
                        cmd.CommandText = upquery;

                    SqlParameter empNameParam = new SqlParameter("@empName", EmpName);
                    cmd.Parameters.Add(empNameParam);
                    SqlParameter salaryParam = new SqlParameter("@salary", Salary);
                    cmd.Parameters.Add(salaryParam);
                    SqlParameter deptNameParam = new SqlParameter("@deptName", DeptName);
                    cmd.Parameters.Add(deptNameParam);
                    SqlParameter destinationParam = new SqlParameter("@destination", Destination);
                    cmd.Parameters.Add(destinationParam);

                    SqlParameter idParam = new SqlParameter("@id", Id);
                    cmd.Parameters.Add(idParam);

                    resultRow = cmd.ExecuteNonQuery();

                    if (resultRow > 0)
                    {
                        GetEmployees();
                    }
                    else
                    {
                        MessageBox.Show("데이터 저장 실패!");
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"예외발생 : {ex.Message}");
                //return;
            }
            finally
            {
                NewEmployee();
            }
        }


        // 신규버튼 클릭 | 저장이후 입력컨트롤 비우기
        public void NewEmployee()
        {
            Id = 0;
            Salary = 0;
            EmpName = DeptName = Destination = string.Empty;
        }
    }
}
