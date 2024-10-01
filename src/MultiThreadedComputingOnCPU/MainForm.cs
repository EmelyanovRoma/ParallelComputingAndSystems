using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiThreadedComputingOnCPU
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            NumberOfThreadsComboBox.SelectedIndex = 0;
        }

        private void EroseImageButton_Click(object sender, EventArgs e)
        {

        }

        private void BlurImageButton_Click(object sender, EventArgs e)
        {

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
            ImagePictureBox.Image = null;
        }
    }
}
