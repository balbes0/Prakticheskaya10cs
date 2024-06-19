using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.ViewModel;

namespace WpfApp1.View.Cards
{
    public partial class CompletedAppointment : UserControl
    {
        public CompletedAppointment(Appointment appointment)
        {
            InitializeComponent();
            DataContext = appointment;
        }
    }
}