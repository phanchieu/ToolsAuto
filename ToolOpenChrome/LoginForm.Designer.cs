namespace ToolOpenChrome
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            label1 = new Label();
            txtTK = new TextBox();
            label2 = new Label();
            btnLogin = new Button();
            lblcode = new Label();
            label3 = new Label();
            txtMK = new TextBox();
            txtCodePc = new TextBox();
            btnCopyCode = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(25, 15);
            label1.TabIndex = 0;
            label1.Text = "TK:";
            // 
            // txtTK
            // 
            txtTK.Location = new Point(49, 12);
            txtTK.Name = "txtTK";
            txtTK.Size = new Size(173, 23);
            txtTK.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 73);
            label2.Name = "label2";
            label2.Size = new Size(0, 15);
            label2.TabIndex = 2;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(71, 101);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 27);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += button1_Click;
            // 
            // lblcode
            // 
            lblcode.AutoSize = true;
            lblcode.Location = new Point(71, 68);
            lblcode.Name = "lblcode";
            lblcode.Size = new Size(26, 15);
            lblcode.TabIndex = 4;
            lblcode.Text = "abc";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 44);
            label3.Name = "label3";
            label3.Size = new Size(29, 15);
            label3.TabIndex = 5;
            label3.Text = "MK:";
            // 
            // txtMK
            // 
            txtMK.Location = new Point(49, 41);
            txtMK.Name = "txtMK";
            txtMK.Size = new Size(173, 23);
            txtMK.TabIndex = 6;
            // 
            // txtCodePc
            // 
            txtCodePc.Enabled = false;
            txtCodePc.Location = new Point(49, 70);
            txtCodePc.Name = "txtCodePc";
            txtCodePc.ReadOnly = true;
            txtCodePc.Size = new Size(114, 23);
            txtCodePc.TabIndex = 7;
            txtCodePc.TabStop = false;
            txtCodePc.TextChanged += txtCodePc_TextChanged;
            // 
            // btnCopyCode
            // 
            btnCopyCode.FlatAppearance.BorderColor = Color.White;
            btnCopyCode.FlatAppearance.BorderSize = 0;
            btnCopyCode.FlatAppearance.MouseDownBackColor = Color.White;
            btnCopyCode.FlatAppearance.MouseOverBackColor = Color.White;
            btnCopyCode.Location = new Point(169, 71);
            btnCopyCode.Name = "btnCopyCode";
            btnCopyCode.Size = new Size(53, 23);
            btnCopyCode.TabIndex = 8;
            btnCopyCode.Text = "copy";
            btnCopyCode.UseVisualStyleBackColor = true;
            btnCopyCode.Click += btnCopyCode_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(234, 140);
            Controls.Add(btnCopyCode);
            Controls.Add(txtCodePc);
            Controls.Add(txtMK);
            Controls.Add(label3);
            Controls.Add(lblcode);
            Controls.Add(btnLogin);
            Controls.Add(label2);
            Controls.Add(txtTK);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập";
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtTK;
        private Label label2;
        private Button btnLogin;
        private Label lblcode;
        private Label label3;
        private TextBox txtMK;
        private TextBox txtCodePc;
        private Button btnCopyCode;
    }
}