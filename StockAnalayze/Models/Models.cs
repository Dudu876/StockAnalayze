using StockAnalayze.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalayze.Models
{
    public class Cluster
    {
        public int Id { get; set; }
        public IList<Stock> Stocks { get; set; }
    }

    public class Stock
    {
        public Stock(string name, Dictionary<DateTime, StockData> datedValues)
        {
            Name = name;
            DatedValues = datedValues;
        }

        public string Name { get; set; }
        public IDictionary<DateTime, StockData> DatedValues { get; set; }
    }

    public class StockData
    {
        public double? Open { get; set; }
        public double? Close { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public string Date { get; set; }

        public StockData Normalize(InputParams features, StockData minData, StockData maxData)
        {
            var normalizedData = new StockData
            {
                Date = Date
            };

            if (features.open)
            {
                normalizedData.Open = NormalizeData(Open.Value, minData.Open.Value, maxData.Open.Value);
            }

            if (features.close)
            {
                normalizedData.Close = NormalizeData(Close.Value, minData.Close.Value, maxData.Close.Value);
            }

            if (features.high)
            {
                normalizedData.High = NormalizeData(High.Value, minData.High.Value, maxData.High.Value);
            }

            if (features.low)
            {
                normalizedData.Low = NormalizeData(Low.Value, minData.Low.Value, maxData.Low.Value);
            }

            return normalizedData;
        }

        private static double NormalizeData(double value, double min, double max, int precision = 2)
        {
            var minMaxNormlValue = (value - min) / (max - min) * 100;
            return Math.Round(minMaxNormlValue, 2);
        }
    }
}
