using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.IntegrityModule;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
namespace SimpleAntivirus.Models
{
    // Represents the data/business logic of the GUI (IntegrityManagement setup) -> This should be accessible by a range of classes.
    public class IntegrityHandlerModel
    {
        public IntegrityDatabaseIntermediary _integDatabase;
        public IntegrityManagement _integManage;
        private List<IntegrityViolation> _recentViolationList;
        public IntegrityHandlerModel()
        {
            IntegrityDatabaseIntermediary integDatabase = new("IntegrityDatabase", false);
            _integDatabase = integDatabase;
            IntegrityManagement integManage = new(integDatabase);
            _integManage = integManage;
            _recentViolationList = new();
        }

        public List<IntegrityViolation> RecentViolationList
        {
            get
            {
                return _recentViolationList;
            }
        }

        public int GetPages()
        {
            return _integManage.GetPages();
        }

        public bool DeleteDirectory(string directory)
        {
            return _integManage.RemoveBaseline(directory);
        }

        public async Task<bool> AddPath(string path)
        {
            return await _integManage.AddBaseline(path);
        }

        public Dictionary<string, string> GetPageSet(int page)
        {
            return _integManage.BaselinePage(page);
        }

        public async Task<int> Scan()
        {
            _recentViolationList = await _integManage.Scan();
            return _recentViolationList.Count();
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
