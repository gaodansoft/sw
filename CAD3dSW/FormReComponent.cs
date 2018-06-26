using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace CAD3dSW
{
    public partial class FormReComponent : Form
    {

        private List<List<string>> lsResult;

        public FormReComponent()
        {
            InitializeComponent();
            lsResult = new List<List<string>>();
        }

        private void bunSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Microsoft Access 文件(*.mdb)|*.mdb|所有文件(*.*)|*.*";
            dlg.ShowDialog();

            txbPath.Text = dlg.FileName;

            ReadBOM();
            RefreshList();

        }

        private void RefreshList()
        {
            lsvComp.Items.Clear();

            for (int i = 0; i < lsResult.Count; i++)
            {
                ListViewItem lvItem = lsvComp.Items.Add((i + 1).ToString());
                string oldName = lsResult[i][0] + "|"+ lsResult[i][1];

                lvItem.SubItems.Add(oldName);
                lvItem.SubItems.Add(lsResult[i][2]);
            }
        }

        private void bunCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunOK_Click(object sender, EventArgs e)
        {           
            this.Close();
        }

        public object GetBOMData()
        {
            return lsResult;
        }

        private void ReadBOM()
        {
            if (string.IsNullOrEmpty(txbPath.Text)) return;

            OleDbConnection con1 = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", txbPath.Text));
            OleDbCommand cmd = new OleDbCommand("SELECT OLD_MDL_NAME,NEW_MDL_NAME FROM PT_QUERY_TASK_BOM", con1);
                        

            con1.Open();
            using (OleDbDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    List<string> ls = new List<string>();

                    string part = dr["OLD_MDL_NAME"].ToString();
                    string[] arr = part.Split('|');

                    if (arr.Length > 1)
                    {
                        ls.Add(Path.GetFileName(arr[0]));
                        ls.Add(arr[1]);
                    }
                    else
                    {
                        ls.Add( part);
                        ls.Add("");
                    }
                    
                    ls.Add(dr["NEW_MDL_NAME"].ToString());
                    lsResult.Add(ls);
                }
            }
            con1.Close();
        }
    }
}
