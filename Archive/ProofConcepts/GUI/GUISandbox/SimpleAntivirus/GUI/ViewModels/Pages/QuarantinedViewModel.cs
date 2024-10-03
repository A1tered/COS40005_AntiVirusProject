using System.ComponentModel;
using System.Windows.Media.Converters;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public class QuarantinedDataRow
    {
        public string OriginalDirectory {  get; set; }
        public string QuarantineDate { get; set; }
    }

    public partial class QuarantinedViewModel : ObservableObject, INotifyPropertyChanged
    {
        private List<string> _pathSelected;
        private bool _allSelected;

        public QuarantinedViewModel()
        {
            _allSelected = false;
        }

        public List<string> PathSelected
        {
            get
            {
                return _pathSelected;
            }
            set
            {
                _pathSelected = value;
            }
        }

        public bool AllSelected
        {
            get
            {
                return _allSelected;
            }
            set
            {
                _allSelected = value;
            }
        }
    }
}
