using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Zxtech.CADTaskServer.Contract;

namespace Zxtech.CADTaskServer
{
    [SerializableAttribute]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CADWCFServer : ICADWCFService
    {
        readonly CADTaskServer cad;
        public CADWCFServer(CADTaskServer taskServer)
        {
            this.cad = taskServer;
        }

        public CADTaskCode RunCADCode(CADTaskCode cadTask)
        {
            var cadTaskCode = cad.RunCADCode(cadTask);
            return cadTaskCode;
        }

        public bool TestServer()
        {
            return true;
        }
    }
}
