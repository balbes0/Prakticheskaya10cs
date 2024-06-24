using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.View.Cards
{
    public partial class AnalysisRichTextBoxView : UserControl
    {
        public AnalysisRichTextBoxView(AnalysisRichTextBox analysisRichTextBox)
        {
            InitializeComponent();
            DataContext = analysisRichTextBox;
        }
    }
}
