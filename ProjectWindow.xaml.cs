using MySql.Data.MySqlClient;
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

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window
    {
        public ProjectWindow()
        {
            InitializeComponent();
            PopulateIDComboBox();
        }

        private int PopulateIDComboBox()
        {
            int last_ID = 0;
            IDComboBox.Items.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                //ProjectIDComboBox
                string query1 = "SELECT projectID FROM projects";
                using (MySqlCommand command = new MySqlCommand(query1, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            last_ID = reader.GetInt32("projectID");
                            IDComboBox.Items.Add(last_ID);
                        }
                    }
                }
            }
            return last_ID;
        }

        private readonly string connectionString = "server=localhost;user id=root;password=password;database=roomsforemployees";
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate the input data
            if (string.IsNullOrEmpty(TechnologyTextBox.Text) || DeadlineDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Technology and Deadline cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Open a connection to the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Create a SQL query to insert a new project record into the database
                string query = "INSERT INTO Projects (Deadline, Technology) VALUES (@deadline, @technology)";

                // Execute the query
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@deadline", DeadlineDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@technology", TechnologyTextBox.Text);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Project created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Clear the text fields
                            DeadlineDatePicker.SelectedDate = null;
                            TechnologyTextBox.Clear();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error creating project: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            PopulateIDComboBox();
        }


        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a new window to display the room records
            ShowRecords.ProjectRecordsWindow projectRecordsWindow = new ShowRecords.ProjectRecordsWindow();
            projectRecordsWindow.Show();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@projectID", IDComboBox.SelectedValue);
                    if (!string.IsNullOrEmpty(TechnologyTextBox.Text))
                    {
                        command.CommandText = "UPDATE projects SET technology=@technology WHERE projectID=@projectID";
                        command.Parameters.AddWithValue("@technology", TechnologyTextBox.Text);
                        command.ExecuteNonQuery();
                    }

                    if (DeadlineDatePicker.SelectedDate != null)
                    {
                        command.CommandText = "UPDATE projects SET deadline=@deadline WHERE projectID=@projectID";
                        command.Parameters.AddWithValue("@deadline", DeadlineDatePicker.SelectedDate);
                        command.ExecuteNonQuery();
                    }

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {

                        MessageBox.Show("Project record updated successfully!");
                        DeadlineDatePicker.SelectedDate = null;
                        TechnologyTextBox.Clear();
                        IDComboBox.SelectedValue = null;
                    }
                    else
                    {
                        MessageBox.Show("Failed to update project record.");
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
                    command.CommandText = "DELETE FROM projects WHERE projectID = @projectID";
                    command.Parameters.AddWithValue("@projectID", IDComboBox.SelectedValue);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // decrement projectID for all records with higher projectID
                        command.CommandText = "UPDATE projects SET projectID = projectID - 1 WHERE projectID > @projectID";
                        command.ExecuteNonQuery();

                        // update projectID in Rooms table for all records that reference the deleted project record
                        command.CommandText = "UPDATE rooms SET projectID = projectID - 1 WHERE projectID > @projectID";
                        command.ExecuteNonQuery();

                        MessageBox.Show("Project record deleted successfully.");

                        command.CommandText = "ALTER TABLE projects AUTO_INCREMENT = @last_ID";
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
