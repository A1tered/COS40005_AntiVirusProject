using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.DataTypes
{
    public class IntegrityViolation
    {
        public long _timeOfViolation;
        public string _path;
        public bool _missing;
        public string _hash;
        public long _fileSizeBytes;
        public string _ogHash;
        public long _ogFileSizeBytes;

        public long TimeOfViolation
        {
            get
            {
                return _timeOfViolation;
            }
            set
            {
                _timeOfViolation = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        public bool Missing
        {
            get
            {
                return _missing;
            }
            set
            {
                _missing = value;
            }
        }

        public string Hash
        {
            get
            {
                return _hash;
            }
            set
            {
                _hash = value;
            }
        }

        public long FileSizeBytes
        {
            get
            {
                return _fileSizeBytes;
            }
            set
            {
                _fileSizeBytes = value;
            }
        }

        public string OriginalHash
        {
            get
            {
                return _ogHash;
            }
            set
            {
                _ogHash = value;
            }
        }

        public long OriginalSize
        {
            get
            {
                return _ogFileSizeBytes;
            }
            set
            {
                _ogFileSizeBytes = value;
            }
        }
    }
}
