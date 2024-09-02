using GUISandbox.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUISandbox.ViewModels.Pages
{
    public class DataRow
    {
        public string Directory { get; set; }
        public string Hash { get; set; }

        public DataRow(string directory, string hash)
        {
            this.Directory = directory;
            this.Hash = hash;
        }
    }
    public partial class IntegrityConfigViewModel : ObservableObject, INotifyPropertyChanged
    {

        private int _pageNumber;
        private ObservableCollection<DataRow> _datasetDirHash; 
        public IntegrityHandlerModel integHandlerModel { get; set; }

        public int _truncateString;
        public IntegrityConfigViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
            integHandlerModel.IntegrityManagement.PropertyChanged += HandleInnerPropertyChange;
            _datasetDirHash = new();
            _truncateString = 40;
            _pageNumber = 0;
        }

        // String is shortened eg. "Hello my name is jack" -> "...is jack"
        private string TruncateString(string itemCandidate)
        {
            if (itemCandidate.Length > _truncateString)
            {
                string redoString = "...";
                int startPoint = itemCandidate.Length - _truncateString;
                redoString = redoString + itemCandidate.Substring(startPoint, _truncateString);
                return redoString;
            }
            return itemCandidate;

        }

        // Add integrity path to model.
        public async Task<bool> AddIntegrityPath(string path)
        {
            return await integHandlerModel.AddPath(path);
        }

        // Get data entries in Model
        public ObservableCollection<DataRow> GetEntries()
        {
            Dictionary<string, string> dirHash = integHandlerModel.IntegrityManagement.BaselinePage(_pageNumber);
            ObservableCollection<DataRow> tempDataRow = new();
            foreach (KeyValuePair<string, string> set in dirHash)
            {
                tempDataRow.Add(new DataRow(TruncateString(set.Key), set.Value));
            }
            DataEntries = tempDataRow;
            return tempDataRow;
        }


        public ObservableCollection<DataRow> DataEntries
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

        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                int t = integHandlerModel.GetPages();
                if (value + 1 < integHandlerModel.GetPages() && value >= 0)
                {
                    _pageNumber = value;
                }
            }
        }

        // If the Model (IntegrityManagement) sends out an event, handle it and update our properties.
        void HandleInnerPropertyChange(object? sender, PropertyChangedEventArgs args)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
