namespace ToolOpenChrome
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            groupBox1 = new GroupBox();
            settingAvanced = new Button();
            btnUpdateChrome = new Button();
            button4 = new Button();
            button3 = new Button();
            btnStop = new Button();
            btnStart = new Button();
            clnCheckbox = new ColumnHeader();
            clnSTT = new ColumnHeader();
            clnTK = new ColumnHeader();
            lvDSTK = new ListView();
            clnCKB = new ColumnHeader();
            clnSoThuTu = new ColumnHeader();
            clnTaiKhoan = new ColumnHeader();
            clnMatKhau = new ColumnHeader();
            clnCookie = new ColumnHeader();
            clnStatus = new ColumnHeader();
            click_mouseRight = new ContextMenuStrip(components);
            lv_select_item = new ToolStripMenuItem();
            lv_select_item_All = new ToolStripMenuItem();
            lv_select_item_BoiDen = new ToolStripMenuItem();
            lv_unSelect_item = new ToolStripMenuItem();
            lv_unSelect_item_All = new ToolStripMenuItem();
            lv_unSelect_item_BoiDen = new ToolStripMenuItem();
            xóaToolStripMenuItem = new ToolStripMenuItem();
            deleteAllAcc = new ToolStripMenuItem();
            deleteAccSelect = new ToolStripMenuItem();
            ctmn_RefreshData = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            click_mouseRight.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(settingAvanced);
            groupBox1.Controls.Add(btnUpdateChrome);
            groupBox1.Controls.Add(button4);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(btnStop);
            groupBox1.Controls.Add(btnStart);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            groupBox1.ForeColor = SystemColors.ButtonFace;
            groupBox1.Location = new Point(12, 300);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(719, 65);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Chức năng";
            // 
            // settingAvanced
            // 
            settingAvanced.ForeColor = SystemColors.ActiveCaptionText;
            settingAvanced.Location = new Point(570, 22);
            settingAvanced.Name = "settingAvanced";
            settingAvanced.Size = new Size(143, 32);
            settingAvanced.TabIndex = 5;
            settingAvanced.Text = "Cài đặt khác";
            settingAvanced.UseVisualStyleBackColor = true;
            settingAvanced.Click += button6_Click;
            // 
            // btnUpdateChrome
            // 
            btnUpdateChrome.ForeColor = SystemColors.ActiveCaptionText;
            btnUpdateChrome.Location = new Point(422, 22);
            btnUpdateChrome.Name = "btnUpdateChrome";
            btnUpdateChrome.Size = new Size(142, 32);
            btnUpdateChrome.TabIndex = 4;
            btnUpdateChrome.Text = "Cập Nhật Chrome";
            btnUpdateChrome.UseVisualStyleBackColor = true;
            btnUpdateChrome.Click += btnUpdateChrome_Click;
            // 
            // button4
            // 
            button4.ForeColor = SystemColors.ActiveCaptionText;
            button4.Location = new Point(318, 22);
            button4.Name = "button4";
            button4.Size = new Size(98, 32);
            button4.TabIndex = 3;
            button4.Text = "Thêm TK";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.ForeColor = SystemColors.ActiveCaptionText;
            button3.Location = new Point(214, 22);
            button3.Name = "button3";
            button3.Size = new Size(98, 32);
            button3.TabIndex = 2;
            button3.Text = "Đổi IP";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // btnStop
            // 
            btnStop.ForeColor = SystemColors.ActiveCaptionText;
            btnStop.Location = new Point(110, 22);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(98, 32);
            btnStop.TabIndex = 1;
            btnStop.Text = "Dừng";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.ForeColor = SystemColors.ActiveCaptionText;
            btnStart.Location = new Point(6, 22);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(98, 32);
            btnStart.TabIndex = 0;
            btnStart.Text = "Chạy";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // clnCheckbox
            // 
            clnCheckbox.Text = "";
            clnCheckbox.Width = 20;
            // 
            // clnSTT
            // 
            clnSTT.Text = "STT";
            clnSTT.TextAlign = HorizontalAlignment.Center;
            clnSTT.Width = 40;
            // 
            // clnTK
            // 
            clnTK.Text = "Tài Khoản";
            clnTK.TextAlign = HorizontalAlignment.Center;
            clnTK.Width = 110;
            // 
            // lvDSTK
            // 
            lvDSTK.CheckBoxes = true;
            lvDSTK.Columns.AddRange(new ColumnHeader[] { clnCKB, clnSoThuTu, clnTaiKhoan, clnMatKhau, clnCookie, clnStatus });
            lvDSTK.ContextMenuStrip = click_mouseRight;
            lvDSTK.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lvDSTK.GridLines = true;
            lvDSTK.Location = new Point(12, 12);
            lvDSTK.Name = "lvDSTK";
            lvDSTK.ShowItemToolTips = true;
            lvDSTK.Size = new Size(719, 282);
            lvDSTK.TabIndex = 2;
            lvDSTK.UseCompatibleStateImageBehavior = false;
            lvDSTK.View = View.Details;
            lvDSTK.ItemChecked += lvDSTK_ItemChecked;
            // 
            // clnCKB
            // 
            clnCKB.Text = "";
            clnCKB.Width = 20;
            // 
            // clnSoThuTu
            // 
            clnSoThuTu.Text = "STT";
            clnSoThuTu.TextAlign = HorizontalAlignment.Center;
            clnSoThuTu.Width = 40;
            // 
            // clnTaiKhoan
            // 
            clnTaiKhoan.Text = "Tài Khoản";
            clnTaiKhoan.TextAlign = HorizontalAlignment.Center;
            clnTaiKhoan.Width = 110;
            // 
            // clnMatKhau
            // 
            clnMatKhau.Text = "Mật Khẩu";
            clnMatKhau.TextAlign = HorizontalAlignment.Center;
            clnMatKhau.Width = 110;
            // 
            // clnCookie
            // 
            clnCookie.Text = "Cookie";
            clnCookie.TextAlign = HorizontalAlignment.Center;
            clnCookie.Width = 130;
            // 
            // clnStatus
            // 
            clnStatus.Text = "Trạng Thái";
            clnStatus.TextAlign = HorizontalAlignment.Center;
            clnStatus.Width = 280;
            // 
            // click_mouseRight
            // 
            click_mouseRight.Items.AddRange(new ToolStripItem[] { lv_select_item, lv_unSelect_item, xóaToolStripMenuItem, ctmn_RefreshData });
            click_mouseRight.Name = "contextMenuStrip1";
            click_mouseRight.Size = new Size(144, 92);
            // 
            // lv_select_item
            // 
            lv_select_item.DropDownItems.AddRange(new ToolStripItem[] { lv_select_item_All, lv_select_item_BoiDen });
            lv_select_item.Name = "lv_select_item";
            lv_select_item.Size = new Size(143, 22);
            lv_select_item.Text = "Chọn";
            // 
            // lv_select_item_All
            // 
            lv_select_item_All.Name = "lv_select_item_All";
            lv_select_item_All.Size = new Size(176, 22);
            lv_select_item_All.Text = "Tất cả";
            lv_select_item_All.Click += lv_select_item_All_Click;
            // 
            // lv_select_item_BoiDen
            // 
            lv_select_item_BoiDen.Name = "lv_select_item_BoiDen";
            lv_select_item_BoiDen.Size = new Size(176, 22);
            lv_select_item_BoiDen.Text = "Dòng đang bôi đen";
            lv_select_item_BoiDen.Click += lv_select_item_BoiDen_Click;
            // 
            // lv_unSelect_item
            // 
            lv_unSelect_item.DropDownItems.AddRange(new ToolStripItem[] { lv_unSelect_item_All, lv_unSelect_item_BoiDen });
            lv_unSelect_item.Name = "lv_unSelect_item";
            lv_unSelect_item.Size = new Size(143, 22);
            lv_unSelect_item.Text = "Bỏ chọn";
            // 
            // lv_unSelect_item_All
            // 
            lv_unSelect_item_All.Name = "lv_unSelect_item_All";
            lv_unSelect_item_All.Size = new Size(176, 22);
            lv_unSelect_item_All.Text = "Tất cả";
            lv_unSelect_item_All.Click += lv_unSelect_item_All_Click;
            // 
            // lv_unSelect_item_BoiDen
            // 
            lv_unSelect_item_BoiDen.Name = "lv_unSelect_item_BoiDen";
            lv_unSelect_item_BoiDen.Size = new Size(176, 22);
            lv_unSelect_item_BoiDen.Text = "Dòng đang bôi đen";
            lv_unSelect_item_BoiDen.Click += lv_unSelect_item_BoiDen_Click;
            // 
            // xóaToolStripMenuItem
            // 
            xóaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { deleteAllAcc, deleteAccSelect });
            xóaToolStripMenuItem.Name = "xóaToolStripMenuItem";
            xóaToolStripMenuItem.Size = new Size(143, 22);
            xóaToolStripMenuItem.Text = "Xóa";
            // 
            // deleteAllAcc
            // 
            deleteAllAcc.Name = "deleteAllAcc";
            deleteAllAcc.Size = new Size(236, 22);
            deleteAllAcc.Text = "Xóa tất cả";
            deleteAllAcc.Click += deleteAllAcc_Click;
            // 
            // deleteAccSelect
            // 
            deleteAccSelect.Name = "deleteAccSelect";
            deleteAccSelect.Size = new Size(236, 22);
            deleteAccSelect.Text = "Xóa tài khoản đang được chọn";
            deleteAccSelect.Click += deleteAccSelect_Click;
            // 
            // ctmn_RefreshData
            // 
            ctmn_RefreshData.Name = "ctmn_RefreshData";
            ctmn_RefreshData.Size = new Size(143, 22);
            ctmn_RefreshData.Text = "Tải lại dữ liệu";
            ctmn_RefreshData.Click += ctmn_RefreshData_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(743, 376);
            Controls.Add(lvDSTK);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "FormMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SUPER-AUTO-BASIC  /v1.0.7/";
            FormClosing += FormMain_FormClosing;
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            click_mouseRight.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ColumnHeader clnSTT;
        private GroupBox groupBox1;
        private Button btnUpdateChrome;
        private Button button4;
        private Button button3;
        private Button btnStop;
        private Button btnStart;
        private Button settingAvanced;
        private ColumnHeader clnCheckbox;
        private ColumnHeader clnTK;
        private ListView lvDSTK;
        private ColumnHeader clnCKB;
        private ColumnHeader clnSoThuTu;
        private ColumnHeader clnTaiKhoan;
        private ColumnHeader clnMatKhau;
        private ColumnHeader clnCookie;
        private ColumnHeader clnStatus;
        private ContextMenuStrip click_mouseRight;
        private ToolStripMenuItem lv_select_item;
        private ToolStripMenuItem lv_select_item_All;
        private ToolStripMenuItem lv_select_item_BoiDen;
        private ToolStripMenuItem lv_unSelect_item;
        private ToolStripMenuItem lv_unSelect_item_All;
        private ToolStripMenuItem lv_unSelect_item_BoiDen;
        private ToolStripMenuItem xóaToolStripMenuItem;
        private ToolStripMenuItem deleteAllAcc;
        private ToolStripMenuItem deleteAccSelect;
        private ToolStripMenuItem ctmn_RefreshData;
    }
}