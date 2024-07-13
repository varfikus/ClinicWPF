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
using ClinicWPF.Services;

namespace ClinicWPF
{
    /// <summary>
    /// Логика взаимодействия для ManageDoctorsWindow.xaml
    /// </summary>
    public partial class ManageDoctorsWindow : Window
    {
        string oldPassword = "";

        public ManageDoctorsWindow()
        {
            InitializeComponent();
            UpdateClinicsList(Doctor.CurrentUser.Id);
            LoadDoctors();
        }

        public void LoadDoctors()
        {
            using (var context = new ClinicdbContext())
            {
                var clinicIds = context.Doctorclinics
                    .Where(dc => dc.Doctorid == Doctor.CurrentUser.Id)
                    .Select(dc => dc.Clinicid)
                    .ToList();

                var doctors = context.Doctorclinics
                    .Where(dc => clinicIds.Contains(dc.Clinicid))
                    .Select(dc => dc.Doctor)
                    .Distinct()
                    .ToList();

                var doctorViewModels = doctors.Select(d => new
                {
                    DoctorName = d.Firstname + " " + d.Lastname,
                    Specialty = d.Specialty,
                    d.Id
                })
                .ToList();

                DoctorsDataGrid.ItemsSource = doctorViewModels;
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.CommandParameter is int Id)
            {
                try
                {
                    using (var context = new ClinicdbContext())
                    {
                        var doctorToManage = context.Doctors.FirstOrDefault(d => d.Id == Id);
                        if (doctorToManage != null)
                        {
                            ManageFieldsLoad(doctorToManage);
                            UpdateClinicsList(doctorToManage.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ManageFieldsLoad(Doctor doctor)
        {
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            SpecialtyTextBox.Text = "";
            PhoneTextBox.Text = "";
            EmailTextBox.Text = "";
            PasswordTextBox.Text = "";
            ChangeButton.Tag = null;

            using (var context = new ClinicdbContext())
            {
                FirstNameTextBox.Text = doctor.Firstname;
                LastNameTextBox.Text = doctor.Lastname;
                SpecialtyTextBox.Text = doctor.Specialty;
                PhoneTextBox.Text = doctor.Phone;
                EmailTextBox.Text = doctor.Email;
                PasswordTextBox.Text = doctor.Password;
                oldPassword = doctor.Password;
                ChangeButton.Tag = doctor.Id;
            }
        }

        private void UpdateClinicsList(int doctorId)
        {
            using (var context = new ClinicdbContext())
            {
                var associatedClinic = context.Doctorclinics
                    .Where(dc => dc.Doctorid == doctorId)
                    .Select(dc => dc.Clinic)
                    .ToList();

                var allClinic = context.Clinics.ToList();

                ClinicsComboBox.ItemsSource = allClinic;
                ClinicsComboBox.DisplayMemberPath = "Clinicname";
                ClinicsComboBox.SelectedValuePath = "Id";

                var currentClinic = associatedClinic.FirstOrDefault();
                if (currentClinic != null)
                {
                    ClinicsComboBox.SelectedValue = currentClinic.Id;
                }
            }
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeButton.Tag is int doctorId)
            {
                using (var context = new ClinicdbContext())
                {
                    var doctor = context.Doctors.FirstOrDefault(d => d.Id == doctorId);
                    if (doctor != null)
                    {
                        doctor.Firstname = FirstNameTextBox.Text;
                        doctor.Lastname = LastNameTextBox.Text;
                        doctor.Specialty = SpecialtyTextBox.Text;
                        doctor.Phone = PhoneTextBox.Text;
                        doctor.Email = EmailTextBox.Text;


                        if (!string.IsNullOrEmpty(PasswordTextBox.Text) && oldPassword != PasswordTextBox.Text)
                        {
                            doctor.Password = Hash.HashPassword(PasswordTextBox.Text);
                        }

                        if (ClinicsComboBox.SelectedValue is int selectedClinicId)
                        {
                            var currentAssociations = context.Doctorclinics.Where(dc => dc.Doctorid == doctorId).ToList();
                            context.Doctorclinics.RemoveRange(currentAssociations);

                            var newAssociation = new Doctorclinic
                            {
                                Doctorid = doctorId,
                                Clinicid = selectedClinicId
                            };
                            context.Doctorclinics.Add(newAssociation);
                        }

                        context.SaveChanges();
                        LoadDoctors();

                        System.Windows.MessageBox.Show("Информация изменена успешно", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Неверный ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Неверный ID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                using (var context = new ClinicdbContext())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(FirstNameTextBox.Text) ||
                            string.IsNullOrEmpty(LastNameTextBox.Text) ||
                            string.IsNullOrEmpty(SpecialtyTextBox.Text) ||
                            string.IsNullOrEmpty(PhoneTextBox.Text) ||
                            string.IsNullOrEmpty(EmailTextBox.Text))
                        {
                            System.Windows.MessageBox.Show("Все поля должны быть заполнены", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (ClinicsComboBox.SelectedValue is int selectedClinicId)
                        {
                            var doctor = new Doctor
                            {
                                Firstname = FirstNameTextBox.Text,
                                Lastname = LastNameTextBox.Text,
                                Specialty = SpecialtyTextBox.Text,
                                Phone = PhoneTextBox.Text,
                                Email = EmailTextBox.Text,
                                Password = Hash.HashPassword(PasswordTextBox.Text)
                            };
                            context.Doctors.Add(doctor);
                            context.SaveChanges();

                            var newAssociation = new Doctorclinic
                            {
                                Doctorid = doctor.Id,
                                Clinicid = selectedClinicId
                            };

                            context.Doctorclinics.Add(newAssociation);
                            context.SaveChanges();

                            System.Windows.MessageBox.Show("Доктор успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadDoctors();
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Выберите клинику из списка", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Неверный пароль", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
