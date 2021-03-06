﻿using System;
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
using System.Windows.Shapes;
using StockAnalayze.Models;

namespace StockAnalayze
{
    /// <summary>
    /// Interaction logic for GraphsWindow.xaml
    /// </summary>
    public partial class ResultsWindow: Window
    {
        public ResultsWindow(IEnumerable<Cluster> clusters)
        {
            InitializeComponent();

            foreach (var cluster in clusters)
            {
                this.resultsList.Children.Add(new ClusterView(cluster));
            }

        }
    }
}
