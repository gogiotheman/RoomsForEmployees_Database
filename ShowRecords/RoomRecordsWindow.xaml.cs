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
    /// Interaction logic for RoomRecordsWindow.xaml
    /// </summary>
    public partial class RoomRecordsWindow : Window
    {
        private readonly string connectionString = "server=localhost;user id=root;password=password;database=roomsforemployees";
        public RoomRecordsWindow()
        {
            InitializeComponent();

            // Load employee records from the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT RoomID, employeeID, projectID, Number, Start, Stop FROM rooms";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Create a data table to store the employee records
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Bind the data table to the data grid control
                        RoomsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
        }
        public RoomRecordsWindow(string roomNumber)
        {
            InitializeComponent();

            // Load employee records from the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT RoomID, employeeID, projectID, Number, Start, Stop FROM rooms Where Number = @Number";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Number", roomNumber);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Create a data table to store the employee records
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Bind the data table to the data grid control
                        RoomsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
        }

        public RoomRecordsWindow(string fk, int fkID)
        {
            InitializeComponent();

            // Load employee records from the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = connection.CreateCommand())
                {
                    if (fk == "employeeID")
                    {
                        command.CommandText = "SELECT * FROM rooms WHERE employeeID = @employeeID";
                        command.Parameters.AddWithValue("@employeeID", fkID);
                        command.ExecuteNonQuery();
                    }
                    else if (fk == "projectID")
                    {
                        command.CommandText = "SELECT * FROM rooms Where projectID = @projectID";
                        command.Parameters.AddWithValue("@projectID", fkID);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command.CommandText = "SELECT * FROM rooms";
                        command.ExecuteNonQuery();
                    }
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Create a data table to store the employee records
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Bind the data table to the data grid control
                        RoomsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
        }
    }
}
