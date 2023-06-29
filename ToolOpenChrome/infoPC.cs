using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolOpenChrome
{
    public partial class infoPC : Form
    {
        public infoPC()
        {
            InitializeComponent();
        }

        public void ReloadInfo()
        {
            // Lấy tên máy tính
            string computerName = Environment.MachineName;

            // Lấy tên người dùng hiện tại
            string userName = Environment.UserName;

            // Lấy thư mục gốc của hệ thống
            string systemDirectory = Environment.SystemDirectory;

            // Lấy thư mục ứng dụng mặc định của người dùng hiện tại
            string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Lấy địa chỉ MAC của card mạng đầu tiên
            string macAddress = "";
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    PhysicalAddress address = adapter.GetPhysicalAddress();
                    byte[] bytes = address.GetAddressBytes();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        macAddress += bytes[i].ToString("X2");
                        if (i != bytes.Length - 1)
                        {
                            macAddress += ":";
                        }
                    }
                    break;
                }
            }

            txtNamePc.Text = computerName;
            txtUserPc.Text = userName;
            txtFolderPc.Text = systemDirectory;
            txtFolderApp.Text = userDirectory;
            txtMACPc.Text = macAddress;
        }
        private void infoPC_Load(object sender, EventArgs e)
        {
            ReloadInfo();
        }

        private void btnReloadInfoPc_Click(object sender, EventArgs e)
        {
            ReloadInfo();
        }

    }
}
