using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClinicWPF.Models;
using ClinicWPF.Services;

namespace ClinicWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Appointment.RemovePastAppointments();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(LoginBox.Text))
                {
                    System.Windows.MessageBox.Show("Поле логина пустое!");
                }
                else if (string.IsNullOrEmpty(PasswordBox.Password))
                {
                    System.Windows.MessageBox.Show("Поле пароля пусто!");
                }
                else
                {
                    using (var context = new ClinicdbContext())
                    {
                        string password = Hash.HashPassword(PasswordBox.Password);
                        if (context.Doctors.Any(d => d.Email == LoginBox.Text && d.Password == password))
                        {
                            Doctor.CurrentUser = context.Doctors.First(d => d.Email == LoginBox.Text && d.Password == password);
                            new MainDoctorWindow().Show();
                            this.Close();
                        }
                        else if (context.Patients.Any(p => p.Email == LoginBox.Text && p.Password == password))
                        {
                            Patient.CurrentUser = context.Patients.First(p => p.Email == LoginBox.Text && p.Password == password);
                            new MainPatientWindow().Show();
                            this.Close();
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Пользователь с указанным логином и паролем не найден!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка входа!");
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is RegistrationWindow)
                {
                    window.Activate();
                    return;
                }
            }

            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
        }
    }
}