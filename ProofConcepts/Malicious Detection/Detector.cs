/**************************************************************************
* File:        [Detector].cs
* Author:      [Pawan]
* Description: [Contains logic to determine if file contains malicious commands]
* Last Modified: [16/09/2024]
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System.Linq;
using System.Collections.Generic;

namespace MaliciousCode
{
    public class Detector
    {
        private readonly DatabaseHandler dbHandler;

        // Constructor to initialize the database handler
        public Detector(DatabaseHandler dbHandler)
        {
            this.dbHandler = dbHandler;
        }

        // Method to check if the file contains any malicious commands
        public bool ContainsMaliciousCommands(string fileContent)
        {
            // Get the list of malicious commands from the database
            var maliciousCommands = dbHandler.GetMaliciousCommands();

            // Check if any malicious commands are present in the file content
            return maliciousCommands.Any(command => fileContent.Contains(command, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
