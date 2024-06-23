using System.Windows.Media.Imaging;
using System.Windows;
using WpfApp1.ViewModel.Helpers;
using System.Collections.ObjectModel;
using WpfApp1.Models;
using Newtonsoft.Json;
using System.Data;
using WpfApp1.View.Cards;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;
using Microsoft.Win32;

namespace WpfApp1.ViewModel
{
    class MainWindowViewModel : BindingHelper
    {
        private string port = "7107";
        private bool isDarkTheme = false;
        private Appointment currentAppointment;
        private DoctorWithSpecialityName currentDoctor;
        private readonly string defaultAnalysisRTBText;
        private readonly string defaultResearchRTBText;
        private readonly string defaultAppointmentRTBText;
        private RichTextBox analysisRTB;
        private RichTextBox researchRTB;
        private RichTextBox appointmentRTB;
        byte[] fileBytes = null;

        public MainWindowViewModel(RichTextBox analysisRTB, RichTextBox researchRTB, RichTextBox appointmentRTB, int id_doctor)
        {
            GetDoctorDetailsByID(id_doctor);
            titleName = $"ЕМИАС — {currentDoctor.Surname} {currentDoctor.Name} {currentDoctor.Patronymic}";
            specialityName = currentDoctor.SpecialityName;
            doctorInitials = $"{currentDoctor.Surname} {currentDoctor.Name[0]}. {currentDoctor.Patronymic[0]}.";
            this.analysisRTB = analysisRTB;
            this.researchRTB = researchRTB;
            this.appointmentRTB = appointmentRTB;

            analysisRTB = researchRTB;

            SwitchThemeCommand = new BindableCommand(_ => SwitchTheme()); //изменить тему приложения
            AnalysisComboBoxCommand = new BindableCommand(_ => AnalysisRTB()); //показать/убрать анализы
            ResearchComboBoxCommand = new BindableCommand(_ => ResearchRTB()); //показать/убрать исследования
            StartAppointmentCommand = new BindableCommand(ShowOMS); //для открытия карточки пациента
            CancelAppointmentCommand = new BindableCommand(CancelAppointment); //кнопка отменить запись
            CancelMissedAppointmentCommand = new BindableCommand(CancelMissedAppointment); //отменить пропущенную запись
            CompleteTheAppointmentCommand = new BindableCommand(_ => CompleteTheAppointment()); //кнопка завершить прием
            DeleteDirectionCommand = new BindableCommand(DeleteDirection); //удалить направление пациента
            AddDirectionCommand = new BindableCommand(_ => AddDirection());
            AttachFileCommand = new BindableCommand(_ => AttachFile());
            LogoutCommand = new BindableCommand(_ => Logout()); //кнопка выйти из аккаунта
            analysisRTBVisibility = Visibility.Collapsed; //РТБ с анализами по дефолту скрыта
            researchRTBVisibility = Visibility.Collapsed; //РТБ с исследованиями по дефолту скрыта
            AttachFilesButton = Visibility.Collapsed; //Кнопка прикрепить файлы по дефолту скрыта
            MainVisibility = Visibility.Hidden;

            UpdateCurrentAppointments();
            GetSpecialitiesList();
        }

        #region Свойства
        #region Список_специальностей
        private List<Speciality> specialitiesList;
        public List<Speciality> SpecialitiesList
        {
            get { return specialitiesList; }
            set
            {
                specialitiesList = value;
                OnPropertyChanged(nameof(SpecialitiesList));
            }
        }

        #region Выбранное_направление
        private Speciality selectedSpecialityId;
        public Speciality SelectedSpecialityId
        {
            get { return selectedSpecialityId; }
            set
            {
                selectedSpecialityId = value;
                OnPropertyChanged(nameof(SelectedSpecialityId));
            }
        }
        #endregion

        #endregion

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

