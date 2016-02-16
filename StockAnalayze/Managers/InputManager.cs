using StockAnalayze.Common;
using StockAnalayze.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalayze.Managers
{
    class InputManager
    {
        private InputParams _inputParams;
        public IDictionary<string,Stock> allStocksData { get; set; }
        public InputManager(InputParams i)
        {
            _inputParams = i;
            allStocksData = new Dictionary<string, Stock>();
        }

        public void GetInputReady()
        {
            var stocksFiles = getStockFiles();
            FileUtils.ProcessStockFiles(stocksFiles);
        }

        private IEnumerable<string> getStockFiles()
        {
            var stockSymbols =
                GetRandomStockSymbols((int)Math.Ceiling(_inputParams.numOfStocks * Consts.SYMBOL_NUMBER_BUFFER_FACTOR)).ToList();

            var stockFiles = new List<string>();

            for (var downloadCount = 0;
                 downloadCount < stockSymbols.Count && stockFiles.Count < _inputParams.numOfStocks;
                 downloadCount++)
            {
                var stockSymbol = stockSymbols[downloadCount];
                var downloadUrl = string.Format(Consts.YAHOO_FINANCE_URL_FORMAT, stockSymbol);

                var symbolFileName = string.Format(Consts.STOCKS_DATA_PATH_BASE + @"\{0}.csv", stockSymbol);

                using (var webClient = new WebClient())
                {
                    try
                    {
                        webClient.DownloadFile(downloadUrl, symbolFileName);
                    }
                    catch (Exception e)
                    {
                        // TODO: Log SYMBOL NOT FOUND
                        continue;
                    }
                }

                var wasTaken = NormalizeYahooStockFile(symbolFileName, stockSymbol,
                                                    Consts.LINES_TO_SKIP_IN_STOCK_FILE, _inputParams.daysAgo);
                if (wasTaken)
                {
                    stockFiles.Add(symbolFileName);
                }
            }

            return stockFiles;
        }

        private IEnumerable<string> GetRandomStockSymbols(int numberOfStocks)
        {
            string symbols;
            using (var webClient = new WebClient())
            {
                symbols = webClient.DownloadString(Consts.NASDAQ_SYMBOLS_URL);
            }

            var stocks = symbols.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .Skip(1).Shuffle(new Random()).Take(numberOfStocks)
                                .Select(stockLine => stockLine.Split(Consts.NASDAQ_STOCK_SYMBOL_SEPERATOR)[0]);

            return stocks;
        }

        private bool NormalizeYahooStockFile(string filePath, string stockSymbol, int linesToSkipInStockFile,
                                             int wantedLines)
        {
            var allFileLines = File.ReadAllLines(filePath).Skip(linesToSkipInStockFile).ToList();
            allFileLines = allFileLines.Take(Math.Min(wantedLines, allFileLines.Count)).ToList();
            if (allFileLines.Count < wantedLines)
            {
                return false;
            }

            var fileFeatures = allFileLines.Select(line =>
            {
                var stringFeatures = line.Split(',');
                var stockData = new StockData
                {
                    Date = stringFeatures[0]
                };

                if (_inputParams.open)
                {
                    stockData.Open = double.Parse(stringFeatures[1]);
                }

                if (_inputParams.high)
                {
                    stockData.High = double.Parse(stringFeatures[2]);
                }

                if (_inputParams.low)
                {
                    stockData.Low = double.Parse(stringFeatures[3]);
                }

                if (_inputParams.close)
                {
                    stockData.Close = double.Parse(stringFeatures[4]);
                }

                return stockData;
            }).ToList();

            //
            allStocksData.Add(stockSymbol,new Stock(stockSymbol,fileFeatures));

            var minData = new StockData();
            var maxData = new StockData();

            if (_inputParams.open)
            {
                minData.Open = fileFeatures.Min(stockData => stockData.Open);
                maxData.Open = fileFeatures.Max(stockData => stockData.Open);
            }

            if (_inputParams.high)
            {
                minData.High = fileFeatures.Min(stockData => stockData.High);
                maxData.High = fileFeatures.Max(stockData => stockData.High);
            }

            if (_inputParams.low)
            {
                minData.Low = fileFeatures.Min(stockData => stockData.Low);
                maxData.Low = fileFeatures.Max(stockData => stockData.Low);
            }

            if (_inputParams.close)
            {
                minData.Close = fileFeatures.Min(stockData => stockData.Close);
                maxData.Close = fileFeatures.Max(stockData => stockData.Close);
            }

            var normalizedData =
                fileFeatures.Select(stockData => stockData.Normalize(_inputParams, minData, maxData));
            var normalizedDataLines = normalizedData.Select(stockData =>
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0},", stockData.Date);

                if (_inputParams.open)
                {
                    stringBuilder.AppendFormat("{0},", stockData.Open);
                }

                if (_inputParams.high)
                {
                    stringBuilder.AppendFormat("{0},", stockData.High);
                }

                if (_inputParams.low)
                {
                    stringBuilder.AppendFormat("{0},", stockData.Low);
                }

                if (_inputParams.close)
                {
                    stringBuilder.AppendFormat("{0}", stockData.Close);
                }

                return stringBuilder.ToString().TrimEnd(',');
            });

            File.WriteAllLines(filePath, normalizedDataLines);

            return true;
        }
    }

    public class InputParams
    {
        public bool open { get; set; }
        public bool close { get; set; }
        public bool high { get; set; }
        public bool low { get; set; }
        public int daysAgo { get; set; }
        public int numOfStocks { get; set; }
        public int clusters { get; set; }

    }
}
