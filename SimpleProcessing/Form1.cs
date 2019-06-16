using ImageProcessor;
using SegmentationBasedOnGraph;
using SegmentationBasedOnGraph.ColorShemes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleProcessing
{
    public partial class Form1 : Form
    {
        double[,,] m_originalImage;
        double[,,] m_workImage;

        string m_imageName;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetScrollsBarsValues();

            OpenFileDialog openFileWindow = new OpenFileDialog();
            if (openFileWindow.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileWindow.FileName);
                m_workImage = BitmapConverter.BitmapToDoubleRgb(bmp);
                m_originalImage = (double[,,])m_workImage.Clone();


                m_imageName = Path.GetFileNameWithoutExtension(openFileWindow.FileName);

                //pictureBox1.Width = bmp.Width;
                //pictureBox1.Height = bmp.Height;
                //pictureBox1.Image = bmp;

                OutputBitmapOnPictureBox(bmp);
            }            
        }

        //яркость
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            BrightnessTextBox.Text = value.ToString();

            m_workImage = DoubleArrayImageOperations.ChangeBrightness(m_workImage, value);
            OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //double[,,] image = DoubleArrayImageOperations.Negative(m_originalImage);
            //pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        private void BlackWhitetrackBar_Scroll(object sender, EventArgs e)
        {
            //int value = BlackWhitetrackBar.Value;
            //BlackWhitetextBox.Text = value.ToString();
            //
            //double[,,] image = DoubleArrayImageOperations.GetWhiteBlack(m_originalImage, value);
            //pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        //оттенки серого
        private void button2_Click(object sender, EventArgs e)
        {
            m_workImage = DoubleArrayImageOperations.GetGrayScale(m_workImage);
            OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));

        }

        //сепия
        private void button3_Click_1(object sender, EventArgs e)
        {
            //double[,,] image = DoubleArrayImageOperations.Sepia(m_originalImage);
            //pictureBox1.Image = BitmapConverter.DoubleRgbToBitmap(image);
        }

        //контраст
        private void ContrastTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = ContrastTrackBar.Value;
            ContrastTextBox.Text = value.ToString();

            m_workImage = DoubleArrayImageOperations.contrast(m_workImage, 1+value/100d);
            OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
        }

        private void вернутьсяКИсходномуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetScrollsBarsValues();
            m_workImage = (double[,,])m_originalImage.Clone();
            OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
        }

        Bitmap segmented;

        private void button4_Click(object sender, EventArgs e)
        {
            double sigma = Convert.ToDouble(sigmaTextBox.Text.Replace(".", ","));
            int k = Convert.ToInt32(KtextBox.Text);
            int min = Convert.ToInt32(MinTextBox.Text);

            IColorSheme colorSheme = null;
            if (GreyScaleRadioButton.Checked)
                colorSheme = new GrayScaleColorSheme();
            if (RgbRadioButton.Checked)
                colorSheme = new RGBColorSheme();
            if (LabRadioButton.Checked)
                colorSheme = new LabColorSheme();

            Segmentation segmentObj = new Segmentation();

            segmented = segmentObj.DoSegmentation(m_workImage, sigma, k, min, colorSheme);
            OutputBitmapOnPictureBox(segmented);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double sigma = Convert.ToDouble(sigmaTextBox.Text.Replace(".", ","));

            GaussianBlur gaussianBlur = new GaussianBlur();
            double[][] filter = gaussianBlur.getKernel(sigma);

            m_workImage = DoubleArrayImageOperations.ConvolutionFilter(m_workImage, filter);
            OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                FileName = $"{m_imageName}_segmented(sigma = {sigmaTextBox.Text}) - k = {KtextBox.Text} - min = {MinTextBox.Text}).jpg",
                
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                segmented.Save(sfd.FileName);
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"width: {pictureBox1.Width} - height: {pictureBox1.Height}");
        }

        void OutputBitmapOnPictureBox(Bitmap image)
        {
            double Wreal = image.Width;
            double Hreal = image.Height;

            if (Wreal > pictureBox1.Width || Hreal > pictureBox1.Height)
            {

                double Wmax = pictureBox1.Width;
                double Hmax = pictureBox1.Height;

                double l = Hreal / Wreal;

                int scaledWidth = (int)Wmax;
                int scaledHeight = (int)Hmax;

                if (Wreal / Wmax > Hreal / Hmax)
                    scaledHeight = (int)(Wmax * l);

                else
                    scaledWidth = (int)(Hmax / l);

                pictureBox1.Image = Service.ResizeImage(image, scaledWidth, scaledHeight);
            }

            else
            {
                pictureBox1.Image = image;
            }
        }

        void ResetScrollsBarsValues()
        {
            trackBar1.Scroll -= trackBar1_Scroll;
            trackBar1.Value = 0;
            BrightnessTextBox.Text = "";
            trackBar1.Scroll += trackBar1_Scroll;

            ContrastTrackBar.Scroll -= ContrastTrackBar_Scroll;
            ContrastTrackBar.Value = 0;
            ContrastTextBox.Text = "";
            ContrastTrackBar.Scroll += ContrastTrackBar_Scroll;
        }

        private void прийнятиЯскравістьТаКонтрастністьЗа0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetScrollsBarsValues();
        }

        Image remebmered;
        private void перемикачАБэToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (remebmered == null)
            {
                remebmered = pictureBox1.Image;
                OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
            }

            else
            {
                pictureBox1.Image = remebmered;
                remebmered = null;
            }
        }
    }
}
