namespace SocketServer_v2
{
    partial class Server
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.clientsListBox = new System.Windows.Forms.ListBox();
            this.beginAcceptButton = new System.Windows.Forms.Button();
            this.startListeningButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.stopListeningButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.clientsListBox);
            this.groupBox2.Location = new System.Drawing.Point(286, 138);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(218, 174);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Clients";
            // 
            // clientsListBox
            // 
            this.clientsListBox.FormattingEnabled = true;
            this.clientsListBox.Location = new System.Drawing.Point(9, 19);
            this.clientsListBox.Name = "clientsListBox";
            this.clientsListBox.Size = new System.Drawing.Size(203, 147);
            this.clientsListBox.TabIndex = 0;
            // 
            // beginAcceptButton
            // 
            this.beginAcceptButton.Location = new System.Drawing.Point(286, 61);
            this.beginAcceptButton.Name = "beginAcceptButton";
            this.beginAcceptButton.Size = new System.Drawing.Size(218, 31);
            this.beginAcceptButton.TabIndex = 4;
            this.beginAcceptButton.Text = "Begin accepting";
            this.beginAcceptButton.UseVisualStyleBackColor = true;
            this.beginAcceptButton.Click += new System.EventHandler(this.beginAcceptButton_Click);
            // 
            // startListeningButton
            // 
            this.startListeningButton.Location = new System.Drawing.Point(286, 24);
            this.startListeningButton.Name = "startListeningButton";
            this.startListeningButton.Size = new System.Drawing.Size(218, 31);
            this.startListeningButton.TabIndex = 5;
            this.startListeningButton.Text = "Start listening";
            this.startListeningButton.UseVisualStyleBackColor = true;
            this.startListeningButton.Click += new System.EventHandler(this.startListeningButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.logListBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 300);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.Location = new System.Drawing.Point(6, 19);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(256, 277);
            this.logListBox.TabIndex = 0;
            // 
            // stopListeningButton
            // 
            this.stopListeningButton.Location = new System.Drawing.Point(286, 98);
            this.stopListeningButton.Name = "stopListeningButton";
            this.stopListeningButton.Size = new System.Drawing.Size(218, 31);
            this.stopListeningButton.TabIndex = 4;
            this.stopListeningButton.Text = "Stop listening";
            this.stopListeningButton.UseVisualStyleBackColor = true;
            this.stopListeningButton.Click += new System.EventHandler(this.stopListeningButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 327);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.stopListeningButton);
            this.Controls.Add(this.beginAcceptButton);
            this.Controls.Add(this.startListeningButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Server";
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox clientsListBox;
        private System.Windows.Forms.Button beginAcceptButton;
        private System.Windows.Forms.Button startListeningButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.Button stopListeningButton;
    }
}

