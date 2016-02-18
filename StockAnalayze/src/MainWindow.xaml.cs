using StockAnalayze.Managers;
using StockAnalayze.src;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace StockAnalayze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void analyze()
        {
            StatusWindow win2 = new StatusWindow();
            win2.Show();

            InputParams i = new InputParams();
            i.numOfStocks = Int32.Parse(this.numOfStocks.Text);
            i.daysAgo = Int32.Parse(this.daysAgo.Text);
            i.clusters = Int32.Parse(this.clusters.Text);
            i.open = (bool)open.IsChecked;
            i.close = (bool)close.IsChecked;
            i.high = (bool)high.IsChecked;
            i.low = (bool)low.IsChecked;

            var im = new Managers.InputManager(i);
            await Task.Run(() => im.GetInputReady());

            var fm = new FilesManager(i.clusters);
            //await Task.Run(() => fm.TestRun());
            await Task.Run(() => fm.Start());
            win2.Close();

            var om = new OutputManager(im.allStocksData);
            ResultsWindow graphs = new ResultsWindow(om.clusters);
            graphs.Show();
        }

        private string checkFeilds()
        {
            if (this.numOfStocks.Text == string.Empty)
            {
                return "The number of stocks must be filled";
            }
            else if(this.daysAgo.Text == string.Empty)
            {
                return "The amount of days must be filled";
            }
            else if (this.clusters.Text == string.Empty)
            {
                return "Clusters number is not filled";
            }
            else if (!this.close.IsChecked.Value && !this.high.IsChecked.Value && 
                    !this.low.IsChecked.Value && !this.open.IsChecked.Value)
            {
                return "One of the checkboxes MUST be checkd";
            }

            return string.Empty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string msg = checkFeilds();
            if (msg != string.Empty)
            {
                MessageBox.Show(msg);
            }
            else
            {
                analyze();
            }
        }

    }
}
