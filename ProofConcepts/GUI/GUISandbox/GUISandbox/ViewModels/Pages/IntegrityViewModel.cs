using GUISandbox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUISandbox.ViewModels.Pages
{
    public partial class IntegrityViewModel : ObservableObject
    {
        public IntegrityHandlerModel integHandlerModel { get; set; }

        public IntegrityViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
        }

        
        public string Progress { get; set; } = "PROGRESS";

    }
}
