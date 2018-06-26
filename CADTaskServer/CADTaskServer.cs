using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Timers;
using System.Windows.Forms;
using Zxtech.CADTaskServer.Contract;
using Zxtech.EdisService.Contract;
using Timer=System.Timers.Timer;
using System.Threading;
using System.Globalization;

namespace Zxtech.CADTaskServer
{
    public delegate void CADEventHander(CADArgs args);
    public delegate void RunCADCodeEventHander(RunCADCodeArgs args);
    public delegate void ErasesAllModelOnSessionEventHander();

    [Serializable]
    public class CADTaskServer
    {
        private readonly Timer GetTaskTimer = new Timer();
        private readonly Timer GetTaskTimerOnTaskRunEnd = new Timer();
        private FormCADTaskServer formCADTaskServer;
        private ServiceHost host;
        private bool isLoad;
        private CADWorkStationInfo workStationInfo;
        public event CADEventHander GetCADTask;
        public event RunCADCodeEventHander RunCADCodeEvent;
        public event ErasesAllModelOnSessionEventHander ErasesAllModelOnSession;

        public CADWorkStationInfo Load(int language)
        {
            if (language == 1)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Application.EnableVisualStyles();
            }

            if (this.formCADTaskServer == null)
            {
                this.formCADTaskServer = new FormCADTaskServer();
                this.formCADTaskServer.Closed += this.formCADTaskServer_Closed;
                this.formCADTaskServer.GetNewCADTask += this.formCADTaskServer_GetCADTask;
                this.formCADTaskServer.AutoGetNewCADTask += this.formCADTaskServer_AutoGetNewCADTask;
                this.formCADTaskServer.CovertHandGet += this.formCADTaskServer_CovertHandGet;
                this.formCADTaskServer.Show();
                this.workStationInfo = new CADWorkStationInfo();
                this.isLoad = true;

                if (this.formCADTaskServer.WsInfo == null)
                {
                    this.workStationInfo.IsLoad = false;
                }
                else
                {
                    this.workStationInfo.IsLoad = true;
                    this.workStationInfo.GetTaskCycle = this.formCADTaskServer.WsInfo.GetTaskCycle;

                    this.workStationInfo.PdsPath = this.formCADTaskServer.ServiceConfigInfo.PdsPath;
                    this.workStationInfo.WorkPath = this.formCADTaskServer.ServiceConfigInfo.WorkPath;
                    this.workStationInfo.DakPath = this.formCADTaskServer.ServiceConfigInfo.BakPath;
                    this.workStationInfo.nFontStrokeMode = this.formCADTaskServer.ServiceConfigInfo.nFontStrokeMode;
                    this.workStationInfo.bRebuildBaseMode = this.formCADTaskServer.ServiceConfigInfo.bRebuildBaseMode;
                    this.workStationInfo.bRebuildBaseModelInstTable = this.formCADTaskServer.ServiceConfigInfo.bRebuildBaseModelInstTable;
                    this.workStationInfo.bRebuildNewInst = this.formCADTaskServer.ServiceConfigInfo.bRebuildNewInst;
                    this.workStationInfo.bIsBackupModel = this.formCADTaskServer.ServiceConfigInfo.bIsBackupModel;
                    this.workStationInfo.bMakeDRW = this.formCADTaskServer.ServiceConfigInfo.bMakeDRW;
                    this.workStationInfo.bMakeDWG = this.formCADTaskServer.ServiceConfigInfo.bMakeDWG;
                    this.workStationInfo.bMakeDXF = this.formCADTaskServer.ServiceConfigInfo.bMakeDXF;
                    this.workStationInfo.bMakePDF = this.formCADTaskServer.ServiceConfigInfo.bMakePDF;
                    this.workStationInfo.nSelectTemplateId = this.formCADTaskServer.ServiceConfigInfo.nSelectTemplateId;
                    this.StartCADWCF(this, this.formCADTaskServer.ServiceConfigInfo.WCFCADServer);

                    // 计时器
                    this.GetTaskTimer.Interval = this.formCADTaskServer.WsInfo.GetTaskCycle*1000;
                    this.GetTaskTimer.Elapsed += this.GetTaskTimer_Elapsed;
                    this.GetTaskTimerOnTaskRunEnd.Interval = 5000;
                    this.GetTaskTimerOnTaskRunEnd.Elapsed += this.GetTaskTimerOnTaskRunEnd_Elapsed;

                    return this.workStationInfo;
                }
            }
            return this.workStationInfo;
        }

        private void GetTaskTimerOnTaskRunEnd_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            // 停表
            this.GetTaskTimerOnTaskRunEnd.Stop();

            if (this.ErasesAllModelOnSession != null)
            {
                this.ErasesAllModelOnSession();
            }

