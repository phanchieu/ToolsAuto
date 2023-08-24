using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;
using System.Security.Policy;
using System.Runtime.InteropServices;
using static ToolOpenChrome.FormMain;
using System.Security.Cryptography;
using OpenQA.Selenium.Support.UI;
using System.Security.Principal;
using System.Configuration;
using OpenQA.Selenium.Interactions;
using System.Globalization;
using System;
using System.IO;
using System.CodeDom.Compiler;

namespace ToolOpenChrome
{
    public partial class FormMain : Form
    {

        /// <summary>
        /// Represents a Windows network interface. Wrapper around the .NET API for network
        /// interfaces, as well as for the unmanaged device.
        /// </summary>
        public class Adapter
        {
            public ManagementObject adapter;
            public string adaptername;
            public string customname;
            public int devnum;

            public Adapter(ManagementObject a, string aname, string cname, int n)
            {
                this.adapter = a;
                this.adaptername = aname;
                this.customname = cname;
                this.devnum = n;
            }

            public Adapter(NetworkInterface i) : this(i.Description) { }

            public Adapter(string aname)
            {
                this.adaptername = aname;

                var searcher = new ManagementObjectSearcher("select * from win32_networkadapter where Name='" + adaptername + "'");
                var found = searcher.Get();
                this.adapter = found.Cast<ManagementObject>().FirstOrDefault();

                // Extract adapter number; this should correspond to the keys under
                // HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}
                try
                {
                    var match = Regex.Match(adapter.Path.RelativePath, "\\\"(\\d+)\\\"$");
                    this.devnum = int.Parse(match.Groups[1].Value);
                }
                catch
                {
                    return;
                }

                // Find the name the user gave to it in "Network Adapters"
                this.customname = NetworkInterface.GetAllNetworkInterfaces().Where(
                    i => i.Description == adaptername
                ).Select(
                    i => " (" + i.Name + ")"
                ).FirstOrDefault();
            }

            /// <summary>
            /// Get the .NET managed adapter.
            /// </summary>
            public NetworkInterface ManagedAdapter
            {
                get
                {
                    return NetworkInterface.GetAllNetworkInterfaces().Where(
                        nic => nic.Description == this.adaptername
                    ).FirstOrDefault();
                }
            }

            /// <summary>
            /// Get the MAC address as reported by the adapter.
            /// </summary>
            public string Mac
            {
                get
                {
                    try
                    {
                        return BitConverter.ToString(this.ManagedAdapter.GetPhysicalAddress().GetAddressBytes()).Replace("-", "").ToUpper();
                    }
                    catch { return null; }
                }
            }

            /// <summary>
            /// Get the registry key associated to this adapter.
            /// </summary>
            public string RegistryKey
            {
                get
                {
                    return String.Format(@"SYSTEM\ControlSet001\Control\Class\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\{0:D4}", this.devnum);
                }
            }

