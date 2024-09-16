using SimpleAntivirus.Alerts;
using SimpleAntivirus.IntegrityModule.DataRelated;
using SimpleAntivirus.Models;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public class AlertRow
    {
        public string Id { get; set; }

        public string Component { get; set; }

        public string Severity { get; set; }

        public string TruncatedMessage { get; set; }

        public string EntireMessage { get; set; }

        public string SuggestedAction { get; set; }

        public string TimeStamp { get; set; }

        public AlertRow(string id, string component, string severity, string truncateMessage, string entireMessage, string suggestedAction, string timestamp)
        {
            Id = id;
            Component = component;
            Severity = severity;
            TruncatedMessage = truncateMessage;
            EntireMessage = entireMessage;
            SuggestedAction = suggestedAction;
            TimeStamp = timestamp;
        }
    }

    public partial class ProtectionHistoryViewModel : ObservableObject
    {
        ProtectionHistoryModel _modelObject;
        AlertRow selectedRow;
        public ProtectionHistoryViewModel(ProtectionHistoryModel model)
        {
            _modelObject = model;
        }

        public async Task<List<AlertRow>> GetEntries()
        {
            List<Alert> alertList = await _modelObject.GetAlerts();
            List<AlertRow> alertRowList = new();
            AlertRow alertCreator;
            foreach (Alert alertItem in alertList)
            {
                // Construct a class to allow for easy binding for the Datagrid.
                alertCreator = new(alertItem.Id.ToString(), alertItem.Component, alertItem.Severity, FileInfoRequester.TruncateString(alertItem.Message, 40), alertItem.Message, alertItem.SuggestedAction, alertItem.Timestamp.ToString());
                alertRowList.Add(alertCreator);
            }
            return alertRowList;
        }

        public AlertRow SelectedRow
        { get; set; }
    }
}