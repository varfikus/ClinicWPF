using ClinicWPF.Models;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для MainMenuWindow.xaml
    /// </summary>
    public partial class MainDoctorWindow : Window
    {
        public MainDoctorWindow()
        {
            InitializeComponent();
            Appointment.RemovePastAppointments();
            LoadAppointments();
            CheckIfMain();
        }

        private void CheckIfMain()
        {
            if (Doctor.CurrentUser.Ismaindoctor == true)
            {
                ManageDoctors.Visibility = Visibility.Visible;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.CommandParameter is int appointmentId)
            {
                var result = System.Windows.MessageBox.Show("Вы действительно хотите удалить данное назначение?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var context = new ClinicdbContext())
                        {
                            var appointmentToRemove = context.Appointments.FirstOrDefault(a => a.Id == appointmentId);
                            if (appointmentToRemove != null)
                            {
                                context.Appointments.Remove(appointmentToRemove);
                                context.SaveChanges();

                                System.Windows.MessageBox.Show("Назначение успешно удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                                LoadAppointments();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void LoadAppointments()
        {
            using (var context = new ClinicdbContext())
            {
                var userAppointments = context.Appointments
                                              .Include(a => a.Doctor)
                                              .Include(a => a.Clinic)
                                              .Where(a => a.Doctorid == Doctor.CurrentUser.Id);

                var appointments = userAppointments
                    .Select(a => new
                    {
                        PatientName = a.Patient.Lastname + " " + a.Patient.Firstname,
                        a.Appointmentdatetime,
                        a.Reasonforvisit,
                        a.Id
                    })
                    .ToList();

                AppointmentsDataGrid.ItemsSource = appointments;
            }
        }

        private void ManageDoctors_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is ManageDoctorsWindow)
                {
                    window.Activate();
                    return;
                }
            }

            ManageDoctorsWindow manageDoctorsWindow = new ManageDoctorsWindow();
            manageDoctorsWindow.Show();
        }
    }
}
