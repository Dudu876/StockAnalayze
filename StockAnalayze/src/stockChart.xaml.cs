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
using Visifire.Charts;


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

        //private void ViewLoaded(object sender, RoutedEventArgs e)
        //{
        //    foreach (var stockVal in stock.values)
        //    {
        //        this.closeGraph.Margin = new Thickness(1);
        //        this.closeGraph.LineMargin = new Thickness(1);

        //        this.openGraph.Margin = new Thickness(1);
        //        this.openGraph.LineMargin = new Thickness(1);

        //        this.highGraph.Margin = new Thickness(1);
        //        this.highGraph.LineMargin = new Thickness(1);

        //        this.lowGraph.Margin = new Thickness(1);
        //        this.lowGraph.LineMargin = new Thickness(1);


        //        DateTime dt = Convert.ToDateTime(stockVal.Date);
        //        if (stockVal.Open.HasValue){
        //            this.openGraph.AddTimeValue(stockVal.Open.Value, dt);
        //        }
        //        if (stockVal.High.HasValue)
        //        {
        //            this.highGraph.AddTimeValue(stockVal.High.Value, dt);
        //        } 
        //        if (stockVal.Close.HasValue)
        //        {
        //            this.closeGraph.AddTimeValue(stockVal.Close.Value, dt);
        //        } 
        //        if (stockVal.Low.HasValue)
        //        {
        //            this.lowGraph.AddTimeValue(stockVal.Low.Value, dt);
        //        }
        //    }
        //}

        private void ViewLoaded(object sender, RoutedEventArgs e){

            //Children.Add(CreateChart(stockName, rates)
        }
        
        public Chart CreateChart()
        {
            Visifire.Charts.Chart visiChart = new Visifire.Charts.Chart();
            Visifire.Charts.Title title = new Visifire.Charts.Title();

            title.Text = stock.Name;
            visiChart.Titles.Add(title);

            visiChart.Width = 200;
            visiChart.Height = 200;

            Visifire.Charts.DataSeries dataSeriesClose = new Visifire.Charts.DataSeries();
            Visifire.Charts.DataSeries dataSeriesOpen = new Visifire.Charts.DataSeries();
            Visifire.Charts.DataSeries dataSeriesLow = new Visifire.Charts.DataSeries();
            Visifire.Charts.DataSeries dataSeriesHigh = new Visifire.Charts.DataSeries();


            Visifire.Charts.DataPoint dataPointClose;
            Visifire.Charts.DataPoint dataPointOpen;
            Visifire.Charts.DataPoint dataPointLow;
            Visifire.Charts.DataPoint dataPointHigh;

            foreach (var stockVal in stock.values)
            {
                if (stockVal.Open.HasValue)
                {
                    dataPointOpen = new DataPoint();
                    dataPointOpen.YValue = stockVal.Open.Value;
                    dataSeriesOpen.DataPoints.Add(dataPointOpen);
                }
                if (stockVal.High.HasValue)
                {
                    dataPointHigh = new DataPoint();
                    dataPointHigh.YValue = stockVal.High.Value;
                    dataSeriesHigh.DataPoints.Add(dataPointHigh);

                }
                if (stockVal.Close.HasValue)
                {
                    dataPointClose = new DataPoint();
                    dataPointClose.YValue = stockVal.Close.Value;
                    dataSeriesClose.DataPoints.Add(dataPointClose);

                }
                if (stockVal.Low.HasValue)
                {
                    dataPointLow = new DataPoint();
                    dataPointLow.YValue = stockVal.Low.Value;
                    dataSeriesLow.DataPoints.Add(dataPointLow);

                }
            }

            dataSeriesLow.RenderAs = RenderAs.Line;
            visiChart.Series.Add(dataSeriesLow);
            dataSeriesHigh.RenderAs = RenderAs.Line;
            visiChart.Series.Add(dataSeriesHigh);
            dataSeriesOpen.RenderAs = RenderAs.Line;
            visiChart.Series.Add(dataSeriesOpen);
            dataSeriesClose.RenderAs = RenderAs.Line;
            visiChart.Series.Add(dataSeriesClose);

            return visiChart;
        }
    }
}
