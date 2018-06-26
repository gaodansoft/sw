using System;
using System.Collections.Generic;
using System.Text;
using Zxtech.CADTaskServer.Contract;
using Zxtech.CADTaskServer;
using System.Windows;
using System.IO;
using CAD3dSW.Controler;
using SolidWorks.Interop.sldworks;
using System.Reflection;
using System.Linq;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace CAD3dSW.Task
{
    public class TaskServerMgr
    {
        //private CADTaskPropFromModel aaa;

        private CADTaskServer taskServer;
        private CADWorkStationInfo stationInfo;
        private Task taskInstance;

        private string ModelID = string.Empty;

        private ISldWorks iSwApp;
        private int language;

        
        private string rootModelFile;

        private List<CADTaskPropFromModel> ModelProperties = new List<CADTaskPropFromModel>();                 //提取模型的属性


        public TaskServerMgr(int lang)
        {
            language = lang;

        }

        public void Load(ISldWorks sw)
        {
            string path = Registry.ClassesRoot.OpenSubKey("CLSID").OpenSubKey("{F5289A16-84A7-4324-8BC3-8DB403C7554D}").OpenSubKey("InprocServer32").GetValue("CodeBase").ToString();
            if (path.Contains("file:///"))
            {
                path = path.Replace("file:///", "");
            }

            path = Path.GetDirectoryName(path);

            try
            {
                Directory.SetCurrentDirectory(path);
            }
            catch (Exception ee)
            {
                WriteLog(ee.Message + ";" + "CAD3dSW.dll注册路径错误:" + path, 2);
                throw;
            }


            iSwApp = sw;
            try
            {
                taskServer = new CADTaskServer();
                taskServer.GetCADTask += new CADEventHander(taskServer_GetCADTask);
                taskServer.RunCADCodeEvent += new RunCADCodeEventHander(taskServer_RunCADCodeEvent);

                stationInfo = taskServer.Load(language, 200);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message + ";连接服务器失败！", 2);
                throw ex;
            }
        }

        void taskServer_GetCADTask(CADArgs args)
        {
            bool openRoot = args.IsOpenRootModel;

            try
            {
                //初始化 模型错误列表
                Utility.ModelErrorList = new StringBuilder();

                RunCadTask();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message + ";运行CAD任务失败！", 2);
                throw;
            }
            if (taskInstance != null)
            {
                //此处通知平台任务中有错误 by hying 2011/11/1
                if (Utility.ModelErrorList.Length > 0)
                {
                    //System.Windows.Forms.MessageBox.Show("存在错误");
                    taskServer.SetModelError();
                }
                ClearMemory();
                taskServer.SetTaskRunEnd(taskInstance.TaskId);
                WriteLog("任务完成！", 1);
                taskInstance = null;
            }
        }

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        public void ClearMemory()
        {
            System.Diagnostics.Process[] process;
            process = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < process.Length; i++)
            {
                try
                {
                    EmptyWorkingSet(process[i].Handle);
                }
                catch
                {

                }
            }

        }

        //手动操作CAD
        void taskServer_RunCADCodeEvent(RunCADCodeArgs args)
        {
            RunCadCode(args.CADCode);
        }

        private void SendMessage(string msg)
        {
            //taskServer.SendLogMessage(stationInfo.t, msg, 0);
        }


        private void CADTaskInit(CADTaskCode code)
        {
            //1 初始化任务信息
            taskInstance = new Task();

            taskInstance.TaskId = int.Parse(code.Para1);
            taskInstance.TaskName = code.Para2;
            taskInstance.UpdateDrwView = code.Para4;

            taskInstance.NewFileDir = code.Para3;
            taskInstance.WorkPath = stationInfo.WorkPath + "\\" + taskInstance.NewFileDir + "\\";

            taskInstance.Properties = code.Para5;

        }

        private void TemplateInit(CADTaskCode code, Model mdl)
        {
            if (code.Para9 == null) return;

            string DXFFileName = code.Para9.Trim(); //Para9:  格式, Y或N\文件名（不包含路径）

            if (DXFFileName == string.Empty)
            {
                mdl.CreateDXF = false;
                return;
            }

            bool dxf = false;
            if (DXFFileName.Substring(0, 2) == "Y\\")
            {
                dxf = true;
            }
            
            mdl.CreateDXF = dxf && stationInfo.bMakeDXF;
            if (mdl.CreateDXF)
            {
                mdl.DXFFileName = taskInstance.WorkPath + DXFFileName.Substring(2);
            }

            mdl.refConfig = code.Para4;

            if (string.IsNullOrEmpty(code.Para5))
            {
                mdl.newConfig = code.Id.ToString();
            }
            else
            {
                mdl.newConfig = code.Para5;
            }

            mdl.PartId = code.PartId;


        }

        private void GetModelModifyItem(CADTaskCode code, Model mdl)
        {
            //1分析属性值
            ParseItem(typeof(CAD3dSW.Controler.PropertyChange), code.Para1, mdl);

            //2尺寸列表
            ParseItem(typeof(CAD3dSW.Controler.DimChange), code.Para2, mdl);

            //3特征列表
            ParseItem(typeof(CAD3dSW.Controler.FeatureChange), code.Para3 + "," + code.Para4, mdl);

            //4组件列表
            //string compList = code.Para4 + code.Para15;     //解析相同件配置映射
            ParseItem(typeof(CAD3dSW.Controler.ComponentChange), code.Para4, mdl);

            ParsePropertyItem(typeof(CAD3dSW.Controler.GetMass), taskInstance.Properties, mdl);

        }

        private void ParseItem(Type objectType, string str, Model mdl)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            Utility.ParseQuotesVal(str, dic1);

            foreach (string key in dic1.Keys)
            {
                object o = Activator.CreateInstance(objectType, new object[] { });

                PropertyInfo prop1 = o.GetType().GetProperty("Name");
                PropertyInfo prop2 = o.GetType().GetProperty("Value");


                prop1.SetValue(o, key, new object[] { });
                prop2.SetValue(o, dic1[key], new object[] { });
                if (key.ToLower() == "sw_material_name")
                {
                    PropertyInfo prop3 = o.GetType().GetProperty("cfgName");
                    prop3.SetValue(o, mdl.newConfig, new object[] { });
                }

                mdl.AddCommand((IChange)o);
            }

        }

        private void ParsePropertyItem(Type objectType, string str, Model mdl)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            object o = Activator.CreateInstance(objectType, new object[] { });
            PropertyInfo prop1 = o.GetType().GetProperty("Name");
            string propName = prop1.GetValue(o, new object[] { }).ToString();


            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            Utility.ParseQuotesVal(str, dic1);
            foreach (var item in dic1)
            {
                if (item.Key == "1040")
                {
                    IProperty otemp = (IProperty)o;
                    otemp.Name = item.Key;
                    otemp.ConfigName = mdl.newConfig;
                    mdl.AddReturnProperty(otemp);
                    break;
                }
            }
            //foreach (string value in dic1.Values)
            //{
            //    if (string.Compare(value, propName, true) == 0)
            //    {
            //        IProperty otemp = (IProperty)o;
            //        otemp.Name=
            //        mdl.AddReturnProperty((IProperty )o);
            //        break;
            //    }                
            //}
        }



        //备份装配模型文档
        public void BackUpFile(string modelFile, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (path.Substring(path.Length - 1) != "\\")
            {
                path += "\\";
            }
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    WriteLog(string.Format("文件夹：{0}创建失败，请检查是否有对该目录写的权限！", taskInstance.WorkPath), 2);
                    throw;
                }
            }

            string name = Path.GetFileName(modelFile);
            WriteLog(string.Format("复制模型 源：{0}  目标 {1}", modelFile, path + name), 1);
            File.Copy(modelFile, path + name, true);

            string[] dependFiles = (string[])iSwApp.GetDocumentDependencies2(modelFile, true, true, false);
            if (!(dependFiles == null))
            {
                for (int i = 0; i < dependFiles.Length; i += 2)
                {
                    try
                    {
                        name = Path.GetFileName(dependFiles[i + 1]);
                        WriteLog(string.Format("复制依赖模型 源：{0}  目标 {1}", dependFiles[i + 1], path + name), 1);
                        File.Copy(dependFiles[i + 1], path + name, true);
                    }
                    catch (Exception ee)
                    {
                        WriteLog(ee.ToString(),2);
                    }
                }
            }

        }



        private void RunCadTask()
        {
            //递归 1.CAD_MSG_TASK_INIT， 装配CAD_MSG_MODIFY_INIT， 装配子件1CAD_MSG_MODIFY_INIT，装配子件1CAD_MSG_MODIFY，...装配CAD_MSG_MODIFY


            List<CADTaskCode> ls = taskServer.GetTaskCadCodeList();
            if (ls == null)
            {
                taskInstance = null;
                return;
            }

            Stack<Model> modelStack = new Stack<Model>();

            #region 处理CAD代码
            for (int i = 0; i < ls.Count; i++)
            {

                switch (ls[i].CadCodeId)
                {
                    case Const.CAD_MSG_TASK_INIT:
                        {
                            ModelID = ls[i].Para1;
                            CADTaskInit(ls[i]);                            
                            //找跟模型
                            CADTaskCode code = ls.FirstOrDefault(p => p.CadCodeId == Const.CAD_MSG_MODIFY_INIT);
                            if (code == null)
                            {
                                WriteLog(string.Format("任务{0}初始化失败，缺少初始化信息“命令ID=1200”！", taskInstance.TaskId), 2);
                                taskServer.UpdateTaskPartPropWithModel(taskInstance.TaskId, ModelProperties);
                                modelStack.Clear();
                                return;
                            }

                            rootModelFile = code.Para3.Trim("\\".ToCharArray());
                            WriteLog(string.Format("任务{0}初始化完成，开始驱动模板…", taskInstance.TaskId), 1);
                            try
                            {
                                Directory.CreateDirectory(taskInstance.WorkPath);
                            }
                            catch (Exception)
                            {
                                WriteLog(string.Format("文件夹：{0}创建失败，请检查是否有对该目录写的权限！", taskInstance.WorkPath), 2);
                                throw;
                            }

                            string modelFile = stationInfo.PdsPath + "\\" + rootModelFile;

                            try
                            {
                                 WriteLog("备份模型",1);
                                BackUpFile(modelFile, taskInstance.WorkPath);
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("备份模型失败！", ex.Message), 2);
                            }

                            CloseAllModel();

                            //string rootModelPath = taskInstance.WorkPath.TrimEnd("\\".ToArray());
                            Model mdl = new Model(iSwApp, taskInstance.WorkPath + Path.GetFileName(rootModelFile), string.Empty);
                            mdl.OpenModel();

                        }
                        break;
                    case Const.CAD_MSG_MODIFY_INIT:
                        {
                            
                            string mdlName = ls[i].Para3;
                            ModelID = ls[i].Para1;

                            if (mdlName == null)
                            {
                                mdlName = string.Empty;
                            }
                            if (mdlName.Contains('\\'))
                            {
                                mdlName = mdlName.Split('\\')[mdlName.Split('\\').Count() - 1];
                            }
                            //hying 2012/3/15 测试代码
                            //if (mdlName == "13502633.SLDPRT")
                            //{
                            //    break;
                            //}
                            //hying 2012/3/15 测试代码 以上可删

                            Model mdl = new Model(iSwApp, taskInstance.WorkPath + mdlName, ModelID, taskInstance.TaskId, taskServer);                            
                            TemplateInit(ls[i], mdl);
                            //mdl.OpenModel();            //2012/02/22    yyg
                            modelStack.Push(mdl);                            
                           // WriteLog(string.Format("CAD_MSG_MODIFY_INIT:{0}",mdlName),1);
                        }
                        break;

                    case Const.CAD_MSG_MODIFY:
                        {
                            Model mdl = modelStack.Peek();
                            GetModelModifyItem(ls[i], mdl);
                                                       

                            mdl.Change();
                            SaveReturnProperties(mdl, mdl.GetProperties());
                            //if (modelStack.Count() > 1)          //判断是否为 root
                            //{
                            mdl.Close();

                            //ClearMemory();  //hying 2012/3/8 测试

                            //}
                            modelStack.Pop();

                        }
                        break;

                    default:
                        break;
                }
            }
            #endregion

            taskServer.UpdateTaskPartPropWithModel(taskInstance.TaskId, ModelProperties);
            modelStack.Clear();
            ModelID = ls[0].Para1;
        }

        private void CloseAllModel()
        {
            do
            {
                ModelDoc2 swModel = iSwApp.ActiveDoc as ModelDoc2;
                if (swModel != null)
                {
                    iSwApp.CloseDoc(swModel.GetPathName());
                }
                else
                {
                    break;
                }

            } while (true);
        }


        private void SaveReturnProperties(Model mdl, Dictionary<string, string> dic)
        {
            foreach (string key in dic.Keys)
            {
                CADTaskPropFromModel prop = new CADTaskPropFromModel();
                prop.PartId = mdl.PartId;
                prop.TaskId = taskInstance.TaskId;
                prop.ProValues = "'" + key + "','" + (double.Parse(dic[key])).ToString("F03") + "'";
                prop.StrNewCfgname = mdl.newConfig;
                ModelProperties.Add(prop);
            }
        }


        private void RunCadCode(CADTaskCode code)
        {
            switch (code.CadCodeId)
            {
                case Const.CAD_MSG_OPEN_MODEL:
                    {
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        //
                        Model mdl = new Model(iSwApp, code.Para1,string.Empty);
                        mdl.OpenModel();

                        stopwatch.Stop();
                        WriteLog("打开模型耗时：" + stopwatch.ElapsedTicks.ToString(), 2);
                    }
                    break;
                case Const.CAD_MSG_GET_CFGS:
                    {
                        //CADCFGS cfgs;
                        //cfgs.nModelID = _wtoi(stuComm.m_pstrPC_PARA1);
                        //res = ::SendMessage (m_wndCad,nCadRunMessage,CAD_MSG_GET_CFGS,LPARAM(&cfgs)); 	
                        //if(!res) return FALSE;

                        //ArrayToString2(cfgs.arrayCfgs,stuComm.m_pstrPC_PARA2);


                    }
                    break;
                case Const.CAD_MSG_GET_ITEMS:
                    {
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();

                        String resault = string.Empty;
                        ReadModelInfo RMI = new ReadModelInfo();

                        if (code.Para2 == "2")
                        {
                            //读取模型中的尺寸列表
                            resault = RMI.getdimlist(iSwApp, null);

                        }
                        else if (code.Para2 == "4")
                        {
                            //读取模型中的特征列表
                            resault = RMI.getFeatureList(iSwApp, null);

                        }
                        else if (code.Para2 == "5")
                        {
                            //读取模型中的零部件列表
                            resault = RMI.getFeatureList(iSwApp, null);

                        }
                        else if (code.Para2 == "50")
                        {
                            //图层列表
                            resault = string.Empty;

                        }
                        code.Para3 = resault;
                        stopwatch.Stop();
                        WriteLog(string.Format("读取模型属性{0}耗时：{1}", code.Para2, stopwatch.ElapsedTicks.ToString()), 2);
                    }
                    break;
                default:
                    break;
            }

        }

        // type: Const.MSG_TYPE_N,MSG_TYPE_WARN,MSG_TYPE_ERROR
        private void WriteLog(string msg, int type)
        {
            if (taskInstance != null)
            {
                if (ModelID != string.Empty)
                {
                    msg = ModelID + ":" + msg;
                }
                taskServer.SendLogMessage(taskInstance.TaskId, msg, type);
            }
        }
    }
}
