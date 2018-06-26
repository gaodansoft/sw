using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using CAD3dSW.Controler;
using Zxtech.CADTaskServer;

namespace CAD3dSW.Controler
{
    /// <summary>
    /// 驱动尺寸
    /// </summary>
    public class DimChange : IChange
    {
        CADTaskServer taskServer;
        public string Name { get; set; }
        public string Value { get; set; }
        public Action<string, int> Log;

        public string Change(ModelDoc2 model)
        {
            try
            {
                Log(string.Format("尺寸:{0} 值: {1}", Name, Value),1);
                if (Model.DimDic.ContainsKey(Name))
                {
                    Name = Model.DimDic[Name];
                }

                Dimension dim = (Dimension)model.Parameter(Name);
                if (dim == null)
                {
                    return string.Format("尺寸驱动出错：未发现尺寸“{0}”！", Name);
                }
                if (double.Parse(Value) < 0)
                {
                    return string.Format("尺寸驱动出错：尺寸“{0}”的值不能为“{1}”！", Name, Value);
                }
                else
                {
                    dim.SetValue2(double.Parse(Value), 0);
                    
                }

                return string.Empty;
            }
            catch 
            {
                string err =  string.Format("设置尺寸“{0}”值为“{1}”出错！", Name, Value);
                Exception ex = new Exception(err);
                throw ex; 
            }


        }
        

    }
}
