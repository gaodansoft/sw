namespace Zxtech.CADTaskServer
{
 public   partial class FormCADTaskServer
    {

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCADTaskServer));
            this.rtbRunInfo = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLableWorkStation = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripWorkStation = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTaskType = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTaskType = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripServerConnect = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGetTask = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonGetTaskSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.labelInfo = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbRunInfo
            // 
            this.rtbRunInfo.AcceptsTab = true;
            this.rtbRunInfo.AccessibleDescription = null;
            this.rtbRunInfo.AccessibleName = null;
            resources.ApplyResources(this.rtbRunInfo, "rtbRunInfo");
            this.rtbRunInfo.BackColor = System.Drawing.SystemColors.Window;
            this.rtbRunInfo.BackgroundImage = null;
            this.rtbRunInfo.Font = null;
            this.rtbRunInfo.Name = "rtbRunInfo";
            this.rtbRunInfo.ReadOnly = true;
            this.rtbRunInfo.TextChanged += new System.EventHandler(this.rtbRunInfo_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AccessibleDescription = null;
            this.statusStrip1.AccessibleName = null;
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.BackgroundImage = null;
            this.statusStrip1.Font = null;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelUserName,
            this.toolStripUserName,
            this.toolStripStatusLableWorkStation,
            this.toolStripWorkStation,
            this.toolStripStatusLabelTaskType,
            this.toolStripTaskType,
            this.toolStripStatusLabel1,
            this.toolStripServerConnect});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabelUserName
            // 
            this.toolStripStatusLabelUserName.AccessibleDescription = null;
            this.toolStripStatusLabelUserName.AccessibleName = null;
            resources.ApplyResources(this.toolStripStatusLabelUserName, "toolStripStatusLabelUserName");
            this.toolStripStatusLabelUserName.BackgroundImage = null;
            this.toolStripStatusLabelUserName.Name = "toolStripStatusLabelUserName";
            // 
            // toolStripUserName
            // 
            this.toolStripUserName.AccessibleDescription = null;
            this.toolStripUserName.AccessibleName = null;
            resources.ApplyResources(this.toolStripUserName, "toolStripUserName");
            this.toolStripUserName.BackgroundImage = null;
            this.toolStripUserName.Name = "toolStripUserName";
            // 
            // toolStripStatusLableWorkStation
            // 
            this.toolStripStatusLableWorkStation.AccessibleDescription = null;
            this.toolStripStatusLableWorkStation.AccessibleName = null;
            resources.ApplyResources(this.toolStripStatusLableWorkStation, "toolStripStatusLableWorkStation");
            this.toolStripStatusLableWorkStation.BackgroundImage = null;
            this.toolStripStatusLableWorkStation.Name = "toolStripStatusLableWorkStation";
            // 
            // toolStripWorkStation
            // 
            this.toolStripWorkStation.AccessibleDescription = null;
            this.toolStripWorkStation.AccessibleName = null;
            resources.ApplyResources(this.toolStripWorkStation, "toolStripWorkStation");
            this.toolStripWorkStation.BackgroundImage = null;
            this.toolStripWorkStation.Name = "toolStripWorkStation";
            this.toolStripWorkStation.Spring = true;
            // 
            // toolStripStatusLabelTaskType
            // 
            this.toolStripStatusLabelTaskType.AccessibleDescription = null;
            this.toolStripStatusLabelTaskType.AccessibleName = null;
            resources.ApplyResources(this.toolStripStatusLabelTaskType, "toolStripStatusLabelTaskType");
            this.toolStripStatusLabelTaskType.BackgroundImage = null;
            this.toolStripStatusLabelTaskType.Name = "toolStripStatusLabelTaskType";
            // 
            // toolStripTaskType
            // 
            this.toolStripTaskType.AccessibleDescription = null;
            this.toolStripTaskType.AccessibleName = null;
            resources.ApplyResources(this.toolStripTaskType, "toolStripTaskType");
            this.toolStripTaskType.BackgroundImage = null;
            this.toolStripTaskType.Name = "toolStripTaskType";
            this.toolStripTaskType.Spring = true;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AccessibleDescription = null;
            this.toolStripStatusLabel1.AccessibleName = null;
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.BackgroundImage = null;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // toolStripServerConnect
            // 
            this.toolStripServerConnect.AccessibleDescription = null;
            this.toolStripServerConnect.AccessibleName = null;
            resources.ApplyResources(this.toolStripServerConnect, "toolStripServerConnect");
            this.toolStripServerConnect.BackgroundImage = null;
            this.toolStripServerConnect.Name = "toolStripServerConnect";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AccessibleDescription = null;
            this.toolStrip1.AccessibleName = null;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackgroundImage = null;
            this.toolStrip1.Font = null;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnStop,
            this.toolStripSeparator1,
            this.btnExit,
            this.toolStripButtonGetTask,
            this.toolStripSeparator2,
            this.toolStripSeparator3,
            this.toolStripButtonGetTaskSelect,
            this.toolStripSeparator4});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // btnStart
            // 
            this.btnStart.AccessibleDescription = null;
            this.btnStart.AccessibleName = null;
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.BackgroundImage = null;
            this.btnStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStart.Name = "btnStart";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.AccessibleDescription = null;
            this.btnStop.AccessibleName = null;
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.BackgroundImage = null;
            this.btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStop.Name = "btnStop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // btnExit
            // 
            this.btnExit.AccessibleDescription = null;
            this.btnExit.AccessibleName = null;
            this.btnExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.BackgroundImage = null;
            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExit.Name = "btnExit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // toolStripButtonGetTask
            // 
            this.toolStripButtonGetTask.AccessibleDescription = null;
            this.toolStripButtonGetTask.AccessibleName = null;
            resources.ApplyResources(this.toolStripButtonGetTask, "toolStripButtonGetTask");
            this.toolStripButtonGetTask.BackgroundImage = null;
            this.toolStripButtonGetTask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonGetTask.Name = "toolStripButtonGetTask";
            this.toolStripButtonGetTask.Click += new System.EventHandler(this.toolStripButtonGetTask_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AccessibleDescription = null;
            this.toolStripSeparator2.AccessibleName = null;
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AccessibleDescription = null;
            this.toolStripSeparator3.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // toolStripButtonGetTaskSelect
            // 
            this.toolStripButtonGetTaskSelect.AccessibleDescription = null;
            this.toolStripButtonGetTaskSelect.AccessibleName = null;
            resources.ApplyResources(this.toolStripButtonGetTaskSelect, "toolStripButtonGetTaskSelect");
            this.toolStripButtonGetTaskSelect.BackgroundImage = null;
            this.toolStripButtonGetTaskSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonGetTaskSelect.Name = "toolStripButtonGetTaskSelect";
            this.toolStripButtonGetTaskSelect.Click += new System.EventHandler(this.toolStripButtonGetTaskSelect_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AccessibleDescription = null;
            this.toolStripSeparator4.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // labelInfo
            // 
            this.labelInfo.AccessibleDescription = null;
            this.labelInfo.AccessibleName = null;
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Font = null;
            this.labelInfo.Name = "labelInfo";
            // 
            // FormCADTaskServer
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.rtbRunInfo);
            this.Font = null;
            this.Icon = null;
            this.Name = "FormCADTaskServer";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.FormCADTaskServer_Load);
            this.Shown += new System.EventHandler(this.FormCADTaskServer_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCADTaskServer_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbRunInfo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelUserName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripUserName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLableWorkStation;
        private System.Windows.Forms.ToolStripStatusLabel toolStripWorkStation;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripButton btnExit;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTaskType;
        private System.Windows.Forms.ToolStripStatusLabel toolStripTaskType;
        private System.Windows.Forms.ToolStripButton toolStripButtonGetTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripServerConnect;
        private System.Windows.Forms.ToolStripButton toolStripButtonGetTaskSelect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
       
    }
}