using DataGrid;
using DataGridNamespace;
using MySql.Data.MySqlClient;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyProject
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void CloseImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void GoToLayout2_Click(object sender, RoutedEventArgs e)
        {
            Layout1.Visibility = Visibility.Collapsed;
            Layout2.Visibility = Visibility.Visible;
        }

        private void GoToLayout1_Click(object sender, RoutedEventArgs e)
        {
            Layout2.Visibility = Visibility.Collapsed;
            Layout1.Visibility = Visibility.Visible;
        }

        // Layout1 Events
        private void textUser_L1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUser_L1.Focus();
        }

        private void txtUser_L1_TextChanged(object sender, TextChangedEventArgs e)
        {
            textUser_L1.Visibility = string.IsNullOrEmpty(txtUser_L1.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void textPassword_L1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword_L1.Focus();
        }

        private void txtPassword_L1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            textPassword_L1.Visibility = string.IsNullOrEmpty(txtPassword_L1.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // زر تسجيل الدخول في Layout1
        private void SignIn_L1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser_L1.Text) || string.IsNullOrEmpty(txtPassword_L1.Password))
            {
                MessageBox.Show("Please fill Username & Password.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string connectionString = "Server=localhost;Database=gestion_theses;User ID=root;Password=";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string loginQuery = "SELECT COUNT(*), Role FROM users WHERE Nom=@Nom AND Password=@Password";
                    using (MySqlCommand loginCmd = new MySqlCommand(loginQuery, conn))
                    {
                        loginCmd.Parameters.AddWithValue("@Nom", txtUser_L1.Text);
                        loginCmd.Parameters.AddWithValue("@Password", txtPassword_L1.Password);

                        using (MySqlDataReader reader = loginCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int count = reader.GetInt32(0);
                                if (count > 0)
                                {
                                    string role = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                    MessageBox.Show("مرحبا بك! تم تسجيل الدخول بنجاح.");
                                    // بعد نجاح تسجيل الدخول يتم فتح نافذة الداشبورد
                                    DashboardView dashboard = new DashboardView();
                                    dashboard.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}");
            }
        }

        // Layout2 Events (للتسجيل)
        private void textUser_L2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUser_L2.Focus();
        }

        private void txtUser_L2_TextChanged(object sender, TextChangedEventArgs e)
        {
            textUser_L2.Visibility = string.IsNullOrEmpty(txtUser_L2.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void textEmail_L2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail_L2.Focus();
        }

        private void txtEmail_L2_TextChanged(object sender, TextChangedEventArgs e)
        {
            textEmail_L2.Visibility = string.IsNullOrEmpty(txtEmail_L2.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void textPassword_L2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword_L2.Focus();
        }

        private void txtPassword_L2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            textPassword_L2.Visibility = string.IsNullOrEmpty(txtPassword_L2.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SignUp_L2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser_L2.Text) || string.IsNullOrEmpty(txtEmail_L2.Text) || string.IsNullOrEmpty(txtPassword_L2.Password))
            {
                MessageBox.Show("Please fill Username, Email & Password.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(txtEmail_L2.Text))
            {
                MessageBox.Show("Invalid Email!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedItem = roleCombo_L2.SelectedItem as ComboBoxItem;
            string roleValue = (selectedItem != null) ? selectedItem.Content.ToString() : "Unknown";
            string connectionString = "Server=localhost;Database=gestion_theses;User ID=root;Password=";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE Email = @Email";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", txtEmail_L2.Text);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Cet email est déjà utilisé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    string insertQuery = "INSERT INTO users (Nom, Email, Password, Role) VALUES (@Nom, @Email, @Password, @Role)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nom", txtUser_L2.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEmail_L2.Text);
                        cmd.Parameters.AddWithValue("@Password", txtPassword_L2.Password);
                        cmd.Parameters.AddWithValue("@Role", roleValue);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Inscription réussie en tant que {roleValue} !");
                Layout2.Visibility = Visibility.Collapsed;
                Layout1.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'inscription : {ex.Message}");
            }
        }

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
