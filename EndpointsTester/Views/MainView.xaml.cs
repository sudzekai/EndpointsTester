using EndpointsTester.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace EndpointsTester.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void BodyTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Tab)
                return;
            e.Handled = true;

            if (sender is not TextBox textBox) return;

            int caretIndex = textBox.CaretIndex; 

            textBox.Text = textBox.Text.Insert(caretIndex, "    ");

            textBox.CaretIndex = caretIndex + 4;
        }
    }
}
