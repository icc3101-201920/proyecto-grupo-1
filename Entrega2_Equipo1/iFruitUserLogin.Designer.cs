namespace Entrega2_Equipo1
{
    partial class iFruitUserLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iFruitUserLogin));
            this.LogoPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.CreateUserPanel = new System.Windows.Forms.Panel();
            this.NewPasswordTextBox = new System.Windows.Forms.TextBox();
            this.NewUserNameTextBox = new System.Windows.Forms.TextBox();
            this.NewPasswordEmptyErrorLabel = new System.Windows.Forms.Label();
            this.NewUsernameErrorLabel = new System.Windows.Forms.Label();
            this.SignUpButton = new System.Windows.Forms.Button();
            this.NewUserLabel = new System.Windows.Forms.Label();
            this.LogInButton = new System.Windows.Forms.Button();
            this.LogInPasswordTextBox = new System.Windows.Forms.TextBox();
            this.UserNamesComboBox = new System.Windows.Forms.ComboBox();
            this.WrongPasswordLabel = new System.Windows.Forms.Label();
            this.UserPicturePictureBox2 = new Entrega2_Equipo1.OvalPictureBox();
            this.LogoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.CreateUserPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserPicturePictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // LogoPanel
            // 
            this.LogoPanel.Controls.Add(this.pictureBox1);
            this.LogoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.LogoPanel.Location = new System.Drawing.Point(0, 0);
            this.LogoPanel.Name = "LogoPanel";
            this.LogoPanel.Size = new System.Drawing.Size(776, 199);
            this.LogoPanel.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(125, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(515, 196);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // CreateUserPanel
            // 
            this.CreateUserPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(32)))), ((int)(((byte)(39)))));
            this.CreateUserPanel.Controls.Add(this.NewPasswordTextBox);
            this.CreateUserPanel.Controls.Add(this.NewUserNameTextBox);
            this.CreateUserPanel.Controls.Add(this.NewPasswordEmptyErrorLabel);
            this.CreateUserPanel.Controls.Add(this.NewUsernameErrorLabel);
            this.CreateUserPanel.Controls.Add(this.SignUpButton);
            this.CreateUserPanel.Location = new System.Drawing.Point(250, 205);
            this.CreateUserPanel.Name = "CreateUserPanel";
            this.CreateUserPanel.Size = new System.Drawing.Size(262, 230);
            this.CreateUserPanel.TabIndex = 10;
            this.CreateUserPanel.Visible = false;
            // 
            // NewPasswordTextBox
            // 
            this.NewPasswordTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(32)))), ((int)(((byte)(39)))));
            this.NewPasswordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NewPasswordTextBox.Font = new System.Drawing.Font("Elixia", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewPasswordTextBox.ForeColor = System.Drawing.Color.DarkGray;
            this.NewPasswordTextBox.Location = new System.Drawing.Point(22, 104);
            this.NewPasswordTextBox.Name = "NewPasswordTextBox";
            this.NewPasswordTextBox.Size = new System.Drawing.Size(224, 37);
            this.NewPasswordTextBox.TabIndex = 16;
            this.NewPasswordTextBox.Text = "password";
            this.NewPasswordTextBox.Enter += new System.EventHandler(this.NewPasswordTextBox_Enter);
            this.NewPasswordTextBox.Leave += new System.EventHandler(this.NewPasswordTextBox_Leave);
            // 
            // NewUserNameTextBox
            // 
            this.NewUserNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(32)))), ((int)(((byte)(39)))));
            this.NewUserNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NewUserNameTextBox.Font = new System.Drawing.Font("Elixia", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewUserNameTextBox.ForeColor = System.Drawing.Color.DarkGray;
            this.NewUserNameTextBox.Location = new System.Drawing.Point(22, 27);
            this.NewUserNameTextBox.Name = "NewUserNameTextBox";
            this.NewUserNameTextBox.Size = new System.Drawing.Size(224, 37);
            this.NewUserNameTextBox.TabIndex = 15;
            this.NewUserNameTextBox.Text = "username";
            this.NewUserNameTextBox.Enter += new System.EventHandler(this.NewUserNameTextBox_Enter);
            this.NewUserNameTextBox.Leave += new System.EventHandler(this.NewUserNameTextBox_Leave);
            // 
            // NewPasswordEmptyErrorLabel
            // 
            this.NewPasswordEmptyErrorLabel.AutoSize = true;
            this.NewPasswordEmptyErrorLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewPasswordEmptyErrorLabel.ForeColor = System.Drawing.Color.White;
            this.NewPasswordEmptyErrorLabel.Location = new System.Drawing.Point(141, 144);
            this.NewPasswordEmptyErrorLabel.Name = "NewPasswordEmptyErrorLabel";
            this.NewPasswordEmptyErrorLabel.Size = new System.Drawing.Size(105, 15);
            this.NewPasswordEmptyErrorLabel.TabIndex = 14;
            this.NewPasswordEmptyErrorLabel.Text = "Empty password";
            this.NewPasswordEmptyErrorLabel.Visible = false;
            // 
            // NewUsernameErrorLabel
            // 
            this.NewUsernameErrorLabel.AutoSize = true;
            this.NewUsernameErrorLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewUsernameErrorLabel.ForeColor = System.Drawing.Color.White;
            this.NewUsernameErrorLabel.Location = new System.Drawing.Point(43, 67);
            this.NewUsernameErrorLabel.Name = "NewUsernameErrorLabel";
            this.NewUsernameErrorLabel.Size = new System.Drawing.Size(203, 15);
            this.NewUsernameErrorLabel.TabIndex = 13;
            this.NewUsernameErrorLabel.Text = "User already exists or empty";
            this.NewUsernameErrorLabel.Visible = false;
            // 
            // SignUpButton
            // 
            this.SignUpButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SignUpButton.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
            this.SignUpButton.FlatAppearance.BorderSize = 0;
            this.SignUpButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(19)))), ((int)(((byte)(70)))));
            this.SignUpButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(12)))), ((int)(((byte)(89)))));
            this.SignUpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SignUpButton.Font = new System.Drawing.Font("Anurati", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignUpButton.ForeColor = System.Drawing.Color.White;
            this.SignUpButton.Location = new System.Drawing.Point(71, 180);
            this.SignUpButton.Name = "SignUpButton";
            this.SignUpButton.Size = new System.Drawing.Size(124, 47);
            this.SignUpButton.TabIndex = 10;
            this.SignUpButton.Text = "SIGN UP";
            this.SignUpButton.UseVisualStyleBackColor = true;
            this.SignUpButton.Click += new System.EventHandler(this.SignUpButton_Click);
            // 
            // NewUserLabel
            // 
            this.NewUserLabel.AutoSize = true;
            this.NewUserLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NewUserLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewUserLabel.ForeColor = System.Drawing.Color.White;
            this.NewUserLabel.Location = new System.Drawing.Point(390, 504);
            this.NewUserLabel.Name = "NewUserLabel";
            this.NewUserLabel.Size = new System.Drawing.Size(112, 15);
            this.NewUserLabel.TabIndex = 14;
            this.NewUserLabel.Text = "Create new user";
            this.NewUserLabel.Click += new System.EventHandler(this.NewUserLabel_Click);
            this.NewUserLabel.Enter += new System.EventHandler(this.NewUserLabel_MouseEnter);
            this.NewUserLabel.Leave += new System.EventHandler(this.NewUserLabel_MouseLeave);
            // 
            // LogInButton
            // 
            this.LogInButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LogInButton.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
            this.LogInButton.FlatAppearance.BorderSize = 0;
            this.LogInButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(19)))), ((int)(((byte)(70)))));
            this.LogInButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(12)))), ((int)(((byte)(89)))));
            this.LogInButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LogInButton.Font = new System.Drawing.Font("Anurati", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogInButton.ForeColor = System.Drawing.Color.White;
            this.LogInButton.Location = new System.Drawing.Point(311, 587);
            this.LogInButton.Name = "LogInButton";
            this.LogInButton.Size = new System.Drawing.Size(124, 47);
            this.LogInButton.TabIndex = 13;
            this.LogInButton.Text = "LOG  I N";
            this.LogInButton.UseVisualStyleBackColor = true;
            this.LogInButton.Click += new System.EventHandler(this.LogInButton_Click);
            // 
            // LogInPasswordTextBox
            // 
            this.LogInPasswordTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(30)))), ((int)(((byte)(38)))));
            this.LogInPasswordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LogInPasswordTextBox.Font = new System.Drawing.Font("Elixia", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogInPasswordTextBox.ForeColor = System.Drawing.Color.Silver;
            this.LogInPasswordTextBox.Location = new System.Drawing.Point(262, 533);
            this.LogInPasswordTextBox.Name = "LogInPasswordTextBox";
            this.LogInPasswordTextBox.Size = new System.Drawing.Size(240, 30);
            this.LogInPasswordTextBox.TabIndex = 12;
            this.LogInPasswordTextBox.Text = "password";
            this.LogInPasswordTextBox.Enter += new System.EventHandler(this.LogInPasswordTextBox_Enter);
            this.LogInPasswordTextBox.Leave += new System.EventHandler(this.LogInPasswordTextBox_Leave);
            // 
            // UserNamesComboBox
            // 
            this.UserNamesComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(30)))), ((int)(((byte)(38)))));
            this.UserNamesComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UserNamesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UserNamesComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UserNamesComboBox.Font = new System.Drawing.Font("Elixia", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserNamesComboBox.ForeColor = System.Drawing.Color.White;
            this.UserNamesComboBox.FormattingEnabled = true;
            this.UserNamesComboBox.Location = new System.Drawing.Point(262, 463);
            this.UserNamesComboBox.Name = "UserNamesComboBox";
            this.UserNamesComboBox.Size = new System.Drawing.Size(240, 38);
            this.UserNamesComboBox.TabIndex = 11;
            this.UserNamesComboBox.SelectedIndexChanged += new System.EventHandler(this.UserNamesComboBox_SelectedIndexChanged);
            // 
            // WrongPasswordLabel
            // 
            this.WrongPasswordLabel.AutoSize = true;
            this.WrongPasswordLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WrongPasswordLabel.ForeColor = System.Drawing.Color.White;
            this.WrongPasswordLabel.Location = new System.Drawing.Point(527, 545);
            this.WrongPasswordLabel.Name = "WrongPasswordLabel";
            this.WrongPasswordLabel.Size = new System.Drawing.Size(0, 15);
            this.WrongPasswordLabel.TabIndex = 15;
            this.WrongPasswordLabel.Visible = false;
            // 
            // UserPicturePictureBox2
            // 
            this.UserPicturePictureBox2.BackColor = System.Drawing.Color.DarkGray;
            this.UserPicturePictureBox2.Location = new System.Drawing.Point(288, 215);
            this.UserPicturePictureBox2.Name = "UserPicturePictureBox2";
            this.UserPicturePictureBox2.Size = new System.Drawing.Size(195, 200);
            this.UserPicturePictureBox2.TabIndex = 16;
            this.UserPicturePictureBox2.TabStop = false;
            // 
            // iFruitUserLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(30)))), ((int)(((byte)(38)))));
            this.ClientSize = new System.Drawing.Size(776, 678);
            this.Controls.Add(this.CreateUserPanel);
            this.Controls.Add(this.UserPicturePictureBox2);
            this.Controls.Add(this.WrongPasswordLabel);
            this.Controls.Add(this.NewUserLabel);
            this.Controls.Add(this.LogInButton);
            this.Controls.Add(this.LogInPasswordTextBox);
            this.Controls.Add(this.UserNamesComboBox);
            this.Controls.Add(this.LogoPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "iFruitUserLogin";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Log In";
            this.Click += new System.EventHandler(this.IFruitUserLogin_Click);
            this.LogoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.CreateUserPanel.ResumeLayout(false);
            this.CreateUserPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserPicturePictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel LogoPanel;
        private System.Windows.Forms.Panel CreateUserPanel;
        private System.Windows.Forms.TextBox NewUserNameTextBox;
        private System.Windows.Forms.Label NewPasswordEmptyErrorLabel;
        private System.Windows.Forms.Label NewUsernameErrorLabel;
        private System.Windows.Forms.Button SignUpButton;
        private System.Windows.Forms.Label NewUserLabel;
        private System.Windows.Forms.Button LogInButton;
        private System.Windows.Forms.TextBox LogInPasswordTextBox;
        private System.Windows.Forms.ComboBox UserNamesComboBox;
        private System.Windows.Forms.Label WrongPasswordLabel;
        private System.Windows.Forms.TextBox NewPasswordTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private OvalPictureBox UserPicturePictureBox2;
    }
}