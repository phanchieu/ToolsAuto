using Newtonsoft.Json.Linq;
using System.Management;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ToolOpenChrome
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            getCodePc();
            checkFileKey();
        }
        private void getCodePc()
        {
            string? machineCode = "";

            // Create management class object
            ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");

            // Get the system UUID
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                machineCode = mo.Properties["UUID"].Value != null ? mo.Properties["UUID"].Value.ToString() : string.Empty;
                break;
            }
            if (machineCode != null && machineCode.Length >= 1)
            {
                //machineCode = machineCode.Substring(0, Math.Min(10, machineCode.Length));
                txtCodePc.Text = machineCode;
                codePc = machineCode;
            }

        }
        string codePc;
        string username;
        string password;
        private async void sendRequest()
        {
            try
            {
                // Set URL endpoint
                string url = "https://superauto.click/api/v1/key/login";

                // Set POST data (if any)
                var postData = new StringContent($"{{\"username\":\"{username}\", \"password\":\"{password}\"}}", Encoding.UTF8, "application/json");

                // Send HTTP POST request
                string response = await SendHttpPostRequest(url, postData);
                dynamic res = JObject.Parse(response);
                int err = (int)res.err;
                if (err == 0)
                //MessageBox.Show(err.ToString());
                {
                    string data_codePc = res.mes.codePc;
                    bool data_autoBasicShopee = res.mes.autoBasicShopee;

                    // Display response in a label or text box
                    if (data_autoBasicShopee)
                    {
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng đăng nhập đúng với tài khoản được cung cấp");
                    }
                }
                else
                {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không đúng");
                }
            }
            catch
            {
                MessageBox.Show("vui lòng thử lại hoặc liên hệ người hỗ trợ cài đặt");
            }
        }
        private async Task<string> SendHttpPostRequest(string url, StringContent postData)
        {
            // Create new HttpClient instance
            HttpClient httpClient = new HttpClient();

            // Send HTTP POST request and receive response
            HttpResponseMessage httpResponse = await httpClient.PostAsync(url, postData);

            // Read response content as string
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            // Return response content
            return responseContent;
        }
        // btn Login
        private void checkFileKey()
        {
            string filePath = "data/key.txt";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                txtTK.Text = lines[0];
                txtMK.Text = lines[1];
                username = txtTK.Text;
                password = txtMK.Text;
                // Sử dụng tài khoản và mật khẩu
                sendRequest();
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtTK.Text.Length > 0 && txtMK.Text.Length > 0)
            {
                username = txtTK.Text;
                password = txtMK.Text;

                string directoryPath = Path.Combine(Application.StartupPath, "data");
                string filePath = Path.Combine(directoryPath, "key.txt");
                string usernameCreate = txtTK.Text;
                string passwordCreate = txtMK.Text;

                // Kiểm tra xem thư mục đã tồn tại hay chưa
                if (!Directory.Exists(directoryPath))
                {
                    // Tạo thư mục mới nếu chưa tồn tại
                    Directory.CreateDirectory(directoryPath);
                }

                string[] lines = new string[] { usernameCreate, passwordCreate };
                File.WriteAllLines(filePath, lines);
                sendRequest();
            }
            else { MessageBox.Show("vui lòng nhập tài khoản và mật khẩu"); }
        }

        private void txtCodePc_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCopyCode_Click(object sender, EventArgs e)
        {
            // Thiết lập thuộc tính ReadOnly và TabStop
            txtCodePc.ReadOnly = true;
            txtCodePc.TabStop = false;

            // Chọn toàn bộ nội dung của TextBox
            txtCodePc.SelectAll();

            // Sao chép nội dung đã chọn vào clipboard
            Clipboard.SetText(txtCodePc.SelectedText);

            // Đặt lại thuộc tính ReadOnly và TabStop
            txtCodePc.ReadOnly = false;
            txtCodePc.TabStop = true;
            MessageBox.Show($"Đã copy: {txtCodePc.Text}");
        }
    }
}
