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
        int kMax = 1200;
        int kStep = 100;

        int minMin = 100;
        int minMax = 1200;
        int minStep = 100;

        int RowsForDetails = 2;
        int startImageRowNumber = 1;
        int startImageColumnNumber = 1;

        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            int width = 545;
            int height = 409;

            //int width = 640;
            //int height = 400;

            Workbook wb = new Workbook();
            SetHeadersAndStyles(wb, width, height);

            OutputSegmentations(wb, ofd.FileName, width, height);

            wb.Save("D:\\diplomaExcelTests\\TEST_Lab12_12.xlsx", SaveFormat.Xlsx);
            
            Console.WriteLine("Done");
        }

        private void SetHeadersAndStyles(Workbook wb, int width, int height)
        {
            Worksheet ws = wb.Worksheets[0];
            ws.Cells[0, 0].Value = @"k\min";

            for (int k = kMin, i = 0; k <= kMax; k += kStep, i++)
            {
                int currentRow = i * (RowsForDetails + 1) + startImageRowNumber;

                ws.Cells[currentRow, 0].Value = $"k = {k}";
                ws.Cells.SetRowHeightPixel(currentRow, height);
            }

            for (int min = minMin, j = startImageColumnNumber; min <= minMax; min += minStep, j++)
            {
                ws.Cells[0, j].Value = $"min = {min}";
                ws.Cells.SetColumnWidthPixel(j, width);
            }
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
                    Bitmap res = segmentationObj.DoSegmentation(b, 0.84, k, min, new SegmentationBasedOnGraph.ColorShemes.LabColorSheme());

                    //приведение результатов к нужному размеру
                    res = Service.ResizeImage(res, width, height);

                    ///сохранения результата в поток
                    MemoryStream memoryStream = new MemoryStream();
                    res.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    //добавления сегментации на страницу 
                    int idx = ws.Pictures.Add(currentRow, j, memoryStream);

                    ws.Cells[currentRow + 1, j].Value = $"Сегментів: {segmentationObj.m_componentLength}"; 
                    ws.Cells[currentRow + 2, j].Value = $"Сума внутрішньокластерних дисперсій: {segmentationObj.GetInternalDifferenceAssessment().ToString("0.00")}"; 

                    counter++;
                    System.Diagnostics.Debug.WriteLine($"Сегментация {counter} посчитана");
                }
            }
        }
    }
}
