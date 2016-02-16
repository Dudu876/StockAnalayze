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
    /// Interaction logic for ClusterView.xaml
    /// </summary>
    public partial class ClusterView : UserControl
    {
        public ClusterView(Cluster cluster)
        {
            InitializeComponent();

            grp.Header = "Cluster " + cluster.Id;

            foreach (var stock in cluster.Stocks)
            {
                this.listCharts.Children.Add(CreateChart(stock));
            }

        }

        
        public Chart CreateChart(Stock stock)
        {
            Visifire.Charts.Chart visiChart = new Visifire.Charts.Chart();
            Visifire.Charts.Title title = new Visifire.Charts.Title();

            title.Text = stock.Name;
            visiChart.Titles.Add(title);

            visiChart.Width = 300;
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

            if (dataSeriesLow.DataPoints.Count > 0)
            {
                dataSeriesLow.RenderAs = RenderAs.Line;
                dataSeriesLow.Name = "Low";
                visiChart.Series.Add(dataSeriesLow);
            }
            if (dataSeriesHigh.DataPoints.Count > 0)
            {
                dataSeriesHigh.RenderAs = RenderAs.Line;
                dataSeriesHigh.Name = "High";
                visiChart.Series.Add(dataSeriesHigh);
            }
            if (dataSeriesOpen.DataPoints.Count > 0)
            {
                dataSeriesOpen.RenderAs = RenderAs.Line;
                dataSeriesOpen.Name = "Open";
                visiChart.Series.Add(dataSeriesOpen);
            }
            if (dataSeriesClose.DataPoints.Count > 0)
            {
                dataSeriesClose.RenderAs = RenderAs.Line;
                dataSeriesClose.Name = "Close";
                visiChart.Series.Add(dataSeriesClose);
            }
            return visiChart;
        }
    }
    
}
