namespace ToolOpenChrome
{
    partial class infoPC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(infoPC));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            txtNamePc = new TextBox();
            txtUserPc = new TextBox();
            txtFolderPc = new TextBox();
            txtFolderApp = new TextBox();
            txtMACPc = new TextBox();
            btnReloadInfoPc = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(81, 15);
            label1.TabIndex = 0;
            label1.Text = "Tên máy tính:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ButtonFace;
            label2.Location = new Point(12, 53);
            label2.Name = "label2";
            label2.Size = new Size(97, 15);
            label2.TabIndex = 1;
            label2.Text = "Tên người dùng:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ButtonFace;
            label3.Location = new Point(12, 97);
            label3.Name = "label3";
            label3.Size = new Size(134, 15);
            label3.TabIndex = 2;
            label3.Text = "Thư mục của hệ thống:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ButtonFace;
            label4.Location = new Point(13, 141);
            label4.Name = "label4";
            label4.Size = new Size(169, 15);
            label4.TabIndex = 3;
            label4.Text = "Thư mục ứng dụng mặc định:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ButtonFace;
            label5.Location = new Point(12, 185);
            label5.Name = "label5";
            label5.Size = new Size(76, 15);
            label5.TabIndex = 4;
            label5.Text = "Địa chỉ MAC:";
            // 
            // txtNamePc
            // 
            txtNamePc.Location = new Point(13, 27);
            txtNamePc.Name = "txtNamePc";
            txtNamePc.Size = new Size(277, 23);
            txtNamePc.TabIndex = 5;
            // 
            // txtUserPc
            // 
            txtUserPc.Location = new Point(12, 71);
            txtUserPc.Name = "txtUserPc";
            txtUserPc.Size = new Size(277, 23);
            txtUserPc.TabIndex = 6;
            // 
            // txtFolderPc
            // 
            txtFolderPc.Location = new Point(13, 115);
            txtFolderPc.Name = "txtFolderPc";
            txtFolderPc.Size = new Size(277, 23);
            txtFolderPc.TabIndex = 7;
            // 
            // txtFolderApp
            // 
            txtFolderApp.Location = new Point(13, 159);
            txtFolderApp.Name = "txtFolderApp";
            txtFolderApp.Size = new Size(277, 23);
            txtFolderApp.TabIndex = 8;
            // 
            // txtMACPc
            // 
            txtMACPc.Location = new Point(12, 203);
            txtMACPc.Name = "txtMACPc";
            txtMACPc.Size = new Size(277, 23);
            txtMACPc.TabIndex = 9;
            // 
            // btnReloadInfoPc
            // 
            btnReloadInfoPc.Location = new Point(214, 250);
            btnReloadInfoPc.Name = "btnReloadInfoPc";
            btnReloadInfoPc.Size = new Size(75, 23);
            btnReloadInfoPc.TabIndex = 10;
            btnReloadInfoPc.Text = "Tải lại";
            btnReloadInfoPc.UseVisualStyleBackColor = true;
            btnReloadInfoPc.Click += btnReloadInfoPc_Click;
            // 
            // infoPC
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(303, 285);
            Controls.Add(btnReloadInfoPc);
            Controls.Add(txtMACPc);
            Controls.Add(txtFolderApp);
            Controls.Add(txtFolderPc);
            Controls.Add(txtUserPc);
            Controls.Add(txtNamePc);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "infoPC";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "infoPC";
            Load += infoPC_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox txtNamePc;
        private TextBox txtUserPc;
        private TextBox txtFolderPc;
        private TextBox txtFolderApp;
        private TextBox txtMACPc;
        private Button btnReloadInfoPc;
    }
}