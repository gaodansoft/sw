using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zxtech.CADTaskServer;

namespace CAD3dSW.Task
{
    public class Task
    {

        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string NewFileDir { get; set; }
        public string UpdateDrwView { get; set; }

        public string WorkPath { get; set; }
        public string Properties;


    }
}
