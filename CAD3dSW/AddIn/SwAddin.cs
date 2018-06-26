using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;
using System.Linq;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using CAD3dSW.Controler;
using System.IO;
using System.Collections.Generic;



namespace CAD3dSW
{

    /// <summary>
    /// Summary description for CAD3dSW.
    /// </summary>
    [Guid("f5289a16-84a7-4324-8bc3-8db403c7554d"), ComVisible(true)]
    [SwAddin(
        Description = "CAD3dSW description",
        Title = "CAD3dSW",
        LoadAtStartup = true
        )]
    public class SwAddin : ISwAddin
    {


        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);


        #region Local Variables
        ISldWorks iSwApp;
        ICommandManager iCmdMgr;
        int addinID;

        //CAD3dSW.Controler.Model docMgr;

        List<List<string>> lsComponent;

        #region Event Handler Variables
        Hashtable openDocs;
        SolidWorks.Interop.sldworks.SldWorks SwEventPtr;
        #endregion


        // Public Properties
        public ISldWorks SwApp
        {
            get { return iSwApp; }
        }
        public ICommandManager CmdMgr
        {
            get { return iCmdMgr; }
        }

        public Hashtable OpenDocs
        {
            get { return openDocs; }
        }

        #endregion

        #region SolidWorks Registration
        [ComRegisterFunctionAttribute]
        public static void RegisterFunction(Type t)
        {

            #region Get Custom Attribute: SwAddinAttribute
            SwAddinAttribute SWattr = null;
            Type type = typeof(SwAddin);
            foreach (System.Attribute attr in type.GetCustomAttributes(false))
            {
                if (attr is SwAddinAttribute)
                {
                    SWattr = attr as SwAddinAttribute;
                    break;
                }
            }
            #endregion
            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                Microsoft.Win32.RegistryKey addinkey = hklm.CreateSubKey(keyname);
                addinkey.SetValue(null, 0);

                addinkey.SetValue("Description", SWattr.Description);
                addinkey.SetValue("Title", SWattr.Title);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                addinkey = hkcu.CreateSubKey(keyname);
                addinkey.SetValue(null, Convert.ToInt32(SWattr.LoadAtStartup), Microsoft.Win32.RegistryValueKind.DWord);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                throw;
            }

        }

        [ComUnregisterFunctionAttribute]
        public static void UnregisterFunction(Type t)
        {
            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                hklm.DeleteSubKey(keyname);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                hkcu.DeleteSubKey(keyname);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                throw;
            }

        }

        #endregion

        #region ISwAddin Implementation
        public SwAddin()
        {
        }

        public bool ConnectToSW(object ThisSW, int cookie)
        {
            
            try
            {
                iSwApp = (ISldWorks)ThisSW;
                addinID = cookie;

                //docMgr = new CAD3dSW.Controler.Model(iSwApp, "", string.Empty);
                //Setup callbacks
                iSwApp.SetAddinCallbackInfo(0, this, addinID);

                #region Setup the Command Manager
                iCmdMgr = iSwApp.GetCommandManager(cookie);
                AddCommandMgr();
                #endregion

                #region Setup the Event Handlers
                SwEventPtr = (SolidWorks.Interop.sldworks.SldWorks)iSwApp;
                openDocs = new Hashtable();
                //AttachEventHandlers();
                #endregion

                #region Setup Sample Property Manager
                AddPMP();
                #endregion

                //System.Text.StringBuilder temp = new System.Text.StringBuilder(5);
                //GetPrivateProfileString("Control", "AutoLoad", "0", temp, 5, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                //if (temp.ToString() == "1")
                //{
                //    RunCADworkstation();
                //}
                RunCADworkstation();

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                throw;
            }


            return true;
        }

        public bool DisconnectFromSW()
        {
            RemoveCommandMgr();
            RemovePMP();
            //DetachEventHandlers();

            iSwApp = null;
            //The addin _must_ call GC.Collect() here in order to retrieve all managed code pointers 
            GC.Collect();
            return true;
        }
        #endregion

        #region UI Methods
        public void AddCommandMgr()
        {
            ICommandGroup cmdGroup;
            BitmapHandler iBmp = new BitmapHandler();
            Assembly thisAssembly;
            int cmdIndex0, cmdIndex1, cmdIndex2;
            string Title = "EDS设计平台", ToolTip = "EDS设计平台";


            int[] docTypes = new int[]{(int)swDocumentTypes_e.swDocASSEMBLY,
                                       (int)swDocumentTypes_e.swDocDRAWING,
                                       (int)swDocumentTypes_e.swDocPART};

            thisAssembly = System.Reflection.Assembly.GetAssembly(this.GetType());

            cmdGroup = iCmdMgr.CreateCommandGroup(1, Title, ToolTip, "", -1);
            cmdGroup.LargeIconList = iBmp.CreateFileFromResourceBitmap("CAD3dSW.Resources.ToolbarLarge.bmp", thisAssembly);
            cmdGroup.SmallIconList = iBmp.CreateFileFromResourceBitmap("CAD3dSW.Resources.ToolbarSmall.bmp", thisAssembly);
            cmdGroup.LargeMainIcon = iBmp.CreateFileFromResourceBitmap("CAD3dSW.Resources.MainIconLarge.bmp", thisAssembly);
            cmdGroup.SmallMainIcon = iBmp.CreateFileFromResourceBitmap("CAD3dSW.Resources.MainIconSmall.bmp", thisAssembly);

            cmdIndex0 = cmdGroup.AddCommandItem("重命名组件", -1, "重命名组件", "重命名组件", 3, "RenameComponent", "", 0);
            cmdIndex1 = cmdGroup.AddCommandItem("启动CAD工作站", -1, "启动CAD工作站，自动处理任务", "CAD工作站", 2, "RunCADworkstation", "EnablePMP", 1);            
            cmdIndex2 = cmdGroup.AddCommandItem("修改为此配置", -1, "修改全部尺寸为此配置", "修改为此配置", 1, "Testing", "", 2);

            //cmdIndex3 = cmdGroup.AddCommandItem("功能测试", -1, "开发中功能测试", "功能测试", 1, "Test", "", 3);

            cmdGroup.HasToolbar = true;
            cmdGroup.HasMenu = true;
            cmdGroup.Activate();

            bool bResult;

            foreach (int type in docTypes)
            {
                ICommandTab cmdTab;

                cmdTab = iCmdMgr.GetCommandTab(type, Title);

                if (cmdTab == null)
                {
                    cmdTab = (ICommandTab)iCmdMgr.AddCommandTab(type, Title);

                    CommandTabBox cmdBox = cmdTab.AddCommandTabBox();

                    int[] cmdIDs = new int[4];
                    int[] TextType = new int[4];


                    cmdIDs[0] = cmdGroup.get_CommandID(cmdIndex0);
                    System.Diagnostics.Debug.Print(cmdGroup.get_CommandID(cmdIndex0).ToString());
                    TextType[0] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal;

                    cmdIDs[1] = cmdGroup.get_CommandID(cmdIndex1);
                    System.Diagnostics.Debug.Print(cmdGroup.get_CommandID(cmdIndex1).ToString());
                    TextType[1] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal;

                    cmdIDs[2] = cmdGroup.get_CommandID(cmdIndex2);
                    System.Diagnostics.Debug.Print(cmdGroup.get_CommandID(cmdIndex2).ToString());
                    TextType[2] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal;
                
                    bResult = cmdBox.AddCommands(cmdIDs, TextType);

                    CommandTabBox cmdBox1 = cmdTab.AddCommandTabBox();
                    cmdIDs = new int[1];
                    TextType = new int[1];

                    //cmdIDs[0] = cmdGroup.ToolbarId;
                    //TextType[0] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow | (int)swCommandTabButtonFlyoutStyle_e.swCommandTabButton_ActionFlyout;

                    bResult = cmdBox1.AddCommands(cmdIDs, TextType);
                    cmdTab.AddSeparator(cmdBox1, cmdGroup.ToolbarId);
                }
            }
            thisAssembly = null;
            iBmp.Dispose();
        }


        public void RemoveCommandMgr()
        {
            iCmdMgr.RemoveCommandGroup(1);
        }

        public Boolean AddPMP()
        {
            //ppage = new UserPMPage(this);

            return true;
        }

        public Boolean RemovePMP()
        {
            //ppage = null;
            return true;
        }

        #endregion

        #region UI Callbacks



        /// <summary>重命名组件
        /// RenameComponent 
        /// </summary>
        public void RenameComponent()
        {
            FormReComponent frm = new FormReComponent();
            if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            lsComponent = (List<List<string>>)frm.GetBOMData();
            if (lsComponent.Count == 0)
                return;

            ModelDoc2 swModel;
            ConfigurationManager swConfMgr;
            Configuration swConf;
            Component2 swRootComp;

            swModel = (ModelDoc2)iSwApp.ActiveDoc;
            int docType = swModel.GetType();
            if (docType != 2) return;


            swConfMgr = (ConfigurationManager)swModel.ConfigurationManager;
            swConf = (Configuration)swConfMgr.ActiveConfiguration;
            swRootComp = (Component2)swConf.GetRootComponent3(true);

            TraverseComponent(swRootComp);


        }

        public void TraverseComponent(Component2 swComp)
        {
            string name = swComp.Name2;

            object[] vChildComp;
            Component2 swChildComp;


            vChildComp = (object[])swComp.GetChildren();
            for (int i = 0; i < vChildComp.Length; i++)
            {

                swChildComp = (Component2)vChildComp[i];
                TraverseComponent(swChildComp);
            }
            try
            {
                ModelDoc2 compMdl = (ModelDoc2)swComp.GetModelDoc2();

                if (compMdl != null)
                {
                    string mdlName = Path.GetFileName(compMdl.GetPathName());
                    string refCfg = swComp.ReferencedConfiguration;

                    string newConfig = GetNewConfig(mdlName, refCfg);
                    if (!string.IsNullOrEmpty(newConfig))
                    {
                        ModifyComponent comp = new ModifyComponent(swComp, refCfg, newConfig);
                        comp.Change();

                        swComp.Select(false);
                        AssemblyDoc assem = (AssemblyDoc)iSwApp.ActiveDoc;

                        assem.CompConfigProperties4(2, 0, true, true, newConfig, false);
                    }
                }
            }
            catch 
            {
            }

        }


        public string GetNewConfig(string mdl, string cfg)
        {
            string result = string.Empty;

            for (int i = 0; i < lsComponent.Count(); i++)
            {
                List<string> ls = lsComponent[i];
                if ( ls[0].Equals(mdl,StringComparison.CurrentCultureIgnoreCase) && ls[1].Equals(cfg,StringComparison.CurrentCultureIgnoreCase))
                {
                    return ls[2];
                }
            }

            return result;
        }





        /// <summary>
        /// 功能测试
        /// </summary>
        public void Testing()
        {

            ReadModelInfo RMI = new ReadModelInfo();
            RMI.Main(iSwApp);

            //Model mdl = new Model(iSwApp, "", "",0,null);
            //mdl.model2 = (ModelDoc2)iSwApp.ActiveDoc;
            //mdl.checkModel();


            return;

        }

        private void addDimList(Feature ft, List<string> dimlist)
        {
            do
            {
                DisplayDimension swDispDim = ft.GetFirstDisplayDimension() as DisplayDimension;
                if (swDispDim != null)
                {
                    do
                    {
                        //Annotation swAnn = swDispDim.GetAnnotation() as Annotation;
                        Dimension swDim = swDispDim.GetDimension() as Dimension;
                        dimlist.Add(swDim.FullName);
                        swDispDim = ft.GetNextDisplayDimension(swDispDim) as DisplayDimension;

                    } while (swDispDim != null);
                }
                ft = ft.GetNextSubFeature() as Feature;

            } while (ft != null);
        }



        /// <summary>
        /// 菜单执行命令
        /// </summary>
        public void RunCADworkstation()
        {
            try
            {
                CAD3dSW.Task.TaskServerMgr server = new CAD3dSW.Task.TaskServerMgr(0);
                server.Load(iSwApp);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 控制菜单可用性
        /// </summary>
        /// <returns></returns>
        //public int EnablePMP()
        //{
        //    if (iSwApp.ActiveDoc != null)
        //        return 1;
        //    else
        //        return 0;
        //}
        #endregion

    }

}
