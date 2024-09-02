using GUISandbox.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUISandbox.ViewModels.Pages
{
    public partial class IntegrityViewModel : ObservableObject, INotifyPropertyChanged
    {

        public string _progressDefiner = "PROGRESS i am";
        public IntegrityHandlerModel integHandlerModel { get; set; }

        public IntegrityViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
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
                this.PropertyChanged(this, new PropertyChangedEventArgs(""));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
