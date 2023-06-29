namespace ToolOpenChrome
{
    partial class ChangeIP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeIP));
            ra_Chose_tmproxy = new RadioButton();
            groupBox1 = new GroupBox();
            rtxt_tmproxy = new RichTextBox();
            btn_saveChangeIp = new Button();
            btnCloseChangeIp = new Button();
            cb_changeInfoPC = new CheckBox();
            cb_changeMAC = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // ra_Chose_tmproxy
            // 
            ra_Chose_tmproxy.AutoSize = true;
            ra_Chose_tmproxy.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            ra_Chose_tmproxy.ForeColor = SystemColors.ButtonHighlight;
            ra_Chose_tmproxy.Location = new Point(12, 12);
            ra_Chose_tmproxy.Name = "ra_Chose_tmproxy";
            ra_Chose_tmproxy.Size = new Size(99, 19);
            ra_Chose_tmproxy.TabIndex = 0;
            ra_Chose_tmproxy.TabStop = true;
            ra_Chose_tmproxy.Text = "tmproxy.com";
            ra_Chose_tmproxy.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rtxt_tmproxy);
            groupBox1.ForeColor = SystemColors.Control;
            groupBox1.Location = new Point(12, 37);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(315, 93);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Key tmproxy";
            // 
            // rtxt_tmproxy
            // 
            rtxt_tmproxy.Location = new Point(6, 22);
            rtxt_tmproxy.Name = "rtxt_tmproxy";
            rtxt_tmproxy.Size = new Size(303, 63);
            rtxt_tmproxy.TabIndex = 0;
            rtxt_tmproxy.Text = "";
            // 
            // btn_saveChangeIp
            // 
            btn_saveChangeIp.Location = new Point(160, 183);
            btn_saveChangeIp.Name = "btn_saveChangeIp";
            btn_saveChangeIp.Size = new Size(75, 23);
            btn_saveChangeIp.TabIndex = 2;
            btn_saveChangeIp.Text = "Lưu";
            btn_saveChangeIp.UseVisualStyleBackColor = true;
            btn_saveChangeIp.Click += btn_saveChangeIp_Click;
            // 
            // btnCloseChangeIp
            // 
            btnCloseChangeIp.Location = new Point(252, 183);
            btnCloseChangeIp.Name = "btnCloseChangeIp";
            btnCloseChangeIp.Size = new Size(75, 23);
            btnCloseChangeIp.TabIndex = 3;
            btnCloseChangeIp.Text = "Thoát";
            btnCloseChangeIp.UseVisualStyleBackColor = true;
            btnCloseChangeIp.Click += btnCloseChangeIp_Click;
            // 
            // cb_changeInfoPC
            // 
            cb_changeInfoPC.AutoSize = true;
            cb_changeInfoPC.ForeColor = SystemColors.ButtonHighlight;
            cb_changeInfoPC.Location = new Point(12, 136);
            cb_changeInfoPC.Name = "cb_changeInfoPC";
            cb_changeInfoPC.Size = new Size(125, 19);
            cb_changeInfoPC.TabIndex = 5;
            cb_changeInfoPC.Text = "Đổi thông tin máy";
            cb_changeInfoPC.UseVisualStyleBackColor = true;
            // 
            // cb_changeMAC
            // 
            cb_changeMAC.AutoSize = true;
            cb_changeMAC.ForeColor = SystemColors.ButtonHighlight;
            cb_changeMAC.Location = new Point(12, 161);
            cb_changeMAC.Name = "cb_changeMAC";
            cb_changeMAC.Size = new Size(128, 19);
            cb_changeMAC.TabIndex = 6;
            cb_changeMAC.Text = "Đổi thông tin MAC";
            cb_changeMAC.UseVisualStyleBackColor = true;
            // 
            // ChangeIP
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(339, 218);
            Controls.Add(cb_changeMAC);
            Controls.Add(cb_changeInfoPC);
            Controls.Add(btnCloseChangeIp);
            Controls.Add(btn_saveChangeIp);
            Controls.Add(groupBox1);
            Controls.Add(ra_Chose_tmproxy);
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "ChangeIP";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cài đặt IP";
            Load += ChangeIP_Load;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton ra_Chose_tmproxy;
        private GroupBox groupBox1;
        private RichTextBox rtxt_tmproxy;
        private Button btn_saveChangeIp;
        private Button btnCloseChangeIp;
        private CheckBox cb_changeInfoPC;
        private CheckBox cb_changeMAC;
    }
}