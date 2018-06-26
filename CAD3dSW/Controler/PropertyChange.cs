using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace CAD3dSW.Controler
{
    public class PropertyChange : IChange
    {


        //private string cfgName=string.Empty;


        public string Name { get; set; }
        public string Value { get; set; }
        public string cfgName { get; set; }
        public Action<string, int> Log;


        /// <summary>
        /// 如果存在，update；否则增加
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="val"></param>
        public string Change(ModelDoc2 model)
        {
            Log(string.Format("属性:{0} 值: {1} cfgName：{2}", Name, Value,cfgName), 1);   
            string path = string.Empty, Mname = string.Empty;
            try
            {
                path = Value.Split('|')[0];
                Mname = Value.Split('|')[1];
            }
            catch (Exception)
            {

                return string.Empty;
            }
            if (cfgName == string.Empty || path == string.Empty || Mname == string.Empty)
            { return string.Empty; }
            try
            {
                PartDoc swPart = ((PartDoc)(model));
                swPart.SetMaterialPropertyName2(cfgName, path, Mname);
            }
            catch 
            {
                string err =  string.Format("装配体不能设置材质！");
                Exception ex = new Exception( err);

                throw ex;
            }            
            //return string.Empty;
            return string.Empty;

            //CustomPropertyManager propMgr = model.Extension.get_CustomPropertyManager(cfgName);

            //if (propMgr != null)
            //{
            //    try
            //    {
            //        if (ExistProperty(model, Name))
            //        {
            //            propMgr.Set(Name, Value);
            //        }
            //        else
            //        {
            //            propMgr.Add2(Name, (int)swCustomInfoType_e.swCustomInfoText, Value);
            //        }
            //        return string.Empty;

            //    }
            //    catch (Exception)
            //    {
            //        string err =  string.Format("设置属性“{0}”为“{1}”时出错！", Name, Value);
            //        Exception ex = new Exception( err);
            //        throw ex;
            //    }

            //}
            //else
            //{
            //    string err = string.Format("属性管理器“{0}”不存在！", cfgName);               
            //    Exception ex = new Exception(err);
                
            //    throw ex;
            //}
            

        }


        private bool ExistProperty(ModelDoc2 model, string prop)
        {
            ModelDocExtension swExt = model.Extension;
            CustomPropertyManager propMgr = swExt.get_CustomPropertyManager(cfgName);
            object propNames = null;
            object propTypes = null;
            object propValues = null;

            string resol = string.Empty;
            propMgr.GetAll(ref propNames, ref propTypes, ref propValues);

            bool has = false;
            if (propNames != null)
            {
                string[] arr = ((string[])propNames);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (string.Compare(arr[i], prop, true) == 0)
                    {
                        has = true;
                        break;
                    }
                }
            }

            return has;
        }
    }
}
