using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace CAD3dSW.Controler
{
    /// <summary>
    /// 特征压缩、解压缩
    /// </summary>
    public class FeatureChange : IChange
    {

        public string Name { get; set; }
        public string Value { get; set; }
        public Action<string, int> Log;
        public string Change(ModelDoc2 model)
        {
            Feature feat;
            Component2 cp = null;

            if (Model.FeatureDic.ContainsKey(Name))
            {
                Name = Model.FeatureDic[Name];
            }
            Log(string.Format("特征:{0} 值: {1}", Name, Value), 1);
            try
            {
                if (model.GetType() == (int)swDocumentTypes_e.swDocPART)
                {

                    feat = (Feature)((PartDoc)model).FeatureByName(Name);

                }
                else if (model.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
                {
                    try
                    {
                        //使用此方法无法控制获取自装配中的同名件。
                        //cp = ((AssemblyDoc)model).GetComponentByName(Name);


                        Object[] cp1 = (Object[])((AssemblyDoc)model).GetComponents(true);

                        for (int i = 0; i < cp1.Length; i++)
                        {
                            cp = (Component2)cp1[i];
                            if (cp.Name == Name)
                            {
                                break;
                            }
                            else
                            {
                                cp = null;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return string.Format("压缩“{0}”出错，{1}", Name, e.Message);
                        throw;
                    }
                    feat = (Feature)((AssemblyDoc)model).FeatureByName(Name);
                }
                else
                {
                    return string.Format("未匹配到指定特征“{0}”", Name);
                }


                if (feat == null)
                {
                    return string.Format("解压缩失败，未匹配到指定特征“{0}”。", Name);
                }


                if (Value != "N")
                {

                    if (feat.GetTypeName() == "FtrFolder")
                    {
                        SuppressFolder(feat, model, 1);
                    }
                    if (feat.IsSuppressed())
                    {
                        feat.SetSuppression(1);
                    }
                    if (cp != null)
                    {
                        if (cp.IsSuppressed())
                        {
                            cp.SetSuppression2(2);
                        }
                    }
                }
                else
                {

                    if (feat.GetTypeName() == "FtrFolder")
                    {
                        SuppressFolder(feat, model, 0);
                    }
                    else
                    {
                        if (!feat.IsSuppressed())
                        {
                            feat.SetSuppression(0);
                        }
                        if (cp != null)
                        {
                            if (!cp.IsSuppressed())
                            {
                                cp.SetSuppression2(0);
                            }
                        }
                    }
                }


                model.EditRebuild3();
                return string.Empty;
            }
            catch (Exception ex)
            {
                string err = string.Format("    压缩特征“{0}”失败！", Name);
                ex = new Exception(ex.ToString() + err);
                throw ex;
            }


        }

        private bool SuppressFolder(Feature swFeature, ModelDoc2 model, int value)
        {
            //bool inFolder = true;
            //do
            //{
            //    if (model.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            //    {
            //        Component2 cp = ((AssemblyDoc)model).GetComponentByName(swFeature.Name);
            //        if (cp != null)
            //        {
            //            cp.SetSuppression2(value == 0 ? 0 : 2);
            //        }
            //    }

            //    swFeature.SetSuppression(value);
            //    swFeature = swFeature.GetNextFeature() as Feature;
            //    if (swFeature.GetTypeName() == "FtrFolder")
            //    {
            //        inFolder = false;
            //    }

            //} while (swFeature != null && inFolder);
            bool sup = false;
            if (value == 1)
            {
                sup = true;
            }

            bool boolstatus = false;
            boolstatus = model.Extension.SelectByID2(swFeature.Name, "FTRFOLDER", 0, 0, 0, sup, 0, null, 0);
            if (sup)
            {
                model.EditUnsuppress2();
            }
            else
            {
                model.EditSuppress2();
            }

            model.ClearSelection2(true);

            return true;

        }



    }
}
