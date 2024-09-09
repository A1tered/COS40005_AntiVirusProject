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
    public partial class IntegrityViewModel : ObservableObject, INotifyPropertyChanged
    {

        public string _progressDefiner;
        public string _progressInfo;
        public bool _scanInUse;
        public IntegrityHandlerModel integHandlerModel { get; set; }

        public IntegrityViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
            // We link the property change event inside the Model, so we can propagate the changes upwards. (Probably not ideal to do
            // this but I cannot be bothered with a different approach.
            integHandlerModel.IntegrityManagement.PropertyChanged += HandleInnerPropertyChange;
            _progressDefiner = "";
            _progressInfo = "";
            _scanInUse = false;
        }

        public async Task<int> Scan()
        { // gray the button
            _scanInUse = true;
            int result = 0;
            Progress = "";
            ProgressInfo = "";
            result = await integHandlerModel.Scan();
            // When finished, set property
            Progress = $"100% Progress";
            ProgressInfo = "";
            _scanInUse = false;
            return result;
        }

        // If the Model (IntegrityManagement) sends out an event, handle it and update our properties.
        void HandleInnerPropertyChange(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Progress")
            {
                Progress = $"{integHandlerModel.IntegrityManagement.Progress}% Progress";
                ProgressInfo = $"{integHandlerModel.IntegrityManagement.ProgressInfo}";
            }
        }

        public bool ScanInUse
        {
            get
            {
                return _scanInUse;
            }
            set
            {
                _scanInUse = value;
            }
        }

        // Property which is binded to
        public string Progress
        {
            get
            {
                return _progressDefiner;
            }
            set
            {
                _progressDefiner = value;
                // Lets the view know something has changed.
                this.PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
            }
        }

        // Property that is binded to
        public string ProgressInfo
        {
            get
            {
                return _progressInfo;
            }
            set
            {
                _progressInfo = value;
                // Lets the view know that something has changed.
                this.PropertyChanged(this, new PropertyChangedEventArgs(""));
            }
        }

        // This event indicates to the binding that value has changed.
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
