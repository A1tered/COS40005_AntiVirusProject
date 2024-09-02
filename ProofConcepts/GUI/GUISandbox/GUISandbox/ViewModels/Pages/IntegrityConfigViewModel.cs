using GUISandbox.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUISandbox.ViewModels.Pages
{
    public partial class IntegrityConfigViewModel : ObservableObject, INotifyPropertyChanged
    {

        private List<DataRow> _datasetDirHash; 
        public IntegrityHandlerModel integHandlerModel { get; set; }

        public IntegrityConfigViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
            integHandlerModel.IntegrityManagement.PropertyChanged += HandleInnerPropertyChange;
            _datasetDirHash = new();
        }

        public List<DataRow> GetEntries(int page)
        {
            Dictionary<string, string> dirHash = integHandlerModel.IntegrityManagement.BaselinePage(page);
            List<DataRow> tempDataRow = new();
            foreach (KeyValuePair<string, string> set in dirHash)
            {
                tempDataRow.Add(new DataRow(set.Key, set.Value));
            }
            DataEntries = tempDataRow;
            return tempDataRow;
        }

        public class DataRow
        {
            string Directory { get; set; }
            string Hash { get; set; }

            public DataRow(string directory, string hash)
            {
                this.Directory = directory;
                this.Hash = hash;
            }
        }

        public List<DataRow> DataEntries
        {
            get
            {
                return _datasetDirHash;
            }
            set
            {
                _datasetDirHash = value;
                //this.PropertyChanged(this, new PropertyChangedEventArgs("DataEntries"));
            }
        }

        // If the Model (IntegrityManagement) sends out an event, handle it and update our properties.
        void HandleInnerPropertyChange(object? sender, PropertyChangedEventArgs args)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
