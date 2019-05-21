using ImageProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph
{
    class SegmentedSetToBitmapConverter
    {
        public static Bitmap Convert(DisjointSet segmentedSet, int height, int width)
        {
            double[,,] im = new double[3, height, width];

            Dictionary<int, ColorCustom> colors = new Dictionary<int, ColorCustom>();
            int totalSize = 0;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    ColorCustom ccc;

                    int comp = segmentedSet.Find(h * width + w);

                    if (colors.TryGetValue(comp, out ccc) == false)
                    {
                        ccc = ColorCustom.GetRandomColor();
                        colors.Add(comp, ccc);

                        int compSize = segmentedSet.Size(comp);
                        totalSize += compSize;
                        System.Diagnostics.Debug.WriteLine("Component: " + comp + " | size: " + compSize);
                    }


                    im[0, h, w] = BitmapConverter.Limit(ccc.r);
                    im[1, h, w] = BitmapConverter.Limit(ccc.g);
                    im[2, h, w] = BitmapConverter.Limit(ccc.b);
                }
            }

            System.Diagnostics.Debug.WriteLine("Total Size: " + totalSize);
            System.Diagnostics.Debug.WriteLine("Height*Width: " + height * width);


            return BitmapConverter.DoubleRgbToBitmap(im);
        }
    }
}
