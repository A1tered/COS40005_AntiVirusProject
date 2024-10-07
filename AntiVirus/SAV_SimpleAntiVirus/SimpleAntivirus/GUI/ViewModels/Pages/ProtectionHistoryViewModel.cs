/**************************************************************************
* File:        ProtectionHistoryViewModel.cs
* Author:      Christopher Thompson
* Description: Communicates with ProtectionHistoryModel to get Alert data.
* Last Modified: 8/10/2024
**************************************************************************/

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
        public event EventHandler AlertPropagate = delegate { };
        public ProtectionHistoryViewModel(ProtectionHistoryModel model)
        {
            _modelObject = model;
            _modelObject.AlertManager.NewAlert += PropagateAlertUp;
        }

        // Get an updated list of contents from the AlertDatabase, this is utilised by the datagrid.
        public async Task<List<AlertRow>> GetEntries()
        {
            List<Alert> alertList = await _modelObject.GetAlerts();
            List<AlertRow> alertRowList = new();
            AlertRow alertCreator;
            foreach (Alert alertItem in alertList)
            {
                // Construct a class to allow for easy binding for the Datagrid.
                alertCreator = new(alertItem.Id.ToString(), alertItem.Component, alertItem.Severity, FileInfoRequester.TruncateString(alertItem.Message, 50), alertItem.Message, alertItem.SuggestedAction, alertItem.Timestamp.ToString());
                alertRowList.Add(alertCreator);
            }
            return alertRowList;
        }

        // Allow access to Alert, from the Model.
        public void PropagateAlertUp(object? o, EventArgs e)
        {
            AlertPropagate.Invoke(this, new EventArgs());
        }

        public async Task ClearAlertDatabase()
        {
            await _modelObject.ClearDatabase();
        }

        public AlertRow SelectedRow
        { get; set; }
    }
}