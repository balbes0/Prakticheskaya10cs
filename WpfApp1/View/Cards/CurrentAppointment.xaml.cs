using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.ViewModel;

namespace WpfApp1.View.Cards
{
    public partial class CurrentAppointment : UserControl
    {
        public CurrentAppointment(Appointment appointment)
        {
            InitializeComponent();
            DataContext = appointment;
        }
    }
}
