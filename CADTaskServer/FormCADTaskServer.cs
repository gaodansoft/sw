using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Zxtech.EdisService.Contract;
using Timer = System.Timers.Timer;
using Zxtech.CADTaskServer.Properties;
using System.Text.RegularExpressions;
using Zxtech.EDS.PDS.CadWsOutsideControl;
using Zxtech.PdsConstant;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO.Compression;

namespace Zxtech.CADTaskServer
{
    [Serializable]
    public partial class FormCADTaskServer : Form
    {
        private readonly string basePath = Directory.GetCurrentDirectory();
        public readonly IEdisDesignService CADDbConnect;
        public readonly IEdisOnlineRunService CADDbOnLineConnect;
        private readonly String cfgFileName = Directory.GetCurrentDirectory() + "\\CADTaskServerConfig.xml";
        public String cfgClientFileName = Directory.GetCurrentDirectory() + "\\CADTaskClientConfig.xml";
        private readonly List<string> FileTypeListToSer = new List<string>();
        private ChannelFactory<IEdisDesignService> factory;
        public bool isAuto = false;
        private List<FileInfo> listFile;
        //private PdsUser loginUserInfo;
        private bool loginWorkstation;
        private FileStream logTxt;
        private PdsTaskInfo taskInfo;
        private int taskType;
        private Timer timerWCFConnect;
        private Timer timerWSTimelimit; //服务计时器
        private PdsTaskInfo selectedTaskInfo;
        private bool isSelectedTaskInfo;//是否是手动选择(可选)
        private string ServerConfigText;
        private string ClientConfigText;
        private ICadWsOutsideControl cadWs;
        private ChannelFactory<ICadWsOutsideControl> cadWsFactory;
        private readonly int UserId = -100;


        // private bool IsAllowReloadProe;
        private bool IsCadWsAuto;
        private string password;
        public bool IsWCFConnect = true;
        public bool TaskRunning;

        public FormCADTaskServer()
        {
            this.InitializeComponent();
            this.timerWCFConnect = new Timer();
            this.timerWCFConnect.Interval = 60000;
            this.timerWCFConnect.Elapsed += this.timerWCFConnect_Elapsed;
            try
            {
                this.ServiceConfigInfo = new WCFServiceConfigInfo();
                this.GetServiceConfigInfo();
                this.CADDbConnect = this.GetIEdisDesignService();
                this.CADDbOnLineConnect = this.GetIEdisOnlineRunService();
            }
            catch (CommunicationException ex)
            {
                SentLogErrorMessage(ex.Message);
                this.toolStripServerConnect.Text = Resources.NotConnected;
                this.IsWCFConnect = false;
                this.timerWCFConnect.Start();
            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message);
            }

            this.ISHasCadTask = false;
            Directory.CreateDirectory(this.basePath + "\\CadTaskLog");
        }

        #region 数据库连接

        private void GetServiceConfigInfo()
        {
            ServerConfigText = File.ReadAllText(this.cfgFileName, Encoding.UTF8);
            ClientConfigText = File.ReadAllText(this.cfgClientFileName, Encoding.UTF8);
            this.ServiceConfigInfo.WCFServicePath = this.GetElement("WCFServicePath");
            this.ServiceConfigInfo.EndpointUrl = this.GetElement("EndpointUrl");
            this.ServiceConfigInfo.MaxArrayLength = this.GetElement("MaxArrayLength");
            this.ServiceConfigInfo.MaxDepth = this.GetElement("MaxDepth");
            this.ServiceConfigInfo.MaxBytesPerRead = this.GetElement("MaxBytesPerRead");
            this.ServiceConfigInfo.MaxNameTableCharCount = this.GetElement("MaxNameTableCharCount");
            this.ServiceConfigInfo.MaxStringContentLength = this.GetElement("MaxStringContentLength");
            this.ServiceConfigInfo.MaxReceivedMessageSize =
                Convert.ToInt64(this.GetElement("MaxReceivedMessageSize"));
            this.ServiceConfigInfo.PdsPath = this.GetElement("PdsPath");
            this.ServiceConfigInfo.WorkPath = this.GetElement("WorkPath");
            this.ServiceConfigInfo.BakPath = this.GetElement("BakPath");

            this.ServiceConfigInfo.bRebuildNewInst = this.GetBool(this.GetElement("RebuildNewInst"));

            this.ServiceConfigInfo.bRebuildBaseMode = this.GetBool(this.GetElement("RebuildBaseMode"));

            this.ServiceConfigInfo.bRebuildBaseModelInstTable =
                this.GetBool(this.GetElement("RebuildBaseModelInstTable"));
            try
            {
                this.ServiceConfigInfo.nFontStrokeMode = Convert.ToInt32(this.GetElement("FontStrokeMode"));
            }
            catch (Exception exception)
            {
                SentLogErrorMessage(exception.Message);
                //MessageBox.Show(exception.Message);
            }
            this.ServiceConfigInfo.bIsBackupModel = this.GetBool(this.GetElement("IsBackupModel"));
            this.taskType = Convert.ToInt32(this.GetElement("TaskType"));
            this.ServiceConfigInfo.CreateUserId = Convert.ToInt32(this.GetElement("CreateUser"));
            this.ServiceConfigInfo.WCFCADServer = this.GetElement("WCFCADServer");

            this.ServiceConfigInfo.bMakeDRW = this.GetBool(this.GetElement("MakeProjectMap"));
            this.ServiceConfigInfo.bMakeDXF = this.GetBool(this.GetElement("MakeSpreadMap"));
            this.ServiceConfigInfo.bMakeDWG = this.GetBool(this.GetElement("MakeDWG"));
            this.ServiceConfigInfo.bMakePDF = this.GetBool(this.GetElement("MakePDF"));
            this.ServiceConfigInfo.nSelectTemplateId = Convert.ToInt32(this.GetElement("SelectPTId"));
            this.ServiceConfigInfo.AttachmentType = this.GetElement("AttachmentType");
            this.ServiceConfigInfo.IsHaveAttachment = this.GetBool(this.GetElement("IsHaveAttachment"));
            this.ServiceConfigInfo.AttachmentSaveObject = this.GetElement("SaveObject");

            foreach (string t in this.ServiceConfigInfo.AttachmentType.Split(';'))
            {
                if (!String.IsNullOrEmpty(t))
                {
                    this.FileTypeListToSer.Add("." + t);
                }
            }
        }
        private string GetElement(string elementName)
        {
            string regexStr = String.Format(@"<{0}>\s*(.*)\s*</{0}>", elementName);
            var re = new Regex(regexStr, RegexOptions.IgnoreCase);
            Match mc = re.Match(ServerConfigText);
            if (mc.Success)
            {
                return mc.Groups[1].ToString();
            }
            mc = re.Match(ClientConfigText);
            if (mc.Success)
            {
                return mc.Groups[1].ToString();
            }
            return string.Empty;
        }

