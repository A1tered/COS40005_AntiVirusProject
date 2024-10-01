/**************************************************************************
 * File:        ViolationHandler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Interface for: Intakes violation and turns to Alert data structure, does any relevant formatting / message info for an alert.
 * Last Modified: 29/09/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.Alerts;
using System.Security.AccessControl;

namespace SimpleAntivirus.IntegrityModule.Interface
{

    public interface IViolationHandler
    {
        public event EventHandler<AlertArgs> AlertFlag;

        // Convert violation data structure to Alert and then notify via event.
        public void ViolationAlert(IntegrityViolation violation);
    }
}
