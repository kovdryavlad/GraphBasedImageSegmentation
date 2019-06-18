using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.ColorShemes
{
    public interface IColorSheme
    {
        double[,,] Convert(double[,,] arrayImage);
        double Difference(double[] colorA, double[] colorB);
        double AssessmentsDifference(double[] colorA, double[] colorB);
    }
}
