using System;
using System.Collections.Generic;

using System.Text;
using SolidWorks.Interop.sldworks;


namespace CAD3dSW.Controler
{
    //模型驱动接口
    public interface IChange
    {
       
        string Change(ModelDoc2 mdl);

    }

    //模型属性提取接口
    public interface IProperty
    {       
        string Name { get; set; }
        string ConfigName { get; set; }
        string GetProperty(ModelDoc2 mdl );
    }
}
