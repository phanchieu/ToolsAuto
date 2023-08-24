using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
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
using System.Reflection.Metadata;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ToolOpenChrome
{
    public partial class SettingBuff : Form
    {
        public SettingBuff()
        {
            InitializeComponent();
        }

        private void SettingBuff_Load(object sender, EventArgs e)
        {
            raOpenChrome.Checked = true;
            cbxSortBy.SelectedIndex = 0;
            txtNameActions.Text = "SuperAutoBuff";
            raBuffNormally.Checked = true;
            raUseDefaultAddress.Checked = true;
            LoadListViewConfigurations();
            getConfurationSetting();
            LoadSettingData();
        }
        private void LoadListViewConfigurations()
        {
            lvConfurations.Items.Clear();
            string folderPath = Path.Combine(Application.StartupPath, "setting");
            string filePath = Path.Combine(folderPath, "SettingAuto.json");

            if (File.Exists(filePath))
            {
                // Đọc nội dung file JSON
                string json = File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(json))
                {
                    // Phân tích nội dung JSON
                    JObject jsonObject = JObject.Parse(json);

                    // Lấy danh sách các tên đối tượng
                    List<string> configurationNames = jsonObject.Properties().Select(property => property.Name).ToList();

                    // Sử dụng danh sách các tên đối tượng trong ListView
                    foreach (string configurationName in configurationNames)
                    {
                        JObject configuration = (JObject)jsonObject[configurationName];

                        // Lấy giá trị của "id"
                        int id = configuration["id"].Value<int>();

                        // Tạo ListViewItem và gán giá trị cho các subitems
                        ListViewItem item = new ListViewItem(id.ToString());
                        item.SubItems.Add(configurationName);

                        // Thêm ListViewItem vào ListView
                        lvConfurations.Items.Add(item);
                    }

                    // Lấy danh sách các phần tử từ ListView và chuyển đổi thành danh sách đối tượng ListViewItem
                    List<ListViewItem> items = lvConfurations.Items.Cast<ListViewItem>().ToList();

                    // Sắp xếp danh sách các đối tượng ListViewItem theo STT
                    items.Sort((x, y) =>
                    {
                        int xSTT = int.Parse(x.SubItems[0].Text);
                        int ySTT = int.Parse(y.SubItems[0].Text);
                        return xSTT.CompareTo(ySTT);
                    });

                    // Xóa các phần tử hiện có trong ListView
                    lvConfurations.Items.Clear();

                    // Thêm lại các phần tử đã được sắp xếp vào ListView
                    lvConfurations.Items.AddRange(items.ToArray());
                    lvConfurations.Refresh();
                }
            }
        }
        int optionSetting;
        int optionBuff;
        int AddressOptions;
        bool namesakeAction; // check trùng tên hành động
        bool validateError;
        private void ValidateConfuration()
        {
            string folderPath = Path.Combine(Application.StartupPath, "setting");
            string filePath = Path.Combine(folderPath, "SettingAuto.json");
            // Đọc nội dung file JSON
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    // Phân tích nội dung JSON
                    JObject jsonObject = JObject.Parse(json);

                    // Lấy danh sách các tên đối tượng
                    List<string> configurationNames = jsonObject.Properties().Select(property => property.Name).ToList();

                    // Sử dụng danh sách các tên đối tượng trong ListView
                    foreach (string configurationName in configurationNames)
                    {
                        if (configurationName.Trim() == txtNameActions.Text.Trim())
                        {
                            namesakeAction = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                namesakeAction = false;
            }

            if (namesakeAction)
            {
                validateError = true;
                MessageBox.Show("Trùng tên cấu hình, vui lòng đặt tên khác");
            }
            else if (richTxtKeySearchProduct.Text.Trim().Length < 1)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm từ khóa tìm kiếm sản phẩm");
            }
            else if (richtxtIDProductOder.Text.Trim().Length < 1)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm ID sản phẩm");
            }
            else if (txtName.Text.Trim().Length < 1 && cbRandomName.Checked == false && cbChoseNameOnFile.Checked == false)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm họ tên hoặc tick vào ngẫu nhiên hoặc theo file");
            }
            else if (txtPhoneNumber.Text.Trim().Length < 1 && cbRandomPhoneNumber.Checked == false && cbChosePhoneNumberOnFile.Checked == false)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm SDT hoặc tick vào ngẫu nhiên hoặc theo file");
            }
            else if (txtCity.Text.Trim().Length < 1 && cbRandomCity.Checked == false && cbChoseCityOnFile.Checked == false)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm Tỉnh/TP hoặc tick vào ngẫu nhiên hoặc theo file");
            }
            else if (txtDistrict.Text.Trim().Length < 1 && cbRandomDistrict.Checked == false && cbChoseDistrictOnFile.Checked == false)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm Quận/Huyện hoặc tick vào ngẫu nhiên hoặc theo file");
            }
            else if (txtWard.Text.Trim().Length < 1 && cbRandomWard.Checked == false && cbChoseWardOnFile.Checked == false)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm Phường/Xã hoặc tick vào ngẫu nhiên hoặc theo file");
            }
            else if (txtSpecificAddress.Text.Trim().Length < 1 && cbRandomSpecificAddress.Checked == false && cbChoseSpecificAddressOnFile.Checked == false)
            {
                validateError = true;
                MessageBox.Show("Vui lòng thêm địa chỉ cụ thể hoặc tick vào ngẫu nhiên hoặc theo file");
            }
            else if (cbxChoseAddressType.SelectedIndex == -1 && cbRandomAddressType.Checked == false)
            {
                validateError = true;
                MessageBox.Show("Vui lòng chọn loại địa chỉ hoặc tick vào ngẫu nhiên");
            }
            else { validateError = false; }
        }
        private void btnSaveConfuration_Click(object sender, EventArgs e)
        {
            ValidateConfuration();
            if (validateError == false)
            {
                string IDProductOder = richtxtIDProductOder.Text;
                string[] lines = IDProductOder.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                JArray jsonArray = new JArray();
                foreach (string line in lines)
                {
                    jsonArray.Add(string.Format(line.Trim()));
                }

                string directoryPath = "setting";
                string filePath = Path.Combine(directoryPath, "SettingAuto.json");

                // Kiểm tra xem file đã tồn tại hay chưa
                if (File.Exists(filePath))
                {
                    // Đọc nội dung hiện có của file JSON
                    string json = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        // Chuyển đổi nội dung thành đối tượng JSON
                        JObject jsonData = JObject.Parse(json);

                        // Thêm dữ liệu mới vào đối tượng JSON
                        JObject newData = new JObject
                        {
                            ["id"] = lvConfurations.Items.Count + 1,
                            ["optionSetting"] = optionSetting,
                            ["settingAuto"] = new JObject
                            {
                                ["nameAction"] = txtNameActions.Text,
                                ["viewPage"] = new JArray(nupdownViewPage.Value, nupdownViewPageTo.Value),
                                ["quantityProduct"] = new JArray(nupdownQuantityProduct.Value, nupdownQuantityProductTo.Value),
                                ["sortedBy"] = cbxSortBy.SelectedIndex,
                                ["KeySearch"] = richTxtKeySearchProduct.Text,
                                ["IDProductSearch"] = jsonArray,
                                ["optionBuff"] = optionBuff,
                                ["viewImgProduct"] = cbViewImgProduct.Checked,
                                ["LikeProduct"] = cbLikeProduct.Checked,
                                ["FollowShop"] = cbFollowShop.Checked,
                                ["BuffAds"] = cbOderProductAds.Checked,
                                ["Likefeedback"] = cbLikeFeedback.Checked,
                                ["viewShop"] = cbViewShop.Checked,
                                ["cbViewProductOtherBefore"] = cbViewProductOtherBeforeOder.Checked,
                                ["cbViewProductOtherAfter"] = cbViewProductOtherAfterOder.Checked,
                                ["viewProductOtherBefore"] = nupdownViewProductOtherBeforeOder.Value,
                                ["viewProductOtherAfter"] = nupdownViewProductOtherAfterOder.Value
                            },
                            ["settingAddress"] = new JObject
                            {
                                ["AddressOptions"] = AddressOptions,
                                ["name"] = new JObject
                                {
                                    ["random"] = cbRandomName.Checked,
                                    ["fileTxt"] = cbChoseNameOnFile.Checked,
                                    ["text"] = txtName.Text,
                                },
                                ["phone"] = new JObject
                                {
                                    ["random"] = cbRandomPhoneNumber.Checked,
                                    ["fileTxt"] = cbChosePhoneNumberOnFile.Checked,
                                    ["text"] = txtPhoneNumber.Text,
                                },
                                ["city"] = new JObject
                                {
                                    ["random"] = cbRandomCity.Checked,
                                    ["fileTxt"] = cbChoseCityOnFile.Checked,
                                    ["text"] = txtCity.Text,
                                },
                                ["district"] = new JObject
                                {
                                    ["random"] = cbRandomDistrict.Checked,
                                    ["fileTxt"] = cbChoseDistrictOnFile.Checked,
                                    ["text"] = txtDistrict.Text,
                                },
                                ["ward"] = new JObject
                                {
                                    ["random"] = cbRandomWard.Checked,
                                    ["fileTxt"] = cbChoseWardOnFile.Checked,
                                    ["text"] = txtWard.Text,
                                },
                                ["specificAddress"] = new JObject
                                {
                                    ["random"] = cbRandomSpecificAddress.Checked,
                                    ["fileTxt"] = cbChoseSpecificAddressOnFile.Checked,
                                    ["text"] = txtSpecificAddress.Text,
                                },
                                ["addressType"] = new JObject
                                {
                                    ["random"] = cbRandomAddressType.Checked,
                                    ["type"] = cbxChoseAddressType.SelectedIndex,
                                }
                            }
                        };

                        jsonData[txtNameActions.Text] = newData; // Thay "a2" bằng key tương ứng

                        // Chuyển đổi đối tượng JSON thành chuỗi
                        string updatedJson = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

                        // Ghi lại nội dung đã được cập nhật vào file JSON
                        File.WriteAllText(filePath, updatedJson);
                    }
                    else
                    {
                        JObject jsonData = new JObject();

                        // Tạo đối tượng cấu hình mới
                        JObject newConfiguration = new JObject
                        {
                            ["id"] = lvConfurations.Items.Count + 1,
                            ["optionSetting"] = optionSetting,
                            ["settingAuto"] = new JObject
                            {
                                ["nameAction"] = txtNameActions.Text,
                                ["viewPage"] = new JArray(nupdownViewPage.Value, nupdownViewPageTo.Value),
                                ["quantityProduct"] = new JArray(nupdownQuantityProduct.Value, nupdownQuantityProductTo.Value),
                                ["sortedBy"] = cbxSortBy.SelectedIndex,
                                ["KeySearch"] = richTxtKeySearchProduct.Text,
                                ["IDProductSearch"] = jsonArray,
                                ["optionBuff"] = optionBuff,
                                ["viewImgProduct"] = cbViewImgProduct.Checked,
                                ["LikeProduct"] = cbLikeProduct.Checked,
                                ["FollowShop"] = cbFollowShop.Checked,
                                ["BuffAds"] = cbOderProductAds.Checked,
                                ["Likefeedback"] = cbLikeFeedback.Checked,
                                ["viewShop"] = cbViewShop.Checked,
                                ["cbViewProductOtherBefore"] = cbViewProductOtherBeforeOder.Checked,
                                ["cbViewProductOtherAfter"] = cbViewProductOtherAfterOder.Checked,
                                ["viewProductOtherBefore"] = nupdownViewProductOtherBeforeOder.Value,
                                ["viewProductOtherAfter"] = nupdownViewProductOtherAfterOder.Value
                            },
                            ["settingAddress"] = new JObject
                            {
                                ["AddressOptions"] = AddressOptions,
                                ["name"] = new JObject
                                {
                                    ["random"] = cbRandomName.Checked,
                                    ["fileTxt"] = cbChoseNameOnFile.Checked,
                                    ["text"] = txtName.Text,
                                },
                                ["phone"] = new JObject
                                {
                                    ["random"] = cbRandomPhoneNumber.Checked,
                                    ["fileTxt"] = cbChosePhoneNumberOnFile.Checked,
                                    ["text"] = txtPhoneNumber.Text,
                                },
                                ["city"] = new JObject
                                {
                                    ["random"] = cbRandomCity.Checked,
                                    ["fileTxt"] = cbChoseCityOnFile.Checked,
                                    ["text"] = txtCity.Text,
                                },
                                ["district"] = new JObject
                                {
                                    ["random"] = cbRandomDistrict.Checked,
                                    ["fileTxt"] = cbChoseDistrictOnFile.Checked,
                                    ["text"] = txtDistrict.Text,
                                },
                                ["ward"] = new JObject
                                {
                                    ["random"] = cbRandomWard.Checked,
                                    ["fileTxt"] = cbChoseWardOnFile.Checked,
                                    ["text"] = txtWard.Text,
                                },
                                ["specificAddress"] = new JObject
                                {
                                    ["random"] = cbRandomSpecificAddress.Checked,
                                    ["fileTxt"] = cbChoseSpecificAddressOnFile.Checked,
                                    ["text"] = txtSpecificAddress.Text,
                                },
                                ["addressType"] = new JObject
                                {
                                    ["random"] = cbRandomAddressType.Checked,
                                    ["type"] = cbxChoseAddressType.SelectedIndex,
                                }
                            }
                        };

                        // Thêm cấu hình vào đối tượng JSON
                        jsonData[txtNameActions.Text] = newConfiguration;

                        // Chuyển đổi đối tượng JSON thành chuỗi
                        string jsonContent = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

                        // Ghi nội dung JSON vào file
                        File.WriteAllText(filePath, jsonContent);
                    }
                }
                else
                {
                    // Nếu file không tồn tại, thực hiện các bước tạo mới file và lưu dữ liệu ban đầu
                    JObject jsonData = new JObject
                    {
                        [txtNameActions.Text] = new JObject
                        {
                            ["id"] = lvConfurations.Items.Count + 1,
                            ["optionSetting"] = optionSetting,
                            ["settingAuto"] = new JObject
                            {
                                ["nameAction"] = txtNameActions.Text,
                                ["viewPage"] = new JArray(nupdownViewPage.Value, nupdownViewPageTo.Value),
                                ["quantityProduct"] = new JArray(nupdownQuantityProduct.Value, nupdownQuantityProductTo.Value),
                                ["sortedBy"] = cbxSortBy.SelectedIndex,
                                ["KeySearch"] = richTxtKeySearchProduct.Text,
                                ["IDProductSearch"] = jsonArray,
                                ["optionBuff"] = optionBuff,
                                ["viewImgProduct"] = cbViewImgProduct.Checked,
                                ["LikeProduct"] = cbLikeProduct.Checked,
                                ["FollowShop"] = cbFollowShop.Checked,
                                ["BuffAds"] = cbOderProductAds.Checked,
                                ["Likefeedback"] = cbLikeFeedback.Checked,
                                ["viewShop"] = cbViewShop.Checked,
                                ["cbViewProductOtherBefore"] = cbViewProductOtherBeforeOder.Checked,
                                ["cbViewProductOtherAfter"] = cbViewProductOtherAfterOder.Checked,
                                ["viewProductOtherBefore"] = nupdownViewProductOtherBeforeOder.Value,
                                ["viewProductOtherAfter"] = nupdownViewProductOtherAfterOder.Value
                            },
                            ["settingAddress"] = new JObject
                            {
                                ["AddressOptions"] = AddressOptions,
                                ["name"] = new JObject
                                {
                                    ["random"] = cbRandomName.Checked,
                                    ["fileTxt"] = cbChoseNameOnFile.Checked,
                                    ["text"] = txtName.Text,
                                },
                                ["phone"] = new JObject
                                {
                                    ["random"] = cbRandomPhoneNumber.Checked,
                                    ["fileTxt"] = cbChosePhoneNumberOnFile.Checked,
                                    ["text"] = txtPhoneNumber.Text,
                                },
                                ["city"] = new JObject
                                {
                                    ["random"] = cbRandomCity.Checked,
                                    ["fileTxt"] = cbChoseCityOnFile.Checked,
                                    ["text"] = txtCity.Text,
                                },
                                ["district"] = new JObject
                                {
                                    ["random"] = cbRandomDistrict.Checked,
                                    ["fileTxt"] = cbChoseDistrictOnFile.Checked,
                                    ["text"] = txtDistrict.Text,
                                },
                                ["ward"] = new JObject
                                {
                                    ["random"] = cbRandomWard.Checked,
                                    ["fileTxt"] = cbChoseWardOnFile.Checked,
                                    ["text"] = txtWard.Text,
                                },
                                ["specificAddress"] = new JObject
                                {
                                    ["random"] = cbRandomAddressType.Checked,
                                    ["fileTxt"] = cbChoseSpecificAddressOnFile.Checked,
                                    ["text"] = txtSpecificAddress.Text,
                                },
                                ["addressType"] = new JObject
                                {
                                    ["random"] = cbRandomAddressType.Checked,
                                    ["type"] = cbxChoseAddressType.SelectedIndex,
                                }
                            }
                        }
                    };

                    // Chuyển đổi đối tượng JSON thành chuỗi
                    string initialJson = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

                    // Tạo thư mục nếu chưa tồn tại
                    Directory.CreateDirectory(directoryPath);

                    // Ghi nội dung ban đầu vào file JSON
                    File.WriteAllText(filePath, initialJson);
                }
                MessageBox.Show("Thêm cấu hình thành công");
                LoadListViewConfigurations();
                autoSelectItemListview();
                getConfurationSetting();
            }
            else
            {
                namesakeAction = false;
                validateError = false;
            }
            resetCbxConfugration();
        }

        private void raOpenChrome_CheckedChanged(object sender, EventArgs e)
        {
            if (raOpenChrome.Checked)
            {
                optionSetting = 0;
            }
        }

        private void raAutoOder_CheckedChanged(object sender, EventArgs e)
        {
            if (raAutoOder.Checked)
            {
                optionSetting = 1;
            }
        }

        private void raCheckLive_CheckedChanged(object sender, EventArgs e)
        {
            if (raCheckLive.Checked)
            {
                optionSetting = 2;
            }
        }

        private void raCheckPhoneNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (raCheckPhoneNumber.Checked)
            {
                optionSetting = 3;
            }
        }

        private void raBuffNormally_CheckedChanged(object sender, EventArgs e)
        {
            if (raBuffNormally.Checked)
            {
                optionBuff = 0;
            }
        }

        private void raBuffDeal_CheckedChanged(object sender, EventArgs e)
        {
            if (raBuffDeal.Checked)
            {
                optionBuff = 1;
            }
        }

        private void raUseDefaultAddress_CheckedChanged(object sender, EventArgs e)
        {
            if (raUseDefaultAddress.Checked)
            {
                AddressOptions = 0;
            }
        }

        private void raAddAddress_CheckedChanged(object sender, EventArgs e)
        {
            if (raAddAddress.Checked)
            {
                AddressOptions = 1;
            }
        }
        private void getConfurationSetting()
        {
            string folderPath = "setting";
            string filePath = Path.Combine(folderPath, "settingAuto.json");

            if (File.Exists(filePath))
            {
                // Đọc nội dung file JSON
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    cbxConfiguration.Items.Clear();
                    // Phân tích nội dung JSON
                    JObject jsonObject = JObject.Parse(json);

                    // Lấy danh sách các tên đối tượng
                    List<string> configurationNames = jsonObject.Properties().Select(property => property.Name).ToList();

                    // Sử dụng danh sách các tên đối tượng trong ListView
                    foreach (string configurationName in configurationNames)
                    {
                        cbxConfiguration.Items.Add(configurationName.ToString());
                    }
                }
            }
        }
        private void resetCbxConfugration()
        {
            cbxConfiguration.Items.Clear();
            getConfurationSetting();
        }
        private void btnSaveSetting_Click(object sender, EventArgs e)
        {
            if (cbxConfiguration.SelectedIndex != -1)
            {
                string directoryPath = Path.Combine(Application.StartupPath, "setting");
                string filePath = Path.Combine(directoryPath, "setting.json");

                // Đọc nội dung hiện có của file JSON
                string json = File.Exists(filePath) ? File.ReadAllText(filePath) : "{}";

                // Chuyển đổi nội dung thành đối tượng JSON
                JObject jsonData = JObject.Parse(json);

                // Thêm dữ liệu mới vào đối tượng JSON
                jsonData["Setting"] = new JObject
                {
                    ["choceConfuration"] = cbxConfiguration.Text,
                    ["cbDeleteAllAddressInAccount"] = cbDeleteAllAddressInAccount.Checked,
                    ["cbChoseQuantityProductOder"] = cbChoseQuantityProductOder.Checked,
                    ["nupdownQuantityProductOder"] = nupdownQuantityProductOder.Value,
                    ["cbChoceShippingUnit"] = cbChoceShippingUnit.Checked,
                    ["cbxChoceShippingUnit"] = cbxChoceShippingUnit.SelectedIndex
                };

                // Chuyển đổi đối tượng JSON thành chuỗi
                string jsonDataString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

                // Tạo thư mục nếu chưa tồn tại
                Directory.CreateDirectory(directoryPath);

                // Ghi nội dung vào file JSON
                File.WriteAllText(filePath, jsonDataString);
                MessageBox.Show("Lưu cài đặt thành công");
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn cấu hình");
            }
        }
        private void LoadSettingData()
        {
            string directoryPath = "setting";
            string fileName = "setting.json";
            string filePath = Path.Combine(directoryPath, fileName);

            // Kiểm tra xem file có tồn tại không
            if (File.Exists(filePath))
            {
                // Đọc nội dung file JSON
                string json = File.ReadAllText(filePath);

                // Phân tích nội dung JSON thành đối tượng tương ứng
                JObject jsonData = JObject.Parse(json);

                // Truy cập vào các thuộc tính trong đối tượng JSON để lấy dữ liệu
                string choceConfuration = (string)jsonData["Setting"]["choceConfuration"];
                bool DeleteAllAddressInAccount = (bool)jsonData["Setting"]["cbDeleteAllAddressInAccount"];
                bool ChoseQuantityProductOder = (bool)jsonData["Setting"]["cbChoseQuantityProductOder"];
                int QuantityProductOder = (int)jsonData["Setting"]["nupdownQuantityProductOder"];
                bool checkboxChoceShippingUnit = (bool)jsonData["Setting"]["cbChoceShippingUnit"];
                int ComboboxChoceShippingUnit = (int)jsonData["Setting"]["cbxChoceShippingUnit"];

                // Sử dụng dữ liệu đã lấy được
                cbxConfiguration.Text = choceConfuration;
                cbDeleteAllAddressInAccount.Checked = DeleteAllAddressInAccount;
                cbChoseQuantityProductOder.Checked = ChoseQuantityProductOder;
                nupdownQuantityProductOder.Value = QuantityProductOder;
                cbChoceShippingUnit.Checked = checkboxChoceShippingUnit;
                cbxChoceShippingUnit.SelectedIndex = ComboboxChoceShippingUnit;
            }
        }
        private void ListView_MouseClickLeft(object sender, EventArgs e)
        {
            System.Windows.Forms.ListView listView = (System.Windows.Forms.ListView)sender;

            if (listView.SelectedItems.Count > 0)
            {
                // Lấy phần tử được kích hoạt
                ListViewItem activatedItem = listView.SelectedItems[0];

                // Xử lý phần tử được kích hoạt
                //MessageBox.Show(activatedItem.SubItems[1].Text);

                string folderPath = Path.Combine(Application.StartupPath, "setting");
                string filePath = Path.Combine(folderPath, "SettingAuto.json");
                // Đọc nội dung file JSON
                string json = File.ReadAllText(filePath);

                // Phân tích nội dung JSON
                JObject jsonObject = JObject.Parse(json);

                // Lấy cục dữ liệu
                JObject Data = jsonObject[activatedItem.SubItems[1].Text].Value<JObject>();

                // Truy cập vào các thuộc tính bên trong cục dữ liệu 
                int id = Data["id"].Value<int>();
                int optionSetting = Data["optionSetting"].Value<int>();
                string nameAction = Data["settingAuto"]["nameAction"].Value<string>();
                JArray viewPage = Data["settingAuto"]["viewPage"].Value<JArray>();
                JArray quantityProduct = Data["settingAuto"]["quantityProduct"].Value<JArray>();
                int sortedBy = Data["settingAuto"]["sortedBy"].Value<int>();
                string KeySearch = Data["settingAuto"]["KeySearch"].Value<string>();
                JArray idProductSearchArray = Data["settingAuto"]["IDProductSearch"].Value<JArray>();
                int optionBuff = Data["settingAuto"]["optionBuff"].Value<int>();
                bool viewImgProduct = Data["settingAuto"]["viewImgProduct"].Value<bool>();
                bool LikeProduct = Data["settingAuto"]["LikeProduct"].Value<bool>();
                bool FollowShop = Data["settingAuto"]["FollowShop"].Value<bool>();
                bool BuffAds = Data["settingAuto"]["BuffAds"].Value<bool>();
                bool Likefeedback = Data["settingAuto"]["Likefeedback"].Value<bool>();
                bool viewShop = Data["settingAuto"]["viewShop"].Value<bool>();

                bool viewProductOtherBeforeChecked = Data["settingAuto"]["cbViewProductOtherBefore"].Value<bool>();
                bool viewProductOtherAfterChecked = Data["settingAuto"]["cbViewProductOtherAfter"].Value<bool>();
                int viewProductOtherBefore = Data["settingAuto"]["viewProductOtherBefore"].Value<int>();
                int viewProductOtherAfter = Data["settingAuto"]["viewProductOtherAfter"].Value<int>();

                int AddressOptions = Data["settingAddress"]["AddressOptions"].Value<int>();
                bool nameRandom = Data["settingAddress"]["name"]["random"].Value<bool>();
                bool nameFileTxt = Data["settingAddress"]["name"]["fileTxt"].Value<bool>();
                string nameText = Data["settingAddress"]["name"]["text"].Value<string>();

                bool phoneRandom = Data["settingAddress"]["phone"]["random"].Value<bool>();
                bool phoneFileTxt = Data["settingAddress"]["phone"]["fileTxt"].Value<bool>();
                string phoneText = Data["settingAddress"]["phone"]["text"].Value<string>();

                bool cityRandom = Data["settingAddress"]["city"]["random"].Value<bool>();
                bool cityFileTxt = Data["settingAddress"]["city"]["fileTxt"].Value<bool>();
                string cityText = Data["settingAddress"]["city"]["text"].Value<string>();

                bool districtRandom = Data["settingAddress"]["district"]["random"].Value<bool>();
                bool districtFileTxt = Data["settingAddress"]["district"]["fileTxt"].Value<bool>();
                string districtText = Data["settingAddress"]["district"]["text"].Value<string>();

                bool wardRandom = Data["settingAddress"]["ward"]["random"].Value<bool>();
                bool wardFileTxt = Data["settingAddress"]["ward"]["fileTxt"].Value<bool>();
                string wardText = Data["settingAddress"]["ward"]["text"].Value<string>();

                bool specificAddressRandom = Data["settingAddress"]["specificAddress"]["random"].Value<bool>();
                bool specificAddressFileTxt = Data["settingAddress"]["specificAddress"]["fileTxt"].Value<bool>();
                string specificAddressText = Data["settingAddress"]["specificAddress"]["text"].Value<string>();

                bool addressTypeRandom = Data["settingAddress"]["addressType"]["random"].Value<bool>();
                int addressType = Data["settingAddress"]["addressType"]["type"].Value<int>();
                // Tiếp tục truy cập vào các thuộc tính khác...

                // Sử dụng dữ liệu đã lấy được
                if (optionSetting == 0) { raOpenChrome.Checked = true; }
                if (optionSetting == 1) { raAutoOder.Checked = true; }
                if (optionSetting == 2) { raCheckLive.Checked = true; }
                if (optionSetting == 3) { raCheckPhoneNumber.Checked = true; }

                txtNameActions.Text = nameAction;
                nupdownViewPage.Value = viewPage[0].Value<int>();
                nupdownViewPageTo.Value = viewPage[1].Value<int>();
                nupdownQuantityProduct.Value = quantityProduct[0].Value<int>();
                nupdownQuantityProductTo.Value = quantityProduct[0].Value<int>();
                cbxSortBy.SelectedIndex = sortedBy;
                richTxtKeySearchProduct.Text = KeySearch;
                // Kiểm tra và lấy các giá trị trong mảng
                richtxtIDProductOder.Text = "";
                foreach (var item in idProductSearchArray)
                {
                    string idProduct = item.Value<string>();

                    // Thêm giá trị vào đối tượng RichTextBox
                    richtxtIDProductOder.AppendText(idProduct + Environment.NewLine);
                }
                if (optionBuff == 0) { raBuffNormally.Checked = true; }
                if (optionBuff == 1) { raBuffDeal.Checked = true; }
                cbViewImgProduct.Checked = viewImgProduct;
                cbLikeProduct.Checked = LikeProduct;
                cbFollowShop.Checked = FollowShop;
                cbOderProductAds.Checked = BuffAds;
                cbLikeFeedback.Checked = Likefeedback;
                cbViewShop.Checked = viewShop;

                cbViewProductOtherBeforeOder.Checked = viewProductOtherBeforeChecked;
                cbViewProductOtherAfterOder.Checked = viewProductOtherAfterChecked;
                nupdownViewProductOtherBeforeOder.Value = viewProductOtherBefore;
                nupdownViewProductOtherAfterOder.Value = viewProductOtherAfter;

                if (AddressOptions == 0) { raUseDefaultAddress.Checked = true; }
                if (AddressOptions == 1) { raAddAddress.Checked = true; }

                cbRandomName.Checked = nameRandom;
                cbChoseNameOnFile.Checked = nameFileTxt;
                txtName.Text = nameText;

                cbRandomPhoneNumber.Checked = phoneRandom;
                cbChosePhoneNumberOnFile.Checked = phoneFileTxt;
                txtPhoneNumber.Text = phoneText;

                cbRandomCity.Checked = cityRandom;
                cbChoseCityOnFile.Checked = cityFileTxt;
                txtCity.Text = cityText;

                cbRandomDistrict.Checked = districtRandom;
                cbChoseDistrictOnFile.Checked = districtFileTxt;
                txtDistrict.Text = districtText;

                cbRandomWard.Checked = wardRandom;
                cbChoseWardOnFile.Checked = wardFileTxt;
                txtWard.Text = wardText;

                cbRandomSpecificAddress.Checked = specificAddressRandom;
                cbChoseSpecificAddressOnFile.Checked = specificAddressFileTxt;
                txtSpecificAddress.Text = specificAddressText;

                cbRandomAddressType.Checked = addressTypeRandom;
                cbxChoseAddressType.SelectedIndex = addressType;
                //MessageBox.Show(string.Format("id: {0}, optionSetting: {1}, nameAction: {2}", id, optionSetting, nameAction));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEditConfuration_Click(object sender, EventArgs e)
        {
            if (lvConfurations.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvConfurations.SelectedItems[0];
                string configurationName = selectedItem.SubItems[1].Text;
                string directoryPath = "setting";
                string fileName = "settingAuto.json";
                string filePath = Path.Combine(directoryPath, fileName);

                // Kiểm tra xem file có tồn tại không
                if (File.Exists(filePath))
                {
                    // Đọc nội dung file JSON
                    string json = File.ReadAllText(filePath);

                    // Chuyển đổi nội dung thành đối tượng JSON
                    JObject jsonData = JObject.Parse(json);

                    // Kiểm tra xem đối tượng cấu hình đã tồn tại trong file hay chưa
                    if (jsonData.ContainsKey(configurationName))
                    {
                        JObject configuration = jsonData[configurationName].Value<JObject>();
                        jsonData.Remove(configurationName);
                        jsonData[txtNameActions.Text] = configuration;
                        string newJsonData = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                        File.WriteAllText(filePath, newJsonData);
                    }

                    if (jsonData.ContainsKey(string.Format(txtNameActions.Text)))
                    {
                        string jsonDataRead = File.ReadAllText(filePath);
                        JObject jsonData1 = JObject.Parse(jsonDataRead);

                        // Cập nhật giá trị của thuộc tính cấu hình
                        JObject configuration = jsonData1[string.Format(txtNameActions.Text)].Value<JObject>();
                        configuration["optionSetting"] = raOpenChrome.Checked ? 0 : raAutoOder.Checked ? 1 : raCheckLive.Checked ? 2 : raCheckPhoneNumber.Checked ? 3 : 0;
                        configuration["settingAuto"]["nameAction"] = txtNameActions.Text;
                        configuration["settingAuto"]["viewPage"] = new JArray(nupdownViewPage.Value, nupdownViewPageTo.Value);
                        configuration["settingAuto"]["quantityProduct"] = new JArray(nupdownQuantityProduct.Value, nupdownQuantityProductTo.Value);
                        configuration["settingAuto"]["sortedBy"] = cbxSortBy.SelectedIndex;
                        configuration["settingAuto"]["KeySearch"] = richTxtKeySearchProduct.Text;
                        configuration["settingAuto"]["IDProductSearch"] = JArray.FromObject(richtxtIDProductOder.Lines);
                        configuration["settingAuto"]["optionBuff"] = raBuffNormally.Checked ? 0 : raBuffDeal.Checked ? 1 : 0;
                        configuration["settingAuto"]["viewImgProduct"] = cbViewImgProduct.Checked;
                        configuration["settingAuto"]["LikeProduct"] = cbLikeProduct.Checked;
                        configuration["settingAuto"]["FollowShop"] = cbFollowShop.Checked;
                        configuration["settingAuto"]["BuffAds"] = cbOderProductAds.Checked;
                        configuration["settingAuto"]["Likefeedback"] = cbLikeFeedback.Checked;
                        configuration["settingAuto"]["viewShop"] = cbViewShop.Checked;
                        configuration["settingAuto"]["cbViewProductOtherBefore"] = cbViewProductOtherBeforeOder.Checked;
                        configuration["settingAuto"]["cbViewProductOtherAfter"] = cbViewProductOtherAfterOder.Checked;
                        configuration["settingAuto"]["viewProductOtherBefore"] = nupdownViewProductOtherBeforeOder.Value;
                        configuration["settingAuto"]["viewProductOtherAfter"] = nupdownViewProductOtherAfterOder.Value;
                        configuration["settingAddress"]["AddressOptions"] = raUseDefaultAddress.Checked ? 0 : raAddAddress.Checked ? 1 : 0;

                        configuration["settingAddress"]["name"]["random"] = cbRandomName.Checked;
                        configuration["settingAddress"]["name"]["fileTxt"] = cbChoseNameOnFile.Checked;
                        configuration["settingAddress"]["name"]["text"] = txtName.Text;

                        configuration["settingAddress"]["phone"]["random"] = cbRandomPhoneNumber.Checked;
                        configuration["settingAddress"]["phone"]["fileTxt"] = cbChosePhoneNumberOnFile.Checked;
                        configuration["settingAddress"]["phone"]["text"] = txtPhoneNumber.Text;

                        configuration["settingAddress"]["city"]["random"] = cbRandomCity.Checked;
                        configuration["settingAddress"]["city"]["fileTxt"] = cbChoseCityOnFile.Checked;
                        configuration["settingAddress"]["city"]["text"] = txtCity.Text;

                        configuration["settingAddress"]["district"]["random"] = cbRandomDistrict.Checked;
                        configuration["settingAddress"]["district"]["fileTxt"] = cbChoseDistrictOnFile.Checked;
                        configuration["settingAddress"]["district"]["text"] = txtDistrict.Text;

                        configuration["settingAddress"]["ward"]["random"] = cbRandomWard.Checked;
                        configuration["settingAddress"]["ward"]["fileTxt"] = cbChoseWardOnFile.Checked;
                        configuration["settingAddress"]["ward"]["text"] = txtWard.Text;

                        configuration["settingAddress"]["specificAddress"]["random"] = cbRandomSpecificAddress.Checked;
                        configuration["settingAddress"]["specificAddress"]["fileTxt"] = cbChoseSpecificAddressOnFile.Checked;
                        configuration["settingAddress"]["specificAddress"]["text"] = txtSpecificAddress.Text;

                        configuration["settingAddress"]["addressType"]["random"] = cbRandomAddressType.Checked;
                        configuration["settingAddress"]["addressType"]["type"] = cbxChoseAddressType.SelectedIndex;

                        // Lưu dữ liệu vào file settingAuto.json
                        string newJsonData1 = JsonConvert.SerializeObject(jsonData1, Formatting.Indented);
                        File.WriteAllText(filePath, newJsonData1);

                        LoadListViewConfigurations();
                        MessageBox.Show("Sửa cấu hình thành công");
                    }

                    LoadListViewConfigurations();
                    autoSelectItemListview();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng thêm cấu hình");
            }
            resetCbxConfugration();
        }
        private void autoSelectItemListview()
        {
            string itemName = txtNameActions.Text;

            // Duyệt qua danh sách các mục trong ListView
            foreach (ListViewItem item in lvConfurations.Items)
            {
                string currentName = item.SubItems[1].Text;
                if (currentName == itemName)
                {
                    item.Selected = true;
                    item.EnsureVisible(); // Cuộn ListView đến phần tử được chọn

                    break;
                }
            }
        }
        private void DeleteConfugration_Click(object sender, EventArgs e)
        {
            if (lvConfurations.SelectedItems.Count > 0)
            {
                DeleteSelectedConfiguration();
                resetCbxConfugration();
            }
        }
        private void DeleteSelectedConfiguration()
        {
            if (lvConfurations.SelectedItems.Count > 0)
            {
                int selectedIndex = lvConfurations.SelectedIndices[0];

                if (selectedIndex >= 0 && selectedIndex < lvConfurations.Items.Count)
                {
                    ListViewItem configurationItem = lvConfurations.Items[selectedIndex];

                    string folderPath = "setting";
                    string filePath = Path.Combine(folderPath, "settingAuto.json");

                    if (File.Exists(filePath))
                    {
                        string json = File.ReadAllText(filePath);

                        JObject jsonObject = JObject.Parse(json);

                        string configurationName = configurationItem.SubItems[1].Text;

                        if (jsonObject.ContainsKey(configurationName))
                        {
                            jsonObject.Remove(configurationName);

                            // Cập nhật lại ID
                            List<JProperty> properties = jsonObject.Properties().ToList();
                            for (int i = 0; i < properties.Count; i++)
                            {
                                JObject configuration = (JObject)properties[i].Value;
                                configuration["id"] = i + 1;
                            }

                            string updatedJson = jsonObject.ToString();

                            File.WriteAllText(filePath, updatedJson);

                            // Gọi hàm để tải lại dữ liệu trong ListView
                            LoadListViewConfigurations();
                        }
                    }
                }
            }
        }
    }
}
