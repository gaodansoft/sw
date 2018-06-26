using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace CAD3dSW.Controler
{
    /// <summary>
    /// 驱动尺寸
    /// </summary>
    public class GetMass : IProperty
    {
        private string name = "mass";
        private string cfg = "";
        public string Name
        {
            get
            {
                return name;
            }
            set { name = value; }
        }

        public string ConfigName
        {
            get
            {
                return cfg;
            }
            set { cfg = value; }
        }


        public string GetProperty(ModelDoc2 swModelDoc2)
        {
            if (swModelDoc2 == null)
            {
                return "0";
            }
            double val = 0;
            if (cfg != string.Empty)
            {
                try
                {
                    object aaa = swModelDoc2.GetConfigurationNames();
                    swModelDoc2.ShowConfiguration2(cfg);
                    swModelDoc2.EditRebuild3();
                    int ren = 0;
                    double[] MassAll = (double[])swModelDoc2.GetMassProperties2(ref ren);
                    val = MassAll[5];
                }
                catch 
                {
                    return "0";
                }
                
                
                //object aaa = swModelDoc2.GetConfigurationNames();
                //swModelDoc2.ShowConfiguration2(cfg);
                //swModelDoc2.EditRebuild3();
                //int ren = 0;
                //double[] MassAll = (double[])swModelDoc2.GetMassProperties2(ref ren);
                //val = MassAll[5];
            }
            return val.ToString("F3");
        }


        //public string Change(ModelDoc2 swModelDoc2)
        //{
        //    double val = 0;
        //    double[] parms = null;
        //    MassProperty swMass;
        //    swMass = (MassProperty)swModelDoc2.Extension.CreateMassProperty();
        //    swMass.UseSystemUnits = false;
        //    //获得质量
        //    val = swMass.Mass;
        //    //获得体积
        //    val = swMass.Volume;
        //    //获得密度
        //    val = swMass.Density;
        //    //获得表面积
        //    val = swMass.SurfaceArea;
        //    //获得重心
        //    parms = (double[])swMass.CenterOfMass;
        //    //" Center of mass - X: " + parms[0] + " ,Y: " + parms[1] + " ,and Z: " + parms[2]

        //    return string.Empty;
        //}        


    }
}
