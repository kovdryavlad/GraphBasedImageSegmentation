using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Aspose.Cells;
using Aspose.Cells.Drawing;
using ImageProcessor;

namespace ConsoleApp1
{
    class Program
    {
        static int kMin = 100;
        static int kMax = 1000;
        static int kStep = 100;
        
        static int minMin = 100;
        static int minMax = 1000;
        static int minStep = 100;

        static int RowsForDetails = 2;
        static int startImageRowNumber = 1;
        static int startImageColumnNumber = 1;

        static void Main(string[] args)
        {
            string filename = @"D:\diplomaExcelTests\testImage1.jpg";

            //int width = 1260;
            //int height = 945;

            int width = 640;
            int height = 400;

            Workbook wb = new Workbook();
            SetHeadersAndStyles(wb, width, height);

            OutputSegmentations(wb, filename, width, height);

            wb.Save("D:\\diplomaExcelTests\\TEST.xlsx", SaveFormat.Xlsx);
            //CreatingExcelFile(@"C:\Users\Vlad\Desktop\Диплом\1.jpg");

            //CreatingExcelFile();

            Console.WriteLine("Done");
            //Console.ReadKey();
        }

        private static void SetHeadersAndStyles(Workbook wb, int width, int height)
        {
            Worksheet ws = wb.Worksheets[0];
            ws.Cells[0, 0].Value = @"k\min";

            for (int k = kMin, i = 0; k <= kMax; k += kStep, i++)
            {
                int currentRow = i*(RowsForDetails + 1) + startImageRowNumber;

                ws.Cells[currentRow, 0].Value = $"k = {k}";
                ws.Cells.SetRowHeightPixel(currentRow, height);
            }

            for (int min = minMin, j = startImageColumnNumber; min <= minMax; min += minStep, j++)
            {
                ws.Cells[0, j].Value = $"min = {min}";
                ws.Cells.SetColumnWidthPixel(j, width);
            }
        }

        private static void OutputSegmentations(Workbook wb, string filename, int width, int height)
        {
            Worksheet ws = wb.Worksheets[0];

            //image reading
            Bitmap b = new Bitmap(filename);

            double[,,] arrayImage = BitmapConverter.BitmapToDoubleRgb(b);

            //MemoryStream memoryStream = new MemoryStream();
            //b.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            //processing cycle
            for (int k = kMin, i = 0; k <= kMax; k += kStep, i++)
            {
                int currentRow = i * (RowsForDetails + 1) + startImageRowNumber;

                for (int min = minMin, j = startImageColumnNumber; min <= minMax; min += minStep, j++)
                {
                    int idx = ws.Pictures.Add(currentRow, j, memoryStream);
                }
            }
        }


        //private static void CreatingExcelFile(Bitmap b)
        private static void CreatingExcelFileTEMP(string filename)
        {
            

            Bitmap b = new Bitmap(filename);

            MemoryStream memoryStream = new MemoryStream();
            b.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            // Load input Excel file inside Aspose.Cells Workbook object.
           // Workbook wb = new Workbook("testImage3.xlsx");
            Workbook wb = new Workbook();

            // Access first worksheet.
            //Worksheet ws = wb.Worksheets[0];
            Worksheet ws = wb.Worksheets[0];

            // Access cell C12 by name.
            Cell cell = ws.Cells[1,1];
            
            // Add picture in Excel cell.
            //int idx = ws.Pictures.Add(cell.Row, cell.Column, @"C:\Users\Vlad\Desktop\Диплом\1.jpg");
            int idx = ws.Pictures.Add(cell.Row, cell.Column, memoryStream);
           
            // Access the picture by index.
            Picture pic = ws.Pictures[idx];

            ws.Cells.SetColumnWidthPixel(1, 640);
            ws.Cells.SetRowHeightPixel(1, 400);

            // Save the workbook in output Excel file.
            wb.Save("D:\\diplomaExcelTests\\TEST.xlsx", SaveFormat.Xlsx);
        }
    }
}
