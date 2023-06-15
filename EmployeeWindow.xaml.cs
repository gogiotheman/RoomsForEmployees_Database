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
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        public EmployeeWindow()
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

                //EmployeeIDComboBox
                string query1 = "SELECT employeeID FROM employees";
                using (MySqlCommand command = new MySqlCommand(query1, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            last_ID = reader.GetInt32("employeeID");
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
            if (string.IsNullOrEmpty(NameTextBox.Text) || string.IsNullOrEmpty(DepartmentTextBox.Text))
            {
                MessageBox.Show("Name and Department cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Open a connection to the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Create a SQL query to insert a new employee record into the database
                string query = "INSERT INTO Employees (Name, Department) VALUES (@name, @department)";

                // Execute the query
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", NameTextBox.Text);
                    command.Parameters.AddWithValue("@department", DepartmentTextBox.Text);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Employee created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Clear the text fields
                            NameTextBox.Clear();
                            DepartmentTextBox.Clear();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error creating employee: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            PopulateIDComboBox();
        }


        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a new window to display the employee records
            EmployeeRecordsWindow employeeRecordsWindow;
            if (string.IsNullOrEmpty(NameTextBox.Text)) employeeRecordsWindow = new EmployeeRecordsWindow();
            else employeeRecordsWindow = new EmployeeRecordsWindow(NameTextBox.Text);
            employeeRecordsWindow.Show();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@employeeID", IDComboBox.SelectedValue);
                    if (!string.IsNullOrEmpty(NameTextBox.Text))
                    {
                        command.CommandText = "UPDATE employees SET name=@name WHERE employeeID=@employeeID";
                        command.Parameters.AddWithValue("@name", NameTextBox.Text);
                        command.ExecuteNonQuery();
                    }

                    if (!string.IsNullOrEmpty(DepartmentTextBox.Text))
                    {
                        command.CommandText = "UPDATE employees SET department=@department WHERE employeeID=@employeeID";
                        command.Parameters.AddWithValue("@department", DepartmentTextBox.Text);
                        command.ExecuteNonQuery();
                    }

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {

                        MessageBox.Show("Employee record updated successfully!");
                        NameTextBox.Clear();
                        DepartmentTextBox.Clear();
                        IDComboBox.SelectedValue = null;
                    }
                    else
                    {
                        MessageBox.Show("Failed to update employee record.");
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
                    command.CommandText = "DELETE FROM employees WHERE employeeID = @employeeID";
                    command.Parameters.AddWithValue("@employeeID", IDComboBox.SelectedValue);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // decrement employeeID for all records with higher employeeID
                        command.CommandText = "UPDATE employees SET employeeID = employeeID - 1 WHERE employeeID > @employeeID";
                        command.ExecuteNonQuery();

                        // update employeeID in Rooms table for all records that reference the deleted employee record
                        command.CommandText = "UPDATE rooms SET employeeID = employeeID - 1 WHERE employeeID > @employeeID";
                        command.ExecuteNonQuery();

                        MessageBox.Show("Employee record deleted successfully.");

                        command.CommandText = "ALTER TABLE employees AUTO_INCREMENT = @last_ID";
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
