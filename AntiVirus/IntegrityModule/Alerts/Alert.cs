/**************************************************************************
 * File:        Alert.cs
 * Author:      Christopher Thompson, etc.
 * Description: Alert data structure which is subject to change.
 * Last Modified: 26/08/2024
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;



namespace IntegrityModule.Alerts
{
    // Alert data structure, subject to change.
    public class Alert
    {
        private long _timeOfViolation;

        private string _component;

        private int _severityLevel;

        private string _message;

        private string _suggestedAction;

        public Alert()
        {
            _component = "";
            _message = "";
            _suggestedAction = "";
        }

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

        public string Component
        {
            get
            {
                return _component;
            }
            set
            {
                _component = value;
            }
        }

        public int SeverityLevel
        {
            get
            {
                return _severityLevel;
            }
            set
            {
                _severityLevel = value;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        public string SuggestedAction
        {
            get
            {
                return _suggestedAction;
            }
            set
            {
                _suggestedAction = value;
            }
        }
    }
}
