using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Zxtech.EdisService.Contract;

namespace Zxtech.CADTaskServer
{
    public partial class FormTaskInfoSelect : Form
    {
        private List<PdsQueryTaskInfo> taskInfos;
        public PdsQueryTaskInfo selectedTaskInfo { get; private set; }
        private  readonly IEdisDesignService CADDbConnect;
        private List<PdsUser> users;
        public FormTaskInfoSelect(IEdisDesignService CADDbConnect, List<PdsQueryTaskInfo> taskInfos)
        {
            this.CADDbConnect = CADDbConnect;
            this.taskInfos = taskInfos;
            InitializeComponent();
        }

        private void FormTaskInfoSelect_Load(object sender, EventArgs e)
        {
            if (this.taskInfos == null) return;
             users=CADDbConnect.GetUserList();
            for (int i = 0; i < this.taskInfos.Count; i++)
            {
                var lvi = new ListViewItem();
                lvi.Tag = taskInfos[i];
                for (int n = 0; n < this.listViewTaskInfo.Columns.Count - 1; n++)
                {
                    lvi.SubItems.Add("");
                }
                this.listViewTaskInfo.Items.Add(lvi);
                SetListItemText(i);
              
            }
        }

        private void SetListItemText(int index)
        {
             int i = index + 1;
             var taskInfo = this.listViewTaskInfo.Items[index].Tag as PdsQueryTaskInfo;
            if (taskInfo != null)
            {
               
                ListViewItem lvi = this.listViewTaskInfo.Items[index];
                lvi.SubItems[this.chId.Index].Text = i.ToString();
                lvi.SubItems[this.chTaskName.Index].Text = taskInfo.TaskName;
                lvi.SubItems[this.chCreateTime.Index].Text = taskInfo.CreateTime.ToString();
                lvi.SubItems[this.chTemplateName.Index].Text = taskInfo.NameTemplate + "(" + taskInfo.TemplateId+ ")"; 
                lvi.SubItems[this.chContractName.Index].Text = taskInfo.TaskShowName;
                PdsUser item = this.users.Find(p => p.Id == taskInfo.Creator);
                if (item != null)
                {
                    lvi.SubItems[this.chCreator.Index].Text = item.Name;
                }
                else
                {
                    lvi.SubItems[this.chCreator.Index].Text = "";
                }

            }
        }
        //提取
        private void buttonOK_Click(object sender, EventArgs e)
        {
          if(  this.listViewTaskInfo.SelectedItems.Count <= 0)return;
          this.selectedTaskInfo = this.listViewTaskInfo.SelectedItems[0].Tag as PdsQueryTaskInfo;


        }

        private void listViewTaskInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewTaskInfo.SelectedItems.Count > 0)
                this.buttonOK.Enabled = true;
            else
                this.buttonOK.Enabled = false;
        }
    }
}
