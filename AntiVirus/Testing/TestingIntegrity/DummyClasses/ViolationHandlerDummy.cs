/**************************************************************************
 * File:        ViolationHandler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Intakes violation and turns to Alert data structure, does any relevant formatting / message info for an alert.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.Alerts;
using System.Security.AccessControl;
using SimpleAntivirus.IntegrityModule.Interface;

namespace TestingIntegrity.DummyClasses
{

    public class ViolationHandlerDummy : IViolationHandler
    {
        public event EventHandler<AlertArgs> AlertFlag;
        public void ViolationAlert(List<IntegrityViolation> integViolation)
        {

        }

        // Convert violation data structure to Alert and then notify via event.
        public void ViolationAlert(IntegrityViolation violation)
        {
            
        }
    }
}
