/**************************************************************************
 * File:        ViolationHandler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Intakes violation and turns to Alert data structure, does any relevant formatting / message info for an alert.
 * Last Modified: 26/08/2024
 **************************************************************************/

using DatabaseFoundations.IntegrityRelated;
using IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.Alerts
{

    public class ViolationHandler
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
            StringBuilder baseMessage = new("Integrity Check Mismatch:");
            baseMessage.Append($"File: {violation.Path}");
            // Parameter creation
            DateTime timeViolation = DateTimeOffset.FromUnixTimeSeconds(violation.TimeOfViolation).DateTime;
            string component = "Integrity Checking";
            string severity = "";
            string message = "";
            string suggestedAction = "";
            //// Determine circumstance
            if (violation.Missing == false)
            {
                baseMessage.Append($"Hash Change Detected");
                if (violation.FileSizeBytesChange != "")
                {
                    severity = "Caution";
                    baseMessage.Append($"Detected Changes:");
                    baseMessage.Append($"Size Change: {violation.FileSizeBytesChange}");
                    suggestedAction = "Check contents of file, alert IT";
                    // Major file size changes
                }
                else if (violation.Hash == "")
                {
                    severity = "Warning";
                    baseMessage.Append($"File cannot be Scanned");
                    suggestedAction = "Scan again later, and raise issue with IT";
                    // Could not scan file
                }
            }
            else
            {
                severity = "Danger";
                baseMessage.Append($"File Missing");
                suggestedAction = "Check for potential compromise of computer";
                // File Missing (High Severity)
            }
            message = baseMessage.ToString();
            Alert alertCreate = new(component, severity, message, suggestedAction);
            AlertArgs argument = new();
            argument.AlertSet = alertCreate;
            AlertFlag?.Invoke(this, argument);
        }

    }
}
