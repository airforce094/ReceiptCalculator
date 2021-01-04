using ReceiptCalculator.MVVM;
using System.Windows;

namespace ReceiptCalculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReceiptVM vm;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ReceiptVM();
            vm = DataContext as ReceiptVM;
        }

        private void OnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, vm.TotalCount);
            MessageBox.Show("已复制");
        }
    }
}
