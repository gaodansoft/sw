using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace CAD3dSW
{
    public class ModifyComponent
    {
        private Component2 Component;
        private string OldConfig;
        private string NewConfig;

        public ModifyComponent( Component2 comp ,string config1, string config2)
        {
            Component = comp;
            OldConfig = config1;
            NewConfig = config2;
        }

        public void Change()
        {
            ModelDoc2 swModel = (ModelDoc2)Component.GetModelDoc2();
            string[] names = (string[])swModel.GetConfigurationNames();

            for (int i = 0; i < names.Length; i++)
            {
                if (NewConfig == names[i])
                    return;
            }

            swModel.ShowConfiguration(OldConfig);
            swModel.AddConfiguration2(NewConfig, "", "", false, false, false, true, 0);

            swModel.Save();
        }
    }
}
