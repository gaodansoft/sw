using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Zxtech.EdisService.Contract;
using System.Net;
using Zxtech.CADTaskServer.Properties;

namespace Zxtech.CADTaskServer

{
     [SerializableAttribute]
       partial class FormWorkStation : Form
    {
        private IEdisDesignService CADDbConnect;
        private IEdisOnlineRunService CADDbOnLineConnect;
        private List<WorkStationInfo> wsList;
        private List<PdsOnlineWorkStation> onlineWsList;
        public delegate void DelegateCADRun(WorkStationInfo wsInfo);
        public DelegateCADRun CADRun;
        public FormWorkStation(IEdisDesignService CADDbConnect, IEdisOnlineRunService CADDbOnLineConnect)
        {
            InitializeComponent();
            this.CADDbConnect = CADDbConnect;
            this.CADDbOnLineConnect = CADDbOnLineConnect;
        }
         //启动
        private void btnStart_Click(object sender, EventArgs e)
        {
            WorkStationInfo wsi = lvWs.SelectedItems[0].Tag as WorkStationInfo;
            CADRun(wsi);

        }

        private void FormWorkStation_Load(object sender, EventArgs e)
        {
          //  wsList = CADDbConnect.GetWorkStationListByNoLogin(WorkStationType.Cad);
            wsList = CADDbConnect.GetWorkStationList(WorkStationType.Cad);
            onlineWsList = CADDbOnLineConnect.GetOnlineWorkStationList(WorkStationType.Cad);
           
            int index = 0;
            foreach (WorkStationInfo wsInfo in wsList)
            {

                ListViewItem lvi = new ListViewItem();
                lvi.Tag = wsInfo;

                for (int i = 0; i < lvWs.Columns.Count; i++)
                {
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem());
                }
                lvi.SubItems.Add(wsInfo.Name);

                if (!IsOnline(wsInfo))
                {

                    lvWs.Items.Add(lvi);

                    SetListItemText(lvi);
                }
                //ListViewItem lvi = new ListViewItem();
                //lvi.Text = index++.ToString();
                //lvi.Tag = wsInfo;
                //lvi.SubItems.Add(wsInfo.Name);
                //lvWs.Items.Add(lvi);
            }
        }
        private void SetListItemText(ListViewItem item)
        {
            var listItem = item.Tag as WorkStationInfo;

            int n = item.Index + 1;

            item.Text = string.Format("{0}", n);

            if (listItem != null)
            {
                item.SubItems[this.chName.Index].Text = listItem.Name;
                item.SubItems[this.chWsID.Index].Text = string.Format("{0}", listItem.WSId);
                item.SubItems[this.chGetTaskCycle.Index].Text = string.Format("{0}", listItem.GetTaskCycle);
                item.SubItems[this.chUpdatelimit.Index].Text = string.Format("{0}", listItem.UpdateTimelimit);
                item.SubItems[this.chIsLogin.Index].Text = string.Format("{0}", IsOnline(listItem.WSId) ? Resources.OnlineStared: Resources.OnlineNoStart);
                listItem.IsLogin = IsOnline(listItem.WSId);
            }
        }

        private void lvWs_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool seleced = lvWs.SelectedItems.Count > 0;
            btnStart.Enabled = seleced;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //this.Close();
        }

        private void lvWs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if( lvWs.SelectedItems.Count > 0)
            {
            WorkStationInfo wsi = lvWs.SelectedItems[0].Tag as WorkStationInfo;
            CADRun(wsi);
            this.DialogResult = DialogResult.OK;
            }

        }
         //是否在线
        private bool IsOnline(int wsId)
        {
            foreach (var ws in onlineWsList)
            {
                if (ws.WSId == wsId) return true;
            }
            return false;
        
        
        }
        //是否在线
        private bool IsOnline(WorkStationInfo info)
        {
            foreach (var ws in onlineWsList)
            {
                if (ws.WSId == info.WSId &&!( ws.HostName == Dns.GetHostName()))
                {

                    return true ;
                }
           
            
            
            }
            return false;
        
        
        
        }

      


      
    }
     [SerializableAttribute]
    public struct WorkStationType
    {
        /// <summary>
        /// 数据工作站
        /// </summary>
        public const int Data = 100;
        /// <summary>
        /// CAD工作站
        /// </summary>
        public const int Cad = 200;
    }
}
