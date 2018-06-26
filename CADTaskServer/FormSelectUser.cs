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

    public partial class FormSelectUser : Form
    {
        public FormSelectUser()
        {
            InitializeComponent();
        }

        private List<PdsUser> userList;
        private List<PdsDepart> departList;
        private PdsUser selectedUser;


        public List<PdsUser> UserList
        {
            get
            {
                if (userList == null)
                {
                    throw new InvalidOperationException("PdsILoginSer:Zxtech.PdsILoginSer:FormSelectUser:userList is null");
                }

                return this.userList;
            }
            set { this.userList = value; }
        }

        public List<PdsDepart> DepartList
        {
            get
            {
                if (departList == null)
                {
                    throw new InvalidOperationException("PdsILoginSer:Zxtech.PdsILoginSer:FormSelectUser:departList is null");
                }
                return this.departList;
            }
            set { this.departList = value; }
        }

        public PdsUser SelectedUser
        {
            get { return this.selectedUser; }
        }

        private void FormSelectUser_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < DepartList.Count; i++)
            {
                var list = DepartList[i];
                TreeNode treeNode = new TreeNode(list.Name);
                treeNode.Tag = GetDepartUserList(list.Id);

                treeViewDepart.Nodes.Add(treeNode);
            }
        }
        //得到部门列表
        private List<PdsUser> GetDepartUserList(int departId)
        {
            var list = new List<PdsUser>();
            for (int i = 0; i < this.UserList.Count; i++)
            {
                var user = this.UserList[i];
                if (user.DepartId == departId)
                {
                    list.Add(user);
                }
            }
            return list;
        }

        private void treeViewDepart_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var list = e.Node.Tag as List<PdsUser>;
            if (list != null)
            {
                listViewUser.Items.Clear();
                foreach (var user in list)
                {
                    var item = new ListViewItem();

                    item.Tag = user;

                    for (int n = 0; n < listViewUser.Columns.Count - 1; n++)
                    {
                        item.SubItems.Add("");
                    }

                    this.listViewUser.Items.Add(item);
                    SetListItemText(item);
                }
            }
        }
        private void SetListItemText(ListViewItem item)
        {
            var itemData = item.Tag as PdsUser;
            if (itemData != null)
            {
                item.Text = (item.Index + 1).ToString();
                item.SubItems[this.columnHeaderUser.Index].Text = itemData.Name;
            }
        }

        private void listViewUser_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listViewUser.SelectedItems.Count <= 0)
            {
                return;
            }

            this.selectedUser = this.listViewUser.SelectedItems[0].Tag as PdsUser;


            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
