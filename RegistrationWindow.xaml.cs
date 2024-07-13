using ClinicWPF.Models;
using ClinicWPF.Services;
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

namespace ClinicWPF
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ClinicdbContext())
            {
                try
                {
                    if (string.IsNullOrEmpty(FirstNameTextBox.Text) ||
                        string.IsNullOrEmpty(LastNameTextBox.Text) ||
                        DateOfBirthPicker.SelectedDate == null ||
                        string.IsNullOrEmpty(GenderComboBox.Text) ||
                        string.IsNullOrEmpty(PhoneTextBox.Text) ||
                        string.IsNullOrEmpty(EmailTextBox.Text) ||
                        string.IsNullOrEmpty(AddressTextBox.Text) ||
                        string.IsNullOrEmpty(PasswordTextBox.Password))
                    {
                        System.Windows.MessageBox.Show("Все поля должны быть заполнены", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var patient = new Patient
                    {
                        Firstname = FirstNameTextBox.Text,
                        Lastname = LastNameTextBox.Text,
                        Dateofbirth = DateOnly.FromDateTime(DateOfBirthPicker.SelectedDate.Value),
                        Gender = GenderComboBox.Text,
                        Phone = PhoneTextBox.Text,
                        Email = EmailTextBox.Text,
                        Address = AddressTextBox.Text,
                        Password = Hash.HashPassword(PasswordTextBox.Password)
                    };
                    context.Patients.Add(patient);
                    context.SaveChanges();

                    System.Windows.MessageBox.Show("Пациент успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    foreach (Window window in System.Windows.Application.Current.Windows)
                    {
                        if (window is MainWindow)
                        {
                            window.Activate();
                            return;
                        }
                    }

                    this.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Activate();
                    return;
                }
            }
        }
    }
}
