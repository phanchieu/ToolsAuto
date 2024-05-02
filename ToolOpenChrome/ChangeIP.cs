using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToolOpenChrome
{
    public partial class ChangeIP : Form
    {
        public ChangeIP()
        {
            InitializeComponent();
        }

        private void btn_saveChangeIp_Click(object sender, EventArgs e)
        {
            // Tạo đối tượng dữ liệu
            var data = new { ChoseIP = "", Key_proxy = "", ChangeInfoPC = false, ChangeInfoMAC = false };
            if (ra_Chose_tmproxy.Checked)
            {
                data = new { ChoseIP = ra_Chose_tmproxy.Text, Key_proxy = rtxt_tmproxy.Text, ChangeInfoPC = cb_changeInfoPC.Checked, ChangeInfoMAC = cb_changeMAC.Checked };

            }
            else
            {
                data = new { ChoseIP = ra_Chose_wwproxy.Text, Key_proxy = rtxt_wwproxy.Text, ChangeInfoPC = cb_changeInfoPC.Checked, ChangeInfoMAC = cb_changeMAC.Checked };
            }

            // Chuyển đổi đối tượng thành chuỗi JSON
            var json = JsonConvert.SerializeObject(data);

            // đường dẫn và tên file cần lưu
            string appPath = Application.StartupPath;
            string settingFolderPath = Path.Combine(appPath, "setting");
            string dataFilePath = Path.Combine(settingFolderPath, "ChangeIP.json");

            // kiểm tra xem thư mục setting đã tồn tại hay chưa
            if (!Directory.Exists(settingFolderPath))
            {
                // nếu chưa tồn tại, tạo mới thư mục
                Directory.CreateDirectory(settingFolderPath);
            }

            // ghi dữ liệu vào file
            File.WriteAllText(dataFilePath, json);

            // Kiểm tra xem file đã lưu thành công hay chưa
            if (File.Exists(dataFilePath))
            {
                MessageBox.Show("Đã được lưu thành công!");
                //Console.WriteLine("File đã được lưu thành công!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi khi lưu!");
            }
        }

        private void btnCloseChangeIp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangeIP_Load(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Application.StartupPath, "setting", "ChangeIP.json");
            if (File.Exists(filePath))
            {
                // Đọc dữ liệu từ file
                string json = File.ReadAllText(filePath);

                // Chuyển đổi chuỗi JSON thành đối tượng JObject
                JObject jsonData = JObject.Parse(json);

                // Lấy giá trị của các thuộc tính
                if (jsonData.TryGetValue("ChoseIP", out JToken? choseIPToken))
                {
                    var choseIP = choseIPToken.ToString();
                    // Xử lý dữ liệu ở đây
                    //MessageBox.Show(choseIP);
                    //ra_Chose_tmproxy.Text = choseIP;
                    if(choseIP == "tmproxy.com")
                    {
                        ra_Chose_tmproxy.Checked = true;
                        if (jsonData.TryGetValue("Key_proxy", out JToken? Key_proxyToken))
                        {
                            var Key_proxy = Key_proxyToken.ToString();
                            // Xử lý dữ liệu ở đây
                            //MessageBox.Show(Key_proxy);
                            rtxt_tmproxy.Text = Key_proxy;
                        }
                    }
                    else
                    {
                        ra_Chose_wwproxy.Checked = true;
                        if (jsonData.TryGetValue("Key_proxy", out JToken? Key_proxyToken))
                        {
                            var Key_proxy = Key_proxyToken.ToString();
                            // Xử lý dữ liệu ở đây
                            //MessageBox.Show(Key_proxy);
                            rtxt_wwproxy.Text = Key_proxy;
                        }
                    }

                }
                if (jsonData.TryGetValue("ChangeInfoPC", out JToken? ChangeInfoPCToken))
                {
                    var ChangeInfoPC = bool.Parse(ChangeInfoPCToken.ToString());
                    // Xử lý dữ liệu ở đây
                    //MessageBox.Show(ChangeInfoPC.ToString());
                    cb_changeInfoPC.Checked = ChangeInfoPC;
                }
                if (jsonData.TryGetValue("ChangeInfoMAC", out JToken? ChangeInfoMACToken))
                {
                    var ChangeInfoMAC = bool.Parse(ChangeInfoMACToken.ToString());
                    // Xử lý dữ liệu ở đây
                    //MessageBox.Show(ChangeInfoMAC.ToString());
                    cb_changeMAC.Checked = ChangeInfoMAC;

                }
            }
        }
    }
}
