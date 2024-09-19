/**************************************************************************
 * File:        IntegrityViolation.cs
 * Author:      Christopher Thompson, etc.
 * Description: Data structure that represents a violation.
 * Last Modified: 26/08/2024
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAntivirus.IntegrityModule.DataTypes
{
    /// <summary>
    /// Represents a data structure for a IntegrityViolation,
    /// includes all the necessary information.
    /// </summary>
    public class IntegrityViolation
    {
        public long _timeOfViolation;
        public long _timeOfSignature;
        public string _path;
        public bool _missing;
        public string _hash;
        public string _fileSizeBytes;
        public string _userRecent;
        public string _ogHash;

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

        public long TimeOfSignature
        {
            get
            {
                return _timeOfSignature;
            }
            set
            {
                _timeOfSignature = value;
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

        public string FileSizeBytesChange
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

        public string RecentUser
        {
            get
            {
                return _userRecent;
            }
            set
            {
                _userRecent = value;
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
    }
}