            /// <summary>
            /// Get the NetworkAddress registry value of this adapter.
            /// </summary>
            public string RegistryMac
            {
                get
                {
                    try
                    {
                        using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(this.RegistryKey, false))
                        {
                            return regkey.GetValue("NetworkAddress").ToString();
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// Sets the NetworkAddress registry value of this adapter.
            /// </summary>
            /// <param name="value">The value. Should be EITHER a string of 12 hexadecimal digits, uppercase, without dashes, dots or anything else, OR an empty string (clears the registry value).</param>
            /// <returns>true if successful, false otherwise</returns>
            public bool SetRegistryMac(string value)
            {
                bool shouldReenable = false;

                try
                {
                    // If the value is not the empty string, we want to set NetworkAddress to it,
                    // so it had better be valid
                    if (value.Length > 0 && !Adapter.IsValidMac(value, false))
                        throw new Exception(value + " is not a valid mac address");

                    using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(this.RegistryKey, true))
                    {
                        if (regkey == null)
                            throw new Exception("Failed to open the registry key");

                        // Sanity check
                        if (regkey.GetValue("AdapterModel") as string != this.adaptername
                            && regkey.GetValue("DriverDesc") as string != this.adaptername)
                            throw new Exception("Adapter not found in registry");

                        // Ask if we really want to do this
                        string question = value.Length > 0 ?
                            "Changing MAC-adress of adapter {0} from {1} to {2}. Proceed?" :
                            "Clearing custom MAC-address of adapter {0}. Proceed?";
                        DialogResult proceed = DialogResult.Yes;
                        //MessageBox.Show(
                        // String.Format(question, this.ToString(), this.Mac, value),
                        // "Change MAC-address?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (proceed != DialogResult.Yes)
                            return false;

                        // Attempt to disable the adepter
                        var result = (uint)adapter.InvokeMethod("Disable", null);
                        if (result != 0)
                            throw new Exception("Failed to disable network adapter.");

                        // If we're here the adapter has been disabled, so we set the flag that will re-enable it in the finally block
                        shouldReenable = true;

                        // If we're here everything is OK; update or clear the registry value
                        if (value.Length > 0)
                            regkey.SetValue("NetworkAddress", value, RegistryValueKind.String);
                        else
                            regkey.DeleteValue("NetworkAddress");


                        return true;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }

                finally
                {
                    if (shouldReenable)
                    {
                        uint result = (uint)adapter.InvokeMethod("Enable", null);
                        if (result != 0)
                            MessageBox.Show("Failed to re-enable network adapter.");
                    }
                }
            }

            public override string ToString()
            {
                return this.adaptername + this.customname;
            }

            /// <summary>
            /// Get a random (locally administered) MAC address.
            /// </summary>
            /// <returns>A MAC address having 01 as the least significant bits of the first byte, but otherwise random.</returns>
            public static string GetNewMac()
            {
                System.Random r = new System.Random();

                byte[] bytes = new byte[6];
                r.NextBytes(bytes);

                // Set second bit to 1
                bytes[0] = (byte)(bytes[0] | 0x02);
                // Set first bit to 0
                bytes[0] = (byte)(bytes[0] & 0xfe);

                return MacToString(bytes);
            }

            /// <summary>
            /// Verifies that a given string is a valid MAC address.
            /// </summary>
            /// <param name="mac">The string.</param>
            /// <param name="actual">false if the address is a locally administered address, true otherwise.</param>
            /// <returns>true if the string is a valid MAC address, false otherwise.</returns>
            public static bool IsValidMac(string mac, bool actual)
            {
                // 6 bytes == 12 hex characters (without dashes/dots/anything else)
                if (mac.Length != 12)
                    return false;

                // Should be uppercase
                if (mac != mac.ToUpper())
                    return false;

                // Should not contain anything other than hexadecimal digits
                if (!Regex.IsMatch(mac, "^[0-9A-F]*$"))
                    return false;

                if (actual)
                    return true;

                // If we're here, then the second character should be a 2, 6, A or E
                char c = mac[1];
                return (c == '2' || c == '6' || c == 'A' || c == 'E');
            }

            /// <summary>
            /// Verifies that a given MAC address is valid.
            /// </summary>
            /// <param name="mac">The address.</param>
            /// <param name="actual">false if the address is a locally administered address, true otherwise.</param>
            /// <returns>true if valid, false otherwise.</returns>
            public static bool IsValidMac(byte[] bytes, bool actual)
            {
                return IsValidMac(Adapter.MacToString(bytes), actual);
            }

            /// <summary>
            /// Converts a byte array of length 6 to a MAC address (i.e. string of hexadecimal digits).
            /// </summary>
            /// <param name="bytes">The bytes to convert.</param>
            /// <returns>The MAC address.</returns>
            public static string MacToString(byte[] bytes)
            {
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }
        }

        //
        private List<string> notFound;

        private List<Thread> runningThreads = new List<Thread>();
        public FormMain()
        {
            InitializeComponent();

        }
        public ListView ListViewControl
        {
            get { return this.lvDSTK; }
        }
        public void LoadDataListView()
        {
            lvDSTK.Items.Clear();
            string filePath = Path.Combine("data", "ListViewAccount.json");

            // Kiểm tra xem file có tồn tại hay không, nếu không thì không làm gì cả
            if (File.Exists(filePath))
            {
                // Đọc nội dung của file và chuyển đổi từ chuỗi json sang đối tượng AccountItem
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json)) // Kiểm tra xem json có null hoặc empty không
                {
                    List<AccountItem> accountItems = JsonConvert.DeserializeObject<List<AccountItem>>(json);
                    foreach (var (item, listViewItem) in
                        // Thêm các đối tượng AccountItem vào ListView
                        from item in accountItems
                        let listViewItem = new ListViewItem("")
                        select (item, listViewItem))
                    {
                        listViewItem.SubItems.Add(item.STT);
                        listViewItem.SubItems.Add(item.User);
                        listViewItem.SubItems.Add(item.Pass);
                        listViewItem.SubItems.Add(item.Cookie);
                        listViewItem.SubItems.Add("");
                        listViewItem.ForeColor = item.Color;
                        lvDSTK.Items.Add(listViewItem);
                    }
                }
            }
        }
        public void SaveDataListView()
        {
            // xử lý sự kiện truyền về, ở đây là in ra giá trị của biến flag
            string directoryPath = "data";
            string filePath = Path.Combine(directoryPath, "ListViewAccount.json");

            // Kiểm tra xem thư mục có tồn tại hay chưa, nếu chưa thì tạo mới
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Tạo đối tượng AccountItem từ các ListViewItem trong ListView
            List<AccountItem> accountItems = new List<AccountItem>();
            foreach (ListViewItem item in lvDSTK.Items)
            {
                accountItems.Add(new AccountItem
                {
                    Checkbox = "",
                    STT = item.SubItems[1].Text,
                    User = item.SubItems[2].Text,
                    Pass = item.SubItems[3].Text,
                    Cookie = item.SubItems[4].Text,
                    Status = "",
                    Color = item.ForeColor
                });
            }

            // Chuyển đối tượng AccountItem thành chuỗi json
            string json = JsonConvert.SerializeObject(accountItems);

            // Lưu chuỗi json vào file
            File.WriteAllText(filePath, json);
        }
        // Load Form
        private void Form1_Load(object sender, EventArgs e)
        {
            lvDSTK.FullRowSelect = true;
            LoadDataListView();

            AdaptersNW = new Adapter[0];

            // Lấy danh sách tất cả các adapter hợp lệ, không phải Microsoft Wi-Fi Direct Virtual Adapter
            var validAdapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(a => a.NetworkInterfaceType != NetworkInterfaceType.Loopback && a.OperationalStatus == OperationalStatus.Up && a.NetworkInterfaceType != NetworkInterfaceType.Tunnel && !a.Description.Contains("Microsoft Wi-Fi Direct Virtual Adapter"))
                .OrderByDescending(a => a.Speed)
                .ToList();

            // Thêm từng adapter vào mảng AdaptersNW
            foreach (var adapter in validAdapters)
            {
                // Tạo một mảng mới có kích thước lớn hơn mảng AdaptersNW 1 đơn vị
                Array.Resize(ref AdaptersNW, AdaptersNW.Length + 1);

                // Thêm adapter mới vào mảng AdaptersNW
                AdaptersNW[AdaptersNW.Length - 1] = new Adapter(adapter);
            }

        }
        Adapter[] AdaptersNW;
        // Check Internet Connection
        public bool CheckInternetConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send("google.com");
                    return result.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        // change MAC
        private void SetRegistryMac(string mac)
        {
            foreach (Adapter a in AdaptersNW)
            {
                if (a.SetRegistryMac(mac))
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
        private void ChangeMAC()
        {
            SetRegistryMac(Adapter.GetNewMac());
            //MessageBox.Show("done");
        }

        // change info user chrome 
        private void changeInfoUserChrome()
        {
            string[] lines = File.ReadAllLines("data/userAgent_Windowns.txt");
            Random rand = new Random();
            randomLine = lines[rand.Next(lines.Length)];
        }
        string randomLine;

        // start open chrome and change info pc,mac
        private void btnStart_Click(object sender, EventArgs e)
        {
            currentIndex = 0;
            start_RunTask();
        }
        int count_TaskRun = 0;
        private void start_RunTask()
        {
            string folderPath = "setting";
            string filePath = Path.Combine(folderPath, "setting.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(json))
                {
                    // Phân tích nội dung JSON
                    JObject jsonData = JObject.Parse(json);

                    // Lấy giá trị các thuộc tính
                    string choceConfuration = jsonData["Setting"]["choceConfuration"]?.ToString();

                    string settingAutoFilePath = Path.Combine("setting", "settingAuto.json");

                    if (File.Exists(settingAutoFilePath))
                    {
                        string settingAutoJson = File.ReadAllText(settingAutoFilePath);

                        if (!string.IsNullOrEmpty(settingAutoJson))
                        {
                            JObject settingAutoData = JObject.Parse(settingAutoJson);

                            if (settingAutoData.HasValues)
                            {
                                string configurationName = choceConfuration; // Tên cấu hình muốn lấy dữ liệu

                                if (settingAutoData.ContainsKey(configurationName))
                                {
                                    JObject configuration = settingAutoData[configurationName].Value<JObject>();

                                    // Lấy các thuộc tính từ configuration
                                    int optionSetting = configuration["optionSetting"]?.Value<int>() ?? 0;
                                    if (optionSetting == 0)
                                    {
                                        // chrome thường
                                        optionBuff = 0;
                                    }
                                    else if (optionSetting == 1)
                                    {
                                        //auto Buff
                                        optionBuff = 1;
                                    }
                                    else if (optionSetting == 2)
                                    {
                                        //Lọc tk sống
                                        optionBuff = 1;

                                    }
                                    else if (optionSetting == 3)
                                    {
                                        // Lọc tk có sdt trong tk
                                        optionBuff = 1;

                                    }
                                }
                                else
                                {
                                    optionBuff = 0;
                                }
                            }
                            else
                            {
                                // Tệp "settingAuto.json" rỗng, không chứa dữ liệu
                                optionBuff = 0;
                            }
                        }
                        else
                        {
                            // Tệp "settingAuto.json" rỗng, không có nội dung
                            optionBuff = 0;
                        }
                    }
                    else
                    {   // Tệp "settingAuto.json" không tồn tại
                        optionBuff = 0;
                    }

                }
                else
                {
                    optionBuff = 0;
                }
            }
            else
            {
                optionBuff = 0;
            }
        
            idItemStart = idLvChecked;
            stopRequested = false;
            if (lvDSTK.InvokeRequired)
            {
                lvDSTK.Invoke((MethodInvoker)delegate
                {
                    if (lvDSTK.CheckedItems.Count > 0)
                    {
                        GetProxyTm();
                        if (n_requset_sc == 0)
                        {
                            // start program
                            Start();
                        }
                        else
                        {
                            // start countdown
                            Thread countdownThread = new Thread(new ThreadStart(Countdown));
                            countdownThread.Start();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn tài khoản trên danh sách");
                    }
                });
            }
            else
            {
                if (lvDSTK.CheckedItems.Count > 0)
                {
                    GetProxyTm();
                    if (n_requset_sc == 0)
                    {
                        // start program
                        Start();
                    }
                    else
                    {
                        // start countdown
                        Thread countdownThread = new Thread(new ThreadStart(Countdown));
                        countdownThread.Start();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn tài khoản trên danh sách");
                }
            }
        }
        // countdown and start
        int idItemStart;
        int optionBuff = 0;
        private void Countdown()
        {
            int remainingTime = n_requset_sc;
            if (optionBuff == 0)
            {
                while (stopRequested == false && remainingTime > 0)
                {
                    Thread.Sleep(1000); // wait for 1 second
                    remainingTime--;

                    Invoke((MethodInvoker)delegate
                    {
                        block_btn_start();

                        int targetID = idItemStart;
                        foreach (ListViewItem item in lvDSTK.Items)
                        {
                            // Lấy giá trị ID của phần tử
                            string itemID = item.SubItems[1].Text;
                            // So sánh giá trị ID của phần tử với giá trị ID cần tìm
                            if (itemID == targetID.ToString())
                            {
                                if (stopRequested)
                                {
                                    item.SubItems[5].Text = "Đã dừng";
                                }
                                item.SubItems[5].Text = string.Format("Chờ đổi IP sau {0} giây", remainingTime);
                                // Thoát khỏi vòng lặp vì đã tìm thấy phần tử cần tìm
                                break;
                            }
                        }

                    });
                }

                if (!stopRequested && remainingTime == 0)
                {
                    GetProxyTm();
                    // countdown finished, start program
                    Start();
                    //Start();
                }
            }
            else
            {
                int countIndex = 0;
                Invoke((MethodInvoker)delegate
                {
                    countIndex = lvDSTK.CheckedItems.Count;
                });
                while (stopRequested == false && remainingTime > 0 && currentIndex < countIndex)
                {
                    Thread.Sleep(1000); // wait for 1 second
                    remainingTime--;

                    Invoke((MethodInvoker)delegate
                    {
                        block_btn_start();

                        ListViewItem selectedItem = lvDSTK.CheckedItems[currentIndex];
                        //string itemID = selectedItem.SubItems[1].Text;
                        if (stopRequested)
                        {
                            selectedItem.SubItems[5].Text = "Đã dừng";
                        }
                        else
                        {
                            selectedItem.SubItems[5].Text = string.Format("Chờ đổi IP sau {0} giây", remainingTime);
                        }

                    });
                }

                if (!stopRequested && remainingTime == 0)
                {
                    GetProxyTm();
                    // countdown finished, start program
                    Start();
                    //Start();
                }
            }

            if (stopRequested)
            {
                Invoke((MethodInvoker)delegate
                {
                    int targetID = idItemStart;
                    foreach (ListViewItem item in lvDSTK.Items)
                    {
                        string itemID = item.SubItems[currentIndex+1].Text;
                        if (itemID == targetID.ToString())
                        {
                            if (stopRequested)
                                item.SubItems[5].Text = "Đã dừng";
                            break;
                        }
                    }
                    open_btn_start();
                });
            }

        }
        // start
        private void block_btn_start()
        {

            btnStart.Enabled = false;
            btnStart.ForeColor = Color.Gray;
        }
        private void open_btn_start()
        {
            btnStart.Enabled = true;
            btnStart.ForeColor = Color.Black;
        }
        private async void Start()
        {
            await Task.Run(() =>
            {
                // code trong luồng
                bool start = false;
                lvDSTK.Invoke((MethodInvoker)delegate
                {
                    if (lvDSTK.CheckedItems.Count > 0)
                    {
                        start = true;
                    }
                });
                if (start)
                {
                    string folderPath = "setting";
                    string filePath = Path.Combine(folderPath, "setting.json");

                    if (File.Exists(filePath))
                    {
                        string json = File.ReadAllText(filePath);

                        if (!string.IsNullOrEmpty(json))
                        {
                            // Phân tích nội dung JSON
                            JObject jsonData = JObject.Parse(json);

                            // Lấy giá trị các thuộc tính
                            string choceConfuration = jsonData["Setting"]["choceConfuration"]?.ToString();

                            string settingAutoFilePath = Path.Combine("setting", "settingAuto.json");

                            if (File.Exists(settingAutoFilePath))
                            {
                                string settingAutoJson = File.ReadAllText(settingAutoFilePath);

                                if (!string.IsNullOrEmpty(settingAutoJson))
                                {
                                    JObject settingAutoData = JObject.Parse(settingAutoJson);

                                    if (settingAutoData.HasValues)
                                    {
                                        string configurationName = choceConfuration; // Tên cấu hình muốn lấy dữ liệu

                                        if (settingAutoData.ContainsKey(configurationName))
                                        {
                                            JObject configuration = settingAutoData[configurationName].Value<JObject>();

                                            // Lấy các thuộc tính từ configuration
                                            int optionSetting = configuration["optionSetting"]?.Value<int>() ?? 0;
                                            if (optionSetting == 0)
                                            {
                                                // chrome thường
                                                OpenChrome();
                                            }
                                            else if (optionSetting == 1 || optionSetting == 2 || optionSetting == 3)
                                            {
                                                //auto
                                                JObject dataObject = new JObject();
                                                dataObject["jsonData"] = jsonData;
                                                dataObject["settingAutoData"] = settingAutoData;
                                                Auto(optionSetting, dataObject);
                                            }
                                        }
                                        else
                                        {
                                            OpenChrome();
                                        }
                                    }
                                    else
                                    {
                                        // Tệp "settingAuto.json" rỗng, không chứa dữ liệu
                                        OpenChrome();
                                    }
                                }
                                else
                                {
                                    // Tệp "settingAuto.json" rỗng, không có nội dung
                                    OpenChrome();
                                }
                            }
                            else
                            {   // Tệp "settingAuto.json" không tồn tại
                                OpenChrome();
                            }

                        }
                        else
                        {
                            OpenChrome();
                        }
                    }
                    else
                    {
                        OpenChrome();
                    }
                }
            });

        }
        private void Test()
        {
        }
        int currentIndex = 0;
        //List<string> selectedItems = new List<string>();
        private void Auto(int optionSetting, object dataObject)
        {
            // Tạo bản sao của danh sách CheckedItems
            List<ListViewItem> checkedItemsCopy = new List<ListViewItem>();
            lvDSTK.Invoke((MethodInvoker)delegate
            {
                foreach (ListViewItem item in lvDSTK.CheckedItems)
                {
                    checkedItemsCopy.Add((ListViewItem)item.Clone());
                }
            });
            // Vòng lặp để thực hiện công việc cho tất cả các phần tử đã được chọn
            if (currentIndex < checkedItemsCopy.Count && stopRequested == false)
            {
                ListViewItem? selectedItem = null;
                string? cookies = "";
                string[]? cookiesArr = null;
                string? username = "";
                string? password = "";
                string? statusItem = "";
                lvDSTK.Invoke((MethodInvoker)delegate
                {
                    if (lvDSTK.CheckedItems.Count > 0)
                    {
                        selectedItem = lvDSTK.CheckedItems[currentIndex];
                        cookies = selectedItem.SubItems[4].Text;
                    }
                });

                if (selectedItem != null)
                {
                    username = selectedItem.SubItems[2].Text;
                    password = selectedItem.SubItems[3].Text;
                    statusItem = selectedItem.SubItems[5].Text;

                    // phân tách các giá trị trong cookies
                    cookiesArr = cookies.Split(';');

                    // Thực hiện các tác vụ khác với các thông tin lấy được
                }
                if (TmProxy.Length > 0)
                {
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = "Đang thêm proxy";
                    });

                    if (ChangeInfoMAC)
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Đang đổi địa chỉ MAC";
                        });
                        ChangeMAC();
                    }
                    while (!CheckInternetConnection())
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Đổi MAC thành công và chờ kết nối mạng";
                        });
                        Thread.Sleep(2000); // Đợi 2 giây trước khi kiểm tra lại
                    }
                    //selectedItem.SubItems[5].Text = "Mở chrome";
                    ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService("chromedriver.exe");
                    chromeService.HideCommandPromptWindow = true;
                   