        private IEdisOnlineRunService GetIEdisOnlineRunService()
        {
            var WCFUrl = new Uri(this.ServiceConfigInfo.WCFServicePath); //WCF Service的服务地址路径
            string EndpointUrl = this.ServiceConfigInfo.EndpointUrl; //Endpoint的子路径

            //要调用WCF Service的Endpoint地址
            string path = WCFUrl + "EdisOnlineRunService"; //EndpointUrl;

            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;


            binding.ReaderQuotas = new XmlDictionaryReaderQuotas
                                       {
                                           MaxArrayLength = Convert.ToInt32(this.ServiceConfigInfo.MaxArrayLength),
                                           MaxDepth = Convert.ToInt32(this.ServiceConfigInfo.MaxDepth),
                                           MaxBytesPerRead = Convert.ToInt32(this.ServiceConfigInfo.MaxBytesPerRead),
                                           MaxNameTableCharCount =
                                               Convert.ToInt32(this.ServiceConfigInfo.MaxNameTableCharCount),
                                           MaxStringContentLength =
                                               Convert.ToInt32(this.ServiceConfigInfo.MaxStringContentLength),
                                       };
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = this.ServiceConfigInfo.MaxReceivedMessageSize;


            ServiceEndpoint httpEndpoint;
            httpEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(IEdisOnlineRunService)),
                                               binding, new EndpointAddress(path));

            ChannelFactory<IEdisOnlineRunService> factory;
            factory = new ChannelFactory<IEdisOnlineRunService>(httpEndpoint);


            return factory.CreateChannel();
        }

        private IEdisDesignService GetIEdisDesignService()
        {
            var WCFUrl = new Uri(this.ServiceConfigInfo.WCFServicePath); //WCF Service的服务地址路径
            string EndpointUrl = this.ServiceConfigInfo.EndpointUrl; //Endpoint的子路径

            //要调用WCF Service的Endpoint地址
            string path = WCFUrl + EndpointUrl;

            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;


            binding.ReaderQuotas = new XmlDictionaryReaderQuotas
                                       {
                                           MaxArrayLength = Convert.ToInt32(this.ServiceConfigInfo.MaxArrayLength),
                                           MaxDepth = Convert.ToInt32(this.ServiceConfigInfo.MaxDepth),
                                           MaxBytesPerRead = Convert.ToInt32(this.ServiceConfigInfo.MaxBytesPerRead),
                                           MaxNameTableCharCount =
                                               Convert.ToInt32(this.ServiceConfigInfo.MaxNameTableCharCount),
                                           MaxStringContentLength =
                                               Convert.ToInt32(this.ServiceConfigInfo.MaxStringContentLength)

                                       };
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = this.ServiceConfigInfo.MaxReceivedMessageSize;
            ServiceEndpoint httpEndpoint;
            httpEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(IEdisDesignService)),
                                               binding, new EndpointAddress(path));

            ChannelFactory<IEdisDesignService> factory;
            factory = new ChannelFactory<IEdisDesignService>(httpEndpoint);


            return factory.CreateChannel();
        }

        private bool GetBool(String boolText)
        {
            if (boolText.ToLower() == "true")
            {
                return true;
            }
            return false;
        }

        #endregion

        public WCFServiceConfigInfo ServiceConfigInfo { get; set; }

        public WorkStationInfo WsInfo { get; private set; }

        public List<TaskCadCode> taskCadList { get; private set; }
        public bool ISHasCadTask { get; private set; }
        public ServerState ServerState { get; private set; }

        public event EventHandler Closed;
        public event EventHandler GetNewCADTask;
        public event EventHandler AutoGetNewCADTask;
        public event EventHandler CovertHandGet;


        //连接属性


        private void btnStop_Click(object sender, EventArgs e)
        {
            this.ServerState = ServerState.Stop;

            this.Refresh();

            this.btnStop.Enabled = false;
            this.btnStart.Enabled = true;
            this.labelInfo.Text = Resources.TaskStop;

            this.toolStripButtonGetTask.Visible = true;
            this.toolStripButtonGetTaskSelect.Visible = true;
            this.isAuto = false;
            this.IsCadWsAuto = false;
            this.CovertHandGet(sender, e);
            try
            {
                if (IsAllowReloadProeWs())
                    cadWs.StopWatch();
            }
            catch (Exception ex)
            {
                SentLogErrorMessage("守护程序：" + ex.Message);
            }

            this.Refresh();
        }

        private void CADFormalRun(WorkStationInfo info)
        {
            this.ServerState = ServerState.Start;
            this.WsInfo = info;
            string hostName = this.GetHostName();
            string ipAddress = this.GetHostIpAddress();
            this.WsInfo.HostName = hostName;
            this.WsInfo.HostIP = ipAddress;

            if (this.WsInfo.IsLogin)
            {
                this.loginWorkstation = true;
            }
            else
            {
                this.loginWorkstation = this.CADDbOnLineConnect.LoginWorkStation(this.WsInfo.WSId, hostName, ipAddress, WorkStationType.Cad);
            }


            if (true)
            {
                this.timerWSTimelimit = new Timer();
                this.timerWSTimelimit.Interval = Convert.ToDouble(this.WsInfo.UpdateTimelimit * 1000 * 60 / 2);
                this.timerWSTimelimit.Elapsed += this.OnWSTimelimitTimedEvent;
                this.timerWSTimelimit.Start();

                this.toolStripWorkStation.Visible = true;
                this.toolStripWorkStation.Text = this.WsInfo.Name;
                this.toolStripStatusLableWorkStation.Visible = true;

                //this.toolStripUserName.Text = this.loginUserInfo.Name;
                // this.toolStripUserName.Visible = true;
                //this.toolStripStatusLabelUserName.Visible = true;

                this.toolStripTaskType.Text = this.GetTaskType(this.taskType.ToString());
                this.Refresh();
            }
        }

        private void OnWSTimelimitTimedEvent(object obj, ElapsedEventArgs e)
        {
            try
            {
                this.CADDbOnLineConnect.UpdateState(this.WsInfo.WSId);
            }
            catch (CommunicationException ex)
            {
                this.toolStripServerConnect.Text = Resources.NotConnected;
                this.IsWCFConnect = false;
                this.timerWCFConnect.Start();
            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message + ex.StackTrace);
            }
        }


        public List<TaskCadCode> GetTaskCadCodeList()
        {
            List<TaskCadCode> task = null;
            if (this.IsWCFConnect)
            {
                try
                {
                    if (this.LoginCADWorkStation())
                    {
                        task = this.GetNewCADTaskInfo();
                    }
                }
                catch (CommunicationException ex)
                {
                    SentLogErrorMessage(ex.Message);
                    this.toolStripServerConnect.Text = Resources.NotConnected;
                    this.IsWCFConnect = false;
                    this.timerWCFConnect.Start();
                }
                catch (Exception ex)
                {
                    SentLogErrorMessage(ex.Message + ex.StackTrace);
                }
                return task;
            }
            return null;
        }
        /// <summary>
        /// 数据工作站登录
        /// </summary>
        /// <returns></returns>
        private bool LoginCADWorkStation()
        {
            List<PdsOnlineWorkStation> workStaionList = this.CADDbOnLineConnect.GetOnlineWorkStationList(WorkStationType.Cad);
            ////工作站存在但被别的机器 注册，工作站停止
            if (workStaionList.Exists(p => p.WSId == this.WsInfo.WSId && (string.Compare(p.HostName, this.WsInfo.HostName, true)) != 0))
            {
                var find = workStaionList.FirstOrDefault(p => p.WSId == this.WsInfo.WSId);
                SentLogErrorMessage(string.Format("工作站被另一台机器注册 计算机名：{0}， 工作站停止！", find.HostName));
                btnStop_Click(null, null);
                return false;
            } //工作站不在重新注册
            else if (!workStaionList.Exists(p => (string.Compare(p.HostName, this.WsInfo.HostName, true) == 0) && p.WSId == this.WsInfo.WSId))
            {
                SentLogErrorMessage("工作站不存在，重新注册！");
                bool ok = this.CADDbOnLineConnect.LoginWorkStation(this.WsInfo.WSId, this.WsInfo.HostName, this.WsInfo.HostIP, ClientType.CadWorkStation);
            }
            return true;
        }

        private List<TaskCadCode> GetNewCADTaskInfo()
        {
            this.rtbRunInfo.Clear();
            this.ISHasCadTask = false;
            string where = GetWhereString();
            string orderby = "Pri,CreateTime";
            // this.SendLogMessage(string.Format("Get Task where:{0}", where));
            this.taskInfo = this.CADDbConnect.GetTaskForWhereFirst(this.WsInfo.WSId, UserId,
                                                                   WorkStationType.Cad, where,
                                                                   orderby);

            if (this.taskInfo != null)
            {
                if (taskInfo.TaskId <= 0)
                    SentLogErrorMessage("任务编号错误！应大于0.");

                this.taskCadList = this.CADDbConnect.GetTaskCadCodeList(this.taskInfo.TaskId);
                if (this.taskCadList.Count > 0)
                {
                    this.ISHasCadTask = true;
                    if (isAuto && IsAllowReloadProeWs())
                    {
                        this.cadWs.StartTask();
                    }
                    this.Text = string.Format("CAD任务服务 - 任务名:{0} ({1})", this.taskInfo.TaskName, this.taskInfo.TaskId);
                    this.Refresh();
                    return this.taskCadList;
                }
                else
                {
                    this.SendLogMessage(string.Format(Resources.GetTask, this.taskInfo.TaskId));
                    this.CADDbConnect.UpdateTaskPartPropWithModel(this.taskInfo.TaskId, this.WsInfo.WSId,
                                                                  UserId,
                                                                  null);

                    bool succ = this.CADDbConnect.SetTaskComplated(this.taskInfo.TaskId, this.taskInfo.CadWSId,
                                                                   UserId, WorkStationType.Cad);
                }
            }

            return null;
        }

        private string GetWhereString()
        {
            if (isSelectedTaskInfo)
            {
                isSelectedTaskInfo = false;
                return string.Format("TaskId=={0}", selectedTaskInfo.TaskId);

            }
            else
            {
                string where = GetHandWereString(true);
                return where;
            }
        }

        private string GetHandWereString(bool isTaskInfo)
        {
            // 任务类型，创建者，系列号
            string whereG = String.Format("&& TaskType=={0}", taskType);
            if (this.ServiceConfigInfo.CreateUserId >= 0)
            {
                if (isTaskInfo)
                    whereG += String.Format("&& Createor=={0}", this.ServiceConfigInfo.CreateUserId);
                else
                    whereG += String.Format("&& Creator=={0}", this.ServiceConfigInfo.CreateUserId);
            }
            if (this.ServiceConfigInfo.nSelectTemplateId >= 0)
            {
                whereG += String.Format("&& SerialId=={0}", this.ServiceConfigInfo.nSelectTemplateId);
            }
            // 当前工作站占用的
            string where1 = String.Format("CadWSId=={0} && State=={1} {2}", this.WsInfo.WSId,
                                          TemplateRunTaskState.CadWsRuning, whereG);

            // 数据工作站完成的
            string where2 = String.Format("State=={0} {1}", TemplateRunTaskState.DataWsComplete, whereG);


            // 合成
            string where = string.Format("({0}) || ({1})", where1, where2);

            // 自动时限制任务终结点为CAD工作站完成
            //if (isAuto)
            //{
            //    where = string.Format("({0}) &&( PdsTaskEndPoint==200 )", where);

            //}



            return where;
        }


        public string GetHostName()
        {
            return Dns.GetHostName();
        }

        private string GetHostIpAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addresses = ipEntry.AddressList;
            string ip = null;
            for (int i = 0; i < addresses.Length; i++)
            {
                ip += string.Format(" ,{0}", addresses[i]);
            }
            return ip;
        }

        public void SendLogMessage(string message)
        {
            int taskId = -1;
            if (this.taskInfo != null)
            {
                taskId = this.taskInfo.TaskId;
            }
            this.SendLogMessage(taskId, message, -1);
        }

        public void SentLogErrorMessage(string message)
        {
            int taskId = -1;
            if (this.taskInfo != null)
            {
                taskId = this.taskInfo.TaskId;
            }
            this.SendLogMessage(taskId, message, 2);
        }

        public void SendLogMessage(int taskId, string message, int messageType)
        {
            //this.Activate();
            //this.Refresh();
            string info;
            switch (messageType)
            {
                case 1:
                    info = string.Format("{0} !    {1}", DateTime.Now, message);
                    break;
                case 2:
                    info = string.Format("{0} !!   {1}", DateTime.Now, message);
                    break;
                default:
                    info = string.Format("{0} {1}", DateTime.Now, message);
                    break;
            }
            //            this.rtbRunInfo.SelectedText = info;

            switch (messageType)
            {
                case 1:
                    this.rtbRunInfo.SelectionColor = Color.Blue;
                    break;
                case 2:
                    this.rtbRunInfo.SelectionColor = Color.Red;
                    break;
                default:
                    break;
            }
            this.rtbRunInfo.AppendText(info);
            this.rtbRunInfo.AppendText("\n");

            // 写入日志
            string path = string.Format("{0}\\CadTasKLog\\CadTaskLog_{1}.txt", this.basePath, taskId);
            this.logTxt = new FileStream(path, FileMode.OpenOrCreate | FileMode.Append);
            var write = new StreamWriter(this.logTxt, Encoding.UTF8);
            write.WriteLine(info + "\n");
            write.Flush();
            this.logTxt.Close();
            Application.DoEvents();
        }

        public bool UpdateTaskPartPropWithModel(int taskId, List<TaskPropFromModel> listTaskPropFromModel)
        {

            if (this.IsWCFConnect)
            {
                // SentLogErrorMessage(" if (conSer.IsWCFConnect)");
                try
                {
                    if (listTaskPropFromModel != null && listTaskPropFromModel.Count > 0)
                    {
                        bool update = this.CADDbConnect.UpdateTaskPartPropWithModel(taskId, this.WsInfo.WSId, UserId,
                                                                              listTaskPropFromModel);
                    }

                    bool succ = this.CADDbConnect.SetTaskComplated(this.taskInfo.TaskId, this.taskInfo.CadWSId,
                                                                   UserId, WorkStationType.Cad);
                    // SentLogErrorMessage(String.Format("taskInfo.TaskId={0}, taskInfo.CadWSId={1}, loginUserInfo.Id={2}, WorkStationType.Cad={3},CADDbConnect.SetTaskComplated={4}...", taskInfo.TaskId, taskInfo.CadWSId, UserId, WorkStationType.Cad, succ));

                    String AttachmentPath = "";
                    //保存工作目录下的附件
                    // SendLogMessage(-1, "文件路径; " + taskInfo.AttachmentPath, 2);
                    if (this.taskInfo != null && this.taskInfo.AttachmentPath != null &&
                        !string.IsNullOrEmpty(this.taskInfo.AttachmentPath))
                    {
                        AttachmentPath = string.Format("{0}\\{1}", this.ServiceConfigInfo.WorkPath,
                                                       this.taskInfo.AttachmentPath);


                        this.listFile = new List<FileInfo>();
                        //     SendLogMessage(AttachmentPath);
                        this.ListFiles(new DirectoryInfo(AttachmentPath));
                        if (this.ServiceConfigInfo.AttachmentSaveObject == "1")//"1表示服务器数据库,0 表示磁盘"
                        {
                            //  SendLogMessage("1表示服务器数据库");
                            List<PdsTaskAttachment> va = this.GetPdsTaskAttachment(this.listFile);
                            this.CADDbConnect.DeleteTaskAttachment(this.taskInfo.TaskId);
                            foreach (PdsTaskAttachment attachment in va)
                            {
                                bool b = this.CADDbConnect.SaveTaskAttachment(attachment);
                            }
                        }
                        else
                        {
                            // SendLogMessage("0表示磁盘");

                            List<FileUploadMessage> filemes = this.GetFileMes(this.taskInfo.AttachmentPath);
                            foreach (FileUploadMessage re in filemes)
                            {
                                this.CADDbConnect.UploadFile(re);
                                if (re.FileData != null)
                                {
                                    re.FileData.Close();
                                }
                            }

                            //foreach (FileInfo info in this.listFile)
                            // {
                            //     info.CopyTo("temp~", true);
                            //     var fm = new FileUploadMessage();
                            //     fm.FileName = info.Name;
                            //     fm.SavePath = AttachmentPath;
                            //     fm.Type = 100;
                            //     FileStream fs = new  FileStream("temp~", FileMode.Open, FileAccess.Read, FileShare.Read);
                            //     fm.FileData = fs;

                            //     this.CADDbConnect.UploadFile(fm);
                            //     if (fm.FileData != null)
                            //     {
                            //         fm.FileData.Close();
                            //     }
                            // }
                        }
                        foreach (FileInfo info in this.listFile)
                        {
                            this.SendLogMessage(string.Format(Resources.SaveError, info.FullName));
                        }
                        if (!this.ServiceConfigInfo.IsHaveAttachment)
                        {
                            try
                            {
                                Directory.Delete(AttachmentPath, true);
                                this.SendLogMessage(string.Format(Resources.FolderDeleted, AttachmentPath));
                            }
                            catch (Exception ex)
                            {
                                this.SentLogErrorMessage(ex.Message);
                            }
                        }
                    }
                    if (IsCadWsAuto)
                    {
                        try
                        {
                            if (isAuto && IsAllowReloadProeWs() && cadWs.IsReachTimerOrTimes())
                                this.cadWs.ReloadProe();
                        }
                        catch (Exception ex)
                        {
                            SentLogErrorMessage("守护程序：" + ex.Message);
                        }

                    }
                    return succ;
                }
                catch (CommunicationException ex)
                {
                    //
                    this.SentLogErrorMessage("UpdateTaskPartPropWithModel()   " + ex.Message + ex.StackTrace);
                    this.toolStripServerConnect.Text = Resources.NotConnected;
                    this.IsWCFConnect = false;
                    this.timerWCFConnect.Start();
                    return false;
                }
                catch (Exception ex)
                {
                    SentLogErrorMessage(ex.Message + ex.StackTrace);
                    return false;
                }
            }
            this.Activate();
            this.Refresh();
            return false;
        }


        private void btnStart_Click(object sender, EventArgs e)
        {

            try
            {

                SetAutoRunToobar();
                if (IsAllowReloadProeWs())
                {
                    SaveCadWsLoadInfo();
                    this.IsCadWsAuto = true;
                    this.AutoGetNewCADTask(null, null);
                    cadWs.StartWatch();
                    return;
                    // toolStripButtonGetTask_Click(null, null);

                }
                this.AutoGetNewCADTask(null, null);

            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message);
            }
        }

        private void SetAutoRunToobar()
        {
            this.ServerState = ServerState.Start;
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
            this.labelInfo.Text = Resources.Running;


            this.toolStripButtonGetTask.Visible = false;
            this.toolStripButtonGetTaskSelect.Visible = false;
            // this.AutoGetNewCADTask(sender, e);
            this.isAuto = true;
            this.Refresh();
        }

        private void SaveCadWsLoadInfo()
        {
            try
            {
                cadWs.SaveCadWsLoadInfo(new CadWsLoadInfo()
                {
                    CadWsId = this.WsInfo.Id,
                    UserId = 0,//this.loginUserInfo.Id,//0513
                    Password = ""//this.loginUserInfo.Password//0513
                });
            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message);
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show(this, Resources.ExitInfo, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {

                try
                {
                    this.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void FormCADTaskServer_Load(object sender, EventArgs e)
        {

            System.Diagnostics.Process.Start("AutoUpdateClient.exe");
            //Cadws
            this.IsCadWsAuto = GetCadWs();
            if (this.IsCadWsAuto)
            {

                this.AutoGetNewCADTask(sender, e);
                SetAutoRunToobar();
                return;
            }


            this.labelInfo.Text = Resources.TaskStop;
            //var formUserLogin = new FormUserLogin(this.CADDbConnect, this.CADDbOnLineConnect);

            //if (formUserLogin.ShowDialog() == DialogResult.OK)
            //{
            //    this.loginUserInfo = formUserLogin.LoginUserInfo;

            try
            {
                // this.loginUserInfo = new PdsUser() {Id = -1};
                var fws = new FormWorkStation(this.CADDbConnect, this.CADDbOnLineConnect);

                fws.CADRun = this.CADFormalRun;

                if (fws.ShowDialog() != DialogResult.OK)
                {
                    //this.CADDbOnLineConnect.UserLogout(this.loginUserInfo.Id);//0513
                    this.Close();
                    return;
                }
                this.timerWSTimelimit.Start();
                this.btnStart.Enabled = true;
                this.btnExit.Enabled = true;
            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message + ex.StackTrace);
                this.Close();
            }
            //}
            //else
            //{
            //    this.Close();
            //}
        }

        private void timerWCFConnect_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.CADDbOnLineConnect.UpdateState(this.WsInfo.WSId);
            }
            catch (CommunicationException ex)
            {
                this.toolStripServerConnect.Text = Resources.NotConnected;
                this.IsWCFConnect = false;
                return;
            }
            this.IsWCFConnect = true;
            this.timerWCFConnect.Stop();
            this.toolStripServerConnect.Text = Resources.Connected;
        }

        public void QuitTask(int taskId, int qType)
        {
            // this.SentLogErrorMessage("QuitTask:WsInfo" + this.WsInfo.WSId + "loginUserInfo:" + this.loginUserInfo.Id +
            //                        "IsWCFConnect;" + this.IsWCFConnect);


            if (this.IsWCFConnect)
            {
                try
                {
                    try
                    {
                        if (qType == 200 && isAuto && IsAllowReloadProeWs())//无法拭除内存时 重启
                        {
                            this.cadWs.ReloadProe();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        SentLogErrorMessage("守护程序：" + ex.Message);
                    }

                    SendLogMessage("正在退出任务");
                    var succ = this.CADDbConnect.QuitTask(taskId, this.WsInfo.WSId, UserId, WorkStationType.Cad);
                    // SendLogMessage("退出任务为"+succ.ToString());
                    try
                    {

                        if (IsCadWsAuto && isAuto)
                        {
                            if (IsAllowReloadProeWs() && cadWs.IsReachTimerOrTimes())
                                this.cadWs.ReloadProe();

                        }
                    }
                    catch (Exception ex)
                    {
                        SentLogErrorMessage("守护程序：" + ex.Message);
                    }
                }
                catch (CommunicationException ex)
                {
                    this.toolStripServerConnect.Text = Resources.NotConnected;
                    this.IsWCFConnect = false;
                    this.timerWCFConnect.Start();
                }
                catch (Exception ex)
                {
                    SentLogErrorMessage(ex.Message + ex.StackTrace);
                }
            }
        }

        private void FormCADTaskServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (timerWSTimelimit != null)
                    this.timerWSTimelimit.Close();
                //if (this.loginUserInfo != null)
                //{
                //    if (this.IsWCFConnect)
                //    {
                //        this.CADDbOnLineConnect.UserLogout(this.loginUserInfo.Id);
                //    }
                //}//0513
                if (this.loginWorkstation)
                {
                    if (this.IsWCFConnect)
                    {
                        this.CADDbOnLineConnect.UnloginWorkStation(this.WsInfo.WSId);
                    }
                }

                this.ServerState = ServerState.Exit;
                if (timerWCFConnect != null)
                    this.timerWCFConnect.Close();

                if (IsAllowReloadProeWs())
                    cadWs.StopWatch();

            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message);
            }
        }

        private String GetTaskType(String value)
        {
            switch (value)
            {
                case "100":

                    return TemplateRunTaskType.ConvertToString(100);
                    break;

                case "200":
                    return TemplateRunTaskType.ConvertToString(200);
                    break;

                default:
                    return TemplateRunTaskType.ConvertToString(300);
                    break;
            }
        }

        private void toolStripButtonGetTask_Click(object sender, EventArgs e)
        {
            this.isAuto = false;
            this.isSelectedTaskInfo = false;
            this.GetNewCADTask(sender, e);
            //gaodan 2008/12/12

        }

        public void TestTaskAttachment()
        {
            // this.attachmentType = AttachmentType.Split(';');
            //  List<FileInfo> fileInfos = GetFileInfo(WorkPath, AttachmentType.Split(';'));
            String AttachmentPath = "";
            //保存工作目录下的附件
            this.listFile = new List<FileInfo>();
            this.ListFiles(new DirectoryInfo(@"D:\download"));

            if (this.ServiceConfigInfo.AttachmentSaveObject == "1")
            {
                List<PdsTaskAttachment> va = this.GetPdsTaskAttachment(this.listFile);
                foreach (PdsTaskAttachment attachment in va)
                {
                    bool b = this.CADDbConnect.SaveTaskAttachment(attachment);
                }
                foreach (FileInfo info in this.listFile)
                {
                    this.SendLogMessage(string.Format(Resources.SaveError, info.FullName));
                }
                if (!this.ServiceConfigInfo.IsHaveAttachment)
                {
                    foreach (FileInfo info in this.listFile)
                    {
                        File.Delete(info.FullName);
                    }
                }
            }
            else
            {
                //List<FileUploadMessage> filemes = this.GetFileMes(null);
                //foreach (FileUploadMessage re in filemes)
                //{
                //    this.CADDbConnect.UploadFile(re);
                //}
            }
        }


        private List<FileUploadMessage> GetFileMes(String AttachmentPath)
        {
            var filemes = new List<FileUploadMessage>();
            foreach (FileInfo info in this.listFile)
            {
                var fm = new FileUploadMessage();
                fm.FileName = info.Name;
                fm.SavePath = AttachmentPath;
                fm.Type = 100;
                FileStream fs = new FileStream(info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                fm.FileData = fs;

                filemes.Add(fm);
            }


            return filemes;
        }


        private List<FileInfo> GetFileInfo(string dir, string[] fileTypes)
        {
            var fileinfos = new List<FileInfo>();
            var dirInfo = new DirectoryInfo(dir);
            if (dirInfo.Exists)
            {
                if (fileTypes == null)
                {
                    FileInfo[] _files = dirInfo.GetFiles();
                    foreach (FileInfo fileinfo in _files)
                    {
                        fileinfos.Add(fileinfo);
                    }
                }
                else
                {
                    foreach (string fileType in fileTypes)
                    {
                        if (!String.IsNullOrEmpty(fileType))
                        {
                            FileInfo[] _files = dirInfo.GetFiles("*." + fileType);
                            foreach (FileInfo fileinfo in _files)
                            {
                                fileinfos.Add(fileinfo);
                            }
                        }
                    }
                }
            }
            return fileinfos;
        }


        private List<PdsTaskAttachment> GetPdsTaskAttachment(List<FileInfo> files)
        {
            var pdsTaskAttachments = new List<PdsTaskAttachment>();

            files.ForEach(f =>
            {

                var pdsTaskAttachment = new PdsTaskAttachment();
                pdsTaskAttachment.TaskId = this.taskInfo.TaskId;
                pdsTaskAttachment.WsID = this.WsInfo.Id;
                pdsTaskAttachment.Attchment1 = this.GetBin(f.FullName);
                pdsTaskAttachment.AttchName1 = f.Name;
                pdsTaskAttachments.Add(pdsTaskAttachment);

            });
            //for (int i = 0; i < files.Count/6; i++)
            //{
            //    var pdsTaskAttachment = new PdsTaskAttachment();

            //    if (this.taskInfo != null)
            //    {
            //        pdsTaskAttachment.TaskId = this.taskInfo.TaskId;
            //    }
            //    pdsTaskAttachment.WsID = this.WsInfo.Id;


            //    for (int j = 0; j < 6; j++)
            //    {
            //        switch (j)
            //        {
            //            case 0:
            //                pdsTaskAttachment.Attchment1 = this.GetBin(files[i*6 + 0].FullName);
            //                pdsTaskAttachment.AttchName1 = files[i*6 + 0].Name;
            //                break;
            //            case 1:
            //                pdsTaskAttachment.Attchment2 = this.GetBin(files[i*6 + 1].FullName);
            //                pdsTaskAttachment.AttchName2 = files[i*6 + 1].Name;
            //                break;
            //            case 2:
            //                pdsTaskAttachment.Attchment3 = this.GetBin(files[i*6 + 2].FullName);
            //                pdsTaskAttachment.AttchName3 = files[i*6 + 2].Name;
            //                break;
            //            case 3:
            //                pdsTaskAttachment.Attchment4 = this.GetBin(files[i*6 + 3].FullName);
            //                pdsTaskAttachment.AttchName4 = files[i*6 + 3].Name;
            //                break;
            //            case 4:
            //                pdsTaskAttachment.Attchment5 = this.GetBin(files[i*6 + 4].FullName);
            //                pdsTaskAttachment.AttchName5 = files[i*6 + 4].Name;
            //                break;
            //            case 5:
            //                pdsTaskAttachment.Attchment6 = this.GetBin(files[i*6 + 5].FullName);
            //                pdsTaskAttachment.AttchName6 = files[i*6 + 5].Name;
            //                break;
            //        }
            //    }


            //   pdsTaskAttachments.Add(pdsTaskAttachment);
            //  }

            //if (files.Count%6 > 0)
            //{
            //    int intf = (files.Count/6)*6;
            //    var pdsTaskAttachment = new PdsTaskAttachment();
            //    if (this.taskInfo != null)
            //    {
            //        pdsTaskAttachment.TaskId = this.taskInfo.TaskId;
            //    }

            //    pdsTaskAttachment.WsID = this.WsInfo.Id;


            //    for (int i = 0; i < files.Count%6; i++)
            //    {
            //        switch (i)
            //        {
            //            case 0:
            //                pdsTaskAttachment.Attchment1 = this.GetBin(files[intf + 0].FullName);
            //                pdsTaskAttachment.AttchName1 = files[intf + 0].Name;
            //                break;
            //            case 1:
            //                pdsTaskAttachment.Attchment2 = this.GetBin(files[intf + 1].FullName);
            //                pdsTaskAttachment.AttchName2 = files[intf + 1].Name;
            //                break;
            //            case 2:
            //                pdsTaskAttachment.Attchment3 = this.GetBin(files[intf + 2].FullName);
            //                pdsTaskAttachment.AttchName3 = files[intf + 2].Name;
            //                break;
            //            case 3:
            //                pdsTaskAttachment.Attchment4 = this.GetBin(files[intf + 3].FullName);
            //                pdsTaskAttachment.AttchName4 = files[intf + 3].Name;
            //                break;
            //            case 4:
            //                pdsTaskAttachment.Attchment5 = this.GetBin(files[intf + 4].FullName);
            //                pdsTaskAttachment.AttchName5 = files[intf + 4].Name;
            //                break;
            //            case 5:
            //                pdsTaskAttachment.Attchment6 = this.GetBin(files[intf + 5].FullName);
            //                pdsTaskAttachment.AttchName6 = files[intf + 5].Name;
            //                break;
            //        }
            //    }
            //    pdsTaskAttachments.Add(pdsTaskAttachment);
            //}
            return pdsTaskAttachments;
        }

        private Binary GetBin(String filename)
        {
            return new Binary(GZip(File.ReadAllBytes(filename), CompressionMode.Compress));
        }
        /// <summary>
        /// 提供内部使用压缩字流的方法
        /// </summary>
        /// <param name="data">字节</param>
        /// <param name="mode">解压或压缩</param>
        /// <returns></returns>

        public static byte[] GZip(byte[] data, CompressionMode mode)
        {
            GZipStream zip = null;
            try
            {
                if (mode == CompressionMode.Compress)
                {
                    var ms = new MemoryStream();
                    zip = new GZipStream(ms, mode, true);
                    zip.Write(data, 0, data.Length);
                    zip.Close();
                    return ms.ToArray();
                }
                else
                {
                    var ms = new MemoryStream();
                    ms.Write(data, 0, data.Length);
                    ms.Flush();
                    ms.Position = 0;
                    zip = new GZipStream(ms, mode, true);
                    var os = new MemoryStream();
                    var SIZE = 1024;
                    var buf = new byte[SIZE];
                    var l = 0;
                    do
                    {
                        l = zip.Read(buf, 0, SIZE);
                        if (l == 0) l = zip.Read(buf, 0, SIZE);
                        os.Write(buf, 0, l);
                    } while (l != 0);
                    zip.Close();
                    return os.ToArray();
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                if (zip != null) zip.Close();
            }
        }




        public void ListFiles(FileSystemInfo fileinfo)
        {
            if (!fileinfo.Exists)
            {
                return;
            }

            var dirinfo = fileinfo as DirectoryInfo;

            if (dirinfo == null)
            {
                return; //不是目录 
            }


            FileSystemInfo[] files = dirinfo.GetFileSystemInfos();

            for (int i = 0; i < files.Length; i++) //遍历目录下所有文件、子目录 
            {
                var file = files[i] as FileInfo;

                if (file != null) // 是文件 
                {
                    if (this.FileTypeListToSer.Contains(file.Extension.ToUpper()))
                    {
                        this.listFile.Add(file);
                    }
                }


                else //是目录 
                {
                    this.ListFiles(files[i]); //对子目录进行递归调用 
                }
            }
        }

        private void rtbRunInfo_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButtonGetTaskSelect_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test");
            try
            {
                MessageBox.Show("Test1");
                string where = GetHandWereString(false);
                //    string orderby = "Pri,CreateTime";
                MessageBox.Show("Test2");
                string orderby = "CreateTime descending";
                MessageBox.Show("Test3");
                var stream = this.CADDbConnect.QueryTaskInfo(where, orderby);
                MessageBox.Show("Test4");
                var ListTaskInfo = DeserializeDataContractFromStream<List<PdsQueryTaskInfo>>(stream);
                MessageBox.Show("Test5");
                FormTaskInfoSelect formSelect = new FormTaskInfoSelect(this.CADDbConnect, ListTaskInfo);
                if (formSelect == null || this.CADDbConnect == null || ListTaskInfo == null)
                {
                    SentLogErrorMessage("窗体对象空");
                    return;
                }
                MessageBox.Show("显示窗体");
                if (formSelect.ShowDialog() == DialogResult.OK)
                {
                    if (formSelect.selectedTaskInfo != null)
                    {
                        this.selectedTaskInfo = (PdsTaskInfo)ConvertObject(typeof(PdsQueryTaskInfo), typeof(PdsTaskInfo), formSelect.selectedTaskInfo);
                        this.selectedTaskInfo.Createor = formSelect.selectedTaskInfo.Creator;
                        this.isSelectedTaskInfo = true;
                        this.isAuto = false;
                        this.GetNewCADTask(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message + ex.StackTrace);
            }
        }
        private bool GetCadWs()
        {
            try
            {
                //Uri WCFUrl = new Uri("http://localhost:567/wxwinter/"); //WCF Service的服务地址路径
                //string EndpointUrl = "EndpointUrl";  //Endpoint的子路径

                //要调用WCF Service的Endpoint地址
                // string path = WCFUrl.ToString() + EndpointUrl;
                var IsAllowReloadProe = IsAllowReloadProeWs();
                if (!IsAllowReloadProe) return false;
                if (cadWs.NaturalExit()) return false;
                //if (!cadWs.IsReachTimerOrTimes()) return false;
                CadWsLoadInfo cadWsLoadInfo = cadWs.GetCadWsLoadInfo();

                if (cadWsLoadInfo == null)
                {
                    // Trace.Assert(false, "cadWsLoadInfo == null");
                    return false;
                }
                try
                {
                    // var userList = this.CADDbConnect.GetUserList();//0513
                    // this.loginUserInfo = userList.Find(p => p.Id == cadWsLoadInfo.UserId);//0513
                    // if (this.loginUserInfo == null) return false;//0513
                    // loginUserInfo.Password = cadWsLoadInfo.Password;//0513
                    //用户和工作站登录

                    // var formUserLogin = new FormUserLogin(this.CADDbConnect, this.CADDbOnLineConnect);


                    //var useLoad = CADDbOnLineConnect.UserLogin(cadWsLoadInfo.UserId, cadWsLoadInfo.Password, FormUserLogin.GetHostName(), FormUserLogin.GetHostIpAddress(), ClientType.CadWorkStation);
                    //if (useLoad)
                    //    SendLogMessage("用户登陆成功!");
                    //else
                    //{
                    //    //  SentLogErrorMessage("用户用户登陆失败!");
                    //    // return false;
                    //}//0513


                    this.WsInfo = CADDbConnect.GetWorkStationList(WorkStationType.Cad).Find(pw => pw.Id == cadWsLoadInfo.CadWsId);
                    CADFormalRun(WsInfo);
                }
                catch (CommunicationException ex)
                {
                    SentLogErrorMessage(ex.Message);
                    this.toolStripServerConnect.Text = Resources.NotConnected;
                    this.IsWCFConnect = false;
                    this.timerWCFConnect.Start();
                }

                return true;

            }
            catch (Exception ex)
            {
                cadWs = null;
                cadWsFactory = null;
                SentLogErrorMessage(ex.Message + ex.StackTrace);
                return false;
            }


        }
        private string GetBasePath()
        {
            string fileName = basePath + "\\Zxtech.PdsCadWsOutsideControl.exe.config";
            if (!File.Exists(fileName)) return string.Empty;
            string configText = File.ReadAllText(fileName);
            string regexStr = "baseAddress=\"(.*)\"";
            var re = new Regex(regexStr, RegexOptions.IgnoreCase);
            Match mc = re.Match(configText);
            string baseAddress = "";
            string address = "";
            if (mc.Success)
            {
                baseAddress = mc.Groups[1].ToString();
            }
            else
            { return string.Empty; }

            regexStr = "\\s+Address=\"(\\w+)\"\\s+";
            re = new Regex(regexStr, RegexOptions.IgnoreCase);
            mc = re.Match(configText);
            if (mc.Success)
            {
                address = mc.Groups[1].ToString();
            }
            else
            { return string.Empty; }
            if (!string.IsNullOrEmpty(baseAddress) && !string.IsNullOrEmpty(address))
                return baseAddress + address;
            return string.Empty;

        }

        private bool IsAllowReloadProeWs()
        {
            try
            {
                string path = GetBasePath();
                if (string.IsNullOrEmpty(path))
                    path = @"net.tcp://localhost:530/CadService/Service";

                ServiceEndpoint NetEndpoint;
                var netTcpBinding = new NetTcpBinding();
                netTcpBinding.Security.Mode = SecurityMode.None;
                NetEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(ICadWsOutsideControl)), netTcpBinding, new EndpointAddress(path));

                ;
                cadWsFactory = new ChannelFactory<ICadWsOutsideControl>(NetEndpoint);



                //创造远程对象
                cadWs = cadWsFactory.CreateChannel();

                var IsAllowReloadProe = cadWs.IsAllowReloadProe();
                return IsAllowReloadProe;

            }
            catch (Exception ex)
            {
                // SendLogMessage("CAD 服务连接失败!");
                cadWs = null;
                return false;
            }

        }
        public string GetCurrentTaskParaList()
        {
            if (this.taskInfo != null)
                return this.taskInfo.ParaList;
            else return null;
        }

        private void FormCADTaskServer_Shown(object sender, EventArgs e)
        {
            //   if (IsCadWsAuto) this.Hide();
        }
        public T DeserializeDataContractFromStream<T>(Stream stream)
        {
            try
            {
                var dcs = new DataContractSerializer(typeof(T));

                var listSeries = (T)dcs.ReadObject(stream);
                stream.Close();
                return listSeries;

            }
            catch (Exception ex)
            {
                SentLogErrorMessage(ex.Message + ex.StackTrace);
                return default(T);
            }
        }
        /// <summary>
        /// 按属性名复制的方式，将对象转换为另一个对象。
        /// </summary>
        /// <param name="srcType">源类型</param>
        /// <param name="destType">目标类型</param>
        /// <param name="srcObject">源对象</param>
        /// <returns>目标对象</returns>
        public static object ConvertObject(Type srcType, Type destType, object srcObject)
        {
            var srcProps = srcType.GetProperties();

            var destObject = Activator.CreateInstance(destType);
            if (destObject == null)
            {
                throw new Exception("error data");
            }

            foreach (var prop in srcProps)
            {
                var destProp = destType.GetProperty(prop.Name);

                if (destProp != null)
                {
                    destProp.SetValue(destObject, prop.GetValue(srcObject, null), null);
                }
            }

            return destObject;
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

    }

    public class WCFServiceConfigInfo
    {
        public string EndpointUrl { get; set; }
        public string MaxArrayLength { get; set; }
        public string MaxBytesPerRead { get; set; }
        public string MaxDepth { get; set; }
        public string MaxNameTableCharCount { get; set; }
        public long MaxReceivedMessageSize { get; set; }
        public string MaxStringContentLength { get; set; }
        public string WCFServicePath { get; set; }
        public string AttachmentType { get; set; }

        public String PdsPath { get; set; }
        public String WorkPath { get; set; }
        public String BakPath { get; set; }

        public bool bRebuildNewInst { get; set; } // 新的实例
        public bool bRebuildBaseMode { get; set; } // 基本模型
        public bool bRebuildBaseModelInstTable { get; set; } // 基本模型的实例表
        public bool bIsBackupModel { get; set; }
        public int nFontStrokeMode { get; set; } // PDF输出字体
        public int CreateUserId { get; set; }
        public string WCFCADServer { get; set; }
        public bool bMakeDRW { get; set; }
        public bool bMakeDXF { get; set; }
        public bool bMakePDF { get; set; }
        public bool bMakeDWG { get; set; }
        public int nSelectTemplateId { get; set; }
        public bool IsHaveAttachment { get; set; }
        public String AttachmentSaveObject { get; set; }
    }



    /// <summary>
    /// 任务类型
    /// </summary>
    public struct TemplateRunTaskType
    {
        /// <summary>
        /// 开发测试
        /// </summary>
        public const int DesignTest = 100;

        /// <summary>
        /// 业务测试
        /// </summary>
        public const int BusinessTest = 200;

        /// <summary>
        /// 业务正式运行
        /// </summary>
        public const int BusinessFormal = 300;
        /// <summary>
        /// 转化为说明
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ConvertToString(int type)
        {
            switch (type)
            {
                case DesignTest:
                    return Properties.Resources.DesignTest;
                case BusinessTest:
                    return Properties.Resources.BusinessTest;
                case BusinessFormal:
                    return Properties.Resources.BusinessFormal;
                default:
                    return string.Empty;
            }
        }
    }

    public enum ServerState
    {
        Stop,
        Start,
        Exit
    }
}