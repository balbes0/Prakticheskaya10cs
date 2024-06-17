using System.Windows.Media.Imaging;
using System.Windows;
using WpfApp1.ViewModel.Helpers;
using System.Collections.ObjectModel;
using WpfApp1.Models;

namespace WpfApp1.ViewModel
{
    class MainWindowViewModel : BindingHelper
    {
        private bool isDarkTheme = false;
        public MainWindowViewModel()
        {
            SwitchThemeCommand = new BindableCommand(_ => SwitchTheme()); //направления пример
            AnalysisComboBoxCommand = new BindableCommand(_ => AnalysisRTB()); //направления пример
            ResearchComboBoxCommand = new BindableCommand(_ => ResearchRTB());
            StartAppointmentCommand = new BindableCommand(ShowOMS);
            CancelAppointmentCommand = new BindableCommand(CancelAppointment);
            CancelMissedAppointmentCommand = new BindableCommand(CancelMissedAppointment);
            CompleteTheAppointmentCommand = new BindableCommand(_ => CompleteTheAppointment());
            analysisRTBVisibility = Visibility.Collapsed; //РТБ с анализами по дефолту скрыта
            researchRTBVisibility = Visibility.Collapsed; //РТБ с исследованиями по дефолту скрыта
            AttachFilesButton = Visibility.Collapsed; //Кнопка прикрепить файлы по дефолту скрыта
            MainVisibility = Visibility.Hidden;

            directions = new List<string>
            {
                "Офтальмолог",
                "Кардиолог",
                "Невролог"
            };

            CurrentAppointments = new ObservableCollection<Appointment>
            {
                new Appointment { OMS = 7777000100000000, FullName = "Иванова Ирина Ивановна", AppointmentTime = new TimeOnly(12, 40), AppointmentDate = new DateOnly(2024, 6, 17) },
                new Appointment { OMS = 7777000200000000, FullName = "Петров Сергей Александрович", AppointmentTime = new TimeOnly(11, 00), AppointmentDate = new DateOnly(2024, 6, 17) },
                new Appointment { OMS = 7777000300000000, FullName = "Сидорова Мария Ивановна", AppointmentTime = new TimeOnly(11, 20), AppointmentDate = new DateOnly(2024, 6, 17)}
            };

            MissedAppointments = new ObservableCollection<Appointment>
            {
                new Appointment { OMS = 7777000100000000, FullName = "Иванова Ирина Ивановна", AppointmentTime = new TimeOnly(9, 40), AppointmentDate = new DateOnly(2024, 6, 17) },
                new Appointment { OMS = 7777000200000000, FullName = "Петров Сергей Александрович", AppointmentTime = new TimeOnly(10, 00), AppointmentDate = new DateOnly(2024, 6, 17) },
                new Appointment { OMS = 7777000300000000, FullName = "Сидорова Мария Ивановна", AppointmentTime = new TimeOnly(10, 20), AppointmentDate = new DateOnly(2024, 6, 17)}
            };
        }


        #region Свойства
        #region Список_записей
        public ObservableCollection<Appointment> CurrentAppointments { get; set; }
        public ObservableCollection<Appointment> MissedAppointments { get; set; }
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
                dateNow = value;
                OnPropertyChanged();
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
        private List<string> directions;
        public List<string> Directions
        {
            get { return directions; }
            set
            {
                directions = value;
                OnPropertyChanged();
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
        #endregion

        private void CompleteTheAppointment() //завершение приема
        {
            
            MainVisibility = Visibility.Collapsed;
            PatientName = string.Empty;
            OMS = string.Empty;
        }

        private void CancelMissedAppointment(object parameter) //для отмены пропущенной записи
        {
            if (parameter is Appointment appointment)
            {
                MissedAppointments.Remove(appointment);
                MessageBox.Show($"Запись для {appointment.FullName} отменена.", "Отмена записи", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelAppointment(object parameter) //для отмены действующей записи
        {
            if (parameter is Appointment appointment)
            {
                CurrentAppointments.Remove(appointment);
                MessageBox.Show($"Запись для {appointment.FullName} отменена.", "Отмена записи", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowOMS(object parameter) //прием записи
        {
            if (parameter is Appointment appointment)
            {
                PatientName = $"Пациент: {appointment.FullName}";
                OMS = $"{appointment.OMS:0000 0000 0000 0000}";
                MainVisibility = Visibility.Visible;
            }
        }

        private void ResearchRTB()
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

        private void AnalysisRTB()
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

        private void SwitchTheme()
        {
            if (isDarkTheme)
            {
                Application.Current.Resources.MergedDictionaries[2] = new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/ThemeLibrary;component/Themes/DarkTheme.xaml")
                };
                ImageTheme = new BitmapImage(new Uri("../Images/LightTheme.png", UriKind.Relative));
                isDarkTheme = false;
            }
            else
            {
                Application.Current.Resources.MergedDictionaries[2] = new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/ThemeLibrary;component/Themes/LightTheme.xaml")
                };
                isDarkTheme = true;
            }
        }
    }
}
