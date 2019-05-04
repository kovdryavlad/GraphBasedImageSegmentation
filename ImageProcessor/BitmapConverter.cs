using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class BitmapConverter
    {
        public unsafe static double[,,] BitmapToDoubleRgb(Bitmap bmp)
        {
            int width  = bmp.Width,
                height = bmp.Height;

            double[,,] res = new double[3, height, width];
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (double* _res = res)
                {
                    double* _r = _res, _g = _res + width * height, _b = _res + 2 * width * height;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *_b = *(curpos++); ++_b;
                            *_g = *(curpos++); ++_g;
                            *_r = *(curpos++); ++_r;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
            return res;
        }

        public unsafe static Bitmap DoubleRgbToBitmap(double[,,] arrayImage)
        {
            int width = arrayImage.GetLength(2),
                height = arrayImage.GetLength(1);

            Bitmap res = new Bitmap(width, height);
            BitmapData bd = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (double* _arrImage = arrayImage)
                {
                    double* _r = _arrImage, _g = _arrImage + width * height, _b = _arrImage + 2 * width * height;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *(curpos++) = Limit(*_b); ++_b;
                            *(curpos++) = Limit(*_g); ++_g;
                            *(curpos++) = Limit(*_r); ++_r;
                        }
                    }
                }
            }
            finally
            {
                res.UnlockBits(bd);
            }
            return res;
        }

        /*
        public unsafe static Bitmap SegmentedSetToBitmap(DisjointSet segmentedSet, int height, int width)
        {
            Color[] colors = Enumerable.Range(0, width*height)
                                          .Select(i => Color.GetRandomColor())
                                          .ToArray();


            Bitmap res = new Bitmap(width, height);
            BitmapData bd = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);


            try
            {
                byte* curpos;
                
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {

                            int component = segmentedSet.Find(h * width + w);
                            Color c = colors[component];

                            *(curpos++) = Limit(c.b); 
                            *(curpos++) = Limit(c.g); 
                            *(curpos++) = Limit(c.r); 
                        }
                    }
                
            }
            finally
            {
                res.UnlockBits(bd);
            }
            return res;
        }
        */

        public unsafe static Bitmap SegmentedSetToBitmap(DisjointSet segmentedSet, int height, int width)
        {
            double[,,] im = new double[3, height, width];

            Dictionary<int, Color> colors = new Dictionary<int, Color>();
            int totalSize = 0;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    Color ccc;

                    int comp = segmentedSet.Find(h * width + w);

                    if (colors.TryGetValue(comp, out ccc) == false)
                    {
                        ccc = Color.GetRandomColor();
                        colors.Add(comp, ccc);

                        int compSize = segmentedSet.Size(comp);
                        totalSize += compSize;
                        System.Diagnostics.Debug.WriteLine("Component: "+comp +" | size: " + compSize);
                    }
                        

                    im[0, h, w] = Limit(ccc.r);
                    im[1, h, w] = Limit(ccc.g);
                    im[2, h, w] = Limit(ccc.b);
                }
            }

            System.Diagnostics.Debug.WriteLine("Total Size: " + totalSize);
            System.Diagnostics.Debug.WriteLine("Height*Width: " + height*width);


            return DoubleRgbToBitmap(im);
        }

        private static byte Limit(double x)
        {
            if (x < 0)
                return 0;
            if (x > 255)
                return 255;
            return (byte)x;
        }
    }
}
