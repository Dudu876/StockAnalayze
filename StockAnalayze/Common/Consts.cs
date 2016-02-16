using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalayze.Common
{
    class Consts
    {
        public const string DEFAULT_HOST = "192.168.91.132";
        public const string DEFAULT_USERNAME = "training";
        public const string DEFAULT_PASSWORD = "training";

        public const string PROGRAM_NAME = "StockAnalayze";

        public const string NASDAQ_SYMBOLS_URL = "ftp://ftp.nasdaqtrader.com/symboldirectory/nasdaqlisted.txt";
        public const char NASDAQ_STOCK_SYMBOL_SEPERATOR = '|';

        public const string YAHOO_FINANCE_URL_FORMAT = "http://ichart.yahoo.com/table.csv?s={0}";

        public const float SYMBOL_NUMBER_BUFFER_FACTOR = 1.20f;
        public const int LINES_TO_SKIP_IN_STOCK_FILE = 1;


        public const string INPUT_FILES_DIR_NAME = "InputFiles";
        public const string STOCK_SYMBOLS_DIR_NAME = "StockSymbols";
        public const string PROCESSED_STOCKS_DIR_NAME = "ProcessedStocks";

        public static readonly string STOCKS_DATA_PATH_BASE = @"..\..\" + INPUT_FILES_DIR_NAME + @"\"+ STOCK_SYMBOLS_DIR_NAME;
        public static readonly string STOCKS_PROCESSED_PATH_BASE = @"..\..\" + INPUT_FILES_DIR_NAME + @"\" + PROCESSED_STOCKS_DIR_NAME;
    }
}
