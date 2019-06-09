using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.ColorShemes
{
    public class RGBColorSheme : IColorSheme
    {
        public double[,,] Convert(double[,,] arrayImage) => arrayImage;

        public double Difference(double[] colorA, double[] colorB)
        {
            double rDiff = Math.Pow(colorA[0] - colorB[0], 2);
            double gDiff = Math.Pow(colorA[1] - colorB[1], 2);
            double bDiff = Math.Pow(colorA[2] - colorB[2], 2);

            return Math.Sqrt(rDiff + gDiff + bDiff);
        }
    }
}
