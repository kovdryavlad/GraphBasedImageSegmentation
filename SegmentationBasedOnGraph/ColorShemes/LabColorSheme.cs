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

        //public double AssessmentsDifference(double[] colorA, double[] colorB)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
