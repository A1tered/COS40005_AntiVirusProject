/**************************************************************************
 * File:        ProgressArgs.cs
 * Author:      Christopher Thompson, others
 * Description: Defines event information for updating progress bar info upwards.
 * Last Modified: 8/10/2024
 **************************************************************************/

namespace SimpleAntivirus.IntegrityModule.DataTypes
{
    public class ProgressArgs : EventArgs
    {
        public double Progress { get; set; }
        public string ProgressInfo { get; set; } = "";
    }
}
