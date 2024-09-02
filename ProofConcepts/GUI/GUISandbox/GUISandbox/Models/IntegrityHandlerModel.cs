using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrityModule;
using DatabaseFoundations;
using IntegrityModule.ControlClasses;
namespace GUISandbox.Models
{

    public class IntegrityHandlerModel
    {
        public IntegrityDatabaseIntermediary _integDatabase;
        public IntegrityManagement _integManage;
        public IntegrityHandlerModel()
        {
            IntegrityDatabaseIntermediary integDatabase = new("IntegrityDatabase", false);
            _integDatabase = integDatabase;
            IntegrityManagement integManage = new(integDatabase);
            _integManage = integManage;
        }

        public async Task<int> Scan()
        {
            int returnItem = await _integManage.Scan();
            return returnItem;
        }

        public IntegrityManagement IntegrityManagement
        {
            get
            {
                return _integManage;
            }
        }
    }
}
