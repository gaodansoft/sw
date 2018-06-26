using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace CAD3dSW.Controler
{
    public class ComponentChange : IChange
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Action<string, int> Log;
        public string Change(ModelDoc2 model)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return string.Empty;
            }

            try
            {
                Log(string.Format("设置部件\"{0}\"的配置为\"{1}\"", Name, Value), 1);
                AssemblyDoc assm = (AssemblyDoc)model;
                Component2 com = assm.GetComponentByName(Name);
                com.ReferencedConfiguration = Value;

                return string.Empty;
            }
            catch (Exception ex)
            {
                string err = string.Format("设置部件\"{0}\"的配置为\"{1}\"时失败", Name, Value);
                ex = new Exception(err + "---" + ex.ToString());
                throw ex;
            }



        }
    }
}
