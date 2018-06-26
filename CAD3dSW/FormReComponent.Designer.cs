namespace CAD3dSW
{
    partial class FormReComponent
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
            this.bunSelect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txbPath = new System.Windows.Forms.TextBox();
            this.bunOK = new System.Windows.Forms.Button();
            this.bunCancel = new System.Windows.Forms.Button();
            this.lsvComp = new System.Windows.Forms.ListView();
            this.colIndex = new System.Windows.Forms.ColumnHeader();
            this.colOldName = new System.Windows.Forms.ColumnHeader();
            this.colNewName = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // bunSelect
            // 
            this.bunSelect.Location = new System.Drawing.Point(491, 13);
            this.bunSelect.Name = "bunSelect";
            this.bunSelect.Size = new System.Drawing.Size(51, 23);
            this.bunSelect.TabIndex = 0;
            this.bunSelect.Text = ">>";
            this.bunSelect.UseVisualStyleBackColor = true;
            this.bunSelect.Click += new System.EventHandler(this.bunSelect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "BOM表数据";
            // 
            // txbPath
            // 
            this.txbPath.Location = new System.Drawing.Point(87, 15);
            this.txbPath.Name = "txbPath";
            this.txbPath.Size = new System.Drawing.Size(398, 21);
            this.txbPath.TabIndex = 2;
            // 
            // bunOK
            // 
            this.bunOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bunOK.Location = new System.Drawing.Point(220, 357);
            this.bunOK.Name = "bunOK";
            this.bunOK.Size = new System.Drawing.Size(75, 23);
            this.bunOK.TabIndex = 3;
            this.bunOK.Text = "确定";
            this.bunOK.UseVisualStyleBackColor = true;
            this.bunOK.Click += new System.EventHandler(this.bunOK_Click);
            // 
            // bunCancel
            // 
            this.bunCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bunCancel.Location = new System.Drawing.Point(323, 357);
            this.bunCancel.Name = "bunCancel";
            this.bunCancel.Size = new System.Drawing.Size(75, 23);
            this.bunCancel.TabIndex = 4;
            this.bunCancel.Text = "取消";
            this.bunCancel.UseVisualStyleBackColor = true;
            this.bunCancel.Click += new System.EventHandler(this.bunCancel_Click);
            // 
            // lsvComp
            // 
            this.lsvComp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIndex,
            this.colOldName,
            this.colNewName});
            this.lsvComp.FullRowSelect = true;
            this.lsvComp.GridLines = true;
            this.lsvComp.Location = new System.Drawing.Point(12, 54);
            this.lsvComp.Name = "lsvComp";
            this.lsvComp.Size = new System.Drawing.Size(530, 285);
            this.lsvComp.TabIndex = 5;
            this.lsvComp.UseCompatibleStateImageBehavior = false;
            this.lsvComp.View = System.Windows.Forms.View.Details;
            // 
            // colIndex
            // 
            this.colIndex.Text = "序号";
            // 
            // colOldName
            // 
            this.colOldName.Text = "旧名";
            this.colOldName.Width = 273;
            // 
            // colNewName
            // 
            this.colNewName.Text = "新名";
            this.colNewName.Width = 312;
            // 
            // FormReComponent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 392);
            this.Controls.Add(this.lsvComp);
            this.Controls.Add(this.bunCancel);
            this.Controls.Add(this.bunOK);
            this.Controls.Add(this.txbPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bunSelect);
            this.Name = "FormReComponent";
            this.Text = "重命名组件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bunSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbPath;
        private System.Windows.Forms.Button bunOK;
        private System.Windows.Forms.Button bunCancel;
        private System.Windows.Forms.ListView lsvComp;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colOldName;
        private System.Windows.Forms.ColumnHeader colNewName;
    }
}