                    if (ChangeInfoPC)
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Đang đổi thông tin loại máy";
                        });
                        changeInfoUserChrome();
                    }
                    string userAgent = randomLine;
                    string addProxy = string.Format("--proxy-server=http://{0}", TmProxy);
                    ChromeOptions options = new ChromeOptions();
                    options.AddArgument("--user-agent=" + userAgent);
                    options.AddArgument(addProxy);
                    options.AddArguments(
                        "--disable-extensions",
                        "--disable-infobars",
                        "--ignore-certificate-errors",
                        "disable-popup-blocking",
                        "--disable-infobars",
                        "--lang=vi"
                        );
                    options.AddExcludedArguments(new List<string> { "enable-automation", "disable-extensions", "enable-logging" });
                    options.AddUserProfilePreference("credentials_enable_service", false);
                    options.AddUserProfilePreference("profile.password_manager_enabled", false);
                    options.AddUserProfilePreference("disable-popup-blocking", "true");
                    options.AddUserProfilePreference("download.prompt_for_download", "false");
                    options.AddUserProfilePreference("safebrowsing.enabled", true);
                    options.AddUserProfilePreference("safebrowsing.disable_download_protection", true);
                    options.AddUserProfilePreference("plugins.plugins_disabled", new string[] { "Chrome PDF Viewer" });
                    options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);


                    //open chrome
                    IWebDriver driver = new ChromeDriver(chromeService, options);
                    int count = 0; // khởi tạo biến count để đếm số lần reload
                    bool isLoaded = false; // khởi tạo biến kiểm tra xem trang đã load thành công hay chưa
                    string url = "https://shopee.vn/buyer/login";
                    while (!isLoaded && count < 3) // nếu trang chưa load thành công và số lần reload < 3
                    {
                        try
                        {
                            driver.Navigate().GoToUrl(url); // load lại trang

                            // Chờ cho trang tải hoàn tất
                            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(14));
                            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                            // Kiểm tra trạng thái của trang
                            string currentState = (string)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState");
                            if (currentState.Equals("complete"))
                            {
                                isLoaded = true; // đánh dấu trang đã load thành công
                            }
                        }
                        catch (Exception ex) // nếu load thất bại, bắt đầu từ đầu vòng lặp
                        {
                            count++;
                        }
                    }

                    if (isLoaded) // nếu load thất bại sau 3 lần reload, xử lý theo ý của bạn
                    {
                        // add cookie
                        bool addCookieSucces = false;
                        //MessageBox.Show(username + ",  " + password + ", " + cookiesArr[0]);
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Đang Thêm cookie";
                        });
                        bool addCookieSuccess = false; // Biến kiểm tra xem đã thêm cookie thành công hay chưa
                        int maxRetryCountAddCookie = 5; // Số lần thử tối đa
                        int retryCountAddCookie = 0; // Số lần thử đã thực hiện

                        while (!addCookieSuccess && retryCountAddCookie < maxRetryCountAddCookie)
                        {
                            for (int i = 0; i < cookiesArr.Length; i++)
                            {
                                string? cookieString = cookiesArr[i];
                                string[]? cookieParts = cookieString.Split('=', 3);
                                string? domain = cookieParts[0]; // "shopee.vn"
                                string? name = cookieParts.Length > 1 ? cookieParts[1] : ""; // "csrftoken"
                                string? value = cookieParts.Length > 1 ? cookieParts[2] : ""; // "XK7nNNLTwn6XRA6YrfkdGyfNQMPseDJ7

                                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                                {
                                    // Tạo cookie
                                    OpenQA.Selenium.Cookie cookie = new OpenQA.Selenium.Cookie(name, value, domain, "/", DateTime.Now.AddDays(1));
                                    // Thêm cookie vào driver
                                    try
                                    {
                                        driver.Manage().Cookies.AddCookie(cookie);
                                        addCookieSucces = true;
                                    }
                                    catch
                                    {
                                        Invoke((MethodInvoker)delegate
                                        {
                                            selectedItem.SubItems[5].Text = "Đang Thêm lại cookie";
                                            open_btn_start();
                                        });
                                    }

                                }
                            }

                            if (!addCookieSuccess)
                            {
                                // Tăng số lần thử và đợi một khoảng thời gian trước khi thử lại
                                retryCountAddCookie++;
                                System.Threading.Thread.Sleep(2000); // Đợi 3 giây trước khi thử lại
                            }
                        }
                        if (addCookieSucces)
                        {
                            lvDSTK.Invoke((MethodInvoker)delegate
                            {
                                selectedItem.SubItems[5].Text = "Tiến hành đăng nhập";
                            });
                            // login
                            int maxRetryCount = 3; // Số lần lặp tối đa
                            int retryCount = 0; // Số lần lặp đã thực hiện
                            bool elementFound = false; // Biến kiểm tra xem đã tìm thấy phần tử hay chưa

                            while (!elementFound && retryCount < maxRetryCount)
                            {
                                try
                                {
                                    // Tìm phần tử
                                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                                    IWebElement usernameLogin = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/form[1]/div[1]/div[2]/div[2]/div[1]/input[1]"));
                                    IWebElement passwordLogin = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/form[1]/div[1]/div[2]/div[3]/div[1]/input[1]"));
                                    IWebElement btnLogin = driver.FindElement(By.XPath("//button[contains(text(),'Đăng nhập')]"));

                                    // Thực hiện các hành động liên quan đến phần tử
                                    usernameLogin.SendKeys(username);
                                    passwordLogin.SendKeys(password);
                                    System.Threading.Thread.Sleep(3000);
                                    btnLogin.Click();
                                    // Nếu tìm thấy phần tử, đánh dấu và thoát khỏi vòng lặp
                                    elementFound = true;
                                }
                                catch (NoSuchElementException)
                                {
                                    // Nếu không tìm thấy phần tử, tăng số lần lặp và tải lại trang
                                    retryCount++;
                                    driver.Navigate().Refresh();
                                }
                            }
                            try
                            {
                                // tắt popup
                                System.Threading.Thread.Sleep(3000);
                                IWebElement divElement = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/shopee-banner-popup-stateful[1]"));
                                // Lấy thuộc tính CSS hiện tại của div
                                string currentCss = divElement.GetAttribute("style");
                                // Thêm thuộc tính CSS mới vào chuỗi thuộc tính hiện tại
                                string newCss = currentCss + "display: none;";

                                // Ghi đè thuộc tính CSS mới vào div
                                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                                jsExecutor.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", divElement, newCss);
                            }
                            catch
                            {
                                lvDSTK.Invoke((MethodInvoker)delegate
                                {
                                    selectedItem.SubItems[5].Text = "Lỗi tắt popup trên trang chủ";
                                });
                            }
                            try
                            {
                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                                Thread.Sleep(3000);
                                //body[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/form[1]/div[1]/div[2]/div[1]/div[2]
                                string xpath1 = "//body[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/form[1]/div[1]/div[2]/div[1]";
                                string xpath2 = "//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/button[1]";

                                // Kiểm tra sự hiện diện của element1 và element2
                                bool isElement1Present = driver.FindElements(By.XPath(xpath1)).Count > 0;
                                bool isElement2Present = driver.FindElements(By.XPath(xpath2)).Count > 0;

                                // Xử lý tương ứng với trạng thái của các phần tử
                                if (!isElement1Present && !isElement2Present)
                                {
                                    // Không tìm thấy element1 và element2, đăng nhập thành công
                                    login_shopee_success(currentIndex);
                                    // tiếp tục hành động
                                    JObject jsonData = dataObject as JObject;
                                    JObject settingAutoData = dataObject as JObject;
                                    //MessageBox.Show(dataObject.ToString());
                                    if (dataObject != null)
                                    {
                                        try
                                        {
                                            string choceConfuration = jsonData["jsonData"]["Setting"]["choceConfuration"]?.ToString();
                                            bool cbDeleteAllAddressInAccount = (bool)jsonData["jsonData"]["Setting"]["cbDeleteAllAddressInAccount"];
                                            bool cbChoseQuantityProductOder = (bool)jsonData["jsonData"]["Setting"]["cbChoseQuantityProductOder"];
                                            int nupdownQuantityProductOder = (int)jsonData["jsonData"]["Setting"]["nupdownQuantityProductOder"];
                                            bool cbChoceShippingUnit = (bool)jsonData["jsonData"]["Setting"]["cbChoceShippingUnit"];
                                            int cbxChoceShippingUnit = (int)jsonData["jsonData"]["Setting"]["cbxChoceShippingUnit"];
                                        
                                            //MessageBox.Show(settingAutoData.ToString());
                                            // Truy cập vào các thuộc tính bên trong cục dữ liệu 
                                            //int id = settingAutoData["id"].Value<int>();
                                            int optionSettingData = settingAutoData["settingAutoData"][choceConfuration]["optionSetting"].Value<int>();
                                            string nameAction = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["nameAction"].Value<string>();
                                            JArray viewPage = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["viewPage"].Value<JArray>();
                                            JArray quantityProduct = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["quantityProduct"].Value<JArray>();
                                            int sortedBy = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["sortedBy"].Value<int>();
                                            string KeySearch = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["KeySearch"].Value<string>();
                                            JArray idProductSearchArray = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["IDProductSearch"].Value<JArray>();
                                            int optionBuff = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["optionBuff"].Value<int>();
                                            bool CbViewImgProduct = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["viewImgProduct"].Value<bool>();
                                            bool CbLikeProduct = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["LikeProduct"].Value<bool>();
                                            bool CbFollowShop = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["FollowShop"].Value<bool>();
                                            bool CbBuffAds = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["BuffAds"].Value<bool>();
                                            bool CbLikefeedback = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["Likefeedback"].Value<bool>();
                                            bool CbviewShop = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["viewShop"].Value<bool>();

                                            bool viewProductOtherBeforeChecked = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["cbViewProductOtherBefore"].Value<bool>();
                                            bool viewProductOtherAfterChecked = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["cbViewProductOtherAfter"].Value<bool>();
                                            int viewProductOtherBefore = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["viewProductOtherBefore"].Value<int>();
                                            int viewProductOtherAfter = settingAutoData["settingAutoData"][choceConfuration]["settingAuto"]["viewProductOtherAfter"].Value<int>();

                                            int AddressOptions = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["AddressOptions"].Value<int>();
                                            bool nameRandom = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["name"]["random"].Value<bool>();
                                            bool nameFileTxt = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["name"]["fileTxt"].Value<bool>();
                                            string nameText = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["name"]["text"].Value<string>();

                                            bool phoneRandom = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["phone"]["random"].Value<bool>();
                                            bool phoneFileTxt = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["phone"]["fileTxt"].Value<bool>();
                                            string phoneText = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["phone"]["text"].Value<string>();

                                            bool cityRandom = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["city"]["random"].Value<bool>();
                                            bool cityFileTxt = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["city"]["fileTxt"].Value<bool>();
                                            string cityText = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["city"]["text"].Value<string>();

                                            bool districtRandom = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["district"]["random"].Value<bool>();
                                            bool districtFileTxt = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["district"]["fileTxt"].Value<bool>();
                                            string districtText = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["district"]["text"].Value<string>();

                                            bool wardRandom = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["ward"]["random"].Value<bool>();
                                            bool wardFileTxt = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["ward"]["fileTxt"].Value<bool>();
                                            string wardText = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["ward"]["text"].Value<string>();

                                            bool specificAddressRandom = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["specificAddress"]["random"].Value<bool>();
                                            bool specificAddressFileTxt = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["specificAddress"]["fileTxt"].Value<bool>();
                                            string specificAddressText = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["specificAddress"]["text"].Value<string>();

                                            bool addressTypeRandom = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["addressType"]["random"].Value<bool>();
                                            int addressType = settingAutoData["settingAutoData"][choceConfuration]["settingAddress"]["addressType"]["type"].Value<int>();

                                            try
                                            {
                                                if (optionSetting == 1)
                                                {
                                                    //autoBuff
                                                    try
                                                    {
                                                        if (AddressOptions == 1)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            if (viewProductOtherBeforeChecked)
                                                            {
                                                                try
                                                                {
                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        selectedItem.SubItems[5].Text = "Xem SP trước khi đặt đơn" ;
                                                                    });
                                                                    int divNumber = 1;
                                                                    //IWebElement clickSearchViewProduct = driver.FindElement(By.XPath("//header/div[2]/div[1]/div[1]/div[2]/div[1]/a[1]"));
                                                                    //clickSearchViewProduct.Click();
                                                                    Thread.Sleep(5000);
                                                                    int numberOfProductsToView = viewProductOtherBefore;
                                                                    int countProductView = 0;
                                                                    bool reachedNumberOfProductsToView = false;

                                                                    // Tạo một mảng chứa các từ khóa tìm kiếm
                                                                    string[] keywords = {
                                                                        "Giày thế thao", "Giày nike", "Giày adidas", "Giày jordan", "Tất nike", "Giày nike vệt đen",
                                                                        "Giày nike trắng", "Giày nike vệt nâu", "Quần nữ", "Quần nam", "Áo nữ", "Áo thun nam",
                                                                        "Áo thun nam nữ", "Áo sơ mi nam", "Áo sơ mi nữ", "Máy xay sinh tố", "Máy làm sữa hạt",
                                                                        "Chảo chống dính", "Nồi chiên không dầu", "Máy làm sữa hạt mini", "Máy làm sữa hạt olivo",
                                                                        "Máy làm sữa hạt unie", "Nồi chiên không dầu lock&lock", "Nồi chiên không dầu camel",
                                                                        "Son blackrouge A12", "Son blackrouge A37", "Son blackrouge A06", "Son blackrouge A33",
                                                                        "Son blackrouge A02", "Laptop dell", "Laptop samsung", "Laptop asus", "Laptop gaming",
                                                                        "Laptop thinkpad", "Laptop lenovo", "Laptop dell latitude", "Laptop dell XPS", "Laptop mini",
                                                                        "Chuột không dây", "Bàn phím cơ", "Tai nghe bluetooth", "Máy ảnh canon", "Máy ảnh sony",
                                                                        "Điện thoại iphone", "Điện thoại samsung", "Điện thoại xiaomi", "Điện thoại oppo",
                                                                        "Điện thoại realme", "Điện thoại nokia", "Điện thoại vivo", "Điện thoại lg", "Điện thoại huawei",
                                                                        "Điện thoại sony", "Điện thoại google pixel", "Máy tính bảng samsung", "Máy tính bảng ipad",
                                                                        "Máy tính bảng huawei", "Máy tính bảng lenovo", "Máy tính bảng asus", "Máy tính bảng acer",
                                                                        "Tivi samsung", "Tivi sony", "Tivi lg", "Tivi panasonic", "Tivi tcl", "Tivi sharp",
                                                                        "Tivi philips", "Tivi toshiba", "Tủ lạnh samsung", "Tủ lạnh lg", "Tủ lạnh panasonic",
                                                                        "Tủ lạnh electrolux", "Tủ lạnh toshiba", "Tủ lạnh hitachi", "Tủ lạnh mitsubishi",
                                                                        "Máy giặt samsung", "Máy giặt lg", "Máy giặt panasonic", "Máy giặt electrolux",
                                                                        "Máy giặt toshiba", "Máy giặt hitachi", "Máy giặt mitsubishi", "Quạt điện", "Quạt hơi nước",
                                                                        "Quạt cây", "Quạt trần", "Quạt đứng", "Quạt sạc điện thoại", "Quạt mini", "Máy tạo ẩm",
                                                                        "Máy lọc không khí", "Máy tẩy da chết", "Máy rửa mặt", "Máy massage mặt", "Máy sấy tóc",
                                                                        "Máy hấp trứng", "Máy ép trái cây", "Máy pha cà phê", "Đèn led", "Đèn pin", "Đèn trang trí",
                                                                        "Đèn ngủ", "Đèn treo tường", "Đèn cây", "Gương trang điểm", "Nồi cơm điện", "Nồi nấu cháo",
                                                                        "Nồi hấp", "Nồi lẩu", "Nồi cơm điện cuckoo", "Nồi cơm điện tiger", "Nồi cơm điện supor",
                                                                        "Nồi cơm điện philips", "Nồi cơm điện sharp", "Máy nướng bánh mỳ", "Máy hút bụi", "Máy lọc nước"
                                                                    };

                                                                    // Tạo một đối tượng Random để random lấy ra từ khóa
                                                                    Random random = new Random();

                                                                    // Random lấy ra một vị trí ngẫu nhiên trong mảng từ khóa
                                                                    int randomIndex = random.Next(keywords.Length);

                                                                    // Lấy từ khóa tại vị trí randomIndex
                                                                    string randomKeyword = keywords[randomIndex];

                                                                    // Lấy ra từ khóa được random lấy ra
                                                                    IWebElement KeySearchProduct = driver.FindElement(By.XPath("//header/div[2]/div[1]/div[1]/div[1]/div[1]/form[1]/input[1]"));
                                                                    IList<IWebElement> notExistDivs = driver.FindElements(By.ClassName("product-not-exist__text"));

                                                                    foreach (char c in randomKeyword)
                                                                    {
                                                                        KeySearchProduct.SendKeys(c.ToString());
                                                                        
                                                                    }
                                                                    IWebElement btnSearchProduct = driver.FindElement(By.XPath($"//header/div[2]/div[1]/div[1]/div[1]/button[1]"));
                                                                    Thread.Sleep(3000);
                                                                    js.ExecuteScript("arguments[0].click();", btnSearchProduct);
                                                                    //btnSearchProduct.Click();
                                                                    Thread.Sleep(10000);
                                                                    new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                                                                    Thread.Sleep(5000);

                                                                    while (reachedNumberOfProductsToView == false)
                                                                    {
                                                                        //Random random = new Random();
                                                                        int randomNumber = random.Next(1, 30);
                                                                        int currentViewProduct = randomNumber;

                                                                        // Dùng vòng lặp để hover từng sản phẩm theo thứ tự từ 1 đến 48
                                                                        for (int i = 1; i <= 30; i++)
                                                                        {
                                                                            Thread.Sleep(500);
                                                                            // Tạo XPath để truy cập đến sản phẩm thứ i
                                                                            //shopee-search-item-result__items
                                                                            IWebElement classElement = driver.FindElement(By.ClassName("shopee-search-item-result__items"));
                                                                            // Tìm linkElement của sản phẩm thứ i
                                                                            IWebElement linkElementViewBeforeOderProduct = classElement.FindElement(By.XPath($".//div[{i}]/a"));

                                                                            // Sử dụng Actions để hover chuột vào sản phẩm
                                                                            Actions actions = new Actions(driver);
                                                                            actions.MoveToElement(linkElementViewBeforeOderProduct).Perform();

                                                                            // Scroll đến sản phẩm thứ i
                                                                            IJavaScriptExecutor scrollJs = (IJavaScriptExecutor)driver;
                                                                            scrollJs.ExecuteScript("arguments[0].scrollIntoView(true);", linkElementViewBeforeOderProduct);

                                                                            // Kiểm tra xem đã đạt số lần xem sản phẩm cần thực hiện chưa
                                                                            if (i == currentViewProduct)
                                                                            {
                                                                                countProductView++;
                                                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                                                {
                                                                                    selectedItem.SubItems[5].Text = "Xem sp vị trí " + randomNumber;
                                                                                });
                                                                                Thread.Sleep(1000);
                                                                                // Tìm link của sản phẩm thứ i và click 
                                                                                linkElementViewBeforeOderProduct.Click();
                                                                                Thread.Sleep(5000);

                                                                                // Nếu danh sách không rỗng, tức là có tồn tại div có class "product-not-exist__text"
                                                                                if (notExistDivs.Count > 0)
                                                                                {
                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                    {
                                                                                        selectedItem.SubItems[5].Text = "Không tìm thấy sản phẩm";
                                                                                    });
                                                                                    Thread.Sleep(5000);
                                                                                    driver.Navigate().Back();
                                                                                }
                                                                                else
                                                                                {
                                                                                    // Không tìm thấy div, thực hiện các hành động khác hoặc bỏ qua
                                                                                    Thread.Sleep(5000);
                                                                                    js.ExecuteScript($"window.scrollTo(0,0);");
                                                                                    // Xem sản phẩm ở đây (thực hiện các thao tác xem sản phẩm)
                                                                                    // like sp
                                                                                    if (CbLikeProduct)
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            IWebElement productBriefingElement = driver.FindElement(By.ClassName("product-briefing"));
                                                                                            IWebElement viewImgProductBeforeOrder = productBriefingElement.FindElement(By.XPath(".//div[2]/div[2]/div[2]/button[1]"));
                                                                                            viewImgProductBeforeOrder.Click();
                                                                                            lvDSTK.Invoke((MethodInvoker)delegate
                                                                                            {
                                                                                                selectedItem.SubItems[5].Text = "Thả tim SP";
                                                                                            });
                                                                                        }
                                                                                        catch
                                                                                        {
                                                                                            lvDSTK.Invoke((MethodInvoker)delegate
                                                                                            {
                                                                                                selectedItem.SubItems[5].Text = "Lỗi tim sp";
                                                                                            });
                                                                                        }
                                                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                                                        {
                                                                                            selectedItem.SubItems[5].Text = "Đã tim SP";
                                                                                        });
                                                                                        Thread.Sleep(3000);
                                                                                    }
                                                                                    // like đánh giá tốt 
                                                                                    if (CbLikefeedback)
                                                                                    {
                                                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                                                        {
                                                                                            selectedItem.SubItems[5].Text = "Like đánh giá tốt";
                                                                                        });
                                                                                        IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                                                                                        bool isProductRatingsDisplayed = (bool)jsExecutor.ExecuteScript("return document.querySelector('.product-ratings') !== null;");

                                                                                        // Scroll cho đến khi tìm thấy div có class "product-ratings"
                                                                                        while (!isProductRatingsDisplayed)
                                                                                        {
                                                                                            Thread.Sleep(500);
                                                                                            jsExecutor.ExecuteScript("window.scrollBy(0, 0);");
                                                                                            isProductRatingsDisplayed = (bool)jsExecutor.ExecuteScript("return document.querySelector('.product-ratings') !== null;");
                                                                                        }

                                                                                        // Sau khi tìm thấy div có class "product-ratings", bạn có thể thực hiện các thao tác mong muốn ở đây
                                                                                        IWebElement productRatings = driver.FindElement(By.CssSelector(".product-ratings"));
                                                                                        jsExecutor.ExecuteScript("arguments[0].scrollIntoView();", productRatings);

                                                                                        IList<IWebElement> filterDivs = driver.FindElements(By.CssSelector(".product-rating-overview__filter"));
                                                                                        // Click vào div thứ 2 trong danh sách các div có class "product-rating-overview__filter"
                                                                                        filterDivs[1].Click();
                                                                                        Thread.Sleep(5000);
                                                                                        try
                                                                                        {
                                                                                            // Tìm tất cả các div có class "shopee-product-rating"
                                                                                            IList<IWebElement> ratingDivs = driver.FindElements(By.ClassName("shopee-product-rating"));
                                                                                        
                                                                                            // Duyệt qua từng div có class "shopee-product-rating"
                                                                                            for (i = 0; i < 9; i++)
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    // Trong mỗi div, tìm và click vào div có class "shopee-product-rating__actions"
                                                                                                    IWebElement RatingDiv = ratingDivs[i];
                                                                                                    IWebElement actionsDiv = RatingDiv.FindElement(By.CssSelector("div.shopee-product-rating__main div.shopee-product-rating__actions div:nth-child(1) > div.shopee-product-rating__like-button"));
                                                                                                    jsExecutor.ExecuteScript("arguments[0].click();", actionsDiv);

                                                                                                    // Đợi một khoảng thời gian ngẫu nhiên từ 1 đến 3 giây
                                                                                                    int delayTime = random.Next(1000, 3000);
                                                                                                    Thread.Sleep(delayTime);

                                                                                                    // Scroll đến div bằng cách sử dụng JavaScript Executor
                                                                                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", actionsDiv);
                                                                                                }
                                                                                                catch
                                                                                                {
                                                                                                    break;
                                                                                                }
                                                                                            };

                                                                                            lvDSTK.Invoke((MethodInvoker)delegate
                                                                                            {
                                                                                                selectedItem.SubItems[5].Text = "Like đánh giá xong";
                                                                                            });
                                                                                            Thread.Sleep(3000);
                                                                                            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
                                                                                            Thread.Sleep(3000);
                                                                                        }
                                                                                        catch
                                                                                        {
                                                                                            lvDSTK.Invoke((MethodInvoker)delegate
                                                                                            {
                                                                                                selectedItem.SubItems[5].Text = "lỗi Like đánh giá";
                                                                                            });
                                                                                        }

                                                                                    }
                                                                                    // xem ảnh sp
                                                                                    if (CbViewImgProduct)
                                                                                    {
                                                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                                                        {
                                                                                            selectedItem.SubItems[5].Text = "Xem ảnh SP";
                                                                                        });
                                                                                        try
                                                                                        {
                                                                                            IWebElement productBriefingElement = driver.FindElement(By.ClassName("product-briefing"));
                                                                                            IWebElement viewImgProductBeforeOrder = productBriefingElement.FindElement(By.XPath(".//div[2]/div/div/div[1]"));
                                                                                            viewImgProductBeforeOrder.Click();
                                                                                            Thread.Sleep(3000);
                                                                                            int ViewImgProduct = 1;
                                                                                            bool viewImgProductBeforeOder = true;
                                                                                            while (viewImgProductBeforeOder)
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    js.ExecuteScript("document.body.style.zoom = '50%';");
                                                                                                    Thread.Sleep(2000);
                                                                                                    IWebElement productViewImg_next = driver.FindElement(By.XPath($"//body/div[@id='modal']/div[3]/div[1]/div[1]/div[2]/div[2]/div[{ViewImgProduct}]")); // Thay ".button-like-div" bằng CssSelector của thẻ div giả lập nút
                                                                                                    js.ExecuteScript("arguments[0].click();", productViewImg_next);
                                                                                                    ViewImgProduct++;
                                                                                                }
                                                                                                catch
                                                                                                {
                                                                                                    js.ExecuteScript("document.body.style.zoom = '100%';");
                                                                                                    if (ViewImgProduct == 1)
                                                                                                    {
                                                                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                        {
                                                                                                            selectedItem.SubItems[5].Text = "Lỗi next ảnh sp";
                                                                                                        });
                                                                                                    }
                                                                                                    viewImgProductBeforeOder = false;
                                                                                                    break;
                                                                                                }
                                                                                        
                                                                                            }
                                                                                            lvDSTK.Invoke((MethodInvoker)delegate
                                                                                            {
                                                                                                selectedItem.SubItems[5].Text = "Xem ảnh SP xong";
                                                                                            });
                                                                                        }
                                                                                        catch
                                                                                        {
                                                                                            lvDSTK.Invoke((MethodInvoker)delegate
                                                                                            {
                                                                                                selectedItem.SubItems[5].Text = "Lỗi xem ảnh sp";
                                                                                            });
                                                                                        }
                                                                                    }
                                                                                    //body/div[@id='modal']/div[3]/div[2]
                                                                                    Thread.Sleep(10000);
                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                    {
                                                                                        selectedItem.SubItems[5].Text = "Đã xem SP trước khi đặt đơn lần " + countProductView;
                                                                                    });
                                                                                    Thread.Sleep(10000);
                                                                                    // Thực hiện các thao tác xem sản phẩm xong
                                                                                    driver.Navigate().Back();
                                                                                    //js.ExecuteScript($"window.scrollTo(0,0);");
                                                                                    Thread.Sleep(5000);
                                                                                    // Sử dụng JavaScript Executor để scroll đến phần tử
                                                                                    IWebElement shopee_search_item_result = driver.FindElement(By.ClassName("shopee-search-item-result"));
                                                                                    js.ExecuteScript("arguments[0].scrollIntoView(true);", shopee_search_item_result);

                                                                                    // Nếu đã xem đủ số lượng sản phẩm cần thực hiện, đặt biến reachedNumberOfProductsToView thành true và thoát khỏi vòng lặp
                                                                                    if (countProductView == numberOfProductsToView)
                                                                                    {
                                                                                        reachedNumberOfProductsToView = true;
                                                                                    }
                                                                                    
                                                                                    // Nếu chưa đạt số lượng sản phẩm cần xem, tăng biến đếm lên
                                                                                    int newViewProduct = random.Next(1, 30);
                                                                                    while (currentViewProduct == newViewProduct)
                                                                                    {
                                                                                        newViewProduct = random.Next(1, 30);
                                                                                    }
                                                                                    currentViewProduct = newViewProduct;

                                                                                    // Nếu đã đạt số lượng sản phẩm cần xem, thoát khỏi vòng lặp
                                                                                    if (reachedNumberOfProductsToView)
                                                                                    {
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (reachedNumberOfProductsToView)
                                                                    {
                                                                        driver.Navigate().Back();
                                                                        Thread.Sleep(10000);
                                                                        js.ExecuteScript($"window.scrollTo(0,0);");
                                                                        // tắt popup
                                                                        System.Threading.Thread.Sleep(3000);
                                                                        IWebElement divElement = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/shopee-banner-popup-stateful[1]"));
                                                                        // Lấy thuộc tính CSS hiện tại của div
                                                                        string currentCss = divElement.GetAttribute("style");
                                                                        // Thêm thuộc tính CSS mới vào chuỗi thuộc tính hiện tại
                                                                        string newCss = currentCss + "display: none;";

                                                                        // Ghi đè thuộc tính CSS mới vào div
                                                                        IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                                                                        jsExecutor.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", divElement, newCss);
                                                                        Thread.Sleep(5000);
                                                                    }
                                                                    //Thread.Sleep(10000);
                                                                    // xem sp trước khi đặt đơn
                                                                }
                                                                catch
                                                                {
                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        selectedItem.SubItems[5].Text = "Lỗi xem sp trước khi đặt đơn";
                                                                    });
                                                                }
                                                            }
                                                            try
                                                            {
                                                                WebDriverWait waitLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                                                                waitLoad.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                                {
                                                                    selectedItem.SubItems[5].Text = "Tìm kiếm " + KeySearch;
                                                                });
                                                                Thread.Sleep(3000);
                                                                IWebElement KeySearchProduct = driver.FindElement(By.XPath("//header/div[2]/div[1]/div[1]/div[1]/div[1]/form[1]/input[1]"));
                                                                foreach (char c in KeySearch)
                                                                {
                                                                    KeySearchProduct.SendKeys(c.ToString());
                                                                }
                                                                IWebElement btnSearchProduct = driver.FindElement(By.XPath("//header/div[2]/div[1]/div[1]/div[1]/button[1]"));
                                                                Thread.Sleep(3000);
                                                                btnSearchProduct.Click();
                                                                // Zoom 25%
                                                                //js.ExecuteScript("document.body.style.zoom = '25%'");
                                                                WebDriverWait LoadPageProductSearch = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                                                                LoadPageProductSearch.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                                                                Thread.Sleep(10000);
                                                                IWebElement shopee_search_item_result = driver.FindElement(By.ClassName("shopee-search-item-result"));
                                                                // Sử dụng JavaScript Executor để scroll đến phần tử
                                                                js.ExecuteScript("arguments[0].scrollIntoView(true);", shopee_search_item_result);

                                                                if (optionBuff == 1)
                                                                {
                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        selectedItem.SubItems[5].Text = "Đang tìm sản phẩm ở trang 1";
                                                                    });
                                                                    //Thread.Sleep(10000);
                                                                    string idProduct = (string)idProductSearchArray[0];
                                                                    bool foundProduct = false;
                                                                    try
                                                                    {
                                                                        int divNumber = 1;
                                                                        Actions actions = new Actions(driver);

                                                                        while (foundProduct == false)
                                                                        {
                                                                            IWebElement classElement = driver.FindElement(By.ClassName("shopee-search-item-result__items"));
                                                                            //IWebElement linkElementViewBeforeOderProduct = classElement.FindElement(By.XPath($".//div[{divNumber}]/a"));

                                                                            new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                                                                            string xpath = $"//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/div[{divNumber}]/a";
                                                                            IWebElement linkElement = null;

                                                                            try // Kiểm tra xem có tìm thấy linkElement hay không
                                                                            {
                                                                                linkElement = classElement.FindElement(By.XPath($".//div[{divNumber}]/a"));
                                                                                actions.MoveToElement(linkElement).Perform();
                                                                                string href = linkElement.GetAttribute("href");
                                                                                Thread.Sleep(1000);

                                                                                // Scroll đến phần tử
                                                                                IJavaScriptExecutor scrollJs = (IJavaScriptExecutor)driver;
                                                                                scrollJs.ExecuteScript("arguments[0].scrollIntoView(true);", linkElement);

                                                                                int startIndex = href.IndexOf("i.") + 2; // Tìm vị trí của từ "i."
                                                                                if (startIndex >= 0)
                                                                                {
                                                                                    string productId = href.Substring(startIndex);
                                                                                    int endIndex = productId.IndexOf("?sp_atk=");
                                                                                    if (endIndex >= 0)
                                                                                    {
                                                                                        productId = productId.Substring(0, endIndex);
                                                                                        if (productId.Trim() == idProduct.Trim())
                                                                                        {
                                                                                            // Scroll đến vị trí sản phẩm và click
                                                                                            scrollJs.ExecuteScript("arguments[0].scrollIntoView(true);", linkElement);
                                                                                            linkElement.Click();
                                                                                            lvDSTK.Invoke((MethodInvoker)delegate
                                                                                            {
                                                                                                selectedItem.SubItems[5].Text = "Đã tìm thấy sản phẩm";
                                                                                            });
                                                                                            // xử lý khi tìm thấy sản phẩm
                                                                                            // like sp
                                                                                            Thread.Sleep(10000);
                                                                                            if (CbLikeProduct)
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    IWebElement productBriefingElement = driver.FindElement(By.ClassName("product-briefing"));
                                                                                                    IWebElement viewImgProductBeforeOrder = productBriefingElement.FindElement(By.XPath(".//div[2]/div[2]/div[2]/button[1]"));
                                                                                                    viewImgProductBeforeOrder.Click();
                                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                    {
                                                                                                        selectedItem.SubItems[5].Text = "Thả tim SP";
                                                                                                    });
                                                                                                }
                                                                                                catch
                                                                                                {
                                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                    {
                                                                                                        selectedItem.SubItems[5].Text = "Lỗi tim sp";
                                                                                                    });
                                                                                                }
                                                                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                {
                                                                                                    selectedItem.SubItems[5].Text = "Đã tim SP";
                                                                                                });
                                                                                                Thread.Sleep(3000);
                                                                                            }
                                                                                            // like đánh giá tốt 
                                                                                            if (CbLikefeedback)
                                                                                            {
                                                                                                Random random = new Random();
                                                                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                {
                                                                                                    selectedItem.SubItems[5].Text = "Like đánh giá tốt";
                                                                                                });
                                                                                                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                                                                                                bool isProductRatingsDisplayed = (bool)jsExecutor.ExecuteScript("return document.querySelector('.product-ratings') !== null;");

                                                                                                // Scroll cho đến khi tìm thấy div có class "product-ratings"
                                                                                                while (!isProductRatingsDisplayed)
                                                                                                {
                                                                                                    Thread.Sleep(500);
                                                                                                    jsExecutor.ExecuteScript("window.scrollBy(0, 0);");
                                                                                                    isProductRatingsDisplayed = (bool)jsExecutor.ExecuteScript("return document.querySelector('.product-ratings') !== null;");
                                                                                                }

                                                                                                // Sau khi tìm thấy div có class "product-ratings", bạn có thể thực hiện các thao tác mong muốn ở đây
                                                                                                IWebElement productRatings = driver.FindElement(By.CssSelector(".product-ratings"));
                                                                                                jsExecutor.ExecuteScript("arguments[0].scrollIntoView();", productRatings);

                                                                                                IList<IWebElement> filterDivs = driver.FindElements(By.CssSelector(".product-rating-overview__filter"));
                                                                                                // Click vào div thứ 2 trong danh sách các div có class "product-rating-overview__filter"
                                                                                                filterDivs[1].Click();
                                                                                                Thread.Sleep(5000);
                                                                                                try
                                                                                                {
                                                                                                    // Tìm tất cả các div có class "shopee-product-rating"
                                                                                                    IList<IWebElement> ratingDivs = driver.FindElements(By.ClassName("shopee-product-rating"));

                                                                                                    // Duyệt qua từng div có class "shopee-product-rating"
                                                                                                    for (int i = 0; i < 9; i++)
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            // Trong mỗi div, tìm và click vào div có class "shopee-product-rating__actions"
                                                                                                            IWebElement RatingDiv = ratingDivs[i];
                                                                                                            bool isLiked = RatingDiv.GetAttribute("class").Contains("shopee-product-rating__like-button--liked");
                                                                                                            // Dựa vào kết quả, bạn có thể thực hiện hành động like hoặc không like tại đây
                                                                                                            if (isLiked)
                                                                                                            {

                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                IWebElement actionsDiv = RatingDiv.FindElement(By.CssSelector("div.shopee-product-rating__main div.shopee-product-rating__actions div:nth-child(1) > div.shopee-product-rating__like-button"));
                                                                                                                jsExecutor.ExecuteScript("arguments[0].click();", actionsDiv);

                                                                                                                // Đợi một khoảng thời gian ngẫu nhiên từ 1 đến 3 giây
                                                                                                                int delayTime = random.Next(1000, 3000);
                                                                                                                Thread.Sleep(delayTime);

                                                                                                                // Scroll đến div bằng cách sử dụng JavaScript Executor
                                                                                                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", actionsDiv);
                                                                                                            }
                                                                                                        }
                                                                                                        catch
                                                                                                        {
                                                                                                            break;
                                                                                                        }
                                                                                                    };

                                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                    {
                                                                                                        selectedItem.SubItems[5].Text = "Like đánh giá xong";
                                                                                                    });
                                                                                                    Thread.Sleep(3000);
                                                                                                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
                                                                                                    Thread.Sleep(3000);
                                                                                                }
                                                                                                catch
                                                                                                {
                                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                    {
                                                                                                        selectedItem.SubItems[5].Text = "lỗi Like đánh giá";
                                                                                                    });
                                                                                                }

                                                                                            }
                                                                                            // xem ảnh sp
                                                                                            if (CbViewImgProduct)
                                                                                            {
                                                                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                {
                                                                                                    selectedItem.SubItems[5].Text = "Xem ảnh SP";
                                                                                                });
                                                                                                try
                                                                                                {
                                                                                                    IWebElement productBriefingElement = driver.FindElement(By.ClassName("product-briefing"));
                                                                                                    IWebElement viewImgProductBeforeOrder = productBriefingElement.FindElement(By.XPath(".//div[2]/div/div/div[1]"));
                                                                                                    viewImgProductBeforeOrder.Click();
                                                                                                    Thread.Sleep(3000);
                                                                                                    int ViewImgProduct = 1;
                                                                                                    bool viewImgProductBeforeOder = true;
                                                                                                    while (viewImgProductBeforeOder)
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            js.ExecuteScript("document.body.style.zoom = '50%';");
                                                                                                            Thread.Sleep(2000);
                                                                                                            IWebElement productViewImg_next = driver.FindElement(By.XPath($"//body/div[@id='modal']/div[3]/div[1]/div[1]/div[2]/div[2]/div[{ViewImgProduct}]")); // Thay ".button-like-div" bằng CssSelector của thẻ div giả lập nút
                                                                                                            js.ExecuteScript("arguments[0].click();", productViewImg_next);
                                                                                                            ViewImgProduct++;
                                                                                                        }
                                                                                                        catch
                                                                                                        {
                                                                                                            js.ExecuteScript("document.body.style.zoom = '100%';");
                                                                                                            if (ViewImgProduct == 1)
                                                                                                            {
                                                                                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                                {
                                                                                                                    selectedItem.SubItems[5].Text = "Lỗi next ảnh sp";
                                                                                                                });
                                                                                                            }
                                                                                                            viewImgProductBeforeOder = false;
                                                                                                            break;
                                                                                                        }

                                                                                                    }
                                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                    {
                                                                                                        selectedItem.SubItems[5].Text = "Xem ảnh SP xong";
                                                                                                    });
                                                                                                }
                                                                                                catch
                                                                                                {
                                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                                    {
                                                                                                        selectedItem.SubItems[5].Text = "Lỗi xem ảnh sp";
                                                                                                    });
                                                                                                }
                                                                                            }
                                                                                            Thread.Sleep(30000);
                                                                                            foundProduct = true;
                                                                                            break; // Dừng vòng lặp nếu đã tìm thấy sản phẩm
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            divNumber++;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            catch
                                                                            {
                                                                                // Không tìm thấy linkElement, chuyển sang trang tiếp theo nếu vẫn còn trang để duyệt
                                                                                IWebElement pageNumber = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[2]/div[2]/div[1]/div[2]/div[1]/span[1]"));
                                                                                string pageNumberText = pageNumber.Text;
                                                                                IWebElement btnNextPage = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[2]/div[2]/div[1]/div[2]/button[2]"));

                                                                                int currentPage = int.Parse(pageNumberText.Trim());
                                                                                int pageEnd = viewPage[1].Value<int>();

                                                                                if (currentPage < pageEnd)
                                                                                {
                                                                                    ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollTo(0, 0);");
                                                                                    Thread.Sleep(3000);
                                                                                    divNumber = 1;
                                                                                    btnNextPage.Click();
                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                    {
                                                                                        selectedItem.SubItems[5].Text = "Đang tìm sản phẩm ở trang " + (currentPage + 1);
                                                                                    });
                                                                                    Thread.Sleep(10000);
                                                                                }
                                                                                else if (currentPage == pageEnd)
                                                                                {
                                                                                    // Không tìm thấy sản phẩm trên bất kỳ trang nào, kết thúc vòng lặp và thông báo không tìm thấy sản phẩm
                                                                                    lvDSTK.Invoke((MethodInvoker)delegate
                                                                                    {
                                                                                        selectedItem.SubItems[5].Text = "Không tìm thấy sản phẩm";
                                                                                    });
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    catch
                                                                    {
                                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            selectedItem.SubItems[5].Text = "Lỗi Buff Deal";
                                                                        });
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    // optionBuff == 0
                                                                }
                                                            }
                                                            catch
                                                            {
                                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                                {
                                                                    selectedItem.SubItems[5].Text = "Lỗi AddressOptions == 0";
                                                                });
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                        {
                                                            selectedItem.SubItems[5].Text = "Lỗi AutoBuff";
                                                        });
                                                        Console.WriteLine(ex);  
                                                    }
                                                }
                                                else if (optionSetting == 2)
                                                {
                                                    // check live
                                                    try
                                                    {
                                                        Thread.Sleep(5000);
                                                        nextAccount();
                                                        driver.Close();
                                                        kill_process();
                                                    }
                                                    catch
                                                    {
                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                        {
                                                            selectedItem.SubItems[5].Text = "Lỗi CheckLive";
                                                        });
                                                    }
                                                }
                                                else if (optionSetting == 3)
                                                {
                                                    // chekc phone
                                                    try
                                                    {

                                                    }
                                                    catch
                                                    {
                                                        lvDSTK.Invoke((MethodInvoker)delegate
                                                        {
                                                            selectedItem.SubItems[5].Text = "Lỗi CheckPhone";
                                                        });
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                lvDSTK.Invoke((MethodInvoker)delegate
                                                {
                                                    selectedItem.SubItems[5].Text = "Lỗi Auto";
                                                });
                                            }
                                        }
                                        catch {
                                            lvDSTK.Invoke((MethodInvoker)delegate
                                            {
                                                selectedItem.SubItems[5].Text = "Lỗi không xác định (line-1123)";
                                            });
                                        }
                                    }

                                    nextAccount();
                                    driver.Close();
                                    kill_process();
                                }
                                else
                                {
                                    string value = "";
                                    if (isElement1Present)
                                    {
                                        // Tìm thấy element1, lấy giá trị text hoặc thông báo tài khoản bị cấm hoặc sai TK,MK
                                        value = driver.FindElement(By.XPath(xpath1)).Text;
                                        if (string.IsNullOrWhiteSpace(value))
                                        {
                                            value = "Tài khoản bị cấm hoặc sai TK,MK";
                                        }
                                    }
                                    else if (isElement2Present)
                                    {
                                        // Tìm thấy element2, thông báo tài khoản bị xác minh danh tính
                                        value = "Tài khoản bị xác minh danh tính";
                                    }
                                    login_shopee_fail(currentIndex, value);
                                    nextAccount();
                                    driver.Close();
                                    kill_process();
                                }
                            }
                            catch {
                                lvDSTK.Invoke((MethodInvoker)delegate
                                {
                                    selectedItem.SubItems[5].Text = "Lỗi không xác định (line-1083)";
                                });
                                nextAccount();
                                driver.Close();
                                kill_process();
                            }

                        }
                        else
                        {
                            lvDSTK.Invoke((MethodInvoker)delegate
                            {
                                selectedItem.SubItems[5].Text = "Thêm cookie thất bại";
                            });
                            nextAccount();
                            driver.Close();
                            kill_process();

                        }
                    }
                    else
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Tải trang thất bại";
                        });
                        nextAccount();
                        driver.Close();
                        kill_process();
                    }
                    Invoke((MethodInvoker)delegate
                    {
                        open_btn_start();
                    });

                }
                else
                {
                    count_TaskRun++;
                    if (count_TaskRun == 10)
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Lấy proxy thất bại";
                        });
                            nextAccount();
                    }
                    else
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = string.Format("Lỗi proxy và chạy lại lần {0}", count_TaskRun);
                        });
                        Thread.Sleep(6000);
                    }

                    if (count_TaskRun < 10)
                    {
                        if (lvDSTK.InvokeRequired)
                        {
                            lvDSTK.Invoke((MethodInvoker)delegate
                            {
                                start_RunTask();
                            });
                        }
                        else
                        {
                            start_RunTask();
                        }
                    }
                }
            }
        }
      
        private void nextAccount()
        {
            if (lvDSTK.InvokeRequired)
            {
                lvDSTK.Invoke((MethodInvoker)delegate
                {
                    currentIndex++;
                    start_RunTask();
                });
            }
            else
            {
                currentIndex++;
                start_RunTask();
            }
        }
        private void AutoCheckLive()
        {
            MessageBox.Show("CHeckLive");
        }
        private void AutoCheckPhone()
        {
            MessageBox.Show("CheckPhone");
        }
        private void OpenChrome()
        {
            ListViewItem? selectedItem = null;
            string? cookies = "";
            string[]? cookiesArr = null;
            string? username = "";
            string? password = "";
            string? statusItem = "";
            lvDSTK.Invoke((MethodInvoker)delegate
            {
                if (lvDSTK.CheckedItems.Count > 0)
                {
                    selectedItem = lvDSTK.CheckedItems[0];
                    cookies = selectedItem.SubItems[4].Text;
                }
            });

            if (selectedItem != null)
            {
                username = selectedItem.SubItems[2].Text;
                password = selectedItem.SubItems[3].Text;
                statusItem = selectedItem.SubItems[5].Text;

                // phân tách các giá trị trong cookies
                cookiesArr = cookies.Split(';');

                // Thực hiện các tác vụ khác với các thông tin lấy được
            }
            if (TmProxy.Length > 0)
            {
                lvDSTK.Invoke((MethodInvoker)delegate
                {
                    selectedItem.SubItems[5].Text = "Đang thêm proxy";
                });

                if (ChangeInfoMAC)
                {
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = "Đang đổi địa chỉ MAC";
                    });
                    ChangeMAC();
                }
                while (!CheckInternetConnection())
                {
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = "Đổi MAC thành công và chờ kết nối mạng";
                    });
                    Thread.Sleep(2000); // Đợi 2 giây trước khi kiểm tra lại
                }
                //selectedItem.SubItems[5].Text = "Mở chrome";
                ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService("chromedriver.exe");
                chromeService.HideCommandPromptWindow = true;
                if (ChangeInfoPC)
                {
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = "Đang đổi thông tin loại máy";
                    });
                    changeInfoUserChrome();
                }
                string userAgent = randomLine;
                string addProxy = string.Format("--proxy-server=http://{0}", TmProxy);
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--user-agent=" + userAgent);
                options.AddArgument(addProxy);
                options.AddArguments(
                    "--disable-extensions",
                    "--disable-infobars",
                    "--ignore-certificate-errors",
                    "disable-popup-blocking",
                    "--disable-infobars",
                    "--lang=vi"
                    );
                options.AddExcludedArguments(new List<string> { "enable-automation", "disable-extensions", "enable-logging" });
                options.AddUserProfilePreference("credentials_enable_service", false);
                options.AddUserProfilePreference("profile.password_manager_enabled", false);
                options.AddUserProfilePreference("disable-popup-blocking", "true");
                options.AddUserProfilePreference("download.prompt_for_download", "false");
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                options.AddUserProfilePreference("safebrowsing.disable_download_protection", true);
                options.AddUserProfilePreference("plugins.plugins_disabled", new string[] { "Chrome PDF Viewer" });
                options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);


                //open chrome
                IWebDriver driver = new ChromeDriver(chromeService, options);
                int count = 0; // khởi tạo biến count để đếm số lần reload
                bool isLoaded = false; // khởi tạo biến kiểm tra xem trang đã load thành công hay chưa
                string url = "https://shopee.vn/buyer/login";
                while (!isLoaded && count < 3) // nếu trang chưa load thành công và số lần reload < 3
                {
                    try
                    {
                        driver.Navigate().GoToUrl(url); // load lại trang

                        // Chờ cho trang tải hoàn tất
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(14));
                        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                        // Kiểm tra trạng thái của trang
                        string currentState = (string)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState");
                        if (currentState.Equals("complete"))
                        {
                            isLoaded = true; // đánh dấu trang đã load thành công
                        }
                    }
                    catch (Exception ex) // nếu load thất bại, bắt đầu từ đầu vòng lặp
                    {
                        count++;
                    }
                }

                if (isLoaded) // nếu load thất bại sau 3 lần reload, xử lý theo ý của bạn
                {
                    // add cookie
                    bool addCookieSucces = false;
                    //MessageBox.Show(username + ",  " + password + ", " + cookiesArr[0]);
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = "Đang Thêm cookie";
                    });
                    bool addCookieSuccess = false; // Biến kiểm tra xem đã thêm cookie thành công hay chưa
                    int maxRetryCountAddCookie = 5; // Số lần thử tối đa
                    int retryCountAddCookie = 0; // Số lần thử đã thực hiện

                    while (!addCookieSuccess && retryCountAddCookie < maxRetryCountAddCookie)
                    {
                        for (int i = 0; i < cookiesArr.Length; i++)
                        {
                            string? cookieString = cookiesArr[i];
                            string[]? cookieParts = cookieString.Split('=', 3);
                            string? domain = cookieParts[0]; // "shopee.vn"
                            string? name = cookieParts.Length > 1 ? cookieParts[1] : ""; // "csrftoken"
                            string? value = cookieParts.Length > 1 ? cookieParts[2] : ""; // "XK7nNNLTwn6XRA6YrfkdGyfNQMPseDJ7

                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                            {
                                // Tạo cookie
                                OpenQA.Selenium.Cookie cookie = new OpenQA.Selenium.Cookie(name, value, domain, "/", DateTime.Now.AddDays(1));
                                // Thêm cookie vào driver
                                try
                                {
                                    driver.Manage().Cookies.AddCookie(cookie);
                                    addCookieSucces = true;
                                }
                                catch
                                {
                                    Invoke((MethodInvoker)delegate
                                    {
                                        selectedItem.SubItems[5].Text = "Đang Thêm lại cookie";
                                        open_btn_start();
                                    });
                                }

                            }
                        }

                        if (!addCookieSuccess)
                        {
                            // Tăng số lần thử và đợi một khoảng thời gian trước khi thử lại
                            retryCountAddCookie++;
                            System.Threading.Thread.Sleep(2000); // Đợi 3 giây trước khi thử lại
                        }
                    }
                    if (addCookieSucces)
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Tiến hành đăng nhập";
                        });
                        // login
                        int maxRetryCount = 3; // Số lần lặp tối đa
                        int retryCount = 0; // Số lần lặp đã thực hiện
                        bool elementFound = false; // Biến kiểm tra xem đã tìm thấy phần tử hay chưa

                        while (!elementFound && retryCount < maxRetryCount)
                        {
                            try
                            {
                                // Tìm phần tử
                                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                                IWebElement usernameLogin = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/form[1]/div[1]/div[2]/div[2]/div[1]/input[1]"));
                                IWebElement passwordLogin = driver.FindElement(By.XPath("//body/div[@id='main']/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/form[1]/div[1]/div[2]/div[3]/div[1]/input[1]"));
                                IWebElement btnLogin = driver.FindElement(By.XPath("//button[contains(text(),'Đăng nhập')]"));

                                // Thực hiện các hành động liên quan đến phần tử
                                usernameLogin.SendKeys(username);
                                passwordLogin.SendKeys(password);
                                System.Threading.Thread.Sleep(3000);
                                btnLogin.Click();
                                // Nếu tìm thấy phần tử, đánh dấu và thoát khỏi vòng lặp
                                elementFound = true;
                            }
                            catch (NoSuchElementException)
                            {
                                // Nếu không tìm thấy phần tử, tăng số lần lặp và tải lại trang
                                retryCount++;
                                driver.Navigate().Refresh();
                            }
                        }

                        try
                        {
                            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                            IWebElement divElement = driver.FindElement(By.XPath("//body[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/form[1]/div[1]/div[2]/div[1]"));
                            string divText = divElement.Text;
                            if (divText == "")
                            {
                                // Đăng nhập thành công
                                login_shopee_success(0);
                            }
                            else
                            {
                                login_shopee_fail(0,"Đăng nhập thất bại");
                            }
                        }
                        catch (NoSuchElementException)
                        {
                            // Đăng nhập thành công
                            login_shopee_success(0);
                        }
                        kill_process();
                        stopRequested = true;
                    }
                    else
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            selectedItem.SubItems[5].Text = "Thêm cookie thất bại";
                        });
                        driver.Quit();
                        kill_process();

                    }
                }
                else
                {
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = "Tải trang thất bại";
                    });
                    driver.Quit();
                    kill_process();
                }

            }
            else
            {
                count_TaskRun++;
                if (count_TaskRun == 3)
                {
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = "Vui lòng thử lại";
                    });
                }
                else
                {
                    lvDSTK.Invoke((MethodInvoker)delegate
                    {
                        selectedItem.SubItems[5].Text = string.Format("Lỗi proxy và chạy lại lần {0}", count_TaskRun);
                    });
                }
                Thread.Sleep(2000);

                if (count_TaskRun < 3)
                {
                    if (lvDSTK.InvokeRequired)
                    {
                        lvDSTK.Invoke((MethodInvoker)delegate
                        {
                            start_RunTask();
                        });
                    }
                    else
                    {
                        start_RunTask();
                    }
                }
            }
            Invoke((MethodInvoker)delegate
            {
                open_btn_start();
            });
        }
        private void login_shopee_success(int n)
        {
            lvDSTK.Invoke((MethodInvoker)delegate
            {
                ListViewItem? selectedItem = null;
                if (lvDSTK.CheckedItems.Count > 0)
                {
                    selectedItem = lvDSTK.CheckedItems[n];
                    selectedItem.SubItems[5].Text = "Đăng nhập thành công";
                    selectedItem.ForeColor = Color.Green;
                }
            });

            Color newColor = Color.Green;

            // Đọc toàn bộ dữ liệu từ file.json vào một List
            List<AccountItem> userList = JsonConvert.DeserializeObject<List<AccountItem>>(File.ReadAllText("data/ListViewAccount.json"));

            // Sử dụng phương thức Invoke để truy cập vào ListView từ luồng khác
            lvDSTK.Invoke((MethodInvoker)delegate {
                // Lấy danh sách các phần tử đã được chọn trong ListView
                ListView.CheckedListViewItemCollection checkedItems = lvDSTK.CheckedItems;

                // Kiểm tra currentIndex có nằm trong khoảng chỉ mục hợp lệ hay không
                if (currentIndex >= 0 && currentIndex < checkedItems.Count)
                {
                    // Lấy phần tử thứ currentIndex trong danh sách các phần tử đã chọn
                    ListViewItem selectedItem = checkedItems[currentIndex];

                    // Lấy giá trị STT của phần tử đã chọn
                    string itemSTT = selectedItem.SubItems[1].Text;

                    // Tìm phần tử có STT là itemSTT và sửa thông tin của phần tử đó
                    AccountItem userToEdit = userList.Find(u => u.STT == itemSTT);
                    if (userToEdit != null)
                    {
                        userToEdit.Color = newColor;
                    }

                    // Ghi lại toàn bộ dữ liệu đã được sửa vào file.json
                    File.WriteAllText("data/ListViewAccount.json", JsonConvert.SerializeObject(userList));
                }
            });
        }
        private void login_shopee_fail(int n, string value)
        {
            lvDSTK.Invoke((MethodInvoker)delegate
            {
                ListViewItem? selectedItem = null;
                if (lvDSTK.CheckedItems.Count > 0)
                {
                    selectedItem = lvDSTK.CheckedItems[n];
                    selectedItem.SubItems[5].Text = value;
                    selectedItem.ForeColor = Color.Red;
                }
            });

            Color newColor = Color.Red;

            // Đọc toàn bộ dữ liệu từ file.json vào một List
            List<AccountItem> userList = JsonConvert.DeserializeObject<List<AccountItem>>(File.ReadAllText("data/ListViewAccount.json"));

            // Sử dụng phương thức Invoke để truy cập vào ListView từ luồng khác
            lvDSTK.Invoke((MethodInvoker)delegate {
                // Lấy danh sách các phần tử đã được chọn trong ListView
                ListView.CheckedListViewItemCollection checkedItems = lvDSTK.CheckedItems;

                // Kiểm tra currentIndex có nằm trong khoảng chỉ mục hợp lệ hay không
                if (currentIndex >= 0 && currentIndex < checkedItems.Count)
                {
                    // Lấy phần tử thứ currentIndex trong danh sách các phần tử đã chọn
                    ListViewItem selectedItem = checkedItems[currentIndex];

                    // Lấy giá trị STT của phần tử đã chọn
                    string itemSTT = selectedItem.SubItems[1].Text;

                    // Tìm phần tử có STT là itemSTT và sửa thông tin của phần tử đó
                    AccountItem userToEdit = userList.Find(u => u.STT == itemSTT);
                    if (userToEdit != null)
                    {
                        userToEdit.Color = newColor;
                    }

                    // Ghi lại toàn bộ dữ liệu đã được sửa vào file.json
                    File.WriteAllText("data/ListViewAccount.json", JsonConvert.SerializeObject(userList));
                }
            });
        }
        private void kill_process()
        {
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill(); // đóng tất cả các tiến trình chromedriver đang chạy
            }
        }
        //get proxy
        int n_requset_sc;
        string TmProxy;
        private string RequestPost(string Param944, string Param945)
        {
            string text = "";

            WebClient webClient = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            text = webClient.UploadString(Param944, Param945);
            if (string.IsNullOrEmpty(text))
            {
                text = "";
            }

            return text;
        }
        //string ChoseIP;
        string Key_proxy;
        bool ChangeInfoPC;
        bool ChangeInfoMAC;
        private void GetInfoChange()
        {

            // Đọc dữ liệu từ file JSON
            string jsonData = File.ReadAllText("setting/ChangeIP.json");
            // Chuyển đổi dữ liệu sang đối tượng trong C#
            var dataObject = JsonConvert.DeserializeObject<ChangeIPData>(jsonData);
            // Truy xuất thông tin trong đối tượng
            //ChoseIP = dataObject.ChoseIP;
            Key_proxy = dataObject.Key_proxy;
            ChangeInfoPC = dataObject.ChangeInfoPC;
            ChangeInfoMAC = dataObject.ChangeInfoMAC;

        }
        private void GetProxyTm()
        {
            GetInfoChange();
            string api_key = Key_proxy;
            string text = "string";
            string param = string.Concat(new string[]
            {
                "{\"api_key\": \"",
                api_key,
                "\",\"sign\": \"",
                text,
                "\"}"
            });
            string text2 = this.RequestPost("https://tmproxy.com/api/proxy/get-new-proxy", param);
            JObject jobject = JObject.Parse(text2);
            string value = Regex.Match(JObject.Parse(text2)["message"].ToString(), "\\d+").Value;

            string proxy = jobject["data"]["https"].ToString();


            if (proxy == "")
            {
                TmProxy = "";
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int result))
                {
                    n_requset_sc = result;
                }
                else
                {
                    n_requset_sc = -1;
                    MessageBox.Show("Vui lòng xem lại proxy");

                }
                //MessageBox.Show(n_requset_sc);
                //MessageBox.Show(TmProxy);
            }
            else
            {
                n_requset_sc = 0;
                TmProxy = proxy;
                //MessageBox.Show(proxy);
                //MessageBox.Show("get ip success");
            }
        }


        // stop and close chrome
        private bool stopRequested = false;
        private void btnStop_Click(object sender, EventArgs e)
        {
            stopRequested = true;
            //Test();
            //foreach (Thread t in runningThreads)
            //{
            //    t.Abort();
            //}

        }
        // Open Form AddAcc
        private void button4_Click(object sender, EventArgs e)
        {
            AddAcc childForm = new AddAcc();
            childForm.MyEvent += Frm2_MyEvent;
            childForm.Owner = this;
            childForm.ShowDialog();

        }
        // callback btn form AddAcc to Form1
        private void Frm2_MyEvent(object sender, bool e)
        {
            if (e)
            {
                SaveDataListView();
            }
        }
        // btn setting avanced
        private void button6_Click(object sender, EventArgs e)
        {
            //ChangeMAC();


            // Tạo một đối tượng của SecondForm
            SettingBuff SettingBuff = new SettingBuff();

            // Hiển thị form
            SettingBuff.Show();
            //MessageBox.Show("Tính năng này đang phát triển thêm");
        }

        // btn select all item
        private void lv_select_item_All_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvDSTK.Items)
            {
                item.Checked = true;
            }
        }

        private void lv_select_item_BoiDen_Click(object sender, EventArgs e)
        {
            // Lấy danh sách các chỉ mục được chọn trong ListView
            ListView.SelectedIndexCollection selectedIndices = lvDSTK.SelectedIndices;

            // Duyệt qua danh sách các chỉ mục và đánh dấu các checkbox tương ứng
            foreach (int index in selectedIndices)
            {
                lvDSTK.Items[index].Checked = true;
            }
        }

        private void lv_unSelect_item_All_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvDSTK.Items)
            {
                item.Checked = false;
            }
        }

        private void lv_unSelect_item_BoiDen_Click(object sender, EventArgs e)
        {
            // Lấy danh sách các chỉ mục được chọn trong ListView
            ListView.SelectedIndexCollection selectedIndices = lvDSTK.SelectedIndices;

            // Duyệt qua danh sách các chỉ mục và bỏ đánh dấu các checkbox tương ứng
            foreach (int index in selectedIndices)
            {
                lvDSTK.Items[index].Checked = false;
            }
        }

        private void deleteAllAcc_Click(object sender, EventArgs e)
        {
            // Duyệt qua tất cả các item trong ListView và xóa chúng
            string directoryPath = "data";
            string filePath = Path.Combine(directoryPath, "ListViewAccount.json");

            // Kiểm tra xem tệp JSON đã tồn tại hay chưa
            if (File.Exists(filePath))
            {
                // Xóa tệp JSON hiện có
                File.Delete(filePath);
            }
            //SaveDataListView();
            LoadDataListView();
        }
        private void CapNhatId()
        {
            // Duyệt qua danh sách các item trong ListView và cập nhật lại giá trị của cột id
            for (int i = 0; i < lvDSTK.Items.Count; i++)
            {
                lvDSTK.Items[i].SubItems[1].Text = (i + 1).ToString();
            }
        }
        private void deleteAccSelect_Click(object sender, EventArgs e)
        {
            for (int i = lvDSTK.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = lvDSTK.Items[i];
                if (item.Checked)
                {
                    //lvDSTK.Items.Remove(item);
                    //MessageBox.Show(item.SubItems[1].Text);
                    string filePath = Path.Combine("data", "ListViewAccount.json");
                    string json = File.ReadAllText(filePath);
                    List<AccountItem> accounts = JsonConvert.DeserializeObject<List<AccountItem>>(json);
                    AccountItem accountToRemove = accounts.FirstOrDefault(a => a.STT == item.SubItems[1].Text);
                    if (accountToRemove != null)
                    {
                        accounts.Remove(accountToRemove);
                    }

                    string updatedJson = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                    File.WriteAllText(filePath, updatedJson);
                }
            }

            // Cập nhật lại giá trị của cột id
            CapNhatId();
            LoadDataListView();
        }

        private void ctmn_RefreshData_Click(object sender, EventArgs e)
        {
            lvDSTK.Refresh();
        }

        // btn open setting change ip
        private void button3_Click(object sender, EventArgs e)
        {
            ChangeIP childForm = new ChangeIP();
            childForm.Owner = this;
            childForm.ShowDialog();
        }

        // update chrome
        private void btnUpdateChrome_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Vui lòng đợi cập nhật");
            string version = string.Empty;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Google\Chrome\BLBeacon"))
            {
                if (key != null)
                {
                    version = key.GetValue("version").ToString();
                }
            }

            // Sử dụng phiên bản Chrome để tải xuống phiên bản tương ứng của ChromeDriver
            string chromeDriverUrl = $"https://chromedriver.storage.googleapis.com/{version}/chromedriver_win32.zip";
            // Tiếp tục xử lý để tải xuống và cập nhật ChromeDriver
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://chromedriver.storage.googleapis.com/LATEST_RELEASE", "chromedriver_version.txt");
                string chromeDriverVersion = File.ReadAllText("chromedriver_version.txt").Trim();
                string chromeDriverDownloadUrl = $"https://chromedriver.storage.googleapis.com/{chromeDriverVersion}/chromedriver_win32.zip";
                client.DownloadFile(chromeDriverDownloadUrl, "chromedriver.zip");
            }
            MessageBox.Show("Đã cập nhật chrome xong");


        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopRequested = true;
        }
        int idLvChecked;
        private void lvDSTK_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.SubItems.Count > 0)
            {
                List<int> checkedIds = new List<int>();
                foreach (ListViewItem item in lvDSTK.CheckedItems)
                {
                    // Lấy giá trị của cột ID của item được check
                    int id;
                    if (int.TryParse(item.SubItems[1].Text, out id))
                    {
                        checkedIds.Add(id);
                    }
                }

                if (checkedIds.Count > 0)
                {
                    int firstCheckedId = checkedIds[0];
                    idLvChecked = firstCheckedId;
                }
            }
        }

        private void btnSearchAccount_Click(object sender, EventArgs e)
        {
            string accountName = txtSearchAccount.Text;
            foreach (ListViewItem item in lvDSTK.Items)
            {
                if (accountName.Trim() == item.SubItems[2].Text)
                {
                    txtSearchAccountSuccess.Text = item.SubItems[1].Text;
                    break;
                }
                else
                {
                    txtSearchAccountSuccess.Text = "Không tìm thấy";
                }
            }
        }

        private void btnSearchAccountFileTxt_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog.Title = "Select a Text File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // Đọc danh sách tên tài khoản từ tệp tin đã chọn
                string[] accountNames = File.ReadAllLines(filePath);

                // Thiết lập mảng notFound
                notFound = new List<string>();

                // Duyệt qua các tên tài khoản và tìm kiếm trên ListView
                foreach (string accountName in accountNames)
                {
                    bool found = false;

                    // Tìm kiếm trong ListView
                    foreach (ListViewItem item in lvDSTK.Items)
                    {
                        if (item.SubItems[2].Text == accountName.Trim())
                        {

                            // Đánh dấu checkbox và gán giá trị Checked là true
                            item.Checked = true;
                            found = true;
                            break;
                        }
                    }

                    // Nếu không tìm thấy, thêm vào mảng notFound
                    if (!found && accountName.Trim().Length != 0)
                    {
                        notFound.Add(accountName.Trim());
                    }
                }

                // Hiển thị thông báo nếu có tên tài khoản không tìm thấy
                if (notFound.Count > 0)
                {
                    string notFoundMessage = "Không tìm thấy các tài khoản sau:\n";
                    notFoundMessage += string.Join(", ", notFound);
                    MessageBox.Show(notFoundMessage);

                    // Sao chép danh sách tài khoản không tìm thấy vào clipboard
                    string notFoundAccounts = string.Join("\n", notFound);
                    Clipboard.SetText(notFoundAccounts);
                    MessageBox.Show("Đã sao chép những tài khoản không tìm thấy");
                }
            }
        }
    }
}