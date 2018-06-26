using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.IO;
using Zxtech.CADTaskServer;

namespace CAD3dSW.Controler
{
    public class Model
    {
        //public Task.TaskServerMgr Taskserver = new CAD3dSW.Task.TaskServerMgr(0);
        private ISldWorks iSwApp;
        private string ModelID = string.Empty;

        private CADTaskServer taskServer;
        private int Taskid;

        private List<IChange> changes = new List<IChange>();

        private List<IProperty> properties = new List<IProperty>();
        private Dictionary<string, string> RetProperties = new Dictionary<string, string>();


        public ModelDoc2 model2 = null;
        private string modelName;

        public bool CreateDXF { get; set; }
        public string DXFFileName { get; set; }
        public string refConfig { get; set; }
        public string newConfig { get; set; }

        public static Dictionary<string, string> DimDic = new Dictionary<string, string>();
        public static Dictionary<string, string> FeatureDic = new Dictionary<string, string>();



        //public Model(ISldWorks sw, ModelDoc2 mdl)
        //{
        //    iSwApp = sw;
        //    model2 = mdl;
        //    CreateDXF = false;
        //}

        public int PartId { get; set; }

        public Model(ISldWorks sw, string file, string MID)
        {
            iSwApp = sw;
            ModelID = MID;
            modelName = file;
            CreateDXF = false;
        }

        public Model(ISldWorks sw, string file, string MID, int TaskID, CADTaskServer Ts)
        {
            iSwApp = sw;
            ModelID = MID;
            modelName = file;
            CreateDXF = false;

            Taskid = TaskID;
            taskServer = Ts;
        }


        public void OpenModel()
        {
            //if (Path.GetFileName(modelName) == "SW_24505210.SLDPRT")
            //{
            //    string hying = "Test";                
            //}
            try
            {
                IDocumentSpecification spec = GetSpec();
                string config = Const.DEFAULTCONFIG;
                if (!string.IsNullOrEmpty(refConfig))
                {
                    if (refConfig.ToLower() != "default")
                    {
                        config = refConfig;
                        spec.ConfigurationName = config;
                    }
                    else
                    {
                        config = Const.DEFAULTCONFIG;
                        spec.ConfigurationName = config;
                    }

                }
                if (!File.Exists(modelName))
                {
                    WriteLog(string.Format("零部件不存在：{0}", modelName), 2);
                    return;
 
                }
                model2 = (ModelDoc2)iSwApp.OpenDoc7(spec);
                if (model2 == null)
                {
                    WriteLog(string.Format("零部件没打开：{0}！",modelName), 2);
                }
                else 
                {
                    WriteLog(string.Format("变化零部件：{0}", Path.GetFileName(modelName)), 1);
                }
            }
            catch (Exception ex)
            {

                WriteLog(string.Format("打开模型：{0}出错，请确认模型是否存在 {1}", modelName, ex.ToString()), 2);
            }


            //try
            //{
            //    int longstatus = 0, longwarnings = 0;
            //    model2 = (ModelDoc2)iSwApp.OpenDoc6(modelName, 2, 0, "", ref longstatus, ref longwarnings);
            //    model2 = (ModelDoc2)iSwApp.ActivateDoc2(Path.GetFileName(modelName).Split('.')[0], false, ref longstatus);                                
            //    WriteLog(string.Format("变化零部件：{0}！", Path.GetFileName(modelName)), 1);

            //}
            //catch (Exception)
            //{
            //    WriteLog(string.Format("打开模型：{0}出错，请确认模型是否处在！", Path.GetFileName(modelName)), 2);
            //}
            WriteLog("ActivateConfig", 1);
            ActivateConfig();
            WriteLog("binReadmodelInfo", 1);
            binReadmodelInfo();
            AddConfig();
        }

        private IDocumentSpecification GetSpec()
        {
            IDocumentSpecification spec = (IDocumentSpecification)iSwApp.GetOpenDocSpec(modelName);
            spec.LoadModel = true;
            return spec;
        }


