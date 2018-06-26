using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Zxtech.EdisService.Contract;
using Zxtech.PdsConstant;
using Zxtech.CADTaskServer.Properties;

namespace Zxtech.CADTaskServer
{
     partial class FormUserLogin : Form
    {

        private readonly string tempFileName = GetEdisTempDataDir() + @"\EdisUserInfo.temp";
        public bool IsLogin  { get; private set; }
        public PdsUser LoginUserInfo  { get; private set; }
         private IEdisDesignService CADDbConnect;
         private IEdisOnlineRunService CADDbOnLineConnect;
         public FormUserLogin(IEdisDesignService CADDbConnect, IEdisOnlineRunService CADDbOnLineConnect)
        {
            this.CADDbConnect = CADDbConnect;
            this.CADDbOnLineConnect = CADDbOnLineConnect;
            InitializeComponent();
        }
         //登录
         private bool Login()
         {
             string userName = comboBoxUser.Text.Trim();
             string passWord = textBoxPassword.Text.Trim();
             return Login(userName, passWord);
         
         }

         //登录
        public bool Login(string userName,string passWord)
        {
          
            string hostName = GetHostName();
            string hostIP = GetHostIpAddress();
            List<EdisOnlineUser> userInfoOnlines = CADDbOnLineConnect.GetOnlineUserList();
            if (userInfoOnlines != null)
            {
                foreach (var user in userInfoOnlines)
                {
                    if (user.UserName == userName)
                    {
                        MessageBox.Show( Resources.LoginRepeat);
                        return false;
                    }
                }
            }
            List<PdsUser> userInfos = CADDbConnect.GetUserList();
            foreach (var userInfo in userInfos)
            {
                
                if(userInfo.Name==userName)
                {
                    if (CADDbOnLineConnect.UserLogin(userInfo.Id, passWord, hostName, hostIP, ClientType.CadWorkStation))
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                     
                        LoginUserInfo = userInfo;
                        LoginUserInfo.Password = passWord;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(Resources.PassError);
                        return false;
                    }
                }
                //else
                //{
                //    MessageBox.Show("用户名错误!请重新输入");
                //    return false;
                //}
            }
            MessageBox.Show(Resources.UserNameError);

            return false;
        
        }
        public  static string GetHostName()
        {
            return Dns.GetHostName();
        }
         //得到主机IP地址
        public static  string GetHostIpAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addresses = ipEntry.AddressList;
            string ipAddress = null;
            for (int i = 0; i < addresses.Length; i++)
            {
                ipAddress += string.Format(" ,{0}", addresses[i]);
            }
            return ipAddress;
        }
         //登录
        private void buttonOK_Click(object sender, EventArgs e)
        {
            IsLogin = Login();
            if(IsLogin)
            {
                SaveUsedInfo();
                buttonOK.DialogResult = DialogResult.OK; 

            }
        }
         //查找用户
        private void buttonFindUser_Click(object sender, EventArgs e)
        {
            List<PdsUser> userInfos = CADDbConnect.GetUserList();
            List<PdsDepart> departInfos = CADDbConnect.GetDepartList();
            FormSelectUser formSelectUser= new FormSelectUser();
            formSelectUser.DepartList = departInfos;
            formSelectUser.UserList = userInfos;
            if(formSelectUser.ShowDialog(this)==System.Windows.Forms.DialogResult.OK)
            {
                this.comboBoxUser.Text = formSelectUser.SelectedUser.Name;
            }

        }
         //得到临时数据路径
        private  static string GetEdisTempDataDir()
        {
            string dir;
            dir = Environment.GetEnvironmentVariable("USERPROFILE") + @"\EdisCADTempData";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
         //保存用户信息
        private void SaveUsedInfo()
        {
         List<string> list = GetListFromComboBox(this.comboBoxUser);

            var info = new SUserInfo { ListUser = list};

            BinSerializeHelper<SUserInfo>.PackToFile(this.tempFileName, info);
        }

        private void FormUserLogin_Load(object sender, EventArgs e)
        {
            if (File.Exists(tempFileName))
            {
                var sInfo = BinSerializeHelper<SUserInfo>.UnPackFromFile(tempFileName);
                if (sInfo != null)
                {

                    if (sInfo.ListUser != null)
                    {
                        this.comboBoxUser.DataSource = sInfo.ListUser;

                        this.comboBoxUser.SelectedIndex = 0;
                        this.textBoxPassword.Focus();
                    }
                }
            }

        }
         //根据combobox变化list列表
       private static List<string> GetListFromComboBox(ComboBox cbo)
        {
            var list = new List<string>();

            string newStr = cbo.Text;

            if (cbo.Items.Count > 0)
            {
                list.AddRange(cbo.DataSource as List<string>);
            }

            list.Remove(newStr);
            list.Insert(0, newStr);

            if (list.Count > 8)
            {
                list.RemoveAt(list.Count - 1);
            }

            return list;
        }

       private void buttonCancel_Click(object sender, EventArgs e)
       {

       }
    }
    public struct TemplateRunTaskState
    {
        /// <summary>
        /// 新任务
        /// </summary>
        public const int NewTask = 100;
        /// <summary>
        /// 数据工作站正在运行
        /// </summary>
        public const int DataWsRuning = 200;
        /// <summary>
        /// 数据工作站已经完成任务
        /// </summary>
        public const int DataWsComplete = 300;
        /// <summary>
        /// CAD工作站正在运行
        /// </summary>
        public const int CadWsRuning = 400;
        /// <summary>
        /// CAD工作站完成任务
        /// </summary>
        public const int CadWsComplete = 500;
        /// <summary>
        /// 数据工作站不能完成这个任务
        /// </summary>
        public const int DataWsNoComplete = 600;
        /// <summary>
        /// CAD工作站不能完成这个任务
        /// </summary>
        public const int CadWsNoComplete = 700;

    }
      class BinSerializeHelper<TPacketType>
        where TPacketType : class
    {
        public static byte[] Pack(TPacketType obj)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);

                byte[] buf = new byte[stream.Length];
                Array.Copy(stream.GetBuffer(), buf, stream.Length);
                return buf;
            }
        }

        public static TPacketType UnPack(byte[] buf)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(buf))
            {
                TPacketType t = formatter.Deserialize(stream) as TPacketType;
                if (t == null)
                {
                    throw new ApplicationException(Resources.ExcepType);
                }

                return t;
            }
        }

        public static void PackToFile(string filePath, TPacketType obj)
        {
            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    formatter.Serialize(stream, obj);
                }
            }catch(Exception e)
            {
                
            }
        }

        public static TPacketType UnPackFromFile(string filePath)
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream   stream = new FileStream(filePath, FileMode.Open, FileAccess.Read); ;
            TPacketType t;
            try
            {
               
                 t = formatter.Deserialize(stream) as TPacketType;
                if (t == null)
                {
                    throw new ApplicationException(Resources.ExcepType);
                }
            }catch(Exception e)
            
            {
                return null;
            }
            finally
            {
                if(stream!=null)
                stream.Dispose();
            }

            return t;
            
        }
    }
      [Serializable]
       class SUserInfo
      {
          public List<string> ListUser { get; set; }
      }
}
