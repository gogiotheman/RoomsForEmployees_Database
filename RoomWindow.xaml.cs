using System;
using System.Collections.Generic;
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
using MySql.Data.MySqlClient;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for RoomWindow.xaml
    /// </summary>
    public partial class RoomWindow : Window
    {
        private readonly string connectionString = "server=localhost;user id=root;password=password;database=roomsforemployees";

        private int PopulateIDComboBox()
        {
            int last_ID = 0;
            IDComboBox.Items.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                //RoomIDComboBox
                string query1 = "SELECT RoomID FROM rooms";
                using (MySqlCommand command = new MySqlCommand(query1, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            last_ID = reader.GetInt32("RoomID");
                            IDComboBox.Items.Add(last_ID);
                        }
                    }
                }
            }
            return last_ID;
        }
        private void PopulateComboBoxes()
        {
            //NumberComboBox
            for (int i = 1; i <= 19; i++)
            {
                if(i < 10)
                {
                    NumberComboBox.Items.Add("0" + i.ToString());
                }
                else NumberComboBox.Items.Add(i.ToString());
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                //EmployeeIDComboBox
                string query1 = "SELECT employeeID FROM employees";
                using (MySqlCommand command = new MySqlCommand(query1, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeIDComboBox.Items.Add(reader.GetInt32("employeeID"));
                        }
                    }
                }

                //ProjectIDComboBox
                string query2 = "SELECT projectID FROM projects";
                using (MySqlCommand command = new MySqlCommand(query2, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProjectIDComboBox.Items.Add(reader.GetInt32("projectID"));
                        }
                    }
                }
            }
        }

        public RoomWindow()
        {
            InitializeComponent();
            PopulateComboBoxes();
            PopulateIDComboBox();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate the input data
            if (NumberComboBox.SelectedValue == null || StartDatePicker.SelectedDate == null || StopDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Number, Start, and Stop cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Open a connection to the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Create a SQL query to insert a new room record into the database
                string query = "INSERT INTO Rooms (EmployeeID, ProjectID, Number, Start, Stop) VALUES (@employeeID, @projectID, @Number, @Start, @Stop)";

                // Execute the query
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", EmployeeIDComboBox.SelectedValue);
                    command.Parameters.AddWithValue("@ProjectID", ProjectIDComboBox.SelectedValue);
                    command.Parameters.AddWithValue("@Number", NumberComboBox.SelectedValue);
                    command.Parameters.AddWithValue("@Start", StartDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@Stop", StopDatePicker.SelectedDate);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Room created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Clear the text fields
                            EmployeeIDComboBox.SelectedValue = null;
                            ProjectIDComboBox.SelectedValue = null;
                            NumberComboBox.SelectedValue = null;
                            StartDatePicker.SelectedDate = null;
                            StopDatePicker.SelectedDate = null;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error creating room: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            PopulateIDComboBox();
        }


        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            short k = 0;
            // Open a new window to display the room records
            ShowRecords.RoomRecordsWindow roomRecordsWindow;
            if (NumberComboBox.SelectedValue != null)
            {
                roomRecordsWindow = new ShowRecords.RoomRecordsWindow(NumberComboBox.SelectedValue.ToString());
                roomRecordsWindow.Show();
                MessageBox.Show("Rooms showed by number");
                k = 1;
                NumberComboBox.SelectedValue = null;
            }
            if (EmployeeIDComboBox.SelectedValue != null)
            {
                roomRecordsWindow = new ShowRecords.RoomRecordsWindow("employeeID", Convert.ToInt32(EmployeeIDComboBox.SelectedValue));
                roomRecordsWindow.Show();
                MessageBox.Show("Rooms showed by employeeID");
                k = 1;
                EmployeeIDComboBox.SelectedValue = null;
            }
            if (ProjectIDComboBox.SelectedValue != null)
            {
                roomRecordsWindow = new ShowRecords.RoomRecordsWindow("projectID", Convert.ToInt32(ProjectIDComboBox.SelectedValue));
                roomRecordsWindow.Show();
                MessageBox.Show("Rooms showed by projectID");
                k = 1;
                ProjectIDComboBox.SelectedValue = null;
            }
            if(k == 0)
            {
                roomRecordsWindow = new ShowRecords.RoomRecordsWindow();
                roomRecordsWindow.Show();
            }
            
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@roomID", IDComboBox.SelectedValue);

                    if (EmployeeIDComboBox.SelectedValue != null)
                    {
                        command.CommandText = "UPDATE rooms SET employeeID=@employeeID WHERE roomID=@roomID";
                        command.Parameters.AddWithValue("@employeeID", EmployeeIDComboBox.SelectedValue);
                        command.ExecuteNonQuery();
                    }

                    if (ProjectIDComboBox.SelectedValue != null)
                    {
                        command.CommandText = "UPDATE rooms SET projectID=@projectID WHERE roomID=@roomID";
                        command.Parameters.AddWithValue("@projectID", ProjectIDComboBox.SelectedValue);
                        command.ExecuteNonQuery();
                    }

                    if (NumberComboBox.SelectedValue != null)
                    {
                        command.CommandText = "UPDATE rooms SET Number=@number WHERE roomID=@roomID";
                        command.Parameters.AddWithValue("@number", NumberComboBox.SelectedValue);
                        command.ExecuteNonQuery();
                    }

                    if (StartDatePicker.SelectedDate != null)
                    {
                        command.CommandText = "UPDATE rooms SET Start=@Start WHERE roomID=@roomID";
                        command.Parameters.AddWithValue("@Start", StartDatePicker.SelectedDate);
                        command.ExecuteNonQuery();
                    }

                    if (StopDatePicker.SelectedDate != null)
                    {
                        command.CommandText = "UPDATE rooms SET Stop=@Stop WHERE roomID=@roomID";
                        command.Parameters.AddWithValue("@Stop", StopDatePicker.SelectedDate);
                        command.ExecuteNonQuery();
                    }

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {

                        MessageBox.Show("Room record updated successfully!");
                        EmployeeIDComboBox.SelectedValue = null;
                        ProjectIDComboBox.SelectedValue = null;
                        NumberComboBox.SelectedValue = null;
                        StartDatePicker.SelectedDate = null;
                        StopDatePicker.SelectedDate = null;
                        IDComboBox.SelectedValue = null;
                    }
                    else
                    {
                        MessageBox.Show("Failed to update room record.");
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM rooms WHERE roomID = @roomID";
                    command.Parameters.AddWithValue("@roomID", IDComboBox.SelectedValue);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // decrement projectID for all records with higher projectID
                        command.CommandText = "UPDATE rooms SET roomID = roomID - 1 WHERE roomID > @roomID";
                        command.ExecuteNonQuery();

                        MessageBox.Show("Room record deleted successfully.");

                        command.CommandText = "ALTER TABLE rooms AUTO_INCREMENT = @last_ID";
                        command.Parameters.AddWithValue("@last_ID", PopulateIDComboBox());
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Error deleting employee record.");
                    }
                }
            }
            PopulateIDComboBox();
        }
    }
}
