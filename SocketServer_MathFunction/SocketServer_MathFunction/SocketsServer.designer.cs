namespace SocketServer_MathFunction
{
    partial class SocketsServer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.powerTextBox = new System.Windows.Forms.TextBox();
            this.pointTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.functionDescriptionRichTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.workerPathTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.workersList = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.timeLeftLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressIdicator = new System.Windows.Forms.ProgressBar();
            this.startExecutionButton = new System.Windows.Forms.Button();
            this.stopExecutionButton = new System.Windows.Forms.Button();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.powerTextBox);
            this.groupBox1.Controls.Add(this.pointTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.functionDescriptionRichTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(291, 156);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Function to express";
            // 
            // powerTextBox
            // 
            this.powerTextBox.Location = new System.Drawing.Point(192, 128);
            this.powerTextBox.Name = "powerTextBox";
            this.powerTextBox.Size = new System.Drawing.Size(93, 20);
            this.powerTextBox.TabIndex = 2;
            // 
            // pointTextBox
            // 
            this.pointTextBox.Location = new System.Drawing.Point(44, 128);
            this.pointTextBox.Name = "pointTextBox";
            this.pointTextBox.Size = new System.Drawing.Size(93, 20);
            this.pointTextBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(149, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Power";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Point";
            // 
            // functionDescriptionRichTextBox
            // 
            this.functionDescriptionRichTextBox.Location = new System.Drawing.Point(3, 16);
            this.functionDescriptionRichTextBox.Name = "functionDescriptionRichTextBox";
            this.functionDescriptionRichTextBox.ReadOnly = true;
            this.functionDescriptionRichTextBox.Size = new System.Drawing.Size(282, 109);
            this.functionDescriptionRichTextBox.TabIndex = 0;
            this.functionDescriptionRichTextBox.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.workerPathTextBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.workersList);
            this.groupBox2.Location = new System.Drawing.Point(309, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(221, 224);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Workers";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(118, 195);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Path to worker";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 195);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Open worker";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // workerPathTextBox
            // 
            this.workerPathTextBox.Enabled = false;
            this.workerPathTextBox.Location = new System.Drawing.Point(78, 169);
            this.workerPathTextBox.Name = "workerPathTextBox";
            this.workerPathTextBox.Size = new System.Drawing.Size(137, 20);
            this.workerPathTextBox.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 169);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Worker path";
            // 
            // workersList
            // 
            this.workersList.FormattingEnabled = true;
            this.workersList.Location = new System.Drawing.Point(6, 19);
            this.workersList.Name = "workersList";
            this.workersList.Size = new System.Drawing.Size(209, 147);
            this.workersList.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.resultTextBox);
            this.groupBox3.Controls.Add(this.logTextBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(12, 174);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 91);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Run info";
            // 
            // resultTextBox
            // 
            this.resultTextBox.Location = new System.Drawing.Point(81, 59);
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.Size = new System.Drawing.Size(204, 20);
            this.resultTextBox.TabIndex = 2;
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(81, 25);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.Size = new System.Drawing.Size(204, 20);
            this.logTextBox.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "result file name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "log file name:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.timeLeftLabel);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.progressIdicator);
            this.groupBox4.Location = new System.Drawing.Point(12, 271);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(518, 59);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Progress";
            // 
            // timeLeftLabel
            // 
            this.timeLeftLabel.AutoSize = true;
            this.timeLeftLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.timeLeftLabel.Location = new System.Drawing.Point(353, 20);
            this.timeLeftLabel.Name = "timeLeftLabel";
            this.timeLeftLabel.Size = new System.Drawing.Size(56, 25);
            this.timeLeftLabel.TabIndex = 2;
            this.timeLeftLabel.Text = "####";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(297, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time spent:";
            // 
            // progressIdicator
            // 
            this.progressIdicator.Location = new System.Drawing.Point(6, 19);
            this.progressIdicator.Name = "progressIdicator";
            this.progressIdicator.Size = new System.Drawing.Size(285, 34);
            this.progressIdicator.TabIndex = 0;
            // 
            // startExecutionButton
            // 
            this.startExecutionButton.Location = new System.Drawing.Point(309, 242);
            this.startExecutionButton.Name = "startExecutionButton";
            this.startExecutionButton.Size = new System.Drawing.Size(112, 32);
            this.startExecutionButton.TabIndex = 4;
            this.startExecutionButton.Text = "Start execution";
            this.startExecutionButton.UseVisualStyleBackColor = true;
            this.startExecutionButton.Click += new System.EventHandler(this.startExecutionButton_Click);
            // 
            // stopExecutionButton
            // 
            this.stopExecutionButton.Location = new System.Drawing.Point(427, 242);
            this.stopExecutionButton.Name = "stopExecutionButton";
            this.stopExecutionButton.Size = new System.Drawing.Size(103, 32);
            this.stopExecutionButton.TabIndex = 4;
            this.stopExecutionButton.Text = "Stop execution";
            this.stopExecutionButton.UseVisualStyleBackColor = true;
            this.stopExecutionButton.Click += new System.EventHandler(this.stopExecutionButton_Click);
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.HorizontalScrollbar = true;
            this.logListBox.Location = new System.Drawing.Point(536, 44);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(290, 290);
            this.logListBox.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(536, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Log:";
            // 
            // SocketsServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 342);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.logListBox);
            this.Controls.Add(this.stopExecutionButton);
            this.Controls.Add(this.startExecutionButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SocketsServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SocketsServer";
            this.Load += new System.EventHandler(this.SocketsServer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox workersList;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox resultTextBox;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label timeLeftLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressIdicator;
        private System.Windows.Forms.Button startExecutionButton;
        private System.Windows.Forms.Button stopExecutionButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox workerPathTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox functionDescriptionRichTextBox;
        private System.Windows.Forms.TextBox powerTextBox;
        private System.Windows.Forms.TextBox pointTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}

