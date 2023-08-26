namespace Test
{
    partial class Form1
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
            BtnTestProfile = new Button();
            SuspendLayout();
            // 
            // BtnTestProfile
            // 
            BtnTestProfile.Location = new Point(66, 55);
            BtnTestProfile.Name = "BtnTestProfile";
            BtnTestProfile.Size = new Size(75, 23);
            BtnTestProfile.TabIndex = 0;
            BtnTestProfile.Text = "TestProfile";
            BtnTestProfile.UseVisualStyleBackColor = true;
            BtnTestProfile.Click += BtnTestProfile_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(339, 217);
            Controls.Add(BtnTestProfile);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button BtnTestProfile;
    }
}