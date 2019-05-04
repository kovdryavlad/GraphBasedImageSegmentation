using ImageProcessor;
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

namespace SimpleProcessing
{
    public partial class Form1 : Form
    {
        double[,,] _image;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileWindow = new OpenFileDialog();
            if (openFileWindow.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileWindow.FileName);
                _image = BitmapConverter.BitmapToDoubleRgb(bmp);

                pictureBox1.Width = bmp.Width;
                pictureBox1.Height = bmp.Height;
                pictureBox1.Image = bmp;
            }            
        }

        //яркость
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            BrightnessTextBox.Text = value.ToString();

            double[,,] image = DoubleArrayImageOperations.ChangeBrightness(_image, value);
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double[,,] image = DoubleArrayImageOperations.Negative(_image);
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        private void BlackWhitetrackBar_Scroll(object sender, EventArgs e)
        {
            int value = BlackWhitetrackBar.Value;
            BlackWhitetextBox.Text = value.ToString();

            double[,,] image = DoubleArrayImageOperations.GetWhiteBlack(_image, value);
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        //оттенки серого
        private void button2_Click(object sender, EventArgs e)
        {
            double[,,] image = DoubleArrayImageOperations.GetGrayScale(_image);
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);

        }

        //сепия
        private void button3_Click_1(object sender, EventArgs e)
        {
            double[,,] image = DoubleArrayImageOperations.Sepia(_image);
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        //контраст
        private void ContrastTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = ContrastTrackBar.Value;
            ContrastTextBox.Text = value.ToString();

            double[,,] image = DoubleArrayImageOperations.contrast(_image, 1+value/100d);
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        private void вернутьсяКИсходномуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(_image);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double sigma = Convert.ToDouble(sigmaTextBox.Text.Replace(".", ","));
            int k = Convert.ToInt32(KtextBox.Text);
            int min = Convert.ToInt32(MinTextBox.Text);

            Segment segmentObj = new Segment();

            pictureBox1.Image = segmentObj.DoSegmentation(_image, sigma, k, min);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double sigma = Convert.ToDouble(sigmaTextBox.Text.Replace(".", ","));

            GaussianBlur gaussianBlur = new GaussianBlur();
            double[][] filter = gaussianBlur.getKernel(sigma);

            double[,,] image = DoubleArrayImageOperations.ConvolutionFilter(_image, filter);
            pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }
    }
}
