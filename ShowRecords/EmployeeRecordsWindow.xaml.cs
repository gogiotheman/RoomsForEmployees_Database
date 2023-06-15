using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for EmployeeRecordsWindow.xaml
    /// </summary>
    public partial class EmployeeRecordsWindow : Window
    {
        private readonly string connectionString = "server=localhost;user id=root;password=password;database=roomsforemployees";
        public EmployeeRecordsWindow()
        {
            InitializeComponent();

            // Load employee records from the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT employeeID, name, department FROM employees";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Create a data table to store the employee records
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Bind the data table to the data grid control
                        EmployeesDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
        }
        public EmployeeRecordsWindow(string employeeName)
        {
            InitializeComponent();

            // Load employee records from the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT employeeID, name, department FROM employees WHERE name=@name";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", employeeName);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Create a data table to store the employee records
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Bind the data table to the data grid control
                        EmployeesDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
        }
    }

}
