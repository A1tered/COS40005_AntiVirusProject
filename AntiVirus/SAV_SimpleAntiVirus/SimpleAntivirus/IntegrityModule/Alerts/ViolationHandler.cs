/**************************************************************************
 * File:        ViolationHandler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Intakes violation and turns to Alert data structure, does any relevant formatting / message info for an alert.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataTypes;
using System.Text;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.IntegrityModule.Interface;

namespace SimpleAntivirus.IntegrityModule.Alerts
{

    public class ViolationHandler : IViolationHandler
    {
        public event EventHandler<AlertArgs> AlertFlag;
        public void ViolationAlert(List<IntegrityViolation> integViolation)
        {
            foreach (IntegrityViolation compVio in integViolation)
            {
                ViolationAlert(compVio);
            }
        }

        // Convert violation data structure to Alert and then notify via event.
        public void ViolationAlert(IntegrityViolation violation)
        {
            StringBuilder baseMessage = new("Integrity Check Mismatch");
            baseMessage.Append($"\nFile: {violation.Path}");
            // Parameter creation
            DateTime timeViolation = DateTimeOffset.FromUnixTimeSeconds(violation.TimeOfViolation).DateTime;
            string component = "Integrity Checking";
            string severity = "";
            string message = "";
            string suggestedAction = "";
            //// Determine circumstance
            ///
            baseMessage.Append($"\nDetected Issues:");
            if (violation.Missing == false)
            {
                baseMessage.Append($"\nHash Change Detected");
                if (violation.FileSizeBytesChange != "")
                {
                    severity = "Caution";
                    baseMessage.Append($"\nSize Change: {violation.FileSizeBytesChange}");
                    suggestedAction = "Check contents of file, alert IT";
                    // Major file size changes
                }
                else if (violation.Hash == "")
                {
                    severity = "Warning";
                    baseMessage.Append($"\nFile cannot be Scanned");
                    suggestedAction = "Scan again later, and raise issue with IT";
                    // Could not scan file
                }
            }
            else
            {
                severity = "Danger";
                baseMessage.Append($"\nFile Missing");
                suggestedAction = "Check for potential compromise of computer";
                // File Missing (High Severity)
            }
            message = baseMessage.ToString();
            Alert alertCreate = new(component, severity, message, suggestedAction);
            AlertArgs argument = new();
            argument.Component = component;
            argument.Severity = severity;
            argument.Message = message;
            argument.SuggestedAction = suggestedAction;
            // Cheap and dirty solution, may not be ideal, and plan to change later on.
            AlertFlag?.Invoke(this, argument);
        }
    }
}
