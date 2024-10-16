﻿using SimpleAntivirus.Models;
using SimpleAntivirus.IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.IntegrityModule.DataRelated;

namespace SimpleAntivirus.ViewModels.Pages
{
    public class ResultRow
    {
        public string DisplayDirectory { get; set; }

        public string SizeChange { get; set; }
        public string Missing { get; set; }

        public string SigTime { get; set; }

        public ResultRow(string displayDirectory, string sizeChange, string missing, string sigTime)
        {
            DisplayDirectory = displayDirectory;
            SizeChange = sizeChange;
            Missing = missing;
            SigTime = sigTime;
        }
    }
    public partial class IntegrityResultsViewModel : ObservableObject, INotifyPropertyChanged
    {

        public IntegrityHandlerModel integHandlerModel { get; set; }
        public IntegrityResultsViewModel(IntegrityHandlerModel model)
        {
            integHandlerModel = model;
        }

        // Update contents of datagrid.
        public List<ResultRow> GetEntries()
        {
            List<ResultRow> resultList = new();
            foreach (IntegrityViolation vio in integHandlerModel.RecentViolationList)
            {
                long d = vio.TimeOfSignature;
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(vio.TimeOfSignature).DateTime.ToLocalTime();
                resultList.Add(new ResultRow(FileInfoRequester.TruncateString(vio.Path), vio.FileSizeBytesChange, vio.Missing == true ? "Yes" : "No", dateTime.ToString()));
            }
            return resultList;
        }
    }

}
