using ImageProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double> timeList = new List<double>();

            for (int i = 0; i < 100; i++)
            {
                DateTime t1 = DateTime.Now;
                double[,,] arrayImage = BitmapConverter.BitmapToDoubleRgb(new Bitmap("testReading.png"));

                DateTime t2 = DateTime.Now;
                double time = (t2 - t1).TotalSeconds;

                System.Diagnostics.Debug.WriteLine("reading"+i + ": " + time.ToString("0.0000"));

                timeList.Add(time);
            }

            STAT stat = new STAT();
            stat.SetData(timeList.ToArray());


            System.Diagnostics.Debug.WriteLine("Expectation: " + stat.Expectation_niz.ToString("0.0000") + " | " + stat.Expectation.ToString("0.0000") + " | " + stat.Expectation_verh.ToString("0.0000"));
            System.Diagnostics.Debug.WriteLine("Sigma: " + stat.Sigma_niz.ToString("0.0000") + " | " + stat.Sigma.ToString("0.0000") + " | " + stat.Sigma_verh.ToString("0.0000"));
        }
    }
}
