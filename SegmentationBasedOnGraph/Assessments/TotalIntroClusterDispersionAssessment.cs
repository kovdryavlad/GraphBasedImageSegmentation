using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SegmentationBasedOnGraph.ColorShemes;
using SegmentationBasedOnGraph.ResultSegments;
using SimpleMatrix;

namespace SegmentationBasedOnGraph.Assessments
{
    class TotalIntroClusterDispersionAssessment : AssessmentBase
    {
        internal override double GeAssessment(AssesmentsSegment[] segments, IColorSheme colorSheme)
        {
            int k = segments.Length;
            int n = segments[0].center.Length;
            Matrix sumMatrix = new Matrix(n);

            for (int i = 0; i < k; i++)
            {
                AssesmentsSegment current = segments[i];
                sumMatrix += current.Length * GetMatrixOfCovariations(current);
            }

            return sumMatrix.Determinant();
        }

        private Matrix GetMatrixOfCovariations(AssesmentsSegment segment)
        {
            int n = segment.center.Length;

            double[] averages = segment.center;

            double[][] cov = ArrayMatrix.GetJaggedArray(n, n);
            
            for (int k = 0; k < n; k++)
                for (int p = 0; p < n; p++)
                {
                    double v = 0;
                    for (int l = 0; l < segment.Length; l++)
                        v += ((segment.points[l][k] - averages[k])) * ((segment.points[l][p] - averages[p]));
            
            
                    cov[k][p] = v / segment.Length;
                }

            return new Matrix(cov);
        }
    }
}
