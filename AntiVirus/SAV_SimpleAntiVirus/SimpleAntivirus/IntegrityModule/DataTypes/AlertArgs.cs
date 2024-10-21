/**************************************************************************
 * File:        AlertArgs.cs
 * Author:      Christopher Thompson, etc.
 * Description: Provides alerts structure for event information.
 * Last Modified: 21/10/2024
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
