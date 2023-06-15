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

namespace WpfApp.ShowRecords
{
    /// <summary>
    /// Interaction logic for ProjectRecordsWindow.xaml
    /// </summary>
    public partial class ProjectRecordsWindow : Window
    {
        private readonly string connectionString = "server=localhost;user id=root;password=password;database=roomsforemployees";
        public ProjectRecordsWindow()
        {
            InitializeComponent();

            // Load employee records from the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT projectID, deadline, technology FROM projects";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Create a data table to store the employee records
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Bind the data table to the data grid control
                        ProjectsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
        }
    }
}
