using StockAnalayze.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalayze.Common
{
    public static class FileUtils
    {
        public static IEnumerable<string> ProcessStockFiles(IEnumerable<string> stockFiles)
        {
            StatusModel.Instance.Status = "Proccesing the stocks data";
            var processedStockFiles = new List<string>();

            var groupedStockFiles = stockFiles.Split(10);
            var fileNumber = 1;
            foreach (var stockFilesGroup in groupedStockFiles)
            {
                var outputFilePath = Path.Combine(Consts.STOCKS_PROCESSED_PATH_BASE, "StocksFile_" + fileNumber);
                processedStockFiles.Add(outputFilePath);

                using (var outputFileStream = new StreamWriter(outputFilePath))
                {
                    foreach (var stockFilePath in stockFilesGroup)
                    {
                        var stockFileLines = File.ReadAllLines(stockFilePath);
                        var preprocessedStockFileLines = stockFileLines.Select(x =>
                        {
                            var startingIndexAfterDate = x.IndexOf(",", StringComparison.Ordinal) + 1;
                            return x.Substring(startingIndexAfterDate).Replace(',', ' ');
                        });
                        var finalStockVector = string.Join(" ", preprocessedStockFileLines);

                        var stockLine = Path.GetFileNameWithoutExtension(stockFilePath) + " " + finalStockVector;
                        outputFileStream.WriteLine(stockLine);
                    }
                }

                fileNumber++;
            }

            return processedStockFiles;
        }
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }
    }
}
