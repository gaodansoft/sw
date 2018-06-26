namespace Zxtech.CADTaskServer
{
    partial class FormTaskInfoSelect
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listViewTaskInfo = new System.Windows.Forms.ListView();
            this.chId = new System.Windows.Forms.ColumnHeader();
            this.chTaskName = new System.Windows.Forms.ColumnHeader();
            this.chContractName = new System.Windows.Forms.ColumnHeader();
            this.chTemplateName = new System.Windows.Forms.ColumnHeader();
            this.chCreator = new System.Windows.Forms.ColumnHeader();
            this.chCreateTime = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.listViewTaskInfo);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(734, 281);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "任务列表";
            // 
            // listViewTaskInfo
            // 
            this.listViewTaskInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chId,
            this.chTaskName,
            this.chContractName,
            this.chTemplateName,
            this.chCreator,
            this.chCreateTime});
            this.listViewTaskInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTaskInfo.FullRowSelect = true;
            this.listViewTaskInfo.GridLines = true;
            this.listViewTaskInfo.HideSelection = false;
            this.listViewTaskInfo.Location = new System.Drawing.Point(3, 17);
            this.listViewTaskInfo.MultiSelect = false;
            this.listViewTaskInfo.Name = "listViewTaskInfo";
            this.listViewTaskInfo.Size = new System.Drawing.Size(728, 261);
            this.listViewTaskInfo.SmallImageList = this.imageList1;
            this.listViewTaskInfo.TabIndex = 0;
            this.listViewTaskInfo.UseCompatibleStateImageBehavior = false;
            this.listViewTaskInfo.View = System.Windows.Forms.View.Details;
            this.listViewTaskInfo.SelectedIndexChanged += new System.EventHandler(this.listViewTaskInfo_SelectedIndexChanged);
            // 
            // chId
            // 
            this.chId.Text = "序号";
            // 
            // chTaskName
            // 
            this.chTaskName.Text = "任务名称";
            this.chTaskName.Width = 108;
            // 
            // chContractName
            // 
            this.chContractName.Text = "任务标识";
            this.chContractName.Width = 116;
            // 
            // chTemplateName
            // 
            this.chTemplateName.Text = "模板名称";
            this.chTemplateName.Width = 149;
            // 
            // chCreator
            // 
            this.chCreator.Text = "创建者";
            this.chCreator.Width = 114;
            // 
            // chCreateTime
            // 
            this.chCreateTime.Text = "创建时间";
            this.chCreateTime.Width = 161;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(536, 299);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "提取";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(645, 299);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "关闭";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FormTaskInfoSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 334);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormTaskInfoSelect";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "手动提取任务(可选)";
            this.Load += new System.EventHandler(this.FormTaskInfoSelect_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView listViewTaskInfo;
        private System.Windows.Forms.ColumnHeader chId;
        private System.Windows.Forms.ColumnHeader chTaskName;
        private System.Windows.Forms.ColumnHeader chContractName;
        private System.Windows.Forms.ColumnHeader chTemplateName;
        private System.Windows.Forms.ColumnHeader chCreator;
        private System.Windows.Forms.ColumnHeader chCreateTime;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}