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
            baseMessage.Append($"{violation.Path}");
            long sizeDifference = Math.Abs(violation.FileSizeBytes - violation.OriginalSize);
            alertCreate.TimeOfViolation = violation.TimeOfViolation;
            //// Determine circumstance
            if (violation.Missing = false)
            {
                if (sizeDifference > 1000)
                {
                    alertCreate.Message = @$"Integrity Check Mismatch:
                    File: {violation.Path}
                    Detected Changes:
                    Hash Change: {violation.OriginalHash} -> {violation.Hash}
                    Size Change: {FileInfoRequester.SizeValueToLabel(sizeDifference)}
                    ";
                    // Major file size changes
                }
                else if (violation.Hash == "")
                {
                    // Could not scan file
                }

            }
            else
            {
                // File Missing (High Severity)
            }
        }
    }
}
