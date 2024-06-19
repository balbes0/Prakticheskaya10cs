using System.Windows.Media.Imaging;
using System.Windows;
using WpfApp1.ViewModel.Helpers;
using System.Collections.ObjectModel;
using WpfApp1.Models;
using Newtonsoft.Json;
using System.Data;
using WpfApp1.View.Cards;
using System.Windows.Controls;

namespace WpfApp1.ViewModel
{
    class MainWindowViewModel : BindingHelper
    {
        private string port = "7107";
        private bool isDarkTheme = false;
        public MainWindowViewModel()
        {
            SwitchThemeCommand = new BindableCommand(_ => SwitchTheme()); //направления пример
            AnalysisComboBoxCommand = new BindableCommand(_ => AnalysisRTB()); //направления пример
            ResearchComboBoxCommand = new BindableCommand(_ => ResearchRTB());
            StartAppointmentCommand = new BindableCommand(ShowOMS);
            CancelAppointmentCommand = new BindableCommand(CancelAppointment); //кнопка отменить запись
            CancelMissedAppointmentCommand = new BindableCommand(CancelMissedAppointment);
            CompleteTheAppointmentCommand = new BindableCommand(_ => CompleteTheAppointment()); //кнопка завершить прием
            LogoutCommand = new BindableCommand(_ => Logout()); //кнопка выйти из аккаунта
            analysisRTBVisibility = Visibility.Collapsed; //РТБ с анализами по дефолту скрыта
            researchRTBVisibility = Visibility.Collapsed; //РТБ с исследованиями по дефолту скрыта
            AttachFilesButton = Visibility.Collapsed; //Кнопка прикрепить файлы по дефолту скрыта
            MainVisibility = Visibility.Hidden;
            UpdateCurrentAppointments();
        }

        #region Свойства
        #region Список_записей
        private ObservableCollection<Appointment> currentAppointments;
        public ObservableCollection<Appointment> CurrentAppointments
        {
            get { return currentAppointments; }
            set
            {
                currentAppointments = value;
                OnPropertyChanged(nameof(CurrentAppointments));
            }
        }

        private ObservableCollection<Appointment> missedAppointments;
        public ObservableCollection<Appointment> MissedAppointments
        {
            get { return missedAppointments; }
            set
            {
                missedAppointments = value;
                OnPropertyChanged(nameof(MissedAppointments));
            }
        }        
        
        private ObservableCollection<Appointment> completedAppointments;
        public ObservableCollection<Appointment> CompletedAppointments
        {
            get { return completedAppointments; }
            set
            {
                completedAppointments = value;
                OnPropertyChanged(nameof(CompletedAppointments));
            }
        }

        private ObservableCollection<Appointment> allAppointments;
        public ObservableCollection<Appointment> AllAppointments
        {
            get { return allAppointments; }
            set
            {
                allAppointments = value;
                OnPropertyChanged(nameof(AllAppointments));
            }
        }

        private ObservableCollection<UserControl> _appointmentCards;
        public ObservableCollection<UserControl> AppointmentCards
        {
            get { return _appointmentCards; }
            set
            {
                _appointmentCards = value;
                OnPropertyChanged(nameof(AppointmentCards));
            }
        }

        #endregion

        #region Имя_пациента
        private string patientName;
        public string PatientName
        {
            get { return patientName; }
            set
            {
                patientName = value;
                OnPropertyChanged(nameof(PatientName));
            }
        }
        #endregion