        #region Кнопка прикрепить файлы
        private string attachFileButton = "Прикрепить дополнительные файлы";
        public string AttachFileButton
        {
            get { return attachFileButton; }
            set
            {
                attachFileButton = value;
                OnPropertyChanged(nameof(AttachFileButton));
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

        private string _dateNow = DateTime.Now.ToString("D");
        public string _DateNow
        {
            get { return _dateNow; }
            set
            {
                if (_dateNow != value)
                {
                    _dateNow = value;
                    OnPropertyChanged();
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
        private ObservableCollection<DirectionsWithSpecialistDetails> directions;
        public ObservableCollection<DirectionsWithSpecialistDetails> Directions
        {
            get { return directions; }
            set
            {
                directions = value;
                OnPropertyChanged(nameof(Directions));
            }
        }
        #endregion

        #region Название анализа и исследования
        private string analysisName;
        public string AnalysisName
        {
            get { return analysisName; }
            set
            {
                analysisName = value;
                OnPropertyChanged();
            }
        }        
        
        private string researchName;
        public string ResearchName
        {
            get { return researchName; }
            set
            {
                researchName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Диагноз
        private string diagnosis;
        public string Diagnosis
        {
            get { return diagnosis; }
            set
            {
                if (diagnosis != value)
                {
                    diagnosis = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #region Жалобы
        private string complaints;
        public string Complaints
        {
            get { return complaints; }
            set
            {
                if (complaints != value)
                {
                    complaints = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion        
        #region Общий осмотр
        private string examination;
        public string Examination
        {
            get { return examination; }
            set
            {
                if (examination != value)
                {
                    examination = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion        
        #region Рекомендации
        private string recommendations;
        public string Recommendations
        {
            get { return recommendations; }
            set
            {
                if (recommendations != value)
                {
                    recommendations = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion        
        #region Дополнения к диагнозу
        private string additionsDiagnosis;
        public string AdditionsDiagnosis
        {
            get { return additionsDiagnosis; }
            set
            {
                if (additionsDiagnosis != value)
                {
                    additionsDiagnosis = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion        
        #region Инициалы доктора
        private string doctorInitials;
        public string DoctorInitials
        {
            get { return doctorInitials; }
            set
            {
                if (doctorInitials != value)
                {
                    doctorInitials = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #region Специальность врача
        private string specialityName;
        public string SpecialityName
        {
            get { return specialityName; }
            set
            {
                if (specialityName != value)
                {
                    specialityName = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Название титла
        private string titleName;
        public string TitleName
        {
            get { return titleName; }
            set
            {
                if (titleName != value)
                {
                    titleName = value;
                    OnPropertyChanged();
                }
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
        public BindableCommand DeleteDirectionCommand { get; set; }
        public BindableCommand AddDirectionCommand { get; set; }
        public BindableCommand AttachFileCommand { get; set; }
        #endregion

        private void GetDoctorDetailsByID(int id_doctor)
        {
            var json = ApiHelper.Get($"https://localhost:{port}/api/Doctors/WithSpecialityName?id={id_doctor}");
            var result = JsonConvert.DeserializeObject<DoctorWithSpecialityName>(json);
            currentDoctor = result;
        }

        private void GetSpecialitiesList() //получения списка специалностей врачей
        {
            var json = ApiHelper.Get($"https://localhost:{port}/api/Specialities");
            var result = JsonConvert.DeserializeObject<List<Speciality>>(json);
            SpecialitiesList = new List<Speciality>(result);
        }

        private void AttachFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        fileBytes = binaryReader.ReadBytes((int)fileStream.Length);
                        AttachFileButton = $"Прикреплен файл: {Path.GetFileName(openFileDialog.FileName)}";
                    }
                }
            }
        }

        private void AddDirection()
        {
            Direction direction = new Direction
            {
                Oms = currentAppointment.OMS,
                SpecialityId = SelectedSpecialityId.IdSpeciality
            };

            bool flag = true;
            foreach (var item in Directions)
            {
                if (item.SpecialityId == SelectedSpecialityId.IdSpeciality)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                string json = JsonConvert.SerializeObject(direction);
                ApiHelper.Post($"https://localhost:{port}/api/Directions", json);
                GetDirections();
            }
            else
            {
                MessageBox.Show("Такое направление уже имеется!");
            }
        }

        private void DeleteDirection(object parameter)
        {
            if (parameter is int idDirection)
            {
                ApiHelper.Delete($"https://localhost:{port}/api/Directions/{idDirection}");
                var directionToDelete = Directions.FirstOrDefault(d => d.IdDirection == idDirection);
                if (directionToDelete != null)
                {
                    Directions.Remove(directionToDelete);
                }
                MessageBox.Show($"Направление удалено!");
            }
        }

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

        private void SaveAnalysDocument()
        {
            TextRange textRange = new TextRange(analysisRTB.Document.ContentStart, analysisRTB.Document.ContentEnd);
            string rtfText;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                textRange.Save(memoryStream, DataFormats.Rtf);
                rtfText = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            AnalysDocument analysDocument = new AnalysDocument
            {
                AppointmentId = currentAppointment.IdAppointment,
                Rtf = rtfText
            };
            string json = JsonConvert.SerializeObject(analysDocument);
            ApiHelper.Post($"https://localhost:{port}/api/AnalysDocuments", json);
        }

        private void SaveAppointmentDocument()
        {
            TextRange textRange = new TextRange(appointmentRTB.Document.ContentStart, appointmentRTB.Document.ContentEnd);
            string rtfText;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                textRange.Save(memoryStream, DataFormats.Rtf);
                rtfText = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            AppointmentDocument appointmentDocument = new AppointmentDocument
            {
                AppointmentId = currentAppointment.IdAppointment,
                Rtf = rtfText
            };
            string json = JsonConvert.SerializeObject(appointmentDocument);
            ApiHelper.Post($"https://localhost:{port}/api/AppointmentDocuments", json);
        }

        private void SaveResearchDocument()
        {
            TextRange textRange = new TextRange(researchRTB.Document.ContentStart, researchRTB.Document.ContentEnd);
            string rtfText;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                textRange.Save(memoryStream, DataFormats.Rtf);
                rtfText = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            ResearchDocument researchDocument = new ResearchDocument();
            researchDocument.AppointmentId = currentAppointment.IdAppointment;
            researchDocument.Rtf = rtfText;
            if (fileBytes == null)
            {
                researchDocument.Attachment = null;
            }
            else
            {
                researchDocument.Attachment = fileBytes;
            }

            string json = JsonConvert.SerializeObject(researchDocument);
            ApiHelper.Post($"https://localhost:{port}/api/ResearchDocuments", json);
        }

        private void ClearingFields() //очистка всех полек
        {
            currentAppointment = null;
            MainVisibility = Visibility.Collapsed;
            Directions = null;
            PatientName = string.Empty;
            OMS = string.Empty;
            byte[] fileBytes = null;
            attachFileButton = "Прикрепить дополнительные файлы";
            Complaints = string.Empty;
            Examination = string.Empty;
            Diagnosis = string.Empty;
            AdditionsDiagnosis = string.Empty;
            Recommendations = string.Empty;
            AnalysisName = string.Empty;
            ResearchName = string.Empty;
        }

        private void CompleteTheAppointment() //завершение приема
        {
            if (currentAppointment != null)
            {
                /*ApiHelper.PostWithoutJson($"https://localhost:{port}/api/Appointments/Complete/{currentAppointment.IdAppointment}");//меняем статус записи на завершен.

                if (AnalysisCheckBox == true)
                {
                    SaveAnalysDocument();
                }

                if (ResearchCheckBox == true)
                {
                    SaveResearchDocument();
                }

                SaveAppointmentDocument();

                MessageBox.Show("Запись завершена!");

                ClearingFields();
                UpdateCurrentAppointments();*/
            }
        }
        
        private void Logout() //кнопка выйти из аккаунта
        {
            MessageBox.Show("типо вышел из аккаунта");
        }

        private void CancelMissedAppointment(object parameter) //для отмены пропущенной записи
        {
            if (parameter is Appointment appointment)
            {
                ApiHelper.PostWithoutJson($"https://localhost:{port}/api/Appointments/Cancel/{appointment.IdAppointment}");
                MessageBox.Show("Запись отменена!");
                UpdateCurrentAppointments();
            }
        }

        private void CancelAppointment(object parameter) //для отмены действующей записи
        {
            if (parameter is Appointment appointment)
            {
                ApiHelper.PostWithoutJson($"https://localhost:{port}/api/Appointments/Cancel/{appointment.IdAppointment}");
                MessageBox.Show("Запись отменена!");
                UpdateCurrentAppointments();
            }
        }

        private void GetDirections() //Получаем список направлений пациента
        {
            var json = ApiHelper.Get($"https://localhost:{port}/api/Directions/WithSpecialistDetails?_oms={currentAppointment.OMS}");
            var result = JsonConvert.DeserializeObject<List<DirectionsWithSpecialistDetails>>(json);
            Directions = new ObservableCollection<DirectionsWithSpecialistDetails>(result);
        }

        private void ShowOMS(object parameter) //прием записи
        {
            if (parameter is Appointment appointment)
            {
                if (currentAppointment == null)
                {
                    currentAppointment = appointment;
                    GetDirections();
                    PatientName = $"Пациент: {currentAppointment.FirstName} {currentAppointment.LastName} {currentAppointment.Patronymic}";
                    OMS = $"{currentAppointment.OMS:0000 0000 0000 0000}";
                    MainVisibility = Visibility.Visible;
                }
                else if (currentAppointment != null)
                {
                    MessageBox.Show("Сначала завершите прием!");
                }
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
