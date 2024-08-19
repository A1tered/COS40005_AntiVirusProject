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

        public void ViolationAlert(IntegrityViolation violation)
        {
            Alert alertCreate = new();
            StringBuilder baseMessage = new("Integrity Check Mismatch:");
            baseMessage.Append($"File: {violation.Path}");
            long sizeDifference = Math.Abs(violation.FileSizeBytes - violation.OriginalSize);
            alertCreate.TimeOfViolation = violation.TimeOfViolation;
            alertCreate.Component = "Integrity Checking";
            //// Determine circumstance
            if (violation.Missing = false)
            {
                baseMessage.Append($"Hash Change Detected");
                if (sizeDifference > 1000)
                {
                    alertCreate.SeverityLevel = 1;
                    baseMessage.Append($"Detected Changes:");
                    baseMessage.Append($"Size Change: {FileInfoRequester.SizeValueToLabel(sizeDifference)}");
                    alertCreate.SuggestedAction = "Check contents of file, alert IT";
                    // Major file size changes
                }
                else if (violation.Hash == "")
                {
                    alertCreate.SeverityLevel = 1;
                    baseMessage.Append($"File cannot be Scanned");
                    alertCreate.SuggestedAction = "Scan again later, and raise issue with IT";
                    // Could not scan file
                }
            }
            else
            {
                alertCreate.SeverityLevel = 2;
                baseMessage.Append($"File Missing");
                alertCreate.SuggestedAction = "Check for potential compromise of computer";
                // File Missing (High Severity)
            }
            AlertArgs argument = new();
            argument.AlertSet = alertCreate;
            AlertFlag?.Invoke(this, argument);
        }

    }
}