        private void binReadmodelInfo()
        {
            try
            {
                DimDic.Clear();
                FeatureDic.Clear();
                ReadModelInfo RMI = new ReadModelInfo();
                string DimList = RMI.getdimlist(iSwApp, model2);

                string[] DimTemp = DimList.Split(',');
                if (DimTemp.Length > 0)
                {
                    for (int i = 0; i < DimTemp.Length - 1; i++)
                    {
                        if (DimTemp[i] != null)
                        {
                            DimDic.Add(DimTemp[i].Trim('\'').ToUpper(), DimTemp[i].Trim('\''));
                        }
                    }
                }
                string FeatureList = RMI.getFeatureList(iSwApp, model2);
                string[] FeatureTemp = FeatureList.Split(',');
                if (FeatureTemp.Length > 0)
                {
                    for (int i = 0; i < FeatureTemp.Length - 1; i++)
                    {
                        if (FeatureTemp[i] != null)
                        {
                            FeatureDic.Add(FeatureTemp[i].Trim('\'').ToUpper(), FeatureTemp[i].Trim('\''));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

        }


        private bool ActivateConfig()
        {

            if (model2 != null)
            {
                //&& refConfig.ToLower() == "default"       By hying 
                //客户端平台，在不指定引用配置时，不会传空值，会传"default"。导致无法引用到正确的配置。
                WriteLog(string.Format("ActivateConfig : {0 }", refConfig), 1);
                if (!string.IsNullOrEmpty(refConfig))
                {
                    if (refConfig.ToLower() == "default")
                    {
                        return model2.ShowConfiguration2(Const.DEFAULTCONFIG);
                    }
                    else
                    {
                        return model2.ShowConfiguration2(refConfig);
                    }
                }
                else
                {
                    return model2.ShowConfiguration2(Const.DEFAULTCONFIG);
                }
            }
            return false;
        }
        private bool AddConfig()
        {
            try
            {
                if (model2 != null)
                {
                    WriteLog("AddConfig : " + newConfig, 1);
                    return model2.AddConfiguration2(newConfig, "", "", false, false, false, true, 0);
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString(), 2);
                throw;
            }

            return false;
        }



        public void AddCommand(IChange cmd)
        {
            changes.Add(cmd);
        }

        public void AddReturnProperty(IProperty prop)
        {
            properties.Add(prop);
        }

        public void Change()
        {
            if (model2 == null)
            {
                OpenModel();
            }

            if (model2 == null)
            {
                return;
            }

            

            for (int i = 0; i < changes.Count; i++)
            {
                try
                {
                    if (changes[i] is ComponentChange)
                    {
                        ((ComponentChange)changes[i]).Log = (mes, type) => WriteLog(mes, type);
                    }
                    if (changes[i] is DimChange)
                    {
                        ((DimChange)changes[i]).Log = (mes, type) => WriteLog(mes, type);
                    }

                    if (changes[i] is FeatureChange)
                    {
                        ((FeatureChange)changes[i]).Log = (mes, type) => WriteLog(mes, type);
                    }
                    if (changes[i] is PropertyChange)
                    {
                        ((PropertyChange)changes[i]).Log = (mes, type) => WriteLog(mes, type);
                    }

                    changes[i].Change(model2);
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message, 2);
                    return;
                }

            }
            WriteLog("properties", 1);

            for (int i = 0; i < properties.Count; i++)
            {
                string val = properties[i].GetProperty(model2);
                string val1;
                if (!RetProperties.TryGetValue(properties[i].Name, out val1))
                {
                    RetProperties.Add(properties[i].Name, val);
                }
            }
            WriteLog("Regen", 1);
            Regen();
            WriteLog("Save", 1);
            Save();

            if (CreateDXF)
            {
                MakeDXFName();
                if (!checkModel())
                {
                    WriteLog(string.Format("展开图{0}输出失败！", DXFFileName), 2);
                    return;
                }
                SaveAsDXF();
            }
            else
            {
                if (!checkModel())
                {
                    WriteLog(string.Format("模型{0}存在错误！", model2.GetPathName()), 2);
                }
            }

        }

        public Dictionary<string, string> GetProperties()
        {
            return RetProperties;
        }

        private void Regen()
        {
            model2.EditRebuild3();
        }

        private void Save()
        {
            //by hying 2012/1/14 清理无用的方程式。
            disableEqu(model2);

            try
            {
                model2.Save();
                WriteLog(string.Format("保存模型{0}！", Path.GetFileName(modelName)), 1);
            }
            catch (Exception)
            {
                WriteLog(string.Format("保存模型{0}失败！", Path.GetFileName(modelName)), 2);
                throw;
            }

        }


        public void Close()
        {
            try
            {
                if (!modelName.Contains("X11502139.SLDASM"))
                {
                    iSwApp.CloseDoc(modelName);
                }

                WriteLog(string.Format("关闭模型{0}！", Path.GetFileName(modelName)), 1);
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("关闭模型失败{0}！", ex.ToString()), 2);
            }

        }

        private void MakeDXFName()
        {
            if (!Path.GetFileName(DXFFileName).Contains(".dxf"))
            {
                DXFFileName = DXFFileName + ".dxf";
            }
            else if (Path.GetFileName(DXFFileName).Length == 0)
            {
                DXFFileName = DXFFileName + string.Format("Temp{0}.dxf", new Random(10000).ToString());
            }
        }


        //钣金展开图
        public bool SaveAsDXF()
        {

            PartDoc swPart;
            object varAlignment;
            double[] dataAlignment = new double[12];
            object varViews;
            string[] dataViews = new string[2];
            long options;

            dataAlignment[0] = 0.0;
            dataAlignment[1] = 0.0;
            dataAlignment[2] = 0.0;
            dataAlignment[3] = 1.0;
            dataAlignment[4] = 0.0;
            dataAlignment[5] = 0.0;
            dataAlignment[6] = 0.0;
            dataAlignment[7] = 1.0;
            dataAlignment[8] = 0.0;
            dataAlignment[9] = 0.0;
            dataAlignment[10] = 0.0;
            dataAlignment[11] = 1.0;
            varAlignment = dataAlignment;
            dataViews[0] = "*Current";
            dataViews[1] = "*Front";
            varViews = dataViews;
            options = 9;                //0001101 - include flat pattern geometry, bend lines and sketches 

            try
            {
                swPart = (PartDoc)model2;

                WriteLog(string.Format("输出展开图{0}！", DXFFileName), 1);
                iSwApp.ActivateDoc(model2.GetPathName());

                bool outresault = swPart.ExportToDWG(DXFFileName, model2.GetPathName(), 1, true, varAlignment, false, false, (int)options, null);
                if (!outresault)
                {
                    WriteLog(string.Format("展开图输出失败；{0}！", DXFFileName), 2);
                }
            }
            catch (Exception)
            {
                WriteLog(string.Format("展开图输出失败；{0}！", DXFFileName), 2);
                throw;
            }

            return true;
        }

        //by hying 2012/1/14  判断是否有失效的方程式，并将其禁用掉
        public void disableEqu(ModelDoc2 swModel)
        {
            // WriteLog("整理方程式", 1);

            try
            {
                EquationMgr swEquationMgr = null;

                swEquationMgr = ((EquationMgr)(swModel.GetEquationMgr()));

                int EQUnum = swEquationMgr.GetCount();
                for (int i = 0; i < EQUnum; i++)
                {
                    double dval = swEquationMgr.get_Value(i);
                    if (swEquationMgr.Status == -1)
                    {
                        swEquationMgr.set_Suppression(i, true);
                        //WriteLog("整理方程式成功", 1);
                    }

                }
            }
            catch (Exception ex)
            {
                //swEquationMgr.set_Suppression(i, true);
                //Exception ex = new Exception("整理方程式失败!");
                throw ex;
            }
        }


        // by hying 2011/10/31  判断模型是否出错
        public bool checkModel()
        {

            model2.EditRebuild3();

            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            object oFeatures;
            object oErrorCodes;
            object oWarnings;
            object[] Features = null;
            int[] ErrorCodes = null;
            bool[] Warnings = null;
            bool boolstatus = false;

            int nbrWhatsWrong = 0;
            Feature swFeature = default(Feature);

            swModelDocExt = (ModelDocExtension)model2.Extension;

            nbrWhatsWrong = swModelDocExt.GetWhatsWrongCount();
            string a = "Number of What's Wrong items: " + nbrWhatsWrong;

            if (nbrWhatsWrong > 0)
            {
                boolstatus = swModelDocExt.GetWhatsWrong(out oFeatures, out oErrorCodes, out oWarnings);
                Features = (object[])oFeatures;
                ErrorCodes = (int[])oErrorCodes;

                Warnings = (bool[])oWarnings;
                //for (int i = 0; i < Features.Length; i++)
                {
                    swFeature = (Feature)Features[0];
                    //object[] swFeature_1 = (object[])swFeature.GetParents();

                    a = " 零件特征: " + swFeature.Name + "存在问题！";
                    WriteLog(a, 2);
                    Utility.ModelErrorList.AppendLine(a);
                }
                return false;
            }

            return true;
        }



        public void WriteLog(string msg, int type)
        {
            if (taskServer != null)
            {
                try
                {
                    if (ModelID != string.Empty)
                    {
                        msg = ModelID + ":" + msg;
                    }
                    taskServer.SendLogMessage(Taskid, msg, type);
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