        #region Номер_ОМС
        private string oms;
        public string OMS
        {
            get { return oms; }
            set
            {
                oms = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Кнопка_прикрепить_файлы
        private Visibility attachFilesButton;
        public Visibility AttachFilesButton
        {
            get { return attachFilesButton; }
            set
            {
                attachFilesButton = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Вижн_мейна
        private Visibility mainVisibility;
        public Visibility MainVisibility
        {
            get { return mainVisibility; }
            set
            {
                mainVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region РТБ_Исследования
        private bool researchCheckBox;
        public bool ResearchCheckBox
        {
            get { return researchCheckBox; }
            set
            {
                researchCheckBox = value;
                OnPropertyChanged();
            }
        }
        private Visibility researchRTBVisibility;
        public Visibility ResearchRTBVisibility
        {
            get { return researchRTBVisibility; }
            set
            {
                researchRTBVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region РТБ_Анализы
        private bool analysisCheckBox;
        public bool AnalysisCheckBox
        {
            get { return analysisCheckBox; }
            set
            {
                analysisCheckBox = value;
                OnPropertyChanged();
            }
        }
        private Visibility analysisRTBVisibility;
        public Visibility AnalysisRTBVisibility
        {
            get { return analysisRTBVisibility; }
            set
            {
                analysisRTBVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Время_сейчас_string
        private string dateNow = DateTime.Now.ToString("D");
        public string DateNow
        {
            get { return dateNow; }
            set
            {
                if (dateNow != value)
                {
                    dateNow = value;
                    OnPropertyChanged(nameof(DateNow));
                    UpdateCurrentAppointments(); // вызов метода для обновления списка
                }
            }
        }
        #endregion

        #region Кнопка_смена_темы_иконка
        private BitmapImage imageTheme = new BitmapImage(new Uri("../Images/NightTheme.png", UriKind.Relative));
        public BitmapImage ImageTheme
        {
            get { return imageTheme; }
            set
            {
                imageTheme = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Список_направлений
        private List<string> directions = new List<string>();
        public List<string> Directions
        {
            get { return directions; }
            set
            {
                directions = value;
                OnPropertyChanged(nameof(Directions));
            }
        }
        #endregion

        #endregion

        #region Команды
        public BindableCommand SwitchThemeCommand { get; set; }
        public BindableCommand AnalysisComboBoxCommand { get; set; }
        public BindableCommand ResearchComboBoxCommand { get; set; }
        public BindableCommand StartAppointmentCommand { get; set; }
        public BindableCommand CancelAppointmentCommand { get; set; }
        public BindableCommand CancelMissedAppointmentCommand { get; set; }
        public BindableCommand CompleteTheAppointmentCommand { get; set; }
        public BindableCommand LogoutCommand { get; set; }
        #endregion

        private void UpdateCurrentAppointments() //метод для обновления списка записей
        {
            var dateNow = DateOnly.FromDateTime(DateTime.Now);
            var timeNow = TimeOnly.FromDateTime(DateTime.Now);

            string DateNowAPI = DateTime.Parse(DateNow).ToString("yyyy-MM-dd");
            var json = ApiHelper.Get($"https://localhost:{port}/api/Appointments/WithPatientDetails?date={DateNowAPI}");
            var result = JsonConvert.DeserializeObject<ObservableCollection<Appointment>>(json);

            var currentAppointmentsTemp = new ObservableCollection<Appointment>();
            var missedAppointmentsTemp = new ObservableCollection<Appointment>();
            var completedAppointmentsTemp = new ObservableCollection<Appointment>();

            var combinedAppointments = new List<Appointment>();
            var appointmentCards = new ObservableCollection<UserControl>();

            foreach (var appointment in result)
            {
                if (appointment.StatusId == 1)
                {
                    var appointmentDate = appointment.AppointmentDate;
                    var appointmentTime = appointment.AppointmentTime;

                    if (appointmentDate < dateNow ||
                        (appointmentDate == dateNow && appointmentTime < timeNow))
                    {
                        MissedAppointment missedAppointment = new MissedAppointment(appointment);
                        missedAppointmentsTemp.Add(appointment);
                        appointmentCards.Add(missedAppointment);
                    }
                    else
                    {
                        CurrentAppointment currentAppointment = new CurrentAppointment(appointment);
                        currentAppointmentsTemp.Add(appointment);
                        appointmentCards.Add(currentAppointment);
                    }
                }
                else if (appointment.StatusId == 2)
                {
                    CompletedAppointment completedAppointment = new CompletedAppointment(appointment);
                    completedAppointmentsTemp.Add(appointment);
                    appointmentCards.Add(completedAppointment);
                }

                combinedAppointments.Add(appointment);
            }

            CurrentAppointments = currentAppointmentsTemp;
            MissedAppointments = missedAppointmentsTemp;
            CompletedAppointments = completedAppointmentsTemp;

            AllAppointments = new ObservableCollection<Appointment>(combinedAppointments.OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime));//сортировка записей по времени

            AppointmentCards = appointmentCards;
        }


        private void CompleteTheAppointment() //завершение приема
        {
            MainVisibility = Visibility.Collapsed;
            PatientName = string.Empty;
            OMS = string.Empty;
        }

        private void Logout()
        {
            MessageBox.Show("типо вышел из аккаунта");
        }

        private void CancelMissedAppointment(object parameter) //для отмены пропущенной записи
        {
            if (parameter is Appointment appointment)
            {
                /*MissedAppointments.Remove(appointment);*/
                MessageBox.Show($"OMS: {appointment.IdAppointment}");
            }
        }

        private void CancelAppointment(object parameter) //для отмены действующей записи
        {
            if (parameter is Appointment appointment)
            {
                /*CurrentAppointments.Remove(appointment);*/
                MessageBox.Show($"Запись для {appointment.IdAppointment}");
            }
        }

        private void ShowOMS(object parameter) //прием записи
        {
            if (parameter is Appointment appointment)
            {
                var json = ApiHelper.Get($"https://localhost:{port}/api/Directions/WithSpecialistDetails?_oms={appointment.OMS}");
                var result = JsonConvert.DeserializeObject<List<DirectionsWithSpecialistDetails>>(json);

                Directions = result.Select(item => item.Name).ToList();

                PatientName = $"Пациент: {appointment.OMS}";
                OMS = $"{appointment.OMS:0000 0000 0000 0000}";
                MainVisibility = Visibility.Visible;
            }
        }

        #region Визуал приколы
        private void ResearchRTB() //чекбокс исследования ртб
        {
            if (researchCheckBox == true)
            {
                ResearchRTBVisibility = Visibility.Visible;
                AttachFilesButton = Visibility.Visible;
            }
            else
            {
                ResearchRTBVisibility = Visibility.Collapsed;
                AttachFilesButton = Visibility.Collapsed;
            }
        }

        private void AnalysisRTB() //чекбокс анализы ртб
        {
            if (analysisCheckBox == true)
            {
                AnalysisRTBVisibility = Visibility.Visible;
            }
            else
            {
                AnalysisRTBVisibility = Visibility.Collapsed;
            }
        }

        private void SwitchTheme() //кнопка сменить тему в титлбаре
        {
            if (isDarkTheme)
            {
                Application.Current.Resources.MergedDictionaries[2] = new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/ThemeLibrary;component/Themes/LightTheme.xaml")
                };
                ImageTheme = new BitmapImage(new Uri("../Images/NightTheme.png", UriKind.Relative));
                isDarkTheme = false;
            }
            else if (!isDarkTheme)
            {
                Application.Current.Resources.MergedDictionaries[2] = new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/ThemeLibrary;component/Themes/DarkTheme.xaml")
                };
                ImageTheme = new BitmapImage(new Uri("../Images/LightTheme.png", UriKind.Relative));
                isDarkTheme = true;
            }
        }
        #endregion
    }
}
