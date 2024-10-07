/**************************************************************************
 * File:        AlertArgs.cs
 * Author:      Christopher Thompson, others
 * Description: AlertArgs structure for eventargs.
 * Last Modified: 8/10/2024
 **************************************************************************/

namespace SimpleAntivirus.IntegrityModule.DataTypes
{
    public class AlertArgs : EventArgs
    {
        public string Component { get; set; } = "";
        public string Severity { get; set; } = "";
        public string Message { get; set; } = "";
        public string SuggestedAction { get; set; } = "";
    }
}
