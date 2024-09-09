//using GUISandbox.Models;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace GUISandbox.ViewModels.Pages
//{
//    public class DataRow
//    {
//        public string DisplayDirectory { get; set; }

//        public string HiddenDirectory { get; set; }
//        public string Hash { get; set; }

//        public DataRow(string displayDirectory, string hash, string directory)
//        {
//            this.DisplayDirectory = displayDirectory;
//            this.HiddenDirectory = directory;
//            this.Hash = hash;
//        }
//    }
//    public partial class IntegrityConfigViewModel : ObservableObject, INotifyPropertyChanged
//    {

//        private ObservableCollection<DataRow> _datasetDirHash; 
//        public IntegrityHandlerModel integHandlerModel { get; set; }
//        public string _pathSelected;
//        public int _truncateString;
//        private string _addProgress;
//        public IntegrityConfigViewModel(IntegrityHandlerModel model)
//        {
//            integHandlerModel = model;
//            integHandlerModel.IntegrityManagement.PropertyChanged += AddProgressHandler;
//            _datasetDirHash = new();
//            _truncateString = 40;
//            _addProgress = "";
//        }

//        // String is shortened eg. "Hello my name is jack" -> "...is jack"
//        public static string TruncateString(string itemCandidate)
//        {
//            int truncateLength = 40;
//            if (itemCandidate.Length > truncateLength)
//            {
//                string redoString = "...";
//                int startPoint = itemCandidate.Length - truncateLength;
//                redoString = redoString + itemCandidate.Substring(startPoint, truncateLength);
//                return redoString;
//            }
//            return itemCandidate;

//        }

//        public int DeleteItem()
//        {
//            if (_pathSelected != null)
//            {
//                bool returnInfo = integHandlerModel.DeleteDirectory(_pathSelected);
//                // File not found in database
//                if (returnInfo)
//                {
//                    return 2;
//                }
//                return 1;
//            }
//            else
//            {
//                // No item selected
//                return 0;
//            }
//        }

//        // Add integrity path to model.
//        public async Task<bool> AddIntegrityPath(string path)
//        {
//            return await integHandlerModel.AddPath(path);
//        }

//        // Get data entries in Model
//        public ObservableCollection<DataRow> GetEntries(string searchTerm = null)
//        {
//            Dictionary<string, string> dirHash = integHandlerModel.GetPageSet(-1);
//            ObservableCollection<DataRow> tempDataRow = new();
//            bool decideAdd = true;
//            foreach (KeyValuePair<string, string> set in dirHash)
//            {
//                decideAdd = true;
//                if (searchTerm != null)
//                {
//                    if (!set.Key.Contains(searchTerm))
//                    {
//                        // Don't add if it doesn't contain search term.
//                        decideAdd = false;
//                    }
//                }
//                if (decideAdd)
//                {
//                    tempDataRow.Add(new DataRow(TruncateString(set.Key), set.Value, set.Key));
//                }
//            }
//            DataEntries = tempDataRow;
//            return tempDataRow;
//        }

//        public string PathSelected
//        {
//            get
//            {
//                return _pathSelected;
//            }
//            set
//            {
//                _pathSelected = value;
//            }
//        }
//        public ObservableCollection<DataRow> DataEntries
//        {
//            get
//            {
//                return _datasetDirHash;
//            }
//            set
//            {
//                _datasetDirHash = value;
//                //this.PropertyChanged(this, new PropertyChangedEventArgs("DataEntries"));
//            }
//        }

//        // Event updater
//        private void AddProgressHandler(object? sender, PropertyChangedEventArgs args)
//        {
//            if (args.PropertyName == "AddProgress")
//            {
//                AddProgress = $"{Math.Round(integHandlerModel.IntegrityManagement.AddProgress, 2)}%";
//            }
//        }

//        public string AddProgress
//        {
//            get
//            {
//                return _addProgress;
//            }
//            set
//            {
//                _addProgress = value;
//                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AddProgress"));
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//    }
//}
