using ImageProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.ColorShemes
{
    public class LabColorSheme : IColorSheme
    {
        public double[,,] Convert(double[,,] arrayImage) => DoubleArrayImageOperations.GetImageInLab(arrayImage);

        public double Difference(double[] colorA, double[] colorB) => LabColorConverter.deltaE(colorA, colorB);

        public double AssessmentsDifference(double[] colorA, double[] colorB)
        {
            double lDiff = Math.Pow((colorA[0] - colorB[0]) / 100d, 2);
            double aDiff = Math.Pow((colorA[1] - colorB[1]) / 255d, 2);
            double bDiff = Math.Pow((colorA[2] - colorB[2]) / 255d, 2);

            return Math.Sqrt(lDiff + aDiff + bDiff);
        }

    }
}
