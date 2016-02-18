using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalayze.Common
{
    class Consts
    {
        public const string DEFAULT_HOST = "10.0.0.19";
        public const string DEFAULT_USERNAME = "training";
        public const string DEFAULT_PASSWORD = "training";

        public const string PROGRAM_NAME = "StockAnalayze";
        public const string MAIN_CLASS_HADOOP = "solution.JobRunner";

        public const string NASDAQ_SYMBOLS_URL = "ftp://ftp.nasdaqtrader.com/symboldirectory/nasdaqlisted.txt";
        public const char NASDAQ_STOCK_SYMBOL_SEPERATOR = '|';

        public const string YAHOO_FINANCE_URL_FORMAT = "http://ichart.yahoo.com/table.csv?s={0}";

        public const float SYMBOL_NUMBER_BUFFER_FACTOR = 1.20f;
        public const int LINES_TO_SKIP_IN_STOCK_FILE = 1;

        public const string INPUT_FILES_DIR_NAME = "InputFiles";
        public const string OUTPUT_FILES_DIR_NAME = "Output";
        public const string STOCK_SYMBOLS_DIR_NAME = "StockSymbols";
        public const string PROCESSED_STOCKS_DIR_NAME = "ProcessedStocks";
        public const string JAVA_FILES_DIR_NAME = "JavaFiles";

        public static readonly string STOCKS_DATA_PATH_BASE = $@"..\..\{INPUT_FILES_DIR_NAME}\{STOCK_SYMBOLS_DIR_NAME}";
        public static readonly string STOCKS_PROCESSED_PATH_BASE = $@"..\..\{INPUT_FILES_DIR_NAME}\{PROCESSED_STOCKS_DIR_NAME}";
        public static readonly string OUTPUT_PATH_BASE = $@"..\..\{OUTPUT_FILES_DIR_NAME}";

        public static readonly string REMOTE_PATH_BASE = $"{PROGRAM_NAME}";
        //public static readonly string HADOOP_PATH_BASE = $"/user/training/{PROGRAM_NAME}";
        public static readonly string HADOOP_PATH_BASE = $"/user/training";

        public static readonly string REMOTE_INPUT_PATH = $"{REMOTE_PATH_BASE}/{INPUT_FILES_DIR_NAME}/";
        //public static readonly string HADOOP_INPUT_PATH = $"{HADOOP_PATH_BASE}/{INPUT_FILES_DIR_NAME}/";
        public static readonly string HADOOP_INPUT_PATH = $"{HADOOP_PATH_BASE}/hadoop_proj/final/";
        //public static readonly string HADOOP_INPUT_BASE = $"{HADOOP_PATH_BASE}/{INPUT_FILES_DIR_NAME}";
        public static readonly string HADOOP_INPUT_BASE = $"{HADOOP_PATH_BASE}/hadoop_proj/final";

        public static readonly string LOCAL_JAVA_FILES_DIR_PATH = $@"..\..\{JAVA_FILES_DIR_NAME}\";
        //public static readonly string REMOTE_JAVA_PATH = $"{REMOTE_PATH_BASE}/{JAVA_FILES_DIR_NAME}/";
        //public static readonly string REMOTE_JAVA_PATH = $"{REMOTE_PATH_BASE}/solution/";
        public static readonly string REMOTE_JAVA_PATH = $"solution/";
        public static readonly string REMOTE_JAR_PATH = $"{PROGRAM_NAME}.jar";

        //public static readonly string REMOTE_OUTPUT_PATH = $"{REMOTE_PATH_BASE}/";
        public static readonly string REMOTE_OUTPUT_PATH = $"/";
        //public static readonly string HADOOP_OUTPUT_PATH = $"{HADOOP_PATH_BASE}/output/kmeans/vectors/";
        public static readonly string HADOOP_OUTPUT_PATH = $"{HADOOP_PATH_BASE}/output/";
        public static readonly string LOCAL_OUTPUT_PATH = $@"..\..\{OUTPUT_FILES_DIR_NAME}\";

        public static readonly string HADOOP_OUTPUT_FILENAME = $"{REMOTE_OUTPUT_PATH}output/part-r-00000";
        public static readonly string REMOTE_OUTPUT_FILENAME = $"output";
        public static readonly string LOCAL_OUTPUT_FILENAME = $@"..\..\{OUTPUT_FILES_DIR_NAME}\output";

    }
}
