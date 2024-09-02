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
        public IntegrityHandlerModel integHandlerModel { get; set; }

        public IntegrityViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
            integHandlerModel.IntegrityManagement.PropertyChanged += HandleInnerPropertyChange;
            _progressDefiner = "";
            _progressInfo = "";
        }

        public async Task<int> Scan()
        {
            int result = 0;
            result = await integHandlerModel.Scan();
            // When finished, set property
            Progress = $"100% Progress";
            ProgressInfo = "";
            return result;
        }

        // If the Model (IntegrityManagement) sends out an event, handle it and update our properties.
        void HandleInnerPropertyChange(object? sender, PropertyChangedEventArgs args)
        {
            Progress = $"{integHandlerModel.IntegrityManagement.Progress}% Progress";
            ProgressInfo = $"{integHandlerModel.IntegrityManagement.ProgressInfo}";
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
