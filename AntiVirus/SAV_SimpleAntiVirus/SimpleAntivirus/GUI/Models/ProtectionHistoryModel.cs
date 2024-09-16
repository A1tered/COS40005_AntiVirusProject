using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.IntegrityModule;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
namespace SimpleAntivirus.Models
{
    // Represents the data/business logic of the GUI (IntegrityManagement setup) -> This should be accessible by a range of classes.
    public class ProtectionHistoryModel
    {
        private AlertManager _alertsManager;
        public ProtectionHistoryModel(AlertManager alertManager)
        {
            _alertsManager = alertManager;
            // What starts the reactive part of IntegrityManagement
        }

        public async Task<List<Alert>> GetAlerts()
        {
            return await _alertsManager.GetAllAlertsAsync();
        }

        
    }
}
