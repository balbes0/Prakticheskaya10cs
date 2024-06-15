using System.Windows.Media.Imaging;
using System.Windows;
using WpfApp1.ViewModel.Helpers;

namespace WpfApp1.ViewModel
{
    class MainWindowViewModel : BindingHelper
    {
        private bool isDarkTheme = false;
        public MainWindowViewModel()
        {
            SwitchThemeCommand = new BindableCommand(_ => SwitchTheme());
            directions = new List<string>
            {
                "Офтальмолог",
                "Кардиолог",
                "Невролог"
            };
        }

        #region Свойства
        private DateTime dateNow = DateTime.Now;
        public DateTime DateNow
        {
            get { return dateNow; }
            set
            {
                dateNow = value;
                OnPropertyChanged();
            }
        }

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

        #region Команды
        public BindableCommand SwitchThemeCommand { get; set; }
        #endregion

        private void SwitchTheme()
        {
            // Применяем противоположную тему
            if (isDarkTheme)
            {
                ApplyLightTheme(); // Переключаем на светлую тему
                isDarkTheme = false;
            }
            else
            {
                ApplyDarkTheme(); // Переключаем на тёмную тему
                isDarkTheme = true;
            }
        }

        // Для переключения на темную тему
        private void ApplyDarkTheme()
        {
            Application.Current.Resources.MergedDictionaries[2] = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/ThemeLibrary;component/Themes/DarkTheme.xaml")
            };
            ImageTheme = new BitmapImage(new Uri("../Images/LightTheme.png", UriKind.Relative));
        }

        // Для переключения на светлую тему
        private void ApplyLightTheme()
        {
            Application.Current.Resources.MergedDictionaries[2] = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/ThemeLibrary;component/Themes/LightTheme.xaml")
            };
            ImageTheme = new BitmapImage(new Uri("../Images/NightTheme.png", UriKind.Relative));
        }
    }
}