            this.GetTaskTimer.Start();
        }

        public void ActivateForm()
        {
            if (this.formCADTaskServer != null)
            {
                Form activeFrom = Form.ActiveForm;
                if (activeFrom != this.formCADTaskServer)
                {
                    this.formCADTaskServer.WindowState = FormWindowState.Normal;
                    this.formCADTaskServer.Activate();
                }
            }
        }

        private void formCADTaskServer_CovertHandGet(object sender, EventArgs e)
        {
            this.GetTaskTimer.Stop();
            this.GetTaskTimerOnTaskRunEnd.Stop();
        }
        //自动获得cad任务
        private void formCADTaskServer_AutoGetNewCADTask(object sender, EventArgs e)
        {
            this.GetTaskTimer.Start();
        }

        private void GetTaskTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.GetTaskTimer.Stop();
            this.InvokeGetCADTask();
        }
        //响应获得cad任务
        private void InvokeGetCADTask()
        {
           // SendLogMessage(1, "InvokeGetCADTask()", 1);//
            if (this.GetCADTask != null && this.formCADTaskServer.TaskRunning == false
                && this.formCADTaskServer.IsWCFConnect && this.isLoad)
            {
               // SendLogMessage(1, "取任务事件" + this.formCADTaskServer.TaskRunning + this.formCADTaskServer.TaskRunning, 1);//
                Thread.Sleep(10000);
                var args = new CADArgs { IsOpenRootModel = false };
                this.GetCADTask(args);
            }
            if(this.formCADTaskServer.isAuto)
            {
                this.GetTaskTimer.Start();
            }
        }

        private void formCADTaskServer_Closed(object sender, EventArgs e)
        {
            this.UnLoad();
        }

        //获得任务编码列表
        public List<CADTaskCode> GetTaskCadCodeList()
        {
            if (this.formCADTaskServer == null)
            {
                return null;
            }

            List<TaskCadCode> taskCadCodes = null;
            try
            {
                taskCadCodes = this.formCADTaskServer.GetTaskCadCodeList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            var listCADTaskCode = new List<CADTaskCode>();
            if (taskCadCodes != null)
            {
                foreach (TaskCadCode taskCadcode in taskCadCodes)
                {
                    var taskCode = new CADTaskCode
                                       {
                                           Id = taskCadcode.Id,
                                           TaskId = taskCadcode.TaskId,
                                           SortId = taskCadcode.SortId,
                                           PartId = taskCadcode.PartId,
                                           CadCodeId = taskCadcode.CadCodeId,
                                           Para1 = taskCadcode.Para1,
                                           Para2 = taskCadcode.Para2,
                                           Para3 = taskCadcode.Para3,
                                           Para4 = taskCadcode.Para4,
                                           Para5 = taskCadcode.Para5,
                                           Para6 = taskCadcode.Para6,
                                           Para7 = taskCadcode.Para7,
                                           Para8 = taskCadcode.Para8,
                                           Para9 = taskCadcode.Para9,
                                           Para10 = taskCadcode.Para10,
                                           Para11 = taskCadcode.Para11,
                                           Para12 = taskCadcode.Para12,
                                           Para13 = taskCadcode.Para13,
                                           Para14 = taskCadcode.Para14,
                                           Para15 = taskCadcode.Para15,
                                           Para16 = taskCadcode.Para16,
                                           Para17 = taskCadcode.Para17,
                                           Para18 = taskCadcode.Para18,
                                           Para19 = taskCadcode.Para19,
                                           Para20 = taskCadcode.Para20
                                       };

                    listCADTaskCode.Add(taskCode);
                }
                this.formCADTaskServer.TaskRunning = true;

                if (this.formCADTaskServer.isAuto)
                {
                    if (listCADTaskCode.Count>0)
                    {
                        this.GetTaskTimer.Stop();
                        this.GetTaskTimerOnTaskRunEnd.Stop();
                    }
                    else
                    {
                        this.GetTaskTimer.Start();
                    }
                }

                return listCADTaskCode;
            }
            return null;
        }
        //随着模型更新任务件属性
        public bool UpdateTaskPartPropWithModel(int taskId, List<CADTaskPropFromModel> listCADTaskPropFromModel)
        {
          
            var listTaskPropFromModel = new List<TaskPropFromModel>();
            if (listCADTaskPropFromModel != null)
            {
                foreach (var cadTaskPropFromModel in listCADTaskPropFromModel)
                {
                    var taskPropFromModel = new TaskPropFromModel
                                                {
                                                    PartId = cadTaskPropFromModel.PartId,
                                                    PropValues = cadTaskPropFromModel.ProValues,
                                                    TaskId = cadTaskPropFromModel.TaskId,
                                                    strNewCfgname = cadTaskPropFromModel.StrNewCfgname
                                                };

                    listTaskPropFromModel.Add(taskPropFromModel);
                }

            }
            if (this.formCADTaskServer == null)
            {
                return false;
            }
            this.formCADTaskServer.TaskRunning = false;
            bool isUpdate = false;
            try
            {
                isUpdate = this.formCADTaskServer.UpdateTaskPartPropWithModel(taskId, listTaskPropFromModel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return isUpdate;
        }

        public void SendLogMessage(int taskId, string message, int messageType)
        {
            if (this.formCADTaskServer == null)
            {
                return;
            }
            this.formCADTaskServer.SendLogMessage(taskId, message, messageType);
        }

        public void UnLoad()
        {
            if (this.formCADTaskServer != null)
            {
                try
                {
                    this.GetTaskTimer.Close();
                    this.GetTaskTimerOnTaskRunEnd.Close();

                    this.formCADTaskServer.Close();
                    this.formCADTaskServer = null;
                    if (this.host != null)
                    {
                        this.host.Close();
                    }
                }
                catch (Exception ex)
                {
                    this.formCADTaskServer.SentLogErrorMessage(ex.Message+ex.StackTrace);
                }
            }
        }
        //退出任务
        public void QuitTask(int nTaskId,int qType)//100 正常放弃 200无法拭除内存
        {
            if (this.formCADTaskServer == null)
            {
                return;
            }
            this.formCADTaskServer.TaskRunning = false;

            try
            {
                this.formCADTaskServer.QuitTask(nTaskId,qType);
            }
            catch (Exception ex)
            {
                this.formCADTaskServer.SentLogErrorMessage(ex.Message + ex.StackTrace);
            }
        }
        //获得cad任务
        private void formCADTaskServer_GetCADTask(object sender, EventArgs e)
        {
            if (this.GetCADTask != null && this.formCADTaskServer.TaskRunning == false)
            {
                var args = new CADArgs { IsOpenRootModel = true };
                this.GetCADTask(args);
            }
        }
        //运行cad编码
        public CADTaskCode RunCADCode(CADTaskCode cadCode)
        {
            if (this.RunCADCodeEvent != null)
            {
                var runArgs = new RunCADCodeArgs {CADCode = cadCode};

                this.RunCADCodeEvent(runArgs);

                return runArgs.CADCode;
            }


            return null;
        }
        //设置任务运行结束
        public void SetTaskRunEnd(int taskId)
        {
           // SendLogMessage(1, "SetTaskRunEnd(int taskId)  taskid=" + taskId + this.formCADTaskServer.isAuto, 1);//
            if (this.formCADTaskServer.isAuto)
            {
                if (taskId < 0)
                {
                    this.GetTaskTimer.Start();
                }
                else
                {
                    this.GetTaskTimerOnTaskRunEnd.Start();
                }
            }
        }
        //启动cad wcf
        private bool StartCADWCF(CADTaskServer cadServer, String url)
        {
            try
            {
                string[] str = url.Split('/');
                string name = str[str.Length - 1];
                string urlName = url.Remove(url.Length - name.Length - 1);

                var ser = new CADWCFServer(cadServer);
                this.host = new ServiceHost(ser, new Uri(urlName));
                this.host.AddServiceEndpoint(typeof (ICADWCFService), new NetTcpBinding(), name);
                this.host.Open();

                return true;
            }
            catch (Exception ex)
            {
                this.formCADTaskServer.SentLogErrorMessage(ex.Message + ex.StackTrace);
                return false;
            }
        }

        public void TestTaskAttachmen()
        {
            this.formCADTaskServer.TestTaskAttachment();
        }
    }


    [Serializable]
    public class CADWorkStationInfo
    {
        public int GetTaskCycle { get; set; }
        public bool IsLoad { get; set; }
        public String PdsPath { get; set; }
        public String WorkPath { get; set; }
        public String DakPath { get; set; }

        public bool bRebuildNewInst { get; set; } // 新的实例
        public bool bRebuildBaseMode { get; set; } // 基本模型
        public bool bRebuildBaseModelInstTable { get; set; } // 基本模型的实例表
        public bool bIsBackupModel { get; set; }
        public int nFontStrokeMode { get; set; } // PDF输出字体

        public bool bMakeDRW { get; set; }
        public bool bMakeDXF { get; set; }
        public bool bMakePDF { get; set; }
        public bool bMakeDWG { get; set; }
        public int nSelectTemplateId { get; set; }
    }

    [Serializable]
    public class CADTaskPropFromModel
    {
        public int PartId { get; set; }
        public string ProValues { get; set; }
        public string StrNewCfgname { get; set; }
        public int TaskId { get; set; }
    }

    [Serializable]
    public class RunCADCodeArgs
    {
        public CADTaskCode CADCode { get; set; }
    }

    [Serializable]
    public class CADArgs
    {
        public bool IsOpenRootModel { get; set; }
    }
}