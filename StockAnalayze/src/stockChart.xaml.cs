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
using StockAnalayze.Models;

namespace StockAnalayze
{
    /// <summary>
    /// Interaction logic for stockChart.xaml
    /// </summary>
    public partial class StockChart : UserControl
    {
        private readonly Stock stock;

        public StockChart(Stock stock)
        {
            this.stock = stock;
            InitializeComponent();
            this.stockName.Text = stock.Name;
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var stockVal in stock.values)
            {

                this.closeGraph.Margin = new Thickness(1);
                this.closeGraph.LineMargin = new Thickness(1);

                this.openGraph.Margin = new Thickness(1);
                this.openGraph.LineMargin = new Thickness(1);

                this.highGraph.Margin = new Thickness(1);
                this.highGraph.LineMargin = new Thickness(1);

                this.lowGraph.Margin = new Thickness(1);
                this.lowGraph.LineMargin = new Thickness(1);


                DateTime dt = Convert.ToDateTime(stockVal.Date);
                if (stockVal.Open.HasValue){
                    this.openGraph.AddTimeValue(stockVal.Open.Value, dt);
                }
                if (stockVal.High.HasValue)
                {
                    this.highGraph.AddTimeValue(stockVal.High.Value, dt);
                } 
                if (stockVal.Close.HasValue)
                {
                    this.closeGraph.AddTimeValue(stockVal.Close.Value, dt);
                } 
                if (stockVal.Low.HasValue)
                {
                    this.lowGraph.AddTimeValue(stockVal.Low.Value, dt);
                }
            }
        }
    }
}
