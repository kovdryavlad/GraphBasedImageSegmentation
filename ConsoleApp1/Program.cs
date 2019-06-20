using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Aspose.Cells;
using Aspose.Cells.Drawing;

namespace ConsoleApp1
{
    class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {
             //OpenFileDialog ofd = new OpenFileDialog();
             //
             //ofd.ShowDialog();
             //
             //if (ofd.ShowDialog() != DialogResult.OK)
             //    return;
             //
             //Bitmap b = new Bitmap(ofd.FileName);
             //Bitmap b = new Bitmap(@"C:\Users\Vlad\Desktop\Диплом\1.jpg");

            //CreatingExcelFile(b);

            CreatingExcelFile();

            Console.WriteLine("Done");
            //Console.ReadKey();
        }

        //private static void CreatingExcelFile(Bitmap b)
        private static void CreatingExcelFile()
        {
            int width = 1260;
            int height = 945;

            //MemoryStream memoryStream = new MemoryStream();
            //b.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            // Load input Excel file inside Aspose.Cells Workbook object.
           // Workbook wb = new Workbook("testImage3.xlsx");
            Workbook wb = new Workbook();

            // Access first worksheet.
            //Worksheet ws = wb.Worksheets[0];
            wb.Worksheets.Add("first");
            Worksheet ws = wb.Worksheets[0];

            // Access cell C12 by name.
            Cell cell = ws.Cells[1,1];
            
            // Add picture in Excel cell.
            int idx = ws.Pictures.Add(cell.Row, cell.Column, @"C:\Users\Vlad\Desktop\Диплом\1.jpg");
           
            // Access the picture by index.
            Picture pic = ws.Pictures[idx];
            pic.

            ws.Cells.SetColumnWidthPixel(1, 640);
            ws.Cells.SetRowHeightPixel(1, 400);

            // Save the workbook in output Excel file.
            wb.Save("D:\\diplomaExcelTests\\TEST.xlsx", SaveFormat.Xlsx);
        }
    }
}
