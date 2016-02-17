using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using StockAnalayze.Models;
using StockAnalayze.Common;

namespace StockAnalayze.Managers
{
    class OutputManager
    {
        public List<Cluster> clusters { get; set; }

        public OutputManager(IDictionary<string, Stock> allStocksData)
        {
            List<string> clusterLines = File.ReadAllLines(Consts.LOCAL_OUTPUT_FILENAME).ToList();

            this.clusters = new List<Cluster>();
            for (int j = 0; j < clusterLines.Count; j++)
            {
                // Adding a new cluster object.
                Cluster currCluster = new Cluster();
                currCluster.Id = j + 1;
                // Get the stocks for every cluster.
                string[] stocksInCluster = clusterLines[j].Split(' ');

                for (int i = 0; i < stocksInCluster.Length; i++)
                {
                    if (stocksInCluster[i] != String.Empty)
                    {
                        currCluster.Stocks.Add(
                            allStocksData[stocksInCluster[i]]);
                    }
                }

                clusters.Add(currCluster);
            }
        }
    }
}
