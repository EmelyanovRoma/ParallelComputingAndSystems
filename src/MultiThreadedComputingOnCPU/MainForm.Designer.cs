namespace MultiThreadedComputingOnCPU
{
    partial class MainForm
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
            this.PictureBoxPanel = new System.Windows.Forms.Panel();
            this.ImagePictureBox = new System.Windows.Forms.PictureBox();
            this.ClearPictureBoxButton = new System.Windows.Forms.Button();
            this.SaveImageButton = new System.Windows.Forms.Button();
            this.OpenImageButton = new System.Windows.Forms.Button();
            this.EroseImageButton = new System.Windows.Forms.Button();
            this.ImageProcessingTimeTextBox = new System.Windows.Forms.TextBox();
            this.BlurImageButton = new System.Windows.Forms.Button();
            this.ImageOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ImageSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.NumberOfThreadsComboBox = new System.Windows.Forms.ComboBox();
            this.PictureBoxPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureBoxPanel
            // 
            this.PictureBoxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureBoxPanel.AutoScroll = true;
            this.PictureBoxPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureBoxPanel.Controls.Add(this.ImagePictureBox);
            this.PictureBoxPanel.Location = new System.Drawing.Point(2, 36);
            this.PictureBoxPanel.Name = "PictureBoxPanel";
            this.PictureBoxPanel.Size = new System.Drawing.Size(796, 411);
            this.PictureBoxPanel.TabIndex = 0;
            // 
            // ImagePictureBox
            // 
            this.ImagePictureBox.Location = new System.Drawing.Point(-1, -1);
            this.ImagePictureBox.Name = "ImagePictureBox";
            this.ImagePictureBox.Size = new System.Drawing.Size(100, 50);
            this.ImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImagePictureBox.TabIndex = 0;
            this.ImagePictureBox.TabStop = false;
            // 
            // ClearPictureBoxButton
            // 
            this.ClearPictureBoxButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearPictureBoxButton.Location = new System.Drawing.Point(723, 7);
            this.ClearPictureBoxButton.Name = "ClearPictureBoxButton";
            this.ClearPictureBoxButton.Size = new System.Drawing.Size(75, 23);
            this.ClearPictureBoxButton.TabIndex = 1;
            this.ClearPictureBoxButton.Text = "Clear";
            this.ClearPictureBoxButton.UseVisualStyleBackColor = true;
            this.ClearPictureBoxButton.Click += new System.EventHandler(this.ClearPictureBoxButton_Click);
            // 
            // SaveImageButton
            // 
            this.SaveImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveImageButton.Location = new System.Drawing.Point(642, 7);
            this.SaveImageButton.Name = "SaveImageButton";
            this.SaveImageButton.Size = new System.Drawing.Size(75, 23);
            this.SaveImageButton.TabIndex = 2;
            this.SaveImageButton.Text = "Save";
            this.SaveImageButton.UseVisualStyleBackColor = true;
            this.SaveImageButton.Click += new System.EventHandler(this.SaveImageButton_Click);
            // 
            // OpenImageButton
            // 
            this.OpenImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenImageButton.Location = new System.Drawing.Point(561, 7);
            this.OpenImageButton.Name = "OpenImageButton";
            this.OpenImageButton.Size = new System.Drawing.Size(75, 23);
            this.OpenImageButton.TabIndex = 3;
            this.OpenImageButton.Text = "Open";
            this.OpenImageButton.UseVisualStyleBackColor = true;
            this.OpenImageButton.Click += new System.EventHandler(this.OpenImageButton_Click);
            // 
            // EroseImageButton
            // 
            this.EroseImageButton.Location = new System.Drawing.Point(2, 7);
            this.EroseImageButton.Name = "EroseImageButton";
            this.EroseImageButton.Size = new System.Drawing.Size(75, 23);
            this.EroseImageButton.TabIndex = 4;
            this.EroseImageButton.Text = "Erose";
            this.EroseImageButton.UseVisualStyleBackColor = true;
            this.EroseImageButton.Click += new System.EventHandler(this.EroseImageButton_Click);
            // 
            // ImageProcessingTimeTextBox
            // 
            this.ImageProcessingTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageProcessingTimeTextBox.BackColor = System.Drawing.Color.White;
            this.ImageProcessingTimeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageProcessingTimeTextBox.Enabled = false;
            this.ImageProcessingTimeTextBox.Location = new System.Drawing.Point(349, 12);
            this.ImageProcessingTimeTextBox.Name = "ImageProcessingTimeTextBox";
            this.ImageProcessingTimeTextBox.Size = new System.Drawing.Size(100, 20);
            this.ImageProcessingTimeTextBox.TabIndex = 5;
            this.ImageProcessingTimeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BlurImageButton
            // 
            this.BlurImageButton.Location = new System.Drawing.Point(83, 7);
            this.BlurImageButton.Name = "BlurImageButton";
            this.BlurImageButton.Size = new System.Drawing.Size(75, 23);
            this.BlurImageButton.TabIndex = 6;
            this.BlurImageButton.Text = "Blur";
            this.BlurImageButton.UseVisualStyleBackColor = true;
            this.BlurImageButton.Click += new System.EventHandler(this.BlurImageButton_Click);
            // 
            // ImageOpenFileDialog
            // 
            this.ImageOpenFileDialog.Title = "Open Image";
            // 
            // ImageSaveFileDialog
            // 
            this.ImageSaveFileDialog.FileName = "Untitled.png";
            this.ImageSaveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            this.ImageSaveFileDialog.Title = "Save Image";
            // 
            // NumberOfThreadsComboBox
            // 
            this.NumberOfThreadsComboBox.FormattingEnabled = true;
            this.NumberOfThreadsComboBox.Items.AddRange(new object[] {
            "2",
            "4",
            "6",
            "8",
            "10",
            "12",
            "14",
            "16"});
            this.NumberOfThreadsComboBox.Location = new System.Drawing.Point(164, 9);
            this.NumberOfThreadsComboBox.Name = "NumberOfThreadsComboBox";
            this.NumberOfThreadsComboBox.Size = new System.Drawing.Size(41, 21);
            this.NumberOfThreadsComboBox.TabIndex = 7;
            this.NumberOfThreadsComboBox.SelectedIndexChanged += new System.EventHandler(this.NumberOfThreadsComboBox_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.NumberOfThreadsComboBox);
            this.Controls.Add(this.BlurImageButton);
            this.Controls.Add(this.ImageProcessingTimeTextBox);
            this.Controls.Add(this.EroseImageButton);
            this.Controls.Add(this.OpenImageButton);
            this.Controls.Add(this.SaveImageButton);
            this.Controls.Add(this.ClearPictureBoxButton);
            this.Controls.Add(this.PictureBoxPanel);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.PictureBoxPanel.ResumeLayout(false);
            this.PictureBoxPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PictureBoxPanel;
        private System.Windows.Forms.PictureBox ImagePictureBox;
        private System.Windows.Forms.Button ClearPictureBoxButton;
        private System.Windows.Forms.Button SaveImageButton;
        private System.Windows.Forms.Button OpenImageButton;
        private System.Windows.Forms.Button EroseImageButton;
        private System.Windows.Forms.TextBox ImageProcessingTimeTextBox;
        private System.Windows.Forms.Button BlurImageButton;
        private System.Windows.Forms.OpenFileDialog ImageOpenFileDialog;
        private System.Windows.Forms.SaveFileDialog ImageSaveFileDialog;
        private System.Windows.Forms.ComboBox NumberOfThreadsComboBox;
    }
}

