/**************************************************************************
* File:        IntegrityResultsViewModel.cs
* Author:      Christopher Thompson
* Description: Handles interaction with IntegrityModel to retrieve most recent violations.
* Last Modified: 8/10/2024
**************************************************************************/

using SimpleAntivirus.Models;
using SimpleAntivirus.IntegrityModule.DataTypes;
using System.ComponentModel;
using SimpleAntivirus.IntegrityModule.DataRelated;

namespace SimpleAntivirus.ViewModels.Pages
{
    public class ResultRow
    {
        public string DisplayDirectory { get; set; }

        public string SizeChange { get; set; }
        public string Missing { get; set; }

        public string SigTime { get; set; }

        public ResultRow(string displayDirectory, string sizeChange, string missing, string sigTime)
        {
            DisplayDirectory = displayDirectory;
            SizeChange = sizeChange;
            Missing = missing;
            SigTime = sigTime;
        }
    }
    public partial class IntegrityResultsViewModel : ObservableObject, INotifyPropertyChanged
    {

        public IntegrityHandlerModel integHandlerModel { get; set; }
        public IntegrityResultsViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
        }

        // Update contents of datagrid.
        public List<ResultRow> GetEntries()
        {
            List<ResultRow> resultList = new();
            foreach (IntegrityViolation vio in integHandlerModel.RecentViolationList)
            {
                long d = vio.TimeOfSignature;
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(vio.TimeOfSignature).DateTime.ToLocalTime();
                resultList.Add(new ResultRow(FileInfoRequester.TruncateString(vio.Path), vio.FileSizeBytesChange, vio.Missing == true ? "True" : "False", dateTime.ToString()));
            }
            return resultList;
        }
    }

}
