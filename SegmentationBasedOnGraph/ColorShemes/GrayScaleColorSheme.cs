using ImageProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.ColorShemes
{
    public class GrayScaleColorSheme : IColorSheme
    {
        public double[,,] Convert(double[,,] arrayImage) => DoubleArrayImageOperations.GetGrayScale(arrayImage);

        public double Difference(double[] colorA, double[] colorB)=>Math.Abs(colorA[0] - colorB[0]);

        //public double AssessmentsDifference(double[] colorA, double[] colorB)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
