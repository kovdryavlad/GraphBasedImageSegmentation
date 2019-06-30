using Aspose.Cells;
using ImageProcessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SegmentationBasedOnGraph;
using System.IO;

namespace SegmentationTestToExcel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int kMin = 100;
        int kMax = 1000;
        int kStep = 100;
        
        int minMin = 100;
        int minMax = 1000;
        int minStep = 100;
        
        int RowsForDetails = 0;
        int startImageRowNumber = 1;
        int startImageColumnNumber = 1;

        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string folder = ofd.SelectedPath;
            string[] files = Directory.GetFiles(folder);

            Workbook wb = new Workbook();
            wb.Worksheets.Clear();

            for (int i = 0; i < files.Length; i++)
            {
                Bitmap bitmap = null;
                try
                {
                    bitmap = new Bitmap(files[i]);
                }
                catch
                {
                    continue;
                }
                Bitmap scalledBitmap = GetScaledBitmap(bitmap);
                double[,,] arrayImage = ImageProcessor.BitmapConverter.BitmapToDoubleRgb(scalledBitmap);

                string shortName = Path.GetFileNameWithoutExtension(files[i]);
                Worksheet ws = wb.Worksheets.Add(shortName);

                SetHeadersAndStyles(ws);

                Segmentation segmentationObj = new Segmentation();
                Edge[] edges = segmentationObj.SegmentationPart1(arrayImage, 0.84, new SegmentationBasedOnGraph.ColorShemes.RGBColorSheme());

                OutputSegmentations(segmentationObj, edges, ws, i);
            }

            wb.Save(Path.Combine(folder, Path.GetFileNameWithoutExtension(folder) + ".xlsx"), SaveFormat.Xlsx);

            System.Diagnostics.Debug.WriteLine("Done");
        }

        Bitmap GetScaledBitmap(Bitmap image, int widthMax = 2000, int heightMax = 2000)
        {
            double Wreal = image.Width;
            double Hreal = image.Height;

            if (Wreal > widthMax || heightMax > 2000)
            {

                double Wmax = widthMax;
                double Hmax = heightMax;

                double l = Hreal / Wreal;

                int scaledWidth = (int)Wmax;
                int scaledHeight = (int)Hmax;

                if (Wreal / Wmax > Hreal / Hmax)
                    scaledHeight = (int)(Wmax * l);

                else
                    scaledWidth = (int)(Hmax / l);

                return Service.ResizeImage(image, scaledWidth, scaledHeight);
            }

            return image;
        }

        private void SetHeadersAndStyles(Worksheet ws)
        {
            ws.Cells[0, 0].Value = @"k\min";

            for (int k = kMin, i = 0; k <= kMax; k += kStep, i++)
            {
                int currentRow = i * (RowsForDetails + 1) + startImageRowNumber;

                ws.Cells[currentRow, 0].Value = $"k = {k}";
            }

            for (int min = minMin, j = startImageColumnNumber; min <= minMax; min += minStep, j++)
            {
                ws.Cells[0, j].Value = $"min = {min}";
            }
        }

        private void OutputSegmentations(Segmentation segmentationObj, Edge[] edges, Worksheet ws, int fileNumber)
        {
           
            //processing cycle
            int counter = 0;
            for (int k = kMin, i = 0; k <= kMax; k += kStep, i++)
            {
                int currentRow = i * (RowsForDetails + 1) + startImageRowNumber;

                for (int min = minMin, j = startImageColumnNumber; min <= minMax; min += minStep, j++)
                {
                    //получение результатов сегментации
                    int components = segmentationObj.SegmentationPart2(edges,  k, min);

                    ws.Cells[currentRow, j].Value = components;

                    counter++;
                    System.Diagnostics.Debug.WriteLine($"Файл: {fileNumber} - Сегментация: {counter} - {DateTime.Now}");
                }
            }
        }

        /*
        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            int width = 640;
            int height = 400;

            Workbook wb = new Workbook();
            SetHeadersAndStyles(wb, width, height);

            OutputSegmentations(wb, ofd.FileName, width, height);

            wb.Save("D:\\diplomaExcelTests\\RGBTestFighting.xlsx", SaveFormat.Xlsx);

            Console.WriteLine("Done");
        }

        

        private void OutputSegmentations(Workbook wb, string filename, int width, int height)
        {
            Worksheet ws = wb.Worksheets[0];

            //image reading
            Bitmap b = new Bitmap(filename);

            double[,,] arrayImage = BitmapConverter.BitmapToDoubleRgb(b);

            //MemoryStream memoryStream = new MemoryStream();
            //b.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            //processing cycle
            int counter = 0;
            for (int k = kMin, i = 0; k <= kMax; k += kStep, i++)
            {
                int currentRow = i * (RowsForDetails + 1) + startImageRowNumber;

                for (int min = minMin, j = startImageColumnNumber; min <= minMax; min += minStep, j++)
                {
                    //получение результатов сегментации
                    Segmentation segmentationObj = new Segmentation();
                    Bitmap res = segmentationObj.DoSegmentation(b, 0.84, k, min, new SegmentationBasedOnGraph.ColorShemes.RGBColorSheme());

                    //приведение результатов к нужному размеру
                    res = Service.ResizeImage(res, width, height);

                    ///сохранения результата в поток
                    MemoryStream memoryStream = new MemoryStream();
                    res.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    //добавления сегментации на страницу 
                    ws.Cells[currentRow + 1, j].Value = segmentationObj.m_componentLength.ToString(); 

                    int idx = ws.Pictures.Add(currentRow, j, memoryStream);

                    ws.Cells[currentRow + 1, j].Value = $"Сегментів: {segmentationObj.m_componentLength}"; 
                    ws.Cells[currentRow + 2, j].Value = $"Сума внутрішньокластерних дисперсій: {segmentationObj.GetInternalDifferenceAssessment().ToString("0.00")}"; 

                    counter++;
                    System.Diagnostics.Debug.WriteLine($"Сегментация {counter} посчитана");
                }
            }
        }
        */
    }
}
