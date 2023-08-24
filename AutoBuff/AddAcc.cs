using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ToolOpenChrome
{
    public partial class AddAcc : Form
    {

        public AddAcc()
        {
            InitializeComponent();
        }

        private void AddAcc_Load(object sender, EventArgs e)
        {
            cbxDinhDangTkLogin.SelectedIndex = 0;
        }


        private void btnThemTaiKhoan_Click(object sender, EventArgs e)
        {
            try
            {
                // Truy cập Form cha và thêm phần tử vào ListView của nó
                FormMain? parentForm = this.Owner as FormMain;
                if (parentForm != null)
                {
                    string input = rtxtAddAcc.Text;
                    List<User> userList = new List<User>();

                    string[] lines = input.Split('\n');
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        User user = new User
                        {
                            Username = parts[0],
                            Password = parts[1],
                            Cookie = parts[2]
                        };
                        userList.Add(user);
                    }
                    foreach (User user in userList)
                    {
                        // Lấy đối tượng ListView của form cha
                        //System.Windows.Forms.ListView? parentListView = parentForm.Controls["lvDSTK"] as System.Windows.Forms.ListView;

                        // Lấy số lượng phần tử trong ListView
                        int count = parentForm.ListViewControl.Items.Count + 1;

                        ListViewItem item = new ListViewItem();
                        item.SubItems.Add(count.ToString());
                        item.SubItems.Add(user.Username);
                        item.SubItems.Add(user.Password);
                        item.SubItems.Add(user.Cookie);
                        parentForm.ListViewControl.Items.Add(item);
                    }
                    bool flag = true; // biến boolean cần chuyển đi là flag và có giá trị true
                    MyEvent?.Invoke(this, flag);
                }
                MessageBox.Show("Thêm tài khoản thành công");
                rtxtAddAcc.Text = "";
            }
            catch
            {
                MessageBox.Show("Vui lòng nhập đúng định dạng!");
            }
        }
        public event EventHandler<bool> MyEvent;

    }
}
