namespace Zxtech.CADTaskServer
{
    partial class FormWorkStation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWorkStation));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvWs = new System.Windows.Forms.ListView();
            this.chIndex = new System.Windows.Forms.ColumnHeader();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.chWsID = new System.Windows.Forms.ColumnHeader();
            this.chGetTaskCycle = new System.Windows.Forms.ColumnHeader();
            this.chUpdatelimit = new System.Windows.Forms.ColumnHeader();
            this.chIsLogin = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnStart = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.lvWs);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lvWs
            // 
            resources.ApplyResources(this.lvWs, "lvWs");
            this.lvWs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chIndex,
            this.chName,
            this.chWsID,
            this.chGetTaskCycle,
            this.chUpdatelimit,
            this.chIsLogin});
            this.lvWs.FullRowSelect = true;
            this.lvWs.GridLines = true;
            this.lvWs.HideSelection = false;
            this.lvWs.Name = "lvWs";
            this.lvWs.SmallImageList = this.imageList1;
            this.lvWs.UseCompatibleStateImageBehavior = false;
            this.lvWs.View = System.Windows.Forms.View.Details;
            this.lvWs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvWs_MouseDoubleClick);
            this.lvWs.SelectedIndexChanged += new System.EventHandler(this.lvWs_SelectedIndexChanged);
            // 
            // chIndex
            // 
            resources.ApplyResources(this.chIndex, "chIndex");
            // 
            // chName
            // 
            resources.ApplyResources(this.chName, "chName");
            // 
            // chWsID
            // 
            resources.ApplyResources(this.chWsID, "chWsID");
            // 
            // chGetTaskCycle
            // 
            resources.ApplyResources(this.chGetTaskCycle, "chGetTaskCycle");
            // 
            // chUpdatelimit
            // 
            resources.ApplyResources(this.chUpdatelimit, "chUpdatelimit");
            // 
            // chIsLogin
            // 
            resources.ApplyResources(this.chIsLogin, "chIsLogin");
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imageList1, "imageList1");
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormWorkStation
            // 
            this.AcceptButton = this.btnStart;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormWorkStation";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.FormWorkStation_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvWs;
        private System.Windows.Forms.ColumnHeader chIndex;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ColumnHeader chWsID;
        private System.Windows.Forms.ColumnHeader chGetTaskCycle;
        private System.Windows.Forms.ColumnHeader chUpdatelimit;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ColumnHeader chIsLogin;
      
    }
}