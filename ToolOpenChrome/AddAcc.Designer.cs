namespace ToolOpenChrome
{
    partial class AddAcc
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAcc));
            groupBox1 = new GroupBox();
            rtxtAddAcc = new RichTextBox();
            btnThemTaiKhoan = new Button();
            cbxDinhDangTkLogin = new ComboBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rtxtAddAcc);
            groupBox1.ForeColor = SystemColors.ButtonHighlight;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(483, 187);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Thêm danh sách tài khoản";
            // 
            // rtxtAddAcc
            // 
            rtxtAddAcc.Location = new Point(6, 22);
            rtxtAddAcc.Name = "rtxtAddAcc";
            rtxtAddAcc.Size = new Size(471, 159);
            rtxtAddAcc.TabIndex = 0;
            rtxtAddAcc.Text = "";
            // 
            // btnThemTaiKhoan
            // 
            btnThemTaiKhoan.Location = new Point(420, 207);
            btnThemTaiKhoan.Name = "btnThemTaiKhoan";
            btnThemTaiKhoan.Size = new Size(75, 23);
            btnThemTaiKhoan.TabIndex = 1;
            btnThemTaiKhoan.Text = "Thêm";
            btnThemTaiKhoan.UseVisualStyleBackColor = true;
            btnThemTaiKhoan.Click += btnThemTaiKhoan_Click;
            // 
            // cbxDinhDangTkLogin
            // 
            cbxDinhDangTkLogin.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxDinhDangTkLogin.FormattingEnabled = true;
            cbxDinhDangTkLogin.Items.AddRange(new object[] { "user|pass|cookie" });
            cbxDinhDangTkLogin.Location = new Point(12, 207);
            cbxDinhDangTkLogin.Name = "cbxDinhDangTkLogin";
            cbxDinhDangTkLogin.Size = new Size(396, 23);
            cbxDinhDangTkLogin.TabIndex = 2;
            // 
            // AddAcc
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(507, 252);
            Controls.Add(cbxDinhDangTkLogin);
            Controls.Add(btnThemTaiKhoan);
            Controls.Add(groupBox1);
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "AddAcc";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Thêm tài khoản";
            Load += AddAcc_Load;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btnThemTaiKhoan;
        private ComboBox cbxDinhDangTkLogin;
        private RichTextBox rtxtAddAcc;
    }
}