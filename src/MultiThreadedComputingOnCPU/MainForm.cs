using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MultiThreadedComputingOnCPU
{
    public partial class MainForm : Form
    {
        private int _threads;

        public MainForm()
        {
            InitializeComponent();
            NumberOfThreadsComboBox.SelectedIndex = 0;
            _threads = int.Parse(NumberOfThreadsComboBox.SelectedItem.ToString());
        }

        private void EroseImageButton_Click(object sender, EventArgs e)
        {
            Filters.Threads = _threads;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();            

            ImagePictureBox.Image = 
                Filters.Erosion.ApplyErossion((Bitmap)ImagePictureBox.Image);

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            ImageProcessingTimeTextBox.Text = 
                ts.Seconds.ToString() + "." +
                ts.Milliseconds.ToString() + " sec";

        }

        private void BlurImageButton_Click(object sender, EventArgs e)
        {
            ImageOffset.Threads = _threads;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ImagePictureBox.Image = 
                ImageOffset.ApplyOffset((Bitmap)ImagePictureBox.Image, 300, 300);

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            ImageProcessingTimeTextBox.Text =
                ts.Seconds.ToString() + "." +
                ts.Milliseconds.ToString() + " sec";
        }

        private void NumberOfThreadsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _threads = int.Parse(NumberOfThreadsComboBox.SelectedItem.ToString());
        }

        private void OpenImageButton_Click(object sender, EventArgs e)
        {
            OpenImage();
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            SaveImage();
        }

        private void ClearPictureBoxButton_Click(object sender, EventArgs e)
        {
            ClearPictureBox();
        }

        private void OpenImage()
        {
            if (ImageOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImagePictureBox.Image = (Bitmap)Bitmap.FromFile(
                    ImageOpenFileDialog.FileName);
            }            
        }

        private void SaveImage() 
        {
            try
            {
                if (ImagePictureBox.Image != null)
                {
                    if (ImageSaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ImageFormat format = ImageFormat.Png;
                        var extension = System.IO.Path.GetExtension(
                            ImageSaveFileDialog.FileName).ToLower();                        

                        switch (extension)
                        {
                            case ".jpg":
                                format = ImageFormat.Jpeg;
                                break;
                            case ".bmp":
                                format = ImageFormat.Bmp;
                                break;
                        }

                        ImagePictureBox.Image.Save(
                            ImageSaveFileDialog.FileName, format);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "No image to save!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving image: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearPictureBox()
        {
            ImagePictureBox.Image.Dispose();
            ImagePictureBox.Image = null;    
            ImageProcessingTimeTextBox.Text = null;
        }        
    }
}
