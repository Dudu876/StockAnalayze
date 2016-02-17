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

        private async void Button_Click(object sender, RoutedEventArgs e)
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
            //await Task.Run(() => im.GetInputReady());

            var fm = new FilesManager();
            //var s = await fm.TestRun();
            await Task.Run(() => fm.TestRun());
            //await Task.Run(() => fm.Start());
            //fm.PrepareRemote();
            //fm.RunHadoop();
            //fm.RetriveOutput();
            win2.Close();
        }
    }
}
