using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalayze.Models
{
    public class StatusModel : INotifyPropertyChanged
    {
        private string _status;
        private int _progress;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                this._status = value;
                NotifyPropertyChanged("Status");
            }
        }
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                this._progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        public static StatusModel Instance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public StatusModel()
        {
            Instance = this;
        }

        public void setProgress(int index, int outOf)
        {
            try
            {
                double d = (double)index / (double)outOf;
                Progress = (int)(d * 100);
            }
            catch (DivideByZeroException)
            {
                Progress = 0;
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
